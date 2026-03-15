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

namespace Simotion
{
    public sealed class SimotionDynTagSettings : DynTagSettings
    {
        #region Constructors

        public SimotionDynTagSettings()
            : base()
        {
            //_StringLength = 0;
            _S7DataFormat = S7DataFormats.Bool;
            _StartAddress = "";
            //_Trans = Step7WordTrans.wtT;
        }

        #endregion
        
        #region Static Members

        private static readonly String StartAddressParameter = "SA";
        private static readonly String S7DataFormatParameter = "SITyp";

        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);
            StartAddress = helper.GetPartByName(StartAddressParameter);
            S7DataFormat = (S7DataFormats)helper.GetPartByName(S7DataFormatParameter,(UInt16)S7DataFormats.Bool);

            ParseAddress(StartAddress);
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;
            
            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);
            StartAddress = helper.GetPartByName(StartAddressParameter);
            S7DataFormat = (S7DataFormats)helper.GetPartByName(S7DataFormatParameter, (UInt16)S7DataFormats.Bool);

            return ParseAddress(StartAddress);
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", StartAddressParameter, DynamicStringParser.CharAssign, StartAddress);
            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", S7DataFormatParameter, DynamicStringParser.CharAssign, (int)S7DataFormat);


            return dynamicstring.ToString();
        }

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (SimotionProtocol.MAX_DATA_BYTES >= ByteSize);
        }
        public override UFUAModel.DataType getProtocolDataType()
        {
            return (SimotionProtocol.DataType(S7DataFormat));
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
            string Tree = SimotionProtocol.GetNodeTree(thistagdefinition.NodeId, thistagdefinition.Name);
            StartAddress += ("." + Tree.Replace('/', '.'));
            S7DataFormat = SimotionCommJob.GetSimotionType(thistagdefinition.DataType.Identifier.ToString());
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

        #endregion

        #region IDataErrorInfo Members

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
            else if(propertyName == "S7DataFormat")
            {

            }

            return null;
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
