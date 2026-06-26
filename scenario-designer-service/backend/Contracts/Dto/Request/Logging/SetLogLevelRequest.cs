namespace ScenarioDesigner.Contracts.Dto.Request.Logging;

// ============================================================================
// DTO ДЛЯ ИЗМЕНЕНИЯ УРОВНЯ ЛОГИРОВАНИЯ
// ============================================================================
public sealed record SetLogLevelRequest(string? Category, string Level);