using Np.RelationsService.Application.Abstractions.Messaging;

namespace Np.RelationsService.Application.Relations.RemoveRelation
{
    public sealed record  RemoveRelationCommand(Guid RelationId, string IdentityId) : UserRequest(IdentityId), ICommand;
}
