using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace MSModel
{
    public class WeeklyCalendarItem : XPObject
    {
        #region Ctor
        WeeklyCalendarItem()
        { }
        public WeeklyCalendarItem(Session session)
            : base(session)
        {

        }
        protected WeeklyCalendarItem(Session session, XPClassInfo classInfo)
            : base(session, classInfo)
        {

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

        #region Properties
        private DateTime _StartDate;
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                SetPropertyValue("StartDate", ref _StartDate, value);
            }
        }
        private DateTime _EndDate;
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                SetPropertyValue("EndDate", ref _EndDate, value);
            }
        }
        private MSScheduledAction _MSScheduledAction;
        [Association("MSScheduledAction-WeeklyCalendarItems")]
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
        private MSScheduledActionRuntime _MSScheduledActionRuntime;
        [Association("MSScheduledActionRuntime-WeeklyCalendarItems")]
        public MSScheduledActionRuntime MSScheduledActionRuntime
        {
            get
            {
                return _MSScheduledActionRuntime;
            }
            set
            {
                SetPropertyValue("MSScheduledActionRuntime", ref _MSScheduledActionRuntime, value);
            }
        }
        #endregion

        #region Methods
        public bool CompareWeekly(WeeklyCalendarItem sd)
        {
            return EndDate == sd.EndDate &&
                StartDate == sd.StartDate;
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
