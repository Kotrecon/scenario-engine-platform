# 📋 Шпаргалка: Валидация DTO в .NET

## Цель

Понять, **что именно тестировать** у разных типов DTO в .NET, чем отличаются **Request DTO**, **Response DTO** и **Options DTO / Configuration DTO**, и где должна жить валидация.

---

## Главная идея

- **Request DTO** — валидирует входной запрос до попадания в сервис.
- **Response DTO** — проверяется через контракт ответа и интеграционные тесты.
- **Options DTO / Configuration DTO** — описывает настройки приложения и валидируется при биндинге конфигурации и на старте приложения.
- **Бизнес-правила** не должны жить в DTO: они принадлежат domain/application layer.

---

## Типы DTO

| Тип DTO                         | Назначение                           | Где живёт                      |
| ------------------------------- | ------------------------------------ | ------------------------------ |
| Request DTO                     | Входные данные API                   | Presentation / API boundary    |
| Response DTO                    | Выходной контракт API                | Presentation / API boundary    |
| Options DTO / Configuration DTO | Настройки приложения из конфигурации | Infrastructure / Configuration |

---

## Таблица 1: Где и когда запускать проверки

| Место / Триггер         | Что проверяем                             | Инструмент                                  | Статус валидации                             |
| ----------------------- | ----------------------------------------- | ------------------------------------------- | -------------------------------------------- |
| Локальная разработка    | DTO и базовые правила входных данных      | DataAnnotations / FluentValidation          | ✅ Обязательно перед коммитом                |
| Unit-тесты              | Правила валидации DTO                     | Тесты валидатора                            | ✅ Обязательно для request DTO и options DTO |
| Integration / API-тесты | DTO на границе контроллера, JSON-контракт | Реальный HTTP-запрос + assert ответа        | ✅ Обязательно для публичных контрактов      |
| CI / PR                 | Валидаторы, контрактные DTO, регрессия    | Автотесты                                   | 🚫 Блок мерджа при падении                   |
| Runtime: Dev            | Входной запрос до попадания в сервис      | ASP.NET Core validation pipeline            | ⚙️ Включено                                  |
| Runtime: Prod           | Входной запрос до попадания в сервис      | ASP.NET Core validation pipeline            | ⚙️ Включено                                  |
| После маппинга          | Доменные правила                          | Доменные валидаторы / бизнес-правила        | ✅ Отдельно от DTO                           |
| Startup / Host build    | Корректность конфигурации приложения      | `ValidateOnStart()` / `IValidateOptions<T>` | ✅ Для Options DTO                           |

---

## Таблица 2: Что и чем проверять

| Объект проверки                 | Инструмент                                                              | Что ловит                                                      | Что не ловит                     |
| ------------------------------- | ----------------------------------------------------------------------- | -------------------------------------------------------------- | -------------------------------- |
| Форма DTO                       | DataAnnotations / FluentValidation                                      | `Required`, длины, диапазоны, формат, enum, nullable           | Бизнес-смысл                     |
| JSON-контракт                   | Integration tests                                                       | Имена полей, типы, обязательность, сериализация                | Доменные ограничения             |
| Бизнес-правила                  | Доменные валидаторы / сервис                                            | Логика уровня приложения, зависимости между полями             | Ошибки формата                   |
| Маппинг DTO → command/entity    | Unit tests                                                              | Потерю/искажение полей                                         | Правила JSON и HTTP              |
| Ответ API                       | Integration tests                                                       | Правильный response shape                                      | Внутреннюю доменную логику       |
| Options DTO / Configuration DTO | `ValidateDataAnnotations()`, `ValidateOnStart()`, `IValidateOptions<T>` | Ошибки конфигурации, обязательные поля, формат, nested binding | JSON-контракт API, бизнес-логику |

---

## Таблица 3: Что тестировать в DTO

| Тип DTO                         | Что тестировать                        | Как                                           |
| ------------------------------- | -------------------------------------- | --------------------------------------------- |
| Request DTO                     | Валидацию входных данных               | Unit-тесты валидатора + API-тест              |
| Response DTO                    | Форму ответа и сериализацию            | Integration-тесты                             |
| DTO с enum                      | Допустимые значения                    | Unit-тесты + контракт ответа                  |
| DTO с nullable-полями           | Поведение при `null` и пустых строках  | Unit-тесты валидатора                         |
| DTO для update                  | Частично заполненные поля              | Отдельные кейсы на partial update             |
| Options DTO / Configuration DTO | Binding, validation, startup fail-fast | Unit-тесты + integration test на host startup |

---

## Таблица 4: Когда валидатор нужен

| Ситуация                                       | Валидатор нужен? | Почему                                     |
| ---------------------------------------------- | ---------------- | ------------------------------------------ |
| DTO принимает входной запрос                   | ✅ Да            | Нужно проверить корректность данных        |
| Есть правила, отличные от `Required` и `Range` | ✅ Да            | DataAnnotations уже недостаточно           |
| DTO только выходит из API                      | ❌ Обычно нет    | Там важнее контракт и сериализация         |
| Поля зависят друг от друга                     | ✅ Да            | Нужна логика, а не только атрибуты         |
| DTO используется в нескольких сценариях        | ✅ Да            | У каждого сценария могут быть свои правила |
| DTO описывает конфигурацию приложения          | ✅ Да            | Нужно ловить misconfiguration до запуска   |

