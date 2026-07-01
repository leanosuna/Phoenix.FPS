using Phoenix.Framework.Rendering.GUI;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Phoenix.FPS.Client.State;

public class StateMainMenu : State
{
    public override void OnSwitch()
    {
        //game.Graphics.Metrics.UPD_SAMPLE_RATE = 0.15f;
        //game.Graphics.Metrics.FPS_SAMPLE_RATE = 0.15f;

    }
    public override void Update(double deltaTime)
    {
    }


    public override void Render(double deltaTime)
    {
    }

    public override void RenderUI()
    {
        game.UI.DrawHCenteredText($"Main", new Vector2(game.WindowWidth / 2, 0), Vector4.One, 18);

        game.UI.DrawHCenteredText($"ups {game.Graphics.Metrics.UPS_SAMPLE}", new Vector2(game.WindowWidth / 2, 20), Vector4.One, 15);
        game.UI.DrawHCenteredText($"fps {game.Graphics.Metrics.FPS_SAMPLE}", new Vector2(game.WindowWidth / 2, 40), Vector4.One, 15);

    }

}
