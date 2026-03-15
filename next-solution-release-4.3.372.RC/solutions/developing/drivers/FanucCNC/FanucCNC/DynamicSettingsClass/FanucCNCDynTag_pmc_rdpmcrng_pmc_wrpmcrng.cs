using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using FanucCNC.Focas_Library;
using Opc.Ua;


namespace FanucCNC
{
    public class FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng : FanucCNCDynTag_BaseFunction
    {
        public enum PmcDataType : int
        {
            Unknown = -1,
            G_Signal = 0,
            F_Signal = 1,
            Y_Signal = 2,
            X_Signal = 3,
            A_MessageDemand = 4,
            R_InternalRelay = 5,
            T_ChangeableTimer = 6,
            K_KeepRelay = 7,
            C_Counter = 8,
            D_DataTable = 9,
            E_ExtendedRelay = 12,
        }

        public enum ReadDataType : ushort
        {
            Byte = 0,
            Word = 1,
            Long = 2,
            FloatinPoint32Bit = 4,
            FloatinPoint64Bit = 5,
        }

        const String AddressParameter = "ADR";

        public FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng(FanucCNCDynTagSettings main, string settings) : base(main, settings)
        {
            OffsetVariableSupported = true;

            Parse(_Address, FanucCNCProtocol.GetDataType(main.VarType), main.ArrayDimension);
        }

        void Reset()
        {
            IsValid = false;            
            //_Address = string.Empty;
            //_FunctionCode = PmcDataType.Unknown;
            //_VarType = BuiltInType.Null;
            _AddressNumber = 0;
            _AddressNumberAndOffset = 0;
            //_AddressENumber = 0;
            _AddressENumberAndOffset = 0;
            _RDataType = ReadDataType.Byte;
            _AddressBitNumber = -1;

            if (_TagDataTypeSize <= 1)
                _TagDataTypeSize = 1;
        }

        void Parse(string address, BuiltInType mdt, uint arrayDimension)
        {            
            Reset();            

            _Address = address;
            _VarType = mdt;

            if (String.IsNullOrWhiteSpace(address))
            {
                InvalidReason = Properties.Resources.ErrorAddressEmpty;
                return;
            }

            if (mdt == BuiltInType.String)
            {
                InvalidReason = Properties.Resources.ErrorInvalidDataFormat;
                return;
            }

            InvalidReason = GetAddressType(ref address, ref _FunctionCode);
            if (string.IsNullOrEmpty(InvalidReason))
            {
                InvalidReason = GetAddressNumbers(_FunctionCode, address, mdt, arrayDimension, ref _AddressNumber, ref _AddressBitNumber); //, ref _AddressENumber,
                _AddressNumberAndOffset = _AddressNumber;
                //_AddressENumberAndOffset = _AddressENumber;
                //_AddressBitNumberAndOffset = _AddressBitNumber;
            }

            IsValid = string.IsNullOrEmpty(InvalidReason);
        }

        private string GetAddressType(ref string address, ref FanucCNCDynTag_pmc_rdpmcrng_pmc_wrpmcrng.PmcDataType type)
        {
            string result = string.Empty;

            switch (address.Substring(0, 1))
            {
                case "G":
                    type = PmcDataType.G_Signal;
                    break;
                case "F":
                    type = PmcDataType.F_Signal;
                    break;
                case "Y":
                    type = PmcDataType.Y_Signal;
                    break;
                case "X":
                    type = PmcDataType.X_Signal;
                    break;
                case "A":
                    type = PmcDataType.A_MessageDemand;
                    break;
                case "R":
                    type = PmcDataType.R_InternalRelay;
                    break;
                case "T":
                    type = PmcDataType.T_ChangeableTimer;
                    break;
                case "K":
                    type = PmcDataType.K_KeepRelay;
                    break;
                case "C":
                    type = PmcDataType.C_Counter;
                    break;
                case "E":
                    type = PmcDataType.E_ExtendedRelay;
                    break;
                case "D":
                    type = PmcDataType.D_DataTable;
                    break;
                default:
                    type = PmcDataType.Unknown;
                    result = Properties.Resources.ErrorAddressDataTypeInvalid;
                    break;
            }
            address = address.Substring(1, address.Length - 1);

            return result;
        }


        private uint GetByteJobSize(BuiltInType mdt, uint arrayDimension) {
            int size = FanucCNCProtocol.GetDataTypeSize(FanucCNCProtocol.GetDataType(mdt));
            if (size <= 0)
                size = 1;

            if (arrayDimension>0)
                size *= (int)arrayDimension;

            if (mdt == BuiltInType.Boolean)
            {
                int bytePart = Math.DivRem(size , 8, out int bitRemain);
                if (bitRemain > 0)
                    bytePart++;

                size = bytePart;
            }

            return (uint)size;
        }

