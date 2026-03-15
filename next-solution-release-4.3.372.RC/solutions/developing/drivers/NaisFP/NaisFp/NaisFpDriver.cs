using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using IpDriverCodeBase;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using DriverCodeBase.Helpers;

namespace NaisFp
{
    public class NaisFpDriver : CommunicationDriver
    {
        #region Overrides
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
                    var configuration = (from tag in new XPQuery<NaisFpDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as NaisFpDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<NaisFpDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new NaisFpDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as NaisFpDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as NaisFpChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new NaisFpChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as NaisFpStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new NaisFpStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new NaisFpTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new NaisFpTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as NaisFpTag;
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
                return new List<ChangeTag>((new XPQuery<NaisFpChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((NaisFptErrorCodes)errorcode)
            {
                case NaisFptErrorCodes.ErrorInvalidLenght:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorInvalidLenght;
                    break;
                case NaisFptErrorCodes.ErrorIncompleteReply:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorIncompleteReply;
                    break;
                case NaisFptErrorCodes.ErrorCrc:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorCrc;
                    break;
                case NaisFptErrorCodes.ErrorWrongSequence:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWrongSequence;
                    break;
                case NaisFptErrorCodes.ErrorWriteResponseAttribute:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWriteResponseAttribute;
                    break;
                case NaisFptErrorCodes.ErrorReadResponseAttribute:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReadResponseAttribute;
                    break;
                case NaisFptErrorCodes.ErrorReadResponseNack:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReadResponseNack;
                    break;
                case NaisFptErrorCodes.ErrorReadResponseLength:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReadResponseLength;
                    break;
                case NaisFptErrorCodes.ErrorNotExpectedMessage:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorNotExpectedMessage;
                    break;
                case NaisFptErrorCodes.ErrorBCC:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorBCC;
                    break;
                case NaisFptErrorCodes.ErrorBadRxChars:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorBadRxChars;
                    break;
                case NaisFptErrorCodes.ErrorStationUnmismatch:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorStationUnmismatch;
                    break;
                case NaisFptErrorCodes.ErrorFormat:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorFormat;
                    break;
                case NaisFptErrorCodes.ErrorWrongCommand:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWrongCommand;
                    break;
                case NaisFptErrorCodes.ErrorProc:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProc;
                    break;
                case NaisFptErrorCodes.ErrorReturnUnknownCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReturnUnknownCode;
                    break;
            }
        }
        #endregion
    }
}
