using System;
using System.Threading.Tasks;
using Autofac;
using AzureServiceBusMessaging;
using InMemoryServiceBus;
using Messaging;
using ServiceA;

namespace ServiceC
{
    public class LineWrittenHandler : IMessageHandler<LineWritten>
    {
        public Task Handle(LineWritten message)
        {
            Console.WriteLine("A wrote: {0}", message.Message);
            return Task.FromResult(true);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = @"Endpoint=sb://rwplayground.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=f4ir0kfMRQaKMJwRJfGvEUfLCwTicLWHsndH33eBRa0=";

            var builder = new ContainerBuilder();

            builder.RegisterMessageHandlers(typeof(LineWrittenHandler).Assembly);

            var messageDispatcher = new AutofacMessageDispatcher(builder.Build());

            var messageBus = new MessageBus(connectionString, messageDispatcher);

            Console.WriteLine("Receiving...");

            messageBus.Start();

            Console.ReadLine();
        }
    }
}
