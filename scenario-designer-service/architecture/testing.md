# План тестирования

| Поле       | Значение   |
| ---------- | ---------- |
| **Версия** | 1.3.0      |
| **Статус** | Active     |
| **Дата**   | 2026-07-01 |

**Легенда:** ✅ — сделано, ⬜ — нужно сделать

---

## Юнит-тесты

### Operability

#### **ConfigurationValidator** — fail-fast при старте

- ✅ Валидные настройки → `ValidateRequiredConfiguration` возвращает `true`
- ✅ Отсутствует обязательная настройка (например, `Jwt:Key`) → возвращает `false`
- ✅ Пустая строка в обязательной настройке → возвращает `false`
- ✅ Логирует конкретное сообщение об ошибке
- ✅ Port = 0 → false
- ✅ Port = -1 → false
- ✅ Port = "abc" → false
- ✅ Port = 65535 → true
- ✅ Port = 99999 → false
- ✅ Jwt:Key ровно 32 символа → true
- ✅ Jwt:Key 31 символ → false
- ✅ Jwt:Key "short" → false
- ✅ Все настройки отсутствуют (null) → false

#### **ReadinessHealthCheck** — сложная логика с кэшем

- ✅ Shutdown flag → сразу `Unhealthy`, без похода в БД
- ✅ Кэш свежий (< 5 сек) → возвращает из кэша, БД не дёргается
- ✅ Кэш протух (> 5 сек) → идёт в БД, обновляет кэш
- ✅ БД упала → `Unhealthy` + запись в лог
- ✅ Unhealthy результат кэшируется (не долбить мёртвую БД)
- ✅ Shutdown не дёргает БД

#### **MinimalResponseWriter** — формат ответа

- ✅ Возвращает `{"status":"Healthy"}` для Healthy
- ✅ Возвращает `{"status":"Unhealthy"}` для Unhealthy
- ✅ Возвращает `{"status":"Degraded"}` для Degraded
- ⬜ Content-Type: `application/json`
- ✅ Не раскрывает внутренние детали (exception, stack trace)
- ✅ Body не пустой

#### **ExceptionHandlerMiddleware**

- ✅ `InvokeAsync_MapsExceptionToCorrectStatusCode` (5 кейсов)
- ✅ `InvokeAsync_ReturnsCorrectJsonFormat` — error.code и error.message
- ✅ `InvokeAsync_DoesNotLeakInternalDetails` — нет StackTrace, InnerException, секретов

---

### Observability

#### **RequestResponseLoggingMiddleware**

- ✅ `InvokeAsync_LogsSuccessfulRequest` — Information лог
- ✅ `InvokeAsync_LogsErrorStatusAsWarning` — Warning лог для 4xx/5xx

#### **CorrelationIdMiddleware**

- ✅ `InvokeAsync_WhenHeaderMissing_GeneratesCorrelationId` — генерирует GUID v7
- ✅ `InvokeAsync_WhenHeaderPresent_UsesIncomingCorrelationId` — прокидывает входящий id
- ✅ `InvokeAsync_SetsCorrelationIdInItems` — записывает в context.Items
- ✅ `InvokeAsync_SetsActivityTag` — устанавливает tag correlation.id в Activity

#### **OpenTelemetryOptions** — валидация DataAnnotations

- ✅ Endpoint = null → валидация падает
- ✅ Endpoint = "" → валидация падает
- ✅ Endpoint = `http://localhost:4317` → проходит
- ✅ Endpoint = "https://..." → проходит
- ✅ Protocol = Grpc → проходит
- ✅ Protocol = HttpProtobuf → проходит
- ✅ UseConsoleExporter = false → проходит
- ✅ UseConsoleExporter = true → проходит
- ✅ Headers пустой → проходит
- ✅ Headers с значениями → проходит
- ✅ LogLevel пустой → проходит
- ✅ LogLevel с значениями → проходит
- ✅ Default values корректны

#### **Result** — void-аналог (10)

- ✅ Success → IsSuccess=true, Errors пуст
- ✅ Failure с одной ошибкой → IsSuccess=false, одна ошибка
- ✅ Failure с несколькими ошибками → IsSuccess=false, все ошибки
- ✅ Failure без ошибок → InvalidOperationException
- ✅ OnSuccess при успехе → вызывается action
- ✅ OnSuccess при неудаче → action НЕ вызывается
- ✅ OnFailure при неудаче → вызывается action с ошибками
- ✅ OnFailure при успехе → action НЕ вызывается
- ✅ ToResult при успехе → Result<T>.Success
- ✅ ToResult при неудаче → Result<T>.Failure

#### **Result<T>** — обобщённый результат (14)

