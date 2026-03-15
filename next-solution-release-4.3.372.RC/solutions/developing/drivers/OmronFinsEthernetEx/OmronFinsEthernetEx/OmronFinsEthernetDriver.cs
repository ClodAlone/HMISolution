using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using IpDriverCodeBaseEx;
using System.Net;
using DriverCodeBaseEx.Enumerators;

namespace OmronFinsEthernet
{
    public class OmronFinsEthernetDriver : CommunicationDriver, IDisposable
    {
        public OmronFinsEthernetDriver()
            : base()
        {
        }

        public OmronFinsEthernetDriver(string strSettingPath)
            : base(strSettingPath)
        {
        }

        #region Overrides
        public override bool Init(string strSettingPath, bool isProtected, Guid protectionCode)
        {
#if !DEBUG
            if (!MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxJ0V2Dx2Ij9ozl7B6jwS4IA=="/*MDB*/))
            {
                throw new LicenseOptionMissingException("Missing 'MDB' option in the license");
            }
#endif
            return base.Init(strSettingPath, isProtected, protectionCode);
        }

        public override bool LoadDriverSettings(IDataLayer idl)
        {
            bool bRet = false;
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                try
                {
                    var configuration = (from tag in new XPQuery<OmronFinsEthernetDriverSettings>(ufw).AsParallel() select tag).Single();
                    LoadDriverSettings(configuration);
                    bRet = true;
                }
                catch (InvalidOperationException ex)
                {
                    OnSystemEvent(null, String.Format(Properties.Resources.ErrorLoadingDriverSettings, ex.Message), EventSeverity.Min);
                    LoadDefaultSettings();
                    bRet = true;
                }
            }

            return bRet;
        }

        public override void LoadDriverSettings(DriverSettings settings)
        {
            base.LoadDriverSettings(settings);

            var conf = settings as OmronFinsEthernetDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<OmronFinsEthernetDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new OmronFinsEthernetDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as OmronFinsEthernetDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra OmronFinsEthernet Driver Settings initializzation
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as OmronFinsEthernetChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new OmronFinsEthernetChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as OmronFinsEthernetStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new OmronFinsEthernetStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new OmronFinsEthernetTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new OmronFinsEthernetTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as OmronFinsEthernetTag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }

        public override bool IsPrototypeSplitEnabled()
        {
            return true;
        }

        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<OmronFinsEthernetChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override bool TestComm(ChannelSettings ch, StationSettings st, BuiltInType tagDataType, string dynamicSettings, out string error)
        {
            error = null;

            Channel channel = CreateChannel(ch);
            Channels[channel.Name] = channel;
            Channels[channel.Name].Init();

            Station station = CreateStation(st);
            Stations[station.Name] = station;
            Stations[station.Name].Init(channel);


            //Enable test execution
            ((OmronFinsEthernetChannel)channel).TestConnection = true;


            IList<object> dataValues = new List<Object>();
            switch (tagDataType)
            {
                case BuiltInType.Boolean:
                    dataValues.Add(new Variant(Convert.ToBoolean(0), new Opc.Ua.TypeInfo(tagDataType, ValueRanks.Scalar)));
                    break;
                case BuiltInType.Byte:
                    dataValues.Add(new Variant(Convert.ToByte(0), new Opc.Ua.TypeInfo(tagDataType, ValueRanks.Scalar)));
                    break;
                case BuiltInType.Int16:
                    dataValues.Add(new Variant(Convert.ToInt16(0), new Opc.Ua.TypeInfo(tagDataType, ValueRanks.Scalar)));
                    break;
                case BuiltInType.Int32:
                    dataValues.Add(new Variant(Convert.ToInt32(0), new Opc.Ua.TypeInfo(tagDataType, ValueRanks.Scalar)));
                    break;
                case BuiltInType.Float:
                    dataValues.Add(new Variant(Convert.ToSingle(0), new Opc.Ua.TypeInfo(tagDataType, ValueRanks.Scalar)));
                    break;
                case BuiltInType.Double:
                    dataValues.Add(new Variant(Convert.ToDouble(0), new Opc.Ua.TypeInfo(tagDataType, ValueRanks.Scalar)));
                    break;
                default:
                    dataValues.Add(new Variant(Convert.ToInt16(0), new Opc.Ua.TypeInfo(tagDataType, ValueRanks.Scalar)));
                    break;
            }

            uint conn = OnReadValues(string.Format(dynamicSettings, station.Name), dataValues);
            if (conn != StatusCodes.Good)
                error = String.Format(Properties.Resources.ErroConnectionFailed);//GetDriverErrorInfo((int)conn, out uint quality, out error);
            else
            {
                error = info;
                info = string.Empty;
            }
            //Disable test execution
            ((OmronFinsEthernetChannel)channel).TestConnection = false;
            return (conn == StatusCodes.Good);

        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((OmronFinsEthernetErrorCodes)errorcode)
            {
                case OmronFinsEthernetErrorCodes.ErrorSid:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorSid;
                    break;
                case OmronFinsEthernetErrorCodes.ErrorEndCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorEndCode;
                    break;
                case OmronFinsEthernetErrorCodes.ErrorAnswer:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorAnswer + AdditionalError;
                    break;
            }
        }
        /// <summary>
        /// Info Test PLC
        /// </summary>
        /// <param name="stinfo"></param>
        public static void Addinfo(string stinfo)
        {
            if (string.IsNullOrWhiteSpace(info))
                info = stinfo;
            else
                info = string.Format("{0}\n{1}", info, stinfo);
        }
        #endregion

