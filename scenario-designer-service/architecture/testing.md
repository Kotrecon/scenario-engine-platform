# План тестирования

## Юнит-тесты

### Operability

#### **ConfigurationValidator** — fail-fast при старте

- Валидные настройки → `ValidateRequiredConfiguration` возвращает `true`
- Отсутствует обязательная настройка (например, `Jwt:Key`) → возвращает `false`
- Пустая строка в обязательной настройке → возвращает `false`
- Логирует конкретное сообщение об ошибке
- Port = 0 → false
- Port = -1 → false
- Port = "abc" → false
- Port = 65535 → true
- Port = 99999 → false
- Jwt:Key ровно 32 символа → true
- Jwt:Key 31 символ → false
- Jwt:Key "short" → false
- Все настройки отсутствуют (null) → false

#### **ReadinessHealthCheck** — сложная логика с кэшем

- Shutdown flag (`IsCancellationRequested = true`) → сразу `Unhealthy`, без похода в БД
- Кэш свежий (< 5 сек) → возвращает из кэша, БД не дёргается
- Кэш протух (> 5 сек) → идёт в БД, обновляет кэш
- БД упала → `Unhealthy` + запись в лог
- Unhealthy результат кэшируется (не долбить мёртвую БД)
- Shutdown не дёргает БД

#### **MinimalResponseWriter** — формат ответа

- Возвращает `{"status":"Healthy"}` для Healthy
- Возвращает `{"status":"Unhealthy"}` для Unhealthy
- Возвращает `{"status":"Degraded"}` для Degraded
- Content-Type: `application/json`
- Не раскрывает внутренние детали (exception, stack trace)
- Body не пустой

#### **ExceptionHandlerMiddleware**

- `InvokeAsync_MapsExceptionToCorrectStatusCode` (5 кейсов) — ArgumentException→400, KeyNotFoundException→404, UnauthorizedAccessException→403, TimeoutException→504, Exception→500
- `InvokeAsync_ReturnsCorrectJsonFormat` — error.code и error.message
- `InvokeAsync_DoesNotLeakInternalDetails` — нет StackTrace, InnerException, секретов

---

### Observability

#### **RequestResponseLoggingMiddleware**

- `InvokeAsync_LogsSuccessfulRequest` — Information лог с method, path, status, duration
- `InvokeAsync_LogsErrorStatusAsWarning` — Warning лог для 4xx/5xx

#### **CorrelationIdMiddleware**

- `InvokeAsync_WhenHeaderMissing_GeneratesCorrelationId` — генерирует GUID v7
- `InvokeAsync_WhenHeaderPresent_UsesIncomingCorrelationId` — прокидывает входящий id
- `InvokeAsync_SetsCorrelationIdInItems` — записывает в context.Items
- `InvokeAsync_SetsActivityTag` — устанавливает tag correlation.id в Activity

#### **OpenTelemetryOptions** — валидация DataAnnotations

- Endpoint = null → валидация падает ([Required])
- Endpoint = "" → валидация падает ([Required])
- Endpoint = `http://localhost:4317` → валидация проходит
- Endpoint = "https://..." → валидация проходит
- Protocol = Grpc → валидация проходит
- Protocol = HttpProtobuf → валидация проходит
- UseConsoleExporter = false → валидация проходит
- UseConsoleExporter = true → валидация проходит
- Headers пустой → валидация проходит
- Headers с значениями → валидация проходит
- LogLevel пустой → валидация проходит
- LogLevel с значениями → валидация проходит
- Default values корректны

---

### API

#### **LoggingController** — изменение уровня логирования в runtime

- `SetLogLevel` с валидным уровнем → возвращает 200, меняет root
- `SetLogLevel` с null Category → меняет root
- `SetLogLevel` с пустым Category → возвращает 400 (ловит валидатор)
- `SetLogLevel` с category → обновляет override
- `SetLogLevel` с category → логирует Warning
- `SetLogLevel` с category + null User → логирует "Unknown"
- `SetLogLevel` с неизвестным category → возвращает 404 + текст ошибки
- `SetLogLevel` с невалидным уровнем → возвращает 400 + текст ошибки
- `SetLogLevel` с null level → возвращает 400 + текст ошибки
- `SetLogLevel` с forbidden level (Fatal) → возвращает 400 + "forbidden"
- `SetLogLevel` (root) логирует Warning
- `SetLogLevel` (root) без User → логирует "Unknown"
- `SetLogLevel` (root) с пустым Identity → логирует "Unknown"
- `SetLogLevel` (root) с null Identity → логирует "Unknown"
- `GetLogLevel` → возвращает Default + Overrides
- `GetCategories` → возвращает реальные элементы
- Валидатор интегрирован в контроллер
- Убран unreachable code (Enum.TryParse после валидатора)

#### **SetLogLevelRequest + SetLogLevelValidator**

- Request = null → ошибка
- Level = null → ошибка
- Level = "" → ошибка
- Level = "Critical" (невалидный) → ошибка
- Level = "Warning" → успех
- Category = null → успех
- Category = "" → ошибка
- Category = "Microsoft.AspNetCore" → успех

