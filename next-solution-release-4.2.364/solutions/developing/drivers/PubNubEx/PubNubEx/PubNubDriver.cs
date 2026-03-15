using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using PubNubMessaging.Core;

namespace PubNub
{
    public class PubNubDriver : CommunicationDriver
    {
        public PubNubDriver()
            : base()
        {
        }

        #region Specific Data Members
        public Pubnub pubnub;
        public string lastPublishErrorMessage = String.Empty;
        public string lastSubscribeErrorMessage = String.Empty;
        #endregion

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

            // Driver specific:
            // Allocate the Pubnub object
            pubnub = new PubNubMessaging.Core.Pubnub(PublishKey, SubscribeKey);

            return (true);
        }

        public override bool LoadDriverSettings(IDataLayer idl)
        {
            bool bRet = false;
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                try
                {
                    var configuration = (from tag in new XPQuery<PubNubDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as PubNubDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            // Extra PubNub Driver Settings initializzation
            _PublishKey = conf.PublishKey;
            _SubscribeKey = conf.SubscribeKey;
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<PubNubDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new PubNubDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as PubNubDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            // Extra PubNub Driver Settings initializzation
            conf.PublishKey = PublishKey;
            conf.SubscribeKey = SubscribeKey;
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as PubNubChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new PubNubChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as PubNubStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new PubNubStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new PubNubTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new PubNubTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as PubNubTag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }

        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<PubNubChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((PubNubErrorCodes)errorcode)
            {
                case PubNubErrorCodes.ErrorCodeErrorPreparingPublishMessage:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.PubNubErrorPreparingPublishMessage;
                    break;
                case PubNubErrorCodes.ErrorCodeErrorSubscription:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.PubNubErrorSubscription + lastSubscribeErrorMessage;
                    break;
                case PubNubErrorCodes.ErrorCodeErrorPublish:
                    quality = StatusCodes.BadCommunicationError;
                    error = String.Format(Properties.Resources.PubNubErrorPublish, lastPublishErrorMessage);
                    break;
                case PubNubErrorCodes.ErrorCodeErrorSubscriptionException:
                    quality = StatusCodes.BadCommunicationError;
                    error = lastSubscribeErrorMessage;
                    break;
                case PubNubErrorCodes.ErrorCodeErrorSubscriptionTimeout:
                    quality = StatusCodes.BadCommunicationError;
                    error = lastSubscribeErrorMessage;
                    break;
            }
        }

        public override bool Suspend()
        {
            bool returnValue = base.Suspend();

            if (pubnub != null)
            {
//#if DEBUG
//                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
//                System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} Suspend calling pubnub.EndPendingRequests",
//                                                   currentTime));
//#endif
                pubnub.EndPendingRequests();
            }

            return (returnValue);
        }

        public override bool Terminate()
        {
            bool returnValue = base.Terminate();

            if (pubnub != null)
            {
//#if DEBUG
//                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
//                System.Diagnostics.Debug.WriteLine(String.Format("PubNub DBG - {0} Terminate calling pubnub.EndPendingRequests",
//                                                   currentTime));
//#endif
                pubnub.EndPendingRequests();
            }

            return (returnValue);
        }
        #endregion

        #region Properties

        /// <summary>   Publish Key. </summary>
        private string _PublishKey;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Publish Key. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string PublishKey
        {
            get { return _PublishKey; }
        }

        /// <summary>   Subscription Key. </summary>
        private string _SubscribeKey;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Subscription Key. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string SubscribeKey
        {
            get { return _SubscribeKey; }
        }
        #endregion
    }
}
