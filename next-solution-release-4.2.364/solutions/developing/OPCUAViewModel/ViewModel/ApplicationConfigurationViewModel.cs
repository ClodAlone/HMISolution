using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModelLib;
using Opc.Ua;
using Opc.Ua.Client;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Threading;
#endif
namespace OPCUAViewModel
{
    class ApplicationConfigurationViewModel : ViewModelBase
    {
#region Members
        public ApplicationConfiguration applicationConfiguration { get; protected set; }
#endregion

#region Constructor
        public ApplicationConfigurationViewModel(ApplicationConfiguration a)
        {
            if (a == null)
                throw new ArgumentNullException("ApplicationConfiguration");

            applicationConfiguration = a;
        }
#endregion

#region Methods
#endregion

#region Properties
        public String ApplicationName
        {
            get
            {
                return applicationConfiguration.ApplicationName;
            }
            set
            {
                if (value == applicationConfiguration.ApplicationName)
                    return;

                applicationConfiguration.ApplicationName = value;
                OnPropertyChanged("ApplicationName");
            }
        }

        public ApplicationType ApplicationType
        {
            get
            {
                return applicationConfiguration.ApplicationType;
            }
            set
            {
                if (value == applicationConfiguration.ApplicationType)
                    return;

                applicationConfiguration.ApplicationType = value;
                OnPropertyChanged("ApplicationType");
            }
        }

        public String ApplicationUri
        {
            get
            {
                return applicationConfiguration.ApplicationUri;
            }
            set
            {
                if (value == applicationConfiguration.ApplicationUri)
                    return;

                applicationConfiguration.ApplicationUri = value;
                OnPropertyChanged("ApplicationUri");
            }
        }

        public CertificateValidator CertificateValidator
        {
            get
            {
                return applicationConfiguration.CertificateValidator;
            }
            set
            {
                if (value == applicationConfiguration.CertificateValidator)
                    return;

                applicationConfiguration.CertificateValidator = value;
                OnPropertyChanged("CertificateValidator");
            }
        }

        public ClientConfiguration ClientConfiguration
        {
            get
            {
                return applicationConfiguration.ClientConfiguration;
            }
            set
            {
                if (value == applicationConfiguration.ClientConfiguration)
                    return;

                applicationConfiguration.ClientConfiguration = value;
                OnPropertyChanged("ClientConfiguration");
            }
        }

        public bool DisableHiResClock
        {
            get
            {
                return applicationConfiguration.DisableHiResClock;
            }
            set
            {
                if (value == applicationConfiguration.DisableHiResClock)
                    return;

                applicationConfiguration.DisableHiResClock = value;
                OnPropertyChanged("DisableHiResClock");
            }
        }

        public DiscoveryServerConfiguration DiscoveryServerConfiguration
        {
            get
            {
                return applicationConfiguration.DiscoveryServerConfiguration;
            }
            set
            {
                if (value == applicationConfiguration.DiscoveryServerConfiguration)
                    return;

                applicationConfiguration.DiscoveryServerConfiguration = value;
                OnPropertyChanged("DiscoveryServerConfiguration");
            }
        }

        public ServiceMessageContext MessageContext
        {
            get
            {
                return applicationConfiguration.CreateMessageContext();
            }
        }

        public String ProductUri
        {
            get
            {
                return applicationConfiguration.ProductUri;
            }
            set
            {
                if (value == applicationConfiguration.ProductUri)
                    return;

                applicationConfiguration.ProductUri = value;
                OnPropertyChanged("ProductUri");
            }
        }

        public SecurityConfiguration SecurityConfiguration
        {
            get
            {
                return applicationConfiguration.SecurityConfiguration;
            }
            set
            {
                if (value == applicationConfiguration.SecurityConfiguration)
                    return;

                applicationConfiguration.SecurityConfiguration = value;
                OnPropertyChanged("SecurityConfiguration");
            }
        }

        public ServerConfiguration ServerConfiguration
        {
            get
            {
                return applicationConfiguration.ServerConfiguration;
            }
            set
            {
                if (value == applicationConfiguration.ServerConfiguration)
                    return;

                applicationConfiguration.ServerConfiguration = value;
                OnPropertyChanged("ServerConfiguration");
            }
        }

        public String SourceFilePath
        {
            get
            {
                return applicationConfiguration.SourceFilePath;
            }
        }

        public TraceConfiguration TraceConfiguration
        {
            get
            {
                return applicationConfiguration.TraceConfiguration;
            }
            set
            {
                if (value == applicationConfiguration.TraceConfiguration)
                    return;

                applicationConfiguration.TraceConfiguration = value;
                OnPropertyChanged("TraceConfiguration");
            }
        }

        public TransportConfigurationCollection TransportConfigurations
        {
            get
            {
                return applicationConfiguration.TransportConfigurations;
            }
            set
            {
                if (value == applicationConfiguration.TransportConfigurations)
                    return;

                applicationConfiguration.TransportConfigurations = value;
                OnPropertyChanged("TransportConfigurations");
            }
        }

        public TransportQuotas TransportQuotas
        {
            get
            {
                return applicationConfiguration.TransportQuotas;
            }
            set
            {
                if (value == applicationConfiguration.TransportQuotas)
                    return;

                applicationConfiguration.TransportQuotas = value;
                OnPropertyChanged("TransportQuotas");
            }
        }
#endregion

#if !WINDOWS_UWP && !NET_STANDARD
#region Validations
        public override string Error
        {
            get
            {
                return null;
            }
        }

        public override string this[string propertyName]
        {
            get
            {
                return PerformValidation(propertyName);
            }
        }
#endregion
#endif
    }
}
