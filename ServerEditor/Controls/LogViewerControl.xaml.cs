using System.Windows.Controls;
using System.Collections.Specialized;
using ServerEditor.ViewModels;

namespace ServerEditor.Controls
{
    public partial class LogViewerControl : UserControl
    {
        public LogViewerControl()
        {
            InitializeComponent();
            this.DataContextChanged += LogViewerControl_DataContextChanged;
        }

        private void LogViewerControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
             if (e.NewValue is LogViewerViewModel newVm)
             {
                 newVm.AllLogs.CollectionChanged += AllLogs_CollectionChanged;
             }
             if (e.OldValue is LogViewerViewModel oldVm)
             {
                 oldVm.AllLogs.CollectionChanged -= AllLogs_CollectionChanged;
             }
        }

        private void AllLogs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
               if (LogListView.Items.Count > 0)
               {
                   LogListView.ScrollIntoView(LogListView.Items[LogListView.Items.Count - 1]);
               }
            }
        }
    }
}