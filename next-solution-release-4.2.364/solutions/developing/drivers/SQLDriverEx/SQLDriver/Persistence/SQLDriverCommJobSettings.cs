using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace SQLDriver
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class SQLDriverCommJobSettings : CommJobSettings
    {
                #region Constructors

        public SQLDriverCommJobSettings(Session session, SQLDriverCommJob job)
            : base(session, job)
        {
            _SQLDriverColumnName = job.SQLDriverColumnName;
            _SQLDriverColumnValue = job.SQLDriverColumnValue;
            _TagName = job.TagName;
        }
        
        public SQLDriverCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected SQLDriverCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _SQLDriverColumnName = String.Empty;
            _FrequencyOfSendingSignal = 30;
            _TagName = string.Empty;
        }

        #region Properties

        /// <summary>
        /// Address
        /// </summary>
        private string _SQLDriverColumnName;
        [Size(SizeAttribute.Unlimited)]
        public string SQLDriverColumnName
        {
            get
            {
                return _SQLDriverColumnName;
            }

            set
            {
                SetPropertyValue("SQLDriverColumnName", ref _SQLDriverColumnName, value);
            }
        }
        /// <summary>
        /// Address
        /// </summary>
        private string _SQLDriverColumnValue;
        [Size(SizeAttribute.Unlimited)]
        public string SQLDriverColumnValue
        {
            get
            {
                return _SQLDriverColumnValue;
            }

            set
            {
                SetPropertyValue("SQLDriverColumnValue", ref _SQLDriverColumnValue, value);
            }
        }

        /// <summary>
        /// Address
        /// </summary>
        private string _TagName;
        [Size(SizeAttribute.Unlimited)]
        public string TagName
        {
            get
            {
                return _TagName;
            }
            set
            {
                SetPropertyValue("TagName", ref _TagName, value);
            }
        }

        /// <summary>
        /// Address
        /// </summary>
        private UInt32 _FrequencyOfSendingSignal;
        [Size(SizeAttribute.Unlimited)]
        public UInt32 FrequencyOfSendingSignal
        {
            get
            {
                return _FrequencyOfSendingSignal;
            }

            set
            {
                SetPropertyValue("FrequencyOfSendingSignal", ref _FrequencyOfSendingSignal, value);
            }
        }

        #endregion


        #region IDataErrorInfo Members
        #endregion
    
    }
}
