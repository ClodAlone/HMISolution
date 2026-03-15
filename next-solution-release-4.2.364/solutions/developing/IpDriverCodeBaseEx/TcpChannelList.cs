using DriverCodeBaseEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using DriverCodeBaseEx.Enumerators;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace IpDriverCodeBaseEx
{
    /// <summary> Tcp communication channel for communication drivers with dynamic aggregation. </summary>
    public abstract class TcpChannelList : ChannelList, IDisposable
    {
        #region Data Members
        TCPManager SocketManager;
        #endregion

        #region Constructors
        public TcpChannelList(CommunicationDriver commdriver, TcpChannelSettings settings, TcpClient connectedClient = null)
            : base(commdriver, settings)
        {
            SocketManager = new TCPManager(this, settings, connectedClient);
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
}