        private string GetAddressNumbers(PmcDataType type, string address, BuiltInType mdt, uint arrayDimension, ref ushort adrNumber, ref sbyte adrBitNumber) //ref ushort adrENumber, 
        {
            string result = Properties.Resources.ErrorAddressInvalid;

            uint jobSize = GetByteJobSize(mdt, arrayDimension);
            if (jobSize > Focas1.IODBPMCXX_CDATA_SIZE)
                return string.Format(Properties.Resources.ErrorInvalidJobSize, Focas1.IODBPMCXX_CDATA_SIZE);

            // when used to calculate the end of range, remove last byte
            jobSize -= 1;

            switch (type)
            {
                case PmcDataType.G_Signal:
                    {
                        var data = address.Split('.'); // bit sepeparator
                        switch (data.Length)
                        {
                            case 1:
                                if (!ushort.TryParse(data[0], out adrNumber))
                                    return result;

                                if (adrNumber < Properties.Settings.Default.Pmc_G_Signal_MinAddress || adrNumber > Properties.Settings.Default.Pmc_G_Signal_MaxAddress)
                                    return result;

                                if (adrNumber + jobSize > Properties.Settings.Default.Pmc_G_Signal_MaxAddress)
                                    return Properties.Resources.ErrorEndAddressOutOfRange;

                                if (mdt != BuiltInType.Null && mdt == BuiltInType.Boolean)
                                    return Properties.Resources.ErrorAddressTagDataTypeIncorrect;
                                result = string.Empty;
                                break;

                            case 2:
                                if (!ushort.TryParse(data[0], out adrNumber) || !sbyte.TryParse(data[1], out adrBitNumber))
                                    return result;

                                if (adrNumber < Properties.Settings.Default.Pmc_G_Signal_MinAddress || adrNumber > Properties.Settings.Default.Pmc_G_Signal_MaxAddress)
                                    return result;

                                if (adrNumber + jobSize > Properties.Settings.Default.Pmc_G_Signal_MaxAddress)
                                    return Properties.Resources.ErrorEndAddressOutOfRange;

                                if (adrBitNumber < 0 || adrBitNumber > 7)
                                    return result;

                                if (mdt != BuiltInType.Null &&  mdt != BuiltInType.Boolean)
                                    return Properties.Resources.ErrorAddressTagDataTypeIncorrect;

                                result = string.Empty;
                                break;
                        }
                    }
                    break;

                case PmcDataType.F_Signal:
                    {
                        var data = address.Split('.'); // bit sepeparator
                        switch (data.Length)
                        {
                            case 1:
                                if (!ushort.TryParse(data[0], out adrNumber))
                                    return result;

                                if (adrNumber < Properties.Settings.Default.Pmc_F_Signal_MinAddress || adrNumber > Properties.Settings.Default.Pmc_F_Signal_MaxAddress)
                                    return result;

                                if (adrNumber + jobSize > Properties.Settings.Default.Pmc_F_Signal_MaxAddress)
                                    return Properties.Resources.ErrorEndAddressOutOfRange;

                                if (mdt != BuiltInType.Null && mdt == BuiltInType.Boolean)
                                    return Properties.Resources.ErrorAddressTagDataTypeIncorrect;
                                result = string.Empty;
                                break;

                            case 2:
                                if (!ushort.TryParse(data[0], out adrNumber) || !sbyte.TryParse(data[1], out adrBitNumber))
                                    return result;

                                if (adrNumber < Properties.Settings.Default.Pmc_F_Signal_MinAddress || adrNumber > Properties.Settings.Default.Pmc_F_Signal_MaxAddress)
                                    return result;

                                if (adrNumber + jobSize > Properties.Settings.Default.Pmc_F_Signal_MaxAddress)
                                    return Properties.Resources.ErrorEndAddressOutOfRange;

                                if (adrBitNumber < 0 || adrBitNumber > 7)
                                    return result;

                                if (mdt != BuiltInType.Null && mdt != BuiltInType.Boolean)
                                    return Properties.Resources.ErrorAddressTagDataTypeIncorrect;

                                result = string.Empty;
                                break;
                        }
                    }
                    break;

                case PmcDataType.Y_Signal:
                    {
                        var data = address.Split('.'); // bit sepeparator
                        switch (data.Length)
                        {
                            case 1:
                                if (!ushort.TryParse(data[0], out adrNumber))
                                    return result;

                                if (adrNumber < Properties.Settings.Default.Pmc_F_Signal_MinAddress || adrNumber > Properties.Settings.Default.Pmc_F_Signal_MaxAddress)
                                    return result;

                                if (adrNumber + jobSize > Properties.Settings.Default.Pmc_F_Signal_MaxAddress)
                                    return Properties.Resources.ErrorEndAddressOutOfRange;

                                if (mdt != BuiltInType.Null && mdt == BuiltInType.Boolean)
                                    return Properties.Resources.ErrorAddressTagDataTypeIncorrect;
                                result = string.Empty;
                                break;

                            case 2:
                                if (!ushort.TryParse(data[0], out adrNumber) || !sbyte.TryParse(data[1], out adrBitNumber))
                                    return result;

                                if (adrNumber < Properties.Settings.Default.Pmc_F_Signal_MinAddress || adrNumber > Properties.Settings.Default.Pmc_F_Signal_MaxAddress)
                                    return result;

                                if (adrNumber + jobSize > Properties.Settings.Default.Pmc_F_Signal_MaxAddress)
                                    return Properties.Resources.ErrorEndAddressOutOfRange;

                                if (adrBitNumber < 0 || adrBitNumber > 7)
                                    return result;

                                if (mdt != BuiltInType.Null && mdt != BuiltInType.Boolean)
                                    return Properties.Resources.ErrorAddressTagDataTypeIncorrect;

                                result = string.Empty;
                                break;
                        }
                    }
                    break;

                case PmcDataType.X_Signal:
                    {
                        var data = address.Split('.'); // bit sepeparator
                        switch (data.Length)
                        {
                            case 1:
                                if (!ushort.TryParse(data[0], out adrNumber))
                                    return result;

                                if (adrNumber < Properties.Settings.Default.Pmc_X_Signal_MinAddress || adrNumber > Properties.Settings.Default.Pmc_X_Signal_MaxAddress)
                                    return result;

                                if (adrNumber + jobSize > Properties.Settings.Default.Pmc_X_Signal_MaxAddress)
                                    return Properties.Resources.ErrorEndAddressOutOfRange;

                                if (mdt != BuiltInType.Null && mdt == BuiltInType.Boolean)
                                    return Properties.Resources.ErrorAddressTagDataTypeIncorrect;
                                result = string.Empty;
                                break;

                            case 2:
                                if (!ushort.TryParse(data[0], out adrNumber) || !sbyte.TryParse(data[1], out adrBitNumber))
                                    return result;

                                if (adrNumber < Properties.Settings.Default.Pmc_X_Signal_MinAddress || adrNumber > Properties.Settings.Default.Pmc_X_Signal_MaxAddress)
                                    return result;

                                if (adrNumber + jobSize > Properties.Settings.Default.Pmc_X_Signal_MaxAddress)
                                    return Properties.Resources.ErrorEndAddressOutOfRange;

                                if (adrBitNumber < 0 || adrBitNumber > 7)
                                    return result;

                                if (mdt != BuiltInType.Null && mdt != BuiltInType.Boolean)
                                    return Properties.Resources.ErrorAddressTagDataTypeIncorrect;

                                result = string.Empty;
                                break;
                        }
                    }
                    break;

                case PmcDataType.A_MessageDemand:
                    {
                        var data = address.Split('.'); // bit sepeparator
                        switch (data.Length)
                        {
                            case 2:
                                if (!ushort.TryParse(data[0], out adrNumber) || !sbyte.TryParse(data[1], out adrBitNumber))
                                    return result;

                                if (adrNumber < Properties.Settings.Default.Pmc_A_MessageDemand_MinAddress || adrNumber > Properties.Settings.Default.Pmc_A_MessageDemand_MaxAddress)
                                    return result;

                                if (adrNumber + jobSize > Properties.Settings.Default.Pmc_A_MessageDemand_MaxAddress)
                                    return Properties.Resources.ErrorEndAddressOutOfRange;

                                if (adrBitNumber < 0 || adrBitNumber > 7)
                                    return result;

                                if (mdt != BuiltInType.Null && mdt != BuiltInType.Boolean)
                                    return Properties.Resources.ErrorAddressTagDataTypeIncorrect;

                                result = string.Empty;
                                break;
                        }
                    }
                    break;

                case PmcDataType.T_ChangeableTimer:
                    {
                        if (!ushort.TryParse(address, out adrNumber))
                            return result;

                        if (adrNumber < Properties.Settings.Default.Pmc_T_ChangeableTimer_MinAddress || adrNumber > Properties.Settings.Default.Pmc_T_ChangeableTimer_MaxAddress)
                            return result;

                        if (adrNumber + jobSize > Properties.Settings.Default.Pmc_T_ChangeableTimer_MaxAddress)
                            return Properties.Resources.ErrorEndAddressOutOfRange;

                        if (mdt != BuiltInType.Null && mdt == BuiltInType.Boolean)
                            return Properties.Resources.ErrorAddressTagDataTypeIncorrect;

                        result = string.Empty;
                    }
                    break;

                case PmcDataType.K_KeepRelay:
                    if (!ushort.TryParse(address, out adrNumber))
                        return result;

                    if (adrNumber < Properties.Settings.Default.Pmc_K_KeepRelay_MinAddress || adrNumber > Properties.Settings.Default.Pmc_K_KeepRelay_MaxAddress)
                        return result;

                    if (adrNumber + jobSize > Properties.Settings.Default.Pmc_K_KeepRelay_MaxAddress)
                        return Properties.Resources.ErrorEndAddressOutOfRange;

                    if (mdt != BuiltInType.Null && mdt == BuiltInType.Boolean)
                        return Properties.Resources.ErrorAddressTagDataTypeIncorrect;

                    result = string.Empty;
                    break;

                case PmcDataType.C_Counter:
                    if (!ushort.TryParse(address, out adrNumber))
                        return result;

                    if (adrNumber < Properties.Settings.Default.Pmc_C_Counter_MinAddress || adrNumber > Properties.Settings.Default.Pmc_C_Counter_MaxAddress)
                        return result;

                    if (adrNumber + jobSize > Properties.Settings.Default.Pmc_C_Counter_MaxAddress)
                        return Properties.Resources.ErrorEndAddressOutOfRange;

                    if (mdt != BuiltInType.Null && mdt == BuiltInType.Boolean)
                        return Properties.Resources.ErrorAddressTagDataTypeIncorrect;

                    result = string.Empty;
                    break;

                case PmcDataType.D_DataTable:
                    {
                        var data = address.Split('.'); // bit sepeparator
                        switch (data.Length)
                        {
                            case 1:
                                if (!ushort.TryParse(data[0], out adrNumber))
                                    return result;

                                if (adrNumber < Properties.Settings.Default.Pmc_D_DataTable_MinAddress || adrNumber > Properties.Settings.Default.Pmc_D_DataTable_MaxAddress)
                                    return result;

                                if (adrNumber + jobSize > Properties.Settings.Default.Pmc_D_DataTable_MaxAddress)
                                    return Properties.Resources.ErrorEndAddressOutOfRange;

                                if (mdt != BuiltInType.Null && mdt == BuiltInType.Boolean)
                                    return Properties.Resources.ErrorAddressTagDataTypeIncorrect;
                                result = string.Empty;
                                break;

                            case 2:
                                if (!ushort.TryParse(data[0], out adrNumber) || !sbyte.TryParse(data[1], out adrBitNumber))
                                    return result;

                                if (adrNumber < Properties.Settings.Default.Pmc_D_DataTable_MinAddress || adrNumber > Properties.Settings.Default.Pmc_D_DataTable_MaxAddress)
                                    return result;

                                if (adrNumber + jobSize > Properties.Settings.Default.Pmc_D_DataTable_MaxAddress)
                                    return Properties.Resources.ErrorEndAddressOutOfRange;

                                if (adrBitNumber < 0 || adrBitNumber > 7)
                                    return result;

                                if (mdt != BuiltInType.Null && mdt != BuiltInType.Null && mdt != BuiltInType.Boolean)
                                    return Properties.Resources.ErrorAddressTagDataTypeIncorrect;

                                result = string.Empty;
                                break;
                        }
                    }
                    break;

                case PmcDataType.R_InternalRelay:
                    {
                        var data = address.Split('.'); // bit sepeparator
                        switch (data.Length)
                        {
                            case 1:
                                if (!ushort.TryParse(data[0], out adrNumber))
                                    return result;

                                if (adrNumber < Properties.Settings.Default.Pmc_R_InternalRelay_MinAddress || adrNumber > Properties.Settings.Default.Pmc_R_InternalRelay_MaxAddress)
                                    return result;

                                if (adrNumber + jobSize > Properties.Settings.Default.Pmc_R_InternalRelay_MaxAddress)
                                    return Properties.Resources.ErrorEndAddressOutOfRange;

                                if (mdt != BuiltInType.Null && mdt == BuiltInType.Boolean)
                                    return Properties.Resources.ErrorAddressTagDataTypeIncorrect;
                                result = string.Empty;
                                break;

                            case 2:
                                if (!ushort.TryParse(data[0], out adrNumber) || !sbyte.TryParse(data[1], out adrBitNumber))
                                    return result;

                                if (adrNumber < Properties.Settings.Default.Pmc_R_InternalRelay_MinAddress || adrNumber > Properties.Settings.Default.Pmc_R_InternalRelay_MaxAddress)
                                    return result;

                                if (adrNumber + jobSize > Properties.Settings.Default.Pmc_R_InternalRelay_MaxAddress)
                                    return Properties.Resources.ErrorEndAddressOutOfRange;

                                if (adrBitNumber < 0 || adrBitNumber > 7)
                                    return result;

                                if (mdt != BuiltInType.Null && mdt != BuiltInType.Boolean)
                                    return Properties.Resources.ErrorAddressTagDataTypeIncorrect;

                                result = string.Empty;
                                break;
                        }
                    }
                    break;

                case PmcDataType.E_ExtendedRelay:
                    {
                        var data = address.Split('.'); // bit sepeparator
                        switch (data.Length)
                        {
                            case 1:
                                if (!ushort.TryParse(data[0], out adrNumber))
                                    return result;

                                if (adrNumber < Properties.Settings.Default.Pmc_E_ExtendedRelay_MinAddress || adrNumber > Properties.Settings.Default.Pmc_E_ExtendedRelay_MaxAddress)
                                    return result;

                                if (adrNumber + jobSize > Properties.Settings.Default.Pmc_E_ExtendedRelay_MaxAddress)
                                    return Properties.Resources.ErrorEndAddressOutOfRange;

                                if (mdt != BuiltInType.Null && mdt == BuiltInType.Boolean)
                                    return Properties.Resources.ErrorAddressTagDataTypeIncorrect;
                                result = string.Empty;
                                break;

                            case 2:
                                if (!ushort.TryParse(data[0], out adrNumber) || !sbyte.TryParse(data[1], out adrBitNumber))
                                    return result;

                                if (adrNumber < Properties.Settings.Default.Pmc_E_ExtendedRelay_MinAddress || adrNumber > Properties.Settings.Default.Pmc_E_ExtendedRelay_MaxAddress)
                                    return result;

                                if (adrNumber + jobSize > Properties.Settings.Default.Pmc_E_ExtendedRelay_MaxAddress)
                                    return Properties.Resources.ErrorEndAddressOutOfRange;

                                if (adrBitNumber < 0 || adrBitNumber > 7)
                                    return result;

                                if (mdt != BuiltInType.Null && mdt != BuiltInType.Boolean)
                                    return Properties.Resources.ErrorAddressTagDataTypeIncorrect;

                                result = string.Empty;
                                break;
                        }
                    }
                    break;
            }

            //adrENumber = (ushort)(adrNumber + (_TagDataTypeSize - 1));

            return result;
        }

