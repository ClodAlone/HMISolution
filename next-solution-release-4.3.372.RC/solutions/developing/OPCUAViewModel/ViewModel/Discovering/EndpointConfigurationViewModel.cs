using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModelLib;
using Opc.Ua;

namespace OPCUAViewModel
{
    class EndpointConfigurationViewModel : ViewModelBase
    {
        #region Members
        public EndpointConfiguration endpointConfiguration { get; private set;  }
        #endregion

        #region Constructor
        public EndpointConfigurationViewModel(EndpointConfiguration epc)
        {
            if (epc == null)
                throw new ArgumentNullException("EndpointConfiguration");

            endpointConfiguration = epc;
        }
        #endregion

        #region Properties

        public int ChannelLifetime
        {
            get
            {
                return endpointConfiguration.ChannelLifetime;
            }
            set
            {
                if (value == endpointConfiguration.ChannelLifetime)
                    return;

                endpointConfiguration.ChannelLifetime = value;
                OnPropertyChanged("ChannelLifetime");
            }
        }

        public int MaxArrayLength
        {
            get
            {
                return endpointConfiguration.MaxArrayLength;
            }
            set
            {
                if (value == endpointConfiguration.MaxArrayLength)
                    return;

                endpointConfiguration.MaxArrayLength = value;
                OnPropertyChanged("MaxArrayLength");
            }
        }

        public int MaxBufferSize
        {
            get
            {
                return endpointConfiguration.MaxBufferSize;
            }
            set
            {
                if (value == endpointConfiguration.MaxBufferSize)
                    return;

                endpointConfiguration.MaxBufferSize = value;
                OnPropertyChanged("MaxBufferSize");
            }
        }

        public int MaxByteStringLength
        {
            get
            {
                return endpointConfiguration.MaxByteStringLength;
            }
            set
            {
                if (value == endpointConfiguration.MaxByteStringLength)
                    return;

                endpointConfiguration.MaxByteStringLength = value;
                OnPropertyChanged("MaxByteStringLength");
            }
        }

        public int MaxMessageSize
        {
            get
            {
                return endpointConfiguration.MaxMessageSize;
            }
            set
            {
                if (value == endpointConfiguration.MaxMessageSize)
                    return;

                endpointConfiguration.MaxMessageSize = value;
                OnPropertyChanged("MaxMessageSize");
            }
        }

        public int MaxStringLength
        {
            get
            {
                return endpointConfiguration.MaxStringLength;
            }
            set
            {
                if (value == endpointConfiguration.MaxStringLength)
                    return;

                endpointConfiguration.MaxStringLength = value;
                OnPropertyChanged("MaxStringLength");
            }
        }

        public int OperationTimeout
        {
            get
            {
                return endpointConfiguration.OperationTimeout;
            }
            set
            {
                if (value == endpointConfiguration.OperationTimeout)
                    return;

                endpointConfiguration.OperationTimeout = value;
                OnPropertyChanged("OperationTimeout");
            }
        }

        public int SecurityTokenLifetime
        {
            get
            {
                return endpointConfiguration.SecurityTokenLifetime;
            }
            set
            {
                if (value == endpointConfiguration.SecurityTokenLifetime)
                    return;

                endpointConfiguration.SecurityTokenLifetime = value;
                OnPropertyChanged("SecurityTokenLifetime");
            }
        }

        public bool UseBinaryEncoding
        {
            get
            {
                return endpointConfiguration.UseBinaryEncoding;
            }
            set
            {
                if (value == endpointConfiguration.UseBinaryEncoding)
                    return;

                endpointConfiguration.UseBinaryEncoding = value;
                OnPropertyChanged("UseBinaryEncoding");
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
