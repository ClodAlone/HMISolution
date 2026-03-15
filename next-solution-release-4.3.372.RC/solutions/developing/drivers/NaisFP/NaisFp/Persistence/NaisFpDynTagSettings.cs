using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Helpers;
using DriverCodeBase.Enumerators;
using System.ComponentModel;
using DriverCodeBase.Properties;
using DriverBaseInterfaces;
using Opc.Ua;
using DevExpress.Xpo;

namespace NaisFp
{
    public sealed class NaisFpDynTagSettings : DynTagSettings
    {
        #region Constructors

        public NaisFpDynTagSettings()
            : base()
        {
            _Address = String.Empty;
        }

        #endregion
        
        #region Static Members

        private static readonly String AddressParameter = "Addr";

        #endregion

        #region Override Functions

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            NaisFpAddress AddressObj = new NaisFpAddress(_Address);
            if (AddressObj.DataFormat == DataFormats.BOOL)
            {
                ushort NumOfBit;
                if (VarType == UFUAModel.DataType.Boolean || ElementNumber == 0)
                    NumOfBit = (ushort)(ByteSize * 8);
                else
                    NumOfBit = (ushort)ArrayDimension;

                if (NumOfBit > 8)
                    return false;

            }
            return (NaisFpProtocol.GetMaxJobSize(FrameFormats.Long) >= ByteSize);
        }

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            _Address = helper.GetPartByName(AddressParameter);
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(AddressParameter)))
                return false;
            _Address = helper.GetPartByName(AddressParameter);
            // Check the address
            NaisFpAddress testAddress = new NaisFpAddress(_Address);
            if (!testAddress.IsValid)
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return ToStringNaisFp(_Address);
        }

        public string ToStringNaisFp(String inAddress)
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", AddressParameter, DynamicStringParser.CharAssign, inAddress);

            return dynamicstring.ToString();
        }

        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            NaisFpAddress nextAddress = new NaisFpAddress(_Address, prevtagdefinition);
            if (!nextAddress.IsValid)
            {
                return string.Empty;
            }
            String dynamicSettings = ToStringNaisFp(nextAddress.Get());
            TryParse(dynamicSettings);
            return dynamicSettings;
        }

        public bool ParseAddress(string inAddress)
        {
            try
            {
                NaisFpAddress addObj = new NaisFpAddress(inAddress);
                if (addObj.IsValid)
                {
                    _Address = inAddress;
                    return true;
                }
            }
            catch (Exception e)
            { }
            return false;
        }

        public string NaisFpAddress()
        {
            NaisFpAddress addObj = new NaisFpAddress(_Address);
            return addObj.Get();
        }

        public override UFUAModel.DataType getProtocolDataType()
        {
            NaisFpAddress addObj = new NaisFpAddress(_Address);
            return (NaisFpProtocol.DataType(addObj));
        }
        #endregion

        #region Properties

        //private int/*LinkType*/ _TagLinkType;
        //[Category("General")]
        //[Description("Link Type")]
        //public override int/*LinkType*/ TagLinkType
        //{
        //    get { return _TagLinkType; }
        //    set
        //    {
        //        _TagLinkType = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("Address"));
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("Address"));
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("Address"));
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("Address"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //    }
        //}

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


        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "TagLinkType")
            {
                if (InvalidFunctionCodeLinkType())
                    return DriverCodeBase.Properties.Resources.JobTypeInvalid;
            }

            if (propertyName == "Address")
            {
                if (InvalidFunctionCodeLinkType())
                    return DriverCodeBase.Properties.Resources.JobTypeInvalid;
                NaisFpAddress testAddress = new NaisFpAddress(Address);
                if (!testAddress.IsValid)
                {
                    return Properties.Resources.ErrorInvalidAddress;
                }
                if (VarType == UFUAModel.DataType.String)
                {
                    return string.Format(Properties.Resources.ErrorInvalidTagType, Address, VarType);
                }

                if ((uint)VarType != unchecked((uint)(-1)))
                    return ProtocolDataSizeValidation(VarType);
            }


            return null;
        }
        private bool InvalidFunctionCodeLinkType()
        {
            LinkType lt = (LinkType)TagLinkType;
            NaisFpAddress testAddress = new NaisFpAddress(_Address);
            return ((lt == LinkType.ExceptionOutput || lt == LinkType.InputOutput || lt == LinkType.UnconditionalOutput) &&
                testAddress.MemoryArea == MemoryAreas.X);
        }

        #endregion

        #region INotifyPropertyChanged Members

        protected override void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "TagLinkType":
                    OnPropertyChanged(new PropertyChangedEventArgs("Address"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("OutputAtStartup"));
                    break;
                case "VarType":
                    OnPropertyChanged(new PropertyChangedEventArgs("Address"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ArrayDimension":
                    OnPropertyChanged(new PropertyChangedEventArgs("Address"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "ElementNumber":
                    OnPropertyChanged(new PropertyChangedEventArgs("Address"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    break;
            }
        }
        #endregion

    }
}
