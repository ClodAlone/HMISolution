using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ScreenSettings;

namespace CommandManager.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for CommandSettingsPropertyEditor.xaml
    /// </summary>
    public partial class CommandSettingsPropertyEditor : UserControl
    {
        public CommandSettingsPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click(object sender, RoutedEventArgs e)
        {
            var command = brush.Tag as CommandManager;
            if (command == null)
                return;
            command.EditDataCommandSetting();
        }
    }
}
