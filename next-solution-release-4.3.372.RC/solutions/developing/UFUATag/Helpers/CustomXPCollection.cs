using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAModel.Helpers
{
    public class CustomXPCollection<T> : XPCollection<T>
    {
        #region Constructors
        public CustomXPCollection()
        { }

        public CustomXPCollection(Session session, object theOwner, XPMemberInfo refProperty) 
            : base(session, theOwner, refProperty)
        { }

        public CustomXPCollection(Session session, XPBaseCollection originalCollection) 
            : base(session, originalCollection)
        { }
        #endregion

        #region Overrides
        public override bool Equals(object obj)
        {
            if (obj is IList<T>)
            {
                var collection = obj as IList<T>;
                if (collection.Count != this.Count)
                    return false;

                for (int ii = 0; ii < collection.Count; ii++)
                {
                    if (!this[ii].Equals(collection[ii]))
                        return false;
                }

                return true;
            }

            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
