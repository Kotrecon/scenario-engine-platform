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
- [ ] ProblemDetails layer (стандартный формат ошибок RFC 7807)

---

## CORS

- [ ] Определить список разрешённых origins для production
- [ ] Настроить origins в `appsettings.json`
- [ ] Рассмотреть поддержку нескольких origins
- [ ] Определить необходимость PATCH, OPTIONS, HEAD
- [ ] Решить вопрос с `Access-Control-Allow-Credentials`
- [ ] Определить разрешённые заголовки
- [ ] Настроить `Access-Control-Max-Age` для кэширования preflight
