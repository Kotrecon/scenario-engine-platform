# Архитектура — Текущее состояние

| Поле       | Значение   |
| ---------- | ---------- |
| **Версия** | 1.3.0      |
| **Статус** | Active     |
| **Дата**   | 2026-07-02 |

---

## Стек технологий

| Компонент      | Технология                                    | Версия  |
| -------------- | --------------------------------------------- | ------- |
| Runtime        | .NET                                          | 10.0    |
| SDK            | Microsoft.NET.Sdk.Web                         | —       |
| DI             | Microsoft.Extensions.DependencyInjection      | 10.0.9  |
| Hosting        | Microsoft.Extensions.Hosting                  | 10.0.9  |
| Configuration  | Microsoft.Extensions.Configuration.Json       | 10.0.9  |
| Logging        | Serilog.AspNetCore                            | 10.0.0  |
| Telemetry      | OpenTelemetry.Extensions.Hosting              | 1.16.0  |
| Auth           | Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.9  |
| Token Gen      | System.IdentityModel.Tokens.Jwt               | 8.19.1  |
| API Versioning | Asp.Versioning.Mvc                            | 10.0.0  |
| OpenAPI        | Microsoft.AspNetCore.OpenApi                  | 10.0.9  |
| API UI         | Scalar.AspNetCore                             | 2.13.19 |
| Testing        | TUnit                                         | 1.56.35 |
| Mocking        | Moq                                           | 4.20.72 |
| Coverage       | Microsoft.Testing.Extensions.CodeCoverage     | 18.8.0  |
| Response Cache | Microsoft.AspNetCore.ResponseCaching          | 10.0.9  |
| ORM            | EF Core (планируется)                         | —       |
| Cache          | Redis (планируется)                           | —       |
| Message Bus    | RabbitMQ / Kafka (планируется)                | —       |

---

## Структура проекта

```bash
backend/
├── Configuration/
│   └── Options/
│       ├── AppSettings.cs
│       ├── JwtOptions.cs
│       ├── OpenTelemetryOptions.cs
│       ├── ApiMetadataOptions.cs
│       └── ContactInfo.cs
├── Contracts/
│   ├── Dto/
│   │   └── Request/
│   │       └── Logging/
│   │           ├── SetLogLevelRequest.cs
│   │           └── SetLogLevelValidator.cs
│   └── Result/
│       ├── Common/
│       │   ├── IError.cs
│       │   ├── Result.cs
│       │   ├── ResultOfT.cs
│       │   ├── ValidationError.cs
│       │   ├── NotFoundError.cs
│       │   ├── ConflictError.cs
│       │   ├── ForbiddenError.cs
│       │   └── BusinessRuleError.cs
│       └── Web/
│           └── ResultExtensions.cs
├── Controllers/
│   └── LoggingController.cs
├── Extensions/
│   ├── ConfigurationExtensions.cs
│   ├── ObservabilityExtensions.cs
│   ├── CorrelationId/
│   │   ├── CorrelationIdExtensions.cs
│   │   └── CorrelationIdMiddleware.cs
│   ├── Cors/
│   │   └── CorsExtensions.cs
│   ├── ExceptionHandler/
│   │   ├── ExceptionHandlerExtensions.cs
│   │   └── ExceptionHandlerMiddleware.cs
│   ├── RequestResponseLogging/
│   │   ├── RequestResponseLoggingExtensions.cs
│   │   └── RequestResponseLoggingMiddleware.cs
│   ├── HealthChecks/
│   │   └── HealthCheckExtensions.cs
│   └── RateLimiting/
│       └── RateLimitingExtensions.cs
├── HealthChecks/
│   ├── IDatabaseHealthChecker.cs
│   ├── DatabaseHealthChecker.cs
│   ├── MinimalResponseWriter.cs
│   └── ReadinessHealthCheck.cs
├── Security/
│   ├── AuthenticationExtensions.cs
│   └── AuthorizationExtensions.cs
├── ScenarioDesigner.Tests/
│   ├── Configuration/
│   │   └── Options/
│   │       ├── AppSettingsTests.cs
│   │       ├── JwtOptionsTests.cs
│   │       ├── OpenTelemetryOptionsTests.cs
│   │       ├── ApiMetadataOptionsTests.cs
│   │       └── ContactInfoTests.cs
│   ├── Contracts/
│   │   ├── Dto/
│   │   │   └── SetLogLevelRequestTests.cs
│   │   └── Result/
│   │       ├── ResultTests.cs
│   │       ├── ResultOfTTests.cs
│   │       ├── ErrorTests.cs
│   │       └── ResultExtensionsTests.cs
│   ├── Controllers/
│   │   ├── LoggingControllerTests.cs
│   │   └── ApiVersioningTests.cs
│   ├── Extensions/
│   │   ├── ConfigurationExtensionsTests.cs
│   │   ├── ObservabilityExtensionsTests.cs
│   │   ├── Cors/
│   │   │   └── CorsExtensionsTests.cs
│   │   ├── CorrelationId/
│   │   │   └── CorrelationIdMiddlewareTests.cs
│   │   ├── ExceptionHandler/
│   │   │   └── ExceptionHandlerMiddlewareTests.cs
│   │   └── RequestResponseLogging/
│   │       └── RequestResponseLoggingMiddlewareTests.cs
│   ├── HealthChecks/
│   │   ├── MinimalResponseWriterTests.cs
│   │   └── ReadinessHealthCheckTests.cs
│   ├── Integration/
│   │   ├── Infrastructure/
│   │   │   └── TestWebApplicationFactory.cs
│   │   ├── AuthenticationTests.cs
│   │   ├── AuthorizationTests.cs
│   │   ├── DevTokenEndpointTests.cs
│   │   ├── CorrelationIdE2ETests.cs
│   │   └── MetadataEndpointTests.cs
│   ├── Security/
│   │   ├── AuthenticationExtensionsTests.cs
│   │   └── AuthorizationExtensionsTests.cs
│   └── Helpers/
│       └── RecursiveValidator.cs
├── Program.cs
├── ScenarioDesigner.csproj
├── ScenarioDesigner.Tests.csproj
├── appsettings.json
├── appsettings.Development.json
└── appsettings.Production.json
```

