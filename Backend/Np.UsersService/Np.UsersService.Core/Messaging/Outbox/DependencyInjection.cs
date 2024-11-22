using Np.UsersService.Core.Messaging.ModelEvents.Abstractions;
using Np.UsersService.Core.Messaging.Outbox.Options;

namespace Np.UsersService.Core.Messaging.Outbox;

public static class DependencyInjection
{
    public static void AddOutboxEntries(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OutboxOptions>(configuration.GetSection("OutboxOptions"));
        services.AddHostedService<OutboxWorker>();
        services.AddScoped<IModelEventService, OutboxModelEventService>();
    }
}
