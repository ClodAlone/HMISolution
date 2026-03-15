using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using DriverCodeBaseEx.Helpers;
using Opc.Ua;

namespace ModbusTCP
{
    public class ModbusTCPStation : Station
    {
                
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public ModbusTCPStation(CommunicationDriver commdriver, ModbusTCPStationSettings settings)
            : base(commdriver, settings)
        {
            _StationID = settings.StationID;
            _AddressType = (AddressTypes)settings.AddressType;
        }

        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as ModbusTCPCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new ModbusTCPCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as ModbusTCPTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new ModbusTCPCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new ModbusTCPTag(td);
        }
        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as ModbusTCPCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new ModbusTCPCommJobSettings(session, commJob);
        }

        #endregion

        private static int CompareTagByDynamic(Tag x, Tag y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0; //==
                else
                    return -1;// x < y
            }
            else
            {
                //x!= null
                if (y == null)
                    return 1; //x > y
                else
                {
                    ModbusTCPDynTagSettings dts = new ModbusTCPDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string cx = string.Format("DS{0}TL{1}MI{2}FC{3}SA{4}FN{5}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.FunctionCode,
                        dts.StartAddress.ToString("00000"), dts.FileNumber.ToString("000"),
                        dts.StringLength.ToString("00000")
                        );
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}FC{3}SA{4}FN{5}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.FunctionCode,
                        dts.StartAddress.ToString("00000"), dts.FileNumber.ToString("000"),
                        dts.StringLength.ToString("00000")
                        );
                    return cx.CompareTo(cy);
                }
            }
        }
        public override List<Tag> SortTags(IList<Tag> tags)
        {
            List<Tag> listTag = new List<Tag>();
            listTag.AddRange(tags);
            listTag.Sort(CompareTagByDynamic);
            return listTag;
        }

        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            ModbusTCPCommJob mJ = e.Job as ModbusTCPCommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                byte[] Answer = (byte[])e.Values;
                List<object> ChangedTags = new List<object>();
                if (ModbusProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
                    foreach (var tag in ChangedTags)
                    {
                        var j = tag as Tag;
                        if (j != null)
                            e.ChangedTags.Add(j);
                    }
                else
                {
                    if (ChangedTags.Count > 0)
                        e.ErrorCode = (DriverErrorCodes)((ModbusErrorCodes)ChangedTags[0]);
                    else
                        e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                }
            }

            //put all the job in error?
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut);

            base.ProcessJobValues(e);
        }

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            ModbusTCPCommJob mj = job as ModbusTCPCommJob;

            ModbusProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }

        #region Properties

        private uint _StationID;
        public uint StationID
        {
            get { return _StationID; }
            set
            {
                _StationID = value;
            }
        }

        private AddressTypes _AddressType;
        public AddressTypes AddressType
        {
            get { return _AddressType; }
            set
            {
                _AddressType = value;
            }
        }

        #endregion

    }
}
