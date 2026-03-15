using System;
using System.Text;
using DriverCodeBase.Enumerators;
using FanucCNC.Focas_Library;

namespace FanucCNC
{
    public class FanucCNCDynTag_cnc_rdspeed : FanucCNCDynTag_BaseFunction
    {
        const String FieldParameter = "FIELD";

        public enum ReadSpeedType : ushort
        {
            FeedRate = 0,
            SpindleSpeed
        }

        public FanucCNCDynTag_cnc_rdspeed(FanucCNCDynTagSettings main, string settings) : base(main, settings)
        {
            CNCPathSupported = true;
        }

        public override void SplitSettings(string settings)
        {
            _Field = (ReadSpeedType)(GetPartByName(FieldParameter, (int)ReadSpeedType.FeedRate));

            IsValid = true;
        }

        public override string CreateSettings()
        {
            var parameters = new StringBuilder(); // base.ToString());
            AddParameter(ref parameters, FieldParameter, (int)_Field);
            return parameters.ToString();
        }

        public DriverErrorCodes ParseReadData(Focas1.ODBSPEED dataRead, out byte[] data)
        {
            DriverErrorCodes result = DriverErrorCodes.ErrorNoError;

            Focas1.SPEEDELM element = null;
            data = null;

            switch (_Field)
            {
                case FanucCNCDynTag_cnc_rdspeed.ReadSpeedType.FeedRate:
                    element = dataRead.actf;
                    break;
                case FanucCNCDynTag_cnc_rdspeed.ReadSpeedType.SpindleSpeed:
                    element = dataRead.acts;
                    break;
            }

            if (element != null)
            {
                switch (Main.VarType)
                {
                    case UFUAModel.DataType.Float:
                        {
                            if (CreateFloatingPoint(element.data, element.dec, out Single resultValue))
                                return ConvertToMoviconDataType(resultValue, out data);
                            else
                                return (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
                        }
                    case UFUAModel.DataType.Double:
                        {
                            if (CreateFloatingPoint(element.data, element.dec, out Double resultValue))
                                return ConvertToMoviconDataType(resultValue, out data);
                            else
                                return (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
                        }
                    default:
                        return ConvertToMoviconDataType(element.data, out data);
                }
            }
            else
            {
                result = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
            }

            return result;
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

                //case "Field":
                //    if (_Field == ReadSpeedType.F)
                //        return Properties.Resources.ErrorAddressEmpty;

                //    break;
                case "TagLinkType":
                    if (Main.TagLinkType != (int)LinkType.Input)
                        return Properties.Resources.ErrorLinkTypeInvalid;
                    break;
            }

            return null;
        }
        #endregion

        #region Properties
        private ReadSpeedType _Field;
        public ReadSpeedType Field
        {
            get { return _Field; }
            set { _Field = value; }
        }

        public short NrData
        {
            get { return -1; }
        }
        #endregion
    }
}
