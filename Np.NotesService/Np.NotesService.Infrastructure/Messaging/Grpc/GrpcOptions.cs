
namespace Np.NotesService.Infrastructure.Messaging.Grpc;

internal class GrpcOptions
{
    public const string Relations = nameof(Relations);

    public required string Address { get; init; }
}
