using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using System.IO;

namespace DriverSerialExample
{
    

    public class DriverSerialExampleDriver : CommunicationDriver
    {
        
        public DriverSerialExampleDriver()
            : base()
        {
        }

        public DriverSerialExampleDriver(string strSettingPath)
            : base(strSettingPath)
        {
        }        

        #region Overrides       
        public override bool LoadDriverSettings(IDataLayer idl)
        {
#if DEBUG
            //using (UnitOfWork ufw = new UnitOfWork(idl))
            //{
            //    var configuration = (from tag in new XPQuery<DriverSerialExampleDriverSettings>(ufw).AsParallel() select tag).ToList();
            //    foreach (var drvsettings in configuration)
            //        drvsettings.Delete();
            //    ufw.CommitChanges();
            //}

            //LoadDefaultSettings();
            //SaveDriverSettings(idl);

            //using (UnitOfWork ufw = new UnitOfWork(idl))
            //{
            //    var configuration = (from tag in new XPQuery<DriverSerialExampleDriverSettings>(ufw).AsParallel() select tag).Single();
            //    configuration.ChannelSettings.Add(new DriverSerialExampleChannelSettings(ufw) 
            //    { 
            //        Name = "MyChannel", 
            //        CommPortName = "COM3",
            //        CommPortBaudRate = 19200,//38400,//9600, steve 020511
            //        CommPortDataBits = 8,
            //        CommPortParity = System.IO.Ports.Parity.None,//Even,
            //        CommPortStopBits = System.IO.Ports.StopBits.One,
            //        CommPortWriteTimeout = 5000,
            //        CommPortReadTimeout = 5000,
            //        CommPortRtsEnable = false,
            //        CommPortDtrEnable = false,
            //        CommPortHandshake = System.IO.Ports.Handshake.None,
            //        WaitTime = 10,
            //        Timeout = 1500,
            //        ScheduleTimeJobsList = 100,
            //        PollingTimeNotInUse = 10000,
            //        PollingTimeInError = 10000,
            //        KeepOpened = true,
            //        FrameType = 0,//0, 0: RTU 1: ASCII
            //        TurnaroundDelay = 100
            //    });
            //    configuration.StationSettings.Add(new DriverSerialExampleStationSettings(ufw) 
            //    { 
            //        Name = "MyStation", 
            //        Channel = "MyChannel",
            //        StationID = 1,
            //        MaxRetriesBeforeError = 1,
            //    });
            //    ufw.CommitChanges();
            //}
#endif
            bool bRet = false;
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                try
                {
                    var configuration = (from tag in new XPQuery<DriverSerialExampleDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as DriverSerialExampleDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra DriverSerialExample Driver Settings initializzation
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<DriverSerialExampleDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new DriverSerialExampleDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as DriverSerialExampleDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra DriverSerialExample Driver Settings initializzation
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as DriverSerialExampleChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new DriverSerialExampleSerialChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as DriverSerialExampleStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new DriverSerialExampleStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new DriverSerialExampleTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new DriverSerialExampleTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as DriverSerialExampleTag;
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
                return new List<ChangeTag>((new XPQuery<DriverSerialExampleChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((DriverSerialExampleErrorCodes)errorcode)
            {
                case DriverSerialExampleErrorCodes.ErrorCRCError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorCRCError;
                    break;
                case DriverSerialExampleErrorCodes.ErrorUnknownFunctionCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnknownFunctionCode;
                    break;
                case DriverSerialExampleErrorCodes.ErrorReplayCodeMalformed:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReplayCodeMalformed;
                    break;
                case DriverSerialExampleErrorCodes.ErrorErrorCodeMalformed:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCodeMalformed;
                    break;
                case DriverSerialExampleErrorCodes.ErrorReplayFrameMalformed:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReplayFrameMalformed;
                    break;
                case DriverSerialExampleErrorCodes.ErrorReceivedDataMalformed:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReceivedDataMalformed;
                    break;
                case DriverSerialExampleErrorCodes.ErrorLRCMalformed:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorLRCMalformed;
                    break;
                case DriverSerialExampleErrorCodes.ErrorProtocolIllegalFunction:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolIllegalFunction;
                    break;
                case DriverSerialExampleErrorCodes.ErrorProtocolIllegalDataAddress:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolIllegalDataAddress;
                    break;
                case DriverSerialExampleErrorCodes.ErrorProtocolIllegalDataValue:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolIllegalDataValue;
                    break;
                case DriverSerialExampleErrorCodes.ErrorProtocolSlaveDeviceFailure:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolSlaveDeviceFailure;
                    break;
                case DriverSerialExampleErrorCodes.ErrorProtocolAcknowledge:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolAcknowledge;
                    break;
                case DriverSerialExampleErrorCodes.ErrorProtocolSlaveDeviceBusy:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolSlaveDeviceBusy;
                    break;
                case DriverSerialExampleErrorCodes.ErrorProtocolMemoryParityError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolMemoryParityError;
                    break;
                case DriverSerialExampleErrorCodes.ErrorProtocolGatewayPathUnavailable:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolGatewayPathUnavailable;
                    break;
                case DriverSerialExampleErrorCodes.ErrorProtocolGatewayTargetFailResponse:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolGatewayTargetFailResponse;
                    break;
                case DriverSerialExampleErrorCodes.ErrorWrongSize:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWrongSize;
                    break;
                case DriverSerialExampleErrorCodes.ErrorStationID:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorStationID;
                    break;
                case DriverSerialExampleErrorCodes.ErrorFunctionCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorFunctionCode;
                    break;
            }
        }
        #endregion

    }

}
