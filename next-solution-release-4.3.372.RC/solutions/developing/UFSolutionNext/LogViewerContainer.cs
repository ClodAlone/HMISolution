using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace UFSolution
{
    internal class LogViewerContainer : ContentControl, IDisposable
    {
        bool bDisposed;
        Object lockObj = new Object();
        internal Log4NetViewer.Log4NetViewer LogViewercontrol;
        WorkSpaceComponent workspace;

        public LogViewerContainer(WorkSpaceComponent ws)
        {
            workspace = ws;
            this.IsVisibleChanged += OnIsVisibleChanged;
        }
        public void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                CreateLogViewer();
                this.IsVisibleChanged -= OnIsVisibleChanged;
            }
        }
        public void CreateLogViewer()
        {
            lock (lockObj)
            {
                if (LogViewercontrol != null)
                    return;

                using (var cursor = new WaitCursor())
                {
                    LogViewercontrol = new Log4NetViewer.Log4NetViewer();

                    workspace.SetDesiredHeightAndWidthInDockedMode(LogViewercontrol, LogViewercontrol.Height, LogViewercontrol.Width);
                    LogViewercontrol.ClearValue(FrameworkElement.WidthProperty);
                    LogViewercontrol.ClearValue(FrameworkElement.HeightProperty);

                    this.Content = LogViewercontrol;
                }
            }
        }
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            this.IsVisibleChanged -= OnIsVisibleChanged;
            LogViewercontrol?.Dispose();
        }
    }
}
