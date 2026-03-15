using log4net;
using OPCUAViewModel;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using UFInterfaces;
using UFUAEditor.ComponentService;
using Utilities;
using ViewModelLib;

namespace WPFScreenSink
{
    public class ScreenSinkSessionCounter : IDisposable, IEntityReference
    {
        #region Fields
        private static ILog log;
        static OPCUAEntityReference activeSessionsCountCustomVariable;
        static OPCUAEntityReference activeSessionsCountSystemTagVariable;
        static PropertyObserver<OPCUAEntityReference> sessionCountTagObserver;
        static PropertyObserver<SessionViewModel> sessionCountSessionObserver;
        readonly IUFUAEditorManager editorComponent;
        readonly DocumentManager.ComponentService.IDocument projectDocument;
        readonly string sessionName;
        readonly string activeSessionCountVariableName;
        readonly string defaultSessionCountVariableName;
        private readonly Func<long> getSessionCount;
        bool bSessionCounterInvalidated;
        bool bDisposed;
        long SessionCount
        {
            get
            {
                return getSessionCount();
            }
        }
#endregion

        #region Props
        public OPCUAEntityReference ActiveSessionsCountVariable
        {
            get
            {
                return !String.IsNullOrEmpty(activeSessionCountVariableName) ? activeSessionsCountCustomVariable : activeSessionsCountSystemTagVariable;
            }
        }
        #endregion

        #region ctor
        public ScreenSinkSessionCounter(IUFUAEditorManager editorComponent, DocumentManager.ComponentService.IDocument projectDocument, string sessionName, string activeSessionCountVariableName, string defaultSessionCountVariableName, Func<long> sessionCounter, string loggerName = "") {
            this.editorComponent = editorComponent;
            this.projectDocument = projectDocument;
            this.sessionName = sessionName;
            this.activeSessionCountVariableName = activeSessionCountVariableName;
            this.defaultSessionCountVariableName = defaultSessionCountVariableName;
            this.getSessionCount = sessionCounter;

            if (log == null && !String.IsNullOrEmpty(loggerName))
                log = LogManager.GetLogger(loggerName);
            SubscribeActiveSessionsCount();
        }
        #endregion

        string GetActiveSessionsWebClientNameNodeId()
        {
            var opcString = editorComponent.GetNodeIdEntityReference(projectDocument, 
                defaultSessionCountVariableName,
                UFUAServerInfo.Guids.SystemTagsGuid.ToString());
            return opcString;
        }

        void SubscribeActiveSessionsCount()
        {
            if (!String.IsNullOrEmpty(activeSessionCountVariableName))
            {
                var split = activeSessionCountVariableName.Split(':');
                var instance = split[0];
                var name = split[0];
                if (split.Length > 1)
                    name = split[1];
                else
                    instance = null;
                var xmlCustomTag = editorComponent.GetTagEntityReference(projectDocument, name, instance);
                if (!String.IsNullOrEmpty(xmlCustomTag))
                {
                    var tag = xmlCustomTag.FromXml<OPCUAEntityReference>();
                    if (tag != null)
                    {
                        activeSessionsCountCustomVariable = tag;
                        activeSessionsCountCustomVariable.Resolve(sessionName, projectDocument);
                        activeSessionsCountCustomVariable.SetInUse(this, true);
                    }
                }
            }
            else
            {
                var serverXML = editorComponent.GetServerEntityReference(projectDocument, bCheckEmpty: true);
                if (serverXML != null)
                {
                    activeSessionsCountSystemTagVariable = GetActiveSessionsWebClientNameNodeId().FromXml<OPCUAEntityReference>();
                    activeSessionsCountSystemTagVariable.Resolve(sessionName, projectDocument);
                    activeSessionsCountSystemTagVariable.SetInUse(this, true);
                }
            }
        }

        void OnSessionCountVariableMonitoredItemReady()
        {
            WriteSessionCountTag(SessionCount);
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
                                WriteSessionCountTag(SessionCount);
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
                log?.ErrorFormat(Properties.Resources.ErrorUpdatingSessionCount, "NullSessionCountVariable");
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

        public void WriteSessionCountTag(long sessionCount)
        {
            try
            {
                ActiveSessionsCountVariable.MonitoredItemViewModel.WriteValue(sessionCount);
            }
            catch (Exception ex)
            {
                log?.ErrorFormat(Properties.Resources.ErrorUpdatingSessionCount, String.Format("{0} - {1} - {2}", ActiveSessionsCountVariable.HumanReadable, sessionCount, ex.Message));
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
                try
                {
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
