# TODO — Общий файл задач

## Security

- [ ] Secret management (Azure Key Vault / AWS Secrets Manager)
- [ ] Data encryption at rest
- [ ] Data encryption in transit

---

## Infrastructure

- [ ] Dockerfile
- [ ] docker-compose.yml
- [ ] CI/CD pipeline

---

## Operability

### Health Checks — Infrastructure

- [ ] Health-порт не пробрасывается наружу (firewall / Nginx / Cloud LB)
- [ ] Доступ к health-порту только из внутренней сети
- [ ] `terminationGracePeriodSeconds: 30` в Kubernetes deployment
- [ ] Дренаж соединений перед остановкой

### Health Checks — Development

- [ ] `RedisHealthCheck` — проверка Redis
- [ ] `RabbitMqHealthCheck` — проверка RabbitMQ
- [ ] `KafkaHealthCheck` — проверка Kafka
- [ ] `ExternalServiceHealthCheck` — проверка внешних сервисов

### Exception Handler

- [ ] Integration-тест: запрос с ошибкой → JSON-ответ
- [ ] Integration-тест: health check на порту 8081 — без exception handler

### Persistence Layer

- [ ] DbContext
- [ ] Entity configurations
- [ ] Migrations
- [ ] Transaction boundary

### Messaging Layer

- [ ] Publisher/consumer contracts
- [ ] Retry mechanism
- [ ] Idempotency
- [ ] Outbox pattern skeleton

---

## Observability

### Request/Response Logging

- [ ] Логирование Content-Type (если нужен для отладки)
- [ ] Логирование User-Agent (для аудита)
- [ ] Исключения по path (фильтр через конфигурацию)
- [ ] Уровни логирования по типу ошибки (4xx = Warning, 5xx = Error)

### Correlation ID

- [ ] Интеграция с distributed tracing (OpenTelemetry)

### Telemetry

- [ ] Swagger/OpenAPI с JWT-авторизацией

---

## CORS

- [ ] Определить список разрешённых origins для production
- [ ] Настроить origins в `appsettings.json`
- [ ] Рассмотреть поддержку нескольких origins
- [ ] Определить необходимость PATCH, OPTIONS, HEAD
- [ ] Решить вопрос с `Access-Control-Allow-Credentials`
- [ ] Определить разрешённые заголовки
- [ ] Настроить `Access-Control-Max-Age` для кэширования preflight

---

## API

- [ ] API Versioning

---

## Result Pattern Library

> Подробности реализации: [`result-pattern.md`](./result-pattern.md)

### Фаза 1: Асинхронная валидация (приоритет: средний)

- [ ] `IAsyncRule<T>` — интерфейс: `Task<bool> IsSatisfiedAsync(T value)`
- [ ] `AsyncValidator<T>` — принимает список `IAsyncRule<T>`, возвращает `Result`
- [ ] Интеграция с EF Core — правило `UniqueRule<T>` для проверки уникальности
- [ ] Тесты: Mock-БД, проверка async-цепочки

### Фаза 2: Локализация (приоритет: низкий)

- [ ] `ILocalizableError` — интерфейс: `string MessageKey { get; }` вместо `Message`
- [ ] `LocalizedString` — обёртка над `IStringLocalizer` для резолва ключей
- [ ] Обратная совместимость — существующие ошибки реализуют оба интерфейса

### Фаза 3: Расширенные типы ошибок

- [ ] `RateLimitError` — HTTP 429 + `Retry-After` заголовок
- [ ] `TimeoutError` — HTTP 504 для долгих операций
- [ ] `UnauthorizedError` — HTTP 401 (отличие от `ForbiddenError` 403)

### Фаза 4: NuGet-пакет

- [ ] `.csproj` с метаданными — Version, Authors, Description, PackageTags
- [ ] strong-naming — поддержка signed assemblies
- [ ] SourceLink — отладка через NuGet
- [ ] Публикация — nuget.org или приватный feed

---

## Связанные документы

- [`operability.md`](./operability.md)
- [`observability.md`](./observability.md)
- [`result-pattern.md`](./result-pattern.md)
- [`api.md`](./api.md)
