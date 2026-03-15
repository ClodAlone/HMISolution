using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using IpDriverCodeBaseEx;
using System.Threading;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;

namespace OmronFinsEthernet
{
    public class OmronFinsEthernetChannel : UdpChannel
    {
        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public OmronFinsEthernetChannel(CommunicationDriver commdriver, OmronFinsEthernetChannelSettings settings)
            : base(commdriver, settings, false)
        {
            _SourceNetworkAddress = settings.SourceNetworkAddress;
            _SourceNode = settings.SourceNode;
            _SourceUnit = settings.SourceUnit;
        }

        #endregion

        #region Data Members        
        private bool sIDError = false;
        #endregion

        #region Override Methods

        public override bool IsDeviceOpen()
        {
            bool returnValue = base.IsDeviceOpen();
            SetStateCommandVariableBit(((!returnValue) || InErrorState()), (UInt16)DriverCodeBaseEx.Enumerators.ChannelVariableBits.ChannelUnconnected);
            return (returnValue);
        }

        public override bool ExecuteJob(ref DriverErrorCodes conn, CommJob job)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                // back compatibilty with old driver
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            } 

            OmronFinsEthernetCommJob mJob = job as OmronFinsEthernetCommJob;
            if (mJob == null)
            {
                // back compatibilty with old driver
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            }

            // on sid error don't send new request but try wait appropiate response from device
            if (sIDError)
            {
                BeginDeviceRead(0);
                // exit and start up "listner"
                return true;
            }

            base.ExecuteJob(ref conn, job);

            // Return constan BUFFER_SIZE 
            uint count = OmronFinsEthernetProtocol.GetFrameLength(mJob);
            byte[] pdu = new byte[count];
            // Returns the number of bytes of the job (frame)
            count = OmronFinsEthernetProtocol.PrepareRequest(mJob, ref pdu, this);
            if(count == 0)
            {
                RemovePendingJob(job);                
                conn = DriverErrorCodes.ErrorFrameError;
                return false;
            }
            lock (lockThreadObject)
                ReceiveBuffer.Clear();
            Flush();
            
            if (!DeviceWrite(pdu, count))
            {
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            }

            //Function parameter to zero for UDP protocols, it is not used 
            BeginDeviceRead(0);

            job.LastExecutionTime = DateTime.UtcNow;
            return true;
        }

        public override bool ProcessNewData(DriverErrorCodes conn, CommJob pendingjob)
        {
            ExecutedJobArgs eJob = new ExecutedJobArgs();
            if (conn != (int)DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError)
            {
                eJob.ErrorCode = conn;
                eJob.Job = pendingjob;
                OnJobExecuted(eJob);
                lock (lockThreadObject)
                    ReceiveBuffer.Clear();
                Flush();
                return true;
            }

            List<byte> receiveBuffer = new List<byte>();
            lock (lockThreadObject)
            {
                receiveBuffer.AddRange(ReceiveBuffer);
                ReceiveBuffer.Clear();
            }

            if (receiveBuffer.Count < OmronFinsEthernetProtocol.ENCAPSULATION_HEADER_SIZE)
            {
                ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = conn, Job = pendingjob };
                eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)OmronFinsEthernetErrorCodes.ErrorAnswer;
                eAJob.Job = pendingjob;
                OnJobExecuted(eAJob);
                Flush();
                return false;
            }

                
            OmronFinsEthernetStation s = pendingjob.Station as OmronFinsEthernetStation;

#if DEBUG
            System.Diagnostics.Debug.WriteLine("Debug SID - {0} - Station {1} - SID Req {2} - SID Rep {3} - {4}", DateTime.Now.ToString("HH:MM:ss.fff"), s.Name, s.GetSID(), receiveBuffer[OmronFinsEthernetProtocol.SID], string.Join(" ", receiveBuffer));
