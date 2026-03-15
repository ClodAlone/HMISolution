using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;

namespace PubNub
{
    class PubNubStation : Station
    {
                
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public PubNubStation(CommunicationDriver commdriver, PubNubStationSettings settings)
            : base(commdriver, settings)
        {
        }
       
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as PubNubCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new PubNubCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as PubNubTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new PubNubCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new PubNubTag(td);
        }
        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as PubNubCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new PubNubCommJobSettings(session, commJob);
        }

        #endregion

        #region Override Methods

        public override bool Startup()
        {
            // Call the base class method
            if (!base.Startup())
            {
                return false;
            }

            // Build the dictionary of the subscribed variables
            ThreadPool.QueueUserWorkItem(o =>
            {
                if (Channel != null)
                {
                    PubNubChannel pubnubChannel = (PubNubChannel)Channel;
                    lock (lockListObject)
                    {
                        foreach (var job in ListWholeJob)
                        {
                            PubNubCommJob pubnubJob = (PubNubCommJob)job;
                            if ((pubnubJob.Type == LinkType.Input) || (pubnubJob.Type == LinkType.InputOutput))
                            {
                                if (!String.IsNullOrWhiteSpace(pubnubJob.TagName))
                                {
                                    pubnubChannel.AddToMapSubscribedVariablesJobs(pubnubJob.TagName, pubnubJob);
                                }
                            }
                        }
                    }
                }
            });

            return true;
        }

        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            PubNubCommJob pJ = e.Job as PubNubCommJob;
            if (pJ == null)
                return;

            // Analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                //byte[] Answer = (byte[])e.Values;
                //List<object> ChangedTags = new List<object>();
                //if (PubNubProtocol.ParseData(Answer, ref pJ, ref ChangedTags))
                //DataValue Answer = (DataValue)e.Values;
                string Answer = (string)e.Values;
                List<object> ChangedTags = new List<object>();
                if (PubNubProtocol.ParseData(Answer, ref pJ, ref ChangedTags))
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
                    if (ChangedTags.Count > 0)
                    {
                        e.ErrorCode = (DriverErrorCodes)((PubNubErrorCodes)ChangedTags[0]);
                    }
                    else
                    {
                        e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                    }
                }
            }

            pJ.Status = PubNubCommJobStatus.Idle;

            // Put all the jobs in error?
            e.GeneralError = ((e.ErrorCode == DriverErrorCodes.ErrorTimeOut) ||
                              (e.ErrorCode == (DriverErrorCodes)PubNubErrorCodes.ErrorCodeErrorSubscription));

            if (Channel != null)
            {
                if (e.ErrorCode != DriverErrorCodes.ErrorNoError)
                {
                    Channel.SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
                }
                else
                {
                    Channel.SetStateCommandVariableBit(false, (UInt16)ChannelVariableBits.ChannelUnconnected);
                }
            }

            base.ProcessJobValues(e);
        }

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            //PubNubCommJob mj = job as PubNubCommJob;

            //PubNubProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }

        #endregion

        #region Specific Methods
        public void ManageSubscriptionError(string errorMessage)
        {
            if (ListWholeJob.Count > 0)
            {
                var listJob = new List<CommJob>();
                lock (lockListObject)
                {
                    listJob.AddRange(ListWholeJob);
                }
                foreach (var job in listJob)
                {
                    // Deactivate the job
                    PubNubCommJob pJob = (PubNubCommJob)job;
                    pJob.Status = PubNubCommJobStatus.Idle;
                }

                ExecutedJobArgs e = new ExecutedJobArgs();
                e.Job = listJob[0];
                e.ErrorCode = (DriverErrorCodes)(PubNubErrorCodes.ErrorCodeErrorSubscription);
                // Put all the jobs in error
                e.GeneralError = true;

                PubNubDriver pubnubDriver = (PubNubDriver)CommDriver;
                pubnubDriver.lastSubscribeErrorMessage = errorMessage;
                if (Channel != null)
                {
                    Channel.SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
                }
                SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
                LastErrorCode = e.ErrorCode;
                InErrorState = true;

                base.ProcessJobValues(e);
            }
        }
        #endregion

        #region Properties

        #endregion

    }
}
