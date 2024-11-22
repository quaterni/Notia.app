
using Microsoft.Extensions.DependencyInjection;
using Np.RelationsService.Application.Abstractions.Behaviors;

namespace Np.RelationsService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicaton(this IServiceCollection services)
    {
        services.AddMediatR(opt => 
        {
            opt.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            opt.AddOpenBehavior(typeof(LoggingBehavior<,>));
            opt.AddOpenBehavior(typeof(UserRequestBehavior<,>));
        });

        return services;
    }
}
