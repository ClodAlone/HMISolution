////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	DriverTcpExampleStation.cs
//
// summary:	Implements the driver TCP example station class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using DevExpress.Xpo;
using DriverCodeBase;
using DriverCodeBase.Enumerators;

namespace DriverTcpExample
{
    /// <summary>   Communication target device. </summary>
    class DriverTcpExampleStation : Station
    {
                
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the station object. </summary>
        ///
        /// <param name="commdriver" type="CommunicationDriver">            The commdriver. </param>
        /// <param name="settings" type="DriverTcpExampleStationSettings">  Options for controlling the
        ///                                                                 operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DriverTcpExampleStation(CommunicationDriver commdriver, DriverTcpExampleStationSettings settings)
            : base(commdriver, settings)
        {
            _StationID = settings.StationID;
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
            var conf = JobSettings as DriverTcpExampleCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new DriverTcpExampleCommJob(this, conf);
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
            var conf = defTag as DriverTcpExampleTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new DriverTcpExampleCommJob(this, conf);
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
            return new DriverTcpExampleTag(td);
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
            var commJob = job as DriverTcpExampleCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new DriverTcpExampleCommJobSettings(session, commJob);
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
                    DriverTcpExampleDynTagSettings dts = new DriverTcpExampleDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string cx = string.Format("DS{0}TL{1}MI{2}FC{3}SA{4}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.FunctionCode,
                        dts.StartAddress.ToString("00000"));
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}FC{3}SA{4}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.FunctionCode,
                        dts.StartAddress.ToString("00000"));
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
            DriverTcpExampleCommJob mJ = e.Job as DriverTcpExampleCommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                byte[] Answer = (byte[])e.Values;
                List<object> ChangedTags = new List<object>();
                if (DriverTcpExampleProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
                    foreach (var tag in ChangedTags)
                    {
                        var j = tag as Tag;
                        if (j != null)
                            e.ChangedTags.Add(j);
                    }
                else
                    e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
            }

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
            DriverTcpExampleCommJob mj = job as DriverTcpExampleCommJob;

            DriverTcpExampleProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }

        #region Properties

        /// <summary>   Identifier for the station. </summary>
        private uint _StationID;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Identifier for the station. </summary>
        ///
        /// <value> The identifier of the station. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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
