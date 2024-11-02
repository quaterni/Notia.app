namespace Np.UsersService.Core.Messaging.MessageHandling;

public static class DependencyInjection
{
    public static void AddMessageHandling(this IServiceCollection services)
    {
        services.AddScoped<MessageHandler>();
    }
}
