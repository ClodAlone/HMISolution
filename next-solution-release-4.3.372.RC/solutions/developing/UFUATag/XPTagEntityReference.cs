using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAModel
{
    [DeferredDeletion(false)]
    public class XPTagEntityReference : XPObject
    {
        #region Constructors
        public XPTagEntityReference(Session session)
            : base(session)
        { }
        #endregion

        #region Properties
        private TagEntityReference _TagEntity;
        [ValueConverter(typeof(ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public TagEntityReference TagEntity
        {
            get
            {
                return _TagEntity;
            }
            set
            {
                SetPropertyValue("TagEntity", ref _TagEntity, value);
            }
        }

        private UFUAAlarmThreshold _UFUAAlarmThresholdReference;
        [Association("UFUAAlarmThreshold-AliasTags")]
        public UFUAAlarmThreshold UFUAAlarmThresholdReference
        {
            get
            {
                return _UFUAAlarmThresholdReference;
            }
            set
            {
                SetPropertyValue("UFUAAlarmThresholdReference", ref _UFUAAlarmThresholdReference, value);
            }
        }
        #endregion

        #region overrides

        public override bool Equals(object obj)
        {
            if (obj is XPTagEntityReference)
            {
                var tagEntityReference = obj as XPTagEntityReference;
                if (tagEntityReference.TagEntity != null && TagEntity != null)
                    return tagEntityReference.TagEntity.ToString() == TagEntity.ToString();

                return false;
            }
            else
                return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}
