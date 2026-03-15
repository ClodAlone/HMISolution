////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	tcpchannel.cs
//
// summary:	Implements the tcpchannel class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using System.Net.Sockets;
using System.Threading;
using DriverCodeBase.Enumerators;
using System.Net;
using System.Net.NetworkInformation;

namespace IpDriverCodeBase
{
    #region Class IpTcpClient
    class IpTcpClient : TcpClient
    {
        public IpTcpClient(string inServerip, int inServerport)
        {
            Serverip = inServerip;
            serverport = inServerport;
        }
        public string Serverip;
        public int serverport;

    }
    #endregion
    #region Class TimeOutSocket
    class TimeOutSocket
    {
        private bool IsConnectionSuccessful = false;
        private Exception socketexception;
        public ManualResetEvent TimeoutObject = new ManualResetEvent(false);

        public TcpClient Connect(string inServerip, int serverport, int timeoutMSec)
        {
            string serverip = inServerip.Trim();
            TimeoutObject.Reset();
            socketexception = null;
            TcpClient tcpclient = new TcpClient();

            tcpclient.BeginConnect(serverip, serverport,
                new AsyncCallback(CallBackMethod), tcpclient);

            if (TimeoutObject.WaitOne(timeoutMSec, false))
            {
                if (IsConnectionSuccessful)
                {
                    return tcpclient;
                }
                else
                {
                    throw socketexception;
                }
            }
            else
            {
                tcpclient.Close();
                throw new TimeoutException("TimeOut Exception");
            }
        }
        private void CallBackMethod(IAsyncResult asyncresult)
        {
            try
            {
                IsConnectionSuccessful = false;
                TcpClient tcpclient = asyncresult.AsyncState as TcpClient;

                if (tcpclient.Client != null)
                {
                    tcpclient.EndConnect(asyncresult);
                    IsConnectionSuccessful = true;
                }
                else
                {
                    socketexception = new TimeoutException("TimeOut Exception");
                }
            }
            catch (Exception ex)
            {
                IsConnectionSuccessful = false;
                socketexception = ex;
            }
            finally
            {
                TimeoutObject.Set();
            }
        }
    }
    #endregion

    #region Class TcpChannelReply
    /// <summary>   A TCP channel reply. </summary>
    public class TcpChannelReply
    {
        #region Data Members
        /// <summary>   Buffer for reply data. </summary>
        public byte[] ReplyBuffer;
        /// <summary>   The offset. </summary>
        private int _offset;
        /// <summary>   The size. </summary>
        private int _size;
        /// <summary>   The exception object. </summary>
        private /*Socket*/Exception _ExceptionObject;
        /// <summary>   The reply stream. </summary>
        private NetworkStream _ReplyStream;
        #endregion

        #region Constructors
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the TcpChannelReply object. </summary>
        ///
        /// <param name="bufferOffset" type="int">  The buffer offset. </param>
        /// <param name="bufferSize" type="int">    Size of the buffer. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public TcpChannelReply(int bufferOffset, int bufferSize)
        {
            ReplyBuffer = new byte[bufferSize];
            _offset = bufferOffset;
            _size = bufferSize;
        }
        #endregion

