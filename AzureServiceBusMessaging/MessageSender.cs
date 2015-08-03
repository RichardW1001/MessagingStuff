using System.Threading.Tasks;
using Messaging;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace AzureServiceBusMessaging
{
    public class MessageSender : IMessageSender
    {
        private readonly string _connectionString;

        public MessageSender(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task Send(ICommandMessage message)
        {
            var queueClient = QueueClient.CreateFromConnectionString(_connectionString, message.GetType().FullName);

            await queueClient.SendAsync(BuildBrokeredMessage(message));
        }

        public async Task Publish(IEventMessage message)
        {
            var topicClient = TopicClient.CreateFromConnectionString(_connectionString, message.GetType().FullName);

            await topicClient.SendAsync(BuildBrokeredMessage(message));
        }

        private static BrokeredMessage BuildBrokeredMessage(IMessage message)
        {
            var serialised = JsonConvert.SerializeObject(message,
                new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    TypeNameHandling = TypeNameHandling.All
                });

            return new BrokeredMessage(serialised);
        }
    }
}