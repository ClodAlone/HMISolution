using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using Opc.Ua;
using System.Runtime.InteropServices;
using System.Text;


namespace PPI
{

    #region enums

    public enum PPIErrorCodes : int
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
        IDS_OVERLAPADDRESSES,
        IDS_INVALIDTAG,
        IDS_TIMEOUTRX,
        IDS_NAKRECEIVED,
        IDS_BADRXCHARS,
        IDS_STATUSNOTZERO,
        IDS_INVALIDSEQNUMBER,
        IDS_HWFAULT,
        IDS_ILLEGALOBJACCESS,
        IDS_INVALIDADDRESS,
        IDS_DATATYPENOTSUPP,
        IDS_OBJNOTEXIST,
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
        public ushortUnion(byte LOBYTE, byte HIBYTE)
        {
            this.USHORT = 0;
            this.LOBYTE = LOBYTE;
            this.HIBYTE = HIBYTE;
        }
        public ushortUnion(byte[] buffer, ushort index, bool isSiemens = false)
        {
            this.USHORT = 0;
            if (!isSiemens)
            {
                this.LOBYTE = buffer[index];
                this.HIBYTE = buffer[index + 1];
            }
            else
            {
                this.HIBYTE = buffer[index];
                this.LOBYTE = buffer[index + 1];
            }
        }
        public ushortUnion(List<byte> buffer, ushort index, bool isSiemens = false)
        {
            this.USHORT = 0;
            if (!isSiemens)
            {
                this.LOBYTE = buffer[index];
                this.HIBYTE = buffer[index + 1];
            }
            else
            {
                this.HIBYTE = buffer[index];
                this.LOBYTE = buffer[index + 1];
            }
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
        public uintUnion(byte LOBYTE_LW, byte HIBYTE_LW, byte LOBYTE_HW, byte HIBYTE_HW)
        {
            this.UINT = 0;
            this.LOUSHORT = new ushortUnion(LOBYTE_LW, HIBYTE_LW);
            this.HIUSHORT = new ushortUnion(LOBYTE_HW, HIBYTE_HW);
        }
        public uintUnion(byte[] buffer, ushort index, bool isSiemens = false)
        {
            this.UINT = 0;
            if (!isSiemens)
            {
                this.LOUSHORT = new ushortUnion(buffer, index, isSiemens);
                this.HIUSHORT = new ushortUnion(buffer, (ushort)(index + 2), isSiemens);
            }
            else
            {
                this.HIUSHORT = new ushortUnion(buffer, index, isSiemens);
                this.LOUSHORT = new ushortUnion(buffer, (ushort)(index + 2), isSiemens);
            }
        }
        public uintUnion(List<byte> buffer, ushort index, bool isSiemens = false)
        {
            this.UINT = 0;
            if (!isSiemens)
            {
                this.LOUSHORT = new ushortUnion(buffer, index, isSiemens);
                this.HIUSHORT = new ushortUnion(buffer, (ushort)(index + 2), isSiemens);
            }
            else
            {
                this.HIUSHORT = new ushortUnion(buffer, index, isSiemens);
                this.LOUSHORT = new ushortUnion(buffer, (ushort)(index + 2), isSiemens);
            }
        }

    }
    
    public struct RequestTelegram
    {

        #region Constructors
        
        public RequestTelegram(ushort size)
        {
            this.size = size;
            this.buffer = new byte[size];
            this.pointer = 0;
            this.Sum = 0;
        }

        #endregion

        #region Member
        
        public byte[] buffer;
        public ushort size;
        public ushort pointer;
        public byte Sum;

        #endregion

        #region Methods

        public void Init(byte pollRequest, byte TelegramLen = 0)
        {
            pointer = 0;
            buffer[pointer++] = pollRequest;
            if (pollRequest == PPIProtocol.SD2)
            {
                buffer[pointer++] = TelegramLen;
                buffer[pointer++] = TelegramLen;
                buffer[pointer++] = pollRequest;
            }
            Sum = 0;
        }

        public void End()
        {
            buffer[pointer++] = Sum;
            buffer[pointer++] = PPIProtocol.ED;
        }

