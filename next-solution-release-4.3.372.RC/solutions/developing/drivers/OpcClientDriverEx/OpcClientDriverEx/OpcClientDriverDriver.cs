using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using DriverCodeBaseEx.Helpers;
using DriverCodeBaseEx.Enumerators;
using OPCUAViewModel;

namespace OpcClientDriver
{
    public enum OpcClientErrorCodes : int
    {
        OpcClientErrorCodeBadConnetion = 100,
        OpcClientErrorCodeBadWaitingForInitialData = 101,
        OpcClientErrorReadWriteFailed = 102,
        OpcClientErrorWritingUnexpectedError = 103,
        OpcClientErrorBadNodeIdOrInvalidState = 104
    }

    public class OpcClientDriverDriver : CommunicationDriver 
    {
        public OpcClientDriverDriver()
           : base()
        {
        }

        #region Overrides
        public override bool Init(string strSettingPath, bool isProtected, Guid protectionCode)
        {
            DriverInfo.Initialize(this);

            if (!base.Init(strSettingPath, isProtected, protectionCode))
                return false;

            return true;
        }

        public override bool LoadDriverSettings(IDataLayer idl)
        {
            bool bRet = false;
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                try
                {
                    var configuration = (from tag in new XPQuery<OpcClientDriverDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as OpcClientDriverDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            _DoNotUseServerRedundancy = conf.DoNotUseServerRedundancy;
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<OpcClientDriverDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new OpcClientDriverDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as OpcClientDriverDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as OpcClientDriverChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new OpcClientDriverChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as OpcClientDriverStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new OpcClientDriverStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new OpcClientDriverTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new OpcClientDriverTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as OpcClientDriverTag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }

        public override bool IsPrototypeSplitEnabled()
        {
            return true;
        }

        public override bool AddDynamics(IList<TagDefinition> tags)
        {
            bool ret = base.AddDynamics(tags);

            return ret;
        }

        public override bool InUseDynamics(IList<TagDefinition> tags, bool bInUse)
        {
            bool ret = base.InUseDynamics(tags, bInUse);

            return ret;
        }

        // Redefined to filter statistic variables.
        public override List<StatisicTag> GetChannelDiagVar()
        {
            return StatisticSetting.GetStatisicTag(OpcClientDriverChannel.OpcClientStatisticNodes);
        }
        public override List<StatisicTag> GetStationDiagVar()
        {
            return StatisticSetting.GetStatisicTag(OpcClientDriverStation.OpcClientStatisticNodes);
        }

        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<OpcClientDriverChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            switch ((OpcClientErrorCodes)errorcode)
            {
                case OpcClientErrorCodes.OpcClientErrorCodeBadConnetion:
                    quality = StatusCodes.BadNotConnected;
                    error = Properties.Resources.ErrorBadConnection;
                    break;
                case OpcClientErrorCodes.OpcClientErrorCodeBadWaitingForInitialData:
                    quality = StatusCodes.BadWaitingForInitialData;
                    error = Properties.Resources.ErrorBadWaitingForInitialData;
                    break;
                case OpcClientErrorCodes.OpcClientErrorReadWriteFailed:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReadWriteFailed;
                    break;
                case OpcClientErrorCodes.OpcClientErrorWritingUnexpectedError:
                    quality = StatusCodes.BadUnexpectedError;
                    error = Properties.Resources.ErrorOnWriting;
                    break;
                case OpcClientErrorCodes.OpcClientErrorBadNodeIdOrInvalidState:
                    quality = StatusCodes.BadInvalidState;
                    error = Properties.Resources.ErrorBadNodeIdOrInvalidState;
                    break;
                default:
                    if((DriverErrorCodes)errorcode == DriverErrorCodes.ErrorNoError)
                    {
                        base.GetDriverErrorInfo((int)errorcode, out quality, out error);
                    }
                    else
                    {
                        quality = StatusCodes.BadNotConnected;
                        if ((uint)errorcode == StatusCodes.BadNotFound)
                            quality = StatusCodes.BadNotFound;

                        string commStatus = StatusCodes.GetBrowseName((uint)errorcode);
                        if (!string.IsNullOrEmpty(commStatus))
                        {
                            error = string.Format(Properties.Resources.ErrorStatusCode, commStatus);
                        }
                        else
                        {
                            base.GetDriverErrorInfo((int)errorcode, out quality, out error);
                        }
                    }
                    break;
            }
        }

        protected override bool StartUpScheduler()
        {
            // disable scheduler startup
            return true;
        }

        public override void SuspendScheduler()
        {
            // disable scheduler 
        }

        public override bool RestartScheduler()
        {
            // disable scheduler
            return true;
        }
        #endregion

        #region Properties

        private bool _DoNotUseServerRedundancy;

        public bool DoNotUseServerRedundancy
        {
            get { return _DoNotUseServerRedundancy; }
        }

        #endregion
    }
}
