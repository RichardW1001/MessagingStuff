using System;
using System.Threading.Tasks;
using Autofac;
using InMemoryServiceBus;
using Messaging;

namespace InMemService
{
    public class WriteLineMessage : ICommandMessage
    {
        public string Message { get; set; }
    }

    public class WriteLineHandler : IMessageHandler<WriteLineMessage>
    {
        public Task Handle(WriteLineMessage message)
        {
            Console.WriteLine("Received: {0}", message.Message);
            return Task.FromResult(true);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<WriteLineHandler>().AsImplementedInterfaces();

            var dispatcher = new AutofacMessageDispatcher(builder.Build());

            var bus = new InMemoryBus(dispatcher);

            bus.Start();

            var message = "";
            while (message != "exit")
            {
                message = Console.ReadLine();
                bus.Send(new WriteLineMessage {Message = message});
            }
        }
    }
}