        public bool IsValidAddress(string address, UFUAModel.DataType mdt, uint arrayDimension, out string error)
        {
            Parse(address, FanucCNCProtocol.GetDataType(mdt), arrayDimension);

            error = InvalidReason;

            return IsValid;
        }

        public override void SplitSettings(string settings)
        {            
            _Address = GetPartByName(AddressParameter);

            IsValid = true;

            CalculateInternalParameters();
        }

        private ushort CalculateRequestSize(int size)
        {
            return (ushort)(8 + size);
        }

        public override void CalculateInternalParameters(List<Tag> tagList = null, int offsetValue = 0, bool aggregation = false)
        {
            //if (IsStructType(Main.VarType))
            //{
            //    _TagDataTypeSize = GetStructSizeAndAssignOffset(tagList);

            //    _AddressENumber = (ushort)(_AddressNumber + _TagDataTypeSize - 1);

            //    _AddressNumberAndOffset = (ushort)(_AddressNumber + offsetValue);
            //    _AddressENumberAndOffset = (ushort)(_AddressENumber + offsetValue);
            //}
            //else
            //{
            //    _TagDataTypeSize = FanucCNCProtocol.GetDataTypeSize(Main.VarType);
            //    if (Main.VarType == UFUAModel.DataType.Boolean)
            //    {
            //        if (Main.ArrayDimension > 0 && _AddressBitNumber < 0)
            //            _AddressBitNumber = 0;

            //        int bitAddressFormat = (_AddressNumber * 8) + _AddressBitNumber + offsetValue;
            //        _AddressNumberAndOffset = (ushort)(bitAddressFormat / 8);
            //        _AddressBitNumberAndOffset = (sbyte)(bitAddressFormat % 8);
            //        tagList[0].BitOffset = (uint)_AddressBitNumberAndOffset;

            //        if (Main.ArrayDimension > 0)
            //        {
            //            _TagDataTypeSize = Math.DivRem((int)(_AddressBitNumberAndOffset + Main.ArrayDimension), 8, out int bitRest);
            //            if (bitRest > 0)
            //                _TagDataTypeSize++;
            //        }

            //        bitAddressFormat = (_AddressNumberAndOffset * 8) + _AddressBitNumberAndOffset + (_TagDataTypeSize * 8);
            //        _AddressENumberAndOffset = (ushort)((bitAddressFormat / 8) - 1);
            //    }
            //    else
            //    {
            //        if (Main.ArrayDimension > 0)
            //            _TagDataTypeSize = _TagDataTypeSize * (int)Main.ArrayDimension;

            //        _AddressENumber = (ushort)(_AddressNumber + _TagDataTypeSize - 1);

            //        _AddressNumberAndOffset = (ushort)(_AddressNumber + offsetValue);
            //        _AddressENumberAndOffset = (ushort)(_AddressENumber + offsetValue);
            //    }
            //}

            //_Size = CalculateRequestSize(_TagDataTypeSize);

            _TagDataTypeSize = GetTagListSizeAndAssignOffset(tagList, aggregation);            

            //if (IsStructType(Main.VarType) {
            if (Main.VarType == UFUAModel.DataType.Boolean) {

                // offsetValue is in byte
                int bitAddressFormat = (_AddressNumber * 8) + _AddressBitNumber + offsetValue;
                _AddressNumberAndOffset = (ushort)(bitAddressFormat / 8);
                _AddressBitNumberAndOffset = (sbyte)(bitAddressFormat % 8);

                bitAddressFormat = (_AddressNumberAndOffset * 8) + _AddressBitNumberAndOffset + ((_TagDataTypeSize -1) * 8);
                _AddressENumberAndOffset = (ushort)(bitAddressFormat / 8);
            }
            else
            {
                // offsetValue is in bit
                _AddressNumberAndOffset = (ushort) (_AddressNumber + offsetValue);
                _AddressENumberAndOffset = (ushort) (_AddressNumberAndOffset + (_TagDataTypeSize -1));
            }            

            _Size = CalculateRequestSize(_TagDataTypeSize);
        }

