
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Np.RelationsService.Infrastructure.Messaging.Grpc;

namespace Np.RelationsService.Infrastructure;

public static class GrpcEndpointsInjection
{
    public static IEndpointRouteBuilder MapGrpcServices(this IEndpointRouteBuilder app)
    {
        app.MapGrpcService<GrpcService>();

        return app;
    }
}
