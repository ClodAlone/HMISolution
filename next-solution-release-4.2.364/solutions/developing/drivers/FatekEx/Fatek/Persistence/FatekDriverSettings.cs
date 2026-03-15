using DriverCodeBaseEx;
using DevExpress.Xpo;

namespace Fatek
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class FatekDriverSettings : DriverSettings
    {
        #region Constructors

        public FatekDriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected FatekDriverSettings()
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
