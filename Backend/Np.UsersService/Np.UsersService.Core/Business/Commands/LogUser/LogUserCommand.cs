using Np.UsersService.Core.Business.Abstractions;

namespace Np.UsersService.Core.Business.Commands.LogUser;

public sealed record LogUserCommand(string Username, string Password) : ICommand<LogUserResponse>;
