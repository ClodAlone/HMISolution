using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UFInterfaces.Service;

namespace UFProjectManager.Service
{
    internal class ServiceViewModel : INotifyPropertyChanged
    {
        #region Declarations
        readonly IServiceControl service;
        #endregion

        #region Constructors
        public ServiceViewModel(IServiceControl service)
        {
            this.service = service;
        }
        #endregion

        #region Public Properties
        public IServiceControl Service
        {
            get
            {
                return service;
            }
        }
        public String Name
        {
            get
            {
                return service.Name;
            }
        }

        public String FriendlyName
        {
            get
            {
                return service.FriendlyName;
            }
        }
 
        public ServiceInstaller.ServiceState Status
        {
            get
            {
                var status = ServiceInstaller.ServiceState.Unknown;
                try
                {
                    if (!String.IsNullOrEmpty(Name))
                        status = ServiceInstaller.ServiceInstaller.GetServiceStatus(Name);
                }
                catch
                { }

                if (status == ServiceInstaller.ServiceState.NotFound || 
                    status == ServiceInstaller.ServiceState.Unknown)
                    serviceConfig = null;

                return status;
            }
        }

        public String DisplayName
        {
            get
            {
                if (ServiceConfig != null)
                    return ServiceConfig.lpDisplayName;

                return String.Empty;
            }
        }

        public ServiceInstaller.ServiceBootFlag StartMode
        {
            get
            {
                if (ServiceConfig != null)
                    return ServiceConfig.dwStartType;

                return ServiceInstaller.ServiceBootFlag.Disabled;
            }
        }

        string userName;
        public String UserName
        {
            get
            {
                if (ServiceConfig != null)
                    return ServiceConfig.lpServiceStartName;

                return userName;
            }
            set
            {
                if (userName == value)
                    return;

                userName = value;
                OnPropertyChanged("UserName");
            }
        }

        internal String Password { get; set; }

        ServiceInstaller.ServiceInstaller.SERVICE_CONFIG serviceConfig;
        public ServiceInstaller.ServiceInstaller.SERVICE_CONFIG ServiceConfig
        {
            get
            {
                if (serviceConfig == null && Status != ServiceInstaller.ServiceState.Unknown)
                {
                    try
                    {
                        serviceConfig = ServiceInstaller.ServiceInstaller.GetServiceConfigInfo(Name);
                    }
                    catch { }
                }

                return serviceConfig;
            }
        }
        #endregion

        #region Public Methods
        public void ForceUpdateCurrentStatus()
        {
            OnPropertyChanged("Status");
            OnPropertyChanged("DisplayName");
            OnPropertyChanged("StartMode");
            OnPropertyChanged("UserName");
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion
    }
}
