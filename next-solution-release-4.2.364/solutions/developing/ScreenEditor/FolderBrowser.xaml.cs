using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using Utilities;
using System.ComponentModel;
using DevExpress.Xpf.Grid;
using WPFUtilities;
using DevExpress.Xpf.Grid.TreeList;
using DevExpress.Xpf.Bars;

namespace ScreenManager
{
    /// <summary>
    /// Interaction logic for NewScreenType.xaml
    /// </summary>
    public partial class FolderBrowser : UserControl, INotifyPropertyChanged
    {
        public String sourcefileName { get; set; }

        Dictionary<TreeListNode, String> mapCreatedFiles = new Dictionary<TreeListNode, String>();
        TreeListNode nodesRoot;
        string originalNodeContent;

        String _FolderPath = string.Empty;
        public String FolderPath
        {
            get
            {
                return _FolderPath;
            }
            set
            {
                if (_FolderPath != value)
                {
                    _FolderPath = value;
                    OnPropertyChanged("FolderPath");
                }
            }
        }
        String _FolderName = string.Empty;
        public String FolderName
        {
            get
            {
                return _FolderName;
            }
            set
            {
                if (_FolderName != value)
                {
                    _FolderName = value;
                    OnPropertyChanged("FolderName");
                }
            }
        }

        bool isPopup;

        public FolderBrowser(string folder, bool bPopup = false)
        {
            InitializeComponent();
            DataContext = this;
            isPopup = bPopup;

            FolderName = folder;

            using (new WaitCursor())
            {
                String startingPath = ApplicationPropertiesHelper.GetProperty<String>("CommonFolder");
                string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
                var ResourceFileName = String.Format("{0}.{1}\\NewScreenTypes", startingPath, mainversion);
                LoadFolders(ResourceFileName);
            }
        }

        public IEnumerable<String> GetListCategories(String Type, String fileType, bool deepsearch)
        {
            var startingPath = ApplicationPropertiesHelper.GetProperty<String>("CommonFolder");
            string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
            var folder = String.Format("{0}.{2}\\{1}", startingPath, Type, mainversion);

            var search = String.Format("*{0}", fileType);
            return SearchForFolder(folder, search, deepsearch);
        }
        static IEnumerable<String> SearchForFolder(String folder, String search, bool deepsearch)
        {
            if (Directory.Exists(folder))
            {
                var list = Directory.GetDirectories(folder);
                foreach (var f in list)
                {
                    if (deepsearch)
                    {
                        foreach (var found in SearchForFolder(f, search, deepsearch))
                            yield return found;
                        string[] directoryGetFiles = Directory.GetFiles(f, search);
                        if (directoryGetFiles.Length > 0)
                            yield return f;
                    }
                    else
                        yield return f;
                }
            }
        }
        private void LoadTreeViewItems(string startingfolder, TreeListControl treeView, TreeListNode treeViewItem, string lastSelectedType, string lastSelectedSubType)
        {
            var categoriesd = GetListCategories(lastSelectedType, lastSelectedSubType, false).ToList();
            foreach (var category in categoriesd)
            {
                var itemRootHeader = Path.GetFileName(category);
                var itemRoot = treeView.AddNode(new TreeItemControl(itemRootHeader), treeViewItem);

                mapCreatedFiles[itemRoot] = category;

                LoadTreeViewItems(string.Empty, treeView, itemRoot, string.Format("{0}\\{1}", lastSelectedType, itemRootHeader), lastSelectedSubType);

                if (startingfolder.Equals(category))
                    treeView.SelectNode(itemRoot);

            }
        }

        private void ChangeFoldersPath(TreeListNode treeViewItemAdv)
        {
            var newValue = (treeViewItemAdv.Content as TreeItemControl).Header as string;
            var oldValue = originalNodeContent;
            var source = mapCreatedFiles[treeViewItemAdv];
            var destination = mapCreatedFiles[treeViewItemAdv];
            if (mapCreatedFiles.ContainsKey(treeViewItemAdv.ParentNode))
                destination = string.Format("{0}\\{1}", mapCreatedFiles[treeViewItemAdv.ParentNode], newValue);
            try
            {
                Directory.Move(source, destination);
                mapCreatedFiles[treeViewItemAdv] = destination;

                var list = (from p in mapCreatedFiles.Keys
                            where mapCreatedFiles[p].StartsWith(source)
                            select p).ToList();
                foreach (TreeListNode i in (from p in mapCreatedFiles.Keys
                                            where mapCreatedFiles[p].StartsWith(source)
                                            select p).ToList())
                {
                    mapCreatedFiles[i] = mapCreatedFiles[i].Replace(source, destination);
                }
            }
            catch
            {
                try
                {
                    System.IO.Directory.CreateDirectory(destination);
                }
                catch
                {
                    (treeViewItemAdv.Content as TreeItemControl).Header = oldValue;
                }
            }
        }

