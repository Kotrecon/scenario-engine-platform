// File: NotFoundError.cs

namespace ScenarioDesigner.Contracts.Result.Common;

/// <summary>
/// Ошибка "не найдено": типична для операций поиска по идентификатору.
/// </summary>
public sealed class NotFoundError : IError
{
    /// <summary>
    /// Сообщение с именем сущности и ID для диагностики.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Машинный код ошибки.
    /// </summary>
    public string Code => "NotFound";

    /// <summary>
    /// HTTP-статус: 404 Not Found.
    /// </summary>
    public int StatusCode => 404;

    /// <summary>
    /// Имя типа сущности (для локализации и логов).
    /// </summary>
    public string EntityName { get; }

    /// <summary>
    /// Создаёт ошибку "не найдено".
    /// </summary>
    /// <param name="entityName">Имя типа сущности (например, "User").</param>
    /// <param name="id">Идентификатор, по которому не нашли.</param>
    public NotFoundError(string entityName, object id)
    {
        EntityName = entityName;
        Message = $"{entityName} с ID '{id}' не найден.";
    }
}