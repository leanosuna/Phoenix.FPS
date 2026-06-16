using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Phoenix.FPS.Client;

public sealed class CFG
{
    public string PlayerName { get; set; } = "no-name";
    public uint PlayerID { get; set; } = uint.MaxValue;
    public int Version { get; set; } = 1;

    public bool Vsync { get; set; } = false;
    public int FPSLimit { get; set; } = 300;

    public bool FullScreen { get; set; } = false;
    public bool WindowedFullScreen { get; set; } = true;
    public bool WindowedMaximized { get; set; } = false;
    public bool Windowed { get; set; } = false;
    public int WindowWidth { get; set; } = 1280;
    public int WindowHeight { get; set; } = 720;


}
