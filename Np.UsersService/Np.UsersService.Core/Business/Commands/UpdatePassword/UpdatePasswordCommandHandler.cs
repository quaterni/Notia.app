using Np.UsersService.Core.Authentication.Abstractions;
using Np.UsersService.Core.Authentication.Errors;
using Np.UsersService.Core.Authentication.Models;
using Np.UsersService.Core.Business.Abstractions;
using Np.UsersService.Core.Exceptions;
using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Business.Commands.UpdatePassword;

public class UpdatePasswordCommandHandler : ICommandHandler<UpdatePasswordCommand>
{
    private readonly ITokenService _tokenService;
    private readonly IIdentityService _identityService;

    public UpdatePasswordCommandHandler(
        ITokenService tokenService,
        IIdentityService identityService)
    {
        _tokenService = tokenService;
        _identityService = identityService;
    }

    public async Task<Result> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        if (request.NewPassword.Equals(request.OldPassword))
        {
            return Result.Failure(UpdatePasswordErrors.SamePasswordError);
        }
        var user = request.User!;
     
        var tokenResult = await _tokenService.GetTokenByUserCredentials(new UserCredentials(user.Username, request.OldPassword), cancellationToken);
        if(tokenResult.IsFailed && tokenResult.Error.Equals(TokenErrors.UnauthorizedError))
        {
            return Result.Failure(UpdatePasswordErrors.InvalidOldPasswordError);
        }

        var updatedPasswordResult = await _identityService.UpdateUserPassword(user.IdentityId!, request.NewPassword);
        if (updatedPasswordResult.IsFailed)
        {
            throw new UnhandledErrorException(updatedPasswordResult.Error);
        }
        return Result.Success();
    }
}
