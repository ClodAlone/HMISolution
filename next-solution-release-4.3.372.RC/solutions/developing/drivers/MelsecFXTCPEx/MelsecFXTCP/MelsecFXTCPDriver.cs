using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;

namespace MelsecFXTCP
{
    public class MelsecFXTCPDriver : CommunicationDriver
    {
        public MelsecFXTCPDriver()
            : base()
        {
        }

        public MelsecFXTCPDriver(string strSettingPath)
            : base(strSettingPath)
        {
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

            return base.Init(strSettingPath, isProtected, protectionCode);
        }

        public override bool LoadDriverSettings(IDataLayer idl)
        {
            bool bRet = false;
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                try
                {
                    var configuration = (from tag in new XPQuery<MelsecFXTCPDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as MelsecFXTCPDriverSettings;
            if (conf == null)
            {
                throw new ArgumentException("Invalid driver settings");
            }
        }

        public override bool IsPrototypeSplitEnabled()
        {
            return true;
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<MelsecFXTCPDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new MelsecFXTCPDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as MelsecFXTCPDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra Melsec FX TCP Driver Settings initializzation
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as MelsecFXTCPChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new MelsecFXTCPChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as MelsecFXTCPStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new MelsecFXTCPStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new MelsecFXTCPTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new MelsecFXTCPTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as MelsecFXTCPTag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }

        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<MelsecFXTCPChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((MelsecFXErrorCodes)errorcode)
            {
                case MelsecFXErrorCodes.ErrorCodeUnknownSubHeader:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnknownSubHeader;
                break;

                case MelsecFXErrorCodes.ErrorCodeMismatchSubheader:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorMismatchSubheader;
                break;

                case MelsecFXErrorCodes.ErrorCodeInvalidCommand:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorInvalidCommand;
                break;

                case MelsecFXErrorCodes.ErrorCodeIncorrectDeviceDesignation:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorIncorrectDeviceDesignation;
                break;

                case MelsecFXErrorCodes.ErrorCodeExceedingAddOrNumOfPoints:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorExceedingAddOrNumOfPoints;
                break;

                case MelsecFXErrorCodes.ErrorCodeWrongHeadDeviceNum:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWrongHeadDeviceNum;
                break;

                case MelsecFXErrorCodes.ErrorCodePCNumberError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorPCNumberError;
                break;

                case MelsecFXErrorCodes.ErrorCodeModeError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorModeError;
                break;

                case MelsecFXErrorCodes.ErrorCodeRemoteError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRemoteError;
                break;

                case MelsecFXErrorCodes.ErrorCodeAbnormalCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorAbnormalCode;
                break;

                case MelsecFXErrorCodes.ErrorCodeMonitoringTimerExceeded:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorMonitoringTimerExceeded;
                break;

                case MelsecFXErrorCodes.ErrorCodeGenericError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorGenericError;
                break;

                case MelsecFXErrorCodes.ErrorCodeIncompleteFrame:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorIncompleteFrame;
                break;
            }
        }

        #endregion
    }
}
