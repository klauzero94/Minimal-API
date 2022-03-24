using System.Text.Json;

namespace Utils;

public class ResponseBase<T, E>
{
    public ResponseBase(string statusCode, bool success, T? data, E? error)
    {
        StatusCode = statusCode;
        Success = success;
        Data = data;
        Error = error;
    }
    public string StatusCode { get; set; } = string.Empty;
    public bool Success { get; set; }
    public T? Data { get; set; }
    public E? Error { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}

public class ResponseBase
{
    public ResponseBase(string statusCode, bool success, object? data = null, object? error = null)
    {
        StatusCode = statusCode;
        Success = success;
        Data = data;
        Error = error;
    }
    public string StatusCode { get; set; } = string.Empty;
    public bool Success { get; set; }
    public object? Data { get; set; }
    public object? Error { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}