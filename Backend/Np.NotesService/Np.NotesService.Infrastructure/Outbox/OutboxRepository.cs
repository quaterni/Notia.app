
using Microsoft.EntityFrameworkCore;
using Np.NotesService.Application.Abstractions.Outbox;

namespace Np.NotesService.Infrastructure.Outbox;

internal class OutboxRepository 
{
    private readonly ApplicationDbContext _dbContext;

    public OutboxRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(OutboxEntry entry)
    {
        _dbContext.Add(entry);
    }

    public async Task<IEnumerable<OutboxEntry>> GetEntriesOrderedByRefreshTime(int limit)
    {
        return await _dbContext.Set<OutboxEntry>()
            .OrderBy(e=> e.RefreshTime)
            .Take(limit)
            .ToListAsync();
    }

    public void Remove(OutboxEntry entry)
    {
        _dbContext.Remove(entry);
    }

    public void Update(OutboxEntry entry)
    {
        _dbContext.Update(entry);
    }
}
