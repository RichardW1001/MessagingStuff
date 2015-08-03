using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Messaging;

namespace InMemoryServiceBus
{
    public static class AutofacHandlerFactoryExtensions
    {
        public static void RegisterMessageHandlers(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies).
                Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageHandler<>))).
                AsImplementedInterfaces();
        }
    }

    public class AutofacMessageDispatcher : MessageDispatcher
    {
        private readonly IContainer _container;
        
        public AutofacMessageDispatcher(IContainer container)
        {
            _container = container;
        }

        protected override IMessageHandler<T> ResolveHandler<T>()
        {
            return _container.Resolve<IMessageHandler<T>>();
        }

        public override IEnumerable<Type> HandlerTypes()
        {
            return _container.ComponentRegistry.Registrations.
                Select(r => r.Activator.LimitType).
                Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageHandler<>))).
                ToArray();
        }
    }
}