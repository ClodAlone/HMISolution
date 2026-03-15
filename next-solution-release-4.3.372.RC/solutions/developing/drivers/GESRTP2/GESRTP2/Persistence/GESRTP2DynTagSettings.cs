////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	GESRTP2DynTagSettings.cs
//
// summary:	Implements the driver GESRTP2 dynamic tag settings class
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
            _StringLength = 32;
        }

        #endregion

        #region Static Members

        /// <summary>   The function code parameter. </summary>
        private static readonly String AreaTypeParameter = "AT";
        /// <summary>   The start address parameter. </summary>
        private static readonly String StartAddressParameter = "SA";
        /// <summary>   The String Length Parameter. </summary>
        private static readonly String StringLengthParameter = "DL";


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

            AreaType = (AreaTypes)(helper.GetPartByName(AreaTypeParameter, (UInt16)AreaTypes.RegisterWords_R));
            StartAddress = helper.GetPartByName(StartAddressParameter, (UInt16)0);
            StringLength = helper.GetPartByName(StringLengthParameter, (uint)32);
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

            // required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(AreaTypeParameter)))
                return false;
            AreaType = (AreaTypes)(helper.GetPartByName(AreaTypeParameter, (UInt16)AreaTypes.RegisterWords_R));

            // optional parameters
            StartAddress = helper.GetPartByName(StartAddressParameter, (UInt16)0);
            StringLength = helper.GetPartByName(StringLengthParameter, (uint)32);

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
			dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", AreaTypeParameter, DynamicStringParser.CharAssign, (int)AreaType);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", StartAddressParameter, DynamicStringParser.CharAssign, StartAddress);
            if(VarType == UFUAModel.DataType.String)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", StringLengthParameter, DynamicStringParser.CharAssign, (uint)StringLength);
            }
            return dynamicstring.ToString();
        }


        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (GESRTP2Protocol.GetMaxJobSize() >= ByteSize);
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
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension / 2 + (prevtagdefinition.ArrayDimension % 2 > 0 ? 1: 0)) : 1));
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
        public override UFUAModel.DataType getProtocolDataType()
        {
            return (GESRTP2Protocol.DataType(AreaType));
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("AreaType"));
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("AreaType"));
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("AreaType"));
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("AreaType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //    }
        //}

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
            set { _StringLength = value; }
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

            if (propertyName == "AreaType")
            {
                if (GESRTP2Protocol.InvalidAreaTypeLinkType(_AreaType,(LinkType)TagLinkType))
                    return string.Format(Properties.Resources.AreaTypeRequireInput, DriverCodeBase.Properties.Resources.LinkType_Input);

                if (GESRTP2Protocol.InvalidAreaDataType(_AreaType, VarType))
                    return UFUAModel.Properties.Resources.DataTypeIncompatible;
                
                if ((uint)VarType != unchecked((uint)(-1)))
                    return ProtocolDataSizeValidation(VarType);
            }
            else if (propertyName == "TagLinkType")
            {
                if (GESRTP2Protocol.InvalidAreaTypeLinkType(_AreaType, (LinkType)TagLinkType))
                    return string.Format(Properties.Resources.AreaTypeRequireInput, DriverCodeBase.Properties.Resources.LinkType_Input);
            }
            else if (propertyName == "StartAddress")
            {
                if (_StartAddress == 0)
                    return Properties.Resources.InvalidZeroStartAdd;
            }
            else if (propertyName == "StringLength")
            {
                if (InvalidStringSize())
                    return Properties.Resources.StrngSizeOutOfRange;
            }

            return null;
        }
        private bool InvalidStringSize()
        {

            return ((VarType == UFUAModel.DataType.String) &&
                    ((StringLength > GESRTP2Protocol.MAX_STRING_SIZE) ||
                    (StringLength < 1)));
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
