namespace Np.UsersService.Core.Shared;

public record Error(string Name, string Description)
{
    public static Error NullError => new("NullError", "Application return null.");
}
