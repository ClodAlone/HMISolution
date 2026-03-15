using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;

namespace MelsecFX
{
    public class MelsecFXStation : Station
    {

        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public MelsecFXStation(CommunicationDriver commdriver, MelsecFXStationSettings settings)
            : base(commdriver, settings)
        {
            _PLCType = settings.PLCType;
        }
       
        #endregion

        #region Abstract Methods
 
        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as MelsecFXCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new MelsecFXCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as MelsecFXTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new MelsecFXCommJob(this, conf);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as MelsecFXCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new MelsecFXCommJobSettings(session, commJob);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new MelsecFXTag(td);
        }

        public override void ProcessJobValues(ExecutedJobArgs e/* CommJob job, DriverErrorCodes error*/)
        {
            MelsecFXCommJob mJ = e.Job as MelsecFXCommJob;
            if (mJ == null)
            {
                return;
            }

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                List<object> ChangedTags = new List<object>();
                if (e.Job.IsCustomJob(CommJob.CustomJobCheckStates.Read))
                {
                    e.Job.GetCustomJobsChangedTags(ref ChangedTags);
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
                    byte[] Answer = (byte[])e.Values;                    
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
                    MelsecFXDynTagSettings dts = new MelsecFXDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    MelsecFXAddress addressObj = new MelsecFXAddress(dts.Address);
                    string cx = string.Format("DS{0}TL{1}MI{2}{3}Addr{4}",
                        string.Empty, dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), addressObj.DataArea.ToString(), addressObj.StartAddress.ToString("0000000"));
                    dts.TryParse(y.TagNode.DynamicSettings);
                    addressObj = new MelsecFXAddress(dts.Address);
                    string cy = string.Format("DS{0}TL{1}MI{2}{3}Addr{4}",
                        string.Empty, dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), addressObj.DataArea.ToString(), addressObj.StartAddress.ToString("0000000"));
                    return cx.CompareTo(cy);
                }
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// PLC Type
        /// </summary>
        private MelsecFXPLCType _PLCType;
        public MelsecFXPLCType PLCType
        {
            get
            {
                return _PLCType;
            }
            set
            {
                _PLCType = value;
            }
        }

        #endregion
    }
}
