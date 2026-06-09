using FluentValidation;

namespace Identity.Application.Features.Roles.Commands.RenameRole;

public sealed class RenameRoleCommandValidator : AbstractValidator<RenameRoleCommand>
{
    public RenameRoleCommandValidator()
    {
        RuleFor(x => x.CurrentName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.NewName).NotEmpty().MaximumLength(100);
    }
}
