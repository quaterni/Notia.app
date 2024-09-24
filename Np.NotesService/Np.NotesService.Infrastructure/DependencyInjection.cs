using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Np.NotesService.Application.Abstractions.Data;
using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Domain.Notes;
using Np.NotesService.Infrastructure.Data;
using Np.NotesService.Infrastructure.Repositories;
using Np.NotesService.Infrastructure.Time;


namespace Np.NotesService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();

            AddPersistance(services, configuration);
            return services;
        }

        public static void AddPersistance(
            IServiceCollection services, 
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("NotesDb") ?? throw new ArgumentNullException("connectionString");

            services.AddScoped<INotesRepository, NotesRepository>();

            services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString));

            services.AddDbContext<ApplicationDbContext>(
                options=> options
                    .UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention());
            services.AddScoped<IUnitOfWork>(sp=> sp.GetRequiredService<ApplicationDbContext>());

        }
    }
}
