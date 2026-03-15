using DriverCodeBaseEx;
using DevExpress.Xpo;

namespace IEC60870_5_104
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class IEC60870_5_104ChangeTag : ChangeTag
    {
        #region Constructors

        public IEC60870_5_104ChangeTag(Session session, Tag tag)
            : base(session, tag)
        {
        }

        public IEC60870_5_104ChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected IEC60870_5_104ChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion
    }
}
