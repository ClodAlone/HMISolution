using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using SerialDriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;
using System.Threading;
using System.Text;
using System.Diagnostics;

namespace PPI
{

    class PPIChannel : SerialChannel, IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public PPIChannel(CommunicationDriver commdriver, PPIChannelSettings settings)
            : base(commdriver, settings)
        {
            _DriverAddress = settings.DriverAddress;
            _UseSignal = settings.UseSignal;
            SetDelays(settings.CommPortBaudRate);
            //DisconnectStation();
        }

        #endregion

        #region Member

        byte[] ReadBuf = new byte[PPIProtocol.MAX_READ];
        RequestTelegram requestTelegram = new RequestTelegram(PPIProtocol.MAX_WRITE);
        byte[] Answer;

        bool ConnectStationOk;

        int MaxTsyn;
        //int MaxTsdi;
        //int MaxTsdr;
        //int MaxTrdy;
        int MaxTqui;
        int MaxTset;
        int MaxTsm;
        int MinTid1;
        //int MinTtd;
        int MaxTslot;

        ushort TelSeqCounter;
        byte FC;	//Frame control
        PPIErrorCodes ErrIDS = (PPIErrorCodes)DriverErrorCodes.ErrorNoError;
        bool PPIJobTerminate = false;
        bool WaitData = false;
        bool onStop = false;

        #endregion

        #region Methods

        void SetRTS(bool Value)
        {
            if(UseSignal)
                RtsEnable(Value);
        }
        void SetDTR(bool Value)
        {
            if (UseSignal)
                DtrEnable(Value);
        }

        void SetDelays(int CommPortBaudRate)
        {
            float fTbit;
            // Ricavo il Tbit
            switch (CommPortBaudRate)
            {
                case 4800:
                    fTbit = (float)(1 / 4800.0);
                    break;
                case 9600:
                    fTbit = (float)(1 / 9600.0);
                    break;
                case 19200:
                    fTbit = (float)(1 / 19200.0);
                    break;
                case 38400:
                    fTbit = (float)(1 / 38400.0);
                    break;
                
                default:
                    fTbit = (float)(1 / 9600.0);
                    break;
            }

            MaxTsyn = (int)(1 + Properties.Settings.Default.ConstantForSerialWaitingTimeCalculation * (fTbit * 33)); // Tempo espresso in msec
            //MaxTsdi = (int)(1 + Properties.Settings.Default.ConstantForSerialWaitingTimeCalculation * (fTbit * 33));
            //MaxTsdr = (int)(1 + Properties.Settings.Default.ConstantForSerialWaitingTimeCalculation * (fTbit * 40));
            //MaxTrdy = (int)(1 + Properties.Settings.Default.ConstantForSerialWaitingTimeCalculation * (fTbit * 20));
            MaxTqui = (int)(1 + Properties.Settings.Default.ConstantForSerialWaitingTimeCalculation * (fTbit * 5));
            MaxTset = (int)(1 + Properties.Settings.Default.ConstantForSerialWaitingTimeCalculation * (fTbit * 2));
            MaxTsm = (int)(1 + Properties.Settings.Default.ConstantForSerialWaitingTimeCalculation * (fTbit * 2) + 2 * MaxTset + MaxTqui);
	        MinTid1	= 1 + MaxTsyn + MaxTsm;
            //MinTtd = (int)(1 + Properties.Settings.Default.ConstantForSerialWaitingTimeCalculation * (fTbit * 17));
            MaxTslot = (int)(1 + Properties.Settings.Default.ConstantForSerialWaitingTimeCalculation * (fTbit * 100));
        }

 
        bool ConnectStation(PPIStation s)
        {
            if (_DriverAddress == s.StationAddress)
            {
                ErrIDS = PPIErrorCodes.IDS_OVERLAPADDRESSES;
                return false;

            }
            if (ConnectStationOk)
                return true;

            if (!IsDeviceOpen())
                DeviceOpen();

            PurgeInputChars();
           
            SetDTR(false);
            Thread.Sleep(MinTid1);
            requestTelegram.Init(PPIProtocol.SD1);  // Poll request
            requestTelegram.insert(s.StationAddress);  // DA: Destination Address
            requestTelegram.insert(_DriverAddress);  // SA: Source Address
            requestTelegram.insert((byte)0x49);  // Init Adapter
            requestTelegram.End();

            if (!DeviceWrite(requestTelegram.buffer, requestTelegram.pointer))
            {
                ErrIDS = PPIErrorCodes.IDS_ERRDEVICEWRITE;
                return false;
            }
            while (GetBytesToWrite() !=0 )
                Thread.Sleep(1);
            SetRTS(false);

            if (!DeviceWaitData(ReadBuf, 6))  //wait for the response message (SD2)
            {
                ErrIDS = PPIErrorCodes.IDS_CONNREFUSED;
                return false;
            }

            ConnectStationOk = true;
            
            return true;
        }

