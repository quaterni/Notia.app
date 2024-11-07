using FluentValidation;
using Np.UsersService.Core.Business.Behaviors;

namespace Np.UsersService.Core.Business;

public static class DependencyInjection
{
    public static void AddBusiness(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        services.AddMediatR(opt =>
        {
            opt.AddOpenBehavior(typeof(LoggingBehavoir<,>));
            opt.AddOpenBehavior(typeof(ValidationBehavior<,>));

            opt.RegisterServicesFromAssemblyContaining<Program>();
        });
    }
}
