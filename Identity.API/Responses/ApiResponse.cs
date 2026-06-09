namespace Identity.Api.Responses;

public sealed class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public IEnumerable<string>? Errors { get; set; }
    public T? Data { get; set; }
}
