
namespace Np.NotesService.Infrastructure.Messaging.Grpc;

internal class GrpcOptions
{
    public const string Relations = nameof(Relations);

    public const string Users = nameof(Users);

    public required string Address { get; init; }
}
