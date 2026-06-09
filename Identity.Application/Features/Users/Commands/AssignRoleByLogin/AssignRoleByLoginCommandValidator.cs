using FluentValidation;

namespace Identity.Application.Features.Users.Commands.AssignRoleByLogin;

public sealed class AssignRoleByLoginCommandValidator : AbstractValidator<AssignRoleByLoginCommand>
{
    public AssignRoleByLoginCommandValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.RoleName)
            .NotEmpty()
            .MaximumLength(100);
    }
}
