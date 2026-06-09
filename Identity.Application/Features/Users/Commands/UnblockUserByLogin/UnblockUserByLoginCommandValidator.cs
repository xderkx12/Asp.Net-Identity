using FluentValidation;

namespace Identity.Application.Features.Users.Commands.UnblockUserByLogin;

public sealed class UnblockUserByLoginCommandValidator : AbstractValidator<UnblockUserByLoginCommand>
{
    public UnblockUserByLoginCommandValidator()
    {
        RuleFor(x => x.Login).NotEmpty().MaximumLength(100);
    }
}
