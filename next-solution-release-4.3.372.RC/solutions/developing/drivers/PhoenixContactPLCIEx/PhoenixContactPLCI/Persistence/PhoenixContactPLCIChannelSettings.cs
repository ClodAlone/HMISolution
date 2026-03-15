using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace PhoenixContactPLCI
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class PhoenixContactPLCIChannelSettings : ChannelSettings
    {
        #region Constructors

        public PhoenixContactPLCIChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private PhoenixContactPLCIChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }


        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();

            _ServerAddress = string.Empty;            
            _ServerPort = PhoenixContactPLCIProtocol.DEFAULT_PORT;
            _SubscribeVariables = false;
        }

        public override void CopyProperties(ChannelSettings ch)
        {
            base.CopyProperties(ch);

            _ServerAddress = ((PhoenixContactPLCIChannelSettings)ch).ServerAddress;
            _ServerPort = ((PhoenixContactPLCIChannelSettings)ch).ServerPort;
            _SubscribeVariables = ((PhoenixContactPLCIChannelSettings)ch).SubscribeVariables;
        }
        /////////////////////////////

        #region Properties

        /// <summary>
        /// IP Address or name of Device
        /// </summary>
        private string _ServerAddress;
        public string ServerAddress
        {
            get
            {
                return _ServerAddress;
            }
            set
            {
                SetPropertyValue("ServerAddress", ref _ServerAddress, value);
            }
        }
        
        /// <summary>
        /// Port Number
        /// </summary>
        private uint _ServerPort;
        public uint ServerPort
        {
            get { return _ServerPort; }
            set
            {
                SetPropertyValue("ServerPort", ref _ServerPort, value);
            }
        }
        
        /// <summary>
        /// Subscribe VariablesUse to PLC
        /// </summary>
        private bool _SubscribeVariables;
        public bool SubscribeVariables
        {
            get { return _SubscribeVariables; }
            set { SetPropertyValue("SubscribeVariables", ref _SubscribeVariables, value); }
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
                case "ServerAddress":
                    if (_ServerAddress.Length == 0)
                        return Properties.Resources.ErrorEnterHostName;

                    if (!Regex.IsMatch(_ServerAddress, ValidIpAddressRegex) && !Regex.IsMatch(_ServerAddress, ValidHostnameRegex))
                        return Properties.Resources.ErrorInvalidHostName;
                    break;
                case "ServerPort":
                    if (_ServerPort <= 0)
                        return Properties.Resources.ErrorInvalidPortNumber;
                    break;
            }

            return null;
        }
        #endregion
    }
}
