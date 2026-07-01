# Scenario Designer Service

Визуальный редактор сценариев оповещения для системы ситуационного управления (ЛСО).  
Часть платформы **Scenario Engine Platform** для критической инфраструктуры.

![ASP.NET](https://img.shields.io/badge/ASP.NET-10-512BD4?style=flat&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-13-239120?style=flat&logo=csharp)
![JavaScript](https://img.shields.io/badge/JavaScript-ES2024-F7DF1E?style=flat&logo=javascript)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-18-4169E1?style=flat&logo=postgresql)
![HTML5](https://img.shields.io/badge/HTML5-E34F26?style=flat&logo=html5)
![CSS3](https://img.shields.io/badge/CSS3-1572B6?style=flat&logo=css3)

**GitHub:** `https://github.com/Kotrecon/scenario-engine-platform`

---

## Что это

Scenario Designer — это бэкенд-сервис для управления сценариями оповещения на критической инфраструктуре. Позволяет:

- Создавать и редактировать сценарии оповещения
- Управлять устройствами и зонами оповещения
- Настраивать политики уведомлений
- Мониторить состояние системы в реальном времени

Соответствует требованиям ГОСТ Р 22.7.05-2022, ISA-18.2, EEMUA 191.

---

## Быстрый старт

```bash
cd backend
dotnet restore
dotnet build
dotnet run
```

API: `http://localhost:8080`  
Health checks: `http://localhost:8081` (внутренняя сеть)

---

## Документация

- [Архитектура](./architecture/architecture.md) — Стек, структура проекта, регистрация сервисов
- [Operability](./architecture/operability.md) — Health Checks, Graceful Shutdown, Rate Limiting, CORS, Exception Handler
- [Observability](./architecture/observability.md) — Логирование, Request/Response Logging, OpenTelemetry
- [API](./architecture/api.md) — Эндпоинты, форматы, коды ошибок
- [ADR](./architecture/adr.md) — Architecture Decision Records
- [TODO](./architecture/TODO.md) — Все незавершённые задачи
- [План тестирования](./architecture/testing.md) — Статус тестов, покрытие

---

## Стандарты

- **ISA-18.2** — Alarm Management
- **ISA-88** — Procedure Control
- **EEMUA 191** — Alarm Systems Guide
- **IEC 62443** — Industrial Cybersecurity
- **ГОСТ Р 22.7.05-2022** — Требования ЛСО
- **ГОСТ Р 42.3.01-2021** — Требования к устройствам
- **СП 484.1311500.2020** — Нормы проектирования систем оповещения

---

## Автор

**[@Kotrecon](https://github.com/Kotrecon)**

Архитектор решений из Санкт-Петербурга. Специализация: .NET, C#, JS, Python, AI/ML, RAG, Агенты, DevOps, GitHub, GitLab, CI/CD, АСУ ТП, промышленное ПО, DB, PostgreSQL.  
[Telegram](https://t.me/Kotrecon) | [Email](mailto:ermakov_k@mail.ru)

---

## Лицензия

MIT
