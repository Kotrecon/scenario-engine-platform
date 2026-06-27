using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using ScenarioDesigner.Controllers;

namespace ScenarioDesigner.Tests.Controllers;

public class ApiVersioningTests
{
    [Test]
    public async Task LoggingController_HasApiVersionAttribute()
    {
        var attribute = typeof(LoggingController).GetCustomAttributes(typeof(ApiVersionAttribute), false)
            .FirstOrDefault() as ApiVersionAttribute;

        await Assert.That(attribute).IsNotNull();
        await Assert.That(attribute!.Versions.Any(v => v == new ApiVersion(1, 0))).IsTrue();
    }

    [Test]
    public async Task LoggingController_HasCorrectRoute()
    {
        var routeAttribute = typeof(LoggingController).GetCustomAttributes(typeof(RouteAttribute), false)
            .FirstOrDefault() as RouteAttribute;

        await Assert.That(routeAttribute).IsNotNull();
        await Assert.That(routeAttribute!.Template).IsEqualTo("api/v{version:apiVersion}/[controller]");
    }
}
