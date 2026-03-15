////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	SerialChannel.cs
//
// summary:	Implements the serial channel class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using System.Runtime.InteropServices;

namespace SerialDriverCodeBase
{
    /// <summary>   base serial channel. </summary>
    public class SerialChannel : Channel, IDisposable
    {
        #region Data Members

        /// <summary>   The communications port. </summary>
        SerialPort commPort;
        /// <summary>   List of locks. </summary>
        protected object lockList = new object();

        
        #endregion

        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the channel object. </summary>
        ///
        /// <param name="commdriver" type="CommunicationDriver">    The commdriver. </param>
        /// <param name="settings" type="SerialChannelSettings">    Options for controlling the
        ///                                                         operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public SerialChannel(CommunicationDriver commdriver, SerialChannelSettings settings)
            : base(commdriver, settings, false)
        {
            //commPort = new SerialPort { /* default commPort settings*/PortName = "COM1", BaudRate = 115200, DataBits = 8, Parity = Parity.None, StopBits = StopBits.One, Handshake = Handshake.None, RtsEnable = false, DtrEnable = false, ReadTimeout = 5000, WriteTimeout = 5000 };
            try
            {
                string portName = string.Empty;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    portName = settings.CommPortName;
                else
                    portName = settings.CommPortNameLinux;

                commPort = new SerialPort { /* commPort settings */
                    PortName = portName, //settings.CommPortName,
                    BaudRate = settings.CommPortBaudRate,// >0 to1 the device limit
                    DataBits = settings.CommPortDataBits, //[5,8]
                    Parity = (Parity)settings.CommPortParity,
                    StopBits = (StopBits)settings.CommPortStopBits,//> None
                    Handshake = (Handshake)settings.CommPortHandshake,
                    RtsEnable = settings.CommPortRtsEnable,
                    DtrEnable = settings.CommPortDtrEnable,
                    ReadTimeout = settings.CommPortReadTimeout,
                    WriteTimeout = settings.CommPortWriteTimeout
                };
            }
            catch(Exception ex)
            { 
                CommDriver.OnSystemEvent(null, string.Format(Properties.Resources.SerialPortCreationError, Name, ex.Message), Opc.Ua.EventSeverity.High);
            }
        }
       
        #endregion

