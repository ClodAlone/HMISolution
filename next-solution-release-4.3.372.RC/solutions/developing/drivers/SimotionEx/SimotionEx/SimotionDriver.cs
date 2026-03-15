using System;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using Accon.AGLink;
using System.Collections.Generic;
using DriverCodeBaseEx.Enumerators;

namespace Simotion
{
    public class SimotionDriver : CommunicationDriver
    {
        public SimotionDriver()
            : base()
        {
        }
        #region Overrides
        public override bool Init(string strSettingPath, bool isProtected, Guid protectionCode)
        {
#if !DEBUG
            if (!MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxwApi90eGlNmfe04G5G+6Aw=="/*AUT*/))
            {
                throw new LicenseOptionMissingException("Missing 'AUT' option in the license");
            }
#endif
            try
            {
                // Set the library license code
                AGL4.Activate("00BEF5-E1F5-260260");
            }
            catch
            {
                OnSystemEvent(ObjectIds.Server, String.Format(Properties.Resources.AGLink40NotFound), EventSeverity.Max);
                return false;
            }

            return base.Init(strSettingPath, isProtected, protectionCode);
        }

        // Used only to import data from PLC 
        public SimotionDriver(string strSettingPath)
            : base(strSettingPath)
        {            
        }

        public override bool LoadDriverSettings(IDataLayer idl)
        {
            bool bRet = false;
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                try
                {
                    var configuration = (from tag in new XPQuery<SimotionDriverSettings>(ufw).AsParallel() select tag).Single();
                    LoadDriverSettings(configuration);
                    bRet = true;
                }
                catch (InvalidOperationException ex)
                {
                    OnSystemEvent(ObjectIds.Server, String.Format(Properties.Resources.ErrorLoadingDriverSettings, ex.Message), EventSeverity.Min);
                    LoadDefaultSettings();
                    bRet = true;
                }
            }

            return bRet;
        }
        
        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<SimotionDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new SimotionDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as SimotionDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as SimotionChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new SimotionChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as SimotionStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new SimotionStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new SimotionTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new SimotionTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as SimotionTag;
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
                return new List<ChangeTag>((new XPQuery<SimotionChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((S7ErrorCodes)errorcode)
            {
                case S7ErrorCodes.ErrorWrongHeader:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWrongHeader;
                    break;
                case S7ErrorCodes.ErrorTooFewData:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorTooFewData;
                    break;
                case S7ErrorCodes.ErrorTooData:
                    quality = StatusCodes.BadConfigurationError;
                    error = Properties.Resources.ErrorTooData;
                    break;
                case S7ErrorCodes.ErrorTagDataTypeTooSmal:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorTagDataTypeTooSmal;
                    break;
                case S7ErrorCodes.ErrorConnectionToDevice:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorConnection;
                    break;
            }

            if ((S7ErrorCodes)errorcode == S7ErrorCodes.ErrorAGLinkTiaFileNotPresent)
            {
                quality = StatusCodes.BadResourceUnavailable;
                error = string.Format(Properties.Resources.ErrorSymbolFileNotFound, string.Empty);
            }
            else if ((S7ErrorCodes)errorcode > S7ErrorCodes.ErrorFromDevice)
            {
                quality = StatusCodes.BadCommunicationError;
                error = string.Format(Properties.Resources.ErrorFromDevice, SimotionProtocol.GetErrorString(errorcode - (int)S7ErrorCodes.ErrorFromDevice));
            }
            else if ((S7ErrorCodes)errorcode > S7ErrorCodes.ErrorStatusNotZero)
            {
                quality = StatusCodes.BadCommunicationError;
                error = string.Format(Properties.Resources.ErrorStatusNotZero, SimotionProtocol.GetErrorString(errorcode - (int)S7ErrorCodes.ErrorStatusNotZero));
            }
        }

        #endregion

        #region Methods
        public override bool TestComm(ChannelSettings ch, StationSettings st, out string error)
        {
            error = string.Empty;

            try
            {
                // Set the library license code
                AGL4.Activate("00BEF5-E1F5-260260");
            }
            catch
            {
                error = new StatusCode(StatusCodes.Bad).ToString();
                return false;
            }

            Channel channel = CreateChannel(ch);
            Channels[channel.Name] = channel;
            Channels[channel.Name].Init();

            Station station = CreateStation(st);
            Stations[station.Name] = station;
            Stations[station.Name].Init(channel);

            // init is required for retrive specific station parameters            
            DriverErrorCodes conn = channel.CheckDevice(null, channel);
            if (conn != DriverErrorCodes.ErrorNoError)
                GetDriverErrorInfo((int)conn, out uint quality, out error);
            else
            {
                string M;
                int ret = AGL4.ReadMLFBNr((int)conn, out M, 1);
                if (ret != AGL4.AGL40_SUCCESS)
                    GetDriverErrorInfo((int)conn, out uint quality, out error);
            }
            channel.DeviceClose();

            return (conn == DriverErrorCodes.ErrorNoError);
        }
        #endregion
    }
}
