using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;

namespace OmronFinsEthernet
{
    public class OmronFinsEthernetDriver : CommunicationDriver
    {
        public OmronFinsEthernetDriver()
            : base()
        {
        }

        #region Overrides
        public override bool Init(string strSettingPath, bool isProtected, Guid protectionCode)
        {
#if !DEBUG
            if (!MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxJ0V2Dx2Ij9ozl7B6jwS4IA=="/*MDB*/))
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
                    var configuration = (from tag in new XPQuery<OmronFinsEthernetDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as OmronFinsEthernetDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<OmronFinsEthernetDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new OmronFinsEthernetDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as OmronFinsEthernetDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra OmronFinsEthernet Driver Settings initializzation
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as OmronFinsEthernetChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new OmronFinsEthernetChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as OmronFinsEthernetStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new OmronFinsEthernetStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new OmronFinsEthernetTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new OmronFinsEthernetTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as OmronFinsEthernetTag;
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
                return new List<ChangeTag>((new XPQuery<OmronFinsEthernetChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((OmronFinsEthernetErrorCodes)errorcode)
            {
                case OmronFinsEthernetErrorCodes.ErrorSid:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorSid;
                    break;
                case OmronFinsEthernetErrorCodes.ErrorEndCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorEndCode;
                    break;
            }
        }

        #endregion
    }
}
