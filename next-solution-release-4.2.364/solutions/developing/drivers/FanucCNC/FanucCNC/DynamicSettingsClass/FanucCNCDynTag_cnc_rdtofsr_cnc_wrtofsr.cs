using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using FanucCNC.Focas_Library;

namespace FanucCNC
{
    public class FanucCNCDynTag_cnc_rdtofsr_cnc_wrtofsr : FanucCNCDynTag_BaseFunction
    {
        public class FC
        {
            public ToolsOffsetMemory Code { get; set; }
            public string Description { get; set; }

            public FC(ToolsOffsetMemory code, string description)
            {
                Code = code;
                Description = description;
            }
        }

        public class TypeParameter
        {
            public short Code { get; set; }
            public string Description { get; set; }
            public string FieldName { get; set; }
            public short Size { get; set; }

            public TypeParameter(short code, string description, string fieldName ,short size)
            {
                Code = code;
                Description = description;
                Size = size;
                FieldName = fieldName;
            }
        }

        public enum ToolsOffsetMemory : short
        {
            Memory_A = 0,
            Memory_B,
            Memory_C,
            Lathe_Memory_C,
            Lathe_SecondGeometry,

            Memory_A_30x = 3000,
            Memory_B_30x,
            Memory_C_30x_WhenTheToolOffsetForMillingAndTurningFunctionOptionIsInvalid,
            Memory_C_30x_WhenTheToolOffsetForMillingAndTurningFunctionOptionIsValid,
            Lathe_Memory_B_30x,
            Coner_R_30x,
            Lathe_WithoutGeometoryWearOffset2ndcoordinate_30x,
            Lathe_GeometryWareOffset2ndCoordinate_30x,
            Lathe_SecondGeometry2ndCoordinate_30x,
            Lathe_4th5thAxisOffsetFunctionWithoutGeometoryWwearOffset_30x,
            Lathe_4th5thAxisOffsetFunctionGeometoryWearOffset_30x,
        }
        
        const String S_NumberParameter = "S_NUMBER";
        const String OffsetMemoryTypeParameter = "OFFMEMTYPE";
        const String OffsetTypeParameter = "OFFTYPE";

        public FanucCNCDynTag_cnc_rdtofsr_cnc_wrtofsr(FanucCNCDynTagSettings main, string settings) : base(main, settings)
        {
            CNCPathSupported = true;
        }

        public override void SplitSettings(string settings)
        {
            _S_Number = GetPartByName(S_NumberParameter, (short)0);
            _OffsetMemoryType = (ToolsOffsetMemory)GetPartByName(OffsetMemoryTypeParameter, (ushort)ToolsOffsetMemory.Memory_A);
            _OffsetType = GetPartByName(OffsetTypeParameter, _OffsetType);

            IsValid = true;

            CalculateInternalParameters();
        }

        public override void CalculateInternalParameters(List<Tag> tagList = null, int offsetValue = 0, bool aggregation = false)
        {
            _E_Number = _S_Number;
            _N = 1;
            TypeParameter param = GetTypeParameter(_OffsetMemoryType, _OffsetType);
            if (param != null)
            {
                _Size = (short)(8 + _N * param.Size);
                _FieldName = param.FieldName;
            }
        }

