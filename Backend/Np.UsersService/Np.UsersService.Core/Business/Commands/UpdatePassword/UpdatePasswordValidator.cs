using FluentValidation;

namespace Np.UsersService.Core.Business.Commands.UpdatePassword;

public class UpdatePasswordValidator : AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordValidator()
    {
        RuleFor(x=> x.OldPassword).NotEmpty().MinimumLength(3);
        RuleFor(x=> x.NewPassword).NotEmpty().MinimumLength(3);
        RuleFor(x=> x.NewPassword).NotEqual(x=> x.OldPassword).WithMessage("Password must not be equal");
    }
}
