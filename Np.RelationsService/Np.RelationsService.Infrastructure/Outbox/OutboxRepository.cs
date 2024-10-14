
using Microsoft.EntityFrameworkCore;


namespace Np.RelationsService.Infrastructure.Outbox;

internal class OutboxRepository
{
    private readonly ApplicationDbContext _context;

    public OutboxRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OutboxEntry>> GetOrderedByRefreshTime(int limit)
    {
        return await _context
            .Set<OutboxEntry>()
            .OrderBy(o=> o.RefreshTime)
            .Take(limit)
            .ToListAsync();
    }

    public void Remove(OutboxEntry entry)
    {
        _context.Remove(entry);
    }

    public void Update(OutboxEntry entry)
    {
        _context.Update(entry);
    }
}
