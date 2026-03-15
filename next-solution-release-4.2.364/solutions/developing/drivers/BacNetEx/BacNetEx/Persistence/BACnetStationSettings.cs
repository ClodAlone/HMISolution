////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\BACnetStationSettings.cs
//
// summary:	Implements the driver BACnet station settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace BACnet
{
    /// <summary>   Settings for the drivers's station(BACnetStation). </summary>
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class BACnetStationSettings : StationSettings
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BACnetStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected BACnetStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set propertys from BACnetStationSettings "st". </summary>
        ///
        /// <param name="st">   . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void CopyProperties(BACnetStationSettings st)
        {
            _ForceInitialPolling = st.ForceInitialPolling;
            _COVInterval = st.COVInterval;
            _TimeSync = st.TimeSync;
            _BBMDRegister = st.BBMDRegister;
            _BBMDAddress = st.BBMDAddress;
            _BBMDLifetime = st.BBMDLifetime;
            _DeviceHostName = st.DeviceHostName;
            _DeviceHostPort = st.DeviceHostPort;
            _DeviceInstance = st.DeviceInstance;
            _WhoIsDisabled = st.WhoIsDisabled;
            base.CopyProperties(st);
        }

        /// <summary>   Set propertys to default value. </summary>
        public void DefaultSettings()
        {
            _ForceInitialPolling = false;
            _COVInterval = 3600;
            _TimeSync = TimeSyncs.None;
            _BBMDAddress = string.Empty;
            _BBMDLifetime = 60;
            _BBMDRegister = false;
            _BBMDAddress = string.Empty;
            _DeviceHostName = "";
            _DeviceHostPort = 47808;
            _DeviceInstance = "";
            _WhoIsDisabled = false;
            base.DefaultSettings();
        }

        #region Properties

        private bool _BBMDRegister;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>    Register to BBMD as foreign </summary>
        ///
        /// <value> This option force the driver to register with a BBMD device as foreign, with the aim of 
        ///         being addressed with the broadcast messages originated in the BBMD BACnet network. 
        ///         This is compulsory if the device belong to a TCP/IP network different from that of the driver, 
        ///         connected through a router. Routers stops all the global broadcast and this prevent a 
        ///         correct information exchange. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool BBMDRegister
        {
            get
            {
                return _BBMDRegister;
            }
            set
            {
                SetPropertyValue("BBMDRegister", ref _BBMDRegister, value);
            }
        }

        private uint _BBMDLifetime;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>    BBMD registration lifetime (min.)  </summary>
        ///
        /// <value> Duration in minutes of the registration as foreign device.
        ///              durations less then 15 minutes are not allowed </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint BBMDLifetime
        {
            get
            {
                return _BBMDLifetime;
            }
            set
            {
                SetPropertyValue("BBMDLifetime", ref _BBMDLifetime, value);
            }
        }

        private string _BBMDAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   BBMD IP address. </summary>
        ///
        /// <value> IP address of the BBMD device. If it is left void, the main BACnet device IP is used. 
        ///         </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string BBMDAddress
        {
            get
            {
                return _BBMDAddress;
            }
            set
            {
                SetPropertyValue("BBMDAddress", ref _BBMDAddress, value);
            }
        }

        private TimeSyncs _TimeSync;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Send time synchronization. </summary>
        ///
        /// <value> Select to synchronize the time of the device with that of the PC. Two modes are allowed: 
        ///         System Time or UTC Time. 
        ///         If selected, the time synchronization is made during the initialization of the communication. 
        ///         </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public TimeSyncs TimeSync
        {
            get
            {
                return _TimeSync;
            }
            set
            {
                SetPropertyValue("TimeSync", ref _TimeSync, value);
            }
        }

        private uint _COVInterval;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   COV duration (sec.). </summary>
        ///
        /// <value> Specifies the duration of the Change Of Value subscription for a property of a Data Object,
        ///         in seconds. 0 means indefinite subscription,   values other than 0 require the renewal of 
        ///         the subscription, upon elapsing of this time.
        ///
        /// . </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint COVInterval
        {
            get
            {
                return _COVInterval;
            }
            set
            {
                SetPropertyValue("COVInterval", ref _COVInterval, value);
            }
        }

        private bool _ForceInitialPolling;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Force Initial Polling. </summary>
        ///
        /// <value> Normally, after a COV subscription is accepted, the device sends the "present value" of 
        ///         a data object. If the device doesn't act so, set this option to "True", to force a 
        ///         single read operation of the current value of a data object, after the COV subscription.
        /// </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ForceInitialPolling
        {
            get
            {
                return _ForceInitialPolling;
            }
            set
            {
                SetPropertyValue("ForceInitialPolling", ref _ForceInitialPolling, value);
            }
        }

        /// <summary>   Device Host name. </summary>
        private string _DeviceHostName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the Device host. </summary>
        ///
        /// <value> The name of the Device host. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string DeviceHostName
        {
            get { return _DeviceHostName; }
            set
            {
                SetPropertyValue("DeviceHostName", ref _DeviceHostName, value);
                this.RaisePropertyChangedEvent("DeviceInstance");
            }
        }

        /// <summary>   Device Host port. </summary>
        private int _DeviceHostPort;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Device host port. </summary>
        ///
        /// <value> The Device host port. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int DeviceHostPort
        {
            get
            {
                return _DeviceHostPort;
            }
            set
            {
                SetPropertyValue("DeviceHostPort", ref _DeviceHostPort, value);
            }
        }

        /// <summary>   Device Number. </summary>
        private string _DeviceInstance;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Device Number. </summary>
        ///
        /// <value> The Device Number. </value>
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
                this.RaisePropertyChangedEvent("DeviceHostName");
            }
        }

        private bool _WhoIsDisabled;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   'Who Is' request is disabled </summary>
        ///
        /// <value> true if the station has not use 'Who Is' to retrive information from device ,false otherwise
        ///         Used in some network configuration to baypass the use of BBMD --> Defice Identifier is required
        /// </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool WhoIsDisabled
        {
            get
            {
                return _WhoIsDisabled;
            }
            set
            {
                _WhoIsDisabled = value;
                this.RaisePropertyChangedEvent("WhoIsDisabled");
            }
        }

        public int getDeviceInstance()
        {            
            return BACnetProtocol.ParseDeviceInstanceString(_DeviceInstance);
        }
        #endregion



        #region IDataErrorInfo Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   BACnetStationSettings property validation. </summary>
        ///
        /// <param name="propertyName"> . </param>
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

            switch (propertyName)
            {
                case "DeviceHostName":
                    if (string.IsNullOrEmpty(_DeviceHostName))
                    {
                        return Properties.Resources.DeviceHostNameNotNull;
                    }
                    if (!Regex.IsMatch(_DeviceHostName, ValidIpAddressRegex) && !Regex.IsMatch(_DeviceHostName, ValidHostnameRegex))
                        return Properties.Resources.InvalidHostDeviceName;

                    if (DriverSettings != null && ((from s in DriverSettings.StationSettings/*.AsParallel()*/
                                                    where s != this && s.Channel == this.Channel &&
                                                    (s as BACnetStationSettings).DeviceHostName == _DeviceHostName &&
                                                    (s as BACnetStationSettings).DeviceHostPort == _DeviceHostPort &&
                                                    (s as BACnetStationSettings).getDeviceInstance() == getDeviceInstance()
                                                    select s).ToList().Count > 0))
                    {
                        return Properties.Resources.SelectExistingDeviceName;
                    }
                    break;
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
                        if (DriverSettings != null && ((from s in DriverSettings.StationSettings/*.AsParallel()*/
                                                        where s != this && s.Channel == this.Channel &&
                                                        (s as BACnetStationSettings).DeviceHostName == _DeviceHostName &&
                                                        (s as BACnetStationSettings).getDeviceInstance() == getDeviceInstance()
                                                        select s).ToList().Count > 0))
                        {
                            return Properties.Resources.SelectExistingDeviceName;
                        }
                    } else {
                        if (_WhoIsDisabled)
                            return Properties.Resources.DeviceInstanceInvalid;
                    }
                                        
                    break;

             case "BBMDLifetime":
                    if (_BBMDRegister)
                    {
                        if (_BBMDLifetime < 1 || _BBMDLifetime > 60)
                            return Properties.Resources.BBMDLifetimeOutOfRange;
                    }
                    break;

             case "BBMDAddress":
                    if (_BBMDRegister)
                    {
                        if (string.IsNullOrEmpty(_BBMDAddress))
                        {
                            return Properties.Resources.DeviceHostNameNotNull;
                        }
                        if (!Regex.IsMatch(_BBMDAddress, ValidIpAddressRegex) && !Regex.IsMatch(_BBMDAddress, ValidHostnameRegex))
                            return Properties.Resources.InvalidHostDeviceName;
                    }
                    break;

             case "WhoIsDisabled":
                    if (_WhoIsDisabled)
                    {
                        //use property's validator to verify if istance number was properly setted
                        if (string.IsNullOrEmpty(_DeviceInstance) || PerformValidation("DeviceInstance") != null) { 
                            return Properties.Resources.DeviceInstanceInvalid;
                        }                        
                    }
                    break;
            }

            return null;
        }

        #endregion

    }
}
