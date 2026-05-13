using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Common
{
    public class Result
    {
        public bool IsSuccess { get; init; }
        public bool IsFailure => !IsSuccess;
        public string Message { get; init; } = string.Empty;

        public Result()
        {
            IsSuccess = true;
            Message = "Operation completed successfully";
        }

        public Result(string message)
        {
            IsSuccess = false;
            Message = message;
        }

        public static Result Failure(string msg) => new(msg);
        public static Result Success() => new();
        public static Result Success(string message) => new() { Message = message };
    }

    // Generic version for returning data
    public class Result<T> : Result
    {
        public T? Data { get; init; }

        public Result(T data)
        {
            IsSuccess = true;
            Data = data;
            Message = "Operation completed successfully";
        }

        public Result(string message) : base(message)
        {
            Data = default;
        }

        public static new Result<T> Failure(string msg) => new(msg);
        public static Result<T> Success(T data) => new(data);
        public static Result<T> Success(T data, string message) => new(data) { Message = message };
    }
}
