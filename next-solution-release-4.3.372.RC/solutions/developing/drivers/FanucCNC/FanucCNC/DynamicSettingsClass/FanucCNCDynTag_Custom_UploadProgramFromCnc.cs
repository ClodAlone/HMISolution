using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DriverCodeBase.Enumerators;

namespace FanucCNC
{
    public class FanucCNCDynTag_Custom_UploadProgramFromCnc : FanucCNCDynTag_BaseFunction
    {
        const String CNCSourcePathParameter = "CNCSOURCEPATH";
        const String RemoveStartEndProgramParameter = "SEPRG";
        const String RemoveProgramNameParameter = "RMPRG";
        //const String PCTargetPathParameter = "PCTARGETPATH";

        public FanucCNCDynTag_Custom_UploadProgramFromCnc(FanucCNCDynTagSettings main, string settings) : base(main, settings)
        {
            StringLength = FanucCNCProtocol.MAX_DATA_BYTES;
            CNCPathSupported = true;
        }
        public override void SplitSettings(string settings)
        {
            _CNCSourcePath = GetPartByName(CNCSourcePathParameter);            
            //_PCTargetPath = GetPartByName(PCTargetPathParameter);
            _RemoveStartEndProgram = (bool)GetPartByName(RemoveStartEndProgramParameter,false);
            _RemoveProgramName = (bool)GetPartByName(RemoveProgramNameParameter, false);

            IsValid = true;
        }

        public override string CreateSettings()
        {
            var parameters = new StringBuilder(); // base.ToString());

            AddParameter(ref parameters, CNCSourcePathParameter, _CNCSourcePath.Trim());
            //AddParameter(ref parameters, PCTargetPathParameter, _PCTargetPath.Trim());
            if (_RemoveStartEndProgram)
                AddParameter(ref parameters, RemoveStartEndProgramParameter, _RemoveStartEndProgram);
            if (_RemoveProgramName)
                AddParameter(ref parameters, RemoveProgramNameParameter, _RemoveProgramName);
            return parameters.ToString();
        }

        public DriverErrorCodes PrepareReadRequest(int cncPath, out string path)
        {
            path = _CNCSourcePath.Replace("{cnc_path}", cncPath.ToString());

            return DriverErrorCodes.ErrorNoError;
        }

        #region IDataErrorInfo Members
        public override String PerformValidation(String propertyName)
        {
            switch (propertyName)
            {
                case "FunctionCode":
                    if (Main.VarType != UFUAModel.DataType.String)
                        return Properties.Resources.ErrorOnlyStringDataTypeAllowed;
                    break;

                case "CNCSourcePath":
                    if (string.IsNullOrWhiteSpace(_CNCSourcePath))
                        return Properties.Resources.ErrorCNCFilePathIsEmpty;

                    //if (Main.VarType != UFUAModel.DataType.String)
                    //    return Properties.Resources.ErrorOnlyStringDataTypeAllowed;
                    break;
                //case "PCTargetPath":
                //    //if (string.IsNullOrWhiteSpace(_SourcePathParameter))
                //    //    return Properties.Resources.ErrorAddressEmpty;
                //    break;
                case "TagLinkType":
                    if (Main.TagLinkType != (int)LinkType.Input && Main.TagLinkType != (int)LinkType.InputOutput)
                        return Properties.Resources.ErrorLinkTypeInvalid;
                    break;
            }

            return null;
        }

        public void RemoveStartEndFromProgram(ref byte[] program)
        {
            if (program == null)
                return;

            List<byte> programTemp = program.ToList();
            if (programTemp.Count > 2 && program[0] == FanucCNCProtocol.CNC_PROGRAM_CHAR_START_END)
            {
                programTemp.RemoveAt(0); // remove *
                programTemp.RemoveAt(0); // remove CR
            }

            if (programTemp.Count > 2 && programTemp[programTemp.Count - 1] == FanucCNCProtocol.CNC_PROGRAM_CHAR_START_END)
            {
                programTemp.RemoveAt(programTemp.Count - 1); // remove *
                programTemp.RemoveAt(programTemp.Count - 1); // remove CR
            }
            program = programTemp.ToArray();
        }

        public void RemoveNameFromProgram(ref byte[] program)
        {
            if (program == null)
                return;

            List<byte> programTemp = program.ToList();
            int ind = programTemp.IndexOf(FanucCNCProtocol.CNC_PROGRAM_END_OF_LINE);
            if (ind > 0)
                program = programTemp.Skip(ind + 1).ToArray();
        }

        public DriverErrorCodes ParseReadData(byte[] program, out byte[] data)
        {
            data = program;
            if (RemoveStartEndProgram)
            {
                RemoveStartEndFromProgram(ref data);

                if (RemoveProgramName)
                    RemoveNameFromProgram(ref data);
            }

            return DriverErrorCodes.ErrorNoError;
        }

        //#region Save downloaded progrom from CNC to pc disk
        //if (CNC_ret == Focas1.EW_OK)
        //{
        //    try
        //    {
        //        File.WriteAllText(Path.Combine(param.PCTargetPath, fileName), program.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        retJob = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorDuringSavingDowloadedProgramFromCNC;
        //    }
        //}                                               
        //#endregion

        #endregion

        #region Properties
        private string _CNCSourcePath;
        public string CNCSourcePath
        {
            get { return _CNCSourcePath; }
            set { _CNCSourcePath = value; }
        }

        //private string _PCTargetPath;
        //public string PCTargetPath
        //{
        //    get { return _PCTargetPath; }
        //    set { _PCTargetPath = value; }
        //}
        private bool _RemoveStartEndProgram;
        public bool RemoveStartEndProgram
        {
            get { return _RemoveStartEndProgram; }
            set { _RemoveStartEndProgram = value; }
        }

        private bool _RemoveProgramName;
        public bool RemoveProgramName
        {
            get { return _RemoveProgramName; }
            set { _RemoveProgramName = value; }
        }        
        #endregion

        #region INotifyPropertyChanged Members
        public override void OnPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "DataType":
                    OnPropertyChanged(new PropertyChangedEventArgs("CNCSourcePath"));
                    break;
            }
        }
        #endregion
    }
}
