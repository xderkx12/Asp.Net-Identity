using Ardalis.Result;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Services;
using Identity.Application.Common.Audit;
using MediatR;

namespace Identity.Application.Features.Sessions.Commands.RevokeSession;

public sealed class RevokeSessionCommandHandler(
    ISessionRepository sessionRepository,
    IAuditLogger auditLogger,
    ICurrentRequestContext currentRequestContext)
    : IRequestHandler<RevokeSessionCommand, Result<RevokeSessionResponse>>
{
    public async Task<Result<RevokeSessionResponse>> Handle(RevokeSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await sessionRepository.GetByIdForUserAsync(request.SessionId, request.UserId, cancellationToken);
        if (session is null)
        {
            return Result.NotFound("Session not found.");
        }

        if (session.RevokedAtUtc is not null)
        {
            return Result.Success(new RevokeSessionResponse(false));
        }

        session.Revoke();
        sessionRepository.Update(session);
        await sessionRepository.SaveChangesAsync(cancellationToken);

        await SafeAuditAsync(request.UserId, request.SessionId, cancellationToken);

        return Result.Success(new RevokeSessionResponse(true));
    }

    private async Task SafeAuditAsync(Guid userId, Guid sessionId, CancellationToken cancellationToken)
    {
        try
        {
            await auditLogger.LogAsync(
                new AuditEvent
                {
                    Action = AuditActions.SessionRevoked,
                    Success = true,
                    ActorUserId = userId,
                    ActorLogin = currentRequestContext.Login,
                    IpAddress = currentRequestContext.IpAddress,
                    UserAgent = currentRequestContext.UserAgent,
                    TargetType = "Session",
                    TargetId = sessionId.ToString()
                },
                cancellationToken);
        }
        catch
        {
        }
    }
}
