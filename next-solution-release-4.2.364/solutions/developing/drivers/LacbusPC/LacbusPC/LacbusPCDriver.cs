using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBase;
using DriverBaseInterfaces;
using Opc.Ua;
using DevExpress.Xpo;

namespace LacbusPC
{
    public class LacbusPCDriver : CommunicationDriver
    {
        #region Constructors
        public LacbusPCDriver()
            : base()
        {
        }
        #endregion

        #region Overrides

        public override bool Init(string strSettingPath, bool isProtected, Guid protectionCode)
        {
#if !DEBUG
            if (!MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxMjY539eIREBouUCiSTGu7w=="/*TLM*/))
            {
                throw new LicenseOptionMissingException("Missing 'TLM' option in the license");
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
                    var configuration = (from tag in new XPQuery<LacbusPCDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as LacbusPCDriverSettings;
            if (conf == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidDriverSettings);

        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<LacbusPCDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new LacbusPCDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as LacbusPCDriverSettings;
            if (conf == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidDriverSettings);

            //TODO: Extra LacbusPC Driver Settings storing
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as LacbusPCChannelSettings;
            if (conf == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidChannelSettings);

            return new LacbusPCChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as LacbusPCStationSettings;
            if (conf == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidStationSettings);

            return new LacbusPCStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new LacbusPCTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new LacbusPCTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as LacbusPCTag;
            if (tagItem == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidTagObject);

            return new TagSettings(session, tagItem);
        }

        public override bool IsPrototypeSplitEnabled()
        {
            return false;
        }

        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<LacbusPCChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((LacbusPCErrorCodes)errorcode)
            {
                case LacbusPCErrorCodes.ErrorCodeConnectionBroken:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorConnectionBroken;
                    break;

                case LacbusPCErrorCodes.ErrorCodeSyntaxError:
                    quality = StatusCodes.BadSyntaxError;
                    error = Properties.Resources.ErrorSyntaxError;
                    break;

                case LacbusPCErrorCodes.ErrorCodeForbiddenCommand:
                    error = Properties.Resources.ErrorForbiddenCommand;
                    quality = StatusCodes.BadCommunicationError;
                    break;

                case LacbusPCErrorCodes.ErrorCodeBadRtuNumber:
                    error = Properties.Resources.ErrorBadRtuNumber;
                    quality = StatusCodes.BadCommunicationError;
                    break;
                
                case LacbusPCErrorCodes.ErrorCodeAlreadyAuthenticatedSCADA:
                    error = Properties.Resources.ErrorAlreadyAuthenticatedSCADA;
                    quality = StatusCodes.BadCommunicationError;
                   break;
                
                case LacbusPCErrorCodes.ErrorCodeDisabledScadaNumber:
                    error = Properties.Resources.ErrorDisabledScadaNumber;
                    quality = StatusCodes.BadCommunicationError;
                    break;
                
                case LacbusPCErrorCodes.ErrorCodeAuthenticationNeeded:
                    error = Properties.Resources.ErrorAuthenticationNeeded;
                    quality = StatusCodes.BadCommunicationError;
                    break;
                
                case LacbusPCErrorCodes.ErrorCodeUnsupportedProtocolVersion:
                    quality = StatusCodes.BadProtocolVersionUnsupported;
                    error = Properties.Resources.ErrorUnsupportedProtocolVersion;
                    break;
                
                case LacbusPCErrorCodes.ErrorCodeUnexpectedErrorCode:
                    quality = StatusCodes.BadUnexpectedError;
                    error = Properties.Resources.ErrorUnexpectedErrorCode;
                    break;

                case LacbusPCErrorCodes.ErrorCodeErrorPreparingFrameRequest:
                    quality = StatusCodes.BadUnexpectedError;
                    error = Properties.Resources.ErrorPreparingFrameRequest;
                    break;

                case LacbusPCErrorCodes.ErrorCodeErrorSendingFrameRequest:
                     quality = StatusCodes.BadCommunicationError;
                     error = Properties.Resources.ErrorSendingFrameRequest;
                   break;
                
                case LacbusPCErrorCodes.ErrorCodeErrorOpeningRequestChannel:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorOpeningRequestChannel;
                    break;

                case LacbusPCErrorCodes.ErrorCodeErrorOpeningRequestChannelSyntax:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorOpenRequestChannelIncorrectSyntax;
                    break;

                case LacbusPCErrorCodes.ErrorCodeErrorOpeningRequestChannelAuthentication:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorOpenRequestChannelAuthenticationFailure;
                    break;

                case LacbusPCErrorCodes.ErrorCodeUnableToProcessTheCommand:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnableToProcessTheCommand;
                    break;

                default:
                    if ((LacbusPCErrorCodes)errorcode >= LacbusPCErrorCodes.ErrorRTUDisconnected)
                    {
                        quality = StatusCodes.BadCommunicationError;
                        error = String.Format(Properties.Resources.ErrorRTUDisconnected, (uint)((LacbusPCErrorCodes)errorcode - LacbusPCErrorCodes.ErrorRTUDisconnected));
                    }
                    else if ((LacbusPCErrorCodes)errorcode >= LacbusPCErrorCodes.ErrorCodeRTUPollRequestFailed)
                    {
                        quality = StatusCodes.BadCommunicationError;
                        error = String.Format(Properties.Resources.ErrorCodeRTUPollRequestFailed, (uint)((LacbusPCErrorCodes)errorcode - LacbusPCErrorCodes.ErrorCodeRTUPollRequestFailed));
                    }
                    break;
            }

        }

        #endregion
    }
}
