using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace Fatek
{
    public class FatekDriver : CommunicationDriver
    {
        public FatekDriver()
            : base()
        {
        }

        public FatekDriver(string strSettingPath)
            : base(strSettingPath)
        {
        }

        #region Overrides
        public override bool Init(string strSettingPath, bool isProtected, Guid protectionCode)
        {
#if !DEBUG
            if (!MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxwApi90eGlNmfe04G5G+6Aw=="/*AUT*/))
            {
                throw new LicenseOptionMissingException("Missing 'MDB' option in the license");
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
                    var configuration = (from tag in new XPQuery<FatekDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as FatekDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");            
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<FatekDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new FatekDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as FatekDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as FatekChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new FatekChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as FatekStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new FatekStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new FatekTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new FatekTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as FatekTag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }
         public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<FatekChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);
            if ((DriverErrorCodes)errorcode == DriverErrorCodes.ErrorNoError)
                return;

            quality = StatusCodes.BadCommunicationError;
            switch ((FatekProtocol.FatekErrorCodes)errorcode)
            {
                case FatekProtocol.FatekErrorCodes.ErrorIllegalValue:
                    error = Properties.Resources.ErrorIllegalValue;
                    break;
                case FatekProtocol.FatekErrorCodes.ErrorIllegalFormat:
                    error = Properties.Resources.ErrorIllegalFormat;
                    break;
                case FatekProtocol.FatekErrorCodes.ErrorCanNotRunLadderChecksumErrorWhenRunPLC:
                    error = Properties.Resources.ErrorCanNotRunLadderChecksumErrorWhenRunPLC;
                    break;
                case FatekProtocol.FatekErrorCodes.ErrorCanNotRunPLCIDLadderIDwhenRunPLC:
                    error = Properties.Resources.ErrorCanNotRunPLCIDLadderIDwhenRunPLC;
                    break;
                case FatekProtocol.FatekErrorCodes.ErrorCanNotRunSnytaxCheckErrorWhenRunPLC:
                    error = Properties.Resources.ErrorCanNotRunSnytaxCheckErrorWhenRunPLC;
                    break;
                case FatekProtocol.FatekErrorCodes.ErrorCanNotRunFunctionNotSupported:
                    error = Properties.Resources.ErrorCanNotRunFunctionNotSupported;
                    break;
                case FatekProtocol.FatekErrorCodes.ErrorIllegalAddress:
                    error = Properties.Resources.ErrorIllegalAddress;
                    break;
                case FatekProtocol.FatekErrorCodes.ErrorReceiveFrameError:
                    error = Properties.Resources.ErrorReceiveFrameError;
                    break;
                case FatekProtocol.FatekErrorCodes.ErrorStationIdInvalid:
                    error = Properties.Resources.ErrorStationIdInvalid;
                    break;
                case FatekProtocol.FatekErrorCodes.ErrorCRCInvalid:
                    error = Properties.Resources.ErrorCRCError;
                    break;
                case FatekProtocol.FatekErrorCodes.ErrorGeneric:
                    error = Properties.Resources.ErrorGeneric;
                    break;
            }
        }

#endregion
    }
}
