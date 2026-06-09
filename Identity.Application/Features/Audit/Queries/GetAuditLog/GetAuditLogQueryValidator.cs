using FluentValidation;

namespace Identity.Application.Features.Audit.Queries.GetAuditLog;

public sealed class GetAuditLogQueryValidator : AbstractValidator<GetAuditLogQuery>
{
    public GetAuditLogQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 500);
        RuleFor(x => x.Action).MaximumLength(100);
        RuleFor(x => x.ActorLogin).MaximumLength(100);
    }
}
