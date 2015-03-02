﻿using Gem.Network.Client;
using Gem.Network.Messages;
using Gem.Network.Utilities.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gem.Network.Chat.Client
{
    class Chat
    {

        #region Fields

        private static Peer peer;
        private static GemClient client;
        private static string name;

        #endregion

        #region Private Helpers

        private static void PrintIntroMessage()
        {
            Console.WriteLine(String.Format(
            @" 
             Commands {0}
-------------------------------------{0}
-setname <newname>  |  Change nickname  
-quit               |  Quit  {0}{0}", Environment.NewLine));

        }

        private static void ClientSetup()
        {
            GemNetwork.ActiveProfile = "GemChat";
            GemNetworkDebugger.Echo = Console.WriteLine;

            client = new GemClient("GemChat", "83.212.103.13", 14242);
        }

        private static void ProcessInput()
        {
            string input = string.Empty;
            while (input != "-quit")
            {
                input = Console.ReadLine();
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                if (input.Length >= 1)
                {
                    if (input[0] == '-')
                    {
                        var processed = input.Split(' ');
                        if (processed.Length >= 2)
                        {
                            if (processed[0] == "-setname")
                            {
                                peer.ChangeName(processed[1]);
                            }
                        }
                    }
                    else
                    {
                        peer.Say(input);
                    }
                }
            }
        }

        #endregion

        static void Main(string[] args)
        {
            ClientSetup();

            //Pick a name
            Console.WriteLine("Your nickname : ");
            string name = Console.ReadLine();

            PrintIntroMessage();

            client.RunAsync(() => new ConnectionApprovalMessage { Sender = "Dummy", Password = "123" });

            //Initialize a chat peer
            peer = new Peer(name);

            ProcessInput();

            peer.SayGoodBye();

            client.Dispose();

            Console.ReadLine();
        }

    }
}
