using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using Opc.Ua;
using DevExpress.Xpo;
using DriverCodeBaseEx.Enumerators;

namespace BrPvi
{
    public class BrPviDriver : CommunicationDriver
    {
        #region Constructors
        public BrPviDriver()
            : base()
        {
        }

        public BrPviDriver(string strSettingPath)
            : base(strSettingPath)
        {
        }
        #endregion

        #region Data elements
        Object lockErrorMessageObj = new object();
        #endregion

        #region Overrides

        public override bool Init(string strSettingPath, bool isProtected, Guid protectionCode)
        {
#if !DEBUG
            if (!MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxwApi90eGlNmfe04G5G+6Aw=="/*AUT*/))
            {
                throw new LicenseOptionMissingException("Missing 'AUT' option in the license");
            }
#endif

            if (!BrPviProcol.IsPviMonitorInstalled())
            {
                OnSystemEvent(ObjectIds.Server, Properties.Resources.ErrorPviMonitorNotInstalled, EventSeverity.Max);
                return false;
            }

            return base.Init(strSettingPath, isProtected, protectionCode);
        }

        public override bool LoadDriverSettings(IDataLayer idl)
        {
            bool bRet = false;
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                try
                {
                    var configuration = (from tag in new XPQuery<BrPviDriverSettings>(ufw).AsParallel() select tag).Single();
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

        public override void LoadDriverSettings(DriverSettings settings)
        {
            base.LoadDriverSettings(settings);

            var conf = settings as BrPviDriverSettings;
            if (conf == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidDriverSettings);

        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<BrPviDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new BrPviDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as BrPviDriverSettings;
            if (conf == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidDriverSettings);

            //TODO: Extra BrPvi Driver Settings storing
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as BrPviChannelSettings;
            if (conf == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidChannelSettings);

            return new BrPviChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as BrPviStationSettings;
            if (conf == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidStationSettings);

            return new BrPviStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new BrPviTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new BrPviTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as BrPviTag;
            if (tagItem == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidTagObject);

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
                return new List<ChangeTag>((new XPQuery<BrPviChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        // TODO
        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);
            if(errorcode <= 6)
            {
                return;
            }

            quality = StatusCodes.BadCommunicationError;

            switch ((BrPviProcol.BrPviErrorCodes)errorcode)
            {
                case BrPviProcol.BrPviErrorCodes.ErrorCodeCreateFailure:
                    error = LastPviCreateError;
                    break;

                case BrPviProcol.BrPviErrorCodes.ErrorCodeEventError:
                    error = LastPviEventError;
                    break;

                case BrPviProcol.BrPviErrorCodes.ErrorCodeConnectionBroken:
                    error = LastPviEventError;
                    break;

                case BrPviProcol.BrPviErrorCodes.ErrorCodeIdentificationError:
                    error = LastPviEventError;
                    quality = StatusCodes.BadNotFound;
                    break;

                case BrPviProcol.BrPviErrorCodes.ErrorCreatePviObjectsFailed:
                    error = LastPviEventError;
                    break;
            }
        }

        #endregion

        #region Methods
        public override bool TestComm(ChannelSettings ch, StationSettings st, out string error)
        {
            error = string.Empty;

            if (!BrPviProcol.IsPviMonitorInstalled())
            {
                error = Properties.Resources.ErrorPviMonitorNotInstalled;
                return false;
            }

            error = null;

            Channel channel = CreateChannel(ch);
            Channels[channel.Name] = channel;
            Channels[channel.Name].Init();

            Station station = CreateStation(st);
            Stations[station.Name] = station;
            Stations[station.Name].Init(channel);

            IList<object> dataValues = new List<Object>();
            dataValues.Add(new Variant(0, new TypeInfo(BuiltInType.Int16, -1)));
            // force drive to terminated execution after pls station's info is retried --> don't continue tryiing to subscribe job
            ((BrPviChannel)channel).TestCommMode = true;
            uint conn = OnReadValues(string.Format(BrPviProcol.TEST_COMM_DYNAMIC_SETTINGS, station.Name), dataValues);            
            if (conn != StatusCodes.Good)
                error = ((StatusCode)conn).ToString();
            
            return (conn == StatusCodes.Good);
        }
        #endregion

        #region Properties

        private string _LastPviCreateError;
        public string LastPviCreateError
        {
            get
            {
                lock(lockErrorMessageObj)
                {
                    return _LastPviCreateError;
                }
            }
            set
            {
                lock (lockErrorMessageObj)
                {
                    _LastPviCreateError = value;
                }
            }
        }

        private string _LastPviEventError;
        public string LastPviEventError
        {
            get
            {
                lock (lockErrorMessageObj)
                {
                    return _LastPviEventError;
                }
            }
            set
            {
                lock (lockErrorMessageObj)
                {
                    _LastPviEventError = value;
                }
            }
        }

        #endregion
    }
}
