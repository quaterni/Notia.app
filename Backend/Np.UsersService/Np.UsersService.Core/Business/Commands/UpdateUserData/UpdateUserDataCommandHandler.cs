using Microsoft.EntityFrameworkCore;
using Np.UsersService.Core.Authentication.Errors;
using Np.UsersService.Core.Authentication.Models;
using Np.UsersService.Core.Business.Abstractions;
using Np.UsersService.Core.Data;
using Np.UsersService.Core.Exceptions;
using Np.UsersService.Core.Messaging.ModelEvents.Abstractions;
using Np.UsersService.Core.Messaging.ModelEvents.Users;
using Np.UsersService.Core.Models.Users;
using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Business.Commands.UpdateUserData;

public class UpdateUserDataCommandHandler : ICommandHandler<UpdateUserDataCommand>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IModelEventService _modelEventService;

    public UpdateUserDataCommandHandler(ApplicationDbContext dbContext, IModelEventService modelEventService)
    {
        _dbContext = dbContext;
        _modelEventService = modelEventService;
    }

    public async Task<Result> Handle(UpdateUserDataCommand request, CancellationToken cancellationToken)
    {
        var user = request.User!;

        if(request.Username != null && await IsUsernameTakenByAnotherUser(user.Id, request.Username))
        {
            return Result.Failure(IdentityErrors.UserExists);
        }
        if (request.Email != null && await IsUsernameTakenByAnotherUser(user.Id, request.Email))
        {
            return Result.Failure(IdentityErrors.UserExists);
        }
        if(request.Username != null)
        {
            user.Username = request.Username;
        }
        if (request.Email != null)
        {
            user.Email = request.Email;
        }

        _modelEventService.PublishEvent(new UserUpdatedEvent(user.Id));
        _modelEventService.PublishEvent(
            new UserUpdatedSecureEvent(
                user.IdentityId!, 
                new UserUpdateRepresentation(request.Username, request.Email)));

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch(DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException(ex);
        }

        return Result.Success();
    }

    private async Task<bool> IsUsernameTakenByAnotherUser(Guid userId, string username, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<User>()
            .Where(u => u.Username.Equals(username) && !u.Id.Equals(userId))
            .AnyAsync();
    }

    private async Task<bool> IsEmailTakenByAnotherUser(Guid userId, string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<User>()
            .Where(u => u.Email.Equals(email) && !u.Id.Equals(userId))
            .AnyAsync();
    }
}
