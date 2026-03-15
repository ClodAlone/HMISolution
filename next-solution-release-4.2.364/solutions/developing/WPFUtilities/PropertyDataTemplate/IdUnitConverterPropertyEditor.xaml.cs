using DocumentManager.ComponentService;
using StringManager.ComponentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using UFInterfaces;
using UnitConverterManager.ComponentService;
using Utilities;
using Utilities.WPF;

namespace WPFUtilities.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for IdUnitConverterPropertyEditor.xaml
    /// </summary>
    public partial class IdUnitConverterPropertyEditor : UserControl
    {
        #region Workspace
        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(IdUnitConverterPropertyEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnWorkspaceChanged), new CoerceValueCallback(OnCoerceWorkspace)));

        private static object OnCoerceWorkspace(DependencyObject o, object value)
        {
            IdUnitConverterPropertyEditor control = o as IdUnitConverterPropertyEditor;
            if (control != null)
                return control.OnCoerceWorkspace((IWorkspace)value);
            else
                return value;
        }

        private static void OnWorkspaceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            IdUnitConverterPropertyEditor control = o as IdUnitConverterPropertyEditor;
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

        public IdUnitConverterPropertyEditor()
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

                IUnitConverterEditorManager unitConverterEditorManager = doc.GetService(typeof(IUnitConverterEditorManager)) as IUnitConverterEditorManager;
                if (unitConverterEditorManager == null)
                    return;

                String value;
                value = (String)button.Tag;

                var unitConverterEditor = unitConverterEditorManager.GetUnitConverterEditor(doc);
                unitConverterEditor.DataContext = value;

                var Dialog = new GeneralDialogContent(unitConverterEditor)
                {
                    Owner = this.FindParent<Window>(),
                    DialogKeepContent = true,
                    Title = Properties.Resources.SelectUnitConverter,
                    HelpLink = "SelectUnitConverter"
                };
                if (Dialog.ShowDialog() != true)
                    return;

                if (unitConverterEditor.DataContext != null)
                {
                    button.Tag = unitConverterEditor.DataContext as String;
                    text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                    text.SelectAll();
                }
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            uri.Tag = null;
            text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }
    }
}
