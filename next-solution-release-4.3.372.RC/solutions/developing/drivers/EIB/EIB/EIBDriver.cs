using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using Utilities;
using Amib.Threading;

namespace EIB
{
    public enum COMMPORT : byte
    {
        Com1,
        Com2,
        Com3,
        Com4,
        Com5,
        Com6,
        Com7,
        Com8,
        Com9,
        Com10
    }

    public enum ConnectorTypes : byte
    {
        KnxIpRouting,
        KnxIpTunneling,
        //USB = 1,
        //KnxIpTunneling = 2,
        //KnxIpRouting = 3,
        //KnxIpcEmiTl = 4,
        //Eiblib = 5
    }

    public enum EISDATAFORMAT : byte
    {
        EISDFBit,
        EISDFByte,
        EISDFWord,
        EISDFDWord,
        EISDFFloat,
        EISDFEIS3,
        EISDFEIS4,
        EISDFEIS5,
        EISDFEIS6,
        EISDFAccessPWD6Bytes,
        EISDFAccessPWD10Bytes,
        EISDFInt64
    }

    public enum EIB_ERROR_CODES: int
    {
        DeviceWriteErrorWriteError = EIBDriver.EIB_WRITE_ERROR + 1,
        DeviceWriteErrorInvalidMessage = EIBDriver.EIB_WRITE_ERROR + 2,
        DeviceWriteErrorDriverNotOpen = EIBDriver.EIB_WRITE_ERROR + 3,
        DeviceWriteErrorQueueOverflow = EIBDriver.EIB_WRITE_ERROR + 4,
        DeviceWriteErrorUnexpectedError = EIBDriver.EIB_WRITE_ERROR + 255,
        DeviceFalconErrorConfirmationRead = EIBDriver.EIB_FALCON_ERROR + 1,
        DeviceFalconErrorConfirmationWrite = EIBDriver.EIB_FALCON_ERROR + 2,
        DeviceFalconErrorConnectionBroken = EIBDriver.EIB_FALCON_ERROR + 3,
        DeviceFalconErrorTimeOut = EIBDriver.EIB_FALCON_ERROR + 4,
    }

    public class EIBDriver : CommunicationDriver
    {
        public const int EIB_WRITE_ERROR = 1000;
        public const int EIB_FALCON_ERROR = 2000;
        
        #region Constructors
        public EIBDriver()
            : base()
        {
        }
        #endregion

        #region Overrides
        public override bool Init(string strSettingPath, bool isProtected, Guid protectionCode)
        {
#if !DEBUG
            if (!MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxDkmnbdFT0I5Fy1y5SsWaqg=="/*FCS*/))
            {
                throw new LicenseOptionMissingException("Missing 'FCS' option in the license");
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
                    var configuration = (from tag in new XPQuery<EIBDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as EIBDriverSettings;
            if (conf == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidDriverSettings);
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<EIBDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new EIBDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as EIBDriverSettings;
            if (conf == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidDriverSettings);

            //TODO: Extra EIB Driver Settings storing
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as EIBChannelSettings;
            if (conf == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidChannelSettings);
            conf.PollingTimeNotInUse = 0;
            return new EIBChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as EIBStationSettings;
            if (conf == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidStationSettings);

            return new EIBStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new EIBTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new EIBTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as EIBTag;
            if (tagItem == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidTagObject);

            return new TagSettings(session, tagItem);
        }

        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<EIBChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);
            switch((EIB_ERROR_CODES)errorcode)
            {
                case EIB_ERROR_CODES.DeviceWriteErrorWriteError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWriteError;
                    break;
                case EIB_ERROR_CODES.DeviceWriteErrorInvalidMessage:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorInvalidMessage;
                    break;
                case EIB_ERROR_CODES.DeviceWriteErrorDriverNotOpen:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorDriverNotOpen;
                    break;
                case EIB_ERROR_CODES.DeviceWriteErrorQueueOverflow:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorQueueOverflow;
                    break;
                case EIB_ERROR_CODES.DeviceWriteErrorUnexpectedError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnexpectedError;
                    break;
                case EIB_ERROR_CODES.DeviceFalconErrorConfirmationRead:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorConfirmationRead;
                    break;
                case EIB_ERROR_CODES.DeviceFalconErrorConfirmationWrite:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorConfirmationWrite;
                    break;
                case EIB_ERROR_CODES.DeviceFalconErrorConnectionBroken:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorConnectionBroken;
                    break;
                case EIB_ERROR_CODES.DeviceFalconErrorTimeOut:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorTimeOut;
                    break;
            }
            System.Diagnostics.Debug.WriteLine(string.Format("Error: {0} ", error));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Prepares this object for use. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool Startup()
        {
            if (_SmartThreadPool == null)
            {
                var startupInfo = new STPStartInfo()
                {
                    ThreadPoolName = "EIBDriver",
                    ThreadPriority = System.Threading.ThreadPriority.Normal,
                    MaxWorkerThreads = SysInfo.GetNumberOfLogicalProcessors(),
                    AreThreadsBackground = false
                };

                _SmartThreadPool = new SmartThreadPool(startupInfo);
            }


            return (base.Startup());
        }
        #endregion

        #region Properties

        private string _AccessProtocolCode;
        public string AccessProtocolCode
        {
            get { return _AccessProtocolCode; }
            set
            {
                _AccessProtocolCode = value;
            }
        }

        /// <summary>   The smart thread pool. </summary>
        SmartThreadPool _SmartThreadPool;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the smart thread pool. </summary>
        ///
        /// <value> The smart thread pool. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal SmartThreadPool SmartThreadPool
        {
            get
            {
                return _SmartThreadPool;
            }
        }

        #endregion

        #region IDisposable
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Dispose()
        {
            base.Dispose();

            if (_SmartThreadPool != null)
            {
                _SmartThreadPool.WaitForIdle();
                _SmartThreadPool.Shutdown();
                _SmartThreadPool = null;
            }
        }

        #endregion
    }
}
