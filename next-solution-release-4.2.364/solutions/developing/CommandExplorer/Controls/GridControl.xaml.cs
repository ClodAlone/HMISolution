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
using UFInterfaces.Commandable;
using System.Collections.ObjectModel;
using Utilities;
using DevExpress.Xpf.Core;
using CommandManager;

namespace CommandExplorer.Controls
{
    /// <summary>
    /// Interaction logic for GridControl.xaml
    /// </summary>
    public partial class GridControl : UserControl, IDisposable
    {
        ObservableCollection<CommandManager.CommandManager> listCommand = new ObservableCollection<CommandManager.CommandManager>();
        ICommandable Commandable;
        CommandEditorUI Container;
        internal Controls.PropertyControl propertyControl;

        public GridControl(CommandEditorUI container, bool isWebHMIProject = false)
        {
            InitializeComponent();

            /*
            var style = ApplicationPropertiesHelper.GetProperty("CurrentSkin") as String;
            switch (style)
            {
                case "Blend": ThemeManager.SetThemeName(this, "MetropolisDark"); break;
                case "VS2010": ThemeManager.SetThemeName(this, "VisualStudio2010"); break;
                case "Office2010Black": ThemeManager.SetThemeName(this, "Office2010Black"); break;
                case "Office2007Silver":
                case "Office2010Silver": ThemeManager.SetThemeName(this, "Office2007Silver"); break;
                case "Office2007Blue":
                case "Office2010Blue": ThemeManager.SetThemeName(this, "Office2007Blue"); break;
                default: ThemeManager.SetThemeName(this, "DevExpressStyle"); break;
            }
            */
            Container = container;

            toolbar.BeginInit();
            var VisibleHMICommands = WebHMIDesignHelper.WebHMIHelper.VisibleHMICommands;
            foreach (var s in CommandManager.CommandManager.LoadCommandTypes())
            {
                if (isWebHMIProject && !VisibleHMICommands.Contains(CommandManager.CommandManager.GetCommandTypeName(s)))
                    continue;
                CommandManager.CommandManager am = CommandManager.CommandManager.CreateFrom(s);

                var btn = new Button();
                btn.Click += addNewCommand_Click;
                btn.Tag = s;

                if (am.Image != null)
                    btn.Content = new Image() { Source = am.Image };
                else
                    btn.Content = String.Format("+ {0}", am.Name);

                btn.ToolTip = String.Format(Properties.Resources.AddNewPrefix, am.Name);
                btn.HorizontalContentAlignment = HorizontalAlignment.Left;
                btn.BorderThickness = new Thickness(0);
                toolbar.Children.Add(btn);
            }
            toolbar.EndInit();

            //gridControl.Model.Options.ListBoxSelectionMode = GridSelectionMode.MultiExtended;
            //gridControl.Model.Options.AllowSelection = GridSelectionFlags.Any & ~GridSelectionFlags.Cell & ~GridSelectionFlags.Column & ~GridSelectionFlags.Table;
            // gridControl.SourceType = typeof(CommandManager.CommandManager);
            gridControl.ItemsSource = listCommand;
            tableView.BestFitColumns();
        }

        public void RefreshList()
        {
            CommandManager.CommandManager am = tableView.FocusedRow as CommandManager.CommandManager;
            am.ForceRefreshCommandSummary();
            gridControl.RefreshData();
        }

        private void tableView_FocusedRowChanged(object sender, DevExpress.Xpf.Grid.FocusedRowHandleChangedEventArgs e)
        {
            UpdateSelection();
        }

        private void tableView_SelectionChanged(object sender, DevExpress.Xpf.Grid.GridSelectionChangedEventArgs e)
        {
            UpdateSelection();
        }

