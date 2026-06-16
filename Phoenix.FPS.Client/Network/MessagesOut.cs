using Phoenix.FPS.Shared;
using Riptide;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.FPS.Client.Network;

internal static class MessagesOut
{
    static Game game = default!;
    public static void Init()
    {
        game = Game.Instance;
    }
    public static void SendPlayerId()
    {
        var m = Message.Create(MessageSendMode.Unreliable, ClientToServer.PlayerIdentity)
            .Add(game.CFG.PlayerID)
            .Add(game.CFG.PlayerName)
            .Add(game.CFG.Version);
        Client.RiptideClient.Send(m);
    }

    

    public static void SendData()
    {
        var m = Message.Create(MessageSendMode.Unreliable, ClientToServer.PlayerData)
            .Add((int)game.Graphics.FPS);
        Client.RiptideClient.Send(m);
    }


}
