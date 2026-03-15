using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UFShortcutSettings.Documents;

namespace UFShortcutEditor.PropertyDataTemplate
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
            var doc = ComponentService.ShortcutEditorManagerComponent.shortcuteditorManagerComponent.Workspace.ContextDocument as UFShortcutDocument;
            if (doc == null)
                return;

            UFShortcutEditorUI shortcutEditorView = doc.ActiveView as UFShortcutEditorUI;
            if (shortcutEditorView == null)
                return;
            shortcutEditorView.ActivateCommandExplorer();
        }
    }
}
