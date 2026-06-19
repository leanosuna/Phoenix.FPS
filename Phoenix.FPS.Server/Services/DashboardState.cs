using Phoenix.FPS.Shared;

namespace Phoenix.FPS.Server.Services;

public record PlayerSnapshot(
    uint ID,
    ushort NetID,
    string Name,
    bool Connected,
    int FPS,
    short RTT,
    uint PacketsIn,
    uint PacketsOut,
    bool Ready,
    PlayerState? State
);

public record ServerSnapshot(
    DateTime StartTime,
    uint ServerTPS,
    int Version,
    string GameModeStatus,
    int ConnectedPlayers,
    int TotalPlayers,
    uint TotalPacketsIn,
    uint TotalPacketsOut,
    List<PlayerSnapshot> Players
);
