using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DevExpress.Xpo;
using DriverBaseInterfaces;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;

namespace MTConnect
{
    public class MTConnectStation : Station
    {                
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public MTConnectStation(CommunicationDriver commdriver, MTConnectStationSettings settings)
            : base(commdriver, settings)
        {
            MaxRetriesBeforeError = 0;
            _DeviceId = settings.DeviceId;
            _MaxNumberOfItemsForRequest = settings.MaxNumberOfItemsForRequest;
        }
       
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as MTConnectCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new MTConnectCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as MTConnectTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new MTConnectCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new MTConnectTag(td);
        }
        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as MTConnectCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new MTConnectCommJobSettings(session, commJob);
        }


        #endregion

        #region Override Methods
        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            //e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut || e.ErrorCode == DriverErrorCodes.ErrorDeviceOpenFailed || e.ErrorCode == (DriverErrorCodes)MTConnectProtocol.MTConnectErrorCodes.ErrorCodeErrorPublish);
            MTConnectCommJob mJ = e.Job as MTConnectCommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                byte[] Answer = (byte[])e.Values;
                //S7Protocol P = new S7Protocol();
                List<object> ChangedTags = new List<object>();
                if (Answer != null)
                {
                    if (/*P*/MTConnectProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
                        foreach (var tag in ChangedTags)
                        {
                            var j = tag as Tag;
                            if (j != null)
                                e.ChangedTags.Add(j);
                        }
                    //e.ChangedTags.AddRange(ChangedTags);
                    else
                        e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                }
            }

            // manage connection error as a timeout error to slow-down communication and release CPU resources
            if (e.ErrorCode == (DriverErrorCodes)MTConnectProtocol.MTConnectErrorCodes.ErrorCodeNoInternetConnection)
                e.ErrorCode = DriverErrorCodes.ErrorTimeOut;

            //put all the job in error?
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut);

            base.ProcessJobValues(e);
        }

        // functionality not suported by this driver
        public override uint ExecuteSyncroJob(CommJob job, bool localmethod, int methodid, IList<object> args, bool takedata = true, NodeId tagNodeId = null, object value = null)
        {
            args[0] = (int)DriverErrorCodes.ErrorTimeOut;

            return StatusCodes.BadNotSupported;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> Sort the list of tags for dynamic settings. </summary>
        ///
        /// <param name="tags"> . </param>
        ///
        /// <returns> The sorted tags. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override List<Tag> SortTags(IList<Tag> tags)
        {
            return tags.ToList();
        }

        #endregion

        #region Method


        #endregion

        #region Properties

        /// <summary>   Server Adress. </summary>
        private string _DeviceId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Device Name. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string DeviceId
        {
            get { return _DeviceId; }
        }

        /// <summary>   Server Port. </summary>
        private uint _MaxNumberOfItemsForRequest;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the  Max Number Of Items For Request. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint MaxNumberOfItemsForRequest
        {
            get { return _MaxNumberOfItemsForRequest; }
        }

        #endregion
    }
}
