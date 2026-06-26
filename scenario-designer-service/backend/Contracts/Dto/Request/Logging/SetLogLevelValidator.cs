using ScenarioDesigner.Contracts.Result.Common;

namespace ScenarioDesigner.Contracts.Dto.Request.Logging;

public static class SetLogLevelValidator
{
    private static readonly HashSet<string> ValidLevels = new(StringComparer.OrdinalIgnoreCase)
    {
        "Debug", "Information", "Warning", "Error", "Fatal"
    };

    public static ScenarioDesigner.Contracts.Result.Common.Result Validate(SetLogLevelRequest? request)
    {
        if (request is null)
            return ScenarioDesigner.Contracts.Result.Common.Result.Failure(new ValidationError("Request is required.", new[] { "request:required" }));

        if (string.IsNullOrWhiteSpace(request.Level))
            return ScenarioDesigner.Contracts.Result.Common.Result.Failure(new ValidationError("Level is required.", new[] { "level:required" }));

        if (!ValidLevels.Contains(request.Level))
        {
            return ScenarioDesigner.Contracts.Result.Common.Result.Failure(new ValidationError(
                $"Invalid level: {request.Level}. Valid levels: {string.Join(", ", ValidLevels)}",
                new[] { "level:invalid" }));
        }

        if (request.Category is not null && string.IsNullOrWhiteSpace(request.Category))
            return ScenarioDesigner.Contracts.Result.Common.Result.Failure(new ValidationError("Category cannot be empty string.", new[] { "category:empty" }));

        return ScenarioDesigner.Contracts.Result.Common.Result.Success();
    }
}
