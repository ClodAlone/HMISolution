using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using System.Runtime.InteropServices;


namespace MpiPcAdapter
{

    #region enums

    public enum MpiPcAdapterErrorCodes : int
    {
        IDS_ERRDEVICEWRITE = 1000,
        IDS_SERIALREADFAILED,
        IDS_ETXNOTRECEIVED,
        IDS_PLCBCC,
        IDS_ANSWERTOOSHORT,
        IDS_UNRECOGNIZEDANSWER,
        IDS_RETURNEDERR,
        IDS_WRONGDESTINATION,
        IDS_CONNREFUSED,
        IDS_WRONGSOURCE,
        IDS_NAK,
        IDS_WRONGSEQUENCE,
        IDS_TOOFEWDATA,
        IDS_WRITEFAILED,
        IDS_UNEXPEXTEDCHARRECEIVED,
        IDS_TOOMANYCONNECTION,
    }

    public enum Step7Format : int
    { frmInvalid = -1, frmBit = 0, frmByte, frmWord, frmDWord }

    public enum Step7Area : int
    {
        aInvalid = -1, aP = 0, aI, aQ, aM, aD, aT, aC, aAI, aAQ, aTIEC, aCIEC, aS,
        aH, aPE, aPA
    }

    public enum Step7WordTrans 
    { wtW, wtC, wtT }

    public enum MpiNetworkBitRates
    { 
        BITRATE_19_2K = 1,
        BITRATE_187_5K, 
        BITRATE_1_5M 
    }
 
    public enum AdapterTypes
    {
        INAT_MPI_PPI_KABEL,
        SIEMENS_6ES7972_0CA21_0XA0,
        VIPA_GREEN_CABLE
    };

    public enum MpiResponses
    {
        MPI_CREATELINE_RESP,
        MPI_CREATECONN1_RESP,
        MPI_CREATECONN2_RESP,
        MPI_CREATECONN3_RESP,
        MPI_ACKMSG_RESP,
        MPI_READ_RESP,
        MPI_WRITE_RESP,
        MPI_CLOSELINE_RESP,
        MPI_CLOSECONN_RESP
    };

    #endregion

    #region dataTypes

    [StructLayout(LayoutKind.Explicit)]
    public struct ushortUnion
    {
        [FieldOffset(0)]
        public ushort USHORT;

        [FieldOffset(0)]
        public byte LOBYTE;
        [FieldOffset(1)]
        public byte HIBYTE;
        // Constructor:
        public ushortUnion(ushort USHORT)
        {
            this.LOBYTE = 0;
            this.HIBYTE = 0;
            this.USHORT = USHORT;
        }
        public ushortUnion(byte[] buffer, ushort index)
        {
            this.USHORT = 0;
            this.LOBYTE = buffer[index];
            this.HIBYTE = buffer[index + 1];
        }
        public ushortUnion(List<byte> buffer, ushort index)
        {
            this.USHORT = 0;
            this.LOBYTE = buffer[index];
            this.HIBYTE = buffer[index + 1];
        }

    }

    [StructLayout(LayoutKind.Explicit)]
    public struct uintUnion
    {
        [FieldOffset(0)]
        public uint UINT;

        [FieldOffset(0)]
        public ushortUnion LOUSHORT;
        [FieldOffset(2)]
        public ushortUnion HIUSHORT;

        // Constructor:
        public uintUnion(uint UINT)
        {
            this.LOUSHORT = new ushortUnion(0x0000);
            this.HIUSHORT = new ushortUnion(0x0000);
            this.UINT = UINT;
        }
        public uintUnion(byte[] buffer, ushort index)
        {
            this.UINT = 0;
            this.LOUSHORT = new ushortUnion(buffer, index);
            this.HIUSHORT = new ushortUnion(buffer, (ushort)(index + 2));
        }
        public uintUnion(List<byte> buffer, ushort index)
        {
            this.UINT = 0;
            this.LOUSHORT = new ushortUnion(buffer, index);
            this.HIUSHORT = new ushortUnion(buffer, (ushort)(index + 2));
        }

    }
    [StructLayout(LayoutKind.Explicit)]
    public struct ushortUnionSiemens
    {
        [FieldOffset(0)]
        public ushort USHORT;

