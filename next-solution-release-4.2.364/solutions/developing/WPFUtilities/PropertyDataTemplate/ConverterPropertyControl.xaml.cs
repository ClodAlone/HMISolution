using DocumentManager.ComponentService;
using StringManager.ComponentService;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UFInterfaces;
using UnitConverterManager.ComponentService;

namespace WPFUtilities.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ConverterPropertyControl.xaml
    /// </summary>
    public partial class ConverterPropertyControl : UserControl
    {

        #region Workspace
        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(ConverterPropertyControl), new UIPropertyMetadata(null));
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

        List<string> itemSource = new List<string>() { string.Empty };
        bool bInit = false;
        public ConverterPropertyControl()
        {
            InitializeComponent();
        }

        private void cmbConverters_PopupOpening(object sender, DevExpress.Xpf.Editors.OpenPopupEventArgs e)
        {
            FillConverters();
        }

        void FillConverters()
        {
            itemSource.Clear();
            if (Workspace != null && Workspace.ContextDocument is IDocument)
            {
                IDocument document = Workspace.ContextDocument as IDocument;
                IUnitConverterEditorManager converterEditor = document.GetService(typeof(IUnitConverterEditorManager)) as IUnitConverterEditorManager;
                if(converterEditor != null)
                {
                    var converters = converterEditor.GetListAvailableConverters(document);
                    if (converters != null)
                        itemSource.AddRange(converters);
                }
            }
            cmbConverters.ItemsSource = itemSource;
            bInit = true;
        }

        private void cmbConverters_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (bInit && cmbConverters.SelectedIndex != -1 && itemSource.Count > cmbConverters.SelectedIndex)
                uriButton.Tag = itemSource[cmbConverters.SelectedIndex];
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            uriButton.Tag = string.Empty;
        }
    }
}
