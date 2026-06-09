using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Users.Commands.BlockUserByLogin;

public sealed record BlockUserByLoginCommand(string Login, DateTime? LockedUntilUtc, string? Reason) : ICommand<BlockUserByLoginResponse>;
