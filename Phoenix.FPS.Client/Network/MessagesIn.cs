using Phoenix.FPS.Shared;
using Riptide;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.FPS.Client.Network
{
    internal static class MessagesIn
    {
        [MessageHandler((ushort)ServerToClient.BroadcastPlayerData)]
        public static void PlayerData(Message message)
        {
            var clientsCount = message.GetInt();
            for(var i = 0; i < clientsCount; i++)
            {
                var id = message.GetInt();
                var connected = message.GetBool();

                //message.Add(p.ID);
                //message.Add(p.Connected);
                if (connected)
                {
                    //message.Add(p.State);
                }

            }
        }


    }
}
