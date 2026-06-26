using System.Numerics;
using System.Text.Json.Nodes;
using Phoenix.Framework.Collisions;

namespace Phoenix.FPS.Client.Editor;

internal class AxisAlignedBoxVolume : BoundingVolume
{
    public Vector3 Center;
    public Vector3 Size = Vector3.One;

    public override BoundingVolumeType Type => BoundingVolumeType.AABB;

    public AxisAlignedBoxVolume() { }

    public AxisAlignedBoxVolume(string name, Vector3 center, Vector3 size)
    {
        Name = name;
        Center = center;
        Size = size;
    }

    public override void DrawGizmo()
    {
        if (!Visible) return;
        var game = Game.Instance;
        var color = Selected ? new Vector3(1, 1, 0) : new Vector3(0, 1, 1);
        var aabb = new AxisAlignedBoundingBox(Center, Size);
        game.Gizmos.AddVolume(aabb, color);
    }

    public override JsonObject Serialize()
    {
        return new JsonObject
        {
            ["Type"] = "AABB",
            ["Name"] = Name,
            ["Center_X"] = Center.X,
            ["Center_Y"] = Center.Y,
            ["Center_Z"] = Center.Z,
            ["Size_X"] = Size.X,
            ["Size_Y"] = Size.Y,
            ["Size_Z"] = Size.Z,
        };
    }

    public new static AxisAlignedBoxVolume Deserialize(JsonObject data)
    {
        return new AxisAlignedBoxVolume
        {
            Name = (string)data["Name"]!,
            Center = new Vector3(
                (float)data["Center_X"]!,
                (float)data["Center_Y"]!,
                (float)data["Center_Z"]!
            ),
            Size = new Vector3(
                (float)data["Size_X"]!,
                (float)data["Size_Y"]!,
                (float)data["Size_Z"]!
            ),
        };
    }
}
