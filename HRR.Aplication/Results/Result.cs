using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRR.Aplication.Results;

public class Result
{
    public bool IsSuccess { get; set; }
    public string? Error { get; set; }
    public ErrorType? ErrorType { get; set; }

    protected Result(bool isSuccess, string? error, ErrorType? errorType)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorType = errorType;
    }

    public static Result Success() => new(true, null, null);
    public static Result Failure(string error, ErrorType errorType) => new(false, error, errorType);
}