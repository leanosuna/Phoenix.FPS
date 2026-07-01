namespace Phoenix.FPS.Client.State;

public class GameState
{
    static Game game;
    public static StateMainMenu MainMenu;
    public static StateLobby Lobby;
    public static StatePlay Play;
    public static bool paused = false;
    private static State _last;

    public static State Current { get; private set; }
    public static void Init()
    {
        game = Game.Instance;
        MainMenu = new StateMainMenu { preferCameraLock = false};
        Lobby = new StateLobby { preferCameraLock = false };
        Play = new StatePlay { preferCameraLock = true };

        SwitchTo(GState.MAIN);
    }
    public static void SwitchTo(GState state)
    {
        _last = Current;
        switch (state)
        {
            case GState.MAIN:
                Current = MainMenu;
                break;
            case GState.LOBBY:
                Current = Lobby;
                break;
            case GState.PLAY:
                Current = Play;
                break;


        }
        Current.OnSwitch();
    }


    public static void SwitchToLast()
    {
        var from = Current; 
        Current = _last;
        _last = from;
        
        Current.OnSwitch();
    }

    public void Update(double dt) => Current.Update(dt);
    public void Render(double dt) => Current.Render(dt);
    public void RenderUI() => Current.RenderUI();

}
public enum GState
{
    MAIN,
    LOBBY,
    PLAY,
}

