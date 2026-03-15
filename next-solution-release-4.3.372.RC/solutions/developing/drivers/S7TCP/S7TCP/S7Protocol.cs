using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DriverCodeBase;
using Opc.Ua;

namespace S7TCP
{
    public enum S7ErrorCodes : int
    {
        ErrorWrongHeader = 1000,
        ErrorTooFewData,
        ErrorConnectionToDevice = 1400,
        ErrorStatusNotZero = 1500,
        ErrorFromDevice = 2000
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


    public class S7Protocol
    {
        public const int RESP_PDU_START = 3;
        public const int PDUREF = RESP_PDU_START + 4;
        public const int ERR_CLS = RESP_PDU_START + 10;
        public const int ERR_COD = RESP_PDU_START + 11;
        public const int SERVICE_ID = RESP_PDU_START + 12;
        public const int POINT_NUM = RESP_PDU_START + 13;
        public const int ACCESS_RESULT = RESP_PDU_START + 14;
        public const int DATA_TYPE = RESP_PDU_START + 15;
        public const int QTY_DATA = RESP_PDU_START + 16;
        public const int START_DATA = RESP_PDU_START + 18;
        public const int NEGOT_PDU_SIZE = RESP_PDU_START + 18;

        public const uint PROTO_ID = 0x32;
        public const byte READ_SERVICE_ID = 0x04;
        public const byte WRITE_SERVICE_ID = 0x05;

        public const byte MAX_TEL_LENGTH =	240;
        public const uint MAX_WRITE	=	MAX_TEL_LENGTH + 50;
        public const uint MAX_READ =	MAX_TEL_LENGTH + 50;
        public const uint MAX_DATA_BYTES = 212;
        public const uint DEFAULTSTRINGLENGTH_TIAPORTAL = 254;
        public const uint SINGLEFRAME_STRING_MAX_SIZE = 200;


        public const uint MAX_MPI_TLG_LEN = 240;
        public static byte [] RequestRead = { 0x32,	// PROTO_ID
                                               0x01,	// ROSCTR Remote Operating Services Control
                                               0x00,	// RED_ID Redundancy Identification (HI)
                                               0x00,	// RED_ID Redundancy Identification (LO) 
                                               0xaa,	// PDU_REF (HI)
                                               0xaa,	// PDU_REF (LO)
                                               0x00,	// PAR_LG Parameter length high byte
                                               0x00,	// PAR_LG Parameter length low byte
                                               0x00,	// DAT_LG Data Length (HI)
                                               0x00,	// DAT_LG Data Length (LO)
                                               0x04,	// SERVICE_ID = read
                                               0x00 };// Number of S7 pointers

        public static byte [] IpReadHeader = { 0x02,   // Length counter
                                               0xf0,   // PDU
                                               0x80 }; // EOT

        public static byte [] IpWriteHeader = { 0x02,   // Length counter
                                               0xf0,   // PDU
                                               0x80 }; // EOT

        public static byte [] RequestWrite = { 0x32,		// PROTO_ID
                                               0x01,		// ROSCTR Remote Operating Services Control
                                               0x00,		// RED_ID Redundancy Identification (HI)
                                               0x00,		// RED_ID Redundancy Identification (LO) 
                                               0xaa,		// PDU_REF (HI)
                                               0xaa,		// PDU_REF (LO)
                                               0x00,		// PAR_LG Parameter length high byte
                                               0x00,		// PAR_LG Parameter length low byte
                                               0x00,		// DAT_LG Data Length (HI)
                                               0x00,		// DAT_LG Data Length (LO)
                                               0x05,		// SERVICE_ID = write
                                               0x00 };	// Number of S7 pointers

        public static byte [] connect_block2 = { 0x02, //Length indicator
	                                            0xF0, //Data PDU
	                                            0x80, //EOT
	                                            // Start of User PDU
	                                            0x32, // PROTO_ID
	                                            0x01, // ROSCTR Remote Operating Services Control
	                                            0x00, // RED_ID Redundancy Identification (HI)
	                                            0x00, // RED_ID Redundancy Identification (LO)
	                                            0x02, // PDU_REF (HI)
	                                            0x00, // PDU_REF (LO)
	                                            0x00, // PAR_LG Parameter length high byte.
	                                            0x08, // PAR_LG Parameter length low byte.
	                                            0x00, // DAT_LG Data Length (HI)
	                                            0x00, // DAT_LG Data Length (LO)
	                                            0xF0, // SERVICE_ID=ESTABLISH ASSOCIATION
	                                            0x00, // Reserved
	                                            0x00, // Max calling suggestion (HI)
	                                            0x01, // Max calling suggestion (LO)
	                                            0x00, // Max called suggestion (HI)
	                                            0x01, // Max called suggestion (LO)
	                                            0x01, // Max PDU size of caller (HI) (=480 bytes)
	                                            0xE0}; // Max PDU size of caller (LO)   

