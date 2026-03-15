using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverBaseInterfaces;
using DevExpress.Xpo;
using Opc.Ua;
using System.Text.RegularExpressions;
using DevExpress.Utils.About;

namespace S7TCP
{
    public class S7TCPDriver : CommunicationDriver
    {
        public static string info;
        public S7TCPDriver()
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
            //*******
            string[] sa = {"E10.3", "I10.3", "EB124", "IB124", "EW124", "IW124", "ED124", "ID124", "A13.5", 
                              "Q13.5", "AB124", "QB124", "AW124", "QW124", "AD124", "QD124", "PE124.0",
                              "PB124", "PW124", "PA124", "M300.0", "F300.0", "MB300", "FB300", "MW32", 
                              "FW32", "MW32,C", "FW32,C", "MD114", "FD114", "DB1.DBX40.0", "DB2.DBB21",
                              "DB3.DBW40", "DB3.DBW40,C", "DB4.DBD24", "T0", "Z0", "C0"};

            string s;
            Regex r = new Regex(@"^DB(?<num>\d+).DB(?<tipo>\w)(?<add>\d+)(?<sep>[,.])?(?<bit>\d+)?(?<conv>\w)?");
            Regex t = new Regex(@"^(?<area>[EIAQPEMFTZC]+)(?<tipo>[BWDX]{1})?(?<add>\d+)(?<sep>[,.])?(?<bit>\d+)?(?<conv>[TC]{1})?");
            
            foreach(string cand in sa)
            {
                Match m = r.Match(cand);
                if (m.Success)
                {
                    s = string.Format("*****************\nDB Address\n{6}\n num:'{0}'\n tipo:'{1}'\n add:'{2}'\n sep:'{3}'\n bit:'{4}'\n conv:'{5}'\n", m.Groups["num"].Value,
                        m.Groups["tipo"].Value, m.Groups["add"].Value, m.Groups["sep"].Value,
                        m.Groups["bit"].Value, m.Groups["conv"].Value, cand);
                    //System.Diagnostics.Trace.TraceInformation(s);
                }
                else
                {
                    Match m1 = t.Match(cand);
                    if (m1.Success)
                    {
                        s = string.Format("*****************\nOther Address\n{6}\n area:'{0}'\n tipo:'{1}'\n add:'{2}'\n sep:'{3}'\n bit:'{4}'\n conv:'{5}'\n", m1.Groups["area"].Value,
                        m1.Groups["tipo"].Value, m1.Groups["add"].Value, m1.Groups["sep"].Value,
                        m1.Groups["bit"].Value, m1.Groups["conv"].Value, cand);
                        //System.Diagnostics.Trace.TraceInformation(s);
                    }
                }
            }
            
            return base.Init(strSettingPath, isProtected, protectionCode);
        }

        // Used only to import data from PLC 
        public S7TCPDriver(string strSettingPath)
            : base(strSettingPath)
        {
        }

        public override bool LoadDriverSettings(IDataLayer idl)
        {
            bool bRet = false;
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                try
                {
                    var configuration = (from tag in new XPQuery<S7TCPDriverSettings>(ufw).AsParallel() select tag).Single();
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

            var conf = settings as S7TCPDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

        }

        public override void SaveDriverSettings(IDataLayer idl)
        {
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var configuration = (from tag in new XPQuery<S7TCPDriverSettings>(ufw).AsParallel() select tag).ToList();

                if (configuration.Count == 0)
                    configuration.Add(new S7TCPDriverSettings(ufw));
                SaveDriverSettings(configuration[0]);
                ufw.CommitChanges();
            }
        }

        public override void SaveDriverSettings(DriverSettings settings)
        {
            base.SaveDriverSettings(settings);

            var conf = settings as S7TCPDriverSettings;
            if (conf == null)
                throw new ArgumentException("Invalid driver settings");

        }

        public override Channel CreateChannel(ChannelSettings settings)
        {
            var conf = settings as S7TCPChannelSettings;
            if (conf == null)
                throw new ArgumentException("Invalid channel settings");

            return new S7TCPChannel(this, conf);
        }

        public override Station CreateStation(StationSettings settings)
        {
            var conf = settings as S7TCPStationSettings;
            if (conf == null)
                throw new ArgumentException("Invalid station settings");

            return new S7TCPStation(this, conf);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd)
        {
            return new S7TCPTag(tagtoAdd);
        }

