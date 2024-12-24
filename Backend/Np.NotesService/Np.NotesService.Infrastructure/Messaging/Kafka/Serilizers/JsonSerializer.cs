
using Confluent.Kafka;
using Newtonsoft.Json;
using Np.NotesService.Infrastructure.Messaging.Abstractions;
using System.Text;

namespace Np.NotesService.Infrastructure.Messaging.Kafka.Serilizers;

internal class JsonSerializer<TMessage> : ISerializer<TMessage>
{
    public byte[] Serialize(TMessage data, SerializationContext context)
    {
        var s = JsonConvert.SerializeObject(data);
        return Encoding.UTF8.GetBytes(s);
    }
}
