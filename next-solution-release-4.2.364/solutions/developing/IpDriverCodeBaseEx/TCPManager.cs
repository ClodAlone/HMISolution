using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IpDriverCodeBaseEx
{
    public class TCPManager : IDisposable
    {
        #region Constants
        public const int testSocketDelay_S = 10;
        public const int testSocketTimeOut_S = 5;

        private const int WSAECONNRESET = 10054;  // Connection reset by peer.
        #endregion
        #region Data Members

        /// <summary>   The TCP channel client. </summary>
        TcpClient TcpChannelClient;
        /// <summary>   true if TCP channel is connected. </summary>
        bool TcpChannelIsConnected;
        /// <summary>   The TCP channel stream. </summary>
        NetworkStream TcpChannelStream;
        /// <summary>   The lock stream. </summary>
        protected object lockStream = new object();

        private TimeOutSocket ChanneTimeOutSocket;
        DateTime inactivityTimer;

        bool isFirstOpen;
        string selectedHostName;

        bool PrimaryHostErrorState;
        bool BackupHostErrorState;
        DateTime testSocketTimer;
        protected object lockTestSocket = new object();
        bool testSocketPending = false;
        IpTcpClient TesTcpclient;
        bool switchHost = false;
        Channel refChannel;
        #endregion

        public event EventHandler<EventArgs> ClientDisconnect;

        #region Constructors
        public TCPManager(Channel channel, TcpChannelSettings settings, TcpClient connectedClient = null, bool autoRestartBeginDeviceRead = false)
        {
            refChannel = channel;
            _TcpChannelReadTimeout = Properties.Settings.Default.TcpChannelSettingsReadTimeout;
            _TcpChannelWriteTimeout = Properties.Settings.Default.TcpChannelSettingsWriteTimeout;
            selectedHostName = TcpChannelHostName;
            _autoRestartBeginDeviceRead = autoRestartBeginDeviceRead;
            if (connectedClient != null)
            {
                TcpChannelClient = connectedClient;
                TcpChannelStream = TcpChannelClient.GetStream();
                TcpChannelStream.ReadTimeout = TcpChannelReadTimeout;
                TcpChannelStream.WriteTimeout = TcpChannelWriteTimeout;
                TcpChannelIsConnected = true;
                channel.CurrentErrorCode = 0;
                ChanneTimeOutSocket = null;
                inactivityTimer = DateTime.UtcNow;
                testSocketTimer = DateTime.UtcNow;
            }
            else
            {
                TcpChannelClient = new TcpClient();
                TcpChannelIsConnected = false;
                _TcpChannelHostName = settings.TcpChannelSettingsHostName;
                _TcpChannelHostPort = settings.TcpChannelSettingsHostPort;
                ChanneTimeOutSocket = new TimeOutSocket();
                TcpChannelBackupHostName = settings.TcpChannelSettingsBackupHostName;
                TcpChannelSwitchHostTimeout = settings.TcpChannelSettingsSwitchHostTimeout;
                isFirstOpen = true;
            }
        }
        #endregion

        #region Properties

        /// <summary>   Host name. </summary>
        private string _TcpChannelHostName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the TCP channel host. </summary>
        ///
        /// <value> The name of the TCP channel host. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string TcpChannelHostName
        {
            get { return _TcpChannelHostName; }
            set
            {
                _TcpChannelHostName = value;
            }
        }

        /// <summary>   Host port. </summary>
        private int _TcpChannelHostPort;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the TCP channel host port. </summary>
        ///
        /// <value> The TCP channel host port. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int TcpChannelHostPort
        {
            get
            {
                return _TcpChannelHostPort;
            }
            set
            {
                _TcpChannelHostPort = value;
            }
        }

        /// <summary>  Backup Host name. </summary>
        private string _TcpChannelBackupHostName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the TCP channel settings backup host. </summary>
        ///
        /// <value> The name of the TCP channel settings backup host. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string TcpChannelBackupHostName
        {
            get { return _TcpChannelBackupHostName; }
            set
            {
                _TcpChannelBackupHostName = value;
            }
        }

        /// <summary>   Switch Host timeout. </summary>
        private int _TcpChannelSwitchHostTimeout;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the TCP channel settings Switch Host timeout. </summary>
        ///
        /// <value> The TCP channel settings Switc hHost timeout. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int TcpChannelSwitchHostTimeout
        {
            get
            {
                return _TcpChannelSwitchHostTimeout;
            }
            set
            {
                _TcpChannelSwitchHostTimeout = value;
            }
        }

        /// <summary>   Read timeout. </summary>
        private int _TcpChannelReadTimeout;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the TCP channel read timeout. </summary>
        ///
        /// <value> The TCP channel read timeout. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int TcpChannelReadTimeout
        {
            get
            {
                return _TcpChannelReadTimeout;
            }
            set
            {
                _TcpChannelReadTimeout = value;
            }
        }

        /// <summary>   Write timeout. </summary>
        private int _TcpChannelWriteTimeout;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the TCP channel write timeout. </summary>
        ///
        /// <value> The TCP channel write timeout. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int TcpChannelWriteTimeout
        {
            get
            {
                return _TcpChannelWriteTimeout;
            }
            set
            {
                _TcpChannelWriteTimeout = value;
            }
        }

        protected bool _autoRestartBeginDeviceRead = false;
        public bool autoRestartBeginDeviceRead
        {
            set { _autoRestartBeginDeviceRead = value; }
            get { return _autoRestartBeginDeviceRead; }
        }
        #endregion

        #region Methods

        public bool Connected(uint maxInactivitySec = 0)
        {
            bool connected = false;

            lock (lockStream)
            {
                connected = IsDeviceOpen();
                if (connected)
                {
                    try
                    {
                        var connections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections().ToList()
                        .FindAll(x => x.LocalEndPoint.Equals(TcpChannelClient.Client.LocalEndPoint)
                                            && x.RemoteEndPoint.Equals(TcpChannelClient.Client.RemoteEndPoint));

                        connected = (connections != null && connections.Count > 0);
                        if (connected)
                        {
                            connected = (!(connections[0].State == TcpState.CloseWait || connections[0].State == TcpState.Closing || connections[0].State == TcpState.Closed));
                            if (connected)
                            {
                                if (maxInactivitySec != 0)
                                {
                                    if (TcpChannelClient.Client.Available > 0)
                                        inactivityTimer = DateTime.UtcNow;
                                    else if ((DateTime.UtcNow - inactivityTimer).TotalSeconds > maxInactivitySec)
                                        connected = false;
                                }
                            }
                        }
                    }
                    catch (Exception ex) { connected = false; }
                }
            }

            return connected;
        }

        public bool IsDeviceOpen()
        {
            bool rt;
            lock (lockStream)
            {
                rt = ((TcpChannelClient != null) && (TcpChannelStream != null) && TcpChannelIsConnected);
                if (!String.IsNullOrEmpty(TcpChannelBackupHostName))
                {
                    switchHost = false;
                    if ((refChannel.GetStateCommandVariableBit(ref switchHost, (UInt16)ChannelVariableBits.SwitchServer) == true) && (switchHost == true))
                    {
                        return (false);
                    }
                    if (!isFirstOpen && rt && (DateTime.UtcNow - testSocketTimer).TotalSeconds > testSocketDelay_S)
                    {
                        try
                        {
                            TestSocketStart(selectedHostName == TcpChannelHostName ? TcpChannelBackupHostName : TcpChannelHostName, TcpChannelHostPort);
                        }
                        catch (Exception ex)
                        {
                            if (selectedHostName == TcpChannelHostName)
                            {
                                BackupHostErrorState = true;
                            }
                            else
                            {
                                PrimaryHostErrorState = true;
                            }
                        }
                    }
                }

                
            }
            refChannel.SetStateCommandVariableBit(!rt, (UInt16)ChannelVariableBits.ChannelUnconnected);
            refChannel.SetStateCommandVariableBit(PrimaryHostErrorState, (UInt16)ChannelVariableBits.PrimaryHostErrorState);
            refChannel.SetStateCommandVariableBit(BackupHostErrorState, (UInt16)ChannelVariableBits.BackupHostErrorState);
            refChannel.SetStateCommandVariableBit(selectedHostName != TcpChannelHostName, (UInt16)ChannelVariableBits.ConnectedHost);
            return rt;
        }

        private void TestSocketStart(string serverip, int serverport)
        {
            lock (lockTestSocket)
            {
                if (testSocketPending)
                {
                    if ((testSocketTimer - testSocketTimer).TotalSeconds >= testSocketDelay_S * 2)
                        CloseTestSocket();
                    return;
                }

                testSocketTimer = DateTime.UtcNow;
                serverip = serverip.Trim();
                TesTcpclient = new IpTcpClient(serverip, serverport);
                TesTcpclient.BeginConnect(serverip, serverport,
                    new AsyncCallback(TestSocketCallBackMethod), TesTcpclient);
                testSocketPending = true;
            }

        }

        private void CloseTestSocket()
        {
            testSocketPending = false;
            if (TesTcpclient == null)
                return;
            try
            {
                TesTcpclient.GetStream().Close();
                TesTcpclient.Close();

                throw new Exception();
            }
            catch (Exception ex)
            {
                if (TesTcpclient != null)
                {
                    if (TesTcpclient.Serverip.Equals(TcpChannelHostName))
                    {
                        PrimaryHostErrorState = true;
                    }
                    else
                        BackupHostErrorState = true;
                    TesTcpclient = null;
                }
            }
        }

        private void TestSocketCallBackMethod(IAsyncResult asyncresult)
        {
            try
            {
                IpTcpClient tcpclient = asyncresult.AsyncState as IpTcpClient;
                if (tcpclient != null)
                {
                    try
                    {
                        if (tcpclient.Client != null)
                        {
                            tcpclient.EndConnect(asyncresult);
                            if (tcpclient.Serverip.Equals(TcpChannelHostName))
                            {
                                PrimaryHostErrorState = false;
                            }
                            else
                                BackupHostErrorState = false;
                            tcpclient.Close();
                        }
                        else
                            throw new Exception();
                    }
                    catch (Exception ex)
                    {
                        if (tcpclient.Serverip.Equals(TcpChannelHostName))
                        {
                            PrimaryHostErrorState = true;
                        }
                        else
                            BackupHostErrorState = true;
                    }
                }
            }
            catch (Exception ex)
            {
            }


            lock (lockTestSocket)
            {
                testSocketTimer = DateTime.UtcNow;
                testSocketPending = false;
            }

        }

        public bool DeviceOpen()
        {
            lock (lockStream)
            {

                if ((!TcpChannelIsConnected || (switchHost == true)) && (ChanneTimeOutSocket != null))
                {
                    if (isFirstOpen)
                    {
                        isFirstOpen = false;
                        inactivityTimer = DateTime.UtcNow;
                        testSocketTimer = DateTime.UtcNow;
                        selectedHostName = TcpChannelHostName;
                    }
                    try
                    {
                        lock (lockTestSocket)
                        {
                            if (testSocketPending)
                            {
                                CloseTestSocket();
                                return false;
                            }
                            if (!String.IsNullOrWhiteSpace(TcpChannelBackupHostName) && !refChannel.ChannelCloseBySuspend &&
                                (((DateTime.UtcNow - inactivityTimer).TotalMilliseconds > TcpChannelSwitchHostTimeout) || (switchHost == true)))
                            {
                                string oldHostName = selectedHostName;
                                if (selectedHostName == TcpChannelHostName)
                                {
                                    selectedHostName = TcpChannelBackupHostName;
                                }
                                else
                                {
                                    selectedHostName = TcpChannelHostName;
                                }
                                refChannel.SystemEvent(null,
                                        string.Format(IpDriverCodeBaseEx.Properties.Resources.SwitchChannelHostName,
                                            refChannel.Name, oldHostName, selectedHostName, switchHost), Opc.Ua.EventSeverity.Low);

                                testSocketTimer = DateTime.UtcNow;
                                inactivityTimer = DateTime.UtcNow;
                            }
                            DeviceClose();
                            TcpChannelClient = ChanneTimeOutSocket.Connect(selectedHostName, TcpChannelHostPort, refChannel.Timeout);
                            TcpChannelClient.NoDelay = Properties.Settings.Default.NoDelay;
                            TcpChannelStream = TcpChannelClient.GetStream();
                            TcpChannelStream.ReadTimeout = TcpChannelReadTimeout;
                            TcpChannelStream.WriteTimeout = TcpChannelWriteTimeout;
                            TcpChannelIsConnected = true;
                            refChannel.CurrentErrorCode = 0;
                            testSocketTimer = DateTime.UtcNow;
                            inactivityTimer = DateTime.UtcNow;
                            if (selectedHostName == TcpChannelHostName)
                                PrimaryHostErrorState = false;
                            else
                                BackupHostErrorState = false;
                            refChannel.ChannelCloseBySuspend = false;
                        }
                    }
                    catch (Exception e)
                    {
                        SocketException s = e as SocketException;
                        if (s != null && refChannel.CurrentErrorCode != s.ErrorCode)
                        {
                            //log error...
                            int nPort = 0;
                            if (TcpChannelClient != null && TcpChannelClient.Client != null && TcpChannelClient.Client.LocalEndPoint != null)
                            {
                                nPort = ((IPEndPoint)TcpChannelClient.Client.LocalEndPoint).Port;
                            }
                            refChannel.CurrentErrorCode = s.ErrorCode;
                            refChannel.SystemEvent(null, string.Format(IpDriverCodeBaseEx.Properties.Resources.SocketError,
                                s.ErrorCode, s.Message, nPort), Opc.Ua.EventSeverity.High);
                        }
                        DeviceClose();
                        if (selectedHostName == TcpChannelHostName)
                            PrimaryHostErrorState = true;
                        else
                            BackupHostErrorState = true;
                    }
                }

            }
            refChannel.SetStateCommandVariableBit(PrimaryHostErrorState, (UInt16)ChannelVariableBits.PrimaryHostErrorState);
            refChannel.SetStateCommandVariableBit(BackupHostErrorState, (UInt16)ChannelVariableBits.BackupHostErrorState);
            refChannel.SetStateCommandVariableBit(selectedHostName != TcpChannelHostName, (UInt16)ChannelVariableBits.ConnectedHost);
            refChannel.SetStateCommandVariableBit(!TcpChannelIsConnected, (UInt16)ChannelVariableBits.ChannelUnconnected);
            if (switchHost == true)
            {
                switchHost = false;
                refChannel.SetStateCommandVariableBit(false, (UInt16)ChannelVariableBits.SwitchServer);
            }
            doBeginDeviceRead = TcpChannelIsConnected;
            return TcpChannelIsConnected;
        }

        public bool DeviceClose()
        {
            lock (lockStream)
            {
                if (TcpChannelClient != null)
                {
                    try
                    {
                        bool ClientShutdown = (TcpChannelClient.Client != null && TcpChannelClient.Client.Connected);
                        if (ClientShutdown)
                        {
                            TcpChannelClient.Client.Shutdown(SocketShutdown.Both);
                            TcpChannelClient.Client.Disconnect(false);
                        }
                        if (TcpChannelStream != null)
                        {
                            TcpChannelStream.Close();
                            TcpChannelStream.Dispose();
                            TcpChannelStream = null;
                        }
                        TcpChannelClient.Close();
                        TcpChannelClient = null;
                        if (ClientShutdown)
                            Thread.Sleep(1000);
                    }
                    catch
                    {

                    }
                }
                TcpChannelIsConnected = false;
                doBeginDeviceRead = false;
            }
            IsDeviceOpen();
            refChannel.SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
            return true;
        }

        /// <summary>   Flushes this object. </summary>
        public bool Flush()
        {
            bool ret = true;
            lock (lockStream)
            {

                if ((TcpChannelClient == null) || !TcpChannelIsConnected || (TcpChannelStream == null))
                {
                    return (ret);
                }
                if (TcpChannelStream.CanRead)
                {
                    int nRead = 0;
                    int size = 100;
                    byte[] Buffer = new byte[size];
                    while (TcpChannelStream.DataAvailable)
                    {
                        try
                        {
                            TcpChannelStream.Read(Buffer, 0, size);
                        }
                        catch (System.IO.IOException e)
                        {
                            string errorMessage = string.Empty;
                            if (e.InnerException != null)
                            {
                                errorMessage = string.Format(IpDriverCodeBaseEx.Properties.Resources.SocketError,
                                    e.InnerException.Message, e.InnerException, ((IPEndPoint)TcpChannelClient.Client.LocalEndPoint).Port.ToString());
                            }
                            else
                            {
                                errorMessage = string.Format(IpDriverCodeBaseEx.Properties.Resources.SocketError, e.Message, e,
                                                             ((IPEndPoint)TcpChannelClient.Client.LocalEndPoint).Port.ToString());
                            }

                            refChannel.SystemEvent(null, errorMessage, Opc.Ua.EventSeverity.High);
                            ret = false;
                            break;
                        }
                    }
                    if (nRead > 0)
                        inactivityTimer = DateTime.UtcNow;
                }
            }
            return (ret);
        }

        public bool DeviceRead(byte[] Buffer, uint Count)
        {
            int readCount = 0;
            lock (lockStream)
            {
                DateTime begin = DateTime.UtcNow;
                if ((TcpChannelClient == null) || !TcpChannelIsConnected || (TcpChannelStream == null))
                {
                    return (false);
                }

                int size = (int)Count;
                try
                {
                    if (!TcpChannelClient.Connected)
                    {
                        DeviceClose();
                        DeviceOpen();
                        if (!TcpChannelClient.Connected)
                            return false;
                    }


                    readCount = TcpChannelStream.Read(Buffer, 0, size);
                    if (readCount != size)
                    {
                        return (false);
                    }

                    inactivityTimer = DateTime.UtcNow;
                }
                catch (Exception e)
                {
                    DeviceClose();
                    return false;
                }
            }
            if(readCount > 0)
                refChannel.IncrementLastRxByte(readCount);
            return (true);
        }
        public uint DeviceReadSynchronous(byte[] Buffer, uint Count)
        {
            int readCount = 0;
            lock (lockStream)
            {
                
                if ((TcpChannelClient == null) || !TcpChannelIsConnected || (TcpChannelStream == null))
                {
                    return (0);
                }

                int size = (int)Count;
                try
                {
                    if (!TcpChannelClient.Connected)
                    {
                        DeviceClose();
                        DeviceOpen();
                        if (!TcpChannelClient.Connected)
                            return (0);
                    }


                    readCount = TcpChannelStream.Read(Buffer, 0, size);

                    inactivityTimer = DateTime.UtcNow;
                }
                catch (Exception e)
                {
                    DeviceClose();
                    return (0);
                }
            }
            if(readCount > 0)
                refChannel.IncrementLastRxByte(readCount);
            return ((uint)readCount);
        }

        public bool DeviceWrite(byte[] Buffer, uint Count)
        {
            lock (lockStream)
            {
                if ((TcpChannelClient == null) || !TcpChannelIsConnected || (TcpChannelStream == null))
                {
                    return (false);
                }

                int size = (int)Count;
                try
                {
                    if (!TcpChannelClient.Connected)
                    {
                        DeviceClose();
                        DeviceOpen();
                        if (!TcpChannelClient.Connected)
                            return false;
                    }
                    TcpChannelStream.Write(Buffer, 0, size);
                }
                catch (Exception e)
                {
                    DeviceClose();
                    return false;
                }

                inactivityTimer = DateTime.UtcNow;
                refChannel.IncrementLastTxByte(size);

                return (true);
            }
        }

        public uint GetBytesToRead()
        {
            lock (lockStream)
            {
                if ((TcpChannelClient == null) || !TcpChannelIsConnected || (TcpChannelStream == null))
                {
                    return (0);
                }

                if (TcpChannelStream.DataAvailable)
                {
                    return (1);
                }

                return (0);
            }
        }

        bool doBeginDeviceRead = true;
        public bool BeginDeviceRead(int Count)
        {
            lock (lockStream)
            {
                try
                {
                    if (!doBeginDeviceRead)
                        return true;
                    TcpChannelReply replyStateObject = new TcpChannelReply(0, Count);
                    replyStateObject.ReplyStream = TcpChannelStream;
                    doBeginDeviceRead = false;
                    TcpChannelStream.BeginRead(replyStateObject.ReplyBuffer, 0, Count, ReadCallBack, replyStateObject);
                }
                catch
                {
                    DeviceClose();
                    return false;
                }
                return true;
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
            byte[] byteBuffer = null;
            bool disconnect = false;
            lock (lockStream)
            {
                if (!TcpChannelIsConnected)
                {
                    doBeginDeviceRead = true;
#if DEBUG
                    //System.Diagnostics.Trace.TraceInformation(string.Format("TCP ReadCallback !connected do:{4} {0:00}:{1:00}:{2:00}.{3:000}",
                    //    DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond,
                    //    doBeginDeviceRead));
#endif
                    return;
                }

                TcpChannelReply replyStateObject = (TcpChannelReply)ar.AsyncState;
                NetworkStream replyNetworkStream = replyStateObject.ReplyStream;

                try
                {
                    int numberOfReadBytes = replyNetworkStream.EndRead(ar);
#if DEBUG
                    //System.Diagnostics.Trace.TraceInformation(string.Format("TCP ReadCallback EndRead byte:{4} {0:00}:{1:00}:{2:00}.{3:000}",
                    //    DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond,
                    //    numberOfReadBytes));
#endif
                    if (numberOfReadBytes > 0)
                    {
                        refChannel.IncrementLastRxByte(numberOfReadBytes);
                        inactivityTimer = DateTime.UtcNow;
                        
                        byteBuffer = new byte[numberOfReadBytes];
                        Array.Copy(replyStateObject.ReplyBuffer, 0, byteBuffer, 0, numberOfReadBytes);
                        
                    }
                    else
                    {
                        // client disconnection
                        disconnect = true;
                    }
                }                
                catch (System.Net.Sockets.SocketException e)
                {
                    replyStateObject.ExceptionObject = e;
                    // client forbicity disconnection
                    if (e.ErrorCode == WSAECONNRESET)
                        disconnect = true;
                }
                catch (Exception e)
                {
                    replyStateObject.ExceptionObject = e;
                }
                finally
                {
                    doBeginDeviceRead = true;
#if DEBUG
                    //System.Diagnostics.Trace.TraceInformation(string.Format("TCP ReadCallback finally do:{4} {0:00}:{1:00}:{2:00}.{3:000}",
                    //    DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond,
                    //    doBeginDeviceRead));
#endif
                }
            }
            if (byteBuffer != null)
            {
                refChannel.UpdateReceiveBuffer(TcpChannelClient, byteBuffer, DateTime.UtcNow, out bool setNewDataEvent, out int beginDeviceReadSize);
                if (autoRestartBeginDeviceRead)
                {
                    if (beginDeviceReadSize > 0)
                        BeginDeviceRead(beginDeviceReadSize);
                }
                if (setNewDataEvent)
                    refChannel.SetNewDataEvent();                
            }

            if (disconnect)
            {
                EventHandler<EventArgs> temp = ClientDisconnect;
                if (temp != null)
                    temp(this, new EventArgs());
            }
        }

        public void Dispose()
        {
            if (ChanneTimeOutSocket != null)
            {
                ChanneTimeOutSocket.TimeoutObject.Set();
                lock (lockStream)
                {
                    ChanneTimeOutSocket = null;
                }
            }
        }
        #endregion


    }
}
