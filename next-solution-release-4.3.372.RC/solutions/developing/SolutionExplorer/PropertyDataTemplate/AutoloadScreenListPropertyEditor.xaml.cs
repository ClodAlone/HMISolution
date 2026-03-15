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
using PropertyControl.ComponentService;
using UFInterfaces;
using UFProjectManager.ComponentService;
using Utilities;
using Utilities.WPF;
using UFProjectManager.Document;

namespace UFProjectManager.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for AutoloadScreenListPropertyEditor.xaml
    /// </summary>
    public partial class AutoloadScreenListPropertyEditor : UserControl
    {
        UFProjectDocument doc;
        public AutoloadScreenListPropertyEditor()
        {
            InitializeComponent();
            doc = UFProjectManagerComponent.projectManagerComponent.Workspace.ContextDocument as UFProjectDocument;
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            var listUri = button.Tag as List<AutoloadScreen>;
            if (listUri != null)
            {
                var control = new ListScreenEditor(listUri, "ScreenManager");
                var Dialog = new GeneralDialogContent(control)
                {
                    DialogKeepContent = true,
                    Title = Properties.Resources.AutoloadScreenListDialogTitle,
                    Owner = this.FindParent<Window>(),
                    HelpLink = "PrechargedScreens"
                };
                if (Dialog.ShowDialog() == true)
                {
                    bool update = (listUri.Count != control.CurrentUri.Length);
                    // I'm used 'ClearStartupScript' and 'AddStartupScript' in order to notify 'OnPropertyChanged' and mark the project as changed.
                    if (doc != null)
                    {
                        doc.ClearAutoloadScreenList();
                        foreach (var uri in control.CurrentUri)
                            doc.AddAutoloadScreen(uri);

                        if (update)
                            uriLabel.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                    }
                }
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var doc = UFProjectManagerComponent.projectManagerComponent.Workspace.ContextDocument as UFProjectDocument;
            if (doc != null)
            {
                doc.ClearAutoloadScreenList();
                uriLabel.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            }
        }
    }
}
