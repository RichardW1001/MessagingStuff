using System;
using System.Linq;
using Autofac;
using Autofac.Builder;
using AzureServiceBusMessaging;
using InMemoryServiceBus;
using Messaging;

namespace ServiceA
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = @"Endpoint=sb://rwplayground.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=f4ir0kfMRQaKMJwRJfGvEUfLCwTicLWHsndH33eBRa0=";
            
            var builder = new ContainerBuilder();

            builder.RegisterMessageHandlers(typeof(WriteLineHandler).Assembly);

            //builder.RegisterType<WriteLineHandler>().As<IMessageHandler<WriteLineCommand>>();
           
            builder.Register(context => new MessageSender(connectionString)).As<IMessageSender>();

            var handlerFactory = new AutofacMessageDispatcher(builder.Build());
            
            var messageBus = new MessageBus(connectionString, handlerFactory);

            Console.WriteLine("Receiving...");

            messageBus.Start();

            Console.ReadLine();
        }
    }
}
