using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using IpDriverCodeBaseEx;

namespace SNMP
{

    public enum SNMPVERSION : byte
    {
        SNMPv1 = 0,
        SNMPv2c = 1,
        SNMPv3 = 3,
    }        

    public class SNMPDriver : CommunicationDriver
    {
        public SNMPTrapChannel ChannelTrap { get; set; }
        
        public SNMPDriver()
            : base()
        {
            ChannelTrap = null;
        }

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

            switch ((SNMPProtocol.SNMPErrorCodes)errorcode)
            {
                case SNMPProtocol.SNMPErrorCodes.SNMPErrorMalformedReply:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.SNMPErrorMalformedReply;
                    break;

                case SNMPProtocol.SNMPErrorCodes.SNMPErrorUnsupportedVersion:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.SNMPErrorUnsupportedVersion;
                    break;

                case SNMPProtocol.SNMPErrorCodes.SNMPErrorWrongCommunity:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.SNMPErrorWrongCommunity;
                    break;

                case SNMPProtocol.SNMPErrorCodes.SNMPErrorWrongReplyType:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.SNMPErrorWrongReplyType;
                    break;

                case SNMPProtocol.SNMPErrorCodes.SNMPErrorWrongReplyID:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.SNMPErrorWrongReplyID;
                    break;

                case SNMPProtocol.SNMPErrorCodes.SNMPErrorOidMismatch:
                    quality = StatusCodes.BadNotFound;
                    error = Properties.Resources.SNMPErrorOidMismatch;
                    break;

                case SNMPProtocol.SNMPErrorCodes.SNMPErrorUnsupportedDataType:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.SNMPErrorUnsupportedDataType;
                    break;

                case SNMPProtocol.SNMPErrorCodes.SNMPErrorDataTypeMismatch:
                    quality = StatusCodes.BadTypeMismatch;
                    error = Properties.Resources.SNMPErrorDataTypeMismatch;
                    break;

                case SNMPProtocol.SNMPErrorCodes.SNMPErrorNoSuchObject:
                    quality = StatusCodes.BadNotFound;
                    error = Properties.Resources.SNMPErrorNoSuchObject;
                    break;

                case SNMPProtocol.SNMPErrorCodes.SNMPErrorInvalidDataFormat:
                    quality = StatusCodes.BadSyntaxError;
                    error = Properties.Resources.SNMPErrorInvalidDataFormat;
                    break;
                case SNMPProtocol.SNMPErrorCodes.SNMPErrorWriteFail:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.SNMPErrorWriteFail;
                    break;

                case SNMPProtocol.SNMPErrorCodes.SNMPErrorUserInvalid:
                    quality = StatusCodes.BadUserAccessDenied;
                    error = Properties.Resources.SNMPErrorUserInvalid;
                    break;

                case SNMPProtocol.SNMPErrorCodes.SNMPErrorTrapOIDNoMatchAnyJobs:
                    quality = StatusCodes.BadNotFound;
                    error = Properties.Resources.SNMPErrorTrapOIDNoMatchAnyJobs;
                    break;

                case SNMPProtocol.SNMPErrorCodes.SNMPErrorEncryptDecriptFailed:
                    quality = StatusCodes.BadUserAccessDenied;
                    error = Properties.Resources.SNMPErrorEncryptDecriptFailed;
                    break;

                case SNMPProtocol.SNMPErrorCodes.SNMPErrorChannelTrapMessageSNMPv1:
                    quality = StatusCodes.BadUnexpectedError;
                    error = Properties.Resources.SNMPErrorChannelTrapMessageSNMPv1;
                    break;
            }
            
            if (errorcode > SNMPProtocol.SNMPErrorNonZeroErrorBindVariables)
            {
                int snmpStatusErrorCode = (errorcode - (int)SNMPProtocol.SNMPErrorNonZeroErrorBindVariables);
                switch (snmpStatusErrorCode)
                {
                    case (int)SNMPProtocol.SnmpErrorVariableBindings.ErrornoSuchObject:                        
                    case (int)SNMPProtocol.SnmpErrorVariableBindings.ErrornoSuchInstance:
                        quality = StatusCodes.BadNotFound;
                        break;
                    default:
                        quality = StatusCodes.BadCommunicationError;
                        break;
                }
                error = string.Format(Properties.Resources.SNMPErrorNonZeroErrorStatus, SNMPProtocol.GetErrorStatusCodeString(snmpStatusErrorCode));
            } 
            else if (errorcode > SNMPProtocol.SNMPErrorNonZeroErrorStatus)
            {                
                int snmpStatusErrorCode = (errorcode - (int)SNMPProtocol.SNMPErrorNonZeroErrorStatus);
                switch (snmpStatusErrorCode)
                {
                    case (int)SNMPProtocol.SnmpErrorStatusCode.NoSuchName:
                        quality = StatusCodes.BadNotFound;
                        break;
                    case (int)SNMPProtocol.SnmpErrorStatusCode.authorizationError:
                        quality = StatusCodes.BadUserAccessDenied;
                        break;
                    default:
                        quality = StatusCodes.BadCommunicationError;
                        break;
                }
                error = string.Format(Properties.Resources.SNMPErrorNonZeroErrorStatus, SNMPProtocol.GetErrorBindingVariablesString(snmpStatusErrorCode));
            }
        }

