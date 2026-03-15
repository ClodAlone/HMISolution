using DriverCodeBase;
using DevExpress.Xpo;

namespace Fatek
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class FatekChangeTag : ChangeTag
    {
        #region Constructors

        public FatekChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public FatekChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected FatekChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion
    }
}
