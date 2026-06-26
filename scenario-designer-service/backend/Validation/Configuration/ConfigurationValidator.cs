using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ScenarioDesigner.Validation.Configuration;

// ============================================================================
// ВАЛИДАЦИЯ ОБЯЗАТЕЛЬНОЙ КОНФИГУРАЦИИ
// Собирает все ошибки в список, чтобы оператор увидел сразу всё, что нужно
// исправить, а не исправлять по одной.
// ============================================================================
public static class ConfigurationValidator
{
    public static bool ValidateRequiredConfiguration(IConfiguration configuration, ILogger logger)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(configuration["AppSettings:ServiceName"]))
            errors.Add("AppSettings:ServiceName is required");

        if (!int.TryParse(configuration["AppSettings:Port"], out var port) || port < 1 || port > 65535)
            errors.Add("AppSettings:Port must be between 1 and 65535");

        if (string.IsNullOrWhiteSpace(configuration["OpenTelemetry:Endpoint"]))
            errors.Add("OpenTelemetry:Endpoint is required");

        if (string.IsNullOrWhiteSpace(configuration["Jwt:Key"]) || configuration["Jwt:Key"]!.Length < 32)
            errors.Add("Jwt:Key is required and must be at least 32 characters");

        if (errors.Count > 0)
        {
            foreach (var error in errors)
            {
                logger.LogError("Configuration validation failed: {Error}", error);
            }

            return false;
        }

        return true;
    }
}