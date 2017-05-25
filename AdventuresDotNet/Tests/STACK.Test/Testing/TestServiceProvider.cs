using Microsoft.VisualStudio.TestTools.UnitTesting;
using STACK.Test;
using System;
using System.Collections.Generic;

namespace STACK.Test
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
            object service;
            _services.TryGetValue(serviceType, out service);
            return service;
        }
    }
           
}
