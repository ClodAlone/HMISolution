using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using UFInterfaces.Constants;
using System.ComponentModel;
using UFInterfaces;
using System.Windows.Media;
using DocumentManager.ComponentService;
using DevExpress.Xpo;
using System.Windows.Media.Imaging;

namespace UFCrossReferenceModel
{
    [DeferredDeletion(false)]
    public class NotUsedItem : XPObject
    {
        #region Constructors
        public NotUsedItem(Session session)
            : base(session)
        { }
        #endregion

        #region Properties
        CrossReferenceType cRType;
        public CrossReferenceType CRType
        {
            get
            {
                return cRType;
            }
            set
            {
                SetPropertyValue("CRType", ref cRType, value);
            }
        }
        int cRValue;
        public int CRValue
        {
            get
            {
                return cRValue;
            }
            set
            {
                SetPropertyValue("CRValue", ref cRValue, value);
            }
        }
        #endregion
    }
    [DeferredDeletion(false)]
    public class NotValidItem : XPObject
    {
        #region Constructors
        public NotValidItem(Session session)
            : base(session)
        { }
        #endregion

        #region Properties
        CrossReferenceType cRType;
        public CrossReferenceType CRType
        {
            get
            {
                return cRType;
            }
            set
            {
                SetPropertyValue("CRType", ref cRType, value);
            }
        }
        int cRValue;
        public int CRValue
        {
            get
            {
                return cRValue;
            }
            set
            {
                SetPropertyValue("CRValue", ref cRValue, value);
            }
        }
        #endregion
    }
}
