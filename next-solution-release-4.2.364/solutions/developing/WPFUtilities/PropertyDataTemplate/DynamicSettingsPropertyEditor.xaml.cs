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
using UFInterfaces.Editors;
using Utilities;
using Utilities.WPF;
using UFInterfaces;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;

namespace WPFUtilities.PropertyDataTemplate
{
    
    /// <summary>
    /// Interaction logic for DynamicSettingsPropertyEditor.xaml
    /// </summary>
    public partial class DynamicSettingsPropertyEditor : UserControl
    {
        #region Workspace
        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(DynamicSettingsPropertyEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnWorkspaceChanged), new CoerceValueCallback(OnCoerceWorkspace)));

        private static object OnCoerceWorkspace(DependencyObject o, object value)
        {
            DynamicSettingsPropertyEditor control = o as DynamicSettingsPropertyEditor;
            if (control != null)
                return control.OnCoerceWorkspace((IWorkspace)value);
            else
                return value;
        }

        private static void OnWorkspaceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DynamicSettingsPropertyEditor control = o as DynamicSettingsPropertyEditor;
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

        public DynamicSettingsPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var settings = button.Tag as string;

            if (Workspace == null)
                return;

            var doc = Workspace.ContextDocument as IDocument;
            var dynamicList = GetDynamicSettingsObjects();
            if (doc != null && dynamicList.Count > 0)
            {
                IUFUAEditorManager ufuaEditorManager = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                if (ufuaEditorManager != null)
                {
                    var oldDynamic = dynamicList[0].DynamicSettingsForEditing;
                    var dynamicSettings = ufuaEditorManager.GetDynamicSettingsControl(doc, dynamicList[0]);
                    if (dynamicSettings == null)
                        return;

                    GeneralDialogContent Dialog = new GeneralDialogContent(dynamicSettings)
                    {
                        Title = Properties.Resources.DynamicSettingsTitle,
                        Owner = this.FindParent<Window>()
                    };

                    var ret = (Dialog.ShowDialog() == true);
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        if (ret)
                        {
                            button.Tag = dynamicList[0].DynamicSettingsForEditing;
                            textBoxDynamic.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                        }
                        else
                            dynamicList[0].DynamicSettingsForEditing = oldDynamic;

                        if (dynamicSettings is IDisposable)
                            (dynamicSettings as IDisposable).Dispose();
                    });
                }
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.Tag = String.Empty;
            textBoxDynamic.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }

        #region Methods
        List<IDynamicSettingsEditing> GetDynamicSettingsObjects()
        {
            var ret = new List<IDynamicSettingsEditing>();
            if (Workspace != null)
            {
                if (Workspace.ContextObject != null && Workspace.ContextObject is IDynamicSettingsEditing)
                {
                    ret.Add(Workspace.ContextObject as IDynamicSettingsEditing);
                }
                else if (Workspace.ContextObjects != null)
                {
                    ret.AddRange(Workspace.ContextObjects.OfType<IDynamicSettingsEditing>());
                }
            }

            return ret;
        }
        #endregion
    }
}
