using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;

namespace MelsecQEth
{
    public class MelsecQEthDriver : CommunicationDriver
    {
        
        public MelsecQEthDriver()
            : base()
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
                    var configuration = (from tag in new XPQuery<MelsecQEthDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as MelsecQEthDriverSettings;
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
                var configuration = (from tag in new XPQuery<MelsecQEthDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new MelsecQEthDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as MelsecQEthDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra Melsec Q TCP Driver Settings initializzation
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as MelsecQEthChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new MelsecQEthChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as MelsecQEthStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new MelsecQEthStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new MelsecQEthTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new MelsecQEthTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as MelsecQEthTag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }

        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<MelsecQEthChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((MelsecQErrorCodes)errorcode)
            {
                case MelsecQErrorCodes.ErrorCodeUnknownSubHeader:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnknownSubHeader;
                break;

                case MelsecQErrorCodes.ErrorCodeMismatchSubheader:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorMismatchSubheader;
                break;

                case MelsecQErrorCodes.ErrorCodeInvalidCommand:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorInvalidCommand;
                break;

                case MelsecQErrorCodes.ErrorCodeIncorrectDeviceDesignation:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorIncorrectDeviceDesignation;
                break;

                case MelsecQErrorCodes.ErrorCodeExceedingAddOrNumOfPoints:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorExceedingAddOrNumOfPoints;
                break;

                case MelsecQErrorCodes.ErrorCodeWrongHeadDeviceNum:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWrongHeadDeviceNum;
                break;

                case MelsecQErrorCodes.ErrorCodePCNumberError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorPCNumberError;
                break;

                case MelsecQErrorCodes.ErrorCodeModeError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorModeError;
                break;

                case MelsecQErrorCodes.ErrorCodeRemoteError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRemoteError;
                break;

                case MelsecQErrorCodes.ErrorCodeAbnormalCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorAbnormalCode;
                break;

                case MelsecQErrorCodes.ErrorCodeMonitoringTimerExceeded:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorMonitoringTimerExceeded;
                break;

                case MelsecQErrorCodes.ErrorCodeGenericError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorGenericError;
                break;

                case MelsecQErrorCodes.ErrorCodeIncompleteFrame:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorIncompleteFrame;
                break;

                case MelsecQErrorCodes.ErrorOnlineChangeDisabled:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorOnlineChangeDisabled;
                    break;

                default:
                    if ((MelsecQErrorCodes)errorcode >= MelsecQErrorCodes.ErrorFromDevice)
                    {
                        quality = StatusCodes.BadCommunicationError;
                        error = string.Format(Properties.Resources.ErrorFromDevice, errorcode - (int)MelsecQErrorCodes.ErrorFromDevice);
                    }
                    break;
            }
        }

        #endregion
    }
}
