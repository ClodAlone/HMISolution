using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace MSModel
{
    public class MSAction : XPObject
    {
        #region Ctor
        protected MSAction()
        {
            
        }
        public MSAction(Session session)
            : base(session)
        {
            
        }
        protected MSAction(Session session, DevExpress.Xpo.Metadata.XPClassInfo classInfo)
            : base(session, classInfo)
        {

        }

        private MSScheduledAction _MSScheduledAction;
        [Association("MSScheduledAction-MSAction")]
        public MSScheduledAction MSScheduledAction
        {
            get
            {
                return _MSScheduledAction;
            }
            set
            {
                SetPropertyValue("MSScheduledAction", ref _MSScheduledAction, value);
            }
        }
        #endregion

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        //const int defaultPropertyName = -1;

        /// <summary>
        /// Adds inside this method the nullable property where you want handle a default value.
        /// </summary>
        private void EnsureDefaultValues()
        {
            // Examples of how to handle a default value
            //if (!PropertyName.HasValue)
            //    PropertyName = defaultPropertyName;
            //if (TimeSpanPropertyName == TimeSpan.Zero)
            //    TimeSpanPropertyName = TimeSpan.FromMinutes(1);
            //if (DateTimePropertyName == DateTime.MinValue)
            //    DateTimePropertyName = DateTime.UtcNow;
        }
        #endregion

        #region Override Methods

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            EnsureDefaultValues();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            EnsureDefaultValues();
        }

        #endregion
    }
}
