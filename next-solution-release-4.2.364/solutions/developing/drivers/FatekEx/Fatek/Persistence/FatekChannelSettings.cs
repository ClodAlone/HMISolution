using DevExpress.Xpo;
using IpDriverCodeBaseEx;

namespace Fatek
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class FatekChannelSettings : TcpChannelSettings// ChannelSettings
    {
        #region Constructors

        public FatekChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private FatekChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            TcpChannelSettingsHostPort = FatekProtocol.PROTOCOL_DEFAULT_TCP_PORT;
        }

        public void CopyProperties(FatekChannelSettings ch)
        {
            base.CopyProperties(ch);
        }

        #region General Properties
        #endregion

        #region IDataErrorInfo Members        
        //protected override String PerformValidation(String propertyName)
        //{
        //    string sBase = base.PerformValidation(propertyName);
        //    if (sBase != null)
        //        return sBase;

        //    //switch (propertyName) {
        //    //    case "TurnaroundDelay":
        //    //    //if (TurnaroundDelay > uint.MaxValue)
        //    //    //{
        //    //    //    return string.Format(Properties.Resources.TurnaroundDelayOutOfRange, uint.MaxValue);
        //    //    //}
        //    //    break;
        //    //}

        //    return null;
        //}

        #endregion

    }
}
