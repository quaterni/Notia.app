
using Np.NotesService.Application.Dtos;

namespace Np.NotesService.Application.Relations;

public interface IRelationsService
{
    Task<IEnumerable<NoteItemDto>> GetNotesFromRoot(Guid userId, CancellationToken cancellationToken);

    Task<IEnumerable<NoteItemDto>> GetOutgoingNotes(Guid noteId, CancellationToken cancellationToken);

    Task<IEnumerable<NoteItemDto>> GetIncomingNotes(Guid noteId, CancellationToken cancellationToken);
}