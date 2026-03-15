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

namespace WPFUtilities.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for RecipeUriPropertyEditor.xaml
    /// </summary>
    public partial class RecipeUriPropertyEditor : UserControl
    {
        #region Workspace
        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(RecipeUriPropertyEditor), new UIPropertyMetadata(null));
        public IWorkspace Workspace
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IWorkspace)GetValue(WorkspaceProperty);
            }
            set
            {
                SetValue(WorkspaceProperty, value);
            }
        }

        #endregion

        public RecipeUriPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            Uri value = (Uri)(button.Tag);

            var doc = Workspace?.ContextDocument;
            if (doc == null)
                return;
            var projectManager = doc.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
            var control = projectManager.GetResourcePickerUserControl(doc, "UFRecipeEditor");
            var Dialog = new GeneralDialogContent(control)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "UFRecipeEditor"
            };
            Dialog.DialogKeepContent = true;
            if (Dialog.ShowDialog() == true)
            {
                var uri = projectManager.GetResourcePickerUserControlUri(control);
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
