using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.FPS.Shared
{
    public enum ServerToClient : ushort
    {
        CommandResponse,
        BroadcastPlayerData,
        PlayerIdentity,
        GameModeChange,
        PlayerTeleport
    }
}
