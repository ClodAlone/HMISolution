using DriverCodeBase.Enumerators;
using System;
using System.ComponentModel;
using System.Text;

namespace FanucCNC
{
    public class FanucCNCDynTag_cnc_pdf_rdmain : FanucCNCDynTag_BaseFunction
    {
        public FanucCNCDynTag_cnc_pdf_rdmain(FanucCNCDynTagSettings main, string settings) : base(main, settings)
        {
            StringLength = FanucCNCProtocol.MAX_STRING_SIZE_PARAMETER;
            CNCPathSupported = true;
        }

        public DriverErrorCodes ParseReadData(byte[] dataRead, out byte[] data)
        {
            data = dataRead;
            return DriverErrorCodes.ErrorNoError;
        }

        public DriverErrorCodes ParseReadData(byte[] dataRead, out string program)
        {
            program = string.Empty;
                
           if (dataRead != null)
                program = new ASCIIEncoding().GetString(dataRead).TrimEnd('\0');

            return DriverErrorCodes.ErrorNoError;
        }

        public DriverErrorCodes PrepareReadRequest(out byte[] dataToWrite)
        {
            dataToWrite = new byte[244];

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

                case "TagLinkType":
                    //if (Main.VarType != UFUAModel.DataType.String)
                    //    return Properties.Resources.ErrorOnlyStringDataTypeAllowed;

                    if (Main.TagLinkType != (int)LinkType.Input)
                        return Properties.Resources.ErrorLinkTypeInvalid;
                    break;
            }

            return null;
        }
        #endregion

        #region INotifyPropertyChanged Members
        public override void OnPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "DataType":
                    OnPropertyChanged(new PropertyChangedEventArgs("TagLinkType"));
                    break;
            }
        }
        #endregion
    }
}
