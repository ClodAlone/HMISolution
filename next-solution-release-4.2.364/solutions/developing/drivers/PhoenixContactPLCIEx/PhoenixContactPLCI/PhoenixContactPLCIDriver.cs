using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace PhoenixContactPLCI
{
    public class PhoenixContactPLCIDriver : CommunicationDriver
    {
        public PhoenixContactPLCIDriver()
            : base()
        {
        }

        // Used only to import data from PLC 
        public PhoenixContactPLCIDriver(string strSettingPath)
            : base()
        {
            _strSettingPath = strSettingPath;
        }


        #region Methods
        public override bool TestComm(ChannelSettings ch, StationSettings st, out string error)
        {
            error = string.Empty;

            if (!IsRunningEnviroment32Bit())
            {
                error = Properties.Resources.ErrorDriverSupportOnly32bitEnvironment;
                return false;
            }

            Channel channel = CreateChannel(ch);
            Channels[channel.Name] = channel;
            Channels[channel.Name].Init();

            Station station = CreateStation(st);
            Stations[station.Name] = station;
            Stations[station.Name].Init(channel);

            DriverErrorCodes conn = channel.CheckDevice(null, channel);
            if (conn == DriverErrorCodes.ErrorNoError)
            {
                if (!channel.IsDeviceOpen())
                    conn = DriverErrorCodes.ErrorDeviceOpenFailed;
            }
            if (conn != DriverErrorCodes.ErrorNoError)
                GetDriverErrorInfo((int)conn, out uint quality, out error);

            channel.DeviceClose();

            return (conn == DriverErrorCodes.ErrorNoError);
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
            if (!base.Init(strSettingPath, isProtected, protectionCode))
                return false;

            if (!IsRunningEnviroment32Bit())
            {
                OnSystemEvent(ObjectIds.Server, Properties.Resources.ErrorDriverSupportOnly32bitEnvironment, EventSeverity.Max);
                return false;
            }
            return true;
        }

        public override bool LoadDriverSettings(IDataLayer idl)
        {
            bool bRet = false;
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                try
                {
                    var configuration = (from tag in new XPQuery<PhoenixContactPLCIDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as PhoenixContactPLCIDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<PhoenixContactPLCIDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new PhoenixContactPLCIDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as PhoenixContactPLCIDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as PhoenixContactPLCIChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new PhoenixContactPLCIChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as PhoenixContactPLCIStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new PhoenixContactPLCIStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new PhoenixContactPLCITag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new PhoenixContactPLCITag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as PhoenixContactPLCITag;
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
                return new List<ChangeTag>((new XPQuery<PhoenixContactPLCIChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);
            if (errorcode <= 6)
            {
                return;
            }

            if (errorcode == (int)DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorParsingAnswer)
            {
                quality = StatusCodes.BadOutOfRange;                
                return;
            }

            quality = StatusCodes.BadCommunicationError;
            switch ((PhoenixContactPLCIProtocol.ErrorCodes)errorcode)
            {                
                case PhoenixContactPLCIProtocol.ErrorCodes.ErrorConnectionBroken:
                    error = Properties.Resources.ErrorConnection;
                    break;
                case PhoenixContactPLCIProtocol.ErrorCodes.ErrorUnMappedTag:
                    quality = StatusCodes.BadNotFound;
                    error = Properties.Resources.ErrorUnmappedTag;
                    break;                                
                case PhoenixContactPLCIProtocol.ErrorCodes.ErrorInvalidDataFormat:                
                    quality = StatusCodes.BadTypeDefinitionInvalid;
                    error = Properties.Resources.ErrorInvalidDataFormat;
                    break;                
                case PhoenixContactPLCIProtocol.ErrorCodes.ErrorInvalidArraySize:
                    quality = StatusCodes.BadTypeDefinitionInvalid;
                    error = Properties.Resources.ErrorInvalidArraySize;
                    break;
                default:                    
                    error = ((PhoenixContactPLCIProtocol.ErrorCodes)errorcode).ToString();
                    break;
            }
        }

        public PhoenixContactPLCIChannel GetChannelFromStation(string stationName)
        {
           PhoenixContactPLCIChannel resultChannel = null;

           foreach (var ch in GetChannels()) {
                Station st = GetChannelStations(ch).Find(s => s.Name == stationName);
                if (st != null)
                {
                    resultChannel = (PhoenixContactPLCIChannel)ch;
                    break;
                }
            }

            return resultChannel;
        }

        public static bool IsRunningEnviroment32Bit()
        {
            //32bit bit enviroment check
            //return (IntPtr.Size == 4);
            return (!Environment.Is64BitProcess);
        }
        #endregion

        #region Properties
        private string _strSettingPath;
        public string strSettingPath
        {
            get { return _strSettingPath; }
        }
        #endregion
    }
}
