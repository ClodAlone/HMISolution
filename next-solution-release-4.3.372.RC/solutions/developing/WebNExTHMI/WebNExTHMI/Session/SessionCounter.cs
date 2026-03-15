using log4net;
using OPCUAViewModel;
using System;
using UFInterfaces;
using UFUAEditor.ComponentService;
using ViewModelLib;
using Utilities;
using System.Globalization;

namespace WebNExTHMI.Session
{
    public class SessionCounter : IDisposable, IEntityReference
    {
        #region Fields
        private static ILog log;
        static OPCUAEntityReference activeSessionsCountCustomVariable;
        static OPCUAEntityReference activeSessionsCountSystemTagVariable;
        static PropertyObserver<OPCUAEntityReference> sessionCountTagObserver;
        static PropertyObserver<SessionViewModel> sessionCountSessionObserver;
        bool bSessionCounterInvalidated;
        bool bDisposed;
        #endregion

        #region Props
        public OPCUAEntityReference ActiveSessionsCountVariable
        {
            get
            {
                return !String.IsNullOrEmpty(PlatformComponents.PlatformComponents.ActiveSessionCountVariableName) ? activeSessionsCountCustomVariable : activeSessionsCountSystemTagVariable;
            }
        }
        #endregion

        #region ctor
        public SessionCounter()
        {
            SubscribeActiveSessionsCount();
        }
        #endregion

        void SubscribeActiveSessionsCount()
        {
            if (!String.IsNullOrEmpty(PlatformComponents.PlatformComponents.ActiveSessionCountVariableName))
            {
                var split = PlatformComponents.PlatformComponents.ActiveSessionCountVariableName.Split(':');
                var instance = split[0];
                var name = split[0];
                if (split.Length > 1)
                    name = split[1];
                else
                    instance = null;
                var xmlCustomTag = PlatformComponents.PlatformComponents.UFUAEditorComponent.GetTagEntityReference(PlatformComponents.PlatformComponents.GetProjectDocument(), name, instance);
                if (!String.IsNullOrEmpty(xmlCustomTag))
                {
                    var tag = xmlCustomTag.FromXml<OPCUAEntityReference>();
                    if (tag != null)
                    {
                        activeSessionsCountCustomVariable = tag;
                        activeSessionsCountCustomVariable.Resolve(PlatformComponents.PlatformComponents.GetSessionString(),
                    PlatformComponents.PlatformComponents.GetProjectDocument());
                        activeSessionsCountCustomVariable.SetInUse(this, true);
                    }
                }
            }
            else
            {
                var serverXML = PlatformComponents.PlatformComponents.UFUAEditorComponent.GetServerEntityReference(PlatformComponents.PlatformComponents.GetProjectDocument(), bCheckEmpty: true);
                if (serverXML != null)
                {
                    activeSessionsCountSystemTagVariable = PlatformComponents.PlatformComponents.GetActiveSessionsWebHMINameNodeId().FromXml<OPCUAEntityReference>();
                    activeSessionsCountSystemTagVariable.Resolve(PlatformComponents.PlatformComponents.GetSessionString(),
                        PlatformComponents.PlatformComponents.GetProjectDocument());
                    activeSessionsCountSystemTagVariable.SetInUse(this, true);
                }
            }
            UpdateActiveSessionCount();
        }

        void OnSessionCountVariableMonitoredItemReady() {
            WriteSessionCountTag(Session.GetSessionCount());
            if (!bDisposed && sessionCountSessionObserver == null)
            {
                var subscription = ActiveSessionsCountVariable.MonitoredItemViewModel.GetSubscriptionViewModelParent();
                if (subscription != null)
                {
                    var session = subscription.GetSessionViewModelParent();
                    if (session != null)
                    {
                        sessionCountSessionObserver = new PropertyObserver<SessionViewModel>(session);
                        sessionCountSessionObserver.RegisterHandler(s => s.Connected, s =>
                        {
                            if (bDisposed)
                                return;

                            if (!s.Connected)
                                bSessionCounterInvalidated = true;
                            else if (bSessionCounterInvalidated && ActiveSessionsCountVariable.MonitoredItemViewModel != null)
                            {
                                bSessionCounterInvalidated = false;
                                WriteSessionCountTag(Session.GetSessionCount());
                            }
                        });
                    }
                }
            }
        }

        public void UpdateActiveSessionCount()
        {
            if (bDisposed)
                return;

            if (ActiveSessionsCountVariable == null)
            {
                Console.Error.WriteLine(String.Format(Properties.Resources.ErrorUpdatingSessionCount, "NullSessionCountVariable"));
                return;
            }

            if (ActiveSessionsCountVariable.MonitoredItemViewModel != null)
                OnSessionCountVariableMonitoredItemReady();
            else if (sessionCountTagObserver == null)
            {
                sessionCountTagObserver = new PropertyObserver<OPCUAEntityReference>(ActiveSessionsCountVariable);
                sessionCountTagObserver.RegisterHandler(n => n.MonitoredItemViewModel, n =>
                {
                    if (bDisposed)
                        return;

                    if (n.MonitoredItemViewModel != null)
                    {
                        sessionCountTagObserver.Dispose();
                        sessionCountTagObserver = null;
                        OnSessionCountVariableMonitoredItemReady();
                    }
                });
            }
        }

        void WriteSessionCountTag(int writingValue)
        {
            try
            {
                ActiveSessionsCountVariable.MonitoredItemViewModel.WriteValue(writingValue);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(String.Format(Properties.Resources.ErrorUpdatingSessionCount, String.Format("{0} - {1} - {2}", ActiveSessionsCountVariable.HumanReadable, writingValue, ex.Message)));
            }
        }

        #region IEntityReference Members

        public ImageSource CollapsedImageSource
        {
            get
            {
                return null;
            }
        }

        public ImageSource ExpandedImageSource
        {
            get
            {
                return null;
            }
        }

        public ContextMenu contextMenu
        {
            get
            {
                return null;
            }
        }

        public object Tooltip
        {
            get
            {
                return null;
            }
        }

        public object ContainedObject
        {
            get
            {
                return null;
            }
        }

        public object EntityParent
        {
            get
            {
                return null;
            }
        }

        public string TypeDefinitionString
        {
            get
            {
                return null;
            }
        }

        #endregion

        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            if (ActiveSessionsCountVariable?.MonitoredItemViewModel != null)
                WriteSessionCountTag(0);

            if (sessionCountTagObserver != null)
            {
                try {
                    sessionCountTagObserver.UnregisterHandler(n => n.MonitoredItemViewModel);
                }
                catch { }
                sessionCountTagObserver.Dispose();
                sessionCountTagObserver = null;
            }
            if (sessionCountSessionObserver != null)
            {
                try {
                    sessionCountSessionObserver.UnregisterHandler(m => m.Connected);
                }
                catch { }
                sessionCountSessionObserver.Dispose();
                sessionCountSessionObserver = null;
            }
            activeSessionsCountSystemTagVariable?.SetInUse(this, false);
            activeSessionsCountCustomVariable?.SetInUse(this, false);
        }
    }
}
