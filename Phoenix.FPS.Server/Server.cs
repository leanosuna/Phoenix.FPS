using Phoenix.FPS.Server.Components;
using Phoenix.FPS.Server.Services;
using Phoenix.FPS.Shared;
using Riptide;
using Riptide.Utils;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Phoenix.FPS.Server;

class Server
{
    public static Riptide.Server RiptideServer = default!;
    
    public static uint ServerTPS;
    public static CFG CFG = default!;
    public static DateTime StartTime = DateTime.UtcNow;
    public static string GameModeStatus = "Idle";

    private static ConcurrentDictionary<ushort, Player> _netPlayers = new();
    private static ConcurrentDictionary<uint, Player> _idPlayers = new();
    public static ConcurrentBag<Player> Players = new();

    static bool riptideToConsole = true;
    static async Task Main(string[] args)
    {
        Log.Enabled = true;
        Log.Verbose = true;
        Log.Time = true;
        
        if(!CfgFile.Load(out CFG))
        {
            Log.Error("Cfg not found.", true);
            return;
        }
        RiptideLogger.Initialize(LogDebug, LogInfo, LogWarn, LogError, false);
        
        RiptideServer = new Riptide.Server();
        RiptideServer.Start(7777, 50);
        RiptideServer.ClientConnected += (s, e) => HandleConnect(e.Client.Id);
        RiptideServer.ClientDisconnected += (s, e) => HandleDisconnect(e.Client.Id);

        
        ServerTPS = (uint)CFG.ServerTPS;
        TargetMS = (1000 / ServerTPS);

        TimerCallback callback = TimerElapsed;

        timerId = timeSetEvent(TargetMS, 0, callback, IntPtr.Zero, 1);

        WebApplication? dashboardApp = null;
        if (CFG.DashboardEnabled)
        {
            try
            {
                dashboardApp = BuildDashboard(args);
                _ = dashboardApp.RunAsync();
                Log.Info($"Dashboard listening on http://localhost:{CFG.DashboardPort}", true);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to start dashboard: {ex.Message}", true);
            }
        }

        Log.Info("Game server started. Press Enter to exit.",true);
        Console.ReadLine();
        Log.Info("Game server stopped.", true);
        if (dashboardApp != null)
        {
            await dashboardApp.StopAsync();
            Log.Info("Dashboard stopped", true);
        }
    }

    static WebApplication BuildDashboard(string[]? args)
    {
        var builder = WebApplication.CreateBuilder(args ?? []);

        builder.WebHost.UseUrls($"http://0.0.0.0:{CFG.DashboardPort}");
        builder.WebHost.UseStaticWebAssets();

        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        builder.Services.AddSingleton<DashboardStateService>();
        builder.Services.AddSingleton<DashboardActions>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAntiforgery();

        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

        return app;
    }
    
    static void UpdateTick()
    {
        MessagesOut.BroadcastPlayerData();

    }

    static void UpdateOneSec()
    {
        //Console.Clear();
        if (Players.Count == 0)
        {
            Log.Info("server empty.",true);
            return;
        }

        foreach (var c in RiptideServer.Clients)
        {
            GetPlayerNet(c.Id, out var p);
            var m = c.Metrics;

            p!.PacketsIn = m.UnreliableIn;
            p.PacketsOut = m.UnreliableOut;

            var status = p.Connected ? $"{p.FPS} fps, {c.RTT} ms, packets in {p.PacketsIn} out {p.PacketsOut}" : "offline";
            Log.Info($"[{p.ID}] ({c.Id}) {status}", true);

            m.Reset();
        }
    }

    public static void HandleConnect(ushort id)
    {
        //Message m = 
        //    Message.Create(MessageSendMode.Reliable, ServerToClient.Version)
        //    .Add(CFG.Version);
        //RiptideServer.Send(m, id);
        Log.Debug($"net {id} Connected", true);
        GetPlayerNet(id, out var p);
        p!.Connected = true;
    }
    public static void HandleDisconnect(ushort id)
    {
        Log.Debug($"net {id} Disconnected", true);
        GetPlayerNet(id, out var p);
        p!.Connected = false;
    }
    internal static void PlayerNetIDChange(Player p, ushort netID)
    {
        Log.Debug($"net change {p.NetID}->{netID}", true);
        _netPlayers.Remove(p.NetID, out _);
        _netPlayers[netID] = p;
        p.NetID = netID;
    }

    internal static void PlayerIDChange(Player p, uint ID)
    {
        Log.Debug($"id change {p.ID}->{ID}", true);
        _idPlayers.Remove(p.ID, out _);
        _idPlayers[ID] = p;
        p.ID = ID;
    }
    public static bool GetPlayerNet(ushort id, out Player? p, bool create = true)
    {
        
        if (!_netPlayers.TryGetValue(id, out p))
        {
            if (!create)
                return false;
            
            p = new Player(id);
            _netPlayers[id] = p;
            Players.Add(p);
        }
        
        return true;
    }
    public static bool GetPlayerFromId(uint id, out Player? p)
    {
        return _idPlayers.TryGetValue(id, out p);
    }

    
    private delegate void TimerCallback(uint id, uint msg, IntPtr user, IntPtr param1, IntPtr param2);

    [DllImport("winmm.dll", SetLastError = true)]
    private static extern uint timeSetEvent(uint msDelay, uint msResolution, TimerCallback callback, IntPtr user, uint eventType);

    private static uint timerId;
    static uint TargetMS;


    private static void TimerElapsed(uint id, uint msg, IntPtr user, IntPtr param1, IntPtr param2)
    {
        ServerUpdate();
    }

    static int t = 0;
    static DateTime lastDt = DateTime.UtcNow;
    private static void ServerUpdate()
    {
        RiptideServer.Update();

        UpdateTick();
        t++;
        if (t == ServerTPS) //1 sec 
        {
            t = 0;
            //var dt = DateTime.UtcNow.AddHours(-3);
            //var diff = dt - lastDt;
            //var tps = 1000 / diff.TotalMilliseconds * ServerTPS;
            //var perc = 100000 / diff.TotalMilliseconds;
            //LogInfo($"[Server TPS = {Math.Round(tps)} : {Math.Round(perc)}%]");
            //lastDt = dt;

            UpdateOneSec();

        }
    }

    private static void LogError(string log) => Log.Error(log, riptideToConsole);

    private static void LogWarn(string log) => Log.Warn(log, riptideToConsole);

    private static void LogInfo(string log) => Log.Info(log, riptideToConsole);

    private static void LogDebug(string log) => Log.Debug(log, riptideToConsole);

 
}