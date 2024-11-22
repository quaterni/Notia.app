
namespace Np.NotesService.Application.Dtos;

public sealed record RelationItem(Guid Id, NoteItemDto OutgoingItem, NoteItemDto IncomingNote);
