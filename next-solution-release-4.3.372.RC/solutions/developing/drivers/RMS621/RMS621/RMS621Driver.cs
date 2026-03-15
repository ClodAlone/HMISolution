using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using DriverSettingsInterfaces;

namespace RMS621
{
    public class RMS621Driver : CommunicationDriver
    {
        #region Constructors

        public RMS621Driver()
            : base()
        {
        }

        #endregion

        #region Overrides

        public override bool Init(string strSettingPath, bool isProtected, Guid protectionCode)
        {
#if !DEBUG
            if (!MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxDkmnbdFT0I5Fy1y5SsWaqg=="/*FCS*/))
            {
                throw new LicenseOptionMissingException("Missing 'FCS' option in the license");
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
                    var configuration = (from tag in new XPQuery<RMS621DriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as RMS621DriverSettings;
            if (conf == null)
            {
                throw new ArgumentException("Invalid driver settings");
            }

            //TODO: Extra RMS621 Driver Settings initializzation
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<RMS621DriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                {
                    configuration.Add(new RMS621DriverSettings(ufw));
                }
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as RMS621DriverSettings;
            if (conf == null)
            {
                throw new ArgumentException("Invalid driver settings");
            }
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as RMS621ChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new RMS621Channel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as RMS621StationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new RMS621Station(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new RMS621Tag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new RMS621Tag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as RMS621Tag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }

        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<RMS621ChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((RMS621Protocol.RMS621ErrorCodes)errorcode)
            {
                case RMS621Protocol.RMS621ErrorCodes.BadCheckSum:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorBadCheckSum;
                    break;
                case RMS621Protocol.RMS621ErrorCodes.BadFormatAnswer:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorBadFormatAnswer;
                    break;
                case RMS621Protocol.RMS621ErrorCodes.FromDevice:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorFromDevice;
                    break;
                case RMS621Protocol.RMS621ErrorCodes.DataMissing:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorDataMissing;
                    break;
                case RMS621Protocol.RMS621ErrorCodes.DataFormatError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorBadFormatAnswer;
                    break;
                case RMS621Protocol.RMS621ErrorCodes.JobDataFormat:
                    quality = StatusCodes.BadOutOfRange;
                    error = Properties.Resources.ErrJobDataFormat;
                    break;
                //case RMS621Protocol.RMS621ErrorCodes.BadRequestCommand:
                //    break;
                case RMS621Protocol.RMS621ErrorCodes.TooMuchCharsReceived:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorTooMuchCharsReceived;
                    break;
            }
        }

        #endregion
    }
}
