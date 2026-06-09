using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Sessions.Commands.RevokeSession;

public sealed record RevokeSessionCommand(Guid UserId, Guid SessionId) : ICommand<RevokeSessionResponse>;
