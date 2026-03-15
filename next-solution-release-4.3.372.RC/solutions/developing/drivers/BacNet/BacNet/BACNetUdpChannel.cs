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
using IpDriverCodeBase;

namespace BACnet
{
    //public struct ReceivedPacket
    //{
    //    public byte[] Data;
    //    public IPEndPoint RemoteEndPoint;
    //    public DateTime TimeStamp;
    //    public ReceivedPacket(byte[] inPacket, IPEndPoint inRemoteEndPoint, DateTime inTimeStamp)
    //    {
    //        Data = inPacket;
    //        RemoteEndPoint = inRemoteEndPoint;
    //        TimeStamp = inTimeStamp;
    //    }
    //}

    #region  Class BackNetUdpChannel
    /// <summary>   An UDP channel. </summary>
    public class BackNetUdpChannel : UdpChannel, IDisposable
    {
        #region Constants
        #endregion

        #region Data Members

        /// <summary>   The UDP channel client. </summary>
        UdpClient Client_Ex;
        /// <summary>   true if UDP channel is connected. </summary>
        /// <summary>   The UDP channel remote end point. </summary>
        IPEndPoint ConnectEndPoint_Ex;
        /// <summary>   status of asynchronous read. </summary>
        IAsyncResult readResul_Ex;
        /// <summary>   List of locks. </summary>

        bool beginDeviceReadAlreadyDone_ex = false;

        bool _AllowOtherBACnetClients = true;
        bool _DeviceOpenErrorLogged = false;

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
        public BackNetUdpChannel(CommunicationDriver commdriver, BACnetChannelSettings settings, bool multipoint)
            : base(commdriver, settings, multipoint)
        {            
            //moved into driver
            //DeviceOpen();
        }

        protected BackNetUdpChannel()
            : base()
        {         
        }

        protected BackNetUdpChannel(CommunicationDriver commdriver)
            : base(commdriver)
        {            
        }
        #endregion

        #region Properties        
        // Setup driver to coexist with other BACnet clients        
        public bool AllowOtherBACnetClients
        {
            get { return _AllowOtherBACnetClients; }
            set { _AllowOtherBACnetClients = value; }
        }

