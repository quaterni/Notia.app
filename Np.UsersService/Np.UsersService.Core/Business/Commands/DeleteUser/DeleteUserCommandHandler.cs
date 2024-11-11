using Microsoft.EntityFrameworkCore;
using Np.UsersService.Core.Business.Abstractions;
using Np.UsersService.Core.Data;
using Np.UsersService.Core.Exceptions;
using Np.UsersService.Core.Messaging.ModelEvents.Abstractions;
using Np.UsersService.Core.Messaging.ModelEvents.Users;
using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Business.Commands.DeleteUser;

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IModelEventService _modelEventService;

    public DeleteUserCommandHandler(
        ApplicationDbContext dbContext,
        IModelEventService modelEventService)
    {
        _dbContext = dbContext;
        _modelEventService = modelEventService;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = request.User!;
        _dbContext.Remove(user);
        _modelEventService.PublishEvent(new UserDeletedEvent(user.Id));
        _modelEventService.PublishEvent(new UserDeletedSecureEvent(user.IdentityId!));

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch(DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException(ex);
        }       
    }
}
