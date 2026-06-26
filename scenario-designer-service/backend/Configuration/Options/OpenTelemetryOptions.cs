using System.ComponentModel.DataAnnotations;
using OpenTelemetry.Exporter;

namespace ScenarioDesigner.Configuration.Options;

// ============================================================================
// НАСТРОЙКИ ТЕЛЕМЕТРИИ (OpenTelemetry)
// Pipeline строится один раз при старте. Endpoint/Protocol/Sinks поменять
// на лету нельзя — только через GitOps + restart.
// ============================================================================
public sealed class OpenTelemetryOptions
{
    // ------------------------------------------------------------------------
    // Endpoint OTLP-коллектора (OpenTelemetry Collector, Jaeger, Tempo, etc.).
    // Формат: http://host:port для gRPC, http://host:port/v1/traces для HTTP.
    // ------------------------------------------------------------------------
    [Required]
    public string Endpoint { get; set; } = null!;

    // ------------------------------------------------------------------------
    // Протокол экспорта: gRPC (рекомендуется) или HTTP/Protobuf.
    // gRPC — эффективнее, но требует поддержки коллектором.
    // ------------------------------------------------------------------------
    public OtlpExportProtocol Protocol { get; set; } = OtlpExportProtocol.Grpc;

    // ------------------------------------------------------------------------
    // Заголовки для аутентификации в коллекторе (если требуется).
    // Формат: {"Authorization": "Bearer token", "X-Custom": "value"}.
    // ------------------------------------------------------------------------
    public Dictionary<string, string> Headers { get; set; } = new();

    // ------------------------------------------------------------------------
    // Console exporter для отладки. Выводит traces/metrics/logs в консоль.
    // Управляется только через appsettings.json (не включается автоматически).
    // В production: false (засоряет консоль метриками каждые 10 секунд).
    // ------------------------------------------------------------------------
    public bool UseConsoleExporter { get; set; } = false;

    // ------------------------------------------------------------------------
    // Фильтры по категориям для OTel-логов.
    // Применяются через builder.Logging.AddFilter() в AddCustomOpenTelemetry().
    // Отдельно от Serilog — чтобы не дублировать логи в коллектор.
    // ------------------------------------------------------------------------
    public Dictionary<string, string> LogLevel { get; set; } = new()
    {
        ["Default"] = "Information"
    };
}