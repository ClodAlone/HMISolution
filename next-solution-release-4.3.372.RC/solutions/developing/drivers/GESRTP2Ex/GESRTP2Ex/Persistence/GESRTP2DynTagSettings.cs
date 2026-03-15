////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	GESRTP2DynTagSettings.cs
//
// summary:	Implements the driver GESRTP2 dynamic tag settings class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Helpers;
using DriverCodeBaseEx.Enumerators;
using System.ComponentModel;
using DriverBaseInterfaces;
using Opc.Ua;
using DevExpress.Xpo;

namespace GESRTP2
{
    /// <summary>   Dynamic tag settings of the GESRTP2 driver. </summary>
    public sealed class GESRTP2DynTagSettings : DynTagSettings
    {
        #region Constructors

        /// <summary>   Initializes the GESRTP2DynTagSettings. </summary>
        public GESRTP2DynTagSettings()
            : base()
        {
            _AreaType = AreaTypes.RegisterWords_R;
            _StartAddress = 1;
            _StringLength = GESRTP2Protocol.DATA_AREA_DEFAULT_STRING_SIZE;
            _StructMixed = false;
            _SymbolicAddress = string.Empty;
        }

        public GESRTP2DynTagSettings(bool structMixed)
            : base()
        {
            _AreaType = AreaTypes.RegisterWords_R;
            _StartAddress = 1;
            _StringLength = GESRTP2Protocol.DATA_AREA_DEFAULT_STRING_SIZE;
            _StructMixed = structMixed;
            _SymbolicAddress = string.Empty;
        }

        #endregion

        #region Static Members

        /// <summary>   The function code parameter. </summary>
        private static readonly String AreaTypeParameter = "AT";
        /// <summary>   The start address parameter. </summary>
        private static readonly String StartAddressParameter = "SA";
        /// <summary>   The String Length Parameter. </summary>
        private static readonly String StringLengthParameter = "DL";
        /// <summary>   The Struct Mixed flag parameter. </summary>
        private static readonly String StructMixedParameter = "SM";
        /// <summary>   The symbolic address parameter. </summary>
        private static readonly String SymbolicAddressParameter = "SYA";

        #endregion

