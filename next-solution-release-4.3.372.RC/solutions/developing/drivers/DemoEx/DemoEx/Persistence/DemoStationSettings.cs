using System;
using DriverCodeBaseEx;
using DevExpress.Xpo;

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
            session.UpdateSchema(typeof(DemoStationSettings));
        }
        protected DemoStationSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        #endregion

        public override void CopyProperties(StationSettings st)
        {
            base.CopyProperties(st);
            _SimulationInterval = ((DemoStationSettings)st).SimulationInterval;
            _LaunchSimulationAtStartUp = ((DemoStationSettings)st).LaunchSimulationAtStartUp;
        }
    
        public void DefaultSettings()
        {
            base.DefaultSettings();
            base.MaxRetriesBeforeError = 0;
        }

        #region Properties
        /// <summary>
        /// Starting simulation intervall of the station (milliseconds)
        /// </summary>
        private uint _SimulationInterval = DemoProtocol.SIMULATION_INTERVAL_DEFAULT_VALUE;
        public uint SimulationInterval
        {
            get { return _SimulationInterval; }
            set { SetPropertyValue("SimulationInterval", ref _SimulationInterval, value); }
        }

        private bool? _LaunchSimulationAtStartUp;

        // Launch simulation during driver start up
        public bool? LaunchSimulationAtStartUp
        {
            get { return _LaunchSimulationAtStartUp; }

            set { SetPropertyValue("LaunchSimulationAtStartUp", ref _LaunchSimulationAtStartUp, value); }
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
                if (SimulationInterval <= 0)
                    return Properties.Resources.ErrorSimulationIntervalNegativeOr0;
            }

            return null;
        }
        #endregion


        #region Properties Default Values        
        /// <summary>
        /// Adds inside this method the nullable property where you want handle a default value.
        /// </summary>
        protected override void EnsureDefaultValues()
        {
            base.EnsureDefaultValues();

            if (!_LaunchSimulationAtStartUp.HasValue)
                _LaunchSimulationAtStartUp = true;
        }
        #endregion
    }
}
