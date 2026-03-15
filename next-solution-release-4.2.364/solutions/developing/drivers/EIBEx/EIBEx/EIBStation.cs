using System;
using System.Collections.Generic;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Knx.Bus.Common.GroupValues;
using System.Threading.Tasks;

namespace EIB
{
    class EIBStation : Station
    {
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public EIBStation(CommunicationDriver commdriver, EIBStationSettings settings)
            : base(commdriver, settings)
        {
        }

        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as EIBCommJobSettings;
            if (conf == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidCommJobSettings);

            return new EIBCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as EIBTag;
            if (conf == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidTagObject);

            return new EIBCommJob(this, conf);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as EIBCommJob;
            if (commJob == null)
                throw new ArgumentException(Properties.Resources.ErrorInvalidJobObject);

            return new EIBCommJobSettings(session, commJob);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new EIBTag(td);
        }
        #endregion

        #region Virtual Methods
        //removed for incompatibility with DriverCodeBaseEx.Station.ProcessJobList()
        //public override bool IsCompatibleWithErrorList(CommJob commjob)
        //{
        //    EIBCommJob ej = (EIBCommJob)commjob;
        //    if (ej != null)
        //        return ej.MustSendARequest(out DateTime nextExecutionTime);

        //    return true;
        //}

        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            EIBCommJob eJ = e.Job as EIBCommJob;
            if (eJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError && e.Values != null)
            {
                byte[] Answer = (e.Values as GroupValue).Value;
                List<Tag> ChangedTags = new List<Tag>();
                eJ.SetJobData(Answer, ref ChangedTags);
                foreach (var tag in ChangedTags)
                {
                    var j = tag as Tag;
                    if (j != null)
                    {
                        e.ChangedTags.Add(j);
                    }
                }
#if DEBUG
                String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                String DbgText =
                String.Format("EIB DBG - PrJVal - {0} Chd = {1}",
                              dbgTime, e.ChangedTags.Count);
                System.Diagnostics.Debug.WriteLine(DbgText);
#endif
            }

            eJ.Status = EibCommJobStatus.Idle;

            base.ProcessJobValues(e);
        }

        public override bool Startup()
        {
            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob.AddRange(ListWholeJob);
            }

            EIBChannel EibChannel = (EIBChannel)Channel;
            foreach (var job in listJob)
                EibChannel.AddJobToInternalMap((EIBCommJob)job);

            return base.Startup();
        }

        #endregion

        #region Specific Methods
        public void ManageConnectionBroken(CommJob jobInError)
        {
            LastErrorCode = (DriverErrorCodes)(EIBProtocol.EIB_ERROR_CODES.DeviceFalconErrorConnectionBroken);
            InErrorState = true;
            if (ListWholeJob.Count > 0)
            {
                var listJob = new List<CommJob>();
                lock (lockListObject)
                {
                    listJob.AddRange(ListWholeJob);
                }
                Parallel.ForEach(listJob, job =>
                {
                    // Possibly, repeat the initial polling
                    EIBCommJob eibJob = (EIBCommJob)job;
                    eibJob.ResetInternalState();
                    eibJob.LastExecutionTime = DateTime.UtcNow.AddMilliseconds(Properties.Settings.Default.ConnectionRepeatDelay);
                });
                ExecutedJobArgs e = new ExecutedJobArgs();
                if (jobInError != null && listJob.Contains(jobInError))
                    e.Job = jobInError;
                else
                    e.Job = listJob[0];
                e.ErrorCode = (DriverErrorCodes)(EIBProtocol.EIB_ERROR_CODES.DeviceFalconErrorConnectionBroken);
                // Put all the jobs in error
                e.GeneralError = true;
                ((EIBChannel)this.GetChannel()).ChannelOnJobExecuted(e);
            }
        }

        public void ManageConnectionRestored()
        {
            InErrorState = false;
            if (ListWholeJob.Count > 0)
            {
                SetStateCommandVariableBit(false, (UInt16)StationVariableBits.StationErrorState);
            }
        }
        #endregion
    }
}
