using Microsoft.Xna.Framework.Graphics;
using System;

namespace STACK.TestBase
{
    public class Mock
    {
		public static GraphicsDeviceServiceMock CreateGraphicsDevice() => new GraphicsDeviceServiceMock();

		public static IServiceProvider Wrap(IGraphicsDeviceService instance)
        {
            var provider = new TestServiceProvider();
            provider.AddService(typeof(IGraphicsDeviceService), instance);
            provider.AddService(typeof(ISkipContent), null);
            return provider;
        }

		public static TestInputProvider Input => new TestInputProvider();
	}
}
