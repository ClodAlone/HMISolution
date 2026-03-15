using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace PubNub
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class PubNubCommJobSettings : CommJobSettings
    {
                #region Constructors

        public PubNubCommJobSettings(Session session, PubNubCommJob job)
            : base(session, job)
        {
            _TagName = job.TagName;
        }
        
        public PubNubCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected PubNubCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _TagName = String.Empty;
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

        #endregion


        #region IDataErrorInfo Members

        #endregion
    
    }
}
