using Identity.Mediator.Abstractions;

namespace Identity.Application.Features.Sessions.Commands.LogoutAllDevices;

public sealed record LogoutAllDevicesCommand(Guid UserId) : ICommand<LogoutAllDevicesResponse>;
