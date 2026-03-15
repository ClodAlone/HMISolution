using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using IpDriverCodeBaseEx;
using System.Threading;

namespace DriverTcpExample
{
    class DriverTcpExampleChannel : TcpChannel
    {
        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public DriverTcpExampleChannel(CommunicationDriver commdriver, DriverTcpExampleChannelSettings settings)
            : base(commdriver, settings)
        {
            _TurnaroundDelay = settings.TurnaroundDelay;
            
        }

        #endregion

        UInt16 TransactionID = 0;

        protected ManualResetEvent BroadcastWait  = new ManualResetEvent(false);

        #region Override Methods

        public override bool DeviceClose()
        {
            return(base.DeviceClose());
        }

        public override bool ExecuteJob(ref DriverErrorCodes conn, CommJob job)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                // back compatibilty with old driver
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            }

            /*
             * so che tipo di frame devo preprare.
             * chiedo alla classe protocollo il messaggio da inviare
             * calcolo crc e spedisco
             */
            DriverTcpExampleCommJob mJob = job as DriverTcpExampleCommJob;
            if (mJob == null)
            {
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            }
            
            uint Truedim = 0;
            byte[] pdu = null;
            uint count = 0;
            bool isBroadcast = false;
            DriverTcpExampleProtocol P = new DriverTcpExampleProtocol();
            
            base.ExecuteJob(ref conn, job);
            uint dim = P.GetFrameLength(mJob);
            pdu = new byte[dim];
                
            try
            {
                Truedim = P.PrepareRequest(mJob, ref pdu, out isBroadcast);
            }
            catch
            {
                Truedim = 0;
            }
            
            if (Truedim == 0)
            {
                RemovePendingJob(job);                
                return false;
            }
            byte[] buf = new byte[Truedim + 6];
            lock (lockThreadObject)
            {
                ReceiveBuffer.Clear();
            }

            ++TransactionID;
            buf[count++] = Convert.ToByte(TransactionID >> 8);
            buf[count++] = Convert.ToByte(TransactionID & 0xff);
            buf[count++] = (byte)0;
            buf[count++] = (byte)0;
            buf[count++] = Convert.ToByte(Truedim >> 8);
            buf[count++] = Convert.ToByte(Truedim);
            Array.Copy(pdu, 0, buf, count, Truedim);
            count += Truedim;

            if (!DeviceWrite(buf, count))
            {
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            }

            if(!isBroadcast)
            {
                if (!BeginDeviceRead(259))
                {
                    conn = DriverErrorCodes.ErrorTimeOut;
                    return false;
                }
                job.LastExecutionTime = DateTime.UtcNow;
            }
            else
            {
                // No reply expected
                job.StartExecutionTime = DateTime.UtcNow;
                job.LastExecutionTime = DateTime.UtcNow;
                // Wait for the Turnaround delay
                int waitBroadCastTime = (int)TurnaroundDelay;
                BroadcastWait.WaitOne(waitBroadCastTime);
                lock (lockThreadObject)
                {
                    job.ClearTagListOnWriting();
                    RemovePendingJob(job);
                    // Empty the receive buffer (just in case the gateway has sent a reply)
                    ReceiveBuffer.Clear();
                    Flush();
                } 
                //LastErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.Job = job;
                eJob.ErrorCode = DriverErrorCodes.ErrorNoError;
                OnJobExecuted(eJob);
            }

