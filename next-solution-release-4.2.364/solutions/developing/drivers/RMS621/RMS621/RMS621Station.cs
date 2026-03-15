using System;
using System.Collections.Generic;
using DevExpress.Xpo;
using DriverCodeBase;
using DriverCodeBase.Enumerators;

namespace RMS621
{
    public class RMS621Station : Station
    {

        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public RMS621Station(CommunicationDriver commdriver, RMS621StationSettings settings)
            : base(commdriver, settings)
        {
            _UnitNumber = settings.UnitNumber;
        }
       
        #endregion

        #region Abstract Methods
 
        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as RMS621CommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new RMS621CommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as RMS621Tag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new RMS621CommJob(this, conf);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as RMS621CommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new RMS621CommJobSettings(session, commJob);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new RMS621Tag(td);
        }

        public override void ProcessJobValues(ExecutedJobArgs e/* CommJob job, DriverErrorCodes error*/)
        {
            RMS621CommJob mJ = e.Job as RMS621CommJob;
            if (mJ == null)
            {
                return;
            }

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                byte[] Answer = (byte[])e.Values;
                List<object> ChangedTags = new List<object>();
                if (RMS621Protocol.ParseData(Answer, ref mJ, ref ChangedTags))
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

        public override List<Tag> SortTags(IList<Tag> tags)
        {
            List<Tag> listTag = new List<Tag>();
            listTag.AddRange(tags);
            listTag.Sort(CompareTagByDynamic);
            return listTag;
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
                    RMS621DynTagSettings dts = new RMS621DynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    //string cx = string.Format("DS{0}TL{1}MI{2}{3}Addr{4}",
                    string cx = string.Format("DS{0}TL{1}MI{2}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"));//, dts..ToString(), addressObj.StartAddress.ToString("0000000"));
                    dts.TryParse(y.TagNode.DynamicSettings);
                    //addressObj = new RMS621Address(dts.Address);
                    //string cy = string.Format("DS{0}TL{1}MI{2}{3}Addr{4}",
                    string cy = string.Format("DS{0}TL{1}MI{2}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"));//, addressObj.DataArea.ToString(), addressObj.StartAddress.ToString("0000000"));
                    return cx.CompareTo(cy);
                }
            }
        }
        #endregion

        #region Properties
                
        private byte _UnitNumber;
        /// <summary>
        /// Device unit number
        /// </summary>
        public byte UnitNumber
        {
            get
            {
                return _UnitNumber;
            }
            set
            {
                _UnitNumber = value;
            }
        }

        #endregion
    }
}
