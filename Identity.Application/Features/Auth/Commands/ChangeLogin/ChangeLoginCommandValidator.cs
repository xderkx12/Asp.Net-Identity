using FluentValidation;

namespace Identity.Application.Features.Auth.Commands.ChangeLogin;

public sealed class ChangeLoginCommandValidator : AbstractValidator<ChangeLoginCommand>
{
    public ChangeLoginCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CurrentPassword).NotEmpty().MaximumLength(256);
        RuleFor(x => x.NewLogin).NotEmpty().MaximumLength(100);
    }
}
