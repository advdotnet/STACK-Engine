using Microsoft.Xna.Framework.Graphics;
using SDL2;
using System;

namespace STACK.Functional.Test
{

    public class GraphicsDeviceServiceMock : IGraphicsDeviceService, IDisposable
    {
        GraphicsDevice _GraphicsDevice;
        IntPtr _Handle;
        SDL2.SDL.SDL_WindowFlags _WindowFlags = (SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL2.SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN);

        public GraphicsDeviceServiceMock()
        {
            SDL.SDL_SetMainReady();
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
            _Handle = SDL.SDL_CreateWindow("", 0, 0, 1, 1, _WindowFlags);

            var Parameters = new PresentationParameters()
            {
                BackBufferWidth = 1,
                BackBufferHeight = 1,
                DeviceWindowHandle = _Handle,
                IsFullScreen = false
            };

            _GraphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef, Parameters);
        }

        public GraphicsDevice GraphicsDevice
        {
            get { return _GraphicsDevice; }
        }

        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;

        public void Dispose()
        {
            _GraphicsDevice.Dispose();
            _GraphicsDevice = null;

            SDL.SDL_DestroyWindow(_Handle);
            SDL.SDL_Quit();
        }
    }
}
