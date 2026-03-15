using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace BrPvi
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class BrPviCommJobSettings : CommJobSettings
    {
        #region Constructors

        public BrPviCommJobSettings(Session session, BrPviCommJob job)
            : base(session, job)
        {
            BrPviVariableName = job.BrPviVariableName;
            BrPviTaskName = job.BrPviTaskName;
            BrPviRefreshRate = job.BrPviRefreshRate;
            BrPviArrayLength = job.BrPviArrayLength;
        }
        
        public BrPviCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected BrPviCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            BrPviVariableName = String.Empty;
            BrPviTaskName = String.Empty;
            BrPviRefreshRate = 500;
            BrPviArrayLength = 0;
        }

        #region Properties

        /// <summary>
        /// PVI Variable Name
        /// </summary>
        private string _BrPviVariableName;
        [Size(SizeAttribute.Unlimited)]
        public string BrPviVariableName
        {
            get
            {
                return _BrPviVariableName;
            }

            set
            {
                SetPropertyValue("BrPviVariableName", ref _BrPviVariableName, value);
            }
        }

        /// <summary>
        /// PVI Task Name
        /// </summary>
        private string _BrPviTaskName;
        [Size(SizeAttribute.Unlimited)]
        public string BrPviTaskName
        {
            get
            {
                return _BrPviTaskName;
            }

            set
            {
                SetPropertyValue("BrPviTaskName", ref _BrPviTaskName, value);
            }
        }

        /// <summary>
        /// Refresh Rate
        /// </summary>
        private int _BrPviRefreshRate;
        public int BrPviRefreshRate
        {
            get { return _BrPviRefreshRate; }
            set { SetPropertyValue("BrPviRefreshRate", ref _BrPviRefreshRate, value); }
        }

        /// <summary>
        /// Array Length
        /// </summary>
        private uint _BrPviArrayLength;
        public uint BrPviArrayLength
        {
            get { return _BrPviArrayLength; }
            set { SetPropertyValue("BrPviArrayLength", ref _BrPviArrayLength, value); }
        }

        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            return null;
        }

        #endregion

    }
}
