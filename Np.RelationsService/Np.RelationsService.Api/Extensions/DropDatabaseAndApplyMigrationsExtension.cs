using Microsoft.EntityFrameworkCore;
using Np.RelationsService.Infrastructure;

namespace Np.RelationsService.Api.Extensions
{
    public static class DropDatabaseAndApplyMigrationsExtension
    {
        public static IApplicationBuilder DropDatabaseAndApplyMigrations(this IApplicationBuilder app)
        {
            var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            Console.WriteLine("-- Drop database.");

            dbContext.Database.EnsureDeleted();

            Console.WriteLine("-- Applying migrations.");

            dbContext.Database.Migrate();
            Console.WriteLine("-- Migrations has applied.");

            return app;
        }
    }
}
