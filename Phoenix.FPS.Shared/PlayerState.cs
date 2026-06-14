using Riptide;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Phoenix.FPS.Shared
{
    public class PlayerState
    {
        public Vector3 Position;
        public bool Valid;

        public float Yaw;
        public float Pitch;
        public byte ClipId;

        public bool Forward;
        public bool Backward;
        public bool Left;
        public bool Right;
        public bool Jump;
        public bool Crouch;
        public bool Sprint;

        public bool Fire;
        public bool ADS;
        public bool Reload;

        public bool Ability1;
        public bool Ability2;
        public bool Ability3;
        public bool Ability4;
     
        //public uint messageId;
        public PlayerState(bool forward, bool backward, bool left, bool right, bool jump, bool crouch, bool sprint,
            bool fire, bool ads, bool reload,  bool ability1, bool ability2, bool ability3, bool ability4)
        {
            Forward = forward;
            Backward = backward;
            Left = left;
            Right = right;
            Fire = fire;
            ADS = ads;
            Reload = reload;
            Jump = jump;
            Crouch = crouch;
            Sprint = sprint;
            Ability1 = ability1;
            Ability2 = ability2;
            Ability3 = ability3;
            Ability4 = ability4;
        }
        public PlayerState() { }

        

    }

}