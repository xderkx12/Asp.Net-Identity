using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Auth.Commands.ResetPassword;

public sealed record ResetPasswordCommand(string ResetToken, string NewPassword) : ICommand<ResetPasswordResponse>;
