using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;

namespace ModbusTCPSlave
{
    public class ModbusTCPSlaveDriver : CommunicationDriver
    {
        public ModbusTCPSlaveDriver()
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
                    var configuration = (from tag in new XPQuery<ModbusTCPSlaveDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as ModbusTCPSlaveDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra Modbus Driver Settings initializzation
        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<ModbusTCPSlaveDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new ModbusTCPSlaveDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as ModbusTCPSlaveDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

            //TODO: Extra Modbus Driver Settings initializzation
        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as ModbusTCPSlaveChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new TcpServer(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as ModbusTCPSlaveStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new ModbusTCPSlaveStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new ModbusTCPSlaveTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new ModbusTCPSlaveTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as ModbusTCPSlaveTag;
            if (tagItem == null)
                throw new ArgumentException("Invalid tag object");

            return new TagSettings(session, tagItem);
        }

        public override List<ChangeTag> LoadChangeSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                return new List<ChangeTag>((new XPQuery<ModbusTCPSlaveChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
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
                case ModbusErrorCodes.ErrorReadError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReadError;
                    break;
                case ModbusErrorCodes.ErrorReceiveFrameError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorReceiveFrameError;
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

        protected override bool StartUpScheduler()
        {
            // disable scheduler startup
            return true;
        }

        public override void SuspendScheduler()
        {
            // disable scheduler 
        }

        public override bool RestartScheduler()
        {
            // disable scheduler
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   Prepares this object for use. </summary>
        /////
        ///// <returns>   true if it succeeds, false if it fails. </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //public override bool Startup()
        //{
        //    bool bRet = base.Startup();
        //    foreach (var channel in GetChannels())
        //        //if (!channel.ThreadRunning())
        //            channel.Startup();
        //    return bRet;
        //}

        #endregion
    }
}
