using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;

namespace MelsecFX
{
    public class MelsecFXDriver : CommunicationDriver
    {
        #region Constructors

        public MelsecFXDriver()
            : base()
        {
        }

        public MelsecFXDriver(string strSettingPath)
            : base(strSettingPath)
        {
        }

        #endregion

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
                    var configuration = (from tag in new XPQuery<MelsecFXDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as MelsecFXDriverSettings;
            if (conf == null)
            {
                throw new ArgumentException("Invalid driver settings");
            }

            //TODO: Extra MelsecFX Driver Settings initializzation
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<MelsecFXDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                {
                    configuration.Add(new MelsecFXDriverSettings(ufw));
                }
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as MelsecFXDriverSettings;
            if (conf == null)
            {
                throw new ArgumentException("Invalid driver settings");
            }

            //TODO: Extra MelsecFX Driver Settings initializzation
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as MelsecFXChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new MelsecFXChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as MelsecFXStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new MelsecFXStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new MelsecFXTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new MelsecFXTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as MelsecFXTag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }

        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<MelsecFXChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((MelsecFXErrorCodes)errorcode)
            {
                case MelsecFXErrorCodes.ErrorCodeNakReplyToOutputCommand:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorNakReplyToOutputCommand;
                    break;
                case MelsecFXErrorCodes.ErrorCodeNakReplyToInputRequest:
                    quality = StatusCodes.BadCommunicationError;
                    error = string.Format(Properties.Resources.ErrorNakReplyToInputRequest, DriverCodeBaseEx.Properties.Resources.LinkType_Input);
                    break;
                case MelsecFXErrorCodes.ErrorCodeIncompleteFrame:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorIncompleteFrame;
                    break;
                case MelsecFXErrorCodes.ErrorCodeStx:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorStx;
                    break;
                case MelsecFXErrorCodes.ErrorCodeEtx:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorEtx;
                    break;
                case MelsecFXErrorCodes.ErrorCodeFcs:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorFcs;
                    break;
            }
        }

        #endregion
    }
}
