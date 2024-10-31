using Np.UsersService.Core.Messaging.RabbitMq.Options;

namespace Np.UsersService.Core.Messaging.RabbitMq;

public static class DependencyInjection
{
    public static void AddRabbitMqMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqConnecitonOptions>(configuration.GetSection("RabbitMq:ConnectionOptions"));
        services.Configure<RabbitMqExchangeOptions>(configuration.GetSection("RabbitMq:ExchangeOptions"));

        services.AddSingleton<RabbitMqChannelFactory>();
    }
}