- ✅ Success → IsSuccess=true, Value, Errors пуст
- ✅ Failure с одной ошибкой → IsSuccess=false, default(T), одна ошибка
- ✅ Failure с несколькими ошибками → IsSuccess=false, default(T), все ошибки
- ✅ Failure без ошибок → InvalidOperationException
- ✅ OnSuccess при успехе → вызывается action со значением
- ✅ OnSuccess при неудаче → action НЕ вызывается
- ✅ OnFailure при неудаче → вызывается action с ошибками
- ✅ OnFailure при успехе → action НЕ вызывается
- ✅ Map при успехе → трансформирует значение
- ✅ Map при неудаче → прокидывает ошибки
- ✅ Bind при успехе → вызывает следующую операцию
- ✅ Bind при неудаче → прокидывает ошибки
- ✅ ToResult при успехе → non-generic Result.Success
- ✅ ToResult при неудаче → non-generic Result.Failure

#### **Ошибки** — свойства и парсинг (10)

- ✅ BusinessRuleError → Message, Code="BusinessRuleViolation", StatusCode=400
- ✅ ConflictError → Message, Code="Conflict", StatusCode=409
- ✅ ForbiddenError → Message="Доступ запрещён.", Code="Forbidden", StatusCode=403
- ✅ NotFoundError → EntityName, Code="NotFound", StatusCode=404
- ✅ ValidationError → Message, Code="ValidationFailed", StatusCode=422
- ✅ ValidationError парсит одну ошибку поля
- ✅ ValidationError парсит несколько ошибок одного поля
- ✅ ValidationError парсит несколько полей
- ✅ ValidationError использует "invalid" когда код отсутствует
- ✅ ValidationError обрабатывает null/пустой вход

#### **ResultExtensions** — маппинг в ProblemDetails (13)

- ✅ Generic Success → OkObjectResult
- ✅ Non-generic Success → OkResult
- ✅ Generic Failure → ObjectResult (404)
- ✅ Non-generic Failure → ObjectResult (400)
- ✅ ProblemDetails.Status/Title/Detail из первой ошибки
- ✅ Extensions["errors"] содержит все ошибки
- ✅ Extensions["validationErrors"] только для ValidationError
- ✅ НЕ содержит validationErrors для не-валидационных ошибок
- ✅ ProblemDetails содержит Status, Title, Detail
- ✅ Errors содержат code и message
- ✅ ValidationErrors структурированы по полям
- ✅ НЕ раскрывает внутренние данные (StackTrace, InnerException)

---

### API

#### **API Versioning** — версионирование URL-based

- ✅ LoggingController имеет атрибут `[ApiVersion("1.0")]`
- ✅ LoggingController имеет route `api/v{version:apiVersion}/[controller]`
- ⬜ Default API version: 1.0
- ⬜ AssumeDefaultVersionWhenUnspecified: true
- ⬜ ReportApiVersions: true (заголовок api-supported-versions)

#### **LoggingController** — изменение уровня логирования в runtime

- ✅ `SetLogLevel` с валидным уровнем → возвращает 200, меняет root
- ✅ `SetLogLevel` с null Category → меняет root
- ✅ `SetLogLevel` с пустым Category → возвращает 400 (ловит валидатор)
- ✅ `SetLogLevel` с category → обновляет override
- ✅ `SetLogLevel` с category → логирует Warning
- ✅ `SetLogLevel` с category + null User → логирует "Unknown"
- ✅ `SetLogLevel` с неизвестным category → возвращает 404 + текст ошибки
- ✅ `SetLogLevel` с невалидным уровнем → возвращает 400 + текст ошибки
- ✅ `SetLogLevel` с null level → возвращает 400 + текст ошибки
- ✅ `SetLogLevel` с forbidden level (Fatal) → возвращает 400 + "forbidden"
- ✅ `SetLogLevel` (root) логирует Warning
- ✅ `SetLogLevel` (root) без User → логирует "Unknown"
- ✅ `SetLogLevel` (root) с пустым Identity → логирует "Unknown"
- ✅ `SetLogLevel` (root) с null Identity → логирует "Unknown"
- ✅ `GetLogLevel` → возвращает Default + Overrides
- ✅ `GetCategories` → возвращает реальные элементы
- ✅ Валидатор интегрирован в контроллер
- ✅ Убран unreachable code (Enum.TryParse после валидатора)

#### **SetLogLevelRequest + SetLogLevelValidator**

- ✅ Request = null → ошибка
- ✅ Level = null → ошибка
- ✅ Level = "" → ошибка
- ✅ Level = "Critical" (невалидный) → ошибка
- ✅ Level = "Warning" → успех
- ✅ Category = null → успех
- ✅ Category = "" → ошибка
- ✅ Category = "Microsoft.AspNetCore" → успех

---

### Configuration

#### **AppSettings** — валидация DataAnnotations

