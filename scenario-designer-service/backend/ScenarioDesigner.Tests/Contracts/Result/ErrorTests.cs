using ScenarioDesigner.Contracts.Result.Common;

namespace ScenarioDesigner.Tests.Contracts.Result;

public class ErrorTests
{
    [Test]
    public async Task BusinessRuleError_PropertiesAreCorrect()
    {
        var error = new BusinessRuleError("Cannot delete active user");

        await Assert.That(error.Message).IsEqualTo("Cannot delete active user");
        await Assert.That(error.Code).IsEqualTo("BusinessRuleViolation");
        await Assert.That(error.StatusCode).IsEqualTo(400);
    }

    [Test]
    public async Task ConflictError_PropertiesAreCorrect()
    {
        var error = new ConflictError("Email already exists");

        await Assert.That(error.Message).IsEqualTo("Email already exists");
        await Assert.That(error.Code).IsEqualTo("Conflict");
        await Assert.That(error.StatusCode).IsEqualTo(409);
    }

    [Test]
    public async Task ForbiddenError_PropertiesAreCorrect()
    {
        var error = new ForbiddenError();

        await Assert.That(error.Message).IsEqualTo("Доступ запрещён.");
        await Assert.That(error.Code).IsEqualTo("Forbidden");
        await Assert.That(error.StatusCode).IsEqualTo(403);
    }

    [Test]
    public async Task NotFoundError_PropertiesAreCorrect()
    {
        var error = new NotFoundError("User", 42);

        await Assert.That(error.EntityName).IsEqualTo("User");
        await Assert.That(error.Code).IsEqualTo("NotFound");
        await Assert.That(error.StatusCode).IsEqualTo(404);
        await Assert.That(error.Message).Contains("User");
        await Assert.That(error.Message).Contains("42");
    }

    [Test]
    public async Task ValidationError_PropertiesAreCorrect()
    {
        var error = new ValidationError("Invalid data", new[] { "email:required" });

        await Assert.That(error.Message).IsEqualTo("Invalid data");
        await Assert.That(error.Code).IsEqualTo("ValidationFailed");
        await Assert.That(error.StatusCode).IsEqualTo(422);
    }

    [Test]
    public async Task ValidationError_ParsesSingleFieldError()
    {
        var error = new ValidationError("Invalid", new[] { "email:required" });

        await Assert.That(error.Details.ContainsKey("email")).IsTrue();
        await Assert.That(error.Details["email"]).Contains("required");
    }

    [Test]
    public async Task ValidationError_ParsesMultipleErrorsForSameField()
    {
        var error = new ValidationError("Invalid", new[] { "email:required", "email:invalid" });

        await Assert.That(error.Details["email"].Length).IsEqualTo(2);
        await Assert.That(error.Details["email"]).Contains("required");
        await Assert.That(error.Details["email"]).Contains("invalid");
    }

    [Test]
    public async Task ValidationError_ParsesMultipleFields()
    {
        var error = new ValidationError("Invalid", new[] { "email:required", "password:minLength" });

        await Assert.That(error.Details.ContainsKey("email")).IsTrue();
        await Assert.That(error.Details.ContainsKey("password")).IsTrue();
    }

    [Test]
    public async Task ValidationError_UsesInvalidWhenCodeMissing()
    {
        var error = new ValidationError("Invalid", new[] { "email" });

        await Assert.That(error.Details["email"]).Contains("invalid");
    }

    [Test]
    public async Task ValidationError_HandlesNullOrEmptyInput()
    {
        var error = new ValidationError("Invalid", null!);

        await Assert.That(error.Details.Count).IsEqualTo(0);
    }
}
