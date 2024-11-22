using Np.RelationsService.Domain.Abstractions;

namespace Np.RelationsService.Domain.Notes
{
    public class Note : Entity
    {
        private Note() : base() 
        { 
        }

        protected Note(Guid id, Guid userId) : base(id)
        {
            UserId = userId;
        }

        public Guid UserId { get; }

        /// <summary>
        /// Create new note 
        /// </summary>
        /// <param name="noteId">Note id</param>
        /// <returns>Result with note</returns>
        public static Result<Note> Create(Guid noteId, Guid userId)
        {
            return new Note(noteId, userId);
        }
    }
}
