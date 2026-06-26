// File: Result.cs

using System;
using System.Collections.Generic;
using System.Linq;

namespace ScenarioDesigner.Contracts.Result.Common;

/// <summary>
/// Необобщённый Result — для операций без возвращаемого значения (аналог void).
/// Примеры: "сохранить файл", "отправить уведомление", "удалить запись".
/// </summary>
public class Result
{
    /// <summary>
    /// Признак успешного выполнения операции.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Список ошибок (пустой при успехе).
    /// </summary>
    public IReadOnlyList<IError> Errors { get; }

    /// <summary>
    /// Конструктор с защитой от неконсистентного состояния.
    /// </summary>
    /// <param name="isSuccess">Флаг успеха.</param>
    /// <param name="errors">Список ошибок (обязателен при !isSuccess).</param>
    protected internal Result(bool isSuccess, IEnumerable<IError> errors)
    {
        if (!isSuccess && (errors == null || !errors.Any()))
            throw new InvalidOperationException(ResultErrorMessages.FailureRequiresErrors);

        IsSuccess = isSuccess;
        Errors = (errors?.ToList() ?? new List<IError>()).AsReadOnly();
    }

    /// <summary>
    /// Создаёт успешный результат.
    /// </summary>
    public static Result Success() => new(true, Array.Empty<IError>());

    /// <summary>
    /// Создаёт неуспешный результат с одной ошибкой.
    /// </summary>
    public static Result Failure(IError error) => new(false, new[] { error });

    /// <summary>
    /// Создаёт неуспешный результат с несколькими ошибками.
    /// </summary>
    public static Result Failure(IEnumerable<IError> errors) => new(false, errors);

    /// <summary>
    /// Выполняет действие при успехе.
    /// </summary>
    public void OnSuccess(Action action)
    {
        if (IsSuccess) action();
    }

    /// <summary>
    /// Выполняет действие при неудаче.
    /// </summary>
    public void OnFailure(Action<IReadOnlyList<IError>> action)
    {
        if (!IsSuccess) action(Errors);
    }

    /// <summary>
    /// Преобразует в обобщённый Result&lt;T&gt;, подставляя значение при успехе.
    /// Полезно для продолжения цепочки после void-операции.
    /// </summary>
    public Result<T> ToResult<T>(T valueIfSuccess) =>
        IsSuccess ? Result<T>.Success(valueIfSuccess) : Result<T>.Failure(Errors);
}

/// <summary>
/// Внутренние сообщения об ошибках для валидации состояния Result.
/// </summary>
internal static class ResultErrorMessages
{
    /// <summary>
    /// Сообщение о том, что неуспешный Result обязан содержать хотя бы одну ошибку.
    /// </summary>
    public static string FailureRequiresErrors =>
#if DEBUG
        "Неуспешный Result должен содержать хотя бы одну ошибку (IError). Проверьте вызовы Result.Failure().";
#else
        "A failure Result must contain at least one IError. This indicates a bug in the calling code.";
#endif
}