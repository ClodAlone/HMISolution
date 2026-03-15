using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using System.Text.RegularExpressions;

namespace PPI
{
    public class PPIDriver : CommunicationDriver
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
                    var configuration = (from tag in new XPQuery<PPIDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as PPIDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<PPIDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new PPIDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as PPIDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as PPIChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new PPIChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as PPIStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new PPIStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new PPITag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new PPITag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as PPITag;
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
                return new List<ChangeTag>((new XPQuery<PPIChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((PPIErrorCodes)errorcode)
            {

                case PPIErrorCodes.IDS_ERRDEVICEWRITE:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_ERRDEVICEWRITE;
                    break;
                case PPIErrorCodes.IDS_SERIALREADFAILED:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_SERIALREADFAILED;
                    break;
                case PPIErrorCodes.IDS_ETXNOTRECEIVED:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_ETXNOTRECEIVED;
                    break;
                case PPIErrorCodes.IDS_PLCBCC:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_PLCBCC;
                    break;
                case PPIErrorCodes.IDS_ANSWERTOOSHORT:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_ANSWERTOOSHORT;
                    break;
                case PPIErrorCodes.IDS_UNRECOGNIZEDANSWER:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_UNRECOGNIZEDANSWER;
                    break;
                case PPIErrorCodes.IDS_RETURNEDERR:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_RETURNEDERR;
                    break;
                case PPIErrorCodes.IDS_WRONGDESTINATION:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_WRONGDESTINATION;
                    break;
                case PPIErrorCodes.IDS_CONNREFUSED:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_CONNREFUSED;
                    break;
                case PPIErrorCodes.IDS_WRONGSOURCE:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_WRONGSOURCE;
                    break;
                case PPIErrorCodes.IDS_NAK:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_NAK;
                    break;
                case PPIErrorCodes.IDS_WRONGSEQUENCE:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_WRONGSEQUENCE;
                    break;
                case PPIErrorCodes.IDS_TOOFEWDATA:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_TOOFEWDATA;
                    break;
                case PPIErrorCodes.IDS_WRITEFAILED:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_WRITEFAILED;
                    break;
                case PPIErrorCodes.IDS_TOOMANYCONNECTION:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_TOOMANYCONNECTION;
                    break;
                case PPIErrorCodes.IDS_OVERLAPADDRESSES:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_OVERLAPADDRESSES;
                    break;
                case PPIErrorCodes.IDS_INVALIDTAG:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_INVALIDTAG;
                    break;
                case PPIErrorCodes.IDS_TIMEOUTRX:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_TIMEOUTRX;
                    break;
                case PPIErrorCodes.IDS_NAKRECEIVED:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_NAKRECEIVED;
                    break;
                case PPIErrorCodes.IDS_BADRXCHARS:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_BADRXCHARS;
                    break;
                case PPIErrorCodes.IDS_STATUSNOTZERO:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_STATUSNOTZERO;
                    break;
                case PPIErrorCodes.IDS_INVALIDSEQNUMBER:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_INVALIDSEQNUMBER;
                    break;
                case PPIErrorCodes.IDS_HWFAULT:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_HWFAULT;
                    break;
                case PPIErrorCodes.IDS_ILLEGALOBJACCESS:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_ILLEGALOBJACCESS;
                    break;
                case PPIErrorCodes.IDS_INVALIDADDRESS:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_INVALIDADDRESS;
                    break;
                case PPIErrorCodes.IDS_DATATYPENOTSUPP:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_DATATYPENOTSUPP;
                    break;
                case PPIErrorCodes.IDS_OBJNOTEXIST:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.IDS_OBJNOTEXIST;
                    break;
            }


        }

        #endregion
    }
}
