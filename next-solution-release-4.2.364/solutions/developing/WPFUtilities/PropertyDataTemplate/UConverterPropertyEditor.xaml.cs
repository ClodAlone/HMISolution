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
    public partial class UConverterPropertyEditor : UserControl
    {

        #region Workspace
        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(UConverterPropertyEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnWorkspaceChanged), new CoerceValueCallback(OnCoerceWorkspace)));

        private static object OnCoerceWorkspace(DependencyObject o, object value)
        {
            UConverterPropertyEditor control = o as UConverterPropertyEditor;
            if (control != null)
                return control.OnCoerceWorkspace((IWorkspace)value);
            else
                return value;
        }

        private static void OnWorkspaceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UConverterPropertyEditor control = o as UConverterPropertyEditor;
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
        

        public UConverterPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            String value = (String)(button.Tag);
            
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

                var editor = new UConverterEditor(varEditor, value);
                var Dialog = new GeneralDialogContent(editor)
                {
                    Owner = this.FindParent<Window>(),
                    Title = Properties.Resources.UnitConverterEditor,
                    HelpLink = "UnitConverterEditor"
                };

                Dialog.DialogKeepContent = true;
                if (Dialog.ShowDialog() == true)
                {
                    button.Tag = editor.model.Expression;
                }
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            //Button button = (Button)sender;
            uri.Tag = null;
        }
    }
}
