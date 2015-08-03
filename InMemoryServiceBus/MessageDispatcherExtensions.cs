using System;
using System.Collections.Generic;
using System.Linq;
using Messaging;

namespace InMemoryServiceBus
{
    public static class MessageDispatcherExtensions
    {
        public static IEnumerable<Type> HandledEvents(this IMessageDispatcher dispatcher)
        {
            return HandledMessageTypes(dispatcher).Where(t => typeof(IEventMessage).IsAssignableFrom(t));
        }

        public static IEnumerable<Type> HandledCommands(this IMessageDispatcher dispatcher)
        {
            return HandledMessageTypes(dispatcher).Where(t => typeof(ICommandMessage).IsAssignableFrom(t));
        }

        public static IEnumerable<Type> HandledMessageTypes(this IMessageDispatcher dispatcher)
        {
            return dispatcher.HandlerTypes().Select(t =>
                t.GetInterfaces().
                    First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageHandler<>))
                    .GetGenericArguments()[0]).ToArray();
        }

        public static IEnumerable<Type> EventHandlers(this IMessageDispatcher dispatcher)
        {
            return dispatcher.HandlerTypes().
                Where(t =>
                    t.GetInterfaces().
                        Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageHandler<>)).
                        Any(i => typeof(IEventMessage).IsAssignableFrom(i.GetGenericArguments()[0]))).
                ToArray();
        }

        public static IEnumerable<Type> CommandHandlers(this IMessageDispatcher dispatcher)
        {
            return dispatcher.HandlerTypes().
                Where(t =>
                    t.GetInterfaces().
                        Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageHandler<>)).
                        Any(i => i.GetGenericArguments()[0] == typeof(ICommandMessage))).
                ToArray();
        }

        public static IEnumerable<Type> GetHandledMessageTypes(this Type handlerType)
        {
            return handlerType.GetInterfaces().
                Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageHandler<>)).
                Select(i => i.GetGenericArguments()[0]);
        }
    }
}