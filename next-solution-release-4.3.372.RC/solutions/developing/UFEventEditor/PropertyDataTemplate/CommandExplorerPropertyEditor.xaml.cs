using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UFEventEditor.Document;

namespace UFEventEditor.PropertyDataTemplate
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
            var doc = ComponentService.EventEditorManagerComponent.eventeditorManagerComponent.Workspace.ContextDocument as EventEditorDocument;
            if (doc == null)
                return;

            EventEditorControl eventEditorView = doc.ActiveView as EventEditorControl;
            if (eventEditorView == null)
                return;
            eventEditorView.ActivateCommandExplorer();
        }
    }
}
