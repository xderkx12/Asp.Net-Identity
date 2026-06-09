using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Security;
using Identity.Application.Abstractions.Services;
using Identity.Application.Common.Exceptions;
using Identity.Application.Features.Auth.Commands.Login;
using Identity.Application.Features.Auth.Commands.Logout;
using Identity.Application.Features.Auth.Commands.Refresh;
using Identity.Domain.Entities;

namespace Identity.Application.Services;

internal sealed class AuthenticationService(
    IUserRepository userRepository,
    ISessionRepository sessionRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    ICurrentRequestContext currentRequestContext) : IAuthenticationService
{
    public async Task<LoginResponse> LoginAsync(LoginCommand command, CancellationToken ct)
    {
        var user = await userRepository.GetByLoginAsync(command.Login, ct);
        if (user is null || !passwordHasher.Verify(command.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid login or password.");

        if (!user.IsActive)
            throw new UnauthorizedException("Account is deactivated.");

        if (user.IsBlockedAt(DateTime.UtcNow))
        {
            var details = user.IsPermanentlyBlocked()
                ? "Account is permanently blocked."
                : $"Account is blocked until {user.LockedUntilUtc:O}.";
            throw new UnauthorizedException(details);
        }

        var refreshToken = tokenService.GenerateRefreshToken();
        var session = new Session(
            Guid.NewGuid(),
            user.Id,
            tokenService.ComputeTokenHash(refreshToken),
            DateTime.UtcNow.AddDays(7),
            currentRequestContext.IpAddress,
            currentRequestContext.UserAgent);

        await sessionRepository.AddAsync(session, ct);
        await sessionRepository.SaveChangesAsync(ct);

        var roles = await userRepository.GetRoleNamesByUserIdAsync(user.Id, ct);
        var accessToken = tokenService.GenerateAccessToken(user.Id, user.Login, roles);

        return new LoginResponse(user.Id, accessToken)
        {
            RefreshToken = refreshToken
        };
    }

    public async Task<RefreshResponse> RefreshAsync(RefreshCommand command, CancellationToken ct)
    {
        var refreshTokenHash = tokenService.ComputeTokenHash(command.RefreshToken);
        var session = await sessionRepository.GetActiveByRefreshTokenHashAsync(refreshTokenHash, ct);
        if (session is null)
            throw new UnauthorizedException("Refresh token is invalid or expired.");

        if (!session.User.IsActive || session.User.IsBlockedAt(DateTime.UtcNow))
        {
            session.Revoke();
            sessionRepository.Update(session);
            await sessionRepository.SaveChangesAsync(ct);
            throw new UnauthorizedException("Account is deactivated or blocked.");
        }

        var newRefreshToken = tokenService.GenerateRefreshToken();
        session.Rotate(
            tokenService.ComputeTokenHash(newRefreshToken),
            DateTime.UtcNow.AddDays(7),
            currentRequestContext.IpAddress,
            currentRequestContext.UserAgent);
        sessionRepository.Update(session);
        await sessionRepository.SaveChangesAsync(ct);

        var roles = await userRepository.GetRoleNamesByUserIdAsync(session.UserId, ct);
        var accessToken = tokenService.GenerateAccessToken(session.UserId, session.User.Login, roles);

        return new RefreshResponse(accessToken)
        {
            RefreshToken = newRefreshToken
        };
    }

    public async Task<LogoutResponse> LogoutAsync(LogoutCommand command, CancellationToken ct)
    {
        var refreshTokenHash = tokenService.ComputeTokenHash(command.RefreshToken);
        var session = await sessionRepository.GetActiveByRefreshTokenHashForUserAsync(
            refreshTokenHash, command.UserId, ct);

        if (session is null)
            return new LogoutResponse(false);

        session.Revoke();
        sessionRepository.Update(session);
        await sessionRepository.SaveChangesAsync(ct);

        return new LogoutResponse(true);
    }
}
