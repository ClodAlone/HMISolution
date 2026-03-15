using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace DriverSerialExample
{

    public enum DriverSerialExampleErrorCodes
    {
        ErrorCRCError = 1000,
        ErrorUnknownFunctionCode,
        ErrorReplayCodeMalformed,
        ErrorErrorCodeMalformed,
        ErrorReplayFrameMalformed,
        ErrorReceivedDataMalformed,
        ErrorLRCMalformed,
        ErrorWrongSize,
        ErrorStationID,
        ErrorFunctionCode,
        ErrorProtocolIllegalFunction = DriverSerialExampleProtocol.PROTOCOL_ERROR + 1,
        ErrorProtocolIllegalDataAddress = DriverSerialExampleProtocol.PROTOCOL_ERROR + 2,
        ErrorProtocolIllegalDataValue = DriverSerialExampleProtocol.PROTOCOL_ERROR + 3,
        ErrorProtocolSlaveDeviceFailure = DriverSerialExampleProtocol.PROTOCOL_ERROR + 4,
        ErrorProtocolAcknowledge = DriverSerialExampleProtocol.PROTOCOL_ERROR + 5,
        ErrorProtocolSlaveDeviceBusy = DriverSerialExampleProtocol.PROTOCOL_ERROR + 6,
        ErrorProtocolMemoryParityError = DriverSerialExampleProtocol.PROTOCOL_ERROR + 8,
        ErrorProtocolGatewayPathUnavailable = DriverSerialExampleProtocol.PROTOCOL_ERROR + 10,
        ErrorProtocolGatewayTargetFailResponse = DriverSerialExampleProtocol.PROTOCOL_ERROR + 11
    }

    //Execution (Function) codes as defined in Modicon DriverSerialExample Protocol
    //(https://www.se.com/us/en/download/document/PIMBUS300/)
    public enum ExecutionCodes
    {
        ReadCoilStatus = 1,
        ReadInputStatus,
        ReadHoldingRegister,
        ReadInputRegister,
        ForceSingleCoil,
        PresetSingleRegister,
        ReadExceptionStatus,
        Diagnostics,
        Program484,
        Poll484,
        FetchCommEventCounter,
        FetchCommEventLog,
        ProgramController,
        PollController,
        ForceMultipleCoils,
        PresetMultipleRegister,
        ReportSlaveID,
        Program884M84,
        ResetCommLink,
        ReadGeneralReference,
        WriteGeneralReference,
        MaskWrite4xRegister,
        ReadWrite4xRegister,
        ReadFIFOQueue
    }

    public class DriverSerialExampleProtocol
    {
        public const int PROTOCOL_ERROR = 1500;
        private const uint UINT_FileReq = 10;
        private const uint UINT_RequestLen = 6;
        private const uint UINT_WriteRequestLen = 7;
        private const uint UINT_WriteFileReqLen = 10;

        public const string TEST_COMM_DYNAMIC_SETTINGS = "DriverSerialExample.Station={0}|LinkType=0|FC=2|SA=0|File=0|BCast=False";
        #region methods override
        public uint GetFrameLength(DriverSerialExampleCommJob job)
        {
            if (job.ReadRequest())
                return (job.FunctionCode == FunctionCodes.FileRecord ?
                    UINT_WriteFileReqLen : UINT_RequestLen);
            else if (job.FunctionCode == FunctionCodes.SingleCoil || job.FunctionCode == FunctionCodes.SingleRegister)
                return 6;
            else
                return (job.FunctionCode == FunctionCodes.FileRecord ?
                    UINT_FileReq : UINT_WriteRequestLen) + job.TotalJobSize;
        }

        // Added to solve FOGBUGZ 12571
        private uint GetBitValues(List<Tag> tagList, UInt16 bitCount, ref byte[] buffer)
        {
            // Build an array of bytes with the bit values stored one bit per byte
            byte[] bitBuffer = new byte[bitCount];
            if (bitBuffer == null)
            {
                return 0;
            }
            uint processedBits = 0;
            for (int i = 0; (i < tagList.Count) && (processedBits < bitCount); i++)
            {
                uint k = tagList[i].Size;
                if ((processedBits + k) > bitCount)
                {
                    break;
                }
                object curVal = tagList[i].Value.Value;
                uint tagArrayDimension = tagList[i].TagNode.ArrayDimension;
                // Single bit
                if (tagArrayDimension == 0)
                {
                    if (Convert.ToSingle(curVal) > 0)
                    {
                        bitBuffer[processedBits++] = 1;
                    }
                    else
                    {
                        bitBuffer[processedBits++] = 0;
                    }
                }
                // Array of bits
                else
                {
                    Array b = curVal as Array;
                    if (b != null && b.GetLength(0) == tagArrayDimension)
                    {
                        for (uint j = 0; (j < tagArrayDimension); j++)
                        {
                            if (Convert.ToSingle(b.GetValue(j)) > 0)
                            {
                                bitBuffer[processedBits++] = 1;
                            }
                            else
                            {
                                bitBuffer[processedBits++] = 0;
                            }
                        }
                    }
                }
            }

            // Compact the bit values in the byte buffer passed as argument
            if (processedBits > 0)
            {
                int byteDim = buffer.GetLength(0);
                for (int i = 0, j = 0, byteIndex = 0; (i < (int)processedBits) && (byteIndex < byteDim); i++)
                {
                    if (bitBuffer[i] > 0)
                    {
                        buffer[byteIndex] += (byte)(1 << j);
                    }
                    if (++j > 7)
                    {
                        j = 0;
                        byteIndex++;
                    }
                }
            }

            return (processedBits);
        }

        public uint PrepareRequest(DriverSerialExampleCommJob job, ref byte[] buffer)
        {
            
            if (job == null || (job.Station as DriverSerialExampleStation == null))
                return 0;
            uint WriteCh = 0;

            UInt16 ItemCount = 0;
            if (job.ReadRequest())
            {
                switch (job.FunctionCode)
                {
                    case FunctionCodes.Coils: // Coils
                    case FunctionCodes.SingleCoil://Single coil
                        job.Executioncode = (byte)ExecutionCodes.ReadCoilStatus;
                        if (job.TagsList[0].TagNode.DataType.IdType == Opc.Ua.IdType.Numeric
                            && (job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean
                            || job.ElementNumber > 0))
                            ItemCount = (UInt16)job.TotalJobSize;
                        else
                            ItemCount = (UInt16)(job.TotalJobSize * 8);
                        break;
                    case FunctionCodes.DiscreteInputs: // Input discretes
                        job.Executioncode = (byte)ExecutionCodes.ReadInputStatus;
                        if (job.TagsList[0].TagNode.DataType.IdType == Opc.Ua.IdType.Numeric
                            && (job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean
                            || job.ElementNumber > 0))
                            ItemCount = (UInt16)(job.TagsList[0].TagNode.ArrayDimension == 0 ? 1 : job.TagsList[0].TagNode.ArrayDimension);
                        else
                            ItemCount = (UInt16)(job.TotalJobSize * 8);
                        break;
                    case FunctionCodes.MultipleRegisters: // Multiple registers
                        job.Executioncode = (byte)ExecutionCodes.ReadHoldingRegister;
                        ItemCount = (UInt16)(job.TotalJobSize >> 1);
                        break;
                    case FunctionCodes.InputRegisters: // Input registers
                        job.Executioncode = (byte)ExecutionCodes.ReadInputRegister;
                        ItemCount = (UInt16)(job.TotalJobSize >> 1);
                        break;
                    case FunctionCodes.SingleRegister: // Single register
                        job.Executioncode = (byte)ExecutionCodes.ReadHoldingRegister;
                        ItemCount = (UInt16)(job.TotalJobSize >> 1);
                        break;
                    case FunctionCodes.FileRecord: // File Record
                        job.Executioncode = (byte)ExecutionCodes.ReadGeneralReference;
                        ItemCount = (UInt16)(job.TotalJobSize >> 1);
                        break;
                    case FunctionCodes.ExceptionStatus:
                        job.Executioncode = (byte)ExecutionCodes.ReadExceptionStatus;
                        ItemCount = 0;
                        break;
                    default:
                        return 0;
                }
                
                //prepare a read request
                buffer[WriteCh++] = (byte)((DriverSerialExampleStation)job.Station).StationID;
                buffer[WriteCh++] = job.Executioncode;
                if (job.FunctionCode == FunctionCodes.FileRecord)
                {
                    buffer[WriteCh++] = 7;
                    buffer[WriteCh++] = 6;
                    buffer[WriteCh++] = (byte)(job.FileNumber >> 8);
                    buffer[WriteCh++] = (byte)job.FileNumber;
                }
                if (job.FunctionCode != FunctionCodes.ExceptionStatus)
                {
                    buffer[WriteCh++] = (byte)(job.StartAddress >> 8);
                    buffer[WriteCh++] = (byte)job.StartAddress;
                    buffer[WriteCh++] = (byte)(ItemCount >> 8);
                    buffer[WriteCh++] = (byte)ItemCount;
                }
                return WriteCh;
            }
            else
            {
                var listOnWriting = new List<Tag>();
                object jobData = null;
                //lock (job.lo)
                //{
                    //prepare a write request
                    job.GetJobData(ref jobData);
                    if (job.GetTagListOnWritingCount() == 0)
                        return 0;
                    listOnWriting.AddRange(job.GetTagListOnWriting());
                //}

                byte[] writeData = (byte[])jobData;
                UInt16 StartAddress = job.StartAddress;
                switch (job.FunctionCode)
                {
                    case FunctionCodes.Coils: // Coils
                        job.Executioncode = (byte)ExecutionCodes.ForceMultipleCoils;
                        //System.Diagnostics.Trace.TraceInformation("PrepareWrite Coils");
                        if (listOnWriting[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                        {
                            //Bit tasks ALWAYS write one bit at once
                            ItemCount = job.elementOnWrite(listOnWriting);
                            StartAddress += (UInt16)listOnWriting[0].ByteOffset;
                        }
                        else
                        {
                            StartAddress += (UInt16)(listOnWriting[0].ByteOffset * 8);
                            if (job.ElementNumber == 0)
                                ItemCount = (ushort)(writeData.Count() * 8);
                        }
                        break;
                    case FunctionCodes.MultipleRegisters: // Multiple registers
                        //System.Diagnostics.Trace.TraceInformation("PrepareWrite MultipleRegisters");
                        job.Executioncode = (byte)ExecutionCodes.PresetMultipleRegister;
                        ItemCount = (UInt16)(writeData.Count() >> 1);
                        StartAddress += (UInt16)(listOnWriting[0].ByteOffset >> 1);
                        break;
                    case FunctionCodes.SingleCoil: // Single coil
                        job.Executioncode = (byte)ExecutionCodes.ForceSingleCoil;
                        ItemCount = (UInt16)1;
                        StartAddress += (UInt16)(listOnWriting[0].ByteOffset + job.LastWrittenSingleCoil);
                        break;
                    case FunctionCodes.SingleRegister: // Single register
                        job.Executioncode = (byte)ExecutionCodes.PresetSingleRegister;
                        ItemCount = (UInt16)1;
                        StartAddress += (UInt16)((listOnWriting[0].ByteOffset + job.LastWrittenSingleCoil) >> 1);
                        break;
                    case FunctionCodes.FileRecord: // File Record
                        job.Executioncode = (byte)ExecutionCodes.WriteGeneralReference;
                        ItemCount = (UInt16)(writeData.Count() >> 1);
                        StartAddress += (UInt16)(listOnWriting[0].ByteOffset >> 1);
                        break;
                    default:
                        return 0;
                }

                buffer[WriteCh++] = (byte)((DriverSerialExampleStation)job.Station).StationID;
                buffer[WriteCh++] = job.Executioncode;
                if (job.FunctionCode == FunctionCodes.FileRecord)
                {
                    buffer[WriteCh++] = (byte)(7 + writeData.Count());
                    buffer[WriteCh++] = 6;
                    buffer[WriteCh++] = (byte)(job.FileNumber >> 8);
                    buffer[WriteCh++] = (byte)job.FileNumber;
                }
                buffer[WriteCh++] = (byte)(StartAddress >> 8);
                buffer[WriteCh++] = (byte)StartAddress;

                // Modified to solve FOGBUGZ 10928
                //buffer[WriteCh++] = (byte)(ItemCount >> 8);
                //buffer[WriteCh++] = (byte)ItemCount;
                //buffer[WriteCh++] = (byte)nData;
                if (job.FunctionCode != FunctionCodes.SingleCoil && job.FunctionCode != FunctionCodes.SingleRegister)
                {
                    buffer[WriteCh++] = (byte)(ItemCount >> 8);
                    buffer[WriteCh++] = (byte)ItemCount;
                    buffer[WriteCh++] = (byte)writeData.Count();
                }

                //add data to write, ask the job...
                if ((job.FunctionCode == FunctionCodes.MultipleRegisters ||
                    job.FunctionCode == FunctionCodes.SingleRegister ||
                    job.FunctionCode == FunctionCodes.FileRecord ||
                    job.FunctionCode == FunctionCodes.InputRegisters) ^ job.SwapBytes)
                    CommJob.SwapByteBuffer(ref writeData);

                if (job.SwapWords)
                    CommJob.SwapWordBuffer(ref writeData);

                //System.Diagnostics.Trace.TraceInformation("{0} - Writing: ", DateTime.UtcNow.ToString("hh:mm:ss:ffff"));

                for (int i = 0; i < writeData.Count(); i++)
                {
                    // Added to solve FOGBUGZ 10928
                    if (job.Executioncode == (byte)ExecutionCodes.ForceSingleCoil)//write single coil
                    {
                        // FOGBUGZ 11354
                        //buffer[WriteCh++] = (byte)(jobdata[i] == 0 ? 0 : 0xff);
                        //buffer[WriteCh++] = (byte)(jobdata[i] == 0 ? 0 : 0);
                        if (i == job.LastWrittenSingleCoil)
                        {
                            buffer[WriteCh++] = (byte)((writeData[i / 8] & (1 << (job.LastWrittenSingleCoil % 8))) == 0 ? 0 : 0xff);
                            buffer[WriteCh++] = 0;
                        }

                    }
                    else if (job.Executioncode == (byte)ExecutionCodes.PresetSingleRegister)//write single register
                    {
                        if (i == job.LastWrittenSingleCoil)
                        {
                            buffer[WriteCh++] = writeData[i];
                            buffer[WriteCh++] = writeData[i + 1];
                        }
                        i++;
                    }
                    else
                    {

                        //System.Diagnostics.Trace.TraceInformation("<{0}>", jobdata[i]);
                        buffer[WriteCh++] = writeData[i];
                    }
                }

                // FOGBUGZ 11354
                if (job.Executioncode == (byte)ExecutionCodes.ForceSingleCoil || 
                    job.Executioncode == (byte)ExecutionCodes.PresetSingleRegister)//write single coil
                {
                    job.LastWrittenSingleCoil++;
                    if (job.Executioncode == (byte)ExecutionCodes.PresetSingleRegister)
                        job.LastWrittenSingleCoil++;

                    if (job.LastWrittenSingleCoil < writeData.Count())
                    {
                        //lock (job.retLockList())
                        //{
                            listOnWriting.ForEach((tag) => 
                            {
                                job.AddTagListToWrite(tag);
                                //if (!job.TagsListToWrite.Contains(tag))
                                //    job.TagsListToWrite.Add(tag);
                            });
                        //}
                    }
                    else
                    {
                        job.LastWrittenSingleCoil = 0;
                    }
                }

                return WriteCh;
            }
            return 0;
        }

        public static bool ParseData(byte [] receivebuffer, ref DriverSerialExampleCommJob job, ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();
            bool areArguments = (items.Count > 0);
            /*
             * put in the returned data values the received buffer, 
             * from ID to the end of data, crc's have been removed
             */
            if (receivebuffer[0] != (byte)((DriverSerialExampleStation)job.Station).StationID)
            { 
                //error. wrong station ID!!!
                items.Add((int)DriverSerialExampleErrorCodes.ErrorStationID);
                return false;
            }

            if (job.Executioncode != receivebuffer[1])
            { 
                //error. unexpected function code in answer.
                items.Add((int)DriverSerialExampleErrorCodes.ErrorFunctionCode);
                return false;
            }

            bool writeoperation = (receivebuffer[1] == 15 || receivebuffer[1] == 16 ||
                receivebuffer[1] == 5 || receivebuffer[1] == 6 ||
                receivebuffer[1] == 21);

            if (areArguments)
            {
                if (items.Count == 0)
                    return false;

                BuiltInType bt = job.Station.GetBuiltInType(items[0].GetType());
                if (bt != BuiltInType.Byte && bt != BuiltInType.Double &&
                    bt != BuiltInType.Float && bt != BuiltInType.Int16 &&
                    bt != BuiltInType.Int32 && bt != BuiltInType.Int64 &&
                    bt != BuiltInType.Integer && bt != BuiltInType.Number &&
                    bt != BuiltInType.SByte && bt != BuiltInType.UInt16 &&
                    bt != BuiltInType.UInt32 && bt != BuiltInType.UInt64 &&
                    bt != BuiltInType.UInteger)
                    return false;
                if (items.Count < (writeoperation ? 1 : 2))
                {

                    items[0] = 11;//DriverSerialExampleErrorCodes.;
                    return false;

                }
            }

            //check if correct amount of data have been received
            byte bytenumber = receivebuffer[2];

            if ((receivebuffer[1] == 3 || receivebuffer[1] == 4 ||
               receivebuffer[1] == 20 || receivebuffer[1] == 7) &&
               (receivebuffer[2] != job.TotalJobSize))
            {
                /*
                 * Read operations.
                 * Error. The amount of data received is not congruent
                 * with job dimension.
                 */
                items.Add((int)DriverSerialExampleErrorCodes.ErrorWrongSize);
                return false;
            }

            if ((receivebuffer[1] == 1 || receivebuffer[1] == 2))
            {
                uint size;
                if (job.ElementNumber == 0 && job.TagsList[0].TagNode.DataType != Opc.Ua.DataTypes.Boolean)
                    size = job.TotalJobSize;
                else
                    size = (UInt16)((job.TotalJobSize + 7) / 8);

                if (receivebuffer[2] != size)
                {
                    items.Add((int)DriverSerialExampleErrorCodes.ErrorWrongSize);
                    if (areArguments)
                        items[0] = (int)DriverSerialExampleErrorCodes.ErrorWrongSize;
                    return false;
                }
            }

            

            switch (receivebuffer[1])
            {
                //read
                case 1://Read Coils
                case 2://Read Discrete Inputs
                    {
                        byte[] jobdata = new byte[bytenumber];
                        receivebuffer.ToList().CopyTo(3, jobdata, 0, bytenumber);

                        if (job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean || job.ElementNumber > 0)
                        {
                            if (job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                                bytenumber *= 8;
                            else
                                bytenumber = (byte)(job.TagsList[0].TagNode.ArrayDimension == 0 ? 1 : job.TagsList[0].TagNode.ArrayDimension);

                            byte[] tempBuffer = new byte[bytenumber];
                            for (ushort bitIndex = 0; bitIndex < bytenumber; bitIndex++)
                                tempBuffer[bitIndex] = (byte)((jobdata[bitIndex / 8] >> (bitIndex % 8)) & 1);

                            jobdata = tempBuffer;
                        }

                        job.SetJobData(jobdata, ref changed);
                        if (areArguments)
                        {
                            if (job.TagsList.Count == items.Count - 1)
                            {
                                for (int k = 0; k < job.TagsList.Count; k++)
                                {
                                    items[k + 1] = job.TagsList[k].Value.Value;
                                }
                            }
                        }
                        else
                            items.AddRange(changed);
                    }
                    break;
                case 3://Read Holding Registers
                case 4://Read Input Registers
                case 20://Read File Record
                case 7://Read Exception Status
                    {
                        byte[] jobdata = new byte[bytenumber];
                        receivebuffer.ToList().CopyTo(3, jobdata, 0, bytenumber);
                        job.SetJobData(jobdata, ref changed);
                        if (areArguments)
                        {
                            if (job.TagsList.Count == items.Count - 1)
                            {
                                for (int k = 0; k < job.TagsList.Count; k++)
                                {
                                    items[k + 1] = job.TagsList[k].Value.Value;
                                }
                            }
                        }
                        else
                            items.AddRange(changed);
                    }
                    break;
                //write
                case 15://Write multiple Coils
                case 16://Write Multiple Registers
                case 5://Write Single Coil
                case 6://Write Single Register
                case 21://Write File Record
                    //didn't receive an error, can be satisfied...
                    break;
            }

            return true;
        }

       #endregion
        public static uint GetMaxJobSize(FunctionCodes FunctionCode, LinkType Type)
        {

            if ((Type == LinkType.Input) ||
               (FunctionCode == FunctionCodes.DiscreteInputs) ||
               (FunctionCode == FunctionCodes.InputRegisters) ||
               (FunctionCode == FunctionCodes.ExceptionStatus))
            {
                switch (FunctionCode)
                {
                    case FunctionCodes.Coils:
                    case FunctionCodes.DiscreteInputs:
                    case FunctionCodes.MultipleRegisters:
                    case FunctionCodes.InputRegisters:
                    case FunctionCodes.FileRecord:
                        return 250;
                    case FunctionCodes.SingleCoil:
                        return 250;
                    case FunctionCodes.SingleRegister:
                        return 250;
                    case FunctionCodes.ExceptionStatus:
                        return 1;
                }
            }
            else
            {
                switch (FunctionCode)
                {
                    case FunctionCodes.Coils:
                        return 100;
                    case FunctionCodes.MultipleRegisters:
                    case FunctionCodes.FileRecord:
                        return 200;
                    case FunctionCodes.SingleCoil:
                        return 100;
                    case FunctionCodes.SingleRegister:
                        return 200;
                }
            }

            return 0;
        }

        

        
    }
}
