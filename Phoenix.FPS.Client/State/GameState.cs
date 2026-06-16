using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.FPS.Client.State;

public abstract class GameState
{
    public Game game;
    public GL GL;
    public GameState()
    {
        game = Game.Instance;
        GL = game.GL;
    }

    public abstract void OnSwitch();
    public abstract void Update(double deltaTime);
    public abstract void Render(double deltaTime);
    public abstract void FocusChanged(bool hasFocus);


}
