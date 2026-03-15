using System;
using DriverCodeBaseEx;
using System.Net;
using System.Threading;
using IpDriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using System.Runtime.InteropServices;

namespace BACnet
{
    #region  Class BacknetUdpChannel
    /// <summary>   An UDP channel. </summary>
    public class BacknetUdpChannel : Channel, IDisposable
    {
        #region Constants
        #endregion

        #region Data Members

        /// <summary>   The UDP channel client. </summary>
        UDPManagerBacNet _DoubleUDPManager;
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
        public BacknetUdpChannel(CommunicationDriver commdriver, BACnetChannelSettings settings)
            : base(commdriver, settings)
        {
            _DoubleUDPManager = new UDPManagerBacNet(this, settings);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                _DoubleUDPManager.AllowOtherBACnetClients = (commdriver as BACnetDriver).AllowOtherBACnetClients;
            else
                _DoubleUDPManager.AllowOtherBACnetClients = false;
        }

        //protected BackNetUdpChannel()
        //    : base()
        //{         
        //}

        //protected BackNetUdpChannel(CommunicationDriver commdriver)
        //    : base(commdriver)
        //{            
        //}
        #endregion

        #region Properties        
        #endregion

        #region Override Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a device is open. </summary>
        ///
        /// <returns>   true if a device is open, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool IsDeviceOpen()
        {
            return _DoubleUDPManager.IsDeviceOpen();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a given device open. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceOpen()
        {
            bool result = _DoubleUDPManager.DeviceOpen();
            if (result) {
                _DoubleUDPManager.BeginDeviceRead(0);
                _DoubleUDPManager.BeginDeviceRead_Ex(0);
            }
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Determines if we can device close. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceClose()
        {
            return _DoubleUDPManager.DeviceClose(bDisposed || CommDriver.bDisposed);
        }

        public override bool DeviceRead(byte[] Buffer, uint Count)
        {
            return _DoubleUDPManager.DeviceRead(Buffer, Count);
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
            return _DoubleUDPManager.DeviceWrite(Buffer, Count);
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
            return _DoubleUDPManager.DeviceWrite(Buffer, Count, RemoteEndPoint);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets bytes to read. </summary>
        ///
        /// <returns>   The bytes to read. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetBytesToRead()
        {
            return _DoubleUDPManager.GetBytesToRead();
        }

        //public ReceivedPacket GetReceivePacketQueue()
        //{
        //    return _DoubleUDPManager.GetReceivePacketQueue();
        //}

        //public int GetReceivePacketQueueCount()
        //{
        //    return _DoubleUDPManager.GetReceivePacketQueueCount();
        //}

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
        public override bool ProcessNewData(DriverErrorCodes conn, CommJob pendingjob)
        {
            return true;
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
        #endregion

        #region Methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Asynchronous reading. </summary>
        ///
        /// <param name="Count" type="int"> Number of. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool BeginDeviceRead(int Count)
        {
            return _DoubleUDPManager.BeginDeviceRead(Count);
        }

        public bool BeginDeviceRead_Ex(int Count)
        {
            return _DoubleUDPManager.BeginDeviceRead_Ex(Count);
        }
        //private void CallBackMethod(IAsyncResult asyncresult)
        //{
        //    try
        //    {
        //        IsConnectionSuccessful = false;
        //        TcpClient tcpclient = asyncresult.AsyncState as TcpClient;

        //        if (tcpclient.Client != null)
        //        {
        //            tcpclient.EndConnect(asyncresult);
        //            IsConnectionSuccessful = true;
        //        }
        //        else
        //        {
        //            socketexception = new TimeoutException("TimeOut Exception");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        IsConnectionSuccessful = false;
        //        socketexception = ex;
        //    }
        //    finally
        //    {
        //        TimeoutObject.Set();
        //    }
        //}
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
            base.Dispose();
            if (_DoubleUDPManager != null)
                _DoubleUDPManager.Dispose();            
        }
        #endregion
    }
    #endregion
}
