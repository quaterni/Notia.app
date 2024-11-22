namespace Np.NotesService.Api.Controllers.Notes
{
    public sealed record NoteResponse(
        string Title,
        string Content,
        DateTime CreateTime,
        DateTime LastUpdateTime,
        Guid Id);
}
