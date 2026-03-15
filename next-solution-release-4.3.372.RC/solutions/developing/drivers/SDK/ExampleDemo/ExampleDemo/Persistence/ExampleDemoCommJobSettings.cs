using DriverCodeBase;
using DevExpress.Xpo;

namespace ExampleDemo
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    internal sealed class ExampleDemoCommJobSettings : CommJobSettings
    {
        #region Constructors

        public ExampleDemoCommJobSettings(Session session, ExampleDemoCommJob job)
            : base(session, job)
        {
            _DemoType = job.DemoType;
        }
        
        protected ExampleDemoCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected ExampleDemoCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        #region Properties
        private ExampleDemoTypes _DemoType;
        public ExampleDemoTypes DemoType
        {
            get
            {
                return _DemoType;
            }
            set
            {
                SetPropertyValue("DemoCode", ref _DemoType, value);
            }
        }
        #endregion

    }
}
