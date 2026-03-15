using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;


namespace ROCDriver
{
    public class ROCDriverDriver : CommunicationDriver
    {
        #region Overrides
        public ROCDriverDriver()
        : base()
        {
        }

        public ROCDriverDriver(string strSettingPath)
            : base()
        {
        }

        public override bool Init(string strSettingPath, bool isProtected, Guid protectionCode)
        {
#if !DEBUG
            if (!MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx1e5QOMTYV9atSLB7Ev1DDA=="/*DVP*/))
            {
                throw new LicenseOptionMissingException("Missing 'DVP' option in the license");
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
                    var configuration = (from tag in new XPQuery<ROCDriverDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as ROCDriverDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<ROCDriverDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new ROCDriverDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as ROCDriverDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as ROCDriverChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new ROCDriverChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as ROCDriverStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new ROCDriverStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new ROCDriverTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new ROCDriverTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as ROCDriverTag;
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
                return new List<ChangeTag>((new XPQuery<ROCDriverChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
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

            // Enable test execution
            ((ROCDriverChannel)channel).TestConnection = true;

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
                error = ((StatusCode)conn).ToString();

            // Disable test execution
            ((ROCDriverChannel)channel).TestConnection = false;

            return (conn == StatusCodes.Good);
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((ROCDriverErrorCodes)errorcode)
            {
                case ROCDriverErrorCodes.ErrorIncompleteReply:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorIncompleteReply;
                    break;
                case ROCDriverErrorCodes.ErrorInvalidHeader:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorInvalidHeader;
                    break;
                case ROCDriverErrorCodes.ErrorUnexpectedOpcode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnexpectedOpcode;
                    break;
                case ROCDriverErrorCodes.ErrorMissingData:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorMissingData;
                    break;
                case ROCDriverErrorCodes.ErrorCrc:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorCrc;
                    break;
                case ROCDriverErrorCodes.ErrorMalformattedErrorMessage:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorMalformattedErrorMessage;
                    break;
                case ROCDriverErrorCodes.ErrorUnexpectedNumberOfParameters:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnexpectedNumberOfParameters;
                    break;
                case ROCDriverErrorCodes.ErrorParsingParameterData:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorParsingParameterData;
                    break;
                case ROCDriverErrorCodes.ErrorParseFailure:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorParseFailure;
                    break;
                case ROCDriverErrorCodes.ErrorTLPMismatch:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorTLPMismatch;
                    break;
                case ROCDriverErrorCodes.ErrorMalformattedReply:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorMalformattedReply;
                    break;
                case ROCDriverErrorCodes.ErrorInConnectionCheck:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorInConnectionCheck;
                    break;
                case ROCDriverErrorCodes.ErrorReadReplyLength:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReadReplyLength;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode1:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode1;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode2:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode2;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode3:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode3;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode4:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode4;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode5:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode5;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode6:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode6;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode12:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode12;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode13:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode13;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode14:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode14;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode15:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode15;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode16:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode16;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode17:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode17;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode18:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode18;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode19:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode19;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode20:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode20;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode21:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode21;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode22:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode22;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode24:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode24;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode25:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode25;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode29:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode29;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode30:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode30;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode31:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode31;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode32:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode32;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode33:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode33;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode34:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode34;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode50:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode50;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode51:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode51;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode52:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode52;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode61:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode61;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode62:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode62;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode63:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode63;
                    break;
                case ROCDriverErrorCodes.ErrorErrorCode77:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCode77;
                    break;
            }
        }
        #endregion
    }
}