        public bool InErrorState()
        {
            bool InErrorState = false;
            foreach (var station in CommDriver.GetChannelStations(this))
            {
                if (((BACnetStation)station).WhoIsError || station.InErrorState)
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

        #region Methods

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
                if (_AllowOtherBACnetClients)
                    returnValue &= (Client_Ex != null);
                SetStateCommandVariableBit(((!returnValue) || InErrorState()), (UInt16)ChannelVariableBits.ChannelUnconnected);
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
                    beginDeviceReadAlreadyDone_ex = false;                    
                    try
                    {
                        if (!_AllowOtherBACnetClients)
                        {
                            #region open socket with 'standard' mode

                            #region before to open socket used to echange data with device open in exclusive mode to test if another software already use BACnet port  (and than close it)
                            Client = new UdpClient();
                            Client.ExclusiveAddressUse = true;
                            EndPoint ep = new System.Net.IPEndPoint(IPAddress.Any, UdpChannelLocalHostPort);
                            if (!String.IsNullOrEmpty(UdpChannelLocalHostName))
                            {
                                IPAddress resolvedIPAddress = null;
                                if (GetResolvedConnecionIPAddress(UdpChannelLocalHostName, out resolvedIPAddress))
                                    ep = new IPEndPoint(resolvedIPAddress, UdpChannelLocalHostPort);
                                else
                                {
                                    ep = new IPEndPoint(IPAddress.Any, UdpChannelLocalHostPort);
                                    if (!_DeviceOpenErrorLogged)
                                        CommDriver.OnSystemEvent(null, string.Format(Properties.Resources.LocalHostNameNotResolved, UdpChannelLocalHostName), Opc.Ua.EventSeverity.High);
                                }
                            }
                            else
                            {
                                ep = new IPEndPoint(IPAddress.Any, UdpChannelLocalHostPort);
                            }

                            // check with exclusive option if another program already use BACnet port
                            try
                            {
                                Client.Client.Bind(ep);
                            }
                            catch (Exception ex) {
                                if (!_DeviceOpenErrorLogged)
                                    CommDriver.OnSystemEvent(null, string.Format(Properties.Resources.BACnetPortInUseByAnotherProgram, UdpChannelLocalHostPort, UdpChannelLocalHostName, Properties.Resources.CaptionAllowOtherBACnetClients), Opc.Ua.EventSeverity.High);
                            }
                            // on test finished, close socket
                            Client.Close();
                            #endregion


                            beginDeviceReadAlreadyDone = false;                            
                            if (!String.IsNullOrEmpty(UdpChannelLocalHostName))
                            {
                                IPAddress resolvedIPAddress;
                                if (GetResolvedConnecionIPAddress(UdpChannelLocalHostName, out resolvedIPAddress))
                                    Client = new UdpClient(new IPEndPoint(resolvedIPAddress, UdpChannelLocalHostPort));
                                else
                                    Client = new UdpClient(UdpChannelLocalHostPort);
                            }
                            else
                                Client = new UdpClient(UdpChannelLocalHostPort);
                            ConnectEndPoint = new IPEndPoint(IPAddress.Any, UdpChannelLocalHostPort);
                            IsConnected = false;
                            EnablePacketBuffering = true;
                            ReceivePacketQueue = new Queue<ReceivedPacket>();
                            CurrentErrorCode = 0;                                                            
                            #endregion
                        }
                        else
                        { 
                            /* We need a shared broadcast "listen" port. This is the 0xBAC0 port */
                            /* This will enable us to have more than 1 client, on the same machine. Perhaps it's not that important though. */
                            /* We (might) only recieve the broadcasts on this. Any unicasts to this might be eaten by another local client */
                            Client = new UdpClient();
                            Client.ExclusiveAddressUse = false;
                            Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                            EndPoint ep = new System.Net.IPEndPoint(IPAddress.Any, UdpChannelLocalHostPort);
                            if (!String.IsNullOrEmpty(UdpChannelLocalHostName))
                            {
                                IPAddress resolvedIPAddress = null;
                                if (GetResolvedConnecionIPAddress(UdpChannelLocalHostName, out resolvedIPAddress))
                                    ep = new IPEndPoint(resolvedIPAddress, UdpChannelLocalHostPort);
                                else
                                {                                    
                                    if (!_DeviceOpenErrorLogged)
                                        CommDriver.OnSystemEvent(null, string.Format(Properties.Resources.LocalHostNameNotResolved, UdpChannelLocalHostName), Opc.Ua.EventSeverity.High);
                                }

                            }
                            Client.Client.Bind(ep);
                            Client.DontFragment = false;
                            Client.EnableBroadcast = false;
                            ConnectEndPoint = new IPEndPoint(IPAddress.Any, UdpChannelLocalHostPort);
                        
                            /* This is our own exclusive port. We'll receive everything sent to this. */
                            /* So this is how we'll present our selves to the world */
                            if (Client_Ex == null)
                            {
                                ep = new IPEndPoint(System.Net.IPAddress.Any, 0);
                                if (!String.IsNullOrEmpty(UdpChannelLocalHostName))
                                {
                                    IPAddress resolvedIPAddress = null;
                                    if (GetResolvedConnecionIPAddress(UdpChannelLocalHostName, out resolvedIPAddress))
                                        ep = new IPEndPoint(resolvedIPAddress, 0);
                                }

                                // Opens the socket, udp will choose the Port number
                                //Client_Ex = new UdpClient((IPEndPoint)ep);
                                Client_Ex = new UdpClient();
                                Client_Ex.ExclusiveAddressUse = false;
                                Client_Ex.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                                Client_Ex.Client.Bind(ep);
                                // Gets the Endpoint : the assigned Udp port number in fact
                                ep = Client_Ex.Client.LocalEndPoint;
                                // closes the socket
                                Client_Ex.Close();
                                // Re-opens it with the freeed port number, to be sure it's a real active/server socket
                                // which cannot be disarmed for listen by .NET for incoming call after a few inactivity
                                // minutes ... yes it's like this at least on several systems
                                //Client_Ex = new UdpClient((IPEndPoint)ep);
                                Client_Ex = new UdpClient();

                                //to avoid this type of error : An existing connection was forcibly closed by the remote host
                                const int SIO_UDP_CONNRESET = -1744830452;
                                Client_Ex.Client.IOControl(
                                    (IOControlCode)SIO_UDP_CONNRESET,
                                    new byte[] { 0, 0, 0, 0 },
                                    null
                                );

                                Client_Ex.ExclusiveAddressUse = false;
                                Client_Ex.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                                Client_Ex.Client.Bind(ep);
                                Client_Ex.DontFragment = false;
                                Client_Ex.EnableBroadcast = true;

                                ConnectEndPoint_Ex = new IPEndPoint(IPAddress.Any, 0);
                            }
                        }

                        IsConnected = false;
                        EnablePacketBuffering = true;
                        ReceivePacketQueue = new Queue<ReceivedPacket>();
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
                        Client = null;
                    }                    
                }
            }

