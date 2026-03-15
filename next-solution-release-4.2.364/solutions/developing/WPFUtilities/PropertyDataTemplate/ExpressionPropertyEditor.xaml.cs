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
using DocumentManager.ComponentService;
using PropertyControl.ComponentService;
using UFInterfaces;
using UFUAEditor.ComponentService;
using Utilities;
using Utilities.WPF;

namespace WPFUtilities.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ScreenUriPropertyEditor.xaml
    /// </summary>
    public partial class ExpressionPropertyEditor : UserControl
    {

        #region Workspace
        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(ExpressionPropertyEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnWorkspaceChanged), new CoerceValueCallback(OnCoerceWorkspace)));

        private static object OnCoerceWorkspace(DependencyObject o, object value)
        {
            ExpressionPropertyEditor control = o as ExpressionPropertyEditor;
            if (control != null)
                return control.OnCoerceWorkspace((IWorkspace)value);
            else
                return value;
        }

        private static void OnWorkspaceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ExpressionPropertyEditor control = o as ExpressionPropertyEditor;
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
        

        public ExpressionPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            String value = (String)(btnClear.Tag);
            
            if (Workspace == null)
                return;

            IDocument Document = Workspace.ContextDocument as IDocument;
            if(Document != null)
            {
                IUFUAEditorManager UFUAEditor = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                // var variables = (from c in UFUAEditor.GetFlatListTags(Document).AsParallel() orderby c select c);
                var varEditor = UFUAEditor.GetRuntimeAddressSpaceControl(Document);
                if (varEditor == null)
                    return;

                var editor = new ExpressonEditor(varEditor, value);
                var Dialog = new GeneralDialogContent(editor)
                {
                    Owner = this.FindParent<Window>(),
                    Title = Properties.Resources.ExpressionEditor,
                    HelpLink = "ExpressionEditor"
                };

                Dialog.DialogKeepContent = true;
                if (Dialog.ShowDialog() == true)
                {
                    btnClear.Tag = editor.model.Expression;
                }
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            btnClear.Tag = null;
        }
    }
}
