using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ScreenManager.ComponentService;
using Utilities;
using Utilities.WPF;

namespace ScreenManager.Popups
{
    /// <summary>
    /// Interaction logic for _3DInnerScreen.xaml
    /// </summary>
    public partial class _3DInnerScreen : UserControl
    {
        readonly ScreenManagerComponent EditorComponent;

        public _3DInnerScreen(ScreenManagerComponent e)
        {
            InitializeComponent();
            EditorComponent = e;
        }

        Uri ShowScreenSelector(String resourceType = "ScreenManager")
        {
            var doc = EditorComponent.Workspace.ContextDocument;
            if (doc == null)
                return null;
            var control = EditorComponent.ProjectManager.GetResourcePickerUserControl(doc, resourceType);
            var Dialog = new GeneralDialogContent(control)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "ScreenSelector"
            };
            Dialog.DialogKeepContent = true;
            if (Dialog.ShowDialog() == true)
            {
                var uri = EditorComponent.ProjectManager.GetResourcePickerUserControlUri(control);
                var urirelative = doc.MakeRelativeUri(uri);
                return urirelative;
            }

            return null;
        }

        private void Click_BrowseScreen(object sender, RoutedEventArgs e)
        {
            var uri = ShowScreenSelector();
            if (uri != null)
                txtScreen.Text = uri.GetPathString();
        }

        private void Click_BrowseParameter(object sender, RoutedEventArgs e)
        {
            var uri = ShowScreenSelector("ScreenParametersEditor");
            if (uri != null)
                txtParameter.Text = uri.GetPathString();
        }
    }
}
