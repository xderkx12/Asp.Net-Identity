using Ardalis.Result;
using Identity.Application.Abstractions.Services;
using MediatR;

namespace Identity.Application.Features.Auth.Commands.Logout;

public sealed class LogoutCommandHandler(
    IAuthenticationService authenticationService)
    : IRequestHandler<LogoutCommand, Result<LogoutResponse>>
{
    public async Task<Result<LogoutResponse>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var response = await authenticationService.LogoutAsync(request, cancellationToken);
        return Result.Success(response);
    }
}
