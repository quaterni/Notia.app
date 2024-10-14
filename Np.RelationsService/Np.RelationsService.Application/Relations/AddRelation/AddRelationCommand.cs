using Np.RelationsService.Application.Abstractions.Messaging;

namespace Np.RelationsService.Application.Relations.AddRelation
{
    public sealed record AddRelationCommand(Guid OutgoingNoteId, Guid IncomingNoteId) : ICommand;
}
