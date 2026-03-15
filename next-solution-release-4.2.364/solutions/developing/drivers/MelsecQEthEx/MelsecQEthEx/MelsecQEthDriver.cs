using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace MelsecQEth
{
    public class MelsecQEthDriver : CommunicationDriver
    {
        public MelsecQEthDriver()
            : base()
        {
        }

        public MelsecQEthDriver(string strSettingPath)
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

            switch ((MelsecQEthProtocol.MelsecQErrorCodes)errorcode)
            {
                case MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeUnknownSubHeader:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnknownSubHeader;
                break;

                case MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeMismatchSubheader:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorMismatchSubheader;
                break;

                case MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeInvalidCommand:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorInvalidCommand;
                break;

                case MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeIncorrectDeviceDesignation:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorIncorrectDeviceDesignation;
                break;

                case MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeExceedingAddOrNumOfPoints:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorExceedingAddOrNumOfPoints;
                break;

                case MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeWrongHeadDeviceNum:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWrongHeadDeviceNum;
                break;

                case MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodePCNumberError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorPCNumberError;
                break;

                case MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeModeError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorModeError;
                break;

                case MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeRemoteError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRemoteError;
                break;

                case MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeAbnormalCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorAbnormalCode;
                break;

                case MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeMonitoringTimerExceeded:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorMonitoringTimerExceeded;
                break;

                case MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeGenericError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorGenericError;
                break;

                case MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeIncompleteFrame:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorIncompleteFrame;
                break;

                case MelsecQEthProtocol.MelsecQErrorCodes.ErrorOnlineChangeDisabled:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorOnlineChangeDisabled;
                    break;
                default:
                    if ((MelsecQEthProtocol.MelsecQErrorCodes)errorcode >= MelsecQEthProtocol.MelsecQErrorCodes.ErrorFromDevice)
                    {
                        switch (errorcode - MelsecQEthProtocol.MelsecQErrorCodes.ErrorFromDevice)
                        {
                            case MelsecQEthProtocol.MelsecQErrorCodes.ErrorLabelDoesNotExist:
                            case MelsecQEthProtocol.MelsecQErrorCodes.ErrorLabelsDoesNotExist:
                                quality = StatusCodes.BadNotFound;
                                error = Properties.Resources.ErrorLabelAddressNotFound;
                                break;
                            case MelsecQEthProtocol.MelsecQErrorCodes.ErrorLabelSizeMismatch:
                                quality = StatusCodes.BadOutOfRange;
                                error = Properties.Resources.ErrorLabelSizeMismatch;
                                break;
                            case MelsecQEthProtocol.MelsecQErrorCodes.ErrorLabelArraySizeMismatch:
                                quality = StatusCodes.BadOutOfRange;
                                error = Properties.Resources.ErrorLabelArraySizeMismatch;
                                break;
                            case MelsecQEthProtocol.MelsecQErrorCodes.ErrorLabelDataCannotBeAccessWithLabel:
                                quality = StatusCodes.BadNotSupported;
                                error = Properties.Resources.ErrorLabelDataCannotBeAccessWithLabel;
                                break;
                            case MelsecQEthProtocol.MelsecQErrorCodes.ErrorLabelTooManyRequest:
                                quality = StatusCodes.BadOutOfRange;
                                error = Properties.Resources.ErrorLabelTooManyRequest;
                                break;
                           default:
                                quality = StatusCodes.BadCommunicationError;
                                error = string.Format(Properties.Resources.ErrorFromDevice, errorcode - (int)MelsecQEthProtocol.MelsecQErrorCodes.ErrorFromDevice);
                                break;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// The TestCommOwn: test connection to the device, the request is owning because request the devive name 
        /// </summary>
        /// <param name="ch">Channel</param>
        /// <param name="st">Station</param>
        /// <param name="tagDataType"></param>
        /// <param name="dynamicSettings"></param>
        /// <param name="error">Message of error</param>
        /// <returns></returns>
        public override bool TestComm(ChannelSettings ch, StationSettings st, BuiltInType tagDataType, string dynamicSettings, out string error)
        {
            error = null;

            Channel channel = CreateChannel(ch);
            Channels[channel.Name] = channel;
            Channels[channel.Name].Init();

            Station station = CreateStation(st);
            Stations[station.Name] = station;
            Stations[station.Name].Init(channel);

            DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;

            if (base.TestComm(ch, st, tagDataType, dynamicSettings, out error))
            {
                ((MelsecQEthChannel)channel).ReadCPUModelName((MelsecQEthStation)station, out error);
            }
            else
            {
                conn = DriverErrorCodes.ErrorTimeOut;
            }

            return (conn == StatusCodes.Good);
        }
        #endregion
    }
}
