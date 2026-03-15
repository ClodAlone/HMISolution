using DocumentManager.ComponentService;
using StringManager.ComponentService;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UFInterfaces;

namespace WPFUtilities.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for CulturePropertyControlxaml.xaml
    /// </summary>
    public partial class CulturePropertyControlxaml : UserControl
    {

        #region Workspace
        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(CulturePropertyControlxaml), new UIPropertyMetadata(null));
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
        public CulturePropertyControlxaml()
        {
            InitializeComponent();
        }

        private void cmbCultures_PopupOpening(object sender, DevExpress.Xpf.Editors.OpenPopupEventArgs e)
        {
            FillCultures();
        }

        void FillCultures()
        {
            itemSource.Clear();
            if (Workspace != null && Workspace.ContextDocument is IDocument)
            {
                IDocument document = Workspace.ContextDocument as IDocument;
                IStringEditorManager stringEditor = document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                if(stringEditor != null)
                {
                    var cultures = stringEditor.GetListAvailableCultures(document);
                    if (cultures != null)
                        itemSource.AddRange(cultures);
                }
            }
            cmbCultures.ItemsSource = itemSource;
            bInit = true;
        }

        private void cmbCultures_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (bInit && cmbCultures.SelectedIndex != -1 && itemSource.Count > cmbCultures.SelectedIndex)
                uriButton.Tag = itemSource[cmbCultures.SelectedIndex];
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            uriButton.Tag = string.Empty;
        }
    }
}
