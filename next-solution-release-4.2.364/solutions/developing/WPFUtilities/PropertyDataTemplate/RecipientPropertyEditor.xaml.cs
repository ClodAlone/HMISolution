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
using DocumentManager.ComponentService;
using UFInterfaces;
using UFUserEditor.ComponentService;
using Utilities;
using Utilities.WPF;

namespace WPFUtilities.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for RecipientPropertyEditor.xaml
    /// </summary>
    public partial class RecipientPropertyEditor : UserControl
    {
        #region Workspace
        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(RecipientPropertyEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnWorkspaceChanged), new CoerceValueCallback(OnCoerceWorkspace)));

        private static object OnCoerceWorkspace(DependencyObject o, object value)
        {
            RecipientPropertyEditor control = o as RecipientPropertyEditor;
            if (control != null)
                return control.OnCoerceWorkspace((IWorkspace)value);
            else
                return value;
        }

        private static void OnWorkspaceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipientPropertyEditor control = o as RecipientPropertyEditor;
            if (control != null)
                control.OnWorkspaceChanged((IWorkspace)e.OldValue, (IWorkspace)e.NewValue);
        }

        protected virtual IWorkspace OnCoerceWorkspace(IWorkspace value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnWorkspaceChanged(IWorkspace oldValue, IWorkspace newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

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
        
        public RecipientPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;

            if (Workspace != null)
            {
                var doc = Workspace.ContextDocument as IDocument;
                if (doc == null)
                    return;

                IUFUserEditorManager userEditorManager = doc.GetService(typeof(IUFUserEditorManager)) as IUFUserEditorManager;
                if (userEditorManager == null)
                    return;

                var rootParent = doc.Parent;
                if (rootParent == null)
                    rootParent = doc;

                var listroles = userEditorManager.GetRoles(rootParent);
                if (listroles == null || listroles.Count() == 0)
                {
                    MessageBox.Show(Properties.Resources.NoUsers);
                    return;
                }

                var seluser = new SelectUser() { DataContext = listroles };

                GeneralDialogContent Dialog = new GeneralDialogContent(seluser)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "RecipeEditor"
                };
                if (Dialog.ShowDialog() == true)
                {
                    var tvm = seluser.treeListControl.GetSelectedNodes().FirstOrDefault();
                    if (tvm != null)
                    {
                        var nodeTag = tvm.Tag;
                        UFUserModel.UFUser us = null;
                        UFUserModel.UFRole el = nodeTag as UFUserModel.UFRole;
                        if (el == null)
                            us = nodeTag as UFUserModel.UFUser;

                        if (el != null || us != null)
                        {
                            button.Tag = string.Format("{0}:{1}", (el != null ? el.Name : string.Format("{0}/{1}",us.UFRoleAss.Name, us.Name)), (el != null ? el.NodeId : us.NodeId));
                            text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                        }
                    }
                }
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;
            button.Tag = null;
        
            text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }
    }
}
