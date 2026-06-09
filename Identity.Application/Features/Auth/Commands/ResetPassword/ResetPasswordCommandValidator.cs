using FluentValidation;

namespace Identity.Application.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.ResetToken).NotEmpty().MaximumLength(512);
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(256);
    }
}
