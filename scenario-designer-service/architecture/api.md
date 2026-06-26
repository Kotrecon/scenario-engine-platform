# API — Текущее состояние

## Эндпоинты

### API (порт 8080)

| Метод | Роут                      | Описание                     | Доступ      |
| ----- | ------------------------- | ---------------------------- | ----------- |
| GET   | `/api/logging/level`      | Текущие уровни логирования   | AuditViewer |
| PUT   | `/api/logging/level`      | Изменить уровень логирования | AdminOnly   |
| GET   | `/api/logging/categories` | Список категорий с overrides | AuditViewer |

### Health Checks (порт 8081 — внутренний)

| Роут            | Описание               | Доступ     |
| --------------- | ---------------------- | ---------- |
| `/health/live`  | Процесс жив            | Внутренний |
| `/health/ready` | Готов принимать трафик | Внутренний |
| `/health`       | Агрегированный ответ   | Внутренний |

---

## Аутентификация

### API (порт 8080)

Все эндпоинты требуют JWT Bearer токен в заголовке:

```bash
Authorization: Bearer <token>
```

### Health Checks (порт 8081)

Аутентификация не требуется. Защита — сетевая изоляция (порт не маршрутизируется наружу).

---

## Logging API

### Запрос

#### PUT /api/logging/level

```json
{
  "category": "Microsoft.AspNetCore",
  "level": "Warning"
}
```

| Поле      | Тип    | Обязательно | Описание                        |
| --------- | ------ | ----------- | ------------------------------- |
| category  | string | Нет         | Категория (null = root level)   |
| level     | string | Да          | Уровень логирования             |

**Допустимые уровни:** Debug, Information, Warning, Error

**Запрещённые уровни:** Fatal, Verbose

### Ответы

#### GET /api/logging/level — 200 OK

```json
{
  "default": "Information",
  "overrides": {
    "Microsoft": "Warning",
    "System": "Warning"
  }
}
```

#### GET /api/logging/categories — 200 OK

```json
["Microsoft", "System"]
```

#### PUT /api/logging/level — 200 OK

```json
{
  "message": "Log level updated"
}
```

#### PUT /api/logging/level — 400 Bad Request

```json
{
  "error": "Invalid level: Critical"
}
```

#### PUT /api/logging/level — 404 Not Found

```json
{
  "error": "Category NonExistent not found"
}
```

---

## Health Checks API

### Запрос

Все эндпоинты принимают GET-запросы без тела.

```bash
GET /health/live
GET /health/ready
GET /health
```

### Ответы

#### 200 OK (Healthy)

```json
{
  "status": "Healthy"
}
```

#### 503 Service Unavailable (Unhealthy)

```json
{
  "status": "Unhealthy"
}
```

#### 429 Too Many Requests (Rate Limit)

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.16",
  "title": "Too Many Requests",
  "status": 429
}
```

### Коды ответа

| Код  | Описание                                  |
| ---- | ----------------------------------------- |
| 200  | Процесс жив / зависимости готовы          |
| 429  | Rate limit превышен (30 req/10s)          |
| 503  | Процесс завис / зависимости недоступны    |

---

## Коды ошибок (общие)

| Код  | Описание                       |
| ---- | ------------------------------ |
| 400  | Невалидный уровень логирования |
| 401  | Не аутентифицирован            |
| 403  | Нет прав (не Admin для PUT)    |
| 404  | Категория не найдена           |
| 429  | Rate limit превышен            |
| 503  | Сервис недоступен (health)     |

---

## Версионирование

Версионирование API пока не реализовано. Планируется в Фазе 2.

## Корреляция

Correlation ID middleware пока не реализован. Планируется в Фазе 0.
