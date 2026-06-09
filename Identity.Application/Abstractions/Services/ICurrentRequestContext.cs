namespace Identity.Application.Abstractions.Services;

public interface ICurrentRequestContext
{
    Guid? UserId { get; }
    string? Login { get; }
    string? IpAddress { get; }
    string? UserAgent { get; }
}
