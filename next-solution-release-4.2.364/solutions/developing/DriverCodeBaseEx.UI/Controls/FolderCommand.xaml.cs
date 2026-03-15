using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UFUAModel;
using Utilities;
using Utilities.WPF;
using WPFUtilities;
using DriverCodeBaseEx.Extensions;
using OPCUAViewModel;
using OPCUAViewModelService.ComponentService;
using UFUAEditor.ComponentService;
using DocumentManager.ComponentService;
using DevExpress.Xpf.Editors;



namespace DriverCodeBaseEx.UI.Controls
{
    /// <summary>
    /// Interaction logic for FolderCommand.xaml
    /// </summary>
    public partial class FolderCommand : UserControl
    {
        #region Declarations
        bool bLoaded, bFilled, bEditing;
        TagEntityReference original;
        #endregion
        public FolderCommand()
        {
            InitializeComponent();
			
        }
        #region Tag Combobox
        private void uriLabel_PopupOpening(object sender, DevExpress.Xpf.Editors.OpenPopupEventArgs e)
        {
            FillComboBox();
        }

        private void uriLabel_GotFocus(object sender, RoutedEventArgs e)
        {
            uriLabel.SelectAll();
        }

        void FillComboBox(bool bForceRefresh = false)
        {
            if ((bFilled && !bForceRefresh) || !OPCUAViewModelComponent.workspaceServiceAvailable)
                return;
            IDocument doc = OPCUAViewModelComponent.workspaceService.ContextDocument as IDocument;
            if (doc == null)
                return;
            var editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (editor == null)
                return;
            bFilled = true;
            progressBar.Visibility = Visibility.Visible;
            var task = Task.Factory.StartNew(() =>
            {
                return editor.GetFlatFullFolderNameCollectionOrderByName(doc, bForceRefresh);
            });
            task.ContinueWith(ret =>
            {
                uriLabel.ItemsSource = ret.Result;
                progressBar.Visibility = Visibility.Collapsed;
                uriLabel.IsPopupOpen = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void uriLabel_KeyDown(object sender, KeyEventArgs e)
        {
            bEditing = true;

            FillComboBox(e.Key == Key.F5);
        }

        private void uriLabel_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            bEditing = true;
        }
        #endregion
    }
}
