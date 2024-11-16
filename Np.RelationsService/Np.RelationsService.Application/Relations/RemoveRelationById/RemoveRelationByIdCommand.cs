using Np.RelationsService.Application.Abstractions.Messaging;

namespace Np.RelationsService.Application.Relations.RemoveRelationById
{
    public sealed record  RemoveRelationByIdCommand(Guid RelationId, string IdentityId) : UserRequest(IdentityId), ICommand;
}
