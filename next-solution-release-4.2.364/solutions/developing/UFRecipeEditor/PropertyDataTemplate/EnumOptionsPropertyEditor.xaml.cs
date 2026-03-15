using System;
using System.Collections;
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
using CommonControls;
using Utilities;
using Utilities.WPF;
using UFInterfaces;

namespace UFRecipeEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for EnumOptionsPropertyEditor.xaml
    /// </summary>
    public partial class EnumOptionsPropertyEditor : UserControl
    {
        #region Workspace
        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(EnumOptionsPropertyEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnWorkspaceChanged), new CoerceValueCallback(OnCoerceWorkspace)));

        private static object OnCoerceWorkspace(DependencyObject o, object value)
        {
            EnumOptionsPropertyEditor control = o as EnumOptionsPropertyEditor;
            if (control != null)
                return control.OnCoerceWorkspace((IWorkspace)value);
            else
                return value;
        }

        private static void OnWorkspaceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EnumOptionsPropertyEditor control = o as EnumOptionsPropertyEditor;
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

        public EnumOptionsPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            var enumStrings = button.Tag as String[];
            var listEnum = enumStrings != null ? enumStrings.ToList() : new List<String>();
            var control = new ListEnumStringsEditor(listEnum, Workspace);
            var Dialog = new GeneralDialogContent(control)
            {
                DialogKeepContent = true,
                Title = Properties.Resources.EnumStringsTitle,
                Owner = this.FindParent<Window>(),
                HelpLink = "ListEnumStringsEditor"
            };
            if (Dialog.ShowDialog() == true)
            {
                if (enumStrings != null || control.CurrentEnums.Length > 0)
                {
                    button.Tag = control.CurrentEnums;
                    uriLabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();
                }
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            uriCommand.Tag = null;
            uriLabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();
        }
    }
}
