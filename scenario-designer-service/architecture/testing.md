# План тестирования

## Юнит-тесты

### Приоритет 1: Критичная инфраструктура

**ConfigurationValidator** — fail-fast при старте

- [x] Валидные настройки → `ValidateRequiredConfiguration` возвращает `true`
- [x] Отсутствует обязательная настройка (например, `Jwt:Key`) → возвращает `false`
- [x] Пустая строка в обязательной настройке → возвращает `false`
- [x] Логирует конкретное сообщение об ошибке
- [x] Port = 0 → false
- [x] Port = -1 → false
- [x] Port = "abc" → false
- [x] Port = 65535 → true
- [x] Port = 99999 → false
- [x] Jwt:Key ровно 32 символа → true
- [x] Jwt:Key 31 символ → false
- [x] Jwt:Key "short" → false
- [x] Все настройки отсутствуют (null) → false

**ReadinessHealthCheck** — сложная логика с кэшем

- [x] Shutdown flag (`IsCancellationRequested = true`) → сразу `Unhealthy`, без похода в БД
- [x] Кэш свежий (< 5 сек) → возвращает из кэша, БД не дёргается
- [x] Кэш протух (> 5 сек) → идёт в БД, обновляет кэш
- [x] БД упала → `Unhealthy` + запись в лог
- [x] Unhealthy результат кэшируется (не долбить мёртвую БД)
- [x] Shutdown не дёргает БД

**MinimalResponseWriter** — формат ответа

- [x] Возвращает `{"status":"Healthy"}` для Healthy
- [x] Возвращает `{"status":"Unhealthy"}` для Unhealthy
- [x] Возвращает `{"status":"Degraded"}` для Degraded
- [x] Content-Type: `application/json`
- [x] Не раскрывает внутренние детали (exception, stack trace)
- [x] Body не пустой

### Приоритет 2: Бизнес-логика и контракты

**LoggingController** — изменение уровня логирования в runtime

- [x] `SetLogLevel` с валидным уровнем → возвращает 200, меняет root
- [x] `SetLogLevel` с null Category → меняет root
- [x] `SetLogLevel` с пустым Category → возвращает 400 (ловит валидатор)
- [x] `SetLogLevel` с category → обновляет override
- [x] `SetLogLevel` с category → логирует Warning
- [x] `SetLogLevel` с category + null User → логирует "Unknown"
- [x] `SetLogLevel` с неизвестным category → возвращает 404 + текст ошибки
- [x] `SetLogLevel` с невалидным уровнем → возвращает 400 + текст ошибки
- [x] `SetLogLevel` с null level → возвращает 400 + текст ошибки
- [x] `SetLogLevel` с forbidden level (Fatal) → возвращает 400 + "forbidden"
- [x] `SetLogLevel` (root) логирует Warning
- [x] `SetLogLevel` (root) без User → логирует "Unknown"
- [x] `SetLogLevel` (root) с пустым Identity → логирует "Unknown"
- [x] `SetLogLevel` (root) с null Identity → логирует "Unknown"
- [x] `GetLogLevel` → возвращает Default + Overrides
- [x] `GetCategories` → возвращает реальные элементы
- [x] Валидатор интегрирован в контроллер
- [x] Убран unreachable code (Enum.TryParse после валидатора)

**DTO/Request** — валидация и сериализация (шаблон, не удалять!)

- Обязательные поля не могут быть null/empty
- Строки имеют правильную длину (min/max)
- Числа в допустимом диапазоне
- Email, URL, GUID — правильный формат
- Имена полей соответствуют контракту (camelCase)
- Null значения обрабатываются правильно
- Enum сериализуется как строка/число
- DateTime форматируется правильно (ISO 8601)

**SetLogLevelRequest + SetLogLevelValidator:**

- [x] Request = null → ошибка
- [x] Level = null → ошибка
- [x] Level = "" → ошибка
- [x] Level = "Critical" (невалидный) → ошибка
- [x] Level = "Warning" → успех
- [x] Category = null → успех
- [x] Category = "" → ошибка
- [x] Category = "Microsoft.AspNetCore" → успех

### Приоритет 3: Конфигурация и безопасность

