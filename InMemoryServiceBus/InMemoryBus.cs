using System;
using System.Reactive;
using System.Threading.Tasks;
using Messaging;

namespace InMemoryServiceBus
{
    public class InMemoryBus : IMessageBus, IMessageSender
    {
        private readonly IMessageDispatcher _dispatcher;
        private readonly InMemorySender _sender;
        private IDisposable _messagesHandle;

        public InMemoryBus(IMessageDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _sender = new InMemorySender();
        }

        public void Start()
        {
            _messagesHandle = _sender.Messages.
                SubscribeSafe(new AnonymousObserver<IMessage>(
                    async m => { await _dispatcher.Handle(m); }
                    ));
        }

        public void Stop()
        {
            _messagesHandle.Dispose();
        }

        public Task Send(ICommandMessage message)
        {
            return _sender.Send(message);
        }

        public Task Publish(IEventMessage message)
        {
            return _sender.Publish(message);
        }
    }
}