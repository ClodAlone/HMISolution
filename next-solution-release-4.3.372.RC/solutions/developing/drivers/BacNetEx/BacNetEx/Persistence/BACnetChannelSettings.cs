////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\BACnetChannelSettings.cs
//
// summary:	Implements the driver BACnet channel settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using IpDriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;
using DriverCodeBaseEx;

namespace BACnet
{
    /// <summary>   Settings for the drivers's channel(BACnetChannel). </summary>
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class BACnetChannelSettings : UdpChannelSettings
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BACnetChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private BACnetChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        /// <summary>   Set propertys to default value. </summary>
        public void DefaultSettings()
        {
            base.DefaultSettings();
            UdpChannelSettingsLocalHostPort = 47808;
            _DeviceInstance = string.Empty;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set propertys from BACnetChannelSettings "ch". </summary>
        ///
        /// <param name="ch">   . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void CopyProperties(ChannelSettings ch)
        {
            base.CopyProperties(ch);
            UdpChannelSettingsLocalHostPort = ((BACnetChannelSettings)ch).UdpChannelSettingsLocalHostPort;
            _DeviceInstance = ((BACnetChannelSettings)ch).DeviceInstance;
        }

        public int getDeviceInstance()
        {
            return BACnetProtocol.ParseDeviceInstanceString(_DeviceInstance);
        }

        #region Properties
        private string _DeviceInstance;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>    DeviceInstance </summary>
        ///
        /// <value> This option enable driver to answer BACnet message WhoIs (with iAm) and send WhoIsiAmAServer
        ///         on StartUp </value> 
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string DeviceInstance
        {
            get
            {
                return _DeviceInstance;
            }
            set
            {
                SetPropertyValue("DeviceInstance", ref _DeviceInstance, value);
            }
        }
        #endregion

        #region IDataErrorInfo Members

        private bool AnyChannelsHaveSameNameAndPort(string localHostName, int localUdpPort)
        {
            IEnumerable<BACnetChannelSettings> Channels = (DriverSettings as BACnetDriverSettings).ChannelSettings.ToList().Cast<BACnetChannelSettings>();

            return Channels.Count(ch => ch.UdpChannelSettingsLocalHostName == localHostName && ch.UdpChannelSettingsLocalHostPort == localUdpPort) > 1;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   BACnetChannelSettings property validation. </summary>
        ///
        /// <param name="propertyName"> . </param>
        ///
        /// <returns>   A String. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override String PerformValidation(String propertyName)
        {
            if (propertyName == "UdpChannelSettingsHostName")
            {
                return (null);
            }

            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
            {
                return sBase;
            }

            switch (propertyName)
            {
                case "DeviceInstance":
                    if (!string.IsNullOrEmpty(_DeviceInstance))
                    {
                        UInt32 numVal;
                        try
                        {
                            numVal = Convert.ToUInt32(_DeviceInstance);
                        }
                        catch (FormatException e)
                        {
                            return Properties.Resources.InvalidNumber;
                        }
                        catch (OverflowException e)
                        {
                            return Properties.Resources.DeviceInstanceOutOfRange;
                        }
                        if (numVal > BACnetEnums.MAX_INSTANCE)
                            return Properties.Resources.DeviceInstanceOutOfRange;
                        
                        // check if device instance is already used
                        if (DriverSettings != null && ((from s in DriverSettings.StationSettings/*.AsParallel()*/
                                                        where (s as BACnetStationSettings).getDeviceInstance() == getDeviceInstance()
                                                            || ((s as BACnetStationSettings).Channel != this.Name || (s as BACnetStationSettings).getDeviceInstance() == getDeviceInstance())
                                                        select s).ToList().Count > 0))
                        {
                            return Properties.Resources.SelectExistingDeviceName;
                        }

                        // to send broadcast iAm message host name/ip address is required to identify target network card
                        if (string.IsNullOrEmpty(base.UdpChannelSettingsLocalHostName))
                        {
                            return (Properties.Resources.DeviceHostNameNotNullWithDeviceInstance);
                        }
                    }
                    break;

                case "UdpChannelSettingsLocalHostName":
                    string ValidationError = PerformValidation("DeviceInstance");
                    if (!String.IsNullOrWhiteSpace(ValidationError))
                        return ValidationError;

                    // check if other channel have same local host name and port
                    if (DriverSettings != null && !((BACnetDriverSettings)DriverSettings).AllowOtherBACnetClients.Value)
                    {
                        if (AnyChannelsHaveSameNameAndPort(UdpChannelSettingsLocalHostName, UdpChannelSettingsLocalHostPort))
                            return string.Format(Properties.Resources.AOBCLocalHostNameAndPortAlreadyUsed, Properties.Resources.CaptionAllowOtherBACnetClients);
                    }
                    break;

                case "UdpChannelSettingsLocalHostPort":
                    // check if other channel have same local host name and port
                    if (DriverSettings != null && !((BACnetDriverSettings)DriverSettings).AllowOtherBACnetClients.Value)
                    {
                        if (AnyChannelsHaveSameNameAndPort(UdpChannelSettingsLocalHostName,UdpChannelSettingsLocalHostPort))
                            return string.Format(Properties.Resources.AOBCLocalHostNameAndPortAlreadyUsed, Properties.Resources.CaptionAllowOtherBACnetClients);
                    }
                    break;
            }

            return null;
        }

        #endregion

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            switch (propertyName)
            {
                case "UdpChannelSettingsLocalHostName":
                    RaisePropertyChangedEvent("UdpChannelSettingsLocalHostPort");
                    RaisePropertyChangedEvent("DeviceInstance");
                    break;

                case "UdpChannelSettingsLocalHostPort":
                    RaisePropertyChangedEvent("UdpChannelSettingsLocalHostName");
                    break;

                case "DeviceInstance":
                    RaisePropertyChangedEvent("UdpChannelSettingsLocalHostName");
                    break;

            }
        }
    }
}
