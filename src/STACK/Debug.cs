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

            if (input.IsKeyPress(Keys.F11) && false)
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

            if (input.IsKeyPress(Keys.F1))
            {
                SetSpeed(GameSpeed.Default);
            }

            if (input.IsKeyPress(Keys.F2))
            {
                SetSpeed(GameSpeed.Double);
            }

            if (input.IsKeyPress(Keys.F3))
            {
                SetSpeed(GameSpeed.Half);
            }
        }
    }
}
