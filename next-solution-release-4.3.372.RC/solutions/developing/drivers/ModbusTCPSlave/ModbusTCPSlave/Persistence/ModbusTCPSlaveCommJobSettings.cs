using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace ModbusTCPSlave
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class ModbusTCPSlaveCommJobSettings : CommJobSettings
    {
                #region Constructors

        public ModbusTCPSlaveCommJobSettings(Session session, ModbusTCPSlaveCommJob job)
            : base(session, job)
        {
            _DataArea = job.DataArea;
            _StartAddress = job.StartAddress;
        }
        
        public ModbusTCPSlaveCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected ModbusTCPSlaveCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _DataArea = DataAreas.HoldingRegisters;
            _StartAddress = 0;
        }

        #region Properties

        private DataAreas _DataArea;
        public DataAreas DataArea
        {
            get
            {
                return _DataArea;
            }
            set
            {
                SetPropertyValue("DataArea", ref _DataArea, value);
            }
        }
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

        #endregion


        #region IDataErrorInfo Members
        #endregion
    
    }
}
