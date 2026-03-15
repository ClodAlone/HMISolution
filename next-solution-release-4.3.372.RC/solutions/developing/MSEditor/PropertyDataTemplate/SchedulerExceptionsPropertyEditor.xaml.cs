using MSEditor.ComponentService;
using MSEditor.Controls;
using MSModel;
using MSSchedulerSettings.Document;
using System.Windows;
using System.Windows.Controls;
using UFInterfaces;
using Utilities;
using Utilities.WPF;

namespace MSEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for SchedulerExceptionsPropertyEditor.xaml
    /// </summary>
    public partial class SchedulerExceptionsPropertyEditor : UserControl
    {
        #region DP
        #region Workspace
        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(SchedulerExceptionsPropertyEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnWorkspaceChanged), new CoerceValueCallback(OnCoerceWorkspace)));

        private static object OnCoerceWorkspace(DependencyObject o, object value)
        {
            SchedulerExceptionsPropertyEditor control = o as SchedulerExceptionsPropertyEditor;
            if (control != null)
                return control.OnCoerceWorkspace((IWorkspace)value);
            else
                return value;
        }

        private static void OnWorkspaceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SchedulerExceptionsPropertyEditor control = o as SchedulerExceptionsPropertyEditor;
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
        #endregion

        #region Ctor
        public SchedulerExceptionsPropertyEditor()
        {
            InitializeComponent();
        }
        #endregion

        private void DlgButton_ClickEdit(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            if (Workspace == null)
                return;

            var document = Workspace.ContextDocument as SchedulerEditorDocument;
            if (document == null)
                return;

            var MSAction = Workspace.ContextObject as MSScheduledAction;
            if (MSAction == null)
                return;

            using (var uow = MSAction.Session.BeginNestedUnitOfWork())
            {
                var newEvControl = new MSSchedulerSettings.Controls.NewEventControl(document, openExceptions: false,bEditing: true, styleName: null, bHideGeneralSettings: true) { DataContext = uow.GetNestedObject(MSAction) };
                GeneralDialogContent Dialog = new GeneralDialogContent(newEvControl)
                {
                    Owner = this.FindParent<Window>(),
                    HelpLink = "EventEditor"
                };
                if (Dialog.ShowDialog() == true)
                    uow.CommitChanges();
            }
        }
    }
}
