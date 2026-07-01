# AGENTS.md — Scenario Engine Platform

## Project Structure

```
scenario-engine-platform/
├── global.json                    # Test runner config
├── scenario-designer-service/
│   ├── README.md                  # Main docs
│   ├── architecture/              # All architecture docs
│   ├── backend/                   # .NET 10 C# API
│   │   ├── ScenarioDesigner.csproj
│   │   ├── ScenarioDesigner.Tests.csproj
│   │   ├── Program.cs
│   │   ├── Contracts/Result/      # Result Pattern library
│   │   └── Controllers/
│   ├── frontend/                  # (planned, empty)
│   └── temp/                      # gitignored
└── .mimocode/                     # MiMoCode skills (gitignored)
```

## Quick Commands

```bash
# Build
cd scenario-designer-service/backend
dotnet build --no-restore

# Run all tests
dotnet test --project "ScenarioDesigner.Tests/ScenarioDesigner.Tests.csproj" --no-build

# Run tests with coverage
dotnet test --project "ScenarioDesigner.Tests/ScenarioDesigner.Tests.csproj" --no-build --coverage --coverage-output-format cobertura

# Generate coverage HTML
reportgenerator -reports:"ScenarioDesigner.Tests/bin/Debug/net10.0/TestResults/*.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

## Test Framework

- **TUnit** 1.56.35 (NOT xUnit, NOT NUnit)
- **Moq** 4.20.72 for mocking
- Coverage: Microsoft.Testing.Extensions.CodeCoverage
- 156 tests, ~60% line coverage

### Test Conventions

```csharp
// TUnit style - async assertions
[Test]
public async Task MethodName_WhenCondition_ExpectedResult()
{
    var result = MyClass.DoWork();
    await Assert.That(result).IsEqualTo(expected);
}
```

## Key Architecture Decisions

1. **Result Pattern** for business errors (NOT exceptions)
   - `Result.Success()` / `Result.Failure(error)`
   - 5 error types: ValidationError (422), NotFoundError (404), ConflictError (409), ForbiddenError (403), BusinessRuleError (400)
   - `result.ToActionResult()` maps to ProblemDetails (RFC 7807)

2. **Namespace conflict**: `ScenarioDesigner.Contracts.Result` namespace conflicts with `Result` type
   - Use fully qualified names in test files: `ScenarioDesigner.Contracts.Result.Common.Result`

3. **API Versioning**: URL-based `/api/v{version}/[controller]`
   - Default version: 1.0
   - `[ApiVersion("1.0")]` attribute on controllers

4. **Two ports**: API (8080), Health checks (8081)

## Documentation

All docs in `scenario-designer-service/architecture/`:
- `architecture.md` — Tech stack, project structure
- `plan.md` — Development roadmap (14 phases)
- `testing.md` — Test inventory and coverage
- `adr.md` — Architecture Decision Records (14 ADRs)
- `TODO.md` — Remaining tasks
- `api.md` — API endpoints
- `observability.md` — Logging, Correlation ID, OpenTelemetry
- `operability .md` — Health checks, Graceful Shutdown, Rate Limiting
- `result-pattern.md` — Result Pattern library docs

## Common Pitfalls

1. **Don't push without explicit request** — user gets very upset
2. **Stop after each step** — read `step.md` in project root
3. **Tests must pass before stopping** — user requires verification
4. **Coverage must be verified** — run actual tools, never claim numbers
5. **TODO.md**: Delete completed items, don't mark with [x]
6. **OpenApi namespace**: Swashbuckle 10.2.3 uses OpenApi 2.7.5 — types are in `Microsoft.OpenApi`, NOT `Microsoft.OpenApi.Models`

## Skills

- `feature-workflow` — 13-step feature development lifecycle with approval gates
- `ui-design` — Marina AI Style design system (global, customizable per project)

## Current Status (Phase 0)

- [x] Health checks
- [x] Exception Handler
- [x] Request/Response Logging
- [x] Correlation ID
- [x] CORS
- [x] Result Pattern + ProblemDetails
- [x] API Versioning
- [ ] Swagger/OpenAPI (in progress — build broken, OpenApi namespace issue)
- [ ] Persistence layer (EF Core)
- [ ] Messaging layer (RabbitMQ/Kafka)