**Options (AppSettings, OpenTelemetryOptions)** — валидация DataAnnotations

**AppSettings:**

- [x] ServiceName = null → валидация падает ([Required])
- [x] ServiceName = "" → валидация падает ([Required])
- [x] ServiceName = "ValidService" → валидация проходит
- [x] Port = 0 → валидация падает ([Range(1, 65535)])
- [x] Port = -1 → валидация падает ([Range(1, 65535)])
- [x] Port = 65536 → валидация падает ([Range(1, 65535)])
- [x] Port = 1 → валидация проходит (минимум)
- [x] Port = 65535 → валидация проходит (максимум)
- [x] Port = 8080 → валидация проходит

**OpenTelemetryOptions:**

- [x] Endpoint = null → валидация падает ([Required])
- [x] Endpoint = "" → валидация падает ([Required])
- [x] Endpoint = `http://localhost:4317` → валидация проходит
- [x] Endpoint = "https://..." → валидация проходит
- [x] Protocol = Grpc → валидация проходит
- [x] Protocol = HttpProtobuf → валидация проходит
- [x] UseConsoleExporter = false → валидация проходит
- [x] UseConsoleExporter = true → валидация проходит
- [x] Headers пустой → валидация проходит
- [x] Headers с значениями → валидация проходит
- [x] LogLevel пустой → валидация проходит
- [x] LogLevel с значениями → валидация проходит
- [x] Default values корректны

**Security Extensions** — JWT + policies

**AuthenticationExtensions:**

- [x] ValidateIssuer = true
- [x] ValidIssuer из конфига (Jwt:Issuer)
- [x] ValidIssuer = "ScenarioDesigner" (default)
- [x] ValidIssuer независим от конфига
- [x] ValidateAudience = true
- [x] ValidAudience из конфига (Jwt:Audience)
- [x] ValidAudience = "ScenarioDesigner" (default)
- [x] ValidAudience независим от конфига
- [x] ValidateLifetime = true
- [x] ValidateIssuerSigningKey = true
- [x] IssuerSigningKey != null
- [x] ClockSkew = 1 minute

**AuthorizationExtensions:**

- [x] Policy AdminOnly существует
- [x] AdminOnly содержит RolesAuthorizationRequirement с ролью Admin
- [x] Policy Operator существует
- [x] Operator содержит RolesAuthorizationRequirement с ролями Admin, Operator
- [x] Policy AuditViewer существует
- [x] AuditViewer содержит RolesAuthorizationRequirement с ролями Admin, Operator, Auditor

**ServiceExtensions** — DI конфигурация

- [ ] Все зависимости резолвятся (нет `InvalidOperationException`)
- [ ] Scoped сервисы не инжектятся в Singleton

---

## Интеграционные тесты

- **Health endpoints**

- [x] `/health/live` возвращает 200 + `{"status":"Healthy"}`
- [x] `/health/ready` возвращает 200
- [ ] `/health` агрегирует все проверки
- [ ] При shutdown readiness возвращает 503

- **Authentication & Authorization**

- [ ] JWT authentication end-to-end
- [ ] Authorization policies (Admin/Operator/Auditor)

- **Controllers**

- [ ] LoggingController end-to-end

---

## Что НЕ нужно тестировать

- **HealthCheckExtensions** — тривиальная регистрация, проверяется интеграционно
- **RateLimitingExtensions** — настраивается через ASP.NET Core
- **Program.cs** — только интеграционные тесты

---

## Итого

**Юнит-тесты:** 92 теста
**Покрытие:** 44.5% (line), 60.5% (branch), 70% (method)
**Классы на 100%:** 10 из 16

| Класс | Тестов | Покрытие |
|-------|--------|----------|
| ConfigurationValidator | 15 | 100% |
| ReadinessHealthCheck | 8 | 100% |
| MinimalResponseWriter | 6 | 100% |
| AppSettings | 9 | 100% |
| OpenTelemetryOptions | 13 | 100% |
| SetLogLevelRequest + Validator | 8 | 100% |
| LoggingController | 16 | 100% |
| AuthenticationExtensions | 12 | 19.5% |
| AuthorizationExtensions | 6 | 100% |

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
