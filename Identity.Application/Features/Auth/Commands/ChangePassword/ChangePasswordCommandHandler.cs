using Ardalis.Result;
using Identity.Application.Abstractions.Services;
using MediatR;

namespace Identity.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler(IAccountService accountService)
    : IRequestHandler<ChangePasswordCommand, Result<ChangePasswordResponse>>
{
    public async Task<Result<ChangePasswordResponse>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var response = await accountService.ChangePasswordAsync(request, cancellationToken);
        return Result.Success(response);
    }
}
