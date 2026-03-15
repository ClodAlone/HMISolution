using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MelsecQEth
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MelsecQEthStationSettings : StationSettings
    {
        #region Constructors

        public MelsecQEthStationSettings(Session session)
            : base(session)
        {
            NetworkNumber = 0;
            PcNumber = 255;
            MaxRetriesBeforeError = 0;
        }

        protected MelsecQEthStationSettings()
        {
            NetworkNumber = 0;
            PcNumber = 255;
            MaxRetriesBeforeError = 0;
        }

        #endregion

        public void CopyProperties(MelsecQEthStationSettings st)
        {
            base.CopyProperties(st);
            NetworkNumber = st.NetworkNumber;
            PcNumber = st.PcNumber;
            PlcType = ((MelsecQEthStationSettings)st).PlcType;
            LabelNrMaxAggregatedRequests = st.LabelNrMaxAggregatedRequests;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            MaxRetriesBeforeError = 0;
            PlcType = MelsecQEthProtocol.PlcTypes.CPU_MODEL_QnA;
            LabelNrMaxAggregatedRequests = MelsecQEth.Properties.Settings.Default.LabelNrMaxAggregatedRequests;
        }

        #region Properties

        /// <summary>
        /// Enter the Network Number 
        /// </summary>
        private uint _NetworkNumber;
        public uint NetworkNumber
        {
            get
            {
                return _NetworkNumber;
            }
            set
            {
                SetPropertyValue("NetworkNumber", ref _NetworkNumber, value);
            }
        }

        /// <summary>
        /// Enter the Pc Number 
        /// </summary>
        private uint _PcNumber;
        public uint PcNumber
        {
            get
            {
                return _PcNumber;
            }
            set
            {
                SetPropertyValue("PcNumberr", ref _PcNumber, value);
            }
        }

        /// <summary>
        /// Enter the PLC type
        /// </summary>
        private MelsecQEthProtocol.PlcTypes? _PlcType;
        public MelsecQEthProtocol.PlcTypes? PlcType
        {
            get { return _PlcType; }
            set
            {
                SetPropertyValue("PlcType", ref _PlcType, value);
                this.RaisePropertyChangedEvent("LabelNrMaxAggregatedRequests");
            }
        }

        /// <summary>
        /// The maximum number of simultaneous requests for tags with 'Label' addressing 
        /// </summary>
        private uint? _LabelNrMaxAggregatedRequests;
        public uint? LabelNrMaxAggregatedRequests
        {
            get { return _LabelNrMaxAggregatedRequests; }
            set
            {
                SetPropertyValue("LabelNrMaxAggregatedRequests", ref _LabelNrMaxAggregatedRequests, value);
            }
        }
        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
            {
                return sBase;
            }
            if (propertyName == "PcNumberr")
            {
                if (PcNumber < 0 || PcNumber > 255)
                    return Properties.Resources.ErrorPcNumberr;
            }
            else if (propertyName == "NetworkNumber")
            {
                if (NetworkNumber < 0 || NetworkNumber > 255)
                    return Properties.Resources.ErrorNetworkNumber;
            }
            else if (propertyName == "LabelNrMaxAggregatedRequests")
            {
                if (MelsecQEthProtocol.PlcSupportLabelAddress((MelsecQEthProtocol.PlcTypes)PlcType))
                {
                    if (_LabelNrMaxAggregatedRequests < 1 || _LabelNrMaxAggregatedRequests > 1000)
                        return String.Format(Properties.Resources.ErrorLabelMaxAggregatedRequests, 1, 1000);
                }
            }
            return null;
        }

        #endregion

        #region Override Methods

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            EnsureDefaultValues();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            EnsureDefaultValues();
        }

        #endregion

        #region Properties Default Values        
        /// <summary>
        /// Adds inside this method the nullable property where you want handle a default value.
        /// </summary>
        private void EnsureDefaultValues()
        {
            if (!_LabelNrMaxAggregatedRequests.HasValue)
                _LabelNrMaxAggregatedRequests = MelsecQEth.Properties.Settings.Default.LabelNrMaxAggregatedRequests;

            if (!_PlcType.HasValue)
                _PlcType = MelsecQEthProtocol.PlcTypes.CPU_MODEL_QnA;
        }
        #endregion
    }
}
