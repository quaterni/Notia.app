using System.Collections;

namespace Np.NotesService.Application.Relations.Service;

public interface IRelationsService
{
    Task<IEnumerable<NoteResponse>> GetNotesFromRoot(CancellationToken cancellationToken);

    Task<IEnumerable<RelationResponse>> GetOutgoingRelations(Guid noteId, CancellationToken cancellationToken);

    Task<IEnumerable<RelationResponse>> GetIncomingRelations(Guid noteId, CancellationToken cancellationToken);
}