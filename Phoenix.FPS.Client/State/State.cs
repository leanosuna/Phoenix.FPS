using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.FPS.Client.State;

public abstract class State
{
    public Game game;
    public GL GL;
    public State()
    {
        game = Game.Instance;
        GL = game.GL;
    }

    internal bool preferCameraLock;


    public abstract void OnSwitch();
    public abstract void Update(double deltaTime);
    public abstract void Render(double deltaTime);
    public abstract void RenderUI();


}
