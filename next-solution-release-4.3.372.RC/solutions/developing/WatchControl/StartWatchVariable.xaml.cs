using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DocumentManager.ComponentService;
using System.Windows.Controls;
using UFUAEditor.ComponentService;

namespace WatchControl
{
    public partial class StartWatchVariable : UserControl, IDisposable
    {
        readonly IDocument parent;
        readonly IUFUAEditorManager UFUAEditor;
        UserControl currentControl;
        public ObservableCollection<string> ProjectNames = new ObservableCollection<string>();
        Dictionary<string, UserControl> loadedVariables = new Dictionary<string, UserControl>();

        public StartWatchVariable(IDocument parent)
        {
            if (parent == null)
                return;

            InitializeComponent();
            this.parent = parent;
            UFUAEditor = parent.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (UFUAEditor == null)
                return;

            currentControl = UFUAEditor.GetRuntimeAddressSpaceControl(parent, inExecution: true, multiSelectionAllowed: true);
            if (currentControl == null)
                return;

            currentControl?.ClearValue(FrameworkElement.WidthProperty);
            currentControl?.ClearValue(FrameworkElement.HeightProperty);
            contentControl.Content = currentControl;
            currentControl.DataContextChanged += contentControl_DataContextChanged;

            loadedVariables.Add(parent.Title, currentControl);
            ProjectNames.Add(parent.Title);

            if (parent.Childs != null && parent.Childs.Any())
            {
                cmbChildProjects.Visibility = Visibility.Visible;
                foreach (IDocument child in parent.Childs)
                    ProjectNames.Add(child.Title);

                cmbChildProjects.ItemsSource = ProjectNames;
                cmbChildProjects.SelectedItem = parent.Title;
            }
        }

        private void cmbChildProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedProj = (from proj in parent.Childs where proj.Title == cmbChildProjects.SelectedItem.ToString() select proj).FirstOrDefault();
            if (selectedProj == null)
                selectedProj = parent;

            if (!loadedVariables.ContainsKey(selectedProj.Title))
            {
                currentControl = UFUAEditor.GetRuntimeAddressSpaceControl(selectedProj, inExecution: true, multiSelectionAllowed: true);
                if (currentControl != null)
                    loadedVariables.Add(selectedProj.Title, currentControl);
            }
            else
                currentControl = loadedVariables[selectedProj.Title];
                
            if (currentControl == null)
                return;

            currentControl.DataContextChanged -= contentControl_DataContextChanged;
            contentControl.Content = currentControl;
            currentControl?.ClearValue(FrameworkElement.WidthProperty);
            currentControl?.ClearValue(FrameworkElement.HeightProperty);
            currentControl.DataContextChanged += contentControl_DataContextChanged;
        }

        private void contentControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DataContext = currentControl.DataContext;
        }

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            currentControl.DataContextChanged -= contentControl_DataContextChanged;

            if (currentControl != null && currentControl is IDisposable)
                (currentControl as IDisposable).Dispose();
        }
        #endregion
    }
}

