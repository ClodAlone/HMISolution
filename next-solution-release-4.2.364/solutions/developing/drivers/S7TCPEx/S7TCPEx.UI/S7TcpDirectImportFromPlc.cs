using Accon.AGLink;
using DriverCodeBaseEx;
using Opc.Ua;
using System;

namespace S7TCP.UI
{

    public class S7TcpDirectImportFromPlc : IDisposable
    {
        private enum State
        {
            eNotOpen = 0,
            eDevOpened,
            eDialedUp,
            eInitAdapter,
            eConnected,
            eNotInit,
        }

        private static Int32 devNrCnt = 0;
        public Int32 devNr;

        private State conState = State.eNotInit;
        public Int32 connNr = -1;
        private Int32 timeout = 1;
        private string MLFBNr;
        private string HostName;
        private ushort HostPort = 0;
        protected object lockStream;
        private string szStationName;
        private int nErrorLibraryCode = AGL4.AGL40_SUCCESS;
        bool lastConnectionError = false;

        CommunicationDriver CommDriver;
        S7TCPStation _station = null;
        S7TCPChannel _channel = null;


        #region CTor
        public S7TcpDirectImportFromPlc(S7TCPDriver driver, S7TCPChannel channel, S7TCPStation station)
        {
            CommDriver = driver;
            _station = station;
            _channel = channel;

            devNr = getDevNr;
            conState = State.eNotInit;
            lockStream = new object();
            MLFBNr = "---";
            HostName = "";
        }
        #endregion


        /// <summary>
        /// Verbindung über TCP/IP
        /// </summary>
        private bool SetParasTcpIP()
        {
            AGL4.S7tcpipTia para = new AGL4.S7tcpipTia();

            para.Conn[0].Address = HostName;
            para.Conn[0].ConnTypeEx = AGL4.ConnTypeEx.eCT_HMI;
            para.Conn[0].PLCClassEx = AGL4.PLC_ClassEx.ePLCEx_AUTO_TIA;
            para.Conn[0].TimeOut = timeout;
            para.Conn[0].PlcNr = 1;
            para.Conn[0].PortNr = HostPort;
            para.Conn[0].OwnAddress = 0;
            para.Conn[0].OwnPortNr = 0;

            if ((nErrorLibraryCode = AGL4.SetDevType(devNr, AGL4.TYPE_S7_TCPIP_TIA)) != AGL4.AGL40_SUCCESS)
            {
                return false;
            }
            if ((nErrorLibraryCode = AGL4.SetParas(devNr, AGL4.TYPE_S7_TCPIP_TIA, (object)para)) != AGL4.AGL40_SUCCESS)
            {
                return false;
            }
            return true;
        }

        public bool Init(string HostName, ushort HostPort, Int32 timeout, CommunicationDriver CommDriver, string stationName)
        {
            this.HostName = HostName;
            this.HostPort = HostPort;
            this.timeout = timeout;
            this.CommDriver = CommDriver;
            this.szStationName = stationName;

            return true;
        }

        public bool Connect()
        {
            Int32 RetVal = 0;

            lock (lockStream)
            {
                if (conState == State.eNotInit)
                {
                    if (string.IsNullOrWhiteSpace(szStationName))
                        return false;

                    conState = State.eNotOpen;
                }

                if (conState == State.eNotOpen)
                {
                    if (!SetParasTcpIP())
                    {
                        return false;
                    }

                    RetVal = AGL4.OpenDevice(devNr);
                    if (RetVal == AGL4.AGL40_SUCCESS)
                    {
                        conState = State.eDevOpened;
                    }
                    else
                    {
                        nErrorLibraryCode = RetVal;
                    }
                }

                if (conState == State.eDevOpened)
                {
                    RetVal = AGL4.DialUp(devNr, timeout);
                    if (RetVal == AGL4.AGL40_SUCCESS)
                    {
                        conState = State.eDialedUp;
                    }
                    else
                    {
                        nErrorLibraryCode = RetVal;
                    }
                }

                if (conState == State.eDialedUp)
                {
                    RetVal = AGL4.InitAdapter(devNr, timeout);
                    if (RetVal == AGL4.AGL40_SUCCESS)
                    {
                        conState = State.eInitAdapter;
                    }
                    else
                    {
                        nErrorLibraryCode = RetVal;
                    }
                }

                if (conState == State.eInitAdapter)
                {
                    RetVal = AGL4.PLCConnect(devNr, 1, out connNr, timeout);
                    if (RetVal == AGL4.AGL40_SUCCESS)
                    {
                        conState = State.eConnected;
                    }
                    else
                    {
                        nErrorLibraryCode = RetVal;
                    }
                }
            }

            if (IsConnected())
            {
                nErrorLibraryCode = AGL4.ReadMLFBNr(connNr, out MLFBNr, timeout);
                CommDriver.OnSystemEvent(null, Properties.Resources.ConnectionEstablished, EventSeverity.High);
                return true;
            }
            else
            {
                Disconnect();
                return false;
            }
        }

