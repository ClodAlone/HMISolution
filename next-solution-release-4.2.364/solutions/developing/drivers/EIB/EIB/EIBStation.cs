using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Opc.Ua;
using DevExpress.Xpo;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using DriverBaseInterfaces;
using Knx.Bus.Common;
using Knx.Bus.Common.Configuration;
using Knx.Bus.Common.GroupValues;
using Knx.Falcon.Sdk;
using DriverCodeBase.Helpers;
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
        public override bool IsCompatibleWithErrorList(CommJob commjob)
        {
            EIBCommJob ej = (EIBCommJob)commjob;
            if (ej != null)
                    return ej.MustSendARequest();

            return true;
        }

        public override bool Startup()
        {
            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob.AddRange(ListWholeJob);
            }

                // Build Group Address Dictionaries
                if (Channel != null)
                {
                    EIBChannel EibChannel = (EIBChannel) Channel;
                        foreach (var job in listJob)
                        {
                            EIBCommJob EibJob = (EIBCommJob)job;
                            if ((EibJob.Type == LinkType.Input) || (EibJob.Type == LinkType.InputOutput))
                            {
                                if (!String.IsNullOrWhiteSpace(EibJob.InputGroups))
                                {
                                    string[] ListAddressSplit = EibJob.InputGroups.Split(new Char[] { ';' });
                                    foreach (var InAdd in ListAddressSplit)
                                    {
                                        EibChannel.AddToMapEIBInputGroupAddJobs(EIBCommJob.EibGroupAddressToUInt16(InAdd), EibJob);
                                    }
                                }
                                if (!String.IsNullOrWhiteSpace(EibJob.PollingGroup))
                                {
                                    EibChannel.AddToMapEIBInputGroupAddJobs(EIBCommJob.EibGroupAddressToUInt16(EibJob.PollingGroup), EibJob);
                                }
                                if (EibJob.Type != LinkType.Input)
                                {
                                    if (!String.IsNullOrWhiteSpace(EibJob.OutputGroup))
                                    {
                                        UInt16 ConvertedAddress = EIBCommJob.EibGroupAddressToUInt16(EibJob.OutputGroup);
                                        if (ConvertedAddress > 0)
                                        {
                                            EibChannel.AddToMapEIBOutputGroupAddJobs(ConvertedAddress, EibJob);
                                        }
                                    }
                                }

                            }
                            else
                            {
                                if (!String.IsNullOrWhiteSpace(EibJob.OutputGroup))
                                {
                                    UInt16 ConvertedAddress = EIBCommJob.EibGroupAddressToUInt16(EibJob.OutputGroup);
                                    if (ConvertedAddress > 0)
                                    {
                                        EibChannel.AddToMapEIBOutputGroupAddJobs(ConvertedAddress, EibJob);
                                    }
                                }
                            }
                        }
                    }

            // Call the base class method
            if (!base.Startup())
            {
                return false;
            }

            return true;
        }

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

        #endregion

        #region Specific Methods

        public void ManageConnectionBroken()
        {
            LastErrorCode = (DriverErrorCodes)(EIB_ERROR_CODES.DeviceFalconErrorConnectionBroken);
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
                    if (eibJob.EnablePolling && (eibJob.PollingTime == 0))
                    {
                        if (!eibJob.PollingEnabled)
                        {
                            eibJob.PollingEnabled = true;
                        }
                    }
                    // Deactivate the job
                    if ((eibJob.Status == EibCommJobStatus.ReadRequestPending) ||
                       (eibJob.Status == EibCommJobStatus.WriteRequestPending))
                    {
                        eibJob.Status = EibCommJobStatus.Idle;
                    }
                });
                ExecutedJobArgs e = new ExecutedJobArgs();
                e.Job = listJob[0];
                e.ErrorCode = (DriverErrorCodes)(EIB_ERROR_CODES.DeviceFalconErrorConnectionBroken);
                // Put all the jobs in error
                e.GeneralError = true;
                base.ProcessJobValues(e);
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
