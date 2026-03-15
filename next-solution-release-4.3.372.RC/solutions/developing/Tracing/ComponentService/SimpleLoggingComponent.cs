using System;
using System.Collections.Generic;
using UFInterfaces.CoreHostComponents;
using Tracing.ViewModel;
using System.Windows.Threading;
using UFInterfaces;
using System.Windows.Controls;

namespace Tracing.ComponentService
{
    public class SimpleLoggingComponent : ComponentBase<ISimpleLogging>, ISimpleLogging, IDisposable
    {
        #region Declaration
        Object lockObject = new Object();
        bool bDisposed;
        Dictionary<String, LogItemViewModel> mapLoggers;
        IWorkspace workspace;
        ContentControl emptyControl;
        #endregion

        public LogItemViewModel GetLogItemViewModel(String logItemViewModel)
        {
            LogItemViewModel logItemVM = null;
            lock (lockObject)
            {
                if (bDisposed)
                    return null;

                if (workspace == null)
                    workspace = GetService(typeof(IWorkspace)) as IWorkspace;
                if (workspace == null)
                    throw new NotImplementedException();

                if (mapLoggers == null)
                    mapLoggers = new Dictionary<String, LogItemViewModel>();
                else
                    mapLoggers.TryGetValue(logItemViewModel, out logItemVM);

                if (logItemVM == null)
                {
                    logItemVM = new LogItemViewModel(Dispatcher.CurrentDispatcher, logItemViewModel, null);
                    mapLoggers.Add(logItemViewModel, logItemVM);

                    if (emptyControl == null)
                        emptyControl = new ContentControl();
                    LoggerDefaultUI TraceUserControl = new LoggerDefaultUI { DataContext = logItemVM };
                    emptyControl.Content = TraceUserControl;
                    workspace.AddDockingChildren(emptyControl, logItemVM.LogSource, UFInterfaces.DockState.AutoHidden, UFInterfaces.DockSide.Bottom);
                }
            }

            // workspace.FlashDockedElement(logItemVM.LogSource);

            return logItemVM;
        }

        #region ISimpleLogging Members

        public void AddItem(String source, string message, DateTime timestamp, int severity, Uri uri)
        {
            GetLogItemViewModel(source).AddItem(message, timestamp, severity, uri);
        }

        public void AddItem(String source, string message, DateTime timestamp)
        {
            var LogItemViewModel = GetLogItemViewModel(source);
            if (LogItemViewModel == null)
                return;
            LogItemViewModel.AddItem(message, timestamp);
        }

        public void AddItem(String source, string message)
        {
            var LogItemViewModel = GetLogItemViewModel(source);
            if (LogItemViewModel == null)
                return;
            LogItemViewModel.AddItem(message);
        }
        #endregion

        #region IUFInterfaceBase Members

        public void Initialize()
        {
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            lock (lockObject)
            {
                bDisposed = true;
            }
        }

        #endregion
    }
}
