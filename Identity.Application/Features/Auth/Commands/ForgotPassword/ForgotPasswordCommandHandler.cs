using Ardalis.Result;
using Identity.Application.Abstractions.Services;
using MediatR;

namespace Identity.Application.Features.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(IAccountService accountService)
    : IRequestHandler<ForgotPasswordCommand, Result<ForgotPasswordResponse>>
{
    public async Task<Result<ForgotPasswordResponse>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var response = await accountService.ForgotPasswordAsync(request, cancellationToken);
        return Result.Success(response);
    }
}
