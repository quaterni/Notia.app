using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Infrastructure.Outbox;
using System.Text.Json;


namespace Np.RelationsService.Infrastructure
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var outboxes = ChangeTracker.Entries<Entity>()
                .Select(e => e.Entity
                ).SelectMany(e =>
                {
                    var domainEvents = e.DomainEvents;
                    e.ClearDomainEvents();
                    return domainEvents;
                })
                .Select(e => new OutboxEntry
                {
                    Id = Guid.NewGuid(),
                    Created = DateTime.UtcNow,
                    RefreshTime = DateTime.UtcNow,
                    EventName = e.GetType().Name,
                    Data = JsonConvert.SerializeObject(e)
                })
                .ToList();

            Set<OutboxEntry>().AddRange(outboxes);

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
