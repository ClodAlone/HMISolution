using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ScreenSettings;

namespace ScreenManager.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for UrlPropertyEditor.xaml
    /// </summary>
    public partial class AnimationExplorerPropertyEditor : UserControl
    {
        public AnimationExplorerPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            var doc = ComponentService.ScreenManagerComponent.screenManagerComponent.Workspace.ContextDocument as ScreenDocument;
            if (doc == null)
                return;

            ScreenEditorView screenEditorView = doc.ActiveView as ScreenEditorView;
            if (screenEditorView == null)
                return;
            screenEditorView.ActivateAnimationExplorer();
        }
    }
}
