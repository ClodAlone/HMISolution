using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;
using DevExpress.Utils.About;

namespace OmronEthernetIP
{
    public class OmronEthernetIPDriver : CommunicationDriver
    {
        public static string info;
        public OmronEthernetIPDriver()
            : base()
        {
        }

        public OmronEthernetIPDriver(string strSettingPath)
            : base(strSettingPath)
        {
        }

        #region Methods
        public override bool TestComm(ChannelSettings ch, StationSettings st, out string error)
        {
            error = string.Empty;

            Channel channel = CreateChannel(ch);
            Channels[channel.Name] = channel;
            Channels[channel.Name].Init();

            Station station = CreateStation(st);
            Stations[station.Name] = station;
            Stations[station.Name].Init(channel);

            // create a dummy tag and job only to pass PLCType parameter
            Tag defTag = CreateTag(new TagDefinition
            {
                NodeId = new NodeId(1),
                DynamicSettings = string.Format(OmronEthernetIPProtocol.TEST_COMM_DYNAMIC_SETTINGS,station.Name),
                DataType = (int)UFUAModel.DataType.Int16,
                SamplingInterval = 0,
                ArrayDimension = 0
            }, 0, 0);                        
            CommJob job = station.CreateJob(defTag);
            ((OmronEthernetIPChannel)channel).TestConnection = true;
            DriverErrorCodes conn = ((OmronEthernetIPChannel)channel).CheckDevice(new List<CommJob> { job }, channel);
            if (conn != DriverErrorCodes.ErrorNoError)
                GetDriverErrorInfo((int)conn, out uint quality, out error);
            else
                error = info;
            info = string.Empty;
            channel.DeviceClose();

            return (conn == DriverErrorCodes.ErrorNoError);
        }
        public static void Addinfo(string stinfo)
        {
            if (string.IsNullOrWhiteSpace(info))
                info = stinfo;
            else
                info = string.Format("{0}\n{1}", info, stinfo);
        }
        #endregion

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
                    var configuration = (from tag in new XPQuery<OmronEthernetIPDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as OmronEthernetIPDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<OmronEthernetIPDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new OmronEthernetIPDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as OmronEthernetIPDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra OmronEthernetIP Driver Settings initializzation
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as OmronEthernetIPChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new OmronEthernetIPChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as OmronEthernetIPStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new OmronEthernetIPStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new OmronEthernetIPTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new OmronEthernetIPTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as OmronEthernetIPTag;
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
                return new List<ChangeTag>((new XPQuery<OmronEthernetIPChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((OmronEthernetIPErrorCodes)errorcode)
            {
                case OmronEthernetIPErrorCodes.ErrorWrongTransaction:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWrongTransaction;
                    break;
                case OmronEthernetIPErrorCodes.ErrorRepTooShort:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRepTooShort;
                    break;
                case OmronEthernetIPErrorCodes.ErrorRepTooLong:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRepTooLong;
                    break;
                case OmronEthernetIPErrorCodes.ErrorRepTagSize:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRepTagSize;
                    break;
                case OmronEthernetIPErrorCodes.ErrorNoRep:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorNoRep;
                    break;
                case OmronEthernetIPErrorCodes.ErrorRepDataType:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRepDataType;
                    break;
                case OmronEthernetIPErrorCodes.ErrorWrongMRServiceReplyCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWrongMRServiceReplyCode;
                    break;
                case OmronEthernetIPErrorCodes.ErrorMRStatus:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorMRStatus;
                    break;
                case OmronEthernetIPErrorCodes.ErrorWrongPCCCRespCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWrongPCCCRespCode;
                    break;
                case OmronEthernetIPErrorCodes.ErrorRepSerNum:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRepSerNum;
                    break;
                case OmronEthernetIPErrorCodes.ErrorRepSerCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRepSerCode;
                    break;
                case OmronEthernetIPErrorCodes.ErrorRepStatus:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRepStatus;
                    break;
                case OmronEthernetIPErrorCodes.ErrorStatusNotZero:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorStatusNotZero;
                    break;
                case OmronEthernetIPErrorCodes.ErrorUnknSTSEXTCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnknSTSEXTCode;
                    break;
                case OmronEthernetIPErrorCodes.ErrorEXTSTSCodeExt:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorEXTSTSCodeExt;
                    break;
                case OmronEthernetIPErrorCodes.ErrorUnknSTSCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnknSTSCode;
                    break;
                case OmronEthernetIPErrorCodes.ErrorOutOfSync:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorOutOfSync;
                    break;
                case OmronEthernetIPErrorCodes.ErrorEncapStatus1:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorEncapStatus1;
                    break;
                case OmronEthernetIPErrorCodes.ErrorEncapStatus2:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorEncapStatus2;
                    break;
                case OmronEthernetIPErrorCodes.ErrorEncapStatus3:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorEncapStatus3;
                    break;
                case OmronEthernetIPErrorCodes.ErrorEncapStatus64:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorEncapStatus64;
                    break;
                case OmronEthernetIPErrorCodes.ErrorEncapStatus65:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorEncapStatus65;
                    break;
                case OmronEthernetIPErrorCodes.ErrorEncapStatus69:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorEncapStatus69;
                    break;
                case OmronEthernetIPErrorCodes.ErrorUnknEncapStatus:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnknEncapStatus;
                    break;
                case OmronEthernetIPErrorCodes.ErrorInvalidReplyFragmentedProtocol:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorInvalidReplyFragmentedProtocol;
                    break;
                case OmronEthernetIPErrorCodes.ErrorCipStatus:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorCipStatus;
                    break;

                default:
                    if((errorcode & 0x10000000) != 0)
                    {
                        quality = StatusCodes.BadCommunicationError;
                        switch (errorcode & 0xFFF00000)
                        {
                            case 0x10200000:
                                error = Properties.Resources.ErrorCipStatus02;
                                break;
                            case 0x10400000:
                                error = Properties.Resources.ErrorCipStatus04;
                                break;
                            case 0x10500000:
                                error = Properties.Resources.ErrorCipStatus05;
                                break;
                            case 0x10C00000:
                                error = Properties.Resources.ErrorCipStatus0C;
                                break;
                            case 0x11100000:
                                error = Properties.Resources.ErrorCipStatus11;
                                break;
                            case 0x11300000:
                                error = Properties.Resources.ErrorCipStatus13;
                                break;
                            case 0x11500000:
                                error = Properties.Resources.ErrorCipStatus15;
                                break;
                            case 0x11F00000:
                                error = Properties.Resources.ErrorCipStatus1F;
                                break;
                            case 0x12000000:
                                error = Properties.Resources.ErrorCipStatus20;
                                break;
                        }

                        if((errorcode & 0x00010000) != 0)
                        {
                            quality = StatusCodes.BadCommunicationError;
                            switch (errorcode & 0x000FFFFF)
                            {
                                case 0x00010102:
                                    error += Properties.Resources.ErrorCipAddStatus102;
                                    break;
                                case 0x00010104:
                                    error += Properties.Resources.ErrorCipAddStatus104;
                                    break;
                                case 0x00011103:
                                    error += Properties.Resources.ErrorCipAddStatus1103;
                                    break;
                                case 0x00012103:
                                    error += Properties.Resources.ErrorCipAddStatus2103;
                                    break;
                                case 0x00012104:
                                    error += Properties.Resources.ErrorCipAddStatus2104;
                                    break;
                                case 0x00018001:
                                    error += Properties.Resources.ErrorCipAddStatus8001;
                                    break;
                                case 0x00018007:
                                    error += Properties.Resources.ErrorCipAddStatus8007;
                                    break;
                                case 0x00018009:
                                    error += Properties.Resources.ErrorCipAddStatus8009;
                                    break;
                                case 0x0001800F:
                                    error += Properties.Resources.ErrorCipAddStatus800F;
                                    break;
                                case 0x00018010:
                                    error += Properties.Resources.ErrorCipAddStatus8010;
                                    break;
                                case 0x00018011:
                                    error += Properties.Resources.ErrorCipAddStatus8011;
                                    break;
                                case 0x00018017:
                                    error += Properties.Resources.ErrorCipAddStatus8017;
                                    break;
                                case 0x00018018:
                                    error += Properties.Resources.ErrorCipAddStatus8018;
                                    break;
                                case 0x00018021:
                                    error += Properties.Resources.ErrorCipAddStatus8021;
                                    break;
                                case 0x00018022:
                                    error += Properties.Resources.ErrorCipAddStatus8022;
                                    break;
                                case 0x00018023:
                                    error += Properties.Resources.ErrorCipAddStatus8023;
                                    break;
                                case 0x00018024:
                                    error += Properties.Resources.ErrorCipAddStatus8024;
                                    break;
                                case 0x00018025:
                                    error += Properties.Resources.ErrorCipAddStatus8025;
                                    break;
                                case 0x00018027:
                                    error += Properties.Resources.ErrorCipAddStatus8027;
                                    break;
                                case 0x00018028:
                                    error += Properties.Resources.ErrorCipAddStatus8028;
                                    break;
                                case 0x00018029:
                                    error += Properties.Resources.ErrorCipAddStatus8029;
                                    break;
                                case 0x00018031:
                                    error += Properties.Resources.ErrorCipAddStatus8031;
                                    break;
                            }
                        }
                    }
                    break;
            }
        }
        #endregion
    }
}
