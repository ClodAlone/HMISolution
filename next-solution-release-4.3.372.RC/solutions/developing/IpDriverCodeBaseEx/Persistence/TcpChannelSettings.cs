////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	persistence\tcpchannelsettings.cs
//
// summary:	Implements the tcpchannelsettings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Text.RegularExpressions;


namespace IpDriverCodeBaseEx
{
    /// <summary>   A TCP channel settings. </summary>
    public class TcpChannelSettings : ChannelSettings
    {
       #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected TcpChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected TcpChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
       
        
        #endregion


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Copies the properties described by ch. </summary>
        ///
        /// <param name="ch" type="TcpChannelSettings"> The ch. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void CopyProperties(ChannelSettings ch)
        {
            base.CopyProperties(ch);
            if(ch is TcpChannelSettings)
            {
                TcpChannelSettingsHostName = ((TcpChannelSettings)ch).TcpChannelSettingsHostName;
                TcpChannelSettingsHostPort = ((TcpChannelSettings)ch).TcpChannelSettingsHostPort;
                TcpChannelSettingsBackupHostName = ((TcpChannelSettings)ch).TcpChannelSettingsBackupHostName;
                TcpChannelSettingsSwitchHostTimeout = ((TcpChannelSettings)ch).TcpChannelSettingsSwitchHostTimeout;
            }
        }

        /// <summary>   Default settings. </summary>
        public void DefaultSettings()
        {
            base.DefaultSettings();
            TcpChannelSettingsHostName = "";
            TcpChannelSettingsHostPort = 0;
            TcpChannelSettingsBackupHostName = "";
            TcpChannelSettingsSwitchHostTimeout = 10000;
        }
        /////////////////////////////

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

        /// <summary>  Backup Host name. </summary>
        private string _TcpChannelSettingsBackupHostName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the TCP channel settings backup host. </summary>
        ///
        /// <value> The name of the TCP channel settings backup host. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string TcpChannelSettingsBackupHostName
        {
            get { return _TcpChannelSettingsBackupHostName; }
            set
            {
                /*if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Host name cannot be null");
                }*/

                SetPropertyValue("TcpChannelSettingsBackupHostName", ref _TcpChannelSettingsBackupHostName, value);
            }
        }

        /// <summary>   Switch Host timeout. </summary>
        private int _TcpChannelSettingsSwitchHostTimeout;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the TCP channel settings Switch Host timeout. </summary>
        ///
        /// <value> The TCP channel settings Switc hHost timeout. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int TcpChannelSettingsSwitchHostTimeout
        {
            get
            {
                return _TcpChannelSettingsSwitchHostTimeout;
            }
            set
            {
                SetPropertyValue("TcpChannelSettingsSwitchHostTimeout", ref _TcpChannelSettingsSwitchHostTimeout, value);
            }
        }

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
        const string ValidHostnameRegex = @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$";
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "TcpChannelSettingsHostName")
            {
                if (TcpChannelSettingsHostName != null && TcpChannelSettingsHostName.Length == 0)
                    return Properties.Resources.EnterHostName;

                if (TcpChannelSettingsHostName != null &&
                    !Regex.IsMatch(TcpChannelSettingsHostName, ValidIpAddressRegex) && 
                    !Regex.IsMatch(TcpChannelSettingsHostName, ValidHostnameRegex))
                    return Properties.Resources.InvalidHostName;
            }
            else if (propertyName == "TcpChannelSettingsBackupHostName")
            {

                if (TcpChannelSettingsBackupHostName != null && 
                    (TcpChannelSettingsBackupHostName.Length > 0) && 
                    (!Regex.IsMatch(TcpChannelSettingsBackupHostName, ValidIpAddressRegex) && 
                    !Regex.IsMatch(TcpChannelSettingsBackupHostName, ValidHostnameRegex)))
                    return Properties.Resources.InvalidHostName;
            }

            
                
            return null;
        }

        #endregion
    }
}
