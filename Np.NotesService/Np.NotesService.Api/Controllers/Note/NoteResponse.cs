namespace Np.NotesService.Api.Controllers.Note
{
    public sealed record NoteResponse(
        string Title,
        string Content,
        DateTime CreateTime,
        DateTime LastUpdateTime,
        Guid Id);
}
