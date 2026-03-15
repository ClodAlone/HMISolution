using System;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Helpers;
using System.ComponentModel;
using DriverBaseInterfaces;

namespace FanucCNC
{
    public sealed class FanucCNCDynTagSettings : DynTagSettings
    {
        #region Constructors
        public FanucCNCDynTagSettings()
            : base()
        {
            _FunctionCode = FanucCNCProtocol.FunctionCode.Unknown;
            _FunctionSettings = null;
            _ParseOk = false;
        }

        #endregion

        #region Static Members

        private const String FunctionCodeParameter = "FOCAFC";
        private const String FunctionSettingsParameter = "FOCAFCS";
        private const String OffsetVariableNameParameter = "FOCAFOVN";
        private const String OffsetVariableIdParameter = "FOCAFCOVID";
        private const String CNCPathParameter = "FOCPATH";

        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);
            _FunctionCode = (FanucCNCProtocol.FunctionCode)(helper.GetPartByName(FunctionCodeParameter, (int)FanucCNCProtocol.FunctionCode.Unknown));
            _FunctionSettings = GetObjectFromSettings(_FunctionCode, helper.GetPartByName(FunctionSettingsParameter));            
            //_OffsetVariableName = helper.GetPartByName(OffsetVariableNameParameter);
            //_OffsetVariableId = helper.GetPartByName(OffsetVariableIdParameter);
            _CNCPath = (short)helper.GetPartByName(CNCPathParameter, 0);

