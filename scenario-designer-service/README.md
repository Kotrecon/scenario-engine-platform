# Scenario Designer Service

Визуальный редактор сценариев оповещения для системы ситуационного управления (ЛСО).  
Часть платформы **Scenario Engine Platform** для критической инфраструктуры.

**GitHub:** https://github.com/Kotrecon/scenario-engine-platform

## Архитектура

```bash
backend/           — C# .NET 10 API (ASP.NET Core Web API)
frontend/          — Vanilla JS UI (планируется)
architecture/      — Документация, ADR, планы развития
docs-reference/    — Справочные документы (ГОСТ, ISA-18.2, EEMUA 191)
temp/              — Временные файлы (gitignored)
```

## Стек

| Компонент   | Технология                                   |
| ----------- | -------------------------------------------- |
| Runtime     | .NET 10.0                                    |
| SDK         | Microsoft.NET.Sdk.Web                        |
| DI          | Microsoft.Extensions.DependencyInjection     |
| Logging     | Serilog + OTel                               |
| Telemetry   | OpenTelemetry (traces, metrics, logs → OTLP) |
| Auth        | JWT Bearer + Policy-based авторизация        |
| Health      | ASP.NET Core Health Checks (два порта)       |
| Rate Limit  | System.Threading.RateLimiting                |
| ORM         | EF Core (планируется)                        |
| Cache       | Redis (планируется)                          |
| Message Bus | RabbitMQ / Kafka (планируется)               |

## Быстрый старт

```bash
cd backend
dotnet restore
dotnet build
dotnet run
```

## Структура backend

```bash
backend/
├── Configuration/
│   └── Options/
│       ├── AppSettings.cs
│       └── OpenTelemetryOptions.cs
├── Contracts/
│   └── Dto/
│       └── Request/
│           └── Logging/
│               └── SetLogLevelRequest.cs
├── Controllers/
│   └── LoggingController.cs
├── Extensions/
│   ├── HealthChecks/
│   │   └── HealthCheckExtensions.cs
│   ├── RateLimiting/
│   │   └── RateLimitingExtensions.cs
│   └── ServiceExtensions.cs
├── HealthChecks/
│   ├── MinimalResponseWriter.cs
│   └── ReadinessHealthCheck.cs
├── Security/
│   ├── AuthenticationExtensions.cs
│   └── AuthorizationExtensions.cs
├── Validation/
│   └── Configuration/
│       └── ConfigurationValidator.cs
├── Program.cs
├── ScenarioDesigner.csproj
├── appsettings.json
└── appsettings.Development.json
```

## Порты

| Порт | Назначение     | Доступ                 |
| ---- | -------------- | ---------------------- |
| 8080 | API (основной) | Внешний                |
| 8081 | Health checks  | Только внутренняя сеть |

## Конфигурация

| Параметр      | Production                                 | Development                             |
| ------------- | ------------------------------------------ | --------------------------------------- |
| Log level     | Information                                | Debug                                   |
| Console sink  | false                                      | true                                    |
| OTel endpoint | `http://otel-collector:4317`               | `http://localhost:4317`                 |
| JWT Key       | YourSuperSecretKeyAtLeast32CharactersLong! | DevelopmentKeyAtLeast32CharactersLong!! |

## API

| Метод | Роут                      | Описание                     | Доступ      |
| ----- | ------------------------- | ---------------------------- | ----------- |
| GET   | `/api/logging/level`      | Текущие уровни логирования   | AuditViewer |
| PUT   | `/api/logging/level`      | Изменить уровень логирования | AdminOnly   |
| GET   | `/api/logging/categories` | Список категорий             | AuditViewer |

## Health Checks

| Роут            | Описание               | Порт | Доступ     |
| --------------- | ---------------------- | ---- | ---------- |
| `/health/live`  | Процесс жив            | 8081 | Внутренний |
| `/health/ready` | Готов принимать трафик | 8081 | Внутренний |
| `/health`       | Агрегированный ответ   | 8081 | Внутренний |

- **Liveness:** всегда Healthy (игнорирует graceful shutdown)
- **Readiness:** Unhealthy при shutdown, кэш 5 сек, CommandTimeout 3 сек
- **Rate limiting:** 30 запросов за 10 секунд

## Роли и политики

| Роль     | Описание                              |
| -------- | ------------------------------------- |
| Admin    | Полный доступ (изменение, управление) |
| Operator | Чтение конфигурации и данных          |
| Auditor  | Только чтение логов и метрик          |

| Политика    | Роли                     |
| ----------- | ------------------------ |
| AdminOnly   | Admin                    |
| Operator    | Admin, Operator          |
| AuditViewer | Admin, Operator, Auditor |

---

## План развития

См. `architecture/plan.md` — 14 фаз, 100+ задач.

## Тестирование

| Компонент                      | Тестов | Покрытие  |
| ------------------------------ | ------ | --------- |
| ConfigurationValidator         | 15     | 100%      |
| ReadinessHealthCheck           | 8      | 100%      |
| MinimalResponseWriter          | 6      | 100%      |
| AppSettings                    | 9      | 100%      |
| OpenTelemetryOptions           | 13     | 100%      |
| SetLogLevelRequest + Validator | 8      | 100%      |
| LoggingController              | 16     | 100%      |
| AuthenticationExtensions       | 12     | 19.5%     |
| AuthorizationExtensions        | 6      | 100%      |
| **Итого**                      | **92** | **44.5%** |

- Фреймворк: TUnit 1.56.35
- Моки: Moq 4.20.72
- Покрытие: Microsoft.Testing.Extensions.CodeCoverage 18.8.0
- HTML отчёт: `coveragereport/index.html`
- Запуск: `dotnet run --project "backend/ScenarioDesigner.Tests/ScenarioDesigner.Tests.csproj"`

## Стандарты

- ISA-18.2 — Alarm Management
- ISA-88 — Procedure Control
- EEMUA 191 — Alarm Systems Guide
- IEC 62443 — Industrial Cybersecurity
- ГОСТ Р 22.7.05-2022 — Требования ЛСО
- ГОСТ Р 42.3.01-2021 — Требования к устройствам
- СП 484.1311500.2020 — Нормы проектирования систем оповещения
