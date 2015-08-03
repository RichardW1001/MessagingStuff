using System;
using System.Threading.Tasks;
using Messaging;

namespace ServiceA
{
    public class LineWritten : IEventMessage
    {
        public string Message { get; set; }
    }

    public class WriteLineHandler : IMessageHandler<WriteLineCommand>
    {
        private readonly IMessageSender _messageSender;

        public WriteLineHandler(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        public Task Handle(WriteLineCommand message)
        {
            Console.WriteLine(message.Message);

            return _messageSender.Publish(new LineWritten {Message = message.Message});
        }
    }
}