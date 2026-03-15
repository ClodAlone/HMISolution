using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Helpers;
using System.Text.RegularExpressions;
using DriverBaseInterfaces;
using Opc.Ua;

namespace S7TIASymbolic
{
    public sealed class S7TIADynTagSettings : DynTagSettings
    {
        #region Constructors

        public S7TIADynTagSettings()
            : base()
        {
            _StringLength = 0;
            _S7DataFormat = S7DataFormats.Bool;
            _StartAddress = "";
            //_Trans = Step7WordTrans.wtT;
        }

        #endregion
        
        #region Static Members

        private static readonly String StartAddressParameter = "SA";
        private static readonly String S7DataFormatParameter = "S7Typ";
        private static readonly String StringLengthParameter = "DL";

        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);
            StartAddress = helper.GetPartByName(StartAddressParameter);
            S7DataFormat = (S7DataFormats)helper.GetPartByName(S7DataFormatParameter,(UInt16)S7DataFormats.Bool);
            StringLength = helper.GetPartByName(StringLengthParameter, (uint)0);

            ParseAddress(StartAddress);
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;
            
            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);
            StartAddress = helper.GetPartByName(StartAddressParameter);
            S7DataFormat = (S7DataFormats)helper.GetPartByName(S7DataFormatParameter, (UInt16)S7DataFormats.Bool);
            StringLength = helper.GetPartByName(StringLengthParameter, (uint)0);

            return ParseAddress(StartAddress);
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", StartAddressParameter, DynamicStringParser.CharAssign, StartAddress);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", S7DataFormatParameter, DynamicStringParser.CharAssign, (int)S7DataFormat);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", StringLengthParameter, DynamicStringParser.CharAssign, (uint)StringLength);

            return dynamicstring.ToString();
        }

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (S7TIAProtocol.MAX_DATA_BYTES >= ByteSize);
        }
        public override UFUAModel.DataType getProtocolDataType()
        {
            return (S7TIAProtocol.DataType(S7DataFormat));
        }

        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            return GetNodeDynSetting(thistagdefinition);
        }

        public override string GetFirstDynSetting(Tag tag, TagDefinition thistagdefinition)
        {
            TryParse(tag.TagNode.DynamicSettings);
            return GetNodeDynSetting(thistagdefinition);
        }

         string GetNodeDynSetting(TagDefinition thistagdefinition)
        {
            string memABAddress = StartAddress;
            S7DataFormats memTagFormat = S7DataFormat;
            string Tree = S7TIAProtocol.GetNodeTree(thistagdefinition.NodeId, thistagdefinition.Name);
            StartAddress += ("." + Tree.Replace('/', '.'));
            S7DataFormat = S7TIACommJob.GetS7TIAType(thistagdefinition.DataType.Identifier.ToString());
            string dynsettings = ToString();
            StartAddress = memABAddress;
            S7DataFormat = memTagFormat;
            return dynsettings;
        }       

        #endregion

        public bool ParseAddress(string address)
        {
            if ((address == null) || (address == String.Empty))
            {
                return false;
            }
            StartAddress = address;
            return true;
        }
 
        #region Properties
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("StartAddress"));
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("StartAddress"));
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("StartAddress"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //    }
        //}
        private string _StartAddress;
        [Category("Device Data")]
        [Description("Address")]
        public string StartAddress
        {
            get { return _StartAddress; }
            set
            {
                _StartAddress = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                OnPropertyChanged(new PropertyChangedEventArgs("DataFormat"));
            }
        }
        private uint _StringLength;
        [Category("Device Data")]
        [Description("String Length")]
        public uint StringLength
        {
            get { return _StringLength; }
            set { _StringLength = value; }
        }
        private S7DataFormats _S7DataFormat;
        [Category("Device Data")]
        [Description("S7 Data Format")]
        public S7DataFormats S7DataFormat
        {
            get { return _S7DataFormat; }
            set { 
                  _S7DataFormat = value;
                  OnPropertyChanged(new PropertyChangedEventArgs("StartAddress"));
            }
        }

        //private Step7WordTrans _Trans;
        //[Category("Device Data")]
        //[Description("Data Transformation")]
        //public Step7WordTrans Trans
        //{
        //    get { return _Trans; }
        //    set { _Trans = value; }
        //}

        #endregion

        #region IDataErrorInfo Members
        //private bool ParseS7TimeConversion(string startAddress)
        //{
        //    if (StartAddress == null || StartAddress.Length == 0)
        //        return true;


        //    int idx = startAddress.IndexOf(S7TIAProtocol.S5_TIME_SEPARATOR_CHAR);
        //    if (idx < 0)
        //        return true;

        //    string timeParameters = startAddress.Substring(idx + 1, startAddress.Length - idx - 1).Trim();

        //    switch (timeParameters)
        //    {
        //        case "Z":
        //        case "C":
        //            //if (Str[0] == 'Z')
        //            //    German = true;
        //            Trans = Step7WordTrans.wtC;
        //            return true;
        //        case "T":
        //            Trans = Step7WordTrans.wtT;
        //            return true;
        //    }

        //    return false;
        //}

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "StartAddress")
            {
                if ((StartAddress == null) || (StartAddress == String.Empty))
                {
                    return UFUAModel.Properties.Resources.InvalidDynamcSettings;
                }
                
            }                        
            else if (propertyName == "StringLength")
            {
                if (InvalidStringSize())
                    return Properties.Resources.StrngSizeOutOfRange;
            }

            else if(propertyName == "S7DataFormat")
            {

            }

            return null;
        }
        private bool InvalidStringSize()
        {
            
            return ((VarType == UFUAModel.DataType.String) && 
                    ((StringLength > 254) ||
                    (StringLength < 1)));
        }

        #endregion

        protected override void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "VarType":
                    OnPropertyChanged(new PropertyChangedEventArgs("StartAddress"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ArrayDimension":
                    OnPropertyChanged(new PropertyChangedEventArgs("StartAddress"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ElementNumber":
                    OnPropertyChanged(new PropertyChangedEventArgs("StartAddress"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    break;
            }
        }
    }
}
