using System.ComponentModel.DataAnnotations;

namespace ScenarioDesigner.Configuration.Options;

// ============================================================================
// БАЗОВЫЕ НАСТРОЙКИ СЕРВИСА
// Минимальный набор параметров, без которых сервис не может существовать.
// Валидируются при старте через ValidateDataAnnotations() + ValidateOnStart()
// в AddAppSettings(), а также через ValidateRequiredConfiguration() в Program.cs.
// ============================================================================
public sealed class AppSettings
{
    // ------------------------------------------------------------------------
    // Имя сервиса для:
    // - OpenTelemetry Resource (service.name)
    // - Логов (enricher WithProperty)
    // - ActivitySource для трассировки
    // ------------------------------------------------------------------------
    [Required]
    public string ServiceName { get; set; } = null!;

    // ------------------------------------------------------------------------
    // Порт Kestrel. В production обычно управляется через environment variable
    // или reverse proxy (nginx, YARP), но здесь для локального запуска.
    // ------------------------------------------------------------------------
    [Range(1, 65535)]
    public int Port { get; set; }
}