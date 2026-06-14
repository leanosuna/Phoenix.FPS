using Phoenix.Framework;
using Phoenix.Framework.AssetImport;
using Phoenix.Framework.Cameras;
using Phoenix.Framework.Rendering;
using Phoenix.Framework.Rendering.Geometry.Model;
using Phoenix.Framework.Rendering.Geometry.Model.Meshes;
using Phoenix.Framework.Rendering.Geometry.Vertices;
using Phoenix.Framework.Rendering.GUI;
using Phoenix.Framework.Rendering.Textures;
using Phoenix.Framework.ShaderHelpers;
using Phoenix.Framework.Maths;

using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Numerics;
using Phoenix.Framework.Rendering.Primitives;
using ImGuiNET;
//using Cube as Phoenix
namespace Phoenix.FPS.Client
{
    public class Game : PhoenixGame
    {
        public static Game Instance = default!; 
        Matrix4x4 _cubeWorld = Matrix4x4.Identity;

        protected override void InitialLoadScreen()
        {
            UI.DrawText("Custom startup screen", 
                position: Vector2.Zero, 
                color: Vector4.One, 
                size: 30);
        }
        ShaderModel shaderModel;
        GLTextureCube cubeTex;
        ShaderSkybox ShaderSkybox;
        Cube cubePrim;
        protected override void Initialize()
        {
            Instance = this;
            //Primitive.SetGL(GL);
            Window.VSync = false;
            Window.FramesPerSecond = 165;
            Log.Enabled = true;
            Log.ConsoleWrite = true;
            
            shaderModel = new ShaderModel();
            shaderModel.AttachUBO(CommonUboHandle, "CommonData");

            ShaderSkybox = new ShaderSkybox();
            ShaderSkybox.AttachUBO(CommonUboHandle, "CommonData");

            //model = AssetLoader.LoadModel("3D/player/swatguy/Ch15_nonPBR");

            var basePath = "Textures/skybox/";
            cubeTex = AssetLoader.LoadTextureCube(
                [basePath + "right",
                basePath + "left",
                basePath + "top",
                basePath + "bottom",
                basePath + "front",
                basePath + "back"]);


            // Asset loading ...
            var cam =  new FreeCamera(
                game: this,
                position: new Vector3(0, 0, -10),
                yaw: MathHelper.PiOver2,
                pitch: 0f,
                fov: MathHelper.PiOver2,
                nearPlane: 0.1f, 
                farPlane: 1000f,
                aspectRatio: WindowWidth / (float)WindowHeight
            );
            cam.SetMoveKeys(Key.W, Key.S, Key.A, Key.D, Key.Space, Key.AltLeft, Key.ShiftLeft, 2f);
            cam.SetPitchYawKeys(Key.Up, Key.Down, Key.Left, Key.Right, Vector2.One);
            cam.MouseAim = true;

            Camera = cam;

            Gizmos.Enabled = true;

            cubePrim = Cube.Create(new InfoCube { MeshPrimitiveType = PrimitiveType.Triangles, Uv = true });

            Network.Client.Init(this);
        }
                
        protected override void Update(double deltaTime)
        {
            var cam = ((FreeCamera)Camera);
            if (InputManager.KeyDown(Key.Escape))
                Stop();

            if (InputManager.KeyDownOnce(Key.Tab))
            {
                InputManager.ToggleMouseMode();
                cam.MouseAim = !cam.MouseAim;
            }


            // Game logic ...
            var t = (float)Graphics.Time;
            cam.Update(deltaTime);
            _cubeWorld = Matrix4x4.CreateScale(5f)
                * MathHelper.RotationMxFromYawPitchRoll(t, MathF.Sin(t), MathF.Cos(t));

            Network.Client.Update();
        }

        protected override void Render(double deltaTime)
        {
            // Render logic ...
            Graphics.SetClearColor(new Vector4(0.1f, 0.1f, 0.1f, 1));
            Graphics.ClearRenderTarget();

            //Gizmos.AddCube(_cubeWorld, Vector3.One);
            //Gizmos.AddCube()
            Graphics.SetFaceCulling(false);
            Graphics.SetDepthTest(true, GLEnum.Lequal);
           

            shaderModel.World.Set(_cubeWorld);
            shaderModel.Tex.Set(cubeTex);
            
            ShaderSkybox.Use();
            ShaderSkybox.Tex.Set(cubeTex);

            Graphics.SetFaceCulling(true, GLEnum.Front);
            cubePrim.Draw();

            Gizmos.AddCube(_cubeWorld, Vector3.One);
            Gizmos.AddAxisLines(400);
        }

        protected override void RenderUI()
        {
            // UI pass...
            UI.DrawRAlignedText($"FPS {(int)Graphics.FPS_SAMPLE}",
                position: new Vector2(WindowWidth, 10),
                color: Vector4.One,
                size: 20);

            int fps = (int)Window.FramesPerSecond;
            
            if(ImGui.DragInt("fps limit", ref fps, 1, 0, 1000))
            {
                Window.FramesPerSecond = fps;
            }

            
            
        }

        protected override void OnWindowResize(Vector2 size)
        {
            // Something needs resizing...
        }

        protected override void OnClose()
        {
            // Something needs disposing...
        }
    }
}