        private const uint UINT_FileReq = 10;
        private const uint UINT_RequestLen = 6;
        private const uint UINT_WriteRequestLen = 7;
        private const uint UINT_WriteFileReqLen = 10;
        #region methods override

       
        public uint GetFrameLength(S7TCPCommJob job)
        {
            return 0;
        }
        public static uint PrepareRequest(List<CommJob> ListJobPending, ref List<CommJob> ListJobExec, ref byte[] buffer)
        {
            // reset job's internal variable 
            Parallel.ForEach(ListJobPending, j =>
            {
                ((S7TCPCommJob)j).WriteItems = 0;
                ((S7TCPCommJob)j).WriteExecuted = false;
            });

            if (ListJobPending[0].Type == DriverCodeBase.Enumerators.LinkType.Input ||
                (ListJobPending[0].Type == DriverCodeBase.Enumerators.LinkType.InputOutput &&
                (ListJobPending[0] as S7TCPCommJob).tmpTagsListToWrite.Count == 0))
                return PrepareReadRequest(ListJobPending, ref ListJobExec, ref buffer);
            else
                return PrepareWriteRequest(ListJobPending, ref ListJobExec, ref buffer);
        }

        const uint nBytesForTheWriteRequest = 12;
        const uint nBytesForTheAnyPointer = 12;
        const uint nBytesForTheVariabileValue = 4;// 1 Byte (Reserved) 1 Byte (Data Type)  2 Byte Length (this bytes Before variable value)

