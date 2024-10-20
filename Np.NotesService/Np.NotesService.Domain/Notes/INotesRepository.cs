using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.NotesService.Domain.Notes
{
    public interface INotesRepository
    {
        public void Add(Note note);

        public void Update(Note note);

        public void Delete(Note note);

        public Task<Note?> GetNoteById(Guid id);
    }
}