---

## Что лучше считать best practice

- DTO должен быть **тупым контейнером данных**.
- Валидация должна быть **отдельно от DTO**, если правила не тривиальные.
- Request DTO валидируется **до** попадания в сервис.
- Response DTO проверяется **через API-контракт**, а не через тесты геттеров.
- Options DTO валидируется **при биндинге конфигурации и на старте**.
- Если правило относится к бизнесу, оно должно жить **не в DTO**, а в domain/application layer.

---

## Золотое правило

**DTO отвечает за форму данных.**  
**Валидатор отвечает за корректность формы.**  
**Domain/Application слой отвечает за смысл.**  
**Options DTO отвечает за конфигурацию приложения.**

---

## DTO/Request — шаблон проверки

- [ ] Обязательные поля не могут быть null/empty.
- [ ] Строки имеют правильную длину (min/max).
- [ ] Числа находятся в допустимом диапазоне.
- [ ] Email, URL, GUID — корректного формата.
- [ ] Имена полей соответствуют контракту (camelCase).
- [ ] Null значения обрабатываются правильно.
- [ ] Enum сериализуется как строка/число.
- [ ] DateTime форматируется правильно (ISO 8601).

---

## Options DTO / Configuration DTO — шаблон проверки

- [ ] Обязательные настройки присутствуют в конфигурации.
- [ ] Формат строк корректный.
- [ ] Вложенные объекты корректно бинсятся.
- [ ] `ValidateDataAnnotations()` проходит без ошибок.
- [ ] `ValidateOnStart()` падает при неверной конфигурации.
- [ ] Кастомные cross-field rules работают.
- [ ] Нет silent misconfiguration.
- [ ] Значения по умолчанию безопасны.

---

## Примеры

### Request DTO

```csharp
public sealed record CreateDeviceRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d+$")]
    public string ExternalId { get; init; } = string.Empty;
}
```

### Response DTO

```csharp
public sealed record DeviceResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}
```

### Options DTO / Configuration DTO

```csharp
public sealed record ApiMetadataOptions
{
    public const string SectionName = "ApiMetadata";

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; init; } = "Scenario Designer API";

    [Required]
    [RegularExpression(@"^\d+\.\d+\.\d+(-[\w.]+)?$")]
    public string Version { get; init; } = "1.0.0";

    [Required]
    [StringLength(500, MinimumLength = 10)]
    public string Description { get; init; } = "API для управления сценариями оповещения";

    public ContactInfo Developer { get; init; } = new();
}
```

---

## Что важно запомнить

- **Request DTO** тестируется как вход.
- **Response DTO** тестируется как контракт выхода.
- **Options DTO** тестируется как конфигурация.
- **Бизнес-валидация** не должна жить в DTO.
- **Startup validation** — обязательна для конфигурационных моделей.

---

## Золотое правило

**DTO отвечает за форму данных.**  
**Валидатор отвечает за корректность формы.**  
**Domain/Application слой отвечает за смысл.**  
**Options DTO отвечает за конфигурацию приложения.**

---

## DTO/Request — шаблон проверки

- [ ] Обязательные поля не могут быть null/empty.
- [ ] Строки имеют правильную длину (min/max).
- [ ] Числа находятся в допустимом диапазоне.
- [ ] Email, URL, GUID — корректного формата.
- [ ] Имена полей соответствуют контракту (camelCase).
- [ ] Null значения обрабатываются правильно.
- [ ] Enum сериализуется как строка/число.
- [ ] DateTime форматируется правильно (ISO 8601).

---

## Options DTO / Configuration DTO — шаблон проверки

- [ ] Обязательные настройки присутствуют в конфигурации.
- [ ] Формат строк корректный.
- [ ] Вложенные объекты корректно бинсятся.
- [ ] `ValidateDataAnnotations()` проходит без ошибок.
- [ ] `ValidateOnStart()` падает при неверной конфигурации.
- [ ] Кастомные cross-field rules работают.
- [ ] Нет silent misconfiguration.
- [ ] Значения по умолчанию безопасны.

---

## Примеры

### Request DTO

```csharp
public sealed record CreateDeviceRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d+$")]
    public string ExternalId { get; init; } = string.Empty;
}
```

### Response DTO

```csharp
public sealed record DeviceResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}
```

### Options DTO / Configuration DTO

```csharp
public sealed record ApiMetadataOptions
{
    public const string SectionName = "ApiMetadata";

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; init; } = "Scenario Designer API";

    [Required]
    [RegularExpression(@"^\d+\.\d+\.\d+(-[\w.]+)?$")]
    public string Version { get; init; } = "1.0.0";

    [Required]
    [StringLength(500, MinimumLength = 10)]
    public string Description { get; init; } = "API для управления сценариями оповещения";

    public ContactInfo Developer { get; init; } = new();
}
```

---

## Что важно запомнить

- **Request DTO** тестируется как вход.
- **Response DTO** тестируется как контракт выхода.
- **Options DTO** тестируется как конфигурация.
- **Бизнес-валидация** не должна жить в DTO.
- **Startup validation** — обязательна для конфигурационных моделей.