        static uint PrepareWriteRequest(List<CommJob> ListJobPending, ref List<CommJob> ListJobExec, ref byte[] buffer)
        {
            uint idx = 0;
            
            S7Protocol.IpWriteHeader.CopyTo(buffer, idx);
            idx += 3; //Header added 3 (BYTE)
            
            S7Protocol.RequestWrite.CopyTo(buffer, idx);
            //idx += 12; //Request Write added 12 (BYTE) 
            idx += nBytesForTheWriteRequest;

            UInt16 AppHandle = ((S7TCPStation)ListJobPending[0].Station).AppHandle;
            bool s7200 = ((S7TCPCommJob)ListJobPending[0]).S7_200;
            byte[] dataBuf = new byte[256];
            uint len2 = 0; //data area length
            uint posdataheader = 0;
            uint anypponterlength = 2;
            byte ItemCount = 0;
            bool RequestEnd = false;
            uint nLastJobByteOffset = 0;
 
            uint nTotalByteAnyPointerAndVarValue = nBytesForTheWriteRequest;

            foreach (S7TCPCommJob j in ListJobPending)
            {
                S7TCPTag firstTag;
                byte LastAddedTag = 0;
                object objectData = null;
                
                lock (j.retLockList())
                {
                    LastAddedTag = (byte)j.TagsListOnWriting.Count();
                    if (j.MethodID != -1 && j.LocalMethod == false)
                        SetS7JobLength(j);

                    if (j.Type == DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
                    {
                        if (j.tmpTagsListToWrite.Count == 0)
                            j.tmpTagsListToWrite.AddRange(j.TagsList);
                    }
                    j.WriteItems =  0;
                    while (j.tmpTagsListToWrite.Count > 0)
                    {
                        uint dataOverheadSize = 16;
                        if((len2 & 1) != 0)
                        {
                            dataOverheadSize++;
                        }
                        if (!j.LimitatedGetJobData(ref objectData, nTotalByteAnyPointerAndVarValue + dataOverheadSize, out firstTag))
                        {
                            RequestEnd = true;
                            if (((byte[])objectData).Length == 0)
                                break;
                        }

                        if ((len2 & 1) != 0)//This condition aligned to word the buffer
                        {
                            dataBuf[len2] = 0;
                            ++len2;
                        }
                        posdataheader = len2;
                        len2 += nBytesForTheVariabileValue;
                        nLastJobByteOffset = len2;

                        byte[] jobdata = (byte[])objectData;
                        int copyLen = 0;
                        //if ((j.Area == Step7Area.aT) || (j.Trans == Step7WordTrans.wtT))
                        //    copyLen = jobdata.Count() / 2;
                        //else
                        copyLen = jobdata.Count();

                        int nTempOffset = (int)firstTag.ByteOffset;
                        if (j.Area == Step7Area.aT || j.Area == Step7Area.aC)
                        {
                            nTempOffset /= (int)firstTag.Size;
                        }
                        else if (j.Trans == Step7WordTrans.wtT)
                        {
                            nTempOffset /= 2;
                        }
                        int res = firstTag.GetBitArrayElementChanged();
                        if (res >= 0)
                        {
                            nTempOffset += res;
                        }

                        if(dataBuf.Count() <= (copyLen + len2))
                        {
                            break;
                        }
                        Array.Copy(jobdata, 0, dataBuf, len2, copyLen);
                        len2 += (uint)copyLen;

                        nTotalByteAnyPointerAndVarValue += ((len2 - nLastJobByteOffset) + dataOverheadSize);

                        nTempOffset += j.ExecutionOffsetWrite;

                        uint anystart = idx;
                        buffer[idx++] = 0x12;
                        buffer[idx++] = 0x0a;
                        anypponterlength += 2;
                        //add anypointer
                        uint wplen = 0;
                        //In the two conditions added 10 (BYTE) 
                        if (s7200)
                            wplen = GetS7200WritePointer(ref buffer, idx, j, nTempOffset);
                        else
                            wplen = GetS7300WritePointer(ref buffer, idx, j, nTempOffset);
                        anypponterlength += wplen;
                        idx += wplen;
                        ItemCount ++;
                        j.WriteItems ++;

                        //uint nTempLength = (uint)(j.Area == Step7Area.aT || j.Trans == Step7WordTrans.wtT ? jobdata.Count() / 2 : jobdata.Count());
                        uint nTempLength = (uint)jobdata.Count();
                        if (j.IsBigArray() && (!j.isProtocolBool()))
                        {
                            j.ExecutionOffsetWrite += (int)nTempLength;
                            uint jobLength = j.TotalJobSize;
                            if(j.isLenStringEnable == true)
                            {
                                jobLength++;
                            }
                            if (j.ExecutionOffsetWrite >= jobLength)
                            {
                                j.ExecutionOffsetWrite = 0;
                            }
                        }
                        
                        uint nTag = nTempLength;
                        if (j.isProtocolBool())
                        {
                            if (firstTag.TagNode.DataType == Opc.Ua.DataTypes.Boolean || j.ElementNumber > 0)
                            {
                                UInt16 sizeDataType = (UInt16)j.GetDataTypeByteSize((uint)firstTag.TagNode.DataType.Identifier);
                                nTag = 0;
                                for (int tagIndex = LastAddedTag; tagIndex < j.TagsListOnWriting.Count(); tagIndex++)
                                    nTag += (j.TagsListOnWriting[tagIndex].Size / sizeDataType);
                            }
                            else
                                nTag = (ushort)(jobdata.Count() * 8);
                        }

                        if ((j.Area == Step7Area.aT) || (j.Area == Step7Area.aC))
                        {
                            buffer[anystart + 4] = (byte)((nTag / 2) >> 8);
                            buffer[anystart + 5] = (byte)(nTag / 2);
                        }
                        else if (j.IsBigArray() || !j.isProtocolBool())
                        {
                            buffer[anystart + 4] = (byte)(nTag >> 8);
                            buffer[anystart + 5] = (byte)nTag;
                        }
                        

                        CreateProtDataHeader(ref dataBuf, posdataheader, nTempLength, j.Format, j.Area);
                    }
                    if (j.TagsListOnWriting.Count > 0)
                    {
                        if (len2 == 0)
                        {
                            lock(j.retLockList())
                            {
                                j.TagsListOnWriting.Clear();
                            }
                        }
                        else

                        if (!ListJobExec.Contains(j))
                        {
                            ListJobExec.Add(j);
                        }
                        j.WriteExecuted = true;
                        j.IsPending = true;
                    }
                }
                if (RequestEnd == true)
                    break;
            }

            if (len2 + idx > buffer.Count())
            {
                return 0;
            }

            //attach data
            Array.Copy(dataBuf, 0, buffer, idx, len2);

            buffer[7] = (byte)(AppHandle >> 8);
            buffer[8] = (byte)(AppHandle >> 0);
            buffer[9] = (byte)((anypponterlength) >> 8);//anypointes length
            buffer[10] = (byte)((anypponterlength) >> 0);
            buffer[11] = (byte)(len2 >> 8);//data length
            buffer[12] = (byte)(len2 >> 0);
            buffer[14] = ItemCount;

            return idx + len2;
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

        public static void PrepareDataWrite(ref byte [] pDst, uint idx, uint nLength, Step7Format format, Step7WordTrans trans)
        {
            uint Len;
	        UInt16 WordVal;
	        UInt32 DWordVal;
            UInt32 DWtemp;
	        UInt16 TimeFactor;
            switch( format ) {
                case Step7Format.frmWord:
                    if (trans != Step7WordTrans.wtT)
                    {
                        Len = nLength / 2;
                    }
                    else {
                        Len = nLength / 4;
                    }

                    if ( 0 <  Len  ) {
				        do {
					        switch( trans ) {
						        case Step7WordTrans.wtW:
                                    S7TCPCommJob.SwapByteBuffer(ref pDst, (int)idx/*4*/, (int)nLength);
                                    Len = 1;
							        break;
                                case Step7WordTrans.wtC:
                                    WordVal = pDst[idx + 1];
                                    WordVal <<= 8;
                                    WordVal |= pDst[idx + 0];
							        if( WordVal > 999 )
								        WordVal = 999;
							        WordVal = Bin2Bcd( WordVal );
                                    pDst[idx + 1] = (byte)(WordVal >> 0);
                                    pDst[idx + 0] = (byte)(WordVal >> 8);
                                    idx += 2;
							        break;
                                case Step7WordTrans.wtT:
                                    DWordVal = (UInt32)(pDst[idx + 3] << 24);
                                    DWtemp = (UInt32)(pDst[idx + 2] << 16);
                                    DWordVal |= DWtemp;
                                    DWtemp = (UInt32)(pDst[idx + 1] << 8);
                                    DWordVal |= DWtemp;
                                    DWordVal |= pDst[idx + 0];
                                    
							        if( DWordVal < 10000 ) {
								        TimeFactor = 0;
								        WordVal = (UInt16)( DWordVal / 10 );
							        }
							        else {
								        if( DWordVal < 100000 ) {
									        TimeFactor = 1;
									        WordVal = (UInt16)( DWordVal / 100 );
								        }
								        else {
									        if( DWordVal < 1000000 ) {
										        TimeFactor = 2;
										        WordVal = (UInt16)( DWordVal / 1000 );
									        }
									        else {
										        TimeFactor = 3;
										        WordVal = (UInt16)( DWordVal / 10000 );
										        if( WordVal > 999 )
											        WordVal = 999;
									        }
								        }
							        }
                                    UInt16 u = Bin2Bcd((int)WordVal);
                                    WordVal = TimeFactor;
                                    WordVal <<= 12;
                                    WordVal |= u;
							        
							        pDst[ idx+1 ] = (byte)( WordVal >> 0 );
							        pDst[ idx+0 ] = (byte)( WordVal >> 8 );
							        idx += 4;
							        break;
					        }

				        } while( --Len > 0);
			        }
			        break;

                case Step7Format.frmDWord:
			        if( 0 < ( Len = nLength/4 ) ) {
                        S7TCPCommJob.SwapWordBuffer(ref pDst, (int)idx, (int)nLength);
                        S7TCPCommJob.SwapByteBuffer(ref pDst, (int)idx, (int)nLength);
			        }
                    break;               
            }
        }

        static uint PrepareReadRequest(List<CommJob> ListJobPending, ref List<CommJob> ListJobExec, ref byte[] buffer)
        {
            uint idx = 0;
            S7Protocol.IpReadHeader.CopyTo(buffer, idx);
            idx += 3;
            S7Protocol.RequestRead.CopyTo(buffer, idx);
            idx += 12;
            UInt16 AppHandle = ((S7TCPStation)ListJobPending[0].Station).AppHandle;
            bool s7200 = ((S7TCPCommJob)ListJobPending[0]).S7_200;
            int length = 0;
            uint TotReqlen = 0;
            foreach (S7TCPCommJob j in ListJobPending)
            {
                if (j.MethodID != -1 && j.LocalMethod == false)
                    SetS7JobLength(j);

                buffer[idx] = 0x12;
                buffer[idx+1] = 0x0a;
                if (s7200)
                {
                    int type = 0x02;
                    int bit = 0;
                    buffer[idx+2] = 0x10;
                    buffer[idx + 3] = 0x0;
                    buffer[idx + 4] = 0x0;
                    buffer[idx + 5] = 0x0;
                    buffer[idx + 6] = 0x0;
                    buffer[idx + 7] = 0x0;
                    buffer[idx + 8] = 0x0;
                    buffer[idx + 9] = 0x0;
                    buffer[idx + 10] = 0x0;
                    buffer[idx + 11] = 0x0;

                    switch (j.Format)
                    {
                        case Step7Format.frmBit:
                            {
                                if (j.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean || j.ElementNumber > 0)
                                    length = (j.Bit + j.Length + 7) / 8;
                                else
                                    length = (j.Bit + j.Length * 8 + 7) / 8;
                            }
                            break;
                        case Step7Format.frmByte:
                            length = j.Length;
                            break;
                        case Step7Format.frmWord:
                            switch (j.Area)
                            {
                                case Step7Area.aTIEC:
                                    type = 0x1f; // TIMER
                                    length = j.Length * 2;
                                    break;
                                case Step7Area.aCIEC:
                                    type = 0x1e; // COUNTER
                                    length = j.Length * 2;
                                    break;
                                default:
                                    length = j.Length * 2;
                                    break;
                            }
                            break;
                        case Step7Format.frmDWord:
                            length = j.Length * 4;
                            break;
                    }

                    switch (j.Area)
                    {
                        case Step7Area.aI:
                            buffer[idx + 8] = 0x81;
                            break;
                        case Step7Area.aQ:
                            buffer[idx + 8] = 0x82;
                            break;
                        case Step7Area.aM:
                            buffer[idx + 8] = 0x83;
                            break;
                        case Step7Area.aD:
                            buffer[idx + 6] = (byte)(j.DbNumber >> 8);
                            buffer[idx + 7] = (byte)(j.DbNumber >> 0);
                            buffer[idx + 8] = 0x84;
                            break;
                        case Step7Area.aS:
                            buffer[idx + 8] = 0x05;
                            break;
                        case Step7Area.aAI:
                            buffer[idx + 8] = 0x06;
                            break;
                        case Step7Area.aAQ:
                            buffer[idx + 8] = 0x07;
                            break;
                        case Step7Area.aTIEC:
                            buffer[idx + 8] = 0x1f;
                            break;
                        case Step7Area.aCIEC:
                            buffer[idx + 8] = 0x1e;
                            break;
                        case Step7Area.aH:
                            buffer[idx + 8] = 0x20;
                            break;
                    }

                    buffer[idx + 3] = (byte)type;
                    buffer[idx + 4] = (byte)(length >> 8);
                    buffer[idx + 5] = (byte)(length >> 0);

                    int offset = (j.Offset << 3) | bit;
                    buffer[idx + 9] = (byte)(offset >> 16);
                    buffer[idx + 10] = (byte)(offset >> 8);
                    buffer[idx + 11] = (byte)(offset >> 0);
                    idx += j.GetReadRequestLength();
                }
                else
                {
                    buffer[idx+2] = 0x10;
                    buffer[idx + 3] = 0x0;
                    buffer[idx + 4] = 0x0;
                    buffer[idx + 5] = 0x0;
                    buffer[idx + 6] = 0x0;
                    buffer[idx + 7] = 0x0;
                    buffer[idx + 8] = 0x0;
                    buffer[idx + 9] = 0x0;
                    buffer[idx + 10] = 0x0;
                    buffer[idx + 11] = 0x0;
                    int offset = j.Offset + (int)j.ExecutionOffset;
                    switch (j.Format)
                    {
                        case Step7Format.frmBit:
                            {
                                if (j.IsBigArray())
                                {
                                    length = j.GetDataLength();
                                }
                                else
                                {
                                    if (j.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean || j.ElementNumber > 0)
                                        length = (j.Bit + j.Length + 7) / 8;
                                    else
                                        length = (j.Bit + j.Length * 8 + 7) / 8;
                                }
                            }
                            break;
                        case Step7Format.frmByte:
                            if (j.IsBigArray())
                            {
                                length = j.GetDataLength();//j.Length;
                            }
                            else
                            {
                                length = j.Length;
                            }
                            break;
                        case Step7Format.frmWord:
                            switch (j.Area)
                            {
                                case Step7Area.aT:
                                    buffer[idx + 3] = 0x1d; // TIMER
                                    buffer[idx + 8] = 0x1d; // TIMER
                                    buffer[idx + 4] = (byte)(j.Length >> 8);
                                    buffer[idx + 5] = (byte)(j.Length >> 0);
                                    buffer[idx + 10] = (byte)(offset >> 8);
                                    buffer[idx + 11] = (byte)(offset >> 0);
                                    buffer[idx + 9] = 0;
                                    buffer[idx + 6] = 0;
                                    buffer[idx + 7] = 0;
                                    idx += j.GetReadRequestLength();
                                    continue;

                                case Step7Area.aC:
                                    buffer[idx + 3] = 0x1c; // COUNTER
                                    buffer[idx + 8] = 0x1c; // COUNTER
                                    buffer[idx + 4] = (byte)(j.Length >> 8);
                                    buffer[idx + 5] = (byte)(j.Length >> 0);
                                    buffer[idx + 10] = (byte)(offset >> 8);
                                    buffer[idx + 11] = (byte)(offset >> 0);
                                    buffer[idx + 9] = 0;
                                    buffer[idx + 6] = 0;
                                    buffer[idx + 7] = 0;
                                    idx += j.GetReadRequestLength();
                                    continue;
                            }
                            if(j.IsBigArray())
                            {
                                length = j.GetDataLength();//j.Length;
                            }
                            else
                            {
                                length = j.Length * 2;
                            }                            
                            break;
                        case Step7Format.frmDWord:
                            if (j.IsBigArray())
                            {
                                length = j.GetDataLength(); //j.Length;
                            }
                            else
                            {
                                length = j.Length * 4;
                            }
                            break;
                    }
                    switch (j.Area)
                    {
                        case Step7Area.aP:
                        case Step7Area.aPE:
                        case Step7Area.aPA:
                            buffer[idx + 8] = 0x80;
                            break;
                        case Step7Area.aI:
                            buffer[idx + 8] = 0x81;
                            break;
                        case Step7Area.aQ:
                            buffer[idx + 8] = 0x82;
                            break;
                        case Step7Area.aM:
                            buffer[idx + 8] = 0x83;
                            break;
                        case Step7Area.aD:
                            buffer[idx + 6] = (byte)(j.DbNumber >> 8);
                            buffer[idx + 7] = (byte)(j.DbNumber >> 0);
                            buffer[idx + 8] = 0x84;
                            break;

                        case Step7Area.aT:
                        case Step7Area.aC:
                            idx += j.GetReadRequestLength();
                            continue;
                    }

                    buffer[idx + 3] = 0x02;//byte
                    buffer[idx + 4] = (byte)(length >> 8);
                    buffer[idx + 5] = (byte)(length >> 0);

                    offset = (offset << 3) | 0;
                    buffer[idx + 9] = (byte)(offset >> 16);
                    buffer[idx + 10] = (byte)(offset >> 8);
                    buffer[idx + 11] = (byte)(offset >> 0);
                    idx += j.GetReadRequestLength();
                }
                TotReqlen += j.GetReadRequestLength();
            }
            buffer[7] = (byte)(AppHandle >> 8);
            buffer[8] = (byte)(AppHandle >> 0);
            buffer[9] = (byte)((TotReqlen + 2) >> 8); //  2 bytes is the length of 0x04 ( read request )
            buffer[10] = (byte)((TotReqlen + 2) >> 0); //  and 0xNN ( number of pointers )
            buffer[14] = (byte)ListJobPending.Count;

            foreach (S7TCPCommJob job in ListJobPending)
            {
                job.IsPending = true;
                if (!ListJobExec.Contains(job))
                    ListJobExec.Add(job);
            }

            return idx;
        }
        
        public static int CreateProtDataHeader(ref byte [] pHdr ,uint init, uint nDataLen, Step7Format format, Step7Area area)
        {
	        int Len;
	        int LocLen;


	        pHdr[ 0 + init ] = 0xff;
	        switch( format ) {
              case Step7Format.frmBit:
                      pHdr[1 + init] = 0x03;
                      pHdr[2 + init] = 0x00;
                      pHdr[3 + init] = 0x01;
		              return 1;
	              break;
              case Step7Format.frmByte:
                  pHdr[1 + init] = 0x04;
	              Len = (int)nDataLen;
	              LocLen = Len * 8;
                  pHdr[2 + init] = (byte)(LocLen >> 8);
                  pHdr[3 + init] = (byte)(LocLen >> 0);
	              return Len;
            case Step7Format.frmWord:
	            switch( area ) {
                  case Step7Area.aT:
                  case Step7Area.aC:
                      pHdr[1 + init] = 0x09;
	                  Len = (int)nDataLen;
	                  LocLen = Len;
                      pHdr[2 + init] = (byte)(LocLen >> 8);
                      pHdr[3 + init] = (byte)(LocLen >> 0);
	                  return Len;
                  default:
                      pHdr[1 + init] = 0x04;
	                  Len = (int)nDataLen;
	                  LocLen = Len * 8;
                      pHdr[2 + init] = (byte)(LocLen >> 8);
                      pHdr[3 + init] = (byte)(LocLen >> 0);
	                  return Len;
	            }
              break;
            case Step7Format. frmDWord:
              pHdr[1 + init] = 0x04;
	          Len = (int)nDataLen;
	          LocLen = Len * 8;
              pHdr[2 + init] = (byte)(LocLen >> 8);
              pHdr[3 + init] = (byte)(LocLen >> 0);
	          return Len;
	        }
	        return -1;
        }

        static uint GetS7300WritePointer(ref byte[] buffer, uint idx, S7TCPCommJob j, int nOffset = 0)
        {
            int length = 0, offset;
            int bit = 0;
            int type = 0x02; // BYTE
            buffer[idx] = 0x10;
            buffer[idx + 1] = 0;
            buffer[idx + 2] = 0;
            buffer[idx + 3] = 0;
            buffer[idx + 4] = 0;
            buffer[idx + 5] = 0;
            buffer[idx + 6] = 0;
            buffer[idx + 7] = 0;
            buffer[idx + 8] = 0;
            buffer[idx + 9] = 0;
            switch (j.Format)
            {
                case Step7Format.frmBit:
                    type = 0x01; // BOOL
                    length = 1;
                    bit = (j.Bit + nOffset) % 8;
                    nOffset = (j.Bit + nOffset) / 8;
                    break;
                case Step7Format.frmByte:
                    length = nOffset;
                    break;
                case Step7Format.frmWord:
                    switch (j.Area)
                    {
                        case Step7Area.aT:
                            buffer[idx + 1] = 0x1d; // TIMER
                            buffer[idx + 6] = 0x1d; // TIMER
                            buffer[idx + 2] = (byte)(j.Length >> 8);
                            buffer[idx + 3] = (byte)(j.Length >> 0);
                            buffer[idx + 8] = (byte)((j.Offset + nOffset) >> 8);
                            buffer[idx + 9] = (byte)((j.Offset + nOffset) >> 0);
                            buffer[idx + 7] = 0;
                            buffer[idx + 4] = 0;
                            buffer[idx + 5] = 0;
                            return 10;

                        case Step7Area.aC:
                            buffer[idx + 1] = 0x1c; // COUNTER
                            buffer[idx + 6] = 0x1c; // COUNTER
                            buffer[idx + 2] = (byte)(j.Length >> 8);
                            buffer[idx + 3] = (byte)(j.Length >> 0);
                            buffer[idx + 8] = (byte)((j.Offset + nOffset) >> 8);
                            buffer[idx + 9] = (byte)((j.Offset + nOffset) >> 0);
                            buffer[idx + 7] = 0;
                            buffer[idx + 4] = 0;
                            buffer[idx + 5] = 0;
                            return 10;
                    }
                    length = nOffset * 2;
                    break;
                case Step7Format.frmDWord:
                    length = nOffset * 4;
                    break;
            }

            switch (j.Area)
            {
                case Step7Area.aP:

                // Added in version 10.1.0.2 (FOGBUGZ 1961)
                case Step7Area.aPE:
                case Step7Area.aPA:

                    buffer[idx + 6] = 0x80;
                    break;
                case Step7Area.aI:
                    buffer[idx + 6] = 0x81;
                    break;
                case Step7Area.aQ:
                    buffer[idx + 6] = 0x82;
                    break;
                case Step7Area.aM:
                    buffer[idx + 6] = 0x83;
                    break;
                case Step7Area.aD:
                    buffer[idx + 4] = (byte)(j.DbNumber >> 8);
                    buffer[idx + 5] = (byte)(j.DbNumber >> 0);
                    buffer[idx + 6] = 0x84;
                    break;

                case Step7Area.aT:
                case Step7Area.aC:
                    return 10;
            }

            buffer[idx + 1] = (byte)type;
            buffer[idx + 2] = (byte)(length >> 8);
            buffer[idx + 3] = (byte)(length >> 0);

            offset = ((j.Offset + nOffset) << 3) | bit;
            buffer[idx + 7] = (byte)(offset >> 16);
            buffer[idx + 8] = (byte)(offset >> 8);
            buffer[idx + 9] = (byte)(offset >> 0);
            return 10;
        }
        static uint GetS7200WritePointer(ref byte[] buffer, uint idx, S7TCPCommJob j, int nOffset = 0)
        {
            int length = 0, offset;
            int bit = 0;
            int type = 0x02; // BYTE
            
            buffer[idx] = 0x10;
            buffer[idx + 1] = 0;
            buffer[idx + 2] = 0;
            buffer[idx + 3] = 0;
            buffer[idx + 4] = 0;
            buffer[idx + 5] = 0;
            buffer[idx + 6] = 0;
            buffer[idx + 7] = 0;
            buffer[idx + 8] = 0;
            buffer[idx + 9] = 0;

            switch (j.Format)
            {
                case Step7Format.frmBit:
                    if (j.Length == 1)
                    {
                        type = 0x01; // BOOL
                        length = j.Length;
                        bit = j.Bit;
                    }
                    else
                        length = j.Length / 8;
                    break;
                case Step7Format.frmByte:
                    length = j.Length;
                    break;
                case Step7Format.frmWord:
                    switch (j.Area)
                    {
                        case Step7Area.aTIEC:
                            type = 0x1f; // TIMER
                            length = j.Length * 2;
                            break;
                        case Step7Area.aCIEC:
                            type = 0x1e; // COUNTER
                            length = j.Length * 2;
                            break;
                        default:
                            length = j.Length * 2;
                            break;
                    }
                    break;
                case Step7Format.frmDWord:
                    length = j.Length * 4;
                    break;
            }

            switch (j.Area)
            {
                case Step7Area.aI:
                    buffer[idx + 6] = 0x81;
                    break;
                case Step7Area.aQ:
                    buffer[idx + 6] = 0x82;
                    break;
                case Step7Area.aM:
                    buffer[idx + 6] = 0x83;
                    break;
                case Step7Area.aD:
                    buffer[idx + 4] = (byte)(j.DbNumber >> 8);
                    buffer[idx + 5] = (byte)(j.DbNumber >> 0);
                    buffer[idx + 6] = 0x84;
                    break;
                case Step7Area.aS:
                    buffer[idx + 6] = 0x05;
                    break;
                case Step7Area.aAI:
                    buffer[idx + 6] = 0x06;
                    break;
                case Step7Area.aAQ:
                    buffer[idx + 6] = 0x07;
                    break;
                case Step7Area.aTIEC:
                    buffer[idx + 6] = 0x1f;
                    break;
                case Step7Area.aCIEC:
                    buffer[idx + 6] = 0x1e;
                    break;
                case Step7Area.aH:
                    buffer[idx + 6] = 0x20;
                    break;
            }

            buffer[idx + 1] = (byte)type;
            buffer[idx + 2] = (byte)(length >> 8);
            buffer[idx + 3] = (byte)(length >> 0);

            offset = ((j.Offset + nOffset) << 3) | bit;
            buffer[idx + 7] = (byte)(offset >> 16);
            buffer[idx + 8] = (byte)(offset >> 8);
            buffer[idx + 9] = (byte)(offset >> 0);
            return 10;
        }

        public static bool SetS7JobLength(S7TCPCommJob job, uint size = 0)
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
                        job.Length = (int)( (size + 3) / 4);
                    }
                    else
                    {
                        job.Length = (int)((size + 1) / 2);
                    }

                    break;
                case Step7Format.frmDWord:
                        job.Length = (int)( (size + 3) / 4);
                    break;
            }
            
