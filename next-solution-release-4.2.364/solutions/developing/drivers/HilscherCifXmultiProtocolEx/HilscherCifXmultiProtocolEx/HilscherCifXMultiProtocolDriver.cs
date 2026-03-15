using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using DriverCodeBaseEx.Helpers;
using DriverCodeBaseEx.Enumerators;

namespace HilscherCifXmultiProtocol
{
    public class HilscherCifXMultiProtocolDriver : CommunicationDriver
    {
        #region Constructors
        public HilscherCifXMultiProtocolDriver()
            : base()
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
                    var configuration = (from tag in new XPQuery<HilscherCifXmultiProtocolDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as HilscherCifXmultiProtocolDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<HilscherCifXmultiProtocolDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new HilscherCifXmultiProtocolDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as HilscherCifXmultiProtocolDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as HilscherCifXmultiProtocolChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new HilscherCifXmultiProtocolChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as HilscherCifXmultiProtocolStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new HilscherCifXmultiProtocolStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new HilscherCifXmultiProtocolTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new HilscherCifXmultiProtocolTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as HilscherCifXmultiProtocolTag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }

        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<HilscherCifXmultiProtocolChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);
            if (errorcode != (int)DriverErrorCodes.ErrorNoError)
            {
                if (errorcode == (int)HilscherErrorCodes.ErrorBusStateOff)
                {
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorBusStateOff;
                }

                // FOGBUGZ 12127
                else if (errorcode == (int)HilscherErrorCodes.ErrorAddressOutOfRange)
                {
                    quality = StatusCodes.BadOutOfRange;
                    error = String.Format(Properties.Resources.ErrorAddressOutOfRange,
                                          LastOutOfRangeAddress, MaxOutOfRangeAddress);
                }

                else if ((errorcode & 0x80000000) != 0)
                {
                    string errorDescription;
                    GetHilscherErrorDescription(errorcode, out errorDescription);
                    error = String.Format(Properties.Resources.ErrorHilscherDriverErrorCode, errorcode, errorDescription);
                    quality = StatusCodes.BadCommunicationError;
                }
            }
        }

        #endregion

        #region Specific Methods

        public string ByteArrayToString(ref byte[] ArrayOfByte)
        {
            try
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                return enc.GetString(ArrayOfByte);
            }
            catch (Exception ex)
            {
                string errorDescription;
                errorDescription = ex.Message.ToString();
                return errorDescription;
            }
        }

        public void GetHilscherErrorDescription(int ErrorNumber, out string errorDescription)
        {
            byte[] szBuffer = new byte[1024];
            UInt32 ulSize = 1024;
            try
            {
                CifXAPI._xDriverGetErrorDescription((UInt32)ErrorNumber, szBuffer, ulSize);
                errorDescription = ByteArrayToString(ref szBuffer);
            }
            catch (Exception ex)
            {
                errorDescription = ex.Message.ToString();
            }
        }

        #endregion

        // FOGBUGZ 12127
        #region Properties
        private UInt32 _LastOutOfRangeAddress;
        public UInt32 LastOutOfRangeAddress
        {
            get
            {
                return _LastOutOfRangeAddress;
            }
            set
            {
                _LastOutOfRangeAddress = value;
            }
        }
        private UInt32 _MaxOutOfRangeAddress;
        public UInt32 MaxOutOfRangeAddress
        {
            get
            {
                return _MaxOutOfRangeAddress;
            }
            set
            {
                _MaxOutOfRangeAddress = value;
            }
        }
        #endregion
    }
}
