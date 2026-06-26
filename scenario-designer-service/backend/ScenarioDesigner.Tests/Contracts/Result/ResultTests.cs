using ScenarioDesigner.Contracts.Result.Common;

namespace ScenarioDesigner.Tests.Contracts.Result;

public class ResultTests
{
    [Test]
    public async Task Success_ReturnsIsSuccessTrue_AndNoErrors()
    {
        var result = ScenarioDesigner.Contracts.Result.Common.Result.Success();

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Errors).IsEmpty();
    }

    [Test]
    public async Task Failure_WithSingleError_ReturnsIsSuccessFalse_AndOneError()
    {
        var error = new BusinessRuleError("rule violated");

        var result = ScenarioDesigner.Contracts.Result.Common.Result.Failure(error);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Errors.Count).IsEqualTo(1);
        await Assert.That(result.Errors[0].Message).IsEqualTo("rule violated");
    }

    [Test]
    public async Task Failure_WithMultipleErrors_ReturnsIsSuccessFalse_AndAllErrors()
    {
        var errors = new IError[]
        {
            new BusinessRuleError("first"),
            new ConflictError("second")
        };

        var result = ScenarioDesigner.Contracts.Result.Common.Result.Failure(errors);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Errors.Count).IsEqualTo(2);
    }

    [Test]
    public async Task Failure_WithoutErrors_Throws()
    {
        var act = () =>
        {
            ScenarioDesigner.Contracts.Result.Common.Result.Failure(Array.Empty<IError>());
            return Task.CompletedTask;
        };

        await Assert.That(act).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task OnSuccess_WhenSuccess_CallsAction()
    {
        var called = false;
        var result = ScenarioDesigner.Contracts.Result.Common.Result.Success();

        result.OnSuccess(() => called = true);

        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task OnSuccess_WhenFailure_DoesNotCallAction()
    {
        var called = false;
        var result = ScenarioDesigner.Contracts.Result.Common.Result.Failure(new BusinessRuleError("err"));

        result.OnSuccess(() => called = true);

        await Assert.That(called).IsFalse();
    }

    [Test]
    public async Task OnFailure_WhenFailure_CallsActionWithErrors()
    {
        IReadOnlyList<IError>? receivedErrors = null;
        var error = new BusinessRuleError("err");
        var result = ScenarioDesigner.Contracts.Result.Common.Result.Failure(error);

        result.OnFailure(e => receivedErrors = e);

        await Assert.That(receivedErrors).IsNotNull();
        await Assert.That(receivedErrors!.Count).IsEqualTo(1);
    }

    [Test]
    public async Task OnFailure_WhenSuccess_DoesNotCallAction()
    {
        var called = false;
        var result = ScenarioDesigner.Contracts.Result.Common.Result.Success();

        result.OnFailure(_ => called = true);

        await Assert.That(called).IsFalse();
    }

    [Test]
    public async Task ToResult_WhenSuccess_ReturnsSuccessResultOfT()
    {
        var result = ScenarioDesigner.Contracts.Result.Common.Result.Success();

        var typed = result.ToResult(42);

        await Assert.That(typed.IsSuccess).IsTrue();
        await Assert.That(typed.Value).IsEqualTo(42);
    }

    [Test]
    public async Task ToResult_WhenFailure_ReturnsFailureResultOfT()
    {
        var error = new BusinessRuleError("err");
        var result = ScenarioDesigner.Contracts.Result.Common.Result.Failure(error);

        var typed = result.ToResult(42);

        await Assert.That(typed.IsSuccess).IsFalse();
        await Assert.That(typed.Errors.Count).IsEqualTo(1);
    }
}
