using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Simotion
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class SimotionChannelSettings : ChannelSettings
    {
        #region Constructors

        public SimotionChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private SimotionChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            PollingTimeNotInUse = 0;
            TcpChannelSettingsHostPort = 102;
            TcpChannelSettingsHostName = "";
        }

        public void CopyProperties(SimotionChannelSettings ch)
        {
            base.CopyProperties(ch);
            TcpChannelSettingsHostName = ch.TcpChannelSettingsHostName;
            TcpChannelSettingsHostPort = ch.TcpChannelSettingsHostPort;
        }

        #region Properties

        /// <summary>   Host name. </summary>
        private string _TcpChannelSettingsHostName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the TCP channel settings host. </summary>
        ///
        /// <value> The name of the TCP channel settings host. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string TcpChannelSettingsHostName
        {
            get { return _TcpChannelSettingsHostName; }
            set
            {
                /*if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Host name cannot be null");
                }*/

                SetPropertyValue("TcpChannelSettingsHostName", ref _TcpChannelSettingsHostName, value);
            }
        }

        /// <summary>   Host port. </summary>
        private int _TcpChannelSettingsHostPort;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the TCP channel settings host port. </summary>
        ///
        /// <value> The TCP channel settings host port. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int TcpChannelSettingsHostPort
        {
            get
            {
                return _TcpChannelSettingsHostPort;
            }
            set
            {
                SetPropertyValue("TcpChannelSettingsHostPort", ref _TcpChannelSettingsHostPort, value);
            }
        }
        #endregion


        #region IDataErrorInfo Memberss        

        const string ValidIpAddressRegex = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
        const string ValidHostnameRegex = @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$";
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;
            if (propertyName == "TcpChannelSettingsHostName")
            {
                if (TcpChannelSettingsHostName == string.Empty)
                    return Properties.Resources.EnterHostName;

                if (!Regex.IsMatch(TcpChannelSettingsHostName, ValidIpAddressRegex) && !Regex.IsMatch(TcpChannelSettingsHostName, ValidHostnameRegex))
                    return Properties.Resources.InvalidHostName;
            }

            return null;
        }

        #endregion
    }
}
