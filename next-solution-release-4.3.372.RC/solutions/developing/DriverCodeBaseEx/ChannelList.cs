////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	ChannelList.cs
//
// summary:	Implements the channelList class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx.Enumerators;
using System.Threading.Tasks;
using Opc.Ua;

namespace DriverCodeBaseEx
{
    /// <summary>   communication channel for communication drivers with dynamic aggregation. </summary>
    public abstract class ChannelList : Channel
    {
        #region Constructors
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the Channel object. </summary>
        ///
        /// <param name="commdriver">                       The communications driver. </param>
        /// <param name="settings" type="ChannelSettings">  Options for controlling the operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ChannelList(CommunicationDriver commdriver, ChannelSettings settings)
            : base(commdriver, settings)
        {

        }

        /// <summary>   Specialised default constructor for use only by derived class. </summary>

        #endregion

        #region Abstracts Methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a device is open. </summary>
        ///
        /// <returns>   true if a device is open, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool IsDeviceOpen()
        {
            return false;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a given device open. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceOpen()
        {
            return false;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Determines if we can device close. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceClose()
        {
            return false;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Synchronous read. </summary>
        ///
        /// <param name="Buffer">   buffer with read values. </param>
        /// <param name="Count">    number of bytes to read. </param>
        ///
        /// <returns>   if true the request byte size was read, othrewise false. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceRead(byte[] Buffer, uint Count)
        {
            return false;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Synchronous write. </summary>
        ///
        /// <param name="Buffer">   buffer with values to write. </param>
        /// <param name="Count">    number of bytes to write. </param>
        ///
        /// <returns>   if true the request write operation was performed, othrewise false. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceWrite(byte[] Buffer, uint Count)
        {
            return false;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets bytes to read. </summary>
        ///
        /// <returns>   The bytes to read. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetBytesToRead()
        {
            return 0;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets bytes to write. </summary>
        ///
        /// <returns>   The bytes to write. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetBytesToWrite()
        {
            return 0;
        }

        /// <summary>   Execute a list of job, return a list of executed job, if proper. </summary>        
        #endregion

        #region Data Members
        #endregion

        #region Private Fields/Methods
        /// <summary> Queue of list of dynamically aggregated job</summary>
        protected readonly Dictionary<CommJobState, List<List<CommJob>>> ScheduledListJobQueue = new Dictionary<CommJobState, List<List<CommJob>>>();

        protected List<List<CommJob>> GetScheduledListJobQueue(CommJobState type)
        {
            lock (lockListObject)
            {
                if (!ScheduledListJobQueue.Keys.Contains(type))
                    ScheduledListJobQueue[type] = new List<List<CommJob>>();
                                
                return ScheduledListJobQueue[type];
            }
        }

        protected List<CommJob> GetAndRemove1StScheduledListJobQueue(CommJobState type)
        {
            List<CommJob> scheduledListJobQueue = new List<CommJob>();
            lock (lockListObject)
            {
                if (!ScheduledListJobQueue.Keys.Contains(type))
                    ScheduledListJobQueue[type] = new List<List<CommJob>>();

                if (ScheduledListJobQueue[type].Count > 0)
                {
                    scheduledListJobQueue.AddRange(ScheduledListJobQueue[type][0]);
                    ScheduledListJobQueue[type].RemoveAt(0);
                }

                foreach (var job in scheduledListJobQueue)
                {
                    job.ScheduleQueue = CommJobState.UnScheduled;
                    job.IsPending = true;
                }

                return scheduledListJobQueue;
            }
        }

        protected void AddScheduledListJobQueue(CommJobState type, List<List<CommJob>> list, out int scheduled)
        {
            lock (lockListObject)
            {
                foreach (var subList in list)
                {
                    foreach (var job in subList)
                        job.ScheduleQueue = type;
                }

                if (!ScheduledListJobQueue.Keys.Contains(type))
                    ScheduledListJobQueue[type] = new List<List<CommJob>>();
                ScheduledListJobQueue[type].AddRange(list);

                scheduled = ScheduledListJobQueue[type].Count;
            }
        }
        #endregion

        #region Properties

        #endregion

        #region runtime properties

        #endregion

        #region Communication Procedures
        public override void commExecuteSyncro(CommJob exjob, NodeId tagNodeId = null, object value = null, int timeOut = 0)
        {
            if (AcquireCommunicationThreadControl(timeOut))
            {
                // Nuovo
                CommDriver.SuspendScheduler();
                RemoveJobFromSchedulingQueue(exjob, (CommJob.ScheduleProperties.SetSyncroExec | CommJob.ScheduleProperties.SetIsPending));                
                CommDriver.RestartScheduler();
                if (SetSynchroJobData(exjob, tagNodeId, value))
                {
                    WaitWaitTimeFromLastCommExecution();

                    exjob.UpdateTagsListOnWriting();
                    commExecution(new List<CommJob>() { exjob }, this);

                    // reevaluate the status of the scheduling queue and restart if necessary
                    if (NeedNewSchedule(out int nrRemainingJobsInQueues))
                    {
                        //PerformanceLogToFile(this.Name, string.Format("communicationProcedure SetRequestSchedule : {0}", nrRemainingJobsInQueues));
                        SetRequestSchedule();
                    }
                }                
                else
                {
                    exjob.SetScheduleProperties((CommJob.ScheduleProperties.UnSetSyncroExec | CommJob.ScheduleProperties.UnSetIsPending));
                }
                ReleaseCommunicationThreadControl();                
            }
        }

        public virtual DriverErrorCodes CheckDevice(List<CommJob> exjoblist, object thischannel)
        {
            //return conn;
            DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;
            if (!IsDeviceOpen())
            {
                if (!DeviceOpen())
                    conn = DriverErrorCodes.ErrorDeviceOpenFailed;
            }

            return conn;
        }

        public virtual bool ExecuteJobList(ref DriverErrorCodes conn, List<CommJob> exjoblist)
        {
            foreach (var job in exjoblist)
                ExecuteJob(job);

            return true;
        }


        public void commExecution(List<CommJob> exjoblist, object thischannel)
        {
            internalCommExecution(exjoblist, thischannel);

            if (!bDisposed && !CommDriver.bDisposed)
            {
                // some jobs are continuously doing writes but should they also read ?
                List<CommJob> jobsToRead = exjoblist.FindAll(job => job.InputOutputRequiresReadAfterContinuosWrite());
                if (jobsToRead != null && jobsToRead.Count() > 0)
                {
                    WaitWaitTimeFromLastCommExecution();

                    CommDriver.SuspendScheduler();
                    foreach (CommJob job in jobsToRead)
                    {
                        //if it has already been scheduled in the meantime, remove it from the queue
                        RemoveJobFromSchedulingQueue(job, CommJob.ScheduleProperties.SetIsPending);
                        // move the tags to be written so that the driver can be read
                        job.UpdateTagsListOnWritingAndTagsListToWrite(job.GetTagListOnWriting());
                    }
                    CommDriver.RestartScheduler();

                    List<List<CommJob>> exList = new List<List<CommJob>>();
                    SplitInExecutionLists(jobsToRead, ref exList);

                    foreach (var jobList in exList)
                        internalCommExecution(jobList, this);
                }                
            }
        }

        public void internalCommExecution(List<CommJob> exjoblist, object thischannel)
        {
            // struct atomic job contain a separate jobs list to manage read/write
            GetCustomJobChildJobs(exjoblist, out List<List<CommJob>> allchildJobs, out bool write);
            foreach (var childJobs in allchildJobs)
            {
                while (!bDisposed && !CommDriver.bDisposed && childJobs.Count(c => c.IsPending) > 0)
                {
                    DriverErrorCodes conn = CheckDevice(childJobs, thischannel);

                    ResetNewDataEvent();

                    if (ExecuteJobList(ref conn, childJobs))
                        WaitNewDataEvent(ref conn);

                    if (!bDisposed && !CommDriver.bDisposed && childJobs.Count(c => c.IsPending) > 0)
                        ProcessNewDataList(conn, childJobs);

                    if (ResetConnection())
                        DeviceClose();
                }
            }
            if (!bDisposed && !CommDriver.bDisposed)
                // merge result of separate jobs list into "main" job
                MergeCustomJobChildJobs(exjoblist, allchildJobs, write);

            SetLastCommExecutionDateTime();
        }

        public virtual List<CommJob> GetNextPendingList()
        {            
            var queue = GetNextPendingQueueList();
            if (queue != null && queue.Count > 0)            
                return queue;
            else
                return null;
        }

        protected override bool ExecuteTask(object state)
        {
            List<CommJob> nextlist = GetNextPendingList();

            if (nextlist != null && nextlist.Count > 0)
            {
                ChannelList me = (ChannelList)state;
                commExecution(nextlist, me);

                //Channel.PerformanceLogToFile("ExecuteTask ,channel=" + this.Name + ", Execute Comm");

                return true;
            }
            else
            {
                //Channel.PerformanceLogToFile("ExecuteTask ,channel=" + this.Name + ", Nothing to Execute Comm");

                return false;
            }
        }

        protected override bool NeedNewSchedule()
        {
            return NeedNewSchedule(out int dummy);
        }

        protected override bool NeedNewSchedule(out int nrRemainingJobsInQueues)
        {
            lock (lockListObject)
            {
                nrRemainingJobsInQueues = NrRemainingJobsInQueues();

                if (ScheduledListJobQueue.Count > 0)
                {
                    return (ScheduledListJobQueue[CommJobState.PollingInUse].Count <= (nInUseMax * Properties.Settings.Default.ScheduleRemainingPercent) &&
                            ScheduledListJobQueue[CommJobState.PollingInError].Count <= (nInErrorMax * Properties.Settings.Default.ScheduleRemainingPercent) &&
                            ScheduledListJobQueue[CommJobState.PollingNotInUse].Count <= (nNotInUseMax * Properties.Settings.Default.ScheduleRemainingPercent) &&
                            ScheduledListJobQueue[CommJobState.PollingNow].Count <= (nNowMax * Properties.Settings.Default.ScheduleRemainingPercent));
                }
                else
                {
                    return false;   
                }
            }
        }

        public override int NrRemainingJobsInQueues()
        {
            lock (lockListObject)
            {
                if (ScheduledListJobQueue.Count > 0)
                    return (ScheduledListJobQueue[CommJobState.PollingInUse].Count + ScheduledListJobQueue[CommJobState.PollingInError].Count + ScheduledListJobQueue[CommJobState.PollingNotInUse].Count + ScheduledListJobQueue[CommJobState.PollingNow].Count);
                else
                    return 0;
            }
        }
        #endregion

        #region Virtual methods

        public override void InitData()
        {
            lock (lockListObject)
            {
                if (!ScheduledListJobQueue.Keys.Contains(CommJobState.PollingInError))
                    ScheduledListJobQueue[CommJobState.PollingInError] = new List<List<CommJob>>();
                if (!ScheduledListJobQueue.Keys.Contains(CommJobState.PollingInUse))
                    ScheduledListJobQueue[CommJobState.PollingInUse] = new List<List<CommJob>>();
                if (!ScheduledListJobQueue.Keys.Contains(CommJobState.PollingNotInUse))
                    ScheduledListJobQueue[CommJobState.PollingNotInUse] = new List<List<CommJob>>();
                if (!ScheduledListJobQueue.Keys.Contains(CommJobState.PollingNow))
                    ScheduledListJobQueue[CommJobState.PollingNow] = new List<List<CommJob>>();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Tests channel communications. </summary>
        ///
        /// <returns>   true if the test passes, false if the test fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool TestChannelComm()
        {
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Call with point-to-point protocol. </summary>
        ///
        /// <exception cref="NotImplementedException">  Thrown when the requested operation is
        ///                                             unimplemented. </exception>
        ///
        /// <param name="conn">                         communication error code. </param>
        /// <param name="list" type="List<CommJob>">    The list of pending jobs. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool ProcessNewDataList(DriverErrorCodes conn, List<CommJob> list)
        {
            return true;
        }

        public virtual void SplitInExecutionLists(List<CommJob> jobList, ref List<List<CommJob>> exList)
        {
        }

        public virtual bool IsScheduledJobsListFull(List<CommJob> jobList)
        {
            return true;
        }

        protected void RescheduleLastQueuedJobsList(CommJobState queue, List<CommJob> wList)
        {            
            lock (lockListObject)
            {
                List<List<CommJob>> exList = ScheduledListJobQueue[queue];

                if (exList.Count > 0)
                {
                    if (!IsScheduledJobsListFull(exList[exList.Count - 1]))
                    {
                        wList.InsertRange(0, exList[exList.Count - 1]);
                        exList.RemoveAt(exList.Count - 1);
                    }
                }
            }
        }
        protected override void RemoveJobFromSchedulingQueue(CommJob job, CommJob.ScheduleProperties schedulerProperties = CommJob.ScheduleProperties.None)
        {            
            lock (lockListObject)
            {
                try
                {
                    if (ScheduledListJobQueue.Count == 0)
                        return;                    

                    if (!job.IsQueued)
                        return;
                 
                    CommJobState jobState = job.ScheduleQueue;
                    if (ScheduledListJobQueue[jobState].Count > 0)
                    {
                        for (int jobListIndex = 0; jobListIndex < ScheduledListJobQueue[jobState].Count; jobListIndex++)
                        {
                            if (ScheduledListJobQueue[jobState][jobListIndex].Contains(job))
                            {
                                ScheduledListJobQueue[jobState][jobListIndex].Remove(job);
                                job.ScheduleQueue = CommJobState.UnScheduled;
                                // if list of jobs in queue is empty remove it
                                if (ScheduledListJobQueue[jobState][jobListIndex].Count == 0)
                                    ScheduledListJobQueue[jobState].RemoveAt(jobListIndex);
                                break;
                            }
                        }
                    }
                }
                finally
                {
                    if (schedulerProperties != CommJob.ScheduleProperties.None)
                        job.SetScheduleProperties(schedulerProperties);
                }
            }
        }
        #endregion

        #region Communication related Methods                
        protected override void RemoveJobOfStationOnSpecificQueue(Station station, CommJobState type)
        {
            lock (lockListObject)
            {
                var inputQueue = GetScheduledListJobQueue(type);

                var lToRemove = (from joblist in inputQueue.AsParallel() where (from job in joblist where job.Station == station select job).ToList().Count > 0 select joblist).ToList();
                foreach (var jr in lToRemove)
                {
                    var l = jr.Select(c => { c.ScheduleQueue = CommJobState.UnScheduled; return c; }).ToList();
                    inputQueue.Remove(jr);
                }
            }
        }

        protected List<CommJob> GetNextPendingQueueList()
        {
            lock (lockListObject)
            {
                if (GetScheduledListJobQueue(CommJobState.PollingNow).Count > 0 && PollingInUseJobsCounter == 0 && PollingNotInUseJobsCounter == 0)
                {
                    if (PollingNowJobs ||
                        (GetScheduledListJobQueue(CommJobState.PollingInUse).Count == 0 &&
                        GetScheduledListJobQueue(CommJobState.PollingNotInUse).Count == 0 &&
                        GetScheduledListJobQueue(CommJobState.PollingInError).Count == 0))
                    {
                        return GetAndRemove1StScheduledListJobQueue(CommJobState.PollingNow);
                    }
                }

                if (GetScheduledListJobQueue(CommJobState.PollingInUse).Count > 0 && PollingNotInUseJobsCounter == 0)
                {
                    if (PollingInUseJobs ||
                        (GetScheduledListJobQueue(CommJobState.PollingNotInUse).Count == 0 &&
                        GetScheduledListJobQueue(CommJobState.PollingInError).Count == 0)
                        && (GetScheduledListJobQueue(CommJobState.PollingNow).Count == 0))
                    {
                        return GetAndRemove1StScheduledListJobQueue(CommJobState.PollingInUse);
                    }
                }

                if (GetScheduledListJobQueue(CommJobState.PollingNotInUse).Count > 0)
                {
                    if (PollingNotInUseJobs ||
                        GetScheduledListJobQueue(CommJobState.PollingInError).Count == 0)
                    {
                        return GetAndRemove1StScheduledListJobQueue(CommJobState.PollingNotInUse);
                    }
                }

                PollingNowJobsCounter = 0;
                PollingInUseJobsCounter = 0;
                PollingNotInUseJobsCounter = 0;

                if (GetScheduledListJobQueue(CommJobState.PollingInError).Count > 0)
                {
                    return GetAndRemove1StScheduledListJobQueue(CommJobState.PollingInError);
                }
                //nothing to do, as the regular sequence
                if (GetScheduledListJobQueue(CommJobState.PollingNow).Count > 0)
                {
                    PollingNowJobsCounter++;
                    return GetAndRemove1StScheduledListJobQueue(CommJobState.PollingNow);
                }
                if (GetScheduledListJobQueue(CommJobState.PollingInUse).Count > 0)
                {
                    PollingInUseJobsCounter++;
                    return GetAndRemove1StScheduledListJobQueue(CommJobState.PollingInUse);
                }
                if (GetScheduledListJobQueue(CommJobState.PollingNotInUse).Count > 0)
                {
                    PollingNotInUseJobsCounter++;
                    return GetAndRemove1StScheduledListJobQueue(CommJobState.PollingNotInUse);
                }
                if (GetScheduledListJobQueue(CommJobState.PollingInError).Count > 0)
                {
                    return GetAndRemove1StScheduledListJobQueue(CommJobState.PollingInError);
                }
            }
            return null;
        }
        #endregion

        #region IChannelBase Interface             
        public override int ChannelScheduleProcedure(DateTime dtNow, out int nrRemainingJobsInQueues)
        {
            //Channel.PerformanceLogToFile(this.Name, string.Format("ChannelScheduleProcedure Start"));

            int nNextScheduleInterval = -1;

            int nNextDelayUse = -1;
            int nNextDelayError = -1;
            int nNextDelayNotUse = -1;

            List<CommJob> startList = new List<CommJob>();
            List<CommJob> wListW = new List<CommJob>();
            List<CommJob> wListR = new List<CommJob>();
            List<CommJob> wListE = new List<CommJob>();
            List<CommJob> wListI = new List<CommJob>();
            List<CommJob> rListR = new List<CommJob>();
            List<CommJob> rListE = new List<CommJob>();
            List<CommJob> rListI = new List<CommJob>();
            Dictionary<Station, List<CommJob>> stationsInError = null;
            List<List<CommJob>> exList = null;

            int nrRemainingJobsInQueuesInitial = NrRemainingJobsInQueues();
            //Stopwatch swStart = Stopwatch.StartNew();
            //Stopwatch swLockScheduler = Stopwatch.StartNew();

            lock (lockListObject)
            { 
                startList.AddRange(channelJobList);

                //write
                wListW = (from job in startList
                          where !job.IsPending && job.InUse && !job.SyncroExec
                              && job.IsConditionalVariableOn()
                              && (job.Type == LinkType.UnconditionalOutput || job.GetTagListToWriteCount() > 0)
                          orderby job.LastExecutionTime ascending
                          select job).ToList();
                if (wListW.Count > 0)
                {
                    //Channel.PerformanceLogToFile(this.Name, string.Format("ChannelScheduleProcedure write"));
                    bool anyQueued = (wListW.Count(j => j.IsQueued) > 0);
                    if (anyQueued)
                    {
                        lock (lockListObject)
                        {
                            wListW.RemoveAll(j => j.ScheduleQueue == CommJobState.PollingNow);
                            foreach (var job in wListW)
                                RemoveJobFromSchedulingQueue(job);
                        }
                    }

                    if (wListW.Count > 0)
                    {
                        wListW.ForEach(job =>
                        {
                            job.ScheduleQueue = CommJobState.PollingNow;
                            job.UpdateTagsListOnWriting();
                        });
                    }
                }

                //read
                wListR = (from job in startList
                          where !job.IsPending && !job.IsQueued && (job.InUse || job.ExecuteFirstTime == true) && !job.InErrorState
                                  && job.IsConditionalVariableOn()
                                  && ((job.Type == LinkType.Input || job.Type == LinkType.InputOutput) && job.GetTagListToWriteCount() == 0)
                                  && (dtNow - job.LastExecutionTime).TotalMilliseconds >= job.SamplingInterval
                          orderby job.LastExecutionTime.AddMilliseconds(job.SamplingInterval) ascending
                          select job).ToList();
                if (wListR.Count > 0)
                {
                    wListR.ForEach(job =>
                    {
                        job.ScheduleQueue = CommJobState.PollingInUse;
                    });
                }

                rListR = (from job in startList
                          where !job.IsPending && !job.IsQueued && job.InUse && !job.InErrorState
                              && job.IsConditionalVariableOn()
                              && ((job.Type == LinkType.Input || job.Type == LinkType.InputOutput) && job.GetTagListToWriteCount() == 0)
                              && ((dtNow - job.LastExecutionTime).TotalMilliseconds < job.SamplingInterval && job.SamplingInterval != CommJob.JOB_NOT_SCHEDULABLE)
                          orderby job.LastExecutionTime.AddMilliseconds(job.SamplingInterval) ascending
                          select job).ToList();

                // error
                stationsInError = (from job in startList
                                   where !job.IsPending && !job.IsQueued && job.InUse && job.InErrorState
                                       && job.IsConditionalVariableOn()
                                       && (job.Type != LinkType.ExceptionOutput || job.GetTagListToWriteCount() > 0)
                                   //&& (dtNow - job.LastErrorTime).TotalMilliseconds >= job.ErrorPollingTime                                   
                                   group job by job.Station into jobsInError
                                   select new { Station = jobsInError.Key, Values = jobsInError.ToList() }).ToDictionary(t => t.Station, t => t.Values);
                if (stationsInError != null)
                {
                    foreach (var stInErr in stationsInError)
                    {
                        Station st = stInErr.Key;
                        List<CommJob> jobs = stInErr.Value;
                        // get jobs to schedule now
                        List<CommJob> jobsToScheduleNow = jobs.FindAll(job => (dtNow - job.LastErrorTime).TotalMilliseconds >= job.ErrorPollingTime).OrderBy(job => job.LastErrorTime.AddMilliseconds(job.ErrorPollingTime)).ToList();
                        if (jobsToScheduleNow == null)
                            jobsToScheduleNow = new List<CommJob>();
                        // station in error and no jobs to execute now ?
                        if (st.InGeneralErrorState && jobsToScheduleNow.Count == 0)
                        {
                            // if first of list (all jobs is error) exceed PollingTimeInError force execution
                            if ((dtNow - st.StationLastErrorTime).TotalMilliseconds >= PollingTimeInError)
                                wListE.Add(jobs[0]);
                        }
                        else
                        {
                            wListE.AddRange(jobsToScheduleNow);
                        }
                    }
                    if (wListE.Count > 0)
                    {
                        wListE.ForEach(job =>
                        {
                            job.ScheduleQueue = CommJobState.PollingInError;
                            job.UpdateTagsListOnWriting();
                        });
                    }
                }

                rListE = (from job in startList
                          where job.InUse && !job.IsPending && !job.IsQueued && job.InErrorState
                                  && job.IsConditionalVariableOn()
                                  && (job.Type != LinkType.ExceptionOutput || job.GetTagListOnWritingCount() > 0)
                                  && (dtNow - job.LastErrorTime).TotalMilliseconds <= job.ErrorPollingTime
                          orderby job.LastErrorTime ascending
                          select job).ToList();

                // not is use
                if (PollingTimeNotInUse > 0)
                {
                    wListI = (from job in startList
                              where !job.IsPending && !job.IsQueued && !job.InUse && !job.InErrorState
                                  && job.IsConditionalVariableOn()
                                  && ((job.Type == LinkType.Input || job.Type == LinkType.InputOutput) && job.GetTagListToWriteCount() == 0)
                                  && ((dtNow - job.LastExecutionTime).TotalMilliseconds >= PollingTimeNotInUse)
                              orderby job.LastExecutionTime.AddMilliseconds(PollingTimeNotInUse) ascending
                              select job).ToList();
                    if (wListI.Count > 0)
                    {
                        wListI.ForEach(job =>
                        {
                            job.ScheduleQueue = CommJobState.PollingNotInUse;
                            job.UpdateTagsListOnWriting();
                        });
                    }

                    rListI = (from job in startList
                              where !job.IsPending && !job.IsQueued && !job.InUse && !job.InErrorState
                                  && job.IsConditionalVariableOn()
                                  && (job.Type != LinkType.ExceptionOutput || job.GetTagListOnWritingCount() > 0)
                                  && (dtNow - job.LastExecutionTime).TotalMilliseconds < PollingTimeNotInUse
                              orderby job.LastExecutionTime.AddMilliseconds(PollingTimeNotInUse) ascending
                              select job).ToList();
                }
            }
            //swLockScheduler.Stop();

            #region Write           
            if (wListW.Count > 0)
            {
                RescheduleLastQueuedJobsList(CommJobState.PollingNow, wListW);
                exList = new List<List<CommJob>>();
                SplitInExecutionLists(wListW, ref exList);
                if (exList.Count > 0)
                    AddScheduledListJobQueue(CommJobState.PollingNow, exList, out nNowMax);

                if (wListW.Count > 0)
                {
                    wListW.ForEach(job =>
                    {
                        job.ScheduleQueue = CommJobState.UnScheduled;
                        job.UpdateTagsListToWrite();
                    });
                }
            }
            else
            {
                nNowMax = GetScheduledListJobQueue(CommJobState.PollingNow).Count();
            }
            #endregion

            #region Read
            if (wListR.Count > 0)
            {
                RescheduleLastQueuedJobsList(CommJobState.PollingInUse, wListR);
                exList = new List<List<CommJob>>();
                SplitInExecutionLists(wListR, ref exList);
                if (exList.Count > 0)
                    AddScheduledListJobQueue(CommJobState.PollingInUse, exList, out nInUseMax);

                if (wListR.Count > 0)
                {
                    wListW.ForEach(job =>
                    {
                        job.ScheduleQueue = CommJobState.UnScheduled;
                    });
                }
            }
            else
            {
                nInUseMax = GetScheduledListJobQueue(CommJobState.PollingInUse).Count();
            }

            if (rListR.Count > 0)
            {
                var dtNextSchedule = rListR[0].LastExecutionTime.AddMilliseconds(rListR[0].SamplingInterval);
                if (dtNextSchedule > dtNow)
                    nNextDelayUse = (int)(dtNextSchedule - dtNow).TotalMilliseconds;
                if (nNextDelayUse <= 0)
                    nNextDelayUse = 0;
            }
            #endregion

            #region Error           
            if (stationsInError != null)
            {
                if (wListE.Count > 0)
                {
                    RescheduleLastQueuedJobsList(CommJobState.PollingInError, wListE);
                    exList = new List<List<CommJob>>();
                    SplitInExecutionLists(wListE, ref exList);
                    if (exList.Count > 0)
                        AddScheduledListJobQueue(CommJobState.PollingInError, exList, out nInErrorMax);
                    if (wListE.Count > 0)
                    {
                        wListE.ForEach(job =>
                        {
                            job.ScheduleQueue = CommJobState.UnScheduled;
                            job.UpdateTagsListToWrite();
                        });
                    }
                }
                else
                {
                    nInErrorMax = GetScheduledListJobQueue(CommJobState.PollingInError).Count();
                }
            }
            else
            {
                nInErrorMax = GetScheduledListJobQueue(CommJobState.PollingInError).Count();
            }

            if (rListE.Count > 0)
            {
                var dtNextSchedule = rListE[0].LastErrorTime.AddMilliseconds(rListE[0].ErrorPollingTime);
                if (dtNextSchedule > dtNow)
                {
                    nNextDelayError = (int)(dtNextSchedule - dtNow).TotalMilliseconds;
                    // if channel (all stations) is in general error and something were scheduled, wait to reschedule that all jobs in error are processed
                    if (InGeneralErrorState && ScheduledListJobQueue[CommJobState.PollingInError].Count > 0)
                        nNextDelayError = -1;
                    // if channel or station is in general error, force to restart not far than PollingTimeInError
                    else if (nNextDelayError > PollingTimeInError && (rListE[0].Station.InGeneralErrorState || InGeneralErrorState))
                        nNextDelayError = PollingTimeInError;
                }
            }
            #endregion

            #region Not in Use
            if (PollingTimeNotInUse > 0)
            {
                if (rListI.Count > 0)
                {
                    var dtNextSchedule = rListI[0].LastExecutionTime.AddMilliseconds(PollingTimeNotInUse);
                    if (dtNextSchedule > dtNow)
                        nNextDelayNotUse = (int)(dtNextSchedule - dtNow).TotalMilliseconds;
                    if (nNextDelayNotUse <= 0)
                        nNextDelayNotUse = 0;
                }
            }

            if (wListI.Count > 0)
            {
                RescheduleLastQueuedJobsList(CommJobState.PollingNotInUse, wListI);

                exList = new List<List<CommJob>>();
                SplitInExecutionLists(wListI, ref exList);
                if (exList.Count > 0)
                    AddScheduledListJobQueue(CommJobState.PollingNotInUse, exList, out nNotInUseMax);

                if (wListI.Count > 0)
                {
                    wListI.ForEach(job =>
                    {
                        job.ScheduleQueue = CommJobState.UnScheduled;
                        job.UpdateTagsListToWrite();
                    });
                }
            }
            else
            {
                nNotInUseMax = GetScheduledListJobQueue(CommJobState.PollingNotInUse).Count();
            }
            #endregion

            //search lower delays
            if (nNextDelayUse >= 0 && (nNextScheduleInterval == -1 || nNextDelayUse < nNextScheduleInterval))
            {
                nNextScheduleInterval = nNextDelayUse;
            }
            if (nNextDelayError >= 0 && (nNextScheduleInterval == -1 || nNextDelayError < nNextScheduleInterval))
            {
                nNextScheduleInterval = nNextDelayError;
            }
            if (nNextDelayNotUse >= 0 && (nNextScheduleInterval == -1 || nNextDelayNotUse < nNextScheduleInterval))
            {
                nNextScheduleInterval = nNextDelayNotUse;
            }

            nrRemainingJobsInQueues = NrRemainingJobsInQueues();

            nNextScheduleInterval = EvaluateMinScheduleInterval(nNextScheduleInterval, nrRemainingJobsInQueues);

            //Channel.PerformanceLogToFile(this.Name, string.Format("ChannelScheduleProcedure Initial Jobs:{0}, Total Jobs:{1}, TotalTime lockScheduler:{2}, TotalTime:{3}, NextScheduleTime:{4}", nrRemainingJobsInQueuesInitial, nrRemainingJobsInQueues, swLockScheduler.ElapsedMilliseconds, swStart.ElapsedMilliseconds, nNextScheduleInterval));

            return nNextScheduleInterval;
        }

        protected override void RemoveScheduledJobs(Station s, List<CommJob> jobs)
        {
            lock (lockListObject)
            {
                foreach (var state in ScheduledListJobQueue.Keys)
                {
                    foreach (var lj in ScheduledListJobQueue[state])
                        lj.RemoveAll(j => j.Station == s && jobs.Contains(j));
                }
                Parallel.ForEach(jobs, job =>
                {
                    job.ScheduleQueue = CommJobState.UnScheduled;
                });
            }
        }

        /// <summary>
        /// Return the list of jobs (instead of use exJob job) to manage read/write operation
        /// </summary>
        /// <param name="exjobList"></param>
        /// <param name="allChildJobs"></param>
        /// <param name="write"></param>
        protected virtual void GetCustomJobChildJobs(List<CommJob> exjobList, out List<List<CommJob>> allChildJobs, out bool write)
        {            
            List<CommJob> resultJobs = new List<CommJob>();
            bool resultWrite = false;
            
            allChildJobs = new List<List<CommJob>>();
            bool someCustomJobs = false;

            foreach (CommJob exjob in exjobList)
            {
                if (!exjob.IsCustomJob())
                {
                    resultJobs.Add(exjob);
                }
                else
                {
                    someCustomJobs = true;
                    // check if is a read or write operation
                    resultWrite = (exjob.GetTagListOnWritingCount() > 0 || exjob.Type == LinkType.UnconditionalOutput);
                    if (resultWrite)
                    {
                        if (exjob.Type == LinkType.UnconditionalOutput)
                            exjob.FillWholeTagsListOnWriting();

                        foreach (var tag in exjob.TagsListOnWriting)
                        {
                            // copy each tag to write from exJob (main) to sub jobs (StructAtomicJobsListWrite)
                            if (exjob.ChildJobsWrite.ContainsKey(tag.TagNode.NodeId))
                            {
                                CommJob childJob = exjob.ChildJobsWrite[tag.TagNode.NodeId];
                                childJob.ClearTagListWrite();                                
                                childJob.TagsList[0].SetWriteVal(tag.Value.Value);
                                childJob.AddTagListOnWriting(childJob.TagsList[0]);
                                if (childJob.IsCustomJob())
                                {
                                    GetCustomJobChildJobs(new List<CommJob>() { childJob }, out List<List<CommJob>> customChildJobs, out bool childWrite);
                                    if (childWrite)
                                    {
                                        foreach (var jobs in customChildJobs)
                                            resultJobs.AddRange(jobs);
                                    }
                                }
                                else
                                {
                                    resultJobs.Add(childJob);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var job in exjob.ChildJobsRead.Values)
                        {
                            if (job.IsCustomJob())
                            {
                                GetCustomJobChildJobs(new List<CommJob>() { job }, out List<List<CommJob>> customChildJobs, out bool childRead);
                                if (!childRead)
                                {
                                    foreach (var jobs in customChildJobs)
                                        resultJobs.AddRange(jobs);
                                }
                            }
                            else
                            {
                                resultJobs.Add(job);
                            }
                        }
                    }
                }
            }

            if (someCustomJobs)
            {
                // reset each jobs before to be executed
                foreach (CommJob j in resultJobs)
                {
                    if (j.IsChildJob)
                        j.ResetInitialValueChildJob();
                }

                SplitInExecutionLists(resultJobs, ref allChildJobs);
            }
            else
            {
                allChildJobs.Add(resultJobs);
            }

            write = resultWrite;
        }

        /// <summary>
        /// Merge the result of "Struct Atomic child jobs" into exJob job and then publish result
        /// </summary>
        /// <param name="exjobList"></param>
        /// <param name="allChildJobs"></param>
        /// <param name="write"></param>
        protected virtual void MergeCustomJobChildJobs(List<CommJob> exjobList, List<List<CommJob>> allChildJobs, bool write)
        {
            foreach (CommJob exjob in exjobList)
            {
                if (!exjob.IsCustomJob())
                {
                    // do nothing here ! a standard job (not childJob) was processed before
                }
                else
                { 
                    ExecutedJobArgs e = new ExecutedJobArgs();
                    e.Job = exjob;
                    e.ErrorCode = DriverErrorCodes.ErrorNoError;

                    List<CommJob> jobChildJobs = new List<CommJob>();
                    foreach (List<CommJob> childJobs in allChildJobs)
                    {                        
                        foreach (CommJob j in childJobs)
                        {
                            if (j.ParentJob == exjob)
                            {
                                jobChildJobs.Add(j);
                                if (!j.IsPending && j.ChildJobErrorCode != DriverErrorCodes.ErrorNoError)
                                {
                                    e.ErrorCode = j.ChildJobErrorCode;
                                    break;
                                }
                            }
                        }
                        if (e.ErrorCode != DriverErrorCodes.ErrorNoError)
                            break;
                    }

                    if (exjob.IsStructAtomic())
                    {
                        if (write)
                        {
                            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
                            {
                                foreach (Tag tag in exjob.GetTagListOnWriting())
                                    tag.SetValue(tag.WriteVal);
                            }
                        }
                        else
                        {
                            // all read jobs are ok
                            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
                            {
                                foreach (Tag tag in exjob.TagsList)
                                {                                    
                                    CommJob j = jobChildJobs.Find(job => job.TagsList[0].TagNode.NodeId == tag.TagNode.NodeId);
                                    Tag cTag = (j == null ? null : j.TagsList[0]);
                                    if (cTag != null)                                    
                                    {                                        
                                        if (tag.GetValue() == null || !tag.IsQualityGood() || !Tag.IsReadValueEqualsTo(tag, tag.GetValue(), cTag.GetValue()))
                                            tag.SetValueFromChild(cTag.GetValue());
                                    }                                     
                                }
                            }
                        }
                    }

                    exjob.RWState = CommJob.RWStates.Standard;
                    OnJobExecuted(e);                    
                }
            }
        }
        #endregion
    }
}
