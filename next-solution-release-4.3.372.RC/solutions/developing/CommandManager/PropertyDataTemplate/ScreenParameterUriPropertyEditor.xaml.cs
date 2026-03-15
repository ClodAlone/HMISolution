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
using CommandManager.ComponentService;
using PropertyControl.ComponentService;
using UFInterfaces;
using UFProjectManager.ComponentService;
using Utilities;
using Utilities.WPF;

namespace CommandManager.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ScreenParameterUriPropertyEditor.xaml
    /// </summary>
    public partial class ScreenParameterUriPropertyEditor : UserControl
    {
        public ScreenParameterUriPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            Uri value = (Uri)(button.Tag);

            var doc = CommandManagerComponent.workspace.ContextDocument;
            if (doc == null)
                return;
            var control = CommandManagerComponent.projectManager.GetResourcePickerUserControl(doc, "ScreenParametersEditor");
            var Dialog = new GeneralDialogContent(control)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "ScreenParametersEditor"
            };
            Dialog.DialogKeepContent = true;
            if (Dialog.ShowDialog() == true)
            {
                var uri = CommandManagerComponent.projectManager.GetResourcePickerUserControlUri(control);
                var urirelative = doc.MakeRelativeUri(uri);
                button.Tag = urirelative;
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            //Button button = (Button)sender;
            uri.Tag = null;
        }
    }
}
