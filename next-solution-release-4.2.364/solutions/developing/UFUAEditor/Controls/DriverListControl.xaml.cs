using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using UFInterfaces.Editors;
using Utilities.WPF;
using Utilities;
using DevExpress.Xpf.Grid;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for DriverListControl.xaml
    /// </summary>
    public partial class DriverListControl : GridControl
    {
        public bool ShowSystemCompatibility { get; set; }
        public bool OpenHelpOnLine { get; set; }
        public DriverListControl()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                win32Col.Visible = win64Col.Visible = linuxCol.Visible = ShowSystemCompatibility;
            };
        }
        
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            int focusedrow = tableView.FocusedRowHandle;
            int listIndex = GetListIndexByRowHandle(focusedrow);
            if (listIndex >= 0)
            {
                string info = string.Empty;
                string friendlyName = string.Empty;
                string name = string.Empty;
                if (SelectedItem is DriverXmlInfo)
                {
                    var driverInfo = (SelectedItem as DriverXmlInfo);
                    if (driverInfo == null)
                        return;
                    info = (SelectedItem as DriverXmlInfo)?.Help;
                    friendlyName = driverInfo.FriendlyName;
                    name = driverInfo.FriendlyName;
                }
                else if(SelectedItem is UFUAModel.UFUACommunicationDriver)
                {
                    var driverInfo = (SelectedItem as UFUAModel.UFUACommunicationDriver);
                    if (driverInfo == null)
                        return;
                    info = (SelectedItem as UFUAModel.UFUACommunicationDriver)?.Help;
                    friendlyName = driverInfo.FriendlyName;
                    name = driverInfo.Name;
                }

                if (OpenHelpOnLine)
                {
                    var helpProvider = ComponentService.UFUAEditorManagerComponent.ufuaEditorManagerComponent.HelpProvider;
                    if(helpProvider != null)
                    {
                        helpProvider.OpenDialogHelpPage(name, true, true);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(info))
                        return;
                    info = info.Replace("\\r\\n", Environment.NewLine);
                    ScrollViewer scrollViewer = new ScrollViewer()
                    {
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                    };

                    TextBlock text = new TextBlock()
                    {
                        FontSize = 14,
                        TextWrapping = TextWrapping.WrapWithOverflow,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextAlignment = TextAlignment.Justify,
                        Text = info,

                    };

                    scrollViewer.Content = text;

                    GeneralDialogContent wnd = new GeneralDialogContent(scrollViewer,
                    WPFUtilities.Properties.Settings.Default.DialogFontSize,
                    WPFUtilities.Properties.Settings.Default.ButtonsWidth,
                    WPFUtilities.Properties.Settings.Default.ButtonsHeight,
                    GeneralDialogButtons.OkButton, new Dictionary<GeneralDialogButtons, String>())
                    {
                        Title = friendlyName,
                        Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                        DialogKeepContent = true,
                        HelpLink = "DriverListInfo"
                    };
                    wnd.ShowDialog();
                }
            }
        }
    }
}
