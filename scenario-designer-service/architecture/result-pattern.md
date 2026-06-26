# Result Pattern — Типобезопасная обработка результатов

> Документ описывает библиотеку `Result` для функциональной обработки бизнес-ошибок без исключений.
>
> - HTTP-контракты (ProblemDetails) — см. [`api.md`](./api.md)
> - Как ошибки логируются — см. [`observability.md`](./observability.md)

---

## Концепция

Бизнес-ошибки — не исключения. Успех и ошибка — равноправные результаты, которые компилятор заставляет обработать.

### Традиционный подход

```csharp
public User GetUser(Guid id)
{
    if (id == Guid.Empty)
        throw new ArgumentException("Invalid ID"); // ← Скрытый контракт!
    // ...
}
```

**Проблемы:**

- Компилятор не контролирует обработку ошибок
- Исключения дороги (стеко-трейс, аллокации)
- Тесты проверяют текст сообщения, а не тип ошибки
- Контроллеры засорены `try/catch`

### Подход через Result

```csharp
public Task<Result<User>> GetUserAsync(Guid id);

var result = await service.GetUserAsync(id);
return result.Match(
    onSuccess: user => Ok(user),
    onFailure: error => error.ToActionResult() // 400/404/422
);
```

**Преимущества:**

- **Типобезопасность**: компилятор напомнит обработать ошибку
- **Читаемость**: сигнатура документирует возможные исходы
- **Тестируемость**: проверяем `error is NotFoundError`, а не парсим текст
- **Чистые контроллеры**: одна строка `return result.ToActionResult()`
- **RFC 7807**: ответы совместимы с `ProblemDetails`

---

## Компоненты библиотеки

### Ядро (`Contracts/Result/Common/`)

| Тип                 | Назначение                                                                              |
| ------------------- | --------------------------------------------------------------------------------------- |
| `Result`            | Результат операции без возвращаемого значения                                           |
| `Result<T>`         | Результат операции с возвращаемым значением `T`                                         |
| `IError`            | Интерфейс бизнес-ошибки (`Message`, `Code`, `StatusCode`)                               |
| `ValidationError`   | Ошибка валидации с деталями по полям (`Details: IReadOnlyDictionary<string, string[]>`) |
| `NotFoundError`     | Ресурс не найден (404)                                                                  |
| `ConflictError`     | Конфликт ресурсов (409)                                                                 |
| `ForbiddenError`    | Доступ запрещён (403)                                                                   |
| `BusinessRuleError` | Нарушение бизнес-правила (400)                                                          |

### Адаптеры (`Contracts/Result/Web/`)

| Тип                | Назначение                                                                            |
| ------------------ | ------------------------------------------------------------------------------------- |
| `ResultExtensions` | Расширения `.ToActionResult()` для маппинга `Result` → `IActionResult` (ASP.NET Core) |

---

## Интерфейс `IError`

```csharp
public interface IError
{
    string Message { get; }   // Человекочитаемое (логи, UI)
    string Code { get; }      // Машинно-читаемое (switch, клиенты)
    int StatusCode { get; }   // HTTP-статус
}
```

---

## Формат ошибок валидации: `field:code`

`ValidationError` принимает строки вида `"fieldName:errorCode"` и автоматически парсит их в структурированный словарь:

```csharp
new ValidationError("Данные не прошли валидацию", new[]
{
    "email:required",
    "email:invalidFormat",
    "password:minLength"
});
// → Details = { "email": ["required", "invalidFormat"], "password": ["minLength"] }
```

**Почему так:** это минимальный «протокол» для передачи структурированной ошибки через границу слоёв.

---

## Стандартные коды ошибок

| Код                     | HTTP | Когда использовать              | Пример                        |
| ----------------------- | ---- | ------------------------------- | ----------------------------- |
| `required`              | 422  | Поле обязательно, но пустое     | `email:required`              |
| `minLength`             | 422  | Длина строки меньше минимума    | `password:minLength`          |
| `maxLength`             | 422  | Длина строки больше максимума   | `name:maxLength`              |
| `range`                 | 422  | Число вне допустимого диапазона | `age:range`                   |
| `invalidFormat`         | 422  | Не соответствует формату        | `email:invalidFormat`         |
| `notFound`              | 404  | Сущность не найдена             | `user:notFound`               |
| `conflict`              | 409  | Уникальное поле уже занято      | `email:conflict`              |
| `forbidden`             | 403  | Недостаточно прав               | `resource:forbidden`          |
| `validationFailed`      | 422  | Общая ошибка валидации          | `form:validationFailed`       |
| `businessRuleViolation` | 400  | Нарушение бизнес-логики         | `order:businessRuleViolation` |

---

## Маппинг ошибок → HTTP-ответы

| Тип ошибки          | StatusCode | Code                  |
| ------------------- | ---------- | --------------------- |
| `ValidationError`   | 422        | ValidationFailed      |
| `NotFoundError`     | 404        | NotFound              |
| `ConflictError`     | 409        | Conflict              |
| `ForbiddenError`    | 403        | Forbidden             |
| `BusinessRuleError` | 400        | BusinessRuleViolation |

