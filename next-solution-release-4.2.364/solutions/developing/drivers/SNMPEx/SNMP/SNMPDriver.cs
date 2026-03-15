using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using IpDriverCodeBaseEx;
using System.Net.Sockets;

namespace SNMP
{

    public enum SNMPVERSION : byte
    {
        SNMPv1,
        SNMPv2c
    }

    

    public class SNMPDriver : CommunicationDriver
    {
        public SNMPTrapChannel ChannelTrap ;
        
        public SNMPDriver()
            : base()
        {
            ChannelTrap = null;
        }

        #region Data Members
        public Object lockChannelTrap = new Object();
        SNMPTrapChannel trapChannel;
        Dictionary<String, SNMPChannel> ChannelsByIPAddress = new Dictionary<String, SNMPChannel>();
        #endregion

        #region Methods
        private void BuildSNMPChannelDictionary()
        {
            List<Channel> channelList = GetChannels();
            foreach (var channel in channelList)
            {
                UdpChannelList snmpChannel = (UdpChannelList)channel;
                if (snmpChannel != null && !String.IsNullOrEmpty(snmpChannel.GetUdpChannelHostName()))
                {
                    IPAddress objIpAddress = new IPAddress(0);
                    if (UdpChannelList.GetResolvedConnecionIPAddress(snmpChannel.GetUdpChannelHostName(), out objIpAddress))
                    {
                        string ipAddress = objIpAddress.ToString();
                        if (!String.IsNullOrWhiteSpace(ipAddress))
                        {
                            ChannelsByIPAddress[ipAddress] = snmpChannel as SNMPChannel;
                        }
                    }
                }
            }
        }

        public void PassMessageToChannel(ReceivedPacket packet)
        {
            // Check if a channel with the same IP address of the remote sender has been defined
            IPEndPoint ipEndPoint = packet.RemoteEndPoint;
            IPAddress ipAddress = ipEndPoint.Address;
            string searchKey = ipAddress.ToString();
            if (ChannelsByIPAddress.ContainsKey(searchKey))
            {
                // Pass the message to the channel
                ChannelsByIPAddress[searchKey].AddTrapMessage(packet);
            }
        }

        public override bool TestComm(ChannelSettings ch, StationSettings st, out string error)
        {
            throw new NotImplementedException();
        }
        public override bool TestComm(ChannelSettings ch, StationSettings st, BuiltInType tagDataType, string dynamicSettings, out string error)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Overrides
        public override bool Init(string strSettingPath, bool isProtected, Guid protectionCode)
        {
#if !DEBUG
            if (!MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxDkmnbdFT0I5Fy1y5SsWaqg=="/*FCS*/))
            {
                throw new LicenseOptionMissingException("Missing 'FCS' option in the license");
            }
#endif
            if (!base.Init(strSettingPath, isProtected, protectionCode))
            {
                return (false);
            }

            // Driver specific:
            // Build an addtional dictionary to retieve channels by their IP address
            BuildSNMPChannelDictionary();

            // create and open the listening channel
            //trapChannel = new SNMPTrapChannel(this);
            //trapChannel.Startup();

            return (true);
        }

        public override bool LoadDriverSettings(IDataLayer idl)
        {
            bool bRet = false;
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                try
                {
                    var configuration = (from tag in new XPQuery<SNMPDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as SNMPDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<SNMPDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new SNMPDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as SNMPDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as SNMPChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new SNMPChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as SNMPStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new SNMPStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new SNMPTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new SNMPTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as SNMPTag;
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
                return new List<ChangeTag>((new XPQuery<SNMPChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((SNMPErrorCodes)errorcode)
            {
                case SNMPErrorCodes.SNMPErrorMalformedReply:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.SNMPErrorMalformedReply;
                    break;

                case SNMPErrorCodes.SNMPErrorUnsupportedVersion:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.SNMPErrorUnsupportedVersion;
                    break;

                case SNMPErrorCodes.SNMPErrorWrongCommunity:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.SNMPErrorWrongCommunity;
                    break;

                case SNMPErrorCodes.SNMPErrorWrongReplyType:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.SNMPErrorWrongReplyType;
                    break;

                case SNMPErrorCodes.SNMPErrorWrongReplyID:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.SNMPErrorWrongReplyID;
                    break;

                case SNMPErrorCodes.SNMPErrorOidMismatch:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.SNMPErrorOidMismatch;
                    break;

                case SNMPErrorCodes.SNMPErrorUnsupportedDataType:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.SNMPErrorUnsupportedDataType;
                    break;

                case SNMPErrorCodes.SNMPErrorDataTypeMismatch:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.SNMPErrorDataTypeMismatch;
                    break;

                case SNMPErrorCodes.SNMPErrorNoSuchObject:
                    quality = StatusCodes.BadNotFound;
                    error = Properties.Resources.SNMPErrorNoSuchObject;
                    break;

                case SNMPErrorCodes.SNMPErrorInvalidDataFormat:
                    quality = StatusCodes.BadSyntaxError;
                    error = Properties.Resources.SNMPErrorInvalidDataFormat;
                    break;
                case SNMPErrorCodes.SNMPErrorWriteFail:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.SNMPErrorWriteFail;
                    break;
            }

            if ((SNMPErrorCodes)errorcode > SNMPErrorCodes.SNMPErrorNonZeroErrorStatus)
            {
                quality = StatusCodes.BadCommunicationError;
                error = string.Format(Properties.Resources.SNMPErrorNonZeroErrorStatus, SNMPProtocol.GetErrorString(errorcode - (int)SNMPErrorCodes.SNMPErrorNonZeroErrorStatus));
            }
        }

        public override bool Suspend()
        {
            if (ChannelTrap != null)
            {
                ChannelTrap.Suspend();
            }
            return (base.Suspend());
        }

        public override bool Startup()
        {
            if(base.Startup())
            {
                StartTrapChannel();
                return (true);
            }
            else
            {
                return (false);
            }
        }                
        #endregion

        #region Properties

        private bool _AllowOtherSNMPClients;

        // Setup driver to coexist with other SNMPC clients        
        public bool AllowOtherSNMPClients
        {
            get { return _AllowOtherSNMPClients; }
        }

        

        #endregion

        public void StartTrapChannel()
        {
            if(ChannelTrap == null)
            {
                ChannelTrap = new SNMPTrapChannel(this);                        
            }
            if((ChannelTrap != null) && !ChannelTrap.Connected())
            {
                ChannelTrap.DeviceOpen();
            }
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
            if(ChannelTrap != null)
            {
                ChannelTrap.Dispose();
            }

            base.Dispose();           
        }

        #endregion
    }
}
