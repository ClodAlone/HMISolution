using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UFUAEditor.Document;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using Utilities.Xpo.UndoRedo;
using WPFUtilities;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for RedundancySettings.xaml
    /// </summary>
    public partial class RedundancySettings : UserControl
    {
        #region Declarations
        readonly UFUAServerDocument Document;
        readonly ObservableCollection<String> listServers;

        bool bLoaded;
        #endregion

        #region Constructors
        public RedundancySettings(UFUAServerDocument doc)
        {
            InitializeComponent();

            redundancySettingsContent.Foreground = ApplicationPropertiesHelper.GetProperty("CurrentSkinForeColor") as Brush;

            Document = doc;
            DataContext = Document.GetConfiguration();

            Document.CreateUndoRedoHelper();

            if (Document.GetConfiguration().ListRedundancyServers != null)
                listServers = new ObservableCollection<String>(Document.GetConfiguration().ListRedundancyServers.Split(new Char[] { ',' }));
            else
                listServers = new ObservableCollection<String>();

            textEditFullSynchronizationTimeSpan.Mask = String.Format("d '({0})' hh:mm", Properties.Resources.TimeSpanFormatDaysPart);
            textEditStartupTimeout.Mask =
                textEditSynchronizeTimeout.Mask =
                textEditTimeout.Mask = String.Format("d '({0})' hh:mm:ss", Properties.Resources.TimeSpanFormatDaysPart);
            listBox.ItemsSource = listServers;
        }
        #endregion

        #region Undo/Redo

        internal void UndoAction()
        {
            UndoRedoAction action;
            var list = Document.UndoAction(this, out action);
            switch (action)
            {
                case UndoRedoAction.Added:
                    {
                        if (list.Count > 0)
                        {
                            list.Sources.ForEach(obj => { obj.Delete(); });
                            ReloadServerList();
                        }
                    }
                    break;
                case UndoRedoAction.Changed:
                    {
                        if (list.Count > 0)
                        {
                            ReloadServerList();
                        }
                    }
                    break;
                case UndoRedoAction.Removed:
                    {
                        if (list.Count > 0)
                        {
                            ReloadServerList();
                        }
                    }
                    break;
            }
        }

        internal void RedoAction()
        {
            UndoRedoAction action;
            var list = Document.RedoAction(this, out action);
            switch (action)
            {
                case UndoRedoAction.Removed:
                    {
                        if (list.Count > 0)
                        {
                            list.Sources.ForEach(obj => { obj.Delete(); });
                            ReloadServerList();
                        }
                    }
                    break;
                case UndoRedoAction.Changed:
                    {
                        if (list.Count > 0)
                        {
                            ReloadServerList();
                        }
                    }
                    break;
                case UndoRedoAction.Added:
                    {
                        if (list.Count > 0)
                        {
                            ReloadServerList();
                        }
                    }
                    break;
            }
        }

        #endregion

        internal void OnActivate()
        {
            Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(DataContext);
        }

        internal void DeleteSelectedItems()
        {
            listServers.Remove(listBox.SelectedItem as String);
            UpdateServerList();
        }

        internal bool IsAnyItemSelected()
        {
            return listBox.SelectedItem != null;
        }

        void ReloadServerList()
        {
            var list = new List<String>();
            if (Document.GetConfiguration().ListRedundancyServers != null)
                list.AddRange(Document.GetConfiguration().ListRedundancyServers.Split(new Char[] { ',' }));

            listServers.Clear();
            list.ForEach((server) => listServers.Add(server));
        }

        void UpdateServerList()
        {
            var server = new StringBuilder();
            for (int i = 0; i < listServers.Count; ++i)
            {
                if (server.Length > 0)
                    server.Append(",");
                server.Append(listServers[i]);
            }

            Document.AddUndoAction(this, Document.GetConfiguration(), UndoRedoAction.Changed);
            Document.GetConfiguration().ListRedundancyServers = server.ToString();
        }

        private void tile_Click(object sender, EventArgs e)
        {
            var tile = sender as DevExpress.Xpf.LayoutControl.Tile;
            tile.IsMaximized = true;
        }

        private void redundancyServersAdd_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtName.Text))
            {
                txtName.Focus();
                return;
            }
            if (listServers.Contains(txtName.Text))
            {
                listBox.SelectedItem = txtName.Text;
                return;
            }

            listServers.Add(txtName.Text);
            UpdateServerList();
        }

        private void redundancyServersRemove_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                listServers.Remove(listBox.SelectedItem as String);
                UpdateServerList();
            }
        }

        private void redundancyServersMoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                var server = listBox.SelectedItem as String;
                var index = listServers.IndexOf(server);
                if (index > 0)
                {
                    listServers.Move(index, index - 1);
                    UpdateServerList();
                }
            }
        }

        private void redundancyServersMoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                var server = listBox.SelectedItem as String;
                var index = listServers.IndexOf(server);
                if (index < listServers.Count - 1)
                {
                    listServers.Move(index, index + 1);
                    UpdateServerList();
                }
            }
        }

        private void redundancyServersClear_Click(object sender, RoutedEventArgs e)
        {
            if (Document.EditorManagerComponent.UIInterface == null ||
                Document.EditorManagerComponent.UIInterface.ShowYesNo(Properties.Resources.RedundancyClearServersConfirmation, 
                CustomDialogIcons.Question) == CustomDialogResults.Yes)
            {
                listServers.Clear();
                UpdateServerList();
            }
        }

    }
}