        public override Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset)
        {
            return new S7TCPTag(tagtoAdd, byteoffset, bitoffset);
        }

        public override TagSettings CreateTagSettings(Session session, Tag tag)
        {
            var tagItem = tag as S7TCPTag;
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
                return new List<ChangeTag>((new XPQuery<S7TCPChangeTag>(ufw)/*.AsParallel()*/ ).ToList());
            }
        }

        public override bool TestComm(ChannelSettings ch, StationSettings st, BuiltInType tagDataType, string dynamicSettings, out string error)
        {
            error = null;

            Channel channel = CreateChannel(ch);
            Channels[channel.Name] = channel;
            Channels[channel.Name].Init();

            Station station = CreateStation(st);
            Stations[station.Name] = station;
            Stations[station.Name].Init(channel);

            var conf = ch as S7TCPChannelSettings;
            //Enable test execution
            info = string.Empty;
            ((S7TCPChannel)channel).TestConnection = true;
                       
            IList<object> dataValues = new List<Object>();
            switch (tagDataType)
            {
                case BuiltInType.Boolean:
                    dataValues.Add(new Variant(Convert.ToBoolean(0), new Opc.Ua.TypeInfo(tagDataType, ValueRanks.Scalar)));
                    break;
                case BuiltInType.Byte:
                    dataValues.Add(new Variant(Convert.ToByte(0), new Opc.Ua.TypeInfo(tagDataType, ValueRanks.Scalar)));
                    break;
                case BuiltInType.Int16:
                    dataValues.Add(new Variant(Convert.ToInt16(0), new Opc.Ua.TypeInfo(tagDataType, ValueRanks.Scalar)));
                    break;
                case BuiltInType.Int32:
                    dataValues.Add(new Variant(Convert.ToInt32(0), new Opc.Ua.TypeInfo(tagDataType, ValueRanks.Scalar)));
                    break;
                case BuiltInType.Float:
                    dataValues.Add(new Variant(Convert.ToSingle(0), new Opc.Ua.TypeInfo(tagDataType, ValueRanks.Scalar)));
                    break;
                case BuiltInType.Double:
                    dataValues.Add(new Variant(Convert.ToDouble(0), new Opc.Ua.TypeInfo(tagDataType, ValueRanks.Scalar)));
                    break;
                default:
                    dataValues.Add(new Variant(Convert.ToInt16(0), new Opc.Ua.TypeInfo(tagDataType, ValueRanks.Scalar)));
                    break;
            }

            IList<object> dataValuesInfo = new List<Object>();
            foreach (var value in dataValues)   
                dataValuesInfo.Add(value);

            uint conn = OnReadValues(string.Format(dynamicSettings, station.Name), dataValues);
            if (conn != StatusCodes.Good)
            {
                error = String.Format(Properties.Resources.ErroConnectionFailed);
            }
            else
            {
                error = info;

                ((S7TCPChannel)channel).TestConnection = false;

                conn = OnReadValues(string.Format(dynamicSettings, station.Name), dataValuesInfo);
                if (conn != StatusCodes.Good)
                    error = string.Format("{0}\n{1}", error , String.Format(Properties.Resources.ErroConnectionEstablishedButPugGetFailed));
                else
                    error = info;
            }
            //Disable test execution
            ((S7TCPChannel)channel).TestConnection = false;
            
            return (conn == StatusCodes.Good);

        }

        public static void Addinfo(string stinfo)
        {
            if (string.IsNullOrWhiteSpace(info))
                info = stinfo;
            else
                info = string.Format("{0}\n{1}", info, stinfo);
        }

        public override void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            base.GetDriverErrorInfo(errorcode, out quality, out error);

            switch ((S7ErrorCodes)errorcode)
            {

                case S7ErrorCodes.ErrorWrongHeader:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorWrongHeader;
                    break;
                case S7ErrorCodes.ErrorTooFewData:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorTooFewData;
                    break;
                case S7ErrorCodes.ErrorConnectionToDevice:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorConnection;
                    break;
                case S7ErrorCodes.ErrorUnrecognizedReply:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorUnrecognizedReply;
                    break;
                case S7ErrorCodes.ErrorPrepareRequestFailed:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorPrepareRequestFailed;
                    break;
                case S7ErrorCodes.ErrorExceptionParsingReply:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorExceptionParsingReply;
                    break;
            }


            if ((S7ErrorCodes)errorcode >= S7ErrorCodes.ErrorFromDevice)
            {
                quality = StatusCodes.BadCommunicationError;
                error = string.Format(Properties.Resources.ErrorFromDevice, S7Protocol.GetErrorString(errorcode - (int)S7ErrorCodes.ErrorFromDevice));
            }
            else if ((S7ErrorCodes)errorcode >= S7ErrorCodes.ErrorStatusNotZero)
            {
                quality = StatusCodes.BadCommunicationError;
                error = string.Format(Properties.Resources.ErrorStatusNotZero, S7Protocol.GetErrorString(errorcode - (int)S7ErrorCodes.ErrorStatusNotZero));
            }
        }
        #endregion
    }
}
