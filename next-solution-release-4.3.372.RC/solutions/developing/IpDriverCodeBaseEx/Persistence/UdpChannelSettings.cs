////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\UdpChannelSettings.cs
//
// summary:	Implements the UDP channel settings class
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
    /// <summary>   An UDP channel settings. </summary>
    public class UdpChannelSettings : ChannelSettings, INotifyPropertyChanged
    {
       #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected UdpChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected UdpChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
       
        
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Copies the properties described by ch. </summary>
        ///
        /// <param name="ch" type="UdpChannelSettings"> The ch. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void CopyProperties(ChannelSettings ch)
        {
            //Giovanni 040313
            base.CopyProperties(ch);
            if(ch is UdpChannelSettings)
            {
                UdpChannelSettingsHostName = ((UdpChannelSettings)ch).UdpChannelSettingsHostName;
                UdpChannelSettingsHostPort = ((UdpChannelSettings)ch).UdpChannelSettingsHostPort;
                UdpChannelSettingsLocalHostName = ((UdpChannelSettings)ch).UdpChannelSettingsLocalHostName;
                UdpChannelSettingsLocalHostPort = ((UdpChannelSettings)ch).UdpChannelSettingsLocalHostPort;
            }
        }

        /// <summary>   Default settings. </summary>
        public void DefaultSettings()
        {
            base.DefaultSettings();
            UdpChannelSettingsHostName = "";
            UdpChannelSettingsHostPort = 0;
            UdpChannelSettingsLocalHostName = "";
            UdpChannelSettingsLocalHostPort = 0;
        }
        /////////////////////////////

        #region Properties

        /// <summary>   Host name. </summary>
        private string _UdpChannelSettingsHostName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the UDP channel settings host. </summary>
        ///
        /// <value> The name of the UDP channel settings host. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string UdpChannelSettingsHostName
        {
            get { return _UdpChannelSettingsHostName; }
            set
            {
                SetPropertyValue("UdpChannelSettingsHostName", ref _UdpChannelSettingsHostName, value);
                RaisePropertyChangedEvent("UdpChannelSettingsHostName");
            }
        }

        /// <summary>   Host port. </summary>
        private int _UdpChannelSettingsHostPort;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the UDP channel settings host port. </summary>
        ///
        /// <value> The UDP channel settings host port. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int UdpChannelSettingsHostPort
        {
            get
            {
                return _UdpChannelSettingsHostPort;
            }
            set
            {
                SetPropertyValue("UdpChannelSettingsHostPort", ref _UdpChannelSettingsHostPort, value);
                RaisePropertyChangedEvent("UdpChannelSettingsHostPort");
            }
        }

        /// <summary>   Local Host name. </summary>
        private string _UdpChannelSettingsLocalHostName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the UDP channel settings Local Host. </summary>
        ///
        /// <value> The name of the UDP channel settings Local Host. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual string UdpChannelSettingsLocalHostName
        {
            get { return _UdpChannelSettingsLocalHostName; }
            set
            {
                SetPropertyValue("UdpChannelSettingsLocalHostName", ref _UdpChannelSettingsLocalHostName, value);
                RaisePropertyChangedEvent("UdpChannelSettingsLocalHostName");
            }
        }

        /// <summary>   Local Host port. </summary>
        private int _UdpChannelSettingsLocalHostPort;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the UDP channel settings Local Host port. </summary>
        ///
        /// <value> The UDP channel settings Local Host port. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual int UdpChannelSettingsLocalHostPort
        {
            get
            {
                return _UdpChannelSettingsLocalHostPort;
            }
            set
            {
                SetPropertyValue("UdpChannelSettingsLocalHostPort", ref _UdpChannelSettingsLocalHostPort, value);
                RaisePropertyChangedEvent("UdpChannelSettingsLocalHostPort");
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
            if (propertyName == "UdpChannelSettingsHostName")
            {
                if (UdpChannelSettingsHostName.Length == 0)
                    return Properties.Resources.EnterHostName;

                if (!Regex.IsMatch(UdpChannelSettingsHostName, ValidIpAddressRegex) && !Regex.IsMatch(UdpChannelSettingsHostName, ValidHostnameRegex))
                    return Properties.Resources.InvalidHostName;
            }
            else if (propertyName == "UdpChannelSettingsLocalHostName")
            {
                if ((UdpChannelSettingsLocalHostName.Length > 0) && (!Regex.IsMatch(UdpChannelSettingsLocalHostName, ValidIpAddressRegex) && !Regex.IsMatch(UdpChannelSettingsLocalHostName, ValidHostnameRegex)))
                    return Properties.Resources.InvalidHostName;
            }
            return null;
        }

        #endregion
    }
}
