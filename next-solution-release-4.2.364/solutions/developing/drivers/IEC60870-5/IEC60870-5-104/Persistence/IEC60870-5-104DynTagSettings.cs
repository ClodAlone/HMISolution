////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	IEC60870_5_104DynTagSettings.cs
//
// summary:	Implements the driver IEC60870_5_104 dynamic tag settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Helpers;
using DriverCodeBase.Enumerators;
using System.ComponentModel;
using DriverBaseInterfaces;
using Opc.Ua;

namespace IEC60870_5_104
{
    /// <summary>   Dynamic tag settings of the IEC60870_5_104 driver. </summary>
    public sealed class IEC60870_5_104DynTagSettings : DynTagSettings
    {
        #region Constructors

        /// <summary>   Initializes the IEC60870_5_104DynTagSettings. </summary>
        public IEC60870_5_104DynTagSettings()
            : base()
        {
            _ASDUType = ASDUSelectableTypes.SinglePoint;
            _StartAddMon = 1;
            _StartAddCtrl = 1;
            _CmdQualifier = CommandQualifiers.NotUsedCQ;
            _ParamQualifier = ParamQualifiers.NotUsedPQ;
            _CmdAction = CommandActions.NotUsedCA;
            _CotVariableId = string.Empty;
            _CotVariableName = string.Empty;
            _QualityVariableId = string.Empty;
            _QualityVariableName = string.Empty;
            _UpdateTimeStamp = false;
            _WriteTimeStamp = false;
            _FileNameVariableId = string.Empty;
            _FileNameVariableName = string.Empty;

        }

        #endregion

        #region Static Members

        /// <summary>   The ASDU Type parameter. </summary>
        private static readonly String ASDUTypeParameter = "ASDU";
        /// <summary>   The Monitor IOA parameter. </summary>
        private static readonly String StartAddMonParameter = "IOAM";
        /// <summary>   The Control IOA parameter. </summary>
        private static readonly String StartAddCtrlParameter = "IOAC";
        /// <summary>   The Command Qualifier parameter. </summary>
        private static readonly String CmdQualifierParameter = "QOC";
        /// <summary>   The Parameter Qualifier parameter. </summary>
        private static readonly String ParamQualifierParameter = "QOP";
        /// <summary>   The Command Action parameter. </summary>
        private static readonly String CmdActionParameter = "CA";
        /// <summary>   The Always update TimeStamp parameter. </summary>
        private static readonly String UpdateTimeStampParameter = "UTS";
        /// <summary>   The Write TimeStamp  parameter. </summary>
        private static readonly String WriteTimeStampParameter = "WTS";
        /// <summary>   The Cot Variable parameter name. </summary>
        private static readonly String CotVariableNameParameter = "COTVName";
        /// <summary>   The Cot Variable parameter id. </summary>
        private static readonly String CotVariableIdParameter = "COTVId";
        /// <summary>   The Quality Variable  parameter name. </summary>
        private static readonly String QualityVariableNameParameter = "QUALVName";
        /// <summary>   The Quality Variable  parameter id. </summary>
        private static readonly String QualityVariableIdParameter = "QUALVId";
        /// <summary>   The File Name Variable parameter name. </summary>
        private static readonly String FileNameVariableNameParameter = "FileVName";
        /// <summary>   The File Name Variable parameter id. </summary>
        private static readonly String FileNameVariableIdParameter = "FileVId";


        #endregion

        #region Override Functions

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes the IEC60870_5_104DynTagSettings from string dynamicSettings.
        /// </summary>
        ///
        /// <param name="dynamicSettings">  . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            ASDUType = (ASDUSelectableTypes)(helper.GetPartByName(ASDUTypeParameter, (UInt16)ASDUSelectableTypes.SinglePoint));
            StartAddMon = helper.GetPartByName(StartAddMonParameter, (UInt32)1);
            StartAddCtrl = helper.GetPartByName(StartAddCtrlParameter, (UInt32)1);
            CmdQualifier = (CommandQualifiers)helper.GetPartByName(CmdQualifierParameter, (UInt16)CommandQualifiers.NotUsedCQ);
            ParamQualifier = (ParamQualifiers)helper.GetPartByName(ParamQualifierParameter, (UInt16)ParamQualifiers.NotUsedPQ);
            CmdAction = (CommandActions)helper.GetPartByName(CmdActionParameter, (UInt16)CommandActions.NotUsedCA);
            UpdateTimeStamp = helper.GetPartByName(UpdateTimeStampParameter, false);
            WriteTimeStamp = helper.GetPartByName(WriteTimeStampParameter, false);
            CotVariableName = helper.GetPartByName(CotVariableNameParameter);
            CotVariableId = helper.GetPartByName(CotVariableIdParameter);
            QualityVariableName = helper.GetPartByName(QualityVariableNameParameter);
            QualityVariableId = helper.GetPartByName(QualityVariableIdParameter);
            FileNameVariableName = helper.GetPartByName(FileNameVariableNameParameter);
            FileNameVariableId = helper.GetPartByName(FileNameVariableIdParameter);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes the IEC60870_5_104DynTagSettings from string dynamicSettings if is possible.
        /// </summary>
        ///
        /// <param name="dynamicSettings">  . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(ASDUTypeParameter)))
                return false;

