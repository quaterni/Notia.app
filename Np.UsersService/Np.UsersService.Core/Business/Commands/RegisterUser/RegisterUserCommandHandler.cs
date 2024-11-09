using Microsoft.EntityFrameworkCore;
using Np.UsersService.Core.Business.Abstractions;
using Np.UsersService.Core.Data;
using Np.UsersService.Core.Exceptions;
using Np.UsersService.Core.Messaging.ModelEvents.Abstractions;
using Np.UsersService.Core.Messaging.ModelEvents.Users;
using Np.UsersService.Core.Models.Users;
using Np.UsersService.Core.Shared;

namespace Np.UsersService.Core.Business.Commands.RegisterUser;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IModelEventService _modelEventService;

    public RegisterUserCommandHandler(
        ApplicationDbContext dbContext,
        IModelEventService modelEventService)
    {
        _dbContext = dbContext;
        _modelEventService = modelEventService;
    }

    public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if(await ContainsEmail(request.Email, cancellationToken))
        {
            return Result.Failure(RegisterUserErrors.EmailExists);
        }

        if (await ContainsUsername(request.Username, cancellationToken))
        {
            return Result.Failure(RegisterUserErrors.UsernameExists);
        }

        var user = new User(request.Username, request.Email, Guid.NewGuid());

        _dbContext.Add(user);

        _modelEventService.PublishEvent(new UserCreatedEvent(user.Id));
        _modelEventService.PublishEvent(
            new UserCreatedSecureEvent(
                user.Username, 
                user.Email, 
                request.Password, 
                user.Id));
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch(DbUpdateConcurrencyException ex)
        {
            throw new ConcurrentException(ex);
        }

        return Result.Success();
    }

    private async Task<bool> ContainsUsername(string username, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<User>()
            .Where(u => u.Username == username)
            .AnyAsync(cancellationToken);
    }

    private async Task<bool> ContainsEmail(string email, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<User>()
            .Where(u => u.Email == email)
            .AnyAsync(cancellationToken);
    }
}
