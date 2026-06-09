using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Auth.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Login) : ICommand<ForgotPasswordResponse>;
