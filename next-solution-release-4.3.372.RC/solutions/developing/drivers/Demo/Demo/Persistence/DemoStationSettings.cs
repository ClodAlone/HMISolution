using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;
namespace Demo
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DemoStationSettings : StationSettings
    {
        #region Constructors

        public DemoStationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected DemoStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public void CopyProperties(DemoStationSettings st)
        {
            base.CopyProperties(st);

            SimulationInterval = st.SimulationInterval;
        }

        public void DefaultSettings()
        {
            base.DefaultSettings();
        }

        #region Properties
        /// <summary>
        /// Starting simulation intervall of the station (milliseconds)
        /// </summary>
        private uint _SimulationInterval = 500;
        public uint SimulationInterval
        {
            get { return _SimulationInterval; }
            set
            {
                SetPropertyValue("SimulationInterval", ref _SimulationInterval, value);
            }
        }
        #endregion

        #region IDataErrorInfo Members
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "SimulationInterval")
            {
                if (SimulationInterval < uint.MinValue || SimulationInterval > uint.MaxValue)
                    return string.Format(Properties.Resources.ValueOutOfRange, uint.MinValue, uint.MaxValue);
            }

            return null;
        }

        #endregion
    }
}
