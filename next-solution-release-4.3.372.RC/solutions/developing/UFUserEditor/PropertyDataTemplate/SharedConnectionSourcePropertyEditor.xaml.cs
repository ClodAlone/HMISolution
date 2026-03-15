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
using CommonControls;
using PropertyControl.ComponentService;
using Utilities;
using Utilities.WPF;
using UFUserEditor.ComponentService;
using UFUserEditor.Document;
using UIMsgBoxAlertService.ComponentService;
using UFInterfaces;

namespace UFUserEditor.PropertyDataTemplate
{

    /// <summary>
    /// Interaction logic for SharedConnectionSourcePropertyEditor.xaml
    /// </summary>
    public partial class SharedConnectionSourcePropertyEditor : UserControl
    {
        #region Dependency Properties

        #region IsBackupRepository
        public static readonly DependencyProperty IsBackupRepositoryProperty = DependencyProperty.Register("IsBackupRepository", typeof(bool), typeof(SharedConnectionSourcePropertyEditor), new UIPropertyMetadata(false));
        
        public bool IsBackupRepository
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(IsBackupRepositoryProperty);
            }
            set
            {
                SetValue(IsBackupRepositoryProperty, value);
            }
        }
        #endregion
        #endregion

        #region Declarations
        UFUserDocument userDocument;
        string lastConnection;
        #endregion

        #region Constructors

        public SharedConnectionSourcePropertyEditor()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                lastConnection = btnEdit.Tag as String;
                if (UFUserEditorManagerComponent.userEditorManagerComponent.Workspace != null)
                    userDocument = UFUserEditorManagerComponent.userEditorManagerComponent.Workspace.ContextDocument as UFUserDocument;
            };
        }

        #endregion

        #region Methods

        private void DlgButton_ClickEdit(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            var connectionstring = btnEdit.Tag as String;
            if (connectionstring == null)
                connectionstring = string.Empty;

            var wizard = new ConnectionWizard(UFUserEditorManagerComponent.userEditorManagerComponent.UIInterface, UFUserEditorManagerComponent.userEditorManagerComponent.HelpProvider)
            {
                ConnectionString = XpoHelpers.XpoHelper.NormalizeConnectionString(connectionstring, userDocument?.rootBase)
            };
            GeneralDialogContent Dialog = new GeneralDialogContent(wizard)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "ConnectionSourcePropertyEditor"
            };
            if (Dialog.ShowDialog() == true)
            {
                wizard.ConnectionString = XpoHelpers.XpoHelper.AddPlaceholderToConnectionString(wizard.ConnectionString, userDocument?.rootBase);
                btnEdit.Tag = wizard.ConnectionString;
                if (userDocument != null && !UFUserEditorManagerComponent.userEditorManagerComponent.SetNewSharedRepository(userDocument, wizard.ConnectionString, IsBackupRepository))
                {
                    btnEdit.Tag = connectionstring;
                }
                else
                    lastConnection = wizard.ConnectionString;
            }
        }

        private void DlgButton_ClickClear(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            var connectionstring = btnEdit.Tag as String;
            if (lastConnection != connectionstring)
                btnEdit.Tag = lastConnection;
            else
                btnEdit.Tag = null;
            if (userDocument != null &&
                !UFUserEditorManagerComponent.userEditorManagerComponent.SetNewSharedRepository(userDocument, btnEdit.Tag as String, IsBackupRepository))
            {
                btnEdit.Tag = connectionstring;
            }
        }

        private void connectionLabel_LostFocus(object sender, RoutedEventArgs e)
        {
            var newConnectionString = XpoHelpers.XpoHelper.AddPlaceholderToConnectionString(connectionLabel.Text, userDocument?.rootBase);
            if (userDocument != null && lastConnection != newConnectionString &&
                !UFUserEditorManagerComponent.userEditorManagerComponent.SetNewSharedRepository(userDocument, newConnectionString, IsBackupRepository))
            {
                connectionLabel.Text = lastConnection;
            }
            else
                lastConnection = newConnectionString;
        }
        #endregion

        #region Properties
        public IWorkspace Workspace
        {
            get
            {
                return UFUserEditorManagerComponent.userEditorManagerComponent.Workspace;
            }
        }
        #endregion
    }
}
