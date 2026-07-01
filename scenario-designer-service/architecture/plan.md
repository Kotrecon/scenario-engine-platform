# План разработки Scenario Designer Service

| Поле       | Значение   |
| ---------- | ---------- |
| **Версия** | 1.2.0      |
| **Статус** | Active     |
| **Дата**   | 2026-07-01 |

> Дорожная карта проекта с разбивкой на фазы.
>
> Связанные документы:
>
> - [`operability.md`](./operability.md) — Health Checks, Graceful Shutdown, Rate Limiting, CORS, Exception Handler
> - [`observability.md`](./observability.md) — Логирование, Correlation ID, OpenTelemetry
> - [`result-pattern.md`](./result-pattern.md) — Result Pattern библиотека
> - [`api.md`](./api.md) — HTTP-контракты
> - [`architecture.md`](./architecture.md) — Стек технологий, структура проекта
> - [`adr.md`](./adr.md) — Architecture Decision Records
> - [`testing.md`](./testing.md) — План тестирования
> - [`auth-flow.md`](./auth-flow.md) — Аутентификация и авторизация
> - [`TODO.md`](./TODO.md) — Текущие задачи

---

## Фаза 0: Backend Infrastructure

- [x] Health checks (подробнее: [`operability.md`](./operability.md))
- [x] Global exception handler middleware
- [x] Request/Response logging middleware
- [x] Correlation ID middleware
- [x] CORS policy
- [x] Result Pattern библиотека (179 тестов, подробнее: [`result-pattern.md`](./result-pattern.md))
- [x] ProblemDetails layer (RFC 7807) — интеграция с Result Pattern
- [x] API Versioning (Asp.Versioning.Mvc, URL-based, подробнее: [`api.md`](./api.md))
- [x] OpenAPI + Scalar UI с JWT-авторизацией (Microsoft.AspNetCore.OpenApi 10.0.9 + Scalar 2.13.19, подробнее: [`adr.md`](./adr.md) ADR-015)
- [x] Dev endpoint `/dev/token` для генерации тестовых JWT (только Development)
- [x] Metadata API endpoint `GET /api/metadata` (публичный, кэш 1 час, подробнее: [`adr.md`](./adr.md) ADR-016)
- [x] Configuration DTO: `AppSettings`, `JwtOptions`, `OpenTelemetryOptions`, `ApiMetadataOptions` + `ContactInfo` (records с DataAnnotations)
- [x] Рефакторинг валидатора конфигурации: разбиение на Options-классы с `IValidatableObject` + `ValidateOnStart()`
- [x] `ConfigurationExtensions` — регистрация Options через `AddOptions<T>().Bind().ValidateDataAnnotations().ValidateOnStart()`
- [x] `ObservabilityExtensions` — `AddCustomLogging` (Serilog) + `AddCustomOpenTelemetry` (OTLP)
- [x] Response caching middleware (`AddResponseCaching()` + `UseResponseCaching()`)
- [x] Authentication/Authorization middleware в pipeline (`UseAuthentication()` + `UseAuthorization()`)
- [x] Unit-тесты Configuration DTO (AppSettings, JwtOptions, OpenTelemetryOptions, ApiMetadataOptions, ContactInfo)
- [x] Unit-тесты ConfigurationExtensions (12 тестов)
- [x] Unit-тесты ObservabilityExtensions (5 тестов)
- [x] Coverlet настройка (`coverlet.runsettings` для исключения сгенерированного кода)
- [ ] Persistence layer: DbContext, entity configs, migrations, transaction boundary (operability)
- [ ] Messaging layer: publisher/consumer contracts, retry, idempotency, outbox skeleton (operability)

---

## Фаза 1: UI Kit — адаптация

