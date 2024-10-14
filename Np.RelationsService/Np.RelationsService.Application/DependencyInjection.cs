
using Microsoft.Extensions.DependencyInjection;

namespace Np.RelationsService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicaton(this IServiceCollection services)
    {
        services.AddMediatR(
            opt => opt.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        return services;
    }
}
