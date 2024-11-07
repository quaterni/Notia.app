using FluentValidation;

namespace Np.UsersService.Core.Business.Commands.LogUser;

public class LogUserValidator : AbstractValidator<LogUserCommand>
{
    public LogUserValidator()
    {
        RuleFor(x=> x.Username).NotEmpty().MinimumLength(3);
        RuleFor(x=> x.Password).NotEmpty();
    }
}
