using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ADEditor.Document;
using Utilities.WPF;
using Utilities;
using System.Reflection;
using ADPluginSettingsInterface;
using DevExpress.Xpo;
using WPFUtilities;

namespace ADEditor.Controls
{
    /// <summary>
    /// Interaction logic for PluginList.xaml
    /// </summary>
    public partial class PluginList : UserControl, IDisposable
    {
        #region Declarations
        
        readonly ADEditorDocument Document;
        
        #endregion

        #region Ctor

        public PluginList(ADEditorDocument doc)
        {
            InitializeComponent();
            Document = doc;

            Document.CreateUndoRedoHelper(this);

            Document.CreateUndoRedoHelper(this);

            //gridDataControl.SourceType = typeof(ADModel.ADPlugin);
            //gridDataControl.Model.SuspendRecordUndo();

            gridDataControl.SelectedItemChanged += (s, e) =>
            {
                e.Handled = true;
                UpdateContextObjects();
            };
            FlatGridRefresh();
        }

        #endregion

        #region Plugins 

        internal void OnActivate()
        {
            UpdateContextObjects();
        }

        void UpdateContextObjects()
        {
            if (gridDataControl.SelectedItems.Count == 0)
                Document.EditorManagerComponent.Workspace.ContextObject = null;
            else if (gridDataControl.SelectedItems.Count == 1)
                Document.EditorManagerComponent.Workspace.ContextObject = Document.GetNestedObject(gridDataControl.SelectedItem);
            else
                Document.EditorManagerComponent.Workspace.ContextObjects = Document.GetNestedObjects(gridDataControl.SelectedItems);
        }

        internal void FlatGridRefresh()
        {
            //var list = new List<GridDataGroupColumn>();
            //foreach (var group in gridDataControl.GroupedColumns)
            //    list.Add(new GridDataGroupColumn() { ColumnName = group.ColumnName });

            //while (gridDataControl.GroupedColumns.Count > 0)
            //    gridDataControl.GroupedColumns.Remove(gridDataControl.GroupedColumns[0]);

            var selected = gridDataControl.SelectedItem;
            gridDataControl.ItemsSource = null;
            gridDataControl.ItemsSource = Document.GetPluginCollection();
            gridDataControl.SelectedItem = selected;

            //foreach (var group in list)
            //{
            //    gridDataControl.GroupedColumns.Add(new GridDataGroupColumn() { ColumnName = group.ColumnName });
            //}
        }

        private void gridDataControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            //if (Document.EditorManagerComponent.PropertyControl != null)
            //    Document.EditorManagerComponent.PropertyControl.Activate();
            //else
            EditSelectedItem();
        }

        internal void EditSelectedItem()
        {
            var selected = gridDataControl.SelectedItem as ADModel.ADPlugin;
            if (selected != null)
            {
                ADEditorDocument.EditPluginSettings(selected, Document);
                FlatGridRefresh();
            }
        }

        internal void DeleteSelectedItems()
        {
            var items = new List<ADModel.ADPlugin>();
            foreach (var item in gridDataControl.SelectedItems)
            {
                if (item is ADModel.ADPlugin)
                    items.Add(item as ADModel.ADPlugin);
            }

            var listundo = new List<IXPSimpleObject>();
            foreach (var plugin in items)
            {
                //var driver = gridDataControl.SelectedItem as UFUAModel.UFUACommunicationDriver;
                if (Document.IsPluginNameUsed(plugin.Name))
                {
                    using (new ResetCursor())
                    {
                        var ret = MessageBox.Show(String.Format(Properties.Resources.PluginUsed, plugin.Name),
                                                plugin.Name, MessageBoxButton.YesNo);
                        if (ret == MessageBoxResult.No)
                            continue;
                    }
                }

                listundo.Add(plugin as IXPSimpleObject);
                plugin.Delete();
            }

            if (listundo.Count > 0)
                Document.AddUndoAction(this, listundo, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Removed);

            items.Clear();
            FlatGridRefresh();
        }

        internal void CopySelectedToClipboard()
        {
            var listplugins = new List<ADModel.ADPlugin>();
            foreach (var item in gridDataControl.SelectedItems)
            {
                if (item is ADModel.ADPlugin)
                    listplugins.Add(item as ADModel.ADPlugin);
            }

            Document.CleanClipbaord();
            Document.CopyPluginsToClipbaord(listplugins);
        }

