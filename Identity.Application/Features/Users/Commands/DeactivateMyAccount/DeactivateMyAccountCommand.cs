using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Users.Commands.DeactivateMyAccount;

public sealed record DeactivateMyAccountCommand(Guid UserId, string CurrentPassword) : ICommand<DeactivateMyAccountResponse>;
