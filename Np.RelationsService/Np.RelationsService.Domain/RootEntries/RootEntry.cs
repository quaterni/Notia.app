using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Domain.Notes;

namespace Np.RelationsService.Domain.RootEntries
{
    public class RootEntry : Entity
    {
        public Note Note { get; }

        private RootEntry() : base()
        {
        }

        protected RootEntry(Note note, Guid id) :base(id)
        {
            Note = note;
        }

        /// <summary>
        /// Create root entry
        /// </summary>
        /// <param name="note">Note that is add to root entry</param>
        /// <returns>Result of root entry</returns>
        public static Result<RootEntry> Create(Note note)
        {
            return new RootEntry(note, Guid.NewGuid());
        }
    }
}