            return job.Length > 0;
        }

        public static int CompareTagByOffset(Tag x, Tag y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0; //==
                else
                    return -1;// x < y
            }
            else
            {
                //x!= null
                if (y == null)
                    return 1; //x > y
                else
                {
                    if (x.ByteOffset > y.ByteOffset)
                        return 1;
                    else if (x.ByteOffset == y.ByteOffset)
                        return 0;
                    else
                        return -1;

                }
            }
        }

        public static bool ParseData(byte[] receivebuffer, ref S7TCPCommJob job, ref List<object> items)
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
            UInt16 WordVal;
            UInt32 DWordVal = 0;
            int TimeFactor;
            switch (job.Format)
            {
                case Step7Format.frmBit:
                    {
                        if (job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean || job.ElementNumber > 0)
                        {
                            int ByteNr;
                            byte BitNr;

                            if (job.TagsList[0].TagNode.ArrayDimension > 0)
                            {
                                BitNr = (byte)job.Bit;

                                for (int i = 0; i < job.Length; i++)
                                {
                                    ByteNr = (job.Bit + i) / 8;
                                    // reset bit mask on byte change
                                    if ((job.Bit + i) % 8 == 0)
                                    {
                                        BitNr = 0;
                                    }
                                    else
                                    {
                                        // skip 1st cycle
                                        if (i > 0)
                                            BitNr++;
                                    }

                                    // each tempBuffer is a element of bit array
                                    tempBuffer[i] = (byte)((receivebuffer[ByteNr] >> BitNr) & 1);
                                }
                            }
                            else
                            {
                                foreach (var tag in job.TagsList)
                                {
                                    // ByteOffset --> from start address bit nr (byte * 8 + bit nr)
                                    // calculate buffer's byte contain bit
                                    ByteNr = (int)((job.Bit + tag.ByteOffset) / 8);
                                    //
                                    // calculate bit nr of byte
                                    BitNr = (byte)((job.Bit + tag.ByteOffset) % 8);

                                    tempBuffer[tag.ByteOffset] = (byte)((receivebuffer[ByteNr] >> BitNr) & 1);
                                }
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
                    if (job.isLenStringEnable && receivebuffer.Length > 0 )
                    {
                        byte tempBufferSize = (byte)(receivebuffer.Length > receivebuffer[0] ? receivebuffer[0] : receivebuffer.Length - 1);
                        tempBuffer = new byte[tempBufferSize];
                        Array.Copy(receivebuffer, 1, tempBuffer, 0, tempBufferSize);
                    }
                    else if ((uint)job.TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.String && receivebuffer.Length > 0)
                    {
                        int stringData = 0;
                        while (stringData < job.Length && receivebuffer[stringData] != 0)
                            stringData++;
                        tempBuffer = new byte[stringData];
                        Array.Copy(receivebuffer,tempBuffer, stringData);
                    }
                    else
                    {
                        Array.Copy(receivebuffer, tempBuffer, job.Length);
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
                    break;
                case Step7Format.frmWord:
                    if (0 < (Len = job.Length))
                    {
                        int i = 0, j = 0;
                        do
                        {
                            WordVal = (UInt16)((receivebuffer[j++] << 8) | receivebuffer[j++]);
                            switch (job.Trans)
                            {
                                case Step7WordTrans.wtW:
                                    if (i < tempBuffer.Length)
                                        tempBuffer[i++] = (byte)(WordVal >> 0);
                                    if (i < tempBuffer.Length)
                                        tempBuffer[i++] = (byte)(WordVal >> 8);
                                    break;
                                case Step7WordTrans.wtC:
                                    WordVal = Bcd2Bin((UInt16)(WordVal & 0xfff));
                                    if (i < tempBuffer.Length)
                                        tempBuffer[i++] = (byte)(WordVal >> 0);
                                    if (i < tempBuffer.Length)
                                        tempBuffer[i++] = (byte)(WordVal >> 8);
                                    break;
                                case Step7WordTrans.wtT:
                                    TimeFactor = (WordVal >> 12) & 3;
                                    WordVal = Bcd2Bin((UInt16)(WordVal & 0xfff));
                                    switch (TimeFactor)
                                    {
                                        case 0:
                                            DWordVal = (UInt32)(WordVal * 10);
                                            break;
                                        case 1:
                                            DWordVal = (UInt32)(WordVal * 100);
                                            break;
                                        case 2:
                                            DWordVal = (UInt32)(WordVal * 1000);
                                            break;
                                        case 3:
                                            DWordVal = (UInt32)(WordVal * 10000);
                                            break;
                                    }
                                    if (i < tempBuffer.Length)
                                        tempBuffer[i++] = (byte)(DWordVal >> 0);
                                    if (i < tempBuffer.Length)
                                        tempBuffer[i++] = (byte)(DWordVal >> 8);
                                    if (i < tempBuffer.Length)
                                        tempBuffer[i++] = (byte)(DWordVal >> 16);
                                    if (i < tempBuffer.Length)
                                        tempBuffer[i++] = (byte)(DWordVal >> 24);
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
                            if (i < tempBuffer.Length)
                                tempBuffer[i++] = receivebuffer[j + 3];
                            if (i < tempBuffer.Length)
                                tempBuffer[i++] = receivebuffer[j + 2];
                            if (i < tempBuffer.Length)
                                tempBuffer[i++] = receivebuffer[j + 1];
                            if (i < tempBuffer.Length)
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

        public static bool CheckReadResponse(List<byte> pData, int Len, ref int DataLen, int jobDataLen, int jobFormat, int jobLength)
        {
          if( pData[ 0 ] == 0xff ) {
            switch( pData[ 1 ] ) {
            case 0x03:
              DataLen = 1;
	          if( ( jobFormat == (int)Step7Format.frmBit ) && ( jobLength == 1 ) ) {
                return Len >= 1;
	          }
              return false;
              break;
            case 0x04:
            case 0x05:
              DataLen = ( ( pData[ 2 ] << 8 ) | pData[ 3 ] ) >> 3;
              break;
            case 0x07:
            case 0x09:
              DataLen = ( ( pData[ 2 ] << 8 ) | pData[ 3 ] );
              break;
            }
#if DEBUG
            if (DataLen != jobDataLen)
            {
                return false;
            }
#endif

            return DataLen == jobDataLen;
          } 

          else {
            DataLen = 0;
            return false;
          }
        }

        public static string GetErrorString(int err)
        {
            String str;
            switch (err)
            {
                case 0x01:
                    str = Properties.Resources.IDS_HWFAULT;
                    break;
                case 0x03:
                    str = Properties.Resources.IDS_ILLEGALOBJACCESS;
                    break;
                case 0x05:
                    str = Properties.Resources.IDS_INVALIDADDRESS;
                    break;
                case 0x06:
                    str = Properties.Resources.IDS_DATATYPENOTSUPP;
                    break;
                case 0x0A:
                    str = Properties.Resources.IDS_OBJNOTEXIST;
                    break;
                default:
                    str = Properties.Resources.IDS_UNKNOWNRESP;
                    break;
            }
            return str;
        }
 
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

        public static bool IsGetTotalWriteRequestLengthValid(uint size)
        {
            // size + 3
            return (size  <= S7Protocol.MAX_MPI_TLG_LEN);
        }
        #endregion
    }
}
