using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;

namespace MelsecQEth
{
    class MelsecQEthStation : Station
    {
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public MelsecQEthStation(CommunicationDriver commdriver, MelsecQEthStationSettings settings)
            : base(commdriver, settings)
        {
            NetworkNumber = settings.NetworkNumber;
            PcNumber = settings.PcNumber;
            MaxRetriesBeforeError = settings.MaxRetriesBeforeError;
        }

        #endregion

        #region Override Methods

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new MelsecQEthTag(td);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as MelsecQEthCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new MelsecQEthCommJobSettings(session, commJob);
        }

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as MelsecQEthCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new MelsecQEthCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as MelsecQEthTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new MelsecQEthCommJob(this, conf);
        }

        public override List<Tag> SortTags(IList<Tag> tags)
        {
            List<Tag> listTag = new List<Tag>();
            listTag.AddRange(tags);
            listTag.Sort(CompareTagByDynamic);
            return listTag;
        }

        public override void ProcessJobValues(ExecutedJobArgs e/* CommJob job, DriverErrorCodes error*/)
        {
            MelsecQEthCommJob mJ = e.Job as MelsecQEthCommJob;
            if (mJ == null)
            {
                return;
            }

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                if (e.Values != null)
                {
                    //MelsecQProtocol P = new MelsecQProtocol();
                    byte[] Answer = (byte[])e.Values;
                    List<object> ChangedTags = new List<object>();
                    if (mJ.ParseData(Answer, ref ChangedTags))
                    {
                        foreach (var tag in ChangedTags)
                        {
                            var j = tag as Tag;
                            if (j != null)
                            {
                                e.ChangedTags.Add(j);
                            }
                        }
                    }
                    else
                    {
                        e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                    }
                }
            }

            //put all the job in error?
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut);

            base.ProcessJobValues(e);
        }

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            MelsecQEthCommJob mj = job as MelsecQEthCommJob;

            mj.ParseData(receivedbuffer, ref arguments);

            return true;
        }

        #endregion

        #region static methods
        private static int CompareTagByDynamic(Tag x, Tag y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0; //==
                }
                else
                {
                    return -1;// x < y
                }
            }
            else
            {
                //x!= null
                if (y == null)
                {
                    return 1; //x > y
                }
                else
                {
                    MelsecQEthDynTagSettings dts = new MelsecQEthDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string cx = string.Format("DS{0}TL{1}MI{2}Addr{3}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.Address,
                        dts.StringLength.ToString("00000"));
                        
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}Addr{3}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.Address,
                        dts.StringLength.ToString("00000"));
                    return cx.CompareTo(cy);
                }
            }
        }
        #endregion
 
        #region Properties

        private uint _NetworkNumber;
        public uint NetworkNumber
        {
            get { return _NetworkNumber; }
            set
            {
                _NetworkNumber = value;
            }
        }
        private uint _PcNumber;
        public uint PcNumber
        {
            get { return _PcNumber; }
            set
            {
                _PcNumber = value;
            }
        }

        #endregion
    }
}
