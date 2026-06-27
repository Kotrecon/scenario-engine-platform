using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScenarioDesigner.Contracts.Dto.Request.Logging;
using ScenarioDesigner.Contracts.Result.Common;
using ScenarioDesigner.Contracts.Result.Web;
using Serilog.Core;
using Serilog.Events;

namespace ScenarioDesigner.Controllers;

// ============================================================================
// API ДЛЯ УПРАВЛЕНИЯ ЛОГИРОВАНИЕМ
// Позволяет менять уровни логирования на лету через LoggingLevelSwitch.
// Все изменения логируются с указанием пользователя (аудит).
// ============================================================================
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class LoggingController : ControllerBase
{
    private readonly LoggingLevelSwitch _rootSwitch;
    private readonly IDictionary<string, LoggingLevelSwitch> _overrideSwitches;
    private readonly ILogger<LoggingController> _logger;

    // ------------------------------------------------------------------------
    // Запрещённые уровни для безопасности.
    // Fatal — отключит все логи (кроме фатальных), скроет следы атаки.
    // Verbose — затопит логи, переполнит диск, затруднит анализ.
    // ------------------------------------------------------------------------
    private static readonly HashSet<LogEventLevel> ForbiddenLevels = new()
    {
        LogEventLevel.Fatal,
        LogEventLevel.Verbose
    };

    public LoggingController(
        LoggingLevelSwitch rootSwitch,
        IDictionary<string, LoggingLevelSwitch> overrideSwitches,
        ILogger<LoggingController> logger)
    {
        _rootSwitch = rootSwitch;
        _overrideSwitches = overrideSwitches;
        _logger = logger;
    }

    // ------------------------------------------------------------------------
    // GET /api/logging/level
    // Получить текущие уровни логирования (root + overrides).
    // Доступно: Admin, Operator, Auditor (только чтение).
    // ------------------------------------------------------------------------
    [HttpGet("level")]
    [Authorize(Policy = "AuditViewer")]
    public IActionResult GetLevel()
    {
        return Ok(new
        {
            Default = _rootSwitch.MinimumLevel.ToString(),
            Overrides = _overrideSwitches.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.MinimumLevel.ToString())
        });
    }

    // ------------------------------------------------------------------------
    // PUT /api/logging/level
    // Изменить уровень логирования для root или конкретной категории.
    // Доступно: только Admin.
    // Все изменения логируются для аудита.
    // ------------------------------------------------------------------------
    [HttpPut("level")]
    public IActionResult SetLevel([FromBody] SetLogLevelRequest request)
    {
        var validationResult = SetLogLevelValidator.Validate(request);
        if (!validationResult.IsSuccess)
            return validationResult.ToActionResult();

        var level = Enum.Parse<LogEventLevel>(request.Level, true);

        if (ForbiddenLevels.Contains(level))
            return ScenarioDesigner.Contracts.Result.Common.Result.Failure(new BusinessRuleError($"Level {level} is forbidden"))
                .ToActionResult();

        if (string.IsNullOrWhiteSpace(request.Category))
        {
            _rootSwitch.MinimumLevel = level;
            _logger.LogWarning("Root log level changed to {Level} by {User}",
                level, User.Identity?.Name ?? "Unknown");
        }
        else
        {
            if (!_overrideSwitches.TryGetValue(request.Category, out var sw))
                return ScenarioDesigner.Contracts.Result.Common.Result.Failure(new NotFoundError("Category", request.Category))
                    .ToActionResult();

            sw.MinimumLevel = level;
            _logger.LogWarning("Log level for {Category} changed to {Level} by {User}",
                request.Category, level, User.Identity?.Name ?? "Unknown");
        }

        return ScenarioDesigner.Contracts.Result.Common.Result.Success().ToActionResult();
    }

    // ------------------------------------------------------------------------
    // GET /api/logging/categories
    // Получить список категорий, для которых настроены overrides.
    // Доступно: Admin, Operator, Auditor (только чтение).
    // ------------------------------------------------------------------------
    [HttpGet("categories")]
    [Authorize(Policy = "AuditViewer")]
    public IActionResult GetCategories()
    {
        return Ok(_overrideSwitches.Keys);
    }
}
