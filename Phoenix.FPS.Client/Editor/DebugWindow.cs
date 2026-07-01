using ImGuiNET;
using System.Numerics;
using System.Collections.Generic;

namespace Phoenix.FPS.Client.Editor;

internal class DebugWindow
{
    public List<ReferencePoint> ReferencePoints = new();
    public List<BoundingVolume> Volumes = new();

    HashSet<int> _selectedRefIndices = new();
    int? _selectedVolIndex;
    int _aabbCount;
    int _obbCount;
    int _cylCount;
    bool _clearOnCreate;

    public DebugWindow()
    {
        foreach (var v in VolumeSerializer.Load())
            Volumes.Add(v);

        foreach (var v in Volumes)
        {
            if (v is AxisAlignedBoxVolume) _aabbCount++;
            else if (v is OrientedBoxVolume) _obbCount++;
            else if (v is CylinderVolume) _cylCount++;
        }
    }

    public void AddReferencePoint(Vector3 pos)
    {
        var countBefore = ReferencePoints.Count;
        ReferencePoints.Add(new ReferencePoint
        {
            Position = pos,
            SourceLabel = $"Pt{ReferencePoints.Count}",
        });
        _selectedRefIndices.Add(countBefore);
    }

    public void CancelSelection()
    {
        foreach (var v in Volumes)
            v.Selected = false;
        _selectedRefIndices.Clear();
        _selectedVolIndex = null;
    }

    public void RenderGizmos()
    {
        var gizmos = Game.Instance.Gizmos;

        foreach (var pt in ReferencePoints)
            gizmos.AddSphere(pt.Position, 0.12f, new Vector3(1, 0.5f, 0));

        foreach (var vol in Volumes)
            vol.DrawGizmo();
    }

    public void RenderUI()
    {
        ImGui.Begin("Debug Volumes");

        ReferencePointsPanel();
        ImGui.Separator();
        VolumesPanel();
        ImGui.Separator();
        PropertiesPanel();
        ImGui.Separator();
        SaveLoadPanel();

        ImGui.End();
    }

    void ReferencePointsPanel()
    {
        if (ImGui.CollapsingHeader($"Reference Points ({ReferencePoints.Count})", ImGuiTreeNodeFlags.DefaultOpen))
        {
            if (ReferencePoints.Count == 0)
                ImGui.TextDisabled("Left-click on map (Tab first) to add points");

            int i = 0;
            while (i < ReferencePoints.Count)
            {
                ImGui.PushID($"ref{i}");

                if (ImGui.SmallButton("X"))
                {
                    ReferencePoints.RemoveAt(i);
                    var toAdjust = new List<int>();
                    foreach (var si in _selectedRefIndices)
                        if (si > i)
                            toAdjust.Add(si);
                    foreach (var si in toAdjust)
                    {
                        _selectedRefIndices.Remove(si);
                        _selectedRefIndices.Add(si - 1);
                    }
                    _selectedRefIndices.Remove(i);
                    ImGui.PopID();
                    continue;
                }
                ImGui.SameLine();
                var pt = ReferencePoints[i];
                bool selected = _selectedRefIndices.Contains(i);
                if (ImGui.Selectable($"[{i}] ({pt.Position.X:F2}, {pt.Position.Y:F2}, {pt.Position.Z:F2})", selected))
                {
                    if (selected)
                        _selectedRefIndices.Remove(i);
                    else
                        _selectedRefIndices.Add(i);
                }

                ImGui.PopID();
                i++;
            }

            ImGui.Spacing();

            ImGui.Checkbox("Clear on create", ref _clearOnCreate);

            ImGui.SameLine();
            if (ImGui.Button("Clear All"))
            {
                ReferencePoints.Clear();
                _selectedRefIndices.Clear();
            }

            if (_selectedRefIndices.Count >= 1)
            {
                if (ImGui.Button("Create AABB"))
                    CreateAABBFromSelected();
                ImGui.SameLine();
                if (ImGui.Button("Create Cyl"))
                    CreateCylinderFromSelected();
                ImGui.SameLine();
                if (ImGui.Button("Create OBB"))
                    CreateOBBFromSelected();
            }
        }
    }

    void CreateAABBFromSelected()
    {
        var pts = GetSelectedPoints();
        Vector3 center, size;
        if (pts.Count >= 2)
        {
            var min = Vector3.Min(pts[0], pts[1]);
            var max = Vector3.Max(pts[0], pts[1]);
            center = (min + max) * 0.5f;
            size = max - min;
        }
        else
        {
            center = pts[0];
            size = Vector3.One * 2;
        }

        _aabbCount++;
        Volumes.Add(new AxisAlignedBoxVolume($"AABB_{_aabbCount}", center, size));
        if (_clearOnCreate) ClearReferencePoints();
    }