        #region Properties
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Socket exception. </summary>
        ///
        /// <value> The exception object. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public /*Socket*/Exception ExceptionObject
        {
            get
            {
                return (_ExceptionObject);
            }
 
            set
            {
                _ExceptionObject = value;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Network stream. </summary>
        ///
        /// <value> The reply stream. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public NetworkStream ReplyStream
        {
            get
            {
                return (_ReplyStream);
            }

            set
            {
                _ReplyStream = value;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Offset. </summary>
        ///
        /// <value> The offset. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int offset
        {
            get
            {
                return (_offset);
            }

            set
            {
                _offset = value;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Size. </summary>
        ///
        /// <value> The size. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int size
        {
            get
            {
                return (_size);
            }

            set
            {
                _size = value;
            }
        }
        #endregion
    }
    #endregion

    #region Class TcpChannel

    /// <summary>   base Tcp channel. </summary>
    public class TcpChannel : Channel, IDisposable
    {
        #region Constants
        public const int testSocketDelay_S = 10;
        public const int testSocketTimeOut_S = 5;
        #endregion
        #region Data Members

        /// <summary>   The TCP channel client. </summary>
        TcpClient TcpChannelClient;
        /// <summary>   true if TCP channel is connected. </summary>
        bool TcpChannelIsConnected;
        /// <summary>   The TCP channel stream. </summary>
        NetworkStream TcpChannelStream;
        /// <summary>   List of locks. </summary>
        protected object lockList = new object();
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

        bool doBeginDeviceRead = true;

        #endregion

        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the TcpChannel object. </summary>
        ///
        /// <param name="commdriver" type="CommunicationDriver">    The commdriver. </param>
        /// <param name="settings" type="TcpChannelSettings">       Options for controlling the
        ///                                                         operation. </param>
        /// <param name="multipoint" type="bool">                   true to multipoint. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public TcpChannel(CommunicationDriver commdriver, TcpChannelSettings settings, bool multipoint, TcpClient connectedClient = null)
            : base(commdriver, settings, multipoint)
        {
            _TcpChannelReadTimeout = Properties.Settings.Default.TcpChannelSettingsReadTimeout;
            _TcpChannelWriteTimeout = Properties.Settings.Default.TcpChannelSettingsWriteTimeout;
            selectedHostName = TcpChannelHostName;
            if (connectedClient != null)
            {
                TcpChannelClient = connectedClient;
                TcpChannelStream = TcpChannelClient.GetStream();
                TcpChannelStream.ReadTimeout = TcpChannelReadTimeout;
                TcpChannelStream.WriteTimeout = TcpChannelWriteTimeout;
                TcpChannelIsConnected = true;
                CurrentErrorCode = 0;
                ChanneTimeOutSocket = null;
                inactivityTimer = DateTime.Now;
                testSocketTimer = DateTime.Now;
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
                                        inactivityTimer = DateTime.Now;
                                    else if ((DateTime.Now - inactivityTimer).TotalSeconds > maxInactivitySec)
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

        #endregion

        #region Override Methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Tests channel communications. </summary>
        ///
        /// <returns>   true if the test passes, false if the test fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool TestChannelComm()
        {
            return DeviceOpen();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a device is open. </summary>
        ///
        /// <returns>   true if a device is open, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool IsDeviceOpen()
        {
            lock (lockStream)
            {
                bool rt = ((TcpChannelClient != null) && (TcpChannelStream != null) && TcpChannelIsConnected);
                if (!String.IsNullOrEmpty(TcpChannelBackupHostName))
                {
                    switchHost = false;
                    if((GetStateCommandVariableBit(ref switchHost, (UInt16)ChannelVariableBits.SwitchServer) == true) && (switchHost == true))
                    {
                        return (false);
                    }
                    if (!isFirstOpen && rt && (DateTime.Now - testSocketTimer).TotalSeconds > testSocketDelay_S)
                    {
                        try
                        {
                            TestSocketStart(selectedHostName == TcpChannelHostName ? TcpChannelBackupHostName : TcpChannelHostName, TcpChannelHostPort);
                        }
                        catch (Exception ex)
                        {
                            if(selectedHostName == TcpChannelHostName)
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

                SetStateCommandVariableBit(!rt, (UInt16)ChannelVariableBits.ChannelUnconnected);
                SetStateCommandVariableBit(PrimaryHostErrorState, (UInt16)ChannelVariableBits.PrimaryHostErrorState);
                SetStateCommandVariableBit(BackupHostErrorState, (UInt16)ChannelVariableBits.BackupHostErrorState);
                SetStateCommandVariableBit(selectedHostName != TcpChannelHostName, (UInt16)ChannelVariableBits.ConnectedHost);
                return rt;
            }
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

                testSocketTimer = DateTime.Now;
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
                testSocketTimer = DateTime.Now;
                testSocketPending = false;
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

                if ((!TcpChannelIsConnected || (switchHost == true)) && (ChanneTimeOutSocket != null))
                {
                    if (isFirstOpen)
                    {
                        isFirstOpen = false;
                        inactivityTimer = DateTime.Now;
                        testSocketTimer = DateTime.Now;
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
                            if (!String.IsNullOrWhiteSpace(TcpChannelBackupHostName) &&
                                (((DateTime.Now - inactivityTimer).TotalMilliseconds > TcpChannelSwitchHostTimeout) || (switchHost == true)))
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
                                CommDriver.OnSystemEvent(null,
                                        string.Format(IpDriverCodeBase.Properties.Resources.SwitchChannelHostName,
                                            Name, oldHostName, selectedHostName, switchHost), Opc.Ua.EventSeverity.Low);
                            }
                            DeviceClose();
                            TcpChannelClient = ChanneTimeOutSocket.Connect(selectedHostName, TcpChannelHostPort, Timeout);
                            TcpChannelClient.NoDelay = Properties.Settings.Default.NoDelay;
                            TcpChannelStream = TcpChannelClient.GetStream();
                            TcpChannelStream.ReadTimeout = TcpChannelReadTimeout;
                            TcpChannelStream.WriteTimeout = TcpChannelWriteTimeout;
                            TcpChannelIsConnected = true;
                            CurrentErrorCode = 0;
                            testSocketTimer = DateTime.Now;
                            inactivityTimer = DateTime.Now;
                            if (selectedHostName == TcpChannelHostName)
                                PrimaryHostErrorState = false;
                            else
                                BackupHostErrorState = false;
                        }
                    }
                    catch (Exception e)
                    {
                        SocketException s = e as SocketException;
                        if (s != null && CurrentErrorCode != s.ErrorCode)
                        {
                            int nPort = 0;
                            if(TcpChannelClient != null && TcpChannelClient.Client != null && TcpChannelClient.Client.LocalEndPoint != null)
                            {
                                nPort = ((IPEndPoint)TcpChannelClient.Client.LocalEndPoint).Port;
                            }
                            //log error...
                            CurrentErrorCode = s.ErrorCode;
                            CommDriver.OnSystemEvent(null, string.Format(IpDriverCodeBase.Properties.Resources.SocketError, 
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

            SetStateCommandVariableBit(PrimaryHostErrorState, (UInt16)ChannelVariableBits.PrimaryHostErrorState);
            SetStateCommandVariableBit(BackupHostErrorState, (UInt16)ChannelVariableBits.BackupHostErrorState);
            SetStateCommandVariableBit(selectedHostName != TcpChannelHostName, (UInt16)ChannelVariableBits.ConnectedHost);
            SetStateCommandVariableBit(!TcpChannelIsConnected, (UInt16)ChannelVariableBits.ChannelUnconnected);
            if (switchHost == true)
            {
                switchHost = false;
                SetStateCommandVariableBit(false, (UInt16)ChannelVariableBits.SwitchServer);
            }
            doBeginDeviceRead = TcpChannelIsConnected;
            return TcpChannelIsConnected;
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
                
            }

            IsDeviceOpen();
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
                    return(ret);
                }
                //int size = 100;
                //byte[] Buffer = new byte[size];
                //while (TcpChannelStream.DataAvailable)
                //    TcpChannelStream.Read(Buffer, 0, size);
                if(TcpChannelStream.CanRead)
                {
                    int nRead = 0;
                    int size = 100;
                    byte[] Buffer = new byte[size];
                    //string prova = ((IPEndPoint)TcpChannelClient.Client.LocalEndPoint).Port.ToString();
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
                                errorMessage = string.Format(IpDriverCodeBase.Properties.Resources.SocketError,
                                    e.InnerException.Message, e.InnerException, ((IPEndPoint)TcpChannelClient.Client.LocalEndPoint).Port.ToString()) ;
                            }
                            else
                            {
                                errorMessage = string.Format(IpDriverCodeBase.Properties.Resources.SocketError, e.Message, e, 
                                                             ((IPEndPoint)TcpChannelClient.Client.LocalEndPoint).Port.ToString());                           
                            }

                            CommDriver.OnSystemEvent(null, errorMessage, Opc.Ua.EventSeverity.High);
                            ret = false;
                            break;
                        }
                    }
                    if (nRead > 0)
                        inactivityTimer = DateTime.Now;
                }        
            }
            return (ret);
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

                DateTime begin = DateTime.Now;
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

                    /*if (!TcpChannelStream.DataAvailable)
                        return false;*/

                    int readCount = TcpChannelStream.Read(Buffer, 0, size);
                    if (readCount != size)
                    {
                        return (false);
                    }
                    inactivityTimer = DateTime.Now;
                    if (StatisticsData != null)
                        lock (lockStatisic)
                            DiagnLastTaskRxBytes += readCount;
                }
                catch (Exception e)
                {
                    DeviceClose();
                    return false;
                }

                return (true);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Synchronous read. </summary>
        ///
        /// <param name="Buffer">   buffer with read values. </param>
        /// <param name="Count">    number of bytes to read. </param>
        ///
        /// <returns>   the number of read bytes. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint DeviceReadSynchronous(byte[] Buffer, uint Count)
        {
            lock (lockStream)
            {
      
                //DateTime begin = DateTime.Now;
                int readCount = 0;
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

                    /*if (!TcpChannelStream.DataAvailable)
                        return false;*/

                    readCount = TcpChannelStream.Read(Buffer, 0, size);
                    //if (readCount != size)
                    //{
                    //    return (false);
                    //}

                    inactivityTimer = DateTime.Now;
                    if (StatisticsData != null)
                        lock (lockStatisic)
                            DiagnLastTaskRxBytes += readCount;
                }
                catch (Exception e)
                {
                    DeviceClose();
                    return (0);
                }

                return ((uint)readCount);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// start send on commPort of "count" bytes from "buffer" if start send return true.
        /// </summary>
        ///
        /// <param name="Buffer">   buffer with values to write. </param>
        /// <param name="Count">    number of bytes to write. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ///
        /// ### <param name="buffer">   . </param>
        /// ### <param name="count">    . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceWrite(byte[] Buffer, uint Count)
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

                inactivityTimer = DateTime.Now;
                if (StatisticsData != null)
                    lock (lockStatisic)
                        DiagnLastTaskTxBytes += size;

                return (true);
            }
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

         ////////////////////////////////////////////////////////////////////////////////////////////////////
         /// <summary>  Gets bytes to write. </summary>
         ///
         /// <returns>  The bytes to write. </returns>
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
        /*
        protected override void WorkingThread(object data)
        {
            int sleepCycle = (Int32)data;
            if (sleepCycle == 0)
                sleepCycle = 1;
            ListJobPending.Clear();
            ListJobExecuted.Clear();

            bool bForceProcessListJobs = false;
            LastScheduleTimeJobsList = DateTime.UtcNow;
            int loop = 0;
            while (true)
            {
                lock (lockThreadObject)
                {
                    bForceProcessListJobs = ProcessListJobs.WaitOne(0);
                    ProcessListJobs.Reset();
                }

                if (bForceProcessListJobs || (DateTime.UtcNow - LastScheduleTimeJobsList).TotalMilliseconds > ScheduleTimeJobsList)
                {
                    LastScheduleTimeJobsList = DateTime.UtcNow;
                    bForceProcessListJobs = true;
                }

                ScheduleListJob(bForceProcessListJobs);

                CommJob nextjob = null;
                if (ListJobPending.Count == 0 || MultiPointProtocol)
                {
                    nextjob = GetNextPendingJob();
                    if (nextjob != null)
                        ListJobPending.Add(nextjob);
                }


                if (nextjob != null)
                {
                    if (!IsDeviceOpen())
                        DeviceOpen();

                    ExecuteJob(nextjob);
                    {
                        if (NewDataToAnlyze.WaitOne(Timeout))
                        {
                            NewDataToAnlyze.Reset();
                            if (ListJobPending.Count > 0)
                            {
                                foreach (var job in ListJobPending)
                                {
                                    if (ProcessNewData(job))
                                        ListJobExecuted.Add(job);
                                }

                                foreach (var job in ListJobExecuted)
                                {
                                    job.LastExecutionTime = DateTime.UtcNow;
                                    ListJobPending.Remove(job);
                                }
                                ListJobExecuted.Clear();

                            }
                        }
                        else
                        {
                            LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorTimeOut;
                            ProcessNewData(ListJobPending[0]);
                            ListJobPending.RemoveAt(0);
                            ReceiveBuffer.Clear();
                        }

                        if (ListJobPending.Count > 0)
                        {
                            if (!MultiPointProtocol)
                            {
                                double dtime = (DateTime.UtcNow - ListJobPending[0].StartExecutionTime).TotalMilliseconds;
                                if (dtime > Timeout)
                                {
                                    //error
                                    LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorTimeOut;
                                    ProcessNewData(ListJobPending[0]);
                                    ListJobPending.RemoveAt(0);
                                    ReceiveBuffer.Clear();
                                }
                            }
                        }
                    }

                }
                if (ListJobPending.Count == 0 && !KeepOpened && IsDeviceOpen())
                    DeviceClose();

                if (nextjob != null && StopWorkerThread.WaitOne(WaitTime, false))
                    break;
                else if (++loop > 4)
                {
                    loop = 0;
                    if (StopWorkerThread.WaitOne(sleepCycle, false))
                        break;
                }
                StopWorkerThread.WaitOne(0, false);
            }
        }
        */
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Asynchronous reading. </summary>
        ///
        /// <param name="Count" type="int"> Number of. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool BeginDeviceRead(int Count)
        {
            lock (lockStream)
            {
                try
                {
                    if (!doBeginDeviceRead)
                    {
                        return true;
                    }
                    doBeginDeviceRead = false;
                    TcpChannelReply replyStateObject = new TcpChannelReply(0, Count);
                    replyStateObject.ReplyStream = TcpChannelStream;
                    TcpChannelStream.BeginRead(replyStateObject.ReplyBuffer, 0, Count, ReadCallBack, replyStateObject);
                }
                catch
                {
                    doBeginDeviceRead = true;
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
            List<byte> rec = null;

            lock (lockStream) 
            {

                if (!TcpChannelIsConnected)
                {
                    doBeginDeviceRead = true;
                    return;
                }
                TcpChannelReply replyStateObject = (TcpChannelReply)ar.AsyncState;
                NetworkStream replyNetworkStream = replyStateObject.ReplyStream;

                try
                {
                    int numberOfReadBytes = replyNetworkStream.EndRead(ar);
                    if(numberOfReadBytes > 0)
                    {
                        rec = new List<byte>();
                        byte[] byteBuffer = new byte[numberOfReadBytes];
                        Array.Copy(replyStateObject.ReplyBuffer, 0, byteBuffer, 0, numberOfReadBytes);
                        rec.AddRange(byteBuffer);
                        //lock (lockThreadObject)//steve 031011
                        //{
                        //    ReceiveBuffer.AddRange(replyStateObject.ReplyBuffer);

                        //    SetNewDataEvent();
                        //}

                        if (StatisticsData != null)
                        {
                            lock (lockStatisic)
                            {
                                DiagnLastTaskRxBytes += numberOfReadBytes;
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                     replyStateObject.ExceptionObject = e;
                }
                finally
                {
                    doBeginDeviceRead = true;
                }
            }

            if (rec != null)
            {
                lock (lockThreadObject)//steve 031011
                {
                    ReceiveBuffer.AddRange(rec);
                    inactivityTimer = DateTime.Now;
                    SetNewDataEvent();
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
            if (ChanneTimeOutSocket != null)
            {
                ChanneTimeOutSocket.TimeoutObject.Set();
                lock (lockStream)
                {
                    ChanneTimeOutSocket = null;
                }
            }
            base.Dispose();
            DeviceClose();            
        }
        #endregion
    }
    #endregion
}
