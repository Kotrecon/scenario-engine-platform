using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ScenarioDesigner.Contracts.Result.Common;

namespace ScenarioDesigner.Validation.Configuration;

// ============================================================================
// ВАЛИДАЦИЯ ОБЯЗАТЕЛЬНЫХ НАСТРОЕК
// Собирает все ошибки в Result, чтобы оператор увидел сразу всё, что нужно
// исправить, а не исправлять по одной.
// ============================================================================
public static class ConfigurationValidator
{
    public static ScenarioDesigner.Contracts.Result.Common.Result ValidateRequiredConfiguration(IConfiguration configuration, ILogger logger)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(configuration["AppSettings:ServiceName"]))
            errors.Add(new BusinessRuleError("AppSettings:ServiceName is required"));

        if (!int.TryParse(configuration["AppSettings:Port"], out var port) || port < 1 || port > 65535)
            errors.Add(new BusinessRuleError("AppSettings:Port must be between 1 and 65535"));

        if (string.IsNullOrWhiteSpace(configuration["OpenTelemetry:Endpoint"]))
            errors.Add(new BusinessRuleError("OpenTelemetry:Endpoint is required"));

        if (string.IsNullOrWhiteSpace(configuration["Jwt:Key"]) || configuration["Jwt:Key"]!.Length < 32)
            errors.Add(new BusinessRuleError("Jwt:Key is required and must be at least 32 characters"));

        if (errors.Count > 0)
        {
            foreach (var error in errors)
            {
                logger.LogError("Configuration validation failed: {Error}", error.Message);
            }

            return ScenarioDesigner.Contracts.Result.Common.Result.Failure(errors);
        }

        return ScenarioDesigner.Contracts.Result.Common.Result.Success();
    }
}
