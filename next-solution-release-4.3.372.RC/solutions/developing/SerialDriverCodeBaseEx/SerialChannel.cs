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
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace SerialDriverCodeBaseEx
{
    /// <summary>   base serial channel. </summary>
    public class SerialChannel : Channel, IDisposable
    {
        #region Data Members

        /// <summary>   The communications port. </summary>
        SerialPort commPort;
        /// <summary>   List of locks. </summary>
        //protected object lockList = new object();
        readonly List<byte> serialReceiveBuffer = new List<byte>();

        private System.Threading.Thread DataReception = null;
        private BlockingCollection<byte> queueReceptionByte = new BlockingCollection<byte>();

        public const byte RECEIVED_NEW_DATA = 0x02;
        public const byte STOP_RECEIVING = 0x1B;


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
            : base(commdriver, settings)
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
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //public virtual void ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        //{
        //    //SetInError(true, e.ToString());
        //    switch (e.EventType)
        //    {
        //        case SerialError.Frame:
        //            LastErrorCode = DriverErrorCodes.ErrorFrameError;
        //            break;
        //        case SerialError.Overrun:
        //            LastErrorCode = DriverErrorCodes.ErrorOverrunError;
        //            break;
        //        case SerialError.RXOver:
        //            LastErrorCode = DriverErrorCodes.ErrorRXOverError;
        //            break;
        //        case SerialError.RXParity:
        //            LastErrorCode = DriverErrorCodes.ErrorRXParityError;
        //            break;
        //        case SerialError.TXFull:
        //            LastErrorCode = DriverErrorCodes.ErrorTXFullError;
        //            break;
        //        default:
        //            LastErrorCode = DriverErrorCodes.ErrorTimeOut;
        //            break;
        //    }

        //    LastErrorTime = DateTime.UtcNow;
        //    SetNewDataEvent();
        //}

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
                    //System.Diagnostics.Debug.WriteLine("@@@ DataReceived byte recieved = " + $"{Received} time =" + $"{ DateTime.Now:hh:mm:ss:mmm.fff}");
                    byte[] buffer = new byte[Received];
                    Received = commPort.Read(buffer, 0, Received);
                    if (Received > 0)
                    {
                        lock (serialReceiveBuffer)
                        {                            
                            //bool bStartTask = serialReceiveBuffer.Count == 0;
                            serialReceiveBuffer.AddRange(buffer);
                        }
                        //I send the message to the thread ThreadDataReception that the serial has received some bytes  
                        queueReceptionByte.Add(RECEIVED_NEW_DATA);                        
                    }
                }
            }
            catch(Exception ex)
            { 
            }
        }

        private void ThreadDataReception()
        {
            Thread.CurrentThread.Name = "ThreadDataReception";
            object item;
            var tempBuffer = new List<byte>();
                    
            while (true)
            {
                item = queueReceptionByte.Take();
                if (((byte)item) == STOP_RECEIVING)
                {
                    break;
                }
                else
                {
                    lock (serialReceiveBuffer)
                    {
                        tempBuffer.AddRange(serialReceiveBuffer);
                        serialReceiveBuffer.Clear();
                    }

                    IncrementLastRxByte(tempBuffer.Count);
                    lock (lockThreadObject)
                    {
                        UpdateReceiveBuffer(commPort, tempBuffer.ToArray(), DateTime.UtcNow, out bool setNewDataEvent, out int beginDeviceReadSize);
                        if (setNewDataEvent)
                            SetNewDataEvent();
                    }
                    tempBuffer.Clear();
                }
            }
            lock (serialReceiveBuffer)
            {
                serialReceiveBuffer.Clear();
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
            bool isOpen = false;
            if (commPort != null )
            {
                if (commPort.IsOpen)
                {
                    isOpen = true;
                }
                else
                {
                    commPort.ReceivedBytesThreshold = 1;
                    commPort.DataReceived += DataReceived;
                    //commPort.ErrorReceived += ErrorReceived;
                    try
                    {
                        commPort.Open();
                        if (commPort.IsOpen && (DataReception == null))
                        {
                            isOpen = true;
                            DataReception = new Thread(ThreadDataReception);
                            DataReception.IsBackground = true;
                            DataReception.Start();
                        }
                    }
                    catch (Exception e)
                    {
                        commPort.DataReceived -= DataReceived;
                        //commPort.ErrorReceived -= ErrorReceived;
                        isOpen = false;
                    }
                }
            }

            SetStateCommandVariableBit(!isOpen, (UInt16)ChannelVariableBits.ChannelUnconnected);

            return isOpen;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Determines if we can device close. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceClose()
        {
            if (commPort != null)
            {
                try
                {
                    commPort.DataReceived -= DataReceived;
                    //commPort.ErrorReceived -= ErrorReceived;
                    commPort.Close();
                }
                catch (Exception ex) 
                { }
                //I send the exit message to the thread ThreadDataReception
                //I await the end of the thread 
                if(DataReception != null)
                {
                    queueReceptionByte.Add(STOP_RECEIVING);
                    DataReception.Join();
                    DataReception = null;
                }
                //clearing the collection
                if (queueReceptionByte.Count > 0)
                { 
                    while (queueReceptionByte.Count > 0)
                    {
                        var obj = queueReceptionByte.Take();
                    }
                }       
            }

            SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
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
            base.Dispose();
            
            DeviceClose();

            if (commPort != null)
                commPort.Dispose();
            
            queueReceptionByte.Dispose();            
        }
        #endregion

    }
}
