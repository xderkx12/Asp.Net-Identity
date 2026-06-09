using FluentValidation;

namespace Identity.Application.Features.Users.Queries.GetUsersByRole;

public sealed class GetUsersByRoleQueryValidator : AbstractValidator<GetUsersByRoleQuery>
{
    public GetUsersByRoleQueryValidator()
    {
        RuleFor(x => x.RoleName).NotEmpty().MaximumLength(100);
    }
}
