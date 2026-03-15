////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Persistence\IEC60870_5_104CommJobSettings.cs
//
// summary:	Implements the driver IEC60870_5_104 communications job settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace IEC60870_5_104
{
    /// <summary>   Settings for the protocol's task(IEC60870_5_104CommJob). </summary>
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class IEC60870_5_104CommJobSettings : CommJobSettings
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set propertys from IEC60870_5_104CommJob "job". </summary>
        ///
        /// <param name="session">  . </param>
        /// <param name="job">      . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IEC60870_5_104CommJobSettings(Session session, IEC60870_5_104CommJob job)
            : base(session, job)
        {
            _ASDUType = job.ASDUType;
            _StartAddMon = job.StartAddMon;
            _StartAddCtrl = job.StartAddCtrl;
            _CmdQualifier = job.CmdQualifier;
            _ParamQualifier = job.ParamQualifier;
            _CmdAction = job.CmdAction;
            _CotVariableName = job.CotVariableName;
            _CotVariableId = job.CotVariableId;
            _QualityVariableName = job.QualityVariableName;
            _QualityVariableId = job.QualityVariableId;
            _UpdateTimeStamp = job.UpdateTimeStamp;
            _WriteTimeStamp = job.WriteTimeStamp;
            _FileNameVariableName = job.FileNameVariableName;
            _FileNameVariableId = job.FileNameVariableId;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set propertys from IEC60870_5_104CommJob "job". </summary>
        ///
        /// <param name="session">  . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IEC60870_5_104CommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected IEC60870_5_104CommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        /// <summary>   Set propertys to default value. </summary>
        public void DefaultSettings()
        {
            base.DefaultSettings();
            _ASDUType = ASDUSelectableTypes.SinglePoint;
            _StartAddMon = 1;
            _StartAddCtrl = 1;
            _CmdQualifier = CommandQualifiers.NotUsedCQ;
            _ParamQualifier = ParamQualifiers.NotUsedPQ;
            _CmdAction = CommandActions.NotUsedCA;
            _CotVariableName = string.Empty;
            _QualityVariableName = string.Empty;
            _UpdateTimeStamp = false;
            _WriteTimeStamp = false;
            _FileNameVariableName = string.Empty;
        }

        #region Properties

        /// <summary>   ASDU Type. </summary>
        private ASDUSelectableTypes _ASDUType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Information Object Type. </summary>
        ///
        /// <value> ASDU Type. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ASDUSelectableTypes ASDUType
        {
            get
            {
                return _ASDUType;
            }
            set
            {
                SetPropertyValue("ASDUType", ref _ASDUType, value);
            }
        }
        
        /// <summary>   Monitor IOA. </summary>
        private UInt32 _StartAddMon;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Information Object Address for Monitor Direction. </summary>
        ///
        /// <value> Monitor IOA. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public UInt32 StartAddMon
        {
            get
            {
                return _StartAddMon;
            }
            set
            {
                SetPropertyValue("StartAddMon", ref _StartAddMon, value);
            }
        }

        /// <summary>   Control IOA. </summary>
        private UInt32 _StartAddCtrl;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Information Object Address for Control Direction (leave 0 if only monitoring). </summary>
        ///
        /// <value> Control IOA. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public UInt32 StartAddCtrl
        {
            get
            {
                return _StartAddCtrl;
            }
            set
            {
                SetPropertyValue("StartAddCtrl", ref _StartAddCtrl, value);
            }
        }

        /// <summary>   Command Qualifier. </summary>
        private CommandQualifiers _CmdQualifier;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Command Qualifier. </summary>
        ///
        /// <value> Command Qualifier. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public CommandQualifiers CmdQualifier
        {
            get
            {
                return _CmdQualifier;
            }
            set
            {
                SetPropertyValue("CmdQualifier", ref _CmdQualifier, value);
            }
        }

        /// <summary>   Parameter Qualifier. </summary>
        private ParamQualifiers _ParamQualifier;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Parameter Qualifier. </summary>
        ///
        /// <value> Parameter Qualifier. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ParamQualifiers ParamQualifier
        {
            get
            {
                return _ParamQualifier;
            }
            set
            {
                SetPropertyValue("ParamQualifier", ref _ParamQualifier, value);
            }
        }

        /// <summary>   Command Action. </summary>
        private CommandActions _CmdAction;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Command Action. </summary>
        ///
        /// <value> Command Action. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public CommandActions CmdAction
        {
            get
            {
                return _CmdAction;
            }
            set
            {
                SetPropertyValue("CmdAction", ref _CmdAction, value);
            }
        }

        /// <summary>   COT Variable Name. </summary>
        private string _CotVariableName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Optional parameter that specifies the name of the variable where the driver will store 
        ///             the ""Cause Of Transmission"" of the received data. </summary>
        ///
        /// <value> COT Variable Name. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string CotVariableName
        {
            get
            {
                return _CotVariableName;
            }
            set
            {
                SetPropertyValue("CotVariableName", ref _CotVariableName, value);
            }
        }

        /// <summary>   COT Variable Id. </summary>
        private string _CotVariableId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Optional parameter that specifies the Id of the variable where the driver will store 
        ///             the ""Cause Of Transmission"" of the received data. </summary>
        ///
        /// <value> COT Variable Id. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string CotVariableId
        {
            get
            {
                return _CotVariableId;
            }
            set
            {
                SetPropertyValue("CotVariableId", ref _CotVariableId, value);
            }
        }

        /// <summary>   Quality Variable Name. </summary>
        private string _QualityVariableName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Optional parameter that specifies the name of the variable where the driver will store 
        ///             the ""Quality Descriptor"" of the received data. </summary>
        ///
        /// <value> Quality Variable Name. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string QualityVariableName
        {
            get
            {
                return _QualityVariableName;
            }
            set
            {
                SetPropertyValue("QualityVariableName", ref _QualityVariableName, value);
            }
        }

        /// <summary>   Quality Variable Id. </summary>
        private string _QualityVariableId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Optional parameter that specifies the Id of the variable where the driver will store 
        ///             the ""Quality Descriptor"" of the received data. </summary>
        ///
        /// <value> Quality Variable Id. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string QualityVariableId
        {
            get
            {
                return _QualityVariableId;
            }
            set
            {
                SetPropertyValue("QualityVariableId", ref _QualityVariableId, value);
            }
        }

        /// <summary> Always update TimeStamp. </summary>
        private bool _UpdateTimeStamp;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Select if the driver have to update timestamp at every read. </summary>
        ///
        /// <value> Always update TimeStamp. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool UpdateTimeStamp
        {
            get
            {
                return _UpdateTimeStamp;
            }
            set
            {
                SetPropertyValue("UpdateTimeStamp", ref _UpdateTimeStamp, value);
            }
        }

        /// <summary> Write TimeStamp. </summary>
        private bool _WriteTimeStamp;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Add local TimeStamp to writes on the device. </summary>
        ///
        /// <value> Write TimeStamp. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool WriteTimeStamp
        {
            get
            {
                return _WriteTimeStamp;
            }
            set
            {
                SetPropertyValue("WriteTimeStamp", ref _WriteTimeStamp, value);
            }
        }

        /// <summary>   File Name Variable Name. </summary>
        private string _FileNameVariableName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Optional parameter that specifies the name of the variable that contains 
        ///             the name of the file to be uploaded. </summary>
        ///
        /// <value> File Name Variable Name. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string FileNameVariableName
        {
            get
            {
                return _FileNameVariableName;
            }
            set
            {
                SetPropertyValue("FileNameVariableName", ref _FileNameVariableName, value);
            }
        }

        /// <summary>   File Name Variable Id. </summary>
        private string _FileNameVariableId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Optional parameter that specifies the Id of the variable that contains 
        ///             the name of the file to be uploaded. </summary>
        ///
        /// <value> File Name Variable Id. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string FileNameVariableId
        {
            get
            {
                return _FileNameVariableId;
            }
            set
            {
                SetPropertyValue("FileNameVariableId", ref _FileNameVariableId, value);
            }
        }

        #endregion

        #region IDataErrorInfo Members
        #endregion

    }
}
