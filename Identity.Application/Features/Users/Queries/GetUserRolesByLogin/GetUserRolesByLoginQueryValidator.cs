using FluentValidation;

namespace Identity.Application.Features.Users.Queries.GetUserRolesByLogin;

public sealed class GetUserRolesByLoginQueryValidator : AbstractValidator<GetUserRolesByLoginQuery>
{
    public GetUserRolesByLoginQueryValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .MaximumLength(100);
    }
}
