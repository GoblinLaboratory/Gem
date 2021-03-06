﻿using Gem.Network.Handlers;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;
using Seterlund.CodeGuard;
using Gem.Network.Builders;
using Autofac;
using System.Collections.Generic;
using Gem.Network.Factories;
using Gem.Network.Events;
using Gem.Network.Messages;
using Gem.Network.Client;

namespace Gem.Network.Fluent
{
   using Extensions;

    public class MessageFlowBuilder : IMessageFlowBuilder
    {

        #region Fields

        private readonly string profile;
        
        private readonly MessageType messageType;

        private MessageFlowArguments messageFlowArgs;

        private readonly byte Id; 

        #endregion
        
        #region Constructor
        
        public MessageFlowBuilder(string profile,MessageType messageType)
        {
            this.profile = profile;
            this.Id = GemNetwork.GetMesssageId(profile);
            this.messageType = messageType;
            this.messageFlowArgs = new MessageFlowArguments();
        }

        #endregion

        #region IMessageFlowBuilder Implementation

        public INetworkEvent AndHandleWith<T>(T objectToHandle, Expression<Func<T, Delegate>> methodToHandle)
        {
            var methodInfo = methodToHandle.GetMethodInfo();
            var types = methodInfo.GetParameters().Select(x => x.ParameterType).ToList();

            Guard.That(types.All(x => x.IsPrimitive || x == typeof(string)), "All types should be primitive");

            var properties = RuntimePropertyInfo.GetPropertyInfo(types.ToArray());
            
            SetDynamicPoco(properties);
            SetMessageHandler(properties.Select(x => RuntimePropertyInfo.GetPrimitiveTypeAlias(x.PropertyType)).ToList(), objectToHandle, methodInfo.Name);
            var argumentsDisposable = GemClient.MessageFlow[profile,messageType].Add(messageFlowArgs);
            SetDynamicEvent(argumentsDisposable);

            messageFlowArgs.IncludesLocalTime = false;
            GemClient.MessageFlow[profile, messageType].SubscribeEvent(messageFlowArgs.ID);

            return messageFlowArgs.EventRaisingclass;
        }

        #endregion
        
        #region MessageFlow Setup

        private void SetMessageHandler(List<string> propertyNames, object invoker, string functionName)
        {
            var handlerType = Dependencies.Container.Resolve<IMessageHandlerFactory>()
                                          .Create(propertyNames, "handler" + Id, functionName);

            messageFlowArgs.MessageHandler = Activator.CreateInstance(handlerType, invoker) as IMessageHandler;
        }

        private void SetDynamicPoco(List<RuntimePropertyInfo> properties)
        {
            var newType = Dependencies.Container.Resolve<IPocoFactory>().Create(properties, "poco" + Id);

            messageFlowArgs.MessagePoco = newType;
        }

        private void SetDynamicEvent(IDisposable argumentDisposable)
        {
            messageFlowArgs.EventRaisingclass = Dependencies.Container.Resolve<IEventFactory>().Create(messageFlowArgs.MessagePoco, argumentDisposable,messageFlowArgs.ID);
        }

        #endregion

    }
}




