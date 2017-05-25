﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace STACK.Functional.Test
{
    [TestClass]
    public class Mock
    {
        public class TestServiceProvider : IServiceProvider
        {
            private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

            public void AddService(Type type, object service)
            {
                _services.Add(type, service);
            }

            public bool RemoveService(Type type)
            {
                return _services.Remove(type);
            }

            public object GetService(Type serviceType)
            {
                return _services[serviceType];
            }
        }

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

        [TestMethod]
        public void GraphicsDeviceMockAvaiable()
        {
            using (var GraphicsDevice = CreateGraphicsDevice())
            {
                Assert.IsNotNull(GraphicsDevice.GraphicsDevice);
            }
        }
    }
}