    void CreateCylinderFromSelected()
    {
        var pos = GetFirstSelectedPoint();
        _cylCount++;
        Volumes.Add(new CylinderVolume
        {
            Name = $"Cyl_{_cylCount}",
            Position = pos,
            Radius = 1f,
            Height = 2f,
        });
        if (_clearOnCreate) ClearReferencePoints();
    }

    void CreateOBBFromSelected()
    {
        var pos = GetFirstSelectedPoint();
        _obbCount++;
        Volumes.Add(new OrientedBoxVolume
        {
            Name = $"OBB_{_obbCount}",
            Position = pos,
            Size = Vector3.One * 2,
        });
        if (_clearOnCreate) ClearReferencePoints();
    }

    void ClearReferencePoints()
    {
        ReferencePoints.Clear();
        _selectedRefIndices.Clear();
    }

    List<Vector3> GetSelectedPoints()
    {
        var list = new List<Vector3>();
        foreach (var idx in _selectedRefIndices)
            if (idx < ReferencePoints.Count)
                list.Add(ReferencePoints[idx].Position);
        return list;
    }

    Vector3 GetFirstSelectedPoint()
    {
        foreach (var idx in _selectedRefIndices)
            if (idx < ReferencePoints.Count)
                return ReferencePoints[idx].Position;
        return Vector3.Zero;
    }

    void VolumesPanel()
    {
        if (ImGui.CollapsingHeader($"Volumes ({Volumes.Count})", ImGuiTreeNodeFlags.DefaultOpen))
        {
            if (ImGui.Button("Add AABB"))
            {
                _aabbCount++;
                Volumes.Add(new AxisAlignedBoxVolume($"AABB_{_aabbCount}", Vector3.Zero, Vector3.One));
            }
            ImGui.SameLine();
            if (ImGui.Button("Add Cyl"))
            {
                _cylCount++;
                Volumes.Add(new CylinderVolume
                {
                    Name = $"Cyl_{_cylCount}",
                    Position = Vector3.Zero,
                    Radius = 1f,
                    Height = 2f,
                });
            }
            ImGui.SameLine();
            if (ImGui.Button("Add OBB"))
            {
                _obbCount++;
                Volumes.Add(new OrientedBoxVolume
                {
                    Name = $"OBB_{_obbCount}",
                    Position = Vector3.Zero,
                    Size = Vector3.One * 2,
                });
            }

            int i = 0;
            while (i < Volumes.Count)
            {
                var vol = Volumes[i];
                ImGui.PushID($"vol{i}");

                ImGui.Checkbox("##vis", ref vol.Visible);
                ImGui.SameLine();
                if (ImGui.SmallButton("X"))
                {
                    Volumes.RemoveAt(i);
                    if (_selectedVolIndex == i)
                        _selectedVolIndex = null;
                    else if (_selectedVolIndex > i)
                        _selectedVolIndex--;
                    ImGui.PopID();
                    continue;
                }
                ImGui.SameLine();
                if (ImGui.Selectable($"{vol.Name}", _selectedVolIndex == i))
                    SelectVolume(i);

                ImGui.PopID();
                i++;
            }
        }
    }

    void SelectVolume(int index)
    {
        foreach (var v in Volumes)
            v.Selected = false;

        _selectedVolIndex = index;
        if (index >= 0 && index < Volumes.Count)
            Volumes[index].Selected = true;
    }

    BoundingVolume DuplicateVolume(BoundingVolume original)
    {
        return original switch
        {
            AxisAlignedBoxVolume aabb => new AxisAlignedBoxVolume
            {
                Name = NextName(BoundingVolumeType.AABB),
                Center = aabb.Center,
                Size = aabb.Size,
                Visible = aabb.Visible,
            },
            OrientedBoxVolume obb => new OrientedBoxVolume
            {
                Name = NextName(BoundingVolumeType.OBB),
                Position = obb.Position,
                Size = obb.Size,
                Yaw = obb.Yaw,
                Pitch = obb.Pitch,
                Roll = obb.Roll,
                Visible = obb.Visible,
            },
            CylinderVolume cyl => new CylinderVolume
            {
                Name = NextName(BoundingVolumeType.Cylinder),
                Position = cyl.Position,
                Radius = cyl.Radius,
                Height = cyl.Height,
                Rotation = cyl.Rotation,
                Visible = cyl.Visible,
            },
            _ => throw new ArgumentException($"Unknown volume type: {original.GetType()}"),
        };
    }