        #region Local Bound with specific port traffic (Send/Receive) management
        private class LocalBoundChannel
        {
            public enum ConnectionStates
            {
                None,
                Opened,
                Closed
            }
            #region Properties
            public OmronFinsEthernetChannel Channel { set; get; }
            public ConnectionStates ConnectionState { set; get; }
            public bool PendingRequest { set; get; }
            #endregion

            public LocalBoundChannel(OmronFinsEthernetChannel channel)
            {
                Channel = channel;
                PendingRequest = false;
                ConnectionState = ConnectionStates.None;
            }
        }

        private object lockLocalBound = new object();
        // Dictionary<localIpAddress:LocalUpPort, Dictionary<DeviceIpAddress:DeviceUpPort, LocalBoundChannel>> 
        private Dictionary<string, Dictionary<string, LocalBoundChannel>> mapLocalBoundPortChannels = new Dictionary<string, Dictionary<string, LocalBoundChannel>>();
        // Dictionary<localIpAddress:LocalUpPort, LocalBoundManager>> 
        private Dictionary<string, OmronFinsEthernetUDPManager> mapUdpSocketManager = new Dictionary<string, OmronFinsEthernetUDPManager>();

        /// <summary>
        /// Check if channel is "registred" into driver internal list to communicate with device
        /// </summary>
        /// <param name="channelIpAddressUdpPortID"></param>
        /// <returns></returns>
        private bool IsValidLocalBoundPortChannel(string channelIpAddressUdpPortID)
        {
            lock (lockLocalBound)
                return mapLocalBoundPortChannels.ContainsKey(channelIpAddressUdpPortID);
        }

        /// <summary>
        /// Initialize internal object to communicate with device when local udp port is set
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public bool InitLocalBoundPort(OmronFinsEthernetChannel ch, UdpChannelSettings settings)
        {
            lock (lockLocalBound)
            {
                if (!mapLocalBoundPortChannels.ContainsKey(ch.LocalIpAddressUdpPortID))
                {
                    if (!mapUdpSocketManager.ContainsKey(ch.LocalIpAddressUdpPortID))
                    {
                        // create udp manager with automatic BeginDeviceRead relaunch
                        OmronFinsEthernetUDPManager sock = new OmronFinsEthernetUDPManager(settings, true);
                        mapUdpSocketManager[ch.LocalIpAddressUdpPortID] = sock;
                    }
                    mapLocalBoundPortChannels[ch.LocalIpAddressUdpPortID] = new Dictionary<string, LocalBoundChannel>();
                }
                LocalBoundChannel lbCh = new LocalBoundChannel(ch);
                mapLocalBoundPortChannels[ch.LocalIpAddressUdpPortID][ch.DeviceIpAddressUdpPortID] = lbCh;
            }
            return true;
        }

