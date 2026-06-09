using Ardalis.Result;
using Identity.Application.Abstractions.Services;
using MediatR;

namespace Identity.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IAccountService accountService)
    : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var response = await accountService.RegisterAsync(request, cancellationToken);
        return Result.Success(response);
    }
}
