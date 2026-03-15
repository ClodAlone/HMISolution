using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace SaiaDataMode
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class SaiaDataModeCommJobSettings : CommJobSettings
    {
                #region Constructors

        public SaiaDataModeCommJobSettings(Session session, SaiaDataModeCommJob job)
            : base(session, job)
        {
            _StartAddress = job.StartAddress.USHORT;
            _DbNumber = job.DbNumber.USHORT;
            _AreaType = job.AreaType;
            _DataConversionType = job.DataConversionType;
        }
        
        public SaiaDataModeCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected SaiaDataModeCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _StartAddress = 0;
            _DbNumber = 0;
            _AreaType = AreaTypes.Inputs;
            _DataConversionType = DataConversionTypes.None;
        }

        #region Properties

        private UInt16 _StartAddress;
        public UInt16 StartAddress
        {
            get
            {
                return _StartAddress;
            }
            set
            {
                SetPropertyValue("StartAddress", ref _StartAddress, value);
            }
        }

        private AreaTypes _AreaType;
        public AreaTypes AreaType
        {
            get
            {
                return _AreaType;
            }
            set
            {
                SetPropertyValue("AreaType", ref _AreaType, value);
            }
        }

        private UInt16 _DbNumber;
        public UInt16 DbNumber
        {
            get
            {
                return _DbNumber;
            }
            set
            {
                SetPropertyValue("DbNumber", ref _DbNumber, value);
            }
        }

        private DataConversionTypes _DataConversionType;
        public DataConversionTypes DataConversionType
        {
            get
            {
                return _DataConversionType;
            }
            set
            {
                SetPropertyValue("DataConversionType", ref _DataConversionType, value);
            }
        }

        #endregion


        #region IDataErrorInfo Members
        #endregion
    
    }
}
