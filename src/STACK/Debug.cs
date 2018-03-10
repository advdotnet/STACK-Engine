using Microsoft.Xna.Framework;
using STACK.Components;
using STACK.Input;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace STACK
{
    public partial class Window : Game
    {
        int BloomIndex = 0;

        public void HandleSkipInputEvent(InputEvent input)
        {
            if (input.Handled)
            {
                return;
            }

            if (input.Type == InputEventType.MouseUp && StackEngine.Game.World != null)
            {
                if (!StackEngine.Game.World.Interactive)
                {
                    SkipText.Start();
                    input.Handled = true;
                }
            }

            if (input.Type == InputEventType.KeyUp && input.IsKeyPress(Keys.Escape) &&
                StackEngine.Game.World != null && !StackEngine.Game.World.Interactive)
            {
                SkipCutscene.Start();
                input.Handled = true;
            }
        }

        public void HandleDebugInputEvent(InputEvent input)
        {
            if (input.Handled)
            {
                return;
            }

            if (input.IsKeyPress(Keys.Tab))
            {
                StackEngine.Console.Toggle();
                StackEngine.Pause(StackEngine.Console.Visible);
            }

            if (StackEngine.Console.Visible)
            {
                return;
            }

            // Switch to the next bloom settings preset?

            if (input.IsKeyPress(Keys.A) && false)
            {
                BloomIndex = (BloomIndex + 1) % BloomSettings.PresetSettings.Length;

                StackEngine.Game.World.Get<RenderSettings>().BloomSettings = BloomSettings.PresetSettings[BloomIndex];
                StackEngine.Game.World.Get<RenderSettings>().BloomEnabled = true;
            }

            // Toggle bloom on or off?
            if (input.IsKeyPress(Keys.F12))
            {
                StackEngine.Game.World.Get<RenderSettings>().BloomEnabled = !StackEngine.Game.World.Get<RenderSettings>().BloomEnabled;
            }

            // Cycle through the intermediate buffer debug display modes?
            if (input.IsKeyPress(Keys.D))
            {
                StackEngine.Renderer.BloomEffect.Visible = true;
                StackEngine.Renderer.BloomEffect.ShowBuffer++;

                if (StackEngine.Renderer.BloomEffect.ShowBuffer > BloomComponent.IntermediateBuffer.FinalResult)
                    StackEngine.Renderer.BloomEffect.ShowBuffer = 0;
            }

            if (input.IsKeyPress(Keys.F1)) SetSpeed(GameSpeed.Default);
            if (input.IsKeyPress(Keys.F2)) SetSpeed(GameSpeed.Double);
            if (input.IsKeyPress(Keys.F3)) SetSpeed(GameSpeed.Half);
            if (input.IsKeyPress(Keys.F4)) SetSpeed(GameSpeed.Infinity);

            //if (input.IsKeyPress(Keys.R))
            //{
            //    ToggleFullScreen();
            //}

            if (input.IsKeyPress(Keys.Add))
            {
                StackEngine.Game.World.Get<Camera>().Zoom += 0.01f;
            }

            if (input.IsKeyPress(Keys.Subtract))
            {
                StackEngine.Game.World.Get<Camera>().Zoom -= 0.01f;
            }

            if (input.IsKeyPress(Keys.Up))
            {
                StackEngine.Game.World.Get<Camera>().Position = new Vector2(StackEngine.Game.World.Get<Camera>().Position.X, StackEngine.Game.World.Get<Camera>().Position.Y - 10.0f);
            }

            if (input.IsKeyPress(Keys.Down))
            {
                StackEngine.Game.World.Get<Camera>().Position = new Vector2(StackEngine.Game.World.Get<Camera>().Position.X, StackEngine.Game.World.Get<Camera>().Position.Y + 10.0f);
            }

            if (input.IsKeyPress(Keys.Left))
            {
                StackEngine.Game.World.Get<Camera>().Position = new Vector2(StackEngine.Game.World.Get<Camera>().Position.X + 10.00f, StackEngine.Game.World.Get<Camera>().Position.Y);
            }

            if (input.IsKeyPress(Keys.Right))
            {
                StackEngine.Game.World.Get<Camera>().Position = new Vector2(StackEngine.Game.World.Get<Camera>().Position.X - 10.00f, StackEngine.Game.World.Get<Camera>().Position.Y);
            }

            if (input.IsKeyPress(Keys.Y))
            {
                StackEngine.Game.World.Get<Camera>().Rotation += 0.025f;
            }

            if (input.IsKeyPress(Keys.X))
            {
                StackEngine.Game.World.Get<Camera>().Rotation -= 0.025f;
            }

            if (input.IsKeyPress(Keys.C))
            {
                StackEngine.Game.World.Get<Camera>().Rotation = 0.00f;
            }
        }
    }
}
