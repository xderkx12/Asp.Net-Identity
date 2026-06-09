using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Auth.Commands.ChangeLogin;

public sealed record ChangeLoginCommand(Guid UserId, string CurrentPassword, string NewLogin) : ICommand<ChangeLoginResponse>;
