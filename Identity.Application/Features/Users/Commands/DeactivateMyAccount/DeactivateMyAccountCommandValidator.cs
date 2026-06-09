using FluentValidation;

namespace Identity.Application.Features.Users.Commands.DeactivateMyAccount;

public sealed class DeactivateMyAccountCommandValidator : AbstractValidator<DeactivateMyAccountCommand>
{
    public DeactivateMyAccountCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CurrentPassword).NotEmpty().MaximumLength(256);
    }
}
