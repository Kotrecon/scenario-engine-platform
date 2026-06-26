// File: ResultOfT.cs

using System;
using System.Collections.Generic;
using System.Linq;

namespace ScenarioDesigner.Contracts.Result.Common;

/// <summary>
/// Обобщённый Result&lt;T&gt; — для операций с возвращаемым значением.
/// Он либо содержит значение (успех), либо ошибки (неуспех). Никогда и то, и другое.
/// </summary>
/// <typeparam name="T">Тип возвращаемого значения при успехе.</typeparam>
public class Result<T>
{
    /// <summary>
    /// Признак успешного выполнения операции.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Возвращаемое значение (доступно только при успехе).
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Список ошибок (пустой при успехе).
    /// </summary>
    public IReadOnlyList<IError> Errors { get; }

    /// <summary>
    /// Конструктор с защитой от неконсистентного состояния.
    /// </summary>
    /// <param name="isSuccess">Флаг успеха.</param>
    /// <param name="value">Значение при успехе.</param>
    /// <param name="errors">Список ошибок (обязателен при !isSuccess).</param>
    protected internal Result(bool isSuccess, T value, IEnumerable<IError> errors)
    {
        if (!isSuccess && (errors == null || !errors.Any()))
            throw new InvalidOperationException(ResultErrorMessages.FailureRequiresErrors);

        IsSuccess = isSuccess;
        Value = value;
        Errors = (errors?.ToList() ?? new List<IError>()).AsReadOnly();
    }

    /// <summary>
    /// Создаёт успешный результат с заданным значением.
    /// </summary>
    public static Result<T> Success(T value) => new(true, value, Array.Empty<IError>());

    /// <summary>
    /// Создаёт неуспешный результат с одной ошибкой.
    /// </summary>
    public static Result<T> Failure(IError error) => new(false, default!, new[] { error });

    /// <summary>
    /// Создаёт неуспешный результат с несколькими ошибками.
    /// </summary>
    public static Result<T> Failure(IEnumerable<IError> errors) => new(false, default!, errors);

    /// <summary>
    /// Выполняет действие со значением при успехе.
    /// </summary>
    public void OnSuccess(Action<T> action)
    {
        if (IsSuccess) action(Value);
    }

    /// <summary>
    /// Выполняет действие при неудаче.
    /// </summary>
    public void OnFailure(Action<IReadOnlyList<IError>> action)
    {
        if (!IsSuccess) action(Errors);
    }

    /// <summary>
    /// Преобразует значение при успехе, применяя функцию.
    /// Аналог LINQ Select: Result&lt;T&gt; → Result&lt;TResult&gt;.
    /// </summary>
    public Result<TResult> Map<TResult>(Func<T, TResult> func) =>
        IsSuccess ? Result<TResult>.Success(func(Value)) : Result<TResult>.Failure(Errors);

    /// <summary>
    /// Выполняет следующую операцию, возвращающую Result, при успехе.
    /// Основа для цепочек без вложенных if (монадический стиль).
    /// </summary>
    public Result<TResult> Bind<TResult>(Func<T, Result<TResult>> func) =>
        IsSuccess ? func(Value) : Result<TResult>.Failure(Errors);

    /// <summary>
    /// Преобразует в необобщённый Result, отбрасывая значение.
    /// Полезно, когда значение больше не нужно в цепочке.
    /// </summary>
    public Result ToResult() => IsSuccess ? Result.Success() : Result.Failure(Errors);
}