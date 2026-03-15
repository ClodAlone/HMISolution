////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	GESRTP2Channel.cs
//
// summary:	Implements the driver GESRTP2 channel class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using IpDriverCodeBase;
using DriverCodeBase.Enumerators;
using System.Threading;
using Opc.Ua;

namespace GESRTP2
{
    /// <summary>   Communication channel of the GESRTP2 driver. </summary>
    public class GESRTP2Channel : TcpChannel
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the GESRTP2Channel object. </summary>
        ///
        /// <param name="commdriver">   . </param>
        /// <param name="settings">     . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public GESRTP2Channel(CommunicationDriver commdriver, GESRTP2ChannelSettings settings)
            : base(commdriver, settings, false)
        {
            IsConnect = false;
        }

        #endregion

        public enum ProcessNewDataStates : ushort
        {
            WaitPdu,
            WaitData,
            RceivedPduData,
        }

        bool IsConnect;
        public UInt16 mInvokeId;
        public byte mSeq;
        public ProcessNewDataStates ProcessNewDataState;
        EncapsPDU servicePdu = new EncapsPDU();
        public List<byte> GESRTP2ReceiveBuffer = new List<byte>();
        public int WaitByte;

        #region Override Methods

        public override bool DeviceWrite(byte[] Buffer, uint Count = 0)
        {
            if (Count == 0)
                return base.DeviceWrite(Buffer, (uint)Buffer.Length);
            else
                return base.DeviceWrite(Buffer, Count);

        }
        public override bool DeviceRead(byte[] Buffer, uint Count = 0)
        {
            if (Count == 0)
                return base.DeviceRead(Buffer, (uint)Buffer.Length);
            else
                return base.DeviceRead(Buffer, Count);
        }

