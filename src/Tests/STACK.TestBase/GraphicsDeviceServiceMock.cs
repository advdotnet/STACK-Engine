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
        GraphicsDevice _GraphicsDevice;
        IntPtr _Handle;
        SDL.SDL_WindowFlags _WindowFlags = (SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL |
            SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN);

        public GraphicsDeviceServiceMock()
        {
            SDL.SDL_SetMainReady();
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
            _Handle = SDL.SDL_CreateWindow(string.Empty, 0, 0, 1, 1, _WindowFlags);

            var Parameters = new PresentationParameters()
            {
                BackBufferWidth = 1,
                BackBufferHeight = 1,
                DeviceWindowHandle = _Handle,
                IsFullScreen = false
            };

            AudioManager.DisableSound();
            FrameworkDispatcher.Update();

            _GraphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef, Parameters);
        }

        public GraphicsDevice GraphicsDevice
        {
            get { return _GraphicsDevice; }
        }

        public event EventHandler<EventArgs> DeviceCreated { add { } remove { } }
        public event EventHandler<EventArgs> DeviceDisposing { add { } remove { } }
        public event EventHandler<EventArgs> DeviceReset { add { } remove { } }
        public event EventHandler<EventArgs> DeviceResetting { add { } remove { } }

        public void Dispose()
        {
            _GraphicsDevice.Dispose();
            _GraphicsDevice = null;

            SDL.SDL_DestroyWindow(_Handle);
            SDL.SDL_Quit();
        }
    }
}
