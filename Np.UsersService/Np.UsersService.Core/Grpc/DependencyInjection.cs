namespace Np.UsersService.Core.Grpc;

public static class DependencyInjection
{
    public static IEndpointRouteBuilder MapGrpcServices(this IEndpointRouteBuilder app)
    {
        app.MapGrpcService<UsersService>();

        return app;
    }
}