            ASDUType = (ASDUSelectableTypes)(helper.GetPartByName(ASDUTypeParameter, (UInt16)ASDUSelectableTypes.SinglePoint));
            StartAddMon = helper.GetPartByName(StartAddMonParameter, (UInt32)1);
            StartAddCtrl = helper.GetPartByName(StartAddCtrlParameter, (UInt32)1);
            CmdQualifier = (CommandQualifiers)helper.GetPartByName(CmdQualifierParameter, (UInt16)CommandQualifiers.NotUsedCQ);
            ParamQualifier = (ParamQualifiers)helper.GetPartByName(ParamQualifierParameter, (UInt16)ParamQualifiers.NotUsedPQ);
            CmdAction = (CommandActions)helper.GetPartByName(CmdActionParameter, (UInt16)CommandActions.NotUsedCA);
            UpdateTimeStamp = helper.GetPartByName(UpdateTimeStampParameter, false);
            WriteTimeStamp = helper.GetPartByName(WriteTimeStampParameter, false);
            CotVariableName = helper.GetPartByName(CotVariableNameParameter);
            CotVariableId = helper.GetPartByName(CotVariableIdParameter);
            QualityVariableName = helper.GetPartByName(QualityVariableNameParameter);
            QualityVariableId = helper.GetPartByName(QualityVariableIdParameter);
            FileNameVariableName = helper.GetPartByName(FileNameVariableNameParameter);
            FileNameVariableId = helper.GetPartByName(FileNameVariableIdParameter);

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Convert IEC60870_5_104DynTagSettings to a string. </summary>
        ///
        /// <returns>   A string that represents this object. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
			dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", ASDUTypeParameter, DynamicStringParser.CharAssign, (int)ASDUType);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", StartAddMonParameter, DynamicStringParser.CharAssign, StartAddMon);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", StartAddCtrlParameter, DynamicStringParser.CharAssign, StartAddCtrl);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", CmdQualifierParameter, DynamicStringParser.CharAssign, (int)CmdQualifier);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", ParamQualifierParameter, DynamicStringParser.CharAssign, (int)ParamQualifier);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", CmdActionParameter, DynamicStringParser.CharAssign, (int)CmdAction);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", UpdateTimeStampParameter, DynamicStringParser.CharAssign, UpdateTimeStamp);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", WriteTimeStampParameter, DynamicStringParser.CharAssign, WriteTimeStamp);

            if (CotVariableName != String.Empty)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", CotVariableNameParameter, DynamicStringParser.CharAssign, CotVariableName);
            }
            if (CotVariableId != String.Empty)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", CotVariableIdParameter, DynamicStringParser.CharAssign, CotVariableId);
            }
            if (QualityVariableName != String.Empty)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", QualityVariableNameParameter, DynamicStringParser.CharAssign, QualityVariableName);
            }
            if (QualityVariableId != String.Empty)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", QualityVariableIdParameter, DynamicStringParser.CharAssign, QualityVariableId);
            }
            if (FileNameVariableName != String.Empty)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", FileNameVariableNameParameter, DynamicStringParser.CharAssign, FileNameVariableName);
            }
            if (FileNameVariableId != String.Empty)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", FileNameVariableIdParameter, DynamicStringParser.CharAssign, FileNameVariableId);
            }
            return dynamicstring.ToString();
        }


        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (IEC60870_5_104Protocol.GetMaxJobSize() >= ByteSize);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Return the string to link a tag after, in target device memory, the tag prevtagdefinition.
        /// </summary>
        ///
        /// <param name="prevtagdefinition">    . </param>
        /// <param name="thistagdefinition">    . </param>
        ///
        /// <returns>   The next dynamic setting. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            bool bits = isProtocolBool();
            bool bytes = isProtocolByte();

            if (prevtagdefinition.DataType.IdType == IdType.Numeric)
            {
                switch ((uint)prevtagdefinition.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                        if(bits)
                            StartAddMon += (prevtagdefinition.ArrayDimension > 0 ? (UInt16)prevtagdefinition.ArrayDimension : (UInt16)1);
                        else if (bytes)
                            StartAddMon += (prevtagdefinition.ArrayDimension > 0 ? (UInt16)(prevtagdefinition.ArrayDimension / 8 + (prevtagdefinition.ArrayDimension % 8 > 0 ? 1 : 0)) : (UInt16)1);
                        else
                            StartAddMon += (prevtagdefinition.ArrayDimension > 0 ? (UInt16)(prevtagdefinition.ArrayDimension / 16 + (prevtagdefinition.ArrayDimension % 16 > 0 ? 1 : 0)) : (UInt16)1);
                        break;
                    case (uint)BuiltInType.SByte:
                    case (uint)BuiltInType.Byte:
                        if (bits)
                            StartAddMon += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 8);
                        else if (bytes)
                            StartAddMon += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension) : 1));
                        else
                            StartAddMon += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension / 2 + (prevtagdefinition.ArrayDimension % 2 > 0 ? 1: 0)) : 1));
                        break;
                    case (uint)BuiltInType.Int16:
                    case (uint)BuiltInType.UInt16:
                        if (bits)
                            StartAddMon += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 2 * 8);
                        else if (bytes)
                            StartAddMon += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension * 2) : 2));
                        else
                            StartAddMon += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension) : 1));
                        break;
                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                        if (bits)
                            StartAddMon += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 4 * 8);
                        else if (bytes)
                            StartAddMon += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension * 4) : 4));
                        else
                            StartAddMon += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension * 2) : 2));
                        break;
                    case (uint)BuiltInType.UInt64:
                    case (uint)BuiltInType.Int64:
                    case (uint)BuiltInType.Double:
                        if (bits)
                            StartAddMon += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 8 * 8);
                        else if (bytes)
                            StartAddMon += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension * 8) : 8));
                        else
                            StartAddMon += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension * 4) : 4));
                        break;
                }
            }

            return ToString();
        }
        public override UFUAModel.DataType getProtocolDataType()
        {
            return (IEC60870_5_104Protocol.DataType(ASDUType));
        }

        #endregion

        #region Properties

        /// <summary>   Type of the tag link. </summary>
        //private int _TagLinkType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Modes to access the tags. Override base property for simultaneous validation of
        /// "AreaType" property.
        /// </summary>
        ///
        /// <value> The type of the tag link. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //[Category("General")]
        //[Description("Link Type")]
        //public override int TagLinkType
        //{
        //    get { return _TagLinkType; }
        //    set
        //    {
        //        _TagLinkType = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("ASDUType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("OutputAtStartup"));
        //    }
        //}

        //private UFUAModel.DataType _VarType;
        //[Category("General")]
        //[Description("Variable Type")]
        //public override UFUAModel.DataType VarType
        //{
        //    get
        //    {
        //        return _VarType;
        //    }
        //    set
        //    {
        //        _VarType = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("ASDUType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
        //    }
        //}
        //private uint _ArrayDimension;
        //[Category("General")]
        //[Description("Array Dimension")]
        //public override uint ArrayDimension
        //{
        //    get
        //    {
        //        return _ArrayDimension;
        //    }
        //    set
        //    {
        //        _ArrayDimension = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("ASDUType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
        //    }
        //}

        /// <summary>   Number of element to exchange. </summary>
        //private int _ElementNumber;
        //[Category("General")]
        //[Description("Element Number")]
        //public override int ElementNumber
        //{
        //    get { return _ElementNumber; }
        //    set
        //    {
        //        _ElementNumber = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("ASDUType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //    }
        //}

        /// <summary>   ASDU Type. </summary>
        private ASDUSelectableTypes _ASDUType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Information Object Type. </summary>
        ///
        /// <value> ASDU Type. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Object Type")]
        [Description("ASDU Type")]
        public ASDUSelectableTypes ASDUType
        {
            get { return _ASDUType; }
            set
            {
                _ASDUType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TagLinkType"));
                OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
            }
        }

        /// <summary>   The start address. </summary>
        private UInt32 _StartAddMon;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   IEC60870_5_104 start address memory Property. </summary>
        ///
        /// <value> The start address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("start address")]
        public UInt32 StartAddMon
        {
            get { return _StartAddMon; }
            set
            {
                _StartAddMon = value;
            }
        }

        /// <summary>   Control IOA. </summary>
        private UInt32 _StartAddCtrl;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Information Object Address for Control Direction (leave 0 if only monitoring). </summary>
        ///
        /// <value> Control IOA. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Control IOA")]
        public UInt32 StartAddCtrl
        {
            get
            {
                return _StartAddCtrl;
            }
            set
            {
                _StartAddCtrl = value;
            }
        }

        /// <summary>   Command Qualifier. </summary>
        private CommandQualifiers _CmdQualifier;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Command Qualifier. </summary>
        ///
        /// <value> Command Qualifier. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Command Qualifier")]
        public CommandQualifiers CmdQualifier
        {
            get
            {
                return _CmdQualifier;
            }
            set
            {
                _CmdQualifier = value;
            }
        }

        /// <summary>   Parameter Qualifier. </summary>
        private ParamQualifiers _ParamQualifier;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Parameter Qualifier. </summary>
        ///
        /// <value> Parameter Qualifier. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Parameter Qualifier")]
        public ParamQualifiers ParamQualifier
        {
            get
            {
                return _ParamQualifier;
            }
            set
            {
                _ParamQualifier = value;
            }
        }

        /// <summary>   Command Action. </summary>
        private CommandActions _CmdAction;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Command Action. </summary>
        ///
        /// <value> Command Action. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Command Action")]
        public CommandActions CmdAction
        {
            get
            {
                return _CmdAction;
            }
            set
            {
                _CmdAction = value;
            }
        }

        /// <summary>   COT Variable. </summary>
        private string _CotVariableName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Optional parameter that specifies the name of the variable where the driver will store 
        ///             the ""Cause Of Transmission"" of the received data. </summary>
        ///
        /// <value> name of COT Variable. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("COT Variable Name")]
        public string CotVariableName
        {
            get
            {
                return _CotVariableName;
            }
            set
            {
                _CotVariableName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CotVariableName"));
            }
        }
        /// <summary>   Node ID of the COT variable. </summary>
        private string _CotVariableId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   COT Variable Node Id. </summary>
        ///
        /// <value> The Node Id of the COT Variable. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("COT Variable Node Id")]
        public string CotVariableId
        {
            get { return _CotVariableId; }
            set
            {
                _CotVariableId = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CotVariableId"));
            }
        }

        /// <summary>   Quality Variable. </summary>
        private string _QualityVariableName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Optional parameter that specifies the name of the variable where the driver will store 
        ///             the ""Quality Descriptor"" of the received data. </summary>
        ///
        /// <value> Quality Variable name. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Quality Variable Name")]
        public string QualityVariableName
        {
            get
            {
                return _QualityVariableName;
            }
            set
            {
                _QualityVariableName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("QualityVariableName"));
            }
        }
        /// <summary>   Node ID of the COT variable. </summary>
        private string _QualityVariableId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Quality Variable Node Id. </summary>
        ///
        /// <value> The Node Id of the Quality Variable. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("Quality Variable Node Id")]
        public string QualityVariableId
        {
            get { return _QualityVariableId; }
            set
            {
                _QualityVariableId = value;
                OnPropertyChanged(new PropertyChangedEventArgs("QualityVariableId"));
            }
        }

        /// <summary> Always update TimeStamp. </summary>
        private bool _UpdateTimeStamp;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Select if the driver have to update timestamp at every read. </summary>
        ///
        /// <value> Always update TimeStamp. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("UpdateTimeStamp")]
        public bool UpdateTimeStamp
        {
            get
            {
                return _UpdateTimeStamp;
            }
            set
            {
                _UpdateTimeStamp = value;
            }
        }

        /// <summary> Write TimeStamp. </summary>
        private bool _WriteTimeStamp;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Add local TimeStamp to writes on the device. </summary>
        ///
        /// <value> Write TimeStamp. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Write TimeStamp")]
        public bool WriteTimeStamp
        {
            get
            {
                return _WriteTimeStamp;
            }
            set
            {
                _WriteTimeStamp = value;
            }
        }

        /// <summary>   File Name Variable. </summary>
        private string _FileNameVariableName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Optional parameter that specifies the name of the variable that contains the name  
        ///             of the file to be uploaded. </summary>
        ///
        /// <value> File Name Variable name. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("File Name Variable Name")]
        public string FileNameVariableName
        {
            get
            {
                return _FileNameVariableName;
            }
            set
            {
                _FileNameVariableName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FileNameVariableName"));
            }
        }
        /// <summary>   Node ID of the COT variable. </summary>
        private string _FileNameVariableId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   File Name Variable Node Id. </summary>
        ///
        /// <value> The Node Id of the File Name Variable. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("File Name Variable Node Id")]
        public string FileNameVariableId
        {
            get { return _FileNameVariableId; }
            set
            {
                _FileNameVariableId = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FileNameVariableId"));
            }
        }

        #endregion

        #region IDataErrorInfo Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   IEC60870_5_104DynTagSettings property validation. </summary>
        ///
        /// <param name="propertyName"> . </param>
        ///
        /// <returns>   A String. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "ASDUType")
            {
                if (IEC60870_5_104Protocol.InvalidAreaTypeInput(_ASDUType, (LinkType)TagLinkType))
                    return string.Format(Properties.Resources.AreaTypeRequireInput, DriverCodeBase.Properties.Resources.LinkType_Input);
                if (IEC60870_5_104Protocol.InvalidAreaTypeOutput(_ASDUType, (LinkType)TagLinkType))
                    return string.Format(Properties.Resources.AreaTypeRequireOutput, DriverCodeBase.Properties.Resources.LinkType_InputOutput, DriverCodeBase.Properties.Resources.LinkType_UnconditionalOutput, DriverCodeBase.Properties.Resources.LinkType_ExceptionOutput);

                if (IEC60870_5_104Protocol.DataTypeIncompatible(_ASDUType, VarType))
                    return UFUAModel.Properties.Resources.DataTypeIncompatible;
            }
            else if (propertyName == "TagLinkType")
            {
                if (IEC60870_5_104Protocol.InvalidAreaTypeInput(_ASDUType, (LinkType)TagLinkType))
                    return string.Format(Properties.Resources.AreaTypeRequireInput, DriverCodeBase.Properties.Resources.LinkType_Input);
                if (IEC60870_5_104Protocol.InvalidAreaTypeOutput(_ASDUType, (LinkType)TagLinkType))
                    return string.Format(Properties.Resources.AreaTypeRequireOutput, DriverCodeBase.Properties.Resources.LinkType_InputOutput, DriverCodeBase.Properties.Resources.LinkType_UnconditionalOutput, DriverCodeBase.Properties.Resources.LinkType_ExceptionOutput);
            }
            else if (propertyName == "ArrayDimension" && ArrayDimension != 0)
            {
                return (UFUAModel.Properties.Resources.ErrorInvalidArray);
            }
            else if (propertyName == "StartAddMon")
            {
                if (StartAddMon > IEC60870_5_104Protocol.MAX_INFORMATION_OBJECT_ADDRESS ||
                    StartAddMon < IEC60870_5_104Protocol.MIN_INFORMATION_OBJECT_ADDRESS)
                    return string.Format(Properties.Resources.ObjectAddressOutOfRange, uint.MaxValue);
            }
            else if (propertyName == "StartAddCtrl")
            {
                if (StartAddCtrl > IEC60870_5_104Protocol.MAX_INFORMATION_OBJECT_ADDRESS ||
                    StartAddCtrl < IEC60870_5_104Protocol.MIN_INFORMATION_OBJECT_ADDRESS)
                    return string.Format(Properties.Resources.ObjectAddressOutOfRange, uint.MaxValue);
            }

            return null;
        }

        #endregion

        #region INotifyPropertyChanged Members

        protected override void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "TagLinkType":
                    OnPropertyChanged(new PropertyChangedEventArgs("ASDUType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("OutputAtStartup"));
                    break;
                case "VarType":
                    OnPropertyChanged(new PropertyChangedEventArgs("ASDUType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ArrayDimension":
                    OnPropertyChanged(new PropertyChangedEventArgs("ASDUType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ElementNumber":
                    OnPropertyChanged(new PropertyChangedEventArgs("ASDUType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    break;
            }
    }
    #endregion


}
}
