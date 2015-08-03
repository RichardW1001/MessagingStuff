using System;
using AzureServiceBusMessaging;
using ServiceA;

namespace ServiceB
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a message to send...");

            var messageSender = new MessageSender(@"Endpoint=sb://rwplayground.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=f4ir0kfMRQaKMJwRJfGvEUfLCwTicLWHsndH33eBRa0=");

            var message = "";
            while (message != "exit")
            {
                message = Console.ReadLine();
                messageSender.Send(new WriteLineCommand {Message = message}).Wait();
                Console.WriteLine("Sent: {0}", message);
            }
        }
    }
}
