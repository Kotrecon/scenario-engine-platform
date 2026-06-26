using System.ComponentModel.DataAnnotations;
using OpenTelemetry.Exporter;
using ScenarioDesigner.Configuration.Options;

namespace ScenarioDesigner.Tests.Configuration.Options;

public class OpenTelemetryOptionsTests
{
    private static bool Validate(OpenTelemetryOptions options, out List<ValidationResult> results)
    {
        results = new List<ValidationResult>();
        return Validator.TryValidateObject(options, new ValidationContext(options), results, true);
    }

    [Test]
    public async Task OpenTelemetryOptions_WhenEndpointIsNull_ValidationFails()
    {
        var options = new OpenTelemetryOptions { Endpoint = null! };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsFalse();
        await Assert.That(results.Any(r => r.MemberNames.Contains("Endpoint"))).IsTrue();
    }

    [Test]
    public async Task OpenTelemetryOptions_WhenEndpointIsEmpty_ValidationFails()
    {
        var options = new OpenTelemetryOptions { Endpoint = "" };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsFalse();
        await Assert.That(results.Any(r => r.MemberNames.Contains("Endpoint"))).IsTrue();
    }

    [Test]
    public async Task OpenTelemetryOptions_WhenEndpointIsValid_ValidationPasses()
    {
        var options = new OpenTelemetryOptions { Endpoint = "http://localhost:4317" };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsTrue();
        await Assert.That(results).IsEmpty();
    }

    [Test]
    public async Task OpenTelemetryOptions_WhenEndpointIsHttps_ValidationPasses()
    {
        var options = new OpenTelemetryOptions { Endpoint = "https://otel-collector:4317" };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsTrue();
    }

    [Test]
    public async Task OpenTelemetryOptions_WhenProtocolIsGrpc_ValidationPasses()
    {
        var options = new OpenTelemetryOptions
        {
            Endpoint = "http://localhost:4317",
            Protocol = OtlpExportProtocol.Grpc
        };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsTrue();
    }

    [Test]
    public async Task OpenTelemetryOptions_WhenProtocolIsHttpProtobuf_ValidationPasses()
    {
        var options = new OpenTelemetryOptions
        {
            Endpoint = "http://localhost:4317",
            Protocol = OtlpExportProtocol.HttpProtobuf
        };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsTrue();
    }

    [Test]
    public async Task OpenTelemetryOptions_WhenUseConsoleExporterIsFalse_ValidationPasses()
    {
        var options = new OpenTelemetryOptions
        {
            Endpoint = "http://localhost:4317",
            UseConsoleExporter = false
        };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsTrue();
    }

    [Test]
    public async Task OpenTelemetryOptions_WhenUseConsoleExporterIsTrue_ValidationPasses()
    {
        var options = new OpenTelemetryOptions
        {
            Endpoint = "http://localhost:4317",
            UseConsoleExporter = true
        };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsTrue();
    }

    [Test]
    public async Task OpenTelemetryOptions_WhenHeadersIsEmpty_ValidationPasses()
    {
        var options = new OpenTelemetryOptions
        {
            Endpoint = "http://localhost:4317",
            Headers = new Dictionary<string, string>()
        };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsTrue();
    }

    [Test]
    public async Task OpenTelemetryOptions_WhenHeadersHasValues_ValidationPasses()
    {
        var options = new OpenTelemetryOptions
        {
            Endpoint = "http://localhost:4317",
            Headers = new Dictionary<string, string>
            {
                ["Authorization"] = "Bearer token123",
                ["X-Custom"] = "value"
            }
        };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsTrue();
    }

    [Test]
    public async Task OpenTelemetryOptions_WhenLogLevelIsEmpty_ValidationPasses()
    {
        var options = new OpenTelemetryOptions
        {
            Endpoint = "http://localhost:4317",
            LogLevel = new Dictionary<string, string>()
        };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsTrue();
    }

    [Test]
    public async Task OpenTelemetryOptions_WhenLogLevelHasValues_ValidationPasses()
    {
        var options = new OpenTelemetryOptions
        {
            Endpoint = "http://localhost:4317",
            LogLevel = new Dictionary<string, string>
            {
                ["Default"] = "Information",
                ["Microsoft"] = "Warning"
            }
        };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsTrue();
    }

    [Test]
    public async Task OpenTelemetryOptions_DefaultValues_AreCorrect()
    {
        var options = new OpenTelemetryOptions();

        await Assert.That(options.Protocol).IsEqualTo(OtlpExportProtocol.Grpc);
        await Assert.That(options.UseConsoleExporter).IsFalse();
        await Assert.That(options.Headers).IsEmpty();
        await Assert.That(options.LogLevel).ContainsKey("Default");
        await Assert.That(options.LogLevel["Default"]).IsEqualTo("Information");
    }
}
