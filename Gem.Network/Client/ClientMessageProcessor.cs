﻿﻿using System;
using Lidgren.Network;
using Gem.Network.Messages;
using System.Net;
using System.Collections.Generic;
using Gem.Network.Utilities;
using Gem.Network.Containers;
using Gem.Network.Utilities.Loggers;
using Gem.Network.Extensions;
using Gem.Network.Events;

namespace Gem.Network
{
    public class ClientMessageProcessor : IMessageProcessor
    {

        #region Declarations

        private readonly IClient client;

        private readonly IAppender Write;
       
        #endregion


        #region Constructor

        public ClientMessageProcessor(IClient client)
        {
            this.client = client;
            Write = new ActionAppender(GemNetworkDebugger.Echo);
        }

        #endregion


        #region Messages

        public void ProcessNetworkMessages()
        {
            NetIncomingMessage im;

            while ((im = this.client.ReadMessage()) != null)
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus)im.ReadByte())
                        {
                            case NetConnectionStatus.Connected:
                                Write.Info("Connected to {0}", im.SenderConnection);

                                ClientMessageType.Connected.Handle(im);
                                break;
                            case NetConnectionStatus.Disconnected:
                                Write.Info(im.SenderConnection + " status changed. " + (NetConnectionStatus)im.SenderConnection.Status);
                                if (im.SenderConnection.Status == NetConnectionStatus.Disconnected
                                 || im.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                                { }
                                break;
                            case NetConnectionStatus.InitiatedConnect:
                                break;
                        }
                        break;

                    case NetIncomingMessageType.Data:
                        ClientMessageType.Data.Handle(im);
                        break;
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                        Write.Warning(im.ReadString());
                        break;
                    case NetIncomingMessageType.ErrorMessage:
                        Write.Error(im.ReadString());
                        break;

                }
             
                client.Recycle(im);
            }

        }

        #endregion


    }
}