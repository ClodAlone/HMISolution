////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	DynTagSettings.cs
//
// summary:	Implements the dynamic tag settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase.Helpers;
using DriverCodeBase.Enumerators;
using System.ComponentModel;
using System.Reflection;
using DriverBaseInterfaces;
using Opc.Ua;
using DriverCodeBase.Extensions;

namespace DriverCodeBase
{
    /// <summary>   settings to link a tag with the target device. </summary>
    public class DynTagSettings : IDataErrorInfo, INotifyPropertyChanged
    {
        #region Constructors
        

        /// <summary>   Default constructor. </summary>
        public DynTagSettings()
        {
            _DriverName = DriverInfo.GetDriverName(this);
            StationName = String.Empty;
            DeviceType = DataType.Undefined;
            DeviceSize = 0;
            TagLinkType = (int) LinkType.InputOutput;

            MethodID = -1;//steve 150711
            ElementNumber = 0;
            // FOGBUGZ 9836
            VarType = (UFUAModel.DataType) (-1);
            ArrayDimension = 0;

            _ConditionalVariableName = String.Empty;
            _ConditionalVariableId = String.Empty;

            IsMethodSupported = false;

        }
        
        #endregion

        #region Static Members

        /// <summary>   The station parameter. </summary>
        private static readonly String StationParameter = "Station";
        /// <summary>   The device link parameter. </summary>
        private static readonly String DeviceLinkParameter = "LinkType";
        /// <summary>   The device type parameter. </summary>
        private static readonly String DeviceTypeParameter = "DeviceType";
        /// <summary>   The device size parameter. </summary>
        private static readonly String DeviceSizeParameter = "DeviceSize";
        /// <summary>   The swap bytes parameter. </summary>
        private static readonly String SwapBytesParameter = "SwapBytes";
        /// <summary>   The swap words parameter. </summary>
        private static readonly String SwapWordsParameter = "SwapWords";
        /// <summary>   The output at startup parameter. </summary>
        private static readonly String OutputAtStartupParameter = "OutputAtStartup";
        /// <summary>   The element number parameter. </summary>
        private static readonly String ElementNumberParameter = "ElementNumber";

        /// <summary>   The conditional variable name parameter. </summary>
        private static readonly String ConditionalVariableNameParameter = "CVName";
        /// <summary>   The conditional variable node id parameter. </summary>
        private static readonly String ConditionalVariableIdParameter = "CVId";

        /// <summary>   The Offset variable name parameter. </summary>
        private static readonly String OffsetVariableNameParameter = "OVName";
        /// <summary>   The Offset variable node id parameter. </summary>
        private static readonly String OffsetVariableIdParameter = "OVId";

        /// <summary>   steve 150711. </summary>
        private static readonly String MethodTypeParameter = "Method";

        #endregion

