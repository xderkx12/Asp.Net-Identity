using FluentValidation;

namespace Identity.Application.Features.Users.Commands.BlockUserByLogin;

public sealed class BlockUserByLoginCommandValidator : AbstractValidator<BlockUserByLoginCommand>
{
    public BlockUserByLoginCommandValidator()
    {
        RuleFor(x => x.Login).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LockedUntilUtc)
            .Must(value => value is null || value > DateTime.UtcNow)
            .WithMessage("LockedUntilUtc must be in the future or null for a permanent block.");
        RuleFor(x => x.Reason).MaximumLength(500);
    }
}
