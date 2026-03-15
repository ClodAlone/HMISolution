using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;

namespace Databoom
{
    class DataboomStation : Station
    {
                
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public DataboomStation(CommunicationDriver commdriver, DataboomStationSettings settings)
            : base(commdriver, settings)
        {
        }
       
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as DataboomCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new DataboomCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as DataboomTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new DataboomCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new DataboomTag(td);
        }
        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as DataboomCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new DataboomCommJobSettings(session, commJob);
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

            // Set to uncertain the quality of the input jobs 
            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob.AddRange(ListWholeJob);
            }
            foreach (var job in listJob)
            {
                DataboomCommJob DataboomJob = (DataboomCommJob)job;
                if ((DataboomJob.Type == LinkType.Input) || (DataboomJob.Type == LinkType.InputOutput))
                {
                    DataboomJob.SetUncertainQuality();
                }
            }

            // Build the dictionary of the subscribed variables
            ThreadPool.QueueUserWorkItem(o =>
            {
                if (Channel != null)
                {
                    DataboomChannel DataboomChannel = (DataboomChannel)Channel;
                    lock (lockListObject)
                    {
                        foreach (var job in ListWholeJob)
                        {
                            DataboomCommJob DataboomJob = (DataboomCommJob)job;
                            if ((DataboomJob.Type == LinkType.Input) || (DataboomJob.Type == LinkType.InputOutput))
                            {
                                if (!String.IsNullOrWhiteSpace(DataboomJob.TagName))
                                {
                                    DataboomChannel.AddToMapSubscribedVariablesJobs(DataboomJob.TagName, DataboomJob);
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
            DataboomCommJob pJ = e.Job as DataboomCommJob;
            if (pJ == null)
                return;

            // Analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                //byte[] Answer = (byte[])e.Values;
                //List<object> ChangedTags = new List<object>();
                //if (DataboomProtocol.ParseData(Answer, ref pJ, ref ChangedTags))
                //DataValue Answer = (DataValue)e.Values;
                string Answer = (string)e.Values;
                List<object> ChangedTags = new List<object>();
                if (DataboomProtocol.ParseData(Answer, ref pJ, ref ChangedTags))
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
                        e.ErrorCode = (DriverErrorCodes)((DataboomErrorCodes)ChangedTags[0]);
                    }
                    else
                    {
                        e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                    }
                }
            }

            pJ.Status = DataboomCommJobStatus.Idle;

            // Put all the jobs in error?
            e.GeneralError = ((e.ErrorCode == DriverErrorCodes.ErrorTimeOut) ||
                              (e.ErrorCode == (DriverErrorCodes)DataboomErrorCodes.ErrorCodeErrorSubscription));

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
            //DataboomCommJob mj = job as DataboomCommJob;

            //DataboomProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }

        public override bool Terminate()
        {
            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob.AddRange(ListWholeJob);
            }

            if (ChannelBase != null)
            {
                foreach (var job in listJob)
                    ((DataboomChannel)ChannelBase).DataBoomUnsubscribeJob(job);
                
            }

            return true;
        }

        public override bool SuspendStation()
        {
            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob.AddRange(ListWholeJob);
            }

            if (ChannelBase != null)
            {
                foreach (var job in listJob)
                {
                    job.SetUncertainLastUsableValueQuality();
                    ((DataboomChannel)ChannelBase).DataBoomUnsubscribeJob(job);
                }
                ChannelBase.JobExecuted -= OnJobExecuted;
                ChannelBase.Suspend();
            }

            return true;
        }

        public override bool Suspend()
        {
            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob.AddRange(ListWholeJob);
            }

            if (ChannelBase != null)
            {
                foreach (var job in listJob)
                {
                    ((DataboomChannel)ChannelBase).DataBoomUnsubscribeJob(job); ;
                    lock (job.retLockList())
                        job.IsPending = false;
                }
                ChannelBase.JobExecuted -= OnJobExecuted;
                ChannelBase.Suspend();
            }

            return true;
        }

        #endregion

        #region Specific Methods
        public void ManageSubscriptionError()
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
                    DataboomCommJob pJob = (DataboomCommJob)job;
                    pJob.Status = DataboomCommJobStatus.Idle;
                }

                ExecutedJobArgs e = new ExecutedJobArgs();
                e.Job = listJob[0];
                e.ErrorCode = (DriverErrorCodes)(DataboomErrorCodes.ErrorCodeErrorSubscription);
                // Put all the jobs in error
                e.GeneralError = true;

                if (Channel != null)
                {
                    Channel.SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
                }

                base.ProcessJobValues(e);
            }
        }
        #endregion

        #region Properties

        #endregion

    }
}
