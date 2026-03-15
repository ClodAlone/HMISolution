using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using System.Xml.Linq;
using DriverCodeBaseEx.Enumerators;
using Utilities;

namespace MTConnect
{
    public class MTConnectDriver : CommunicationDriver
    {
        public MTConnectDriver()
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
            if (!base.Init(strSettingPath, isProtected, protectionCode))
            {
                return (false);
            }

            // Driver specific:

            return (true);
        }

        // Used only to import data from PLC 
        public MTConnectDriver(string strSettingPath)
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
                    var configuration = (from tag in new XPQuery<MTConnectDriverSettings>(ufw).AsParallel() select tag).Single();
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

        public override bool IsPrototypeSplitEnabled()
        {
            return true;
        }

        public override void LoadDriverSettings(DriverSettings settings)
        {
            base.LoadDriverSettings(settings);

            var conf = settings as MTConnectDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<MTConnectDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new MTConnectDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as MTConnectDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
          
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as MTConnectChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new MTConnectChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as MTConnectStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new MTConnectStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new MTConnectTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new MTConnectTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as MTConnectTag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }

        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<MTConnectChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((MTConnectProtocol.MTConnectErrorCodes)errorcode)
            {
                //case MTConnectProtocol.MTConnectErrorCodes.ErrorCodeErrorPreparingPublishMessage:
                //    quality = StatusCodes.BadCommunicationError;
                //    error = Properties.Resources.MTConnectErrorPreparingPublishMessage;
                //    break;
                case MTConnectProtocol.MTConnectErrorCodes.ErrorCodeConvertValue:
                    quality = StatusCodes.BadDataUnavailable;
                    error = Properties.Resources.MTConnectErrorGettingRemoteDateTime;
                    break;
                case MTConnectProtocol.MTConnectErrorCodes.ErrorCodeEthernetCardConnection:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.MTConnectErrorEthernetCardNotAvailable;
                    break;
                case MTConnectProtocol.MTConnectErrorCodes.ErrorCodeNoInternetConnection:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.MTConnectErrorInternetNotAvailable;
                    break;
                case MTConnectProtocol.MTConnectErrorCodes.ErrorCodeTagNoPresent:
                    quality = StatusCodes.BadDataUnavailable;
                    error = Properties.Resources.MTConnectErrorPreparingPublishMessage;
                    break;
            }
        }

        public override bool TestComm(ChannelSettings ch, StationSettings st, out string error)
        {
            DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;
            error = null;
            using (new WaitCursor())
            {
                
                Channel channel = CreateChannel(ch);
                Channels[channel.Name] = channel;
                Channels[channel.Name].Init();

                Station station = CreateStation(st);
                Stations[station.Name] = station;
                Stations[station.Name].Init(channel);
                XElement doc = ((MTConnectChannel)channel).GetCommandProbeForTest(((MTConnectChannel)channel).ServerAddress, ((MTConnectChannel)channel).ServerPort, out error);
                if (doc == null)
                {
                    conn = (DriverErrorCodes)MTConnectProtocol.MTConnectErrorCodes.ErrorCodeNoInternetConnection;
                }

                channel.DeviceClose();
            }

            return (conn == DriverErrorCodes.ErrorNoError);
        }

        #endregion

        #region Properties


        #endregion
    }
}
