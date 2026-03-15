using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MelsecQEth
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MelsecQEthCommJobSettings : CommJobSettings
    {
        #region Constructors

        public MelsecQEthCommJobSettings(Session session, MelsecQEthCommJob job)
            : base(session, job)
        {
            _Address = job.Address;
            _CpuTarget = job.CpuTarget;
            _StringLength = job.StringLength;
            _UnicodeString = job.UnicodeString;
        }

        public MelsecQEthCommJobSettings(Session session)
            : base(session)
        {
        }

        public MelsecQEthCommJobSettings()
        {
        }

        #endregion

        public void DefaultSettings()
        {
            _Address = String.Empty;
        }

        #region Properties

        /// <summary>
        /// Address
        /// </summary>
        private string _Address;
        public string Address
        {
            get
            {
                return _Address;
            }

            set
            {
                SetPropertyValue("Address", ref _Address, value);
            }
        }

        /// <summary>
        /// CpuTarget
        /// </summary>
        private CpuTargets _CpuTarget;
        public CpuTargets CpuTarget
        {
            get
            {
                return _CpuTarget;
            }

            set
            {
                SetPropertyValue("CpuTarget", ref _CpuTarget, value);
            }
        }

        /// <summary>
        /// String size
        /// </summary>
        private UInt16 _StringLength;
        public UInt16 StringLength
        {
            get
            {
                return _StringLength;
            }

            set
            {
                SetPropertyValue("StringLength", ref _StringLength, value);
            }
        }

        private bool _UnicodeString;
        public bool UnicodeString
        {
            get
            {
                return _UnicodeString;
            }

            set
            {
                SetPropertyValue("UnicodeString", ref _UnicodeString, value);
            }
        }

        #endregion

        #region IDataErrorInfo Members
        #endregion
    }
}