        #region Virtual Functions

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the DynTagSettings of base driver from string dynamicSettings. </summary>
        ///
        /// <param name="dynamicSettings">  . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void Parse(String dynamicSettings)
        {
            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            StationName = helper.GetPartByName(StationParameter);
            TagLinkType = (int)/*(LinkType)*/(helper.GetPartByName(DeviceLinkParameter, (int)LinkType.InputOutput));
            DeviceType = (DataType)(helper.GetPartByName(DeviceTypeParameter, (int)DataType.Undefined));
            DeviceSize = helper.GetPartByName(DeviceSizeParameter, (int)0);
            SwapBytes = helper.GetPartByName(SwapBytesParameter, false);
            SwapWords = helper.GetPartByName(SwapWordsParameter, false);
            OutputAtStartup = helper.GetPartByName(OutputAtStartupParameter, false);
            ElementNumber = helper.GetPartByName(ElementNumberParameter, 0);

            ConditionalVariableName = helper.GetPartByName(ConditionalVariableNameParameter);
            ConditionalVariableId = helper.GetPartByName(ConditionalVariableIdParameter);

            OffsetVariableName = helper.GetPartByName(OffsetVariableNameParameter);
            OffsetVariableId = helper.GetPartByName(OffsetVariableIdParameter);

            MethodID = helper.GetPartByName(MethodTypeParameter, -1);//steve 150711
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes the DynTagSettings of base driver from string dynamicSettings if is possible.
        /// </summary>
        ///
        /// <param name="dynamicSettings">  . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool TryParse(String dynamicSettings)
        {
            if (String.IsNullOrEmpty(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            if (helper.GetDriverName() == null)
                return false;
            if (helper.GetDriverName().ToLower() != /*DriverInfo.GetDriverName()*/DriverName.ToLower())
                return false;
            
            // required parameter
            StationName = helper.GetPartByName(StationParameter);
            if (String.IsNullOrEmpty(StationName))
                return false;
                        
            // optional parameters
            DeviceType = (DataType)(helper.GetPartByName(DeviceTypeParameter, (int)DataType.Undefined));
            DeviceSize = helper.GetPartByName(DeviceSizeParameter, (int)0);
            TagLinkType = (int)/*(LinkType)*/(helper.GetPartByName(DeviceLinkParameter, (int)LinkType.InputOutput));

            SwapBytes = helper.GetPartByName(SwapBytesParameter, false);
            SwapWords = helper.GetPartByName(SwapWordsParameter, false);
            OutputAtStartup = helper.GetPartByName(OutputAtStartupParameter, false);

            ElementNumber = helper.GetPartByName(ElementNumberParameter, 0);
 
            ConditionalVariableName = helper.GetPartByName(ConditionalVariableNameParameter);
            ConditionalVariableId = helper.GetPartByName(ConditionalVariableIdParameter);

            OffsetVariableName = helper.GetPartByName(OffsetVariableNameParameter);
            OffsetVariableId = helper.GetPartByName(OffsetVariableIdParameter);

            MethodID = helper.GetPartByName(MethodTypeParameter, -1);//steve 150711

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   convert DynTagSettings to a string. </summary>
        ///
        /// <returns>   A string that represents this object. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual string ToString()
        {
            var dynamicstring = new StringBuilder(DriverName/*DriverInfo.GetDriverName()*/);
            dynamicstring.Append(DynamicStringParser.CharDriver);
            dynamicstring.AppendFormat("{0}{1}{2}", StationParameter, DynamicStringParser.CharAssign, StationName);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", DeviceLinkParameter, DynamicStringParser.CharAssign, (int)TagLinkType);
            if (DeviceType != DataType.Undefined)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", DeviceTypeParameter, DynamicStringParser.CharAssign, (int)DeviceType);
            }
            if (DeviceSize > 0)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", DeviceSizeParameter, DynamicStringParser.CharAssign, DeviceSize);
            }
            if (SwapBytes)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", SwapBytesParameter, DynamicStringParser.CharAssign, SwapBytes);
            }
            if (SwapWords)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", SwapWordsParameter, DynamicStringParser.CharAssign, SwapWords);
            }
            if (OutputAtStartup)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", OutputAtStartupParameter, DynamicStringParser.CharAssign, OutputAtStartup);
            }
            if(ElementNumber > 0)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", ElementNumberParameter, DynamicStringParser.CharAssign, ElementNumber);
            }

