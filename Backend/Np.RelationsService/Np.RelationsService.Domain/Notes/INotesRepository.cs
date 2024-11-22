using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.RelationsService.Domain.Notes
{
    public interface INotesRepository
    {
        Task<bool> Contains(Guid id);

        void Add(Note note);

        void Remove(Note note);

        Task<Note?> GetNoteById(Guid id);
    }
}
