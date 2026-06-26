# Global Exception Handler Middleware — План реализации

## Концепция

Перехватывает все необработанные исключения в pipeline и возвращает единообразный JSON-ответ. Предотвращает утечку внутренних деталей (stack traces, IP, названия сервисов) во внешнюю сеть.

## Порядок в Program.cs

```csharp
// 1. DI
builder.Services.AddCustomCors();
builder.Services.AddCustomCorrelationId();
builder.Services.AddCustomExceptionHandler();

// 2. Pipeline (ПЕРВЫМ middleware)
app.UseCustomExceptionHandler();
app.UseCors();
app.UseCustomCorrelationId();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCustomHealthChecks();
```

## Реализация

### 1. Middleware/ExceptionHandlerMiddleware.cs

```csharp
public sealed class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status403Forbidden,
            TimeoutException => StatusCodes.Status504GatewayTimeout,
            _ => StatusCodes.Status500InternalServerError
        };

        var response = JsonSerializer.Serialize(new
        {
            error = new
            {
                code = context.Response.StatusCode,
                message = exception.Message  // Только message, без stack trace
            }
        });

        return context.Response.WriteAsync(response);
    }
}
```

### 2. Extensions/ExceptionHandlerExtensions.cs

```csharp
public static class ExceptionHandlerExtensions
{
    public static IServiceCollection AddCustomExceptionHandler(this IServiceCollection services)
    {
        // DI не нужен — middleware создается через RequestDelegate
        return services;
    }

    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}
```

## Формат ответа

```json
{
  "error": {
    "code": 400,
    "message": "Invalid scenario name"
  }
}
```

## Что НЕ отдаём

- Stack traces
- IP-адреса
- Названия сервисов/БД
- Внутренние пути файлов

## Исключения из /health/*

Реализовано через архитектуру: health checks на порту 8081, exception handler — на порту 8080.

## Тесты

- [ ] Unit-тест: маппинг исключений в HTTP-коды
- [ ] Integration-тест: запрос с ошибкой → JSON-ответ
- [ ] Integration-тест: health check на порту 8081 — без exception handler

## Чек-лист

- [ ] Создать `Middleware/ExceptionHandlerMiddleware.cs`
- [ ] Создать `Extensions/ExceptionHandlerExtensions.cs`
- [ ] Добавить `app.UseCustomExceptionHandler()` в Program.cs (ПЕРВЫМ middleware)
- [ ] Проверить, что health checks на порту 8081 не обрабатываются exception handler
- [ ] Добавить unit-тесты
- [ ] Добавить integration-тесты
