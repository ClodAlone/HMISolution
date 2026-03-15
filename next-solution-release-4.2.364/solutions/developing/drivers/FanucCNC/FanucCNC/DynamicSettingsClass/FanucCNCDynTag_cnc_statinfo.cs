using DriverCodeBase.Enumerators;
using System;
using System.Text;

namespace FanucCNC
{
    public class FanucCNCDynTag_cnc_statinfo : FanucCNCDynTag_BaseFunction
    {
        const String FieldParameter = "FIELD";

        public enum FieldType : int
        {
            Tmmode = 0,
            Aut,
            Run,
            Motion,
            Mstb,
            Emergency,
            Alarm,
            Edit,
        }

        public FanucCNCDynTag_cnc_statinfo(FanucCNCDynTagSettings main, string settings) : base(main, settings)
        {
            CNCPathSupported = true;
        }

        public override void SplitSettings(string settings)
        {
            _Field = (FieldType)(GetPartByName(FieldParameter, (int)FieldType.Tmmode));

            IsValid = true;
        }

        public override string CreateSettings()
        {
            var parameters = new StringBuilder(); // base.ToString());
            //parameters.AppendFormat("{0}{1}{2}{3}", FieldParameter, CharAssign, (int)_Field, CharSep);
            AddParameter(ref parameters, FieldParameter, (int)_Field);
            return parameters.ToString();
        }

        public DriverErrorCodes ParseReadData(Focas_Library.Focas1.ODBST dataRead, out byte[] data)
        {
            DriverErrorCodes result = DriverErrorCodes.ErrorNoError;

            switch (_Field)
            {
                case FanucCNCDynTag_cnc_statinfo.FieldType.Tmmode:
                    result = ConvertToMoviconDataType(dataRead.tmmode, out data);
                    break;
                case FanucCNCDynTag_cnc_statinfo.FieldType.Aut:
                    result = ConvertToMoviconDataType(dataRead.aut, out data);
                    break;
                case FanucCNCDynTag_cnc_statinfo.FieldType.Run:
                    result = ConvertToMoviconDataType(dataRead.run, out data);
                    break;
                case FanucCNCDynTag_cnc_statinfo.FieldType.Edit:
                    result = ConvertToMoviconDataType(dataRead.edit, out data);
                    break;
                case FanucCNCDynTag_cnc_statinfo.FieldType.Motion:
                    result = ConvertToMoviconDataType(dataRead.motion, out data);
                    break;
                case FanucCNCDynTag_cnc_statinfo.FieldType.Mstb:
                    result = ConvertToMoviconDataType(dataRead.mstb, out data);
                    break;
                case FanucCNCDynTag_cnc_statinfo.FieldType.Emergency:
                    result = ConvertToMoviconDataType(dataRead.emergency, out data);
                    break;
                case FanucCNCDynTag_cnc_statinfo.FieldType.Alarm:
                    result = ConvertToMoviconDataType(dataRead.alarm, out data);
                    break;
                default:
                    result = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorFunctionNotSupported;
                    data = null;
                    break;
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
                //    if (_Field == FieldType.Unknown)
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
        private FieldType _Field;
        public FieldType Field
        {
            get { return _Field; }
            set { _Field = value; }
        }
        #endregion
    }
}
