using System;
using System.Collections.Generic;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using FanucCNC.Focas_Library;
using System.Globalization;

namespace FanucCNC
{
    public class FanucCNCDynTag_cnc_rdparam_cnc_wrparam : FanucCNCDynTag_BaseFunction
    {
        const String PNumberParameter = "NUMBER";
        const String PSizeParameter = "SIZE";
        const String AxisNrParameter = "AXISNR";

        public enum ParameterSize : byte
        {
            ByteSize,
            WordSize, // 2byte
            DoubleWordSize, // 4 byte
            RealSize, // 8 byte
        } 

        public FanucCNCDynTag_cnc_rdparam_cnc_wrparam(FanucCNCDynTagSettings main, string settings) : base(main, settings)
        {
            CNCPathSupported = true;
        }
        
        public override void SplitSettings(string settings)
        {
            _PNumber = GetPartByName(PNumberParameter, (short)0);
            _PSize = (ParameterSize)GetPartByName(PSizeParameter, (byte)ParameterSize.ByteSize);
            _AxisNr = GetPartByName(AxisNrParameter, (short)0);

            IsValid = true;

            CalculateInternalParameters();
        }

        public override void CalculateInternalParameters(List<Tag> tagList = null, int offsetValue = 0, bool aggregation = false)
        {
            //_Size = (short)(4 + (8 * _AxisNr));
            _Size = (short)(4 + (GetParameterByteSize(_PSize) * _AxisNr));
        }

        public override string CreateSettings()
        {
            var parameters = new StringBuilder(); // base.ToString());
            AddParameter(ref parameters, PNumberParameter, _PNumber);
            AddParameter(ref parameters, PSizeParameter, (byte)_PSize);
            AddParameter(ref parameters, AxisNrParameter, _AxisNr);
            return parameters.ToString();
        }

        public DriverErrorCodes PrepareWriteRequest(byte[] jobdata, out Focas1.IODBPSD dataToWrite)
        {
            DriverErrorCodes result = DriverErrorCodes.ErrorNoError;

            dataToWrite = new Focas1.IODBPSD();

            switch (_PSize)
            {
                case ParameterSize.ByteSize:
                    result = ConvertFromMoviconDataType(jobdata, out dataToWrite.u.cdatas[0]);
                    break;
                case ParameterSize.WordSize:
                    result = ConvertFromMoviconDataType(jobdata, out dataToWrite.u.idatas[0]);
                    break;
                case ParameterSize.DoubleWordSize:
                    result = ConvertFromMoviconDataType(jobdata, out dataToWrite.u.ldatas[0]);
                    break;
                case ParameterSize.RealSize:

                    int valuePart = 0;
                    int decimalPart = 0;

                    switch (Main.VarType)
                    {
                        case UFUAModel.DataType.Float:
                            if (!SplitFloatingPointInto(Convert.ToSingle(jobdata), out valuePart, out decimalPart))
                            {
                                dataToWrite.u.rdatas[0].prm_val = valuePart * (int)Math.Pow(10, decimalPart) + decimalPart;
                                dataToWrite.u.rdatas[0].dec_val = decimalPart.ToString().Length;
                            } 
                            else
                            {
                                result = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
                            }

                            break;
                        case UFUAModel.DataType.Double:
                            if (!SplitFloatingPointInto(Convert.ToDouble(jobdata), out valuePart, out decimalPart)) {                             
                                dataToWrite.u.rdatas[0].prm_val = valuePart * (int)Math.Pow(10, decimalPart) + decimalPart;
                                dataToWrite.u.rdatas[0].dec_val = decimalPart.ToString().Length;
                            }
                            else
                            {
                                result = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
                            }
                            break;
                        default:
                            result = ConvertFromMoviconDataType(jobdata, out dataToWrite.u.ldatas[0]);
                            break;
                    }
                    break;
            }

            return result; 
        }

        public DriverErrorCodes ParseReadData(Focas_Library.Focas1.IODBPSD dataRead, out byte[] data)
        {
            data = null;

            switch (_PSize)
            {
                case ParameterSize.ByteSize:
                    return ConvertToMoviconDataType(dataRead.u.cdatas[_AxisNr -1], out data);
                case ParameterSize.WordSize:
                    return ConvertToMoviconDataType(dataRead.u.idatas[_AxisNr -1], out data);
                case ParameterSize.DoubleWordSize:
                    return ConvertToMoviconDataType(dataRead.u.ldatas[_AxisNr - 1], out data);
                case ParameterSize.RealSize:
                    switch (Main.VarType)
                    {
                        case UFUAModel.DataType.Float:
                            {
                                if (CreateFloatingPoint(dataRead.u.rdatas[_AxisNr - 1].prm_val, dataRead.u.rdatas[_AxisNr - 1].dec_val, out Single resultValue))
                                    return ConvertToMoviconDataType(resultValue, out data);
                                else
                                    return (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
                                break;
                            }
                        case UFUAModel.DataType.Double:
                            {
                                if (CreateFloatingPoint(dataRead.u.rdatas[_AxisNr - 1].prm_val, dataRead.u.rdatas[_AxisNr - 1].dec_val, out double resultValue))
                                    return ConvertToMoviconDataType(resultValue, out data);
                                else
                                    return (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
                            }
                        default:
                            return ConvertToMoviconDataType(dataRead.u.rdatas[_AxisNr - 1].prm_val, out data);
                    }
            }

            return DriverErrorCodes.ErrorNoError;
        }        

        private uint GetParameterByteSize(ParameterSize pSize)
        {
            uint size = 0;

             switch (pSize) {
                case ParameterSize.ByteSize:
                    size = 1;
                    break;
                case ParameterSize.WordSize: // 2byte
                    size = 2;
                    break;
                case ParameterSize.DoubleWordSize: // 4 byte
                    size = 4;
                    break;
                case ParameterSize.RealSize: // 8 byte
                    size = 8;
                    break;
            }

            return size;
        }

        #region IDataErrorInfo Members
        public override String PerformValidation(String propertyName)
        {
            switch (propertyName) {
                case "FunctionCode":
                    if (Main.VarType == UFUAModel.DataType.String)
                        return Properties.Resources.ErrorInvalidDataFormat;
                    break;

                case "PNumber":
                    if (_PNumber<=0)
                        return string.Format(Properties.Resources.ErrorAddressInvalid, Focas_Library.Focas1.MAX_AXIS);
                    break;

                case "AxisNr":
                    if (_AxisNr<=0 || _AxisNr >= Focas_Library.Focas1.MAX_AXIS)
                        return string.Format(Properties.Resources.ErrorAxisNrInvalid, Focas_Library.Focas1.MAX_AXIS);
                    break;
            }

            return null;
        }
        #endregion

        #region Properties
        private short _PNumber;
        public short PNumber
        {
            get { return _PNumber; }
            set { _PNumber = value; }
        }

        private ParameterSize _PSize;
        public ParameterSize PSize
        {
            get { return _PSize; }
            set { _PSize = value; }
        }

        private short _AxisNr;
        public short AxisNr
        {
            get { return _AxisNr; }
            set { _AxisNr = value; }
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