        public List<FC> GetListMemoryType()//FanucCNCProtocol.MachineSeries machineSerie)
        {
            List<FC> lst = new List<FC>();

            switch (MachineModel)
            {
                case FanucCNCProtocol.MachineSeries.Serie0iB:
                    lst.Add(new FC(ToolsOffsetMemory.Memory_A, "Memory A"));
                    lst.Add(new FC(ToolsOffsetMemory.Memory_B, "Memory B"));
                    lst.Add(new FC(ToolsOffsetMemory.Memory_C, "Memory C"));
                    lst.Add(new FC(ToolsOffsetMemory.Lathe_Memory_C, "Lathe Memory C"));
                    lst.Add(new FC(ToolsOffsetMemory.Lathe_SecondGeometry, "Lathe SecondGeometry"));
                    break;
                case FanucCNCProtocol.MachineSeries.Serie30iB:
                case FanucCNCProtocol.MachineSeries.Serie31iB:
                case FanucCNCProtocol.MachineSeries.Serie35iB:
                    lst.Add(new FC(ToolsOffsetMemory.Memory_A_30x, "Memory A"));
                    lst.Add(new FC(ToolsOffsetMemory.Memory_B_30x, "Memory B"));
                    lst.Add(new FC(ToolsOffsetMemory.Memory_C_30x_WhenTheToolOffsetForMillingAndTurningFunctionOptionIsInvalid, "Memory C WhenTheToolOffsetForMillingAndTurningFunctionOptionIsInvalid"));
                    lst.Add(new FC(ToolsOffsetMemory.Memory_C_30x_WhenTheToolOffsetForMillingAndTurningFunctionOptionIsValid, "Memory C WhenTheToolOffsetForMillingAndTurningFunctionOptionIsValid"));
                    lst.Add(new FC(ToolsOffsetMemory.Lathe_Memory_B_30x, "Lathe Memory B"));
                    lst.Add(new FC(ToolsOffsetMemory.Coner_R_30x, "Coner R"));
                    lst.Add(new FC(ToolsOffsetMemory.Lathe_WithoutGeometoryWearOffset2ndcoordinate_30x, "Lathe Without Geometory/wear offset(2nd coordinate)"));
                    lst.Add(new FC(ToolsOffsetMemory.Lathe_GeometryWareOffset2ndCoordinate_30x, "Lathe Geometry/ware offset(2nd coordinate)"));
                    lst.Add(new FC(ToolsOffsetMemory.Lathe_SecondGeometry2ndCoordinate_30x, "Lathe Second geometry(2nd Coordinate)"));
                    lst.Add(new FC(ToolsOffsetMemory.Lathe_4th5thAxisOffsetFunctionWithoutGeometoryWwearOffset_30x, "4th/5th Axis Offset Function/Without Geometory/wear offset"));
                    lst.Add(new FC(ToolsOffsetMemory.Lathe_4th5thAxisOffsetFunctionGeometoryWearOffset_30x, "4th/5th Axis Offset Function/Geometory/wear offset"));
                    break;
            }

            return lst;
        }

        private TypeParameter GetTypeParameter(ToolsOffsetMemory type, short parameterCode)
        {
            Dictionary<short, TypeParameter> lst = getListTypeParameter(type);

            if (lst != null)
            {
                if (lst.ContainsKey(parameterCode))
                    return lst[parameterCode];
            }

            return null;
        }


        public List<TypeParameter> GetListTypeParameter(ToolsOffsetMemory type)
        {
            return getListTypeParameter(type).Values.ToList();
        }
        
