using Ardalis.Result;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Services;
using Identity.Application.Common.Audit;
using MediatR;

namespace Identity.Application.Features.Users.Commands.ActivateUserByLogin;

public sealed class ActivateUserByLoginCommandHandler(
    IUserRepository userRepository,
    IAuditLogger auditLogger,
    ICurrentRequestContext currentRequestContext)
    : IRequestHandler<ActivateUserByLoginCommand, Result<ActivateUserByLoginResponse>>
{
    public async Task<Result<ActivateUserByLoginResponse>> Handle(ActivateUserByLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByLoginAsync(request.Login, cancellationToken);
        if (user is null)
        {
            return Result.NotFound("User not found.");
        }

        user.Activate();
        userRepository.Update(user);
        await userRepository.SaveChangesAsync(cancellationToken);

        await SafeAuditAsync(user.Id, user.Login, cancellationToken);
        return Result.Success(new ActivateUserByLoginResponse(user.Login, user.IsActive));
    }

    private async Task SafeAuditAsync(Guid targetUserId, string targetLogin, CancellationToken cancellationToken)
    {
        try
        {
            await auditLogger.LogAsync(
                new AuditEvent
                {
                    Action = AuditActions.UserActivated,
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
