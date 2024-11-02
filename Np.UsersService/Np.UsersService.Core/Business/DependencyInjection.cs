using Np.UsersService.Core.Business.Behaviors;

namespace Np.UsersService.Core.Business;

public static class DependencyInjection
{
    public static void AddBusiness(this IServiceCollection services)
    {
        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssemblyContaining<Program>();
        });
    }
}