        public override string CreateSettings()
        {
            var parameters = new StringBuilder(); // base.ToString());

            //parameters.AppendFormat("{0}{1}{2}{3}", AddressParameter, CharAssign, _Address, CharSep);
            AddParameter(ref parameters, AddressParameter, _Address);
            return parameters.ToString();
        }

        private ushort GetTagListSizeAndAssignOffset(List<Tag> tagList, bool aggregation)
        {
            ushort resultBytePart = 0;
            ushort resultBitPart = 0;
            int bytes = 0;

            if (tagList == null)
                return 0;
            
            foreach (var tag in tagList)
            {
                UFUAModel.DataType dt = FanucCNCProtocol.GetDataType((BuiltInType)((uint)tag.TagNode.DataType.Identifier));
                ushort nrElements = (ushort)(tag.TagNode.ArrayDimension == 0 ? 1 : tag.TagNode.ArrayDimension);
                switch (dt) {
                    case UFUAModel.DataType.Boolean:
                        if (!aggregation)
                        {
                            tag.ByteOffset = resultBytePart;
                            // set bit offset for SetJobData parsing (read)
                            if (tagList.Count == 1)
                                tag.BitOffset = (uint)_AddressBitNumber;
                            else
                                tag.BitOffset = resultBitPart;
                        }
                        else
                        {
                            resultBytePart = (ushort)tag.ByteOffset;
                            // set bit offset for SetJobData parsing (read)
                            resultBitPart = (ushort)tag.BitOffset;
                        }
                        
                        resultBitPart += nrElements;
                        // from total nr of bit get nr of bytes and remain bits
                        bytes = Math.DivRem(resultBitPart, 8, out int bitRest);
                        resultBitPart = (ushort)bitRest;
                        resultBytePart += (ushort)bytes;

                        ((FanucCNCTag)tag).RequestSize = (bytes == 0 ? 1: bytes);
                        break;
                    case UFUAModel.DataType.SByte:
                    case UFUAModel.DataType.Byte:
                    case UFUAModel.DataType.Int16:
                    case UFUAModel.DataType.UInt16:
                    case UFUAModel.DataType.Int32:
                    case UFUAModel.DataType.UInt32:
                    case UFUAModel.DataType.Int64:
                    case UFUAModel.DataType.UInt64:
                    case UFUAModel.DataType.Float:
                    case UFUAModel.DataType.Double:
                        // if previous element is a bit, start to next byte
                        if (resultBitPart != 0)
                        {
                            resultBitPart = 0;
                            resultBytePart++;                        
                        }
                        if (!aggregation)
                            tag.ByteOffset = (ushort)resultBytePart;                        
                        else
                            resultBytePart = (ushort)tag.ByteOffset;

                        bytes = (ushort)(FanucCNCProtocol.GetDataTypeSize(dt) * nrElements);                        
                        resultBytePart += (ushort)bytes;

                        ((FanucCNCTag)tag).RequestSize = bytes;

                        break;
                    case UFUAModel.DataType.String:
                        // not manage
                        break;
                }
            }

            if (resultBitPart != 0)
                resultBytePart++;

            return resultBytePart;
        }

