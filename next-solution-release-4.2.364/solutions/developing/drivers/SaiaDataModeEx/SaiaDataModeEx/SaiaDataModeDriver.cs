using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using IpDriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;

namespace SaiaDataMode
{
    public class SaiaDataModeDriver : CommunicationDriver
    {
        #region Overrides

        public SaiaDataModeDriver()
            : base()
        {
        }

        public SaiaDataModeDriver(string strSettingPath)
            : base()
        {
        }

        public override bool Init(string strSettingPath, bool isProtected, Guid protectionCode)
        {
#if !DEBUG
            if (!MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxwApi90eGlNmfe04G5G+6Aw=="/*AUT*/))
            {
                throw new LicenseOptionMissingException("Missing 'AUT' option in the license");
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
                    var configuration = (from tag in new XPQuery<SaiaDataModeDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as SaiaDataModeDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<SaiaDataModeDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new SaiaDataModeDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as SaiaDataModeDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as SaiaDataModeChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new SaiaDataModeChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as SaiaDataModeStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new SaiaDataModeStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new SaiaDataModeTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new SaiaDataModeTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as SaiaDataModeTag;
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
                return new List<ChangeTag>((new XPQuery<SaiaDataModeChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((SaiaDataModetErrorCodes)errorcode)
            {
                case SaiaDataModetErrorCodes.ErrorInvalidLenght:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorInvalidLenght;
                    break;
                case SaiaDataModetErrorCodes.ErrorIncompleteReply:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorIncompleteReply;
                    break;
                case SaiaDataModetErrorCodes.ErrorCrc:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorCrc;
                    break;
                case SaiaDataModetErrorCodes.ErrorWrongSequence:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWrongSequence;
                    break;
                case SaiaDataModetErrorCodes.ErrorWriteResponseAttribute:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWriteResponseAttribute;
                    break;
                case SaiaDataModetErrorCodes.ErrorReadResponseAttribute:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReadResponseAttribute;
                    break;
                case SaiaDataModetErrorCodes.ErrorReadResponseNack:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReadResponseNack;
                    break;
                case SaiaDataModetErrorCodes.ErrorReadResponseLength:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReadResponseLength;
                    break;
            }
        }        
        #endregion
    }
}
