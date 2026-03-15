using System;
using System.Collections.Generic;
using System.Threading;
using DevExpress.Xpo;
using DriverBaseInterfaces;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
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
            MaxRetriesBeforeError = 0;
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
        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut || e.ErrorCode == DriverErrorCodes.ErrorDeviceOpenFailed || e.ErrorCode == (DriverErrorCodes)DataboomProtocol.DataboomErrorCodes.ErrorCodeErrorPublish);

            base.ProcessJobValues(e);
        }
        
        public virtual uint OnWriteTag(TagDefinition tag, ref object value, ref StatusCode statusCode, ref DateTime timestamp)
        {
            CommJob job;
            lock (lockListObject)
            {
                if (!mapTagJob.ContainsKey(tag.NodeId))
                    return StatusCodes.BadNodeIdInvalid;

                job = mapTagJob[tag.NodeId];

                ////check if the tag must be write 
                //if ((RewritingOfTheSameValue == false) &&
                //    (job.Type == LinkType.ExceptionOutput || job.Type == LinkType.InputOutput))
                //{
                //    DataValue objTmp = new DataValue();
                //    objTmp.Value = Utils.Clone(value);
                //    int nTagsToWrite = (from t in job.TagsList.AsParallel()
                //                        where (t.TagNode.NodeId == tagnodeid) &&
                //                               (!StatusCode.IsGood(t.Value.StatusCode) ||
                //                               (t.LastValue == null) ||
                //                               (!t.LastValue.Equals(objTmp.Value)))
                //                        select t).Count();
                //    if (nTagsToWrite == 0)
                //    {
                //        job.RemoveTagFromTagListToWrite(tagnodeid);

                //        return (StatusCodes.Good);
                //    }
                //}
            }

            uint ret = StatusCodes.BadConfigurationError;
            if (ChannelBase != null)
            {
                if (!CommDriver.WriteAsync && job.IsConditionalVariableOn())
                {
                    List<object> outputvalues = new List<object>();
                    outputvalues.Add(new uint());
#if DEBUG
                    System.Diagnostics.Trace.TraceInformation("{0} OnWriteTag !CommDriver.WriteAsync", Thread.CurrentThread.ManagedThreadId);
#endif                    
                    ret = ExecuteSyncroJob(job, false, 0, outputvalues, false, tag.NodeId, value); //synchro...
                    if (ret == StatusCodes.Good)
                        ret = (Convert.ToInt32(outputvalues[0]) != 0 ? StatusCodes.Bad : StatusCodes.Good);
                    job.ClearTagListWrite();
                }
                else
                {
                    // store (only "valid value" to write/long) into TagListToWrite but don't launch scheduler with data change value
                    ret = job.OnWriteTag(tag.NodeId, ref value);
                    //if (ret == StatusCodes.Good)
                    //{
                    //    // for job in PollingInError state, wait "normal" scheduling (to avoid station state var unstable value)
                    //    if (!Channel.IsJobInPollingInErrorState(job))
                    //        ChannelBase.ChangeStateJob(job, CommJobState.PollingNow);
                    //}
                }
            }
            return ret;
        }

        // functionality not suported by this driver
        public override uint ExecuteSyncroJob(CommJob job, bool localmethod, int methodid, IList<object> args, bool takedata = true, NodeId tagNodeId = null, object value = null)
        {
            args[0] = (int)DriverErrorCodes.ErrorTimeOut;

            return StatusCodes.BadNotSupported;
        }
        #endregion
    }
}