        public DriverErrorCodes PrepareReadRequest(out ushort addressNumber, out ushort addressENumber, out Focas1.IODBPMCXX cnc_struct)
        {
            cnc_struct = new Focas1.IODBPMCXX();
            //if (isMemberOfStructOrAggregateJob)
            //{
            //    addressNumber = (ushort)(_AddressNumberAndOffset + tag.ByteOffset);
            //    addressENumber = (ushort)(addressNumber + ((FanucCNCTag)tag).RequestSize - 1);
            //    size = CalculateRequestSize(((FanucCNCTag)tag).RequestSize);
            //}
            //else
            //{
                addressNumber = _AddressNumberAndOffset;
                addressENumber = _AddressENumberAndOffset;
                //size = Size;
            //}

            return DriverErrorCodes.ErrorNoError;
        }

        public DriverErrorCodes PrepareReadRequestBeforeWriteBit(bool isMemberOfStructOrAggregateJob, Tag tag, out ushort addressNumber, out ushort addressENumber, out Focas1.IODBPMCXX cnc_struct, out ushort size)
        {
            cnc_struct = new Focas1.IODBPMCXX();

            if (isMemberOfStructOrAggregateJob) {
                addressNumber = (ushort)(_AddressNumberAndOffset + tag.ByteOffset);
                addressENumber = (ushort)(addressNumber + ((FanucCNCTag)tag).RequestSize - 1);
                size = CalculateRequestSize(((FanucCNCTag)tag).RequestSize);
            }
            else
                { 
                addressNumber = (ushort)(_AddressNumberAndOffset + tag.ByteOffset);
                addressENumber = (ushort)(_AddressENumberAndOffset + tag.ByteOffset);
                size = Size;
            }

            return DriverErrorCodes.ErrorNoError;
        }

