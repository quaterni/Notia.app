using System.Text.Json.Serialization;

namespace Np.UsersService.Core.Dtos.Users;

public class UserView
{
    public string Username { get; set; }

    public string Email { get; set; }

    [JsonPropertyName("id")]
    public string IdentityId { get; set; }
}
