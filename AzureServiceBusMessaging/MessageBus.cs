using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using InMemoryServiceBus;
using Messaging;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace AzureServiceBusMessaging
{
    public class MessageBus
    {

        private readonly string _connectionString;
        private readonly IMessageDispatcher _dispatcher;

        public MessageBus(string connectionString, IMessageDispatcher dispatcher)
        {
            _connectionString = connectionString;
            _dispatcher = dispatcher;
        }

        private void Initialise()
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);

            foreach (var commandType in _dispatcher.HandledCommands())
            {
                if (!namespaceManager.QueueExists(commandType.FullName))
                {
                    namespaceManager.CreateQueue(commandType.FullName);    
                }
            }

            foreach (var eventType in _dispatcher.HandledEvents())
            {
                if (!namespaceManager.TopicExists(eventType.FullName))
                {
                    namespaceManager.CreateTopic(eventType.FullName);
                }
            }

            foreach (var eventHandler in _dispatcher.EventHandlers())
            {
                foreach (var messageType in eventHandler.GetHandledMessageTypes())
                {
                    if (!namespaceManager.SubscriptionExists(messageType.FullName, eventHandler.FullName))
                    {
                        namespaceManager.CreateSubscription(messageType.FullName, eventHandler.FullName);
                    }    
                }
            }
        }

        public void Start()
        {
            Initialise();

            var queueClients = _dispatcher.HandledCommands().
                Select(t => QueueClient.CreateFromConnectionString(_connectionString, t.FullName, ReceiveMode.PeekLock));

            //TODO: Should be 1 subscription per handler type or per handler assembly?
            var subscriptionClients = _dispatcher.EventHandlers().
                SelectMany(handlerType => 
                    handlerType.GetHandledMessageTypes().
                        Select(messageType => SubscriptionClient.CreateFromConnectionString(_connectionString, messageType.FullName, handlerType.FullName)));

            var events = Observable.While(() => true, subscriptionClients.ToObservable()).
                Select(c => Observable.Defer(() => c.ReceiveAsync(TimeSpan.FromSeconds(20)).ToObservable()));

            var commands = Observable.While(() => true, queueClients.ToObservable()).
                Select(c => Observable.Defer(() => c.ReceiveAsync(TimeSpan.FromSeconds(20)).ToObservable()));

            _observableHandle = commands.Merge(events).
                Merge(10).
                Where(m => m != null).
                Subscribe(m => Handle(m));
        }

        private async Task Handle(BrokeredMessage brokeredMessage)
        {
            dynamic innerMessage = JsonConvert.DeserializeObject(brokeredMessage.GetBody<string>(),
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat
                }) as IMessage;

            try
            {
                await _dispatcher.Handle(innerMessage);

                await brokeredMessage.CompleteAsync();
            }
            catch (Exception exception)
            {
                if (brokeredMessage.DeliveryCount >= 5)
                {
                    brokeredMessage.DeadLetter(exception.GetBaseException().Message, exception.GetBaseException().ToString());
                }

                brokeredMessage.Abandon();
            }
        }

        private IDisposable _observableHandle;

        public void Stop()
        {
            _observableHandle.Dispose();
        }
    }
}