        /// <summary>
        /// Dispatch the data recived by driver (when local upd port is set) to channal that sent data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SockUpdManager_NewDataReceived(object sender, NewDataReceivedArgs e)
        {
            List<OmronFinsEthernetChannel> channels = new List<OmronFinsEthernetChannel>();
            lock (lockLocalBound)
            {
                OmronFinsEthernetUDPManager receiver = sender as OmronFinsEthernetUDPManager;
                IPEndPoint remoteEndPoint = e.Sender as IPEndPoint;
                if (remoteEndPoint != null)
                {
                    string receiverIpAddressUdpPortID = OmronFinsEthernetProtocol.GetIpAddressUdpPortID(receiver.UdpChannelLocalHostName, receiver.UdpChannelLocalHostPort);
                    string deviceIpAddressUdpPortID = OmronFinsEthernetProtocol.GetIpAddressUdpPortID(remoteEndPoint.Address.ToString(), remoteEndPoint.Port);

                    // message come from a managed PLC ?
                    if (IsValidLocalBoundPortChannel(receiverIpAddressUdpPortID))
                    {
                        // dispach received data to all channel with same ip address/port
                        foreach (LocalBoundChannel ch in mapLocalBoundPortChannels[receiverIpAddressUdpPortID].Values)
                        {
                            if (ch.Channel.DeviceIpAddressUdpPortID == deviceIpAddressUdpPortID && ch.PendingRequest)
                            {
                                channels.Add(ch.Channel);
                                ch.PendingRequest = false;
                            }
                        }
                    }
                }
            }

            if (channels.Count > 0)
            {
                // dispatch received data to channel that sent request
                foreach (var channel in channels)
                {
                    channel.SetReceiveBuffer(e.RxBytes);
                    channel.SetNewDataEvent();
                }
            }
        }

        /// <summary>
        /// Override of DeviceOpen() channel's method called when local udp port is set
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public bool DeviceOpenLocalBoundPort(OmronFinsEthernetChannel ch)
        {
            lock (lockLocalBound)
            {
                if (!IsValidLocalBoundPortChannel(ch.LocalIpAddressUdpPortID))
                    return false;

                OmronFinsEthernetUDPManager sock = mapUdpSocketManager[ch.LocalIpAddressUdpPortID];

                bool open;
                // if was opened by another channel, do nothig (status variable will be updated later)
                if (sock.IsDeviceOpen())
                {
                    open = true;
                }
                else
                {                    
                    open = sock.DeviceOpen(ch);
                    if (open)
                        sock.NewDataReceived += SockUpdManager_NewDataReceived;
                }
                if (open)
                {
                    // refresh status variable
                    sock.DeviceOpenUpdateState(ch);
                    mapLocalBoundPortChannels[ch.LocalIpAddressUdpPortID][ch.DeviceIpAddressUdpPortID].ConnectionState = LocalBoundChannel.ConnectionStates.Opened;
                }

                return open;
            }
        }

        /// <summary>
        /// Override of IsDeviceOpen() channel's method called when local udp port is set
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public bool IsDeviceOpenLocalBoundPort(OmronFinsEthernetChannel ch)
        {
            lock (lockLocalBound)
            {
                if (!IsValidLocalBoundPortChannel(ch.LocalIpAddressUdpPortID))
                    return false;

                OmronFinsEthernetUDPManager sock = mapUdpSocketManager[ch.LocalIpAddressUdpPortID];
                return (mapLocalBoundPortChannels[ch.LocalIpAddressUdpPortID][ch.DeviceIpAddressUdpPortID].ConnectionState == LocalBoundChannel.ConnectionStates.Opened && sock.IsDeviceOpen(ch));
            }
        }

