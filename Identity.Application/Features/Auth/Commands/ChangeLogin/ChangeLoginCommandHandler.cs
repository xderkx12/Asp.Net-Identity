using Ardalis.Result;
using Identity.Application.Abstractions.Services;
using MediatR;

namespace Identity.Application.Features.Auth.Commands.ChangeLogin;

public sealed class ChangeLoginCommandHandler(IAccountService accountService)
    : IRequestHandler<ChangeLoginCommand, Result<ChangeLoginResponse>>
{
    public async Task<Result<ChangeLoginResponse>> Handle(ChangeLoginCommand request, CancellationToken cancellationToken)
    {
        var response = await accountService.ChangeLoginAsync(request, cancellationToken);
        return Result.Success(response);
    }
}
