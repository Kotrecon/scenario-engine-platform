using Microsoft.AspNetCore.Mvc;
using ScenarioDesigner.Contracts.Result.Common;
using ScenarioDesigner.Contracts.Result.Web;

namespace ScenarioDesigner.Tests.Contracts.Result;

public class ResultExtensionsTests
{
    [Test]
    public async Task ToActionResult_GenericSuccess_ReturnsOkObjectResult()
    {
        var result = ScenarioDesigner.Contracts.Result.Common.Result<string>.Success("hello");

        var actionResult = result.ToActionResult();

        var okResult = actionResult as OkObjectResult;
        await Assert.That(okResult).IsNotNull();
        await Assert.That(okResult!.Value).IsEqualTo("hello");
    }

    [Test]
    public async Task ToActionResult_NonGenericSuccess_ReturnsOkResult()
    {
        var result = ScenarioDesigner.Contracts.Result.Common.Result.Success();

        var actionResult = result.ToActionResult();

        var okResult = actionResult as OkResult;
        await Assert.That(okResult).IsNotNull();
    }

    [Test]
    public async Task ToActionResult_GenericFailure_ReturnsProblemDetails()
    {
        var result = ScenarioDesigner.Contracts.Result.Common.Result<string>.Failure(new NotFoundError("User", 1));

        var actionResult = result.ToActionResult();

        var objectResult = actionResult as ObjectResult;
        await Assert.That(objectResult).IsNotNull();
        await Assert.That(objectResult!.StatusCode).IsEqualTo(404);

        var problem = objectResult.Value as ProblemDetails;
        await Assert.That(problem).IsNotNull();
    }

    [Test]
    public async Task ToActionResult_NonGenericFailure_ReturnsProblemDetails()
    {
        var result = ScenarioDesigner.Contracts.Result.Common.Result.Failure(new BusinessRuleError("err"));

        var actionResult = result.ToActionResult();

        var objectResult = actionResult as ObjectResult;
        await Assert.That(objectResult).IsNotNull();
        await Assert.That(objectResult!.StatusCode).IsEqualTo(400);
    }

    [Test]
    public async Task ProblemDetails_StatusTitleDetail_ComeFromFirstError()
    {
        var errors = new IError[]
        {
            new ValidationError("Validation failed", new[] { "email:required" }),
            new BusinessRuleError("rule violated")
        };
        var result = ScenarioDesigner.Contracts.Result.Common.Result.Failure(errors);

        var actionResult = result.ToActionResult();

        var objectResult = actionResult as ObjectResult;
        var problem = objectResult!.Value as ProblemDetails;
        await Assert.That(problem!.Status).IsEqualTo(422);
        await Assert.That(problem.Title).IsEqualTo("ValidationFailed");
        await Assert.That(problem.Detail).IsEqualTo("Validation failed");
    }

    [Test]
    public async Task ProblemDetails_ExtensionsContainAllErrors()
    {
        var errors = new IError[]
        {
            new BusinessRuleError("first"),
            new ConflictError("second")
        };
        var result = ScenarioDesigner.Contracts.Result.Common.Result.Failure(errors);

        var actionResult = result.ToActionResult();

        var objectResult = actionResult as ObjectResult;
        var problem = objectResult!.Value as ProblemDetails;
        await Assert.That(problem!.Extensions.ContainsKey("errors")).IsTrue();
        var json = System.Text.Json.JsonSerializer.Serialize(problem.Extensions["errors"]);
        await Assert.That(json).Contains("first");
        await Assert.That(json).Contains("second");
    }

    [Test]
    public async Task ProblemDetails_ExtensionsContainValidationErrors_ForValidationError()
    {
        var result = ScenarioDesigner.Contracts.Result.Common.Result.Failure(new ValidationError("Invalid", new[] { "email:required" }));

        var actionResult = result.ToActionResult();

        var objectResult = actionResult as ObjectResult;
        var problem = objectResult!.Value as ProblemDetails;
        await Assert.That(problem!.Extensions.ContainsKey("validationErrors")).IsTrue();
    }

    [Test]
    public async Task ProblemDetails_DoesNotContainValidationErrors_ForNonValidationError()
    {
        var result = ScenarioDesigner.Contracts.Result.Common.Result.Failure(new NotFoundError("User", 1));

        var actionResult = result.ToActionResult();

        var objectResult = actionResult as ObjectResult;
        var problem = objectResult!.Value as ProblemDetails;
        await Assert.That(problem!.Extensions.ContainsKey("validationErrors")).IsFalse();
    }

    [Test]
    public async Task ProblemDetails_HasTypeTitleStatusDetail()
    {
        var result = ScenarioDesigner.Contracts.Result.Common.Result.Failure(new BusinessRuleError("err"));

        var actionResult = result.ToActionResult();

        var objectResult = actionResult as ObjectResult;
        var problem = objectResult!.Value as ProblemDetails;
        await Assert.That(problem!.Status).IsNotNull();
        await Assert.That(problem.Title).IsNotNull();
        await Assert.That(problem.Detail).IsNotNull();
    }

    [Test]
    public async Task ProblemDetails_ErrorsContainCodeAndMessage()
    {
        var result = ScenarioDesigner.Contracts.Result.Common.Result.Failure(new BusinessRuleError("rule violated"));

        var actionResult = result.ToActionResult();

        var objectResult = actionResult as ObjectResult;
        var problem = objectResult!.Value as ProblemDetails;
        await Assert.That(problem!.Extensions.ContainsKey("errors")).IsTrue();
        var json = System.Text.Json.JsonSerializer.Serialize(problem.Extensions["errors"]);
        await Assert.That(json).Contains("rule violated");
    }

    [Test]
    public async Task ProblemDetails_ValidationErrorsAreStructuredByField()
    {
        var result = ScenarioDesigner.Contracts.Result.Common.Result.Failure(new ValidationError("Invalid", new[] { "email:required", "password:minLength" }));

        var actionResult = result.ToActionResult();

        var objectResult = actionResult as ObjectResult;
        var problem = objectResult!.Value as ProblemDetails;
        var validationErrors = problem!.Extensions["validationErrors"] as Dictionary<string, string[]>;
        await Assert.That(validationErrors).IsNotNull();
        await Assert.That(validationErrors!.ContainsKey("email")).IsTrue();
        await Assert.That(validationErrors.ContainsKey("password")).IsTrue();
    }

    [Test]
    public async Task ProblemDetails_DoesNotExposeInternalData()
    {
        var result = ScenarioDesigner.Contracts.Result.Common.Result.Failure(new BusinessRuleError("err"));

        var actionResult = result.ToActionResult();

        var objectResult = actionResult as ObjectResult;
        var problem = objectResult!.Value as ProblemDetails;
        var json = System.Text.Json.JsonSerializer.Serialize(problem);
        await Assert.That(json).DoesNotContain("StackTrace");
        await Assert.That(json).DoesNotContain("InnerException");
    }
}