        /// <summary>
        /// Override of DeviceClose() channel's method called when local udp port is set
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public bool DeviceCloseLocalBoundPort(OmronFinsEthernetChannel ch)
        {
            lock (lockLocalBound)
            {
                if (!IsValidLocalBoundPortChannel(ch.LocalIpAddressUdpPortID))
                    return false;

                OmronFinsEthernetUDPManager sock = mapUdpSocketManager[ch.LocalIpAddressUdpPortID];
                mapLocalBoundPortChannels[ch.LocalIpAddressUdpPortID][ch.DeviceIpAddressUdpPortID].ConnectionState = LocalBoundChannel.ConnectionStates.Closed;
                // refresh status variable
                sock.DeviceCloseUpdateState(ch);

                bool anyChannelActive = (mapLocalBoundPortChannels[ch.LocalIpAddressUdpPortID].Values.Count(channel => channel.ConnectionState == LocalBoundChannel.ConnectionStates.Opened) > 0);
                if (!anyChannelActive)
                {
                    sock.NewDataReceived -= SockUpdManager_NewDataReceived;
                    sock.DeviceClose(ch);
                }

                return true;
            }
        }

        /// <summary>
        /// Override of BeginDeviceRead() channel's method called when local udp port is set
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public bool BeginDeviceReadLocalBoundPort(OmronFinsEthernetChannel ch, int Count)
        {
            lock (lockLocalBound)
            {
                if (!IsValidLocalBoundPortChannel(ch.LocalIpAddressUdpPortID))
                    return false;

                OmronFinsEthernetUDPManager sock = mapUdpSocketManager[ch.LocalIpAddressUdpPortID];
                sock.BeginDeviceRead(Count);
                return true;
            }
        }

        /// <summary>
        /// Override of DeviceWrite() channel's method called when local udp port is set
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="Buffer"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public bool DeviceWriteLocalBoundPort(OmronFinsEthernetChannel ch, byte[] Buffer, uint Count)
        {
            lock (lockLocalBound)
            {
                if (!IsValidLocalBoundPortChannel(ch.LocalIpAddressUdpPortID))
                    return false;

                OmronFinsEthernetUDPManager sock = mapUdpSocketManager[ch.LocalIpAddressUdpPortID];
                return sock.DeviceWrite(ch, Buffer, Count);
            }
        }

        /// <summary>
        /// Override of ResetNewDataEvent() channel's method called when local udp port is set
        /// </summary>
        /// <param name="ch"></param>
        public void ResetNewDataEventLocalBoundPort(OmronFinsEthernetChannel ch)
        {
            lock (lockLocalBound)
            {
                if (!IsValidLocalBoundPortChannel(ch.LocalIpAddressUdpPortID))
                    return;

                Dictionary<string, LocalBoundChannel> map = mapLocalBoundPortChannels[ch.LocalIpAddressUdpPortID];
                if (map.ContainsKey(ch.DeviceIpAddressUdpPortID))
                    map[ch.DeviceIpAddressUdpPortID].PendingRequest = true;
            }
        }

        /// <summary>
        /// Override of WaitNewDataEvent() channel's method called when local udp port is set
        /// </summary>
        /// <param name="ch"></param>
        public void WaitNewDataEventLocalBoundPort(OmronFinsEthernetChannel ch)
        {
            lock (lockLocalBound)
            {
                if (!IsValidLocalBoundPortChannel(ch.LocalIpAddressUdpPortID))
                    return;

                Dictionary<string, LocalBoundChannel> map = mapLocalBoundPortChannels[ch.LocalIpAddressUdpPortID];
                if (map.ContainsKey(ch.DeviceIpAddressUdpPortID))
                    map[ch.DeviceIpAddressUdpPortID].PendingRequest = false;
            }
        }
        #endregion

        #region property
        /// <summary>   Additional error. </summary>
        public string AdditionalError;
        public static string info;
        #endregion

        public override void Dispose()
        {
            base.Dispose();

            foreach (OmronFinsEthernetUDPManager soc in mapUdpSocketManager.Values)
                soc.Dispose();
        }
    }
}
