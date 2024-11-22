namespace Np.UsersService.Core.Authentication.Keycloak.Options;

public class RealmUserCreationOptions
{
    public required string FirstNamePlaceholder { get; set; }
    public required string LastNamePlaceholder { get; set; }
    public bool Enabled { get; set; }
    public bool EmailVerified { get; set; }
    public bool TemporaryCredentials { get; set; }
}
