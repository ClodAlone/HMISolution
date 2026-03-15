////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\IEC60870_5_104ChannelSettings.cs
//
// summary:	Implements the driver IEC60870_5_104 channel settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using IpDriverCodeBaseEx;
using DevExpress.Xpo;

/* Unmerged change from project 'IEC60870-5-104Ex (netstandard2.0)'
Before:
using System.ComponentModel;
After:
using System.ComponentModel;
using IEC60870_5_104.Persistence;
using IEC60870_5_104;
*/
using System.ComponentModel;

namespace IEC60870_5_104
{
    /// <summary>   Settings for the drivers's channel(IEC60870_5_104Channel). </summary>
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class IEC60870_5_104ChannelSettings : TcpChannelSettings
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IEC60870_5_104ChannelSettings(Session session)
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
        private IEC60870_5_104ChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }


        #endregion

        /// <summary>   Set propertys to default value. </summary>
        public void DefaultSettings()
        {
            base.DefaultSettings();
            TcpChannelSettingsHostPort = 2404;
            OriginatorAddress = 1;
            InitializationTimeOut = 20;
            RepeatInitialInterrogation = false;
            EnableInitialClockSynchronization = true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set propertys from IEC60870_5_104ChannelSettings "ch". </summary>
        ///
        /// <param name="ch">   . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void CopyProperties(IEC60870_5_104ChannelSettings ch)
        {
            base.CopyProperties(ch);
            OriginatorAddress = ch.OriginatorAddress;
            InitializationTimeOut = ch.InitializationTimeOut;
            RepeatInitialInterrogation = ch.RepeatInitialInterrogation;
            EnableInitialClockSynchronization = ch.EnableInitialClockSynchronization;
        }

        #region Properties

        /// <summary>   The Originator Address for this Driver. </summary>
        private byte _OriginatorAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Originator Address for this Driver. </summary>
        ///
        /// <value> The Originator Address for this Driver. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public byte OriginatorAddress
        {
            get
            {
                return _OriginatorAddress;
            }
            set
            {
                SetPropertyValue("OriginatorAddress", ref _OriginatorAddress, value);
            }
        }

        /// <summary>   The timeout for the initial data exchange procedure. </summary>
        private uint _InitializationTimeOut;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initialization Timeout. </summary>
        ///
        /// <value> The Initialization Timeout. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint InitializationTimeOut
        {
            get
            {
                return _InitializationTimeOut;
            }
            set
            {
                SetPropertyValue("InitializationTimeOut", ref _InitializationTimeOut, value);
            }
        }

        /// <summary>   Repeat initial data request. </summary>
        private bool _RepeatInitialInterrogation;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   If any data is missing, repeat periodically the initial data request until all data have been initialized. </summary>
        ///
        /// <value> Repeat initial data request. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool RepeatInitialInterrogation
        {
            get
            {
                return _RepeatInitialInterrogation;
            }
            set
            {
                SetPropertyValue("RepeatInitialInterrogation", ref _RepeatInitialInterrogation, value);
            }
        }

        /// <summary>   Enable Initial Clock Synchronization. </summary>
        private bool _EnableInitialClockSynchronization;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   If set to True, an initial clock synchronization between the supervisor and the devices is performed. </summary>
        ///
        /// <value> Enable Initial Clock Synchronization. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool EnableInitialClockSynchronization
        {
            get
            {
                return _EnableInitialClockSynchronization;
            }
            set
            {
                SetPropertyValue("EnableInitialClockSynchronization", ref _EnableInitialClockSynchronization, value);
            }
        }


        #endregion


        #region IDataErrorInfo Members
        #endregion

    }
}
