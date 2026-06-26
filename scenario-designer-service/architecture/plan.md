# План разработки Scenario Designer Service

## Фаза 0: Backend Infrastructure

- [x] Health checks (подробнее: `architecture/operability .md`)
- [x] Global exception handler middleware
- [x] Request/Response logging middleware
- [x] Correlation ID middleware
- [x] CORS policy
- [ ] API Versioning
- [ ] Swagger/OpenAPI с JWT-авторизацией

## Фаза 1: UI Kit — адаптация

- [ ] Адаптация дизайн-системы под проект
- [ ] Цветовая схема под EEMUA 191
- [ ] Тёмная тема (#0a0c10 bg, #111318 panel)
- [ ] Базовые компоненты: кнопки, инпуты, таблицы, модалки

## Фаза 2: Domain Layer

- [ ] Энумы: `TriggerStatus`, `ScenarioStatus`, `ExecutionMode`, `AutoFallbackAction`, `ActionType`
- [ ] Сущность `Trigger` + валидатор + unit-тесты
- [ ] Сущность `Scenario` + валидатор + unit-тесты
- [ ] Сущность `ScenarioAction` + валидатор + unit-тесты
- [ ] Связь M:N `ScenarioTrigger` + валидатор + unit-тесты
- [ ] UI: базовые карточки для отображения сущностей

## Фаза 3: Data & Storage

- [ ] Database (EF Core) + migrations
- [ ] Repository pattern + integration-тесты
- [ ] Redis cache
- [ ] Extended health check: БД
- [ ] UI: список сценариев, список триггеров

## Фаза 4: Application Services

- [ ] `IScenarioService` — CRUD + lifecycle + тесты
- [ ] `ITriggerService` — CRUD + валидация + тесты
- [ ] `IScenarioActionService` — CRUD + тесты
- [ ] State machine для связей (подготовка к n8n-style)

## Фаза 5: API Layer

- [ ] DTO + unit-тесты валидации
- [ ] `ScenariosController` + integration-тесты
- [ ] `TriggersController` + integration-тесты
- [ ] `ScenarioActionsController` + integration-тесты
- [ ] `AuthController` — login, register, refresh, logout
- [ ] `AdminController` — дашборды, настройки пользователей
- [ ] UI: страница входа/регистрации
- [ ] UI: админ-дашборд (метрики, KPI, список пользователей)

## Фаза 6: UI Editor (n8n-style оформление)

- [ ] Визуальный редактор сценариев в стиле n8n (только оформление)
- [ ] Карточки для триггеров и условий (визуальный стиль)
- [ ] Валидация в реальном времени
- [ ] Contract-тесты API (Pact)

## Фаза 7: Resilience & Security

- [ ] Circuit breaker (Polly)
- [ ] Retry policies (Polly)
- [ ] Timeout policies
- [ ] Rate limiting
- [ ] Secret management
- [ ] Audit logging
- [ ] Data encryption
- [ ] Load tests (k6)

## Фаза 8: Messaging & Events

- [ ] Message bus (RabbitMQ / Kafka)
- [ ] Pub/Sub pattern
- [ ] Event-driven architecture
- [ ] Outbox pattern

## Фаза 9: Background Jobs

- [ ] Scheduled tasks
- [ ] Background workers

## Фаза 10: Observability

- [ ] Кастомные метрики (Counter: scenarios_created, scenarios_executed)
- [ ] Histogram для времени выполнения сценариев
- [ ] Metrics dashboards (Grafana)
- [ ] Log aggregation (ELK / Loki)
- [ ] Alerting (Prometheus Alertmanager)

## Фаза 11: Integration (когда появятся сервисы)

- [ ] Контракты для Scenario Registry
- [ ] Контракты для Scenario Engine

## Фаза 12: UI Polish

- [ ] Анимации переходов состояний
- [ ] Responsive layout
- [ ] Экспорт/импорт сценариев в JSON
- [ ] Подготовка к графовой БД

## Фаза 13: Documentation

- [ ] README.md
- [ ] API documentation
- [ ] Architecture decision records (ADR)
- [ ] Runbooks

## Ключевые принципы

1. **Тесты параллельно с кодом**: unit-тесты с domain model, integration-тесты с репозиториями и контроллерами, contract-тесты после стабилизации API
2. **UI по вертикальным слайсам**: базовые карточки → списки → страницы → редактор → polish
3. **Extended health check добавляется по мере появления компонентов**: БД в Фазе 3, остальные — когда понадобятся
4. **n8n-style — только визуальное оформление**, не функциональность графов
5. **Контракты с другими сервисами — когда они появятся** (Фаза 11)
6. **Docker и CI/CD — когда понадобятся**, не сейчас
