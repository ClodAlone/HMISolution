using DriverCodeBase.Enumerators;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace FanucCNC
{
    public class FanucCNCDynTag_Custom_DownloadProgramToCnc : FanucCNCDynTag_BaseFunction
    {
        //const String PCSourcePathParameter = "PCSOURCEPATH";
        const String AddStartEndProgramParameter = "SEPRG";
        const String AddProgramNameParameter = "PNPRGR";
        //const String RemoveRParameter = "RMR";
        const String CNCTargetPathParameter = "CNCTARGETPATH";
        const String ManageTemporaryProgramParameter = "TEMPPRG";
        

        public FanucCNCDynTag_Custom_DownloadProgramToCnc(FanucCNCDynTagSettings main, string settings) : base(main, settings)
        {
            StringLength = FanucCNCProtocol.MAX_DATA_BYTES;
            CNCPathSupported = true;
        }

        public override void SplitSettings(string settings)
        {
            //_PCSourcePath = GetPartByName(PCSourcePathParameter);
            _AddStartEndProgram = GetPartByName(AddStartEndProgramParameter, false);
            _AddProgramName = GetPartByName(AddProgramNameParameter, false);
            //_RemoveR = GetPartByName(RemoveRParameter, false);
            _CNCTargetPath = GetPartByName(CNCTargetPathParameter);
            _ManageTemporaryProgram = GetPartByName(ManageTemporaryProgramParameter,false);

            IsValid = true;
        }

        public override string CreateSettings()
        {
            var parameters = new StringBuilder(); // base.ToString());

            //AddParameter(ref parameters, PCSourcePathParameter, _PCSourcePath.Trim());
            if (_AddStartEndProgram)
                AddParameter(ref parameters, AddStartEndProgramParameter, _AddStartEndProgram);
            if (_AddProgramName)
                AddParameter(ref parameters, AddProgramNameParameter, _AddProgramName);
            //if (_RemoveR)
            //    AddParameter(ref parameters, RemoveRParameter, _RemoveR);
            if (!string.IsNullOrEmpty(_CNCTargetPath))
                AddParameter(ref parameters, CNCTargetPathParameter, _CNCTargetPath.Trim());
            if (_ManageTemporaryProgram)
                AddParameter(ref parameters, ManageTemporaryProgramParameter, _ManageTemporaryProgram);
            return parameters.ToString();
        }

        public DriverErrorCodes PrepareWriteRequest(byte[] jobData, short cnc_path, out string cncProgramPath, out string cncProgramName, out string cncProgramBody)
        {
            cncProgramPath = string.Empty;
            cncProgramName = string.Empty;
            
            cncProgramBody = new ASCIIEncoding().GetString(jobData).TrimEnd('\0');

            //if (_RemoveR)
            //    cncProgramBody = cncProgramBody.Replace("\r", string.Empty);

            if (string.IsNullOrEmpty(cncProgramBody))
                return (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorFileBodyEmpty;

            string path = _CNCTargetPath.Replace("{cnc_path}",cnc_path.ToString());
            if (string.IsNullOrEmpty(path))
                return (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorInvalidCNCPath;

            // is a 
            if (path.EndsWith(@"/"))
            {
                cncProgramPath = path;
                cncProgramName = string.Empty;
            } else
            {
                int last = path.LastIndexOf(@"/");

                cncProgramPath = path.Substring(0,last+1);
                cncProgramName = path.Substring(last + 1, path.Length-last-1);
            }

            if (_AddProgramName)
                cncProgramBody = string.Format("\nO1\n{0}", cncProgramBody);

            if (_AddStartEndProgram)
                cncProgramBody = string.Format("{0}\n%", cncProgramBody);

            return DriverErrorCodes.ErrorNoError;
        }

        #region IDataErrorInfo Members
        public override String PerformValidation(String propertyName)
        {            
            switch (propertyName) {
                case "FunctionCode":
                    if (Main.VarType != UFUAModel.DataType.String)
                        return Properties.Resources.ErrorOnlyStringDataTypeAllowed;
                    break;

                case "CNCTargetPath":
                    if (string.IsNullOrWhiteSpace(_CNCTargetPath))
                        return Properties.Resources.ErrorAddressEmpty;
                    break;

                
                case "TagLinkType":
                    if (Main.TagLinkType == (int)LinkType.Input || Main.TagLinkType == (int)LinkType.InputOutput)
                        return Properties.Resources.ErrorLinkTypeInvalid;
                    break;
            }

            return null;
        }
        #endregion

        #region Properties
        //private string _PCSourcePath;
        //public string PCSourcePath
        //{
        //    get { return _PCSourcePath; }
        //    set { _PCSourcePath = value; }
        //}

        private bool _AddStartEndProgram;
        public bool AddStartEndProgram
        {
            get { return _AddStartEndProgram; }
            set { _AddStartEndProgram = value; }
        }

        private bool _AddProgramName;
        public bool AddProgramName
        {
            get { return _AddProgramName; }
            set { _AddProgramName = value; }
        }

        private string _CNCTargetPath;
        public string CNCTargetPath
        {
            get { return _CNCTargetPath; }
            set { _CNCTargetPath = value; }
        }

        //private bool _RemoveR;
        //public bool RemoveR
        //{
        //    get { return _RemoveR; }
        //    set { _RemoveR = value; }
        //}

        private bool _ManageTemporaryProgram;
        public bool ManageTemporaryProgram
        {
            get { return _ManageTemporaryProgram; }
            set { _ManageTemporaryProgram = value; }
        }     
        #endregion

        #region INotifyPropertyChanged Members
        public override void OnPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "DataType":
                    OnPropertyChanged(new PropertyChangedEventArgs("PCSourcePath"));
                    break;
            }
        }
        #endregion
    }
}
