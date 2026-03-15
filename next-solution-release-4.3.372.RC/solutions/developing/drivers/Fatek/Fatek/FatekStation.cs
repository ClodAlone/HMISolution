using System;
using System.Collections.Generic;
using DevExpress.Xpo;
using DriverCodeBase;
using DriverCodeBase.Enumerators;

namespace Fatek
{
    class FatekStation : Station
    {
                
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public FatekStation(CommunicationDriver commdriver, FatekStationSettings settings)
            : base(commdriver, settings)
        {
            _StationID = settings.StationID;
        }
       
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as FatekCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new FatekCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as FatekTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new FatekCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new FatekTag(td);
        }
        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as FatekCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new FatekCommJobSettings(session, commJob);
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
                    FatekDynTagSettings dts = new FatekDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string cx = string.Format("DS{0}TL{1}MI{2}SA{3}SAA{4}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"),
                        dts.Area,
                        dts.AreaAddress.ToString("0000000")
                        );
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}SA{3}SAA{4}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"),
                        dts.Area,
                        dts.AreaAddress.ToString("0000000")
                        );
                    return cx.CompareTo(cy);
                    //return x.TagNode.DynamicSettings.CompareTo(y.TagNode.DynamicSettings);
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
            FatekCommJob mJ = e.Job as FatekCommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                if (e.Values != null)
                {
                    byte[] Answer = (byte[])e.Values;
                    List<object> ChangedTags = new List<object>();                
                    if (/*P*/FatekProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
                    {
                        foreach (var tag in ChangedTags)
                        {
                            var j = tag as Tag;
                            if (j != null)
                                e.ChangedTags.Add(j);
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
            FatekCommJob mj = job as FatekCommJob;

            FatekProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
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
        #endregion
    }
}
