using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ADEditor.ComponentService;
using ADEditor.Controls;
using ADEditor.Document;
using Opc.Ua;
using Utilities;
using Utilities.WPF;

namespace ADEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for PluginNamePropertyEditor.xaml
    /// </summary>
    public partial class PluginNamePropertyEditor : UserControl
    {
        public PluginNamePropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;

            if (ADEditorManagerComponent.adeditorManagerComponent.Workspace != null && 
                ADEditorManagerComponent.adeditorManagerComponent.PropertyControl != null)
            {
                var doc = ADEditorManagerComponent.adeditorManagerComponent.Workspace.ContextDocument as ADEditorDocument;
                var adNotification = ADEditorManagerComponent.adeditorManagerComponent.PropertyControl.SelectObject as ADModel.ADNotification;
                if (doc != null && adNotification != null)
                {
                    AvailablePlugins selectplugin = new AvailablePlugins(doc.GetGeneralSettings());

                    GeneralDialogContent addPluginDialog = new GeneralDialogContent(selectplugin)
                    {
                        Owner = this.FindParent<Window>(),
                        HelpLink = "EditTransport"
                    };
                    if (addPluginDialog.ShowDialog() == true)
                    {
                        var plugindesc = selectplugin.GetSelectedPluginInfo();
                        if (plugindesc != null)
                        {
                            adNotification.PluginName = plugindesc.Name;
                            adNotification.PluginID = plugindesc.NodeId;
                            text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                        }
                    }
                }
            }
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Button button = (Button)sender;

            if (ADEditorManagerComponent.adeditorManagerComponent.PropertyControl != null)
            {
                var adNotification = ADEditorManagerComponent.adeditorManagerComponent.PropertyControl.SelectObject as ADModel.ADNotification;
                if (adNotification != null)
                {
                    adNotification.PluginName = String.Empty;
                    adNotification.PluginID = NodeId.Null;
                }
            }
            
            text.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }
    }
}
