using Ardalis.Result;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Services;
using Identity.Application.Common.Audit;
using MediatR;

namespace Identity.Application.Features.Users.Commands.BlockUserByLogin;

public sealed class BlockUserByLoginCommandHandler(
    IUserRepository userRepository,
    ISessionRepository sessionRepository,
    IAuditLogger auditLogger,
    ICurrentRequestContext currentRequestContext)
    : IRequestHandler<BlockUserByLoginCommand, Result<BlockUserByLoginResponse>>
{
    public async Task<Result<BlockUserByLoginResponse>> Handle(BlockUserByLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByLoginAsync(request.Login, cancellationToken);
        if (user is null)
        {
            return Result.NotFound("User not found.");
        }

        var lockedUntil = request.LockedUntilUtc ?? DateTime.MaxValue;
        user.Block(lockedUntil, request.Reason);
        userRepository.Update(user);
        await userRepository.SaveChangesAsync(cancellationToken);

        var revoked = await sessionRepository.RevokeAllActiveForUserAsync(user.Id, cancellationToken);
        await sessionRepository.SaveChangesAsync(cancellationToken);

        await SafeAuditAsync(user.Id, user.Login, request.Reason, revoked, cancellationToken);

        return Result.Success(new BlockUserByLoginResponse(user.Login, user.LockedUntilUtc, user.LockReason, revoked));
    }

    private async Task SafeAuditAsync(Guid targetUserId, string targetLogin, string? reason, int revokedSessions, CancellationToken cancellationToken)
    {
        try
        {
            await auditLogger.LogAsync(
                new AuditEvent
                {
                    Action = AuditActions.UserBlocked,
                    Success = true,
                    ActorUserId = currentRequestContext.UserId,
                    ActorLogin = currentRequestContext.Login,
                    IpAddress = currentRequestContext.IpAddress,
                    UserAgent = currentRequestContext.UserAgent,
                    TargetType = "User",
                    TargetId = targetUserId.ToString(),
                    TargetName = targetLogin,
                    Details = $"Reason: {reason ?? "(none)"}, RevokedSessions: {revokedSessions}"
                },
                cancellationToken);
        }
        catch
        {
        }
    }
}
