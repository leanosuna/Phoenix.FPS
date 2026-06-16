namespace Phoenix.FPS.Client.State;

public class GameStateManager
{
    static Game game;
    public static StateMainMenu mainMenu;
    //public static StateRun run;
    //public static StateCarSelect carSelect;

    public static bool paused = false;
    static GameState last;
    public static void Init()
    {
        game = Game.Instance;
        //mainMenu = new StateMainMenu();
        //run = new StateRun();
        //carSelect = new StateCarSelect();

        //SwitchTo(State.MAIN);
    }
    //public static void SwitchTo(State state)
    //{
    //    last = game.gameState;
    //    GameState newState = game.gameState;
    //    switch (state)
    //    {
    //        case State.MAIN:
    //            newState = mainMenu;
    //            break;
    //        case State.RUN:
    //            newState = run;
    //            break;
    //        case State.CARSELECT:
    //            newState = carSelect;
    //            break;


    //    }

    //    game.gameState = newState;
    //    newState.OnSwitch();

    //}


    //public static void SwitchToLast()
    //{

    //    game.gameState = last;
    //    game.gameState.OnSwitch();
    //}

}
public enum State
{
    MAIN,
    OPTIONS,
    CARSELECT,
    RUN,
}