        [FieldOffset(0)]
        public byte LOBYTE;
        [FieldOffset(1)]
        public byte HIBYTE;
        // Constructor:
        public ushortUnionSiemens(ushort USHORT)
        {
            this.LOBYTE = 0;
            this.HIBYTE = 0;
            this.USHORT = USHORT;
        }
        public ushortUnionSiemens(byte[] buffer, ushort index)
        {
            this.USHORT = 0;
            this.HIBYTE = buffer[index];
            this.LOBYTE = buffer[index + 1];
        }
        public ushortUnionSiemens(List<byte> buffer, ushort index)
        {
            this.USHORT = 0;
            this.HIBYTE = buffer[index];
            this.LOBYTE = buffer[index + 1];
        }

    }

    [StructLayout(LayoutKind.Explicit)]
    public struct uintUnionSiemens
    {
        [FieldOffset(0)]
        public uint UINT;

        [FieldOffset(0)]
        public ushortUnionSiemens LOUSHORT;
        [FieldOffset(2)]
        public ushortUnionSiemens HIUSHORT;

        // Constructor:
        public uintUnionSiemens(uint UINT)
        {
            this.LOUSHORT = new ushortUnionSiemens(0x0000);
            this.HIUSHORT = new ushortUnionSiemens(0x0000);
            this.UINT = UINT;
        }
        public uintUnionSiemens(byte[] buffer, ushort index)
        {
            this.UINT = 0;
            this.HIUSHORT = new ushortUnionSiemens(buffer, index);
            this.LOUSHORT = new ushortUnionSiemens(buffer, (ushort)(index + 2));
        }
        public uintUnionSiemens(List<byte> buffer, ushort index)
        {
            this.UINT = 0;
            this.HIUSHORT = new ushortUnionSiemens(buffer, index);
            this.LOUSHORT = new ushortUnionSiemens(buffer, (ushort)(index + 2));
        }

    }

    #endregion


    public class MpiPcAdapterProtocol
    {
        #region constants

        public const int MAX_LOCAL_HANDLE_ALLOWED = 64;

        public const byte SOH = 0x01;
        public const byte STX = 0x02;
        public const byte DLE = 0x10;
        public const byte ETX = 0x03;
        public const byte EOT = 0x04;
        public const byte NAK = 0x15;
        public const byte ACK = 0x06;
        public const byte ENQ = 0x05;

        public const uint MAX_JOB_DATA_BYTES = 424;
        public const uint MAX_BOOL_WRITE_ELEMENT = 12;
        public const uint MAX_SINGLE_TASK_BYTE = 212;

        public const string regDbArea = @"^DB(?<num>\d+).DB(?<tipo>[BWDX]{1})(?<add>\d+)(?<bit>\.\d+)?(?<len>:\d+)?(?<conv>,\w)?$";
        public const string regMemArea = @"^(?<area>[EIAQPEMFTZC]+)(?<tipo>[BWDX]{1})?(?<add>\d+)(?<bit>\.\d+)?(?<len>:\d+)?(?<conv>,[TC]{1})?$";
        //public const string regDbArea = @"^DB(?<num>\d+).DB(?<tipo>\w)(?<add>\d+)(?<bit>.\d+)?(?<len>:\d+})?(?<conv>,\w)?";
        //public const string regMemArea = @"^(?<area>[EIAQPEMFTZC]+)(?<tipo>[BWDX]{1})?(?<add>\d+)(?<bit>.\d+)?(?<len>:\d+})?(?<conv>,[TC]{1})?";
        public const string TEST_COMM_DYNAMIC_SETTINGS = "MpiPcAdapter.Station={0}|LinkType=1|SA=MB0";

        #endregion

        #region static member
        #endregion

        #region member
        #endregion