        void DisconnectStation()
        {
            ConnectStationOk = false;
            FC = 0x0;
            DeviceClose();
        }

        bool Layer2(PPIStation s, PPICommJob j)
        {
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
                    ErrIDS = PPIErrorCodes.IDS_INVALIDTAG;
                    //lock (lockThreadObject)
                    //{
                    //    RemovePendingJob(j);
                    //    j.IsPending = false;
                    //}
                    return false;
            }

            if (ErrIDS != (PPIErrorCodes)DriverErrorCodes.ErrorNoError)
            {
                PurgeInputChars();
                ErrIDS = (PPIErrorCodes)DriverErrorCodes.ErrorNoError;
                DisconnectStation();
            }

            if (!ConnectStation(s))
            {
                //lock (lockThreadObject)
                //{
                //    RemovePendingJob(j);
                //    j.IsPending = false;
                //}
                return false;
            }


            ushort BytesToRequest;
            if (j.Format == Step7Format.frmBit)
                BytesToRequest = (ushort)((j.Bit + j.GetNumOfElements() + 7) / 8);
            else
                BytesToRequest = (ushort)(j.GetNumOfElements() * elementsize);


	        int Retry = 0;
            byte[] TempBuf;
            if (!PreparePDU(s, j, out TempBuf, ref BytesToRequest, elementsize))
            {
                //lock (lockThreadObject)
                //{
                //    RemovePendingJob(j);
                //    j.IsPending = false;
                //}
                return false;
            }
            requestTelegram.Init(PPIProtocol.SD2, (byte)(TempBuf.Count() + 3));			// Carattere di Start Delimiter 2 + LE Length Byte
            requestTelegram.insert(s.StationAddress);		// DA: Destination Address
            requestTelegram.insert(_DriverAddress);		// SA: Source Address

            if (FC == 0x0)
                FC = 0x6C;
            else
                FC ^= 0x20;
            requestTelegram.insert(FC);		// FC: Frame Control
            FC |= 0x10;


            requestTelegram.insert(TempBuf);		// Copy the PDU
            requestTelegram.End();
                   
	        //Send request to device
	        Retry = 0;
	        bool OK = false;
	        do{
                Thread.Sleep(MaxTslot);

                if (!DeviceWrite(requestTelegram.buffer, requestTelegram.pointer))
                {
                    ErrIDS = PPIErrorCodes.IDS_ERRDEVICEWRITE;
			        return false;
		        }
		        SetRTS(false);

		        //Wait for Acknowledge char
                OK = (DeviceWaitData(ReadBuf, 1));
                        
	        } while (!OK && ++Retry < 1);

	        if (!OK)
	        {
                ErrIDS = PPIErrorCodes.IDS_TIMEOUTRX;
                //lock (lockThreadObject)
                //{
                //    RemovePendingJob(j);
                //    j.IsPending = false;
                //}
                return false;
            }

	        uint Bytes;
	        switch (ReadBuf[0])
	        {
	        case PPIProtocol.SC:
		        break;
	        case PPIProtocol.SD1:
	        case PPIProtocol.SD2:
		        Bytes = GetBytesToRead();
		        if (Bytes > 0)
                    DeviceWaitData(ReadBuf, Bytes);
                ErrIDS = PPIErrorCodes.IDS_NAKRECEIVED;
                //lock (lockThreadObject)
                //{
                //    RemovePendingJob(j);
                //    j.IsPending = false;
                //}
                    return false;

	        default:
                ErrIDS = PPIErrorCodes.IDS_BADRXCHARS;
                //lock (lockThreadObject)
                //{
                //    RemovePendingJob(j);
                //    j.IsPending = false;
                //}
                return false;
	        }

