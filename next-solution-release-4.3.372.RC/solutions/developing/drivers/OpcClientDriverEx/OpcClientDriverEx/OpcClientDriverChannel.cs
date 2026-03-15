using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using DriverCodeBaseEx.Helpers;
using Opc.Ua;
using static DriverCodeBaseEx.CommJob;

namespace OpcClientDriver
{
    class OpcClientDriverChannel : Channel
    {
        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public OpcClientDriverChannel(CommunicationDriver commdriver, OpcClientDriverChannelSettings settings)
            : base(commdriver, settings)
        {
            _HostName = settings.HostName;
            _BackupHostList = OpcClientDriverProtocol.GetValidBackupHostList(settings.BackupHostList);
        }

        #endregion


        #region Override Methods
        bool bDisposing;
        public override void Dispose()
        {
            if (bDisposing)
                return;
            bDisposing = true;
            base.Dispose();
        }
        public override bool Startup()
        {
            if (bDisposing)
                return false;
            return true;
        }

        public override void commExecuteSyncro(CommJob exjob, NodeId tagNodeId = null, object value = null, int timeOut = 0)
        {
            if (AcquireCommunicationThreadControl(timeOut))
            {
                RemoveJobFromSchedulingQueue(exjob, (CommJob.ScheduleProperties.SetSyncroExec | CommJob.ScheduleProperties.SetIsPending));
                if (SetSynchroJobData(exjob, tagNodeId, value))
                {
                    exjob.UpdateTagsListOnWriting();                        
                    ((OpcClientDriverStation)(exjob.Station)).ExecuteSyncroJob(exjob);
                }
                else
                {
                    exjob.SetScheduleProperties((CommJob.ScheduleProperties.UnSetSyncroExec | ScheduleProperties.UnSetIsPending));
                }                    
                ReleaseCommunicationThreadControl();                
            }
        }

        //protected override void StartTimers()
        //{            
        //}

        //protected override void StopTimers(bool bTerminate = true)
        //{
        //}

        //definition due, they're abstract
        public override bool IsDeviceOpen(){return true;}
        public override bool DeviceOpen() { return true; }
        public override bool DeviceClose() { return true; }
        public override bool DeviceRead(byte[] Buffer, uint Count){return true;}
        public override bool DeviceWrite(byte[] Buffer, uint Count){return true;}
        public override uint GetBytesToRead(){return 0;}
        public override uint GetBytesToWrite(){return 0;}

        #endregion

        #region Properties

        private string _HostName;
        public string HostName
        {
            get { return _HostName; }
            set { _HostName = value; }
        }

        private string[] _BackupHostList;
        public string[] BackupHostList
        {
            get { return _BackupHostList; }
            set { _BackupHostList = value; }
        }        
        #endregion

        #region Statistics
        /// <summary>   The statistic nodes. </summary>
        public static readonly StatisticSetting.NodeDataNames[] OpcClientStatisticNodes = new StatisticSetting.NodeDataNames[]
        {
            StatisticSetting.NodeDataNames.TotalJobsError,
            StatisticSetting.NodeDataNames.LastErrorTime,
            StatisticSetting.NodeDataNames.LastError,
            StatisticSetting.NodeDataNames.InErrorState,
        };
        #endregion
    }
}
