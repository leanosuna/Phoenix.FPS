using Phoenix.Framework.AssetImport;
using Phoenix.Framework.Rendering;
using Phoenix.Framework.Rendering.Primitives;
using Phoenix.Framework.Rendering.Shaders;
using Phoenix.Framework.Rendering.Textures;
using Phoenix.Framework.ShaderHelpers;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.FPS.Client.Maps;

public class SkyBox
{
    Cube _cube;
    ShaderSkybox _shader;
    GLTextureCube _tex;
    Game game;
    public SkyBox()
    {
        game = Game.Instance;
        _shader = new ShaderSkybox();
        _shader.AttachUBO(game.CommonUboHandle, "CommonData");
        _cube = Cube.Create(new InfoCube { MeshPrimitiveType = PrimitiveType.Triangles, Uv = true });

        var basePath = "Textures/skybox/";
        _tex = AssetLoader.LoadTextureCube(
            [basePath + "right",
            basePath + "left",
            basePath + "top",
            basePath + "bottom",
            basePath + "front",
            basePath + "back"]);
    }

    public void Draw()
    {
        _shader.Use();
        _shader.Tex.Set(_tex);


        var pre = game.Graphics.FaceCulling;
        game.Graphics.SetFaceCulling(true, GLEnum.Front);
        _cube.Draw();
        game.Graphics.SetFaceCulling(pre.Enabled, pre.Face, pre.FrontIsCcw);

    }

}
