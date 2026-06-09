using FluentValidation;

namespace Identity.Application.Features.Auth.Commands.Logout;

public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
