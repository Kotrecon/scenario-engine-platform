// File: IError.cs

using System;

namespace ScenarioDesigner.Contracts.Result.Common;

/// <summary>
/// Интерфейс ошибки — основа для типобезопасной обработки неудач.
/// Позволяет различать типы ошибок в коде, легко тестировать и безопасно сериализовать.
/// </summary>
public interface IError
{
    /// <summary>
    /// Человекочитаемое сообщение для логов и UI.
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Машинно-читаемый код для клиентов и switch-логики.
    /// Пример: "ValidationFailed", "NotFound".
    /// </summary>
    string Code { get; }

    /// <summary>
    /// HTTP-статус, соответствующий этой ошибке.
    /// Пример: 422 для валидации, 404 для не найдено.
    /// </summary>
    int StatusCode { get; }
}