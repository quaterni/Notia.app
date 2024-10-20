
namespace Np.RelationsService.Application.Abstractions.Messaging.Events;

public interface IEventProcessor
{
    Task Process(MessageBusEvent eventMessage);
}
