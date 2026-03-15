using System;
using DriverCodeBase.Enumerators;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DriverCodeBase;
using FanucCNC.Focas_Library;

namespace FanucCNC
{
    public class FanucCNCDynTag_cnc_rdaxisdata : FanucCNCDynTag_BaseFunction
    {
        const String ClassParameter = "CLASS";
        const String TypeDataParameter = "TYPEDATA";
        const String AxisNrParameter = "AXISNR";

        public class FC
        {
            public short Code { get; set; }
            public string Description { get; set; }

            public FC(short code, string description)
            {
                Code = code;
                Description = description;
            }
        }
                
        public FanucCNCDynTag_cnc_rdaxisdata(FanucCNCDynTagSettings main, string settings) : base(main, settings)
        {
            CNCPathSupported = true;
        }        

        public override void SplitSettings(string settings)
        {
            _Class = GetPartByName(ClassParameter, GetClassList()[0].Code);
            _TypeData = GetPartByName(TypeDataParameter, (short)0);
            _AxisNr = GetPartByName(AxisNrParameter, (short)0);

            IsValid = true;

            CalculateInternalParameters();
        }

        public override void CalculateInternalParameters(List<Tag> tagList = null, int offsetValue = 0, bool aggregation = false)
        {
            if (Main.ArrayDimension != 0)
            {
                _StartElement = 0;
                _NrElements = (short)Main.ArrayDimension;
            }
            else
            {
                _StartElement = (short)(_AxisNr - 1);
                _NrElements = 1;
            }
        }

        public override string CreateSettings()
        {
            var parameters = new StringBuilder(); // base.ToString());

            AddParameter(ref parameters, ClassParameter, _Class);
            AddParameter(ref parameters, TypeDataParameter, _TypeData);
            AddParameter(ref parameters, AxisNrParameter, (int)_AxisNr);
            return parameters.ToString();
        }

