using ECommerce.BuildingBlocks.Application.Errors;

namespace ECommerce.BuildingBlocks.Application;

public class Result
{
    protected Result(bool isSuccess, Error error, string message = "")
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("Successful result cannot contain an error.");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("Failure result must contain an error.");

        IsSuccess = isSuccess;
        Error = error;
        Message = message;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public string Message { get; }

    public static Result Success(string message = "") =>
        new(true, Error.None, message);

    public static Result Failure(Error error) =>
        new(false, error, error.Message);

    public static Result ValidationFailure(string code, string message) =>
        Failure(Error.Validation(code, message));

    public static Result NotFound(string code, string message) =>
        Failure(Error.NotFound(code, message));

    public static Result Conflict(string code, string message) =>
        Failure(Error.Conflict(code, message));

    public static Result Unauthorized(string code, string message) =>
        Failure(Error.Unauthorized(code, message));

    public static Result Forbidden(string code, string message) =>
        Failure(Error.Forbidden(code, message));
}

public class Result<T> : Result
{
    private Result(T data, string message = "")
        : base(true, Error.None, message)
    {
        Data = data;
    }

    private Result(Error error)
        : base(false, error, error.Message)
    {
        Data = default;
    }

    public T? Data { get; }

    public static Result<T> Success(T data, string message = "") =>
        new(data, message);

    public static new Result<T> Failure(Error error) =>
        new(error);

    public static new Result<T> ValidationFailure(string code, string message) =>
        Failure(Error.Validation(code, message));

    public static new Result<T> NotFound(string code, string message) =>
        Failure(Error.NotFound(code, message));

    public static new Result<T> Conflict(string code, string message) =>
        Failure(Error.Conflict(code, message));

    public static new Result<T> Unauthorized(string code, string message) =>
        Failure(Error.Unauthorized(code, message));

    public static new Result<T> Forbidden(string code, string message) =>
        Failure(Error.Forbidden(code, message));
}