#endif

            //check sequence number
            if (!s.CheckSID(receiveBuffer[OmronFinsEthernetProtocol.SID]))
            {
                CommDriver.OnSystemEvent(null, string.Format("{0}: expected={1}, arrived={2}", Properties.Resources.ErrorSid, s.GetSID(), receiveBuffer[OmronFinsEthernetProtocol.SID]), EventSeverity.Min);
                // keep job active and wait and try to wait a valid (compatible) answer
                sIDError = true;
                return false;
            }

            if (sIDError)
                CommDriver.OnSystemEvent(null, string.Format("{0}:{1}", "SID match", receiveBuffer[OmronFinsEthernetProtocol.SID]), EventSeverity.Min);

            //check MRES and SRES
            if (receiveBuffer[OmronFinsEthernetProtocol.MRES] != 0 ||
                (receiveBuffer[OmronFinsEthernetProtocol.SRES] & 0xBF) != 0)
            {
                ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = conn, Job = pendingjob };
                ((OmronFinsEthernetDriver)eAJob.Job.Station.GetCommDriver()).AdditionalError = $"Main:{receiveBuffer[OmronFinsEthernetProtocol.MRES]:X2},Sub:{receiveBuffer[OmronFinsEthernetProtocol.SRES]:X2}";
                eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)OmronFinsEthernetErrorCodes.ErrorAnswer;
                Flush();
                eAJob.Job = pendingjob;
                OnJobExecuted(eAJob);
                return true;
            }
            OmronFinsEthernetCommJob mJob = pendingjob as OmronFinsEthernetCommJob;
            if (mJob.CommandCodeOnExecute() == CommandCodes.Read)
            {
                if (receiveBuffer.Count < OmronFinsEthernetProtocol.ENCAPSULATION_HEADER_SIZE + mJob.TotalJobSize)
                {
                    return false;
                }
            }

            LastErrorMessage = "";
            eJob.ErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;

            if (mJob.CommandCodeOnExecute() == CommandCodes.Read)
            {
                // Copy received data
                byte[] Answer;
                uint copySize = (ushort)((mJob.isProtocolBool() &&
                    (mJob.TagsList[0].TagNode.DataType != Opc.Ua.DataTypes.Boolean && mJob.ElementNumber == 0)) ?
                    mJob.TotalJobSize * 8 : mJob.TotalJobSize);
                if (mJob.AddressObj.DataFormat != DataFormats.Bit)
                    copySize += copySize % 2;
                Answer = new byte[copySize];
                receiveBuffer.CopyTo((int)OmronFinsEthernetProtocol.ENCAPSULATION_HEADER_SIZE, Answer, 0, (int)copySize);
                eJob.Values = Answer;
            }

            Flush();
            eJob.Job = pendingjob;
            OnJobExecuted(eJob);

            return true;            
        }

        protected override void OnJobExecuted(ExecutedJobArgs e)
        {
            sIDError = false;
            base.OnJobExecuted(e);
        }
        #endregion

        #region Methods

        public void Flush()
        {
            if(GetBytesToRead()!=0)
            {
                byte[] Buffer = new byte[0];
                DeviceRead(Buffer,0);
            }
        }

        public bool InErrorState()
        {
            bool InErrorState = false;
            foreach (var station in CommDriver.GetChannelStations(this))
            {
                if (station.InErrorState && (station.GetChannel() == this))
                {
                        InErrorState = true;
                }
                else
                {
                    InErrorState = false;
                    break;
                }
            }
            return InErrorState;
        }

        #endregion

        #region Properties

        private byte _SourceNetworkAddress;
        public byte SourceNetworkAddress
        {
            get
            {
                return _SourceNetworkAddress;
            }
            set
            {
                _SourceNetworkAddress = value;
            }
        }

        private byte _SourceNode;
        public byte SourceNode
        {
            get
            {
                return _SourceNode;
            }
            set
            {
                _SourceNode = value;
            }
        }

        private byte _SourceUnit;
        public byte SourceUnit
        {
            get
            {
                return _SourceUnit;
            }
            set
            {
                _SourceUnit = value;
            }
        }
        
        
        #endregion
    }
}
