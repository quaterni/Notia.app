using Np.RelationsService.Domain.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.RelationsService.Domain.Relations
{
    public interface IRelationsRepository
    {
        Task<bool> HasRelation(Note first, Note second, CancellationToken cancellationToken = default);

        void AddRelation(Relation relation);

        Task<Relation?> GetRelationById(Guid id, CancellationToken cancellationToken = default);
        Task<Relation?> GetRelationByNotes(Guid incomingNoteId, Guid outgoingNoteId, CancellationToken cancellationToken = default);

        Task<bool> HasOutgoingRelations(Guid outgoingNoteId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Relation>> GetOutgoingRelations(Guid outgoingNoteId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Relation>> GetIncomingRelations(Guid incomingNoteId, CancellationToken cancellationToken = default);

        void RemoveRelation(Relation relation);
    }
}
