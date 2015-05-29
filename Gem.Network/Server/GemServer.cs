﻿using Gem.Network.Async;
using Gem.Network.Commands;
using Gem.Network.Fluent;
using Gem.Network.Managers;
using Gem.Network.Messages;
using Gem.Network.Providers;
using Gem.Network.Utilities.Loggers;
using Lidgren.Network;
using Seterlund.CodeGuard;
using System;

namespace Gem.Network.Server
{
    /// <summary>
    /// The class that handles server side connection , message processing and configuration
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class GemServer
    {

        #region Fields

        /// <summary>
        /// The server instance
        /// </summary>
        private readonly IServer server;

        /// <summary>
        /// The message processor
        /// </summary>
        private readonly IMessageProcessor messageProcessor;

        /// <summary>
        /// The current server configuration
        /// </summary>
        private ServerConfig serverConfig;

        /// <summary>
        /// This is used to process messages async
        /// </summary>
        private ParallelTaskStarter asyncMessageProcessor;

        #endregion

        #region Properties

        /// <summary>
        /// The outgoing messages configuration
        /// </summary>
        public PackageConfig PackageConfig
        {
            get;
            set;
        }

        /// <summary>
        /// Shows if the server is connected
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return server.IsConnected;
            }
        }

        #endregion

        #region Constructor

        public GemServer(string profileName, ServerConfig serverConfig, PackageConfig packageConfig)
        {
            //validate server config
            Guard.That(serverConfig).IsNotNull();
            Guard.That(packageConfig).IsNotNull();

            GemNetwork.ActiveProfile = profileName;

            //setup authentication
            if (serverConfig.RequireAuthentication)
            {
                RequireAuthentication();
            }
            else
            {
                Profile(GemNetwork.ActiveProfile).OnIncomingConnection((srvr, netconnection, msg) =>
                {
                    netconnection.Approve();
                    GemNetworkDebugger.Append.Info(String.Format("Approved {0} {3} Sender: {1}{3} Message: {2}",
                                            netconnection, msg.Sender, msg.Message, Environment.NewLine));
                });
            }

            this.serverConfig = serverConfig;
            this.PackageConfig = packageConfig;

            server = GemNetwork.Server;

            messageProcessor = new ServerMessageProcessor(server);
            asyncMessageProcessor = new ParallelTaskStarter(TimeSpan.Zero);
        }
                
        #endregion
        
        #region Settings Helpers

        private void RequireAuthentication()
        {
            Profile(GemNetwork.ActiveProfile).OnIncomingConnection((svr, netconnection, msg) =>
            {
                if (msg.Password == server.Password)
                {
                    netconnection.Approve();
                    GemNetworkDebugger.Append.Info(String.Format("Approved {0} {3} Sender: {1}{3} Message: {2}"
                                            , netconnection, msg.Sender, msg.Message, Environment.NewLine));
                }
                else
                {
                    GemNetworkDebugger.Append.Warn(String.Format("Declined connection {0}. Reason: Invalid credentials {4} Sender: {1}{4} Message: {2}{4} Password: {3}"
                                           , netconnection, msg.Sender, msg.Message, msg.Password, Environment.NewLine));
                    netconnection.Deny();
                }
            });
        }


        #endregion
        
        #region Start / Close Connection

        public void Disconnect()
        {
            asyncMessageProcessor.Stop();
            server.Disconnect();
        }

        public void Dispose()
        {
            Disconnect();
        }

        public void RunAsync()
        {
            try
            {
                server.Connect(serverConfig, PackageConfig);
                asyncMessageProcessor.Start(() => messageProcessor.ProcessNetworkMessages());
            }
            catch (Exception ex)
            {
                GemNetworkDebugger.Append.Error("Unable to start the server. Reason: {0}", ex.Message);
            }
        }

        #endregion
        
        #region Settings

        private static ServerMessageFlowManager serverMessageFlowManager;
        internal static ServerMessageFlowManager MessageFlow
        {
            get
            {
                return serverMessageFlowManager
                      = serverMessageFlowManager ?? new ServerMessageFlowManager();
            }
        }

        private static ServerConfigurationManager serverConfigurationManager;
        internal static ServerConfigurationManager ServerConfiguration
        {
            get
            {
                return serverConfigurationManager
                      = serverConfigurationManager ?? new ServerConfigurationManager();
            }
        }

        public static IServerMessageRouter Profile(string profileName)
        {
            return new ServerMessageRouter(profileName);
        }

        public static void RegisterCommand(string command, string description, bool requireAuthorization, ExecuteCommand callback)
        {
            GemNetwork.Commander.RegisterCommand(command, requireAuthorization, description, callback);
        }

        public static void SetConsolePassword(string password)
        {
            GemNetwork.Commander.SetPassword(password);
        }

        public static void ExecuteCommand(string command)
        {
            GemNetwork.Commander.ExecuteCommand(null, command);
        }

        public static void ExecuteCommand(NetConnection sender, string command)
        {
            GemNetwork.Commander.ExecuteCommand(sender, command);
        }

        #endregion

    }
}
