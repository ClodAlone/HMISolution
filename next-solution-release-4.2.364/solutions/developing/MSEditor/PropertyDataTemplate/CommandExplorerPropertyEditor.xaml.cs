using CommandExplorer.ComponentService;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MSEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for CommandExplorerPropertyEditor.xaml
    /// </summary>
    public partial class CommandExplorerPropertyEditor : UserControl
    {
        public CommandExplorerPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var doc = ComponentService.SchedulerEditorManagerComponent.schedulereditorManagerComponent.Workspace.ContextDocument as MSSchedulerSettings.Document.SchedulerEditorDocument;
            if (doc == null)
                return;

            var commandExplorer = doc.GetService(typeof(ICommandExplorer)) as ICommandExplorer;
            if (commandExplorer == null)
                return;

            int indexTab = -1;
            if (button.Tag is String)
            {
                var propertyName = button.Tag as String;
                if (propertyName == "CommandsOn")
                    indexTab = 0;
                else if (propertyName == "CommandsOff")
                    indexTab = 1;
            }
            commandExplorer.Activate(indexTab);
        }
    }
}
