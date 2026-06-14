using Phoenix.FPS.Shared;
using Riptide;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.FPS.Client.Network
{
    internal static class MessagesOut
    {
        public static void SendPlayerId()
        {
            var id = (uint)1234;
            var name = "nix";
            var version = 1;
            var m = Message.Create(MessageSendMode.Unreliable, ClientToServer.PlayerIdentity)
                .Add(id)
                .Add(name)
                .Add(version);
            Client.RiptideClient.Send(m);
        }



        public static void SendData()
        {
            var m = Message.Create(MessageSendMode.Unreliable, ClientToServer.PlayerData)
                .Add((int)Game.Instance.Graphics.FPS);
            Client.RiptideClient.Send(m);
        }


    }
}
