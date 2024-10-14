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
        Task<bool> HasRelation(Note first, Note second);

        void AddRelation(Relation relation);

        Task<Relation?> GetRelationById(Guid id);

        Task<bool> HasOutgoingRelations(Guid outgoingNoteId);

        Task<IEnumerable<Relation>> GetOutgoingRelations(Guid outgoingNoteId);

        Task<IEnumerable<Relation>> GetIncomingRelations(Guid incomingNoteId);

        void RemoveRelation(Relation relation);
    }
}
