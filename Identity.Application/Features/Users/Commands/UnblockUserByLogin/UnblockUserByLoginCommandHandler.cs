using Ardalis.Result;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Services;
using Identity.Application.Common.Audit;
using MediatR;

namespace Identity.Application.Features.Users.Commands.UnblockUserByLogin;

public sealed class UnblockUserByLoginCommandHandler(
    IUserRepository userRepository,
    IAuditLogger auditLogger,
    ICurrentRequestContext currentRequestContext)
    : IRequestHandler<UnblockUserByLoginCommand, Result<UnblockUserByLoginResponse>>
{
    public async Task<Result<UnblockUserByLoginResponse>> Handle(UnblockUserByLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByLoginAsync(request.Login, cancellationToken);
        if (user is null)
        {
            return Result.NotFound("User not found.");
        }

        user.Unblock();
        userRepository.Update(user);
        await userRepository.SaveChangesAsync(cancellationToken);

        await SafeAuditAsync(user.Id, user.Login, cancellationToken);
        return Result.Success(new UnblockUserByLoginResponse(user.Login));
    }

    private async Task SafeAuditAsync(Guid targetUserId, string targetLogin, CancellationToken cancellationToken)
    {
        try
        {
            await auditLogger.LogAsync(
                new AuditEvent
                {
                    Action = AuditActions.UserUnblocked,
                    Success = true,
                    ActorUserId = currentRequestContext.UserId,
                    ActorLogin = currentRequestContext.Login,
                    IpAddress = currentRequestContext.IpAddress,
                    UserAgent = currentRequestContext.UserAgent,
                    TargetType = "User",
                    TargetId = targetUserId.ToString(),
                    TargetName = targetLogin
                },
                cancellationToken);
        }
        catch
        {
        }
    }
}
