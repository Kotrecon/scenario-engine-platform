using System.ComponentModel.DataAnnotations;

namespace ScenarioDesigner.Contracts.Dto.Request.Logging;

public static class SetLogLevelValidator
{
    private static readonly HashSet<string> ValidLevels = new(StringComparer.OrdinalIgnoreCase)
    {
        "Debug", "Information", "Warning", "Error", "Fatal"
    };

    public static ValidationResult? Validate(SetLogLevelRequest? request)
    {
        if (request is null)
            return new ValidationResult("Request is required.");

        if (string.IsNullOrWhiteSpace(request.Level))
            return new ValidationResult("Level is required.", new[] { nameof(request.Level) });

        if (!ValidLevels.Contains(request.Level))
        {
            return new ValidationResult(
                $"Invalid level: {request.Level}. Valid levels: {string.Join(", ", ValidLevels)}",
                new[] { nameof(request.Level) });
        }

        if (request.Category is not null && string.IsNullOrWhiteSpace(request.Category))
            return new ValidationResult("Category cannot be empty string.", new[] { nameof(request.Category) });

        return ValidationResult.Success;
    }
}
