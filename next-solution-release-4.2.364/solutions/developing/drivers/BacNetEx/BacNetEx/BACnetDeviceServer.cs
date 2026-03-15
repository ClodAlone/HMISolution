////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	BACnetDeviceServer.cs
//
// summary:	Implements the driver BACnet WhoIs and iAm message service 
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using IpDriverCodeBaseEx;

namespace BACnet
{
    /// <summary>   Communication target device. </summary>
    public class BACnetDeviceServer
    {
        #region Constructors

        public BACnetDeviceServer(BACnetChannel channel)
        {
            DefaultSettings();
            _Channel = channel;
        }    

        public BACnetDeviceServer(BACnetChannel channel,int deviceId, string localHostName, int localHostPort)
        {
            DefaultSettings();
            _Channel = channel;
            _DeviceId = deviceId;
            _LocalHostName = localHostName;
            _LocalHostPort = localHostPort;
        }

        private void DefaultSettings()
        {            
            _DeviceId = -1;
            _LocalHostName = string.Empty;
            _LocalHostIP = null;
            FirstCycle = true;
        }

        #endregion

        #region Abstract Methods

        #endregion

        #region member

        #endregion

        #region Properties
        
        private int _DeviceId;
        public int DeviceId
        {
            get
            {
                return _DeviceId;
            }
            set
            {
                _DeviceId = value;
            }
        }                

        private bool _FirstCycle;
        public bool FirstCycle
        {
            get
            {                
                return _FirstCycle;
            }
            set
            {
                _FirstCycle = value;
            }
        }
                
        string _LocalHostName;
        public string LocalHostName
        {
            get { return _LocalHostName; }
            set {_LocalHostName = value;}
        }

        int _LocalHostPort;
        public int LocalHostPort
        {
            get { return _LocalHostPort; }
            set { _LocalHostPort = value; }
        }

        IPAddress _LocalHostIP;
        public IPAddress LocalHostIP
        {
            get
            {
                if (_LocalHostIP != null)
                    return _LocalHostIP;

                if (!string.IsNullOrEmpty(_LocalHostName))
                {
                    IPAddress resolvedIPAddress;
                    if (UDPManager.GetResolvedConnecionIPAddress(_LocalHostName, out resolvedIPAddress))
                        _LocalHostIP = resolvedIPAddress;
                }

                return _LocalHostIP;
            }
            set
            {
                _LocalHostIP = value;
            }
        }

        BACnetChannel _Channel;
        IPEndPoint _BroadcastEndPoint;

        /// <summary>   The UDP remote end point. </summary>
        IPEndPoint _LocalEndPoint = null;
        public IPEndPoint LocalEndPoint
        {
            get
            {
                if (_LocalEndPoint == null && !string.IsNullOrEmpty(_LocalHostName))
                {
                    try
                    {
                        if (UDPManager.GetResolvedConnecionIPAddress(_LocalHostName, out IPAddress resolvedIPAddress))
                            _LocalEndPoint = new IPEndPoint(resolvedIPAddress, _LocalHostPort);
                        
                    }
                    catch (Exception ex)
                    {
                        _LocalEndPoint = null;
                    }
                }
                return _LocalEndPoint;
            }
        }

        #endregion

        private UnicastIPAddressInformation GettAddress_DefaultInterface()
        {
            UnicastIPAddressInformation UniIP = null;
            int NbAdd = 0;

            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
                if (adapter.OperationalStatus == OperationalStatus.Up)
                    if (adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                        if (!(adapter.Description.Contains("VirtualBox") || adapter.Description.Contains("VMware")))
                            foreach (UnicastIPAddressInformation ip in adapter.GetIPProperties().UnicastAddresses)
                            {
                                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                {
                                    UniIP = ip;
                                    NbAdd++;
                                }
                            }
            if (NbAdd == 1)
                return UniIP;
            else
                return null;

        }
        private IPAddress GetBroadcastAddress()
        {
            // general broadcast by default if nothing better is found
            IPAddress ResultIp = System.Net.IPAddress.Parse("255.255.255.255");
            UnicastIPAddressInformation ipAddr = null;

            if (LocalHostIP == null)
                return null;

            if (LocalHostIP.ToString() == "0.0.0.0")
                ipAddr = GettAddress_DefaultInterface();
            else
            {
                // restricted local broadcast (directed ... routable)
                foreach (NetworkInterface adapter in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()) { 
                    foreach (UnicastIPAddressInformation ip in adapter.GetIPProperties().UnicastAddresses)
                        if (LocalHostIP.Equals(ip.Address))
                        {
                            ipAddr = ip;
                            break;
                        }
                }
            }

            if (ipAddr != null)
            {
                try
                {
                    string[] strCurrentIP = ipAddr.Address.ToString().Split('.');
                    string[] strIPNetMask = ipAddr.IPv4Mask.ToString().Split('.');
                    StringBuilder BroadcastStr = new StringBuilder();
                    for (int i = 0; i < 4; i++)
                    {
                        BroadcastStr.Append(((byte)(int.Parse(strCurrentIP[i]) | ~int.Parse(strIPNetMask[i]))).ToString());
                        if (i != 3) BroadcastStr.Append('.');
                    }
                    ResultIp = System.Net.IPAddress.Parse(BroadcastStr.ToString());
                }
                catch { }
            }

            return ResultIp;
        }

        private IPEndPoint GetBroadcastEndPoing(int port) {
            IPAddress BroadcastIpAddres = GetBroadcastAddress();
            if (BroadcastIpAddres != null)
                return new IPEndPoint(BroadcastIpAddres, BACnetEnums.BACnet_DEFAULT_UDP_PORT);
            else
                return null;
        }

        public void Init()
        {
            _BroadcastEndPoint = GetBroadcastEndPoing(BACnetEnums.BACnet_DEFAULT_UDP_PORT);
        }

        public bool SendiAm()
        {
            return SendiAm(_BroadcastEndPoint);
        }

        public bool SendiAm(IPEndPoint remoteEndPoint)
        {
            if (remoteEndPoint == null)
                return false;

            return (_Channel.DeviceWrite(IPFrameFactory.iAm(_DeviceId,BACnetEnums.PROGEA_VENDOR_ID), remoteEndPoint));
        }

        public bool SendiAmARouter()
        {
            return SendiAmARouter(_BroadcastEndPoint);
        }

        public bool SendiAmARouter(IPEndPoint remoteEndPoint)
        {
            if (remoteEndPoint == null)
                return false;
            
            return (_Channel.DeviceWrite(IPFrameFactory.iAmARouterToNetwork(), remoteEndPoint));
        }
    }
}