        #region Override Functions

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes the GESRTP2DynTagSettings from string dynamicSettings.
        /// </summary>
        ///
        /// <param name="dynamicSettings">  . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            SymbolicAddress = helper.GetPartByName(SymbolicAddressParameter);
            if (!string.IsNullOrEmpty(SymbolicAddress))
            {
                AreaType = AreaTypes.Symbolic;
                StringLength = GESRTP2Protocol.SYMBOLIC_DEFAULT_STRING_SIZE;
            }
            else
            { 
                AreaType = (AreaTypes)(helper.GetPartByName(AreaTypeParameter, (UInt16)AreaTypes.RegisterWords_R));
                StartAddress = helper.GetPartByName(StartAddressParameter, (UInt16)0);
                StringLength = helper.GetPartByName(StringLengthParameter, (uint)GESRTP2Protocol.DATA_AREA_DEFAULT_STRING_SIZE);
                StructMixed = helper.GetPartByName(StructMixedParameter, (bool)false);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes the GESRTP2DynTagSettings from string dynamicSettings if is possible.
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

            SymbolicAddress = helper.GetPartByName(SymbolicAddressParameter);
            if (!string.IsNullOrEmpty(SymbolicAddress))
            {
                AreaType = AreaTypes.Symbolic;
                StringLength = GESRTP2Protocol.SYMBOLIC_DEFAULT_STRING_SIZE;
            }
            else
            {
                if (String.IsNullOrEmpty(helper.GetPartByName(AreaTypeParameter)))
                    return false;
                AreaType = (AreaTypes)(helper.GetPartByName(AreaTypeParameter, (UInt16)AreaTypes.RegisterWords_R));

                // optional parameters
                StartAddress = helper.GetPartByName(StartAddressParameter, (UInt16)0);                
                StructMixed = helper.GetPartByName(StructMixedParameter, (bool)false);
                StringLength = helper.GetPartByName(StringLengthParameter, (uint)(uint)GESRTP2Protocol.DATA_AREA_DEFAULT_STRING_SIZE);
            }            

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Convert GESRTP2DynTagSettings to a string. </summary>
        ///
        /// <returns>   A string that represents this object. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());			
            if (AreaType == AreaTypes.Symbolic)
            {
                AreaType = AreaTypes.Symbolic;
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", SymbolicAddressParameter, DynamicStringParser.CharAssign, SymbolicAddress);                
            }
            else
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", AreaTypeParameter, DynamicStringParser.CharAssign, (int)AreaType);
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", StartAddressParameter, DynamicStringParser.CharAssign, StartAddress);
                if (StructMixed)
                {
                    dynamicstring.Append(DynamicStringParser.CharSep);
                    dynamicstring.AppendFormat("{0}{1}{2}", StructMixedParameter, DynamicStringParser.CharAssign, StructMixed);
                }
            }
            if (AreaType != AreaTypes.Symbolic && VarType == UFUAModel.DataType.String)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", StringLengthParameter, DynamicStringParser.CharAssign, (uint)StringLength);
            }            
            return dynamicstring.ToString();
        }


        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (GESRTP2Protocol.MAX_DATA_BYTES >= ByteSize);
        }

        public override string GetFirstDynSetting(Tag tag, TagDefinition thistagdefinition)
        {
            if (AreaType == AreaTypes.Symbolic)
            {
                TryParse(tag.TagNode.DynamicSettings);
                return GetNodeDynSetting(thistagdefinition);
            }
            else
            {
                return base.GetFirstDynSetting(tag, thistagdefinition);
            }
        }

        string GetNodeDynSetting(TagDefinition thistagdefinition)
        {
            string memABAddress = SymbolicAddress;
            string Tree = GESRTP2Protocol.GetNodeTree(thistagdefinition.NodeId, thistagdefinition.Name);
            SymbolicAddress += ("." + Tree.Replace('/', '.'));
            string dynsettings = ToString();
            SymbolicAddress = memABAddress;
            return dynsettings;
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
            if (AreaType == AreaTypes.Symbolic)
            {
                return GetNodeDynSetting(thistagdefinition);
            }
            else
            {
                bool bits = isProtocolBool();
                bool bytes = isProtocolByte();

                if (prevtagdefinition.DataType.IdType == IdType.Numeric)
                {
                    switch ((uint)prevtagdefinition.DataType.Identifier)
                    {
                        case (uint)BuiltInType.Boolean:
                            if (bits)
                                StartAddress += (prevtagdefinition.ArrayDimension > 0 ? (UInt16)prevtagdefinition.ArrayDimension : (UInt16)1);
                            else if (bytes)
                                StartAddress += (prevtagdefinition.ArrayDimension > 0 ? (UInt16)(prevtagdefinition.ArrayDimension / 8 + (prevtagdefinition.ArrayDimension % 8 > 0 ? 1 : 0)) : (UInt16)1);
                            else
                                StartAddress += (prevtagdefinition.ArrayDimension > 0 ? (UInt16)(prevtagdefinition.ArrayDimension / 16 + (prevtagdefinition.ArrayDimension % 16 > 0 ? 1 : 0)) : (UInt16)1);
                            break;
                        case (uint)BuiltInType.SByte:
                        case (uint)BuiltInType.Byte:
                            if (bits)
                                StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 8);
                            else if (bytes)
                                StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension) : 1));
                            else
                                StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension / 2 + (prevtagdefinition.ArrayDimension % 2 > 0 ? 1 : 0)) : 1));
                            break;
                        case (uint)BuiltInType.Int16:
                        case (uint)BuiltInType.UInt16:
                            if (bits)
                                StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 2 * 8);
                            else if (bytes)
                                StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension * 2) : 2));
                            else
                                StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension) : 1));
                            break;
                        case (uint)BuiltInType.Float:
                        case (uint)BuiltInType.UInt32:
                        case (uint)BuiltInType.Int32:
                            if (bits)
                                StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 4 * 8);
                            else if (bytes)
                                StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension * 4) : 4));
                            else
                                StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension * 2) : 2));
                            break;
                        case (uint)BuiltInType.UInt64:
                        case (uint)BuiltInType.Int64:
                        case (uint)BuiltInType.Double:
                            if (bits)
                                StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 8 * 8);
                            else if (bytes)
                                StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension * 8) : 8));
                            else
                                StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension * 4) : 4));
                            break;
                        case (uint)BuiltInType.String:
                            if (bits)
                                StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 32 * 8);
                            else if (bytes)
                                StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension * 32) : 32));
                            else
                                StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension * 16) : 16));
                            break;
                    }
                }

                return ToString();
            }
        }
        public override UFUAModel.DataType getProtocolDataType()
        {
            return (GESRTP2Protocol.GetUFUAModelDataTypeFromBuiltType(GESRTP2Protocol.DataType(AreaType)));
        }

        #endregion

        #region Methods
        public static string AddStructMixedParameter(string dynamicSettings)
        {
            StringBuilder newDynamicSettings = new StringBuilder(dynamicSettings);
            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);
            if (!helper.GetPartByName(StructMixedParameter, (bool)false))
            {
                newDynamicSettings.Append(DynamicStringParser.CharSep);
                newDynamicSettings.AppendFormat("{0}{1}{2}", StructMixedParameter, DynamicStringParser.CharAssign, true);
            }

            return newDynamicSettings.ToString();
        }

        public bool ProtocolDataSizeBig(Tag tag)
        {
            switch ((uint)tag.TagNode.DataType.Identifier)
            {
                case (uint)BuiltInType.Float:
                case (uint)BuiltInType.Double:
                case (uint)BuiltInType.String:
                    return false;
            }
            return GetProtocolDataBitSize() > CommJob.GetDataTypeBitSize((uint)tag.TagNode.DataType.Identifier);
        }

        #endregion

        #region Properties        
        /// <summary>   The Area Type. </summary>
        private AreaTypes _AreaType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   GESRTP2 memory area Property. </summary>
        ///
        /// <value> The Area Type. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Data Area")]
        public AreaTypes AreaType
        {
            get { return _AreaType; }
            set
            {
                _AreaType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TagLinkType"));
                OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                OnPropertyChanged(new PropertyChangedEventArgs("StartAddress"));
                OnPropertyChanged(new PropertyChangedEventArgs("SymbolicAddress"));
            }
        }

        /// <summary>   The start address. </summary>
        private UInt16 _StartAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   GESRTP2 start address memory Property. </summary>
        ///
        /// <value> The start address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Start Address")]
        public UInt16 StartAddress
        {
            get { return _StartAddress; }
            set
            {
                _StartAddress = value;
            }
        }

        /// <summary>   The symbolic address. </summary>
        private string _SymbolicAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   GESRTP2 symbolic address Property. </summary>
        ///
        /// <value> The symbolic address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("Symbolic Address")]
        [Size(SizeAttribute.Unlimited)]
        public string SymbolicAddress
        {
            get { return _SymbolicAddress; }
            set
            {
                _SymbolicAddress = value;
            }
        }

        /// <summary>   The String Length. </summary>
        private uint _StringLength;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   GESRTP2 String Length Property. </summary>
        ///
        /// <value> The String Length. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        [Category("Device Data")]
        [Description("String Length")]
        public uint StringLength
        {
            get { return _StringLength; }
            set { 
                    _StringLength = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("StationName"));
                }
        }

        /// <summary>
        /// Dictionary of station _stationSettingList<Neme, Ogbject Station> 
        /// </summary>
        private Dictionary<string, GESRTP2StationSettings> _stationSettingList;
        public Dictionary<string, GESRTP2StationSettings> stationSettingList
        {
            get { return _stationSettingList; }
            set
            {
                _stationSettingList = value;
            }
        }

        private bool _StructMixed;
        [Category("General")]
        [Description("Struct Mixed")]
        public bool StructMixed
        {
            get
            {
                return _StructMixed;
            }
            set
            {
                _StructMixed = value;
            }
        }

        #endregion

        #region IDataErrorInfo Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   GESRTP2DynTagSettings property validation. </summary>
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

            switch (propertyName)
            {
                case "AreaType":
                    if (_AreaType != AreaTypes.Symbolic)
                    {
                        if (GESRTP2Protocol.InvalidAreaTypeLinkType(_AreaType, (LinkType)TagLinkType))
                            return string.Format(Properties.Resources.AreaTypeRequireInput, DriverCodeBaseEx.Properties.Resources.LinkType_Input);

                        if (GESRTP2Protocol.InvalidAreaDataType(_AreaType, VarType))
                            return UFUAModel.Properties.Resources.DataTypeIncompatible;
                    
                        if ((uint)VarType != unchecked((uint)(-1)))
                            return ProtocolDataSizeValidation(VarType);
                    }
                    
                    if (!TagSizeCheck())
                        return Properties.Resources.TagSizeOutOfRange;
                    
                    break;
                case "TagLinkType":
                    if (_AreaType != AreaTypes.Symbolic)
                    {
                        if (GESRTP2Protocol.InvalidAreaTypeLinkType(_AreaType, (LinkType)TagLinkType))
                            return string.Format(Properties.Resources.AreaTypeRequireInput, DriverCodeBaseEx.Properties.Resources.LinkType_Input);
                    }
                    break;
                case "StartAddress":
                    if (_AreaType != AreaTypes.Symbolic)
                    {
                        if (_StartAddress == 0)
                            return Properties.Resources.InvalidZeroStartAddress;
                    }
                    break;
                case "SymbolicAddress":
                    if (AreaType == AreaTypes.Symbolic)
                    {
                        if (String.IsNullOrWhiteSpace(_SymbolicAddress))
                            return Properties.Resources.InvalidSymbolicAddress;
                    }
                    break;
                case "StringLength":
                    if (AreaType != AreaTypes.Symbolic)
                    {
                        if (InvalidStringSize())
                            return Properties.Resources.StringSizeOutOfRange;
                    }
                    break;
                case "StationName":
                    if (!TagSizeCheck())
                        return Properties.Resources.TagSizeOutOfRange;
                    break;
            }            

            return null;
        }
        /// <summary>
        /// TagSizeCheck -> this method check the size of tag
        /// </summary>
        /// <returns>Return a false if the tag size is contain in the protocol size</returns>
        private bool TagSizeCheck()
        {
            bool res = true;

            uint protocolSize = GESRTP2Protocol.PLCTYPE_SERIES90_MAX_PDU_SIZE; // use this value for back compatibility with old projects
            if (stationSettingList != null &&  stationSettingList.ContainsKey(StationName))
                protocolSize = GESRTP2Protocol.GetMaxJobSize(stationSettingList[StationName].PlcType);

            uint arrayDimension = (ArrayDimension == 0 ? 1 : ArrayDimension);

            uint size = 0;
            if (AreaType == AreaTypes.Symbolic)
            {
                size = GetDataTypeByteSize(VarType);

                // !isProtocolBool()) || ProtocolDataSizeBig() --> conversion
                if (VarType == UFUAModel.DataType.Boolean || VarType == UFUAModel.DataType.Byte || VarType == UFUAModel.DataType.SByte)
                    size *= 2;

                size *= arrayDimension;

                if (size > protocolSize)
                    res = false;
            }
            else
            {
                BuiltInType dt = GESRTP2Protocol.DataType(AreaType);

                size = GetDataTypeByteSize(GESRTP2Protocol.GetUFUAModelDataTypeFromBuiltType(dt));

                if (GESRTP2Protocol.GetBuiltTypeFromUFUAModelDataType(VarType) == BuiltInType.String)
                {
                    // align string size to plc data size
                    if ((1 + StringLength) % size == 0)
                        size = (1 + StringLength);
                    else
                        size = (1 + StringLength) + 1;
                }
                
                size *= arrayDimension;

                //verify with the plc data type
                if (size > protocolSize)
                {
                    res = false;
                }
                else
                { 
                    //verify with the Movicon data type
                    size = GetDataAreaTagSize(VarType);
                    if (size > protocolSize)                
                        res = false;
                }
            }
            
            return res;
        }
        /// <summary>
        /// GetTagSize -> this method calculate the size of the tag
        /// </summary>
        /// <param name="dt">Movicon data type</param>
        /// <returns>Reutrn the tag size of Movicon data type</returns>
        private uint GetDataAreaTagSize(UFUAModel.DataType dt)
        {
            switch (dt)
            {
                case UFUAModel.DataType.String:
                    if (GetProtocolDataByteSize() == 1)
                        return (uint)StringLength;
                    else
                        return (uint)((StringLength + 1) / 2) * 2;
                default:
                    return GetDataTypeByteSize(dt);
            }
        }
        /// <summary>
        /// GetDataTypeByteSize -> This method calculate the byte size the Movicon data type of the tag 
        /// </summary>
        /// <param name="DataType"></param>
        /// <returns>return the byte size of Movicon data type</returns>
        private uint GetDataTypeByteSize(UFUAModel.DataType DataType)
        {
            switch (DataType)
            {
                case UFUAModel.DataType.Boolean:
                case UFUAModel.DataType.SByte:
                case UFUAModel.DataType.Byte:
                    return 1;
                case UFUAModel.DataType.Int16:
                case UFUAModel.DataType.UInt16:
                    return 2;
                case UFUAModel.DataType.Float:
                case UFUAModel.DataType.UInt32:
                case UFUAModel.DataType.Int32:
                    return 4;
                case UFUAModel.DataType.UInt64:
                case UFUAModel.DataType.Int64:
                case UFUAModel.DataType.Double:
                    return 8;
                default:
                    return 1;
            }
        }

        private bool InvalidStringSize()
        {
            return ((VarType == UFUAModel.DataType.String) && ((StringLength > GESRTP2Protocol.DATA_AREA_MAX_STRING_SIZE) || (StringLength < 1)));
        }

        #endregion

        #region INotifyPropertyChanged Members

        protected override void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "TagLinkType":
                    OnPropertyChanged(new PropertyChangedEventArgs("AreaType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("OutputAtStartup"));
                    break;
                case "VarType":
                    OnPropertyChanged(new PropertyChangedEventArgs("AreaType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ArrayDimension":
                    OnPropertyChanged(new PropertyChangedEventArgs("AreaType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ElementNumber":
                    OnPropertyChanged(new PropertyChangedEventArgs("AreaType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    break;
            }
        }
        #endregion        
    }
}

