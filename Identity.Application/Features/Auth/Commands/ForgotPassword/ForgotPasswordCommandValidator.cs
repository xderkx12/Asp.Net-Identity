using FluentValidation;

namespace Identity.Application.Features.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Login).NotEmpty().MaximumLength(100);
    }
}
