using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Domain.Notes;
using Np.NotesService.Infrastructure;

namespace Np.NotesService.Api.Extensions
{
    public static class SeedDataIfEmptyExtension
    {
        class SeedDateTimeProvider(DateTime currentTime) : IDateTimeProvider
        {
            public DateTime GetCurrentTime()
            {
                return currentTime;
            }
        }

        public static IApplicationBuilder SeedDataIfEmpty(this IApplicationBuilder app) 
        {
            var scope = app.ApplicationServices.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if(dbContext.Set<Note>().FirstOrDefault() is not null)
            {
                return app;
            }

            Console.WriteLine("-- Starting seed data.");

            List<Note> notes = new List<Note>()
            {
                Note.Create("Note #1", Guid.Empty, new SeedDateTimeProvider(new DateTime(2024, 9, 18, 22, 00, 16, DateTimeKind.Unspecified))),
                Note.Create("Note #2", Guid.Empty, new SeedDateTimeProvider(new DateTime(2024, 9, 18, 22, 12, 59, DateTimeKind.Unspecified))),
                Note.Create("Note #1.1", Guid.Empty, new SeedDateTimeProvider(new DateTime(2024, 9, 18, 22,  5, 12, DateTimeKind.Unspecified))),
                Note.Create("Note #1.2", Guid.Empty, new SeedDateTimeProvider(new DateTime(2024, 9, 18, 22, 8, 44, DateTimeKind.Unspecified))),
                Note.Create("Note #1.2.1", Guid.Empty, new SeedDateTimeProvider(new DateTime(2024, 9, 18, 22, 11, 37, DateTimeKind.Unspecified))),
            };

            dbContext.Set<Note>().AddRange(notes);
            dbContext.SaveChanges();

            Console.WriteLine("-- Seed data added.");

            return app;
        }
    }
}
