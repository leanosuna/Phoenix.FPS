using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.FPS.Server;

public sealed class CFG
{
    public int ServerTPS { get; set; } = 200;
    public int Version { get; set; } = 1;
    public int DashboardPort { get; set; } = 7775;
    public bool DashboardEnabled { get; set; } = true;
}
