using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Messaging;

namespace InMemoryServiceBus
{
    public interface IMessageDispatcher
    {
        Task Handle(IMessage message);

        IEnumerable<Type> HandlerTypes();
    }
}