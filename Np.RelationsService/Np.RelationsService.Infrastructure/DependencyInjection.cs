using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Np.RelationsService.Application.Abstractions.Data;
using Np.RelationsService.Application.Abstractions.Messaging.Events;
using Np.RelationsService.Application.Services;
using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.Relations;
using Np.RelationsService.Domain.RootEntries;
using Np.RelationsService.Infrastructure.Data;
using Np.RelationsService.Infrastructure.Messaging.RabbitMq;
using Np.RelationsService.Infrastructure.Outbox;
using Np.RelationsService.Infrastructure.Repositories;

namespace Np.RelationsService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            AddPersistance(services, configuration);

            AddMessaging(services);

            return services;
        }

        private static void AddMessaging(IServiceCollection services)
        {
            services.AddGrpc();

            services.AddScoped<OutboxRepository>();
            services.AddSingleton<RabbitMqChannelFactory>();

            services.AddScoped<IEventProcessor, ApplicationEventService>();

            services.AddHostedService<MessageBusWorker>();
            services.AddHostedService<OutboxWorker>();
        }

        private static void AddPersistance(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("RelationsDb");
            if (connectionString == null) 
            { 
                throw new ArgumentNullException(connectionString);
            }

            services.AddScoped<INotesRepository, NotesRepository>();
            services.AddScoped<IRelationsRepository, RelationsRepository>();
            services.AddScoped<IRootEntriesRepository, RootEntriesRepository>();

            services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>(
                s => new SqlConnectionFactory(connectionString));

            services.AddDbContext<ApplicationDbContext>(
                opt => opt
                    .UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention());

            services.AddScoped<IUnitOfWork>(sp=> sp.GetRequiredService<ApplicationDbContext>());
        }
    }
}