        public override bool DeviceOpen()
        {
            base.DeviceOpen();
            if(IsDeviceOpen())
            {
                if (!IsConnect)
                {
                    if (Connect() != (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError)
                    {
                        DeviceClose();
                        return false;
                    }
                    if (Session() != (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError)
                    {
                        DeviceClose();
                        return false;
                    }
                }
                IsConnect = true;
                return true;
            }
            IsConnect = false;
            return false;
        }

        public override bool DeviceClose()
        {
            IsConnect = false;
            return base.DeviceClose();
        }

        /*
         * serial methods rule...


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
        /// Starts sending phase of GESRTP2CommJob. DeviceWrite starts sending the frame.
        /// </summary>
        ///
        /// <param name="job">  . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ExecuteJob(CommJob job)
        {
            GESRTP2CommJob mJob = job as GESRTP2CommJob;
            if (mJob == null)
                return;

            if (job.IsPending == true)
            {
                return;
            }

            byte[] pdu = null;
            lock (mJob.retLockList())
            {
                base.ExecuteJob(job);
                pdu = GESRTP2Protocol.PrepareRequest(this, mJob);
            }
            if (pdu.Length == 0)
            {
                lock (lockThreadObject)
                {
                    RemovePendingJob(job);
                    job.IsPending = false;
                }
                return;
            }

            if (!DeviceWrite(pdu))
            {
                return;
            }

            ProcessNewDataState = ProcessNewDataStates.WaitPdu;
            GESRTP2ReceiveBuffer.Clear();
            WaitByte = (int)(GESRTP2Protocol.Pdu_SIZE + (JobByteSize(mJob) > GESRTP2Protocol.ImediateBufferSize && !mJob.onWrite ? JobByteSize(mJob) : 0));
            if (!BeginDeviceRead(WaitByte)) 
            {
                return;
            }
            job.LastExecutionTime = DateTime.UtcNow;
        }
        public int JobByteSize(GESRTP2CommJob mJob)
        {
            return (int)(GESRTP2Protocol.DataType(mJob.AreaType) == UFUAModel.DataType.Boolean ? (((mJob.StartAddress - 1) % 8) + mJob.TotalJobSize + 7) / 8 : mJob.TotalJobSize);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Perform the step of receive of the task GESRTP2CommJob. Until have been received all
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
                mSeq++;
                mInvokeId++;
                OnJobExecuted(new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob });
                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                ReceiveBuffer.Clear();
                Flush();
                return true;
            }

            GESRTP2ReceiveBuffer.AddRange(ReceiveBuffer);
            WaitByte -= ReceiveBuffer.Count();
            ReceiveBuffer.Clear();

            lock (lockList)
            {
                if (pendingjob == null ||
                    pendingjob.Station as GESRTP2Station == null)
                {
                    Flush();
                    ReceiveBuffer.Clear();
                    return false;
                }

                GESRTP2CommJob mJob = pendingjob as GESRTP2CommJob;
                if (mJob == null)
                {
                    Flush();
                    ReceiveBuffer.Clear();
                    return false;
                }

                GESRTP2ErrorCodes Error = (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError;
                if (ProcessNewDataState == ProcessNewDataStates.WaitPdu)
                {
                    if (GESRTP2ReceiveBuffer.Count < GESRTP2Protocol.Pdu_SIZE)
                    {
                        BeginDeviceRead(WaitByte);
                        return false;
                    }

                    servicePdu.Expand(GESRTP2ReceiveBuffer.GetRange(0, GESRTP2Protocol.Pdu_SIZE).ToArray());
                    if (servicePdu.Mailbox.Typ == TrafficTypes.InvalidRequestNack)
                    {
                        mSeq++;
                        mInvokeId++;
                        OnJobExecuted(new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)GESRTP2ErrorCodes.ErrorReceiveNack, Job = pendingjob });
                        Flush();
                        return true;
                    }
                    GESRTP2ReceiveBuffer.RemoveRange(0, GESRTP2Protocol.Pdu_SIZE);
                    ProcessNewDataState = ProcessNewDataStates.WaitData;
                }
                if (ProcessNewDataState == ProcessNewDataStates.WaitData)
                {
                    if (JobByteSize(mJob) > GESRTP2Protocol.ImediateBufferSize && !mJob.onWrite && (servicePdu.Mailbox.Typ != TrafficTypes.CompletionAck && GESRTP2ReceiveBuffer.Count < JobByteSize(mJob)))
                    {
                        BeginDeviceRead(WaitByte);
                        return false;
                    }
                    else
                    {
                        ProcessNewDataState = ProcessNewDataStates.RceivedPduData;
                    }
                }

                if (ProcessNewDataState == ProcessNewDataStates.RceivedPduData)
                {
                    byte[] buffer;
                    if (mJob.onWrite)
                    {
                        buffer = new byte[GESRTP2Protocol.Pdu_SIZE];

                        if (servicePdu.Mailbox.Typ != TrafficTypes.CompletionAck)
                        {
                            Error = GESRTP2ErrorCodes.ErrorUnexpectedWriteReply;
                        }
                        else
                        {
                            Error = ReadPduOk(servicePdu);
                        }
                    }
                    else
                    {
                        mJob.readData.Clear();

                        // check pdu type and invokeId 
                        Error = ReadPduOk(servicePdu);
                        if (Error == (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError)
                        {
                            Error = ReadResponse(mJob);
                        }
                    }
                    mSeq++;
                    mInvokeId++;

                    OnJobExecuted(new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)Error, Job = pendingjob });

                    if (Error != (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError &&
                        Error != GESRTP2ErrorCodes.ErrorReceiveNack)
                    {
                        Flush();
                        DeviceClose();
                        ReceiveBuffer.Clear();
                    }

                    return true;
                }
            }
            return false;
        }

        public GESRTP2ErrorCodes ReadResponse(GESRTP2CommJob job)
        {
            byte[] buffer;
            if (servicePdu.Mailbox.Seq != mSeq)
            {
                return GESRTP2ErrorCodes.ErrorUnexpectedMailSeq;
            }
            switch (servicePdu.Mailbox.Typ)
            {
                case TrafficTypes.CompletionAck:
                    if (JobByteSize(job) > GESRTP2Protocol.ImediateBufferSize)
                    {
                        return GESRTP2ErrorCodes.ErrorReceiveFewData;
                    }
                    buffer = new byte[JobByteSize(job)];
                    Array.Copy(servicePdu.Mailbox.Data, buffer, JobByteSize(job));
                    job.readData.AddRange(buffer);
                    break;
                case TrafficTypes.CompletionAckTB:
                    if (JobByteSize(job) > GESRTP2ReceiveBuffer.Count())
                    {
                        GESRTP2ReceiveBuffer.Clear();
                        return GESRTP2ErrorCodes.ErrorReceiveFewData;
                    }
                    job.readData.AddRange(GESRTP2ReceiveBuffer.GetRange(0, JobByteSize(job)));
                    GESRTP2ReceiveBuffer.Clear();
                    break;
                case TrafficTypes.InvalidRequestNack:
                    return GESRTP2ErrorCodes.ErrorReceiveNack;
                default:
                    return GESRTP2ErrorCodes.ErrorUnexpectedReadReply;
            }

            return (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError;
        }

        #endregion

        #region Properties

        
        #endregion

        #region methods
        private byte[] GESRTP2Read()
        {
            byte[] buffer = new byte[GESRTP2Protocol.Pdu_SIZE];
            if (DeviceRead(buffer))
            {
                EncapsPDU Pdu = new EncapsPDU();
                Pdu.Expand(buffer);
                if (Pdu.DataLength > 0)
                {
                    byte[] bufferData = new byte[Pdu.DataLength];
                    if(DeviceRead(bufferData))
                    {
                        List<byte> retVal = new List<byte>(buffer);
                        retVal.AddRange(bufferData);
                        return retVal.ToArray();
                    }
                    else
                        return new byte[0];
                }
                return buffer;
            }
            else
                return new byte[0];
        }

        /// <summary>   Connect . </summary>
        private GESRTP2ErrorCodes Connect()
        {
            GESRTP2ErrorCodes Error = (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError;
            // prepare ConnectRequest message
            byte[] buffer = EncapsPDUFactory.Connect().Pack();
            if (DeviceWrite(buffer))
            {
                buffer = GESRTP2Read();
                if (buffer.Length == GESRTP2Protocol.Pdu_SIZE)
                {
                    EncapsPDU Pdu = new EncapsPDU();
                    Pdu.Expand(buffer);
                    // check pdu type and invokeId 
                    if ((Pdu.PduType != PduTypes.ConnectResponse) ||
                        (Pdu.InvokeId != 0) || (Pdu.DataLength != 0))
                        Error = GESRTP2ErrorCodes.ErrorUnexpectedConnectionReply;
                    if ((Pdu.Capability & (uint)Capabiities.SRTP_SRP_SERVER) == 0)
                        Error = GESRTP2ErrorCodes.ErrorUnexpectedCapabilities;
                }
                else
                    Error = GESRTP2ErrorCodes.ErrorRxRead;
            }
            else
                Error = GESRTP2ErrorCodes.ErrorTxWrite;
            ReceiveBuffer.Clear();
            if (Error != (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError)
                DeviceClose();
            return Error;
        }
        private GESRTP2ErrorCodes Session()
        {
            GESRTP2ErrorCodes Error = (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError;
            mSeq = 1;
            mInvokeId = 1;
            byte[] buffer = EncapsPDUFactory.Session(SessionEnables.ESTABLISH, mSeq, mInvokeId).Pack();
            if (DeviceWrite(buffer))
            {
                buffer = GESRTP2Read();
                if (buffer.Length == GESRTP2Protocol.Pdu_SIZE)
                {
                    EncapsPDU Pdu = new EncapsPDU();
                    Pdu.Expand(buffer);
                    // check pdu type and invokeId 
                    if ((Pdu.DataLength == 0) &&
                        (Pdu.Mailbox.Typ == TrafficTypes.CompletionAck))
                    {
                        Error = (ReadPduOk(Pdu));
                    }
                    else
                        Error = GESRTP2ErrorCodes.ErrorUnexpectedSessionReply;

                }
                else
                    Error = GESRTP2ErrorCodes.ErrorRxRead;
            }
            else
                Error = GESRTP2ErrorCodes.ErrorTxWrite;

            ReceiveBuffer.Clear();
            if (Error == (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError)
                mSeq++;
            else
                DeviceClose();
            return Error;
        }
        // check pdu type and invokeId 
        public GESRTP2ErrorCodes ReadPduOk(EncapsPDU Pdu)
        {
            if (Pdu.PduType != PduTypes.DataResponse)
            {
                return GESRTP2ErrorCodes.ErrorUnexpectedPduType;
            }
            if (Pdu.InvokeId != mInvokeId)
            {
                return GESRTP2ErrorCodes.ErrorUnexpectedInvokeId;
            }
            return (GESRTP2ErrorCodes)DriverErrorCodes.ErrorNoError;
        }

        #endregion
    }
}
