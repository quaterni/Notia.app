using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Domain.Notes;

namespace Np.RelationsService.Domain.Relations
{
    public class Relation : Entity
    {
        public Note Outgoing { get; } = null!;

        public Note Incoming { get; } = null!;

        private Relation() :base()
        {
        }

        protected Relation(Note outgoing, Note incoming, Guid id) : base(id)
        {
            Outgoing = outgoing;
            Incoming = incoming;
        }

        /// <summary>
        /// Create relation between two notes
        /// </summary>
        /// <remarks>
        /// Result fails if
        /// <br/>- note incoming itself
        /// </remarks>
        /// <param name="outgoing">Outgoing note</param>
        /// <param name="incoming">Incoming note</param>
        /// <returns>Result of relation</returns>
        public static Result<Relation> Create(Note outgoing, Note incoming)
        {
            if (outgoing.Equals(incoming))
            {
                return Result.Failed<Relation>(RelationErrors.ItselfRelation);
            }

            return new Relation(outgoing, incoming, Guid.NewGuid());
        }
    }
}
