using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Helpers;
using DriverCodeBaseEx.Enumerators;
using System.ComponentModel;
using DriverCodeBaseEx.Properties;
using DriverBaseInterfaces;
using Opc.Ua;
using DevExpress.Xpo;
using DevExpress.XtraPrinting.Native;

namespace ROCDriver
{
    public sealed class ROCDriverDynTagSettings : DynTagSettings
    {
        #region Constructors

        public ROCDriverDynTagSettings()
            : base()
        {
            _PointType = 0;
            _LogicalNumber = 0;
            _Parameter = 0;
            _DataType = DataTypes.BIN;
            _StringLength = ROCDriverProtocol.StringDefaultLength;
        }

        #endregion

        #region Static Members

        private static readonly String PointTypeParameter = "PT";
        private static readonly String LogicalNumberParameter = "LN";
        private static readonly String ParameterParameter = "PRM";
        private static readonly String DataTypeParameter = "DT";
        private static readonly String StringLengthParameter = "SL";

        #endregion

        #region Override Functions

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (ROCDriverProtocol.GetMaxJobSize() >= ByteSize);
        }

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            _PointType = (Byte)helper.GetPartByName(PointTypeParameter, (UInt16)0);
            _LogicalNumber = (Byte)helper.GetPartByName(LogicalNumberParameter, (UInt16)0);
            _Parameter = (Byte)helper.GetPartByName(ParameterParameter, (UInt16)0);
            _DataType = (DataTypes)helper.GetPartByName(DataTypeParameter, (Byte)0);
            _StringLength = (Byte)helper.GetPartByName(StringLengthParameter, (uint)ROCDriverProtocol.StringDefaultLength);
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // Required parameters
            if (String.IsNullOrEmpty(helper.GetPartByName(PointTypeParameter)) ||
                String.IsNullOrEmpty(helper.GetPartByName(LogicalNumberParameter)) ||
                String.IsNullOrEmpty(helper.GetPartByName(ParameterParameter)) ||
                String.IsNullOrEmpty(helper.GetPartByName(DataTypeParameter)))
            {
                return false;
            }
            _PointType = (Byte)helper.GetPartByName(PointTypeParameter, (UInt16)0);
            _LogicalNumber = (Byte)helper.GetPartByName(LogicalNumberParameter, (UInt16)0);
            _Parameter = (Byte)helper.GetPartByName(ParameterParameter, (UInt16)0);
            _DataType = (DataTypes)helper.GetPartByName(DataTypeParameter, (Byte)0);

            // Optional parameters
            StringLength = helper.GetPartByName(StringLengthParameter, (uint)ROCDriverProtocol.StringDefaultLength);

            return true;
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());

            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", PointTypeParameter, DynamicStringParser.CharAssign, PointType);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", LogicalNumberParameter, DynamicStringParser.CharAssign, LogicalNumber);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", ParameterParameter, DynamicStringParser.CharAssign, Parameter);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", DataTypeParameter, DynamicStringParser.CharAssign, (Byte)DataType);

            if ((VarType == UFUAModel.DataType.String) || (((int)VarType == -1) && (StringLength > 0)))
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", StringLengthParameter, DynamicStringParser.CharAssign, StringLength);
            }

            return dynamicstring.ToString();
        }

        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            String dynamicSettings = ToString();
            TryParse(dynamicSettings);
            return dynamicSettings;
        }

        public override UFUAModel.DataType getProtocolDataType()
        {
            switch(DataType)
            {
                case DataTypes.BIN:
                case DataTypes.UINT8:
                    return UFUAModel.DataType.Byte;
                case DataTypes.AC:
                    return UFUAModel.DataType.String;
                case DataTypes.INT8:
                    return UFUAModel.DataType.SByte;
                case DataTypes.INT16:
                    return UFUAModel.DataType.Int16;
                case DataTypes.INT32:
                    return UFUAModel.DataType.Int32;
                case DataTypes.UINT16:
                    return UFUAModel.DataType.UInt16;
                case DataTypes.UINT32:
                    return UFUAModel.DataType.UInt32;
                case DataTypes.FLOAT:
                    return UFUAModel.DataType.Float;
                case DataTypes.DBL:
                    return UFUAModel.DataType.Double;
                case DataTypes.TLP:
                    return UFUAModel.DataType.UInt32;
                case DataTypes.TIME:
                    return UFUAModel.DataType.String;
            }
            return (UFUAModel.DataType.Boolean);
        }
        #endregion

        #region Properties

        private String _Address;
        [Category("Device Data")]
        [Description("Address")]
        public string Address
        {
            get { return _Address; }
            set 
            { 
                _Address = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TagLinkType"));
                OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
            }
        }

        private Byte _PointType;
        [Category("Device Data")]
        [Description("Point Type")]
        public Byte PointType
        {
            get { return _PointType; }
            set
            {
                _PointType = value;
            }
        }

        private Byte _LogicalNumber;
        [Category("Device Data")]
        [Description("Logical Number")]
        public Byte LogicalNumber
        {
            get { return _LogicalNumber; }
            set
            {
                _LogicalNumber = value;
            }
        }

        private Byte _Parameter;
        [Category("Device Data")]
        [Description("Parameter")]
        public Byte Parameter
        {
            get { return _Parameter; }
            set
            {
                _Parameter = value;
            }
        }

        private DataTypes _DataType;
        [Category("Device Data")]
        [Description("Data Type")]
        public DataTypes DataType
        {
            get { return _DataType; }
            set
            {
                _DataType = value;
            }
        }

        /// <summary>   The String Length. </summary>
        private uint _StringLength;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   ROC String Length Property. </summary>
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

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "StringLength")
            {
                if (InvalidStringSize())
                    return Properties.Resources.ErrorInvalidStringSize;
            }
            else if (propertyName == "DataType")
            {
                return CheckInvalidDataType();
            }
            else if(propertyName == "ElementNumber")
            {
                return (CheckInvalidElementNumber());
            }

            return null;
        }

        private bool InvalidStringSize()
        {
            return ((VarType == UFUAModel.DataType.String) &&
                    ((StringLength > ROCDriverProtocol.GetMaxJobSize()) || (StringLength < 1)));
        }

        private String CheckInvalidDataType()
        {
            String retErrorString = null;
            if ((uint)VarType != unchecked((uint)(-1)))
            {
                if(((DataType == DataTypes.AC) || (DataType == DataTypes.TIME)) && (VarType != UFUAModel.DataType.String))
                {
                    retErrorString = Properties.Resources.ErrorDataTypeRequiresStringVar;
                }
            }
            return (retErrorString);
        }

        private String CheckInvalidElementNumber()
        {
            String retErrorString = null;
            if ((uint)VarType != unchecked((uint)(-1)))
            {
                if ((DataType == DataTypes.BIN) && (VarType == UFUAModel.DataType.Boolean))
                {
                    if((ElementNumber < 0) || (ElementNumber > 7))
                    {
                        retErrorString = Properties.Resources.InvalidElementNumber;
                    }
                }
            }
            return (retErrorString);
        }


        #endregion

        #region INotifyPropertyChanged Members

        protected override void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
