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
using UFUAEditor.Controls;
using UFUAEditor.Document;
using Utilities;
using Utilities.WPF;
using CommonControls;

namespace UFUAEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for EnumStringsPropertyEditor.xaml
    /// </summary>
    public partial class EnumStringsPropertyEditor : UserControl
    {
        #region Workspace
        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(EnumStringsPropertyEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnWorkspaceChanged), new CoerceValueCallback(OnCoerceWorkspace)));

        private static object OnCoerceWorkspace(DependencyObject o, object value)
        {
            EnumStringsPropertyEditor control = o as EnumStringsPropertyEditor;
            if (control != null)
                return control.OnCoerceWorkspace((IWorkspace)value);
            else
                return value;
        }

        private static void OnWorkspaceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            EnumStringsPropertyEditor control = o as EnumStringsPropertyEditor;
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

        public EnumStringsPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            var enumStrings = button.Tag as XPCollection<UFUAModel.UFUAEnumString>;
            if (enumStrings != null)
            {
                var listEnum = (from c in enumStrings orderby c.Oid select c.Data).ToList();
                var control = new ListEnumStringsEditor(listEnum, Workspace);
                var Dialog = new GeneralDialogContent(control)
                {
                    DialogKeepContent = true,
                    Title = Properties.Resources.EnumStringsTitle,
                    Owner = this.FindParent<Window>(),
                    HelpLink = "EnumStringEditor"
                };
                if (Dialog.ShowDialog() == true)
                {
                    bool update = (listEnum.Count != control.CurrentEnums.Length);

                    var taglist = UFUAEditorManagerComponent.ufuaEditorManagerComponent.GetSelectedObjects<UFUAModel.UFUATag>();
                    foreach (var tagref in taglist)
                    {
                        while (tagref.EnumStrings.Count > 0)
                            tagref.EnumStrings[0].Delete();
                                
                        foreach (var item in control.CurrentEnums)
                            tagref.EnumStrings.Add(new UFUAModel.UFUAEnumString(enumStrings.Session) { Data = item });
                    }

                    if (update)
                        uriLabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();
                }
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var enumStrings = button.Tag as XPCollection<UFUAModel.UFUAEnumString>;

            var taglist = UFUAEditorManagerComponent.ufuaEditorManagerComponent.GetSelectedObjects<UFUAModel.UFUATag>();
            foreach (var tagref in taglist)
            {
                while (tagref.EnumStrings.Count > 0)
                    tagref.EnumStrings[0].Delete();
            }

            uriLabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();
        }
    }
}
