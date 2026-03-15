using System;
using System.Collections.Generic;
using DriverCodeBaseEx;
using IpDriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using System.Net;
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

            // with local bound port set, channel socketManager is initializzaed (with base()) but not used
            if (IsLocalBoundPortSet())
                InitLocalBoundPortChannel(commdriver, settings);
        }

        #endregion

        #region Data Members
        private OmronFinsEthernetDriver commDriver = null;
        private bool sIDError = false;
        #endregion

        #region Override Methods        
        public override bool DeviceOpen()
        {
            if (IsLocalBoundPortSet())
            {
                return commDriver.DeviceOpenLocalBoundPort(this);
            }
            else
            {
                return base.DeviceOpen();
            }
        }

        public override bool DeviceClose()
        {
            if (IsLocalBoundPortSet())
            {
                return commDriver.DeviceCloseLocalBoundPort(this);
            }
            else
            {
                return base.DeviceClose();
            }
        }

        public override bool BeginDeviceRead(int Count)
        {
            if (IsLocalBoundPortSet())
            {
                return commDriver.BeginDeviceReadLocalBoundPort(this, Count);
            }
            else
            {
                return base.BeginDeviceRead(Count);
            }
        }

        public override bool IsDeviceOpen()
        {
            bool returnValue;

            if (IsLocalBoundPortSet())
                returnValue = commDriver.IsDeviceOpenLocalBoundPort(this);
            else
                returnValue = base.IsDeviceOpen();
            SetStateCommandVariableBit(((!returnValue) || InErrorState()), (UInt16)DriverCodeBaseEx.Enumerators.ChannelVariableBits.ChannelUnconnected);
            return (returnValue);            
        }

        public override bool DeviceWrite(byte[] Buffer, uint Count)
        {
            if (IsLocalBoundPortSet())
            {
                return commDriver.DeviceWriteLocalBoundPort(this, Buffer, Count);
            }
            else
            {
                return base.DeviceWrite(Buffer, Count);
            }
        }

        public override void ResetNewDataEvent()
        {            
            if (IsLocalBoundPortSet())            
                commDriver.ResetNewDataEventLocalBoundPort(this);            
            base.ResetNewDataEvent();            
        }

        public override void WaitNewDataEvent(ref DriverErrorCodes conn)
        {
            base.WaitNewDataEvent(ref conn);
            if (IsLocalBoundPortSet())
                commDriver.WaitNewDataEventLocalBoundPort(this);
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
            if(!TestConnection)
                count = OmronFinsEthernetProtocol.PrepareRequest(mJob, ref pdu, this);
            else
                count = OmronFinsEthernetProtocol.PrepareRequestTestInformation(mJob, ref pdu, this);

            if (count == 0)
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
            if (receiveBuffer[OmronFinsEthernetProtocol.MRES] != 0 || (receiveBuffer[OmronFinsEthernetProtocol.SRES] & 0xBF) != 0)
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
                if (!TestConnection)
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
                else
                {
                    string plc = System.Text.Encoding.UTF8.GetString(receiveBuffer.ToArray(), 14, 20);                       
                    string CpuVersion = System.Text.Encoding.UTF8.GetString(receiveBuffer.ToArray(), 31, 20);
                    string[] CpuVersionList = CpuVersion.Split('\0');
                    CpuVersion = CpuVersionList[0];
                    //string fiemware = System.Text.Encoding.UTF8.GetString(ReceiveBuffer, 37, 7);
                    OmronFinsEthernetDriver.Addinfo(string.Format(Properties.Resources.PLCFamily, plc));
                    OmronFinsEthernetDriver.Addinfo(string.Format(Properties.Resources.PLCModuleVersion, CpuVersion));
                    //OmronFinsEthernetDriver.Addinfo(string.Format("PLC firmware : {0}", fiemware));
                }
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
            // do nothing with local bound port with specific port
            if (IsLocalBoundPortSet())
            {

            }
            else
            {
                if (GetBytesToRead() != 0)
                {
                    byte[] Buffer = new byte[0];
                    DeviceRead(Buffer, 0);
                }
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

        /// <summary>
        /// Check if communication with device shoulb be managed be channel or driver (local host port != 0)
        /// </summary>
        /// <returns></returns>
        public bool IsLocalBoundPortSet()
        {
            return (SocketManager != null && SocketManager.UdpChannelLocalHostPort != 0);
        }

        /// <summary>
        /// Initialize the local bound traffic using with a specific port
        /// </summary>
        /// <param name="commdriver"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        private bool InitLocalBoundPortChannel(CommunicationDriver commdriver, UdpChannelSettings settings)
        {
            IPAddress resolvedIPAddress;

            commDriver = commdriver as OmronFinsEthernetDriver;

            OmronFinsEthernetUDPManager.GetResolvedConnecionIPAddress(settings.UdpChannelSettingsLocalHostName, out resolvedIPAddress);
            if (resolvedIPAddress == null)
                OmronFinsEthernetUDPManager.GetResolvedConnecionIPAddress(string.Empty, out resolvedIPAddress);
            LocalIpAddressUdpPortID = OmronFinsEthernetProtocol.GetIpAddressUdpPortID(resolvedIPAddress.ToString(), settings.UdpChannelSettingsLocalHostPort);

            _DeviceHostName = settings.UdpChannelSettingsHostName;
            _DevicePort = settings.UdpChannelSettingsHostPort;
            OmronFinsEthernetUDPManager.GetResolvedConnecionIPAddress(_DeviceHostName, out resolvedIPAddress);
            DeviceIpAddressUdpPortID = OmronFinsEthernetProtocol.GetIpAddressUdpPortID((resolvedIPAddress != null ? resolvedIPAddress.ToString() : _DeviceHostName), _DevicePort);

            commDriver.InitLocalBoundPort(this, settings);

            return true;
        }

        public void SetReceiveBuffer(byte[] receiveBuffer)
        {
            lock (lockThreadObject)
            {
                ReceiveBuffer.Clear();
                ReceiveBuffer.AddRange(receiveBuffer);
            }
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
        
        /// <summary>
        /// Flag the test connection
        /// </summary>
        public bool TestConnection = false;

        /// <summary>
        /// Identifier (local IpAddress:local UdpPort) of channel when communication is managed at driver level
        /// </summary>
        public string _LocalIpAddressUdpPortID = string.Empty;
        public string LocalIpAddressUdpPortID
        {
            get
            {
                return _LocalIpAddressUdpPortID;
            }
            set
            {
                _LocalIpAddressUdpPortID = value;
            }
        }

        /// <summary>
        /// Identifier (Device IdAddress:Device UdpPort) of device when communication is managed at driver level
        /// </summary>
        public string _DeviceIpAddressUdpPortID = string.Empty;
        public string DeviceIpAddressUdpPortID
        {
            get
            {
                return _DeviceIpAddressUdpPortID;
            }
            set
            {
                _DeviceIpAddressUdpPortID = value;
            }
        }

        /// <summary>
        /// Device ip address
        /// </summary>
        private string _DeviceHostName;
        public string DeviceHostName
        {
            get { return _DeviceHostName; }
            set
            {
                _DeviceHostName = value;
            }
        }

        /// <summary>
        /// Device udp port
        /// </summary>
        private int _DevicePort;
        public int DevicePort
        {
            get
            {
                return _DevicePort;
            }
            set
            {
                _DevicePort = value;
            }
        }
        #endregion
    }
}
