using System.Windows;
using System.Windows.Controls;
using UFInterfaces;
using Utilities;
using Utilities.WPF;

namespace OPCUAViewModel.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ExpressionPropertyEditor.xaml
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
            var value = (string)btnClear.Tag;

            var editor = new ExpressionEditor(value);
            var dialog = new GeneralDialogContent(editor)
            {
                Owner = this.FindParent<Window>(),
                Title = WPFUtilities.Properties.Resources.ExpressionEditor,
                HelpLink = "ExpressionEditor",
                DialogKeepContent = true
            };

            if (dialog.ShowDialog() == true)
            {
                btnClear.Tag = editor.Model.Expression;
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            btnClear.Tag = null;
        }

    }
}
