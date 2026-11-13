using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRR.Aplication.Results;

public class Result<T> : Result
{
    public T? Value { get; set; }

    private Result(bool isSuccess, string? error, ErrorType? errorType, T? value) : base(isSuccess, error, errorType)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, null, null, value);
    public static new Result<T> Failure(string error, ErrorType errorType) => new(false, error, errorType, default);
}
