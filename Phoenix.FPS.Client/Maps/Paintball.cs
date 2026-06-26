using Phoenix.Framework.AssetImport;
using Phoenix.Framework.Collisions;
using Phoenix.Framework.Rendering.Geometry.Model;
using Phoenix.Framework.Rendering.Gizmos;
using Phoenix.Framework.Rendering.Textures;
using Phoenix.Framework.ShaderHelpers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Phoenix.FPS.Client.Maps;

internal class Paintball
{
    Model _model;
    Game game;
    GLTexture[] _textures;
    ShaderModel ShaderModel;

    internal MapMeshCollider[] Colliders;
    //float MapScale = 0.007f;
    float MapScale = 1f;

    Matrix4x4 World;
    public Paintball()
    {
        game = Game.Instance;
        _model = AssetLoader.LoadModel("3D/maps/paintball/paintball");

        var path = "Textures/paintball/";
        _textures = [
            AssetLoader.LoadTexture(path + "knotty-plywood-bl/knotty-plywood_albedo"),
            AssetLoader.LoadTexture(path + "streaky-plywood-bl/streaky-plywood_albedo"),
            AssetLoader.LoadTexture(path + "rusted-steel-bl/rusted-steel_albedo"),
            AssetLoader.LoadTexture(path + "pebbled-asphalt1-bl/pebbled_asphalt_albedo"),
            ];

        ShaderModel = game.ShaderModel;
        World = Matrix4x4.CreateScale(MapScale);
        ProcessCollisionTriangles();
    }

    void ProcessCollisionTriangles()
    {
        List<MapMeshCollider> mapColliders = new();
        uint id = 0;
        foreach (var part in _model.Parts)
        {
            foreach(var mesh in part.Meshes)
            {
                var meshVertices = mesh.GetVertexData<VertexPosUvNorm>();
                var meshIndices = mesh.GetIndexData();

                mapColliders.Add(new MapMeshCollider(id++, meshVertices, meshIndices));
            }
        }
        Colliders = mapColliders.ToArray();

        //var meshVertices = _model.GetVertexData<VertexPosUvNorm>();
        //uint id = 0;
        //Colliders = meshVertices.Select(mv => { return new MapMeshCollider(MapScale, id++, mv); }).ToArray();

    }
    public bool SkipDraw;
    public void Draw()
    {
        if (SkipDraw)
            return;

        _model.Draw(ShaderModel, PerElement);

        //foreach (var item in Colliders)
        //{
        //    foreach(var col in item.CollisionTriangles)
        //    {
        //        game.Gizmos.AddCube(col.V[0], Vector3.One * 0.25f, Vector3.One);
        //    }
        //}
    }
    void PerElement(int p, string pName, int m, string mName, Matrix4x4 transform)
    {
        var w = World;
        ShaderModel.World.Set(w);

        if (pName != "Cube")
        {
            //ErrorListWindow.Add($"{i} {m.Name} {colors[i]}");

            var useTex = !(m == 3 || m == 4 || m == 8 || m == 9 || m == 14);
            ShaderModel.UseTex.Set(useTex);
            ShaderModel.Color.Set(Vector3.One);
            ShaderModel.Tex.Set(TexIndex(m));

        }
        else
        {
            ShaderModel.UseTex.Set(true);
            ShaderModel.Tex.Set(TexIndex(-1));
        }

    }

    public GLTexture TexIndex(int i)
    {
        switch (i)
        {
            case 0:
            case 2:
                return _textures[0];
            case 1:
                return _textures[1];

            case 5:
            case 6:
            case 7:
            case 10:
            case 11:
            case 12:
            case 13:
                return _textures[2];

            default: return _textures[3];
        }
    }
}
