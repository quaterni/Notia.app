
namespace Np.NotesService.Infrastructure.Messaging.Abstractions;

public interface IProducer<in TMessage> : IDisposable
{
    Task SendAsync(TMessage message, CancellationToken cancellationToken = default);
}
