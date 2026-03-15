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

namespace ModbusTCPSlave
{
    public sealed class ModbusTCPSlaveDynTagSettings : DynTagSettings
    {
        #region Constructors

        public ModbusTCPSlaveDynTagSettings()
            : base()
        {
            DataArea = DataAreas.HoldingRegisters;
            StartAddress = 0;
        }

        #endregion
        
        #region Static Members

        private static readonly String DataAreaParameter = "DA";
        private static readonly String StartAddressParameter = "SA";
        
        
        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            DataArea = (DataAreas)(helper.GetPartByName(DataAreaParameter, (UInt16)DataAreas.HoldingRegisters));
            StartAddress = helper.GetPartByName(StartAddressParameter, (UInt16)0);
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(DataAreaParameter)))
                return false;

            // optional parameters
            DataArea = (DataAreas)(helper.GetPartByName(DataAreaParameter, (UInt16)DataAreas.HoldingRegisters));
            StartAddress = helper.GetPartByName(StartAddressParameter, (UInt16)0);

            return true;
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
			dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", DataAreaParameter, DynamicStringParser.CharAssign, (int)DataArea);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", StartAddressParameter, DynamicStringParser.CharAssign, StartAddress);

            return dynamicstring.ToString();
        }

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (ModbusSlaveProtocol.GetMaxJobSize(DataArea) >= ByteSize);
        }

        public bool ParseAddress(string address/*, string stationname, ref string dynamicaddress*/)
        {
            try
            {
                if (address.Length > 2)
                {
                    DataAreas da = DataAreas.HoldingRegisters;
                    switch (address.Substring(0, 2))
                    {
                        case "CS":
                            da = DataAreas.Coils;
                            break;
                        case "IS":
                            da = DataAreas.DiscreteInputs;
                            break;
                        case "HR":
                            da = DataAreas.HoldingRegisters;
                            break;
                        case "IR":
                            da = DataAreas.InputRegisters;
                            break;
                        case "SC":
                            da = DataAreas.Coils;
                            break;
                        default:
                            return false;
                    }
                    int nidx = address.IndexOf('.');
                    if (nidx == -1)
                        StartAddress = Convert.ToUInt16(address.Substring(2));
                    else
                    {
                        if (da != DataAreas.HoldingRegisters)
                            return false;
                        byte tmpElementNumber = Convert.ToByte(address.Substring(nidx + 1));
                        if (tmpElementNumber > 15)
                            return false;
                        StartAddress = Convert.ToUInt16(address.Substring(2, nidx - 2));
                        ElementNumber = tmpElementNumber;
                    }
                    DataArea = da;
                    return true;
                }
            }
            catch (Exception e)
            { }
            //dynamicaddress = "";
            return false;
        }

        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            bool bits = (DataArea == DataAreas.Coils ||
                DataArea == DataAreas.DiscreteInputs);

            if (prevtagdefinition.DataType.IdType == IdType.Numeric)
            {
                switch ((uint)prevtagdefinition.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                        if(bits)
                            StartAddress += (prevtagdefinition.ArrayDimension > 0 ? (UInt16)prevtagdefinition.ArrayDimension : (UInt16)1);
                        else
                            StartAddress += (prevtagdefinition.ArrayDimension > 0 ? (UInt16)(prevtagdefinition.ArrayDimension / 16 + (prevtagdefinition.ArrayDimension % 16 > 0 ? 1 : 0)) : (UInt16)1);
                        break;
                    case (uint)BuiltInType.SByte:
                    case (uint)BuiltInType.Byte:
                        if (bits)
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 8);
                        else
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension / 2 + (prevtagdefinition.ArrayDimension % 2 > 0 ? 1: 0)) : 1));
                        break;
                    case (uint)BuiltInType.Int16:
                    case (uint)BuiltInType.UInt16:
                        if (bits)
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 2 * 8);
                        else
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension) : 1));
                        break;
                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                        if (bits)
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 4 * 8);
                        else
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension * 2) : 2));
                        break;
                    case (uint)BuiltInType.UInt64:
                    case (uint)BuiltInType.Int64:
                    case (uint)BuiltInType.Double:
                        if (bits)
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? prevtagdefinition.ArrayDimension : 1) * 4 * 8);
                        else
                            StartAddress += (UInt16)((prevtagdefinition.ArrayDimension > 0 ? (prevtagdefinition.ArrayDimension * 2) : 2));
                        break;
                }
            }

            return ToString();
        }
        public override UFUAModel.DataType getProtocolDataType()
        {
            switch (DataArea)
            {
                case DataAreas.Coils: // Coils
                case DataAreas.DiscreteInputs: // Input discretes
                    return UFUAModel.DataType.Boolean;
                case DataAreas.HoldingRegisters: // Holding registers
                case DataAreas.InputRegisters: // Input registers
                    return UFUAModel.DataType.UInt16;
            }
            return 0;
        }
        #endregion

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
        //        OnPropertyChanged(new PropertyChangedEventArgs("DataArea"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("DataArea"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //    }
        //}

        private DataAreas _DataArea;
        [Category("Device Data")]
        [Description("Data Area")]
        public DataAreas DataArea
        {
            get { return _DataArea; }
            set
            {
                _DataArea = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
            }
        }

        private UInt16 _StartAddress;
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

        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "DataAreas")
            {
                if (DataArea != DataAreas.Coils && DataArea != DataAreas.DiscreteInputs &&
                    DataArea != DataAreas.HoldingRegisters && DataArea != DataAreas.InputRegisters)
                    return Properties.Resources.InvalidDataArea;

                if ((uint)VarType != unchecked((uint)(-1)))
                    return ProtocolDataSizeValidation(VarType);
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
                case "VarType":
                    OnPropertyChanged(new PropertyChangedEventArgs("DataArea"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    break;
                case "ArrayDimension":
                    OnPropertyChanged(new PropertyChangedEventArgs("DataArea"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    break;
            }
        }
        #endregion
    }
}