        #region static methods
        public static UFUAModel.DataType DataType(Step7Format Format)
        {
            switch (Format)
            {
                case Step7Format.frmDWord:
                    return UFUAModel.DataType.UInt32;
                case Step7Format.frmWord:
                    return UFUAModel.DataType.UInt16;
                case Step7Format.frmByte:
                    return UFUAModel.DataType.Byte;
                case Step7Format.frmBit:
                    return UFUAModel.DataType.Boolean;
                default:
                    return 0;
            }
        }



        #endregion

        #region methods 

        public static void IncReadData(MpiPcAdapterCommJob j)
        {
            if (j.TotalJobSize > MAX_SINGLE_TASK_BYTE)
            {
                j.PartialElementEnd = (ushort)(MAX_SINGLE_TASK_BYTE + j.PartialElementStart);
                if (j.PartialElementEnd >= j.TotalJobSize)
                    j.PartialElementEnd = 0;
            }
        }

        public static byte[] GetWriteData(MpiPcAdapterCommJob j, ref uint byteOffset, ref uint bitOffset, out uint nElement)
        {
            var listOnWriting = new List<Tag>();
            object objectData = null;
            //lock (j.retLockList())
            //{
            j.GetJobData(ref objectData);
            nElement = 0;
            if (j.GetTagListOnWritingCount() == 0)
                return (byte[])objectData;
            listOnWriting.AddRange(j.GetTagListOnWriting());
            //}

            byte[] jobdata = (byte[])objectData;
            if(j.isProtocolBool())
            {
                if (listOnWriting[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean || j.ElementNumber > 0)
                {
                    UInt16 sizeDataType = (UInt16)j.GetDataTypeByteSize((uint)listOnWriting[0].TagNode.DataType.Identifier);
                    foreach (var tag in listOnWriting)
                        nElement += (tag.Size / sizeDataType);
                }
                else
                    nElement = (ushort)(jobdata.Count() * 8);
            }
            else
                nElement = (ushort)jobdata.Count();

            byteOffset = listOnWriting[0].ByteOffset;
            bitOffset = listOnWriting[0].BitOffset;
            return jobdata;
        }

        public static byte[] GetPartialWriteData(byte[] bufferIn, MpiPcAdapterCommJob j, uint elementsize)
        {
            byte[] BufferOut;
            uint maxElement = j.Format == Step7Format.frmBit ? MAX_BOOL_WRITE_ELEMENT : MAX_SINGLE_TASK_BYTE;
            j.PartialElementEnd = (ushort)(maxElement + j.PartialElementStart);
            if (j.PartialElementEnd >= j.Length * elementsize)
                j.PartialElementEnd = 0;

            uint length = (uint)(j.GetNumOfElements((int)elementsize) * elementsize);

            if (j.Format == Step7Format.frmBit)
            {
                byte indexIn;
                BufferOut = new byte[(length + 7) / 8];
                for (byte i = 0; i < length; i++)
                {
                    indexIn = (byte)(i + j.PartialElementStart);
                    if ((bufferIn[indexIn / 8] & (1 << (indexIn % 8))) == 0)
                        BufferOut[i / 8] &= (byte)((1 << (i % 8)) ^ 0xff);
                    else
                        BufferOut[i / 8] |= (byte)(1 << (i % 8));
                }
            }
            else
            {
                BufferOut = new byte[length];
                Array.Copy(bufferIn, j.PartialElementStart, BufferOut, 0, BufferOut.Length);
            }
            return BufferOut;
        }

        public static void PrepareDataWrite(ref byte[] pDst, Step7Format format, Step7WordTrans trans, uint index, uint tsize)
        {
            uint Length = tsize;
            uint idx = index;
            uint idxDword = index;
            ushortUnion WordVal = new ushortUnion(0);
            uintUnion DWordVal = new uintUnion(0);
            UInt16 TimeFactor;
            switch (format)
            {
                case Step7Format.frmWord:
                    if (0 < (Length >> 1))
                    {
                        do
                        {
                            switch (trans)
                            {
                                case Step7WordTrans.wtW:
                                    MpiPcAdapterCommJob.SwapByteBuffer(ref pDst, (int)idx, (int)Length);
                                    Length = 2;
                                    break;
                                case Step7WordTrans.wtC:
                                    WordVal = new ushortUnion(pDst, (ushort)idx);
                                    if (WordVal.USHORT > 999)
                                        WordVal.USHORT = 999;
                                    WordVal.USHORT = Bin2Bcd(WordVal.USHORT);
                                    pDst[idx++] = WordVal.HIBYTE;
                                    pDst[idx++] = WordVal.LOBYTE;
                                    break;
                                case Step7WordTrans.wtT:
                                    DWordVal = new uintUnion(pDst, (ushort)idxDword);
                                    DWordVal.UINT += 5;

                                    if (DWordVal.UINT < 10000)
                                    {
                                        TimeFactor = 0;
                                        WordVal.USHORT = (UInt16)(DWordVal.UINT / 10);
                                    }
                                    else
                                    {
                                        DWordVal.UINT += 45;
                                        if (DWordVal.UINT < 100000)
                                        {
                                            TimeFactor = 1;
                                            WordVal.USHORT = (UInt16)(DWordVal.UINT / 100);
                                        }
                                        else
                                        {
                                            DWordVal.UINT += 450;
                                            if (DWordVal.UINT < 1000000)
                                            {
                                                TimeFactor = 2;
                                                WordVal.USHORT = (UInt16)(DWordVal.UINT / 1000);
                                            }
                                            else
                                            {
                                                DWordVal.UINT += 4500; 
                                                TimeFactor = 3;
                                                WordVal.USHORT = (UInt16)(DWordVal.UINT / 10000);
                                                if (WordVal.USHORT > 999)
                                                    WordVal.USHORT = 999;
                                            }
                                        }
                                    }
                                    WordVal.USHORT = (ushort)(TimeFactor << 12 | Bin2Bcd((int)WordVal.USHORT));
                                    pDst[idx++] = WordVal.HIBYTE;
                                    pDst[idx++] = WordVal.LOBYTE;
                                    idxDword += 4;
                                    Length -= 2;
                                    break;
                            }
                            Length -= 2;
                        } while (Length > 0);
                    }
                    break;

                case Step7Format.frmDWord:
                    if (0 < (Length >> 2))
                    {
                        MpiPcAdapterCommJob.SwapWordBuffer(ref pDst, (int)idx, (int)Length);
                        MpiPcAdapterCommJob.SwapByteBuffer(ref pDst, (int)idx, (int)Length);
                    }
                    break;
            }
        }

        static UInt16 Bin2Bcd(int Val)
        {
            UInt16 RetVal = 0;
            int a, b;

            Val %= 1000;

            a = Val / 100;
            Val %= 100;
            b = Val / 10;
            Val %= 10;
            RetVal = (UInt16)(a << 8);
            RetVal |= (UInt16)(b << 4);
            RetVal |= (UInt16)Val;
            return RetVal;
        }

        public static bool SetS7JobLength(MpiPcAdapterCommJob job, uint size = 0)
        {
            if (!job.IsValid)
                return false;

            if (size == 0)
                size = job.TotalJobSize;
            switch (job.Format)
            {
                case Step7Format.frmBit:
                    job.Length = (int)size;
                    break;
                case Step7Format.frmByte:
                    job.Length = (int)size;
                    break;
                case Step7Format.frmWord:
                    if (job.Trans == Step7WordTrans.wtT || job.Area == Step7Area.aT)
                    {
                        job.Length = (int)((size + 3) / 4);
                    }
                    else
                    {
                        job.Length = (int)((size + 1) / 2);
                    }

                    break;
                case Step7Format.frmDWord:
                    job.Length = (int)((size + 3) / 4);
                    break;
            }
            
            return job.Length > 0;
        }

        public static bool ParseData(byte[] receivebuffer, ref MpiPcAdapterCommJob job, ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();
            bool areArguments = (items.Count > 0);

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
                if (items.Count < 2)
                {
                    items[0] = 11;
                    return false;
                }
            }
            
            byte[] tempBuffer = new byte[job.TotalJobSize];
            int Len;
            int TimeFactor;
            switch (job.Format)
            {
                case Step7Format.frmBit:
                    {
                        if (job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean || job.ElementNumber > 0)
                        {
                            int nStart = job.Bit;
                            byte nMask = (byte)(1 << nStart);
                            int nByteOffs;
                            int i;
                            for (i = 0; i < job.TotalJobSize; i++)
                            {
                                nByteOffs = (nStart + i) / 8;
                                if ((nStart + i) % 8 == 0)
                                {
                                    nMask = 1;
                                }
                                if ((receivebuffer[nByteOffs] & nMask) != 0)
                                {
                                    tempBuffer[i] = (byte)1;
                                }
                                else
                                {
                                    tempBuffer[i] = (byte)0;
                                }
                                nMask <<= 1;
                            }
                        }
                        else
                            Array.Copy(receivebuffer, tempBuffer, job.Length);
                        job.SetJobData(tempBuffer, ref changed);

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
                case Step7Format.frmByte:
                    Array.Copy(receivebuffer, tempBuffer, job.Length);
                    job.SetJobData(tempBuffer, ref changed);
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
                    break;
                case Step7Format.frmWord:
                    if (0 < (Len = job.Length))
                    {
                        int i = 0, j = 0;
                        do
                        {
                            ushortUnionSiemens WordVal = new ushortUnionSiemens(receivebuffer,(ushort)j);
                            j+=2;
                            switch (job.Trans)
                            {
                                case Step7WordTrans.wtW:
                                    tempBuffer[i++] = WordVal.LOBYTE;
                                    tempBuffer[i++] = WordVal.HIBYTE;
                                    break;
                                case Step7WordTrans.wtC:
                                    WordVal.USHORT = Bcd2Bin((UInt16)(WordVal.USHORT & 0xfff));
                                    tempBuffer[i++] = WordVal.LOBYTE;
                                    tempBuffer[i++] = WordVal.HIBYTE;
                                    break;
                                case Step7WordTrans.wtT:
                                    TimeFactor = (WordVal.USHORT >> 12) & 3;
                                    WordVal.USHORT = Bcd2Bin((UInt16)(WordVal.USHORT & 0xfff));
                                    uintUnion DWordVal = new uintUnion(0);
                                    switch (TimeFactor)
                                    {
                                        case 0:
                                            DWordVal.UINT = (UInt32)(WordVal.USHORT * 10);
                                            break;
                                        case 1:
                                            DWordVal.UINT = (UInt32)(WordVal.USHORT * 100);
                                            break;
                                        case 2:
                                            DWordVal.UINT = (UInt32)(WordVal.USHORT * 1000);
                                            break;
                                        case 3:
                                            DWordVal.UINT = (UInt32)(WordVal.USHORT * 10000);
                                            break;
                                    }
                                    tempBuffer[i++] = DWordVal.LOUSHORT.LOBYTE;
                                    tempBuffer[i++] = DWordVal.LOUSHORT.HIBYTE;
                                    tempBuffer[i++] = DWordVal.HIUSHORT.LOBYTE;
                                    tempBuffer[i++] = DWordVal.HIUSHORT.HIBYTE;
                                    break;
                            }
                        } while (--Len > 0);
                        job.SetJobData(tempBuffer, ref changed);
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
                case Step7Format.frmDWord:
                    if (0 < (Len = job.Length))
                    {
                        int i = 0, j = 0;
                        do
                        {
                            tempBuffer[i++] = receivebuffer[j + 3];
                            tempBuffer[i++] = receivebuffer[j + 2];
                            tempBuffer[i++] = receivebuffer[j + 1];
                            tempBuffer[i++] = receivebuffer[j];
                            j += 4;
                        } while (--Len > 0);
                        job.SetJobData(tempBuffer, ref changed);
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
            }

            return true;
        }

        static UInt16 Bcd2Bin(UInt16 Val)
        {
            int a, b, c;

            a = (Val >> 8) & 0x0f;
            b = (Val >> 4) & 0x0f;
            c = (Val >> 0) & 0x0f;

            if ((a > 9) || (b > 9) || (c > 9))
                return 0;
            return Convert.ToUInt16((a * 100) + (b * 10) + c);
        }



        #endregion
    }
}
