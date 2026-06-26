using System.ComponentModel.DataAnnotations;
using ScenarioDesigner.Configuration.Options;

namespace ScenarioDesigner.Tests.Configuration.Options;

public class AppSettingsTests
{
    private static bool Validate(AppSettings options, out List<ValidationResult> results)
    {
        results = new List<ValidationResult>();
        return Validator.TryValidateObject(options, new ValidationContext(options), results, true);
    }

    [Test]
    public async Task AppSettings_WhenServiceNameIsNull_ValidationFails()
    {
        var options = new AppSettings { ServiceName = null!, Port = 8080 };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsFalse();
        await Assert.That(results.Any(r => r.MemberNames.Contains("ServiceName"))).IsTrue();
    }

    [Test]
    public async Task AppSettings_WhenServiceNameIsEmpty_ValidationFails()
    {
        var options = new AppSettings { ServiceName = "", Port = 8080 };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsFalse();
        await Assert.That(results.Any(r => r.MemberNames.Contains("ServiceName"))).IsTrue();
    }

    [Test]
    public async Task AppSettings_WhenServiceNameIsValid_ValidationPasses()
    {
        var options = new AppSettings { ServiceName = "ValidService", Port = 8080 };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsTrue();
        await Assert.That(results).IsEmpty();
    }

    [Test]
    public async Task AppSettings_WhenPortIsZero_ValidationFails()
    {
        var options = new AppSettings { ServiceName = "ValidService", Port = 0 };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsFalse();
        await Assert.That(results.Any(r => r.MemberNames.Contains("Port"))).IsTrue();
    }

    [Test]
    public async Task AppSettings_WhenPortIsNegative_ValidationFails()
    {
        var options = new AppSettings { ServiceName = "ValidService", Port = -1 };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsFalse();
        await Assert.That(results.Any(r => r.MemberNames.Contains("Port"))).IsTrue();
    }

    [Test]
    public async Task AppSettings_WhenPortIsAboveMax_ValidationFails()
    {
        var options = new AppSettings { ServiceName = "ValidService", Port = 65536 };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsFalse();
        await Assert.That(results.Any(r => r.MemberNames.Contains("Port"))).IsTrue();
    }

    [Test]
    public async Task AppSettings_WhenPortIsMin_ValidationPasses()
    {
        var options = new AppSettings { ServiceName = "ValidService", Port = 1 };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsTrue();
    }

    [Test]
    public async Task AppSettings_WhenPortIsMax_ValidationPasses()
    {
        var options = new AppSettings { ServiceName = "ValidService", Port = 65535 };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsTrue();
    }

    [Test]
    public async Task AppSettings_WhenPortIsTypical_ValidationPasses()
    {
        var options = new AppSettings { ServiceName = "ValidService", Port = 8080 };

        var isValid = Validate(options, out var results);

        await Assert.That(isValid).IsTrue();
    }
}
