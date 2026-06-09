using Identity.Application.Features.Auth.Commands.ChangeLogin;
using Identity.Application.Features.Auth.Commands.ChangePassword;
using Identity.Application.Features.Auth.Commands.ForgotPassword;
using Identity.Application.Features.Auth.Commands.Register;
using Identity.Application.Features.Auth.Commands.ResetPassword;

namespace Identity.Application.Abstractions.Services;

public interface IAccountService
{
    Task<RegisterResponse> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken = default);
    Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordCommand command, CancellationToken cancellationToken = default);
    Task<ChangeLoginResponse> ChangeLoginAsync(ChangeLoginCommand command, CancellationToken cancellationToken = default);
    Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordCommand command, CancellationToken cancellationToken = default);
    Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordCommand command, CancellationToken cancellationToken = default);
}