            if (!String.IsNullOrEmpty(ConditionalVariableName) && !String.IsNullOrEmpty(ConditionalVariableId))
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", ConditionalVariableNameParameter, DynamicStringParser.CharAssign, ConditionalVariableName);
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", ConditionalVariableIdParameter, DynamicStringParser.CharAssign, ConditionalVariableId);
            }

            if (!String.IsNullOrEmpty(OffsetVariableName) && !String.IsNullOrEmpty(OffsetVariableId))
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", OffsetVariableNameParameter, DynamicStringParser.CharAssign, OffsetVariableName);
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", OffsetVariableIdParameter, DynamicStringParser.CharAssign, OffsetVariableId);
            }

            if (MethodID >= 0)//steve 150711
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", MethodTypeParameter, DynamicStringParser.CharAssign, MethodID);
            }

            return dynamicstring.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the next dynamic setting. </summary>
        ///
        /// <param name="prevtagdefinition" type="TagDefinition">   The prevtagdefinition. </param>
        /// <param name="thistagdefinition" type="TagDefinition">   The thistagdefinition. </param>
        ///
        /// <returns>   The next dynamic setting. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            return string.Empty;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the first dynamic setting. </summary>
        ///
        /// <param name="tag" type="Tag">                           The tag. </param>
        /// <param name="thistagdefinition" type="TagDefinition">   The thistagdefinition. </param>
        ///
        /// <returns>   The first dynamic setting. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual string GetFirstDynSetting(Tag tag, TagDefinition thistagdefinition)
        {
            TryParse(tag.TagNode.DynamicSettings);
            return tag.TagNode.DynamicSettings;
        }
        public virtual UFUAModel.DataType getProtocolDataType()
        {
            return _VarType;
        }
        public virtual bool isTagByteSizeOk(uint ByteSize)
        {
            return true;
        }

        #endregion

        #region Functions
        public virtual List<string> GetParamNames()
        {
            return new List<string>() {StationParameter, DeviceLinkParameter, DeviceTypeParameter , DeviceSizeParameter,
                SwapBytesParameter, SwapWordsParameter, OutputAtStartupParameter, ElementNumberParameter,
                ConditionalVariableNameParameter, ConditionalVariableIdParameter, OffsetVariableNameParameter,
                OffsetVariableIdParameter, MethodTypeParameter
            };
        }
        public uint GetProtocolDataByteSize()
        {
            return (GetProtocolDataBitSize() + 7) / 8;
        }
        public uint GetProtocolDataBitSize()
        {
            return GetDataTypeBitSize(getProtocolDataType());
        }
        public static uint GetDataTypeBitSize(UFUAModel.DataType DataType)
        {
            switch (DataType)
            {
                case UFUAModel.DataType.Boolean:
                    return 1;
                case UFUAModel.DataType.SByte:
                case UFUAModel.DataType.Byte:
                    return 8;
                case UFUAModel.DataType.Int16:
                case UFUAModel.DataType.UInt16:
                    return 16;
                case UFUAModel.DataType.Float:
                case UFUAModel.DataType.UInt32:
                case UFUAModel.DataType.Int32:
                    return 32;
                case UFUAModel.DataType.UInt64:
                case UFUAModel.DataType.Int64:
                case UFUAModel.DataType.Double:
                    return 64;
                default:
                    return 8;
            }
        }

        public bool ProtocolDataSizeEqual(UFUAModel.DataType DataType)
        {
            if (DataType == UFUAModel.DataType.String)
                return true;
            return GetProtocolDataBitSize() == GetDataTypeBitSize(DataType);
        }
        public bool ProtocolDataSizeBig(UFUAModel.DataType DataType)
        {
            switch (getProtocolDataType())
            {
                case UFUAModel.DataType.Float:
                case UFUAModel.DataType.Double:
                case UFUAModel.DataType.String:
                    return false;
            }
            return GetProtocolDataBitSize() > GetDataTypeBitSize(DataType);
        }
        public bool ProtocolDataSizeSmall(UFUAModel.DataType DataType)
        {
            if (DataType == UFUAModel.DataType.String)
                return false;
            return GetProtocolDataBitSize() < GetDataTypeBitSize(DataType);
        }
        public bool isProtocolBool()
        {
            return (GetProtocolDataBitSize() < 8);
        }

        public bool isProtocolByte()
        {
            return (GetProtocolDataBitSize() == 8);
        }
        public virtual string ProtocolDataSizeValidation(UFUAModel.DataType DataType)
        {
            if (ProtocolDataSizeBig(DataType))
            {
                if (ElementNumber > GetProtocolDataBitSize() / GetDataTypeBitSize(DataType) - 1)
                    return UFUAModel.Properties.Resources.DataTypeIncompatible;
                //if (ArrayDimension > 0)
                //    return UFUAModel.Properties.Resources.ArrayInvalid;
                if (!isTagByteSizeOk((GetProtocolDataBitSize() * (ArrayDimension == 0 ? 1 : ArrayDimension) + 7) / 8))
                    return UFUAModel.Properties.Resources.TagOverSize;
                if (TagLinkType == (int)LinkType.ExceptionOutput || TagLinkType == (int)LinkType.UnconditionalOutput)
                    return DriverCodeBase.Properties.Resources.JobTypeInvalid;
            }
            else
            {
                //if (DataType != UFUAModel.DataType.Boolean && GetProtocolDataBitSize() < 8 && ArrayDimension > 0)
                //    if (ArrayDimension > 0)
                //        return UFUAModel.Properties.Resources.ArrayInvalid;
                if (!isTagByteSizeOk(((ElementNumber == 0 ? GetDataTypeBitSize(DataType) : GetProtocolDataBitSize()) * (ArrayDimension == 0 ? 1 : ArrayDimension) + 7) / 8))
                    return UFUAModel.Properties.Resources.TagOverSize;
            }
            
            return null;

        }
        #endregion
 
        #region Properties
        /// <summary>   Name of the driver. </summary>
        private string _DriverName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the driver. </summary>
        ///
        /// <value> The name of the driver. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string DriverName
        {
            get
            { return _DriverName;
            }
            set
            {
                _DriverName = value;
                OnPropertyChanged("DriverName");
            }
        }

        /// <summary>   Name of the station. </summary>
        private string _StationName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Station Name. </summary>
        ///
        /// <value> The name of the station. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("Station Name")]
        public string StationName
        {
            get
            {
                return _StationName;
            }
            set
            {
                _StationName = value;
                OnPropertyChanged("StationName");
            }
        }

        /// <summary>   Type of the device. </summary>
        private DataType _DeviceType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Device Type. </summary>
        ///
        /// <value> The type of the device. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("Device Type")]
        public DataType DeviceType
        {
            get
            {
                return _DeviceType;
            }
            set
            {
                _DeviceType = value;
                OnPropertyChanged("DeviceType");
            }
        }

        /// <summary>   Size of the device. </summary>
        private int _DeviceSize;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Device Size. </summary>
        ///
        /// <value> The size of the device. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("Device Size")]
        public int DeviceSize
        {
            get
            {
                return _DeviceSize;
            }
            set
            {
                _DeviceSize = value;
                OnPropertyChanged("DeviceSize");
            }
        }

        /// <summary>   Type of the tag link. </summary>
        private int/*LinkType*/ _TagLinkType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   modes to access the tags property. </summary>
        ///
        /// <value> The type of the tag link. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("Link Type")]
        public int/*LinkType*/ TagLinkType
        {
            get
            {
                return _TagLinkType;
            }
            set
            {
                _TagLinkType = value;
				OnPropertyChanged("TagLinkType");
                OnPropertyChanged("OutputAtStartup");
            }
        }

        /// <summary>   true to swap bytes. </summary>
        private bool _SwapBytes;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Swap Bytes. </summary>
        ///
        /// <value> true if swap bytes, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("Swap Bytes")]
        public bool SwapBytes
        {
            get
            {
                return _SwapBytes;
            }
            set
            {
                _SwapBytes = value;
                OnPropertyChanged("SwapBytes");
            }
        }
        /// <summary>   true to swap words. </summary>
        private bool _SwapWords;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Swap Words. </summary>
        ///
        /// <value> true if swap words, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("Swap Words")]
        public bool SwapWords
        {
            get
            {
                return _SwapWords;
            }
            set
            {
                _SwapWords = value;
                OnPropertyChanged("SwapWords");
            }
        }
        /// <summary>   true to output at startup. </summary>
        private bool _OutputAtStartup;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Output at Startup. </summary>
        ///
        /// <value> true if output at startup, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("Output at Startup")]
        public bool OutputAtStartup
        {
            get
            {
                return _OutputAtStartup;
            }
            set
            {
                _OutputAtStartup = value;
                OnPropertyChanged("OutputAtStartup");
				OnPropertyChanged("TagLinkType");
            }
        }
        /// <summary>   tag method identifier. </summary>
        private int _MethodID;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Method identifier. </summary>
        ///
        /// <value> The identifier of the method. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("Method identifier")]
        public int MethodID
        {
            get { return _MethodID; }
            set
            {
                _MethodID = value;
                OnPropertyChanged("MethodID");
            }
        }
        /// <summary>   Tag dataType. </summary>
        private UFUAModel.DataType _VarType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Variable Type. </summary>
        ///
        /// <value> The type of the variable. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("Variable Type")]
        public UFUAModel.DataType VarType
        {
            get
            {
                return _VarType;
            }
            set
            {
                _VarType = value;
                OnPropertyChanged("VarType");
                OnPropertyChanged("ArrayDimension");
                OnPropertyChanged("ElementNumber");
            }
        }
        /// <summary>   is method tag. </summary>
        private bool _IsMethod;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Is Method. </summary>
        ///
        /// <value> true if this object is method, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("Is Method")]
        public bool IsMethod
        {
            get
            {
                return _IsMethod;
            }
            set
            {
                _IsMethod = value;
                OnPropertyChanged("IsMethod");
            }
        }
        /// <summary>   is object tag. </summary>
        private bool _IsObjectType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Is Object. </summary>
        ///
        /// <value> true if this object is method, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("Is Object Type")]
        public bool IsObjectType
        {
            get
            {
                return _IsObjectType;
            }
            set
            {
                _IsObjectType = value;
                OnPropertyChanged("IsObjectType");
            }
        }
        /// <summary>   FOGBUGZ 9836. </summary>
        private uint _ArrayDimension;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   tag array dimension. </summary>
        ///
        /// <value> The array dimension. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint ArrayDimension
        {
            get
            {
                return _ArrayDimension;
            }
            set
            {
                _ArrayDimension = value;
                OnPropertyChanged("ArrayDimension");
                OnPropertyChanged("VarType");
                OnPropertyChanged("ElementNumber");
            }
        }

        /// <summary>   Number of element to exchange. </summary>
        private int _ElementNumber;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the number of element to exchange. </summary>
        ///
        /// <value> Number of element to exchange. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("Element Number")]
        public int ElementNumber
        {
            get
            {
                return _ElementNumber;
            }
            set
            {
                _ElementNumber = value;
                OnPropertyChanged("ElementNumber");
                OnPropertyChanged("VarType");
                OnPropertyChanged("ArrayDimension");
            }
        }

        /// <summary>   Name of the conditional variable. </summary>
        private string _ConditionalVariableName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Conditional Variable Name. </summary>
        ///
        /// <value> The name of the conditional value. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("Conditional Variable Name")]
        public virtual string ConditionalVariableName
        {
            get { return _ConditionalVariableName; }
            set
            {
                _ConditionalVariableName = value;
                OnPropertyChanged("ConditionalVariableName");
            }
        }

        /// <summary>   Node ID of the conditional variable. </summary>
        private string _ConditionalVariableId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Conditional Variable Node Id. </summary>
        ///
        /// <value> The Node Id of the conditional value. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("Conditional Variable Node Id")]
        public virtual string ConditionalVariableId
        {
            get { return _ConditionalVariableId; }
            set
            {
                _ConditionalVariableId = value;
                OnPropertyChanged("ConditionalVariableId");
            }
        }

        private string _OffsetVariableName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Offset Variable Name. </summary>
        ///
        /// <value> The name of the conditional value. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("Offset Variable Name")]
        public string OffsetVariableName
        {
            get { return _OffsetVariableName; }
            set
            {
                _OffsetVariableName = value;
                OnPropertyChanged("OffsetVariableName");
            }
        }

        /// <summary>   Node ID of the conditional variable. </summary>
        private string _OffsetVariableId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Offset Variable Node Id. </summary>
        ///
        /// <value> The Node Id of the conditional value. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("General")]
        [Description("Offset Variable Node Id")]
        public string OffsetVariableId
        {
            get { return _OffsetVariableId; }
            set
            {
                _OffsetVariableId = value;
                OnPropertyChanged("OffsetVariableId");
            }
        }

        /// <summary>  Method functionality support</summary>
        private bool _IsMethodSupported;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Method functionality support. </summary>
        ///
        /// <value> Method functionality support. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsMethodSupported
        {
            get { return _IsMethodSupported; }
            protected set
            {
                _IsMethodSupported = value;
                OnPropertyChanged("IsMethodSupported");
            }
        }
        #endregion

        #region IDataErrorInfo Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets an error message indicating what is wrong with this object. </summary>
        ///
        /// <value>
        /// An error message indicating what is wrong with this object. The default is an empty string
        /// ("").
        /// </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Indexer to get items within this collection using array index syntax. </summary>
        ///
        /// <param name="propertyName" type="string">   Name of the property. </param>
        ///
        /// <returns>   The indexed item. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string this[string propertyName]
        {
            get
            {                
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Performs the validation action. </summary>
        ///
        /// <param name="propertyName" type="String">   Name of the property. </param>
        ///
        /// <returns>   A String. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual String PerformValidation(String propertyName)
        {
            // if method is not supported and method dynamic link validation is query, report error
            if (!IsMethodSupported && IsMethod)
                return DriverCodeBase.Properties.Resources.MethodNotSupported;

            switch (propertyName)
            {
                case "StationName":
                    if (string.IsNullOrEmpty(StationName))
                    {
                        return Properties.Resources.StationNameNotNull;
                    }
                    break;
                case "TagLinkType":
                    {
                        LinkType lt = (LinkType)TagLinkType;
                        if (lt/*TagLinkType*/ != LinkType.ExceptionOutput && lt/*TagLinkType*/ != LinkType.Input &&
                           lt/*TagLinkType*/ != LinkType.InputOutput && lt/*TagLinkType*/ != LinkType.UnconditionalOutput)
                            return Properties.Resources.JobTypeInvalid;
                        if (OutputAtStartup && lt == LinkType.Input)
                            return (DriverCodeBase.Properties.Resources.OutputAtStartupNotSupported);
                        if (VarType >= UFUAModel.DataType.Boolean)
                        {
                            if (ProtocolDataSizeValidation(VarType) == DriverCodeBase.Properties.Resources.JobTypeInvalid)
                                return (DriverCodeBase.Properties.Resources.JobTypeInvalid);
                        }
                    }
                    break;

                case "OutputAtStartup":
                    {
                        LinkType lt = (LinkType)TagLinkType;
                        if (OutputAtStartup && lt == LinkType.Input)
                            return (DriverCodeBase.Properties.Resources.OutputAtStartupNotSupported);
                    }
                    break;
                // FOGBUGZ 9836
                case "SwapBytes":
                    if (SwapBytes == true)
                    {
                        if ((VarType == UFUAModel.DataType.Boolean) ||
                            ((VarType == UFUAModel.DataType.Byte) && (ArrayDimension < 2)) ||
                            ((VarType == UFUAModel.DataType.SByte) && (ArrayDimension < 2)))
                        {
                            return Properties.Resources.SwapBytesNotAdmitted;
                        }
                    }
                    break;
                case "SwapWords":
                    if (SwapWords == true)
                    {
                        if ((VarType == UFUAModel.DataType.Boolean) ||
                            ((VarType == UFUAModel.DataType.Byte) && (ArrayDimension < 4)) ||
                            ((VarType == UFUAModel.DataType.SByte) && (ArrayDimension < 4)) ||
                            ((VarType == UFUAModel.DataType.Int16) && (ArrayDimension < 2)) ||
                            ((VarType == UFUAModel.DataType.UInt16) && (ArrayDimension < 2)))
                        {
                            return Properties.Resources.SwapWordsNotAdmitted;
                        }
                    }
                    break;
                case "MethodID":
                    // try to validate a method dynamic link
                    if (IsMethod && MethodID == -1)
                        return Properties.Resources.MethodMissing;

                    // try to validate a standard tag's dynamic link
                    if (!IsMethod && MethodID != -1)
                        return Properties.Resources.MethodValidatedAsTag;
                    break;

                case "ArrayDimension":
                    string Aux = ProtocolDataSizeValidation(VarType);
                    if (!String.IsNullOrEmpty(Aux))
                        return Aux;
                    //if (Aux == UFUAModel.Properties.Resources.ErrorInvalidBoleanArray ||
                    //    Aux == UFUAModel.Properties.Resources.ArraysOfStringsAreInvalid ||
                    //    Aux == UFUAModel.Properties.Resources.ErroorProtocolSetIncompatibleWithArrays ||
                    //    Aux == UFUAModel.Properties.Resources.ErrorInvalidArray ||
                    //    Aux == UFUAModel.Properties.Resources.TagOverSize)
                    //    return Aux;
                    break;
                case "ConditionalVariableId":
                case "ConditionalVariableName":
                    {
                        Opc.Ua.NodeId nodeId;
                        if (!NodeIdHelper.TryParse(ConditionalVariableId, out nodeId))
                            return Properties.Resources.ConditionalVariableINodeIdInvalid;
                        break;
                    }
                case "OffsetVariableId":
                case "OffsetVariableName":
                    {
                        Opc.Ua.NodeId nodeId;
                        if (!NodeIdHelper.TryParse(OffsetVariableId, out nodeId))
                            return Properties.Resources.OffsetVariableINodeIdInvalid;
                        break;
                    }
                case "DataType":
                case "ElementNumber":
                    if (VarType == (UFUAModel.DataType)(-1))
                        return null;
                    return ProtocolDataSizeValidation(VarType);

                case "IsMethodSupported":
                    if (IsMethod && !IsMethodSupported)
                    {
                        return Properties.Resources.MethodNotSupported;
                    }
                    break;
            }
            return null;
        }

        #endregion


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Raises the property changed event. </summary>
        ///
        /// <param name="e" type="PropertyChangedEventArgs">    Event information to send to registered
        ///                                                     event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }
        #region INotifyPropertyChanged Members

        /// <summary>   Occurs when a property value changes. </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion
    }


}
