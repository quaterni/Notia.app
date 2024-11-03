using System.ComponentModel.DataAnnotations;

namespace Np.UsersService.Core.Models.Users;

public class User
{
    private User()
    {
    }

    public User(string username, string email, Guid id)
    {
        Username = username;
        Id = id;
        Email = email;
    }

    [Required]   
    [MinLength(3)]
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public Guid Id { get; init; }

    public string? IdentityId { get; set; }

    public bool IsSyncrhonizedWithIdentity { get; set; }
}
