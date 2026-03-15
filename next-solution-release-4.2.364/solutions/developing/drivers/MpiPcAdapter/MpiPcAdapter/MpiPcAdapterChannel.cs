using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using SerialDriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;
using System.Threading;
using System.Text;
using System.Diagnostics;

namespace MpiPcAdapter
{
    class RequestTelegram
    {
        #region Constructors
        public RequestTelegram()
        {
            pdu = new byte[512];
            pduPointer = new ushortUnion(0);
            bBCC = 0;
        }
        #endregion

        #region Member
        public byte[] pdu;
        public ushortUnion pduPointer;
        public byte bBCC;
        ushortUnion wAux;
        #endregion
        #region Methods
        public void Init()
        {
            pduPointer.USHORT = 0;
            bBCC = 0;
        }
        public void Insert(byte value)
        {
            pdu[pduPointer.USHORT++] = value;
            if (value == MpiPcAdapterProtocol.DLE)
                pdu[pduPointer.USHORT++] = value;
            else
                bBCC ^= value;
        }
        public void Insert(ushort value)
        {
            wAux.USHORT = value;
            Insert(wAux.HIBYTE);
            Insert(wAux.LOBYTE);
        }
        public bool Insert(byte[] buffer)
        {
            if (buffer.Length == 0)
                return false;
            for (int i = 0; i < buffer.Length; i++ )
                Insert(buffer[i]);
            return true;
        }
        public void End()
        {
            bBCC ^= pdu[pduPointer.USHORT++] = MpiPcAdapterProtocol.DLE;
            bBCC ^= pdu[pduPointer.USHORT++] = MpiPcAdapterProtocol.ETX;
            bBCC ^= pdu[pduPointer.USHORT++] = bBCC;
        }

        #endregion
    }

    class MpiPcAdapterChannel : SerialChannel, IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public MpiPcAdapterChannel(CommunicationDriver commdriver, MpiPcAdapterChannelSettings settings)
            : base(commdriver, settings)
        {
            _PcMpiID = settings.PcMpiID;
            _MpiNetworkBitRate = settings.MpiNetworkBitRate;
            _PcOnlyMaster = settings.PcOnlyMaster;
            _HighestStationAddress = settings.HighestStationAddress;

            AddPrefix = false;
            
            InitLocalHandleFree();
        }

        #endregion

        #region Member

        RequestTelegram requestTelegram = new RequestTelegram();
        bool[] LocalHandleFree = new bool[MpiPcAdapterProtocol.MAX_LOCAL_HANDLE_ALLOWED];
        private Object thisLock = new Object();

        byte[] ReadBuf = new byte[512];
        byte[] TempBuf = new byte[512];
        byte[] Answer;
        byte[] WriteData;
        uint WriteDataByteOffset;
        uint WriteDataBitOffset;

        bool DleReceived = false;
        bool EtxReceived = false;
        bool DleOnBorder = false;
        bool EtxOnBorder = false;
        bool RecMsgComplete = false;
        byte RecBCC = 0;
        ushort RecChars = 0;
        bool AddPrefix;
        int Mpi3964rLastErr;
        byte TelSeqCounter;

        AdapterTypes AdapterType = AdapterTypes.SIEMENS_6ES7972_0CA21_0XA0;
        int FirmwareMajorVer = 0;
        int FirmwareMinorVer = 0;

        bool MpiInitDone = false;
        uint ConnCounter = 0;

        DriverErrorCodes ErrIDS = DriverErrorCodes.ErrorNoError;
        bool MpiJobTerminate = false;
        bool WaitData = false;
        bool onStop = false;

        #endregion

        #region Methods

        public void InitLocalHandleFree()
        {
            for (int i = 0; i < MpiPcAdapterProtocol.MAX_LOCAL_HANDLE_ALLOWED; i++)
            {
                LocalHandleFree[i] = true;
            }
        }
        public bool GetFreeLocalHandle(ref byte LocalHandle)
        {
            byte i = 0;
            lock (thisLock)
            {
                for (i = 0; i < MpiPcAdapterProtocol.MAX_LOCAL_HANDLE_ALLOWED; i++)
                {
                    if (LocalHandleFree[i] && (i != LocalHandle))
                    {
                        LocalHandle = i;
                        LocalHandleFree[i] = false;
                        break;
                    }
                }
            }
            return (i < MpiPcAdapterProtocol.MAX_LOCAL_HANDLE_ALLOWED);
        }
        /*
        Frees the local handle, return TRUe only if it was occupied
        To call in station's ClosePlcConnection
        */
        public bool SetFreeLocalHandle(byte LocalHandle)
        {
	        if (!LocalHandleFree[LocalHandle])
            {
		        LocalHandleFree[LocalHandle] = true;
		        return true;
	        }
	        return true;
        }
 

        public bool InitMpiAdapter()
        {
            if (!MpiInitDone)
            {
                if (base.IsDeviceOpen())
                    base.DeviceClose();
                if (base.DeviceOpen())
                {
                    if (Start3964R())
                        if (SendCreateLine())
                            if (WaitCtrlChar(MpiPcAdapterProtocol.DLE))
                                if (GestResponse(MpiResponses.MPI_CREATELINE_RESP))
                                    MpiInitDone = true;
                    if (!MpiInitDone)
                    {
                        base.DeviceClose();
                    }
                }
            }

            return MpiInitDone;
        }
        public bool CloseMpiAdapter()
        {
            MpiInitDone = false;

            foreach (var station in CommDriver.GetChannelStations(this))
            {
                MpiPcAdapterStation s = station as MpiPcAdapterStation;
                if (s != null)
                {
                    ClosePlcConnection(s);
                }
            }

            if (Start3964R())
                if (SendCloseLine())
                    if (WaitCtrlChar(MpiPcAdapterProtocol.DLE))
                        GestResponse(MpiResponses.MPI_CLOSELINE_RESP);

            return true;
        }
        public bool InitPlcConnection(MpiPcAdapterStation s)
        {
            if (!s.ConnEstablished)
            {
                byte LocalHandle = 0;
                if (!GetFreeLocalHandle(ref LocalHandle))
                {
                    return false;
                }
                s.LocalHandle = LocalHandle;

                if (GestCreateConnection(s, 1))
                    if (!GestResponse(MpiResponses.MPI_CREATECONN1_RESP, s))
                        CloseConnection(s);
                    else
                    {
                        s.ConnEstablished = true;
                        ConnCounter++;
                        if (GestCreateConnection(s, 2))
                            if (!GestResponse(MpiResponses.MPI_CREATECONN2_RESP, s))
                                CloseConnection(s);
                            else if (GestCreateConnection(s, 3))
                                if (!GestResponse(MpiResponses.MPI_ACKMSG_RESP, s))
                                    CloseConnection(s);
                                else if (!GestResponse(MpiResponses.MPI_CREATECONN3_RESP, s))
                                    CloseConnection(s);
                                else if (!GestAck(s))
                                    CloseConnection(s);
                                else
                                    s.ConnEstablished = true;
                    }

                if (!s.ConnEstablished)
                {
                    //String str;
                    //if (ErrIDS == IDS_CONNREFUSED){
                    //    str.Format(m_nErrIDS, m_nMpiAddress);
                    //}else
                    //    str.LoadString(m_nErrIDS);

                    //SetInError(true, m_nErrIDS, str);

                    SetFreeLocalHandle(s.LocalHandle);
                    SetFreeLocalHandle(s.RemoteHandle);
                }
            }

            return s.ConnEstablished;

        }

        public void CloseConnection(MpiPcAdapterStation s)
        {
            if (!ClosePlcConnection(s))
                CloseMpiAdapter();
            else if (ConnCounter == 0)
                CloseMpiAdapter();
        }

        
        public bool ClosePlcConnection(MpiPcAdapterStation s)
        {
            if (!s.ConnEstablished)
                return true;

            s.ConnEstablished = false;
            if (ConnCounter > 0)
                ConnCounter--;

            s.SeqNumber = 0;
            SetFreeLocalHandle(s.RemoteHandle);
            SetFreeLocalHandle(s.LocalHandle);
 
            if (Start3964R())
                if (SendCloseConnection(s))
                    if (WaitCtrlChar(MpiPcAdapterProtocol.DLE))
                        if (GestResponse(MpiResponses.MPI_CLOSECONN_RESP, s))
                            return true;

            return false;
        }

