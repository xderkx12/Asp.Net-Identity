using FluentValidation;

namespace Identity.Application.Features.Sessions.Commands.RevokeSession;

public sealed class RevokeSessionCommandValidator : AbstractValidator<RevokeSessionCommand>
{
    public RevokeSessionCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.SessionId).NotEmpty();
    }
}
