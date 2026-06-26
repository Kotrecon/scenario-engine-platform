# TODO — Общий файл задач

## CORS

### Текущая политика

- Origins: `*` (все источники) — для разработки
- Methods: GET, POST, PUT, DELETE
- Credentials: не настроены

### Необходимо решить

- [ ] Определить список разрешенных origins для production
- [ ] Настроить origins в appsettings.json
- [ ] Рассмотреть поддержку нескольких origins
- [ ] Определить необходимости PATCH, OPTIONS, HEAD
- [ ] Решить вопрос с Access-Control-Allow-Credentials
- [ ] Определить разрешенные заголовки
- [ ] Настроить Access-Control-Max-Age для кэширования preflight

## Безопасность

- [ ] Secret management (Azure Key Vault / AWS Secrets Manager)
- [ ] Data encryption at rest
- [ ] Data encryption in transit

## Инфраструктура

- [ ] Dockerfile
- [ ] docker-compose.yml
- [ ] CI/CD pipeline