### Формат ответа (ProblemDetails, RFC 7807)

**Успех (200):**

```json
{
  "value": { ... }
}
```

**Ошибка (400/404/409/422):**

```json
{
  "type": "https://httpstatuses.com/422",
  "title": "ValidationFailed",
  "status": 422,
  "detail": "Данные не прошли валидацию",
  "errors": [
    { "code": "required", "message": "Email обязателен" },
    { "code": "minLength", "message": "Пароль слишком короткий" }
  ],
  "validationErrors": {
    "email": ["required"],
    "password": ["minLength"]
  }
}
```

### Структура extensions

| Поле               | Тип                            | Описание                                      |
| ------------------ | ------------------------------ | --------------------------------------------- |
| `errors`           | `Array<{code, message}>`       | Все ошибки операции (универсальный формат)    |
| `validationErrors` | `Dictionary<string, string[]>` | Только для `ValidationError`, детали по полям |

---

## Функциональные цепочки

```csharp
GetUserById(id)
    .Bind(user => user.IsActive
        ? Result<User>.Success(user)
        : Result<User>.Failure(new ForbiddenError()))
    .Map(user => user.Name)
    .OnSuccess(name => Console.WriteLine($"Hello, {name}!"))
    .OnFailure(error => logger.Warn("Operation failed: {Code}", error.Code));
```

---

## Полевые валидаторы (рекомендуемый паттерн)

Для переиспользуемой валидации выносите правила в отдельные статические классы:

```csharp
// Validators/EmailValidator.cs
public static class EmailValidator
{
    public static List<IError> Validate(string? email, string fieldName = "email")
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(email))
            errors.Add(new ValidationError("Email обязателен", new[] { $"{fieldName}:required" }));
        else if (!IsValidFormat(email))
            errors.Add(new ValidationError("Неверный формат", new[] { $"{fieldName}:invalidFormat" }));

        return errors;
    }
}

// Использование в валидаторе DTO
public Result Validate(CreateUserRequest request)
{
    var errors = new List<IError>();
    errors.AddRange(EmailValidator.Validate(request.Email));
    errors.AddRange(PasswordValidator.Validate(request.Password));
    return errors.Any() ? Result.Failure(errors) : Result.Success();
}
```

**Преимущества:**

- Одно правило — много точек применения
- Легко тестировать изолированно
- Чистые DTO (без атрибутов)
- Готово к миграции правил в конфигурацию (`appsettings.json`)

---

## Шпаргалка

| Операция           | Код                                                  |
| ------------------ | ---------------------------------------------------- |
| Успех              | `Result<T>.Success(value)`                           |
| Ошибка (одна)      | `Result<T>.Failure(new NotFoundError("Entity", id))` |
| Ошибка (несколько) | `Result<T>.Failure(new[] { error1, error2 })`        |
| Цепочка            | `.Bind(x => NextOperation(x))`                       |
| Преобразование     | `.Map(x => x.Property)`                              |
| В контроллере      | `return result.ToActionResult();`                    |
| Обработка          | `result.Match(onSuccess: ..., onFailure: ...)`       |
| Конвертация void→T | `voidResult.ToResult<T>(value)`                      |

---

## Файлы

- `Contracts/Result/Common/*.cs` — ядро библиотеки
- `Contracts/Result/Web/ResultExtensions.cs` — web-адаптер

---

## Тесты (47)

| Группа             | Тестов | Описание                                                    |
| ------------------ | ------ | ----------------------------------------------------------- |
| `Result`           | 10     | Success, Failure, OnSuccess, OnFailure, ToResult            |
| `Result<T>`        | 14     | Success, Failure, Map, Bind, OnSuccess, OnFailure, ToResult |
| Ошибки             | 10     | Properties, ValidationError парсинг                         |
| `ResultExtensions` | 13     | ToActionResult, ProblemDetails формат, extensions           |

---

## Интеграция с экосистемой

| Технология         | Как интегрировать                                                                                                       |
| ------------------ | ----------------------------------------------------------------------------------------------------------------------- |
| **ASP.NET Core**   | `ResultExtensions.ToActionResult()` + `ProblemDetails`                                                                  |
| **Serilog**        | Логировать `error.Code` и `error.Message` как структурированные свойства (см. [`observability.md`](./observability.md)) |
| **OpenAPI/Scalar** | Документировать ответы `422` со схемой `ProblemDetails`                                                                 |
| **Тестирование**   | Мокировать зависимости, проверять `result.IsSuccess` и `result.Errors`                                                  |

---

## Исключения

- Health checks на порту 8081 — **НЕ обрабатываются** (отдельный pipeline).
- `GetLevel()`, `GetCategories()` — возвращают `Ok()` напрямую (только чтение, без бизнес-ошибок).

---

## Связанные документы

- [`api.md`](./api.md) — HTTP-контракты, ProblemDetails формат ответов
- [`observability.md`](./observability.md) — как ошибки логируются через Serilog
- [`operability.md`](./operability.md) — Exception Handler middleware
- [`adr.md`](./adr.md) — архитектурные решения

---
