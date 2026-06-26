using Phoenix.Framework.Collisions;
using Phoenix.Framework.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Phoenix.FPS.Client.Players;

internal class Player
{
    public Vector3 Position;
    public float Yaw;
    public float Pitch;
    public Vector3 FrontDir
    {
        get => Vector3.Normalize(new Vector3(
                MathF.Cos(-Yaw) * MathF.Cos(Pitch),
                MathF.Sin(Pitch),
                MathF.Sin(-Yaw) * MathF.Cos(Pitch)));
    }
            
    public Matrix4x4 Transform => 
        MathHelper.RotationMxFromYawPitchRoll(Yaw, Pitch, 0) 
        * Matrix4x4.CreateTranslation(Position);

    public OrientedBoundingBox obb;
    public Player()
    {
        obb = new OrientedBoundingBox();
    }
    public void Update()
    {
        obb.Update(Position, MathHelper.RotationMxFromYawPitchRoll(Yaw, Pitch, 0));
    }
}
