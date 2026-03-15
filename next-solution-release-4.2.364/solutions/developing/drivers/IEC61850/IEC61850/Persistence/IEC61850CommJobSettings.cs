using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace IEC61850
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class IEC61850CommJobSettings : CommJobSettings
    {
        #region Constructors

        public IEC61850CommJobSettings(Session session, IEC61850CommJob job)
            : base(session, job)
        {
            _LogicalDeviceName = job.LogicalDeviceName;
            _LogicalNodeName = job.LogicalNodeName;
            _FunctionalConstraint = job.FunctionalConstraint;
            _DataItemIdentifier = job.DataItemIdentifier;
            _MMSDataType = job.MMSDataType;
            _DataMaximumLength = job.DataMaximumLength;
            _RetryOutputInCaseOfError = job.RetryOutputInCaseOfError;
            _ReportLogicalDeviceName = job.ReportLogicalDeviceName;
            _ReportLogicalNodeName = job.ReportLogicalNodeName;
            _ReportName = job.ReportName;
            _ReportType = job.ReportType;
            _InitializeData = job.InitializeData;
        }

        public IEC61850CommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected IEC61850CommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _LogicalDeviceName = String.Empty;
            _LogicalNodeName = String.Empty;
            _FunctionalConstraint = FunctionalConstraints.None;
            _DataItemIdentifier = String.Empty;
            _MMSDataType = MMSDataTypes.Boolean;
            _DataMaximumLength = 0;
            _RetryOutputInCaseOfError = false;
            _ReportLogicalDeviceName = String.Empty;
            _ReportLogicalNodeName = String.Empty;
            _ReportName = String.Empty;
            _ReportType = ReportTypes.None;
            _InitializeData = true;
        }

        #region Properties

        // Name of the logical device
        private string _LogicalDeviceName;
        public string LogicalDeviceName
        {
            get { return _LogicalDeviceName; }
            set
            {
                SetPropertyValue("LogicalDeviceName", ref _LogicalDeviceName, value);
            }
        }

        //Name of the logical node
        private string _LogicalNodeName;
        public string LogicalNodeName
        {
            get { return _LogicalNodeName; }
            set
            {
                SetPropertyValue("LogicalNodeName", ref _LogicalNodeName, value);
            }
        }

        // Functional constraint of the data item
        private FunctionalConstraints _FunctionalConstraint;
        public FunctionalConstraints FunctionalConstraint
        {
            get { return _FunctionalConstraint; }
            set
            {
                SetPropertyValue("FunctionalConstraint", ref _FunctionalConstraint, value);
            }
        }

        // Identifier of the data item
        private string _DataItemIdentifier;
        public string DataItemIdentifier
        {
            get { return _DataItemIdentifier; }
            set
            {
                SetPropertyValue("DataItemIdentifier", ref _DataItemIdentifier, value);
            }
        }

        // MMS data type
        private MMSDataTypes _MMSDataType;
        public MMSDataTypes MMSDataType
        {
            get { return _MMSDataType; }
            set
            {
                SetPropertyValue("MMSDataType", ref _MMSDataType, value);
            }
        }

        // The data maximum length
        private uint _DataMaximumLength;
        public uint DataMaximumLength
        {
            get { return _DataMaximumLength; }
            set
            {
                SetPropertyValue("DataMaximumLength", ref _DataMaximumLength, value);
            }
        }

        // Retry output in case of error
        private bool _RetryOutputInCaseOfError;
        public bool RetryOutputInCaseOfError
        {
            get { return _RetryOutputInCaseOfError; }
            set
            {
                SetPropertyValue("RetryOutputInCaseOfError", ref _RetryOutputInCaseOfError, value);
            }
        }

        // Name of the logical device of the report
        private string _ReportLogicalDeviceName;
        public string ReportLogicalDeviceName
        {
            get { return _ReportLogicalDeviceName; }
            set
            {
                SetPropertyValue("ReportLogicalDeviceName", ref _ReportLogicalDeviceName, value);
            }
        }

        // Name of the logical node of the report
        private string _ReportLogicalNodeName;
        public string ReportLogicalNodeName
        {
            get { return _ReportLogicalNodeName; }
            set
            {
                SetPropertyValue("ReportLogicalNodeName", ref _ReportLogicalNodeName, value);
            }
        }

        // Name of the logical node of the report
        private string _ReportName;
        public string ReportName
        {
            get { return _ReportName; }
            set
            {
                SetPropertyValue("ReportName", ref _ReportName, value);
            }
        }

        // Report type
        private ReportTypes _ReportType;
        public ReportTypes ReportType
        {
            get { return _ReportType; }
            set
            {
                SetPropertyValue("ReportType", ref _ReportType, value);
            }
        }

        // Initialize Data
        private bool _InitializeData;
        public bool InitializeData
        {
            get { return _InitializeData; }
            set
            {
                SetPropertyValue("InitializeData", ref _InitializeData, value);
            }
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
