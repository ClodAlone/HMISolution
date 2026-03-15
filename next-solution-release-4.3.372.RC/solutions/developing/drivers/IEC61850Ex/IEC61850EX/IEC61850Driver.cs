using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace IEC61850
{
    public class IEC61850Driver : CommunicationDriver
    {
        public IEC61850Driver()
            : base()
        {
        }

        public IEC61850Driver(string strSettingPath)
            : base(strSettingPath)
        {
        }

        #region Overrides
        public override bool Init(string strSettingPath, bool isProtected, Guid protectionCode)
        {
#if !DEBUG
            if (!MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxMjY539eIREBouUCiSTGu7w=="/*TLM*/))
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
                    var configuration = (from tag in new XPQuery<IEC61850DriverSettings>(ufw).AsParallel() select tag).Single();
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
        //public override bool IsPrototypeSplitEnabled()
        //{
        //    return true;
        //}
        public override void LoadDriverSettings(DriverSettings settings)
        {
            base.LoadDriverSettings(settings);

            var conf = settings as IEC61850DriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra IEC 61850 Driver Settings initialization
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<IEC61850DriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new IEC61850DriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as IEC61850DriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra IEC 61850 Driver Settings initialization
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as IEC61850ChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new IEC61850Channel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as IEC61850StationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new IEC61850Station(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new IEC61850Tag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new IEC61850Tag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as IEC61850Tag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }
         public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<IEC61850ChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);
            if ((IEC61850ErrorCodes)errorcode == IEC61850ErrorCodes.ErrorConnectFailed)
            {
                quality = StatusCodes.BadCommunicationError;
                error = Properties.Resources.IEC61850ConnectionFailed;
            }
            else if ((IEC61850ErrorCodes)errorcode == IEC61850ErrorCodes.ErrorUnexpectedReply)
            {
                quality = StatusCodes.BadCommunicationError;
                error = Properties.Resources.IEC61850ErrorUnexpectedReply;
            }
            else if((IEC61850ErrorCodes)errorcode == IEC61850ErrorCodes.ErrorParsingError)
            {
                quality = StatusCodes.BadCommunicationError;
                error = Properties.Resources.IEC61850ErrorParsing;
            }
            else if ((IEC61850ErrorCodes)errorcode == IEC61850ErrorCodes.ErrorTimeout)
            {
                quality = StatusCodes.BadTimeout;
                error = Properties.Resources.IEC61850ErrorTimeout;
            }
            else if ((IEC61850ErrorCodes)errorcode >= IEC61850ErrorCodes.ErrorDataAccessError)
            {
                quality = StatusCodes.BadCommunicationError;
                error = string.Format(Properties.Resources.IEC61850ErrorDataAccess, errorcode - IEC61850ErrorCodes.ErrorDataAccessError);
            }
        }

        public override bool TestComm(ChannelSettings ch, StationSettings st, out string error)
        {
            error = null;

            Channel channel = CreateChannel(ch);
            Channels[channel.Name] = channel;
            Channels[channel.Name].Init();

            Station station = CreateStation(st);
            Stations[station.Name] = station;
            Stations[station.Name].Init(channel);


            DriverErrorCodes conn = DriverErrorCodes.ErrorDeviceOpenFailed;

            if (((IEC61850Channel)channel).SetIec61850Station())
            {
                conn = channel.CheckDevice(null, channel);
                if (conn != DriverErrorCodes.ErrorNoError)
                    GetDriverErrorInfo((int)conn, out uint quality, out error);

                channel.DeviceClose();
            }

            return (conn == DriverErrorCodes.ErrorNoError);
        }

        #endregion

    }
}