        private Dictionary<short, TypeParameter> getListTypeParameter(ToolsOffsetMemory type)
        {
            Dictionary<short, TypeParameter> lst = new Dictionary<short, TypeParameter>();

            switch (type)
            {
                case ToolsOffsetMemory.Memory_A:
                    lst.Add(0, new TypeParameter(0, "Tool offset (0)", "m_ofs", 4));
                    break;
                case ToolsOffsetMemory.Memory_B:
                    lst.Add(0, new TypeParameter(0, "Tool geometry offset (0)", "m_ofs", 4));
                    lst.Add(1, new TypeParameter(1, "Tool wear offset (1)", "m_ofs", 4));
                    break;
                case ToolsOffsetMemory.Memory_C:
                    lst.Add(0, new TypeParameter(0, "Tool length/geometry (0)", "m_ofs", 4));
                    lst.Add(1, new TypeParameter(1, "Tool length/wear (1)", "m_ofs", 4));
                    lst.Add(2, new TypeParameter(2, "Cutter radius/geometry (2)", "m_ofs", 4));
                    lst.Add(3, new TypeParameter(3, "Cutter radius/wear (3)", "m_ofs", 4));
                    break;
                case ToolsOffsetMemory.Lathe_Memory_C:
                    lst.Add(0, new TypeParameter(0, "Direction of imaginary tool nose (0)", "t_tip", 2));
                    lst.Add(1, new TypeParameter(1, "X-axis offset (1)", "t_ofs", 4));
                    lst.Add(2, new TypeParameter(2, "Y-axis offset (2)", "t_ofs", 4));
                    lst.Add(3, new TypeParameter(3, "Z-axis offset (3)", "t_ofs", 4));
                    lst.Add(4, new TypeParameter(4, "Nose radius offset (4)", "t_ofs", 4));
                    break;
                case ToolsOffsetMemory.Lathe_SecondGeometry:
                    lst.Add(100, new TypeParameter(100, "X-axis offset (100)", "t_ofs", 4));
                    lst.Add(101, new TypeParameter(101, "Y-axis offset (101)", "t_ofs", 4));
                    lst.Add(102, new TypeParameter(102, "Z-axis offset (102)", "t_ofs", 4));
                    break;

                case ToolsOffsetMemory.Memory_A_30x:
                    lst.Add(0, new TypeParameter(0, "Tool offset (0)", "m_ofs", 4));
                    lst.Add(9, new TypeParameter(9, "Direction of imaginary tool nose (9)", "t_tip", 2));
                    break;
                case ToolsOffsetMemory.Memory_B_30x:
                    lst.Add(0, new TypeParameter(0, "Tool geometry offset (0)", "m_ofs", 4));
                    lst.Add(1, new TypeParameter(1, "Tool wear offset (1)", "m_ofs", 4));
                    lst.Add(9, new TypeParameter(9, "Direction of imaginary tool nose (9)", "t_tip", 2));
                    break;
                case ToolsOffsetMemory.Memory_C_30x_WhenTheToolOffsetForMillingAndTurningFunctionOptionIsInvalid:
                    lst.Add(0, new TypeParameter(0, "Tool length/geometry (0)", "m_ofs", 4));
                    lst.Add(1, new TypeParameter(1, "Tool length/wear (1)", "m_ofs", 4));
                    lst.Add(2, new TypeParameter(2, "Cutter radius/geometry (2)", "m_ofs", 4));
                    lst.Add(3, new TypeParameter(3, "Cutter radius/wear (3)", "m_ofs", 4));
                    lst.Add(9, new TypeParameter(9, "Direction of imaginary tool nose (9)", "t_tip", 2));
                    break;
                case ToolsOffsetMemory.Memory_C_30x_WhenTheToolOffsetForMillingAndTurningFunctionOptionIsValid:
                    lst.Add(0, new TypeParameter(0, "Direction of imaginary tool nose (0)", "t_tip", 2));
                    lst.Add(1, new TypeParameter(1, "X-axis geometry offset (1)", "t_ofs", 4));
                    lst.Add(2, new TypeParameter(2, "Y-axis geometry offset (2)", "t_ofs", 4));
                    lst.Add(3, new TypeParameter(3, "Tool length/geometry (3)", "t_ofs", 4));
                    lst.Add(4, new TypeParameter(4, "Cutter radius/geometry (4)", "t_ofs", 4));
                    lst.Add(5, new TypeParameter(5, "X-axis wear offset (5)", "t_ofs", 4));
                    lst.Add(6, new TypeParameter(6, "Y-axis wear offset (6)", "t_ofs", 4));
                    lst.Add(7, new TypeParameter(7, "Tool length/wear (7)", "t_ofs", 4));
                    lst.Add(8, new TypeParameter(8, "Cutter radius/wear (8)", "t_ofs", 4));
                    break;
                case ToolsOffsetMemory.Lathe_Memory_B_30x:
                    lst.Add(0, new TypeParameter(0, "Direction of imaginary tool nose (0)", "t_tip", 2));
                    lst.Add(1, new TypeParameter(1, "X-axis offset (1)", "t_ofs", 4));
                    lst.Add(2, new TypeParameter(2, "Y-axis offset (2)", "t_ofs", 4));
                    lst.Add(3, new TypeParameter(3, "Z-axis offset (3)", "t_ofs", 4));
                    lst.Add(4, new TypeParameter(4, "Nose radius offset (4)", "t_ofs", 4));
                    lst.Add(5, new TypeParameter(5, "X-axis wear offset (5)", "t_ofs", 4));
                    lst.Add(6, new TypeParameter(6, "Y-axis wear offset (6)", "t_ofs", 4));
                    lst.Add(7, new TypeParameter(7, "Z-axis wear offset (7)", "t_ofs", 4));
                    lst.Add(8, new TypeParameter(8, "Nose radius wear offset (8)", "t_ofs", 4));
                    break;
                case ToolsOffsetMemory.Coner_R_30x:
                    lst.Add(10, new TypeParameter(10, "Corner R/geometry (10)", "t_ofs", 4));
                    lst.Add(11, new TypeParameter(11, "Corner R/wear (11)", "t_ofs", 4));
                    break;
                case ToolsOffsetMemory.Lathe_WithoutGeometoryWearOffset2ndcoordinate_30x:
                    lst.Add(20, new TypeParameter(20, "Direction of imaginary tool nose (20)", "t_tip", 2));
                    lst.Add(21, new TypeParameter(21, "X-axis offset (21)", "t_ofs", 4));
                    lst.Add(22, new TypeParameter(22, "Y-axis offset (22)", "t_ofs", 4));
                    lst.Add(23, new TypeParameter(23, "Z-axis offset (23)", "t_ofs", 4));
                    lst.Add(24, new TypeParameter(24, "Nose radius offset (24)", "t_ofs", 4));
                    break;
                case ToolsOffsetMemory.Lathe_GeometryWareOffset2ndCoordinate_30x:
                    lst.Add(20, new TypeParameter(20, "Direction of imaginary tool nose (20)", "t_tip", 2));
                    lst.Add(21, new TypeParameter(21, "X-axis offset (21)", "t_ofs", 4));
                    lst.Add(22, new TypeParameter(22, "Y-axis offset (22)", "t_ofs", 4));
                    lst.Add(23, new TypeParameter(23, "X-axis offset (23)", "t_ofs", 4));
                    lst.Add(24, new TypeParameter(24, "Nose radius offset (24)", "t_ofs", 4));
                    lst.Add(25, new TypeParameter(25, "X-axis wear offset (25)", "t_ofs", 4));
                    lst.Add(26, new TypeParameter(26, "Y-axis wear offset (26)", "t_ofs", 4));
                    lst.Add(27, new TypeParameter(27, "X-axis wear offset (27)", "t_ofs", 4));
                    lst.Add(28, new TypeParameter(28, "Nose radius wear offset (28)", "t_ofs", 4));
                    break;
                case ToolsOffsetMemory.Lathe_SecondGeometry2ndCoordinate_30x:
                    lst.Add(120, new TypeParameter(120, "X-axis offset (120)", "t_ofs", 4));
                    lst.Add(121, new TypeParameter(121, "Y-axis offset (121)", "t_ofs", 4));
                    lst.Add(122, new TypeParameter(122, "X-axis offset (122)", "t_ofs", 4));
                    break;

                case ToolsOffsetMemory.Lathe_4th5thAxisOffsetFunctionWithoutGeometoryWwearOffset_30x:
                    lst.Add(12, new TypeParameter(12, "4th axis offset (12)", "t_ofs", 4));
                    lst.Add(14, new TypeParameter(14, "5th axis offset (14)", "t_ofs", 4));
                    break;
                case ToolsOffsetMemory.Lathe_4th5thAxisOffsetFunctionGeometoryWearOffset_30x:
                    lst.Add(12, new TypeParameter(12, "4th axis geometory offset (12)", "t_ofs", 4));
                    lst.Add(13, new TypeParameter(13, "4th axis offset (13)", "t_ofs", 4));
                    lst.Add(14, new TypeParameter(14, "5th axis geometory offset (14)", "t_ofs", 4));
                    lst.Add(15, new TypeParameter(15, "5th axis offset (15)", "t_ofs", 4));
                    break;
            }

            return lst;
        }       