            return true;
        }

        public override bool ResetConnection()
        {
            return (Properties.Settings.Default.MaxConsecutiveFrameError > 0) && (nErrorFrameCnt >= Properties.Settings.Default.MaxConsecutiveFrameError);
        }

        int nErrorFrameCnt = 0;
        public override bool ProcessNewData(DriverErrorCodes conn, CommJob pendingjob)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                ExecutedJobArgs erJob = new ExecutedJobArgs { ErrorCode = conn, Job = pendingjob };
                OnJobExecuted(erJob);
                //LastErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
                lock (lockThreadObject)
                {
                    ReceiveBuffer.Clear();
                }
                if (Flush() == false)
                {
                    DeviceClose();
                }
                return true;
            }
            /*
             * Minimum length = 6 (MODBUS TCP header) + 3 (Unit Identifier, Function Code, Exception Code or, at least, 1 data byte)
             */
            Int32 minLen = 9;
            byte[] msg;
            lock (lockThreadObject)
            {
                if (pendingjob == null)
                {
                    ReceiveBuffer.Clear();
                    Flush();
                    return false;
                }
                if (ReceiveBuffer.Count < minLen)
                {
                    Flush();
                    return false;
                }
                msg = new byte[ReceiveBuffer.Count];
                ReceiveBuffer.CopyTo(msg);
            }

                
            uint tid = Convert.ToUInt16(msg[0] * 256 + msg[1]);
            if(tid != TransactionID || msg[2] != 0 || msg[3] != 0)
            {
                ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = conn, Job = pendingjob };
                eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)ModbusErrorCodes.ErrorReceiveFrameError;
                    
                lock(lockThreadObject)
                    ReceiveBuffer.Clear();

                nErrorFrameCnt++;
                Flush();
                eAJob.Job = pendingjob;
                OnJobExecuted(eAJob);
                return true;
            }
            nErrorFrameCnt = 0;
            uint remaining = Convert.ToUInt16(msg[4] * 256 + msg[5]);
            if(remaining < 3)
            {
                lock(lockThreadObject)
                    ReceiveBuffer.Clear();

                Flush();
                return false;
            }
            if ((remaining + 6) > msg.Length)
            {
                Flush();
                return false;
            }
               

            byte replycode = 0;
            byte errorcode = 0;
            ExecutedJobArgs eJob = new ExecutedJobArgs();

            replycode = msg[7];
                

            if ((replycode & 0x80) > 0)
            {
                //error
                errorcode = msg[8];
                eJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)(DriverTcpExampleProtocol.PROTOCOL_ERROR + errorcode);
                    
                lock(lockThreadObject)
                    ReceiveBuffer.RemoveRange(0, 9);
                    
                Flush();
            }
            else
            {
                int count = 0;// Length from Unit ID to data
                int totcount = 0;// Message total length
                //waited chars...
                switch (replycode)
                { 
                    case 1://Read Coils
                    case 2://Read Discrete Input
                    case 3://Read Multiple Register
                    case 4://Read Input Register
                        count = msg[8] + 2 + 1;
                        totcount = count + 6;
                        break;
                    case 5://Write Single Coil
                    case 6://Write Single Register
                    case 15://Write Multiple Coils
                    case 16://Write Multiple Register
                        count = 5 + 1;
                        totcount = count + 6;
                        break;
                    case 20://Read File Record
                    case 21://Write File Record
                        count = msg[8] + 2 + 1;
                        totcount = count + 6;
                        break;
                    case 7://Read Excetion Status
                        count = 2 + 1;
                        totcount = count + 6;
                        break;
                    case 22://Mask Write Register
                        count = 7 + 1;
                        totcount = count + 6;
                        break;
                    default:
                        eJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)ModbusErrorCodes.ErrorUnknownFunctionCode;
                        ReceiveBuffer.Clear();
                        eJob.Job = pendingjob;
                        OnJobExecuted(eJob);
                        return true;
                }
                //frame completa se...
                if (msg.Length < totcount)
                {
                    eJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)ModbusErrorCodes.ErrorReadError;
                        
                    lock(lockThreadObject)
                        ReceiveBuffer.Clear();
                        
                    Flush();
                    eJob.Job = pendingjob;
                    OnJobExecuted(eJob);
                    return true;
                }
                LastErrorMessage = "";
                    
                //copio da Function code in avanti...
                byte[] Answer;
                Answer = new byte[count];
                Array.Copy(msg, 6, Answer, 0, count);
                
                eJob.Values = Answer;

                lock(lockThreadObject)
                    ReceiveBuffer.RemoveRange(0, totcount);
                    
                Flush();
            }
            eJob.Job = pendingjob;
            OnJobExecuted(eJob);
            return true;
        }

        #endregion

        #region Properties

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

        #region methods

        #endregion
    }
}
