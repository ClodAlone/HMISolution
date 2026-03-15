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
using DevExpress.Xpo;
using PropertyControl.ComponentService;
using UFInterfaces;
using UFUAEditor.ComponentService;
using Utilities;
using Utilities.WPF;
using UFInterfaces.Editors;

namespace UFUAEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for EnumDynamicSettingsPropertyEditor.xaml
    /// </summary>
    public partial class EnumDynamicSettingsPropertyEditor : UserControl
    {
        #region Workspace
        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(EnumDynamicSettingsPropertyEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnWorkspaceChanged), new CoerceValueCallback(OnCoerceWorkspace)));

        private static object OnCoerceWorkspace(DependencyObject o, object value)
        {
            EnumDynamicSettingsPropertyEditor control = o as EnumDynamicSettingsPropertyEditor;
            if (control != null)
                return control.OnCoerceWorkspace((IWorkspace)value);
            else
                return value;
        }

        private static void OnWorkspaceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EnumDynamicSettingsPropertyEditor control = o as EnumDynamicSettingsPropertyEditor;
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

        #region DynSettingsEditObject
        public static readonly DependencyProperty DynSettingsEditObjectProperty = DependencyProperty.Register("DynSettingsEditObject", typeof(IDynamicSettingsEditing), typeof(EnumDynamicSettingsPropertyEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDynSettingsEditObjectChanged), new CoerceValueCallback(OnCoerceDynSettingsEditObject)));

        private static object OnCoerceDynSettingsEditObject(DependencyObject o, object value)
        {
            EnumDynamicSettingsPropertyEditor control = o as EnumDynamicSettingsPropertyEditor;
            if (control != null)
                return control.OnCoerceDynSettingsEditObject((IDynamicSettingsEditing)value);
            else
                return value;
        }

        private static void OnDynSettingsEditObjectChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EnumDynamicSettingsPropertyEditor control = o as EnumDynamicSettingsPropertyEditor;
            if (control != null)
                control.OnDynSettingsEditObjectChanged((IDynamicSettingsEditing)e.OldValue, (IDynamicSettingsEditing)e.NewValue);
        }

        protected virtual IDynamicSettingsEditing OnCoerceDynSettingsEditObject(IDynamicSettingsEditing value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDynSettingsEditObjectChanged(IDynamicSettingsEditing oldValue, IDynamicSettingsEditing newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public IDynamicSettingsEditing DynSettingsEditObject
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IDynamicSettingsEditing)GetValue(DynSettingsEditObjectProperty);
            }
            set
            {
                SetValue(DynSettingsEditObjectProperty, value);
            }
        }
        #endregion

        #region Constructors
        public EnumDynamicSettingsPropertyEditor()
        {
            InitializeComponent();
        }
        #endregion

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var dynamicSettings = uriLabel.Text;

            var listEnum = new List<string>();
            if (!String.IsNullOrEmpty(dynamicSettings))
                listEnum.AddRange(dynamicSettings.Split(UFUAServerInfo.UFUAServerInfo.GetDynSettingsSeparator()));

            var control = new ListDynamicSettingsEditor(listEnum, Workspace, DynSettingsEditObject);
            var Dialog = new GeneralDialogContent(control)
            {
                DialogKeepContent = true,
                Title = Properties.Resources.EnumDynamicSettingsTitle,
                Owner = this.FindParent<Window>(),
                HelpLink = "EnumDynamicSettingsEditor"
            };
            if (Dialog.ShowDialog() == true)
            {
                string text = null;
                var dynamicstrings = new List<string>();
                control.CurrentEnums.ToList().ForEach((item) => 
                {
                    if (!String.IsNullOrWhiteSpace(item))
                        dynamicstrings.Add(item);
                });

                if (dynamicstrings.Count > 0)
                    text = String.Join(String.Format("{0}", UFUAServerInfo.UFUAServerInfo.GetDynSettingsSeparator()), dynamicstrings);

                uriLabel.Text = text;
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            uriLabel.Text = null;
        }
    }
}
