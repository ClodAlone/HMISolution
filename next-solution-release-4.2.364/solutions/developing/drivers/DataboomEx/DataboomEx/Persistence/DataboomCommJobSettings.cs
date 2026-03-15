using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace Databoom
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class DataboomCommJobSettings : CommJobSettings
    {
                #region Constructors

        public DataboomCommJobSettings(Session session, DataboomCommJob job)
            : base(session, job)
        {
            _TagName = job.TagName;
            _FrequencyOfSendingSignal = (uint)job.FrequencyOfSendingSignal.TotalSeconds;
        }
        
        public DataboomCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected DataboomCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _TagName = String.Empty;
            _FrequencyOfSendingSignal = 30;
        }

        #region Properties

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
