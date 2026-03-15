using System;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Helpers;
using System.ComponentModel;
using DriverBaseInterfaces;

namespace DICom
{
    public sealed class DIComDynTagSettings : DynTagSettings
    {
        #region Constructors

        public DIComDynTagSettings()
            : base()
        {
            VarName = string.Empty;
        }

        #endregion
        
        #region Static Members
        
        private static readonly String VarNameParameter = "VN";
                
        
        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);
            
            VarName = helper.GetPartByName(VarNameParameter);
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);
            
            //if (String.IsNullOrEmpty(helper.GetPartByName(VarNameParameter)))
            //    return false;

            // optional parameters
            VarName = helper.GetPartByName(VarNameParameter);

            return true;
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            if (!string.IsNullOrWhiteSpace(VarName))
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", VarNameParameter, DynamicStringParser.CharAssign, VarName);
            }
            return dynamicstring.ToString();
        }

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            //return (ModbusSlaveProtocol.GetMaxJobSize(DataArea) >= ByteSize);
            return true;
        }

        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            return GetNodeDynSetting(thistagdefinition);
        }

        public override string GetFirstDynSetting(Tag tag, TagDefinition thistagdefinition)
        {
            // always reset root value (because not used)
            _VarName = string.Empty;
            TryParse(tag.TagNode.DynamicSettings);
            return GetNodeDynSetting(thistagdefinition);
        }

        string GetNodeDynSetting(TagDefinition thistagdefinition)
        {
            string oldVarName = _VarName;
            string Tree = DIComProtocol.GetNodeTree(thistagdefinition.NodeId, thistagdefinition.Name);
            
            _VarName += Tree.Replace('/', '.');

            // replace some string's pattern to obtain device variable name
            _VarName = DIComProtocol.CorrectMoviconVariableNameToVarName(_VarName);

            string dynsettings = ToString();
            _VarName = oldVarName;            
            return dynsettings;
        }

        #endregion

        #region Properties
        private string _VarName;
        [Category("Var Name")]
        [Description("Var Name")]
        public string VarName
        {
            get { return _VarName; }
            set { _VarName = value; }
        }        
        #endregion

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "VarName")
            {
                if (!IsObjectType)
                {
                    if (String.IsNullOrWhiteSpace(_VarName) || _VarName.Length > DIComProtocol.MAX_VAR_NAME_SIZE)
                        return Properties.Resources.ErrorInvalidVarName;
                }
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