        public override string CreateSettings()
        {
            var parameters = new StringBuilder(); // base.ToString());

            AddParameter(ref parameters, S_NumberParameter, _S_Number);            
            AddParameter(ref parameters, OffsetMemoryTypeParameter, (short)_OffsetMemoryType);
            AddParameter(ref parameters, OffsetTypeParameter, _OffsetType);
            return parameters.ToString();
        }


        public DriverErrorCodes PrepareWriteRequest(byte[] jobdata, out Focas1.IODBTO_1_1 dataToWrite)
        {
            DriverErrorCodes result = DriverErrorCodes.ErrorNoError;

            dataToWrite = new Focas1.IODBTO_1_1();
            dataToWrite.datano_s = _S_Number;
            dataToWrite.datano_e = _E_Number;
            dataToWrite.type = (short)_OffsetType;
            switch (_FieldName)
            {
                case "m_ofs":
                    {
                        result = ConvertFromMoviconDataType(jobdata, out int value);
                        if (result == DriverErrorCodes.ErrorNoError)
                            dataToWrite.ofs.m_ofs[0] = value;
                        else
                            dataToWrite = null;
                    }
                    break;
                case "t_tip":
                    {
                        result = ConvertFromMoviconDataType(jobdata, out short value);
                        if (result == DriverErrorCodes.ErrorNoError)
                            dataToWrite.ofs.t_tip[0] = value;
                        else
                            dataToWrite = null;
                    }
                    break;
                case "t_ofs":
                    {
                        result = ConvertFromMoviconDataType(jobdata, out int value);
                        if (result == DriverErrorCodes.ErrorNoError)
                            dataToWrite.ofs.t_ofs[0] = value;
                        else
                            dataToWrite = null;
                    }
                    break;
            }

            return result;
        }


