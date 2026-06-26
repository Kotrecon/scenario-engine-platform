using ScenarioDesigner.Extensions;
using ScenarioDesigner.Extensions.HealthChecks;
using ScenarioDesigner.Extensions.RateLimiting;
using ScenarioDesigner.Security;
using ScenarioDesigner.Validation.Configuration;
using Serilog;
using Serilog.Extensions.Logging;

// ============================================================================
// ТОЧКА ВХОДА. Fail-fast: если что-то пошло не так при старте — логируем и
// завершаемся с кодом 1, без исключений в консоль.
// ============================================================================
try
{
    var builder = WebApplication.CreateBuilder(args);

    // ------------------------------------------------------------------------
    // 1. ЛОГИРОВАНИЕ (Serilog) — ПЕРВЫМ
    // Настраивается до валидации, чтобы ILogger работал через Serilog.
    // LoggingLevelSwitch регистрируется в DI для динамического изменения
    // уровней через API.
    // ------------------------------------------------------------------------
    builder.AddCustomLogging();

    // ------------------------------------------------------------------------
    // 2. ВАЛИДАЦИЯ ОБЯЗАТЕЛЬНЫХ НАСТРОЕК
    // Проверяем до регистрации сервисов — чтобы упасть с понятным сообщением,
    // а не получить NullReferenceException в runtime.
    // ILogger работает через Serilog (настроен выше).
    // ------------------------------------------------------------------------
    var logger = new SerilogLoggerFactory(Log.Logger).CreateLogger<Program>();
    if (!ConfigurationValidator.ValidateRequiredConfiguration(builder.Configuration, logger))
    {
        Environment.Exit(1);
    }

    // ------------------------------------------------------------------------
    // 3. ОПЦИИ (IOptions<T> с валидацией DataAnnotations и ValidateOnStart)
    // ------------------------------------------------------------------------
    builder.AddAppSettings();

    // ------------------------------------------------------------------------
    // 4. ТЕЛЕМЕТРИЯ (OpenTelemetry: logs + traces + metrics → OTLP)
    // Pipeline строится один раз при старте. Endpoint/Protocol/Sinks
    // поменять на лету нельзя — только через GitOps + restart.
    // ------------------------------------------------------------------------
    builder.AddCustomOpenTelemetry();

    // ------------------------------------------------------------------------
    // 5. БЕЗОПАСНОСТЬ (JWT аутентификация + policy-based авторизация)
    // Роли: Admin (изменение), Operator (чтение), Auditor (только чтение).
    // ------------------------------------------------------------------------
    builder.AddCustomAuthentication();
    builder.AddCustomAuthorization();

    // ------------------------------------------------------------------------
    // 5.1 HEALTH CHECKS
    // Регистрация health checks в DI.
    // Liveness: всегда Healthy (игнорирует shutdown).
    // Readiness: Unhealthy при shutdown (через IHostApplicationLifetime).
    // ------------------------------------------------------------------------
    builder.Services.AddCustomHealthChecks();

    // ------------------------------------------------------------------------
    // 5.2 RATE LIMITING
    // Ограничение количества запросов к health-эндпоинтам.
    // ------------------------------------------------------------------------
    builder.Services.AddCustomRateLimiting();

    // ------------------------------------------------------------------------
    // 6. ЗАПУСК ХОСТА
    // ------------------------------------------------------------------------
    var app = builder.Build();

    app.MapControllers();

    // ------------------------------------------------------------------------
    // 6.1 RATE LIMITING — в основном pipeline
    // ------------------------------------------------------------------------
    app.UseRateLimiter();

    // ------------------------------------------------------------------------
    // 6.2 HEALTH CHECKS — привязка к порту 8081
    // ------------------------------------------------------------------------
    app.UseCustomHealthChecks();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    Environment.Exit(1);
}
finally
{
    Log.CloseAndFlush();
}