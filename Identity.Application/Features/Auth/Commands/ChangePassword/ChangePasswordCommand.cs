using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Auth.Commands.ChangePassword;

public sealed record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword) : ICommand<ChangePasswordResponse>;
