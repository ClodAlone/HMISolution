using DriverCodeBaseEx;
using DevExpress.Xpo;

namespace Demo
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DemoCommJobSettings : CommJobSettings
    {
        #region Constructors

        public DemoCommJobSettings(Session session, DemoCommJob job)
            : base(session, job)
        {
            _SimulationInterval = job.SimulationInterval;
            _DemoType = job.DemoType;
            _MinValue = job.MinValue;
            _MaxValue = job.MaxValue;
            _DeltaValue = job.DeltaValue;
            _NrCycles = job.NrCycles;
            _FactorSinCos = job.FactorSinCos;
        }
        
        public DemoCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected DemoCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _SimulationInterval = DemoProtocol.SIMULATION_INTERVAL_DEFAULT_VALUE;
            _DemoType = DemoProtocol.DemoTypes.Sin;
            _MinValue = 0;
            _MaxValue = 100;
            _DeltaValue = 1;
            _NrCycles = 0;
            _FactorSinCos = 1;
        }

        #region Properties        

        private DemoProtocol.DemoTypes _DemoType;
        public DemoProtocol.DemoTypes DemoType
        {
            get { return _DemoType; }
            set { _DemoType = value; }
        }

        private uint _SimulationInterval;
        public uint SimulationInterval
        {
            get { return _SimulationInterval; }
            set { _SimulationInterval = value; }
        }

        private double _MinValue;
        public double MinValue
        {
            get { return _MinValue; }
            set { _MinValue = value; }
        }
                
        private double _MaxValue;
        public double MaxValue
        {
            get { return _MaxValue; }
            set { _MaxValue = value; }
        }

        private double _DeltaValue;
        public double DeltaValue
        {
            get { return _DeltaValue; }
            set { _DeltaValue = value; }
        }
        
        private uint _NrCycles;
        public uint NrCycles
        {
            get { return _NrCycles; }
            set { _NrCycles = value; }
        }

        private double _FactorSinCos;
        public double FactorSinCos
        {
            get { return _FactorSinCos; }
            set { _FactorSinCos = value; }
        }
        #endregion

        #region IDataErrorInfo Members
        #endregion

    }
}
