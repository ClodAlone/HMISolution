using System;
using System.ComponentModel;
using System.ComponentModel.Design;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Configuration;
using Utilities;
using System.ServiceModel.Discovery;
#endif
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Diagnostics;

namespace UFInterfaces.CoreHostComponents
{
    public class ServiceProvidingContainer : Container, IDisposable
    {
        public IServiceContainer ServiceContainer { get; private set; }
#if !WINDOWS_UWP && !NET_STANDARD
        DiscoveryClient discoveryClient;
#endif
        public ServiceProvidingContainer()
        {
            ServiceContainer = new ServiceContainer();
            ServiceContainer.AddService(typeof(IServiceContainer), ServiceContainer);
        }

        protected override object GetService(Type service)
        {
            Object ret = ServiceContainer.GetService(service);
            if (ret != null)
                return ret;

#if !WINDOWS_UWP && !NET_STANDARD
            if (LoadOnDemand(service))
                return ServiceContainer.GetService(service);
#endif
            return null;
        }

        public object GetServicePublic(Type service)
        {
            object ret = GetService(service);
            if (ret != null)
                return ret;

#if !WINDOWS_UWP && !NET_STANDARD
            if (LoadOnDemand(service))
                return GetService(service);
#endif
            return null;
        }

        public T GetServicePublic<T>() where T : class
        {
            T ret = GetService(typeof(T)) as T;
            if (ret != null)
                return ret;

#if !WINDOWS_UWP && !NET_STANDARD
            if (LoadOnDemand(typeof(T)))
                return GetService(typeof(T)) as T;

            /*
            try
            {
                if (discoveryClient == null)
                    discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
                FindResponse discoveryResponse = discoveryClient.Find(new FindCriteria(typeof(T)));
                if (discoveryResponse.Endpoints.Count > 0)
                {
                    EndpointAddress address = discoveryResponse.Endpoints[0].Address;

                    var Service = ChannelFactory<T>.CreateChannel(DiscoveryHelper.DiscoverBinding(typeof(T), address.Uri), address);
                    return Service;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
            */
#endif
            return null;
        }

#if !WINDOWS_UWP && !NET_STANDARD
        bool LoadOnDemand(Type service)
        {
            String scheme = service.Name;
            scheme = scheme.Substring(1); // remove the I
            scheme = scheme.Replace("Service", ""); // remove the Service

            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings[scheme]))
                scheme = ConfigurationManager.AppSettings[scheme];

            String baseFolderDesigner = String.Format("{0}LOD\\", AppDomain.CurrentDomain.BaseDirectory);
            List<IComponent> list = null;
            if (System.IO.Directory.Exists(baseFolderDesigner))
                list = FindAndLoadDLL.LoadDLLs<IComponent>(baseFolderDesigner, String.Format("{0}*.dll", scheme));
            if (list == null || list.Count <= 0)
            {
                baseFolderDesigner = String.Format("{0}", AppDomain.CurrentDomain.BaseDirectory);
                list = FindAndLoadDLL.LoadDLLs<IComponent>(baseFolderDesigner, String.Format("{0}*.dll", scheme));
                if (list.Count <= 0)
                    return false;
            }

            // list.ForEach(component =>
            list.ForEach(component =>
                {
                    Add(component);
                    if (component is IUFInterfaceBase)
                        (component as IUFInterfaceBase).Initialize();
                });

            return true;
        }
#endif

#region IDisposable Members

        void IDisposable.Dispose()
        {
#if !WINDOWS_UWP && !NET_STANDARD
            if (discoveryClient != null)
            {
                discoveryClient = null;
            }
#endif
        }

#endregion
    }
}
