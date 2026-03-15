using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utilities.WPF;
using OPCUABrowser.ComponentService;
using OPCUAViewModelService.ComponentService;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using Utilities;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;
using System.Threading.Tasks;
using WPFUtilities;
using OPCUAViewModel;

namespace OPCUAViewModel.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for AlarmsSourcePropertyEditor.xaml
    /// </summary>
    public partial class AlarmsSourcePropertyEditor : UserControl
    {
        bool bLoaded;
        public AlarmsSourcePropertyEditor()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    var currentStyle = ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");
                    if (!string.IsNullOrEmpty(currentStyle))
                        ThemeHelper.SetTheme(uriLabel, currentStyle);
                }
            };
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            var value = uriButton.Tag as String;
            if (value == null)
                value = String.Empty;

            IUFUAEditorManager editor = null;
            IDocument document = null;
            if (OPCUAViewModelComponent.workspaceService.ContextDocument != null)
            {
                document = OPCUAViewModelComponent.workspaceService.ContextDocument;
                editor = document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            }

            if (editor == null)
                return;

            OPCUAViewModelComponent.workspaceService.IsBusy = true;
            Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                OPCUAViewModelComponent.workspaceService.IsBusy = false;
            });

            var alrbrowser = editor.GetAlarmListControl(document, true);
            if (alrbrowser == null)
                return;

            alrbrowser.ClearValue(FrameworkElement.WidthProperty);
            alrbrowser.ClearValue(FrameworkElement.HeightProperty);
            alrbrowser.DataContext = value;

            GeneralDialogContent wnd = new GeneralDialogContent(alrbrowser)
            {
                DialogKeepContent = true,
                //Title = Properties.Resources.AlarmList,
                HelpLink = "BrowseServerAlarm"
            };
            bool bRet = wnd.ShowDialog() == true;
            if (bRet)
            { 
                uriButton.Tag = alrbrowser.DataContext as String;
                uriButton.GetBindingExpression(Button.TagProperty).UpdateSource();
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            uriButton.Tag = null;
            uriButton.GetBindingExpression(Button.TagProperty).UpdateSource();
            uriLabel.Text = String.Empty;
        }

        private void uriLabel_KeyDown(object sender, KeyEventArgs e)
        {
            FillComboBox(e.Key == Key.F5);
        }

        private void uriLabel_PopupOpening(object sender, DevExpress.Xpf.Editors.OpenPopupEventArgs e)
        {
            FillComboBox();
        }

        bool bFilled;
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
                var ret = new List<String>() { String.Empty };
                var list = editor.GetFlatListAlarmSources(doc, bForceRefresh).OrderBy(x => x);
                foreach (var item in list)
                    ret.Add(item.Replace('/', '\\'));
                return ret;
            });
            task.ContinueWith(ret =>
            {
                uriLabel.ItemsSource = ret.Result;
                progressBar.Visibility = Visibility.Collapsed;
                uriLabel.IsPopupOpen = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
