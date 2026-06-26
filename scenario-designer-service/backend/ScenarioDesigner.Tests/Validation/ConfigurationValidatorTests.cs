using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using ScenarioDesigner.Validation.Configuration;

namespace ScenarioDesigner.Tests.Validation;

public class ConfigurationValidatorTests
{
    private readonly Mock<ILogger> _loggerMock;

    public ConfigurationValidatorTests()
    {
        _loggerMock = new Mock<ILogger>();
    }

    [Test]
    public async Task ValidateRequiredConfiguration_WhenValid_ReturnsSuccess()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppSettings:ServiceName"] = "TestService",
                ["AppSettings:Port"] = "8080",
                ["OpenTelemetry:Endpoint"] = "http://localhost:4317",
                ["Jwt:Key"] = "ThisIsAVeryLongKeyForTesting123456"
            })
            .Build();

        var result = ConfigurationValidator.ValidateRequiredConfiguration(config, _loggerMock.Object);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Errors).IsEmpty();
    }

    [Test]
    public async Task ValidateRequiredConfiguration_WhenMissingServiceName_ReturnsFailure()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppSettings:ServiceName"] = "",
                ["AppSettings:Port"] = "8080",
                ["OpenTelemetry:Endpoint"] = "http://localhost:4317",
                ["Jwt:Key"] = "ThisIsAVeryLongKeyForTesting123456"
            })
            .Build();

        var result = ConfigurationValidator.ValidateRequiredConfiguration(config, _loggerMock.Object);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Errors[0].Message).Contains("ServiceName");
    }

    [Test]
    public async Task ValidateRequiredConfiguration_WhenMissingPort_ReturnsFailure()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppSettings:ServiceName"] = "TestService",
                ["AppSettings:Port"] = "",
                ["OpenTelemetry:Endpoint"] = "http://localhost:4317",
                ["Jwt:Key"] = "ThisIsAVeryLongKeyForTesting123456"
            })
            .Build();

        var result = ConfigurationValidator.ValidateRequiredConfiguration(config, _loggerMock.Object);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Errors[0].Message).Contains("Port");
    }

    [Test]
    public async Task ValidateRequiredConfiguration_WhenPortIsZero_ReturnsFailure()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppSettings:ServiceName"] = "TestService",
                ["AppSettings:Port"] = "0",
                ["OpenTelemetry:Endpoint"] = "http://localhost:4317",
                ["Jwt:Key"] = "ThisIsAVeryLongKeyForTesting123456"
            })
            .Build();

        var result = ConfigurationValidator.ValidateRequiredConfiguration(config, _loggerMock.Object);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Errors[0].Message).Contains("Port");
    }

    [Test]
    public async Task ValidateRequiredConfiguration_WhenPortIsMax_ReturnsSuccess()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppSettings:ServiceName"] = "TestService",
                ["AppSettings:Port"] = "65535",
                ["OpenTelemetry:Endpoint"] = "http://localhost:4317",
                ["Jwt:Key"] = "ThisIsAVeryLongKeyForTesting123456"
            })
            .Build();

        var result = ConfigurationValidator.ValidateRequiredConfiguration(config, _loggerMock.Object);

        await Assert.That(result.IsSuccess).IsTrue();
    }

    [Test]
    public async Task ValidateRequiredConfiguration_WhenPortOutOfRange_ReturnsFailure()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppSettings:ServiceName"] = "TestService",
                ["AppSettings:Port"] = "99999",
                ["OpenTelemetry:Endpoint"] = "http://localhost:4317",
                ["Jwt:Key"] = "ThisIsAVeryLongKeyForTesting123456"
            })
            .Build();

        var result = ConfigurationValidator.ValidateRequiredConfiguration(config, _loggerMock.Object);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Errors[0].Message).Contains("Port");
    }

    [Test]
    public async Task ValidateRequiredConfiguration_WhenPortNegative_ReturnsFailure()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppSettings:ServiceName"] = "TestService",
                ["AppSettings:Port"] = "-1",
                ["OpenTelemetry:Endpoint"] = "http://localhost:4317",
                ["Jwt:Key"] = "ThisIsAVeryLongKeyForTesting123456"
            })
            .Build();

        var result = ConfigurationValidator.ValidateRequiredConfiguration(config, _loggerMock.Object);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Errors[0].Message).Contains("Port");
    }

    [Test]
    public async Task ValidateRequiredConfiguration_WhenPortNotNumber_ReturnsFailure()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppSettings:ServiceName"] = "TestService",
                ["AppSettings:Port"] = "abc",
                ["OpenTelemetry:Endpoint"] = "http://localhost:4317",
                ["Jwt:Key"] = "ThisIsAVeryLongKeyForTesting123456"
            })
            .Build();

        var result = ConfigurationValidator.ValidateRequiredConfiguration(config, _loggerMock.Object);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Errors[0].Message).Contains("Port");
    }

    [Test]
    public async Task ValidateRequiredConfiguration_WhenMissingEndpoint_ReturnsFailure()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppSettings:ServiceName"] = "TestService",
                ["AppSettings:Port"] = "8080",
                ["OpenTelemetry:Endpoint"] = "",
                ["Jwt:Key"] = "ThisIsAVeryLongKeyForTesting123456"
            })
            .Build();

        var result = ConfigurationValidator.ValidateRequiredConfiguration(config, _loggerMock.Object);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Errors[0].Message).Contains("Endpoint");
    }

    [Test]
    public async Task ValidateRequiredConfiguration_WhenMissingJwtKey_ReturnsFailure()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppSettings:ServiceName"] = "TestService",
                ["AppSettings:Port"] = "8080",
                ["OpenTelemetry:Endpoint"] = "http://localhost:4317",
                ["Jwt:Key"] = ""
            })
            .Build();

        var result = ConfigurationValidator.ValidateRequiredConfiguration(config, _loggerMock.Object);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Errors[0].Message).Contains("Jwt:Key");
    }

    [Test]
    public async Task ValidateRequiredConfiguration_WhenJwtKeyExactly32Chars_ReturnsSuccess()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppSettings:ServiceName"] = "TestService",
                ["AppSettings:Port"] = "8080",
                ["OpenTelemetry:Endpoint"] = "http://localhost:4317",
                ["Jwt:Key"] = "12345678901234567890123456789012"
            })
            .Build();

        var result = ConfigurationValidator.ValidateRequiredConfiguration(config, _loggerMock.Object);

        await Assert.That(result.IsSuccess).IsTrue();
    }

    [Test]
    public async Task ValidateRequiredConfiguration_WhenJwtKey31Chars_ReturnsFailure()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppSettings:ServiceName"] = "TestService",
                ["AppSettings:Port"] = "8080",
                ["OpenTelemetry:Endpoint"] = "http://localhost:4317",
                ["Jwt:Key"] = "1234567890123456789012345678901"
            })
            .Build();

        var result = ConfigurationValidator.ValidateRequiredConfiguration(config, _loggerMock.Object);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Errors[0].Message).Contains("Jwt:Key");
    }

    [Test]
    public async Task ValidateRequiredConfiguration_WhenJwtKeyTooShort_ReturnsFailure()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppSettings:ServiceName"] = "TestService",
                ["AppSettings:Port"] = "8080",
                ["OpenTelemetry:Endpoint"] = "http://localhost:4317",
                ["Jwt:Key"] = "short"
            })
            .Build();

        var result = ConfigurationValidator.ValidateRequiredConfiguration(config, _loggerMock.Object);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Errors[0].Message).Contains("Jwt:Key");
    }

    [Test]
    public async Task ValidateRequiredConfiguration_WhenAllMissing_ReturnsFailureWithMultipleErrors()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppSettings:ServiceName"] = null,
                ["AppSettings:Port"] = null,
                ["OpenTelemetry:Endpoint"] = null,
                ["Jwt:Key"] = null
            })
            .Build();

        var result = ConfigurationValidator.ValidateRequiredConfiguration(config, _loggerMock.Object);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Errors.Count).IsEqualTo(4);
    }

    [Test]
    public async Task ValidateRequiredConfiguration_WhenMissingServiceName_LogsError()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppSettings:ServiceName"] = "",
                ["AppSettings:Port"] = "8080",
                ["OpenTelemetry:Endpoint"] = "http://localhost:4317",
                ["Jwt:Key"] = "ThisIsAVeryLongKeyForTesting123456"
            })
            .Build();

        var loggerMock = new Mock<ILogger>();

        ConfigurationValidator.ValidateRequiredConfiguration(config, loggerMock.Object);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    o != null &&
                    o.ToString()!.Contains("ServiceName")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
