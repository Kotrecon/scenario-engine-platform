// File: ForbiddenError.cs

namespace ScenarioDesigner.Contracts.Result.Common;

/// <summary>
/// Ошибка доступа: клиент пытается сделать то, на что у него нет прав.
/// HTTP-аналог — 403 Forbidden.
/// </summary>
public sealed class ForbiddenError : IError
{
    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    public string Message { get; } = "Доступ запрещён.";

    /// <summary>
    /// Машинный код ошибки.
    /// </summary>
    public string Code => "Forbidden";

    /// <summary>
    /// HTTP-статус: 403 Forbidden.
    /// </summary>
    public int StatusCode => 403;
}