        #region Methods
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   RTS enable. </summary>
        ///
        /// <param name="Value" type="bool">    true to value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RtsEnable(bool Value)
        {
            if(commPort != null)
            commPort.RtsEnable = Value;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Dtr enable. </summary>
        ///
        /// <param name="Value" type="bool">    true to value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void DtrEnable(bool Value)
        {
            if (commPort != null)
            commPort.DtrEnable = Value;
        }

        #endregion

        #region Properties

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the read timeout. </summary>
        ///
        /// <value> The read timeout. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int ReadTimeout
        {
            get 
            {
                if (commPort != null)
                return commPort.ReadTimeout;
                return SerialPort.InfiniteTimeout;
            }
            set {
                if (commPort != null)
                commPort.ReadTimeout = value; 
            }
        }

        #endregion


        #region Virtual Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Error received. </summary>
        ///
        /// <param name="sender" type="object">                     Source of the event. </param>
        /// <param name="e" type="SerialErrorReceivedEventArgs">    Serial error received event
        ///                                                         information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            //SetInError(true, e.ToString());
            switch(e.EventType)
            {
                case SerialError.Frame:
                    LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorFrameError;
                    break;
                case SerialError.Overrun:
                    LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorOverrunError;
                    break;
                case SerialError.RXOver:
                    LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorRXOverError;
                    break;
                case SerialError.RXParity:
                    LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorRXParityError;
                    break;
                case SerialError.TXFull:
                    LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorTXFullError;
                    break;
                default:
                    LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorTimeOut;
                    break;
            }
            
            LastErrorTime = DateTime.UtcNow;
            SetNewDataEvent();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Data received. </summary>
        ///
        /// <param name="sender" type="object">                 Source of the event. </param>
        /// <param name="e" type="SerialDataReceivedEventArgs"> Serial data received event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (commPort == null)
                return;
            try
            {
                int Received = commPort.BytesToRead;
                if (Received > 0)
                {
                    byte[] buffer = new byte[Received];
                    Received = commPort.Read(buffer, 0, Received);
                    if (Received > 0)
                    {
                        lock (lockList)
                        {
                            ReceiveBuffer.AddRange(buffer);
                            if (StatisticsData != null)
                                lock (lockStatisic)
                                    DiagnLastTaskRxBytes += buffer.Count();
                        }
                        SetNewDataEvent();
                    }
                }
            }
            catch(Exception ex)
            { 
            }
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
            bool returnValue = (commPort != null && commPort.IsOpen);
            SetStateCommandVariableBit(!returnValue, (UInt16)ChannelVariableBits.ChannelUnconnected);
            return (returnValue);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a given device open. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceOpen()
        {            
            if (commPort != null )
            {
                if (commPort.IsOpen)
                    return true;

                commPort.ReceivedBytesThreshold = 1;
                commPort.DataReceived += DataReceived;
                commPort.ErrorReceived += ErrorReceived;
                try
                {
                    commPort.Open();
                }
                catch(Exception e)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Determines if we can device close. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceClose()
        {
            if (commPort != null)
                commPort.Close();
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Synchronous read. </summary>
        ///
        /// <param name="buffer">   . </param>
        /// <param name="count">    . </param>
        ///
        /// <returns>   if true the request byte size was read, othrewise false. </returns>
        ///
        /// ### <param name="Buffer">   buffer with read values. </param>
        /// ### <param name="Count">    number of bytes to read. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceRead(byte[] buffer, uint count)
        {
            if (commPort == null || !commPort.IsOpen)
                return false;

            Int32 byteReads = 0;
            bool bRet = false;
            try
            {
                byteReads = commPort.Read(buffer, 0, (int)count);
                bRet = true;
            }
            catch (Exception ex)
            {
                //SetInError(true, ex.Message);
            }

            //if (bRet)
            //    TotalRxBytes += (uint)byteReads;

            if (bRet)
                if (StatisticsData != null)
                    lock (lockStatisic)
                        DiagnLastTaskRxBytes += byteReads;

            return bRet;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// start send on commPort of "count" bytes from "buffer" if start send return true.
        /// </summary>
        ///
        /// <param name="buffer">   . </param>
        /// <param name="count">    . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ///
        /// ### <param name="Buffer">   buffer with values to write. </param>
        /// ### <param name="Count">    number of bytes to write. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceWrite(byte[] buffer, uint count)
        {
            if (commPort == null || !commPort.IsOpen)
                return false;

            bool bRet = false;
            try
            {
                commPort.Write(buffer, 0, (int)count);
                bRet = true;
            }
            catch (Exception ex)
            {
                //SetInError(true, ex.Message);
            }

            //if (bRet)
            //    TotalTxBytes += (uint)count;

            if (bRet)
                if (StatisticsData != null)
                    lock (lockStatisic)
                        DiagnLastTaskTxBytes += count;

            return bRet;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets bytes to read. </summary>
        ///
        /// <returns>   The bytes to read. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetBytesToRead()
        {
            uint count = 0;
            if (commPort == null || !commPort.IsOpen)
                return count;

            try
            {
                count = (uint)commPort.BytesToRead;
            }
            catch (Exception ex)
            {
                //SetInError(true, ex.Message);
            }

            return count;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets bytes to write. </summary>
        ///
        /// <returns>   The bytes to write. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetBytesToWrite()
        {
            uint count = 0;
            if (commPort == null || !commPort.IsOpen)
                return count;

            try
            {
                count = (uint)commPort.BytesToWrite;
            }
            catch (Exception ex)
            {
                //SetInError(true, ex.Message);
            }

            return count;
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
            DeviceClose();
            if (commPort != null)
            {
                commPort.Dispose();
            }

            base.Dispose();
        }
        #endregion

    }
}
