using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DriverSettingsInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace CoDeSys
{
    public class CoDeSysDriver : CommunicationDriver, ICommunicationDriverWebEditing
    {
        public CoDeSysDriver()
            : base()
        {
        }

        public CoDeSysDriver(string strSettingPath)
            : base(strSettingPath)
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
                return false;

            //32bit do not yet supported
            if (!Environment.Is64BitProcess)
            {
                OnSystemEvent(ObjectIds.Server, Properties.Resources.Error32bitEnvironmentUnsupported, EventSeverity.Max);
                return false;
            }

            //test codesys library and codesys wrapper presence; if one of these is not present, stop driver
            CoDeSysPLCHandlerWrapper Wrapper = new CoDeSysPLCHandlerWrapper();
            if (!Wrapper.IsWrapperInstalled())
            {
                //throw new LicenseOptionMissingException(Properties.Resources.ErrorCoDeSysWrapperNotInstalled);
                OnSystemEvent(ObjectIds.Server, Properties.Resources.ErrorCoDeSysWrapperNotInstalled, EventSeverity.Max);
                return false;
            }

            if (!Wrapper.IsCoDeSysInstalled())
            {
                //throw new LicenseOptionMissingException(Properties.Resources.ErrorCoDeSysNotInstalled);
                OnSystemEvent(ObjectIds.Server, Properties.Resources.ErrorCoDeSysNotInstalled, EventSeverity.Max);
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
                    var configuration = (from tag in new XPQuery<CoDeSysDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as CoDeSysDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<CoDeSysDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new CoDeSysDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as CoDeSysDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra EtherNetIP Driver Settings initializzation
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as CoDeSysChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new CoDeSysChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as CoDeSysStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new CoDeSysStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new CoDeSysTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new CoDeSysTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as CoDeSysTag;
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
                return new List<ChangeTag>((new XPQuery<CoDeSysChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
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
            switch ((CoDeSysProtocol.CoDeSysErrorCodes)errorcode)
            {                
                case CoDeSysProtocol.CoDeSysErrorCodes.ErrorConnectionBroken:
                    error = Properties.Resources.ErrorConnection;
                    break;
                case CoDeSysProtocol.CoDeSysErrorCodes.ErrorUnmappedTag:
                    quality = StatusCodes.BadNotFound;
                    error = Properties.Resources.ErrorUnmappedTag;
                    break;
                case CoDeSysProtocol.CoDeSysErrorCodes.ErrorArraySizeBigger:
                    quality = StatusCodes.BadBoundNotSupported;
                    error = Properties.Resources.ErrorArraySizeBigger;
                    break;
                case CoDeSysProtocol.CoDeSysErrorCodes.ErrorWriteArraySizeSmaller:
                    quality = StatusCodes.BadBoundNotSupported;
                    error = Properties.Resources.ErrorWriteArraySizeSmaller;
                    break;
                case CoDeSysProtocol.CoDeSysErrorCodes.ErrorWrapperCannotCreateStation:
                    quality = StatusCodes.BadDeviceFailure;
                    error = Properties.Resources.ErrorCoDeSysWrapperCannotCreateStation;
                    break;
                case CoDeSysProtocol.CoDeSysErrorCodes.ErrorNoUpdate:
                    quality = StatusCodes.BadNotReadable;
                    error = Properties.Resources.ErrorNoUpdate;
                    break;
                case CoDeSysProtocol.CoDeSysErrorCodes.ErrorNoVariableListRead:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorConnection;
                    break;
                default:
                    CoDeSysProtocol.PLCHandlerErrors Ret = (CoDeSysProtocol.PLCHandlerErrors)CoDeSysProtocol.UnShiftErrorCode(errorcode);
                    error = Ret.ToString();
                    break;
            }
        }
        #endregion

        #region ICommunicationDriverWebEditing Interface
        object ICommunicationDriverWebEditing.GeneralSettingsEditor
        {
            get
            {
                return null;
            }
        }

        public static CoDeSysDynTagSettings DynSettings;// = new EtherNetIPDynTagSettings("EtherNetIP");
        object ICommunicationDriverWebEditing.DynamicSettingsEditor(string dynamicSettings)
        {
            if (DynSettings == null)
                DynSettings = new CoDeSysDynTagSettings();
            DynSettings.Parse(dynamicSettings);

            return DynSettings;
        }

        public string GetDynamicSettings(object value)
        {
            var dynsettings = value as CoDeSysDynTagSettings;
            if (dynsettings == null)
                return String.Empty;

            return dynsettings.ToString();
        }


        public object ImportTagsEditor
        {
            get { return null; }
        }

        #endregion

        //#region TestComm
        //public override bool TestComm(ChannelSettings ch, StationSettings st, out string error)
        //{
        //    error = null;

        //    Channel channel = CreateChannel(ch);
        //    Channels[channel.Name] = channel;
        //    Channels[channel.Name].Init();

        //    Station station = CreateStation(st);
        //    Stations[station.Name] = station;
        //    Stations[station.Name].Init(channel);

        //    ((CoDeSysChannel)channel).InitPLCHandler();
        //    DriverErrorCodes conn = channel.CheckDevice(null, channel);
        //    if (conn != DriverErrorCodes.ErrorNoError)
        //        GetDriverErrorInfo((int)conn, out uint quality, out error);

        //    channel.DeviceClose();

        //    return (conn == DriverErrorCodes.ErrorNoError);
        //}
        //#endregion
    }
}
