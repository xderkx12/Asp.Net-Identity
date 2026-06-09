using FluentValidation;

namespace Identity.Application.Features.Users.Commands.ActivateUserByLogin;

public sealed class ActivateUserByLoginCommandValidator : AbstractValidator<ActivateUserByLoginCommand>
{
    public ActivateUserByLoginCommandValidator()
    {
        RuleFor(x => x.Login).NotEmpty().MaximumLength(100);
    }
}
