# Request/Response Logging Middleware — План реализации

## Концепция

Middleware логирует входящие запросы и исходящие ответы для observability, аудита и отладки.

## Что логировать

### Входящий запрос
- Method (GET, POST, PUT, DELETE)
- Path + QueryString
- Content-Type (если есть)
- Content-Length (если есть)

### Исходящий ответ
- Status Code
- Content-Type
- Duration (мс)

### Формат лога
```
[Information] HTTP GET /api/scenarios?page=1 → 200 (45ms)
[Warning] HTTP POST /api/scenarios → 400 (12ms)
[Error] HTTP PUT /api/scenarios/1 → 500 (1523ms)
```

## Файлы

| Файл | Описание |
|------|----------|
| `Extensions/RequestResponseLogging/RequestResponseLoggingMiddleware.cs` | Middleware |
| `Extensions/RequestResponseLogging/RequestResponseLoggingExtensions.cs` | Extension-методы |
| `ScenarioDesigner.Tests/Extensions/RequestResponseLogging/RequestResponseLoggingMiddlewareTests.cs` | Unit-тесты |

## Порядок в Program.cs

```
DI:
builder.Services.AddCustomCors();
builder.Services.AddCustomCorrelationId();
builder.Services.AddCustomRequestResponseLogging();  ← ДОБАВИТЬ
builder.Services.AddCustomHealthChecks();
builder.Services.AddCustomRateLimiting();

Pipeline:
app.UseCors();
app.UseCustomCorrelationId();
app.UseCustomRequestResponseLogging();  ← ДОБАВИТЬ (после Correlation ID)
app.MapControllers();
app.UseRateLimiter();
app.UseCustomHealthChecks();
```

## Исключения

- Health checks на порту 8081 — НЕ логировать (отдельный pipeline)
- Фильтрация по categories через Serilog (уже настроено)

## Что НЕ логировать

- Тела запросов/ответов (太大, чувствительные данные)
- Health check эндпоинты
- Статические файлы (если появятся)

## Тесты

| Тест | Проверка |
|------|----------|
| InvokeAsync_LogsRequestAndResponse | Запрос → 200, логируется |
| InvokeAsync_LogsErrorStatus | Запрос → 500, логируется Warning/Error |
| InvokeAsync_IncludesCorrelationId | Correlation ID присутствует в логе |

## Порядок middleware

```
Request → Cors → CorrelationId → RequestResponseLogging → Next
Response ← RequestResponseLogging ← CorrelationId ← Cors ← Next
```

RequestResponseLogging оборачивает next, замеряет duration, логирует результат.
