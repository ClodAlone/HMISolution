using System;
using System.Windows;
using System.Windows.Controls;
using Utilities.WPF;
using System.Collections.Generic;
using System.Threading;
using ScreenManager.ComponentService;
using UFProjectManager.ComponentService;
using System.ComponentModel;
using DeployServer.Processes;
using System.Windows.Threading;
using UIMsgBoxAlertService.ComponentService;

namespace UFProjectManager.Controls
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RemoteProjectsList : UserControl, INotifyPropertyChanged
    {
        #region Declarations
        bool bLoaded;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        
        #region Properties
        List<string> foldersList = new List<string>();
        public List<string> FoldersList
        {
            get
            {
                return foldersList;
            }
            set
            {
                if (value != foldersList)
                {
                    foldersList = value;
                    OnPropertyChanged("FoldersList");
                }
            }
        }
        #endregion

        #region Ctor
        public RemoteProjectsList(List<string> list, string currentProject)
        {
            InitializeComponent();

            DataContext = this;
            
            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    FoldersList = list;
                    if (!String.IsNullOrEmpty(currentProject) && FoldersList.Contains(currentProject))
                        RemoteProjectsGrid.SelectedItem = currentProject;
                }
            };
        }
        #endregion
        #region public Methods
        /// <summary>
        /// Retrieves the selected entity inside the UserControl's grid
        /// </summary>
        /// <returns>The string representing the selected project's folder name</returns>
        public string GetSelectedProject()
        {
            return RemoteProjectsGrid.SelectedItem as string;
        }
        #endregion
        #region private Methods


        void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        void OnRowDoubleClick(object sender, RoutedEventArgs e)
        {
            var dialog = this.FindParent<Window>();
            if (dialog != null)
            {
                dialog.DialogResult = true;
                if (dialog.DialogResult == true)
                    dialog.Close();
            }
        }
        #endregion
    }
}
