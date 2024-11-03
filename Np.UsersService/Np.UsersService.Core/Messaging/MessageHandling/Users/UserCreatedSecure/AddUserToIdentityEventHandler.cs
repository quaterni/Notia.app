using MediatR;
using Microsoft.EntityFrameworkCore;
using Np.UsersService.Core.Authentication.Abstractions;
using Np.UsersService.Core.Authentication.Errors;
using Np.UsersService.Core.Data;
using Np.UsersService.Core.Dtos.Users;
using Np.UsersService.Core.Models.Users;

namespace Np.UsersService.Core.Messaging.MessageHandling.Users.UserCreatedSecure;

public partial class AddUserToIdentityEventHandler : INotificationHandler<UserCreatedSecureApplicationEvent>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IIdentityService _identityService;

    public AddUserToIdentityEventHandler(
        ApplicationDbContext dbContext, 
        IIdentityService identityService)
    {
        _dbContext = dbContext;
        _identityService = identityService;
    }

    public async Task Handle(UserCreatedSecureApplicationEvent notification, CancellationToken cancellationToken)
    {
        string identityId = string.Empty;
        var resultIdentityId = await _identityService.CreateUserAsync(new CreateUserRequest(notification.Username, notification.Email, notification.Password), cancellationToken);
        var user = await GetUserAsync(notification.Id);

        if(user == null)
        {

            throw new ApplicationException("User not found in database, when attemping add user to identity");
        }
        if (user.IsSyncrhonizedWithIdentity)
        {
            return;
        }

        if(resultIdentityId.IsFailed && resultIdentityId.Error.Equals(IdentityErrors.UserExists))
        {
            var resultUserView = await _identityService.GetUserByCredentialsAsync(notification.Username, notification.Email, cancellationToken);
            if (resultUserView.IsFailed)
            {
                throw new ApplicationException("Result failed by unknown error when request identity service");
            }
            identityId = resultUserView.Value.IdentityId;
        }

        user.IdentityId = !string.IsNullOrEmpty(identityId) ? 
            identityId : 
            throw new ApplicationException("Identity id is empty unexpectedly");
        user.IsSyncrhonizedWithIdentity = true;

        _dbContext.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<User?> GetUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<User>()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
