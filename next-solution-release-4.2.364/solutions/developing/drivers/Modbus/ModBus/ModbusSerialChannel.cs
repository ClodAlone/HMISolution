using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using SerialDriverCodeBase;
using System.Threading;

namespace ModBus
{
    public class ModbusSerialChannel : SerialChannel
    {
        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public ModbusSerialChannel(CommunicationDriver commdriver, ModbusChannelSettings settings)
            : base(commdriver, settings)
        {
            _FrameType = settings.FrameType;
            _TurnaroundDelay = settings.TurnaroundDelay;
        }

        #endregion

        #region Override Methods
        /*
         * serial methods rule...
        public override void DeviceOpen()
        {
            base.DeviceOpen();
        }

        public override void DeviceClose()
        {
            base.DeviceClose();
        }

        public override bool DeviceRead(byte[] Buffer, uint Count)
        {
            throw new NotImplementedException();
        }

        public override bool DeviceWrite(byte[] Buffer, uint Count)
        {
            throw new NotImplementedException();
        }

        public override uint GetBytesToRead()
        {
            throw new NotImplementedException();
        }

        public override uint GetBytesToWrite()
        {
            throw new NotImplementedException();
        }
        */
        public override void ExecuteJob(CommJob job)
        {
            
            /*
             * so che tipo di frame devo preprare.
             * chiedo alla classe protocollo il messaggio da inviare
             * calcolo crc e spedisco
             */
            ModbusCommJob mJob = job as ModbusCommJob;
            if (mJob == null)
                return;

            var lkJob = mJob.retLockList();

            //base.ExecuteJob(job);

            uint dim = 0;
            uint count = 0;
            ModbusProtocol P = new ModbusProtocol();
            byte[] buf;
            if (job.IsPending == true)
            {
                return;
            }
            lock (lkJob)
            {
                base.ExecuteJob(job);
                dim = P.GetFrameLength(mJob);
                buf = new byte[dim + 2];//CRC1,CRC2
                try
                {
                    count = P.PrepareRequest(mJob, ref buf);
                }
                catch
                {
                    count = 0;
                }
                if (count == 0)
                {
                    lock (lockThreadObject)
                    {
                        RemovePendingJob(job);
                        job.IsPending = false;
                    }
                    return;
                }
            }

            ReceiveBuffer.Clear();

            if (FrameType == 0)
            {
                //RTU
                UInt16 crc = ComputeCRC16(buf, count);
                buf[count++] = (byte)crc;
                buf[count++] = (byte)(crc >> 8);

                DeviceWrite(buf, count);
            }
            else
            {
                //ASCII
                uint loop = count;
                count = 0;
                byte[] temp = new byte[((dim) * 2) + 5];
                byte lrc = ComputeLRC(buf, loop);
                temp[count++] = (byte)':';
                for (int i = 0; i < (loop); i++)
                {
                    temp[count++] = (byte)buf[i].ToString("X2")[0];
                    temp[count++] = (byte)buf[i].ToString("X2")[1];
                }
                temp[count++] = (byte)lrc.ToString("X2")[0];
                temp[count++] = (byte)lrc.ToString("X2")[1];
                temp[count++] = (byte)0x0D;
                temp[count++] = (byte)0x0A;
                
                DeviceWrite(temp, count);
            }
            job.LastExecutionTime = DateTime.UtcNow;
            //System.Diagnostics.Trace.TraceInformation(string.Format("{0} ExecuteJob", DateTime.Now.ToLongTimeString()));
        }