- ✅ ServiceName = null → валидация падает ([Required])
- ✅ ServiceName = "" → валидация падает ([Required])
- ✅ ServiceName = "ValidService" → валидация проходит
- ✅ Port = 0 → валидация падает ([Range(1, 65535)])
- ✅ Port = -1 → валидация падает ([Range(1, 65535)])
- ✅ Port = 65536 → валидация падает ([Range(1, 65535)])
- ✅ Port = 1 → валидация проходит (минимум)
- ✅ Port = 65535 → валидация проходит (максимум)
- ✅ Port = 8080 → валидация проходит

---

### Security

#### **AuthenticationExtensions** — JWT

- ✅ ValidateIssuer = true
- ✅ ValidIssuer из конфига (Jwt:Issuer)
- ✅ ValidIssuer = "ScenarioDesigner" (default)
- ✅ ValidIssuer независим от конфига
- ✅ ValidateAudience = true
- ✅ ValidAudience из конфига (Jwt:Audience)
- ✅ ValidAudience = "ScenarioDesigner" (default)
- ✅ ValidAudience независим от конфига
- ✅ ValidateLifetime = true
- ✅ ValidateIssuerSigningKey = true
- ✅ IssuerSigningKey != null
- ✅ ClockSkew = 1 minute
- ✅ SigningKey — SymmetricSecurityKey с длиной ≥ 32 байт (HS256)

#### **AuthorizationExtensions** — policies

- ✅ Policy AdminOnly существует
- ✅ AdminOnly содержит RolesAuthorizationRequirement с ролью Admin
- ✅ Policy Operator существует
- ✅ Operator содержит RolesAuthorizationRequirement с ролями Admin, Operator
- ✅ Policy AuditViewer существует
- ✅ AuditViewer содержит RolesAuthorizationRequirement с ролями Admin, Operator, Auditor

---

### Infrastructure

#### **CorsExtensions**

- ⬜ `AddCustomCors_RegistersCorsServices` — регистрирует ICorsPolicyProvider и IOptions<CorsOptions>
- ✅ `AddCustomCors_DefaultPolicyResolvesViaProvider` — policy резолвится через provider
- ✅ `AddCustomCors_DefaultPolicy_AllowsEverything` — Origins содержит "_", Methods содержит "_", Headers содержит "\*"

#### **ServiceExtensions** — DI конфигурация

- ⬜ Все зависимости резолвятся (нет `InvalidOperationException`)
- ⬜ Scoped сервисы не инжектятся в Singleton

---

## Интеграционные тесты

### Health endpoints

- ⬜ `/health/live` возвращает 200 + `{"status":"Healthy"}`
- ⬜ `/health/ready` возвращает 200
- ⬜ `/health` агрегирует все проверки
- ⬜ При shutdown readiness возвращает 503

### Authentication & Authorization (end-to-end)

- ⬜ Валидный JWT-токен с ролью Admin → 200 на `/api/v1/Logging/level` (AdminOnly)
- ⬜ Валидный JWT-токен с ролью Operator → 403 на `/api/v1/Logging/level` (AdminOnly)
- ⬜ Валидный JWT-токен с ролью Auditor → 403 на `/api/v1/Logging/level` (AdminOnly)
- ⬜ Валидный JWT-токен с ролью Auditor → 200 на `/api/v1/Logging/categories` (AuditViewer)
- ⬜ Валидный JWT-токен с ролью Operator → 200 на `/api/v1/Logging/categories` (AuditViewer)
- ⬜ Валидный JWT-токен с ролью Admin → 200 на `/api/v1/Logging/categories` (AuditViewer)
- ⬜ Запрос без токена → 401 Unauthorized
- ⬜ Запрос с невалидным токеном (подделанная подпись) → 401
- ⬜ Запрос с expired токеном → 401
- ⬜ Запрос с неверным issuer → 401
- ⬜ Запрос с неверным audience → 401

### Dev Token Endpoint

- ⬜ POST `/dev/token` возвращает 200 + валидный JWT
- ⬜ Токен из `/dev/token` проходит валидацию на защищённом эндпоинте
- ⬜ `/dev/token` недоступен в Production (endpoint не зарегистрирован)

### Controllers

- ⬜ LoggingController end-to-end (через TestServer)

### Correlation ID

- ⬜ Response header `X-Correlation-Id` возвращается в ответе
- ⬜ Incoming `X-Correlation-Id` прокидывается в response без перегенерации

---

## Configuration DTO Tests

### ApiMetadataOptions (Options DTO)

**Позитивные тесты:**

