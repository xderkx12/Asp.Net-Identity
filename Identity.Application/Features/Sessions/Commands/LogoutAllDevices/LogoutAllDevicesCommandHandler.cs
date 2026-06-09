using Ardalis.Result;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Services;
using Identity.Application.Common.Audit;
using MediatR;

namespace Identity.Application.Features.Sessions.Commands.LogoutAllDevices;

public sealed class LogoutAllDevicesCommandHandler(
    ISessionRepository sessionRepository,
    IAuditLogger auditLogger,
    ICurrentRequestContext currentRequestContext)
    : IRequestHandler<LogoutAllDevicesCommand, Result<LogoutAllDevicesResponse>>
{
    public async Task<Result<LogoutAllDevicesResponse>> Handle(LogoutAllDevicesCommand request, CancellationToken cancellationToken)
    {
        var revoked = await sessionRepository.RevokeAllActiveForUserAsync(request.UserId, cancellationToken);
        await sessionRepository.SaveChangesAsync(cancellationToken);

        await SafeAuditAsync(request.UserId, revoked, cancellationToken);

        return Result.Success(new LogoutAllDevicesResponse(revoked));
    }

    private async Task SafeAuditAsync(Guid userId, int revokedSessions, CancellationToken cancellationToken)
    {
        try
        {
            await auditLogger.LogAsync(
                new AuditEvent
                {
                    Action = AuditActions.SessionsRevokedAll,
                    Success = true,
                    ActorUserId = userId,
                    ActorLogin = currentRequestContext.Login,
                    IpAddress = currentRequestContext.IpAddress,
                    UserAgent = currentRequestContext.UserAgent,
                    TargetType = "User",
                    TargetId = userId.ToString(),
                    Details = $"RevokedSessions: {revokedSessions}"
                },
                cancellationToken);
        }
        catch
        {
        }
    }
}
