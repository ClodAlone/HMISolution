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
using UFInterfaces;
using DocumentManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using HelpProvider.ComponentService;

namespace CommonControls.PropertyDataTemplate
{
    
    /// <summary>
    /// Interaction logic for BitMaskPropertyEditor.xaml
    /// </summary>
    public partial class ConnectionSourcePropertyEditor : UserControl
    {
        #region HelpProvider
        public static readonly DependencyProperty HelpProviderProperty = DependencyProperty.Register("HelpProvider", typeof(IHelpProvider), typeof(ConnectionSourcePropertyEditor), new UIPropertyMetadata(null));
        public IHelpProvider HelpProvider
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IHelpProvider)GetValue(HelpProviderProperty);
            }
            set
            {
                SetValue(HelpProviderProperty, value);
            }
        }

        #endregion
        #region UIMsgBoxAlertService
        public static readonly DependencyProperty UIMsgBoxAlertServiceProperty = DependencyProperty.Register("UIMsgBoxAlertService", typeof(IUIMsgBoxAlertService), typeof(ConnectionSourcePropertyEditor), new UIPropertyMetadata(null));
        public IUIMsgBoxAlertService UIMsgBoxAlertService
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IUIMsgBoxAlertService)GetValue(UIMsgBoxAlertServiceProperty);
            }
            set
            {
                SetValue(UIMsgBoxAlertServiceProperty, value);
            }
        }

        #endregion
        
        #region Workspace

        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace", typeof(IWorkspace), typeof(ConnectionSourcePropertyEditor), new UIPropertyMetadata(null));

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

        public ConnectionSourcePropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var doc = Workspace?.ContextDocument;
            var connectionstring = XpoHelpers.XpoHelper.NormalizeConnectionString(button.Tag as string, doc?.rootBase);
            if (CommonControls.ConnectionWizardDialog.GetConnectionString(ref connectionstring, "ConnectionSourcePropertyEditor", this.FindParent<Window>(), UIMsgBoxAlertService, HelpProvider))
            {
                connectionstring = XpoHelpers.XpoHelper.AddPlaceholderToConnectionString(connectionstring, doc?.rootBase);
                button.Tag = connectionstring;
                BindingOperations.GetMultiBindingExpression(connectionlabel, TextBox.TextProperty).UpdateTarget();
            }
        }

        private void DlgButton_Clear(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            connection.Tag = string.Empty;
            BindingOperations.GetMultiBindingExpression(connectionlabel, TextBox.TextProperty).UpdateTarget();
        }
    }
}
