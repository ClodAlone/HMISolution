////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	ChannelList.cs
//
// summary:	Implements the channelList class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DriverCodeBaseEx.Enumerators;
using DriverBaseInterfaces;
using DriverCodeBaseEx.Helpers;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Security;

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

        List<List<CommJob>> GetScheduledListJobQueue(CommJobState type)
        {
            lock (lockThreadObject)
            {
                if (!ScheduledListJobQueue.Keys.Contains(type))
                    ScheduledListJobQueue[type] = new List<List<CommJob>>();
                return ScheduledListJobQueue[type];
            }
        }
        #endregion

        #region Properties

        #endregion

        #region runtime properties

        #endregion

        #region Communication Procedures
        public override void commExecuteSyncro(CommJob exjob, NodeId tagNodeId = null, object value = null)
        {
            try
            {
                SuspendJobsExecution();
                if (SetSynchroJobData(exjob, tagNodeId, value))
                {
                    exjob.UpdateTagsListOnWriting();
                    exjob.SyncroExec = true;
                    exjob.IsPending = true;                    
                    commExecution(new List<CommJob>() { exjob }, this);
                }
            }
            finally
            {
                exjob.SyncroExec = false;
                RestartJobsExecution();
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
            if (exjoblist.Count == 1 && exjoblist[0].IsCustomJob())
            {
                // struct atomic job contain a separate jobs list to manage read/write
                GetCustomJobChildJobs(exjoblist[0], out List<List<CommJob>> allchildJobs, out bool write);
                foreach (var childJobs in allchildJobs)
                {
                    bool inErrorState = false;
                    while (!inErrorState && !bDisposed && !CommDriver.bTerminating && childJobs.Count(c => c.IsPending) > 0)
                    {
                        DriverErrorCodes conn = CheckDevice(childJobs, thischannel);

                        ResetNewDataEvent();

                        if (ExecuteJobList(ref conn, childJobs))
                            WaitNewDataEvent(ref conn);

                        if (!bDisposed && !CommDriver.bTerminating && childJobs.Count(c => c.IsPending) > 0)
                            ProcessNewDataList(conn, childJobs);

                        if (ResetConnection())
                            DeviceClose();

                        inErrorState = (childJobs.Count(c => c.InErrorState) > 0);
                    }
                    if (inErrorState)
                        break;
                }
                // merge result of separate jobs list into "main" job
                MergeCustomJobChildJobs(exjoblist[0], allchildJobs, write);
            }
            else
            {
                while (!bDisposed && !CommDriver.bTerminating && exjoblist.Count(c => c.IsPending) > 0)
                {
                    DriverErrorCodes conn = CheckDevice(exjoblist, thischannel);

                    ResetNewDataEvent();

                    if (ExecuteJobList(ref conn, exjoblist))
                        WaitNewDataEvent(ref conn);

                    if (!bDisposed && !CommDriver.bTerminating && exjoblist.Count(c => c.IsPending) > 0)
                        ProcessNewDataList(conn, exjoblist);

                    if (ResetConnection())
                        DeviceClose();
                }
            }
        }

        public virtual void GetNextPendingList(List<CommJob> inList)
        {
            lock (lockThreadObject)
            {
                var queue = GetNextPendingQueueList();
                if(queue != null && queue.Count > 0)
                {
                    inList.AddRange(queue[0]);
                    inList = inList.Select(c => { 
                        c.IsQueued = false; 
                        c.IsPending = true; 
                        return c; 
                    }).ToList();
                    queue.RemoveAt(0);
                }
            }
        }

        private void communicationProcedureList(object state)
        {
            Channel me = (Channel)state;
            bool bDecrementCounter = false;
            List<CommJob> nextlist = new List<CommJob>();

            try
            {
                lock (lockThreadObject)
                {
                    bCommunicationProcedure = true;
                    if (communicationProcedureTimer != null)
                    {
                        bDecrementCounter = true;
                        communicationProcedureTimer.Dispose();
                        communicationProcedureTimer = null;
                    }

                    if (bDisposed)
                        return;

                    GetNextPendingList(nextlist);                    
                }
                if (nextlist.Count > 0)
                {
                    commExecution(nextlist, me);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                lock (lockThreadObject)
                {
                    bCommunicationProcedure = false;

                    if (!KeepOpened && IsDeviceOpen())
                        DeviceClose();

                    if (!bDisposed && !bStopping)
                    {
                        if (ScheduleEvent != null && NeedNewSchedule())
                            ScheduleEvent.Set();

                        bool bRestart = RemainingJobInQueues();
                        if (bRestart || bRepromote)
                        {
                            //choose the correct delay for the new timer instance!!!!!
                            int nNextRun = 0;
                            if (repromoteDelay != -1 && !bRestart)
                                nNextRun = repromoteDelay;

                            if (InGeneralErrorState)
                                nNextRun = (Timeout > nNextRun ? Timeout : nNextRun);

                            bRepromote = false;
                            StartTimerExecution(communicationProcedureList, me, Math.Max(nNextRun, WaitTime));
                            repromoteDelay = -1;
                        }
                    }                    
                    if (CommunicationProcedureFinished != null)
                        CommunicationProcedureFinished.Set();
                    if (bDecrementCounter)
                    {
                        if (Interlocked.Decrement(ref CurrentCommunicationThreadsRunning) == MaxCommunicationThreadsAllowed)
                            lockScheduler.Set();
                    }
                }
            }
        }

        
        protected override bool NeedNewSchedule()
        {
            lock (lockThreadObject)
            {
                return (ScheduledListJobQueue[CommJobState.PollingInUse].Count <= (nInUseMax * Properties.Settings.Default.ScheduleRemainingPercent) &&
                    ScheduledListJobQueue[CommJobState.PollingInError].Count <= (nInErrorMax * Properties.Settings.Default.ScheduleRemainingPercent) &&
                    ScheduledListJobQueue[CommJobState.PollingNotInUse].Count <= (nNotInUseMax * Properties.Settings.Default.ScheduleRemainingPercent) &&
                    ScheduledListJobQueue[CommJobState.PollingNow].Count <= (nNowMax * Properties.Settings.Default.ScheduleRemainingPercent));
            }
        }

        protected override bool RemainingJobInQueues()
        {
            lock (lockThreadObject)
            {
                return (ScheduledListJobQueue[CommJobState.PollingInUse].Count > 0 ||
                ScheduledListJobQueue[CommJobState.PollingInError].Count > 0 ||
                ScheduledListJobQueue[CommJobState.PollingNotInUse].Count > 0 ||
                ScheduledListJobQueue[CommJobState.PollingNow].Count > 0);
            }
        }
        #endregion

        #region Virtual methods

        public override void InitData()
        {
            lock (lockThreadObject)
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

        public override void ForceExecution()
        {
            StartTimerExecution(communicationProcedureList, this, 0);
        }

        protected override void StartTimers()
        {
            StartTimerExecution(communicationProcedureList, this, WaitTime);
        }

        public virtual void SplitInExecutionLists(List<CommJob> jobList, ref List<List<CommJob>> exList)
        {
            
        }
        
        public virtual bool IsScheduledJobsListFull(List<CommJob> jobList)
        {
            return true;
        }

        private void RescheduleLastQueuedJobsList(List<CommJob> wList, ref List<List<CommJob>> exList)
        {
            if (exList.Count > 0)
            {
                if (!IsScheduledJobsListFull(exList[exList.Count - 1]))
                {
                    wList.InsertRange(0, exList[exList.Count - 1]);
                    exList.RemoveAt(exList.Count - 1);
                }
            }
        }
        #endregion

        #region Communication related Methods                
        protected override void RemoveJobOfStationOnSpecicQueue(Station station, CommJobState type)
        {
            var inputQueue = GetScheduledListJobQueue(type);

            var lToRemove = (from joblist in inputQueue.AsParallel() where (from job in joblist where job.Station == station select job).ToList().Count > 0 select joblist).ToList();
            foreach (var jr in lToRemove)
            {
                var l = jr.Select(c => { c.IsQueued = false; return c; }).ToList();
                inputQueue.Remove(jr);
            }
        }

        protected List<List<CommJob>> GetNextPendingQueueList()
        {
            if (GetScheduledListJobQueue(CommJobState.PollingNow).Count > 0 && PollingInUseJobsCounter == 0 && PollingNotInUseJobsCounter == 0)
            {
                if (PollingNowJobs ||
                    (GetScheduledListJobQueue(CommJobState.PollingInUse).Count == 0 &&
                    GetScheduledListJobQueue(CommJobState.PollingNotInUse).Count == 0 &&
                    GetScheduledListJobQueue(CommJobState.PollingInError).Count == 0))
                {
                    return GetScheduledListJobQueue(CommJobState.PollingNow);
                }
            }

            if (GetScheduledListJobQueue(CommJobState.PollingInUse).Count > 0 && PollingNotInUseJobsCounter == 0)
            {
                if (PollingInUseJobs ||
                    (GetScheduledListJobQueue(CommJobState.PollingNotInUse).Count == 0 &&
                    GetScheduledListJobQueue(CommJobState.PollingInError).Count == 0)
                    && (GetScheduledListJobQueue(CommJobState.PollingNow).Count == 0))
                {
                    return GetScheduledListJobQueue(CommJobState.PollingInUse);
                }
            }

            if (GetScheduledListJobQueue(CommJobState.PollingNotInUse).Count > 0)
            {
                if (PollingNotInUseJobs ||
                    GetScheduledListJobQueue(CommJobState.PollingInError).Count == 0)
                {
                    return GetScheduledListJobQueue(CommJobState.PollingNotInUse);
                }
            }

            PollingNowJobsCounter = 0;
            PollingInUseJobsCounter = 0;
            PollingNotInUseJobsCounter = 0;

            if (GetScheduledListJobQueue(CommJobState.PollingInError).Count > 0)
            {
                return GetScheduledListJobQueue(CommJobState.PollingInError);
            }
            //nothing to do, as the regular sequence
            if (GetScheduledListJobQueue(CommJobState.PollingNow).Count > 0)
            {
                PollingNowJobsCounter++;
                return GetScheduledListJobQueue(CommJobState.PollingNow);
            }
            if (GetScheduledListJobQueue(CommJobState.PollingInUse).Count > 0)
            {
                PollingInUseJobsCounter++;
                return GetScheduledListJobQueue(CommJobState.PollingInUse);
            }
            if (GetScheduledListJobQueue(CommJobState.PollingNotInUse).Count > 0)
            {
                PollingNotInUseJobsCounter++;
                return GetScheduledListJobQueue(CommJobState.PollingNotInUse);
            }
            if (GetScheduledListJobQueue(CommJobState.PollingInError).Count > 0)
            {
                return GetScheduledListJobQueue(CommJobState.PollingInError);
            }
            return null;
        }
        #endregion

        #region IChannelBase Interface
        protected override void RestartCommunicationTimer(int dueTime)
        {
            StartTimerExecution(communicationProcedureList, this, dueTime);
        }

        protected override int ChannelScheduleProcedure(List<CommJob> startList)
        {
            int nNextScheduleInterval = -1;
            int nNextDelayUse = -1;
            int nNextDelayError = -1;
            int nNextDelayNotUse = -1;

            DateTime dtNow = DateTime.UtcNow;
            
            lock (lockThreadObject)
            {
                List<CommJob> wList = null;
                wList = (from job in startList
                         where  !job.IsPending && job.InUse && !job.SyncroExec 
                                && job.IsConditionalVariableOn()
                                && (job.Type == LinkType.UnconditionalOutput || job.GetTagListToWriteCount() > 0)
                         orderby job.LastExecutionTime ascending
                         select job).ToList();

                if (wList != null && wList.Count > 0)
                {
                    bool anyQueued = (wList.Count(j => j.IsQueued) > 0);
                    if (anyQueued)
                    {
                        if (ScheduledListJobQueue[CommJobState.PollingNow].Count > 0)
                        {
                            foreach (var l in ScheduledListJobQueue[CommJobState.PollingNow])
                            {
                                var tlRemove = (from p in wList
                                                where l.Contains(p)
                                                select p).ToList();
                                wList.RemoveAll((o) => tlRemove.Contains(o));
                                if (wList.Count == 0)
                                    break;
                            }
                        }

                        foreach (var job in wList)
                        {
                            if (job.IsQueued)
                            {
                                bool removed = false;
                                foreach (var jobState in new List<CommJobState> { CommJobState.PollingInUse, CommJobState.PollingInError, CommJobState.PollingNotInUse })
                                {
                                    if (ScheduledListJobQueue[jobState].Count > 0)
                                    {
                                        for (int jobListIndex = 0; jobListIndex < ScheduledListJobQueue[jobState].Count; jobListIndex++)
                                        {
                                            if (ScheduledListJobQueue[jobState][jobListIndex].Contains(job))
                                            {
                                                removed = true;
                                                ScheduledListJobQueue[jobState][jobListIndex].Remove(job);
                                                // if list of jobs in queue is empty remove it
                                                if (ScheduledListJobQueue[jobState][jobListIndex].Count == 0)
                                                    ScheduledListJobQueue[jobState].RemoveAt(jobListIndex);
                                                break;
                                            }
                                        }
                                    }
                                    if (removed)
                                        break;
                                }
                            }
                        }
                    }

                    if (wList.Count > 0)
                    {
                        wList.ForEach(job =>
                        {
                            job.IsQueued = true;
                            job.UpdateTagsListOnWriting();
                        });
                        var exList = ScheduledListJobQueue[CommJobState.PollingNow];
                        RescheduleLastQueuedJobsList(wList, ref exList);
                        SplitInExecutionLists(wList, ref exList);
                        if (wList.Count > 0)
                        {
                            wList.ForEach(job =>
                            {
                                job.IsQueued = false;
                                job.UpdateTagsListToWrite();
                            });
                        }
                        nNowMax = exList.Count;
                    }
                }

                wList = (from job in startList
                             //where !job.IsPending && !job.IsQueued && job.InUse && !job.InErrorState
                         where !job.IsPending && !job.IsQueued && (job.InUse || job.ExecuteFirstTime == true) && !job.InErrorState
                                 && job.IsConditionalVariableOn()
                                    && ((job.Type == LinkType.Input || job.Type == LinkType.InputOutput) && job.GetTagListToWriteCount() == 0)
                                    && (((dtNow - job.LastExecutionTime).TotalMilliseconds >= job.SamplingInterval) || job.ExecuteFirstTime == true)
                            orderby job.LastExecutionTime.AddMilliseconds(job.SamplingInterval) ascending
                            select job).ToList();
                if (wList != null && wList.Count > 0)
                {
                    wList.ForEach(job =>
                    {
                        job.IsQueued = true;
                        job.UpdateTagsListOnWriting();
                    });
                    var exList = ScheduledListJobQueue[CommJobState.PollingInUse];
                    RescheduleLastQueuedJobsList(wList, ref exList);
                    SplitInExecutionLists(wList, ref exList);
                    if (wList.Count > 0)
                    {
                        wList.ForEach(job =>
                        {
                            job.IsQueued = false;
                            job.UpdateTagsListToWrite();
                        });
                    }
                    nInUseMax = exList.Count;
                }

                Dictionary<Station, List<CommJob>> stationsInError = (from job in startList
                                                                        where !job.IsPending && !job.IsQueued && job.InUse && job.InErrorState
                                                                            && job.IsConditionalVariableOn()
                                                                            && (job.Type != LinkType.ExceptionOutput || job.GetTagListToWriteCount() > 0)
                                                                        //&& (dtNow - job.LastErrorTime).TotalMilliseconds >= job.ErrorPollingTime
                                                                        orderby job.LastErrorTime.AddMilliseconds(job.ErrorPollingTime) ascending
                                                                        group job by job.Station into jobsInError
                                                                        select new { Station = jobsInError.Key, Values = jobsInError.ToList() }).ToDictionary(t => t.Station, t => t.Values);

                if (stationsInError != null)
                {
                    wList = new List<CommJob>();
                    foreach (var stInErr in stationsInError)
                    {
                        Station st = stInErr.Key;
                        List<CommJob> jobs = stInErr.Value;
                        // get jobs to schedule now
                        List<CommJob> jobsToScheduleNow = jobs.FindAll(job => (dtNow - job.LastErrorTime).TotalMilliseconds >= job.ErrorPollingTime);
                        if (jobsToScheduleNow == null)
                            jobsToScheduleNow = new List<CommJob>();
                        // station in error and no jobs to execute now ?
                        if (st.InGeneralErrorState && jobsToScheduleNow.Count == 0)
                        {
                            // if first of list (all jobs is error) exceed PollingTimeInError force execution
                            if ((dtNow - st.StationLastErrorTime).TotalMilliseconds >= PollingTimeInError)
                                wList.Add(jobs[0]);
                        }
                        else
                        {
                            wList.AddRange(jobsToScheduleNow);
                        }
                    }
                    if (wList.Count > 0)
                    {
                        wList.ForEach(job =>
                        {
                            job.IsQueued = true;
                            job.UpdateTagsListOnWriting();
                        });
                        var exList = ScheduledListJobQueue[CommJobState.PollingInError];
                        RescheduleLastQueuedJobsList(wList, ref exList);
                        SplitInExecutionLists(wList, ref exList);
                        if (wList.Count > 0)
                        {
                            wList.ForEach(job =>
                            {
                                job.IsQueued = false;
                                job.UpdateTagsListToWrite();
                            });
                        }
                        nInErrorMax = exList.Count;
                    }
                }

                if (PollingTimeNotInUse > 0)
                {
                    wList = (from job in startList
                                where !job.IsPending && !job.InUse && !job.IsQueued && !job.InErrorState
                                    && job.IsConditionalVariableOn()
                                    //&& (job.Type != LinkType.ExceptionOutput || job.GetTagListToWriteCount() > 0)
                                    && ((job.Type == LinkType.Input || job.Type == LinkType.InputOutput) && job.GetTagListToWriteCount() == 0)
                                    && (((dtNow - job.LastExecutionTime).TotalMilliseconds >= PollingTimeNotInUse) || job.ExecuteFirstTime == true)
                                orderby job.LastExecutionTime.AddMilliseconds(PollingTimeNotInUse) ascending
                                select job).ToList();
                    if (wList != null && wList.Count > 0)
                    {
                        wList.ForEach(job =>
                        {
                            job.IsQueued = true;
                            job.UpdateTagsListOnWriting();
                        });
                        var exList = ScheduledListJobQueue[CommJobState.PollingNotInUse];
                        RescheduleLastQueuedJobsList(wList, ref exList);
                        SplitInExecutionLists(wList, ref exList);
                        if (wList.Count > 0)
                        {
                            wList.ForEach(job =>
                            {
                                job.IsQueued = false;
                                job.UpdateTagsListToWrite();
                            });
                        }
                        nNotInUseMax = exList.Count;
                    }
                }

                List<CommJob> rList;
                rList = (from job in startList
                            where !job.IsPending && !job.IsQueued && job.InUse && !job.InErrorState
                                && job.IsConditionalVariableOn()
                                && (job.Type != LinkType.ExceptionOutput || job.GetTagListOnWritingCount() > 0)
                                && (((dtNow - job.LastExecutionTime).TotalMilliseconds < job.SamplingInterval && job.SamplingInterval != CommJob.JOB_NOT_SCHEDULABLE) || job.ExecuteFirstTime == true)
                            orderby job.LastExecutionTime.AddMilliseconds(job.SamplingInterval) ascending
                            select job).ToList();
                if (rList.Count > 0)
                {
                    var dtNextSchedule = rList[0].LastExecutionTime.AddMilliseconds(rList[0].SamplingInterval);
                    if (dtNextSchedule > dtNow)
                        nNextDelayUse = (int)(dtNextSchedule - dtNow).TotalMilliseconds;
                    else
                        return 0;
                }

                if (PollingTimeInError > 0)
                {
                    rList = (from job in startList
                                where !job.IsPending && !job.IsQueued && job.InUse && job.InErrorState
                                        && job.IsConditionalVariableOn()
                                        && (job.Type != LinkType.ExceptionOutput || job.GetTagListOnWritingCount() > 0)
                                        && (dtNow - job.LastErrorTime).TotalMilliseconds <= job.ErrorPollingTime
                                orderby job.LastErrorTime ascending
                                select job).ToList();
                    if (rList.Count > 0)
                    {
                        var dtNextSchedule = rList[0].LastErrorTime.AddMilliseconds(rList[0].ErrorPollingTime);
                        if (dtNextSchedule > dtNow)
                        {
                            nNextDelayError = (int)(dtNextSchedule - dtNow).TotalMilliseconds;
                            // if channel (all stations) is in general error and something were scheduled, wait to reschedule that all jobs in error are processed
                            if (InGeneralErrorState && ScheduledListJobQueue[CommJobState.PollingInError].Count > 0)
                                nNextDelayError = -1;
                            // if channel or station is in general error, force to restart not far than PollingTimeInError
                            else if (nNextDelayError > PollingTimeInError && (rList[0].Station.InGeneralErrorState || InGeneralErrorState))
                                nNextDelayError = PollingTimeInError;
                        }
                        else
                            return 0;
                    }
                }

                if (PollingTimeNotInUse > 0)
                {
                    rList = (from job in startList
                                where !job.IsPending && !job.IsQueued && !job.InUse && !job.InErrorState
                                    && job.IsConditionalVariableOn()
                                    && (job.Type != LinkType.ExceptionOutput || job.GetTagListOnWritingCount() > 0)
                                    && (((dtNow - job.LastExecutionTime).TotalMilliseconds < PollingTimeNotInUse) || job.ExecuteFirstTime == true)
                                orderby job.LastExecutionTime.AddMilliseconds(PollingTimeNotInUse) ascending
                                select job).ToList();
                    if (rList.Count > 0)
                    {
                        var dtNextSchedule = rList[0].LastExecutionTime.AddMilliseconds(PollingTimeNotInUse);
                        if (dtNextSchedule > dtNow)
                            nNextDelayNotUse = (int)(dtNextSchedule - dtNow).TotalMilliseconds;
                        else
                            return 0;
                    }
                }

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
                
                return nNextScheduleInterval;
            }
        }

        protected override bool IsJobScheduledToWrite(CommJob job)
        {
            bool scheduled = false;

            lock (lockThreadObject)
            {
                if (job.IsQueued)
                {
                    if (ScheduledListJobQueue[CommJobState.PollingNow].Count > 0)
                    {
                        for (int jobListIndex = 0; jobListIndex < ScheduledListJobQueue[CommJobState.PollingNow].Count; jobListIndex++)
                        {
                            if (ScheduledListJobQueue[CommJobState.PollingNow][jobListIndex].Contains(job))
                            {
                                scheduled = true;
                                break;
                            }
                        }
                    }
                }
            }

            return scheduled;
        }

        protected override void RemoveScheduledJobs(Station s, List<CommJob> jobs)
        {
            lock (lockThreadObject)
            {
                foreach (var state in ScheduledListJobQueue.Keys)
                {
                    foreach (var lj in ScheduledListJobQueue[state])
                        lj.RemoveAll(j => j.Station == s && jobs.Contains(j));
                }
                Parallel.ForEach(jobs, job =>
                {
                    job.IsQueued = false;
                });
            }
        }

        /// <summary>
        /// Return the list of jobs (instead of use exJob job) to manage read/write operation
        /// </summary>
        /// <param name="exjob"></param>
        /// <param name="allChildJobs"></param>
        /// <param name="write"></param>
        protected virtual void GetCustomJobChildJobs(CommJob exjob, out List<List<CommJob>> allChildJobs, out bool write)
        {
            allChildJobs = new List<List<CommJob>>();
            List<CommJob> resultJobs = new List<CommJob>();

            // check if is a read or write operation
            write = (exjob.GetTagListOnWritingCount() > 0 || exjob.Type == LinkType.UnconditionalOutput);
            if (write)
            {
                if (exjob.Type == LinkType.UnconditionalOutput)
                    exjob.FillWholeTagsListOnWriting();

                foreach (var tag in exjob.TagsListOnWriting)
                {
                    // copy each tag to write from exJob (main) to sub jobs (StructAtomicJobsListWrite)
                    if (exjob.ChildJobsWrite.ContainsKey(tag.TagNode.NodeId))
                    {
                        CommJob childJob = exjob.ChildJobsWrite[tag.TagNode.NodeId];
                        childJob.TagsListOnWriting.Clear();
                        childJob.TagsListToWrite.Clear();
                        childJob.TagsList[0].Value.Value = Utils.Clone(tag.Value.Value);
                        childJob.AddTagListOnWriting(childJob.TagsList[0]);
                        if (childJob.IsCustomJob())
                        {
                            GetCustomJobChildJobs(childJob, out List<List<CommJob>> customChildJobs, out bool childWrite);
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
                        GetCustomJobChildJobs(job, out List<List<CommJob>> customChildJobs, out bool childWrite);
                        if (!childWrite)
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

            // reset each jobs before to be executed
            Parallel.ForEach(resultJobs, j =>
            {
                j.InitChildJobBeforeExecution();
            });

            SplitInExecutionLists(resultJobs, ref allChildJobs);            
        }

        /// <summary>
        /// Merge the result of "Struct Atomic child jobs" into exJob job and then publish result
        /// </summary>
        /// <param name="exjob"></param>
        /// <param name="allChildJobs"></param>
        /// <param name="write"></param>
        protected virtual void MergeCustomJobChildJobs(CommJob exjob, List<List<CommJob>> allChildJobs, bool write)
        {
            ExecutedJobArgs e = new ExecutedJobArgs();
            e.Job = exjob;
            e.ErrorCode = DriverErrorCodes.ErrorNoError;
            
            foreach (List<CommJob> childJobs in allChildJobs)
            {
                foreach (CommJob j in childJobs)
                {
                    if (!j.IsPending && j.ChildJobErrorCode != DriverErrorCodes.ErrorNoError)
                    {                        
                        e.ErrorCode = j.ChildJobErrorCode;
                        break;
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
                        foreach (Tag tag in exjob.TagsList)
                            tag.SetIniternalValue(tag.Value.Value);
                    }
                }
                else
                {
                    // all read jobs are ok
                    if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
                    {
                        foreach (List<CommJob> childJobs in allChildJobs)
                        {
                            foreach (CommJob j in childJobs)
                            {
                                Tag tag = exjob.TagsList.Find(t => t.TagNode.NodeId == j.TagsList[0].TagNode.NodeId);
                                if (tag != null)
                                {
                                    if (tag.GetReadValue() == null || !tag.IsQualityGood() || !Tag.IsReadValueEqualsTo(j.TagsList[0], j.TagsList[0].GetReadValue(), tag.GetReadValue()))
                                        tag.SetReadValueChildJob(j.TagsList[0].GetReadValue());
                                }
                            }
                        }
                    }
                }
            }

            exjob.RWState = CommJob.RWStates.Standard;
            OnJobExecuted(e);
        }
        #endregion
    }
}
