using RabbitMQ.Client;

namespace Np.NotesService.Infrastructure.Messaging.RabbitMq;

public interface IRabbitMqChannelFactory
{
    IModel CreateChannel();
}