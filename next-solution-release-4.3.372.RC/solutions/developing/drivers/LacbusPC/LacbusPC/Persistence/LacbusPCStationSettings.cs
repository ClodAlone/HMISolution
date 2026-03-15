using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace LacbusPC
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class LacbusPCStationSettings : StationSettings
    {
        #region Constructors

        public LacbusPCStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected LacbusPCStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void CopyProperties(LacbusPCStationSettings st)
        {
            base.CopyProperties(st);
            LacbusPCRTUNumber = st.LacbusPCRTUNumber;
            LacbusPCProtocolType = st.LacbusPCProtocolType;
            LacbusPCPhoneNumber = st.LacbusPCPhoneNumber;
            LacbusPCAutomaticPollRTU = st.LacbusPCAutomaticPollRTU;
            LacbusPCCommunicationDuration = st.LacbusPCCommunicationDuration;
            LacbusPCAutomaticPollRTUFrequency = st.LacbusPCAutomaticPollRTUFrequency;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            LacbusPCRTUNumber = 0;
            LacbusPCProtocolType = LacbusPcUnderlyingProtocols.LacbusPC;
            LacbusPCPhoneNumber = String.Empty;
            LacbusPCAutomaticPollRTU = true;
            LacbusPCCommunicationDuration = 0;
            LacbusPCAutomaticPollRTUFrequency = 60;
        }

        #region Properties

        /// <summary>
        /// Enter the Number Of the RTU
        /// </summary>
        private UInt16 _LacbusPCRTUNumber;
        public UInt16 LacbusPCRTUNumber
        {
            get
            {
                return _LacbusPCRTUNumber;
            }
            set
            {
                SetPropertyValue("LacbusPCRTUNumber", ref _LacbusPCRTUNumber, value);
                RaisePropertyChangedEvent("LacbusPCProtocolType");
                RaisePropertyChangedEvent("LacbusPCPhoneNumber");
            }
            
        }

        /// <summary>
        /// Select the communication protocol for the RTU
        /// </summary>
        private LacbusPcUnderlyingProtocols _LacbusPCProtocolType;
        public LacbusPcUnderlyingProtocols LacbusPCProtocolType
        {
            get
            {
                return _LacbusPCProtocolType;
            }
            set
            {
                SetPropertyValue("LacbusPCProtocolType", ref _LacbusPCProtocolType, value);
                RaisePropertyChangedEvent("LacbusPCRTUNumber");
                RaisePropertyChangedEvent("LacbusPCPhoneNumber");
            }
        }

        /// <summary>
        /// Enter the Phone Number
        /// </summary>
        private string _LacbusPCPhoneNumber;
        public string LacbusPCPhoneNumber
        {
            get
            {
                return _LacbusPCPhoneNumber;
            }
            set
            {
                SetPropertyValue("LacbusPCPhoneNumber", ref _LacbusPCPhoneNumber, value);
                RaisePropertyChangedEvent("LacbusPCProtocolType");
                RaisePropertyChangedEvent("LacbusPCRTUNumber");
            }
        }

        /// <summary>
        /// Enable/Disable the automatic "Poll RTU Request" for this station 
        /// </summary>
        private bool _LacbusPCAutomaticPollRTU;
        [Category("Device Data")]
        [Description("Automatic Polling")]
        [Size(SizeAttribute.Unlimited)]
        public bool LacbusPCAutomaticPollRTU
        {
            get
            {
                return _LacbusPCAutomaticPollRTU;
            }
            set
            {
                SetPropertyValue("LacbusPCAutomaticPollRTU", ref _LacbusPCAutomaticPollRTU, value);
            }
        }

        /// <summary>
        /// Enter the Communication Duration for Poll Request (seconds)
        /// </summary>
        private UInt16 _LacbusPCCommunicationDuration;
        [Category("Device Data")]
        [Description("Communication Duration")]
        [Size(SizeAttribute.Unlimited)]
        public UInt16 LacbusPCCommunicationDuration
        {
            get
            {
                return _LacbusPCCommunicationDuration;
            }
            set
            {
                SetPropertyValue("LacbusPCCommunicationDuration", ref _LacbusPCCommunicationDuration, value);
            }
        }

        /// <summary>
        /// Enter the Frequency for Poll Requests (seconds)
        /// </summary>
        private UInt16 _LacbusPCAutomaticPollRTUFrequency;
        [Category("Device Data")]
        [Description("Polling Frequency")]
        [Size(SizeAttribute.Unlimited)]
        public UInt16 LacbusPCAutomaticPollRTUFrequency
        {
            get
            {
                return _LacbusPCAutomaticPollRTUFrequency;
            }
            set
            {
                SetPropertyValue("LacbusPCAutomaticPollRTUFrequency", ref _LacbusPCAutomaticPollRTUFrequency, value);
            }
        }

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
            {
                return sBase;
            }
           
            if (propertyName == "LacbusPCProtocolType")
            {
                switch (_LacbusPCProtocolType)
                {
                    case LacbusPcUnderlyingProtocols.LacbusPC:
                        if (DriverSettings != null && ((from c in DriverSettings.StationSettings/*.AsParallel()*/
                                                        where c != this && 
                                                        ((LacbusPCStationSettings)c)._LacbusPCProtocolType == _LacbusPCProtocolType &&
                                                        (((LacbusPCStationSettings)c).Channel == Channel)
                                                        select c).ToList().Count > 0))
                        {
                            return Properties.Resources.ErrorCommunicationProtocolAlreadyExist;
                        }
                        break;
                    case LacbusPcUnderlyingProtocols.LacbusRTU:
                        if (DriverSettings != null && ((from c in DriverSettings.StationSettings/*.AsParallel()*/
                                                        where c != this && 
                                                        ((LacbusPCStationSettings)c)._LacbusPCRTUNumber == _LacbusPCRTUNumber &&
                                                        ((LacbusPCStationSettings)c)._LacbusPCProtocolType == _LacbusPCProtocolType &&
                                                        (((LacbusPCStationSettings)c).Channel == Channel)
                                                        select c).ToList().Count > 0))
                        {
                            return Properties.Resources.ErrorCommunicationProtocolAlreadyExistRtu;
                        }
                        break;
                    case LacbusPcUnderlyingProtocols.LacbusSofbusSMS:
                        if (DriverSettings != null && ((from c in DriverSettings.StationSettings/*.AsParallel()*/
                                                        where c != this &&
                                                        ((LacbusPCStationSettings)c)._LacbusPCProtocolType == _LacbusPCProtocolType &&
                                                        ((LacbusPCStationSettings)c)._LacbusPCRTUNumber == _LacbusPCRTUNumber &&
                                                        ((LacbusPCStationSettings)c)._LacbusPCPhoneNumber == _LacbusPCPhoneNumber &&
                                                        (((LacbusPCStationSettings)c).Channel == Channel)
                                                        select c).ToList().Count > 0))
                        {
                            return Properties.Resources.ErrorCommunicationProtocolAlreadyExistPhoneAndRTU;
                        }
                        break;
                    case LacbusPcUnderlyingProtocols.SofbusPL:
                        if (DriverSettings != null && ((from c in DriverSettings.StationSettings/*.AsParallel()*/
                                                        where c != this &&
                                                        ((LacbusPCStationSettings)c)._LacbusPCProtocolType == _LacbusPCProtocolType &&
                                                        ((LacbusPCStationSettings)c)._LacbusPCRTUNumber == _LacbusPCRTUNumber &&
                                                        (((LacbusPCStationSettings)c).Channel == Channel)
                                                        select c).ToList().Count > 0))
                        {
                            return Properties.Resources.ErrorCommunicationProtocolAlreadyExistRtu;
                        }
                        break;
                }                
            }
            if (propertyName == "LacbusPCRTUNumber")
            {
                switch (_LacbusPCProtocolType)
                {
                    case LacbusPcUnderlyingProtocols.LacbusRTU:
                        if (DriverSettings != null && ((from c in DriverSettings.StationSettings/*.AsParallel()*/
                                                        where c != this &&
                                                        ((LacbusPCStationSettings)c)._LacbusPCRTUNumber == _LacbusPCRTUNumber &&
                                                        ((LacbusPCStationSettings)c)._LacbusPCProtocolType == _LacbusPCProtocolType &&
                                                        (((LacbusPCStationSettings)c).Channel == Channel)
                                                        select c).ToList().Count > 0))
                        {
                            return Properties.Resources.ErrorCommunicationProtocolAlreadyExistRtu;
                        }
                        break;
                    case LacbusPcUnderlyingProtocols.LacbusSofbusSMS:
                        if (DriverSettings != null && ((from c in DriverSettings.StationSettings/*.AsParallel()*/
                                                        where c != this &&
                                                        ((LacbusPCStationSettings)c)._LacbusPCProtocolType == _LacbusPCProtocolType &&
                                                        ((LacbusPCStationSettings)c)._LacbusPCRTUNumber == _LacbusPCRTUNumber &&
                                                        ((LacbusPCStationSettings)c)._LacbusPCPhoneNumber == _LacbusPCPhoneNumber &&
                                                        (((LacbusPCStationSettings)c).Channel == Channel)
                                                        select c).ToList().Count > 0))
                        {
                            return Properties.Resources.ErrorCommunicationProtocolAlreadyExistPhoneAndRTU;
                        }
                        break;
                    case LacbusPcUnderlyingProtocols.SofbusPL:
                        if (DriverSettings != null && ((from c in DriverSettings.StationSettings/*.AsParallel()*/
                                                        where c != this &&
                                                        ((LacbusPCStationSettings)c)._LacbusPCProtocolType == _LacbusPCProtocolType &&
                                                        ((LacbusPCStationSettings)c)._LacbusPCRTUNumber == _LacbusPCRTUNumber &&
                                                        (((LacbusPCStationSettings)c).Channel == Channel)
                                                        select c).ToList().Count > 0))
                        {
                            return Properties.Resources.ErrorCommunicationProtocolAlreadyExistRtu;
                        }
                        break;
                }
            }
            if (propertyName == "LacbusPCPhoneNumber")
            {
                switch (_LacbusPCProtocolType)
                {
                    case LacbusPcUnderlyingProtocols.LacbusSofbusSMS:
                        if (DriverSettings != null && ((from c in DriverSettings.StationSettings/*.AsParallel()*/
                                                        where c != this &&
                                                        ((LacbusPCStationSettings)c)._LacbusPCProtocolType == _LacbusPCProtocolType &&
                                                        ((LacbusPCStationSettings)c)._LacbusPCRTUNumber == _LacbusPCRTUNumber &&
                                                        ((LacbusPCStationSettings)c)._LacbusPCPhoneNumber == _LacbusPCPhoneNumber &&
                                                        (((LacbusPCStationSettings)c).Channel == Channel)
                                                        select c).ToList().Count > 0))
                        {
                            return Properties.Resources.ErrorCommunicationProtocolAlreadyExistPhoneAndRTU;
                        }
                        break;                  
                }
            }
            return null;
        }

        #endregion

        #region IDataErrorInfo Members
        #endregion

        #region OnPropertyChanged
        protected override void OnPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "Channel":
                    RaisePropertyChangedEvent("LacbusPCProtocolType");
                    RaisePropertyChangedEvent("LacbusPCRTUNumber");
                    RaisePropertyChangedEvent("LacbusPCPhoneNumber");
                    break;
            }
        }
        #endregion
    }
}