        public DriverErrorCodes ParseReadData(List<Tag> tagList, FanucCNC.Focas_Library.Focas1.IODBPMCXX dataRead, out byte[] data)
        {
            if (dataRead == null || dataRead.cdata == null)
            {
                data = null;
                return (DriverErrorCodes)FanucCNCProtocol.ErrorCodes.ErrorConvertingInOutData;
            }
            else {
                data = new byte[_TagDataTypeSize];
                //if (IsBitAddress() && Main.ArrayDimension == 0)
                //{
                //    //dataRead.cdata[0] = (byte)((dataRead.cdata[0] >> (byte)_AddressBitNumberAndOffset) & 1);
                //    dataRead.cdata[0] = (byte)((dataRead.cdata[tagList[0].ByteOffset] >> (byte)tagList[0].BitOffset) & 1);
                //}

                //var littleEndianBytes = dataRead.cdata.Take(_TagDataTypeSize).Reverse().ToArray();
                Array.Copy(dataRead.cdata, 0, data, 0, _TagDataTypeSize);
                return DriverErrorCodes.ErrorNoError;
            }
        }

        public DriverErrorCodes PrepareWriteRequest(bool isMemberOfStructOrAggregateJob, Tag tag, byte[] data, out Focas1.IODBPMCXX dataToWrite, out ushort size)
        {
            dataToWrite = new Focas1.IODBPMCXX
            {
                type_a = AddressType,
                type_d = (short)_RDataType,                
            };

            if (isMemberOfStructOrAggregateJob)
            {
                dataToWrite.datano_s = (short)(_AddressNumberAndOffset + tag.ByteOffset);
                dataToWrite.datano_e = (short)(dataToWrite.datano_s + ((FanucCNCTag)tag).RequestSize - 1);
                size = CalculateRequestSize(((FanucCNCTag)tag).RequestSize);
            } 
            else
            {
                dataToWrite.datano_s = (short)_AddressNumberAndOffset;
                dataToWrite.datano_e = (short)_AddressENumberAndOffset;
                size = Size;
            }

            // maximum nr of element is also define into Focas1.IODBPMC0 class declaration
            dataToWrite.cdata = new byte[Focas1.IODBPMCXX_CDATA_SIZE];

            Array.Copy(data, 0, dataToWrite.cdata, 0, data.Length);

            return DriverErrorCodes.ErrorNoError;// ConvertFromMoviconDataType(data, out dataToWrite.data[0]);
        }

