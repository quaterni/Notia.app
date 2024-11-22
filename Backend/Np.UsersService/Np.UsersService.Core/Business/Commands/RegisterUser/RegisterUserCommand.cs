using Np.UsersService.Core.Business.Abstractions;

namespace Np.UsersService.Core.Business.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string Username, 
    string Email, 
    string Password) : ICommand;
