using FluentValidation;
using Microsoft.IdentityModel.Tokens;

namespace Np.UsersService.Core.Business.Commands.UpdateUserData;

public class UpdateUserValidator : AbstractValidator<UpdateUserDataCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().When(x=> x.Email != null);
        RuleFor(x => x.Username).MinimumLength(3).When(x=> x.Username != null);
    }
}
