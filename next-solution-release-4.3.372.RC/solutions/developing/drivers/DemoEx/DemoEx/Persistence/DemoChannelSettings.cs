using DriverCodeBaseEx;
using DevExpress.Xpo;

namespace Demo
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DemoChannelSettings : ChannelSettings
    {
        #region Constructors

        public DemoChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(DemoChannelSettings));
        }

        private DemoChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        public override void CopyProperties(ChannelSettings ch)
        {
            base.CopyProperties(ch);
        }

        #region Properties

        #endregion
        
    }
}
