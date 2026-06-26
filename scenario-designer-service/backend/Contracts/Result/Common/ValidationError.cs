// File: ValidationError.cs

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ScenarioDesigner.Contracts.Result.Common;

/// <summary>
/// Ошибка валидации: входные данные не соответствуют правилам.
/// Хранит детали ошибок по полям в формате { "email": ["required", "invalid"] }.
/// </summary>
public sealed class ValidationError : IError
{
    /// <summary>
    /// Общее сообщение об ошибке валидации.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Машинный код ошибки.
    /// </summary>
    public string Code => "ValidationFailed";

    /// <summary>
    /// HTTP-статус: 422 Unprocessable Entity (RFC 4918).
    /// </summary>
    public int StatusCode => 422;

    /// <summary>
    /// Детали ошибок по полям: { "email": ["required", "invalid"], "password": ["minLength"] }.
    /// Сериализуется в JSON как "errors" для фронтенда.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Details { get; }

    /// <summary>
    /// Создаёт ошибку валидации.
    /// </summary>
    /// <param name="message">Общее сообщение.</param>
    /// <param name="details">
    /// Список ошибок в формате "field:code" (например, "email:required").
    /// </param>
    public ValidationError(string message, IEnumerable<string> details)
    {
        Message = message;
        Details = ParseFieldErrors(details);
    }

    /// <summary>
    /// Парсит строки в формате "field:code" в словарь { field: [codes] }.
    /// Поддерживает множественные ошибки для одного поля.
    /// </summary>
    private static Dictionary<string, string[]> ParseFieldErrors(IEnumerable<string> errors)
    {
        var result = new Dictionary<string, List<string>>();

        foreach (var error in errors ?? Enumerable.Empty<string>())
        {
            // Формат: "field:code" или "field:code:extra"
            var parts = error.Split(':', 2);
            var field = parts[0];
            var code = parts.Length > 1 ? parts[1] : "invalid";

            if (!result.ContainsKey(field))
                result[field] = new List<string>();

            result[field].Add(code);
        }

        return result.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToArray()
        );
    }
}