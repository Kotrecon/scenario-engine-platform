# Request/Response Logging Middleware — Текущее состояние

## Концепция

Middleware логирует входящие запросы и исходящие ответы для observability, аудита и отладки.

## Реализовано

### Что логирует

- Method (GET, POST, PUT, DELETE)
- Path + QueryString
- Status Code
- Duration (мс)

### Формат лога

```
[Information] HTTP GET /api/scenarios?page=1 → 200 (45ms)
[Warning] HTTP POST /api/scenarios → 400 (12ms)
```

### Файлы

| Файл | Описание |
|------|----------|
| `Extensions/RequestResponseLogging/RequestResponseLoggingMiddleware.cs` | Middleware |
| `Extensions/RequestResponseLogging/RequestResponseLoggingExtensions.cs` | Extension-методы |
| `ScenarioDesigner.Tests/Extensions/RequestResponseLogging/RequestResponseLoggingMiddlewareTests.cs` | Unit-тесты (2) |

### Порядок в Program.cs

```
DI:
builder.Services.AddCustomCors();
builder.Services.AddCustomCorrelationId();
builder.Services.AddCustomRequestResponseLogging();
builder.Services.AddCustomHealthChecks();
builder.Services.AddCustomRateLimiting();

Pipeline:
app.UseCors();
app.UseCustomCorrelationId();
app.UseCustomRequestResponseLogging();
app.MapControllers();
app.UseRateLimiter();
app.UseCustomHealthChecks();
```

### Исключения

- Health checks на порту 8081 — НЕ логирует (отдельный pipeline)
- Фильтрация по categories через Serilog (уже настроено)

### Что НЕ логирует

- Тела запросов/ответов (чувствительные данные)
- Health check эндпоинты

### Тесты

| Тест | Проверка |
|------|----------|
| InvokeAsync_LogsSuccessfulRequest | GET → 200, Information + проверка "ms" |
| InvokeAsync_LogsErrorStatusAsWarning | POST → 400, Warning |

### Покрытие

- RequestResponseLoggingMiddleware: 100% line, 100% branch
- RequestResponseLoggingExtensions: 0% (пустой метод)

---

## Позже (расширение)

- [ ] Логирование Content-Type (если нужен для отладки)
- [ ] Логирование User-Agent (для аудита)
- [ ] Исключения по path (фильтр через конфигурацию)
- [ ] Уровни логирования по типу ошибки (4xx = Warning, 5xx = Error)
