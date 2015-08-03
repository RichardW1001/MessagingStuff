using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Messaging;

namespace InMemoryServiceBus
{
    public class InMemorySender : IMessageSender
    {
        private readonly Subject<IMessage> _messages;

        public InMemorySender()
        {
            _messages = new Subject<IMessage>();
        }

        public IObservable<IMessage> Messages
        {
            get
            {
                return _messages.
                    Where(m => m != null).
                    ObserveOn(TaskPoolScheduler.Default);
            }
        }

        public Task Send(ICommandMessage message)
        {
            _messages.OnNext(message);
            return Task.FromResult(true);
        }

        public Task Publish(IEventMessage message)
        {
            _messages.OnNext(message);
            return Task.FromResult(true);
        }
    }
}