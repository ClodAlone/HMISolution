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
using UFInterfaces;
using UFProjectManager.ComponentService;
using Utilities;
using Utilities.WPF;
using WPFUtilities.Converters;

namespace UFProjectManager.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ScreenUriPropertyEditor.xaml
    /// </summary>
    public partial class ScreenUriPropertyEditor : UserControl
    {
        public ScreenUriPropertyEditor()
        {
            InitializeComponent();
            UriConverter uriConverter = TryFindResource("UriConverter") as UriConverter;
            if (uriConverter != null)
            {
                uriConverter.document = UFProjectManagerComponent.projectManagerComponent.Workspace.ContextDocument;
                uriConverter.getRenamed = true;
            }
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            Uri value = (Uri)(button.Tag);

            var doc = UFProjectManagerComponent.projectManagerComponent.Workspace.ContextDocument;
            if (doc == null)
                return;

            var control = UFProjectManagerComponent.projectManagerComponent.GetResourcePickerUserControl(doc, "ScreenManager");
            var Dialog = new GeneralDialogContent(control)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "SelectScreen"
            };
            Dialog.DialogKeepContent = true;
            if (Dialog.ShowDialog() == true)
            {
                var uri = UFProjectManagerComponent.projectManagerComponent.GetResourcePickerUserControlUri(control);
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
