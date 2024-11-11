using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Np.UsersService.Core.Data;
using Np.UsersService.Core.Models.Users;

namespace Np.UsersService.Core.Grpc;

public class UsersService : GrpcUsersService.GrpcUsersServiceBase
{
    private readonly ApplicationDbContext _dbContext;

    public UsersService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async override Task<UserInfo> GetUserById(GetUserByIdRequest request, ServerCallContext context)
    {
        if(!Guid.TryParse(request.Id.Value, out var userId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Id value is not UUID"));
        }

        var user = await _dbContext.Set<User>().Where(u=> u.Id == userId).FirstOrDefaultAsync();
        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }

        return new UserInfo()
        {
            Email = user.Email,
            Username = user.Username,
            Id = new GrpcUuid() { Value = user.Id.ToString() },
            IdentityId = user.IdentityId
        };
    }

    public async override Task<UserInfo> GetUserByIdentityId(GetUserByIdentityIdRequest request, ServerCallContext context)
    {
        var user = await _dbContext.Set<User>().Where(u => u.IdentityId == request.IdentityId).FirstOrDefaultAsync();

        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }

        return new UserInfo()
        {
            Email = user.Email,
            Username = user.Username,
            Id = new GrpcUuid() { Value = user.Id.ToString() },
            IdentityId = user.IdentityId
        };
    }
}