        public DriverErrorCodes ParseReadData(Focas1.IODBTO_1_1 dataRead, out byte[] data)
        {
            DriverErrorCodes result = DriverErrorCodes.ErrorNoError;
            data = null;

            switch (_FieldName)
            {
                case "m_ofs":
                    result = ConvertToMoviconDataType(dataRead.ofs.m_ofs[0], out data);
                    break;
                case "t_tip":
                    result = ConvertToMoviconDataType(dataRead.ofs.t_tip[0], out data);
                    break;
                case "t_ofs":
                    result = ConvertToMoviconDataType(dataRead.ofs.t_ofs[0], out data);
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

                case "S_Number":
                    switch (MachineModel)
                    {
                        case FanucCNCProtocol.MachineSeries.Serie0iB:
                            if (_S_Number != 0 && _S_Number != 1)
                                return Properties.Resources.ErrorStartToolOffsetNumberInvalid;
                            break;
                        case FanucCNCProtocol.MachineSeries.Serie30iB:
                        case FanucCNCProtocol.MachineSeries.Serie31iB:
                        case FanucCNCProtocol.MachineSeries.Serie35iB:
                            if (_S_Number != 0 && _S_Number != 1 && _S_Number != 9)
                                return Properties.Resources.ErrorStartToolOffsetNumberInvalid;
                            break;
                    }
                    break;                
            }
            return null;
        }
        #endregion

        #region Properties
        private short _S_Number;
        public short S_Number
        {
            set { _S_Number = value; }
            get { return _S_Number; }
        }

        private ToolsOffsetMemory _OffsetMemoryType;
        public ToolsOffsetMemory OffsetMemoryType
        {
            set { _OffsetMemoryType = value; }
            get { return _OffsetMemoryType; }
        }

        private short _OffsetType;
        public short OffsetType
        {
            set { _OffsetType = value; }
            get { return _OffsetType; }
        }

        private short _E_Number;
        public short E_Number
        {
            get { return _E_Number; }            
        }

        private short _Size;
        public short Size
        {
            get { return _Size; }         
        }

        private short _N;
        //public short N
        //{
        //    get { return _N; }            
        //}        
        private string _FieldName;
        #endregion
    }
}