        public bool IsBitAddress()
        {
            return (_AddressBitNumber != -1);
        }

        public bool IsArray()
        {
            return (Main != null && (Main.ArrayDimension >0));
        }

        public void MaskValueBeforeWriteBit(Tag tag, FanucCNC.Focas_Library.Focas1.IODBPMCXX dataRead, ref byte[] jobData)
        {
            uint arraySize = (Main.ArrayDimension == 0 ? 1 : Main.ArrayDimension);
            // calculate the nr of bytes used by array of bit
            int nrBytes = Math.DivRem((int)(tag.BitOffset + arraySize -1), 8, out int bitRest);
            if (bitRest > 0 || nrBytes == 0)
                nrBytes++;

            for (int arrayIndex = 0; arrayIndex < arraySize; arrayIndex++)
            {
                int byteNr = (int)Math.DivRem((int)(tag.BitOffset + arrayIndex), 8, out bitRest);

                // assign default value
                byte result = dataRead.cdata[byteNr];

                switch (jobData[arrayIndex])
                {
                    case 1: // set bit to 1
                        result = (byte)(dataRead.cdata[byteNr] | (byte)(jobData[arrayIndex] << (byte)bitRest));
                        break;
                    case 0: // set bit to 0                            
                        if (((dataRead.cdata[byteNr] >> (byte)bitRest) & 1) == 1)
                            // remove 
                            result = (byte)(dataRead.cdata[byteNr] - (byte)(Math.Pow(2, bitRest)));
                        break;                    
                }

                dataRead.cdata[byteNr] = result;
            }

            Array.Resize(ref jobData, nrBytes);
            Array.Copy(dataRead.cdata, 0, jobData, 0, nrBytes);
        }


