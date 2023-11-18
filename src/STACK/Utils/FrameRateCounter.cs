using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace STACK.Utils
{
	public class FrameRateCounter
	{
		private int _updateCounter = 0;
		private int _frameRate = 0;
		private int _frameCounter = 0;
		private TimeSpan _elapsedTime = TimeSpan.Zero;
		private TimeSpan _updateTime = TimeSpan.Zero;
		private float _updateAVG = 0;
		private readonly System.Diagnostics.Stopwatch _watch = new System.Diagnostics.Stopwatch();

		public FrameRateCounter()
		{
		}

		public void UpdateStart()
		{
			_updateCounter++;
			_elapsedTime += TimeSpan.FromSeconds(GameSpeed.TickDuration);

			if (_elapsedTime > TimeSpan.FromSeconds(1))
			{
				_elapsedTime -= TimeSpan.FromSeconds(1);
				_frameRate = _frameCounter;
				_frameCounter = 0;

				_updateAVG = (float)_updateTime.Milliseconds / _updateCounter;
				_updateTime = TimeSpan.Zero;

				_updateCounter = 0;
			}
			_watch.Start();
		}

		public void UpdateEnd()
		{
			_watch.Stop();
			_updateTime += _watch.Elapsed;
			_watch.Reset();
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
			const int width = 12;
			var offset = 0;
			var totalOffset = (int)Math.Floor(Math.Log10(number) + 1) * width;

			if (number == 0)
			{
				renderer.DrawString(font, "0", new Vector2(x + width, y), Color.White);
				return;
			}

			while (number > 0)
			{
				renderer.DrawString(font, GetDigitText(number % 10), new Vector2(x - (offset * width) + totalOffset, y), Color.White);
				number /= 10;
				offset++;
			}
		}

		public void Draw(SpriteBatch renderer, SpriteFont font, Vector2 mouse)
		{
			_frameCounter++;

			renderer.Begin();

			renderer.DrawString(font, "Updates:", new Vector2(10, 10 + 20), Color.White);
			DrawNumber(_frameRate, 200, 10 + 20, renderer, font);

			renderer.DrawString(font, "Update Time:", new Vector2(10, 10 + 40), Color.White);
			DrawNumber((int)(_updateAVG * 1000000), 200, 10 + 40, renderer, font);

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
