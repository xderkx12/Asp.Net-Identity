using FluentValidation;

namespace Identity.Application.Features.Sessions.Queries.GetMySessions;

public sealed class GetMySessionsQueryValidator : AbstractValidator<GetMySessionsQuery>
{
    public GetMySessionsQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
