using Ardalis.Result;
using Identity.Application.Abstractions.Services;
using MediatR;

namespace Identity.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IAuthenticationService authenticationService)
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var response = await authenticationService.LoginAsync(request, cancellationToken);
        return Result.Success(response);
    }
}
