
namespace Np.NotesService.Application.Notes.GetNote;

public class GetNoteResponse
{
    public string Title { get; init; }
    public string Content { get; init; }
    public DateTime CreateTime { get; init; }
    public DateTime LastUpdateTime { get; init; }
    public Guid Id { get; init; }
}

