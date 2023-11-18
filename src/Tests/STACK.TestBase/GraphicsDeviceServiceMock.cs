using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDL2;
using STACK.Components;
using System;

namespace STACK.TestBase
{
	/// <summary>
	/// IGraphicsDeviceService mock
	/// </summary>
	public class GraphicsDeviceServiceMock : IGraphicsDeviceService, IDisposable
	{
		private GraphicsDevice _graphicsDevice;
		private readonly IntPtr _handle;
		private readonly SDL.SDL_WindowFlags _windowFlags = (SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL |
			SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN);

		public GraphicsDeviceServiceMock()
		{
			SDL.SDL_SetMainReady();
			SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
			_handle = SDL.SDL_CreateWindow(string.Empty, 0, 0, 1, 1, _windowFlags);

			var parameters = new PresentationParameters()
			{
				BackBufferWidth = 1,
				BackBufferHeight = 1,
				DeviceWindowHandle = _handle,
				IsFullScreen = false
			};

			AudioManager.DisableSound();
			FrameworkDispatcher.Update();

			_graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef, parameters);
		}

		public GraphicsDevice GraphicsDevice => _graphicsDevice;

		public event EventHandler<EventArgs> DeviceCreated { add { } remove { } }
		public event EventHandler<EventArgs> DeviceDisposing { add { } remove { } }
		public event EventHandler<EventArgs> DeviceReset { add { } remove { } }
		public event EventHandler<EventArgs> DeviceResetting { add { } remove { } }

		public void Dispose()
		{
			_graphicsDevice.Dispose();
			_graphicsDevice = null;

			SDL.SDL_DestroyWindow(_handle);
			SDL.SDL_Quit();
		}
	}
}
