using Messaging;

namespace ServiceA
{
    public class WriteLineCommand : ICommandMessage
    {
        public string Message { get; set; }
    }
}