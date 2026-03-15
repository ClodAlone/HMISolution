using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;

namespace DriverSerialExample
{
    public class DriverSerialExampleStation : Station
    {
        
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public DriverSerialExampleStation(CommunicationDriver commdriver, DriverSerialExampleStationSettings settings)
            : base(commdriver, settings)
        {
            _StationID = settings.StationID;
        }
       
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as DriverSerialExampleCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new DriverSerialExampleCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as DriverSerialExampleTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new DriverSerialExampleCommJob(this, conf);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as DriverSerialExampleCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new DriverSerialExampleCommJobSettings(session, commJob);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new DriverSerialExampleTag(td);
        }
        #endregion

        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            DriverSerialExampleCommJob mJ = e.Job as DriverSerialExampleCommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                byte[] Answer = (byte[])e.Values;
                List<object> ChangedTags = new List<object>();
                if (DriverSerialExampleProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
                    foreach (var tag in ChangedTags)
                    {
                        var j = tag as Tag;
                        if (j != null)
                            e.ChangedTags.Add(j);
                    }
                else
                {
                    if (ChangedTags.Count > 0)
                        e.ErrorCode = (DriverErrorCodes)((DriverSerialExampleErrorCodes)ChangedTags[0]);
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
            DriverSerialExampleCommJob mj = job as DriverSerialExampleCommJob;

            DriverSerialExampleProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sort tags. </summary>
        ///
        /// <param name="tags" type="IList<Tag>">   The tags. </param>
        ///
        /// <returns>   The sorted tags. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override List<Tag> SortTags(IList<Tag> tags)
        {
            List<Tag> listTag = new List<Tag>();
            listTag.AddRange(tags);
            listTag.Sort(CompareTagByDynamic);
            return listTag;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Compare Tag By Dynamic. </summary>
        ///
        /// <param name="x" type="Tag">   The first tag to be compared. </param>
        /// <param name="y" type="Tag">   The second tag to be compared. </param>
        ///
        /// <returns>  int value  (Less than zero, zero or Greater than zero) . </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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
                    DriverSerialExampleDynTagSettings dts = new DriverSerialExampleDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string cx = string.Format("DS{0}TL{1}MI{2}FC{3}SA{4}File{5}",
                                               string.Empty,
                                               dts.TagLinkType.ToString("000"),
                                               dts.MethodID.ToString("000"),
                                               dts.FunctionCode,
                                               dts.StartAddress.ToString("0000"),
                                               dts.FileNumber.ToString("0000"));
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}FC{3}SA{4}File{5}",
                                               string.Empty, 
                                               dts.TagLinkType.ToString("000"),
                                               dts.MethodID.ToString("000"), 
                                               dts.FunctionCode, 
                                               dts.StartAddress.ToString("0000"), 
                                               dts.FileNumber.ToString("0000"));
                    return cx.CompareTo(cy);
                }
            }
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
