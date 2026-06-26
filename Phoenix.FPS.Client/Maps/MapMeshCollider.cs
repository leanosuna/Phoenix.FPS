using Phoenix.Framework.Collisions;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Phoenix.FPS.Client.Maps;

internal class MapMeshCollider
{
    public BoundingSphere BoundingSphere;
    public CollisionTriangle[] CollisionTriangles;
    public uint ID;
    public MapMeshCollider(uint mid, VertexPosUvNorm[] vertices, uint[] indices)
    {
        ID = mid;
        var triCount = indices.Length / 3;
        CollisionTriangles = new CollisionTriangle[triCount];
        
        for (var i = 0; i < triCount; i++)
        {
            var idx = i * 3;
            var i0 = indices[idx];
            var i1 = indices[idx + 1];
            var i2 = indices[idx + 2];

            var v0 = vertices[i0];
            var v1 = vertices[i1];
            var v2 = vertices[i2];

            var edge1 = v1.Pos - v0.Pos;
            var edge2 = v2.Pos - v0.Pos;
            var n = Vector3.Normalize(Vector3.Cross(edge1, edge2));

            CollisionTriangles[i] = new CollisionTriangle((uint)i, [v0.Pos, v1.Pos, v2.Pos], n);
        }
    }
    
}
