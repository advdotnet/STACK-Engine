using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace STACK.Graphics
{

	public class DisplaySettings
	{
		public Viewport Viewport { get; private set; }
		public Point VirtualResolution { get; private set; }
		public Point? TargetResolution { get; private set; }
		public Matrix ScaleMatrix => _scaleMatrix;
		public float AspectRatio { get; private set; }

		private Matrix _scaleMatrix = Matrix.Identity;
		private float _windowScaleX, _windowScaleY;

		public DisplaySettings(Point virtualResolution, Point realResolution, Point? targetResolution)
		{
			VirtualResolution = virtualResolution;
			TargetResolution = targetResolution;
			AspectRatio = (float)VirtualResolution.X / VirtualResolution.Y;
			Viewport = new Viewport(0, 0, VirtualResolution.X, VirtualResolution.Y);
			OnClientSizeChanged(realResolution.X, realResolution.Y);
		}

		private void CacheValues(float width, float height)
		{
			_windowScaleX = (float)Viewport.Width / VirtualResolution.X;
			_windowScaleY = (float)Viewport.Height / VirtualResolution.Y;
			var viewportScaleX = (float)width / VirtualResolution.X;
			var viewportScaleY = (float)height / VirtualResolution.Y;

			Matrix.CreateScale(viewportScaleX, viewportScaleY, 1.0f, out _scaleMatrix);
		}

		public Vector2 TransformPoint(float x, float y)
		{
			x -= Viewport.X;
			y -= Viewport.Y;

			return new Vector2(x / _windowScaleX, y / _windowScaleY);
		}

		public void OnClientSizeChanged(int width, int height)
		{
			int viewPortWidth, viewPortHeight;

			if (TargetResolution.HasValue)
			{
				viewPortWidth = TargetResolution.Value.X;
				viewPortHeight = TargetResolution.Value.Y;
			}
			else
			{
				viewPortWidth = width;
				viewPortHeight = (int)(viewPortWidth / AspectRatio + 0.5f);

				if (viewPortHeight > height)
				{
					viewPortHeight = height;
					viewPortWidth = (int)(viewPortHeight * AspectRatio + 0.5f);
				}
			}

			var x = (width / 2) - (viewPortWidth / 2);
			var y = (height / 2) - (viewPortHeight / 2);

			Viewport = new Viewport(x, y, viewPortWidth, viewPortHeight);

			CacheValues(width, height);
		}
	}
}
