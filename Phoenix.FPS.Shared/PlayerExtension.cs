using Riptide;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.FPS.Shared
{
    public static class PlayerExtension
    {
        public static Message Add(this Message message, PlayerState state)
        {
            return message.Add(state.Position).Add(state.Yaw).Add(state.Pitch).Add(state.ClipId)
                .Add(state.Forward).Add(state.Backward).Add(state.Left).Add(state.Right)
                .Add(state.Jump).Add(state.Crouch).Add(state.Sprint)
                .Add(state.Fire).Add(state.ADS).Add(state.Reload)
                .Add(state.Ability1).Add(state.Ability2).Add(state.Ability3).Add(state.Ability4);
        }


        public static PlayerState GetPlayerState(this Message message)
        {
            return new PlayerState
            {
                Position = message.GetVector3(),
                Yaw = message.GetFloat(),
                Pitch = message.GetFloat(),
                ClipId = message.GetByte(),
                Forward = message.GetBool(),
                Backward = message.GetBool(),
                Left = message.GetBool(),
                Right = message.GetBool(),
                Jump = message.GetBool(),
                Crouch = message.GetBool(),
                Sprint = message.GetBool(),
                Fire = message.GetBool(),
                ADS = message.GetBool(),
                Reload = message.GetBool(),
                Ability1 = message.GetBool(),
                Ability2 = message.GetBool(),
                Ability3 = message.GetBool(),
                Ability4 = message.GetBool(),
            };
        }
    }
}
