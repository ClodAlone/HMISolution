////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	GESRTP2Station.cs
//
// summary:	Implements the driver GESRTP2 station class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using DevExpress.Xpo;
using DriverBaseInterfaces;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;

namespace GESRTP2
{
    /// <summary>   Communication target device. </summary>
    public class GESRTP2Station : Station
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the station object. </summary>
        ///
        /// <param name="commdriver" type="CommunicationDriver">            The commdriver. </param>
        /// <param name="settings" type="GESRTP2StationSettings">  Options for controlling the
        ///                                                                 operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public GESRTP2Station(CommunicationDriver commdriver, GESRTP2StationSettings settings)
            : base(commdriver, settings)
        {
            _PlcType = settings.PlcType;
            _Rack = settings.Rack;
            _Slot = (Byte)settings.Slot;

            _Dir = string.Empty;
            _LookUpTable = GESRTP2Protocol.LOOKUP_TABLE_NAME;
        }

        #endregion

        #region Abstract Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new protocol task instance from CommJobSettings. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <param name="JobSettings">  . </param>
        ///
        /// <returns>   The new job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as GESRTP2CommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new GESRTP2CommJob(this, conf);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new protocol task instance from Tag. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <param name="defTag">   . </param>
        ///
        /// <returns>   The new job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as GESRTP2Tag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new GESRTP2CommJob(this, conf);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new variable instance from TagDefinition. </summary>
        ///
        /// <param name="td">   . </param>
        ///
        /// <returns>   The new tag. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new GESRTP2Tag(td);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new settings instance for protocol's task. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <param name="session">  . </param>
        /// <param name="job">      . </param>
        ///
        /// <returns>   The new job settings. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as GESRTP2CommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new GESRTP2CommJobSettings(session, commJob);
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compare tags by Dynamic Settings. Return 0 if x = y , 1 if x &gt; y and -1 if x &lt; y.
        /// </summary>
        ///
        /// <param name="x">    . </param>
        /// <param name="y">    . </param>
        ///
        /// <returns>   An int. </returns>
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
                    GESRTP2DynTagSettings dts = new GESRTP2DynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string cx = string.Format("DS{0}TL{1}MI{2}AT{3}SA{4}",
                        string.Empty, dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.AreaType,
                        dts.StartAddress.ToString("00000"),
                        dts.StringLength.ToString("00000")
                        );
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}AT{3}SA{4}",
                        string.Empty, dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.AreaType,
                        dts.StartAddress.ToString("00000"),
                        dts.StringLength.ToString("00000")
                        );
                    return cx.CompareTo(cy);
                    //return x.TagNode.DynamicSettings.CompareTo(y.TagNode.DynamicSettings);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sort the list of tags for dynamic settings. </summary>
        ///
        /// <param name="tags"> . </param>
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
        /// <summary>   Processed received data in ExecutedJobArgs. To call asyncronously... </summary>
        ///
        /// <param name="e">    . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            GESRTP2CommJob mJ = e.Job as GESRTP2CommJob;
            if (mJ == null)
                return;

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
                            e.ChangedTags.Add(j);
                    }
                }
                else
                {
                    if (e.Values != null)
                    {
                        byte[] Answer = (byte[])e.Values;
                        if (GESRTP2Protocol.ParseData(Answer, ref mJ, ref ChangedTags))
                            foreach (var tag in ChangedTags)
                            {
                                var j = tag as Tag;
                                if (j != null)
                                    e.ChangedTags.Add(j);
                            }
                        else
                            e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                    }
                }
            }

            //put all the job in error?
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut);

            base.ProcessJobValues(e);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Validation of the response data type input. If correct copy data in job. </summary>
        ///
        /// <param name="receivedbuffer">   . </param>
        /// <param name="job">              . </param>
        /// <param name="arguments">        [in,out]. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            GESRTP2CommJob mj = job as GESRTP2CommJob;

            GESRTP2Protocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Return maximum memory size for aggregation of GESRTP2CommJob objects.
        /// </summary>
        ///
        /// <returns>   The aggregate maximum job size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint GetAggregateMaxJobSize()
        {
            uint AggregLimit = GetCommDriver().AggregationLimit;
            uint JobMaxSize = GESRTP2Protocol.GetMaxJobSize(_PlcType);
            if ((AggregLimit != 0) && (AggregLimit < JobMaxSize))
            {
                return AggregLimit;
            }
            else
            {
                return JobMaxSize;
            }
        }

        public bool HasDir()
        {
            return !String.IsNullOrEmpty(_Dir);
        }

        public void ResetRunTimeSymbolicParameters()
        {
            List<CommJob> listJobs = new List<CommJob>();
            lock (lockListObject)
                listJobs = ListWholeJob.FindAll(j => GESRTP2Protocol.IsSymbolic(j));

            _Dir = string.Empty;
            _LookUpTable = GESRTP2Protocol.LOOKUP_TABLE_NAME;
            if (listJobs != null && listJobs.Count > 0)
            {
                foreach (GESRTP2CommJob job in listJobs)
                    job.ResetRunTimeSymbolicParameters();
            }
        }

        #region Properties

        private GESRTP2Protocol.PlcTypes _PlcType;
        public GESRTP2Protocol.PlcTypes PlcType
        {
            get { return _PlcType; }
            set { _PlcType = value; }
        }

        private Byte _Rack;
        public Byte Rack
        {
            get { return _Rack; }
            set { _Rack = value; }
        }

        private Byte _Slot;
        public Byte Slot
        {
            get { return _Slot; }
            set { _Slot = value; }
        }

        private String _Dir = string.Empty;
        public string Dir
        {
            get { return _Dir; }
            set { _Dir = value; }
        }

        private String _LookUpTable = string.Empty;
        public string LookUpTable
        {
            get { return _LookUpTable; }
            set { _LookUpTable = value; }
        }
        #endregion
    }
}
