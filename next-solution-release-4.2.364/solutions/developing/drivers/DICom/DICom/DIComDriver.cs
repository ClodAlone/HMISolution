using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using DriverCodeBase.Enumerators;
using System.Threading;
using System.IO;

namespace DICom
{
    public class DIComDriver : CommunicationDriver
    {
        public DIComDriver()
            : base()
        {
            _FunctionMode = DIComProtocol.FunctionMode.ForcePublishCollectedMessagesDataWithSameDateTime;
        }

        public bool WizardDataCollect(string strProjectPath, TimeSpan collectLength, out List<DIComProtocol.WizardDataDevice> devices, out string error)
        {
            error = null;
            _FunctionMode = DIComProtocol.FunctionMode.ConfigMode;

            _LogToFile = Properties.Settings.Default.LOG_TO_FILE;

            if (_LogToFile)
                LogToFile("Start WizardDataCollect");

            //// from project source path, create DevExpress well formatted "path" to access to Movicon's main project file
            //strProjectPath = string.Format("{0}{1}{2}{3}UFUAServer{4}Server.UFUAServer", strProjectPath, Path.DirectorySeparatorChar, Path.GetFileName(strProjectPath), Path.DirectorySeparatorChar, Path.DirectorySeparatorChar);
            //strProjectPath = string.Format("XpoProvider=InMemoryDataStore;data source={0}{1}{2}{3}", '"', strProjectPath, '"', ";");

            //// from database
            //strSettingPath = "XpoProvider=MSSqlServer;data source=(local);integrated security=SSPI;initial catalog=DICom"

            List<TagDefinition> tags = null;
            try
            {
                // read Movicon's project driver settings
                Init(strProjectPath);
                tags = WizardDataCollectCreateTagForChannels();
            } catch (Exception ex)
            {
                error = ex.Message;
            }            

            if (tags ==null || tags.Count == 0)
            {
                error = string.Format("Error during WizardDataCollect: {0}", error);
                if (_LogToFile)
                    LogToFile(error);
                devices = null;
                return false;
            }
            
            AddDynamics(tags);

            // StartUp driver's channels
            Startup();

            // wait all channel complete data collect (collectLength)
            WizardDataCollectWaitData(collectLength);

            // stop driver's channels execution
            StopChannels();

            if (SomeChannelFailedToStartUp())
            {
                error = "Some channel failed to StartUp";
                if (_LogToFile)
                    LogToFile(error);
                devices = null;
                return false;
            }

            devices = WizardDataCollectProcessNewData();

            if (_LogToFile)
                LogToFile("Stop WizardDataCollect");

            return true;
        }
        #region Overrides
        public override bool Init(string strSettingPath, bool isProtected, Guid protectionCode)
        {
#if !DEBUG
            if (!MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxwApi90eGlNmfe04G5G+6Aw=="/*AUT*/))
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
                    var configuration = (from tag in new XPQuery<DIComDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as DIComDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra Modbus Driver Settings initializzation
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<DIComDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new DIComDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as DIComDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra Modbus Driver Settings initializzation
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as DIComChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new DICommTcpServer(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as DIComStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new DIComStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new DIComTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new DIComTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as DIComTag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }

        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<DIComChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((DIComProtocol.DIComErrorCodes)errorcode)
            {
                case DIComProtocol.DIComErrorCodes.ErrorReadError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReadError;
                    break;
                case DIComProtocol.DIComErrorCodes.ErrorReceiveFrameError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReceiveFrameError;
                    break;
                case DIComProtocol.DIComErrorCodes.ErrorProtocolIllegalDataAddress:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolIllegalVarName;
                    break;
                case DIComProtocol.DIComErrorCodes.ErrorProtocolIllegalDataValue:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolIllegalDataValue;
                    break;
                case DIComProtocol.DIComErrorCodes.ErrorWrongSize:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWrongSize;
                    break;
                case DIComProtocol.DIComErrorCodes.ErrorVarNotCollected:
                    quality = StatusCodes.BadNoData;
                    error = Properties.Resources.ErrorNoDataCollected;
                    break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Prepares this object for use. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Startup()
        {
            bool bRet = base.Startup();
            foreach (var channel in GetChannels())
                if (!channel.ThreadRunning())
                    channel.Startup();
            return bRet;
        }

        private void StopChannels()
        {
            foreach (Channel ch in GetChannels())
                ch.Suspend();
        }

        #endregion

        #region Specific Methos
        public List<Station> GetChannelStationsByName(string channelName)
        {
            List<Station> listStation = new List<Station>();
            foreach (Station st in GetStations())
                if (st.GetChannel().Name == channelName)
                    listStation.Add(st);
            return listStation;
        }

        private void WizardDataCollectWaitData(TimeSpan acquireLength)
        {
            System.Threading.Thread.Sleep((int)acquireLength.TotalMilliseconds);
        }

        List<DIComProtocol.WizardDataDevice> WizardDataCollectProcessNewData()
        {
            List<DIComProtocol.WizardDataDevice> devices = new List<DIComProtocol.WizardDataDevice>();            
            foreach (DICommTcpServer ch in GetChannels())
            {                
                foreach (DIComStation st in GetChannelStations(ch))
                {
                    Dictionary<int, List<DIComProtocol.DiComVar>> configVars = ch.RestoreClientConfigRecords();

                    if (configVars != null && configVars.Count > 0)
                    {
                        DIComProtocol.WizardDataDevice device = new DIComProtocol.WizardDataDevice();
                        device.StationName = st.Name;
                        device.DynamicLink = new DIComDynTagSettings() { StationName = device.StationName, TagLinkType = (int)LinkType.InputOutput }.ToString();

                        if (_LogToFile)
                            LogToFile(string.Format("WizardDataCollectProcessNewData Station:{0}, DL:{1}", device.StationName, device.DynamicLink));

                        foreach (var recordID in configVars.Keys)
                        {
                            foreach (DIComProtocol.DiComVar var in configVars[recordID])
                            {
                                DIComProtocol.WizardDataDevice.Variable variable = new DIComProtocol.WizardDataDevice.Variable();
                                variable.TagName = DIComProtocol.CorrectVarNameToMoviconVariableName(var.VarName);
                                variable.TagType = DIComProtocol.GetMoviconTypeId(var.VarType);
                                variable.VarSize = var.VarSize;
                                variable.DynamicLink = new DIComDynTagSettings() { StationName = device.StationName, VarName = var.VarName, TagLinkType = (int)LinkType.InputOutput }.ToString();

                                if (_LogToFile)
                                    LogToFile(string.Format("WizardDataCollectProcessNewData Variable TagName:{0}, TagType:{1}, VarSize:{2}, VarName:{3}, DL:{4}", variable.TagName, variable.TagType, variable.VarSize, var.VarName, variable.DynamicLink));

                                device.Variables.Add(variable);
                            }
                        }
                        devices.Add(device);
                    }
                }
            }

            return devices;
        }

        private List<TagDefinition> WizardDataCollectCreateTagForChannels()
        {
            List<TagDefinition> tags = new List<TagDefinition>();
            foreach (var ch in GetChannels())
            {
                foreach (var st in GetChannelStations(ch))
                {
                    TagDefinition tgdf = new TagDefinition()
                    {
                        NodeId = new NodeId(new Guid()),
                        DynamicSettings = string.Format("DICom.Station={0}|LinkType=1|VN=VarName", st.Name),
                        SamplingInterval = 0,
                        ArrayDimension = 0,
                        InitialValue = null,
                        MemberOrder = 0,
                        DataType = (int)BuiltInType.Int16,
                        Name = string.Format("{0}_{1}", st.Name, "VarName")
                    };
                    tags.Add(tgdf);
                }
            }

            return tags;
        }

        private bool SomeChannelFailedToStartUp()
        {
            return (GetChannels().Count(ch=> ((DICommTcpServer)ch).GetLastErrorCode() == (DriverErrorCodes)DIComProtocol.DIComErrorCodes.ErrorChannelFailedToStart) > 0);
        }

        private void LogToFile(string message)
        {
            log.Debug(string.Format("Wizard_{0}-{1}", DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff"), message));
        }

        #endregion

        #region Properties
        private DIComProtocol.FunctionMode _FunctionMode;
        public DIComProtocol.FunctionMode FunctionMode
        {
            get { return _FunctionMode; }
        }

        bool _LogToFile = false;
        #endregion
    }
}
