using Ardalis.Result;
using Identity.Application.Abstractions.Services;
using MediatR;

namespace Identity.Application.Features.Auth.Commands.Refresh;

public sealed class RefreshCommandHandler(
    IAuthenticationService authenticationService)
    : IRequestHandler<RefreshCommand, Result<RefreshResponse>>
{
    public async Task<Result<RefreshResponse>> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var response = await authenticationService.RefreshAsync(request, cancellationToken);
        return Result.Success(response);
    }
}
