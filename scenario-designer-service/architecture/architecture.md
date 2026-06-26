# Архитектура — Текущее состояние

## Стек технологий

| Компонент     | Технология                                    | Версия  |
| ------------- | --------------------------------------------- | ------- |
| Runtime       | .NET                                          | 10.0    |
| SDK           | Microsoft.NET.Sdk.Web                         | —       |
| DI            | Microsoft.Extensions.DependencyInjection      | 10.0.9  |
| Hosting       | Microsoft.Extensions.Hosting                  | 10.0.9  |
| Configuration | Microsoft.Extensions.Configuration.Json       | 10.0.9  |
| Logging       | Serilog.AspNetCore                            | 10.0.0  |
| Telemetry     | OpenTelemetry.Extensions.Hosting              | 1.16.0  |
| Auth          | Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.9  |
| Testing       | TUnit                                         | 1.56.35 |
| Mocking       | Moq                                           | 4.20.72 |
| Coverage      | Microsoft.Testing.Extensions.CodeCoverage     | 18.8.0  |
| ORM           | EF Core (планируется)                         | —       |
| Cache         | Redis (планируется)                           | —       |
| Message Bus   | RabbitMQ / Kafka (планируется)                | —       |

---

## Структура проекта

```bash
backend/
├── Configuration/
│   └── Options/
│       ├── AppSettings.cs
│       └── OpenTelemetryOptions.cs
├── Contracts/
│   └── Dto/
│       └── Request/
│           ├── Logging/
│           │   ├── SetLogLevelRequest.cs
│           │   └── SetLogLevelValidator.cs
│           └── Scenarios/
├── Controllers/
│   └── LoggingController.cs
├── Extensions/
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
│   ├── RateLimiting/
│   │   └── RateLimitingExtensions.cs
│   └── ServiceExtensions.cs
├── HealthChecks/
│   ├── IDatabaseHealthChecker.cs
│   ├── DatabaseHealthChecker.cs
│   ├── MinimalResponseWriter.cs
│   └── ReadinessHealthCheck.cs
├── ScenarioDesigner.Tests/             ← TUnit тесты
│   ├── Configuration/
│   │   └── Options/
│   │       ├── AppSettingsTests.cs
│   │       └── OpenTelemetryOptionsTests.cs
│   ├── Contracts/
│   │   └── Dto/
│   │       └── SetLogLevelRequestTests.cs
│   ├── Controllers/
│   │   └── LoggingControllerTests.cs
│   ├── HealthChecks/
│   │   ├── MinimalResponseWriterTests.cs
│   │   └── ReadinessHealthCheckTests.cs
│   ├── Security/
│   │   ├── AuthenticationExtensionsTests.cs
│   │   └── AuthorizationExtensionsTests.cs
│   ├── Extensions/
│   │   └── Cors/
│   │       └── CorsExtensionsTests.cs
│   └── Validation/
│       └── ConfigurationValidatorTests.cs
├── Security/
│   ├── AuthenticationExtensions.cs
│   └── AuthorizationExtensions.cs
├── Validation/
│   └── Configuration/
│       └── ConfigurationValidator.cs
├── Program.cs
├── ScenarioDesigner.csproj
├── ScenarioDesigner.Tests.csproj
├── appsettings.json
└── appsettings.Development.json
```

---

## Регистрация сервисов (порядок в Program.cs)

> Это основной источник истины для порядка регистрации и middleware pipeline.

```csharp
1. ConfigurationValidator.ValidateRequiredConfiguration — fail-fast
2. builder.AddAppSettings() — IOptions<T> с валидацией
3. builder.AddCustomLogging() — Serilog + LoggingLevelSwitch
4. builder.AddCustomOpenTelemetry() — OTel logs/traces/metrics
5. builder.AddCustomAuthentication() — JWT Bearer
6. builder.AddCustomAuthorization() — Policy-based
7. builder.Services.AddCustomCors() — CORS (AllowAll для разработки)
8. builder.Services.AddCustomExceptionHandler() — Exception Handler
9. builder.Services.AddCustomCorrelationId() — Correlation ID (Guid.CreateVersion7)
10. builder.Services.AddCustomRequestResponseLogging() — Request/Response logging
11. builder.Services.AddCustomHealthChecks() — health checks DI
12. builder.Services.AddCustomRateLimiting() — rate limiter DI
13. app.MapControllers() — MVC pipeline
14. app.UseCustomExceptionHandler() — Exception Handler (ПЕРВЫМ middleware)
15. app.UseCors() — CORS middleware
16. app.UseCustomCorrelationId() — Correlation ID middleware
17. app.UseCustomRequestResponseLogging() — Request/Response logging middleware
18. app.UseRateLimiter() — rate limiter middleware
19. app.UseCustomHealthChecks() — health endpoints на порту 8081
```

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

---

## Среды

| Параметр      | Production                                 | Development                             |
| ------------- | ------------------------------------------ | --------------------------------------- |
| Log level     | Information                                | Debug                                   |
| Console sink  | false                                      | true                                    |
| OTel endpoint | `http://otel-collector:4317`               | `http://localhost:4317`                 |
| JWT Key       | YourSuperSecretKeyAtLeast32CharactersLong! | DevelopmentKeyAtLeast32CharactersLong!! |

---

## Связанные документы

- [`operability.md`](./operability.md) — Health Checks, Graceful Shutdown, Rate Limiting, CORS, Exception Handler
- [`observability.md`](./observability.md) — Логирование (Serilog), Request/Response Logging, OpenTelemetry
- [`adr.md`](./adr.md) — Architecture Decision Records
- [`TODO.md`](./TODO.md) — Все незавершённые задачи

---
