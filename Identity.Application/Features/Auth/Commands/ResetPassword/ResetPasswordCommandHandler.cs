using Ardalis.Result;
using Identity.Application.Abstractions.Services;
using MediatR;

namespace Identity.Application.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler(IAccountService accountService)
    : IRequestHandler<ResetPasswordCommand, Result<ResetPasswordResponse>>
{
    public async Task<Result<ResetPasswordResponse>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var response = await accountService.ResetPasswordAsync(request, cancellationToken);
        return Result.Success(response);
    }
}
