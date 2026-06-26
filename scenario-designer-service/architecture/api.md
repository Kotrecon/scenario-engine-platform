# API — Текущее состояние

> Документ описывает публичные и внутренние API-эндпоинты сервиса, их аутентификацию, форматы запросов/ответов и коды ошибок.
>
> Детали реализации health checks — в [`operability.md`](./operability.md).

---

## Общие принципы

### Аутентификация

**API (порт 8080):**
Все эндпоинты требуют JWT Bearer токен в заголовке:

```bash
Authorization: Bearer <token>
```

**Health Checks (порт 8081):**
Аутентификация не требуется. Защита — сетевая изоляция (порт не маршрутизируется наружу).

### Корреляция

Заголовок `X-Correlation-Id` автоматически генерируется (`Guid.CreateVersion7`) или прокидывается из входящего запроса. Доступен в логах и OTel-трейсах.

### Версионирование

Версионирование API пока не реализовано. Планируется в Фазе 2.

---

## Эндпоинты

### API (порт 8080)

| Метод | Роут                      | Описание                     | Доступ      |
| ----- | ------------------------- | ---------------------------- | ----------- |
| GET   | `/api/logging/level`      | Текущие уровни логирования   | AuditViewer |
| PUT   | `/api/logging/level`      | Изменить уровень логирования | AdminOnly   |
| GET   | `/api/logging/categories` | Список категорий с overrides | AuditViewer |

### Health Checks (порт 8081 — внутренний)

| Метод | Роут            | Описание               | Доступ     |
| ----- | --------------- | ---------------------- | ---------- |
| GET   | `/health/live`  | Процесс жив            | Внутренний |
| GET   | `/health/ready` | Готов принимать трафик | Внутренний |
| GET   | `/health`       | Агрегированный ответ   | Внутренний |

---

## Logging API

### PUT /api/logging/level

Изменяет уровень логирования для категории или root level.

**Запрос:**

```json
{
  "category": "Microsoft.AspNetCore",
  "level": "Warning"
}
```

| Поле     | Тип    | Обязательно | Описание                      |
| -------- | ------ | ----------- | ----------------------------- |
| category | string | Нет         | Категория (null = root level) |
| level    | string | Да          | Уровень логирования           |

**Допустимые уровни:** Debug, Information, Warning, Error  
**Запрещённые уровни:** Fatal, Verbose (безопасность)

**Ответы:**

- `200 OK` — уровень изменён:

  ```json
  {
    "message": "Log level updated"
  }
  ```

- `400 Bad Request` — невалидный уровень:

  ```json
  {
    "error": "Invalid level: Critical"
  }
  ```

- `404 Not Found` — категория не найдена:

  ```json
  {
    "error": "Category NonExistent not found"
  }
  ```

### GET /api/logging/level

Возвращает текущие уровни логирования.

**Ответ (200 OK):**

```json
{
  "default": "Information",
  "overrides": {
    "Microsoft": "Warning",
    "System": "Warning"
  }
}
```

### GET /api/logging/categories

Возвращает список категорий с overrides.

**Ответ (200 OK):**

```json
["Microsoft", "System"]
```

---

## Health Checks API

> Детали реализации (что проверяет каждый эндпоинт, graceful shutdown, rate limiting) — в [`operability.md`](./operability.md).

### Запрос

Все эндпоинты принимают GET-запросы без тела:

```bash
GET /health/live
GET /health/ready
GET /health
```

### Ответы

**200 OK (Healthy):**

```json
{
  "status": "Healthy"
}
```

**503 Service Unavailable (Unhealthy):**

```json
{
  "status": "Unhealthy"
}
```

**429 Too Many Requests (Rate Limit):**

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.16",
  "title": "Too Many Requests",
  "status": 429
}
```

### Коды ответа

| Код | Описание                               |
| --- | -------------------------------------- |
| 200 | Процесс жив / зависимости готовы       |
| 429 | Rate limit превышен (30 req/10s)       |
| 503 | Процесс завис / зависимости недоступны |

---

## Коды ошибок (общие)

| Код | Описание                       |
| --- | ------------------------------ |
| 400 | Невалидный уровень логирования |
| 401 | Не аутентифицирован            |
| 403 | Нет прав (не Admin для PUT)    |
| 404 | Категория не найдена           |
| 429 | Rate limit превышен            |
| 503 | Сервис недоступен (health)     |

---

## Связанные документы

- [`operability.md`](./operability.md) — Health Checks, Graceful Shutdown, Rate Limiting, Exception Handler
- [`observability.md`](./observability.md) — Логирование, OpenTelemetry
- [`architecture.md`](./architecture.md) — Стек технологий, структура проекта, безопасность
