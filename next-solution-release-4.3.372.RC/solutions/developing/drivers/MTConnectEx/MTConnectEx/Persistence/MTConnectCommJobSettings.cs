using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MTConnect
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class MTConnectCommJobSettings : CommJobSettings
    {
                #region Constructors

        public MTConnectCommJobSettings(Session session, MTConnectCommJob job)
            : base(session, job)
        {
            _TagName = job.TagName;
            _PathParameter = job.PathParameter;
            _FrequencyOfSendingSignal = (uint)job.FrequencyOfSendingSignal.TotalSeconds;
            //_Category = job.Category;
            //_SupervisonDataType = job.SupervisonDataType;
        }
        
        public MTConnectCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected MTConnectCommJobSettings()
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
            //_Category = (int) MTConnectProtocol.Category_E.Sample;
            //_SupervisonDataType = (int)MTConnectProtocol.FormatDataType_E.Double;
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
        /// PathParameter
        /// </summary>
        private string _PathParameter;
        [Size(SizeAttribute.Unlimited)]
        public string PathParameter
        {
            get
            {
                return _PathParameter;
            }

            set
            {
                SetPropertyValue("PathParameter", ref _PathParameter, value);
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

        ///// <summary>
        ///// Category
        ///// </summary>
        //private MTConnectProtocol.Category_E _Category;
        //[Size(SizeAttribute.Unlimited)]
        //public MTConnectProtocol.Category_E Category
        //{
        //    get
        //    {
        //        return _Category;
        //    }

        //    set
        //    {
        //        SetPropertyValue("Category", ref _Category, value);
        //    }
        //}

        //// <summary>
        ///// SupervisonDataType
        ///// </summary>
        //private MTConnectProtocol.FormatDataType_E _SupervisonDataType;
        //[Size(SizeAttribute.Unlimited)]
        //public MTConnectProtocol.FormatDataType_E SupervisonDataTypey
        //{
        //    get
        //    {
        //        return _SupervisonDataType;
        //    }

        //    set
        //    {
        //        SetPropertyValue("SupervisonDataType", ref _SupervisonDataType, value);
        //    }
        //}


        #endregion


        #region IDataErrorInfo Members
        #endregion

    }
}
