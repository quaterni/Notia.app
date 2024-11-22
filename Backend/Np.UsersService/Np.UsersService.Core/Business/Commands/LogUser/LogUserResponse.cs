using Np.UsersService.Core.Authentication.Models;

namespace Np.UsersService.Core.Business.Commands.LogUser;

public sealed record LogUserResponse(AuthorizationToken Token);