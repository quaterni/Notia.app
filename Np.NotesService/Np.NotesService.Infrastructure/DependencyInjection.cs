using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Np.NotesService.Application.Abstractions.Data;
using Np.NotesService.Application.Abstractions.Outbox;
using Np.NotesService.Application.Relations.Service;
using Np.NotesService.Application.Users;
using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Domain.Notes;
using Np.NotesService.Infrastructure.Authorization;
using Np.NotesService.Infrastructure.Data;
using Np.NotesService.Infrastructure.Messaging.Grpc;
using Np.NotesService.Infrastructure.Messaging.RabbitMq;
using Np.NotesService.Infrastructure.Outbox;
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

            services.AddScoped<IRelationsService, GrpcRelationsService>();

            AddPersistance(services, configuration);

            AddMessaging(services, configuration);
            AddAuthentication(services, configuration);
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
            services.Configure<GrpcOptions>(
                GrpcOptions.Relations, 
                configuration.GetRequiredSection("RelationsService:Grpc"));
            services.AddScoped<IRelationsService, GrpcRelationsService>();

            services.Configure<GrpcOptions>(
                GrpcOptions.Users,
                configuration.GetRequiredSection("UsersService:Grpc"));
            services.AddScoped<IUsersService, UsersService>();

            services.Configure<RabbitMqConnectionOptions>(
                configuration.GetRequiredSection("RabbitMq:Connection"));
            services.Configure<RabbitMqExchangeOptions>(
                configuration.GetRequiredSection("RabbitMq:Exchange"));
            services.AddSingleton<IRabbitMqChannelFactory, RabbitMqChannelFactory>();

            services.Configure<OutboxOptions>(
                configuration.GetRequiredSection("OutboxOptions"));
            services.AddScoped<OutboxRepository>();
            services.AddHostedService<OutboxWorker>();
        }

        private static void AddPersistance(
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