        bool SendCommand( MpiPcAdapterStation s, MpiPcAdapterCommJob j)
        {
 
            uintUnion uintAux = new uintUnion(0);
            uint elementsize;

            switch (j.Format)
            {
                case Step7Format.frmByte:
                case Step7Format.frmBit:
                    elementsize = 1; // Bytes or Bits
                    break;
                case Step7Format.frmWord:
                    elementsize = 2; // Words
                    break;
                case Step7Format.frmDWord:
                    elementsize = 4; // DWords
                    break;
                default:
                    return false;
            }

            if (j.Type == DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
            {
                lock (j.retLockList())
                {
                    if (j.TagsListToWrite.Count == 0)
                        j.TagsListToWrite.AddRange(j.TagsList);
                }
            }

            uint bitOffset = 0;
            uint byteOffset = 0; 
            
            byte[] wBuffer = new byte[0];
            if (!j.IsRead)
            {
                bitOffset = WriteDataBitOffset;
                byteOffset = WriteDataByteOffset;
                wBuffer = MpiPcAdapterProtocol.GetPartialWriteData(WriteData, j, elementsize);
                if (wBuffer.Length == 0)
                    return false;
            }
            else
            {
                MpiPcAdapterProtocol.IncReadData(j);
            }


            ushort BytesToRequest ;
            if (j.Format == Step7Format.frmBit)
            {
                //When dealing with bits, the internal buffer is allocated a number of bytes equal to the
                //number of bits to be exchanged, plus the bit offset of the first bit (see PrepareAndValidate)
                //we always request n full bytes so that all bits are included. 
                if (j.IsRead)
                    BytesToRequest = 1;
                else
                {
                    BytesToRequest = (ushort)((j.Bit + j.GetNumOfElements((int)elementsize) + 7) / 8);
                    uint tmtOffset = byteOffset + j.PartialElementStart;
                    bitOffset = tmtOffset % 8;
                    byteOffset = tmtOffset / 8;
                }
            }
            else
            {
                if (!j.IsRead)
                    BytesToRequest = (ushort)wBuffer.Length;
                else
                    BytesToRequest = (ushort)(j.GetNumOfElements((int)elementsize) * elementsize);
                byteOffset += j.PartialElementStart;
            }

            //the only case when we use more than one S7 Pointer is for writing bits
            byte NumOfPointers;
            if (!j.IsRead && j.Format == Step7Format.frmBit)
                NumOfPointers = (byte)j.GetNumOfElements((int)elementsize);
            else
                NumOfPointers = 0x01;

            requestTelegram.Init();

            requestTelegram.Insert(0x00) ; // ?
            requestTelegram.Insert(0x0C) ; // ?

            requestTelegram.Insert(s.RemoteHandle); // ?
            requestTelegram.Insert(s.LocalHandle); // ?

            requestTelegram.Insert(0xF1) ; // ?

            TelSeqCounter = s.SeqNumber;
            requestTelegram.Insert(TelSeqCounter) ; // ?

	        /*
	         *	Start of Protocol Data Unit (PDU)
	         */
	        requestTelegram.Insert(0x32); // PROTO_ID
	        requestTelegram.Insert(0x01); // ROSCTR Remote Operating Services Control
	        //0x01 = Acknowledgement request
	        //0x02 = Acknowledgement without parameter and data fields
	        //0x03 = Acknowledgement with either or both parameter and data fields
	        //0x07 = Acknowledgement for functions with extended header information in the parameter area
	        requestTelegram.Insert(0x00); // RED_ID Redundancy Identification (HI)
	        requestTelegram.Insert(0x00); // RED_ID Redundancy Identification (LO)

	        requestTelegram.Insert(0x00); // PDU_REF (HI)
	        requestTelegram.Insert(0x00); // PDU_REF (LO)

            if (!j.IsRead)
                requestTelegram.Insert((ushort)(2 + NumOfPointers * 12)); // PAR_LG Parameter length .
            else
                requestTelegram.Insert((ushort)2); // PAR_LG Parameter length .

            //calculate the DATA_LG
            if (j.IsRead)
                requestTelegram.Insert((ushort)0); // DAT_LG Data Length
	        else
	        {
		        //Write Cmd
		        if (j.Format == Step7Format.frmBit)
                    requestTelegram.Insert((ushort)((BytesToRequest - 1) * 6 + 5)); // DAT_LG Data Length
		        else
                    requestTelegram.Insert((ushort)(4 + BytesToRequest)); // DAT_LG Data Length
	        }

            requestTelegram.Insert((byte)(j.IsRead ? 0x04 : 0x05)); // SERVICE_ID
	        requestTelegram.Insert(NumOfPointers); // Number of S7 pointers

	        //
	        //From here on we Build the S7 pointer to the PLC data
	        //

	        byte n;
	        for (n=0; n<NumOfPointers; n++)
	        {
                requestTelegram.Insert(0x12) ; // Variable Specification
                requestTelegram.Insert(0x0A) ; // V_ADDR_LG Variable Length

		        //Syntax ID
                requestTelegram.Insert(MpiPcAdapterProtocol.DLE); // constant

		        // Type.
		        switch (j.Area)
		        {
		        case Step7Area.aC:
                    requestTelegram.Insert(0x1C) ; // Counter (S7300/400)
			        break;
		        case Step7Area.aT:
                    requestTelegram.Insert(0x1D) ; // Timer (S7300/400)
			        break;
		        default:
                    switch (j.Format)
                    {
                        case Step7Format.frmBit:
                            requestTelegram.Insert((byte)(j.IsRead ? 0x02 : 0x01)); 
                            break;
                        case Step7Format.frmByte:
                            requestTelegram.Insert(0x02) ; 
                            break;
                        case Step7Format.frmWord:
                            requestTelegram.Insert(0x04) ; 
                            break;
                        case Step7Format.frmDWord:
                            requestTelegram.Insert(0x06) ; 
                            break;
                    }
			        break;
		        }
		        // Number of elements.
                if (!j.IsRead && j.Format == Step7Format.frmBit)
                    requestTelegram.Insert((ushort)1);
                else
                    requestTelegram.Insert((ushort)j.GetNumOfElements((int)elementsize));

		        if(j.Area != Step7Area.aC && j.Area != Step7Area.aT) 
		        {
			        // The following is for all data types except timers and counters
			        // DB number.
			        if(j.Area == Step7Area.aD) 
                        requestTelegram.Insert((ushort)(j.DbNumber)) ; //Subarea field
			        else 
                        requestTelegram.Insert((ushort)0) ; //Subarea field

			        // Area pointer
			        //10000zzz 00000yyy yyyyyyyy yyyyyxxx
			        //z=Data Area	y=Byte Address	x=Bit Number
			        // Modified in version 10.1.0.6 (FOGBUGZ 1962)
			        //bBCC ^= m_WriteBuf[uCh++] = (0x80 | (BYTE)SiemAddress.GetDataArea()); // Operand area.
			        Step7Area DataArea = j.Area;
			        if(j.Area == Step7Area.aAQ || j.Area == Step7Area.aAI)
                        requestTelegram.Insert((byte)(0x80 | (byte)Step7Area.aP)); //Operand area
                    else
                        requestTelegram.Insert((byte)(0x80 | (byte)j.Area)); //Operand area
			
			        int Elem = (int)(j.Offset + byteOffset);
                    byte BitNum = 0;
                    if (!j.IsRead )
			        {
                        if (j.Format == Step7Format.frmBit)
                        {
                            int BitOffs = (int)(j.Bit + n + bitOffset);
                            Elem += ( BitOffs / 8);
                            BitNum = (byte)(BitOffs % 8);
                        }
                    }
			        else
			        {
                        if (j.Format == Step7Format.frmBit)
                            BitNum = (byte)j.Bit;

			        }
                    uintAux.UINT = (uint)((Elem << 3) | BitNum);
                    
                    requestTelegram.Insert(uintAux.HIUSHORT.LOBYTE) ; 
                    requestTelegram.Insert(uintAux.LOUSHORT.USHORT) ; 
        
		        }
		        else
		        {
			        //This is for timers and counters only
                    requestTelegram.Insert(0x00) ; 
                    requestTelegram.Insert(0x00) ; 

			        //Type is always WORD
			        if (j.Area == Step7Area.aC)
                        requestTelegram.Insert(0x1C) ; 
			        else	//Timer
                        requestTelegram.Insert(0x1D) ;

                    requestTelegram.Insert(0);

                    if (!j.IsRead)
                    {
                        if (j.Area == Step7Area.aT)
                            requestTelegram.Insert((ushort)(j.Offset + byteOffset / (elementsize * 2)));
                        else
                            requestTelegram.Insert((ushort)(j.Offset + byteOffset / elementsize));
                    }
                    else
                    {
                        requestTelegram.Insert((ushort)(j.Offset));
                    }
		        }
	        }	//for (BYTE n=0; n<nNumOfPointers; n++)

            if (!j.IsRead)
	        {
                //if (wBuffer.Count() != BytesToRequest * NumOfPointers * j.Length)
                //{
                //    ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ERRDEVICEWRITE;
                //    return false;
                //}

                ushort DataLength = requestTelegram.pduPointer.USHORT;	
		        for (n=0; n<NumOfPointers; n++)
		        {
			        //Note: for all data types except counter and timer, bit data type must be used
                    requestTelegram.Insert(0xFF) ; // Reserved

			        ushort Elems;
                    if(j.Area != Step7Area.aC && j.Area != Step7Area.aT) 
			        {
            	        if (j.Format == Step7Format.frmBit)
				        {
                            requestTelegram.Insert(0x03) ; // Data type = ONE bit.
					        Elems = 1;
				        }
				        else
				        {
                            requestTelegram.Insert(0x04) ; // Data type = bit.
                            Elems = (ushort)(BytesToRequest * 8);
				        }
			        }
			        else
			        {
                        requestTelegram.Insert(0x09) ; // Data type = byte.
                        Elems = (ushort)(j.GetNumOfElements((int)elementsize) * 2);
                    }

                    requestTelegram.Insert(Elems) ; // Length in bits

                    if (j.Format == Step7Format.frmBit)
                    {
                        if ((wBuffer[n/8] & (1 << (n % 8))) == 0)
                            requestTelegram.Insert(0);
                        else
                            requestTelegram.Insert(1);
                    }
                    else
                        requestTelegram.Insert(wBuffer);

                    
                    //Add fill byte for all values except the last one
                    if (j.Format == Step7Format.frmBit && n < NumOfPointers-1)
                        requestTelegram.Insert(0x00) ;

                }	//for (n=0; n<nNumOfPointers; n++)
	        }

	        //End of telegram
            requestTelegram.End();

            if (!DeviceWrite(requestTelegram.pdu, requestTelegram.pduPointer.USHORT))
            {
                ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ERRDEVICEWRITE;
                Thread.Sleep(1);
                return false;
            }
            return true;
        }


        bool GestAck(MpiPcAdapterStation s)
        {
            if (Start3964R())
                if (SendAck(s))
                    if (WaitCtrlChar(MpiPcAdapterProtocol.DLE))
                        return true;
            return false;
        }
        bool SendAck(MpiPcAdapterStation s)
        {
            requestTelegram.Init();

            requestTelegram.Insert(0x00) ; // ?
            requestTelegram.Insert(0x0C) ; // ?

            requestTelegram.Insert(s.RemoteHandle); // PLC Conn
            requestTelegram.Insert(s.LocalHandle); // PLC Conn

            requestTelegram.Insert(0xB0); // ACK code
            requestTelegram.Insert(0x01) ; // 

            requestTelegram.Insert(TelSeqCounter) ; // ?

            requestTelegram.End();

            if (!DeviceWrite(requestTelegram.pdu, requestTelegram.pduPointer.USHORT))
            {
                ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ERRDEVICEWRITE;
                Thread.Sleep(1);
                return false;
            }
            return true;
        }
 
        bool SendCreateLine()
        {
            requestTelegram.Init();
            requestTelegram.Insert(0x01); // START_TOOL_PROTOCOL_ID.
            requestTelegram.Insert(0x03) ; // START_TOOL_FUNCTION_ID.
            requestTelegram.Insert(0x02) ; // Status byte.
            requestTelegram.Insert((byte)(_HighestStationAddress + 8));
            requestTelegram.Insert(0x00); // ?

            requestTelegram.Insert(0x9F) ; // ?
            requestTelegram.Insert(0x01) ; // ?
            requestTelegram.Insert(0x3C) ; // ?
            requestTelegram.Insert(0x00) ; // ?
            requestTelegram.Insert(0x90) ; // ?
            requestTelegram.Insert(0x01) ; // ?
            requestTelegram.Insert(0x14) ; // ?
            requestTelegram.Insert(0x00) ; // ?
            requestTelegram.Insert(0x00) ; // ?
            requestTelegram.Insert(0x05) ; // ?

            switch(_MpiNetworkBitRate) 
            {
                //case MpiNetworkBitRates.BITRATE_19_2K:
                //    requestBuffer.insert(0x02) ; 
                //    break;
                //case MpiNetworkBitRates.BITRATE_187_5K:
                //    requestBuffer.insert(0x02) ; 
                //    break;
                //case MpiNetworkBitRates.BITRATE_1_5M:
                //    requestBuffer.insert(0x02) ; 
                //    break;
                default:
                    requestTelegram.Insert(0x02) ;// 187.5 Kbps.
                    break;
            }

            requestTelegram.Insert(_PcMpiID) ;
            requestTelegram.Insert(_HighestStationAddress);

            requestTelegram.Insert(0x02) ; // ?
            requestTelegram.Insert(0x01) ; // ?
            requestTelegram.Insert(0x01) ; // ?
            requestTelegram.Insert(0x03) ; // ?
            
            if(_PcOnlyMaster)  // PC == only master on the bus?
                requestTelegram.Insert(0x80) ;
            else 
                requestTelegram.Insert(0x81) ;

            requestTelegram.End();
            
            if (!DeviceWrite(requestTelegram.pdu, requestTelegram.pduPointer.USHORT))
            {
                ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ERRDEVICEWRITE;
                Thread.Sleep(1);
                return false;
            }
            return true;
        }
        bool SendCloseLine()
        {
            requestTelegram.Init();

            requestTelegram.Insert(0x01); // START_TOOL_PROTOCOL_ID.
            requestTelegram.Insert(0x04) ; // START_TOOL_FUNCTION_ID.
            requestTelegram.Insert(0x02) ; // Status byte.

            requestTelegram.End();
            if (!DeviceWrite(requestTelegram.pdu, requestTelegram.pduPointer.USHORT))
            {
                ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ERRDEVICEWRITE;
                Thread.Sleep(1);
                return false;
            }
            return true;
        }

        bool GestCreateConnection(MpiPcAdapterStation s,uint MsgNum)
        {
            if (Start3964R())
                if (SendCreateConnection(s, MsgNum))
                    if (WaitCtrlChar(MpiPcAdapterProtocol.DLE))
                        return true;
            return false;
        }
        bool SendCreateConnection(MpiPcAdapterStation s,uint MsgNum)
        {
            requestTelegram.Init();
	        switch (MsgNum)
	        {
	        case 1:
                requestTelegram.Insert(0x00); // Protocol?
                requestTelegram.Insert(0x0D); // ?
                requestTelegram.Insert(0x00); // ?
                
                requestTelegram.Insert(s.LocalHandle); // ?

                requestTelegram.Insert(0xE0); // ?
                requestTelegram.Insert(0x04); // ?
                requestTelegram.Insert(0x00); // ?
                requestTelegram.Insert(0x80); // ?
                requestTelegram.Insert(0x00); // ?
                requestTelegram.Insert(0x02); // ?

                requestTelegram.Insert(0x01); // ?
                requestTelegram.Insert(0x06); // ?

                requestTelegram.Insert(0x01); // ?
		        
                requestTelegram.Insert(0x00); // ?
                requestTelegram.Insert(0x00); // ?
                requestTelegram.Insert(0x01); // ?
                requestTelegram.Insert(0x02); // ?

                requestTelegram.Insert(s.DeviceID); // ?

                requestTelegram.Insert(0x01); // ?

		        //Rack and slot are not used
                requestTelegram.Insert(0); // Bit 7..5 = Rack, 4..0 = Slot.
		        break;

	        case 2:
                requestTelegram.Insert(0x00); // Protocol?
                requestTelegram.Insert(0x0C); // ?

                requestTelegram.Insert(s.RemoteHandle); // ?
                requestTelegram.Insert(s.LocalHandle); // ?

                requestTelegram.Insert(0x05); // ?
                requestTelegram.Insert(0x01); // ?
		        break;
	        case 3:
                requestTelegram.Insert(0x00); // Protocol?
                requestTelegram.Insert(0x0C); // ?

                requestTelegram.Insert(s.RemoteHandle); // ?
                requestTelegram.Insert(s.LocalHandle); // ?

                requestTelegram.Insert(0xF1); // ?

                TelSeqCounter = 0;
                s.SeqNumber = TelSeqCounter;
                requestTelegram.Insert(TelSeqCounter); // ?

                requestTelegram.Insert(0x32); // ?
                requestTelegram.Insert(0x01); // ?
                requestTelegram.Insert(0x00); // ?
                requestTelegram.Insert(0x00); // ?

                requestTelegram.Insert(0x00); // ?
                requestTelegram.Insert(0x00); // ?


                requestTelegram.Insert(0x00); // ?
                requestTelegram.Insert(0x08); // ?
                requestTelegram.Insert(0x00); // ?
                requestTelegram.Insert(0x00); // ?
                requestTelegram.Insert(0xF0); // ?
                requestTelegram.Insert(0x00); // ?
                requestTelegram.Insert(0x00); // ?
                requestTelegram.Insert(0x01); // ?
                requestTelegram.Insert(0x00); // ?
                requestTelegram.Insert(0x02); // ?
                requestTelegram.Insert(0x01); // ?
                requestTelegram.Insert(0xE0); // ?
                break;
            }

            requestTelegram.End();

            if (!DeviceWrite(requestTelegram.pdu, requestTelegram.pduPointer.USHORT))
            {
                ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ERRDEVICEWRITE;
                Thread.Sleep(1);
                return false;
            }

	        return true;
        }

        bool SendCloseConnection(MpiPcAdapterStation s)
        {
            requestTelegram.Init();

            requestTelegram.Insert(0x00); // Protocol?
            requestTelegram.Insert(0x0C); // ?

            requestTelegram.Insert(s.RemoteHandle); // ?
            requestTelegram.Insert(s.LocalHandle); // ?

            requestTelegram.Insert(0x80); // ?

            requestTelegram.End();

            if (!DeviceWrite(requestTelegram.pdu, requestTelegram.pduPointer.USHORT))
            {
                ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ERRDEVICEWRITE;
                Thread.Sleep(1);
                return false;
            }

	        return true;
        }

        bool GestResponse(MpiResponses resp, MpiPcAdapterStation s = null,  MpiPcAdapterCommJob j = null)
        {
            if (WaitCtrlChar(MpiPcAdapterProtocol.STX))
                if (SendCtrlChar(MpiPcAdapterProtocol.DLE))
                    if (WaitResponse(resp, s, j))
                        if (SendCtrlChar(MpiPcAdapterProtocol.DLE))
                            if (ErrIDS == DriverErrorCodes.ErrorNoError)
                                return true;
            return false;
        }

        bool WaitResponse(MpiResponses resp, MpiPcAdapterStation s = null, MpiPcAdapterCommJob j = null)
        {                                     
            bool ReadOK;
            uint Ch;
            int ExpectedBytesOk = 0;
            uint MinExpectedBytes = 0;
            uint ExtraChars = 0;
            int BytesToRequest = 0;
            bool IsBit = false;
            int elementsize = 0;
            
            DleReceived = false;
            EtxReceived = false;
            DleOnBorder = false;
            EtxOnBorder = false;
            RecMsgComplete = false;
            RecBCC = 0;
            RecChars = 0;

            if (j != null)
            {
                IsBit = j.Format == Step7Format.frmBit;
                
                switch (j.Format)
                {
                    case Step7Format.frmByte:
                    case Step7Format.frmBit:
                        elementsize = 1; // Bytes or Bits
                        break;
                    case Step7Format.frmWord:
                        elementsize = 2; // Words
                        break;
                    case Step7Format.frmDWord:
                        elementsize = 4; // DWords
                        break;
                    default:
                        return false;
                }
                BytesToRequest = elementsize * j.GetNumOfElements((int)elementsize);
            }
            
            switch (resp)
            {
                case MpiResponses.MPI_CREATELINE_RESP:
                    ExpectedBytesOk = 9;
                    MinExpectedBytes = 5;
                    break;
                case MpiResponses.MPI_CREATECONN1_RESP:
                    ExpectedBytesOk = 20;
                    MinExpectedBytes = 5;
                    break;
                case MpiResponses.MPI_CREATECONN2_RESP:
                    ExpectedBytesOk = 6;
                    MinExpectedBytes = 5;
                    break;
                case MpiResponses.MPI_CREATECONN3_RESP:
                    ExpectedBytesOk = 26;
                    MinExpectedBytes = 5;
                    break;
                case MpiResponses.MPI_ACKMSG_RESP:
                    ExpectedBytesOk = 7;
                    MinExpectedBytes = 5;
                    break;
                case MpiResponses.MPI_READ_RESP:
                    ExpectedBytesOk = 24 + BytesToRequest;
                    MinExpectedBytes = 24;
                    break;
                case MpiResponses.MPI_WRITE_RESP:
                    if (IsBit)
                        //we receive one access result for each bit we have written
                        ExpectedBytesOk = (int)(20 + j.GetNumOfElements(elementsize));
                    else
                        ExpectedBytesOk = 21;
                    MinExpectedBytes = 21;
                    break;
                case MpiResponses.MPI_CLOSELINE_RESP:
                    ExpectedBytesOk = 3;
                    MinExpectedBytes = 3;
                    break;
                case MpiResponses.MPI_CLOSECONN_RESP:
                    ExpectedBytesOk = 5;
                    MinExpectedBytes = 5;
                    break;
                }

	            ExpectedBytesOk += 3;
	            MinExpectedBytes +=3;		//account for DLE+ETX+BCC
	            Ch = MinExpectedBytes;

            do 
	        {
                if (!(ReadOK = DeviceWaitData(ReadBuf, Ch)))
		        {
                    ErrIDS = DriverErrorCodes.ErrorTimeOut;

			        return false;
		        }
		        //we have to read nExtraChars more in case there are DLEs in the received data
		        ExtraChars = GetReadChars(ReadBuf, Ch);
		        Ch = ExtraChars;
	        } while (Ch > 0);

	        if (!RecMsgComplete)
	        {
		        if (EtxOnBorder)
		        {
			        EtxOnBorder = false;
			        ExpectedBytesOk--;
		        }

		        if (ExpectedBytesOk - MinExpectedBytes > 0)
		        {
			        Ch = (uint)(ExpectedBytesOk - MinExpectedBytes);
			        do 
			        {
                        if (!(ReadOK = DeviceWaitData(ReadBuf, Ch)))
				        {
                            ErrIDS = DriverErrorCodes.ErrorTimeOut;
					        return false;
				        }
				        //we have to read nExtraChars more in case there are DLEs in the received data
				        ExtraChars = GetReadChars(ReadBuf, Ch);
				        Ch = ExtraChars;
			        } while (Ch > 0);
		        }
		        if (!RecMsgComplete)
		        {
                    ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ETXNOTRECEIVED;
			        return false;
		        }
	        }

	        ushort RecCharsTot = (ushort)(RecChars + 3);	//account for DLE ETX BCC
	        //we have received the complete telegram, now we check the BCC
	        byte bCC = BCC(TempBuf, RecChars);
	        if (RecBCC != bCC)
	        {
                ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_PLCBCC;
		        return false;
	        }

	        String AnswerBuffer = "";
	        int SearchIndex;
	        bool ByteData = false;
	        ushort Offs = (ushort)(AddPrefix? 2 : 0);


            //data integrity is OK, now we can go through the received data
            //note that RecCharsTot = response telegram length + 3 (DLE+ETX+BCC)
            switch (resp)
            {
            case MpiResponses.MPI_CREATELINE_RESP:
                if( RecCharsTot < 12 ) 
                {
                    ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ANSWERTOOSHORT;
                    return true;
                }
                if( (TempBuf[0] != 0x01) ||
                    (TempBuf[1] !=  0x03) ) 
                {
                    ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                    return true;
                }

                Encoding ascii = Encoding.ASCII;
                Encoding unicode = Encoding.Unicode;
                char[] asciiChars = new char[RecChars];
                ascii.GetChars(TempBuf, 0, RecChars, asciiChars, 0);
                AnswerBuffer = new string(asciiChars);
                
                SearchIndex = AnswerBuffer.IndexOf("E=");
                if( SearchIndex < 0 ) 
                {
                    SearchIndex = AnswerBuffer.IndexOf( "e=");
                }
                if( SearchIndex >= 0 ) 
                {
                    if( (int)RecCharsTot > (SearchIndex + 6) )
                    {
                        try
                        {
                            Mpi3964rLastErr = Convert.ToInt32(AnswerBuffer.Substring(SearchIndex+2,4), 16);
                        }
                        catch (Exception e)
                        {
                            ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                            return true;
                        }
                    }
                    else
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                        Mpi3964rLastErr = 0;
                        return true;
                    }

                    ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_RETURNEDERR;

                    return true;
                }
                // Get the firmware version of the adapter and set, accordingly to
                // it, the adapter type.
                else 
                {
                    SearchIndex = AnswerBuffer.IndexOf( "V" );
                    if (SearchIndex < 0)
                    {
                        SearchIndex = AnswerBuffer.IndexOf("v");
                    }
                    if (SearchIndex < 0) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                        return true;
                    }
                    if( (int)RecCharsTot < (SearchIndex + 6) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ANSWERTOOSHORT;
                        return true;
                    }
                    
                    if(!Int32.TryParse(AnswerBuffer.Substring(SearchIndex+1,2),out FirmwareMajorVer) ||
                        !Int32.TryParse(AnswerBuffer.Substring(SearchIndex+4,2),out FirmwareMinorVer)
                        )
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                        return true;
                    }

                    if (FirmwareMajorVer > 0 || FirmwareMinorVer > 60) 
                    {
                        AdapterType = AdapterTypes.INAT_MPI_PPI_KABEL;
                    }
                    else 
                    {
                        AdapterType = AdapterTypes.SIEMENS_6ES7972_0CA21_0XA0;
                    }
                }
                ErrIDS = DriverErrorCodes.ErrorNoError;

