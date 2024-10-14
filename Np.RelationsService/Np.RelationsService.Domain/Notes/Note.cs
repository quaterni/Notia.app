using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Domain.Relations;
using Np.RelationsService.Domain.RootEntries;
using Np.RelationsService.Domain.RootEntries;
using System.Linq;

namespace Np.RelationsService.Domain.Notes
{
    public class Note : Entity
    {
        private Note() : base() 
        { 
        }

        protected Note(Guid id) : base(id)
        {            
        }

        /// <summary>
        /// Create new note 
        /// </summary>
        /// <param name="id">Note id</param>
        /// <returns>Result with note</returns>
        public static Result<Note> Create(Guid id)
        {
            return new Note(id);
        }
    }
}
