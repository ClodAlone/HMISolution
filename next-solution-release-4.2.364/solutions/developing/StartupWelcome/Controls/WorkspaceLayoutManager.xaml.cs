using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UIMsgBoxAlertService.ComponentService;
using Utilities.WPF;

namespace StartupWelcome.Controls
{
    /// <summary>
    /// Interaction logic for WorkspaceLayoutManager.xaml
    /// </summary>
    public partial class WorkspaceLayoutManager : UserControl, INotifyPropertyChanged, IDisposable
    {
        #region Properties
        public ObservableCollection<LayoutFile> LayoutsList { get; set; } = new ObservableCollection<LayoutFile>();
        public bool IsValid { get; private set; } = true;
        public LayoutFile SelectedLayout
        {
            get
            {
                var selLayout = gridControl.SelectedItem as LayoutFile;
                if (selLayout != null && selLayout.Name == SavedLayoutName)
                    return gridControl.SelectedItem as LayoutFile;
                if (SavedLayoutName != null)
                    return new LayoutFile() { Name = SavedLayoutName, Path = $"{layoutsFolder}{SavedLayoutName}\\{SavedLayoutName}" };
                return null;
            }
        }
        public String SavedLayoutName
        {
            get
            {
                return savedLayoutName;
            }
            set
            {
                if (SavedLayoutName != value)
                {
                    savedLayoutName = value;
                    OnPropertyChanged("SavedLayoutName");
                }
            }
        }
        #endregion

        #region Declarations
        IUIMsgBoxAlertService UIMsgBoxAlertService { get; }
        Task loadingFilesTask;
        String savedLayoutName;
        public enum LayoutOperation
        {
            Load,
            Save
        };
        public LayoutOperation OperatingMode { get; set; }
        string layoutsFolder = String.Format("{0}\\{1}\\{2}\\",
                                 Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                 Properties.Settings.Default.CompanyName,
                                 Properties.Settings.Default.LayoutApplicationFolder);
        bool bDisposed;

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Ctor
        public WorkspaceLayoutManager(IUIMsgBoxAlertService uiMsgBoxAlertService, LayoutOperation mode = LayoutOperation.Load)
        {
            InitializeComponent();
            UIMsgBoxAlertService = uiMsgBoxAlertService;
            OperatingMode = mode;
            layoutNameDockPanel.Visibility = OperatingMode == LayoutOperation.Load ? Visibility.Collapsed : Visibility.Visible;

            LoadLayoutsList();
        }
        #endregion

        #region Methods
        void LoadLayoutsList()
        {
            if (bDisposed)
                return;

            if (loadingFilesTask == null || loadingFilesTask.IsCanceled || loadingFilesTask.IsCompleted)
            {
                LayoutsList.Clear();
                gridControl.ShowLoadingPanel = true;

                loadingFilesTask = Task.Factory.StartNew(() =>
                {
                    Directory.GetDirectories(layoutsFolder).ToList()
                    .ForEach((folderPath) =>
                    {
                        var dirName = Path.GetFileName(folderPath);
                        LayoutsList.Add(new LayoutFile() { Name = dirName, Path = $"{folderPath}\\{dirName}" });
                    });
                });
                loadingFilesTask.ContinueWith(ret =>
                {
                    if (bDisposed)
                        return;

                    gridControl.ShowLoadingPanel = false;
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
        #endregion

        #region INotifyPropertyChanged
        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            if (loadingFilesTask != null)
            {
                try
                {
                    loadingFilesTask.Wait();
                }
                catch { }
            }
        }
        #endregion

        private void gridControl_SelectionChanged(object sender, DevExpress.Xpf.Grid.GridSelectionChangedEventArgs e)
        {
            if (gridControl.SelectedItem as LayoutFile != null)
                SavedLayoutName = (gridControl.SelectedItem as LayoutFile).Name;
        }

        private void GridControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (gridControl.SelectedItem as LayoutFile == null || e.Key != Key.Delete)
                return;

            var actLayout = gridControl.SelectedItem as LayoutFile;
            var confirm = UIMsgBoxAlertService.ShowYesNoCancel(String.Format(Properties.Resources.LayoutDeleteConfirmation, actLayout.Name), CustomDialogIcons.Question);
            if (confirm == CustomDialogResults.Yes)
            {
                Directory.Delete(Path.GetDirectoryName(actLayout.Path), true);
                LoadLayoutsList();
            }
        }

        private void OnValidating(object sender, DevExpress.Xpf.Editors.ValidationEventArgs e)
        {
            var text = e.Value as string;
            if (text != null && text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                IsValid = e.IsValid = false;
                e.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical;
                e.ErrorContent = Properties.Resources.LayoutInvalidFileName;
            }
            else
                IsValid = true;
        }

        private void OnRowDoubleClick(object sender, DevExpress.Xpf.Grid.RowDoubleClickEventArgs e)
        {
            var dialog = this.FindParent<Window>() as Utilities.GeneralDialog;
            if (dialog != null)
            {
                dialog.DialogResult = true;
                if (dialog.DialogResult == true)
                    dialog.Close();
            }
        }
    }

    public class LayoutFile
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public LayoutFile()
        {

        }
    }
}
