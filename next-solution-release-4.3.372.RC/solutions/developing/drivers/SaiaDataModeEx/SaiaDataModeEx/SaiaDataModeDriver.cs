using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using IpDriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using DevExpress.Utils.About;
using DriverCodeBaseEx.Enumerators;

namespace SaiaDataMode
{
    public class SaiaDataModeDriver : CommunicationDriver
    {
        public static string info;
        #region Overrides

        public SaiaDataModeDriver()
            : base()
        {
        }

        public SaiaDataModeDriver(string strSettingPath)
            : base()
        {
        }

        public override bool Init(string strSettingPath, bool isProtected, Guid protectionCode)
        {
#if !DEBUG
            if (!MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxwApi90eGlNmfe04G5G+6Aw=="/*AUT*/))
            {
                throw new LicenseOptionMissingException("Missing 'AUT' option in the license");
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
                    var configuration = (from tag in new XPQuery<SaiaDataModeDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as SaiaDataModeDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<SaiaDataModeDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new SaiaDataModeDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as SaiaDataModeDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as SaiaDataModeChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new SaiaDataModeChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as SaiaDataModeStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new SaiaDataModeStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new SaiaDataModeTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new SaiaDataModeTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as SaiaDataModeTag;
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
                return new List<ChangeTag>((new XPQuery<SaiaDataModeChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
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

            var conf = ch as SaiaDataModeChannelSettings;
            //Enable test execution
            if ((conf != null) && (conf.ChannelType == ChannelTypes.Socket))
            {
                ((SaiaDataModeUdpChannel)((SaiaDataModeChannel)channel).ChildChannel).TestConnection = true;
            }
            else
            {
                ((SaiaDataModeSerialChannel)((SaiaDataModeChannel)channel).ChildChannel).TestConnection = true;
            }

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
            if ((conf != null) && (conf.ChannelType == ChannelTypes.Socket))
            {
                ((SaiaDataModeUdpChannel)((SaiaDataModeChannel)channel).ChildChannel).TestConnection = false;
            }
            else
            {
                ((SaiaDataModeSerialChannel)((SaiaDataModeChannel)channel).ChildChannel).TestConnection = false;
            }
            return (conn == StatusCodes.Good);

        }
        public static void Addinfo(string stinfo)
        {
            if (string.IsNullOrWhiteSpace(info))
                info = stinfo;
            else
                info = string.Format("{0}\n{1}", info, stinfo);
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((SaiaDataModetErrorCodes)errorcode)
            {
                case SaiaDataModetErrorCodes.ErrorInvalidLenght:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorInvalidLenght;
                    break;
                case SaiaDataModetErrorCodes.ErrorIncompleteReply:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorIncompleteReply;
                    break;
                case SaiaDataModetErrorCodes.ErrorCrc:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorCrc;
                    break;
                case SaiaDataModetErrorCodes.ErrorWrongSequence:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWrongSequence;
                    break;
                case SaiaDataModetErrorCodes.ErrorWriteResponseAttribute:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWriteResponseAttribute;
                    break;
                case SaiaDataModetErrorCodes.ErrorReadResponseAttribute:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReadResponseAttribute;
                    break;
                case SaiaDataModetErrorCodes.ErrorReadResponseNack:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReadResponseNack;
                    break;
                case SaiaDataModetErrorCodes.ErrorReadResponseLength:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReadResponseLength;
                    break;
            }
        }        
        #endregion
    }
}
