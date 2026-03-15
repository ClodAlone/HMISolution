using MenuSettings.Documents;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UFMenuEditor.PropertyDataTemplate
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
            var doc = ComponentService.MenuEditorManagerComponent.menueditorManagerComponent.Workspace.ContextDocument as UFMenuDocument;
            if (doc == null)
                return;

            UFMenuEditorUI menuEditorView = doc.ActiveView as UFMenuEditorUI;
            if (menuEditorView == null)
                return;
            menuEditorView.ActivateCommandExplorer();
        }
    }
}
