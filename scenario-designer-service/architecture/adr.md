# ADR — Architecture Decision Records

> Архитектурные решения, принятые в проекте. Каждое решение фиксирует контекст, решение и последствия.

---

## ADR-001: Web SDK вместо Console SDK

**Статус:** Принято

**Контекст:** Проект был начат как консольный (Microsoft.NET.Sdk), но требовался ASP.NET Core для API, controllers, JWT auth.

**Решение:** Переключиться на `Microsoft.NET.Sdk.Web`.

**Последствия:**

- Доступны AddControllers, MapControllers, middleware pipeline
- Kestrel встроен
- Дополнительные пакеты не нужны (`Microsoft.AspNetCore.*` уже в SDK)

---

## ADR-002: Serilog через appsettings.json

**Статус:** Принято

**Контекст:** Serilog можно настраивать программно (WriteTo.Console) или через конфигурацию (ReadFrom.Configuration).

**Решение:** Стандартный формат Serilog в appsettings.json + ReadFrom.Configuration.

**Последствия:**

- Sinks настраиваются через JSON (не код)
- Добавление/удаление sinks без перекомпиляции
- Enricherse конфигурации

---

## ADR-003: LoggingLevelSwitch в DI

**Статус:** Принято

**Контекст:** Уровни логирования должны меняться на лету через API без restart.

**Решение:** LoggingLevelSwitch зарегистрирован как singleton в DI, контроллер получает его через constructor injection.

**Последствия:**

- Динамическое изменение уровней через PUT /api/logging/level
- Нет restart для смены уровня
- Fatal и Verbose запрещены (безопасность)

---

## ADR-004: Policy-based авторизация

**Статус:** Принято

**Контекст:** Нужно разграничить доступ: Admin (изменение), Operator (чтение), Auditor (только чтение).

**Решение:** Policy-based авторизация с тремя политиками: AdminOnly, Operator, AuditViewer.

**Последствия:**

- Гибкость: можно добавить policy без изменения кода контроллера
- Тестируемость: policies можно проверить через IAuthorizationService
- Читаемость: `[Authorize(Policy = "AdminOnly")]` вместо `[Authorize(Roles = "Admin")]`

---

## ADR-005: Health checks на отдельном порту

**Статус:** Принято

**Контекст:** Health checks должны быть доступны для оркестратора (Kubelet, Prometheus), но не доступны извне.

**Решение:** Два Kestrel-эндпоинта: API (8080) и Health (8081). Health-порт не маршрутизируется наружу (firewall / Nginx / Cloud LB).

**Последствия:**

- Middleware (logging, correlation, swagger, CORS) автоматически исключены из health
- Сетевая изоляция вместо секретных заголовков
- Проще настраивать firewall / Nginx

---

## ADR-006: Rate limiting на health-ветку

**Статус:** Принято

**Контекст:** Даже с сетевой изоляцией, при утечке учётных данных или DDoS-атаке health-чеки могут перегрузить БД.

**Решение:** Fixed window limiter: 30 запросов за 10 секунд на `/health/*`.

**Последствия:**

- Дополнительная мера защиты (не основная)
- Kubelet (5-10 сек) + Prometheus (15-30 сек) + 3 эндпоинта = ~5-6 запросов за 10 сек
- Лимит 30 — с запасом

---

## ADR-007: CORS AllowAll для разработки

**Статус:** Принято (временно)

**Контекст:** Фронтенд и API могут работать на разных портах/доменах во время разработки. Нужно разрешить кросс-доменные запросы.

**Решение:** AllowAll политика (`*` origins, `*` methods, `*` headers) для разработки. В production будет ограничена.

**Последствия:**

- Удобно для локальной разработки
- Не безопасно для production (нужно ограничить origins)
- Конфигурация в appsettings.json, политика в CorsExtensions.cs
- TODO: см. [`TODO.md`](./TODO.md) — раздел CORS

---

## ADR-008: Correlation ID через Guid.CreateVersion7

**Статус:** Принято

**Контекст:** Нужно связывать логи и трейсы в рамках одного запроса для отладки и мониторинга.

**Решение:** X-Correlation-Id генерируется через Guid.CreateVersion7 (.NET 10) или прокидывается из входящего заголовка. Добавляется в LogContext (Serilog) и Activity (OpenTelemetry).

**Последствия:**

- Time-ordered UUID (v7) — логи сортируются по времени
- Совместим с OTel trace-id
- Заголовок возвращается в ответе для клиента
- Не работает на health-checks (отдельный порт 8081)

---
