using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Security;
using Identity.Application.Abstractions.Services;
using Identity.Application.Common.Exceptions;
using Identity.Application.Features.Auth.Commands.ChangeLogin;
using Identity.Application.Features.Auth.Commands.ChangePassword;
using Identity.Application.Features.Auth.Commands.ForgotPassword;
using Identity.Application.Features.Auth.Commands.Register;
using Identity.Application.Features.Auth.Commands.ResetPassword;
using Identity.Domain.Entities;

namespace Identity.Application.Services;

internal sealed class AccountService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    ISessionRepository sessionRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IAccountService
{
    public async Task<RegisterResponse> RegisterAsync(RegisterCommand command, CancellationToken ct)
    {
        var loginAlreadyExists = await userRepository.ExistsByLoginAsync(command.Login, ct);
        if (loginAlreadyExists)
            throw new ConflictException("User with this login already exists.");

        var user = new User(Guid.NewGuid(), command.Login, passwordHasher.Hash(command.Password));
        await userRepository.AddAsync(user, ct);

        var assignedRoles = new List<string>();
        var defaultRole = await roleRepository.GetByNameAsync("employee", ct);
        if (defaultRole is not null)
        {
            user.AssignRole(defaultRole.Id);
            assignedRoles.Add(defaultRole.Name);
        }

        await userRepository.SaveChangesAsync(ct);

        var token = tokenService.GenerateAccessToken(user.Id, user.Login, assignedRoles);
        return new RegisterResponse(user.Id, token);
    }

    public async Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordCommand command, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, ct)
                   ?? throw new NotFoundException("User not found.");

        if (!passwordHasher.Verify(command.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedException("Current password is invalid.");

        user.ChangePassword(passwordHasher.Hash(command.NewPassword));
        userRepository.Update(user);
        await userRepository.SaveChangesAsync(ct);

        var revoked = await sessionRepository.RevokeAllActiveForUserAsync(user.Id, ct);
        await sessionRepository.SaveChangesAsync(ct);

        return new ChangePasswordResponse(revoked);
    }

    public async Task<ChangeLoginResponse> ChangeLoginAsync(ChangeLoginCommand command, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, ct)
                   ?? throw new NotFoundException("User not found.");

        if (!passwordHasher.Verify(command.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedException("Current password is invalid.");

        var newLogin = command.NewLogin.Trim();
        if (string.Equals(newLogin, user.Login, StringComparison.Ordinal))
            return new ChangeLoginResponse(user.Login);

        var conflict = await userRepository.ExistsByLoginAsync(newLogin, ct);
        if (conflict)
            throw new ConflictException("Login is already taken.");

        user.ChangeLogin(newLogin);
        userRepository.Update(user);
        await userRepository.SaveChangesAsync(ct);

        return new ChangeLoginResponse(user.Login);
    }

    public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordCommand command, CancellationToken ct)
    {
        var user = await userRepository.GetByLoginAsync(command.Login, ct);

        if (user is null || !user.IsActive)
            return new ForgotPasswordResponse(null, null);

        var rawToken = tokenService.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddHours(1);
        user.IssuePasswordResetToken(tokenService.ComputeTokenHash(rawToken), expiresAt);
        userRepository.Update(user);
        await userRepository.SaveChangesAsync(ct);

        return new ForgotPasswordResponse(rawToken, expiresAt);
    }

    public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordCommand command, CancellationToken ct)
    {
        var tokenHash = tokenService.ComputeTokenHash(command.ResetToken);
        var user = await userRepository.GetByPasswordResetTokenHashAsync(tokenHash, ct);

        if (user is null || !user.IsPasswordResetTokenValid(tokenHash, DateTime.UtcNow))
            throw new UnauthorizedException("Reset token is invalid or expired.");

        if (!user.IsActive)
            throw new UnauthorizedException("Account is deactivated.");

        user.ChangePassword(passwordHasher.Hash(command.NewPassword));
        userRepository.Update(user);
        await userRepository.SaveChangesAsync(ct);

        var revoked = await sessionRepository.RevokeAllActiveForUserAsync(user.Id, ct);
        await sessionRepository.SaveChangesAsync(ct);

        return new ResetPasswordResponse(revoked);
    }
}
