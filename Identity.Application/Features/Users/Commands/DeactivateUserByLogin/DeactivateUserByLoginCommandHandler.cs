using Ardalis.Result;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Services;
using Identity.Application.Common.Audit;
using MediatR;

namespace Identity.Application.Features.Users.Commands.DeactivateUserByLogin;

public sealed class DeactivateUserByLoginCommandHandler(
    IUserRepository userRepository,
    ISessionRepository sessionRepository,
    IAuditLogger auditLogger,
    ICurrentRequestContext currentRequestContext)
    : IRequestHandler<DeactivateUserByLoginCommand, Result<DeactivateUserByLoginResponse>>
{
    public async Task<Result<DeactivateUserByLoginResponse>> Handle(DeactivateUserByLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByLoginAsync(request.Login, cancellationToken);
        if (user is null)
        {
            return Result.NotFound("User not found.");
        }

        user.Deactivate();
        userRepository.Update(user);
        await userRepository.SaveChangesAsync(cancellationToken);

        var revoked = await sessionRepository.RevokeAllActiveForUserAsync(user.Id, cancellationToken);
        await sessionRepository.SaveChangesAsync(cancellationToken);

        await SafeAuditAsync(user.Id, user.Login, revoked, cancellationToken);

        return Result.Success(new DeactivateUserByLoginResponse(user.Login, user.IsActive, revoked));
    }

    private async Task SafeAuditAsync(Guid targetUserId, string targetLogin, int revokedSessions, CancellationToken cancellationToken)
    {
        try
        {
            await auditLogger.LogAsync(
                new AuditEvent
                {
                    Action = AuditActions.UserDeactivated,
                    Success = true,
                    ActorUserId = currentRequestContext.UserId,
                    ActorLogin = currentRequestContext.Login,
                    IpAddress = currentRequestContext.IpAddress,
                    UserAgent = currentRequestContext.UserAgent,
                    TargetType = "User",
                    TargetId = targetUserId.ToString(),
                    TargetName = targetLogin,
                    Details = $"RevokedSessions: {revokedSessions}"
                },
                cancellationToken);
        }
        catch
        {
        }
    }
}
