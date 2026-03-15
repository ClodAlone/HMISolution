using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Helpers;
using DriverCodeBaseEx.Enumerators;
using System.ComponentModel;
using Opc.Ua;
using DriverBaseInterfaces;

namespace MelsecFXTCP
{
    public sealed class MelsecFXTCPDynTagSettings : DynTagSettings
    {
       #region Constructors

        public MelsecFXTCPDynTagSettings()
            : base()
        {
            Address = String.Empty;
            fxAddress = new MelsecFXAddress();
        }

        #endregion

        #region Members

        MelsecFXAddress fxAddress;

        #endregion

        #region Static Members

        private static readonly String AddressParameter = "Addr";

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
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("OutputAtStartup"));
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
                OnPropertyChanged(new PropertyChangedEventArgs("TagLinkType"));
                OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
            }
        }

        // Added in version 2.2.30.0 (FOGBUGZ 15458)
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("FunctionCode"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
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

        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);
            Address = helper.GetPartByName(AddressParameter);
            fxAddress.Set(Address);
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
            fxAddress.Set(Address);
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


        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            
            if (prevtagdefinition.DataType.IdType == IdType.Numeric)
            {
                fxAddress.SetStructFieldAdd((uint)prevtagdefinition.DataType.Identifier, prevtagdefinition.ArrayDimension);
                Address = fxAddress.Address;
            }
            return ToString();
        }

        //// Added in version 2.2.30.0 (FOGBUGZ 15458)
        //public override bool isTagByteSizeOk(uint ByteSize)
        //{
        //    return (GetMaximumNumberOfByte() >= ByteSize);
        //}

        //Redefined the method because, using the basic method not associated different types of tags.
        public override UFUAModel.DataType getProtocolDataType()
        {
            MelsecFXAddress AddressObj = new MelsecFXAddress(_Address);
            return (MelsecFXTCPCommJob.DataType(AddressObj, VarType));
        }

        #endregion

        // TODO (used in import routine)
        public bool ParseAddress(string address)
        {
            try
            {
                fxAddress.Set(address);
                if (fxAddress.IsValid)
                {
                    _Address = address;
                    if (fxAddress.DataArea == MelsecFXDataArea.DataArea_X)
                    {
                        TagLinkType = (int)LinkType.Input;
                    }
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
                MelsecFXAddress fxAddressObj = new MelsecFXAddress(Address);
                if (!fxAddressObj.IsValid)
                {
                    return Properties.Resources.ErrorInvalidAddress;
                }
                if (fxAddressObj.DataArea == MelsecFXDataArea.DataArea_X)
                {
                    if (TagLinkType != (int)LinkType.Input)
                    {
                        return Properties.Resources.ErrorReadOnly;
                    }
                }
                // Added in version 2.2.30.0 (FOGBUGZ 15458)     
                if (VarType == UFUAModel.DataType.String)
                {
                    return string.Format(Properties.Resources.ErrorInvalidTagType,Address,VarType);
                }     
                else if (CheckBitDeviceAddress(fxAddressObj))
                {
                    return string.Format(Properties.Resources.ErrorInvalidAssignedAddress,Address);
                }
                else if ((uint)VarType != unchecked((uint)(-1)))
                {
                    return ProtocolDataSizeValidation(VarType);
                }                    
                  
            }
            else if (propertyName == "TagLinkType")
            {
                MelsecFXAddress fxAddressObj = new MelsecFXAddress(Address);
                if (fxAddressObj.IsValid == true)
                {
                    if (fxAddressObj.DataArea == MelsecFXDataArea.DataArea_X)
                    {
                        if (TagLinkType != (int)LinkType.Input)
                        {
                            return Properties.Resources.ErrorReadOnly;
                        }
                    }
                }
            }
                        
            return null;
        }

        // Added in version 2.2.30.0 (FOGBUGZ 15458)
        protected bool CheckBitDeviceAddress(MelsecFXAddress fxAddressObj)
        {
            bool bChecked = false;
            
            switch (fxAddressObj.DataArea)
            {
                case MelsecFXDataArea.DataArea_X:
                case MelsecFXDataArea.DataArea_Y:
                case MelsecFXDataArea.DataArea_S:
                case MelsecFXDataArea.DataArea_M:
                case MelsecFXDataArea.DataArea_M_Special:
                case MelsecFXDataArea.DataArea_L:
                case MelsecFXDataArea.DataArea_B:
                case MelsecFXDataArea.DataArea_F:
                case MelsecFXDataArea.DataArea_TS:
                case MelsecFXDataArea.DataArea_TC:
                case MelsecFXDataArea.DataArea_C:                
                case MelsecFXDataArea.DataArea_CC:
                case MelsecFXDataArea.DataArea_CS:
                case MelsecFXDataArea.DataArea_CS_16: 
                case MelsecFXDataArea.DataArea_CS_32:
                    if ((VarType == UFUAModel.DataType.Double) || (VarType == UFUAModel.DataType.Float) || 
                             (VarType == UFUAModel.DataType.UInt64) || (VarType == UFUAModel.DataType.Int64))
                    {
                        bChecked = true;
                    }
                    else
                    {

                        if ((VarType != UFUAModel.DataType.Boolean) && ((fxAddressObj.StartAddress % 16) != 0))
                        {
                            bChecked = true;
                        }
                    }
                    break;              
                
                case MelsecFXDataArea.DataArea_TN:
                case MelsecFXDataArea.DataArea_CN:
                case MelsecFXDataArea.DataArea_CN_16:
                case MelsecFXDataArea.DataArea_CN_32:
                    if ((VarType == UFUAModel.DataType.Boolean) || (VarType == UFUAModel.DataType.Byte) || 
                        (VarType == UFUAModel.DataType.SByte))
                    {
                        bChecked = true;
                    }    
                    else if ((VarType == UFUAModel.DataType.Double) || (VarType == UFUAModel.DataType.Float) || 
                             (VarType == UFUAModel.DataType.UInt64) || (VarType == UFUAModel.DataType.Int64))
                    {
                        bChecked = true;
                    }                    
                    break;

                //case MelsecFXDataArea.DataArea_D:
                //case MelsecFXDataArea.DataArea_W:
                //case MelsecFXDataArea.DataArea_R:
                //case MelsecFXDataArea.DataArea_D_Special:
                //    if ((VarType == UFUAModel.DataType.Boolean) || (VarType == UFUAModel.DataType.Byte) ||
                //        (VarType == UFUAModel.DataType.SByte))
                //    {
                //        bChecked = true;
                //    }                    
                //    break;
                default:
                    break;
            }
            return bChecked;
        }

        // Added in version 2.2.30.0 (FOGBUGZ 15458)
        protected uint GetMaximumNumberOfByte()
        {
            uint uiLength = 1;

            switch (fxAddress.DataArea)
            {
                case MelsecFXDataArea.DataArea_X:
                case MelsecFXDataArea.DataArea_Y:
                case MelsecFXDataArea.DataArea_S:
                case MelsecFXDataArea.DataArea_M:
                case MelsecFXDataArea.DataArea_M_Special:
                case MelsecFXDataArea.DataArea_L:
                case MelsecFXDataArea.DataArea_B:
                case MelsecFXDataArea.DataArea_F:
                case MelsecFXDataArea.DataArea_TS:
                case MelsecFXDataArea.DataArea_TC:
                case MelsecFXDataArea.DataArea_C:
                case MelsecFXDataArea.DataArea_CC:
                case MelsecFXDataArea.DataArea_CS:
                case MelsecFXDataArea.DataArea_CS_16:
                case MelsecFXDataArea.DataArea_CS_32:
                    uiLength = 20;                         
                    break;

                case MelsecFXDataArea.DataArea_TN:
                case MelsecFXDataArea.DataArea_CN:
                case MelsecFXDataArea.DataArea_CN_16:
                case MelsecFXDataArea.DataArea_CN_32:                   
                case MelsecFXDataArea.DataArea_D:
                case MelsecFXDataArea.DataArea_W:
                case MelsecFXDataArea.DataArea_R:
                case MelsecFXDataArea.DataArea_D_Special:
                    uiLength = 128;
                    break;
                default:
                    break;
            }
            return uiLength;
        }

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            // size of job if always calculated in byte (bool's data type included)
            return (128 >= ByteSize);
        }

        #endregion

        #region INotifyPropertyChanged Members

        protected override void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

            switch (propertyName) {
                case "TagLinkType":
                    OnPropertyChanged(new PropertyChangedEventArgs("Address"));
                    OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("OutputAtStartup"));
                    break;                
                case "ArrayDimension":
                    OnPropertyChanged(new PropertyChangedEventArgs("Address"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;
                case "VarType":
                    OnPropertyChanged(new PropertyChangedEventArgs("FunctionCode"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
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
