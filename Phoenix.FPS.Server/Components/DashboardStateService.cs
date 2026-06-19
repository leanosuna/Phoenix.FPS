using Phoenix.FPS.Server.Services;

namespace Phoenix.FPS.Server.Components;

public class DashboardStateService
{
    public ServerSnapshot GetSnapshot()
    {
        var players = new List<PlayerSnapshot>();
        uint totalIn = 0, totalOut = 0;

        try
        {
            foreach (var player in Server.Players)
            {
                totalIn += (uint)player.PacketsIn;
                totalOut += (uint)player.PacketsOut;

                var client = Server.RiptideServer?.Clients?.FirstOrDefault(c => c.Id == player.NetID);

                players.Add(new PlayerSnapshot(
                    player.ID,
                    player.NetID,
                    player.Name ?? "",
                    player.Connected,
                    player.FPS,
                    (short)(client?.RTT ?? 0),
                    (uint)player.PacketsIn,
                    (uint)player.PacketsOut,
                    player.Ready,
                    player.State
                ));
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Dashboard GetSnapshot error: {ex.Message}");
        }

        return new ServerSnapshot(
            Server.StartTime,
            Server.ServerTPS,
            Server.CFG.Version,
            Server.GameModeStatus,
            players.Count(p => p.Connected),
            players.Count,
            totalIn,
            totalOut,
            players
        );
    }
}

public class DashboardActions
{
    public void ResetMetrics()
    {
        if (Server.RiptideServer?.Clients == null) return;
        foreach (var c in Server.RiptideServer.Clients)
            c.Metrics.Reset();
        Log.Info("Metrics reset via dashboard");
    }

    public void ClearLog()
    {
        Log.ClearLog();
        Log.Info("Log cleared via dashboard");
    }

    public void ToggleGameMode()
    {
        if (Server.GameModeStatus == "Idle")
        {
            Server.GameModeStatus = "Running";
            Log.Info("Game started via dashboard");
        }
        else
        {
            Server.GameModeStatus = "Idle";
            Log.Info("Game stopped via dashboard");
        }
    }
}
