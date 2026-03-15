using CommandManager.ComponentService;
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
using Utilities;
using Utilities.WPF;

namespace CommandManager.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for CommandsPropertyEditor.xaml
    /// </summary>
    public partial class CommandsPropertyEditor : UserControl
    {
        public CommandsPropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_ClickEdit(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            
            if (CommandManagerComponent.commandExplorer == null)
                return;

            var commands = button.Tag as String;
            CommandManagerList commandList;
            try
            {
                if (!String.IsNullOrEmpty(commands))
                    commandList = commands.FromXml<CommandManagerList>();
                else
                    commandList = new CommandManagerList();
            }
            catch
            {
                commandList = new CommandManagerList();
            }

            var explorerControl = CommandManagerComponent.commandExplorer.control;
            CommandManagerComponent.commandExplorer.SetSync(explorerControl, true);
            explorerControl.ClearValue(FrameworkElement.WidthProperty);
            explorerControl.ClearValue(FrameworkElement.HeightProperty);
            explorerControl.DataContext = new Hepers.CommandsEditObject(commandList);

            var Dialog = new GeneralDialogContent(explorerControl)
            {
                Owner = this.FindParent<Window>(),
                DialogKeepContent = true,
                HelpLink = "CommandsEditor"
            };
            if (Dialog.ShowDialog() == true)
            {
                CommandManagerComponent.commandExplorer.PropagateChanges(explorerControl);

                if (commandList.Count > 0)
                    commands = commandList.ToXml();
                else
                    commands = null;

                button.Tag = commands;
                //txtValue.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            }
        }

        //private void DlgButton_ClickClear(object sender, RoutedEventArgs e)
        //{
        //    Button button = (Button)sender;
        //    button.Tag = null;
        //    txtValue.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        //}
    }
}