---

## Регистрация сервисов (порядок в Program.cs)

> Это основной источник истины для порядка регистрации и middleware pipeline.

### DI Container (порядок регистрации)

```csharp
1.  builder.AddCustomLogging() — Serilog + LoggingLevelSwitch
2.  builder.AddAppSettings() — IOptions<AppSettings> + ValidateOnStart
3.  builder.AddJwt() — IOptions<JwtOptions> + ValidateOnStart
4.  builder.AddApiMetadata() — IOptions<ApiMetadataOptions> + ValidateOnStart
5.  builder.AddOpenTelemetryOptions() — IOptions<OpenTelemetryOptions> + ValidateOnStart
6.  builder.AddCustomOpenTelemetry() — OTel logs/traces/metrics pipeline
7.  builder.AddCustomAuthentication() — JWT Bearer
8.  builder.AddCustomAuthorization() — Policy-based
9.  builder.Services.AddApiVersioning() — URL-based versioning (v1)
10. builder.Services.AddCustomCors() — CORS (AllowAll для разработки)
11. builder.Services.AddCustomExceptionHandler() — Exception Handler
12. builder.Services.AddCustomCorrelationId() — Correlation ID (Guid.CreateVersion7)
13. builder.Services.AddCustomRequestResponseLogging() — Request/Response logging
14. builder.Services.AddCustomHealthChecks() — health checks DI
15. builder.Services.AddCustomRateLimiting() — rate limiter DI
16. builder.Services.AddResponseCaching() — response caching
17. builder.Services.AddOpenApi() — OpenAPI 3.1 документ + JWT Bearer схема
```

### Middleware Pipeline (порядок выполнения)

```csharp
19. app.UseCustomExceptionHandler() — Exception Handler (ПЕРВЫМ middleware)
20. app.UseCors() — CORS middleware
21. app.UseCustomCorrelationId() — Correlation ID middleware
22. app.UseCustomRequestResponseLogging() — Request/Response logging middleware
23. app.UseAuthentication() — JWT authentication
24. app.UseAuthorization() — policy-based authorization
25. app.UseResponseCaching() — response caching
26. app.UseRateLimiter() — rate limiter middleware
27. app.MapControllers() — MVC pipeline
28. app.MapGet("/api/metadata") — metadata endpoint [AllowAnonymous]
29. app.MapOpenApi() — OpenAPI endpoint (/openapi/v1.json) [Development only]
30. app.MapScalarApiReference() — Scalar UI (/scalar/v1) [Development only]
31. app.MapPost("/dev/token") — Dev token endpoint [Development only]
32. app.UseCustomHealthChecks() — health endpoints на порту 8081
```

---

## OpenAPI + Scalar UI

### Назначение

Интерактивная документация и тестирование API в окружении Development.

### Endpoints

| URL                | Описание                               |
| ------------------ | -------------------------------------- |
| `/openapi/v1.json` | OpenAPI 3.1 документ                   |
| `/scalar/v1`       | Scalar UI — интерактивная документация |

### Особенности

- Включено только в `Development`
- JWT Bearer схема встроена в документ (кнопка Authorize в UI)
- XML-комментарии из кода автоматически включаются в документацию
- Source generator генерирует трансформеры для OpenAPI-документа
- Метаданные (title, version, description, developer) берутся из `ApiMetadataOptions` — единый источник с `/api/metadata`

---

## Metadata API

