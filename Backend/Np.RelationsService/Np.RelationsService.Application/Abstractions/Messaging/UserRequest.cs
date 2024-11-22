
namespace Np.RelationsService.Application.Abstractions.Messaging;

public record UserRequest(string IdentityId)
{
    public Guid? UserId { get; set; }
}
