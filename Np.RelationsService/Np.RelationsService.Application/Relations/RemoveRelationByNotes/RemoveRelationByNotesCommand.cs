
using Np.RelationsService.Application.Abstractions.Messaging;

namespace Np.RelationsService.Application.Relations.RemoveRelationByNotes;

public record RemoveRelationByNotesCommand(Guid IncomingNote, Guid OutgoingNote, string IdentityId) : UserRequest(IdentityId), ICommand;