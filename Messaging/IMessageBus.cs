using System;

namespace Messaging
{
    public interface IMessageBus
    {
        void Start();
        void Stop();
    }
}