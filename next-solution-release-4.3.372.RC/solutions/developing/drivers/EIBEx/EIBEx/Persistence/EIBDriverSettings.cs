using DriverCodeBaseEx;
using DevExpress.Xpo;

namespace EIB
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class EIBDriverSettings : DriverSettings
    {
        #region Constructors

        public EIBDriverSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected EIBDriverSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
               
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
        }

        #region Properties

        /// <summary>
        /// Access Protocol Code
        /// </summary>
        private string _AccessProtocolCode;
        public string AccessProtocolCode
        {
            get { return _AccessProtocolCode; }
            set
            {
                SetPropertyValue("AccessProtocolCode", ref _AccessProtocolCode, value);
            }
        }

        #endregion

        #region IDataErrorInfo Members
        #endregion
    }
}
