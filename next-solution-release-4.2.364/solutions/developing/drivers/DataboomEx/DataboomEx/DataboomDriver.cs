using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;

namespace Databoom
{
    public class DataboomDriver : CommunicationDriver
    {
        public DataboomDriver()
            : base()
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

            // Driver specific:

            return (true);
        }

        public override bool LoadDriverSettings(IDataLayer idl)
        {
            bool bRet = false;
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                try
                {
                    var configuration = (from tag in new XPQuery<DataboomDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as DataboomDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            // Extra Databoom Driver Settings initializzation
            _UrlToPostTo = conf.UrlToPostTo;
            _UrlClockPost = conf.UrlClockPost;
            _ApiKey = conf.ApiKey;
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<DataboomDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new DataboomDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as DataboomDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            // Extra Databoom Driver Settings initializzation
            conf.UrlToPostTo = UrlToPostTo;
            conf.UrlClockPost = UrlClockPost;
            conf.ApiKey = ApiKey;            
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as DataboomChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new DataboomChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as DataboomStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new DataboomStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new DataboomTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new DataboomTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as DataboomTag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }

        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<DataboomChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((DataboomProtocol.DataboomErrorCodes)errorcode)
            {
                case DataboomProtocol.DataboomErrorCodes.ErrorCodeErrorPreparingPublishMessage:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.DataboomErrorPreparingPublishMessage;
                    break;
                case DataboomProtocol.DataboomErrorCodes.ErrorCodeFailedGettingServiceDateTime:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.DataboomErrorGettingRemoteDateTime;
                    break;
                case DataboomProtocol.DataboomErrorCodes.ErrorCodeErrorPublish:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.DataboomErrorPublish;
                    break;
                case DataboomProtocol.DataboomErrorCodes.ErrorCodeNoInternetConnection:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.DatabommErrorInternetNotAvailable;
                    break;
            }
        }
        #endregion

        #region Properties

        /// <summary>   Url To Post To. </summary>
        private string _UrlToPostTo;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Url To Post To. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string UrlToPostTo
        {
            get { return _UrlToPostTo; }
        }

        /// <summary>   Url Clock Post. </summary>
        private string _UrlClockPost;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Url Clock Post Key. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string UrlClockPost
        {
            get { return _UrlClockPost; }
        }

        /// <summary>   Api Key. </summary>
        private string _ApiKey;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the  Api Key. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string ApiKey
        {
            get { return _ApiKey; }
        }

        #endregion
    }
}