        void LoadFolders(string Path)
        {
            if (Directory.Exists(Path))
            {
                /********************************************/

                var treeViewAdv = TryFindResource("treeview") as TreeListControl;

                treeViewAdv.Visibility = Visibility.Collapsed;

                nodesRoot = treeViewAdv.AddNode(new TreeItemControl("Screens", null));
                mapCreatedFiles[nodesRoot] = Path;

                using (var cursor = new WaitCursor())
                {
                    LoadTreeViewItems(string.Format("{0}\\{1}", Path, FolderName), treeViewAdv, nodesRoot, "NewScreenTypes", ".xaml");
                }

                if (!System.IO.Directory.Exists(string.Format("{0}\\{1}", Path, FolderName)))
                {
                    //itemRoot.Items.Add(itemFolder);
                    var itemFolder = treeViewAdv.AddNode(new TreeItemControl(FolderName), nodesRoot);
                    mapCreatedFiles[itemFolder] = string.Format("{0}\\{1}", Path, FolderName);
                    treeViewAdv.SelectNode(itemFolder);
                }

                scroller.Content = treeViewAdv;

                treeViewAdv.Visibility = Visibility.Visible;
            }
        }

        private List<IBarManagerControllerAction> InitContextMenu(bool limitedMenu = false)
        {
            List<IBarManagerControllerAction> ContextItems = new List<IBarManagerControllerAction>();

            Image add = new System.Windows.Controls.Image();
            add.Source = ScreenManager.ComponentService.ScreenManagerComponent.GetBitmapImage("AddFolderSmall", true); 
            add.Width = 16;
            add.Height = 16;
            var b1 = new BarButtonItem()
            {
                Content = Properties.Resources.AddNewFolderCommand,
                Command = new DevExpress.Mvvm.DelegateCommand<object>(param => AddFolder()),
                Glyph = add.Source
            };
            ContextItems.Add(b1);

            if (!limitedMenu)
            {
                Image delete = new System.Windows.Controls.Image();
                delete.Source = ScreenManager.ComponentService.ScreenManagerComponent.GetBitmapImage("DeleteSmall", true);
                delete.Width = 16;
                delete.Height = 16;
                var b2 = new BarButtonItem()
                {
                    Content = Properties.Resources.DeleteFolderCommand,
                    Command = new DevExpress.Mvvm.DelegateCommand<object>(param => Delete()),
                    Glyph = delete.Source
                };
                ContextItems.Add(b2);
                Image rename = new System.Windows.Controls.Image();
                rename.Source = ScreenManager.ComponentService.ScreenManagerComponent.GetBitmapImage("RenameSmall", true);
                rename.Width = 16;
                rename.Height = 16;
                var b3 = new BarButtonItem()
                {
                    Content = Properties.Resources.RenameFolderCommand,
                    Command = new DevExpress.Mvvm.DelegateCommand<object>(param => Rename()),
                    Glyph = rename.Source
                };
                ContextItems.Add(b3);
            }

            return ContextItems;
        }
        private void AddFolder()
        {
            var tree = scroller.Content as TreeListControl;
            TreeListNode selectedItem = tree?.GetSelectedNodes().FirstOrDefault();
            if (selectedItem == null || !mapCreatedFiles.ContainsKey(selectedItem))
                return;


            var destPath = string.Format("{0}\\{1}", mapCreatedFiles[selectedItem], "New Folder");
            try
            {
                var i = 0;
                while (Directory.Exists(destPath))
                {
                    destPath = String.Format("{0} {1}", destPath, ++i);
                }
                Directory.CreateDirectory(destPath);
            }
            catch
            {
                return;
            }

            var itemRoot = tree.AddNode(new TreeItemControl(System.IO.Path.GetFileName(destPath)), selectedItem);

            mapCreatedFiles[itemRoot] = destPath;

            selectedItem.IsExpanded = true;
            tree.SelectNode(itemRoot);
        }

