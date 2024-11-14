
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Np.NotesService.Application.Users;
using Np.NotesService.Domain.Abstractions;
using static Np.NotesService.Infrastructure.Messaging.Grpc.GrpcUsersService;

namespace Np.NotesService.Infrastructure.Messaging.Grpc;

internal class UsersService : IUsersService
{
    private GrpcOptions _options;

    public UsersService(IOptionsSnapshot<GrpcOptions> options)
    {
        _options = options.Get(GrpcOptions.Users);
    }

    public async Task<Result<Guid>> GetUserIdAsync(string identityId)
    {
        var channel = GrpcChannel.ForAddress(_options.Address);
        var client = new GrpcUsersServiceClient(channel);

        try
        {
            var userResponse = await client.GetUserByIdentityIdAsync(new GetUserByIdentityIdRequest() { IdentityId = identityId });

            var userId = Guid.Parse(userResponse.Id.Value);
            return userId;
        }
        catch (RpcException ex)
        {
            switch (ex.StatusCode)
            {
                case StatusCode.NotFound:
                    return Result.Failure<Guid>(ex.Message);
                default:
                    throw new ApplicationException("Unhandled rpc exception", ex);
            }
        }
    }
}
