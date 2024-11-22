
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Np.NotesService.Infrastructure.Messaging.Grpc;
using Np.RelationsService.Application.Abstractions.Users;
using Np.RelationsService.Domain.Abstractions;
using static Np.NotesService.Infrastructure.Messaging.Grpc.GrpcUsersService;

namespace Np.RelationsService.Infrastructure.Messaging.Grpc.Users;

internal class UsersService : IUsersService
{
    private readonly GrpcOptions _grpcOptions;

    public UsersService(IOptionsSnapshot<GrpcOptions> options)
    {
        _grpcOptions = options.Get(GrpcOptions.Users);
    }

    public async Task<Result<Guid>> GetUserIdAsync(string identityId, CancellationToken cancellationToken = default)
    {
        var channel = GrpcChannel.ForAddress(_grpcOptions.Address);
        var client = new GrpcUsersServiceClient(channel);

        try
        {
            var userInfo = await client.GetUserByIdentityIdAsync(
                new GetUserByIdentityIdRequest() { IdentityId = identityId },
                cancellationToken: cancellationToken);
            return Guid.Parse(userInfo.Id.Value);
        }
        catch (RpcException ex) 
        {
            switch (ex.StatusCode)
            {
                case StatusCode.NotFound:
                    return Result.Failed<Guid>(UsersServiceErrors.NotFound);
                default:
                    return Result.Failed<Guid>(Error.Undefined);
            }
        }
    }
}
