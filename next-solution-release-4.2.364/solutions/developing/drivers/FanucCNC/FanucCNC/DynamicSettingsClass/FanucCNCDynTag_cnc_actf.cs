using DriverCodeBase.Enumerators;
using System;

namespace FanucCNC
{
    public class FanucCNCDynTag_cnc_actf : FanucCNCDynTag_BaseFunction
    {
        public FanucCNCDynTag_cnc_actf(FanucCNCDynTagSettings main, string settings) : base(main, settings)
        {
            CNCPathSupported = true;
        }

        public DriverErrorCodes ParseReadData(Focas_Library.Focas1.ODBACT dataRead, out byte[] data)
        {            
            return ConvertToMoviconDataType(dataRead.data, out data);
        }

        #region IDataErrorInfo Members
        public override String PerformValidation(String propertyName)
        {
            switch (propertyName)
            {
                case "FunctionCode":
                    if (Main.VarType == UFUAModel.DataType.String)
                        return Properties.Resources.ErrorInvalidDataFormat;
                    break;

                case "TagLinkType":
                    if (Main.TagLinkType != (int)LinkType.Input)
                        return Properties.Resources.ErrorLinkTypeInvalid;
                    break;
            }

            return null;
        }
        #endregion
    }
}