        public DriverErrorCodes ParseReadData(Focas_Library.Focas1.ODBAXDT dataRead, short nrElements, out byte[] data)
        {
            DriverErrorCodes result = DriverErrorCodes.ErrorNoError;

            // not mananged for nrElements > 1
            //nrElements

            int moviconDataTypeSize = FanucCNCProtocol.GetDataTypeSize(Main.VarType);
            data = new byte[_NrElements * moviconDataTypeSize];
            for (int n = _StartElement; n < (_StartElement + _NrElements); n++) {
                Focas_Library.Focas1.ODBAXDT_data dataN = null;
                switch (n + 1) {
                    case 1:
                        dataN = dataRead.data1;
                        break;
                    case 2:
                        dataN = dataRead.data2;
                        break;
                    case 3:
                        dataN = dataRead.data3;
                        break;
                    case 4:
                        dataN = dataRead.data4;
                        break;
                    case 5:
                        dataN = dataRead.data5;
                        break;
                    case 6:
                        dataN = dataRead.data6;
                        break;
                    case 7:
                        dataN = dataRead.data7;
                        break;
                    case 8:
                        dataN = dataRead.data8;
                        break;
                    case 9:
                        dataN = dataRead.data9;
                        break;
                    case 10:
                        dataN = dataRead.data10;
                        break;
                    case 11:
                        dataN = dataRead.data11;
                        break;
                    case 12:
                        dataN = dataRead.data12;
                        break;
                    case 13:
                        dataN = dataRead.data13;
                        break;
                    case 14:
                        dataN = dataRead.data14;
                        break;
                    case 15:
                        dataN = dataRead.data15;
                        break;
                    case 16:
                        dataN = dataRead.data16;
                        break;
                }

                if (dataN != null)
                {
                    byte[] d = null;
                    switch (Main.VarType)
                    {
                        case UFUAModel.DataType.Float:
                            {
                                if (CreateFloatingPoint(dataN.data, dataN.dec, out Single resultValue))
                                    result = ConvertToMoviconDataType(resultValue, out d);
                                else
                                    result = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
                                break;
                            }
                        case UFUAModel.DataType.Double:
                            {
                                if (CreateFloatingPoint(dataN.data, dataN.dec, out double resultValue))
                                    result = ConvertToMoviconDataType(resultValue, out d);
                                else
                                    result = (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
                                break;
                            }
                        default:
                            result = ConvertToMoviconDataType(dataN.data, out d);
                            break;
                    }                    
                    if (result == DriverErrorCodes.ErrorNoError)
                        Array.Copy(d, 0, data, ((n - _StartElement) * moviconDataTypeSize), moviconDataTypeSize);
                }
            }
            
            return result;
        }

        public DriverErrorCodes PrepareReadRequest(out short cnc_Len)
        {
            cnc_Len = (short)(_StartElement + _NrElements);

            return DriverErrorCodes.ErrorNoError;
        }

        public List<FC> GetClassList()
        {
            List<FC> lst = new List<FanucCNCDynTag_cnc_rdaxisdata.FC>();

            lst.Add(new FC(1, "Position Value (1)"));
            lst.Add(new FC(2, "Servo (2)"));
            lst.Add(new FC(3, "Spindle (3)"));
            lst.Add(new FC(4, "Selected Spindle (4)"));
            lst.Add(new FC(5, "Speed (5)"));

            return lst;
        }

        public List<FC> GetTypeDataList(short classCode)
        {
            return GetTypeData(classCode).Values.ToList();
        }

        private Dictionary<short,FC> GetTypeData(short classCode)
        {
            Dictionary<short, FC> lst = new Dictionary<short, FC>();

            switch (classCode)
            {
                case 1: // Position value
                    lst.Add(0, new FC(0, "Absolute position (0)"));
                    lst.Add(1, new FC(1, "Machine position (1)"));
                    lst.Add(2, new FC(2, "Relative position (2)"));
                    lst.Add(3, new FC(3, "Distance to go (3)"));
                    lst.Add(4, new FC(4, "Handle interruption(Input unit) (4)"));
                    lst.Add(5, new FC(5, "Handle interruption(Output unit) (5)"));
                    lst.Add(6, new FC(6, "Start point of program restart (6)"));
                    lst.Add(7, new FC(7, "Distance to go of program restart (7)"));
                    lst.Add(8, new FC(8, "Start point of block restart (8)"));
                    lst.Add(9, new FC(9, "Distance to go of block restart (9)"));
                    break;
                case 2: // Servo
                    lst.Add(0, new FC(0, "Servo load meter (0)"));
                    lst.Add(1, new FC(1, "Load current (% unit) (1)"));
                    lst.Add(2, new FC(2, "Load current (Ampere unit) (2)"));
                    break;
                case 3: // 3 (Spindle) and cls = 4 (Selected spindle)
                case 4: // 3 (Spindle) and cls = 4 (Selected spindle)
                    lst.Add(0, new FC(0, "Spindle load meter (0)"));
                    lst.Add(1, new FC(1, "Spindle motor speed (1)"));
                    lst.Add(2, new FC(2, "Spindle speed according to parameter 3799#2 (2)"));
                    lst.Add(3, new FC(3, "Spindle speed got from Spindle motor speed (3)"));
                    lst.Add(4, new FC(4, "Spindle load meter average of each 250ms (4)"));
                    lst.Add(5, new FC(5, "Spindle load meter maximum value (5)"));
                    lst.Add(6, new FC(6, "Spindle load meter maximum value, average of each 250ms (6)"));
                    lst.Add(7, new FC(7, "Time that spindle can continue processing (7"));
                    break;
                case 5: // (Speed)
                    lst.Add(0, new FC(0, "Feed rate(F)(Feed per minute) (0)"));
                    lst.Add(1, new FC(1, "Spindle speed(S) (1)"));
                    lst.Add(2, new FC(2, "Jog speed / Dry run speed (2)"));
                    lst.Add(3, new FC(3, "Tool tip speed (3"));
                    lst.Add(4, new FC(4, "Rotation speed of servo motor (4)"));
                    lst.Add(5, new FC(5, "Feed rate(F / S) Note (5"));
                    break;
            }
            return lst;
        }

        #region IDataErrorInfo Members
        public override String PerformValidation(String propertyName)
        {            
            switch (propertyName) {
                case "FunctionCode":
                    if (Main.VarType == UFUAModel.DataType.String)
                        return Properties.Resources.ErrorInvalidDataFormat;

                    if (Main.ArrayDimension > Focas_Library.Focas1.MAX_AXIS)
                        return string.Format(Properties.Resources.ErrorArraySizeDimensionExceedNrMaxAxis, Focas_Library.Focas1.MAX_AXIS);

                    //if (Main.VarType == UFUAModel.DataType.String)
                    //    return Properties.Resources.ErrorInvalidDataFormat;
                    break;
                //    case "Address":
                //        if (string.IsNullOrWhiteSpace(_Address))
                //            return Properties.Resources.ErrorAddressEmpty;

                //        if (!IsValidAddress(Address, Main.VarType, Main.ArrayDimension, out string errorMessage))
                //            return string.Format(Properties.Resources.ErrorInvalidAddress, errorMessage);
                //        break;

                case "AxisNr": // no array supported
                    if (_AxisNr <= 0)
                        return (string.Format(Properties.Resources.ErrorAxisNrInvalid, Focas1.MAX_AXIS));
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
        private short _Class;
        public short Class
        {
            get { return _Class; }
            set { _Class = value; }
        }

        private short _TypeData;
        public short TypeData
        {
            get { return _TypeData; }
            set { _TypeData = value; }
        }

        private short _StartElement;
        private short _NrElements;
        
        public short Size
        {
            get { return 1; }            
        }

        private short _AxisNr;
        public short AxisNr
        {
            get { return _AxisNr; }
            set { _AxisNr = value; }
        }
        #endregion
    }
}
