using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;

namespace HilscherCifXmultiProtocol
{
    class HilscherCifXmultiProtocolStation : Station
    {
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public HilscherCifXmultiProtocolStation(CommunicationDriver commdriver, HilscherCifXmultiProtocolStationSettings settings)
            : base(commdriver, settings)
        {
        }
       
        #endregion
        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as HilscherCifXmultiProtocolCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new HilscherCifXmultiProtocolCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as HilscherCifXmultiProtocolTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new HilscherCifXmultiProtocolCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new HilscherCifXmultiProtocolTag(td);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as HilscherCifXmultiProtocolCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new HilscherCifXmultiProtocolCommJobSettings(session, commJob);
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
            HilscherCifXmultiProtocolCommJob hcJ = e.Job as HilscherCifXmultiProtocolCommJob;
            if (hcJ == null)
            {
                return;
            }

            if ((e.ErrorCode == DriverErrorCodes.ErrorNoError) && (hcJ.ExecutedInOutputMode == false))
            {
                byte[] Answer = (byte[])e.Values;
                List<object> ChangedTags = new List<object>();
                if (ParseReceivedData(Answer, ref hcJ, ref ChangedTags))
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
            //put all the job in error?
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut);
            base.ProcessJobValues(e);
        }

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            HilscherCifXmultiProtocolCommJob hj = job as HilscherCifXmultiProtocolCommJob;

            ParseReceivedData(receivedbuffer, ref hj, ref arguments);
            return true;
        }

        #endregion

        #region Specific Methods

        private bool ParseReceivedData(byte[] receivebuffer, ref HilscherCifXmultiProtocolCommJob hcJ,
                                       ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();

            int ReceivedBytes = receivebuffer.Length;
            if (ReceivedBytes < hcJ.TotalJobSize)
            {
                return false;
            }

            byte[] tempBuffer = new byte[hcJ.TotalJobSize];
            for (uint i = 0; i < hcJ.TotalJobSize; i++)
            {
                tempBuffer[i] = receivebuffer[i];
            }

            hcJ.SetJobData(tempBuffer, ref changed);
            items.AddRange(changed);

            return true;
        }

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
                    HilscherCifXmultiProtocolDynTagSettings dts = new HilscherCifXmultiProtocolDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string cx = string.Format("DS{0}TL{1}MI{2}DA{3}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.DataAddress.ToString("00000"));
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}DA{3}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.DataAddress.ToString("00000"));
                    return cx.CompareTo(cy);
                }
            }
        }

        public void SetOutputDataAtStartup()
        {
            foreach (CommJob jb in ListWholeJob)
            {
                if (jb.Type != LinkType.Input)
                {
                    foreach (Tag tg in jb.TagsList)
                    {
                        jb.AddTagListOnWriting(tg);
                    }
                }
            }
        }

        #endregion

        #region Properties

        #endregion
    }
}
