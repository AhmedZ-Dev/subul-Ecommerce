namespace backend.Common.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public string[]? Errors { get; set; }

    public ApiResponse(bool success, T? data, string? message = null, string[]? errors = null)
    {
        Success = success;
        Data = data;
        Message = message;
        Errors = errors;
    }
}
