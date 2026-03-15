using System;
using DriverCodeBase.Enumerators;

namespace FanucCNC
{
    public class FanucCNCDynTag_cnc_getpath_cnc_setpath : FanucCNCDynTag_BaseFunction
    {        
        public FanucCNCDynTag_cnc_getpath_cnc_setpath(FanucCNCDynTagSettings main, string settings) : base(main, settings)
        {
        }

        public DriverErrorCodes PrepareWriteRequest(byte[] jobdata, out short dataToWrite)
        {
            return ConvertFromMoviconDataType(jobdata, out dataToWrite);
        }

        public DriverErrorCodes ParseReadData(short cnc_path, out byte[] data)
        {
            return ConvertToMoviconDataType(cnc_path, out data);            
        }

        #region IDataErrorInfo Members
        public override String PerformValidation(String propertyName)
        {
            switch (propertyName)
            {
                case "FunctionCode":
                    if (Main.VarType == UFUAModel.DataType.String)
                        return Properties.Resources.ErrorInvalidDataFormat;

                    if (Main.ArrayDimension != 0)
                        return Properties.Resources.ErrorArrayNotSupported;
                    break;
            }

            return null;
        }
        #endregion
    }
}