        public override bool ProcessNewData(CommJob pendingjob)
        {
            //System.Diagnostics.Trace.TraceInformation(string.Format("ProcessNewData Enter:{0}", DateTime.Now.ToLongTimeString()));
                
            if (LastErrorCode != (int)DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError)
            {

                ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                OnJobExecuted(eJob);
                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;


                return true;
            }
            /*
             * put in the returned data values the received buffer, 
             * from ID to the end of data, crc's have been removed
             */
            int minLen = (FrameType == 0 ? 5 : 11);
            lock (lockList)
            {
                if (pendingjob == null)
                {
                    ReceiveBuffer.Clear();
                    return false;
                }
                if (ReceiveBuffer.Count < minLen)
                    return false;
                byte replycode = 0;
                byte errorcode = 0;
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                if (FrameType == 0)
                {
                    //RTU
                    replycode = ReceiveBuffer[1];
                }
                else 
                { 
                    //ASCII
                    string s = new string((char)ReceiveBuffer[3],1);
                    s += (char)ReceiveBuffer[4];
                    try
                    {
                        replycode = Convert.ToByte(s, 16);
                    }
                    catch(Exception ex)
                    {
                        eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)ModbusErrorCodes.ErrorReplayCodeMalformed;
                        ReceiveBuffer.Clear();

                        eJob.Job = pendingjob;
                        OnJobExecuted(eJob);
                        return true;
                    }
                }
                if ((replycode & 0x80) > 0)
                {
                    //error
                    if (FrameType == 0)
                        errorcode = ReceiveBuffer[2];
                    else
                    {
                        string s = new string((char)ReceiveBuffer[5], 1);
                        s += (char)ReceiveBuffer[6];
                        try
                        {
                            errorcode = Convert.ToByte(s, 16);
                        }
                        catch(Exception ex)
                        {
                            eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)ModbusErrorCodes.ErrorErrorCodeMalformed;
                            ReceiveBuffer.Clear();

                            eJob.Job = pendingjob;
                            OnJobExecuted(eJob);
                            return true;
                        }
                    }
                    eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)(ModbusProtocol.PROTOCOL_ERROR + errorcode);
                    ReceiveBuffer.RemoveRange(0, minLen);
                }
                else
                {
                    int count = 0;//from ID a data
                    int totcount = 0;//from ID to end (crc/lrc included)
                    //waited chars...
                    switch (replycode)
                    { 
                        case 1://Read Coils
                        case 2://Read Discrete Input
                        case 3://Read Multiple Register
                        case 4://Read Input Register
                            if (FrameType == 0)
                            {
                                count = ReceiveBuffer[2] + 2 + 1;
                                totcount = count + 2;//crc
                            }
                            else
                            {
                                string s = new string((char)ReceiveBuffer[5], 1);
                                s += (char)ReceiveBuffer[6];
                                try
                                {
                                    count = Convert.ToByte(s, 16) + 2 + 1;
                                }
                                catch(Exception ex)
                                {
                                    eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)ModbusErrorCodes.ErrorReplayFrameMalformed;
                                    ReceiveBuffer.Clear();

                                    eJob.Job = pendingjob;
                                    OnJobExecuted(eJob);
                                    return true;
                                }
                                count *= 2;
                                totcount = count + 4;//count,lrc,lrc,0x0d, 0x0a
                            }
                            break;
                        case 5://Write Single Coil
                        case 6://Write Single Register
                        case 15://Write Multiple Coils
                        case 16://Write Multiple Register
                            if (FrameType == 0)
                            {
                                count = 5 + 1;
                                totcount = count + 2;//crc
                            }
                            else
                            {
                                count = 5 + 1;
                                count *= 2;
                                totcount = count + 4;//count,lrc,lrc,0x0d, 0x0a
                            }
                            break;
                        case 20://Read File Record
                        case 21://Write File Record
                            if (FrameType == 0)
                            {
                                count = ReceiveBuffer[2] + 2 + 1;
                                totcount = count + 2;//crc
                            }
                            else
                            {
                                string s = new string((char)ReceiveBuffer[5], 1);
                                s += (char)ReceiveBuffer[6];
                                count = Convert.ToByte(s, 16) + 2 + 1;
                                count *= 2;
                                totcount = count + 4;//count,lrc,lrc,0x0d, 0x0a
                            }
                            break;
                        case 7://Read Excetion Status
                            if (FrameType == 0)
                            {
                                count = 2 + 1;
                                totcount = count + 2;//crc
                            }
                            else
                            {
                                count = 2 + 1;
                                count *= 2;
                                totcount = count + 4;//count,lrc,lrc,0x0d, 0x0a
                            }
                            break;
                        default:
                            eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)ModbusErrorCodes.ErrorUnknownFunctionCode;
                            ReceiveBuffer.Clear();

