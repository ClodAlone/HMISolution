using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using IpDriverCodeBaseEx;

namespace BACnet
{
    public class UDPManagerBacNet : UDPManager
    {
        #region Constants        
        #endregion

        #region Data Members        
        protected UdpClient Client_Ex;
        /// <summary>   true if UDP channel is connected. </summary>
        
        protected IPEndPoint ConnectEndPoint_Ex;
                       
        protected bool beginDeviceReadAlreadyDone_Ex = false;

        IAsyncResult readResul_Ex;
        
        bool _DeviceOpenErrorLogged = false;
        #endregion

        #region Constructors
        public UDPManagerBacNet(Channel channel, UdpChannelSettings settings)
            :base(channel, settings)
        { 

        }
        #endregion

        #region Properties
        // Setup driver to coexist with other BACnet clients
        bool _AllowOtherBACnetClients = true;
        public bool AllowOtherBACnetClients
        {
            get { return _AllowOtherBACnetClients; }
            set { _AllowOtherBACnetClients = value; }
        }

        public bool autoRestartBeginDeviceRead
        {
            set { base.autoRestartBeginDeviceRead = value; }
            get { return base.autoRestartBeginDeviceRead; }
        }
        #endregion

        #region Methods
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

        public bool BeginDeviceRead_Ex(int Count)
        {
            if (!_AllowOtherBACnetClients)
                return true;

            if (Client_Ex == null)
                return (false);

            if (beginDeviceReadAlreadyDone_Ex == true)
            {
                return (true);
            }

            beginDeviceReadAlreadyDone_Ex = true;
            EndDeviceRead_Ex();
            lock (lockStream)
            {
                try
                {
                    readResul_Ex = Client_Ex.BeginReceive(new AsyncCallback(ReadCallBack_Ex), this);
                }
                catch
                {
                    beginDeviceReadAlreadyDone_Ex = false;
                    //https://support.microsoft.com/en-us/kb/260018
                    EndDeviceRead_Ex();
                    return false;
                }
            }
            return (true);
        }

        public bool IsClientNull()
        {
            return (Client == null);
        }

        public bool IsClient_ExNull()
        {
            return (Client_Ex == null);
        }

        //public ReceivedPacket GetReceivePacketQueue()
        //{
        //    if (ReceivePacketQueue != null && ReceivePacketQueue.Count() > 0)
        //        return ReceivePacketQueue.Dequeue();
        //    else
        //        return new ReceivedPacket();
        //}

        //public int GetReceivePacketQueueCount()
        //{
        //    if (ReceivePacketQueue != null)
        //        return ReceivePacketQueue.Count();
        //    else
        //        return 0;
        //}

        private bool InErrorState()
        {            
            // set channel in error only if all station are in error
            return (((BACnetChannel)refChannel).GetStations().Count(s=> !s.InErrorState) == 0);
        }
        #endregion

        #region Override Methods
        public override bool IsDeviceOpen()
        {
            lock (lockStream)
            {
                bool returnValue = Client != null;
                if (_AllowOtherBACnetClients)
                    returnValue &= (Client_Ex != null);
                refChannel.SetStateCommandVariableBit((!returnValue || InErrorState()), (UInt16)ChannelVariableBits.ChannelUnconnected);
                return (returnValue);
            }
        }

        public override bool DeviceOpen()
        {
            autoRestartBeginDeviceRead = true;

            lock (lockStream)
            {
                if (Client == null)
                {
                    beginDeviceReadAlreadyDone = false;
                    beginDeviceReadAlreadyDone_Ex = false;
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
                                        refChannel.SystemEvent(null, string.Format(Properties.Resources.LocalHostNameNotResolved, UdpChannelLocalHostName), Opc.Ua.EventSeverity.High);
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
                            catch (Exception ex)
                            {
                                if (!_DeviceOpenErrorLogged)
                                    refChannel.SystemEvent(null, string.Format(Properties.Resources.BACnetPortInUseByAnotherProgram, UdpChannelLocalHostPort, UdpChannelLocalHostName, Properties.Resources.CaptionAllowOtherBACnetClients), Opc.Ua.EventSeverity.High);
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
                            //ReceivePacketQueue = new Queue<ReceivedPacket>();
                            refChannel.CurrentErrorCode = 0;

                            //BeginDeviceRead(0);
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
                                        refChannel.SystemEvent(null, string.Format(Properties.Resources.LocalHostNameNotResolved, UdpChannelLocalHostName), Opc.Ua.EventSeverity.High);
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

                        //ReceivePacketQueue = new Queue<ReceivedPacket>();
                        refChannel.CurrentErrorCode = 0;
                        //BeginDeviceRead(0);
                        //if (Client_Ex !=null)
                        //    BeginDeviceRead_Ex(0);
                    }
                    catch (Exception e)
                    {
                        SocketException s = e as SocketException;
                        if (s != null && refChannel.CurrentErrorCode != s.ErrorCode)
                        {
                            //log error...
                            refChannel.CurrentErrorCode = s.ErrorCode;
                            refChannel.SystemEvent(null, string.Format(IpDriverCodeBaseEx.Properties.Resources.UdpSocketError,s.ErrorCode, s.Message), Opc.Ua.EventSeverity.High);
                        }
                        Client = null;
                    }
                }
            }

            _DeviceOpenErrorLogged = true;

            return (Client != null && (!_AllowOtherBACnetClients ? true : (Client_Ex != null)));
        }

