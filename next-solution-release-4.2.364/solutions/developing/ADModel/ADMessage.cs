using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using System.ComponentModel;

namespace ADModel
{
    class ADMessage : XPObject, IDataErrorInfo
    {
        #region Constructors
        public ADMessage(Session session)
            : base(session)
        { }
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
        private Guid _MessageID;
        [ReadOnly(true)]
        public Guid MessageID
        {
            get
            {
                return _MessageID;
            }
            set
            {
                SetPropertyValue("MessageID", ref _MessageID, value);
            }
        }
        private string _Message;
        [Size(SizeAttribute.Unlimited)]
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                SetPropertyValue("Message", ref _Message, value);
            }
        }
        private DateTime _Deadline;
        public DateTime Deadline
        {
            get
            {
                return _Deadline;
            }
            set
            {
                SetPropertyValue("Deadline", ref _Deadline, value);
            }
        }
        private string _Recipient;
        [Size(SizeAttribute.Unlimited)]
        public string Recipient
        {
            get
            {
                return _Recipient;
            }
            set
            {
                SetPropertyValue("Recipient", ref _Recipient, value);
            }
        }
        private int _Errors;
        public int Errors
        {
            get
            {
                return _Errors;
            }
            set
            {
                SetPropertyValue("Errors", ref _Errors, value);
            }
        }
        private MediaType _Media;
        public MediaType Media
        {
            get
            {
                return _Media;
            }
            set
            {
                SetPropertyValue("Media", ref _Media, value);
            }
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

        #region IDataErrorInfo Members
        [Browsable(false)]
        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}