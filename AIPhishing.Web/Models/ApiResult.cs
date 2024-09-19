namespace AIPhishing.Web.Models;

public class ApiResult
{
    public int ErrorCode { get; init; }

    public string? ErrorMessage { get; init; } = null;

    public ApiResult()
    {
    }

    public ApiResult(int errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
}

public class ApiResult<T> : ApiResult 
{
    private readonly T _result;

    public T Result => this._result;

    public ApiResult(T result) => this._result = result;
}