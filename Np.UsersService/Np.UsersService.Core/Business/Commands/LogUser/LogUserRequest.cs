using Np.UsersService.Core.Business.Abstractions;

namespace Np.UsersService.Core.Business.Commands.LogUser;

public sealed record LogUserRequest(string Username, string Password) : ICommand<LogUserResponse>;
