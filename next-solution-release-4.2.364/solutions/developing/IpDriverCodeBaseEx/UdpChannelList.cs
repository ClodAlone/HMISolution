using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;

namespace IpDriverCodeBaseEx
{
    public abstract class UdpChannelList : ChannelList, IDisposable
    {
        #region Data Members
        UDPManager SocketManager;
        #endregion

        #region Constructors
        public UdpChannelList(CommunicationDriver commdriver, UdpChannelSettings settings)
            : base(commdriver, settings)
        {
            SocketManager = new UDPManager(this, settings);
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
        /// <param name="RemoteEndPoint"> remote udp channel. </param>
        ///
        /// <returns>   if true the request byte size was read, othrewise false. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool DeviceRead(byte[] Buffer, uint Count, IPEndPoint RemoteEndPoint)
        {
            return SocketManager.DeviceRead(Buffer, Count, RemoteEndPoint);
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
            return SocketManager.DeviceWrite(Buffer, Count);
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
            return SocketManager.DeviceWrite(Buffer, Count, RemoteEndPoint);
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
        /// <param name="conn" type="DriverErrorCodes"> Communication error code. </param>
        /// <param name="pendingjob" type="CommJob">    The pendingjob. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool ProcessNewData(DriverErrorCodes conn, CommJob pendingjob)
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

        public string GetUdpChannelHostName()
        {
            return (SocketManager.UdpChannelHostName);
        }

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
