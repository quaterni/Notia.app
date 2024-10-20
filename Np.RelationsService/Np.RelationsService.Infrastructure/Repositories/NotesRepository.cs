using Microsoft.EntityFrameworkCore;
using Np.RelationsService.Domain.Notes;

namespace Np.RelationsService.Infrastructure.Repositories;

internal class NotesRepository : INotesRepository
{
    private readonly ApplicationDbContext _dbContext;

    public NotesRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Note note)
    {
        _dbContext.Add(note);
    }

    public async Task<bool> Contains(Guid id)
    {
        return await _dbContext.Set<Note>().AnyAsync(n=> n.Id.Equals(id));
    }

    public async Task<Note?> GetNoteById(Guid id)
    {
        return await _dbContext.Set<Note>().FirstOrDefaultAsync(n => n.Id.Equals(id));
    }

    public void Remove(Note note)
    {
        _dbContext.Set<Note>().Remove(note);
    }
}

