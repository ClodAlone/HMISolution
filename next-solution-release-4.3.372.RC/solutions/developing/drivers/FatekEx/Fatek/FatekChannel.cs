using System;
using System.Collections.Generic;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using IpDriverCodeBaseEx;

namespace Fatek
{
    class FatekChannel : TcpChannel
    {
        #region Constructors

        /// <summary>
        /// Initializes the FatekChannel object.
        /// </summary>
        public FatekChannel(CommunicationDriver commdriver, FatekChannelSettings settings)
            : base(commdriver, settings)
        {
        }

        #endregion

        #region Override Methods
        /*
         * serial methods rule...
        public override void DeviceOpen()
        {
            base.DeviceOpen();
        }*/

        /*public override bool DeviceRead(byte[] Buffer, uint Count)
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

        public override bool ExecuteJob(ref DriverErrorCodes conn, CommJob job)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                // back compatibilty with old driver
                return false;
            }

            FatekCommJob mJob = job as FatekCommJob;
            if (mJob == null)
            {
                return false;
            }

                       
            base.ExecuteJob(job);

            List<byte> pdu = new List<byte>();
            uint Truedim = FatekProtocol.PrepareRequest(mJob, ref pdu);            
            if (Truedim == 0)
            {
                RemovePendingJob(job);                
                return false;
            }

            lock (lockThreadObject)
            {
                ReceiveBuffer.Clear();
            }
            
            // if write fail, try to handle "Unable to write data to the transport connection: An existing connection was forcibly closed by the remote host." error
            // try to redo DeviceOpen(), if write failed again, manage with TimeOut
            if (!DeviceWrite(pdu.ToArray(), (uint)pdu.Count))
            {
                bool writeOk = false;
                DeviceOpen();
                if (IsDeviceOpen())
                    writeOk = DeviceWrite(pdu.ToArray(), (uint)pdu.Count);
                
                if (!writeOk)
                {
                    // leave job into PendingJob to generatore a TimeOut (General Error)
                    //job.IsPending = false;
                    conn = DriverErrorCodes.ErrorTimeOut;
                    return false;
                }
            }
            
            if (!BeginDeviceRead(FatekProtocol.PROTOCOL_FRAME_MAX_SIZE))
            {
                //job.IsPending = false;
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            }
            job.LastExecutionTime = DateTime.UtcNow;
            return true;
        }

         public override bool ProcessNewData(DriverErrorCodes conn, CommJob pendingjob)
        {
            if (conn != (int)DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError)
            {
                bool CallDeviceClose = false;
#if NET_STANDARD
                if((pendingjob.Station.LastErrorCode != DriverErrorCodes.ErrorTimeOut)&&
                    (conn == DriverErrorCodes.ErrorTimeOut))
                {
                    CallDeviceClose = true;
                }
#endif
                ExecutedJobArgs timeoutJob = new ExecutedJobArgs { ErrorCode = conn, Job = pendingjob };
                OnJobExecuted(timeoutJob);
                lock (lockThreadObject)
                {
                    ReceiveBuffer.Clear();
                }
                if ((Flush() == false) ||
                    (CallDeviceClose))
                {
                  DeviceClose();
                }           

                // In case of perduring timeout error close the socket
                if ((_CloseSocketAfterErrorTimeout > 0) && (conn == DriverErrorCodes.ErrorTimeOut))
                {
                    if(_LastTimeoutError == DateTime.MinValue)
                    {
                        _LastTimeoutError = DateTime.UtcNow;
                    }
                    if((DateTime.UtcNow - _LastTimeoutError).TotalMilliseconds > _CloseSocketAfterErrorTimeout)
                    {
                        _LastTimeoutError = DateTime.MinValue;
                        DeviceClose();
                    }
                }
                else
                {
                    // No timeout error --> Reset the time of the last timeout error 
                    _LastTimeoutError = DateTime.MinValue;
                }

                LastErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;

                return true;
            }            
                        
            // No timeout error --> Reset the time of the last timeout error 
            _LastTimeoutError = DateTime.MinValue;

            DriverErrorCodes errorCode;
            byte[] dataBuffer;
            lock (lockThreadObject)
            {
                try
                {
                    errorCode = FatekProtocol.ParseAnswer((FatekCommJob)pendingjob, ReceiveBuffer, out dataBuffer);
                    //// reviced data incomplete --> wait more
                    //if (errorCode == (DriverCodeBase.Enumerators.DriverErrorCodes)FatekProtocol.FatekErrorCodes.ErrorReceiveFrameError)
                    //    return false;
                }
                catch (Exception ex)
                {
                    errorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)FatekProtocol.FatekErrorCodes.ErrorReceiveFrameError;
                    dataBuffer = null;
                }
                ReceiveBuffer.Clear();
            }
            
            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = errorCode, Job = pendingjob };
            if (errorCode == DriverErrorCodes.ErrorNoError)
            {
                LastErrorMessage = string.Empty;
                eJob.Values = dataBuffer;
            }
            else
            {
                Flush();
            }

            OnJobExecuted(eJob);
            return true;            
        }
        #endregion

        #region Properties

        // In case of perduring timeout error close the socket
        private uint _CloseSocketAfterErrorTimeout = Properties.Settings.Default.CloseSocketAfterErrorTimeout;
        private DateTime _LastTimeoutError = DateTime.MinValue;

        #endregion

        #region methods

        #endregion
    }
}
