using Microsoft.Extensions.DependencyInjection;
using Np.NotesService.Application.Abstractions.Mediator.Behaviors;

namespace Np.NotesService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(opt => 
            { 
                opt.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

                opt.AddOpenBehavior(typeof(LoggingBehavoir<,>));
                opt.AddOpenBehavior(typeof(UserRequestBehavior<,>));
            });
            return services;
        }
    }
}