            ParseAddress();
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);

            // required parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(FunctionCodeParameter)))
                return false;

            _FunctionCode = (FanucCNCProtocol.FunctionCode)(helper.GetPartByName(FunctionCodeParameter, (int)FanucCNCProtocol.FunctionCode.Unknown));
            _FunctionSettings = GetObjectFromSettings(_FunctionCode, helper.GetPartByName(FunctionSettingsParameter));
            //_OffsetVariableName = helper.GetPartByName(OffsetVariableNameParameter);
            //_OffsetVariableId = helper.GetPartByName(OffsetVariableIdParameter);
            _CNCPath = (short)helper.GetPartByName(CNCPathParameter, 0);

            return ParseAddress();
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());

            dynamicstring.Append(DynamicStringParser.CharSep);
            dynamicstring.AppendFormat("{0}{1}{2}", FunctionCodeParameter, DynamicStringParser.CharAssign, (int)_FunctionCode);
            if (_FunctionSettings != null && _FunctionSettings.HasSettings)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", FunctionSettingsParameter, DynamicStringParser.CharAssign, _FunctionSettings.ToString());
            }

            //if (!String.IsNullOrEmpty(_OffsetVariableName) && !String.IsNullOrEmpty(_OffsetVariableId))
            //{
            //    dynamicstring.Append(DynamicStringParser.CharSep);
            //    dynamicstring.AppendFormat("{0}{1}{2}", OffsetVariableNameParameter, DynamicStringParser.CharAssign, _OffsetVariableName);
            //    dynamicstring.Append(DynamicStringParser.CharSep);
            //    dynamicstring.AppendFormat("{0}{1}{2}", OffsetVariableIdParameter, DynamicStringParser.CharAssign, _OffsetVariableId);
            //}
            if (_CNCPath != 0)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", CNCPathParameter, DynamicStringParser.CharAssign, _CNCPath);
            }

            return dynamicstring.ToString();
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
            //string memABAddress = Address;
            //FanucCNCProtocol.DataArea memDataArea = _DataArea;

            //string Tree = FanucCNCProtocol.GetNodeTree(thistagdefinition.NodeId, thistagdefinition.Name);
            //Address += ("." + Tree.Replace('/', '.'));
            //_DataArea = FanucCNCProtocol.DataArea.CNC_DATA;
            //    //FanucCNCProtocol.AddressType.Unknown;// FanucCNCProtocol.GetDataFormatFromMoviconDataType((uint)thistagdefinition.DataType.Identifier);
            //_StringLength = 0;
            string dynsettings = ToString();

            //Address = memABAddress;
            //_DataArea = memDataArea;

            return dynsettings;
        }

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (FanucCNCProtocol.MAX_DATA_BYTES >= ByteSize);
        }

        private FanucCNCDynTag_BaseFunction GetObjectFromSettings(FanucCNCProtocol.FunctionCode code, string settings)
        {
            FanucCNCDynTag_BaseFunction result = null;

            switch (code)
            {
                case FanucCNCProtocol.FunctionCode.Func_pmc_rdpmcrng_pmc_wrpmcrng:
                    {
                        FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng obj = new FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng(this, settings);
                        //if (obj.IsValid)
                            result = obj;
                    }
                    break;
                case FanucCNCProtocol.FunctionCode.Func_cnc_actf:
                    {
                        FanucCNCDynTag_cnc_actf obj = new FanucCNCDynTag_cnc_actf(this, settings);
                        //if (obj.IsValid)
                            result = obj;
                    }
                    break;
                case FanucCNCProtocol.FunctionCode.Func_cnc_absolute:
                    {
                        FanucCNCDynTag_cnc_absolute obj = new FanucCNCDynTag_cnc_absolute(this, settings);
                       // if (obj.IsValid)
                            result = obj;
                    }
                    break;
                case FanucCNCProtocol.FunctionCode.Func_cnc_rdaxisdata:
                    {
                        FanucCNCDynTag_cnc_rdaxisdata obj = new FanucCNCDynTag_cnc_rdaxisdata(this, settings);
                        //if (obj.IsValid)
                            result = obj;
                    }
                    break;
                case FanucCNCProtocol.FunctionCode.Func_cnc_rdzofs_cnc_wrzofs:
                    {
                        FanucCNCDynTag_cnc_rdzofs_cnc_wrzofs obj = new FanucCNCDynTag_cnc_rdzofs_cnc_wrzofs(this, settings);
                        //if (obj.IsValid)
                            result = obj;
                    }
                    break;
                case FanucCNCProtocol.FunctionCode.Func_cnc_rdspeed:
                    {
                        FanucCNCDynTag_cnc_rdspeed obj = new FanucCNCDynTag_cnc_rdspeed(this, settings);
                        //if (obj.IsValid)
                            result = obj;
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_rdalmmsg2:
                    {
                        FanucCNCDynTag_cnc_rdalmmsg2 obj = new FanucCNCDynTag_cnc_rdalmmsg2(this, settings);
                        //if (obj.IsValid)
                            result = obj;
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_rdopmsg3:
                    {
                        FanucCNCDynTag_cnc_rdopmsg3 obj = new FanucCNCDynTag_cnc_rdopmsg3(this, settings);
                        //if (obj.IsValid)
                            result = obj;
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_pdf_rdactpt:
                    {
                        FanucCNCDynTag_cnc_pdf_rdactpt obj = new FanucCNCDynTag_cnc_pdf_rdactpt(this, settings);
                        //if (obj.IsValid)
                            result = obj;
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_pdf_rdmain:
                    {
                        FanucCNCDynTag_cnc_pdf_rdmain obj = new FanucCNCDynTag_cnc_pdf_rdmain(this, settings);
                        //if (obj.IsValid)
                            result = obj;
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_rdtofsr_cnc_wrtofsr:
                    {
                        FanucCNCDynTag_cnc_rdtofsr_cnc_wrtofsr obj = new FanucCNCDynTag_cnc_rdtofsr_cnc_wrtofsr(this, settings);
                        //if (obj.IsValid)
                            result = obj;
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_rdparam_cnc_wrparam:
                    {
                        FanucCNCDynTag_cnc_rdparam_cnc_wrparam obj = new FanucCNCDynTag_cnc_rdparam_cnc_wrparam(this, settings);
                        //if (obj.IsValid)
                            result = obj;
                    }
                    break;
                case FanucCNCProtocol.FunctionCode.Func_cnc_statinfo:
                    {
                        FanucCNCDynTag_cnc_statinfo obj = new FanucCNCDynTag_cnc_statinfo(this, settings);
                        //if (obj.IsValid)
                            result = obj;
                    }
                    break;
                //case FanucCNCProtocol.FunctionCode.Func_cnc_setpath:
                //    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_rdsvmeter:
                    {
                        FanucCNCDynTag_cnc_rdsvmeter obj = new FanucCNCDynTag_cnc_rdsvmeter(this, settings);
                        //if (obj.IsValid)
                            result = obj;
                    }
                    break;

                case FanucCNCProtocol.FunctionCode.Func_cnc_getpath_cnc_setpath:
                    {
                        FanucCNCDynTag_cnc_getpath_cnc_setpath obj = new FanucCNCDynTag_cnc_getpath_cnc_setpath(this, settings);
                        //if (obj.IsValid)
                            result = obj;
                    }
                    break;
                //#endregion

                //#region File Management
                //case FanucCNCProtocol.FunctionCode.Func_cnc_rdpdf_alldir:
                //    break;
                //case FanucCNCProtocol.FunctionCode.Func_cnc_pdf_add:
                //    break;
                case FanucCNCProtocol.FunctionCode.Func_cnc_pdf_slctmain:
                    {
                        FanucCNCDynTag_cnc_pdf_slctmain obj = new FanucCNCDynTag_cnc_pdf_slctmain(this, settings);
                        //if (obj.IsValid)
                        result = obj;
                    }
                    break;
                case FanucCNCProtocol.FunctionCode.Func_cnc_pdf_del:
                    {
                        FanucCNCDynTag_cnc_pdf_del obj = new FanucCNCDynTag_cnc_pdf_del(this, settings);
                        //if (obj.IsValid)
                            result = obj;
                    }
                    break;
                case FanucCNCProtocol.FunctionCode.Func_Custom_DownloadProgramToCnc:
                    {
                        FanucCNCDynTag_Custom_DownloadProgramToCnc obj = new FanucCNCDynTag_Custom_DownloadProgramToCnc(this, settings);
                        //if (obj.IsValid)
                            result = obj;
                    }
                    break;
                case FanucCNCProtocol.FunctionCode.Func_Custom_UploadProgramFromCnc:
                    {
                        FanucCNCDynTag_Custom_UploadProgramFromCnc obj = new FanucCNCDynTag_Custom_UploadProgramFromCnc(this, settings);
                        //if (obj.IsValid)
                            result = obj;
                    }
                    break;
            }
            return result;
        }

        //use base definition
        //public override UFUAModel.DataType getProtocolDataType()
        //{            
        //    return VarType;
        //}

        #endregion

        #region Functions


        public bool ParseAddress()
        {

            //if (string.IsNullOrEmpty(_Address)) // || _FanucCNCVarType == FanucCNCProtocol.VarType.VAR_TYPE_E_UNKNOWN)
            //    _ParseOk = false;
            //else
            _ParseOk = (_FunctionSettings != null && ((FanucCNCDynTag_BaseFunction)_FunctionSettings).IsValid);
            //_ParseOk = true;

            return _ParseOk;
        }

        #endregion

        #region Properties       

        private FanucCNCProtocol.FunctionCode _FunctionCode;
        public FanucCNCProtocol.FunctionCode FunctionCode
        {
            get { return _FunctionCode; }
            set { _FunctionCode = value; }
        }

        private FanucCNCDynTag_BaseFunction _FunctionSettings;
        public FanucCNCDynTag_BaseFunction FunctionSettings
        {
            get { return _FunctionSettings; }
            set { _FunctionSettings = value; }
        }

        //private string _OffsetVariableName;
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   Conditional Variable Name. </summary>
        /////
        ///// <value> The name of the conditional value. </value>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ////[Category("General")]
        ////[Description("Conditional Variable Name")]
        //public string OffsetVariableName
        //{
        //    get { return _OffsetVariableName; }
        //    set
        //    {
        //        _OffsetVariableName = value;
        //        OnPropertyChanged("OffsetVariableName");
        //    }
        //}

        ///// <summary>   Node ID of the conditional variable. </summary>
        //private string _OffsetVariableId;
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   Conditional Variable Node Id. </summary>
        /////
        ///// <value> The Node Id of the conditional value. </value>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ////[Category("General")]
        ////[Description("Conditional Variable Node Id")]
        //public string OffsetVariableId
        //{
        //    get { return _OffsetVariableId; }
        //    set
        //    {
        //        _OffsetVariableId = value;
        //        OnPropertyChanged("OffsetVariableId");
        //    }
        //}

        private short _CNCPath;
        public short CNCPath
        {
            get { return _CNCPath; }
            set
            {
                _CNCPath = value;
                OnPropertyChanged("CNCPath"); 
            }
        }

        private bool _ParseOk;
        public bool ParseOk
        {
            get { return _ParseOk; }
            set { _ParseOk = value; }
        }
        #endregion

        #region IDataErrorInfo Members

        public void ForceFunctionSettingsBaseClassValidation()
        {
            OnPropertyChanged("FunctionCode");
            OnPropertyChanged("TagLinkType");
            OnPropertyChanged("DataType");
            OnPropertyChanged("ArrayDimension");
        }

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            switch (propertyName) {
                case "FunctionCode":
                    if (_FunctionCode == FanucCNCProtocol.FunctionCode.Unknown) // || _FunctionCode == FanucCNCProtocol.FunctionCode.Menu_Func_Separator)
                        return Properties.Resources.ErrorFunctionCodeUnknown;

                    // for base property "custom" performvalidation 
                    if (_FunctionSettings != null)
                        return ((FanucCNCDynTag_BaseFunction)_FunctionSettings)[propertyName];
                    break;
                case "TagLinkType":
                case "DataType":
                case "ArrayDimension":
                    // for base property "custom" performvalidation 
                    if (_FunctionSettings != null)
                        return ((FanucCNCDynTag_BaseFunction)_FunctionSettings)[propertyName];
                    break;
                case "CNCPath":
                    if (_CNCPath<0 || _CNCPath>Focas_Library.Focas1.MAX_AXIS)
                        return Properties.Resources.ErrorCNCPathOutOfRange;
                    break;
            }
            return null;
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
