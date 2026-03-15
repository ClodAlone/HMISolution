using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Helpers;
using DriverCodeBaseEx.Enumerators;
using System.ComponentModel;

namespace MelsecFX
{
    public sealed class MelsecFXDynTagSettings : DynTagSettings
    {
       #region Constructors

        public MelsecFXDynTagSettings()
            : base()
        {
            Address = String.Empty;
        }

        #endregion
        
        #region Static Members

        private static readonly String AddressParameter = "Addr";

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

        private string _Address;
        [Category("Device Data")]
        [Description("Address")]
        public string Address
        {
            get { return _Address; }
            set
            {
                _Address = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
            }
        }

        #endregion
 
        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);
            Address = helper.GetPartByName(AddressParameter);
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(AddressParameter)))
                return false;
            Address = helper.GetPartByName(AddressParameter);

            // Check the address
            MelsecFXAddress fxAddress = new MelsecFXAddress(Address);
            if (!fxAddress.IsValid)
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", AddressParameter,
                                       DynamicStringParser.CharAssign,
                                       Address);

            return dynamicstring.ToString();
        }

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            bool isBitVariables = false;
            if (VarType == UFUAModel.DataType.Boolean)
                isBitVariables = true;
            return (MelsecFXCommJob.GetMaxJobSize(isBitVariables) >= ByteSize);
        }
        public override UFUAModel.DataType getProtocolDataType()
        {
            MelsecFXAddress AddressObj = new MelsecFXAddress(_Address);
            return (MelsecFXCommJob.DataType(AddressObj,VarType));
        }
        #endregion

        // TODO (used in import routine)
        public bool ParseAddress(string address)
        {
            try
            {
                MelsecFXAddress addObj = new MelsecFXAddress(address);
                if (addObj.IsValid)
                {
                    _Address = address;
                    return true;
                }
            }
            catch (Exception e)
            { }
            return false;
        }

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            // Check the address parameter
            if (propertyName == "Address")
            {
                MelsecFXAddress fxAddress = new MelsecFXAddress(Address);
                if (!fxAddress.IsValid)
                {
                    return Properties.Resources.ErrorInvalidAddress;
                }
                if (VarType == UFUAModel.DataType.String)
                {
                    return string.Format(Properties.Resources.ErrorInvalidTagType, Address, VarType);
                }
                if (fxAddress.DataArea == MelsecFXDataArea.DataArea_X &&
                    (LinkType)TagLinkType != LinkType.Input)
                    return Properties.Resources.ErrorReadOnly;
                if ((uint)VarType != unchecked((uint)(-1)))
                {
                    if ((uint)VarType != unchecked((uint)(-1)))
                        return ProtocolDataSizeValidation(VarType);

                }
                
            }
            else if (propertyName == "TagLinkType")
            {
                MelsecFXAddress fxAddress = new MelsecFXAddress(Address);
                if (fxAddress.DataArea == MelsecFXDataArea.DataArea_X &&
                    (LinkType)TagLinkType != LinkType.Input)
                    return Properties.Resources.ErrorReadOnly;
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
    }
    #endregion
}
