using System.Threading.Tasks;

namespace Messaging
{
    public interface IMessageHandler<T> where T : IMessage
    {
        Task Handle(T message);
    }
}