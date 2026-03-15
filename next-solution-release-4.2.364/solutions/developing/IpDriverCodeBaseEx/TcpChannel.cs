////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	tcpchannel.cs
//
// summary:	Implements the tcpchannel class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using System.Net.Sockets;
using System.Threading;
using DriverCodeBaseEx.Enumerators;
using System.Net;
using System.Net.NetworkInformation;

namespace IpDriverCodeBaseEx
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
        public Exception ExceptionObject
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
        #region Data Members
        protected TCPManager SocketManager;
        #endregion

        #region Constructors
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the TcpChannel object. </summary>
        ///
        /// <param name="commdriver" type="CommunicationDriver">    The commdriver. </param>
        /// <param name="settings" type="TcpChannelSettings">       Options for controlling the
        ///                                                         operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public TcpChannel(CommunicationDriver commdriver, TcpChannelSettings settings, TcpClient connectedClient = null, bool autoRestartBeginDeviceRead = false)
            : base(commdriver, settings/*, listExec*/)
        {
            SocketManager = new TCPManager(this, settings, connectedClient, autoRestartBeginDeviceRead);
        }
        #endregion

        #region Override Methods
        protected override void StopTimers(bool bTerminate = true)
        {
            if (bTerminate)
                SocketManager.Dispose();
            base.StopTimers(bTerminate);
        }

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
            return SocketManager.IsDeviceOpen();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a given device open. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceOpen()
        {
            return SocketManager.DeviceOpen();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Determines if we can device close. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceClose()
        {
            return SocketManager.DeviceClose();
        }

        /// <summary>   Flushes this object. </summary>
        public bool Flush()
        {
            return SocketManager.Flush();
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
            return SocketManager.DeviceRead(Buffer, Count);
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
            return SocketManager.DeviceReadSynchronous(Buffer, Count);
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
            return SocketManager.DeviceWrite(Buffer, Count);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets bytes to read. </summary>
        ///
        /// <returns>   The bytes to read. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetBytesToRead()
        {
            return SocketManager.GetBytesToRead();
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
        /// <param name="conn" type="DriverErrorCodes"> Communication error code. </param>
        /// <param name="pendingjob" type="CommJob">    The pendingjob. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool ProcessNewData(DriverErrorCodes conn, CommJob pendingjob)
        {
            return false;
        }
        #endregion

        #region Virtual Methods

        //bool doBeginDeviceRead = true;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Asynchronous reading. </summary>
        ///
        /// <param name="Count" type="int"> Number of. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool BeginDeviceRead(int Count)
        {
            return SocketManager.BeginDeviceRead(Count);
        }

        public override bool Startup()
        {
            if (!bChannelStarted)
            {
                lock (lockThreadObject)
                {
                    if (NewDataToAnlyze == null)
                        NewDataToAnlyze = new ManualResetEvent(false);
                    else
                        NewDataToAnlyze.Reset();
                }
            }
            return base.Startup();
        }

        public virtual bool Connected(uint maxInactivitySec = 0)
        {
            return SocketManager.Connected(maxInactivitySec);
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
            StopTimers();
                        
            SocketManager.Dispose();

            DeviceClose();
            base.Dispose();
        }
        #endregion
    }
    #endregion
}
