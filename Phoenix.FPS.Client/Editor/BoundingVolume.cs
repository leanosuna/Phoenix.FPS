using System.Numerics;
using System.Text.Json.Nodes;

namespace Phoenix.FPS.Client.Editor;

internal enum BoundingVolumeType
{
    AABB,
    OBB,
    Cylinder,
}

internal abstract class BoundingVolume
{
    public string Name = "";
    public bool Visible = true;
    public bool Selected;

    public abstract BoundingVolumeType Type { get; }

    public abstract void DrawGizmo();

    public abstract JsonObject Serialize();

    public static BoundingVolume Deserialize(JsonObject data)
    {
        var type = (string)data["Type"]!;
        return type switch
        {
            "AABB" => AxisAlignedBoxVolume.Deserialize(data),
            "OBB" => OrientedBoxVolume.Deserialize(data),
            "Cylinder" => CylinderVolume.Deserialize(data),
            _ => throw new ArgumentException($"Unknown volume type: {type}"),
        };
    }
}
