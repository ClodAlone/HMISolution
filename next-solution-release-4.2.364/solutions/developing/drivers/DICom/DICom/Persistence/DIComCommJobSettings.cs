using System;
using DriverCodeBase;
using DevExpress.Xpo;

namespace DICom
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DIComCommJobSettings : CommJobSettings
    {
        #region Constructors

        public DIComCommJobSettings(Session session, DIComCommJob job)
            : base(session, job)
        {
            _VarName = job.VarName;
        }
        
        public DIComCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected DIComCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _VarName = string.Empty;
        }

        #region Properties

        private string _VarName;
        public string VarName
        {
            get { return _VarName; }
            set { SetPropertyValue("VarName", ref _VarName, value); }
        }
        #endregion

        #region IDataErrorInfo Members
        #endregion
    
    }
}
