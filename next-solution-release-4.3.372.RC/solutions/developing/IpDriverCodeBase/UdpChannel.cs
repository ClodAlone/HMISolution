////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	udpchannel.cs
//
// summary:	Implements the udpchannel class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using System.Net;
using System.Net.Sockets;
using DriverCodeBase.Enumerators;

namespace IpDriverCodeBase
{
    public struct ReceivedPacket
    {
        public byte[] Data;
        public IPEndPoint RemoteEndPoint;
        public DateTime TimeStamp;
        public ReceivedPacket(byte[] inPacket, IPEndPoint inRemoteEndPoint, DateTime inTimeStamp)
        {
            Data = inPacket;
            RemoteEndPoint = inRemoteEndPoint;
            TimeStamp = inTimeStamp;
        }
    }

    #region  Class UdpChannel
    /// <summary>   An UDP channel. </summary>
    public class UdpChannel : Channel, IDisposable
    {
        #region Constants
        public const int MAX_PacketQueueSize = 5000;
        #endregion

        #region Data Members

        /// <summary>   The UDP channel client. </summary>
        protected UdpClient Client;
        /// <summary>   true if UDP channel is connected. </summary>
        protected bool IsConnected;
        /// <summary>   The UDP channel remote end point. </summary>
        protected IPEndPoint ConnectEndPoint;
        /// <summary>   status of asynchronous read. </summary>
        IAsyncResult readResul; 
        /// <summary>   List of locks. </summary>
        protected object lockList = new object();
        /// <summary>   The lock stream. </summary>
        protected object lockStream = new object();

        protected bool EnablePacketBuffering;
        protected Queue<ReceivedPacket> ReceivePacketQueue;

        protected bool beginDeviceReadAlreadyDone = false;

        #endregion

        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the TcpChannel object. </summary>
        ///
        /// <param name="commdriver" type="CommunicationDriver">    The commdriver. </param>
        /// <param name="settings" type="UdpChannelSettings">       Options for controlling the
        ///                                                         operation. </param>
        /// <param name="multipoint" type="bool">                   true to multipoint. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public UdpChannel(CommunicationDriver commdriver, UdpChannelSettings settings, bool multipoint)
            : base(commdriver, settings, multipoint)
        {
            _UdpChannelReadTimeout = Properties.Settings.Default.UdpChannelSettingsReadTimeout;
            _UdpChannelWriteTimeout = Properties.Settings.Default.UdpChannelSettingsWriteTimeout;

            _UdpChannelHostName = settings.UdpChannelSettingsHostName;
            _UdpChannelHostPort = settings.UdpChannelSettingsHostPort;
            _UdpChannelLocalHostName = settings.UdpChannelSettingsLocalHostName;
            _UdpChannelLocalHostPort = settings.UdpChannelSettingsLocalHostPort;
            
            CurrentErrorCode = 0;
            //DeviceOpen();
        }

        protected UdpChannel()
            : base()
        {
            _UdpChannelHostName = String.Empty;
            _UdpChannelHostPort = 0;
            _UdpChannelLocalHostName = String.Empty;
            _UdpChannelLocalHostPort = 0;
            _UdpChannelReadTimeout = Properties.Settings.Default.UdpChannelSettingsReadTimeout;
            _UdpChannelWriteTimeout = Properties.Settings.Default.UdpChannelSettingsWriteTimeout;
            CurrentErrorCode = 0;
        }

        protected UdpChannel(CommunicationDriver commdriver)
            : base(commdriver)
        {
            _UdpChannelHostName = String.Empty;
            _UdpChannelHostPort = 0;
            _UdpChannelLocalHostName = String.Empty;
            _UdpChannelLocalHostPort = 0;
            _UdpChannelReadTimeout = Properties.Settings.Default.UdpChannelSettingsReadTimeout;
            _UdpChannelWriteTimeout = Properties.Settings.Default.UdpChannelSettingsWriteTimeout;
            CurrentErrorCode = 0;
        }
        #endregion

        #region Properties

        /// <summary>   Host name. </summary>
        private string _UdpChannelHostName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the UDP channel host. </summary>
        ///
        /// <value> The name of the UDP channel host. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string UdpChannelHostName
        {
            get { return _UdpChannelHostName; }
            set
            {
                _UdpChannelHostName = value;
            }
        }

