using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;

namespace Demo
{
    class DemoChannel : ChannelList
    {
        #region Constructors
        /// <summary>
        /// Initializes the DataboomChannel object.
        /// </summary>
        public DemoChannel(CommunicationDriver commdriver, DemoChannelSettings settings)
            : base(commdriver, settings)
        {                        
        }
        #endregion

        #region Abstracts Methods
        public override bool IsDeviceOpen()
        {
            return true;
        }

        public override bool DeviceOpen()
        {
            return true;
        }

        public override bool DeviceClose()
        {
            return true;
        }

        public override bool DeviceRead(byte[] Buffer, uint Count) { return true; }
        public override bool DeviceWrite(byte[] Buffer, uint Count) { return true; }
        public override uint GetBytesToRead() { return 1; }
        public override uint GetBytesToWrite() { return 1; }

        #endregion

        #region Data Members               
        #endregion

        #region Override Methods
        public override void SplitInExecutionLists(List<CommJob> jobList, ref List<List<CommJob>> exList)
        {
            int jobIndex = 0;
            while (jobIndex < jobList.Count)
            {
                List<CommJob> list = new List<CommJob>();
                bool first = true;
                uint simulationInterval = 0;
                // limit the nr max of jobs (tags) can be written in a single Http message 
                while (jobIndex < jobList.Count)
                {
                    DemoCommJob j = jobList.ElementAt(jobIndex) as DemoCommJob;

                    if (first)
                    {
                        first = false;
                        simulationInterval = j.SimulationInterval;
                    }

                    if (simulationInterval == j.SimulationInterval)
                    {
                        list.Add(j);
                        jobList.RemoveAt(jobIndex);
                        jobIndex--;                        
                    }
                    jobIndex++;
                }

                if (list.Count > 0)
                {
                    exList.Add(list);
                    jobIndex = 0;
                }
            }
        }

        public override bool ExecuteJobList(ref DriverErrorCodes conn, List<CommJob> list)
        {
            foreach (DemoCommJob j in list)
            {
                base.ExecuteJob(j);
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.ErrorCode = conn;
                eJob.Job = j;
                eJob.LastExecutionTime = j.GetNextScheduleTime();

                if (conn == DriverErrorCodes.ErrorNoError)
                {
                    j.UpdateValue();

                    eJob.Values = j.GetCurrentValues();
                }

                OnJobExecuted(eJob);
            }

            return false;
        }

        public override int ChannelScheduleProcedure(DateTime dtNow, out int nrRemainingJobsInQueues)
        {
            int nNextScheduleInterval = -1;
            int nNextDelayUse = -1;

            List<CommJob> startList = new List<CommJob>();
            lock (lockListObject)
            {
                startList.AddRange(channelJobList);                 
            }

            List<CommJob> wList = null;
            List<CommJob> rList = null;
            List<List<CommJob>> exList = null;
            lock (lockListObject)
            {
                wList = (from job in startList
                         where !job.IsPending && job.InUse && !job.IsQueued
                             && ((dtNow - job.LastExecutionTime).TotalMilliseconds >= job.SamplingInterval)
                         orderby job.LastExecutionTime ascending
                         select job).ToList();

                rList = (from job in startList
                         where !job.IsPending && !job.IsQueued && job.InUse
                             && (((dtNow - job.LastExecutionTime).TotalMilliseconds < job.SamplingInterval && job.SamplingInterval != CommJob.JOB_NOT_SCHEDULABLE) || job.ExecuteFirstTime == true)
                         orderby job.LastExecutionTime.AddMilliseconds(job.SamplingInterval) ascending
                         select job).ToList();


                if (wList.Count > 0)
                {
                    wList.ForEach(job =>
                    {
                        job.ScheduleQueue = CommJobState.PollingInUse;
                    });
                }

                if (rList.Count > 0)
                {
                    var dtNextSchedule = rList[0].LastExecutionTime.AddMilliseconds(rList[0].SamplingInterval);
                    if (dtNextSchedule > dtNow)
                        nNextDelayUse = (int)(dtNextSchedule - dtNow).TotalMilliseconds;
                    if (nNextDelayUse <= 0)
                        nNextDelayUse = 0;
                }
            }

            if (wList.Count > 0)
            {
                RescheduleLastQueuedJobsList(CommJobState.PollingInUse, wList);

                exList = new List<List<CommJob>>();
                SplitInExecutionLists(wList, ref exList);

                if (exList.Count > 0)
                    AddScheduledListJobQueue(CommJobState.PollingInUse, exList, out nInUseMax);

                if (wList.Count > 0)
                {
                    lock (lockListObject)
                    {
                        wList.ForEach(job =>
                        {
                            job.ScheduleQueue = CommJobState.UnScheduled;
                        });
                    }
                }
            }
            else
            {
                nInUseMax = GetScheduledListJobQueue(CommJobState.PollingInUse).Count();
            }

            //search lower delays
            if (nNextDelayUse >= 0 && (nNextScheduleInterval == -1 || nNextDelayUse < nNextScheduleInterval))
                nNextScheduleInterval = nNextDelayUse;

            nrRemainingJobsInQueues = NrRemainingJobsInQueues();

            nNextScheduleInterval = EvaluateMinScheduleInterval(nrRemainingJobsInQueues, nNextScheduleInterval);                    

            return nNextScheduleInterval;            
        }

        public override void SubscribeJob(CommJob job, CommJobState state)
        {
            base.SubscribeJob(job, state);
            ((DemoCommJob)job).StartStopSampling(((DemoCommJob)job).GetInUseState());
        }
        #endregion

        #region Local Methods
        #endregion

        #region Public Methods
        public void PublicOnJobExecuted(ExecutedJobArgs e)
        {
            OnJobExecuted(e);
        }

        public CommunicationDriver GetCommDriver()
        {
            return CommDriver;
        }          
                
        public void ReStartScheduler()
        {
            SetRequestSchedule();
        }
        #endregion
    }
}
