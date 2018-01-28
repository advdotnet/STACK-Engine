using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace STACK.Utils
{
    public class FrameRateCounter
    {
        int UpdateRate = 0;
        int UpdateCounter = 0;
        int FrameRate = 0;
        int FrameCounter = 0;
        TimeSpan ElapsedTime = TimeSpan.Zero;
        TimeSpan UpdateTime = TimeSpan.Zero;
        float UpdateAVG = 0;

        System.Diagnostics.Stopwatch Watch = new System.Diagnostics.Stopwatch();

        public FrameRateCounter()
        {
        }

        public void UpdateStart()
        {
            UpdateCounter++;
            ElapsedTime += TimeSpan.FromSeconds(GameSpeed.TickDuration);

            if (ElapsedTime > TimeSpan.FromSeconds(1))
            {
                ElapsedTime -= TimeSpan.FromSeconds(1);
                FrameRate = FrameCounter;
                FrameCounter = 0;

                UpdateAVG = (float)UpdateTime.Milliseconds / UpdateCounter;
                UpdateTime = TimeSpan.Zero;

                UpdateRate = UpdateCounter;
                UpdateCounter = 0;
            }
            Watch.Start();
        }

        public void UpdateEnd()
        {
            Watch.Stop();
            UpdateTime += Watch.Elapsed;
            Watch.Reset();
        }

        private string GetDigitText(int number)
        {
            switch (number)
            {
                case 0: return "0";
                case 1: return "1";
                case 2: return "2";
                case 3: return "3";
                case 4: return "4";
                case 5: return "5";
                case 6: return "6";
                case 7: return "7";
                case 8: return "8";
                case 9: return "9";
                default: return "-";
            }
        }

        private void DrawNumber(int number, int x, int y, SpriteBatch renderer, SpriteFont font)
        {
            const int WIDTH = 12;
            int Offset = 0;
            int TotalOffset = (int)Math.Floor(Math.Log10(number) + 1) * WIDTH;

            if (number == 0)
            {
                renderer.DrawString(font, "0", new Vector2(x + WIDTH, y), Color.White);
                return;
            }

            while (number > 0)
            {
                renderer.DrawString(font, GetDigitText(number % 10), new Vector2(x - (Offset * WIDTH) + TotalOffset, y), Color.White);
                number = number / 10;
                Offset++;
            }
        }

        public void Draw(SpriteBatch renderer, SpriteFont font, Vector2 mouse)
        {
            FrameCounter++;

            renderer.Begin();

            renderer.DrawString(font, "Updates:", new Vector2(10, 10 + 20), Color.White);
            DrawNumber(FrameRate, 200, 10 + 20, renderer, font);

            renderer.DrawString(font, "Update Time:", new Vector2(10, 10 + 40), Color.White);
            DrawNumber((int)(UpdateAVG * 1000000), 200, 10 + 40, renderer, font);

            renderer.DrawString(font, "GC0:", new Vector2(10, 10 + 60), Color.White);
            DrawNumber(GC.CollectionCount(0), 200, 10 + 60, renderer, font);

            renderer.DrawString(font, "GC1:", new Vector2(10, 10 + 80), Color.White);
            DrawNumber(GC.CollectionCount(1), 200, 10 + 80, renderer, font);

            renderer.DrawString(font, "GC2:", new Vector2(10, 10 + 100), Color.White);
            DrawNumber(GC.CollectionCount(2), 200, 10 + 100, renderer, font);

            renderer.DrawString(font, "Mouse:", new Vector2(10, 10 + 120), Color.White);
            DrawNumber((int)mouse.X, 200, 10 + 120, renderer, font);
            DrawNumber((int)mouse.Y, 250, 10 + 120, renderer, font);

            renderer.End();
        }
    }
}
