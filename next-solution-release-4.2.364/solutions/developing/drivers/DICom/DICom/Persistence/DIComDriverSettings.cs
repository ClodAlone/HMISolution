using DriverCodeBase;
using DevExpress.Xpo;

namespace DICom
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DIComDriverSettings : DriverSettings
    {
       #region Constructors

        public DIComDriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected DIComDriverSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
               
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
        }

        #region IDataErrorInfo Members

        #endregion
   
    }
}
