using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using ScenarioDesigner.Extensions.RequestResponseLogging;

namespace ScenarioDesigner.Tests.Extensions.RequestResponseLogging;

public class RequestResponseLoggingMiddlewareTests
{
    [Test]
    public async Task InvokeAsync_LogsSuccessfulRequest()
    {
        var logger = new Mock<ILogger<RequestResponseLoggingMiddleware>>();
        var middleware = new RequestResponseLoggingMiddleware(_ => Task.CompletedTask, logger.Object);
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/api/test";
        context.Response.StatusCode = 200;

        await middleware.InvokeAsync(context);

        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("GET") && v.ToString()!.Contains("/api/test") && v.ToString()!.Contains("200")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task InvokeAsync_LogsErrorStatusAsWarning()
    {
        var logger = new Mock<ILogger<RequestResponseLoggingMiddleware>>();
        var middleware = new RequestResponseLoggingMiddleware(_ => Task.CompletedTask, logger.Object);
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Path = "/api/test";
        context.Response.StatusCode = 400;

        await middleware.InvokeAsync(context);

        logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task InvokeAsync_Logs500AsWarning()
    {
        var logger = new Mock<ILogger<RequestResponseLoggingMiddleware>>();
        var middleware = new RequestResponseLoggingMiddleware(_ => Task.CompletedTask, logger.Object);
        var context = new DefaultHttpContext();
        context.Request.Method = "PUT";
        context.Request.Path = "/api/test/1";
        context.Response.StatusCode = 500;

        await middleware.InvokeAsync(context);

        logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
