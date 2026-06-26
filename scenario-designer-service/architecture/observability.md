# Observability — Текущее состояние

## Логирование (Serilog)

| Параметр     | Production                                           | Development                                          |
| ------------ | ---------------------------------------------------- | ---------------------------------------------------- |
| Root level   | Information                                          | Debug                                                |
| Microsoft    | Warning                                              | Information                                          |
| System       | Warning                                              | Information                                          |
| Console sink | false                                                | true (текстовый формат)                              |
| File sink    | true (JSON, rolling day, 14 дней)                    | true (текстовый формат, 7 дней)                      |
| Enrichers    | FromLogContext, WithMachineName, WithEnvironmentName | FromLogContext, WithMachineName, WithEnvironmentName |

**Файлы логов:**

- Production: `logs/scenario-designer-.log` (RollingInterval: Day, retention: 14 дней)
- Development: `logs/dev-.log` (RollingInterval: Day, retention: 7 дней)

**Динамическое управление:**

- LoggingLevelSwitch зарегистрирован в DI (singleton)
- Root level и overrides менять на лету через `PUT /api/logging/level`
- Fatal и Verbose запрещены (безопасность)

## Телеметрия (OpenTelemetry)

| Параметр         | Production                                  | Development                                 |
| ---------------- | ------------------------------------------- | ------------------------------------------- |
| Endpoint         | `http://otel-collector:4317`                | `http://localhost:4317`                     |
| Protocol         | gRPC                                        | gRPC                                        |
| Console exporter | false                                       | false                                       |
| Logs             | OTLP                                        | OTLP                                        |
| Traces           | OTLP + HttpClient instrumentation           | OTLP + HttpClient instrumentation           |
| Metrics          | OTLP + Runtime instrumentation + HttpClient | OTLP + Runtime instrumentation + HttpClient |

**Фильтры OTel-логов (отдельно от Serilog):**

- Default: Information
- Microsoft: Warning
- System: Warning

## Контроллеры

| Контроллер        | Роуты                         | Доступ      |
| ----------------- | ----------------------------- | ----------- |
| LoggingController | `GET /api/logging/level`      | AuditViewer |
|                   | `PUT /api/logging/level`      | AdminOnly   |
|                   | `GET /api/logging/categories` | AuditViewer |

---
