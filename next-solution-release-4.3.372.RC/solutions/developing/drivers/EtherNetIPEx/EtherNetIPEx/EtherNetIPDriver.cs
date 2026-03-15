using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace EtherNetIP
{
    public class EtherNetIPDriver : CommunicationDriver
    {
        public static string info;
        public EtherNetIPDriver()
            : base()
        {
        }

        // Used only to import data from PLC 
        public EtherNetIPDriver(string strSettingPath)
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
#if DEBUG
            //using (UnitOfWork ufw = new UnitOfWork(idl))
            //{
            //}
#endif
            bool bRet = false;
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                try
                {
                    var configuration = (from tag in new XPQuery<EtherNetIPDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as EtherNetIPDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<EtherNetIPDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new EtherNetIPDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as EtherNetIPDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra EtherNetIP Driver Settings initializzation
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as EtherNetIPChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new EtherNetIPChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as EtherNetIPStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new EtherNetIPStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new EtherNetIPTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new EtherNetIPTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as EtherNetIPTag;
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
                return new List<ChangeTag>((new XPQuery<EtherNetIPChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((EtherNetIpErrorCodes)errorcode)
            {
                case EtherNetIpErrorCodes.ErrorWrongTransaction:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWrongTransaction;
                    break;
                case EtherNetIpErrorCodes.ErrorRepTooShort:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRepTooShort;
                    break;
                case EtherNetIpErrorCodes.ErrorRepTooLong:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRepTooLong;
                    break;
                case EtherNetIpErrorCodes.ErrorRepTagSize:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRepTagSize;
                    break;
                case EtherNetIpErrorCodes.ErrorNoRep:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorNoRep;
                    break;
                case EtherNetIpErrorCodes.ErrorRepDataType:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRepDataType;
                    break;
                case EtherNetIpErrorCodes.ErrorWrongMRServiceReplyCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWrongMRServiceReplyCode;
                    break;
                //case EtherNetIpErrorCodes.ErrorMRStatus:
                //    quality = StatusCodes.BadCommunicationError;
                //    error = Properties.Resources.ErrorMRStatus;
                //    break;
                case EtherNetIpErrorCodes.ErrorWrongPCCCRespCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWrongPCCCRespCode;
                    break;
                case EtherNetIpErrorCodes.ErrorRepSerNum:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRepSerNum;
                    break;
                case EtherNetIpErrorCodes.ErrorRepSerCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRepSerCode;
                    break;
                case EtherNetIpErrorCodes.ErrorRepStatus:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRepStatus;
                    break;
                case EtherNetIpErrorCodes.ErrorStatusNotZero:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorStatusNotZero;
                    break;
                case EtherNetIpErrorCodes.ErrorUnknSTSEXTCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnknSTSEXTCode;
                    break;
                case EtherNetIpErrorCodes.ErrorEXTSTSCodeExt:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorEXTSTSCodeExt;
                    break;
                case EtherNetIpErrorCodes.ErrorUnknSTSCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnknSTSCode;
                    break;
                case EtherNetIpErrorCodes.ErrorOutOfSync:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorOutOfSync;
                    break;
                case EtherNetIpErrorCodes.ErrorEncapStatus1:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorEncapStatus1;
                    break;
                case EtherNetIpErrorCodes.ErrorEncapStatus2:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorEncapStatus2;
                    break;
                case EtherNetIpErrorCodes.ErrorEncapStatus3:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorEncapStatus3;
                    break;
                case EtherNetIpErrorCodes.ErrorEncapStatus64:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorEncapStatus64;
                    break;
                case EtherNetIpErrorCodes.ErrorEncapStatus65:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorEncapStatus65;
                    break;
                case EtherNetIpErrorCodes.ErrorEncapStatus69:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorEncapStatus69;
                    break;
                case EtherNetIpErrorCodes.ErrorUnknEncapStatus:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnknEncapStatus;
                    break;
                case EtherNetIpErrorCodes.ErrorEnbeddedStation:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorEnbeddedStation;
                    break;
            }
            if((EtherNetIpErrorCodes)errorcode > EtherNetIpErrorCodes.ErrorMRStatus)
            {
                quality = StatusCodes.BadCommunicationError;
                error = string.Format(Properties.Resources.ErrorMRStatus, errorcode - EtherNetIpErrorCodes.ErrorMRStatus);

            }
        }

        #endregion


        #region Methods
        public bool TestComm(EtherNetIPChannelSettings ch, EtherNetIPStationSettings st, out string error)
        {
            bool result = false;
            error = string.Empty;

            Channel channel = CreateChannel(ch);
            Channels[channel.Name] = channel;
            Channels[channel.Name].Init();

            EtherNetIPDriver.info = string.Empty;
            EtherNetIPDriver.TestConnection = true;

            Station station = CreateStation(st);
            Stations[station.Name] = station;
            Stations[station.Name].Init(channel);
                        
            switch (((EtherNetIPStation)station).PlcType)
            {
                case PlcTypes.ControlLogix_CompactLogix:
                case PlcTypes.Micro800_series:
                case PlcTypes.SLC500_MicroLogix:
                    {
                        // request to PLC some info
                        EtherNetIPCommJob job = new EtherNetIPCommJob(station);
                        job.AddressType = AddressTypes.TagName;
                        DriverErrorCodes conn = ((EtherNetIPChannel)channel).CheckDevice(new List<CommJob> { job }, channel);
                        if (conn != DriverErrorCodes.ErrorNoError)
                        {
                            GetDriverErrorInfo((int)conn, out uint quality, out error);
                            info = error;
                        }

                        error = info;
                        info = string.Empty;
                        channel.DeviceClose();
                        EtherNetIPDriver.TestConnection = false;
                        result = (conn == DriverErrorCodes.ErrorNoError);
                    }
                    break;                
                case PlcTypes.PLC5:
                    {
                        // absolute address                        
                        IList<object> dataValues = new List<Object>();
                        dataValues.Add(new Variant((Int16)0, new TypeInfo(BuiltInType.Int16, -1)));
                        uint conn = OnReadValues(string.Format(EtherNetIpProtocol.TEST_COMM_DYNAMIC_SETTINGS_ABSOLUTE, station.Name), dataValues);
                        if (conn != StatusCodes.Good)
                            error = new StatusCode(conn).ToString();
                        EtherNetIPDriver.TestConnection = false;
                        result = (conn == StatusCodes.Good);
                    }
                    break;
            }
            return result;
        }

        private static object lockTestConnection = new object();
        public static void Addinfo(string stinfo)
        {
            if (TestConnection)
            {
                lock(lockTestConnection)
                {
                    if (string.IsNullOrWhiteSpace(info))
                        info = stinfo;
                    else
                        info = string.Format("{0}\n{1}", info, stinfo);
                }
            }
        }
        #endregion

        #region
        public static bool TestConnection = false;
        #endregion
    }
}
