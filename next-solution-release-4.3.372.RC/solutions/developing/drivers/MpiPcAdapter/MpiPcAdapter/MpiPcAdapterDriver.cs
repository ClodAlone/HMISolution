using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using System.Text.RegularExpressions;

namespace MpiPcAdapter
{
    public class MpiPcAdapterDriver : CommunicationDriver
    {
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
                    var configuration = (from tag in new XPQuery<MpiPcAdapterDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as MpiPcAdapterDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<MpiPcAdapterDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new MpiPcAdapterDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as MpiPcAdapterDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as MpiPcAdapterChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new MpiPcAdapterChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as MpiPcAdapterStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new MpiPcAdapterStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new MpiPcAdapterTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new MpiPcAdapterTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as MpiPcAdapterTag;
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
                return new List<ChangeTag>((new XPQuery<MpiPcAdapterChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((MpiPcAdapterErrorCodes)errorcode)
            {

                case MpiPcAdapterErrorCodes.IDS_ERRDEVICEWRITE:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_ERRDEVICEWRITE;
                    break;
                case MpiPcAdapterErrorCodes.IDS_SERIALREADFAILED:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_SERIALREADFAILED;
                    break;
                case MpiPcAdapterErrorCodes.IDS_ETXNOTRECEIVED:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_ETXNOTRECEIVED;
                    break;
                case MpiPcAdapterErrorCodes.IDS_PLCBCC:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_PLCBCC;
                    break;
                case MpiPcAdapterErrorCodes.IDS_ANSWERTOOSHORT:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_ANSWERTOOSHORT;
                    break;
                case MpiPcAdapterErrorCodes.IDS_UNRECOGNIZEDANSWER:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_UNRECOGNIZEDANSWER;
                    break;
                case MpiPcAdapterErrorCodes.IDS_RETURNEDERR:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_RETURNEDERR;
                    break;
                case MpiPcAdapterErrorCodes.IDS_WRONGDESTINATION:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_WRONGDESTINATION;
                    break;
                case MpiPcAdapterErrorCodes.IDS_CONNREFUSED:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_CONNREFUSED;
                    break;
                case MpiPcAdapterErrorCodes.IDS_WRONGSOURCE:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_WRONGSOURCE;
                    break;
                case MpiPcAdapterErrorCodes.IDS_NAK:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_NAK;
                    break;
                case MpiPcAdapterErrorCodes.IDS_WRONGSEQUENCE:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_WRONGSEQUENCE;
                    break;
                case MpiPcAdapterErrorCodes.IDS_TOOFEWDATA:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_TOOFEWDATA;
                    break;
                case MpiPcAdapterErrorCodes.IDS_WRITEFAILED:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_WRITEFAILED;
                    break;
                case MpiPcAdapterErrorCodes.IDS_TOOMANYCONNECTION:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_TOOMANYCONNECTION;
                    break;
            }


        }

        #endregion
    }
}
