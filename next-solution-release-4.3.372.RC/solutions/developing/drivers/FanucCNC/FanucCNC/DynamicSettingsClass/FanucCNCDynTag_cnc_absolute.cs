using System;
using System.Collections.Generic;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using FanucCNC.Focas_Library;
using System.ComponentModel;

namespace FanucCNC
{
    public class FanucCNCDynTag_cnc_absolute : FanucCNCDynTag_BaseFunction
    {
        const String AxisNrParameter = "AXISNR";
        
        public FanucCNCDynTag_cnc_absolute(FanucCNCDynTagSettings main, string settings) : base(main, settings)
        {
            CNCPathSupported = false;
        }

        public override void SplitSettings(string settings)
        {
            _AxisNr = GetPartByName(AxisNrParameter, (short)0);

            IsValid = true;

            CalculateInternalParameters();
        }

        public override void CalculateInternalParameters(List<Tag> tagList = null, int offsetValue = 0, bool aggregation = false)
        {
            _MoviconDataTypeSize = FanucCNCProtocol.GetDataTypeSize(Main.VarType);
            // force to read all axis data
            if (_AxisNr == -1)
            {
                if (Main.ArrayDimension == 0)
                    _NrAxis = 1;
                else
                {
                    _NrAxis = (short)Main.ArrayDimension;
                    if (_NrAxis > Focas1.MAX_AXIS)
                        _NrAxis = Focas1.MAX_AXIS;
                }
            }
            else 
            {
                _NrAxis = 1;
            }
            if (_NrAxis == 1)
                _Size = (short)(4 + (1 * 4));
            else
                _Size = (short)(4 + (Focas1.MAX_AXIS * 4));
        }

        public override string CreateSettings()
        {
            var parameters = new StringBuilder(); // base.ToString());

            AddParameter(ref parameters, AxisNrParameter, (int)_AxisNr);
            return parameters.ToString();
        }

        public DriverErrorCodes ParseReadData(Focas1.ODBAXIS dataRead, out byte[] data)
        {
            DriverErrorCodes result = DriverErrorCodes.ErrorNoError;

            data = new byte[_NrAxis * _MoviconDataTypeSize];
            for (int n = 0; n < _NrAxis; n++)
            {
                result = ConvertToMoviconDataType(dataRead.data[n], out byte[] d);
                if (result == DriverErrorCodes.ErrorNoError)
                    Array.Copy(d, 0, data, (n * _MoviconDataTypeSize), _MoviconDataTypeSize);
            }
            
            return result;
        }

        public DriverErrorCodes PrepareReadRequest(out Focas1.ODBAXIS dataToWrite)
        {
            dataToWrite = new Focas1.ODBAXIS();
            //if (_NrAxis == 1)
            //    dataToWrite.data = new int[1];
            //else
            //    dataToWrite.data = new int[16];

            //_Size = (short)(4 + (dataToWrite.data.Length * 4));

            dataToWrite.data = new int[Focas1.MAX_AXIS];
            return DriverErrorCodes.ErrorNoError;
        }

        #region IDataErrorInfo Members
        public override String PerformValidation(String propertyName)
        {            
            switch (propertyName)
            {
                case "FunctionCode":
                    if (Main.ArrayDimension != 0)
                        return string.Format(Properties.Resources.ErrorArrayNotSupported, Focas_Library.Focas1.MAX_AXIS);

                    if (Main.VarType == UFUAModel.DataType.String)
                        return Properties.Resources.ErrorInvalidDataFormat;
                    break;

                case "AxisNr":
                    if (_AxisNr == 0 || _AxisNr > Focas_Library.Focas1.MAX_AXIS)
                        return string.Format(Properties.Resources.ErrorAxisNrInvalid, Focas_Library.Focas1.MAX_AXIS);
                //    break;

                //case "ArrayDimension":
                    //if (Main.ArrayDimension != 0)
                    //    return string.Format(Properties.Resources.ErrorArrayNotSupported, Focas_Library.Focas1.MAX_AXIS);
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
        private short _NrAxis;

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

        int _MoviconDataTypeSize;
        #endregion

        #region INotifyPropertyChanged Members
        public override void OnPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "ArrayDimension":
                    OnPropertyChanged(new PropertyChangedEventArgs("AxisNr"));
                    break;
            }
        }
        #endregion
    }
}
