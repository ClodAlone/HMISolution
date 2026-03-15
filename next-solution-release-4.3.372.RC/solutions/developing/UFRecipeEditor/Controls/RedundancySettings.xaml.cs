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
using UFRecipeSettings.Documents;
using UFRecipeEditor.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using Utilities.Xpo.UndoRedo;
using WPFUtilities;

namespace UFRecipeEditor.Controls
{
    /// <summary>
    /// Interaction logic for RedundancySettings.xaml
    /// </summary>
    public partial class RedundancySettings : UserControl
    {
        #region Declarations
        readonly RecipeUAServerDocument Document;
        readonly ObservableCollection<String> listServers;

        bool bLoaded;
        #endregion

        #region Constructors
        public RedundancySettings(RecipeUAServerDocument doc)
        {
            InitializeComponent();

            redundancySettingsContent.Foreground = ApplicationPropertiesHelper.GetProperty("CurrentSkinForeColor") as Brush;

            Document = doc;
            DataContext = Document.GetConfiguration();

            Document.CreateUndoRedoHelper();

            listServers = new ObservableCollection<String>();
            ReloadServerList();

            textEditStartupTimeout.Mask =
                textEditSynchronizeTimeout.Mask =
                textEditTimeout.Mask =
                textEditTransactionLogTimeout.Mask = String.Format("d '({0})' hh:mm:ss", Properties.Resources.TimeSpanFormatDaysPart);
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
            ReloadServerList();
            RecipeEditorManagerComponent.recipeEditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(DataContext);
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
            if (Document.GetConfiguration().UseSharedRedundancyServers)
            {
                if (RecipeEditorManagerComponent.recipeEditorManagerComponent.UfuaEditorService != null)
                {
                    var servers = RecipeEditorManagerComponent.recipeEditorManagerComponent.UfuaEditorService.GetServerUriArray(Document);
                    if (servers != null)
                        list.AddRange(servers);
                    Document.GetConfiguration().RecipeRedundancyServerList = string.Join(",", list);
                }
            }
            else if (Document.GetConfiguration().ListRedundancyServers != null)
            {
                list.AddRange(Document.GetConfiguration().ListRedundancyServers.Split(new Char[] { ',' }));
                Document.GetConfiguration().RecipeRedundancyServerList = Document.GetConfiguration().ListRedundancyServers;
            }

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
            Document.GetConfiguration().RecipeRedundancyServerList = server.ToString();
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
            if (RecipeEditorManagerComponent.recipeEditorManagerComponent.UIInterface == null ||
                RecipeEditorManagerComponent.recipeEditorManagerComponent.UIInterface.ShowYesNo(Properties.Resources.RedundancyClearServersConfirmation, 
                CustomDialogIcons.Question) == CustomDialogResults.Yes)
            {
                listServers.Clear();
                UpdateServerList();
            }
        }

        private void redundancyServersUseShared_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            txtName.Text = null;
            ReloadServerList();
        }
    }
}
