using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverCodeBase;
using DriverBaseInterfaces;
using Opc.Ua;
using DevExpress.Xpo;

namespace TwinCAT
{
    public class TwinCATDriver : CommunicationDriver
    {
        #region Constructors
        public TwinCATDriver()
            : base()
        {
        }

        // Used only to import data from PLC 
        public TwinCATDriver(string strSettingPath)
            : base()
        {
            _strSettingPath = strSettingPath;
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
            // Used only to import data from PLC
            _strSettingPath = strSettingPath;

            return base.Init(strSettingPath, isProtected, protectionCode);
        }

        public override bool LoadDriverSettings(IDataLayer idl)
        {
            bool bRet = false;
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                try
                {
                    var configuration = (from tag in new XPQuery<TwinCATDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as TwinCATDriverSettings;
            if (conf == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidDriverSettings);

        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<TwinCATDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new TwinCATDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as TwinCATDriverSettings;
            if (conf == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidDriverSettings);

            //TODO: Extra TwinCAT Driver Settings storing
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as TwinCATChannelSettings;
            if (conf == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidChannelSettings);

            return new TwinCATChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as TwinCATStationSettings;
            if (conf == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidStationSettings);

            return new TwinCATStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new TwinCATTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new TwinCATTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as TwinCATTag;
            if (tagItem == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidTagObject);

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
                return new List<ChangeTag>((new XPQuery<TwinCATChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        // TODO
        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);
            if(errorcode <= 6)
            {
                return;
            }

            quality = StatusCodes.BadCommunicationError;

            switch ((TwinCATErrorCodes)errorcode)
            {
                case TwinCATErrorCodes.ErrorCodeErrorClassDevice:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1792);
                    break;
                case TwinCATErrorCodes.ErrorCodeServiceNotSupported:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1793);
                    break;
                case TwinCATErrorCodes.ErrorCodeInvalidIndexGroup:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1794);
                    break;
                case TwinCATErrorCodes.ErrorCodeInvalidIndexOffset:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1795);
                    break;
                case TwinCATErrorCodes.ErrorCodeReadWriteNotPermitted:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1796);
                    break;
                case TwinCATErrorCodes.ErrorCodeParameterSizeNotCorrect:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1797);
                    break;
                case TwinCATErrorCodes.ErrorCodeInvalidParameterValue:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1798);
                    break;
                case TwinCATErrorCodes.ErrorCodeDeviceNotReady:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1799);
                    break;
                case TwinCATErrorCodes.ErrorCodeDeviceBusy:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1800);
                    break;
                case TwinCATErrorCodes.ErrorCodeInvalidContext:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1801);
                    break;
                case TwinCATErrorCodes.ErrorCodeOutOfMemory:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1802);
                    break;
                case TwinCATErrorCodes.ErrorCodeInvalidParameterValue2:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1803);
                    break;
                case TwinCATErrorCodes.ErrorCodeNotFound:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1804);
                    break;
                case TwinCATErrorCodes.ErrorCodeSintaxError:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1805);
                    break;
                case TwinCATErrorCodes.ErrorCodeObjectsNotMatch:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1806);
                    break;
                case TwinCATErrorCodes.ErrorCodeObjectsAlreadyExist:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1807);
                    break;
                case TwinCATErrorCodes.ErrorCodeSymbolNotFound:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1808);
                    break;
                case TwinCATErrorCodes.ErrorCodeSymbolVersionInvalid:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1809);
                    break;
                case TwinCATErrorCodes.ErrorCodeServerInvalidState:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1810);
                    break;
                case TwinCATErrorCodes.ErrorCodeAdsTransModeNotSupported:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1811);
                    break;
                case TwinCATErrorCodes.ErrorCodeNotificationHandleInvalid:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1812);
                    break;
                case TwinCATErrorCodes.ErrorCodeNotificationClientNotRegistered:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1813);
                    break;
                case TwinCATErrorCodes.ErrorCodeNoMoreNotificationsHandles:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1814);
                    break;
                case TwinCATErrorCodes.ErrorCodeSizeWatchTooBig:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1815);
                    break;
                case TwinCATErrorCodes.ErrorCodeDeviceNotInitialized:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1816);
                    break;
                case TwinCATErrorCodes.ErrorCodeDeviceTimeout:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1817);
                    break;
                case TwinCATErrorCodes.ErrorCodeQueryInterfaceFailed:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1818);
                    break;
                case TwinCATErrorCodes.ErrorCodeWrongInterfaceRequired:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1819);
                    break;
                case TwinCATErrorCodes.ErrorCodeClassIdInvalid:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1820);
                    break;
                case TwinCATErrorCodes.ErrorCodeObjectIdInvalid:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1821);
                    break;
                case TwinCATErrorCodes.ErrorCodeRequestPending:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1822);
                    break;
                case TwinCATErrorCodes.ErrorCodeRequestAborted:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1823);
                    break;
                case TwinCATErrorCodes.ErrorCodeSignalWarning:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1824);
                    break;
                case TwinCATErrorCodes.ErrorCodeInvalidArrayIndex:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1825);
                    break;
                case TwinCATErrorCodes.ErrorCodeSymbolNotActive:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1826);
                    break;
                case TwinCATErrorCodes.ErrorCodeAccessDenied:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1827);
                    break;
                case TwinCATErrorCodes.ErrorCodeErrorClassClient:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1856);
                    break;
                case TwinCATErrorCodes.ErrorCodeInvalidParameterAtService:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1857);
                    break;
                case TwinCATErrorCodes.ErrorCodepollingListEmpty:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1858);
                    break;
                case TwinCATErrorCodes.ErrorCodeVarConnectionAlreadyInUse:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1859);
                    break;
                case TwinCATErrorCodes.ErrorCodeInvokeIdInUse:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1860);
                    break;
                case TwinCATErrorCodes.ErrorCodeTimeoutElapsed:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1861);
                    break;
                case TwinCATErrorCodes.ErrorCodeErrorInWin32:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1862);
                    break;
                case TwinCATErrorCodes.ErrorCodeInvalidClientTimeoutValue:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1863);
                    break;
                case TwinCATErrorCodes.ErrorCodeAdsPortNotOpened:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1864);
                    break;
                case TwinCATErrorCodes.ErrorCodeInternalErrorInAdsSync:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1872);
                    break;
                case TwinCATErrorCodes.ErrorCodeHashTableOverflow:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1873);
                    break;
                case TwinCATErrorCodes.ErrorCodeKeyNotFoundInHash:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1874);
                    break;
                case TwinCATErrorCodes.ErrorCodeNoMoreSymbolsInCache:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1875);
                    break;
                case TwinCATErrorCodes.ErrorCodeInvalidResponseReceived:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1876);
                    break;
                case TwinCATErrorCodes.ErrorCodeSyncPortLocked:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          Properties.Resources.AdsError1877);
                    break;
                default:
                    error = String.Format(Properties.Resources.AdsGenericError,
                                          errorcode);
                    break;
            }
        }

        private string _strSettingPath;
        public string strSettingPath
        {
            get { return _strSettingPath; }
        }

        #endregion
    }
}
