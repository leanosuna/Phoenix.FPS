using Phoenix.FPS.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.FPS.Server
{
    public class Player
    {
        public uint ID = uint.MaxValue;
        public ushort NetID = ushort.MaxValue;
        public bool Connected = false;
        public string Name = "";
        public PlayerState State = default!;
        public int FPS;
        public Player()
        {
            
        }
        public Player(uint id)
        {
            ID = id;
        }
        public Player(ushort netId)
        {
            NetID = netId;
        }
    }
}
