using Phoenix.FPS.Shared;
using Riptide;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.FPS.Server
{
    internal static class MessagesOut
    {
        public static void BroadcastPlayerData()
        {
            Message message = Message.Create(MessageSendMode.Unreliable, ServerToClient.BroadcastPlayerData)
                .Add(Server.Players.Count);

            foreach (var p in Server.Players)
            {
                message.Add(p.ID);
                message.Add(p.Connected);
                if(p.Connected)
                {
                    //message.Add(p.State);
                }
                
            }
        }
    }
}
