using FluentValidation;

namespace Np.UsersService.Core.Business.Commands.RegisterUser;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Username).MinimumLength(3);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(3);
    }
}
