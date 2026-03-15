using System;
using DevExpress.Xpo;

namespace UFRecipeExecuter
{
    [DeferredDeletion(false)]
    public class UFRecipeTransactionLogItem : XPObject
    {
        #region Constructors
        public UFRecipeTransactionLogItem(Session session) 
            : base (session) 
        { }

        public UFRecipeTransactionLogItem(Session session, UFRecipeTransactionLogItem template)
            : base (session)
        {
            EventType = template.EventType;
            RecipeName = template.RecipeName;
            XmlDataSet = template.XmlDataSet;
            EventDateTime = template.EventDateTime;
            EventDateTimeUtc = template.EventDateTimeUtc;
        }

        public UFRecipeTransactionLogItem(TransactionLogEventType eventType, string recipeName, string xmlDataSet, DateTime eventDateTime, DateTime eventDateTimeUTC)
        {
            EventType = eventType;
            RecipeName = recipeName;
            XmlDataSet = xmlDataSet;
            EventDateTime = eventDateTime;
            EventDateTimeUtc = eventDateTimeUTC;
        }
        #endregion

        #region Properties
        private TransactionLogEventType _EventType;
        [Indexed(Unique = false)]
        /// <summary>
        /// Event which defines the type of action will be recorded in the transaction log
        /// </summary>
        public TransactionLogEventType EventType
        {
            get
            {
                return _EventType;
            }
            set
            {
                SetPropertyValue(nameof(EventType), ref _EventType, value);
            }
        }

        private string _RecipeName;
        [Indexed(Unique = false)]
        [Size(SizeAttribute.Unlimited)]
        /// <summary>
        /// Serialized Data Set
        /// </summary>
        public string RecipeName
        {
            get
            {
                return _RecipeName;
            }
            set
            {
                SetPropertyValue(nameof(RecipeName), ref _RecipeName, value);
            }
        }

        private string _XmlDataSet;
        [Indexed(Unique = false)]
        [Size(SizeAttribute.Unlimited)]
        /// <summary>
        /// Serialized Data Set
        /// </summary>
        public string XmlDataSet
        {
            get
            {
                return _XmlDataSet;
            }
            set
            {
                SetPropertyValue(nameof(XmlDataSet), ref _XmlDataSet, value);
            }
        }

        private DateTime _EventDateTime;
        [Indexed(Unique = false)]
        public DateTime EventDateTime
        {
            get
            {
                return _EventDateTime;
            }
            set
            {
                SetPropertyValue(nameof(EventDateTime), ref _EventDateTime, value);
            }
        }

        private DateTime _EventDateTimeUtc;
        [Indexed(Unique = false)]
        public DateTime EventDateTimeUtc
        {
            get
            {
                return _EventDateTimeUtc;
            }
            set
            {
                SetPropertyValue(nameof(EventDateTimeUtc), ref _EventDateTimeUtc, value);
            }
        }
        #endregion
    }
}
