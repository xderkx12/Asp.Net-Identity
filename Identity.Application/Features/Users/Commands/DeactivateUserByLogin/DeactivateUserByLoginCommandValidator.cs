using FluentValidation;

namespace Identity.Application.Features.Users.Commands.DeactivateUserByLogin;

public sealed class DeactivateUserByLoginCommandValidator : AbstractValidator<DeactivateUserByLoginCommand>
{
    public DeactivateUserByLoginCommandValidator()
    {
        RuleFor(x => x.Login).NotEmpty().MaximumLength(100);
    }
}
