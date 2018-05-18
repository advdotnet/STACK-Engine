using Microsoft.Xna.Framework.Graphics;
using System;

namespace STACK.TestBase
{
    public class Mock
    {
        public static GraphicsDeviceServiceMock CreateGraphicsDevice()
        {
            return new GraphicsDeviceServiceMock();
        }

        public static IServiceProvider Wrap(IGraphicsDeviceService instance)
        {
            var Provider = new TestServiceProvider();
            Provider.AddService(typeof(IGraphicsDeviceService), instance);
            Provider.AddService(typeof(ISkipContent), null);
            return Provider;
        }

        public static TestInputProvider Input
        {
            get
            {
                return new TestInputProvider();
            }
        }
    }
}
