using ScenarioDesigner.Contracts.Dto.Request.Logging;

namespace ScenarioDesigner.Tests.Contracts.Dto;

public class SetLogLevelRequestTests
{
    [Test]
    public async Task Validate_WhenRequestIsNull_ReturnsError()
    {
        var result = SetLogLevelValidator.Validate(null);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.ErrorMessage).IsEqualTo("Request is required.");
    }

    [Test]
    public async Task Validate_WhenLevelIsNull_ReturnsError()
    {
        var request = new SetLogLevelRequest(Category: "Microsoft", Level: null!);

        var result = SetLogLevelValidator.Validate(request);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.ErrorMessage).IsEqualTo("Level is required.");
        await Assert.That(result!.MemberNames).Contains("Level");
    }

    [Test]
    public async Task Validate_WhenLevelIsEmpty_ReturnsError()
    {
        var request = new SetLogLevelRequest(Category: "Microsoft", Level: "");

        var result = SetLogLevelValidator.Validate(request);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.ErrorMessage).IsEqualTo("Level is required.");
        await Assert.That(result!.MemberNames).Contains("Level");
    }

    [Test]
    public async Task Validate_WhenLevelIsInvalid_ReturnsError()
    {
        var request = new SetLogLevelRequest(Category: "Microsoft", Level: "Critical");

        var result = SetLogLevelValidator.Validate(request);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.ErrorMessage).Contains("Invalid level");
        await Assert.That(result!.MemberNames).Contains("Level");
    }

    [Test]
    public async Task Validate_WhenLevelIsValid_ReturnsSuccess()
    {
        var request = new SetLogLevelRequest(Category: "Microsoft", Level: "Warning");

        var result = SetLogLevelValidator.Validate(request);

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task Validate_WhenCategoryIsNull_ReturnsSuccess()
    {
        var request = new SetLogLevelRequest(Category: null, Level: "Information");

        var result = SetLogLevelValidator.Validate(request);

        await Assert.That(result).IsNull();
    }

    [Test]
    public async Task Validate_WhenCategoryIsEmpty_ReturnsError()
    {
        var request = new SetLogLevelRequest(Category: "", Level: "Information");

        var result = SetLogLevelValidator.Validate(request);

        await Assert.That(result).IsNotNull();
        await Assert.That(result!.ErrorMessage).IsEqualTo("Category cannot be empty string.");
        await Assert.That(result!.MemberNames).Contains("Category");
    }

    [Test]
    public async Task Validate_WhenCategoryIsValid_ReturnsSuccess()
    {
        var request = new SetLogLevelRequest(Category: "Microsoft.AspNetCore", Level: "Warning");

        var result = SetLogLevelValidator.Validate(request);

        await Assert.That(result).IsNull();
    }
}