    string NextName(BoundingVolumeType type)
    {
        return type switch
        {
            BoundingVolumeType.AABB => $"AABB_{++_aabbCount}",
            BoundingVolumeType.OBB => $"OBB_{++_obbCount}",
            BoundingVolumeType.Cylinder => $"Cyl_{++_cylCount}",
            _ => "Volume",
        };
    }

    void PropertiesPanel()
    {
        if (_selectedVolIndex == null || _selectedVolIndex >= Volumes.Count)
        {
            ImGui.TextDisabled("Select a volume to edit");
            return;
        }

        var vol = Volumes[_selectedVolIndex.Value];

        var label = vol.Name;
        if (ImGui.InputText("Name", ref label, 64))
            vol.Name = label;

        ImGui.SameLine();
        if (ImGui.Button("Duplicate"))
        {
            var copy = DuplicateVolume(vol);
            Volumes.Add(copy);
            SelectVolume(Volumes.Count - 1);
        }

        ImGui.Separator();

        switch (vol)
        {
            case AxisAlignedBoxVolume aabb:
                EditAABBProperties(aabb);
                break;
            case OrientedBoxVolume obb:
                EditOBBProperties(obb);
                break;
            case CylinderVolume cyl:
                EditCylinderProperties(cyl);
                break;
        }
    }

    void EditAABBProperties(AxisAlignedBoxVolume vol)
    {
        var center = vol.Center;
        var size = vol.Size;

        if (ImGui.DragFloat3("Center", ref center, 0.1f))
            vol.Center = center;

        if (ImGui.DragFloat3("Size", ref size, 0.1f, 0.01f, 100f))
            vol.Size = Vector3.Max(size, Vector3.One * 0.01f);

        ImGui.Text($"Min: {vol.Center.X - vol.Size.X * 0.5f:F2}, {vol.Center.Y - vol.Size.Y * 0.5f:F2}, {vol.Center.Z - vol.Size.Z * 0.5f:F2}");
        ImGui.Text($"Max: {vol.Center.X + vol.Size.X * 0.5f:F2}, {vol.Center.Y + vol.Size.Y * 0.5f:F2}, {vol.Center.Z + vol.Size.Z * 0.5f:F2}");

        if (_selectedRefIndices.Count >= 2)
        {
            ImGui.Separator();
            if (ImGui.Button("Fit to Selected Pts"))
            {
                var pts = GetSelectedPoints();
                var min = Vector3.Min(pts[0], pts[1]);
                var max = Vector3.Max(pts[0], pts[1]);
                vol.Center = (min + max) * 0.5f;
                vol.Size = max - min;
            }
        }
    }

    void EditOBBProperties(OrientedBoxVolume vol)
    {
        var pos = vol.Position;
        var size = vol.Size;
        var yaw = vol.Yaw;
        var pitch = vol.Pitch;
        var roll = vol.Roll;

        if (ImGui.DragFloat3("Position", ref pos, 0.1f))
            vol.Position = pos;

        if (ImGui.DragFloat3("Size", ref size, 0.1f, 0.01f, 100f))
            vol.Size = Vector3.Max(size, Vector3.One * 0.01f);

        if (ImGui.DragFloat("Yaw", ref yaw, 0.05f))
            vol.Yaw = yaw;

        if (ImGui.DragFloat("Pitch", ref pitch, 0.05f))
            vol.Pitch = pitch;

        if (ImGui.DragFloat("Roll", ref roll, 0.05f))
            vol.Roll = roll;

        ImGui.Separator();
        if (ImGui.Button("Reset Rotation"))
        {
            vol.Yaw = 0;
            vol.Pitch = 0;
            vol.Roll = 0;
        }
    }

    void EditCylinderProperties(CylinderVolume vol)
    {
        var pos = vol.Position;
        var radius = vol.Radius;
        var height = vol.Height;
        var rot = vol.Rotation;

        if (ImGui.DragFloat3("Position", ref pos, 0.1f))
            vol.Position = pos;

        if (ImGui.DragFloat("Radius", ref radius, 0.05f, 0.01f, 100f))
            vol.Radius = radius;

        if (ImGui.DragFloat("Height", ref height, 0.05f, 0.01f, 100f))
            vol.Height = height;

        if (ImGui.DragFloat("Rotation", ref rot, 0.05f))
            vol.Rotation = rot;
    }

    void SaveLoadPanel()
    {
        if (ImGui.Button("Save Volumes"))
            VolumeSerializer.Save(Volumes);

        ImGui.SameLine();
        if (ImGui.Button("Load Volumes"))
        {
            foreach (var v in Volumes)
                v.Selected = false;
            _selectedVolIndex = null;
            Volumes = VolumeSerializer.Load();
        }
    }

    public void Save() => VolumeSerializer.Save(Volumes);
}
