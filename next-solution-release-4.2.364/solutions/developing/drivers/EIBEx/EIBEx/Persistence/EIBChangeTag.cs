using DriverCodeBaseEx;
using DevExpress.Xpo;

namespace EIB
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class EIBChangeTag : ChangeTag
    {
        #region Constructors

        public EIBChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public EIBChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected EIBChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
