namespace Np.UsersService.Core.Dtos.Users;

public sealed record UpdatePasswordRequest(string OldPassword, string NewPassword);