	        //
	        //Ack received, now send the poll request
	        //
	        Retry = 0;
	        bool RespReceived = false;
	        do {
                DateTime LastTime = DateTime.UtcNow;
		        do {
			        //while (dwTidBase + (m_dwMinTid1*2) > ::GetTickCount())
			        Thread.Sleep(MinTid1);

                    requestTelegram.Init(PPIProtocol.SD1);  // Poll request
                    requestTelegram.insert(s.StationAddress);  // DA: Destination Address
                    requestTelegram.insert(_DriverAddress);  // SA: Source Address

			        if (Retry == 0)
                        FC ^= 0x20;
                    requestTelegram.insert(FC);  // FC: Frame Control
                    requestTelegram.End();

                    //Send request to device
                    if (!DeviceWrite(requestTelegram.buffer, requestTelegram.pointer))
                    {
                        ErrIDS = PPIErrorCodes.IDS_ERRDEVICEWRITE;
                        return false;
                    }
			        SetRTS(false);

			        //
			        //Now we wait for the response message (SD2)
			        //
                    if (!DeviceWaitData(ReadBuf, 1))
                        return false; 

			        switch (ReadBuf[0])
			        {
			        case PPIProtocol.SC:
				        //the device is still processing the response
				        break;
			        case PPIProtocol.SD1:
		                Bytes = GetBytesToRead();
		                if (Bytes > 0)
                            DeviceWaitData(ReadBuf, Bytes);
                        ErrIDS = PPIErrorCodes.IDS_NAKRECEIVED;
                        return false;
			        case PPIProtocol.SD2:
				        RespReceived = true;
				        break;
			        default:
                        ErrIDS = PPIErrorCodes.IDS_BADRXCHARS;
                        return false;
			        }
                    
		        } while (/*(DateTime.UtcNow - LastTime).TotalMilliseconds < ReadTimeout && */!RespReceived);

	        // Modified in version 10.0.0.18 (FOGBUGZ 7531)
	        //} while (!bRespReceived && ++nRetry < 3);
	        } while (!RespReceived && ++Retry < 1);

	        if (!RespReceived)
	        {
                ErrIDS = PPIErrorCodes.IDS_TIMEOUTRX;
                return false;
	        }

	        //
	        //Now we get and analyze the response
	        //
            ushort MinExpectedBytes = 0;
            ushort Qty = BytesToRequest;


	        //Get the response length (LE + LEr)
            if (!DeviceWaitData(ReadBuf, 2))
            {

                ErrIDS = PPIErrorCodes.IDS_TIMEOUTRX;
                return false;
	        }

	        if (ReadBuf[0] != ReadBuf[1])
	        {
                ErrIDS = PPIErrorCodes.IDS_BADRXCHARS;
                return false;
	        }

	        MinExpectedBytes = (ushort)(ReadBuf[0] + 3);
            byte[] tmpBuffer = new byte[MinExpectedBytes];
            if (!DeviceWaitData(tmpBuffer, MinExpectedBytes)) 
            {

                ErrIDS = PPIErrorCodes.IDS_BADRXCHARS;
                return false;
	        }

            Array.Copy(tmpBuffer,0,ReadBuf,2,MinExpectedBytes);

            ushort CheckSum = ComputeFCS(ReadBuf, 3, (ushort)(MinExpectedBytes - 3));
            if (CheckSum != ReadBuf[MinExpectedBytes])
	        {
                ErrIDS = PPIErrorCodes.IDS_BADRXCHARS;
                return false;
	        }

	        //check ERR_CLS and ERR_COD
	        if (ReadBuf[16] != 0 || ReadBuf[17] != 0)
	        {
                ErrIDS = PPIErrorCodes.IDS_NAKRECEIVED;
                return false;
	        }

	        //check access result (0xFF = OK)
	        byte NumOfPointers;
	        if (!j.IsRead  &&  j.Format == Step7Format.frmBit)
		        NumOfPointers =  (byte)BytesToRequest;
	        else
		        NumOfPointers = 1;

	        for (byte i=0; i<NumOfPointers; i++)
	        {
		        if (ReadBuf[20+i] != 0xFF)
		        {
                    switch(ReadBuf[20+i])
                    {
                        case 0x01:
                            ErrIDS = PPIErrorCodes.IDS_HWFAULT;
                            break;
                        case 0x03:
                            ErrIDS = PPIErrorCodes.IDS_ILLEGALOBJACCESS;
                            break;
                        case 0x05:
                            ErrIDS = PPIErrorCodes.IDS_INVALIDADDRESS;
                            break;
                        case 0x06:
                            ErrIDS = PPIErrorCodes.IDS_DATATYPENOTSUPP;
                            break;
                        case 0x0A:
                            ErrIDS = PPIErrorCodes.IDS_OBJNOTEXIST;
                            break;
                        default:
                            ErrIDS = PPIErrorCodes.IDS_STATUSNOTZERO;
                            break;
                    }

                    return false;
		        }
	        }