        public bool insert(byte value)
        {
            if (size > pointer)
            {
                buffer[pointer++] = value;
                Sum += value;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool insert(char value)
        {
            return insert((byte)(value & 0xff));
        }

        public bool compare(ushort index, char value)
        {
            return buffer[index] == (byte)(value & 0xff);
        }

        public bool insert(string value)
        {
            ushort len = (ushort)value.Length;
            if (size >= pointer + len)
            {
                byte[] asciiBytes = Encoding.ASCII.GetBytes(value);
                foreach (byte b in asciiBytes)
                {
                    if (!insert(b))
                        return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool insert(RequestTelegram inTxBuffer)
        {
            if (size >= pointer + inTxBuffer.pointer)
            {
                int i = 0;
                while (i < inTxBuffer.pointer)
                {
                    if (!insert(inTxBuffer.buffer[i++]))
                        return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool insert(byte[] buffer)
        {
            if (size >= pointer + buffer.Count())
            {
                int i = 0;
                while (i < buffer.Count())
                {
                    if (!insert(buffer[i++]))
                        return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

    }

    #endregion


    public class PPIProtocol
    {
        #region constants
        
        public const ushort MAX_WRITE = 256;
        public const ushort MAX_READ = 256;
        public const ushort MAX_DATA_BYTES = 200;
        public const ushort MAX_ELEMENT_BOOL = 12;

        public const ushort PPI_MAX_ID = 127;
        public const ushort PPI_MIN_ID = 1;

        public const byte DLE = 0x10;
        public const byte SD1 = 0x10;
        public const byte SD2 = 0x68;
        public const byte SD4 = 0xDC;
        public const byte SRD = 0x6C;
        public const byte DL = 0x08;
        public const byte SC = 0xE5;
        public const byte FC = 0x7C;
        public const byte ED = 0x16;
        public const byte RR = 0x02;
        public const byte RS = 0x03;

        public const byte PROTO_ID = 0x32;
        public const byte RED_ID = 0x00;
        public const byte READ_SERVICE_ID = 0x04;
        public const byte WRITE_SERVICE_ID = 0x05;
        public const byte NO_VAR = 0x01;
        public const byte VAR_SPC = 0x12;
        public const byte V_ADDR_LG = 0x0A;
        public const byte SYNTAX_ID = 0x10;

        public const string regMemArea = @"^(?<area>[VIQMSTZCHEA]|AI|AQ)(?<tipo>[BWD]{1})?(?<add>\d+)(?<bit>\.\d+)?(?<len>:\d+)?$";
        //public const string regMemArea = @"^(?<area>[EIAQPEMFTZC]+)(?<tipo>[BWDX]{1})?(?<add>\d+)(?<bit>.\d+)?(?<len>:\d+})?(?<conv>,[TC]{1})?";


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

        public static byte[] GetWriteData(PPICommJob j, int elementsize, ref ushort BytesToRequest, ref uint byteOffset)
        {
            lock (j.retLockList())
            {
                object objectData = null;
                j.GetJobData(ref objectData);
                if (j.TagsListOnWriting.Count == 0)
                    return (byte[])objectData;
                byte[] jobdata = (byte[])objectData;
                BytesToRequest = (ushort)jobdata.Count();
                byteOffset = j.TagsListOnWriting[0].ByteOffset;

                return jobdata;
            }
        }
 
        public static void PrepareDataWrite(byte[] pDst, ref List<byte> dataBuf, Step7Format format, Step7WordTrans trans, uint index, uint tsize)
        {
            uint Length = tsize;
            uint idxIn = index;
            ushortUnion WordVal = new ushortUnion(0);
            uintUnion DWordVal = new uintUnion(0);
            byte[] dataBufTmp;

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
                                    dataBufTmp = new byte[Length];
                                    Array.Copy(pDst, index, dataBufTmp, 0, Length);
                                    PPICommJob.SwapByteBuffer(ref dataBufTmp);
                                    dataBuf.AddRange(dataBufTmp);
                                    idxIn += Length;
                                    Length = 0;
                                    break;
                                case Step7WordTrans.wtC:
                                    WordVal = new ushortUnion(pDst, (ushort)idxIn);
                                    //if (WordVal.USHORT > 999)
                                    //    WordVal.USHORT = 999;
                                    //WordVal.USHORT = Bin2Bcd(WordVal.USHORT);
                                    dataBuf.Add(0);
                                    dataBuf.Add(WordVal.HIBYTE);
                                    dataBuf.Add(WordVal.LOBYTE);
                                    idxIn += 2;
                                    Length -= 2;
                                    break;
                                case Step7WordTrans.wtT:
                                    dataBuf.Add(0);
                                    dataBuf.Add(0);
                                    dataBuf.Add(0);
                                    dataBuf.Add(pDst[idxIn + 1]);
                                    dataBuf.Add(pDst[idxIn]);
                                    Length -= 2;
                                    idxIn += 2;
                                    break;
                            }
                        } while (Length > 0);
                    }
                    break;

                case Step7Format.frmDWord:
                    if (0 < (Length >> 2))
                    {
                        do
                        {
                            switch (trans)
                            {
                                case Step7WordTrans.wtW:
                                case Step7WordTrans.wtC:
                                    dataBufTmp = new byte[Length];
                                    Array.Copy(pDst, index, dataBufTmp, 0, Length);
                                    PPICommJob.SwapWordBuffer(ref dataBufTmp);
                                    PPICommJob.SwapByteBuffer(ref dataBufTmp);
                                    dataBuf.AddRange(dataBufTmp);
                                    idxIn += Length;
                                    Length = 0;
                                    break;
                                case Step7WordTrans.wtT:
                                    dataBuf.Add(0);
                                    dataBuf.Add(pDst[idxIn + 3]);
                                    dataBuf.Add(pDst[idxIn + 2]);
                                    dataBuf.Add(pDst[idxIn + 1]);
                                    dataBuf.Add(pDst[idxIn]);
                                    idxIn += 4;
                                    Length -= 4;
                                    break;
                            }
                        } while (Length > 0);
                    }
                    break;
                default:
                    dataBufTmp = new byte[Length];
                    Array.Copy(pDst, index, dataBufTmp, 0, Length);
                    dataBuf.AddRange(dataBufTmp);
                    idxIn += Length;
                    Length = 0;
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

        public static bool SetS7JobLength(PPICommJob job, uint size = 0)
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
                    if ( job.Area == Step7Area.aT)
                    {
                        job.Length = (int)((size + 3) / 4);
                    }
                    else
                    {
                        job.Length = (int)(size / 2);
                    }

                    break;
                case Step7Format.frmDWord:
                    job.Length = (int)(size / 4);
                    break;
            }
          
            return job.Length > 0;
        }

        public static bool ParseData(byte[] receivebuffer, ref PPICommJob job, ref List<object> items)
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
            //int TimeFactor;
            switch (job.Format)
            {
                case Step7Format.frmBit:
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
                    Array.Copy(receivebuffer, tempBuffer, job.GetNumOfElements());
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
                    if (0 < (Len = job.GetNumOfElements()))
                    {
                        int i = 0, j = 0;
                        do
                        {
                            ushortUnion WordVal ;
                            switch (job.Trans)
                            {
                                case Step7WordTrans.wtT:
                                case Step7WordTrans.wtW:
                                    WordVal = new ushortUnion(receivebuffer, (ushort)j, true);
                                    tempBuffer[i++] = WordVal.LOBYTE;
                                    tempBuffer[i++] = WordVal.HIBYTE;
                                    j += 2;
                                    break;
                                case Step7WordTrans.wtC:
                                    WordVal = new ushortUnion(receivebuffer, (ushort)j, true);
                                    //WordVal.USHORT = Bcd2Bin((UInt16)(WordVal.USHORT & 0xfff));
                                    tempBuffer[i++] = WordVal.LOBYTE;
                                    tempBuffer[i++] = WordVal.HIBYTE;
                                    j += 2;
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
                    if (0 < (Len = job.GetNumOfElements()))
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
