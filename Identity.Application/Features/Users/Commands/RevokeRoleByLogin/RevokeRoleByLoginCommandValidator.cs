using FluentValidation;

namespace Identity.Application.Features.Users.Commands.RevokeRoleByLogin;

public sealed class RevokeRoleByLoginCommandValidator : AbstractValidator<RevokeRoleByLoginCommand>
{
    public RevokeRoleByLoginCommandValidator()
    {
        RuleFor(x => x.Login).NotEmpty().MaximumLength(100);
        RuleFor(x => x.RoleName).NotEmpty().MaximumLength(100);
    }
}
