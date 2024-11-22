
namespace Np.RelationsService.Infrastructure.Messaging.Grpc;

internal class GrpcOptions 
{
    public const string Users = nameof(Users);

    public required string Address { get; init; }
}
