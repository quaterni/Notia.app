using Microsoft.EntityFrameworkCore;
using Np.NotesService.Domain.Abstractions;

namespace Np.NotesService.Infrastructure
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch(DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
