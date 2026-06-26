using Microsoft.AspNetCore.Mvc;
using ScenarioDesigner.Contracts.Result.Common;

namespace ScenarioDesigner.Contracts.Result.Web;

/// <summary>
/// Расширения для интеграции Result с ASP.NET Core.
/// Конвертирует Result в стандартные IActionResult (ProblemDetails, RFC 7807).
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Конвертирует обобщённый Result&lt;T&gt; в IActionResult.
    /// Успех → 200 OK с данными. Ошибка → ProblemDetails (RFC 7807).
    /// </summary>
    public static IActionResult ToActionResult<T>(this Common.Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        return BuildProblemDetails(result.Errors);
    }

    /// <summary>
    /// Конвертирует необобщённый Result (void) в IActionResult.
    /// Успех → 200 OK. Ошибка → ProblemDetails (RFC 7807).
    /// </summary>
    public static IActionResult ToActionResult(this Common.Result result)
    {
        if (result.IsSuccess)
            return new OkResult();

        return BuildProblemDetails(result.Errors);
    }

    private static IActionResult BuildProblemDetails(IReadOnlyList<IError> errors)
    {
        var firstError = errors[0];

        var problem = new ProblemDetails
        {
            Status = firstError.StatusCode,
            Title = firstError.Code,
            Detail = firstError.Message,
        };

        problem.Extensions["errors"] = errors.Select(e => new
        {
            code = e.Code,
            message = e.Message
        }).ToList();

        var validationErrors = errors.OfType<ValidationError>().FirstOrDefault();
        if (validationErrors is not null)
        {
            problem.Extensions["validationErrors"] = validationErrors.Details;
        }

        return new ObjectResult(problem) { StatusCode = firstError.StatusCode };
    }

}
