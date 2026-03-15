using System;
using System.ComponentModel;

namespace UFInterfaces.CoreHostComponents
{
    public class ComponentHost
    {
        ServiceProvidingContainer _components = new ServiceProvidingContainer();

        public IContainer Components
        {
            get { return _components; }
        }

        public object GetService(Type service)
        {
            return _components.GetServicePublic(service);
        }

        public T GetService<T>() where T : class
        {
            return _components.GetServicePublic<T>() as T;
        }
    }
}
