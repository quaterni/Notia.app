using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.Relations;
using Np.RelationsService.Domain.RootEntries;
using Np.RelationsService.Infrastructure;

namespace Np.RelationsService.Api.Extensions
{
    public static class SeedDataExtension
    {
        static NoteEntry One => new("Note #1", new Guid("5a62f186-c789-4c66-ba7b-eff732f35745"));

        static NoteEntry OneOne => new("Note #1.1", new Guid("5d997276-355c-4540-a88e-25ff5d7021b1"));

        static NoteEntry OneTwo => new("Note #1.2", new Guid("73756fe8-3402-4f2b-a0d6-956540442380"));

        static NoteEntry OneTwoOne => new("Note #1.2.1", new Guid("32782f11-3b34-4b61-9e9c-8c141051bbcf"));

        static NoteEntry Two => new("Note #2", new Guid("43273ee6-74ce-4e0e-b59c-544a1a642c99"));

        public static IApplicationBuilder SeedData(this IApplicationBuilder app) 
        {
            using var scope = app.ApplicationServices.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            Console.WriteLine("--> Start seeding data");


            if (dbContext.Set<Note>().Any())
            {
                Console.WriteLine("--> Database contains data, seeding stopped");
                return app;
            }


            var one = Note.Create(One.Id).Value;
            var oneOne = Note.Create(OneOne.Id).Value;
            var oneTwo = Note.Create(OneTwo.Id).Value;
            var oneTwoOne = Note.Create(OneTwoOne.Id).Value;
            var two = Note.Create(Two.Id).Value;

            Console.WriteLine("--> Seeding notes");


            dbContext.Set<Note>().AddRange([
                one,
                oneOne,
                oneTwo,
                oneTwoOne,
                two
            ]);

            Console.WriteLine("--> Seeding root entries");

            dbContext.Set<RootEntry>().AddRange([
                RootEntry.Create(one).Value,
                RootEntry.Create(two).Value
            ]);

            Console.WriteLine("--> Seeding relations");


            dbContext.Set<Relation>().AddRange([
                Relation.Create(oneOne, one).Value,
                Relation.Create(oneTwo, one).Value,
                Relation.Create(oneTwoOne, oneTwo).Value,
            ]);

            Console.WriteLine("--> Saving changes");

            dbContext.SaveChanges();

            Console.WriteLine("--> Seeding data done");

            return app;
        }

        record NoteEntry(string Title, Guid Id);
    }
}
