using System;
using DriverCodeBase;
using DevExpress.Xpo;
using System.Text.RegularExpressions;

namespace FanucCNC
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class FanucCNCChannelSettings : ChannelSettings
    {
        #region Constructors

        public FanucCNCChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private FanucCNCChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }


        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();

            _DeviceAddress = string.Empty;            
            _DevicePort = FanucCNCProtocol.DEFAULT_PORT;
            _MachineSerie = FanucCNCProtocol.MachineSeries.Serie0iB;
        }

        public void CopyProperties(FanucCNCChannelSettings ch)
        {
            base.CopyProperties(ch);

            _DeviceAddress = ch.DeviceAddress;
            _DevicePort = ch.DevicePort;
            _MachineSerie = ch.MachineSerie;
        }
        /////////////////////////////

        #region Properties

        /// <summary>
        /// IP Address or name of Device
        /// </summary>
        private string _DeviceAddress;
        public string DeviceAddress
        {
            get { return _DeviceAddress; }
            set
            {
                SetPropertyValue("DeviceAddress", ref _DeviceAddress, value);
            }
        }
        
        /// <summary>
        /// Port Number
        /// </summary>
        private uint _DevicePort;
        public uint DevicePort
        {
            get { return _DevicePort; }
            set
            {
                SetPropertyValue("DevicePort", ref _DevicePort, value);
            }
        }

        /// <summary>
        /// Machine series (30i, ecc)
        /// </summary>
        private FanucCNCProtocol.MachineSeries _MachineSerie;
        public FanucCNCProtocol.MachineSeries MachineSerie
        {
            get { return _MachineSerie; }
            set 
            {
                SetPropertyValue("MachineSerie", ref _MachineSerie, value);
            }
        }
        #endregion

        #region IDataErrorInfo Members
        #endregion

        #region Overrides
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Performs the validation action. </summary>
        ///
        /// <param name="propertyName" type="String">   Name of the property. </param>
        ///
        /// <returns>   A String. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        const string ValidIpAddressRegex = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
        const string ValidHostnameRegex = @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-\_]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-\_]*[A-Za-z0-9])$";
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName)
            {
                case "DeviceAddress":
                    if (_DeviceAddress.Length == 0)
                        return Properties.Resources.ErrorEnterHostName;

                    if (!Regex.IsMatch(_DeviceAddress, ValidIpAddressRegex) && !Regex.IsMatch(_DeviceAddress, ValidHostnameRegex))
                        return Properties.Resources.ErrorInvalidHostName;
                    break;
                case "DevicePort":
                    if (_DevicePort <= 0)
                        return Properties.Resources.ErrorInvalidPortNumber;
                    break;
            }

            return null;
        }
        #endregion
    }
}
