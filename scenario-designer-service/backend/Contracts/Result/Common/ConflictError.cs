// File: ConflictError.cs

namespace ScenarioDesigner.Contracts.Result.Common;

/// <summary>
/// Ошибка конфликта: попытка создать/изменить ресурс, который уже существует
/// или находится в состоянии, конфликтующем с запросом.
/// Пример: регистрация пользователя с уже занятым email.
/// </summary>
public sealed class ConflictError : IError
{
    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Машинный код ошибки.
    /// </summary>
    public string Code => "Conflict";

    /// <summary>
    /// HTTP-статус: 409 Conflict.
    /// </summary>
    public int StatusCode => 409;

    /// <summary>
    /// Создаёт ошибку конфликта.
    /// </summary>
    /// <param name="message">Детали конфликта.</param>
    public ConflictError(string message) => Message = message;
}