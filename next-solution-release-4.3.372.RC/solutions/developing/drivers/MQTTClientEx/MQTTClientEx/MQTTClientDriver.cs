using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace MQTTClient
{
    public class MQTTClientDriver : CommunicationDriver
    {
        public MQTTClientDriver()
            : base()
        {
        }

        public MQTTClientDriver(string strSettingPath)
            : base(strSettingPath)
        {
        }


        #region Overrides
        public override bool Init(string strSettingPath, bool isProtected, Guid protectionCode)
        {
#if !DEBUG
            if (!MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxY165CTSLQWk3YWCP69LQxg=="/*IOT*/))
            {
                throw new LicenseOptionMissingException("Missing 'IOT' option in the license");
            }
#endif
            if (!base.Init(strSettingPath, isProtected, protectionCode))
            {
                return (false);
            }

            return (true);
        }

        public override bool LoadDriverSettings(IDataLayer idl)
        {
            bool bRet = false;
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                try
                {
                    var configuration = (from tag in new XPQuery<MQTTClientDriverSettings>(ufw).AsParallel() select tag).Single();
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

        public override void LoadDefaultSettings()
        {
            base.LoadDefaultSettings();
        }

        public override bool IsPrototypeSplitEnabled()
        {
            return (true);
        }

        public override void LoadDriverSettings(DriverSettings settings)
        {
            base.LoadDriverSettings(settings);

            var conf = settings as MQTTClientDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<MQTTClientDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new MQTTClientDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as MQTTClientDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as MQTTClientChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new MQTTClientChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as MQTTClientStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new MQTTClientStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new MQTTClientTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new MQTTClientTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as MQTTClientTag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }

        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<MQTTClientChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((MQTTClientErrorCodes)errorcode)
            {
                case MQTTClientErrorCodes.ErrorCodeErrorPreparingPublishMessage:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.MQTTClientErrorPreparingPublishMessage;
                    break;
                case MQTTClientErrorCodes.ErrorCodeErrorSubscription:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.MQTTClientErrorSubscription;
                    break;
                case MQTTClientErrorCodes.ErrorCodeErrorPublish:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.MQTTClientErrorPublish;
                    break;
                case MQTTClientErrorCodes.ErrorCodeErrorSubscriptionFailed:
                    quality = StatusCodes.BadCommunicationError;
                    error = String.Format(Properties.Resources.MQTTClientErrorSubscribing, LastTagInError, LastChannelInError);
                    break;
                case MQTTClientErrorCodes.ErrorCodeErrorSubscriptionTimeout:
                    quality = StatusCodes.BadCommunicationError;
                    error = String.Format(Properties.Resources.MQTTClientErrorSubscriptionTimeout, LastTagInError, LastChannelInError);
                    break;
                case MQTTClientErrorCodes.ErrorCodeErrorConnectionBroken:
                    quality = StatusCodes.BadConnectionRejected;
                    error = Properties.Resources.MQTTClientConnectionBroken;
                    break;
                case MQTTClientErrorCodes.ErrorCodeErrorPublishTimeout:
                    quality = StatusCodes.BadCommunicationError;
                    error = String.Format(Properties.Resources.MQTTClientErrorPublishTimeout, LastTagInError, LastChannelInError);
                    break;
                case MQTTClientErrorCodes.ErrorCodeErrorPublishJson:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.MQTTClientErrorPublishJson;
                    break;
                case MQTTClientErrorCodes.ErrorCodeErrorBadDecodingError:
                    quality = StatusCodes.BadDecodingError;
                    error = Properties.Resources.MQTTCErrorCodeErrorBadDecodingError;
                    break;
            }
        }

        public override bool Suspend()
        {
            bool returnValue = base.Suspend();

            return (returnValue);
        }

        public override bool Terminate()
        {
            bool returnValue = base.Terminate();

            return (returnValue);
        }
        #endregion

        #region Properties
        private string _LastChannelInError;
        public string LastChannelInError
        {
            get
            {
                return _LastChannelInError;
            }
            set
            {
                _LastChannelInError = value;
            }
        }

        private string _LastTagInError;
        public string LastTagInError
        {
            get
            {
                return _LastTagInError;
            }
            set
            {
                _LastTagInError = value;
            }
        }
        #endregion
    }
}
