using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModelLib;
using Opc.Ua;

namespace OPCUAViewModel
{
    class ConfiguredEndpointViewModel : ViewModelBase
    {
        #region Members
        public ConfiguredEndpoint configuredEndpoint { get; private set; }
        #endregion

        #region Constructor
        public ConfiguredEndpointViewModel(ConfiguredEndpoint cep)
        {
            if (cep == null)
                throw new ArgumentNullException("ConfiguredEndpoint");

            configuredEndpoint = cep;
        }
        #endregion

        #region Properties

        public BinaryEncodingSupport BinaryEncodingSupport
        {
            get
            {
                return configuredEndpoint.BinaryEncodingSupport;
            }
            set
            {
                if (value == configuredEndpoint.BinaryEncodingSupport)
                    return;

                configuredEndpoint.BinaryEncodingSupport = value;
                OnPropertyChanged("BinaryEncodingSupport");
            }
        }

        public EndpointConfiguration Configuration
        {
            get
            {
                return configuredEndpoint.Configuration;
            }
            set
            {
                if (value == configuredEndpoint.Configuration)
                    return;

                configuredEndpoint.Configuration = value;
                OnPropertyChanged("Configuration");
            }
        }

        public EndpointDescription Description
        {
            get
            {
                return configuredEndpoint.Description;
            }
        }

        public Uri EndpointUrl
        {
            get
            {
                return configuredEndpoint.EndpointUrl;
            }
            set
            {
                if (value == configuredEndpoint.EndpointUrl)
                    return;

                configuredEndpoint.EndpointUrl = value;
                OnPropertyChanged("EndpointUrl");
            }
        }

        #endregion

#region Methods
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