        void UpdateSelection()
        {
            if (propertyControl.contentAnimation.Content is IDisposable)
                (propertyControl.contentAnimation.Content as IDisposable).Dispose();

            if (tableView.FocusedRow != null)
            {
                CommandManager.CommandManager am = tableView.FocusedRow as CommandManager.CommandManager;
                var userControl = am.Editor;
                userControl.DataContext = am;
                userControl.ClearValue(FrameworkElement.WidthProperty);
                userControl.ClearValue(FrameworkElement.HeightProperty);
                propertyControl.contentAnimation.Content = userControl;
            }
            else
                propertyControl.contentAnimation.Content = null;

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                btnDelete.IsEnabled = tableView.FocusedRow != null;
            });
        }

        private void toolbar_LayoutUpdated(object sender, EventArgs e)
        {
            var listWidths = (from c in toolbar.Children.OfType<Button>()
                              orderby c.ActualWidth descending
                              select c.ActualWidth).ToList();
            if (listWidths.Count > 0 && listWidths[0] > 0)
            {
                foreach (Button btn in toolbar.Children)
                    btn.Width = listWidths[0];
            }
        }

        public void PropagateChanges()
        {
            Commandable.CommandList = listCommand;
        }

        private void addNewCommand_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                var btn = sender as Button;
                var type = btn.Tag as String;
                CommandManager.CommandManager am = CommandManager.CommandManager.CreateFrom(type);
                listCommand.Add(am);
                tableView.FocusedRow = am;
            }
        }

        private void control_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            gridControl.ItemsSource = null;
            listCommand.Clear();
            Commandable = DataContext as ICommandable;
            if (Commandable == null)
                return;

            foreach (var v in Commandable.CommandList)
                listCommand.Add(v as CommandManager.CommandManager);

            gridControl.ItemsSource = listCommand;
            if (listCommand.Count > 0)
                tableView.FocusedRow = listCommand.First();
        }

        private void DeleteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btnDelete.IsEnabled = false;
            e.Handled = true;
            if (tableView.FocusedRow != null && listCommand.Contains(tableView.FocusedRow))
                listCommand.Remove(tableView.FocusedRow as CommandManager.CommandManager);

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                btnDelete.IsEnabled = tableView.FocusedRow != null;
            });
        }

        private void DeleteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tableView.FocusedRow != null;
        }

        public void Dispose()
        {
            listCommand.Clear();

            if (propertyControl.contentAnimation.Content is IDisposable)
                (propertyControl.contentAnimation.Content as IDisposable).Dispose();
        }

        private void MoveUpCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = tableView.FocusedRow as CommandManager.CommandManager;
            int index = listCommand.IndexOf(command);
            listCommand.Remove(command);
            listCommand.Insert(index - 1, command);
            tableView.FocusedRow = command;
        }

        private void MoveDownCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = tableView.FocusedRow as CommandManager.CommandManager;
            int index = listCommand.IndexOf(command);
            listCommand.Remove(command);
            listCommand.Insert(index + 1, command);
            tableView.FocusedRow = command;
        }

        private void MoveUpCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tableView.FocusedRow != null && listCommand.Contains(tableView.FocusedRow) && listCommand.IndexOf(tableView.FocusedRow as CommandManager.CommandManager) > 0;
        }

        private void MoveDownCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tableView.FocusedRow != null && listCommand.Contains(tableView.FocusedRow) && listCommand.IndexOf(tableView.FocusedRow as CommandManager.CommandManager) < listCommand.Count - 1;
        }

        private void CopyCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = tableView.FocusedRow as CommandManager.CommandManager;
            var list = new CommandManagerList();
            list.Add(command);
            var str = list.ToXml();
            Clipboard.SetText(str, TextDataFormat.UnicodeText);
        }

        private void PasteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = tableView.FocusedRow as CommandManager.CommandManager;
            int index = listCommand.Count;
            if (command != null && listCommand.Contains(command))
                index = listCommand.IndexOf(command);

            var str = Clipboard.GetText(TextDataFormat.UnicodeText);
            var list = str.FromXml<CommandManagerList>();
            foreach (var cmd in list)
                listCommand.Insert(index, cmd);
            if (list.Count > 0)
                tableView.FocusedRow = list[list.Count - 1];
        }

        private void CopyCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tableView.FocusedRow != null;
        }

        private void PasteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                string str = Clipboard.GetText(TextDataFormat.UnicodeText);
                if (str.Contains("CommandManagerList"))
                    e.CanExecute = str.FromXml<CommandManagerList>() != null;
                else
                    e.CanExecute = false;
            }
            catch (Exception ex)
            {
                e.CanExecute = false;
            }
        }

        private void CopyAllCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var list = new CommandManagerList();
            foreach (var am in listCommand)
                list.Add(am);
            var str = list.ToXml();
            Clipboard.SetText(str, TextDataFormat.UnicodeText);
        }

        private void CopyAllCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = listCommand.Count > 0;
        }
    }
}
