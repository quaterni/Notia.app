using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Np.NotesService.Application.Abstractions.Outbox;
using Np.NotesService.Domain.Abstractions;

namespace Np.NotesService.Infrastructure
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public ApplicationDbContext(
            DbContextOptions options,
            IDateTimeProvider dateTimeProvider) : base(options)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var domaintEvents= ChangeTracker.Entries<Entity>()
                .Select(e=> e.Entity)
                .SelectMany(e=>
                {
                    var domainEvents = e.DomainEvents;
                    e.ClearDomainEvents();
                    return domainEvents;
                })
                .Select(e=> new OutboxEntry
                {
                    Id = Guid.NewGuid(),
                    Created = _dateTimeProvider.GetCurrentTime(),
                    RefreshTime = _dateTimeProvider.GetCurrentTime(),
                    EventName = e.GetType().Name,
                    Data = JsonConvert.SerializeObject(e)
                })
                .ToList();

            Set<OutboxEntry>().AddRange(domaintEvents);

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
