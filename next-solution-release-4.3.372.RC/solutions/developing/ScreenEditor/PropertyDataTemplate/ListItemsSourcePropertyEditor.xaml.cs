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
using DataReader;
using PropertyControl.ComponentService;
using ScreenManager.ComponentService;
using ScreenManager.Popups;
using ScreenSettings;
using UFInterfaces;
using UFProjectManager.ComponentService;
using Utilities;
using Utilities.WPF;

namespace ScreenManager.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for StartupLogicsPropertyEditor.xaml
    /// </summary>
    public partial class ListItemsSourcePropertyEditor : UserControl
    {
        ScreenDocument doc;
        public ListItemsSourcePropertyEditor()
        {
            InitializeComponent();
            doc = ScreenManagerComponent.screenManagerComponent.Workspace.ContextDocument as ScreenDocument;
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            var value = button.Tag as String;
            var stringEditor = ScreenManagerComponent.screenManagerComponent.StringEditor.GetStringEditor(doc);
            stringEditor.DataContext = value;

            var Dialog = new GeneralDialogContent(stringEditor)
            {
                Owner = this.FindParent<Window>(),
                DialogKeepContent = true,
                Title = Properties.Resources.SelectStringEditor,
                HelpLink = "StringEditor"
            };
            if (Dialog.ShowDialog() != true)
                return;

            button.Tag = stringEditor.DataContext as String;
            uriLabel.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.Tag = null;
            uriLabel.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }
    }
}
