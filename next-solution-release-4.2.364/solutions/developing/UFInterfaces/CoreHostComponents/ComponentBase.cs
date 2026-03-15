using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace UFInterfaces.CoreHostComponents
{
    public class ComponentBase<T> : Component where T : IUFInterfaceBase
    {
        public override ISite Site
        {
            get
            {
                return base.Site;
            }
            set
            {
                base.Site = value;

                // publish this instance as a service
                IServiceContainer serviceContainer = (IServiceContainer)GetService(typeof(IServiceContainer));
                if (serviceContainer != null)
                {
                    serviceContainer.AddService(typeof(T), this);
                }
            }
        }
    }
}