---

### Configuration

#### **AppSettings** — валидация DataAnnotations

- ServiceName = null → валидация падает ([Required])
- ServiceName = "" → валидация падает ([Required])
- ServiceName = "ValidService" → валидация проходит
- Port = 0 → валидация падает ([Range(1, 65535)])
- Port = -1 → валидация падает ([Range(1, 65535)])
- Port = 65536 → валидация падает ([Range(1, 65535)])
- Port = 1 → валидация проходит (минимум)
- Port = 65535 → валидация проходит (максимум)
- Port = 8080 → валидация проходит

---

### Security

#### **AuthenticationExtensions** — JWT

- ValidateIssuer = true
- ValidIssuer из конфига (Jwt:Issuer)
- ValidIssuer = "ScenarioDesigner" (default)
- ValidIssuer независим от конфига
- ValidateAudience = true
- ValidAudience из конфига (Jwt:Audience)
- ValidAudience = "ScenarioDesigner" (default)
- ValidAudience независим от конфига
- ValidateLifetime = true
- ValidateIssuerSigningKey = true
- IssuerSigningKey != null
- ClockSkew = 1 minute

#### **AuthorizationExtensions** — policies

- Policy AdminOnly существует
- AdminOnly содержит RolesAuthorizationRequirement с ролью Admin
- Policy Operator существует
- Operator содержит RolesAuthorizationRequirement с ролями Admin, Operator
- Policy AuditViewer существует
- AuditViewer содержит RolesAuthorizationRequirement с ролями Admin, Operator, Auditor

---

### Infrastructure

#### **CorsExtensions**

- `AddCustomCors_RegistersCorsServices` — регистрирует ICorsPolicyProvider и IOptions<CorsOptions>
- `AddCustomCors_DefaultPolicyExists` — DefaultPolicy не null
- `AddCustomCors_DefaultPolicyResolvesViaProvider` — policy резолвится через provider
- `AddCustomCors_DefaultPolicy_AllowsEverything` — Origins содержит "_", Methods содержит "_", Headers содержит "\*"

#### **ServiceExtensions** — DI конфигурация

- Все зависимости резолвятся (нет `InvalidOperationException`)
- Scoped сервисы не инжектятся в Singleton

---

## Интеграционные тесты

### Health endpoints

- `/health/live` возвращает 200 + `{"status":"Healthy"}`
- `/health/ready` возвращает 200
- `/health` агрегирует все проверки
- При shutdown readiness возвращает 503

### Authentication & Authorization

- JWT authentication end-to-end
- Authorization policies (Admin/Operator/Auditor)

### Controllers

- LoggingController end-to-end

### Correlation ID

- Response header `X-Correlation-Id` возвращается в ответе
- Incoming `X-Correlation-Id` прокидывается в response без перегенерации

---

## Что НЕ нужно тестировать

Следующие компоненты либо тривиальны, либо покрываются интеграционными тестами — дополнительные unit-тесты не требуются:

- **HealthCheckExtensions** — тривиальная регистрация, проверяется интеграционно
- **RateLimitingExtensions** — настраивается через ASP.NET Core built-in API
- **CorsExtensions** — unit-тесты регистрации и проверки policy уже есть (`Extensions/Cors/CorsExtensionsTests.cs`), дополнительная логика покрывается интеграционными тестами
- **CorrelationIdExtensions** — тривиальная регистрация, проверяется интеграционно
- **RequestResponseLoggingExtensions** — тривиальная регистрация
- **ExceptionHandlerExtensions** — тривиальная регистрация
- **Program.cs** — только интеграционные тесты

---

## Итого

**Юнит-тесты:** 108 тестов  
**Покрытие:** 51.5% (line), 63% (branch)  
**Классы на 100%:** 11 из 14

| Класс                            | Тестов | Покрытие                   |
| -------------------------------- | ------ | -------------------------- |
| ConfigurationValidator           | 15     | 100%                       |
| ReadinessHealthCheck             | 8      | 100%                       |
| MinimalResponseWriter            | 6      | 100%                       |
| AppSettings                      | 9      | 100%                       |
| OpenTelemetryOptions             | 13     | 100%                       |
| SetLogLevelRequest + Validator   | 8      | 100%                       |
| LoggingController                | 16     | 100% (line), 95% (branch)  |
| AuthenticationExtensions         | 12     | 100% (line), 50% (branch)  |
| AuthorizationExtensions          | 6      | 100%                       |
| CorsExtensions                   | 3      | 100%                       |
| CorrelationIdMiddleware          | 4      | 84% (line), 83.3% (branch) |
| RequestResponseLoggingMiddleware | 2      | 100%                       |
| RequestResponseLoggingExtensions | —      | 0% (пустой метод)          |
| ExceptionHandlerMiddleware       | 7      | 100%                       |
| ExceptionHandlerExtensions       | —      | 0% (пустой метод)          |

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
