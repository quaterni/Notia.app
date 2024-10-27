using System.ComponentModel.DataAnnotations;

namespace Np.UsersService.Core.Models.Users;

public class User
{
    private User()
    {
    }

    public User(string username, Guid id, string identityId)
    {
        Username = username;
        Id = id;
        IdentityId = identityId;
    }

    [Required]   
    [MinLength(3)]
    public required string Username { get; set; }

    public required string Email { get; set; }

    public required Guid Id { get; init; }

    public required string IdentityId { get; init; }
}
