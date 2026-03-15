using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using IpDriverCodeBaseEx;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace OmronFinsEthernet
{
    public class OmronFinsEthernetUDPManager : IDisposable
    {
        #region Constants
        public const int MAX_PacketQueueSize = 5000;
        #endregion

        #region Data Members

        /// <summary>   The UDP channel client. </summary>
        protected UdpClient Client;
        /// <summary>   The UDP channel remote end point. </summary>
        protected IPEndPoint ConnectEndPoint;
        /// <summary>   status of asynchronous read. </summary>
        IAsyncResult readResul;
        /// <summary>   The lock stream. </summary>
        protected object lockStream = new object();

        protected bool autoRestartBeginDeviceRead = false;
        //protected bool EnablePacketBuffering;
        //protected Queue<ReceivedPacket> ReceivePacketQueue;

        protected bool beginDeviceReadAlreadyDone = false;        
        #endregion

        #region Constructors
        public OmronFinsEthernetUDPManager(UdpChannelSettings settings, bool autorestartbegindeviceread = false)
        {
            _UdpChannelLocalHostName = settings.UdpChannelSettingsLocalHostName;
            OmronFinsEthernetUDPManager.GetResolvedConnecionIPAddress(_UdpChannelLocalHostName, out IPAddress resolvedIPAddress);
            if (resolvedIPAddress == null)
                OmronFinsEthernetUDPManager.GetResolvedConnecionIPAddress(string.Empty, out resolvedIPAddress);
            _UdpChannelLocalHostName = resolvedIPAddress.ToString();

            _UdpChannelLocalHostPort = settings.UdpChannelSettingsLocalHostPort;
            _UdpChannelReadTimeout = 2000; // Properties.Settings.Default.UdpChannelSettingsReadTimeout;
            _UdpChannelWriteTimeout = 2000; //Properties.Settings.Default.UdpChannelSettingsWriteTimeout;
            autoRestartBeginDeviceRead = autorestartbegindeviceread;
        }
        #endregion

        #region Properties
        /// <summary>   Local Host name. </summary>
        private string _UdpChannelLocalHostName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the UDP channel Local host. </summary>
        ///
        /// <value> The name of the UDP channel Local host. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string UdpChannelLocalHostName
        {
            get { return _UdpChannelLocalHostName; }
            set
            {
                _UdpChannelLocalHostName = value;
            }
        }

        /// <summary>   Local Host port. </summary>
        private int _UdpChannelLocalHostPort;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the UDP channel Local host port. </summary>
        ///
        /// <value> The UDP channel Local host port. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int UdpChannelLocalHostPort
        {
            get
            {
                return _UdpChannelLocalHostPort;
            }
            set
            {
                _UdpChannelLocalHostPort = value;
            }
        }

        /// <summary>   Read timeout. </summary>
        private int _UdpChannelReadTimeout;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the UDP channel read timeout. </summary>
        ///
        /// <value> The UDP channel read timeout. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int UdpChannelReadTimeout
        {
            get
            {
                return _UdpChannelReadTimeout;
            }
            set
            {
                _UdpChannelReadTimeout = value;
            }
        }

        /// <summary>   Write timeout. </summary>
        private int _UdpChannelWriteTimeout;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the UDP channel write timeout. </summary>
        ///
        /// <value> The UDP channel write timeout. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int UdpChannelWriteTimeout
        {
            get
            {
                return _UdpChannelWriteTimeout;
            }
            set
            {
                _UdpChannelWriteTimeout = value;
            }
        }

        public bool EnableBroadcast
        {
            get
            {
                if (Client != null)
                    return Client.EnableBroadcast;
                return false;
            }
            set
            {
                if (Client != null)
                    Client.EnableBroadcast = value;
            }
        }

        #endregion

        #region Methods

        public static bool GetResolvedConnecionIPAddress(string HostName, out IPAddress resolvedIPAddress)
        {
            bool isResolved = false;
            IPHostEntry hostEntry = null;
            IPAddress resolvIP = null;
            try
            {
                HostName = HostName.Trim();
                if (!IPAddress.TryParse(HostName, out resolvIP))
                {
                    hostEntry = Dns.GetHostEntry(HostName);

                    if (hostEntry != null && hostEntry.AddressList != null && hostEntry.AddressList.Length > 0)
                    {
                        if (hostEntry.AddressList.Length == 1)
                        {
                            resolvIP = hostEntry.AddressList[0];
                            isResolved = true;
                        }
                        else
                        {
                            foreach (IPAddress var in hostEntry.AddressList)
                            {
                                if (var.AddressFamily == AddressFamily.InterNetwork)
                                {
                                    resolvIP = var;
                                    isResolved = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    isResolved = true;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                resolvedIPAddress = resolvIP;
            }

            return isResolved;
        }

        public virtual bool IsDeviceOpen(OmronFinsEthernetChannel refChannel)
        {
            bool returnValue;
            lock (lockStream)
            {
                returnValue = Client != null;
            }
            refChannel.SetStateCommandVariableBit(!returnValue, (UInt16)ChannelVariableBits.ChannelUnconnected);
            return (returnValue);
        }

        public bool IsDeviceOpen()
        {
            bool returnValue;
            lock (lockStream)
            {
                returnValue = Client != null;
            }
            return (returnValue);
        }

        public virtual bool DeviceOpen(OmronFinsEthernetChannel refChannel)
        {
            lock (lockStream)
            {
                if (Client == null)
                {
                    beginDeviceReadAlreadyDone = false;

                    try
                    {
                        if (!String.IsNullOrEmpty(_UdpChannelLocalHostName))
                        {
                            IPAddress resolvedIPAddress;
                            if (GetResolvedConnecionIPAddress(_UdpChannelLocalHostName, out resolvedIPAddress))
                                Client = new UdpClient(new IPEndPoint(resolvedIPAddress, _UdpChannelLocalHostPort));
                            else
                                Client = new UdpClient(_UdpChannelLocalHostPort);
                        }
                        else
                            Client = new UdpClient(_UdpChannelLocalHostPort);
                    }
                    catch (Exception e)
                    {
                        SocketException s = e as SocketException;
                        if (s != null && refChannel.CurrentErrorCode != s.ErrorCode)
                        {
                            //log error...
                            refChannel.CurrentErrorCode = s.ErrorCode;
                            refChannel.SystemEvent(null, string.Format(IpDriverCodeBaseEx.Properties.Resources.UdpSocketError,
                                s.ErrorCode, s.Message), Opc.Ua.EventSeverity.High);
                        }
                        Client = null;
                    }
                }
            }
            return Client != null;
        }

        public void DeviceOpenUpdateState(OmronFinsEthernetChannel refChannel)
        {
            refChannel.SetStateCommandVariableBit(Client == null, (UInt16)ChannelVariableBits.ChannelUnconnected);            
        }

        public virtual bool DeviceClose(OmronFinsEthernetChannel refChannel, bool dispose = false)
        {
            lock (lockStream)
            {
                if (Client != null)
                {
                    try
                    {
                        Client.Close();
                    }
                    catch (Exception e)
                    {
                    }
                }
                Client = null;
                beginDeviceReadAlreadyDone = false;
            }
            return true;
        }

        public void DeviceCloseUpdateState(OmronFinsEthernetChannel refChannel)
        {
            refChannel.SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
        }

        public virtual bool DeviceRead(OmronFinsEthernetChannel refChannel, byte[] Buffer, uint Count)
        {
            int copySize = 0;
            lock (lockStream)
            {
                if (Client == null || ConnectEndPoint == null)
                {
                    return (false);
                }

                int size = (int)Count;
                byte[] recvBuffer;
                try
                {
                    recvBuffer = Client.Receive(ref ConnectEndPoint);
                }
                catch (Exception e)
                {
                    return (false);
                }
                if (recvBuffer.Length != size)
                {
                    return (false);
                }

                // Copy the received bytes
                int recvSize = recvBuffer.Length;
                copySize = size;
                if (copySize > recvSize)
                {
                    copySize = recvSize;
                }
                for (int i = 0; i < copySize; i++)
                {
                    Buffer[i] = recvBuffer[i];
                }
            }
            if(copySize > 0)
                refChannel.IncrementLastRxByte(copySize);
            return (true);
        }

        public virtual bool DeviceRead(OmronFinsEthernetChannel refChannel, byte[] Buffer, uint Count, IPEndPoint RemoteEndPoint)
        {
            int copySize = 0;
            lock (lockStream)
            {
                if (Client == null || RemoteEndPoint == null)
                {
                    return (false);
                }

                int size = (int)Count;
                byte[] recvBuffer;
                try
                {
                    recvBuffer = Client.Receive(ref RemoteEndPoint);
                }
                catch (Exception e)
                {
                    return (false);
                }
                if (recvBuffer.Length != size)
                {
                    return (false);
                }

                // Copy the received bytes
                int recvSize = recvBuffer.Length;
                copySize = size;
                if (copySize > recvSize)
                {
                    copySize = recvSize;
                }
                for (int i = 0; i < copySize; i++)
                {
                    Buffer[i] = recvBuffer[i];
                }
            }
            if(copySize > 0)
                refChannel.IncrementLastRxByte(copySize);
            return (true);
        }

        public virtual bool DeviceWrite(OmronFinsEthernetChannel refChannel, byte[] Buffer, uint Count)
        {
            int size = 0;
            lock (lockStream)
            {
                if ((Client == null))
                {
                    return (false);
                }

                size = (int)Count;
                try
                {
                    Client.Send(Buffer, size, refChannel.DeviceHostName, refChannel.DevicePort);
                }
                catch (Exception e)
                {
                    return (false);
                }
            }
            if(size > 0)
                refChannel.IncrementLastTxByte(size);
            return (true);
        }

        public virtual bool DeviceWrite(OmronFinsEthernetChannel refChannel, byte[] Buffer, uint Count, IPEndPoint RemoteEndPoint)
        {
            int size = 0;
            lock (lockStream)
            {
                if ((Client == null) || RemoteEndPoint == null)
                {
                    return (false);
                }

                size = (int)Count;
                try
                {
                    Client.Send(Buffer, size, RemoteEndPoint);
                }
                catch (Exception e)
                {
                    return (false);
                }
            }
            if(size > 0)
                refChannel.IncrementLastTxByte(size);
            return (true);
        }

        public uint GetBytesToRead()
        {
            lock (lockStream)
            {
                if (Client == null)
                {
                    return (0);
                }

                return ((uint)Client.Available);
            }
        }

        public virtual bool BeginDeviceRead(int Count)
        {
            if (Client == null)
                return (false);
            if (beginDeviceReadAlreadyDone == true)
            {
                return (true);
            }

            beginDeviceReadAlreadyDone = true;
            EndDeviceRead();
            lock (lockStream)
            {
                try
                {
                    readResul = Client.BeginReceive(new AsyncCallback(ReadCallBack), this);
                }
                catch
                {
                    beginDeviceReadAlreadyDone = false;
                    //https://support.microsoft.com/en-us/kb/260018
                    EndDeviceRead();
                    return false;
                }
            }
            return (true);
        }

        public virtual void EndDeviceRead()
        {
            lock (lockStream)
            {
                if (readResul != null)
                {
                    readResul.AsyncWaitHandle.Close();
                    readResul.AsyncWaitHandle.Dispose();
                    readResul = null;
                }
            }

        }
        #endregion

        public event EventHandler<NewDataReceivedArgs> NewDataReceived;

        #region CallBack Method
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Back, called when the read. </summary>
        ///
        /// <param name="ar" type="IAsyncResult">   The archive. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ReadCallBack(IAsyncResult ar)
        {
            byte[] replyBuffer = new byte[0];
            OmronFinsEthernetUDPManager replyUdpChannel = (OmronFinsEthernetUDPManager)ar.AsyncState;
            IPEndPoint remoteEP = replyUdpChannel.ConnectEndPoint;
            lock (lockStream)
            {
                try
                {
                    if(replyUdpChannel.Client != null)
                        replyBuffer = replyUdpChannel.Client.EndReceive(ar, ref remoteEP);
                }
                catch (Exception e)
                {
                }
            }

            beginDeviceReadAlreadyDone = false;

            if (autoRestartBeginDeviceRead)
                BeginDeviceRead(0);

            if (replyBuffer.Count() != 0)
            {
                EventHandler<NewDataReceivedArgs> temp = NewDataReceived;
                if (temp != null)
                {
                    NewDataReceivedArgs e = new NewDataReceivedArgs() { Sender = remoteEP, RxBytes = replyBuffer, Timestamp = DateTime.UtcNow };
                    temp(this, e);
                }
            }
        }

        public virtual void Dispose()
        {
            EndDeviceRead();
        }
        #endregion
    }
}