                return true;

            case MpiResponses.MPI_CREATECONN1_RESP:
                if(s == null ) 
                {
                    ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                    return false;
                }
                if( !AddPrefix ) 
                {
                    if( RecCharsTot < 8 ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ANSWERTOOSHORT;
                        return true;
                    }
                    if( (TempBuf[0] !=  0x00) ||
                        ((TempBuf[1] !=  0x0D) &&
                        (TempBuf[1] !=  0x0C)) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                        return true;
                    }
                    if( (RecCharsTot < 20) &&
                        ((TempBuf[1] ==  0x0D) ||
                        (TempBuf[1] ==  0x0C)) &&
                        (TempBuf[2] == s.LocalHandle) &&
                        (TempBuf[4] ==  0x80) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_CONNREFUSED;
                        return true;
                    }
                    if( (RecCharsTot >= 20) &&
                        (TempBuf[1] == 0x0C) &&
                        (TempBuf[2] == s.LocalHandle)) 
                    {
                        //get the remote handle and store it
                        s.RemoteHandle = TempBuf[3]; 
                        ErrIDS = DriverErrorCodes.ErrorNoError;
                        return true;
                    }
                    if (TempBuf[2] != s.LocalHandle) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                        return true;
                    }
                }
                else 
                {
                    if( RecCharsTot < 10 ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ANSWERTOOSHORT;
                        return true;
                    }
                    if( (TempBuf[0] !=  0x04) ||
                        (TempBuf[2] !=  0x080) ||
                        ((TempBuf[3] !=  0x0D) &&
                        (TempBuf[3] !=  0x0C)) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                        return true;
                    }
                    if( (RecCharsTot < 21) &&
                        ((TempBuf[3] ==  0x0D) ||
                        (TempBuf[3] ==  0x0C)) &&
                        (TempBuf[4] == s.LocalHandle) &&
                        (TempBuf[6] ==  0x80) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_CONNREFUSED;
                        return true;
                    }

                    // This is for supporting the INAT adapters
                    // with firmware version >= 2.11. 

                    if( (TempBuf[1] != (0x80 | s.DeviceID)) &&
                        (TempBuf[1] !=  s.DeviceID) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGSOURCE;
                        return true;
                    }
                    if( (RecCharsTot >= 21) &&
                        (TempBuf[3] == 0x0C) &&
                        (TempBuf[4] == s.LocalHandle))
                    {
                        s.RemoteHandle = TempBuf[5];
                        ErrIDS = DriverErrorCodes.ErrorNoError;
                        return true;
                    }

                    if (TempBuf[4] != s.LocalHandle) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                        return true;
                    }
                }
                ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;

                return true;

            case MpiResponses.MPI_CREATECONN2_RESP:
                if(s == null ) 
                {
                    ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                    return false;
                }
                if( !AddPrefix ) 
                {
                    if( RecCharsTot < 8 ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ANSWERTOOSHORT;
                        return true;
                    }
                    if( (TempBuf[0] != 0x00) ||
                        (TempBuf[1] != 0x0C) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                        return true;
                    }
                    if (TempBuf[2] != s.LocalHandle) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                        return true;
                    }
                    if( TempBuf[3] != s.RemoteHandle ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGSOURCE;
                        return true;
                    }
                    if( (TempBuf[4] == 0x80) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_CONNREFUSED;
                        return true;
                    }
                }
                else 
                {
                    if( RecCharsTot < 10 ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ANSWERTOOSHORT;
                        return true;
                    }
                    if( (TempBuf[0] != 0x04) ||
                        (TempBuf[2] != 0x080) ||
                        (TempBuf[3] != 0x0C) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                        return true;
                    }
                    if( TempBuf[4] != s.LocalHandle ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                        return true;
                    }

                    // This is for supporting the INAT adapters
                    // with firmware version >= 2.11. 
                    if( ((TempBuf[1] !=  (0x80 |  s.DeviceID)) &&
                        (TempBuf[1] !=   s.DeviceID)) ||
                        (TempBuf[5] != s.RemoteHandle) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGSOURCE;
                        return true;
                    }
                    if( TempBuf[6] ==  0x80 ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_CONNREFUSED;
                        return true;
                    }
                }
                ErrIDS = DriverErrorCodes.ErrorNoError;
                return true;
            case MpiResponses.MPI_CREATECONN3_RESP:
                if(s == null ) 
                {
                    ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                    return false;
                }
                if( !AddPrefix ) 
                {
                    if( (TempBuf[0] != 0x00) || (TempBuf[1] != 0x0C) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                        return true;
                    }
                    if (TempBuf[2] != s.LocalHandle) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                        return true;
                    }
                    if( TempBuf[3] != s.RemoteHandle ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGSOURCE;
                        return true;
                    }
                    if( (TempBuf[4] == 0x80) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_CONNREFUSED;
                        return true;
                    }
                    if( RecCharsTot < 26 ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ANSWERTOOSHORT;
                        return true;
                    }
                }
                else 
                {
                    if( (TempBuf[0] != 0x04) ||
                        (TempBuf[2] != 0x080) ||
                        (TempBuf[3] != 0x0C) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                        return true;
                    }
                    if (TempBuf[4] != s.LocalHandle) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                        return true;
                    }

                    // Modified in version 8.07.05 for supporting the INAT adapters
                    // with firmware version >= 2.11. 
                    if( ((TempBuf[1] != (0x80 |  s.DeviceID)) &&
                        (TempBuf[1] !=  s.DeviceID)) ||
                        (TempBuf[5] != s.RemoteHandle) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGSOURCE;
                        return true;
                    }
                    if( RecCharsTot < 7 ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ANSWERTOOSHORT;
                        return true;
                    }
                    if( TempBuf[6] == 0x80 ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_CONNREFUSED;
                        return true;
                    }
                    if( RecCharsTot < 28 ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ANSWERTOOSHORT;
                        return true;
                    }
                }
                ErrIDS = DriverErrorCodes.ErrorNoError;
                return true;
            case MpiResponses.MPI_ACKMSG_RESP:
                if(s == null ) 
                {
                    ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                    return false;
                }
                if( !AddPrefix ) 
                {
                    if( (TempBuf[0] != 0x00) || (TempBuf[1] != 0x0C) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                        return true;
                    }
                    if (TempBuf[2] != s.LocalHandle) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                        return true;
                    }
                    if( TempBuf[3] != s.RemoteHandle ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGSOURCE;
                        return true;
                    }
                    if( (TempBuf[4] == 0x80) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_CONNREFUSED;
                        return true;
                    }
                    if( (TempBuf[4] != 0xB0) || (TempBuf[5] != 0x01) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_NAK;
                        return true;
                    }
                    if( RecCharsTot < 7 ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ANSWERTOOSHORT;
                        return true;
                    }
                    if( TempBuf[6] != TelSeqCounter ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGSEQUENCE;
                        return true;
                    }
                }
                else 
                {
                    if( (TempBuf[0] != 0x04) ||
                        (TempBuf[2] != 0x080) ||
                        (TempBuf[3] != 0x0C) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                        return true;
                    }

                    if (TempBuf[4] != s.LocalHandle) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                        return true;
                    }

                    // This is for supporting the INAT adapters
                    // with firmware version >= 2.11. 
                    if( ((TempBuf[1] != (0x80 |  s.DeviceID)) &&
                        (TempBuf[1] !=  s.DeviceID)) ||
                        (TempBuf[5] != s.RemoteHandle) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGSOURCE;
                        return true;
                    }
                    if( RecCharsTot < 9 ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ANSWERTOOSHORT;
                        return true;
                    }
                    if( TempBuf[6] == 0x80 ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_CONNREFUSED;
                        return true;
                    }
                    if( (TempBuf[6] != 0xB0) || (TempBuf[7] != 0x01) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_NAK;
                        return true;
                    }
                    if( TempBuf[8] != TelSeqCounter ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGSEQUENCE;
                        return true;
                    }
                }
                ErrIDS = DriverErrorCodes.ErrorNoError;
                return true;

            case MpiResponses.MPI_READ_RESP:
                if(s == null ) 
                {
                    ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                    return false;
                }
                if( !AddPrefix ) 
                {
                    if( (TempBuf[0] != 0x00) || (TempBuf[1] != 0x0C) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                        return true;
                    }
                    if (TempBuf[2] != s.LocalHandle) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                        return true;
                    }
                    if( TempBuf[3] != s.RemoteHandle ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGSOURCE;
                        return true;
                    }
                    if( (TempBuf[4] == 0x80) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_CONNREFUSED;
                        return true;
                    }
                }
                else 
                {
                    if( (TempBuf[0] != 0x04) ||
                        (TempBuf[2] != 0x080) ||
                        (TempBuf[3] != 0x0C) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                        return true;
                    }
                    if (TempBuf[4] != s.LocalHandle) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                        return true;
                    }

                    // This is for supporting the INAT adapters
                    // with firmware version >= 2.11. 
                    if( ((TempBuf[1] != (0x80 |  s.DeviceID)) &&
                        (TempBuf[1] !=  s.DeviceID)) ||
                        (TempBuf[5] != s.RemoteHandle) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGSOURCE;
                        return true;
                    }
                    if( RecCharsTot < 7 ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ANSWERTOOSHORT;
                        return true;
                    }
                    if( TempBuf[6] ==  0x80 ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_CONNREFUSED;
                        return true;
                    }
                }

                if( TempBuf[5+Offs] != TelSeqCounter ) 
                {
                    ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGSEQUENCE;
                    return true;
                }
                if( RecCharsTot < 21+Offs) 
                {
                    ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ANSWERTOOSHORT;
                    return true;
                }
                ushortUnionSiemens w = new ushortUnionSiemens(TempBuf, (ushort)(14 + Offs));
                if( w.USHORT <= 4 ) 
                {
                    ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_TOOFEWDATA;
                    return true;
                }

                if( RecCharsTot < (w.USHORT + 20 + Offs) ) 
                {
                    ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ANSWERTOOSHORT;
                    return true;
                }

                if( (TempBuf[20+Offs] == 0xFF) 
                    && (TempBuf[21+Offs] == 0x09))
                    ByteData = true;

                // Calculate the number of bytes received.
                {		
                    if( (w.USHORT - 4) < BytesToRequest) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_TOOFEWDATA;
                        return true;
                    }
                    ushortUnionSiemens NumOfRecBytes = new ushortUnionSiemens(TempBuf, (ushort)(22 + Offs));
                    if( !ByteData ) 
                    {
                        //in case we received bit data (which is for all data types except timers and counters)
                        NumOfRecBytes.USHORT = (ushort)((NumOfRecBytes.USHORT + 7) /8);
                    }
                    if (NumOfRecBytes.USHORT == 0 || NumOfRecBytes.USHORT != BytesToRequest) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_TOOFEWDATA;
                        return true;
                    }
                    if (NumOfRecBytes.USHORT > Answer.Length - j.PartialElementStart)
                        NumOfRecBytes.USHORT = (ushort)(Answer.Length - j.PartialElementStart);
                    Array.Copy(TempBuf, 24 + Offs, Answer, j.PartialElementStart, NumOfRecBytes.USHORT);
                    ErrIDS = DriverErrorCodes.ErrorNoError;
                }
                return true;

            case MpiResponses.MPI_WRITE_RESP:
            {
                if (s == null)
                {
                    ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                    return false;
                }
                if (!AddPrefix) 
                {
                    if( (TempBuf[0] != 0x00) || (TempBuf[1] != 0x0C) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                        return true;
                    }
                    if (TempBuf[2] != s.LocalHandle) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                        return true;
                    }
                    if( TempBuf[3] != s.RemoteHandle ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGSOURCE;
                        return true;
                    }
                    if( (TempBuf[4] == 0x80) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_CONNREFUSED;
                        return true;
                    }
                }
                else 
                {
                    if( (TempBuf[0] != 0x04) ||
                        (TempBuf[2] != 0x080) ||
                        (TempBuf[3] != 0x0C) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                        return true;
                    }
                    if (TempBuf[4] != s.LocalHandle) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                        return true;
                    }

                    // This is for supporting the INAT adapters
                    // with firmware version >= 2.11. 
                    if( ((TempBuf[1] != (0x80 |  s.DeviceID)) &&
                        (TempBuf[1] !=  s.DeviceID)) ||
                        (TempBuf[5] != s.RemoteHandle) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGSOURCE;
                        return true;
                    }
                    if( RecCharsTot < 7 ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ANSWERTOOSHORT;
                        return true;
                    }
                    if( TempBuf[6] == 0x80 ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_CONNREFUSED;
                        return true;
                    }
                }

                if( TempBuf[5+Offs] != TelSeqCounter ) 
                {
                    ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGSEQUENCE;
                    return true;
                }
                if( RecCharsTot < 21 + Offs ) 
                {
                    ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ANSWERTOOSHORT;
                    return true;
                }

                //check access result (0xFF = OK)
                byte NumOfPointers;
                if (IsBit)
                    NumOfPointers = (byte)j.GetNumOfElements((int)elementsize);
                else
                    NumOfPointers = 1;

                for (byte i = 0; i < NumOfPointers; i++)
                {
                    if( TempBuf[20+Offs] != 0xFF ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRITEFAILED;
                        return true;
                    }
                }
                ErrIDS = DriverErrorCodes.ErrorNoError;
                return true;
            }
            case MpiResponses.MPI_CLOSELINE_RESP:
                if( (TempBuf[0] != 0x01) || (TempBuf[1] != 0x04) ) 
                {
                    ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                    return true;
                }
                ErrIDS = DriverErrorCodes.ErrorNoError;
                return true;

            case MpiResponses.MPI_CLOSECONN_RESP:
                if(s == null ) 
                {
                    ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                    return false;
                }

                if( !AddPrefix ) 
                {
                    if( (TempBuf[0] != 0x00) || (TempBuf[1] != 0x0C) ||
                        (TempBuf[4] != 0xC0) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                        return true;
                    }
                    if (TempBuf[2] != s.LocalHandle) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                        return true;
                    }
                    if( RecCharsTot < 8 ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ANSWERTOOSHORT;
                        return true;
                    }
                }
                else 
                {
                    if( (TempBuf[0] != 0x04) ||
                        (TempBuf[2] != 0x080) ||
                        (TempBuf[3] != 0x0C) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                        return true;
                    }
                    if (TempBuf[4] != s.LocalHandle) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION;
                        return true;
                    }
                    // This is for supporting the INAT adapters
                    // with firmware version >= 2.11. 
                    //
                    if( (TempBuf[1] != (0x80 |  s.DeviceID)) &&
                        (TempBuf[1] !=  s.DeviceID) ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_WRONGSOURCE;
                        return true;
                    }
                    if( RecCharsTot < 10 ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ANSWERTOOSHORT;
                        return true;
                    }
                    if( TempBuf[6] != 0x080 ) 
                    {
                        ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER;
                        return true;
                    }
                }
                ErrIDS = DriverErrorCodes.ErrorNoError;
                return true;
            }
            
            //we should never arrive here
            Debug.Assert(false);
            return false;
            
        }

        byte BCC(byte[] ReadBuffer, ushort Num)
        {
            byte Ret = MpiPcAdapterProtocol.DLE ^ MpiPcAdapterProtocol.ETX;
            ushort i = 0;

            while (i < Num)
            {
                if (ReadBuffer[i] != MpiPcAdapterProtocol.DLE)
                    Ret ^= ReadBuffer[i];
                i++;
            }
            return (Ret);
        }

        bool Start3964R()
        {
            DateTime init = DateTime.UtcNow;
            int sleepCycle = WaitTime;
            if (sleepCycle == 0)
                sleepCycle = 1;
            for (double dtime = 0;dtime < Timeout && !onStop; dtime = (DateTime.UtcNow - init).TotalMilliseconds)
            {
                lock (lockList)
                {
                    ReceiveBuffer.Clear();
                }

                if (SendCtrlChar(MpiPcAdapterProtocol.STX))
                {
                    byte WaitChar = 0;
                    if (WaitCtrlChar(ref WaitChar))
                    {
                        if (WaitChar == MpiPcAdapterProtocol.DLE)
                        {
                            ErrIDS = DriverErrorCodes.ErrorNoError;
                            return true;
                        }
                        if (WaitChar == MpiPcAdapterProtocol.STX)
                        {
                            if (!SendCtrlChar(MpiPcAdapterProtocol.DLE))
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            return false;
        }
        bool SendCtrlChar(byte c)
        {
            byte[] buf = new byte[1];
            buf[0] = c;
            
            if (DeviceWrite(buf, 1)) {
                return true;
            }
            else
            {
                ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_ERRDEVICEWRITE;
                Thread.Sleep(1);
                return false;
            }
        }
        bool WaitCtrlChar(byte c)
        {
            byte[] buf = new byte[1];

            if (DeviceWaitData(buf, 1))
            {
                if (buf[0] == c)
                {
                    ErrIDS = DriverErrorCodes.ErrorNoError;
                    return true;
                }
                ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_UNEXPEXTEDCHARRECEIVED;
                return false;
            }

            ErrIDS = DriverErrorCodes.ErrorTimeOut;
            return false;
        }
        bool WaitCtrlChar(ref byte c)
        {
            byte[] buf = new byte[1];

            if (DeviceWaitData(buf, 1))
            {
                c = buf[0];
                ErrIDS = DriverErrorCodes.ErrorNoError;
                return true;
            }

            ErrIDS = DriverErrorCodes.ErrorTimeOut;
            return false;
        }

        uint GetReadChars(byte[] ReadBuffer, uint ReadCh)
        {
            //copy chars from read buffer to temp buffer, taking DLEs into account
            //returns 0 if no DLE was read
            //return > 0 if we need to read more chars because DLE was read
            uint DLEs = 0;
            uint i = 0;

            while (ReadCh > i)
            {
                if (ReadBuffer[i] == MpiPcAdapterProtocol.DLE || DleReceived)
                {
                    if (!DleReceived)
                    {
                        DleReceived = true;
                        ++i;
                        continue;
                    }

                    if (ReadBuffer[i] == MpiPcAdapterProtocol.ETX || EtxReceived)
                    {
                        if (!EtxReceived)
                        {
                            if (DleOnBorder)
                                EtxOnBorder = true;	//must be reset by caller!!!

                            DleOnBorder = false;
                            EtxReceived = true;
                            ++i;	//with BCC we skip ETX for calculation
                            RecBCC = 0;
                            continue;
                        }

                        RecBCC = ReadBuffer[i];
                        DleReceived = false;
                        EtxReceived = false;
                        RecMsgComplete = true;
                        return 0;
                    }
                    else if (ReadBuffer[i] == MpiPcAdapterProtocol.DLE && DleReceived && !DleOnBorder)
                    {
                        ++DLEs;
                    }
                }

                DleOnBorder = false;
                DleReceived = false;
                EtxReceived = false;
                TempBuf[RecChars++] = ReadBuffer[i++];

            }


            if (DLEs == 0 && DleReceived && !EtxReceived)
            {
                DLEs++;
                DleOnBorder = true;
            }

            return DLEs;

        }
        public void PurgeInputChars()
        {
            DateTime init = DateTime.UtcNow;
            int sleepCycle = WaitTime;
            if (sleepCycle == 0)
                sleepCycle = 1;
            while (!onStop)
            {
                if (ReceiveBuffer.Count > 0)
                {
                    lock (lockList)
                    {
                        ReceiveBuffer.Clear();
                    }
                }
                Thread.Sleep(sleepCycle);
                double dtime = (DateTime.UtcNow - init).TotalMilliseconds;
                if (ReceiveBuffer.Count == 0 || dtime >= Timeout || onStop)
                    break;

            }
        }
        public bool DeviceWaitData(byte[] buffer, uint count)
        {
            WaitData = true;
            DateTime init = DateTime.UtcNow;
            uint WaitingBytes = GetBytesToRead();
            double dtime = (DateTime.UtcNow - init).TotalMilliseconds;

            bool terminate = false;
            while (dtime < Timeout / 2 && !terminate && !onStop)
            {
              
                if (ReceiveBuffer.Count >= count)
                {
                    lock (lockThreadObject)
                    {
                        ReceiveBuffer.GetRange(0, (int)count).CopyTo(buffer);
                        ReceiveBuffer.RemoveRange(0, (int)count);
                    }
                    terminate = true;
                }
                else
                {
                    if (NewDataToAnlyze.WaitOne(10))
                        NewDataToAnlyze.Reset();
                    dtime = (DateTime.UtcNow - init).TotalMilliseconds;
                }
            }
            if (!terminate && !onStop)
                PurgeInputChars();

            WaitData = false;

            return terminate;
        }


       
        #endregion

        #region Override Methods

        public override bool TestChannelComm()
        {
            return base.DeviceOpen();
        }

        public override bool IsDeviceOpen()
        {
            if(MpiInitDone)
            {
                return base.IsDeviceOpen() ;
            }
            SetStateCommandVariableBit(!MpiInitDone, (UInt16)ChannelVariableBits.ChannelUnconnected);
            return (MpiInitDone);
        }

        public override bool DeviceOpen()
        {
            if (base.DeviceClose())
                if (base.DeviceOpen())
                    if (InitMpiAdapter())
                        return true;
            return false;
        }

        public override bool DeviceClose()
        {
            if (MpiInitDone)
            {
                CloseMpiAdapter();
            }
            return base.DeviceClose();
        }
        public override bool DeviceWrite(byte[] buffer, uint count)
        {
            if (!onStop)
                return base.DeviceWrite(buffer, count);
            else
                return true;
        }

        protected override void WorkingThread(object data)
        {
            int sleepCycle = WaitTime;
            if (sleepCycle == 0)
                sleepCycle = 1;
            ListJobPending.Clear();
            ListJobExecuted.Clear();

            NextScheduleTimeJobsList = DateTime.UtcNow;
            int loop = 0;
            while (true)
            {
                MpiPcAdapterCommJob nextjob = null;
                if (ListJobPending.Count == 0 || MultiPointProtocol)
                {
                    ScheduleListJob();
                    if (SynchroJob != null)
                        nextjob = SynchroJob as MpiPcAdapterCommJob;
                    else
                        nextjob = GetNextPendingJob() as MpiPcAdapterCommJob;
                    if (nextjob != null)
                        ListJobPending.Add(nextjob);
                }
                else
                {
                    MpiPcAdapterCommJob lastJob = ListJobPending[0] as MpiPcAdapterCommJob;
                    if (lastJob.PartialElementEnd != 0 || lastJob.TagsListToWrite.Count != 0)
                        nextjob = lastJob;
                }

                if (nextjob != null && (!nextjob.ExecuteTask || (nextjob.PartialElementEnd != 0) || nextjob.TagsListToWrite.Count != 0))
                {
                    if (!IsDeviceOpen())
                        DeviceOpen();

                    ExecuteJob(nextjob);
                }

                lock (lockThreadObject)
                {
                    NewDataToAnlyze.Reset();
                    if (ListJobPending.Count > 0)
                    {
                        foreach (var job in ListJobPending)
                        {
                            if (ProcessNewData(job))
                                ListJobExecuted.Add(job);
                        }

                        foreach (var job in ListJobExecuted)
                        {
                            job.LastExecutionTime = DateTime.UtcNow;
                            if (SynchroJob != null && SynchroJob == job)
                            {
                                job.ResetSynchro.WaitOne(Timeout);
                                SynchroJob = null;
                                job.ResetSynchro.Reset();
                            }
                            ListJobPending.Remove(job);

                        }
                        ListJobExecuted.Clear();

                    }

                    if (ListJobPending.Count > 0)
                    {
                        if (!MultiPointProtocol)
                        {
                            double dtime = (DateTime.UtcNow - ListJobPending[0].StartExecutionTime/*LastExecutionTime*/).TotalMilliseconds;
                            if (dtime > Timeout)
                            {
                                if (SynchroJob != null && SynchroJob == ListJobPending[0])
                                {
                                    ListJobPending[0].ResetSynchro.WaitOne(Timeout);
                                    SynchroJob = null;
                                    ListJobPending[0].ResetSynchro.Reset();
                                }
                                //error
                                LastErrorCode = DriverErrorCodes.ErrorTimeOut;
                                ProcessNewData(ListJobPending[0]);
                                ListJobPending.RemoveAt(0);
                                ReceiveBuffer.Clear();
                            }
                        }
                    }
                }

                if (ListJobPending.Count == 0 && !KeepOpened && IsDeviceOpen())
                    DeviceClose();

                if (nextjob != null && StopWorkerThread.WaitOne(sleepCycle))
                    break;
                else if (++loop > 4)
                {
                    loop = 0;
                    if (StopWorkerThread.WaitOne(sleepCycle))
                        break;
                }
                StopWorkerThread.WaitOne(0);
            }
        }

        public override void ExecuteJob(CommJob job)
        {

            if (!MpiInitDone)
            {
                ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_CONNREFUSED; 
                return;
            }
 
            MpiPcAdapterStation s = job.Station as MpiPcAdapterStation;
            if (s == null)
                return;

            if (!InitPlcConnection(s))
            {
                ErrIDS = (DriverErrorCodes)MpiPcAdapterErrorCodes.IDS_CONNREFUSED;
                return;
            }

            MpiPcAdapterCommJob myJob = job as MpiPcAdapterCommJob;
            if (myJob == null)
                return;

            myJob.ExecuteTask = true;

            base.ExecuteJob(job);
           
            ReceiveBuffer.Clear();
            ErrIDS = DriverErrorCodes.ErrorNoError;			//IDS of the current error

            myJob.PartialElementStart = 0;
            myJob.PartialElementEnd = 0;
            if (myJob.IsRead)
            {
                Answer = new byte[job.TotalJobSize];
                //MpiPcAdapterProtocol.SetS7JobLength(myJob);
                if(MpiPcAdapterProtocol.SetS7JobLength(myJob) == false)
                {
                    lock (lockThreadObject)
                    {
                        RemovePendingJob(job);
                        job.IsPending = false;
                    }
                    return;
                }
            }
            else
            {
                uint nElement;
                WriteData = MpiPcAdapterProtocol.GetWriteData(myJob, ref WriteDataByteOffset, ref WriteDataBitOffset, out nElement);
                if(WriteData == null)
                {
                    lock (lockThreadObject)
                    {
                        RemovePendingJob(job);
                        job.IsPending = false;
                    }
                    return;
                }
                //MpiPcAdapterProtocol.SetS7JobLength(myJob, nElement);
                if(MpiPcAdapterProtocol.SetS7JobLength(myJob, nElement) == false)
                {
                    lock (lockThreadObject)
                    {
                        RemovePendingJob(job);
                        job.IsPending = false;
                    }
                    return;
                }
            }

            do
            {
                MpiJobTerminate = false;
                if (Start3964R())
                    if (SendCommand(s, myJob))
                        if (WaitCtrlChar(MpiPcAdapterProtocol.DLE))
                            if (GestResponse(MpiResponses.MPI_ACKMSG_RESP, s))
                                //now we should receive the response telegram to our request
                                if (GestResponse(myJob.IsRead ? MpiResponses.MPI_READ_RESP : MpiResponses.MPI_WRITE_RESP, s, myJob))
                                    //now we send the ACK telegram to the received response
                                    if (GestAck(s))
                                        MpiJobTerminate = true;
                myJob.PartialElementStart = myJob.PartialElementEnd;
            } while (MpiJobTerminate && myJob.PartialElementStart != 0 && !onStop);
            //now we send our request
            if (!MpiJobTerminate)
            {
                DriverErrorCodes memErrIDS = ErrIDS;
                if (memErrIDS < DriverErrorCodes.ErrorParsingAnswer )
                {
                    CloseConnection(s);
                    ErrIDS = memErrIDS;
                }
            }

        }

        public override bool ProcessNewData(CommJob pendingjob)
        {
            ExecutedJobArgs eJob;
            MpiPcAdapterCommJob myJob = pendingjob as MpiPcAdapterCommJob;
            if (myJob == null)
                return true;
            if (LastErrorCode != (int)DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError)
            {
                eJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                OnJobExecuted(eJob);
                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                ReceiveBuffer.Clear();
            }
            else
            {
                if (MpiJobTerminate && myJob.PartialElementStart == 0 )
                {
                    eJob = new ExecutedJobArgs { ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError, Job = pendingjob };
                    eJob.Values = Answer;
                    OnJobExecuted(eJob);
                }
                else
                {
                    eJob = new ExecutedJobArgs { ErrorCode = ErrIDS, Job = pendingjob };
                    OnJobExecuted(eJob);
                }

            }


            return true;
        }

        #endregion

        #region Properties

        private byte _PcMpiID;
        public byte PcMpiID
        {
            get { return _PcMpiID; }
            set { _PcMpiID = value; }
        }
        
        private MpiNetworkBitRates _MpiNetworkBitRate;
        public MpiNetworkBitRates MpiNetworkBitRate
        {
            get { return _MpiNetworkBitRate; }
            set { _MpiNetworkBitRate = value; }
        }

        private bool _PcOnlyMaster;
        public bool PcOnlyMaster
        {
            get { return _PcOnlyMaster; }
            set { _PcOnlyMaster = value; }
        }

        private byte _HighestStationAddress;
        public byte HighestStationAddress
        {
            get { return _HighestStationAddress; }
            set { _HighestStationAddress = value; }
        }

        
        #endregion

        #region methods

        #endregion
        #region IDisposable

        public override void Dispose()
        {
            onStop = true;
            while (WaitData)
                Thread.Sleep(1);

            base.Dispose();
        }
        #endregion
    }
}
