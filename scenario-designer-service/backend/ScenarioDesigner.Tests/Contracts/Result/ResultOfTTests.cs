using ScenarioDesigner.Contracts.Result.Common;

namespace ScenarioDesigner.Tests.Contracts.Result;

public class ResultOfTTests
{
    [Test]
    public async Task Success_ReturnsIsSuccessTrue_ValueAndNoErrors()
    {
        var result = ScenarioDesigner.Contracts.Result.Common.Result<string>.Success("hello");

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value).IsEqualTo("hello");
        await Assert.That(result.Errors).IsEmpty();
    }

    [Test]
    public async Task Failure_WithSingleError_ReturnsIsSuccessFalse_DefaultValue_AndOneError()
    {
        var error = new NotFoundError("User", 1);

        var result = ScenarioDesigner.Contracts.Result.Common.Result<int>.Failure(error);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Value).IsEqualTo(0);
        await Assert.That(result.Errors.Count).IsEqualTo(1);
    }

    [Test]
    public async Task Failure_WithMultipleErrors_ReturnsIsSuccessFalse_DefaultValue_AndAllErrors()
    {
        var errors = new IError[]
        {
            new BusinessRuleError("first"),
            new ConflictError("second")
        };

        var result = ScenarioDesigner.Contracts.Result.Common.Result<string>.Failure(errors);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Value).IsNull();
        await Assert.That(result.Errors.Count).IsEqualTo(2);
    }

    [Test]
    public async Task Failure_WithoutErrors_Throws()
    {
        var act = () =>
        {
            ScenarioDesigner.Contracts.Result.Common.Result<int>.Failure(Array.Empty<IError>());
            return Task.CompletedTask;
        };

        await Assert.That(act).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task OnSuccess_WhenSuccess_CallsActionWithValue()
    {
        string? received = null;
        var result = ScenarioDesigner.Contracts.Result.Common.Result<string>.Success("data");

        result.OnSuccess(v => received = v);

        await Assert.That(received).IsEqualTo("data");
    }

    [Test]
    public async Task OnSuccess_WhenFailure_DoesNotCallAction()
    {
        var called = false;
        var result = ScenarioDesigner.Contracts.Result.Common.Result<string>.Failure(new BusinessRuleError("err"));

        result.OnSuccess(_ => called = true);

        await Assert.That(called).IsFalse();
    }

    [Test]
    public async Task OnFailure_WhenFailure_CallsActionWithErrors()
    {
        IReadOnlyList<IError>? receivedErrors = null;
        var result = ScenarioDesigner.Contracts.Result.Common.Result<int>.Failure(new BusinessRuleError("err"));

        result.OnFailure(e => receivedErrors = e);

        await Assert.That(receivedErrors).IsNotNull();
        await Assert.That(receivedErrors!.Count).IsEqualTo(1);
    }

    [Test]
    public async Task OnFailure_WhenSuccess_DoesNotCallAction()
    {
        var called = false;
        var result = ScenarioDesigner.Contracts.Result.Common.Result<int>.Success(1);

        result.OnFailure(_ => called = true);

        await Assert.That(called).IsFalse();
    }

    [Test]
    public async Task Map_WhenSuccess_TransformsValue()
    {
        var result = ScenarioDesigner.Contracts.Result.Common.Result<int>.Success(5);

        var mapped = result.Map(x => x * 2);

        await Assert.That(mapped.IsSuccess).IsTrue();
        await Assert.That(mapped.Value).IsEqualTo(10);
    }

    [Test]
    public async Task Map_WhenFailure_PropagatesErrors()
    {
        var error = new BusinessRuleError("err");
        var result = ScenarioDesigner.Contracts.Result.Common.Result<int>.Failure(error);

        var mapped = result.Map(x => x * 2);

        await Assert.That(mapped.IsSuccess).IsFalse();
        await Assert.That(mapped.Errors[0].Message).IsEqualTo("err");
    }

    [Test]
    public async Task Bind_WhenSuccess_InvokesNext()
    {
        var result = ScenarioDesigner.Contracts.Result.Common.Result<int>.Success(5);

        var bound = result.Bind(x => ScenarioDesigner.Contracts.Result.Common.Result<string>.Success($"value={x}"));

        await Assert.That(bound.IsSuccess).IsTrue();
        await Assert.That(bound.Value).IsEqualTo("value=5");
    }

    [Test]
    public async Task Bind_WhenFailure_PropagatesErrors()
    {
        var error = new BusinessRuleError("err");
        var result = ScenarioDesigner.Contracts.Result.Common.Result<int>.Failure(error);

        var bound = result.Bind(x => ScenarioDesigner.Contracts.Result.Common.Result<string>.Success($"value={x}"));

        await Assert.That(bound.IsSuccess).IsFalse();
        await Assert.That(bound.Errors[0].Message).IsEqualTo("err");
    }

    [Test]
    public async Task ToResult_WhenSuccess_ReturnsNonGenericSuccess()
    {
        var result = ScenarioDesigner.Contracts.Result.Common.Result<int>.Success(42);

        var nonGeneric = result.ToResult();

        await Assert.That(nonGeneric.IsSuccess).IsTrue();
    }

    [Test]
    public async Task ToResult_WhenFailure_ReturnsNonGenericFailure()
    {
        var error = new BusinessRuleError("err");
        var result = ScenarioDesigner.Contracts.Result.Common.Result<int>.Failure(error);

        var nonGeneric = result.ToResult();

        await Assert.That(nonGeneric.IsSuccess).IsFalse();
        await Assert.That(nonGeneric.Errors.Count).IsEqualTo(1);
    }
}
