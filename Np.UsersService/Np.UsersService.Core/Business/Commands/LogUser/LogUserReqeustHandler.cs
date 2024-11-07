using Np.UsersService.Core.Authentication.Abstractions;
using Np.UsersService.Core.Authentication.Errors;
using Np.UsersService.Core.Authentication.Models;
using Np.UsersService.Core.Business.Abstractions;
using Np.UsersService.Core.Exceptions;
using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Business.Commands.LogUser;

public class LogUserReqeustHandler : ICommandHandler<LogUserRequest, LogUserResponse>
{
    private readonly ITokenService _tokenService;

    public LogUserReqeustHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<Result<LogUserResponse>> Handle(LogUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _tokenService.GetTokenByUserCredentials(new UserCredentials(request.Username, request.Password));

        if (result.IsFailed && result.Error.Equals(TokenErrors.UnauthorizedError)) 
        {
            return Result.Failure<LogUserResponse>(LogUserErrors.UserUnathorized);
        }

        if (result.IsFailed)
        {
            throw new UnhandledErrorException(result.Error);
        }

        return new LogUserResponse(result.Value);
    }
}
