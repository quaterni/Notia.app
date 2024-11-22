
using Microsoft.EntityFrameworkCore;
using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.RootEntries;

namespace Np.RelationsService.Infrastructure.Repositories;

internal class RootEntriesRepository : IRootEntriesRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RootEntriesRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(RootEntry rootEntry)
    {
        _dbContext.Add(rootEntry);
    }

    public async Task<RootEntry?> GetRootEntryById(Guid rootEntryId)
    {
        return await _dbContext.Set<RootEntry>()
            .FirstOrDefaultAsync(r=> r.Id.Equals(rootEntryId));
    }

    public async Task<RootEntry?> GetRootEntryByNoteId(Guid noteId)
    {
        return await _dbContext.Set<RootEntry>()
            .FirstOrDefaultAsync(r => r.Note.Id.Equals(noteId));
    }

    public async Task<bool> IsEntryRoot(Note note, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<RootEntry>()
            .AnyAsync(r=> r.Note.Id.Equals(note.Id));
    }

    public void Remove(RootEntry rootEntry)
    {
        _dbContext.Remove(rootEntry);
    }
}