                            eJob.Job = pendingjob;
                            OnJobExecuted(eJob);
                            return true;
                    }
                    //frame completa se...
                    if (ReceiveBuffer.Count < (FrameType == 0 ? totcount : totcount + 1))
                        return false;
                    
                    LastErrorMessage = "";
                    
                    //copio da Function code in avanti...
                    byte[] Answer;
                    UInt16 receivedCrc;
                    byte receivedLrc;
                    if (FrameType == 0)
                    {
                        Answer = new byte[count];
                        ReceiveBuffer.CopyTo(0, Answer, 0, count);
                        UInt16 crc = ComputeCRC16(Answer, (uint)count);
                        receivedCrc = (UInt16)(ReceiveBuffer[totcount - 1] << 8);
                        receivedCrc += ReceiveBuffer[totcount - 2];
                        if (crc != receivedCrc)
                        {
                            //error
                            eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)ModbusErrorCodes.ErrorCRCError;
                        }
                        else
                            eJob.Values = Answer;

                        ReceiveBuffer.RemoveRange(0, totcount);
                    }
                    else
                    {
                        Answer = new byte[count];
                        ReceiveBuffer.CopyTo(1, Answer, 0, count);
                        string a = Encoding.UTF8.GetString(Answer);
                        try
                        {
                            for (int i = 0; i < count / 2; i++)
                            {
                                Answer[i] = Convert.ToByte(a.Substring(2 * i, 2), 16);
                            }
                        }
                        catch(Exception ex)
                        {
                            eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)ModbusErrorCodes.ErrorReceivedDataMalformed;
                            ReceiveBuffer.Clear();

                            eJob.Job = pendingjob;
                            OnJobExecuted(eJob);
                            return true;
                        }
                        byte lrc = ComputeLRC(Answer, (uint)count/2);
                        string s = new string((char)ReceiveBuffer[count+1], 1);
                        s += (char)ReceiveBuffer[count + 2];
                        try
                        {
                            receivedLrc = Convert.ToByte(s, 16);
                        }
                        catch(Exception ex)
                        {
                            eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)ModbusErrorCodes.ErrorLRCMalformed;
                            ReceiveBuffer.Clear();

                            eJob.Job = pendingjob;
                            OnJobExecuted(eJob);
                            return true;
                        }
                        if (lrc != receivedLrc)
                        {
                            //error
                            eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)ModbusErrorCodes.ErrorCRCError;
                        }
                        else
                            eJob.Values = Answer;

                        ReceiveBuffer.RemoveRange(0, totcount+1);
                    }
                }
                //System.Diagnostics.Trace.TraceInformation(string.Format("{1} ProcessNewData Error:{0}", eJob.ErrorCode, DateTime.Now.ToLongTimeString()));
                eJob.Job = pendingjob;
                OnJobExecuted(eJob);
                return true;
            }
        }
        #endregion

        #region Properties

        private byte _FrameType;
        public byte FrameType
        {
            get { return _FrameType; }
            set
            {
                _FrameType = value;
            }
        }

        private uint _TurnaroundDelay;
        public uint TurnaroundDelay
        {
            get { return _TurnaroundDelay; }
            set
            {
                _TurnaroundDelay = value;
            }
        }
        
        
        #endregion

        static UInt16[] CRC16Table = {0, 49345, 49537, 320, 49921, 960, 640, 49729, 50689, 1728,
                                         1920, 51009, 1280, 50625, 50305, 1088, 52225, 3264, 3456,
                                         52545, 3840, 53185, 52865, 3648, 2560, 51905, 52097, 2880,
                                         51457, 2496, 2176, 51265, 55297, 6336, 6528, 55617, 6912,
                                         56257, 55937, 6720, 7680, 57025, 57217, 8000, 56577, 7616,
                                         7296, 56385, 5120, 54465, 54657, 5440, 55041, 6080, 5760,
                                         54849, 53761, 4800, 4992, 54081, 4352, 53697, 53377, 4160,
                                         61441, 12480, 12672, 61761, 13056, 62401, 62081, 12864, 
                                         13824, 63169, 63361, 14144, 62721, 13760, 13440, 62529,
                                         15360, 64705, 64897, 15680, 65281, 16320, 16000, 65089,
                                         64001, 15040, 15232, 64321, 14592, 63937, 63617, 14400,
                                         10240, 59585, 59777, 10560, 60161, 11200, 10880, 59969,
                                         60929, 11968, 12160, 61249, 11520, 60865, 60545, 11328,
                                         58369, 9408, 9600, 58689, 9984, 59329, 59009, 9792, 8704,
                                         58049, 58241, 9024, 57601, 8640, 8320, 57409, 40961, 24768,
                                         24960, 41281, 25344, 41921, 41601, 25152, 26112, 42689, 
                                         42881, 26432, 42241, 26048, 25728, 42049, 27648, 44225,
                                         44417, 27968, 44801, 28608, 28288, 44609, 43521, 27328,
                                         27520, 43841, 26880, 43457, 43137, 26688, 30720, 47297,
                                         47489, 31040, 47873, 31680, 31360, 47681, 48641, 32448,
                                         32640, 48961, 32000, 48577, 48257, 31808, 46081, 29888,
                                         30080, 46401, 30464, 47041, 46721, 30272, 29184, 45761,
                                         45953, 29504, 45313, 29120, 28800, 45121, 20480, 37057,
                                         37249, 20800, 37633, 21440, 21120, 37441, 38401, 22208,
                                         22400, 38721, 21760, 38337, 38017, 21568, 39937, 23744,
                                         23936, 40257, 24320, 40897, 40577, 24128, 23040, 39617,
                                         39809, 23360, 39169, 22976, 22656, 38977, 34817, 18624,
                                         18816, 35137, 19200, 35777, 35457, 19008, 19968, 36545,
                                         36737, 20288, 36097, 19904, 19584, 35905, 17408, 33985,
                                         34177, 17728, 34561, 18368, 18048, 34369, 33281, 17088,
                                         17280, 33601, 16640, 33217, 32897, 16448};


        #region methods
        public void InitCRC16Table()
        {
            UInt16 i, j, k, accum;
            for (i = 0; i < 256; ++i)
            {
                accum = 0;
                k = i;
                for (j = 8; j > 0; j--)
                {
                    if (((k ^ accum) & 0x0001) > 0)
                    {
                        accum = (UInt16)(accum >> 1);
                        accum = (UInt16)(accum ^ 0xA001);
                    }
                    else
                        accum >>= 1;

                    k >>= 1;
                }

                CRC16Table[i] = accum;
            }
        }

        public static UInt16 ComputeCRC16(byte[] buf, uint Len)
        {
            UInt16 ival, temp;
            ival = 0xFFFF;
            int i = 0;
            while ((Len--) > 0)
            {
                byte b1 = (byte)(ival >> 8);
                temp = (UInt16)(b1 & 0xFF);
                ival = (UInt16)(temp ^ CRC16Table[((byte)ival ^ buf[i++]) & 0xFF]);
            }

            return ival;
        }
        public static byte ComputeLRC(byte[] buf, uint Len)
        {
            byte bTempValue = 0;
            int i = 0;
            while (Len-- > 0)
            {
                bTempValue += buf[i++];
            }

            bTempValue = (byte)(0xFF - bTempValue);
            bTempValue += 0x01;

            return bTempValue;
        }
        #endregion
    }
}
