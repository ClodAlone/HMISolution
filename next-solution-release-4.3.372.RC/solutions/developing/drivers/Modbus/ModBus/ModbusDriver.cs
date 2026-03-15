using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using System.IO;

namespace ModBus
{
    

    public class ModbusDriver : CommunicationDriver
    {
        
        public ModbusDriver()
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

#if NET_STANDARD
            //add additional assembly search's paths (linux don't managed it properly)
            string AssemblySearchLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location).Replace('\\', Path.DirectorySeparatorChar);
            string AssemblyDriverSearchLocation = string.Format("{0}{1}Drivers", AssemblySearchLocation, Path.DirectorySeparatorChar);
            //assembly search by name 
            Utilities.AssemblyResolver.AssemblyResolver resolver = new Utilities.AssemblyResolver.AssemblyResolver(AssemblySearchLocation, AssemblyDriverSearchLocation);
#endif
            return base.Init(strSettingPath, isProtected, protectionCode);
        }
        
        public override bool LoadDriverSettings(IDataLayer idl)
        {
#if DEBUG
            //using (UnitOfWork ufw = new UnitOfWork(idl))
            //{
            //    var configuration = (from tag in new XPQuery<ModbusDriverSettings>(ufw).AsParallel() select tag).ToList();
            //    foreach (var drvsettings in configuration)
            //        drvsettings.Delete();
            //    ufw.CommitChanges();
            //}

            //LoadDefaultSettings();
            //SaveDriverSettings(idl);

            //using (UnitOfWork ufw = new UnitOfWork(idl))
            //{
            //    var configuration = (from tag in new XPQuery<ModbusDriverSettings>(ufw).AsParallel() select tag).Single();
            //    configuration.ChannelSettings.Add(new ModbusChannelSettings(ufw) 
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
            //    configuration.StationSettings.Add(new ModbusStationSettings(ufw) 
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
                    var configuration = (from tag in new XPQuery<ModbusDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as ModbusDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra Modbus Driver Settings initializzation
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<ModbusDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new ModbusDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as ModbusDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra Modbus Driver Settings initializzation
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as ModbusChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new ModbusSerialChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as ModbusStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new ModbusStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new ModbusTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new ModbusTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as ModbusTag;
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
                return new List<ChangeTag>((new XPQuery<ModBusChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((ModbusErrorCodes)errorcode)
            {
                case ModbusErrorCodes.ErrorCRCError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorCRCError;
                    break;
                case ModbusErrorCodes.ErrorUnknownFunctionCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnknownFunctionCode;
                    break;
                case ModbusErrorCodes.ErrorReplayCodeMalformed:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReplayCodeMalformed;
                    break;
                case ModbusErrorCodes.ErrorErrorCodeMalformed:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorErrorCodeMalformed;
                    break;
                case ModbusErrorCodes.ErrorReplayFrameMalformed:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReplayFrameMalformed;
                    break;
                case ModbusErrorCodes.ErrorReceivedDataMalformed:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReceivedDataMalformed;
                    break;
                case ModbusErrorCodes.ErrorLRCMalformed:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorLRCMalformed;
                    break;
                case ModbusErrorCodes.ErrorProtocolIllegalFunction:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolIllegalFunction;
                    break;
                case ModbusErrorCodes.ErrorProtocolIllegalDataAddress:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolIllegalDataAddress;
                    break;
                case ModbusErrorCodes.ErrorProtocolIllegalDataValue:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolIllegalDataValue;
                    break;
                case ModbusErrorCodes.ErrorProtocolSlaveDeviceFailure:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolSlaveDeviceFailure;
                    break;
                case ModbusErrorCodes.ErrorProtocolAcknowledge:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolAcknowledge;
                    break;
                case ModbusErrorCodes.ErrorProtocolSlaveDeviceBusy:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolSlaveDeviceBusy;
                    break;
                case ModbusErrorCodes.ErrorProtocolMemoryParityError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolMemoryParityError;
                    break;
                case ModbusErrorCodes.ErrorProtocolGatewayPathUnavailable:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolGatewayPathUnavailable;
                    break;
                case ModbusErrorCodes.ErrorProtocolGatewayTargetFailResponse:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorProtocolGatewayTargetFailResponse;
                    break;
                case ModbusErrorCodes.ErrorWrongSize:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWrongSize;
                    break;
                case ModbusErrorCodes.ErrorStationID:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorStationID;
                    break;
                case ModbusErrorCodes.ErrorFunctionCode:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorFunctionCode;
                    break;
            }
        }
        #endregion

    }

}
