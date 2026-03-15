using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using DriverSettingsInterfaces;

namespace FanucCNC
{
    public class FanucCNCDriver : CommunicationDriver, ICommunicationDriverWebEditing
    {
        public FanucCNCDriver()
            : base()
        {
        }

        // Used only to import data from PLC 
        public FanucCNCDriver(string strSettingPath)
            : base()
        {
            _strSettingPath = strSettingPath;
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

            if (!IsRunningEnviroment32Bit())
            {
                OnSystemEvent(ObjectIds.Server, Properties.Resources.ErrorDriverSupportOnly32bitEnvironment, EventSeverity.Max);
                return false;
            }

            // add to DLLImport function Movicon\Driver search path
            FanucCNCProtocol.AddDllFocasLibrarySearchPath();

            //test if all focas library were installed
            if (!FanucCNCProtocol.CheckFocasLibrary())
            {
                OnSystemEvent(ObjectIds.Server, Properties.Resources.ErrorFocasLibraryNotInstalled, EventSeverity.Max);
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
                    var configuration = (from tag in new XPQuery<FanucCNCDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as FanucCNCDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<FanucCNCDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new FanucCNCDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as FanucCNCDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as FanucCNCChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new FanucCNCChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as FanucCNCStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new FanucCNCStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new FanucCNCTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new FanucCNCTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as FanucCNCTag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }

        public override bool IsPrototypeSplitEnabled()
        {
            return false;
        }

        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<FanucCNCChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);
            if (errorcode <= 6)
            {
                return;
            }

            if (errorcode == (int)DriverCodeBase.Enumerators.DriverErrorCodes.ErrorParsingAnswer)
            {
                quality = StatusCodes.BadOutOfRange;                
                return;
            }

            //quality = StatusCodes.q;            
            switch ((FanucCNCProtocol.ErrorCodes)errorcode)
            {
                case FanucCNCProtocol.ErrorCodes.ErrorConnectionBroken:
                    //error = Properties.Resources.ErrorConnection;
                    error = ((FanucCNCProtocol.ErrorCodes)errorcode).ToString();
                    quality = StatusCodes.BadNotConnected;
                    break;
            }
            //    case FanucCNCProtocol.ErrorCodes.ErrorUnMappedTag:
            //        quality = StatusCodes.BadNotFound;
            //        error = Properties.Resources.ErrorUnmappedTag;
            //        break;                                
            //    case FanucCNCProtocol.ErrorCodes.ErrorInvalidDataFormat:                
            //        quality = StatusCodes.BadTypeDefinitionInvalid;
            //        error = Properties.Resources.ErrorInvalidDataFormat;
            //        break;                
            //    case FanucCNCProtocol.ErrorCodes.ErrorInvalidArraySize:
            //        quality = StatusCodes.BadTypeDefinitionInvalid;
            //        error = Properties.Resources.ErrorInvalidArraySize;
            //        break;
            //    default:                    
            //        error = ((FanucCNCProtocol.ErrorCodes)errorcode).ToString();
            //        break;
            //}
        }

        public FanucCNCChannel GetChannelFromStation(string stationName)
        {
           FanucCNCChannel resultChannel = null;

           foreach (var ch in GetChannels()) {
                Station st = GetChannelStations(ch).Find(s => s.Name == stationName);
                if (st != null)
                {
                    resultChannel = (FanucCNCChannel)ch;
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


        #region ICommunicationDriverWebEditing Interface
        object ICommunicationDriverWebEditing.GeneralSettingsEditor
        {
            get
            {
                return null;
            }
        }

        public static FanucCNCDynTagSettings DynSettings;// = new EtherNetIPDynTagSettings("EtherNetIP");
        object ICommunicationDriverWebEditing.DynamicSettingsEditor(string dynamicSettings)
        {
            if (DynSettings == null)
                DynSettings = new FanucCNCDynTagSettings();
            DynSettings.Parse(dynamicSettings);

            return DynSettings;
        }

        public string GetDynamicSettings(object value)
        {
            var dynsettings = value as FanucCNCDynTagSettings;
            if (dynsettings == null)
                return String.Empty;

            return dynsettings.ToString();
        }


        public object ImportTagsEditor
        {
            get { return null; }
        }

        #endregion
    }
}
