using FluentValidation;

namespace Identity.Application.Features.Sessions.Commands.LogoutAllDevices;

public sealed class LogoutAllDevicesCommandValidator : AbstractValidator<LogoutAllDevicesCommand>
{
    public LogoutAllDevicesCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
