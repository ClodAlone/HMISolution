using DriverCodeBase;
using DevExpress.Xpo;

namespace RMS621
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class RMS621ChangeTag : ChangeTag
    {
        #region Constructors

        public RMS621ChangeTag(Session session, Tag tag)
            : base(session,tag)
        {
        }

        public RMS621ChangeTag(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        protected RMS621ChangeTag()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

    }
}
