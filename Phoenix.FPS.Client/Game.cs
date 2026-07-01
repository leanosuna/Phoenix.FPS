using ImGuiNET;
using Phoenix.FPS.Client.Editor;
using Phoenix.FPS.Client.Maps;
using Phoenix.FPS.Client.Network;
using Phoenix.FPS.Client.Players;
using Phoenix.FPS.Client.State;
using Phoenix.Framework;
using Phoenix.Framework.AssetImport;
using Phoenix.Framework.Cameras;
using Phoenix.Framework.Collisions;
using Phoenix.Framework.Maths;
using Phoenix.Framework.Rendering;
using Phoenix.Framework.Rendering.Geometry.Model;
using Phoenix.Framework.Rendering.Geometry.Model.Meshes;
using Phoenix.Framework.Rendering.Geometry.Vertices;
using Phoenix.Framework.Rendering.GUI;
using Phoenix.Framework.Rendering.Primitives;
using Phoenix.Framework.Rendering.Textures;
using Phoenix.Framework.ShaderHelpers;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Collections.Concurrent;
using System.Numerics;
namespace Phoenix.FPS.Client;

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
    public ShaderModel ShaderModel;
    public CFG CFG;
    
    Paintball MapPaintball;
    public SkyBox SkyBox;

    Player p;
    FreeCamera FreeCamera;
    DebugWindow _debugWindow;
    bool hit;
    
    
    protected override void Initialize()
    {
        Window.Title = "FPS Client";
        Instance = this;
        MessagesIn.Init();
        MessagesOut.Init();

        Log.Enabled = true;
        Log.ConsoleWrite = true;
        if (!CfgFile.Load(out CFG))
        {
            Log.Error("CFG not found");
            Stop();
        }
        if(CFG.PlayerID == 0)
        {
            CFG.PlayerID = (uint)new Random().NextInt64();
        }
        CfgFile.Save(CFG);

        ApplyWindowSettings();
        p = new Player { Position = Vector3.UnitY * 1, Pitch = 0, Yaw = 0};

        ShaderModel = new ShaderModel();
        ShaderModel.AttachUBO(CommonUboHandle, "CommonData");
                
        MapPaintball = new Paintball();
        SkyBox = new SkyBox();
        
        var cam = new FreeCamera(
            game: this,
            position: new Vector3(0, 0, -10),
            yaw: MathHelper.PiOver2,
            pitch: 0f,
            fov: MathHelper.PiOver2,
            nearPlane: 0.1f,
            farPlane: 1000f,
            aspectRatio: WindowWidth / (float)WindowHeight
        );
        cam.SetMoveKeys(Key.W, Key.S, Key.A, Key.D, Key.Space, Key.ControlLeft, Key.ShiftLeft, 2f);
        //cam.SetPitchYawKeys(Key.Up, Key.Down, Key.Left, Key.Right, Vector2.One);
        cam.MouseAim = true;
        cam.MoveSpeed = 15f;
        Camera = cam;
        FreeCamera = cam;
        Gizmos.Enabled = true;
        Input.SetMouseMode(CursorMode.Raw);  
        Network.Client.Init();

        GameState.Init();

        _debugWindow = new DebugWindow();

    }
    ConcurrentBag<(Vector3 a, Vector3 b, Vector3 c, Vector3 h)> thit = new();
    protected override void Update(double deltaTime)
    {
        GameState.Current.Update(deltaTime);

        var cam = ((FreeCamera)Camera);
        var dt = (float)deltaTime;
        if (Input.KeyDown(Key.Escape))
            Stop();

        if (Input.KeyDownOnce(Key.CapsLock))
        {
            Input.ToggleMouseMode();
            cam.MouseAim = !cam.MouseAim;
        }

        MapPaintball.SkipDraw = Input.KeyDown(Key.Number1);
        if(Input.KeyDown(Key.Up))
        {
            p.Position += p.FrontDir * dt * 4;
        }
        if (Input.KeyDown(Key.Down))
        {
            p.Position -= p.FrontDir * dt * 4;
        }
        if (Input.KeyDown(Key.Left))
        {
            p.Yaw += dt * 2;
        }
        if (Input.KeyDown(Key.Right))
        {
            p.Yaw -= dt * 2;
        }

        //_debugWindow.Volumes

        // Game logic ...
        var t = (float)Graphics.Metrics.Time;
        cam.Update(deltaTime);
        
        Network.Client.Update();
        
        p.Update();
        //var ray = new Ray(p.Position, p.FrontDir);
        var ray = new Ray(FreeCamera.Position, FreeCamera.Front);
        thit.Clear();
        
        
        if (Input.MouseLeftDownOnce() && cam.MouseAim)
        {

            foreach (var col in MapPaintball.Colliders)
            {
                var tris = col.CollisionTriangles.OrderByDescending(ct => DistanceToP(ct, FreeCamera.Position));

                foreach (var tr in tris)
                {

                    var hitPos = ray.Intersects(tr.V[0], tr.V[1], tr.V[2]);

                    if (hitPos.HasValue)
                    {
                        if (Vector3.DistanceSquared(hitPos.Value, FreeCamera.Position) < 50)
                        {
                            thit.Add((tr.V[0], tr.V[1], tr.V[2], hitPos.Value));
                        }
                    }
                }
            }
            hit = thit.Count > 0;

            if(hit)
            {
                var nearest = thit.OrderBy(t => Vector3.DistanceSquared(t.h, FreeCamera.Position)).First();
                _debugWindow.AddReferencePoint(nearest.h);
            }
        }




        if (Input.MouseRightDownOnce() && cam.MouseAim)
            _debugWindow.CancelSelection();

        
        Gizmos.AddLine(p.Position, p.Position + p.FrontDir * 2, Vector3.One);

    }

    float DistanceToP(CollisionTriangle ct, Vector3 pos)
    {
        var centroid = (ct.V[0] + ct.V[1] + ct.V[2]) / 3;
        return Vector3.DistanceSquared(p.Position, centroid);
    }

    bool ValidateCT(CollisionTriangle ct, Vector3 pos, float checkDistSq = 5)
    {
        var centroid = (ct.V[0] + ct.V[1] + ct.V[2]) / 3;

        return Vector3.DistanceSquared(pos, ct.V[0]) < checkDistSq ||
            Vector3.DistanceSquared(pos, ct.V[1]) < checkDistSq ||
            Vector3.DistanceSquared(p.Position, ct.V[2]) < checkDistSq ||
            Vector3.DistanceSquared(p.Position, centroid) < checkDistSq;
    }
    protected override void Render(double deltaTime)
    {
        
        Graphics.SetClearColor(new Vector4(0.1f, 0.1f, 0.1f, 1));
        Graphics.ClearRenderTarget();

        Graphics.SetFaceCulling(false);
        Graphics.SetDepthTest(true, GLEnum.Lequal);

        SkyBox.Draw();
        
        GameState.Current.Render(deltaTime);

        var w = Matrix4x4.CreateScale(0.007f);
        var i = 0;

        MapPaintball.Draw();
        

        Gizmos.AddCube(p.Transform, new Vector3(0, 1, 1), hit);

        foreach(var t in thit)
        {
            Gizmos.AddLine(t.a, t.b, new Vector3(1, 0, 1));
            Gizmos.AddLine(t.a, t.c, new Vector3(1, 0, 1));
            Gizmos.AddLine(t.b, t.c, new Vector3(1, 0, 1));
            Gizmos.AddSphere(t.h, 0.1f, new Vector3(0, 1, 1));

        }

        _debugWindow.RenderGizmos();

    }

    int crosshairLen = 15;
    protected override void RenderUI()
    {
        GameState.Current.RenderUI();


        var drawList = ImGui.GetForegroundDrawList();

        var center = WindowSize / 2;
        var horizontalL = new Vector2(center.X - crosshairLen / 2, center.Y);
        var horizontalR = new Vector2(center.X + crosshairLen / 2, center.Y);
        var verticalU = new Vector2(center.X, center.Y + crosshairLen / 2);
        var verticalD = new Vector2(center.X, center.Y - crosshairLen / 2);


        drawList.AddLine(horizontalL, horizontalR, ImGui.ColorConvertFloat4ToU32(Vector4.One),2);
        drawList.AddLine(verticalU, verticalD, ImGui.ColorConvertFloat4ToU32(Vector4.One),2);

        UI.DrawRAlignedText($"FPS {Graphics.Metrics.FPS_SAMPLE}",
            position: new Vector2(WindowWidth, 10),
            color: Vector4.One,
            size: 20);

        _debugWindow.RenderUI();
            
    }
    
    public void ValidateWindowSettings()
    {
        CFG.FPSLimit = Math.Clamp(CFG.FPSLimit, 0, 1000);
        CFG.WindowWidth = Math.Clamp(CFG.WindowWidth, 0, 10000);
        CFG.WindowHeight = Math.Clamp(CFG.WindowHeight, 0, 10000);
    }

    public void ApplyWindowSettings()
    {

        Window.VSync = CFG.Vsync;
        Window.FramesPerSecond = CFG.FPSLimit;

        var full = CFG.FullScreen || CFG.WindowedFullScreen;
        var max = CFG.WindowedMaximized;
        var window = new Vector2D<int>(CFG.WindowWidth, CFG.WindowHeight);
        var res = full?
            Window.Monitor?.VideoMode.Resolution: window;
        
        if (!res.HasValue)
            res = window;

        Graphics.SetResolution(res.Value.ToNum(), full);
        Window.Center();
        //Window.WindowBorder = full ? WindowBorder.Hidden : WindowBorder.Resizable;
        Window.WindowState = max ? WindowState.Maximized : WindowState.Normal;
    }

    protected override void OnWindowResize(Vector2 size)
    {
        // Something needs resizing...
    }

    protected override void OnClose()
    {
        _debugWindow?.Save();
    }
}
