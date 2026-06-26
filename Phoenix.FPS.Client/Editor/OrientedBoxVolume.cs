using System.Numerics;
using System.Text.Json.Nodes;
using Phoenix.Framework.Collisions;
using Phoenix.Framework.Maths;

namespace Phoenix.FPS.Client.Editor;

internal class OrientedBoxVolume : BoundingVolume
{
    public Vector3 Position;
    public Vector3 Size = Vector3.One;
    public float Yaw;
    public float Pitch;
    public float Roll;

    public override BoundingVolumeType Type => BoundingVolumeType.OBB;

    public override void DrawGizmo()
    {
        if (!Visible) return;
        var game = Game.Instance;
        var color = Selected ? new Vector3(1, 1, 0) : new Vector3(1, 0.5f, 0);
        var obb = new OrientedBoundingBox();
        obb.Update(Position, MathHelper.RotationMxFromYawPitchRoll(Yaw, Pitch, Roll));
        obb.Size = Size;
        game.Gizmos.AddVolume(obb, color);
    }

    public override JsonObject Serialize()
    {
        return new JsonObject
        {
            ["Type"] = "OBB",
            ["Name"] = Name,
            ["Pos_X"] = Position.X,
            ["Pos_Y"] = Position.Y,
            ["Pos_Z"] = Position.Z,
            ["Size_X"] = Size.X,
            ["Size_Y"] = Size.Y,
            ["Size_Z"] = Size.Z,
            ["Yaw"] = Yaw,
            ["Pitch"] = Pitch,
            ["Roll"] = Roll,
        };
    }

    public new static OrientedBoxVolume Deserialize(JsonObject data)
    {
        return new OrientedBoxVolume
        {
            Name = (string)data["Name"]!,
            Position = new Vector3(
                (float)data["Pos_X"]!,
                (float)data["Pos_Y"]!,
                (float)data["Pos_Z"]!
            ),
            Size = new Vector3(
                (float)data["Size_X"]!,
                (float)data["Size_Y"]!,
                (float)data["Size_Z"]!
            ),
            Yaw = (float)data["Yaw"]!,
            Pitch = (float)data["Pitch"]!,
            Roll = (float)data["Roll"]!,
        };
    }
}
