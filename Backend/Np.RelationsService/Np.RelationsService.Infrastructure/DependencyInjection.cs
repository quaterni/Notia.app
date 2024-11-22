using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Np.RelationsService.Application.Abstractions.Data;
using Np.RelationsService.Application.Abstractions.Messaging.Events;
using Np.RelationsService.Application.Abstractions.Users;
using Np.RelationsService.Application.Services;
using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.Relations;
using Np.RelationsService.Domain.RootEntries;
using Np.RelationsService.Infrastructure.Data;
using Np.RelationsService.Infrastructure.Messaging.Grpc.Users;
using Np.RelationsService.Infrastructure.Messaging.Grpc;
using Np.RelationsService.Infrastructure.Messaging.RabbitMq;
using Np.RelationsService.Infrastructure.Messaging.RabbitMq.Options;
using Np.RelationsService.Infrastructure.Outbox;
using Np.RelationsService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Np.RelationsService.Infrastructure.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace Np.RelationsService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddAuthentication(services, configuration);
            AddPersistance(services, configuration);
            AddMessaging(services, configuration);

            return services;
        }


        private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var configurationOptions = new AuthenticationOptions();
                    configuration.GetSection("Authentication").Bind(configurationOptions);
                    options.Audience = configurationOptions.Audience;
                    options.MetadataAddress = configurationOptions.MetadataAddress;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = configurationOptions.ValidIssuer
                    };
                });
        }

        private static void AddMessaging(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConnectionOptions>(configuration.GetSection("RabbitMq:Connection"));
            services.Configure<QueueOptions>(configuration.GetSection("RabbitMq:Queue"));
            services.Configure<ExchangeOptions>(configuration.GetSection("RabbitMq:Exchange"));

            services.AddGrpc();

            services.AddScoped<OutboxRepository>();
            services.AddSingleton<RabbitMqChannelFactory>();

            services.AddScoped<IEventProcessor, ApplicationEventService>();

            services.AddHostedService<MessageBusWorker>();
            services.AddHostedService<OutboxWorker>();

            services.Configure<GrpcOptions>(
                GrpcOptions.Users,
                configuration.GetRequiredSection("UsersService:Grpc"));
            services.AddScoped<IUsersService, UsersService>();
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