### Назначение

Публичный endpoint с метаданными API. Используется фронтендом для отображения версии API, footer, about-страницы.

### Endpoint

| URL             | Описание              | Доступ    |
| --------------- | --------------------- | --------- |
| `/api/metadata` | Метаданные API (JSON) | Анонимный |

### Особенности

- Не требует аутентификации (`AllowAnonymous`)
- Кэшируется на 1 час (`ResponseCache(Duration = 3600)`)
- Источник данных: `ApiMetadataOptions` (appsettings.json)
- Единый источник с OpenAPI/Scalar — те же метаданные

---

## Безопасность

| Компонент      | Реализация                                      |
| -------------- | ----------------------------------------------- |
| Аутентификация | JWT Bearer                                      |
| Авторизация    | Policy-based (AdminOnly, Operator, AuditViewer) |
| Roles          | Admin, Operator, Auditor                        |
| Issuer         | ScenarioDesigner                                |
| Audience       | ScenarioDesigner                                |
| ClockSkew      | 1 минута                                        |
| Token Gen      | JsonWebTokenHandler (Microsoft.IdentityModel)   |
| Dev Endpoint   | /dev/token (только Development)                 |
| Metadata API   | /api/metadata (публичный, без секретов)         |

---

## Среды

| Параметр      | Production                                 | Development                             |
| ------------- | ------------------------------------------ | --------------------------------------- |
| Log level     | Information                                | Debug                                   |
| Console sink  | false                                      | true                                    |
| OTel endpoint | `http://otel-collector:4317`               | `http://localhost:4317`                 |
| JWT Key       | YourSuperSecretKeyAtLeast32CharactersLong! | DevelopmentKeyAtLeast32CharactersLong!! |
| OpenAPI UI    | Отключено                                  | Включено (/scalar/v1)                   |
| Dev Endpoint  | Отключено                                  | Включено (/dev/token)                   |
| ApiMetadata   | Production значения                        | Development значения                    |

---

## Связанные документы

- [`api.md`](./api.md) — API-эндпоинты, аутентификация, форматы запросов/ответов
- [`operability.md`](./operability.md) — Health Checks, Graceful Shutdown, Rate Limiting, CORS, Exception Handler, Response Caching
- [`observability.md`](./observability.md) — Логирование (Serilog), Request/Response Logging, OpenTelemetry
- [`adr.md`](./adr.md) — Architecture Decision Records
- [`auth-flow.md`](./auth-flow.md) — Аутентификация и авторизация, политики, известные баги
- [`deployment.md`](./deployment.md) — Развёртывание: Docker, Kubernetes, env vars, secrets
- [`TODO.md`](./TODO.md) — Все незавершённые задачи

---

## Что изменилось в v1.2.0

| Элемент             | Изменение                                                                     |
| ------------------- | ----------------------------------------------------------------------------- |
| Версия документа    | 1.1.0 → 1.2.0                                                                 |
| Стек технологий     | Добавлен `Microsoft.AspNetCore.ResponseCaching`                               |
| Структура проекта   | Добавлены `JwtOptions`, `ApiMetadataOptions`, `ContactInfo`                   |
| Структура проекта   | Добавлены `ConfigurationExtensions`, `ObservabilityExtensions`                |
| Структура проекта   | Удалены `ServiceExtensions`, `ConfigurationValidator`                         |
| Структура проекта   | Добавлены тесты для всех Options-классов и Extensions                         |
| Структура проекта   | Добавлен `Helpers/RecursiveValidator.cs`                                      |
| DI Container        | Добавлены `AddJwt()`, `AddApiMetadata()`, `AddOpenTelemetryOptions()`         |
| DI Container        | Добавлен `AddResponseCaching()`                                               |
| Middleware Pipeline | Добавлены `UseAuthentication()`, `UseAuthorization()`, `UseResponseCaching()` |
| Middleware Pipeline | Добавлен `/api/metadata` endpoint                                             |
| Middleware Pipeline | Убран `ConfigurationValidator.ValidateRequiredConfiguration`                  |
| Безопасность        | Добавлена строка про Metadata API                                             |
| Среды               | Добавлена строка про ApiMetadata                                              |
| Раздел Metadata API | Новый раздел                                                                  |

---

## Что изменилось в v1.3.0

| Элемент             | Изменение                                                                     |
| ------------------- | ----------------------------------------------------------------------------- |
| Версия документа    | 1.2.0 → 1.3.0                                                                 |
| Структура проекта   | Добавлен `appsettings.Production.json`                                        |
| DI Container        | Исправлено дублирование: `AddOpenTelemetryOptions` → `AddCustomOpenTelemetry` |
| DI Container        | Перенумерованы пункты (18 → 17)                                               |
| Связанные документы | Добавлены `auth-flow.md`, `deployment.md`                                     |