        /// <summary>   Host port. </summary>
        private int _UdpChannelHostPort;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the UDP channel host port. </summary>
        ///
        /// <value> The UDP channel host port. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int UdpChannelHostPort
        {
            get
            {
                return _UdpChannelHostPort;
            }
            set
            {
                _UdpChannelHostPort = value;
            }
        }

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
        
        #endregion

        #region Override Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a device is open. </summary>
        ///
        /// <returns>   true if a device is open, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool IsDeviceOpen()
        {
            lock (lockStream)
            {
                bool returnValue = Client != null;
                SetStateCommandVariableBit(!returnValue, (UInt16)ChannelVariableBits.ChannelUnconnected);
                return (returnValue);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a given device open. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceOpen()
        {
            lock (lockStream)
            {
                if (Client == null)
                {
                    beginDeviceReadAlreadyDone = false;
                    if (!String.IsNullOrEmpty(_UdpChannelHostName))
                    {
                        try
                        {
                            Client = new UdpClient();
                            Client.Connect(UdpChannelHostName, UdpChannelHostPort);
                            Client.Client.ReceiveTimeout = _UdpChannelReadTimeout;
                            Client.Client.SendTimeout = _UdpChannelWriteTimeout;
                            ConnectEndPoint = (IPEndPoint)Client.Client.RemoteEndPoint;
                            IsConnected = true;
                            CurrentErrorCode = 0;
                        }
                        catch (Exception e)
                        {
                            SocketException s = e as SocketException;
                            if (s != null && CurrentErrorCode != s.ErrorCode)
                            {
                                //log error...
                                CurrentErrorCode = s.ErrorCode;
                                CommDriver.OnSystemEvent(null, string.Format(IpDriverCodeBase.Properties.Resources.UdpSocketError,
                                    s.ErrorCode, s.Message), Opc.Ua.EventSeverity.High);
                            }
                            IsConnected = false;
                            Client = null;
                        }
                        EnablePacketBuffering = false;
                    }
                    else
                    {
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
                            ConnectEndPoint = new IPEndPoint(IPAddress.Any, _UdpChannelLocalHostPort);
                            IsConnected = false;
                            EnablePacketBuffering = true;
                            ReceivePacketQueue = new Queue<ReceivedPacket>();
                            CurrentErrorCode = 0;
                        }
                        catch(Exception e)
                        {
                            SocketException s = e as SocketException;
                            if (s != null && CurrentErrorCode != s.ErrorCode)
                            {
                                //log error...
                                CurrentErrorCode = s.ErrorCode;
                                CommDriver.OnSystemEvent(null, string.Format(IpDriverCodeBase.Properties.Resources.UdpSocketError,
                                    s.ErrorCode, s.Message), Opc.Ua.EventSeverity.High);
                            }
                            Client = null;
                        }
                    }
                }
            }
            return Client != null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Determines if we can device close. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceClose()
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
                IsConnected = false;
                beginDeviceReadAlreadyDone = false;
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Synchronous read. </summary>
        ///
        /// <param name="Buffer">   buffer with read values. </param>
        /// <param name="Count">    number of bytes to read. </param>
        ///
        /// <returns>   if true the request byte size was read, othrewise false. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceRead(byte[] Buffer, uint Count)
        {
            lock (lockStream)
            {
                if (Client == null || !IsConnected || ConnectEndPoint == null) 
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
                int copySize = size;
                if (copySize > recvSize)
                {
                    copySize = recvSize;
                }
                for (int i = 0; i < copySize; i++)
                {
                    Buffer[i] = recvBuffer[i];
                }

                if (StatisticsData != null)
                    lock (lockStatisic)
                        DiagnLastTaskRxBytes += copySize;
            }
            return (true);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Synchronous read. </summary>
        ///
        /// <param name="Buffer">   buffer with read values. </param>
        /// <param name="Count">    number of bytes to read. </param>
        /// <param name="RemoteEndPoint"> remote udp channel. </param>
        ///
        /// <returns>   if true the request byte size was read, othrewise false. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool DeviceRead(byte[] Buffer, uint Count, IPEndPoint RemoteEndPoint)
        {
            lock (lockStream)
            {
                if (Client == null || IsConnected || RemoteEndPoint == null)
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
                int copySize = size;
                if (copySize > recvSize)
                {
                    copySize = recvSize;
                }
                for (int i = 0; i < copySize; i++)
                {
                    Buffer[i] = recvBuffer[i];
                }

                if (StatisticsData != null)
                    lock (lockStatisic)
                        DiagnLastTaskRxBytes += copySize;
            }
            return (true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Synchronous write. </summary>
        ///
        /// <param name="Buffer">   buffer with values to write. </param>
        /// <param name="Count">    number of bytes to write. </param>
        ///
        /// <returns>   if true the request write operation was performed, othrewise false. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceWrite(byte[] Buffer, uint Count)
        {
            lock (lockStream)
            {
                if ((Client == null) || !IsConnected)
                {
                    return (false);
                }

                int size = (int)Count;
                try
                {
                    Client.Send(Buffer, size);
                }
                catch (Exception e)
                {
                    return (false);
                }

                if (StatisticsData != null)
                    lock (lockStatisic)
                        DiagnLastTaskTxBytes += size;
            }
            return (true);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Synchronous write. </summary>
        ///
        /// <param name="Buffer">   buffer with values to write. </param>
        /// <param name="Count">    number of bytes to write. </param>
        /// <param name="RemoteEndPoint"> remote udp channel. </param>
        ///
        /// <returns>   if true the request write operation was performed, othrewise false. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool DeviceWrite(byte[] Buffer, uint Count, IPEndPoint RemoteEndPoint)
        {
            lock (lockStream)
            {
                if ((Client == null) || IsConnected || RemoteEndPoint == null)
                {
                    return (false);
                }

                int size = (int)Count;
                try
                {
                    Client.Send(Buffer, size, RemoteEndPoint);
                }
                catch (Exception e)
                {
                    return (false);
                }

                if (StatisticsData != null)
                    lock (lockStatisic)
                        DiagnLastTaskTxBytes += size;
            }
            return (true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets bytes to read. </summary>
        ///
        /// <returns>   The bytes to read. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetBytesToRead()
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets bytes to write. </summary>
        ///
        /// <returns>   The bytes to write. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetBytesToWrite()
        {
            return (0);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the job operation. </summary>
        ///
        /// <param name="job" type="CommJob">   The job. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ExecuteJob(CommJob job)
        {
            base.ExecuteJob(job);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Process the new data described by pendingjob. </summary>
        ///
        /// <param name="pendingjob" type="CommJob">    The pendingjob. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool ProcessNewData(CommJob pendingjob)
        {
            return true;
        }
        #endregion

        #region Virtual Methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Asynchronous reading. </summary>
        ///
        /// <param name="Count" type="int"> Number of. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool BeginDeviceRead(int Count)
        {
            if (Client == null )
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
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Asynchronous read end. </summary>
        ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void EndDeviceRead()
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

        #region CallBack Method
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Back, called when the read. </summary>
        ///
        /// <param name="ar" type="IAsyncResult">   The archive. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ReadCallBack(IAsyncResult ar)
        {
            byte[] replyBuffer = new byte[0];
            UdpChannel replyUdpChannel = (UdpChannel)ar.AsyncState;
            IPEndPoint remoteEP = replyUdpChannel.ConnectEndPoint;
            lock (lockStream)
            {
                try
                {
                    replyBuffer = replyUdpChannel.Client.EndReceive(ar, ref remoteEP);
                }
                catch (Exception e)
                {
                }
            }

            beginDeviceReadAlreadyDone = false;

            if(EnablePacketBuffering)
                BeginDeviceRead(0);

            if (replyBuffer.Count() != 0)
            {
                lock (lockThreadObject)
                {
                    if (EnablePacketBuffering)
                    {
                        ReceivedPacket packet = new ReceivedPacket(replyBuffer, remoteEP, DateTime.Now);
                        if (ReceivePacketQueue.Count() > MAX_PacketQueueSize)
                            ReceivePacketQueue.Dequeue();
                        ReceivePacketQueue.Enqueue(packet);
                    }
                    else
                        replyUdpChannel.ReceiveBuffer.AddRange(replyBuffer);

                    if (replyUdpChannel.StatisticsData != null)
                        lock (replyUdpChannel.lockStatisic)
                            replyUdpChannel.DiagnLastTaskRxBytes += replyBuffer.Count();

                    replyUdpChannel.SetNewDataEvent();
                }
            }
        }
        #endregion


        #region IDisposable
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Dispose()
        {
            EndDeviceRead();
            DeviceClose();
            base.Dispose();
        }
        #endregion
    }
    #endregion
}
