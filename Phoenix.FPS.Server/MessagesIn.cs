using Phoenix.FPS.Shared;
using Riptide;


namespace Phoenix.FPS.Server;

internal static class MessagesIn
{
    [MessageHandler((ushort)ClientToServer.PlayerIdentity)]
    static void PlayerIdentity(ushort fromClientId, Message message)
    {
        //Console.WriteLine("identity received");
        var playerId = message.GetUInt();
        var playerName = message.GetString();
        var version = message.GetInt();

        var current = Server.CFG.Version;
        if (version != current)
        {
            var versionErrorMsg = Message.Create().AddString("VERSION MISSMATCH");
            Server.RiptideServer.DisconnectClient(fromClientId, versionErrorMsg);
            //Console.WriteLine($"id {playerId} wrong version: {version} -> (current {current})");
            return;
        }

        Player? p;
        if (Server.GetPlayerFromId(playerId, out p))
        {
            //registered player changed netId?
            if (p!.NetID != fromClientId)
                Server.PlayerNetIDChange(p, fromClientId);
        }
        else
        {
            Server.GetPlayerNet(fromClientId, out p);
            //registered player changed id?
            if (p!.ID != playerId)
                Server.PlayerIDChange(p, playerId);

        }

        p.Name = playerName;

        p.Connected = true;

        //playersJustJoined.Add(p);
    }

    [MessageHandler((ushort)ClientToServer.PlayerData)]
    static void PlayerData(ushort fromClientId, Message message)
    {
        var fps = message.GetInt();
        if(Server.GetPlayerNet(fromClientId, out var p))
        {
            p!.FPS = fps;
        }

    }
}
