namespace Np.UsersService.Core.Messaging.ModelEvents.Abstractions;

public interface IModelEventService
{
    void PublishEvent(IModelEvent modelEvent);
}
