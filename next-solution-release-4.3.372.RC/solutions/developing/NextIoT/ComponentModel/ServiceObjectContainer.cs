//------------------------------------------------------------------------------
// <copyright file="ServiceObjectContainer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

/*
 */
namespace System.ComponentModel.Design {
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <devdoc>
    ///     This is a simple implementation of IServiceContainer.
    /// </devdoc>
    public class ServiceContainer : IServiceContainer, IDisposable
    {
        private ServiceCollection<object> services;
        private IServiceProvider parentProvider;
        private static Type[] _defaultServices = new Type[] { typeof(IServiceContainer), typeof(ServiceContainer) };
        
        /// <devdoc>
        ///     Creates a new service object container.  
        /// </devdoc>
        public ServiceContainer() {
        }
        
        /// <devdoc>
        ///     Creates a new service object container.  
        /// </devdoc>
        public ServiceContainer(IServiceProvider parentProvider) {
            this.parentProvider = parentProvider;
        }
        
        /// <devdoc>
        ///     Retrieves the parent service container, or null
        ///     if there is no parent container.
        /// </devdoc>
        private IServiceContainer Container { 
            get {
                IServiceContainer container = null;
                if (parentProvider != null) {
                    container = (IServiceContainer)parentProvider.GetService(typeof(IServiceContainer));
                }
                return container;
            }
        }

        /// <devdoc>
        ///     This property returns the default services that are implemented directly on this IServiceContainer.
        ///     the default implementation of this property is to return the IServiceContainer and ServiceContainer
        ///     types.  You may override this proeprty and return your own types, modifying the default behavior
        ///     of GetService.
        /// </devdoc>
        protected virtual Type[] DefaultServices {
            get {
                return _defaultServices;
            }
        }
        
        /// <devdoc>
        ///     Our collection of services.  The service collection is demand
        ///     created here.
        /// </devdoc>
        private ServiceCollection<object> Services {
            get {
                if (services == null) {
                    services = new ServiceCollection<object>();
                }
                return services;
            }
        }
        
        /// <devdoc>
        ///     Adds the given service to the service container.
        /// </devdoc>
        public void AddService(Type serviceType, object serviceInstance) {
            AddService(serviceType, serviceInstance, false);
        }

        /// <devdoc>
        ///     Adds the given service to the service container.
        /// </devdoc>
        public virtual void AddService(Type serviceType, object serviceInstance, bool promote) {
            if (promote) {
                IServiceContainer container = Container;
                if (container != null) {
                    container.AddService(serviceType, serviceInstance, promote);
                    return;
                }
            }
            
            // We're going to add this locally.  Ensure that the service instance
            // is correct.
            //
            if (serviceType == null) throw new ArgumentNullException("serviceType");
            if (serviceInstance == null) throw new ArgumentNullException("serviceInstance");
            
            if (Services.ContainsKey(serviceType)) {
                throw new ArgumentException("ErrorServiceExists");
            }
            
            Services[serviceType] = serviceInstance;
        }

        /// <devdoc>
        ///     Adds the given service to the service container.
        /// </devdoc>
        public void AddService(Type serviceType, ServiceCreatorCallback callback) {
            AddService(serviceType, callback, false);
        }

        /// <devdoc>
        ///     Adds the given service to the service container.
        /// </devdoc>
        public virtual void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
            if (promote) {
                IServiceContainer container = Container;
                if (container != null) {
                    container.AddService(serviceType, callback, promote);
                    return;
                }
            }
            
            // We're going to add this locally.  Ensure that the service instance
            // is correct.
            //
            if (serviceType == null) throw new ArgumentNullException("serviceType");
            if (callback == null) throw new ArgumentNullException("callback");
            
            if (Services.ContainsKey(serviceType)) {
                throw new ArgumentException("ErrorServiceExists");
            }
            
            Services[serviceType] = callback;
        }

        /// <devdoc>
        ///     Disposes this service container.  This also walks all instantiated services within the container
        ///     and disposes any that implement IDisposable, and clears the service list.
        /// </devdoc>
        public void Dispose() {
            Dispose(true);
        }

        /// <devdoc>
        ///     Disposes this service container.  This also walks all instantiated services within the container
        ///     and disposes any that implement IDisposable, and clears the service list.
        /// </devdoc>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                ServiceCollection<object> serviceCollection = services;
                services = null;
                if (serviceCollection != null) {
                    foreach(object o in serviceCollection.Values) {
                        if (o is IDisposable) {
                            ((IDisposable)o).Dispose();
                        }
                    }
                }
            }
        }

        /// <devdoc>
        ///     Retrieves the requested service.
        /// </devdoc>
        public virtual object GetService(Type serviceType) {
            object service = null;
            
            // Try locally.  We first test for services we
            // implement and then look in our service collection.
            //
            Type[] defaults = DefaultServices;
            for (int idx = 0; idx < defaults.Length; idx++) {
                if (serviceType == defaults[idx]) {
                    service = this;
                    break;
                }
            }

            if (service == null) {
                Services.TryGetValue(serviceType, out service);
            }
            
            // Is the service a creator delegate?
            //
            if (service is ServiceCreatorCallback) {
                service = ((ServiceCreatorCallback)service)(this, serviceType);
                
                // And replace the callback with our new service.
                //
                Services[serviceType] = service;
            }
            
            if (service == null && parentProvider != null) {
                service = parentProvider.GetService(serviceType);
            }
                        
            return service;
        }
        
        /// <devdoc>
        ///     Removes the given service type from the service container.
        /// </devdoc>
        public void RemoveService(Type serviceType) {
            RemoveService(serviceType, false);
        }

        /// <devdoc>
        ///     Removes the given service type from the service container.
        /// </devdoc>
        public virtual void RemoveService(Type serviceType, bool promote) {
            if (promote) {
                IServiceContainer container = Container;
                if (container != null) {
                    container.RemoveService(serviceType, promote);
                    return;
                }
            }
            
            // We're going to remove this from our local list.
            //
            if (serviceType == null) throw new ArgumentNullException("serviceType");
            Services.Remove(serviceType);
        }

        /// <summary>
        /// Use this collection to store mapping from the Type of a service to the object that provides it in a way
        /// that is aware of embedded types.   The comparer for this collection will call Type.IsEquivalentTo(...)
        /// instead of doing a reference comparison which will fail in type embedding scenarios.  To speed the lookup
        /// performance we will use hash code of Type.FullName.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private sealed class ServiceCollection<T> : Dictionary<Type, T> {
            static EmbeddedTypeAwareTypeComparer serviceTypeComparer = new EmbeddedTypeAwareTypeComparer();

            private sealed class EmbeddedTypeAwareTypeComparer : IEqualityComparer<Type> {
                #region IEqualityComparer<Type> Members

                public bool Equals(Type x, Type y) {
                    return x == y;
                }

                public int GetHashCode(Type obj) {
                    return obj.FullName.GetHashCode();
                }

                #endregion
            }

            public ServiceCollection() : base(serviceTypeComparer) {
            }
        }
    }

}

