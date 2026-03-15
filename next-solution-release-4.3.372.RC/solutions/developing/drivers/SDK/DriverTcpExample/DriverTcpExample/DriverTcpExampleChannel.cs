////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	DriverTcpExampleChannel.cs
//
// summary:	Implements the driver TCP example channel class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using DriverCodeBase;
using IpDriverCodeBase;

namespace DriverTcpExample
{
    /// <summary>   Communication channel of the DriverTcpExample driver. </summary>
    class DriverTcpExampleChannel : TcpChannel
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the DriverTcpExampleChannel object. </summary>
        ///
        /// <param name="commdriver">   . </param>
        /// <param name="settings">     . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DriverTcpExampleChannel(CommunicationDriver commdriver, DriverTcpExampleChannelSettings settings)
            : base(commdriver, settings, false)
        {
            _TurnaroundDelay = settings.TurnaroundDelay;
        }

        #endregion

        /// <summary>   Protocol transaction IDentifier. </summary>
        UInt16 TransactionID = 0;
        

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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Starts sending phase of DriverTcpExampleCommJob. DeviceWrite starts sending the frame.
        /// </summary>
        ///
        /// <param name="job">  . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ExecuteJob(CommJob job)
        {
            DriverTcpExampleCommJob mJob = job as DriverTcpExampleCommJob;
            if (mJob == null)
                return;
            
            var lkJob = mJob.retLockList();
            
            uint Truedim = 0;
            byte[] pdu = null;
            uint count = 0;
            DriverTcpExampleProtocol P = new DriverTcpExampleProtocol();
            lock (lkJob)
            {
                base.ExecuteJob(job);
            
                uint dim = P.GetFrameLength(mJob);
                pdu = new byte[dim];
                
                Truedim = P.PrepareRequest(mJob, ref pdu);
            }
            if (Truedim == 0)
            {
                return;
            }
            byte[] buf = new byte[Truedim + 6];
            ReceiveBuffer.Clear();
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
                return;
            }

            if (!BeginDeviceRead((int)job.TotalJobSize + DriverTcpExampleProtocol.INT_AnswerLen)) 
            {
                return;
            }

            job.LastExecutionTime = DateTime.UtcNow;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Perform the step of receive of the task DriverTcpExampleCommJob. Until have been received all
        /// the data continue the reception exiting with false. When all data have been received execute
        /// OnJobExecuted(eJob) and going out with true.
        /// </summary>
        ///
        /// <param name="pendingjob">   . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool ProcessNewData(CommJob pendingjob)
        {
            if (LastErrorCode != (int)DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError)
            {
                
                ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                OnJobExecuted(eJob);
                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                ReceiveBuffer.Clear();
                Flush();
                return true;
            }
            /*
             * Put in the returned data values the received buffer, 
             * from ID to the end of data, crc's have been removed.
             */
            Int32 minLen = 6;
            lock (lockList)
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

                uint tid = Convert.ToUInt16(ReceiveBuffer[0] * 256 + ReceiveBuffer[1]);
                if(tid != TransactionID || ReceiveBuffer[2] != 0 || ReceiveBuffer[3] != 0)
                {
                    ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                    eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)DriverTcpExampleErrorCodes.ErrorReceiveFrameError;
                    ReceiveBuffer.Clear();
                    Flush();
                    eAJob.Job = pendingjob;
                    OnJobExecuted(eAJob);
                    return true;
                }

                uint remaining = Convert.ToUInt16(ReceiveBuffer[4] * 256 + ReceiveBuffer[5]);
                byte[] msg;
                msg = new byte[remaining];

                byte replycode = 0;
                byte errorcode = 0;
                ExecutedJobArgs eJob = new ExecutedJobArgs();

                replycode = ReceiveBuffer[7];
                

                if ((replycode & 0x80) > 0)
                {
                    //error
                    errorcode = ReceiveBuffer[8];
                    eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)(DriverTcpExampleProtocol.PROTOCOL_ERROR + errorcode);
                    ReceiveBuffer.RemoveRange(0, 9);
                    Flush();
                }
                else
                {
                    int count = 0;//from ID a data
                    int totcount = 0;//from ID to end (crc/lrc included)
                    //waited chars...
                    switch (replycode)
                    { 
                        case 1://Read Coils
                        case 3://Read Multiple Register
                            count = ReceiveBuffer[8] + 2 + 1;
                            totcount = count + 6;
                            break;
                        case 15://Write Multiple Coils
                        case 16://Write Multiple Register
                            count = 5 + 1;
                            totcount = count + 6;
                            break;
                        default:
                            eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)DriverTcpExampleErrorCodes.ErrorUnknownFunctionCode;
                            ReceiveBuffer.Clear();
                           
                            eJob.Job = pendingjob;
                            OnJobExecuted(eJob);
                            return true;
                    }
                    //frame completa se...
                    if (ReceiveBuffer.Count < totcount)
                    {
                       eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)DriverTcpExampleErrorCodes.ErrorReadError;
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
                    ReceiveBuffer.CopyTo(6, Answer, 0, count);
                    eJob.Values = Answer;

                    ReceiveBuffer.RemoveRange(0, totcount);
                    Flush();
                }
                
                eJob.Job = pendingjob;
                OnJobExecuted(eJob);
                return true;
            }
        }
        #endregion

        #region Properties

        /// <summary>   The turnaround delay. </summary>
        private uint _TurnaroundDelay;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Communication protocol turnaround delay (not used). </summary>
        ///
        /// <value> The turnaround delay. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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