            _DeviceOpenErrorLogged = true;

            return (Client != null && (!_AllowOtherBACnetClients ? true : ( Client_Ex != null)));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Determines if we can device close. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceClose()
        {
            base.DeviceClose();

            if (_AllowOtherBACnetClients)
            {
                lock (lockStream)
                {
                    if (Client_Ex != null)
                    {
                        try
                        {
                            Client_Ex.Close();
                        }
                        catch (Exception e)
                        {
                        }
                    }
                    Client_Ex = null;
                    beginDeviceReadAlreadyDone_ex = false;
                }
                IsConnected = false;
            }

            return true;
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
            if (!_AllowOtherBACnetClients)
            {
                return base.DeviceWrite(Buffer, Count);
            }
            else
            {
                lock (lockStream)
                {
                    if ((Client_Ex == null) || !IsConnected)
                    {
                        return (false);
                    }

                    int size = (int)Count;
                    try
                    {
                        Client_Ex.Send(Buffer, size);
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
        public override bool DeviceWrite(byte[] Buffer, uint Count, IPEndPoint RemoteEndPoint)
        {                        
            int size = 0;
            if (!_AllowOtherBACnetClients)
            {
                return base.DeviceWrite(Buffer, Count, RemoteEndPoint);
            }
            else
            {
                lock (lockStream)
                {
                    if ((Client_Ex == null) || IsConnected || RemoteEndPoint == null)
                    {
                        return (false);
                    }

                    size = (int)Count;
                    try
                    {
                        Client_Ex.Send(Buffer, size, RemoteEndPoint);
                    }
                    catch (Exception e)
                    {
                        return (false);
                    }
                }
                if (StatisticsData != null)
                    lock (lockStatisic)
                        DiagnLastTaskTxBytes += size;

                return (true);
            }            
        }              
        #endregion

        #region Virtual Methods        
        public bool BeginDeviceRead_Ex(int Count)
        {
            if (!_AllowOtherBACnetClients)
                return true;

            if (Client_Ex == null)
                return (false);
            if (beginDeviceReadAlreadyDone_ex == true)
            {
                return (true);
            }

            beginDeviceReadAlreadyDone_ex = true;
            EndDeviceRead_Ex();
            lock (lockStream)
            {
                try
                {
                    readResul_Ex = Client_Ex.BeginReceive(new AsyncCallback(ReadCallBack_Ex), this);
                }
                catch (Exception e)
                {
                    beginDeviceReadAlreadyDone_ex = false;
                    //https://support.microsoft.com/en-us/kb/260018
                    EndDeviceRead_Ex();
                    return false;
                }
            }
            return (true);
        }

        public void EndDeviceRead_Ex()
        {
            lock (lockStream)
            {
                if (readResul_Ex != null)
                {
                    readResul_Ex.AsyncWaitHandle.Close();
                    readResul_Ex.AsyncWaitHandle.Dispose();
                    readResul_Ex = null;
                }
            }

        }
        #endregion

        #region CallBack Method        
        public void ReadCallBack_Ex(IAsyncResult ar)
        {
            byte[] replyBuffer = new byte[0];
            BackNetUdpChannel replyUdpChannel = (BackNetUdpChannel)ar.AsyncState;
            //IPEndPoint remoteEP = replyUdpChannel.ConnectEndPoint;
            IPEndPoint remoteEP = new IPEndPoint(System.Net.IPAddress.Any, 0);
            lock (lockStream)
            {
                try
                {
                    replyBuffer = replyUdpChannel.Client_Ex.EndReceive(ar, ref remoteEP);
                }
                catch (Exception e)
                {
                }
            }

            beginDeviceReadAlreadyDone_ex = false;

            if (EnablePacketBuffering)
                BeginDeviceRead_Ex(0);

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
            EndDeviceRead_Ex();
            DeviceClose();
            base.Dispose();
        }
        #endregion
    }
    #endregion
}
