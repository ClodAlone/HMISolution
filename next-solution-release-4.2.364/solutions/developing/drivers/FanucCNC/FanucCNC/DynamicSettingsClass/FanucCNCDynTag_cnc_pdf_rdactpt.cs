using System;
using System.Text;
using System.ComponentModel;
using DriverCodeBase.Enumerators;

namespace FanucCNC
{
    public class FanucCNCDynTag_cnc_pdf_rdactpt : FanucCNCDynTag_BaseFunction
    {
        public class FC
        {
            public ProgramElement Code { get; set; }
            public string Description { get; set; }

            public FC(ProgramElement code, string description)
            {
                Code = code;
                Description = description;
            }
        }

        const String PPartParameter = "PPART";

        public enum ProgramElement : short
        {
            Name = 0,
            Pointer
        }

        public FanucCNCDynTag_cnc_pdf_rdactpt(FanucCNCDynTagSettings main, string settings) : base(main, settings)
        {
            CNCPathSupported = true;
        }
        public override void SplitSettings(string settings)
        {
            _ProgramPart = (ProgramElement)GetPartByName(PPartParameter, (short)ProgramElement.Name);
            if (_ProgramPart == ProgramElement.Name)
                StringLength = FanucCNCProtocol.MAX_STRING_SIZE_PARAMETER;

            IsValid = true;
        }

        public override string CreateSettings()
        {
            var parameters = new StringBuilder(); // base.ToString());
                        
            AddParameter(ref parameters, PPartParameter, (short)_ProgramPart);
            return parameters.ToString();
        }

        public DriverErrorCodes ParseReadData(byte[] programName, int programPointer, out byte[] data)
        {
            DriverErrorCodes result = DriverErrorCodes.ErrorNoError;

            switch (_ProgramPart)
            {
                case ProgramElement.Name:
                    data = new byte[StringLength];

                    if (programName == null)
                        programName = new byte[StringLength];

                    int programSize = programName.Length;
                    if (programSize > StringLength)
                        programSize = (int)StringLength;
                    
                    Array.Copy(programName, 0, programName, 0, programSize);
                    break;

                case ProgramElement.Pointer:
                    result = ConvertToMoviconDataType(programPointer, out data);
                    break;
                default:
                    data = null;
                    result = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorFunctionNotSupported;
                    break;
            }

            return result;
        }

        #region IDataErrorInfo Members
        public override String PerformValidation(String propertyName)
        {
            switch (propertyName) {
                case "ProgramPart":
                    switch (_ProgramPart)
                    {
                        case ProgramElement.Name:
                            if (Main.VarType != UFUAModel.DataType.String)
                                return Properties.Resources.ErrorOnlyStringDataTypeAllowed;
                        break;

                        case ProgramElement.Pointer:
                            if (Main.VarType == UFUAModel.DataType.String)
                                return Properties.Resources.ErrorInvalidDataFormat;
                            break;
                    }
                    break;
                case "TagLinkType":
                    if (Main.TagLinkType != (int)LinkType.Input)
                        return Properties.Resources.ErrorLinkTypeInvalid;
                    break;
            }

            return null;
        }
        #endregion

        #region Properties
        private ProgramElement _ProgramPart;
        public ProgramElement ProgramPart
        {
            get { return _ProgramPart; }
            set { _ProgramPart = value; }
        }
        #endregion

        #region INotifyPropertyChanged Members
        public override void OnPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "DataType":
                    OnPropertyChanged(new PropertyChangedEventArgs("ProgramPart"));
                    break;
            }
        }
        #endregion
    }
}
