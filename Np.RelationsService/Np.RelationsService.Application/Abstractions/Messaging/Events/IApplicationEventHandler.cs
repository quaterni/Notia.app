
using MediatR;

namespace Np.RelationsService.Application.Abstractions.Messaging.Events;

public interface IApplicationEventHandler<T> : INotificationHandler<T> 
    where T: IApplicationEvent;
