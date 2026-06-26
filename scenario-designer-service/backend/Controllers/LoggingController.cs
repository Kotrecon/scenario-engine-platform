using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScenarioDesigner.Contracts.Dto.Request.Logging;
using Serilog.Core;
using Serilog.Events;

namespace ScenarioDesigner.Controllers;

// ============================================================================
// API ДЛЯ УПРАВЛЕНИЯ ЛОГИРОВАНИЕМ
// Позволяет менять уровни логирования на лету через LoggingLevelSwitch.
// Все изменения логируются с указанием пользователя (аудит).
// ============================================================================
[ApiController]
[Route("api/[controller]")]
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
        // --------------------------------------------------------------------
        // Валидация через SetLogLevelValidator
        // --------------------------------------------------------------------
        var validationError = SetLogLevelValidator.Validate(request);
        if (validationError != null)
            return BadRequest(new { error = validationError.ErrorMessage });

        // --------------------------------------------------------------------
        // Проверка запрещённых уровней (безопасность)
        // Валидатор уже проверил что Level валиден и входит в ValidLevels.
        // --------------------------------------------------------------------
        var level = Enum.Parse<LogEventLevel>(request.Level, true);

        if (ForbiddenLevels.Contains(level))
            return BadRequest(new { error = $"Level {level} is forbidden" });

        // --------------------------------------------------------------------
        // Изменение уровня
        // --------------------------------------------------------------------
        if (string.IsNullOrWhiteSpace(request.Category))
        {
            // Root level
            _rootSwitch.MinimumLevel = level;
            _logger.LogWarning("Root log level changed to {Level} by {User}",
                level, User.Identity?.Name ?? "Unknown");
        }
        else
        {
            // Override для конкретной категории
            if (!_overrideSwitches.TryGetValue(request.Category, out var sw))
                return NotFound(new { error = $"Category {request.Category} not found" });

            sw.MinimumLevel = level;
            _logger.LogWarning("Log level for {Category} changed to {Level} by {User}",
                request.Category, level, User.Identity?.Name ?? "Unknown");
        }

        return Ok(new { message = "Log level updated" });
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