        public override bool DeviceClose(bool dispose = false)
        {
            EndDeviceRead();
            base.DeviceClose(dispose);

            if (_AllowOtherBACnetClients)
            {
                EndDeviceRead_Ex();
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
                    beginDeviceReadAlreadyDone_Ex = false;
                }
            }

            return true;
        }

        public override bool DeviceWrite(byte[] Buffer, uint Count)
        {
            //lock (lockStream)
            //{
            //    if ((Client == null) || !IsConnected)
            //    {
            //        return (false);
            //    }

            //    int size = (int)Count;
            //    try
            //    {
            //        Client.Send(Buffer, size);
            //    }
            //    catch (Exception e)
            //    {
            //        return (false);
            //    }
            //    refChannel.IncrementLastTxByte(size);
            //}
            //return (true);            
            if (!_AllowOtherBACnetClients)
            {
                return base.DeviceWrite(Buffer, Count);
            }
            else
            {
                lock (lockStream)
                {
                    if ((Client_Ex == null))
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

                    //if (StatisticsData != null)
                    //    lock (lockStatisic)
                    //        DiagnLastTaskTxBytes += size;
                }
                return (true);
            }
        }

        public override bool DeviceWrite(byte[] Buffer, uint Count, IPEndPoint RemoteEndPoint)
        {            
            if (!_AllowOtherBACnetClients)
            {
                return base.DeviceWrite(Buffer, Count, RemoteEndPoint);
            }
            else
            {
                lock (lockStream)
                {
                    if ((Client_Ex == null) || RemoteEndPoint == null)
                    {
                        return (false);
                    }

                    int size = (int)Count;
                    try
                    {
                        Client_Ex.Send(Buffer, size, RemoteEndPoint);
                    }
                    catch (Exception e)
                    {
                        return (false);
                    }
                }
                //if (StatisticsData != null)
                //    lock (lockStatisic)
                //        DiagnLastTaskTxBytes += size;

                return (true);
            }
        }
        
        #endregion

        #region CallBack Method
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Back, called when the read. </summary>
        ///
        /// <param name="ar" type="IAsyncResult">   The archive. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ReadCallBack_Ex(IAsyncResult ar)
        {
            byte[] replyBuffer = new byte[0];
            UDPManagerBacNet replyUdpChannel = (UDPManagerBacNet)ar.AsyncState;
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

            beginDeviceReadAlreadyDone_Ex = false;

            if (autoRestartBeginDeviceRead)
                BeginDeviceRead_Ex(0);

            if (replyBuffer.Count() != 0)
            {
                replyUdpChannel.refChannel.IncrementLastRxByte(replyBuffer.Count());

                //if (EnablePacketBuffering)
                //{
                //    ReceivedPacket packet = new ReceivedPacket(replyBuffer, remoteEP, DateTime.Now);
                //    if (ReceivePacketQueue.Count() > MAX_PacketQueueSize)
                //        ReceivePacketQueue.Dequeue();
                //    ReceivePacketQueue.Enqueue(packet);
                //}
                //else
                //    replyUdpChannel.refChannel.UpdateReceiveBuffer/*.ReceiveBuffer.AddRange*/(remoteEP, replyBuffer, DateTime.UtcNow);

                replyUdpChannel.refChannel.UpdateReceiveBuffer(remoteEP, replyBuffer, DateTime.UtcNow, out bool setNewDataEvent, out int beginDeviceReadSize);
                if (setNewDataEvent)
                    replyUdpChannel.refChannel.SetNewDataEvent();
            }
        }

        public override void Dispose()
        {
            EndDeviceRead_Ex();
            base.Dispose();            
        }
        #endregion
    }
}
