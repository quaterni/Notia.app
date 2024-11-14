
namespace Np.NotesService.Application.Abstractions.Mediator;

public record UserRequest(string IdentityId)
{
    public Guid? UserId { get; set; } = null;
}
