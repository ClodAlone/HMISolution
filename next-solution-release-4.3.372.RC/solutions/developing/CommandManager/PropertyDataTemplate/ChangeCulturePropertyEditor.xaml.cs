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
using CommandManager.UserControls;
using PropertyControl.ComponentService;
using StringManager.ComponentService;
using UFInterfaces;
using UFProjectManager.ComponentService;
using Utilities;
using Utilities.WPF;

namespace CommandManager.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ChangeCulturePropertyEditor.xaml
    /// </summary>
    public partial class ChangeCulturePropertyEditor : UserControl
    {
        public ChangeCulturePropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            var doc = CommandManagerComponent.workspace.ContextDocument;
            if (doc == null)
                return;
            if (!CommandManagerComponent.uriRisolverServiceAvailable)
                throw new NotImplementedException("Expecting the missing IUriResolver Interface");
            var listDocumentManagers = CommandManagerComponent.uriRisolver.GetListInstalledDocumentManagers();
            var stringManager = (from c in listDocumentManagers where c.TypeScheme == "StringManager" select c).Single() as IStringEditorManager;

            var control = new ListCultures();
            control.listbox.ItemsSource = stringManager.GetListAvailableCultures(doc);
            var Dialog = new GeneralDialogContent(control)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "ListCultures"
            };
            if (Dialog.ShowDialog() == true)
            {
                button.Tag = control.listbox.SelectedItem as String;
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.Tag = null;
        }
    }
}
