using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.FPS.Shared
{
    public enum ClientToServer : ushort
    {
        PlayerIdentity,
        PlayerData,
        Command
    }
}
