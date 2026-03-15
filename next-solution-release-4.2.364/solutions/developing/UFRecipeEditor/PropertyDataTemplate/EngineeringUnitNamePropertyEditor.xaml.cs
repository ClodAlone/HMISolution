using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;
using System.Threading.Tasks;
using UFInterfaces;

namespace UFRecipeEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for EngineeringUnitNamePropertyEditor.xaml
    /// </summary>
    public partial class EngineeringUnitNamePropertyEditor : UserControl
    {
        #region Workspace
        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(EngineeringUnitNamePropertyEditor), new UIPropertyMetadata(null));

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

        #region Constructors
        public EngineeringUnitNamePropertyEditor()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            uriButton.Tag = null;
            uriButton.GetBindingExpression(Button.TagProperty).UpdateSource();
            uriLabel.Text = String.Empty;
        }

        private void uriLabel_KeyDown(object sender, KeyEventArgs e)
        {
            FillComboBox(e.Key == Key.F5);
        }

        private void uriLabel_PopupOpening(object sender, DevExpress.Xpf.Editors.OpenPopupEventArgs e)
        {
            FillComboBox();
        }

        bool bFilled;
        void FillComboBox(bool bForceRefresh = false)
        {
            if ((bFilled && !bForceRefresh) || Workspace == null)
                return;
            IDocument doc = Workspace.ContextDocument as IDocument;
            if (doc == null)
                return;
            var editor = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (editor == null)
                return;
            bFilled = true;
            progressBar.Visibility = Visibility.Visible;
            var task = Task.Factory.StartNew(() =>
            {
                var ret = new List<String>() { String.Empty };
                return editor.GetEngineeringUnitNames(doc).OrderBy(x => x);
            });
            task.ContinueWith(ret =>
            {
                uriLabel.ItemsSource = ret.Result;
                progressBar.Visibility = Visibility.Collapsed;
                uriLabel.IsPopupOpen = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion
    }
}