- [ ] Адаптация дизайн-системы под проект
- [ ] Цветовая схема под EEMUA 191
- [ ] Тёмная тема (#0a0c10 bg, #111318 panel)
- [ ] Базовые компоненты: кнопки, инпуты, таблицы, модалки

---

## Фаза 2: Domain Layer

- [ ] Энумы: `TriggerStatus`, `ScenarioStatus`, `ExecutionMode`, `AutoFallbackAction`, `ActionType`
- [ ] Сущность `Trigger` + валидатор + unit-тесты
- [ ] Сущность `Scenario` + валидатор + unit-тесты
- [ ] Сущность `ScenarioAction` + валидатор + unit-тесты
- [ ] Связь M:N `ScenarioTrigger` + валидатор + unit-тесты
- [ ] UI: базовые карточки для отображения сущностей

---

## Фаза 3: Data & Storage

- [ ] Database (EF Core) + migrations
- [ ] Repository pattern + integration-тесты
- [ ] Redis cache
- [ ] Extended health check: БД
- [ ] UI: список сценариев, список триггеров

---

## Фаза 4: Application Services

- [ ] `IScenarioService` — CRUD + lifecycle + тесты
- [ ] `ITriggerService` — CRUD + валидация + тесты
- [ ] `IScenarioActionService` — CRUD + тесты
- [ ] State machine для связей (подготовка к n8n-style)

---

## Фаза 5: API Layer

- [ ] DTO + unit-тесты валидации
- [ ] `ScenariosController` + integration-тесты
- [ ] `TriggersController` + integration-тесты
- [ ] `ScenarioActionsController` + integration-тесты
- [ ] `AuthController` — login, register, refresh, logout
- [ ] `AdminController` — дашборды, настройки пользователей
- [ ] UI: страница входа/регистрации
- [ ] UI: админ-дашборд (метрики, KPI, список пользователей)

---

## Фаза 6: UI Editor (n8n-style оформление)

- [ ] Визуальный редактор сценариев в стиле n8n (только оформление)
- [ ] Карточки для триггеров и условий (визуальный стиль)
- [ ] Валидация в реальном времени
- [ ] Contract-тесты API (Pact)

---

## Фаза 7: Resilience & Security

- [ ] Circuit breaker (Polly)
- [ ] Retry policies (Polly)
- [ ] Timeout policies
- [ ] Rate limiting (расширение на API-эндпоинты)
- [ ] Secret management
- [ ] Audit logging
- [ ] Data encryption
- [ ] Load tests (k6)

---

## Фаза 8: Messaging & Events

- [ ] Message bus (RabbitMQ / Kafka)
- [ ] Pub/Sub pattern
- [ ] Event-driven architecture
- [ ] Outbox pattern

---

## Фаза 9: Background Jobs

- [ ] Scheduled tasks
- [ ] Background workers

---

## Фаза 10: Observability

- [ ] Кастомные метрики (Counter: scenarios_created, scenarios_executed)
- [ ] Histogram для времени выполнения сценариев
- [ ] Metrics dashboards (Grafana)
- [ ] Log aggregation (ELK / Loki)
- [ ] Alerting (Prometheus Alertmanager)

---

## Фаза 11: Integration (когда появятся сервисы)

- [ ] Контракты для Scenario Registry
- [ ] Контракты для Scenario Engine

---

## Фаза 12: UI Polish

- [ ] Анимации переходов состояний
- [ ] Responsive layout
- [ ] Экспорт/импорт сценариев в JSON
- [ ] Подготовка к графовой БД

---

## Фаза 13: Documentation

- [ ] README.md (актуализация)
- [ ] API documentation (Scalar UI + XML-комментарии)
- [ ] Architecture decision records (ADR) — см. [`adr.md`](./adr.md)
- [ ] Runbooks

---

## Ключевые принципы

1. **Тесты параллельно с кодом**: unit-тесты с domain model, integration-тесты с репозиториями и контроллерами, contract-тесты после стабилизации API
2. **UI по вертикальным слайсам**: базовые карточки → списки → страницы → редактор → polish
3. **Extended health check добавляется по мере появления компонентов**: БД в Фазе 3, остальные — когда понадобятся
4. **n8n-style — только визуальное оформление**, не функциональность графов
5. **Контракты с другими сервисами — когда они появятся** (Фаза 11)
6. **Docker и CI/CD — когда понадобятся**, не сейчас
7. **Result Pattern для всех бизнес-операций**: типобезопасная обработка ошибок без исключений (см. [`result-pattern.md`](./result-pattern.md))