        public bool Disconnect()
        {
            lock (lockStream)
            {
                if (conState == State.eNotInit)
                    return false;
                Int32 _RetVal = 0;
                if (conState == State.eConnected)
                {
                    _RetVal = AGL4.PLCDisconnect(connNr, timeout);
                    if ((_RetVal != AGL4.AGL40_CONNECTION_CLOSED) &&
                         (_RetVal != AGL4.AGL40_NOT_CONNECTED) &&
                         (_RetVal != AGL4.AGL40_SUCCESS))
                    {
                        nErrorLibraryCode = _RetVal;
                    }
                    conState = State.eInitAdapter;
                }
                if (conState == State.eInitAdapter)
                {
                    _RetVal = AGL4.ExitAdapter(devNr, timeout);
                    if (_RetVal != AGL4.AGL40_SUCCESS)
                    {
                        nErrorLibraryCode = _RetVal;
                    }
                    conState = State.eDialedUp;
                }
                if (conState == State.eDialedUp)
                {
                    _RetVal = AGL4.HangUp(devNr, timeout);
                    if (_RetVal != AGL4.AGL40_SUCCESS)
                    {
                        nErrorLibraryCode = _RetVal;
                    }
                    conState = State.eDevOpened;
                }
                if (conState == State.eDevOpened)
                {
                    _RetVal = AGL4.CloseDevice(devNr);
                    if (_RetVal != AGL4.AGL40_SUCCESS)
                    {
                        nErrorLibraryCode = _RetVal;
                    }
                    conState = State.eNotOpen;
                }

                return _RetVal == AGL4.AGL40_SUCCESS;
            }
        }

        public bool IsConnected()
        {
            lock (lockStream)
            {
                return conState == State.eConnected;
            }
        }

        public bool IsInit()
        {
            return conState != State.eNotInit;
        }

        public bool InitOffLine()
        {
            if (_station == null)
            {
                return false;
            }

            if (Init(_channel.TcpChannelHostName, (ushort)_channel.TcpChannelHostPort, _channel.Timeout, CommDriver, _station.Name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsDeviceOpen()
        {
            return IsConnected();
        }

        public bool DeviceOpen()
        {
            if (!IsConnected())
            {
                Connect();
            }
            if (!IsDeviceOpen() && !lastConnectionError)
                CommDriver.OnSystemEvent(ObjectIds.Server, Properties.Resources.ErrorConnection, Opc.Ua.EventSeverity.High);
            lastConnectionError = !IsDeviceOpen();
            return !lastConnectionError;
        }

        public bool DeviceClose()
        {
            return Disconnect();
        }

        public bool DisConnection()
        {
            if (!IsInit())
                return true;

            return Disconnect();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {            
        }

        /// <summary>
        /// Region Properties
        /// </summary>
        #region Properties

        private Int32 getDevNr
        {
            get
            {
                devNrCnt++;
                return devNrCnt - 1;
            }
        }        

        public int GetErrorLibraryCode
        {
            get
            {
                return nErrorLibraryCode;
            }
        }

        #endregion
    }
}
