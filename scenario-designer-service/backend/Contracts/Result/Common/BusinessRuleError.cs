// File: BusinessRuleError.cs

namespace ScenarioDesigner.Contracts.Result.Common;

/// <summary>
/// Общая ошибка бизнес-правила: например, "нельзя отменить оплаченный заказ".
/// Используется, когда нарушена логика предметной области.
/// </summary>
public sealed class BusinessRuleError : IError
{
    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Машинный код ошибки.
    /// </summary>
    public string Code => "BusinessRuleViolation";

    /// <summary>
    /// HTTP-статус: 400 Bad Request.
    /// </summary>
    public int StatusCode => 400;

    /// <summary>
    /// Создаёт ошибку бизнес-правила.
    /// </summary>
    /// <param name="message">Описание нарушенного правила.</param>
    public BusinessRuleError(string message) => Message = message;
}