        public override void OffsetVariableValueChanged(int newValue, List<Tag> tagList)
        {
            CalculateInternalParameters(tagList, newValue);
            //if (IsBitAddress())
            //{
            //    int bitAddressFormat = (_AddressNumber * 8) + _AddressBitNumber + (newValue * 8);
            //    _AddressNumberAndOffset = (ushort)(bitAddressFormat / 8);
            //    _AddressBitNumberAndOffset = (sbyte)(bitAddressFormat % 8);

            //    bitAddressFormat = (_AddressENumber * 8) + _AddressBitNumber + (newValue * 8);
            //    _AddressENumberAndOffset = (ushort)(bitAddressFormat / 8);
            //}
            //else
            //{
            //    _AddressNumberAndOffset = (ushort)(_AddressNumber + newValue);
            //    _AddressENumberAndOffset = (ushort)(_AddressENumber + newValue);
            //}
        }

        public override void ReParseData()
        {
            Parse(_Address, FanucCNCProtocol.GetDataType(Main.VarType), Main.ArrayDimension);
        }

        #region IDataErrorInfo Members
        public override String PerformValidation(String propertyName)
        {
            switch (propertyName) {
                case "Address":
                    if (string.IsNullOrWhiteSpace(_Address))
                        return Properties.Resources.ErrorAddressEmpty;

                    if (!IsValidAddress(Address, Main.VarType, Main.ArrayDimension, out string errorMessage))
                        return string.Format(Properties.Resources.ErrorInvalidAddress, errorMessage);
                    break;
            }

            return null;
        }
        #endregion

        #region Properties

        private int _TagDataTypeSize;

        private string _Address;
        public string Address
        {
            get { return _Address; }
            set { _Address = value; }
        }

        private PmcDataType _FunctionCode;
        public PmcDataType FunctionCode
        {
            get { return _FunctionCode; }
        }

        public short AddressType
        {
            get { return (short)_FunctionCode; }
        }

        private ReadDataType _RDataType;
        public short RDataType
        {
            get { return (short)_RDataType; }
        }

        private BuiltInType _VarType;
        public BuiltInType VarType
        {
            get { return _VarType; }
        }

        private ushort _AddressNumber;
        public ushort AddressNumber
        {
            get { return _AddressNumber; }
        }
        
        //private ushort _AddressENumber;
        //public ushort AddressENumber
        //{
        //    get { return _AddressENumber; }
        //}

        private sbyte _AddressBitNumber;
        public sbyte AddressBitNumber
        {
            get { return _AddressBitNumber; }
        }

        private ushort _Size;
        public ushort Size
        {
            get { return _Size; }
        }

        private ushort _AddressNumberAndOffset;
        private ushort _AddressENumberAndOffset;
        private sbyte _AddressBitNumberAndOffset;

        #endregion
    }
}
