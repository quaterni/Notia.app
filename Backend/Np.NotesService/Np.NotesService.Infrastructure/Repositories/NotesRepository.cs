using Microsoft.EntityFrameworkCore;
using Np.NotesService.Domain.Notes;

namespace Np.NotesService.Infrastructure.Repositories
{
    internal class NotesRepository : INotesRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public NotesRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Note note)
        {
            _dbContext.Add(note);
        }

        public void Delete(Note note)
        {
            _dbContext.Remove(note);
        }

        public async Task<Note?> GetNoteById(Guid id)
        {
            return await _dbContext.Set<Note>().FirstOrDefaultAsync(n=> n.Id.Equals(id));
        }

        public void Update(Note note)
        {
            _dbContext.Update(note);
        }
    }
}
