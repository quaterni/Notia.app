
using Microsoft.EntityFrameworkCore;
using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.Relations;

namespace Np.RelationsService.Infrastructure.Repositories;

internal class RelationsRepository : IRelationsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RelationsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void AddRelation(Relation relation)
    {
        _dbContext.Add(relation);
    }

    public async Task<IEnumerable<Relation>> GetIncomingRelations(Guid incomingNoteId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Relation>()
            .Include(r=> r.Incoming)
            .Include(r=> r.Outgoing)
            .Where(r=> r.Incoming.Id.Equals(incomingNoteId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Relation>> GetOutgoingRelations(Guid outgoingNoteId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Relation>()
            .Include(r => r.Incoming)
            .Include(r => r.Outgoing)
            .Where(r => r.Outgoing.Id.Equals(outgoingNoteId))
            .ToListAsync();
    }

    public async Task<Relation?> GetRelationById(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Relation>()
            .Include(r => r.Incoming)
            .Include(r => r.Outgoing)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Relation?> GetRelationByNotes(Guid incomingNoteId, Guid outgoingNoteId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Relation>()
            .Include(r => r.Incoming)
            .Include(r => r.Outgoing)
            .Where(r => r.Incoming.Id.Equals(incomingNoteId) && r.Outgoing.Id.Equals(outgoingNoteId))
            .FirstOrDefaultAsync();
    }

    public async Task<bool> HasOutgoingRelations(Guid outgoingNoteId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Relation>()
            .Where(r=> r.Outgoing.Id.Equals(outgoingNoteId))
            .AnyAsync();
    }

    public async Task<bool> HasRelation(Note first, Note second, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Relation>()
            .AnyAsync(
            r=> (r.Incoming.Equals(first) && r.Outgoing.Equals(second)) || 
            (r.Incoming.Equals(second) && r.Outgoing.Equals(first)));
    }

    public void RemoveRelation(Relation relation)
    {
        _dbContext.Remove(relation);
    }
}
