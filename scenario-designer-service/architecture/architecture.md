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

## Регистрация сервисов (порядок в Program.cs)

```text
1. ConfigurationValidator.ValidateRequiredConfiguration — fail-fast
2. builder.AddAppSettings() — IOptions<T> с валидацией
3. builder.AddCustomLogging() — Serilog + LoggingLevelSwitch
4. builder.AddCustomOpenTelemetry() — OTel logs/traces/metrics
5. builder.AddCustomAuthentication() — JWT Bearer
6. builder.AddCustomAuthorization() — Policy-based
7. builder.Services.AddCustomHealthChecks() — health checks DI
8. builder.Services.AddCustomRateLimiting() — rate limiter DI
9. app.MapControllers() — MVC pipeline
10. app.UseRateLimiter() — rate limiter middleware
11. app.UseCustomHealthChecks() — health endpoints на порту 8081
```

## Порты

| Порт | Назначение     | Доступ                 |
| ---- | -------------- | ---------------------- |
| 8080 | API (основной) | Внешний                |
| 8081 | Health checks  | Только внутренняя сеть |

## Безопасность

| Компонент      | Реализация                                      |
| -------------- | ----------------------------------------------- |
| Аутентификация | JWT Bearer                                      |
| Авторизация    | Policy-based (AdminOnly, Operator, AuditViewer) |
| Roles          | Admin, Operator, Auditor                        |
| Issuer         | ScenarioDesigner                                |
| Audience       | ScenarioDesigner                                |
| ClockSkew      | 1 минута                                        |

## Среды

| Параметр      | Production                                 | Development                             |
| ------------- | ------------------------------------------ | --------------------------------------- |
| Log level     | Information                                | Debug                                   |
| Console sink  | false                                      | true                                    |
| OTel endpoint | `http://otel-collector:4317`               | `http://localhost:4317`                 |
| JWT Key       | YourSuperSecretKeyAtLeast32CharactersLong! | DevelopmentKeyAtLeast32CharactersLong!! |

---

## ADR (Architecture Decision Records)

### ADR-001: Web SDK вместо Console SDK

**Статус:** Принято

**Контекст:** Проект был начат как консольный (Microsoft.NET.Sdk), но требовался ASP.NET Core для API, controllers, JWT auth.

**Решение:** Переключиться на `Microsoft.NET.Sdk.Web`.

**Последствия:**

- Доступны AddControllers, MapControllers, middleware pipeline
- Kestrel встроен
- Дополнительные пакеты не нужны (Microsoft.AspNetCore.\* уже в SDK)

---

### ADR-002: Serilog через appsettings.json

**Статус:** Принято

**Контекст:** Serilog можно настраивать программно (WriteTo.Console) или через конфигурацию (ReadFrom.Configuration).

**Решение:** Стандартный формат Serilog в appsettings.json + ReadFrom.Configuration.

**Последствия:**

- Sinks настраиваются через JSON (не код)
- Добавление/удаление sinks без перекомпиляции
- Enricherse конфигурации

---

### ADR-003: LoggingLevelSwitch в DI

**Статус:** Принято

**Контекст:** Уровни логирования должны меняться на лету через API без restart.

**Решение:** LoggingLevelSwitch зарегистрирован как singleton в DI, контроллер получает его через constructor injection.

**Последствия:**

- Динамическое изменение уровней через PUT /api/logging/level
- Нет restart для смены уровня
- Fatal и Verbose запрещены (безопасность)

---

### ADR-004: Policy-based авторизация

**Статус:** Принято

**Контекст:** Нужно разграничить доступ: Admin (изменение), Operator (чтение), Auditor (только чтение).

**Решение:** Policy-based авторизация с тремя политиками: AdminOnly, Operator, AuditViewer.

**Последствия:**

- Гибкость: можно добавить policy без изменения кода контроллера
- Тестируемость: policies можно проверить через IAuthorizationService
- Читаемость: `[Authorize(Policy = "AdminOnly")]` вместо `[Authorize(Roles = "Admin")]`

---

### ADR-005: Health checks на отдельном порту

**Статус:** Принято

**Контекст:** Health checks должны быть доступны для оркестратора (Kubelet, Prometheus), но не доступны извне.

**Решение:** Два Kestrel-эндпоинта: API (8080) и Health (8081). Health-порт не маршрутизируется наружу (firewall / Nginx / Cloud LB).

**Последствия:**

- Middleware (logging, correlation, swagger, CORS) автоматически исключены из health
- Сетевая изоляция вместо секретных заголовков
- Проще настраивать firewall / Nginx

---

### ADR-006: Rate limiting на health-ветку

**Статус:** Принято

**Контекст:** Даже с сетевой изоляцией, при утечке учётных данных или DDoS-атаке health-чеки могут перегрузить БД.

**Решение:** Fixed window limiter: 30 запросов за 10 секунд на `/health/*`.

**Последствия:**

- Дополнительная мера защиты (не основная)
- Kubelet (5-10 сек) + Prometheus (15-30 сек) + 3 эндпоинта = ~5-6 запросов за 10 сек
- Лимит 30 — с запасом