        internal void PasteFromClipboard()
        {
            var listplugins = Document.PasteClipboardPlugins();
            if (listplugins.Count > 0)
            {
                FlatGridRefresh();

                // add list to undo manager
                var listundo = new List<IXPSimpleObject>();
                listplugins.ForEach(item => { listundo.Add(item as IXPSimpleObject); });
                Document.AddUndoAction(this, listundo, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added);
            }
        }

        internal ADModel.ADPlugin GetSelectedItem()
        {
            return gridDataControl.SelectedItem as ADModel.ADPlugin;
        }

        internal bool IsAnyItemSelected()
        {
            return gridDataControl.SelectedItem is ADModel.ADPlugin;
        }

        internal void RunTest()
        {
            var plugin = gridDataControl.SelectedItem as ADModel.ADPlugin;
            if (plugin != null)
            {
                var uidll = ADServerInfo.ADServerInfo.GetPluginUIName(string.Format("{0}{1}", ADServerInfo.ADServerInfo.GetServerFolder(), plugin.AssemblyName));

                IPluginWpfEditing pluginWpfEditing = null;
                try
                {
                    var types = Assembly.LoadFile(uidll).GetTypes();
                    var list = (from t in types.AsParallel()
                                where !t.IsAbstract && typeof(IPluginWpfEditing).IsAssignableFrom(t)
                                select (IPluginWpfEditing)Activator.CreateInstance(t)).ToList();

                    pluginWpfEditing = list[0];
                }
                catch (Exception ex)
                {
                    if (Document.EditorManagerComponent.UIInterface != null)
                        Document.EditorManagerComponent.UIInterface.ShowInformation(String.Format(Properties.Resources.PluginNotFound, uidll));
                }

                if (pluginWpfEditing == null || pluginWpfEditing.GeneralSettingsEditor == null)
                    return;

                var control = pluginWpfEditing.PluginTestEditor;
                control.DataContext = new List<object>() {Document.ConnectionString, plugin};

                GeneralDialogContent Dialog = new GeneralDialogContent(control)
                {
                    Title = plugin.Name,
                    Owner = this.FindParent<Window>(),
                    HelpLink = "PluginEditor"
                };
                Dialog.ShowDialog();
            }
        }

        #endregion

        #region Undo/Redo

        internal bool IsAnyUndoActionAvailable()
        {
            return Document.UndoContainsSomething(this);
        }

        internal bool IsAnyRedoActionAvailable()
        {
            return Document.RedoContainsSomething(this);
        }

        internal void CleanUndoActions()
        {
            Document.CleanUndoActions(this);
        }

        internal void CleanRedoActions()
        {
            Document.CleanRedoActions(this);
        }

        internal void UndoAction()
        {
            XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action;
            var list = Document.UndoAction(this, out action);
            switch (action)
            {
                case XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added:
                    {
                        if (list.Count > 0)
                        {
                            list.ForEach(obj => { obj.Delete(); });
                            FlatGridRefresh();
                        }
                    }
                    break;
                case XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed:
                    {
                        if (list.Count > 0)
                        {
                            FlatGridRefresh();
                        }
                    }
                    break;
                case XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Removed:
                    {
                        if (list.Count > 0)
                        {
                            list.ForEach(obj => { Document.AddExistingObject(obj, null); });
                            FlatGridRefresh();
                        }
                    }
                    break;
            }
        }

        internal void RedoAction()
        {
            XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action;
            var list = Document.RedoAction(this, out action);
            switch (action)
            {
                case XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Removed:
                    {
                        if (list.Count > 0)
                        {
                            list.ForEach(obj => { obj.Delete(); });
                            FlatGridRefresh();
                        }
                    }
                    break;
                case XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Changed:
                    {
                        if (list.Count > 0)
                        {
                            FlatGridRefresh();
                        }
                    }
                    break;
                case XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.Added:
                    {
                        if (list.Count > 0)
                        {
                            list.ForEach(obj => { Document.AddExistingObject(obj, null); });
                            FlatGridRefresh();
                        }
                    }
                    break;
            }
        }

        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            Document.EditorManagerComponent.Workspace.ContextObject = null;

            // gridDataControl.Model.Dispose();
            try
            {
                gridDataControl.Dispose();
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}
