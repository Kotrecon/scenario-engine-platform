# База данных — Текущее состояние

## Текущий статус

База данных пока не подключена. Хранилище — in-memory (пока нет EF Core).

## План подключения

### Фаза 4: Data & Storage

- [ ] Database (EF Core)
- [ ] Migrations
- [ ] Repository pattern
- [ ] Caching (Redis)
- [ ] Distributed cache

## Сущности (планируемые)

| Сущность        | Описание            | Ключевые поля                                        |
| --------------- | ------------------- | ---------------------------------------------------- |
| Scenario        | Сценарий оповещения | ScenarioId, Name, Priority, Status, Version          |
| Trigger         | Триггер             | TriggerId, Name, EventCode, Condition, Justification |
| ScenarioAction  | Действие сценария   | ActionId, ScenarioId, StepOrder, ActionType          |
| ScenarioTrigger | Связь M:N           | ScenarioId, TriggerId                                |

## Схема (планируемая)

```bash
Scenario (1) ←→ (M) ScenarioAction
Scenario (M) ←→ (M) Trigger (через ScenarioTrigger)
```

## Миграции

Миграции пока не созданы. Будут созданы после подключения EF Core.

## Кэширование

Redis пока не подключен. Планируется в Фазе 4.

## Резервное копирование

Пока не настроено. Планируется после подключения БД.
