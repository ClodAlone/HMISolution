////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\IEC60870_5_104StationSettings.cs
//
// summary:	Implements the driver IEC60870_5_104 station settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using DriverCodeBaseEx;

/* Unmerged change from project 'IEC60870-5-104Ex (netstandard2.0)'
Before:
using DevExpress.Xpo;
After:
using DevExpress.Xpo;
using IEC60870_5_104;
using IEC60870_5_104.Persistence;
*/
using DevExpress.Xpo;

namespace IEC60870_5_104
{
    /// <summary>   Settings for the drivers's station(IEC60870_5_104Station). </summary>
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class IEC60870_5_104StationSettings : StationSettings
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IEC60870_5_104StationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected IEC60870_5_104StationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set propertys from IEC60870_5_104StationSettings "st". </summary>
        ///
        /// <param name="st">   . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void CopyProperties(IEC60870_5_104StationSettings st)
        {
            base.CopyProperties(st);
            _CommonAddress = st.CommonAddress;
            _MaxOutAPDU = st.MaxOutAPDU;
            _MaxInAPDU = st.MaxInAPDU;
            _MissingAckTimeout = st.MissingAckTimeout;
            _NoInMsgTimeout = st.NoInMsgTimeout;
            _NoActivityTimeout = st.NoActivityTimeout;
            _EnablePeriodicReset = st.EnablePeriodicReset;
            _FileTransferDirectory = st.FileTransferDirectory;
        }

        /// <summary>   Set propertys to default value. </summary>
        public void DefaultSettings()
        {
            base.DefaultSettings();
            _CommonAddress = 3;
            _MaxOutAPDU = 12;		//max number of I outstanding APDUs before stopping transmission (k)
            _MaxInAPDU = 8;		//number of I incoming APDUs after which an S acknowledge is sent (w)
            _MissingAckTimeout = 15000;	//timeout for resetting connection in case a message is not ack'ed (t1)
            _NoInMsgTimeout = 10000;		//timeout after which  an S acknowledge is sent if no more data coming(t2)
            _NoActivityTimeout = 20000;	//timeout after which a test frame is sent if no activity (t3)
            _EnablePeriodicReset = false;
            _FileTransferDirectory = string.Empty;
        }

        #region Properties

        /// <summary>   Common Address. </summary>
        private ushort _CommonAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Controlled station's Common Address. </summary>
        ///
        /// <value> Common Address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ushort CommonAddress
        {
            get
            {
                return _CommonAddress;
            }
            set
            {
                SetPropertyValue("CommonAddress", ref _CommonAddress, value);
            }
        }

        /// <summary>   Max Outstanding APDUs. </summary>
        private uint _MaxOutAPDU;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Max number of I outstanding APDUs before stopping transmission (k). </summary>
        ///
        /// <value> Max Outstanding APDUs. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint MaxOutAPDU
        {
            get
            {
                return _MaxOutAPDU;
            }
            set
            {
                SetPropertyValue("MaxOutAPDU", ref _MaxOutAPDU, value);
            }
        }

        /// <summary>   Max Incoming APDUs. </summary>
        private uint _MaxInAPDU;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Number of I incoming APDUs after which an S acknowledge is sent (w). </summary>
        ///
        /// <value> Max Incoming APDUs. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint MaxInAPDU
        {
            get
            {
                return _MaxInAPDU;
            }
            set
            {
                SetPropertyValue("MaxInAPDU", ref _MaxInAPDU, value);
            }
        }

        /// <summary>   Missing ACK Timeout in ms. </summary>
        private uint _MissingAckTimeout;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Confirm timeout of send or test APDUs (t1). </summary>
        ///
        /// <value> Missing ACK Timeout in ms. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint MissingAckTimeout
        {
            get
            {
                return _MissingAckTimeout;
            }
            set
            {
                SetPropertyValue("MissingAckTimeout", ref _MissingAckTimeout, value);
            }
        }

        /// <summary>   No Incoming Msg Timeout in ms. </summary>
        private uint _NoInMsgTimeout;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Maximum amount of time after receiving an I-Type data APDU before sending an S-Type 
        ///             confirm APDU (t2) </summary>
        ///
        /// <value> No Incoming Msg Timeout in ms. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint NoInMsgTimeout
        {
            get
            {
                return _NoInMsgTimeout;
            }
            set
            {
                SetPropertyValue("NoInMsgTimeout", ref _NoInMsgTimeout, value);
            }
        }

        /// <summary>   No Activity Timeout in ms. </summary>
        private uint _NoActivityTimeout;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Timeout for sending test frames in case of a long idle state (t3) </summary>
        ///
        /// <value> No Activity Timeout in ms. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint NoActivityTimeout
        {
            get
            {
                return _NoActivityTimeout;
            }
            set
            {
                SetPropertyValue("NoActivityTimeout", ref _NoActivityTimeout, value);
            }
        }

        /// <summary>   Connection periodic reset. </summary>
        private bool _EnablePeriodicReset;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Enable periodically stop and restart of the connection </summary>
        ///
        /// <value> Connection periodic reset. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool EnablePeriodicReset
        {
            get
            {
                return _EnablePeriodicReset;
            }
            set
            {
                SetPropertyValue("EnablePeriodicReset", ref _EnablePeriodicReset, value);
            }
        }

        /// <summary>   Folder for File Transfer. </summary>
        private string _FileTransferDirectory;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Folder for File Transfer </summary>
        ///
        /// <value> Name of the folder for File Transfer. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string FileTransferDirectory
        {
            get
            {
                return _FileTransferDirectory;
            }
            set
            {
                SetPropertyValue("FileTransferDirectory", ref _FileTransferDirectory, value);
            }
        }



        #endregion



        #region IDataErrorInfo Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   IEC60870_5_104StationSettings property validation. </summary>
        ///
        /// <param name="propertyName"> . </param>
        ///
        /// <returns>   A String. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override string PerformValidation(string propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;
            if (propertyName == "Channel")
            {
                if ((from c in DriverSettings.StationSettings
                     where c.Channel == Channel && c.Name != Name
                     select c).ToList().Count != 0)
                {
                    return Properties.Resources.ChannelAlreadyAllocated;
                }
            }

            return null;
        }

        #endregion

    }
}
