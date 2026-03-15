using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using OPCUAViewModel;
using UFInterfaces;
using Utilities;
using ViewModelLib;

namespace AlarmSink
{
    public class AlarmSink : IEntityReference, IDisposable
    {
        static Thread threadTerminatedSinks;
        static long maxSessions = 0;
        static bool bStopThreadTerminatedSinks = false;
        readonly static List<AlarmSink> activeSinks = new List<AlarmSink>();

        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.WebClientHTML5Log);
        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);

        OPCUAEntityReference serverAlarmsSoundReference;
        OPCUAEntityReference serverReference;
        MonitoredItemViewModel server;

        bool bRefreshed;
        bool bDisposed;
        DateTime lastTimeUsed;

        #region OnRecycling
        /// <summary>
        /// Triggers the Recycling event.
        /// </summary>
        public event EventHandler Recycling;
        public virtual void OnRecycling()
        {
            var e = Recycling;
            if (e != null)
                e(this, EventArgs.Empty);
        }
        #endregion

        static AlarmSink()
        {
            bStopThreadTerminatedSinks = false;
#if !DEBUG
            maxSessions = MSZ.MSZView.GetModule("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxQ0HNh4VycfQXLY5Z6lNe+QBGr0uKoS0SZW/4j3jqDSg="/* WCL5 */);
#else
            maxSessions = 50;
#endif
        }

        void CheckInstances()
        {
            lock (activeSinks)
            {
                if (bDisposed)
                    return;

                if (threadTerminatedSinks == null)
                {
                    threadTerminatedSinks = new Thread((o) =>
                    {
                        Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                        while (!bStopThreadTerminatedSinks)
                        {
                            Thread.Sleep(2000);
                            AlarmSink check = null;
                            lock (activeSinks)
                            {
                                if (activeSinks.Count == 0)
                                    break;

                                var list = (from c in activeSinks/*.AsParallel()*/
                                            where c.lastTimeUsed.AddSeconds(SessionTimeout) < DateTime.UtcNow
                                            select c).ToList();
                                if (list.Count > 0)
                                    check = list[0];
                            }
                            if (check != null)
                            {
                                log.Info(Properties.Resources.EndingExpiredSession);
                                check.Dispose();
                                check.OnRecycling();
                            }
                        }
                    }) { IsBackground = true };
                    threadTerminatedSinks.Start();
                }

                lock (activeSinks)
                {
                    if (activeSinks.Count >= maxSessions)
                    {
                        logLicense.Warn(String.Format(Properties.Resources.LicenseClientExceeded, activeSinks.Count, maxSessions));
                        return;
                    }

                    if (!activeSinks.Contains(this))
                        activeSinks.Add(this);
                }
            }
        }

        public AlarmSink(String reference, String session, String alarmsSoundReference = null)
        {
            serverReference = reference.FromXml<OPCUAEntityReference>();
            serverReference.Resolve(session);
            serverReference.SetInUse(this, true);

            if (alarmsSoundReference != null)
            {
                serverAlarmsSoundReference = alarmsSoundReference.FromXml<OPCUAEntityReference>();
                serverAlarmsSoundReference.Resolve(session);
                serverAlarmsSoundReference.SetInUse(this, true);
            }

            lastTimeUsed = DateTime.UtcNow;
            CheckInstances();
        }

        private int _SessionTimeout = 60;
        public int SessionTimeout
        {
            get { return _SessionTimeout; }
            set
            {
                _SessionTimeout = value;
            }
        }

        private int _SessionOffset = 0;
        public int SessionOffset
        {
            get { return _SessionOffset; }
            set
            {
                _SessionOffset = value;
            }
        }

        public bool IsConnected
        {
            get
            {
                lastTimeUsed = DateTime.UtcNow;
                return server != null && server.GetSubscriptionViewModelParent() != null && 
                    server.GetSubscriptionViewModelParent().GetSessionViewModelParent() != null &&
                    server.GetSubscriptionViewModelParent().GetSessionViewModelParent().Connected;
            }
        }

        public bool IsSoundBuzzing
        {
            get
            {
                lastTimeUsed = DateTime.UtcNow;
                if (serverAlarmsSoundReference != null && serverAlarmsSoundReference.MonitoredItemViewModel != null)
                {
                    bool isBuzzing = false;
                    if (bool.TryParse(serverAlarmsSoundReference.MonitoredItemViewModel.InvariantCultureValue, out isBuzzing))
                        return isBuzzing;
                }

                return false;
            }
        }

        public void AckAll()
        {
            lastTimeUsed = DateTime.UtcNow;
            if (!IsConnected || !server.ConditionAcknowledgeAllCommand.CanExecute(null))
                return;
            server.ConditionAcknowledgeAllCommand.Execute(null);
        }

        public void AckSelected(String[] list)
        {
            lastTimeUsed = DateTime.UtcNow;
            if (!IsConnected || list == null)
                return;

            foreach (var item in list)
            {
                var found = (from c in server.ConditionStateList where c.NodeIdString == item select c).ToList();
                if (found.Count == 0 || !found[0].AcknowledgeCommand.CanExecute(null))
                    continue;
                found[0].AcknowledgeCommand.Execute(null);
            }
        }

        public void ConfirmAll()
        {
            lastTimeUsed = DateTime.UtcNow;
            if (!IsConnected || !server.ConditionConfirmAllCommand.CanExecute(null))
                return;
            server.ConditionConfirmAllCommand.Execute(null);
        }

        public void ConfirmSelected(String[] list)
        {
            lastTimeUsed = DateTime.UtcNow;
            if (!IsConnected || list == null)
                return;

            foreach (var item in list)
            {
                var found = (from c in server.ConditionStateList where c.NodeIdString == item select c).ToList();
                if (found.Count == 0 || !found[0].ConfirmCommand.CanExecute(null))
                    continue;
                found[0].ConfirmCommand.Execute(null);
            }
        }

        public void Refresh()
        {
            lastTimeUsed = DateTime.UtcNow;
            if (!IsConnected || !server.ConditionRefreshCommand.CanExecute(null))
                return;
            server.ConditionRefreshCommand.Execute(null);
        }

        public SafeObservableCollection<ConditionStateViewModel> ConditionList
        {
            get
            {
                if (server == null)
                {
                    if (serverReference.MonitoredItemViewModel == null)
                        return null;
                    server = serverReference.MonitoredItemViewModel;
                }
                else if (!bRefreshed)
                {
                    if (server.ConditionRefreshCommand.CanExecute(null) && server.GetSubscriptionViewModelParent().GetSessionViewModelParent().Connected == true)
                    {
                        server.ConditionRefreshCommand.Execute(null);
                        bRefreshed = false;
                    }
                }
                return server.ConditionStateList;
            }
        }

        public void Dispose()
        {
            lock (activeSinks)
            {
                if (activeSinks.Contains(this))
                    activeSinks.Remove(this);

                bStopThreadTerminatedSinks = activeSinks.Count == 0;
            }

            if (bDisposed)
                return;
            bDisposed = true;

            if (serverReference != null)
            {
                serverReference.SetInUse(this, false);
                serverReference = null;
                server = null;
            }

            if (serverAlarmsSoundReference != null)
            {
                serverAlarmsSoundReference.SetInUse(this, false);
                serverAlarmsSoundReference = null;
            }
        }

        public System.Windows.Media.ImageSource CollapsedImageSource
        {
            get { throw new NotImplementedException(); }
        }

        public System.Windows.Media.ImageSource ExpandedImageSource
        {
            get { throw new NotImplementedException(); }
        }

        public System.Windows.Controls.ContextMenu contextMenu
        {
            get { throw new NotImplementedException(); }
        }

        public object Tooltip
        {
            get { throw new NotImplementedException(); }
        }

        public object ContainedObject
        {
            get { throw new NotImplementedException(); }
        }

        public object EntityParent
        {
            get { throw new NotImplementedException(); }
        }

        public string TypeDefinitionString
        {
            get { throw new NotImplementedException(); }
        }
    }
}
