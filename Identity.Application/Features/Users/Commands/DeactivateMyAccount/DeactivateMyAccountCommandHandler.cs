using Ardalis.Result;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Security;
using Identity.Application.Abstractions.Services;
using Identity.Application.Common.Audit;
using Identity.Application.Common.Exceptions;
using MediatR;

namespace Identity.Application.Features.Users.Commands.DeactivateMyAccount;

public sealed class DeactivateMyAccountCommandHandler(
    IUserRepository userRepository,
    ISessionRepository sessionRepository,
    IPasswordHasher passwordHasher,
    IAuditLogger auditLogger,
    ICurrentRequestContext currentRequestContext)
    : IRequestHandler<DeactivateMyAccountCommand, Result<DeactivateMyAccountResponse>>
{
    public async Task<Result<DeactivateMyAccountResponse>> Handle(DeactivateMyAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
                   ?? throw new NotFoundException("User not found.");

        if (!passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
        {
            throw new UnauthorizedException("Current password is invalid.");
        }

        user.Deactivate();
        userRepository.Update(user);
        await userRepository.SaveChangesAsync(cancellationToken);

        var revoked = await sessionRepository.RevokeAllActiveForUserAsync(user.Id, cancellationToken);
        await sessionRepository.SaveChangesAsync(cancellationToken);

        await SafeAuditAsync(user.Id, user.Login, revoked, cancellationToken);

        return Result.Success(new DeactivateMyAccountResponse(revoked));
    }

    private async Task SafeAuditAsync(Guid userId, string userLogin, int revokedSessions, CancellationToken cancellationToken)
    {
        try
        {
            await auditLogger.LogAsync(
                new AuditEvent
                {
                    Action = AuditActions.UserDeactivated,
                    Success = true,
                    ActorUserId = userId,
                    ActorLogin = userLogin,
                    IpAddress = currentRequestContext.IpAddress,
                    UserAgent = currentRequestContext.UserAgent,
                    TargetType = "User",
                    TargetId = userId.ToString(),
                    TargetName = userLogin,
                    Details = $"Self-deactivated. RevokedSessions: {revokedSessions}"
                },
                cancellationToken);
        }
        catch
        {
        }
    }
}
