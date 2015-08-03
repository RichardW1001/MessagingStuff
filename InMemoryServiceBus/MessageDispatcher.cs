using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Messaging;

namespace InMemoryServiceBus
{
    public abstract class MessageDispatcher : IMessageDispatcher
    {
        private static readonly MethodInfo HandleInternalMethod;

        static MessageDispatcher()
        {
            HandleInternalMethod = typeof (AutofacMessageDispatcher).
                GetMethod("HandleInternal", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public Task Handle(IMessage message)
        {
            var handleInternal = HandleInternalMethod.MakeGenericMethod(message.GetType());

            return (Task)handleInternal.Invoke(this, new object[] { message });
        }

        protected Task HandleInternal<T>(T message) where T : IMessage
        {
            var handler = ResolveHandler<T>();
            return handler.Handle(message);
        }

        protected abstract IMessageHandler<T> ResolveHandler<T>() where T : IMessage;
        
        public abstract IEnumerable<Type> HandlerTypes();
    }
}
