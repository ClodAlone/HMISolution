using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
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
            session.UpdateSchema(typeof(MelsecQEthStationSettings));
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

        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
            MaxRetriesBeforeError = 0;
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

            return null;
        }

        #endregion
    }
}