        private void Rename()
        {
            var tree = scroller.Content as TreeListControl;
            if (tree == null || tree.View.AllowEditing)
                return;

            var selectedItem = tree.GetSelectedNodes().FirstOrDefault();

            if (selectedItem != null && selectedItem != nodesRoot)
            {
                originalNodeContent = (selectedItem.Content as TreeItemControl).Header as string;
                tree.EditNode(selectedItem);
            }
        }

        private void Delete()
        {
            var tree = scroller.Content as TreeListControl;
            var selectedItem = tree?.GetSelectedNodes().FirstOrDefault();
            if (selectedItem == null)
                return;
            try
            {
                if (mapCreatedFiles.ContainsKey(selectedItem))
                {
                    if (Directory.Exists(mapCreatedFiles[selectedItem]))
                    {
                        //System.IO.Directory.Delete(mapCreatedFiles[(selectedItem as FrameworkElement)]);
                        MessageBoxResult ret = MessageBoxResult.No;

                        DirectoryInfo directoryinfo = new DirectoryInfo(mapCreatedFiles[selectedItem]);
                        if (directoryinfo.GetFiles().Count() > 0 || directoryinfo.GetDirectories().Count() > 0)
                            ret = MessageBox.Show(Properties.Resources.FolderNotEmptyWarning,
                                Properties.Resources.MessageTitle, MessageBoxButton.YesNo, MessageBoxImage.Information);
                        else
                            ret = MessageBox.Show(Properties.Resources.DeleteFolderWarning,
                                Properties.Resources.MessageTitle, MessageBoxButton.YesNo, MessageBoxImage.Information);


                        if (ret == MessageBoxResult.Yes)
                        {
                            Empty(directoryinfo);
                            mapCreatedFiles.Remove(selectedItem);

                            var _parent = selectedItem.ParentNode;
                            _parent.Nodes.Remove(selectedItem);
                            tree.SelectNode(_parent);
                        }
                    }
                }
            }
            catch
            {
            }

        }

        public static void Empty(DirectoryInfo directory)
        {
            try
            {
                foreach (FileInfo file in directory.GetFiles()) file.Delete();
                foreach (DirectoryInfo subDirectory in directory.GetDirectories())
                {
                    Empty(subDirectory);
                    subDirectory.Delete(true);
                }
                directory.Delete(true);
            }
            catch (Exception)
            {

            }
        }

        #region INotifyPropertyChanged
        private void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        private void TreeViewAdv_SelectedItemChanged(object sender, TreeListSelectionChangedEventArgs e)
        {
            var selectedItem = (sender as TreeListControl).GetSelectedNodes().FirstOrDefault();
            if (selectedItem == null)
                return;
            if (mapCreatedFiles.ContainsKey(selectedItem))
            {
                FolderPath = string.Format("{0}\\", mapCreatedFiles[selectedItem]);
                FolderName = System.IO.Path.GetFileName(mapCreatedFiles[selectedItem]);
            }
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var tree = scroller.Content as TreeListControl;
            if (tree == null)
                return;

            if (e.Key == Key.Enter && tree.View.AllowEditing)
            {
                e.Handled = true;
                tree.View.HideEditor();
            }
            else if (!tree.View.AllowEditing)
            {
                if (e.Key == Key.F2)
                {
                    e.Handled = true;
                    Rename();
                }
            }
        }

        void OnHiddenEditor(object sender, TreeListEditorEventArgs e)
        {
            var tree = scroller.Content as TreeListControl;
            if (tree == null)
                return;

            tree.DisableEditing();

           if (mapCreatedFiles.ContainsKey(e.Node))
            {
                e.Handled = true;
                ChangeFoldersPath(e.Node);
            }
        }

        private void OnShowGridMenu(object sender, GridMenuEventArgs e)
        {
            var view = sender as TreeListView;
            if (view == null || view.ActiveEditor != null || !(e.TargetElement is LightweightCellEditor))
                return;

            var isRoot = (view.DataControl as TreeListControl).GetSelectedNodes().FirstOrDefault() == nodesRoot;
            var menuItems = InitContextMenu(isRoot);
            foreach (var menuItem in menuItems)
                e.Customizations.Add(menuItem);
        }
    }
}
