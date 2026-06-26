using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ScenarioDesigner.Configuration.Options;
using Serilog;
using Serilog.Events;

namespace ScenarioDesigner.Extensions;

// ============================================================================
// EXTENSION-МЕТОДЫ ДЛЯ РЕГИСТРАЦИИ СЕРВИСОВ
// Каждый метод инкапсулирует настройку одного аспекта (options, logging, telemetry).
// Вызываются из Program.cs в строгом порядке.
// ============================================================================
public static class ServiceExtensions
{
    // ------------------------------------------------------------------------
    // 3. ОПЦИИ (IOptions<T>)
    // Bind + ValidateDataAnnotations + ValidateOnStart:
    // - Читает секцию из IConfiguration
    // - Валидирует атрибуты [Required], [Range]
    // - ValidateOnStart бросает исключение при старте, если валидация не пройдена
    // ------------------------------------------------------------------------
    public static IHostApplicationBuilder AddAppSettings(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<AppSettings>()
            .Bind(builder.Configuration.GetSection("AppSettings"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return builder;
    }

    // ------------------------------------------------------------------------
    // 4. ЛОГИРОВАНИЕ (Serilog)
    // Sinks (Console/File) настраиваются через стандартный формат Serilog
    // в appsettings.json и читаются Serilog.Settings.Configuration.
    // Sinks пересоздать на лету нельзя — только через GitOps + restart.
    // Уровни логирования (root + overrides) вынесены в LoggingLevelSwitch,
    // зарегистрированный в DI, и могут меняться на лету через API.
    // ------------------------------------------------------------------------
    public static WebApplicationBuilder AddCustomLogging(this WebApplicationBuilder builder)
    {
        // --------------------------------------------------------------------
        // LoggingLevelSwitch для динамического изменения уровней через API.
        // Значения берутся из IConfiguration (Serilog:MinimumLevel).
        // Регистрируем как singleton для доступа из LoggingController.
        // --------------------------------------------------------------------
        var rootLevelSwitch = new Serilog.Core.LoggingLevelSwitch(
            Enum.TryParse<LogEventLevel>(
                builder.Configuration["Serilog:MinimumLevel:Default"],
                true,
                out var lvl) ? lvl : LogEventLevel.Information);

        var overrideSwitches = new Dictionary<string, Serilog.Core.LoggingLevelSwitch>();

        var overrideSection = builder.Configuration.GetSection("Serilog:MinimumLevel:Override");
        foreach (var kvp in overrideSection.GetChildren())
        {
            if (Enum.TryParse<LogEventLevel>(kvp.Value, true, out var olvl))
                overrideSwitches[kvp.Key] = new Serilog.Core.LoggingLevelSwitch(olvl);
        }

        builder.Services.AddSingleton(rootLevelSwitch);
        builder.Services.AddSingleton<IDictionary<string, Serilog.Core.LoggingLevelSwitch>>(overrideSwitches);

        // --------------------------------------------------------------------
        // Serilog читает sinks из стандартного формата appsettings.json
        // через Serilog.Settings.Configuration.
        // Enrichers добавляют контекст к каждому лог-событию для корреляции
        // в агрегаторе (ELK, Loki, Splunk).
        // --------------------------------------------------------------------
        builder.Host.UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ServiceName", context.Configuration["AppSettings:ServiceName"])
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .MinimumLevel.ControlledBy(rootLevelSwitch);

            // Overrides применяются после ReadFrom.Configuration, чтобы
            // переопределить уровни для конкретных категорий.
            foreach (var kvp in overrideSwitches)
                loggerConfiguration.MinimumLevel.Override(kvp.Key, kvp.Value);
        });

        return builder;
    }

    // ------------------------------------------------------------------------
    // 5. ТЕЛЕМЕТРИЯ (OpenTelemetry)
    // Pipeline строится один раз при старте. Endpoint/Protocol/Sinks
    // поменять на лету нельзя — только через GitOps + restart.
    // Фильтры по категориям применяются через builder.Logging.AddFilter()
    // до AddOpenTelemetry(), чтобы влиять на все провайдеры.
    // ------------------------------------------------------------------------
    public static IHostApplicationBuilder AddCustomOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<OpenTelemetryOptions>()
            .Bind(builder.Configuration.GetSection("OpenTelemetry"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>()!;
        var otelOptions = builder.Configuration.GetSection("OpenTelemetry").Get<OpenTelemetryOptions>()!;

        // --------------------------------------------------------------------
        // ResourceBuilder — метаданные сервиса для OTel.
        // service.name — обязательный атрибут для корреляции traces/metrics/logs.
        // --------------------------------------------------------------------
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(appSettings.ServiceName);

        // --------------------------------------------------------------------
        // Общий делегат для настройки OTLP-экспортера.
        // Избегаем дублирования кода для logs/traces/metrics.
        // --------------------------------------------------------------------
        Action<OtlpExporterOptions> configureOtlp = otlp =>
        {
            otlp.Endpoint = new Uri(otelOptions.Endpoint);
            otlp.Protocol = otelOptions.Protocol;
            if (otelOptions.Headers.Count > 0)
                otlp.Headers = string.Join(",", otelOptions.Headers.Select(x => $"{x.Key}={x.Value}"));
        };

        // --------------------------------------------------------------------
        // Фильтрация для всех провайдеров (включая OTel).
        // Применяется до AddOpenTelemetry(), чтобы влиять на весь pipeline.
        // --------------------------------------------------------------------
        foreach (var kvp in otelOptions.LogLevel)
        {
            if (Enum.TryParse<LogLevel>(kvp.Value, true, out var level))
                builder.Logging.AddFilter(kvp.Key, level);
        }

        // --------------------------------------------------------------------
        // OTel Logs — экспорт логов в коллектор.
        // Работает параллельно с Serilog: Serilog пишет в консоль/файл,
        // OTel пишет в коллектор. Это разные destinations, не дублирование.
        // --------------------------------------------------------------------
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.SetResourceBuilder(resourceBuilder);
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
            logging.ParseStateValues = true;

            logging.AddOtlpExporter(configureOtlp);
            if (otelOptions.UseConsoleExporter) logging.AddConsoleExporter();
        });

        // --------------------------------------------------------------------
        // OTel Traces + Metrics
        // AddSource — регистрирует ActivitySource для трассировки.
        // AddHttpClientInstrumentation — автоматически трассирует HTTP-вызовы.
        // AddRuntimeInstrumentation — метрики CLR (GC, threads, memory).
        // --------------------------------------------------------------------
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(appSettings.ServiceName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(appSettings.ServiceName)
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(configureOtlp);

                if (otelOptions.UseConsoleExporter) tracing.AddConsoleExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(configureOtlp);

                if (otelOptions.UseConsoleExporter) metrics.AddConsoleExporter();
            });

        return builder;
    }
}