- ✅ Значения по умолчанию проходят валидацию
- ✅ Все поля заданы явно и валидны
- ✅ Title длиной 3 символа проходит
- ✅ Title длиной 100 символов проходит
- ✅ Version в формате 1.0.0 проходит
- ✅ Version с суффиксом 1.0.0-beta1 проходит
- ✅ Description длиной 10 символов проходит
- ✅ Description длиной 500 символов проходит
- ✅ Developer с валидным Name, Email, Url проходит

**Негативные тесты:**

- ✅ Title = null не проходит
- ✅ Title = "" не проходит
- ✅ Title короче 3 символов не проходит
- ✅ Title длиннее 100 символов не проходит
- ✅ Version = null не проходит
- ✅ Version = "" не проходит
- ✅ Version не в semver-формате не проходит
- ✅ Description = null не проходит
- ✅ Description = "" не проходит
- ✅ Description короче 10 символов не проходит
- ✅ Description длиннее 500 символов не проходит
- ✅ Developer = null не проходит (nested validation)
- ✅ Developer с пустым Name не проходит
- ✅ Developer с невалидным Url не проходит
- ✅ Developer с невалидным Email не проходит

### ContactInfo (Options DTO, nested)

**Позитивные тесты:**

- ✅ Значения по умолчанию проходят
- ✅ Name длиной 2 символа проходит
- ✅ Name длиной 100 символов проходит
- ✅ Email = null проходит (nullable, без [Required])
- ✅ Email в корректном формате проходит
- ✅ Url = `https://localhost` проходит
- ✅ Url длиной 500 символов проходит

**Негативные тесты:**

- ✅ Name = null не проходит
- ✅ Name = "" не проходит
- ✅ Name короче 2 символов не проходит
- ✅ Name длиннее 100 символов не проходит
- ✅ Email в неверном формате не проходит
- ✅ Url = null не проходит
- ✅ Url = "" не проходит
- ✅ Url не является корректным абсолютным URL не проходит
- ✅ Url длиннее 500 символов не проходит

---

## Что НЕ нужно тестировать

Следующие компоненты либо тривиальны, либо покрываются интеграционными тестами — дополнительные unit-тесты не требуются:

- **HealthCheckExtensions** — тривиальная регистрация, проверяется интеграционно
- **RateLimitingExtensions** — настраивается через ASP.NET Core built-in API
- **CorrelationIdExtensions** — тривиальная регистрация, проверяется интеграционно
- **RequestResponseLoggingExtensions** — тривиальная регистрация
- **ExceptionHandlerExtensions** — тривиальная регистрация
- **Program.cs** — только интеграционные тесты

---

## Итого

| Категория            | Всего   | ✅ Сделано | ⬜ Осталось |
| -------------------- | ------- | ---------- | ----------- |
| Юнит-тесты           | 157     | 152        | 5           |
| Интеграционные тесты | 13      | 0          | 13          |
| **Всего**            | **170** | **152**    | **18**      |

---

## Coverlet

### Установка (один раз)

```powershell
dotnet tool install -g coverlet.console
```

Или использовать встроенный в `dotnet test` (не требует установки):

```powershell
dotnet test --collect:"XPlat Code Coverage"
```

### Просмотр результата

После `dotnet test` покрытие сохраняется в `TestResults/<guid>/coverage.cobertura.xml`.

**Конвертация в HTML для просмотра:**

```powershell
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

Открой `coveragereport/index.html` в браузере.

### Фильтры (исключить тесты и генераторы)

```powershell
dotnet test --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ExcludeByAttribute=GeneratedCodeAttribute,CompilerGeneratedAttribute
```

Или через `coverlet.runsettings`:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat code coverage">
        <Configuration>
          <Exclude>[xunit*]*,[*Tests]*</Exclude>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
```

```powershell
dotnet test --settings coverlet.runsettings --collect:"XPlat Code Coverage"
```

### Порог покрытия (fail build при низком %)

В `.csproj` тестового проекта:

```xml
<PropertyGroup>
  <CoverletOutputFormat>cobertura</CoverletOutputFormat>
  <Threshold>80</Threshold>
  <ThresholdType>line</ThresholdType>
</PropertyGroup>
```

Или через CLI:

```powershell
dotnet test --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Threshold=80
```

---

## Связанные документы

- [`api.md`](./api.md) — описывает эндпоинты, которые тестируются
- [`operability.md`](./operability.md) — описывает health checks, exception handler
- [`observability.md`](./observability.md) — описывает логирование, correlation ID

---

## Что изменилось в v1.3.0

| Элемент                                    | Изменение                         |
| ------------------------------------------ | --------------------------------- |
| Версия документа                           | 1.2.0 → 1.3.0                     |
| Сопоставление с `dotnet test --list-tests` | ✅ проставлены по реальным тестам |
| Итоговая таблица                           | 152 из 170 сделано (89%)          |
| Осталось юнит-тестов                       | 5                                 |
| Осталось интеграционных                    | 13                                |
