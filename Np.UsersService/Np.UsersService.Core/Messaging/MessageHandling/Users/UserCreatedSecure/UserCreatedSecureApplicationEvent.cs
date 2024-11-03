
namespace Np.UsersService.Core.Messaging.MessageHandling.Users.UserCreatedSecure;

public sealed record UserCreatedSecureApplicationEvent(string Username, string Email, string Password, Guid Id) : IApplicationEvent;
