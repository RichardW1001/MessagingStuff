using System.Threading.Tasks;

namespace Messaging
{
    public interface IMessageSender
    {
        Task Send(ICommandMessage message);
        Task Publish(IEventMessage message);
    }
}