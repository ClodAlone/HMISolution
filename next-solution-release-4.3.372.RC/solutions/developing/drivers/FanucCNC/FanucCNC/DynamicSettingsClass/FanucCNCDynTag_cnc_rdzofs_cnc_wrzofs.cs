using System;
using System.Collections.Generic;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using FanucCNC.Focas_Library;

namespace FanucCNC
{
    public class FanucCNCDynTag_cnc_rdzofs_cnc_wrzofs : FanucCNCDynTag_BaseFunction
    {                
        const String AxisNrParameter = "AXISNR";
        const String DatoNoParameter = "DATANO";

        public FanucCNCDynTag_cnc_rdzofs_cnc_wrzofs(FanucCNCDynTagSettings main, string settings) : base(main, settings)
        {
            CNCPathSupported = true;
        }

        public override void SplitSettings(string settings)
        {
            _AxisNr = GetPartByName(AxisNrParameter, (short)0);
            _DataNo = GetPartByName(DatoNoParameter, (short)0);

            IsValid = true;

            CalculateInternalParameters();
        }

        public override void CalculateInternalParameters(List<Tag> tagList = null, int offsetValue = 0, bool aggregation = false)
        {
            _Size = 0;
        }

        public override string CreateSettings()
        {
            var parameters = new StringBuilder(); // base.ToString());

            AddParameter(ref parameters, AxisNrParameter, (int)_AxisNr);
            AddParameter(ref parameters, DatoNoParameter, _DataNo);
            return parameters.ToString();
        }

        public DriverErrorCodes PrepareWriteRequest(byte[] jobdata, out Focas1.IODBZOFS dataToWrite)
        {
            dataToWrite = new Focas1.IODBZOFS();
            dataToWrite.datano = _DataNo;
            dataToWrite.type = _AxisNr;
            _Size = 4 * (4 * 1);
            
            return ConvertFromMoviconDataType(jobdata, out dataToWrite.data[0]);
        }

        public DriverErrorCodes ParseReadData(Focas1.IODBZOFS daraRead, out byte[] data)
        {            
            return ConvertToMoviconDataType(daraRead.data[0], out data);
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

                case "DataNo":
                    if (_DataNo < 0)
                        return Properties.Resources.ErrorDataNoInvalid;
                    break;
                case "AxisNr": // no array supported
                    if (_AxisNr <= 0)
                        return (string.Format(Properties.Resources.ErrorAxisNrInvalid, Focas1.MAX_AXIS));
                    break;
            }

            return null;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Type parameter into CNC_RDZOFS and CNC_WRZOFS function
        /// </summary>
        private short _AxisNr;
        public short AxisNr
        {
            get { return _AxisNr; }
            set { _AxisNr = value; }
        }        

        private short _DataNo;
        public short DataNo
        {
            get { return _DataNo; }
            set { _DataNo = value; }
        }

        private short _Size;
        public short Size
        {
            get { return _Size; }
            set { _Size = value; }
        }
        #endregion
    }
}