	        //check PDU reference
	        ushortUnion PDURef = new ushortUnion(ReadBuf, 10, true);
	        if (TelSeqCounter != PDURef.USHORT)
	        {
                ErrIDS = PPIErrorCodes.IDS_INVALIDSEQNUMBER;
                return false;
	        }

	        if (j.IsRead)
	        {
		        bool ByteData = false;
		        //Data type for PPI should never be 0x09, but in MPI it can, so we keep this test
		        //0x09 in MPI is for timers and counters
		        if (TempBuf[21] == 0x09)
		        {
                    Debug.Assert(false);
			        ByteData = true;
		        }
                
                ushortUnion NumOfRecBytes = new ushortUnion(ReadBuf, 22,true);
		        if( !ByteData ) 
		        {
			        //in case we received bit data (in PPI:ALWAYS)
			        //m_ReadBuf[22]&m_ReadBuf[23] contain the number of received BITS
			        NumOfRecBytes.USHORT = (ushort)((NumOfRecBytes.USHORT + 7)/8);
		        }

                ushort BytesToRequestTest = BytesToRequest; 
                switch (j.Area)
                {
                    case Step7Area.aCIEC:
			            Qty = (ushort)(BytesToRequest / 2);	//quantity of elements to be accessed
			            BytesToRequestTest = (ushort)(Qty * 3);	//each element is 3 bytes
			            break;
                    case Step7Area.aTIEC:
			            Qty = (ushort)(BytesToRequest / 2);	//quantity of returned objects
			            BytesToRequestTest = (ushort)(Qty * 5);	//each element is 5 bytes
			            break;
                    case Step7Area.aH:
			            Qty = (ushort)(BytesToRequest / 4);	//quantity of returned objects
			            BytesToRequestTest = (ushort)(Qty * 5);	//each element is 5 bytes
			            break;

                    default:
			            break;
                }


                if (NumOfRecBytes.USHORT == 0 || NumOfRecBytes.USHORT != BytesToRequestTest) 
		        {
                    ErrIDS = PPIErrorCodes.IDS_TOOFEWDATA;
                    return false;
		        }

                Answer = new byte[BytesToRequest];

		        if (j.Format != Step7Format.frmBit) 
		        {
			        int i;
			        switch (j.Area)
			        {
                        case Step7Area.aCIEC:
                            for (i = 0; i < Qty; i++)
                                Array.Copy(ReadBuf, 25 + 3 * i, Answer, 2 * i, 2);
                            break;
                        case Step7Area.aTIEC:
                            for (i = 0; i < Qty; i++)
                                Array.Copy(ReadBuf, 27 + 5 * i, Answer, 2 * i, 2);
                            break;
                        case Step7Area.aH:
				            for (i=0; i<Qty; i++)
                                Array.Copy(ReadBuf, 25 + 5 * i, Answer, 4 * i, 4);
				            break;
			        default:
				        //all other data areas receive raw bytes
                            Array.Copy(ReadBuf, 24, Answer, 0, BytesToRequest);
				        break;
			        }
		        }
		        else
                    Array.Copy(ReadBuf, 24, Answer, 0, BytesToRequest);
            }
            PPIJobTerminate = true;
            ErrIDS = (PPIErrorCodes)DriverErrorCodes.ErrorNoError;
            return true;
        }

        bool PreparePDU(PPIStation s, PPICommJob j, out byte[] outBuf, ref ushort BytesToRequest, uint elementsize)
        {
            uint byteOffset = 0;
            uint bitOffset = 0;

            outBuf = new byte[0];
            byte[] wBuffer = new byte[0];
            if (!j.IsRead)
            {
                wBuffer = PPIProtocol.GetWriteData(j, (int)elementsize, ref BytesToRequest, ref byteOffset);
                if ((wBuffer == null) || (wBuffer.Length == 0))
                    return false;
            }

            if (j.Format == Step7Format.frmBit)
            {
                
                uint tmtOffset = byteOffset + j.PartialElementStart;
                bitOffset = tmtOffset % 8;
                byteOffset = tmtOffset / 8;
            }
            else
            {
                byteOffset += j.PartialElementStart;
            }

            byte index = 0;
            byte[] TempBuf = new byte[PPIProtocol.MAX_WRITE];
            ushortUnion UshortAux = new ushortUnion(0);


            TempBuf[index++] = 0x32; // PROTO_ID
            TempBuf[index++] = 0x01; // ROSCTR Remote Operating Services Control
            //0x01 = Acknowledgement request
            //0x02 = Acknowledgement without parameter and data fields
            //0x03 = Acknowledgement with either or both parameter and data fields
            //0x07 = Acknowledgement for functions with extended header information in the parameter area

            TempBuf[index++] = 0x00; // RED_ID Redundancy Identification (HI)
            TempBuf[index++] = 0x00; // RED_ID Redundancy Identification (LO)

            TelSeqCounter = s.SeqNumber;
            UshortAux.USHORT = TelSeqCounter;
            TempBuf[index++] = UshortAux.HIBYTE; // PDU_REF (HI)
            TempBuf[index++] = UshortAux.LOBYTE; // PDU_REF (LO)

            ////the only case when we use more than one S7 Pointer is for writing bits
            byte NumOfPointers;
            if (!j.IsRead && j.Format == Step7Format.frmBit)
                
                NumOfPointers = (byte)wBuffer.Length;
            else
                NumOfPointers = 0x01;
            

            UshortAux.USHORT = (ushort)(2 + NumOfPointers * 12);

            TempBuf[index++] = UshortAux.HIBYTE; // PAR_LG (HI)
            TempBuf[index++] = UshortAux.LOBYTE; // PAR_LG (LO)

            //The DATA_LG will be adjusted later in case of WriteCmd
            byte DataLgOffs = index;
            TempBuf[index++] = 0; // DAT_LG (HI)
            TempBuf[index++] = 0; // DAT_LG (LO)

            TempBuf[index++] = (byte)(j.IsRead ? 0x04 : 0x05); // SERVICE_ID
            TempBuf[index++] = NumOfPointers; // Number of S7 pointers
            //
            //From here on we Build the S7 pointer to the PLC data
            //

            // Number of elements.
            UshortAux.USHORT = (ushort)(BytesToRequest / elementsize);

            byte n;
            for (n = 0; n < NumOfPointers; n++)
            {
                TempBuf[index++] = 0x12; // Variable Specification
                TempBuf[index++] = 0x0A; // V_ADDR_LG Variable Length

                //Syntax ID
                TempBuf[index++] = PPIProtocol.DLE; // constant

                // Type.
                switch (j.Area)
                {
                    case Step7Area.aCIEC:
                    case Step7Area.aTIEC:
                    case Step7Area.aH:
                        TempBuf[index++] = getAreaCode(j.Area); 
                        break;

                    default:
                        switch (j.Format)
                        {
                            case Step7Format.frmBit:
                                if (j.IsRead)
                                    TempBuf[index++] = 0x02;
                                else
                                {
                                    TempBuf[index++] = 0x01;
                                    UshortAux.USHORT = 1;
                                }
                                break;
                            case Step7Format.frmByte:
                                TempBuf[index++] = 0x02;
                                break;
                            case Step7Format.frmWord:
                                TempBuf[index++] = 0x04;
                                break;
                            case Step7Format.frmDWord:
                                TempBuf[index++] = 0x06;
                                break;
                        }
                        break;
                }
                TempBuf[index++] = UshortAux.HIBYTE;
                TempBuf[index++] = UshortAux.LOBYTE;

                if (j.Area != Step7Area.aC && j.Area != Step7Area.aT)
                {
                    // The following is for all data types except timers and counters
                    // DB number.
                    if (j.Area == Step7Area.aD)
                        UshortAux.USHORT = 0x01; //Subarea field
                    else
                        UshortAux.USHORT = 0x00; //Subarea field
                    
                    TempBuf[index++] = UshortAux.HIBYTE;
                    TempBuf[index++] = UshortAux.LOBYTE; 

                    // Area pointer
                    TempBuf[index++] = getAreaCode(j.Area);

                    int Elem = j.Offset;
                    byte BitNum = 0;
                    if (!j.IsRead)
                    {
                        Elem += (int)(j.Area == Step7Area.aCIEC || j.Area == Step7Area.aTIEC || j.Area == Step7Area.aH ? 
                                                byteOffset / 2 : byteOffset);
                        if (j.Format == Step7Format.frmBit)
                        {
                            int BitOffs = (int)(j.Bit + n + bitOffset);
                            Elem += (BitOffs / 8);
                            BitNum = (byte)(BitOffs % 8);
                        }
                    }

                    uintUnion uintAux = (j.Area == Step7Area.aCIEC || j.Area == Step7Area.aTIEC || j.Area == Step7Area.aH) ? 
                                        new uintUnion((uint)Elem ) :
                                        new uintUnion((uint)((Elem << 3) | BitNum)) ;

                    TempBuf[index++] = uintAux.HIUSHORT.LOBYTE;
                    TempBuf[index++] = uintAux.LOUSHORT.HIBYTE;
                    TempBuf[index++] = uintAux.LOUSHORT.LOBYTE; 

                }
                else
                {
                    //This is for timers and counters only
                    TempBuf[index++] = 0x00;
                    TempBuf[index++] = 0x00;

                    //Type is always WORD
                    TempBuf[index++] = 0x05;

                    TempBuf[index++] = PPIProtocol.DLE;

                    if (!j.IsRead)
                    {
                        if (j.Area == Step7Area.aT)
                            UshortAux.USHORT = (ushort)(j.Offset + byteOffset / (elementsize * 2));
                        else
                            UshortAux.USHORT = (ushort)(j.Offset + byteOffset / elementsize);
                    }
                    else
                    {
                        UshortAux.USHORT = (ushort)(j.Offset);
                    }

                    TempBuf[index++] = UshortAux.HIBYTE;
                    TempBuf[index++] = UshortAux.LOBYTE; 

                }
            }	//for (BYTE n=0; n<nNumOfPointers; n++)

            if (!j.IsRead)
            {
                //if (wBuffer.Count() != BytesToRequest * NumOfPointers)
                //{
                //    ErrIDS = PPIErrorCodes.IDS_INVALIDTAG;
                //    outBuf = new byte[0];
                //    return false;
                //}

                byte DataLength = index;
                for (n = 0; n < NumOfPointers; n++)
                {
                    //Note: for all data types except counter and timer, bit data type must be used
                    TempBuf[index++] = 0xFF; // Reserved.

                    if (j.Area != Step7Area.aC && j.Area != Step7Area.aT)
                    {
                        if (j.Format == Step7Format.frmBit)
                        {
                            TempBuf[index++] = 0x03; // Data type = ONE bit.
                            UshortAux.USHORT = 1;
                        }
                        else
                        {
                            TempBuf[index++] = 0x04; // Data type = byte, word, etc.
                            UshortAux.USHORT = (ushort)(wBuffer.Length * 8);
                        }
                    }
                    else
                    {
                        TempBuf[index++] = 0x09; // Data type = byte.
                        UshortAux.USHORT = (ushort)j.GetNumOfElements();
                    }
                    
                    TempBuf[index++] = UshortAux.HIBYTE; // Length in bits
                    TempBuf[index++] = UshortAux.LOBYTE; // Length in bits

                    if (j.Format == Step7Format.frmBit)
                    {
                        //if ((wBuffer[n / 8] & (1 << (n % 8))) == 0)
                        //    TempBuf[index++] = 0x00;
                        //else
                        //    TempBuf[index++] = 0x01;
                        TempBuf[index++] = wBuffer[n];

                    }
                    else
                    {
                        foreach (byte data in wBuffer)
                            TempBuf[index++] = data;
                    }
                    //Add fill byte for all values except the last one
                    if (j.Format == Step7Format.frmBit && n < NumOfPointers - 1)
                        TempBuf[index++] = 0; 
                }
                //Adjust the DATA_LG parameter
                UshortAux.USHORT = (ushort)(index - DataLength);
                TempBuf[DataLgOffs] = UshortAux.HIBYTE;
                TempBuf[DataLgOffs + 1] = UshortAux.LOBYTE;
            }

            outBuf = new byte[index];
            Array.Copy(TempBuf, outBuf, index);
            return true;
        }

        byte ComputeFCS(byte[] ReadBuffer,ushort offset, ushort inDataLength)
        {
            byte Sum = 0;
            ushort DataLength = (ushort)(inDataLength + offset);

            for (int i = offset; i < DataLength; i++)
                Sum += ReadBuffer[i];

	        return Sum;
        }

        byte getAreaCode(Step7Area area)
        {
            switch (area)
            {
                case Step7Area.aP:
                case Step7Area.aPE:
                case Step7Area.aPA:
                    return 0x80;
                case Step7Area.aI:
                    return 0x81;
                case Step7Area.aQ:
                    return 0x82;
                case Step7Area.aM:
                    return 0x83;
                case Step7Area.aD:
                    return 0x84;
                case Step7Area.aT:
                    return 0x85;
                case Step7Area.aC:
                    return 0x86;
                case Step7Area.aTIEC:
                    return 0x1F;
                case Step7Area.aCIEC:
                    return 0x1E;
                case Step7Area.aS:
                    return 0x05;
                case Step7Area.aAI:
                    return 0x06;
                case Step7Area.aAQ:
                    return 0x07;
                case Step7Area.aH:
                    return 0x20;
            }
            return 0xFF;

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
                    lock (lockThreadObject)
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
            //DateTime init = DateTime.UtcNow;
            //uint WaitingBytes = GetBytesToRead();
            //double dtime = (DateTime.UtcNow - init).TotalMilliseconds;

            bool terminate = false;
            while (/*dtime < Timeout / 2 && */!terminate /*&& !onStop*/)
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
                    //if (NewDataToAnlyze.WaitOne(10))
                    //    NewDataToAnlyze.Reset();
                    //dtime = (DateTime.UtcNow - init).TotalMilliseconds;
                    DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;
                    WaitNewDataEvent(ref conn);

                    if (conn != (int)DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError)
                    {
                        break;
                    }
                }
            }
            if (!terminate && !onStop)
                PurgeInputChars();

            WaitData = false;

            return terminate;
        }


        #endregion

        #region Override Methods

 
        public override bool ExecuteJob(ref DriverErrorCodes conn, CommJob job)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                // back compatibilty with old driver
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            }
            PPIJobTerminate = false; 
            PPIStation s = job.Station as PPIStation;
            if (s == null)
            {
                conn = DriverErrorCodes.ErrorTimeOut;
                return (false);
            }

            PPICommJob myJob = job as PPICommJob;
            if (myJob == null)
            {
                conn = DriverErrorCodes.ErrorTimeOut;
                return (false);
            }

            base.ExecuteJob(job);
            bool bRetLayer2 = Layer2(s, myJob);
            if (!bRetLayer2)
            {
                conn = DriverErrorCodes.ErrorTimeOut;
            }
            return false;

        }

        public override bool ProcessNewData(DriverErrorCodes conn, CommJob pendingjob)
        {
            ExecutedJobArgs eJob;
            PPICommJob myJob = pendingjob as PPICommJob;
            if (myJob == null)
                return true;
            if (conn != (int)DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError)
            {
                eJob = new ExecutedJobArgs { ErrorCode = conn, Job = pendingjob };
                OnJobExecuted(eJob);
                //LastErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
                lock (lockThreadObject)
                {
                    ReceiveBuffer.Clear();
                }
            }
            else
            {
                if (PPIJobTerminate)
                {
                    if (myJob.PartialElementStart == myJob.PartialElementEnd || pendingjob.IsRead)
                    {
                        eJob = new ExecutedJobArgs { ErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError, Job = pendingjob };
                        eJob.Values = Answer;
                        OnJobExecuted(eJob);
                    }
                    else
                    {
                        myJob.PartialElementStart = myJob.PartialElementEnd;
                        return false;
                    }
                }
                else
                {
                    eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)ErrIDS, Job = pendingjob };
                    OnJobExecuted(eJob);
                }

                myJob.PartialElementStart = 0;
                myJob.PartialElementEnd = 0;
            }


            return true;
        }

        #endregion

        #region Properties

        private byte _DriverAddress;
        public byte DriverAddress
        {
            get { return _DriverAddress; }
            set { _DriverAddress = value; }
        }

        private bool _UseSignal;
        public bool UseSignal
        {
            get { return _UseSignal; }
            set { _UseSignal = value; }
        }
        
      
        #endregion

        #region methods

        #endregion
 
        #region IDisposable

        public override void Dispose()
        {
            onStop = true;
            SpinWait.SpinUntil(() => { return !WaitData; });

            base.Dispose();
        }
        #endregion

    }
}