        public override bool Suspend()
        {
            if (ChannelTrap != null)
            {
                ChannelTrap.Dispose();
                ChannelTrap = null;
            }
            bool result = base.Suspend();            

            return result;
        }

        public override bool Startup()
        {
            bool result = base.Startup();
            if (result)
                result = StartUpTrapChannel();

            return result;            
        }

        public override bool Terminate()
        {
            if (ChannelTrap != null)
            {
                ChannelTrap.Dispose();
                ChannelTrap = null;
            }
            bool returnValue = base.Terminate();            

            return (returnValue);
        }
        #endregion

        #region Trap messages Methods
        SNMPTrapChannel trapChannel = null;
        Dictionary<String, List<SNMPChannel>> trapChannelsByIPAddress = new Dictionary<String, List<SNMPChannel>>();
        List<SNMPStation> trapAllStations = new List<SNMPStation>();

        private string CreateChannelSearchKey(string ipAddress, SNMPVERSION protocolVersion, string userName = null)
        {
            string key = string.Empty;
            switch (protocolVersion)
            {
                case SNMPVERSION.SNMPv1:
                    // trap not supported
                    break;
                case SNMPVERSION.SNMPv2c:
                    key = ipAddress;
                    break;
                case SNMPVERSION.SNMPv3:
                    key = ipAddress;
                    if (!string.IsNullOrEmpty(userName))
                        key = string.Format("{0}-{1}", key, userName);
                    break;
            }

            return key;
        }

        private void BuildSNMPChannelDictionary()
        {
            List<Channel> channelList = GetChannels();
            foreach (SNMPChannel snmpChannel in channelList)
            {
                if (snmpChannel.snmpVersion == SNMPVERSION.SNMPv2c || snmpChannel.snmpVersion == SNMPVERSION.SNMPv3)
                {
                    if (!String.IsNullOrEmpty(snmpChannel.GetUdpChannelHostName()))
                    {
                        if (UdpChannelList.GetResolvedConnecionIPAddress(snmpChannel.GetUdpChannelHostName(), out IPAddress objIpAddress))
                        {
                            foreach (SNMPStation snmpStation in this.GetChannelStations(snmpChannel))
                            {
                                trapAllStations.Add(snmpStation);
                                string searchKey = CreateChannelSearchKey(objIpAddress.ToString(), snmpChannel.snmpVersion, snmpStation.snmpUserName);
                                if (!String.IsNullOrWhiteSpace(searchKey))
                                {
                                    if (!trapChannelsByIPAddress.ContainsKey(searchKey))                                    
                                        trapChannelsByIPAddress[searchKey] = new List<SNMPChannel>();

                                    if (!trapChannelsByIPAddress[searchKey].Contains(snmpChannel))
                                        trapChannelsByIPAddress[searchKey].Add(snmpChannel);
                                }
                            }
                        }

                        //// create a "compatibility" element fpr v2c protocol channel and trap information message (that not require autentication)
                        //if (snmpChannel.snmpVersion == SNMPVERSION.SNMPv3)
                        //{
                        //    string searchKey = CreateChannelSearchKey(objIpAddress.ToString(), SNMPVERSION.SNMPv2c);
                        //    if (!String.IsNullOrWhiteSpace(searchKey))
                        //    {
                        //        if (!trapChannelsByIPAddress.ContainsKey(searchKey))
                        //            trapChannelsByIPAddress[searchKey] = new List<SNMPChannel>();

                        //        if (!trapChannelsByIPAddress[searchKey].Contains(snmpChannel))
                        //            trapChannelsByIPAddress[searchKey].Add(snmpChannel);
                        //    }
                        //}
                    }
                }
            }
        }

        public List<SNMPChannel> GetCompatibleTrapChannels(SNMPProtocol.SNMPMessageGeneralData messageGeneralData)
        {
            List<SNMPChannel> channels = new List<SNMPChannel>();
            // Check if a channel with the same IP address of the remote sender has been defined            
            string searchKey = CreateChannelSearchKey(messageGeneralData.TrapRemoteEndPoint.Address.ToString(), messageGeneralData.snmpVersion, messageGeneralData.ParsedUsm.GetUserNameString());
            if (!string.IsNullOrEmpty(searchKey))
            {
                if (trapChannelsByIPAddress.ContainsKey(searchKey))
                    channels.AddRange(trapChannelsByIPAddress[searchKey]);
            }

            return channels;
        }

        public bool StartUpTrapChannel()
        {
            // startup trap only if some channel are configured v2c or v3 : v1 is not supported
            if (trapAllStations.Count > 0)
            {
                ChannelTrap = new SNMPTrapChannel(this);
                ChannelTrap.SetAutenticationStations(trapAllStations);
                ChannelTrap.StartUpTrapMessagesDispatcherThread();
            }

            return true;
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
            if (ChannelTrap != null)
            {
                ChannelTrap.Dispose();
                ChannelTrap = null;
            }
            base.Dispose();
        }            
        #endregion
    }
}
