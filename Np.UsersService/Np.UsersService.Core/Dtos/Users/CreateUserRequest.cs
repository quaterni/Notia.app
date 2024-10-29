namespace Np.UsersService.Core.Dtos.Users;

public sealed record CreateUserRequest(string Username, string Email, string Password);