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
using VFS;
using UFInterfaces;
using System.IO;
using Utilities;
using Utilities.WPF;
using System.Xml.Linq;
using AmazedSaint.Elastic;
using AmazedSaint.Elastic.Lib;
using System.Dynamic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Controls.Primitives;
using UFProjectManager.ComponentService;
using System.Collections;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using WPFUtilities;
using DevExpress.Xpf.NavBar;
using DevExpress.Xpf.Bars;

namespace SymbolGallery
{
    /// <summary>
    /// Interaction logic for LibraryBrowser.xaml
    /// </summary>
    public partial class LibraryBrowser : UserControl, IDisposable
    {
        readonly BitmapImage openFolderImg;
        readonly BitmapImage closedFolderImg;
        readonly IWorkspace workspace;
        readonly bool isDynamicFolder;
        readonly NavBarControl navigationControl;
        readonly String startingFolder;
		readonly String relativeFolderTag;
        readonly Dictionary<String, String> mapItems = new Dictionary<String, String>();

        FileSystemProviderBase fileSystemProvider;
        bool bLoaded;
        bool bDynamic = false;
        bool bIsEditable;
        internal bool bElastic = false;

        public event EventHandler<LibrarySelectionChangedEventArgs> SelectedChanged;
        public event EventHandler<LibraryDropChangedEventArgs> DropChanged;

        readonly List<string> modelSelectionManageList = new List<string>();
        readonly List<string> modelFolderList = new List<string>();
        readonly Dictionary<string, TreeListNodeCollection> mapModelList = new Dictionary<string, TreeListNodeCollection>();
        readonly Dictionary<string, TreeListNode> mapModelItem = new Dictionary<string, TreeListNode>();
        static List<String> hiddenFolderList;

        public LibraryBrowser(IWorkspace w, FileSystemProviderBase fsp, bool bdf,
                              String sf, String rft = null, bool isEditable = false)
        {
            InitializeComponent();
            workspace = w;
            fileSystemProvider = fsp;
            isDynamicFolder = bdf;
            startingFolder = sf;
            relativeFolderTag = rft;
            bIsEditable = isEditable;

            openFolderImg = SharedResources.Helpers.ResourceManager.GetCommonImage("SymbolGallery", "OpenFolderSmall", true);
            closedFolderImg = SharedResources.Helpers.ResourceManager.GetCommonImage("SymbolGallery", "CloseFolderSmall", true);

            btnCopyLink.IsChecked = bCopyLink;
            btnMergeCode.IsChecked = !bCopyLink;
            //btnProtectKey.Background = NormalBackground;

            // UpdateMergeCode();
            if(!isEditable)
                toolbarTabLibrary.Visibility = Visibility.Collapsed;
        }
        public void UpdateMergeCode()
        {
            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    // using (var cursor = new WaitCursor())
                    {
                        btnMergeCode.IsEnabled = CheckDropLink();
                    }
                });
        }
        public bool CopyLink
        {
            get
            {
                return bCopyLink;
            }
        }

        public String StartingFolder
        {
            get
            {
                return startingFolder;
            }
        }

        public String RelativeFolderTag
        {
            get
            {
                return relativeFolderTag;
            }
        }

        public FileSystemProviderBase FileSystemProvider
        {
            get
            {
                return fileSystemProvider;
            }
            set
            {
                if (fileSystemProvider == value)
                    return;
                fileSystemProvider = value;
            }
        }
        readonly IEnumerable<string> tables = new string[2] { "Styles","Symbols" };
        internal void LoadSymbolGalleryTree()
        {
            if (bLoaded)
                return;
            bLoaded = true;
            if(hiddenFolderList == null)
                hiddenFolderList = LoadHiddenFolderList();

            mapItems.Clear();
            mapModelItem.Clear();
            mapModelList.Clear();
            modelSelectionManageList.Clear();
            modelFolderList.Clear();

            string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
            string startingPath = String.Format("{0}.{1}\\Cultures\\", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"),mainversion);
            string fileToOpen = string.Empty;

            foreach (string key in tables)
            {
                fileToOpen = string.Format("{0}{1}\\StringTable_{2}.xml", startingPath, System.Threading.Thread.CurrentThread.CurrentUICulture.Name, key);

                if (!File.Exists(fileToOpen))
                    fileToOpen = string.Format("{0}StringTable_{1}.xml", startingPath, key);
                if (File.Exists(fileToOpen))
                {
                    LoadFromXml(fileToOpen);
                }
            }


            workspace.StatusText(Properties.Resources.LoadingSymbolLibraries);

            var task1 = Task.Factory.StartNew(() =>
            {
                var ret1 = GetDynamic();
                bDynamic = String.IsNullOrEmpty(ret1.Key);
            });
            task1.ContinueWith((t) =>
            {
                Debug.WriteLine("I have observed a {0}",
                    t.Exception.InnerException.GetType().Name);
            }, TaskContinuationOptions.OnlyOnFaulted);

            treeListView.Nodes.Clear();

            var ic = new TreeItemControl(Properties.Resources.Library);
            var newitem = treeListControl.AddNode(ic, null, startingFolder);
            treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);
            mapModelItem.Add(startingFolder, newitem);
            UpdateFolderIcon(newitem, false);
            treeListControl.SelectNode(newitem);
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                {
                    newitem.IsExpanded = true;
                });
        }
               
        void UpdateFolderIcon(TreeListNode node, bool isOpen)
        {
            (node.Content as TreeItemControl).ResourceIcon = isOpen ? openFolderImg : closedFolderImg;
        }

        void ClearNodes(TreeListNode node)
        {
            node.Nodes.Clear();
        }

        void OnTreeNodeCollapsing(object sender, TreeListNodeAllowEventArgs e)
        {
            if (!(e.Node.Content as TreeItemControl).IsNodeExpanding)
                UpdateFolderIcon(e.Node, false);
        }

        void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            TreeListNode node = treeListControl.View.GetNodeByRowHandle(e.HitInfo.RowHandle);
            if (node == null)
                return;

            treeListControl.SelectNode(node);
            node.IsExpanded = !node.IsExpanded;
        }

        bool NeedToBeExpanded(TreeListNode item)
        {
            int i = 0;
            if (Properties.Settings.Default.FolderAlwaysExpandible)
                return true;

            if (fileSystemProvider != null)
            {
                if (fileSystemProvider.Exists(new FileManagerFolder(fileSystemProvider, item.Tag.ToString())))
                {
                    return fileSystemProvider.GetFolders(new FileManagerFolder(fileSystemProvider, item.Tag.ToString())).Any();
                }
                return false;
            }
            else
            {
                foreach (var v in Directory.GetDirectories(item.Tag?.ToString()))
                {
                    if (hiddenFolderList.Contains(v.ToUpper()))
                        i++;
                }

            if (i == Directory.GetDirectories(item.Tag?.ToString()).Count()) //if all subfolders are hidden, folder is not expandible
                return false;

                return Directory.GetDirectories(item.Tag?.ToString()).Any();
            }
        }
        void OnTreeNodeExpanding(object sender, TreeListNodeAllowEventArgs e)
        {
            if (e == null)
                return;

            TreeListNode item = e.Node;

            if (item == null || TreeListControlHelper.WasExpanded(item)) //Node's subtree already populated
                return;

            (item.Content as TreeItemControl).IsNodeExpanding = true;
            treeListControl.BeginDataUpdate();

            try
            {
                ClearDummyNode(item);
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }

            try
            {
                if (item.Nodes.Count == 0)
                {
                    treeViewFolder_LoadOnDemand(item);
                    mapModelList[item.Tag.ToString()] = item.Nodes;
                }
                e.Handled = true;

            }
            finally
            {
                (item.Content as TreeItemControl).IsNodeExpanding = false;
            }            
        }
        void OnTreeNodeExpanded(object sender, TreeListNodeEventArgs e)
        {
            var item = e.Node as TreeListNode;
            if (item != null)
                UpdateFolderIcon(item, item.IsExpanded);
        }

        void ClearDummyNode(TreeListNode node)
        {
            List<TreeListNode> dummyNodes = (from n in node.Nodes where n.Tag == TreeListControlHelper.DummyNode select n).ToList();
            dummyNodes.ForEach(dnode => { node.Nodes.Remove(dnode); });
        }

        private List<string> LoadHiddenFolderList()
        {
            try
            {
                var doc = XElement.Load(SymbolGalleryUI.GetHiddenPluginFileName());
                var plugin = (from item in doc.Descendants("Plugin")
                              where item.HasAttributes && item.Attribute("FolderPath") != null
                              select
                              String.Format("{0}\\{1}", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), item.Attribute("FolderPath").Value).ToUpper()
                              ).ToList<string>();
                return plugin;
            }
            catch (Exception ex)
            {
                return new List<String>();
            }
        }

        private TreeListNode AddTreeItem(String item, bool bFolder = true)
        {
            if (string.IsNullOrEmpty(item) || mapModelItem.ContainsKey(item))
                return mapModelItem[item];
            var directory = new DirectoryInfo(item);
            var _HeaderKey = !bFolder ? System.IO.Path.GetFileNameWithoutExtension(item) : directory.Name;
            TreeListNode parent = null;
            TreeListNode subitem = null;

            if (fileSystemProvider != null)
            {
                mapModelItem.TryGetValue(item.Replace($"\\{directory.Name}", ""), out parent);
            }
            else
            {
                mapModelItem.TryGetValue(directory.Parent.FullName, out parent);
            }

            if (parent != null && !parent.IsExpanded && !(parent.Content as TreeItemControl).IsNodeExpanding)
            {
                parent.IsExpanded = true;
                var list = (from p in parent.Nodes where p.Tag.ToString() == item select p).ToList();
                if (list.Count > 0)
                    return list[0];
            }

            if (mapItems.ContainsKey(_HeaderKey))
                _HeaderKey = mapItems[_HeaderKey];

            treeListControl.BeginDataUpdate();
            try
            {
                var ic = new TreeItemControl(item, _HeaderKey);
                subitem = treeListControl.AddNode(ic, parent, item);

                if (NeedToBeExpanded(subitem))
                    treeListControl.AddNode(null, subitem, TreeListControlHelper.DummyNode);

                mapModelList.Add(item, subitem.Nodes);
                mapModelItem.Add(item, subitem);
                if(bFolder)
                    modelFolderList.Add(item);
                UpdateFolderIcon(subitem, false);
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }

            if (fileSystemProvider != null)
            {
                var task1 = Task.Factory.StartNew(() =>
                {
                    var folder = new FileManagerFolder(fileSystemProvider, item);
                    if (fileSystemProvider.Exists(folder))
                    {
                        var folders = fileSystemProvider.GetFolders(folder).ToList();
                        if (folders.Count > 0)
                            return true;

                        if (isDynamicFolder)
                        {
                            var files = fileSystemProvider.GetFiles(folder).ToList();
                            if (files.Count > 0)
                                return true;
                        }
                    }
                   
                    return false;
                });
                //var task2 = task1.ContinueWith(ret =>
                //{
                //    //if(subitem != null)
                //    //    subitem.IsLoadOnDemand = ret.Result;
                //}, TaskScheduler.FromCurrentSynchronizationContext());
                //task1.ContinueWith((t) =>
                //{
                //    Debug.WriteLine("I have observed a {0}",
                //        t.Exception.InnerException.GetType().Name);
                //}, TaskContinuationOptions.OnlyOnFaulted);
                //task2.ContinueWith((t) =>
                //{
                //    Debug.WriteLine("I have observed a {0}",
                //        t.Exception.InnerException.GetType().Name);
                //}, TaskContinuationOptions.OnlyOnFaulted);
            }
            else
            {
                var task1 = Task.Factory.StartNew(() =>
                {
                    if (isDynamicFolder)
                    {
                        if (bFolder && (Directory.GetDirectories(item).Length > 0 ||
                                        Directory.GetFiles(item, SymbolGalleryUI.searchPatternXaml).Length > 0))
                            return true;
                    }
                    else
                    {
                        if (bFolder && Directory.GetDirectories(item).Length > 0)
                            return true;
                    }

                    return false;
                });
                //var task2 = task1.ContinueWith(ret =>
                //{
                //    //if (subitem != null)
                //    //    subitem.IsLoadOnDemand = ret.Result;
                //}, TaskScheduler.FromCurrentSynchronizationContext());
            }

            if (isDynamicFolder)
            {
                if (!bFolder)
                    if (subitem != null)
                        modelSelectionManageList.Add(item);
            }
            else
            {
                modelSelectionManageList.Add(item);
            }

            return subitem;
        }

        void treeListControl_SelectionChanged(object sender, TreeListSelectionChangedEventArgs e)
        {
            e.Handled = true;
            UpdateSelection();
        }
        internal void UpdateSelection()
        { 
            TreeListNode item = treeListControl.GetSelectedNodes().FirstOrDefault() as TreeListNode;

            if (item == null || item.Tag == null || !(item.Tag is String))
            {
                return;
            }

            if(!modelSelectionManageList.Contains(item.Tag.ToString()))
                return;

            var filePath = item.Tag as String;
            var temp = SelectedChanged;
            if (temp != null)
                temp(this, new LibrarySelectionChangedEventArgs(filePath, isDynamicFolder, fileSystemProvider));
        }

        String GetFileIndex()
        {
            return String.Format("{0}\\index.dat", startingFolder);
        }

        static readonly String sz = "1r8atLoiwsEZ4Uym9rYHrw==";
        dynamic GetDynamic()
        {
            var t1 = String.Empty;
            var file = GetFileIndex();
            if (fileSystemProvider != null)
            {
                var fileManagerFile = new FileManagerFile(fileSystemProvider, file);
                if (!fileSystemProvider.Exists(fileManagerFile))
                    try
                    {
                        if (!bIsEditable)
                            SaveIndexFile(WPFUtilities.CryptString.CryptString.EncryptString(sz), false, true);
                        else
                            SaveIndexFile("", false, false);
                    }
                    catch (Exception ex)
                    {
                        
                    }

                if (!fileSystemProvider.Exists(fileManagerFile))
                {
                    var xamlData = fileSystemProvider.ReadFile(fileManagerFile);
                    //t1 = System.Text.Encoding.Unicode.GetString(xamlData);
                    //GIULIA
                    t1 = WPFUtilities.CryptString.CryptString.DecryptString(System.Text.Encoding.Unicode.GetString(xamlData));
                }
            }
            else
            {
                if (!File.Exists(file))
                    try
                    {
                        if (!bIsEditable)
                            SaveIndexFile(WPFUtilities.CryptString.CryptString.EncryptString(sz), false, true);
                        else
                            SaveIndexFile("", false, false);
                    }
                    catch (Exception ex)
                    {
                        
                    }

                if (File.Exists(file))
                {
                    try
                    {
                        File.SetAttributes(GetFileIndex(), FileAttributes.Hidden | FileAttributes.System);
                    }
                    catch (Exception ex)
                    {
                    }

                    t1 = File.ReadAllText(file);
                }
            }

            try
            {
                var ret = XElement.Parse(WPFUtilities.CryptString.CryptString.DecryptString(t1)).ToElastic();
                ValidateIndexFile(ret);
                return ret;
            }
            catch (Exception ex)
            {
                var ret = new ExpandoObject();
                ValidateIndexFile(ret);
                return ret;
            }
        }

        static public void ValidateIndexFile(dynamic var)
        {
            try
            {
                var.IsSystemKey = var.IsSystemKey;
            }
            catch (Exception ex)
            {
                var.IsSystemKey = false;
            }
            try
            {
                var.Key = var.Key;
            }
            catch (Exception ex)
            {
                var.Key = String.Empty;
            }
        }

        void SaveIndexFile(String key, bool bCheck = true, bool bIsSystemKey = false)
        {
            if (bCheck)
            {
                var elastic = GetDynamic();
                if (elastic.IsSystemKey is String && elastic.IsSystemKey == "true" ||
                    elastic.IsSystemKey is bool && elastic.IsSystemKey)
                    return;
            }
            dynamic store = new ElasticObject("data");
            ValidateIndexFile(store);
            store.IsSystemKey = bIsSystemKey;
            store.Key = key;
            var el = store > FormatType.Xml;
            var sel = el.ToString() as String;
            var toWrite = WPFUtilities.CryptString.CryptString.EncryptString(sel);
            if (fileSystemProvider != null)
            {
                fileSystemProvider.UploadFile(null, GetFileIndex(),
                    System.Text.Encoding.Unicode.GetBytes(toWrite));
            }
            else
            {
                try
                {
                    File.SetAttributes(GetFileIndex(), FileAttributes.Normal);
                }
                catch (Exception ex)
                {
                }
                File.WriteAllText(GetFileIndex(), toWrite);
                try
                {
                    File.SetAttributes(GetFileIndex(), FileAttributes.Hidden | FileAttributes.System);
                }
                catch (Exception ex)
                {                   
                }
            }
        }

        internal bool ProtectLibrary()
        {
            var elastic = GetDynamic();
            var bCan = CheckDropLink();

          #region SystemKey
            if (elastic.IsSystemKey is String && elastic.IsSystemKey == "true" ||
                elastic.IsSystemKey is bool && elastic.IsSystemKey)
            {
                #region bCan
                if (bCan)
                {
                    var keyControl = new Controls.KeyControlMessage();
                    keyControl.textBox.Text = Properties.Resources.SystemLibraryIsNotProtected;
                    var newDialog = new GeneralDialogContent(keyControl)
                    {
                        Owner = this.FindParent<Window>(),
                        Title = Properties.Resources.ProtectKeyTitle,
                        HelpLink = "ProtectLibrary"
                    };

                    if (newDialog.ShowDialog() == true)
                    {
                        var temp = DropChanged;
                        var messageControl = new Controls.KeyControlMessage();
                        messageControl.textBox.Text = Properties.Resources.SystemActivateProtection;
                        var newMessageDialog = new GeneralDialogContent(messageControl)
                        {
                            Owner = this.FindParent<Window>(),
                            Title = Properties.Resources.ProtectKeyTitle,
                            HelpLink = "ProtectLibrary"
                        };

                        if (newMessageDialog.ShowDialog() == true)
                        {
                            bElastic = false;
                            bCopyLink = true;
                            btnCopyLink.IsChecked = bCopyLink;
                            btnMergeCode.IsChecked = !bCopyLink;
                            SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.LibraryNowIsProtected);
                            if (temp != null)
                                temp(this, new LibraryDropChangedEventArgs(this, bCopyLink));
                            return false;
                        }
                        else
                        {
                            return CheckDropLink();
                        }
                    }
                    return CheckDropLink();
                }
                #endregion
                #region !bCan
                else
                {
                    var keyControl = new Controls.EnterKey();
                    keyControl.textBox.Text = Properties.Resources.SystemLibraryIsProtected;

                    var newDialog = new GeneralDialogContent(keyControl)
                    {
                        Owner = this.FindParent<Window>(),
                        Title = Properties.Resources.EnterSystemLicenseKey,
                        HelpLink = "ProtectLibrary"
                    };

                    if (newDialog.ShowDialog() == true)
                    {
                        var temp = DropChanged;
                        if (keyControl.keyBox.Password.Length > 0 &&
                            keyControl.keyBox.Password == elastic.Key)
                        {
                            var messageControl = new Controls.ControlMessage();
                            messageControl.textBox.Text = Properties.Resources.Disclaimer;
                            var captions = GeneralDialogContent.GetDefaultButtonCaptions();
                            captions[GeneralDialogButtons.OkButton] = Properties.Resources.LabelOk;
                            captions[GeneralDialogButtons.CancelButton] = Properties.Resources.LabelCancel;
                            var newDialog1 = new GeneralDialogContent(messageControl, GeneralDialogButtons.OkCancelHelpButtons, captions)
                            {
                                Owner = this.FindParent<Window>(),
                                Title = Properties.Resources.DisclaimerTitle,
                                HelpLink = "ProtectLibrary"
                            };
                            if (newDialog1.ShowDialog() == true)
                            {
                                bElastic = true;
                                bCopyLink = true;
                                btnCopyLink.IsChecked = bCopyLink;
                                btnMergeCode.IsChecked = !bCopyLink;
                                SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.LibraryNowIsNotProtected);
                                if (temp != null)
                                    temp(this, new LibraryDropChangedEventArgs(this, bCopyLink));
                                return true;

                            }
                        }
                        else
                        {
                            bElastic = false;
                            bCopyLink = true;
                            btnCopyLink.IsChecked = bCopyLink;
                            btnMergeCode.IsChecked = !bCopyLink;
                            SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.WrongPassword, true);
                            if (temp != null)
                                temp(this, new LibraryDropChangedEventArgs(this, bCopyLink));
                            return false;
                        }
                    }
                    bElastic = false;
                    return CheckDropLink();
                }
                #endregion
                return CheckDropLink();
            }
          #endregion

          #region userkey
            #region bCan
            if (bCan)
            {
                var keyControl = new Controls.KeyControlMessage();
                keyControl.textBox.Text = Properties.Resources.LibraryIsNotProtected;
                var newDialog = new GeneralDialogContent(keyControl)
                {
                    Owner = this.FindParent<Window>(),
                    Title = Properties.Resources.ProtectKeyTitle,
                    HelpLink = "ProtectLibrary"
                };

                if (newDialog.ShowDialog() == true)
                {
                    var newkeyControl = new Controls.NewKey();
                    newkeyControl.Title.Text = Properties.Resources.CreatePassword;

                    var newkeyDialog = new GeneralDialogContent(newkeyControl)
                    {
                        Owner = this.FindParent<Window>(),
                        Title = Properties.Resources.EnterNewLicenseKey,
                        HelpLink = "ProtectLibrary"
                    };
                    if (newkeyDialog.ShowDialog() == true)
                    {
                        if (newkeyControl.keyBox.Password.Length > 0 && 
                            newkeyControl.verifykeyBox.Password == newkeyControl.keyBox.Password)
                        {
                            try
                            {
                                SaveIndexFile(newkeyControl.verifykeyBox.Password);
                            }
                            catch (Exception ex)
                            {
                                SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.AnErrorOccuredProtectingTheLibrary,true);
                            }
                        }
                        else
                        {
                            SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.PasswordErrorLibraryIsNotProtected, true);
                        }
                    }

                    var temp = DropChanged;
                    if (CheckDropLink())
                    {
                        bCopyLink = true;
                        btnCopyLink.IsChecked = bCopyLink;
                        btnMergeCode.IsChecked = !bCopyLink;
                        SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.LibraryNowIsNotProtected);
                        if (temp != null)
                            temp(this, new LibraryDropChangedEventArgs(this, bCopyLink));
                        return true;
                    }
                    else
                    {
                        bCopyLink = true;
                        btnCopyLink.IsChecked = bCopyLink;
                        btnMergeCode.IsChecked = !bCopyLink;
                        SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.LibraryNowIsProtected);
                        if (temp != null)
                            temp(this, new LibraryDropChangedEventArgs(this, bCopyLink));
                        return false;
                    }
                }
                return CheckDropLink();
            }
            #endregion
            #region !bCan
            else
            {
                if (!String.IsNullOrEmpty(elastic.Key))
                {
                    var keyControl = new Controls.EnterKey();
                    keyControl.textBox.Text = Properties.Resources.LibraryIsProtected;
                    keyControl.ManagePassword.Visibility = System.Windows.Visibility.Visible;

                    var newDialog = new GeneralDialogContent(keyControl)
                    {
                        Owner = this.FindParent<Window>(),
                        Title = Properties.Resources.EnterLicenseKey,
                        HelpLink = "ProtectLibrary"
                    };
                    bool bManagePassword = false;
                    keyControl.ManagePassword.Click += (o, e) =>
                        {
                            bManagePassword = true;
                            newDialog.DialogResult = true;
                        };

                    if (newDialog.ShowDialog() == true)
                    {
                        var temp = DropChanged;
                        if (bManagePassword)
                        {
                            if (keyControl.keyBox.Password.Length > 0 &&
                                keyControl.keyBox.Password == elastic.Key)
                            {
                                var newkeyControl = new Controls.NewKey();
                                newkeyControl.Title.Text = Properties.Resources.ChangePassword;

                                var newkeyDialog = new GeneralDialogContent(newkeyControl)
                                {
                                    Owner = this.FindParent<Window>(),
                                    Title = Properties.Resources.EnterNewLicenseKey,
                                    HelpLink = "ProtectLibrary"
                                };
                                if (newkeyDialog.ShowDialog() == true)
                                {
                                    if (newkeyControl.keyBox.Password.Length > 0 &&
                                        newkeyControl.verifykeyBox.Password == newkeyControl.keyBox.Password)
                                    {
                                        try
                                        {
                                            SaveIndexFile(newkeyControl.verifykeyBox.Password);
                                        }
                                        catch (Exception ex)
                                        {
                                            SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.AnErrorOccuredProtectingTheLibrary,true);
                                        }
                                    }
                                    else
                                        SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.PasswordErrorLibraryIsNotProtected,true);

                                }

                                
                                if (CheckDropLink())
                                {
                                    bCopyLink = true;
                                    btnCopyLink.IsChecked = bCopyLink;
                                    btnMergeCode.IsChecked = !bCopyLink;
                                    SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.LibraryNowIsNotProtected);
                                    if (temp != null)
                                        temp(this, new LibraryDropChangedEventArgs(this, bCopyLink));
                                    return true;
                                }
                                else
                                {
                                    bCopyLink = true;
                                    btnCopyLink.IsChecked = bCopyLink;
                                    btnMergeCode.IsChecked = !bCopyLink;
                                    SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.LibraryNowIsProtected);
                                    if (temp != null)
                                        temp(this, new LibraryDropChangedEventArgs(this, bCopyLink));
                                    return false;
                                }
                            }
                            else
                            {
                                bCopyLink = true;
                                btnCopyLink.IsChecked = bCopyLink;
                                btnMergeCode.IsChecked = !bCopyLink;
                                SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.WrongPassword,true);
                                if (temp != null)
                                    temp(this, new LibraryDropChangedEventArgs(this, bCopyLink));
                                return false;
                            }
                        }
                        else
                        {
                            if (keyControl.keyBox.Password.Length > 0 &&
                                keyControl.keyBox.Password == elastic.Key)
                            {
                                try
                                {
                                    SaveIndexFile("");
                                }
                                catch (Exception ex)
                                {
                                    SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.AnErrorOccuredProtectingTheLibrary,true);
                                }
                            }
                            else
                            {
                                bCopyLink = true;
                                btnCopyLink.IsChecked = bCopyLink;
                                btnMergeCode.IsChecked = !bCopyLink;
                                SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.WrongPassword,true);
                                if (temp != null)
                                    temp(this, new LibraryDropChangedEventArgs(this, bCopyLink));
                                return false;
                            }

                            if (CheckDropLink())
                            {
                                bCopyLink = true;
                                btnCopyLink.IsChecked = bCopyLink;
                                btnMergeCode.IsChecked = !bCopyLink;
                                SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.LibraryNowIsNotProtected);
                                if (temp != null)
                                    temp(this, new LibraryDropChangedEventArgs(this, bCopyLink));
                                return true;
                            }
                            else
                            {
                                bCopyLink = true;
                                btnCopyLink.IsChecked = bCopyLink;
                                btnMergeCode.IsChecked = !bCopyLink;
                                SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.LibraryNowIsProtected);
                                if (temp != null)
                                    temp(this, new LibraryDropChangedEventArgs(this, bCopyLink));
                                return false;
                            }
                        }
                    }
                    return CheckDropLink();
                }
                else
                {
                    var keyControl = new Controls.KeyControlMessage();
                    keyControl.textBox.Text = Properties.Resources.LibraryIsNotProtected;
                    var newDialog = new GeneralDialogContent(keyControl)
                    {
                        Owner = this.FindParent<Window>(),
                        Title = Properties.Resources.ProtectKeyTitle,
                        HelpLink = "ProtectLibrary"
                    };

                    if (newDialog.ShowDialog() == true)
                    {
                        var newkeyControl = new Controls.NewKey();
                        newkeyControl.Title.Text = Properties.Resources.CreatePassword;

                        var newkeyDialog = new GeneralDialogContent(newkeyControl)
                        {
                            Owner = this.FindParent<Window>(),
                            Title = Properties.Resources.EnterNewLicenseKey,
                            HelpLink = "ProtectLibrary"
                        };
                        if (newkeyDialog.ShowDialog() == true)
                        {
                            if (newkeyControl.keyBox.Password.Length > 0 &&
                                newkeyControl.verifykeyBox.Password == newkeyControl.keyBox.Password)
                            {
                                try
                                {
                                    SaveIndexFile(newkeyControl.verifykeyBox.Password);
                                }
                                catch (Exception ex)
                                {
                                    SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.AnErrorOccuredProtectingTheLibrary,true);
                                }
                            }
                            else
                                SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.PasswordErrorLibraryIsNotProtected,true);

                        }

                        var temp = DropChanged;
                        if (CheckDropLink())
                        {
                            bCopyLink = true;
                            btnCopyLink.IsChecked = bCopyLink;
                            btnMergeCode.IsChecked = !bCopyLink;
                            SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.LibraryNowIsNotProtected);
                            if (temp != null)
                                temp(this, new LibraryDropChangedEventArgs(this, bCopyLink));
                            return true;
                        }
                        else
                        {
                            bCopyLink = true;
                            btnCopyLink.IsChecked = bCopyLink;
                            btnMergeCode.IsChecked = !bCopyLink;
                            SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.LibraryNowIsProtected);
                            if (temp != null)
                                temp(this, new LibraryDropChangedEventArgs(this, bCopyLink));
                            return false;
                        }
                    }
                    return CheckDropLink();
                }
            }
            #endregion
          #endregion
        }

        //bool bCopyLink = true;
        bool _bCopyLink = true;
        public bool bCopyLink
        {
            get
            {
                return _bCopyLink;
            }
            private set
            {
                if (_bCopyLink == value)
                    return;
                _bCopyLink = value;
            }
        }

        private void btnDropLink_Click(object sender, RoutedEventArgs e)
        {
            SetDropLink();
        }

        private void btnMergeCode_Click(object sender, RoutedEventArgs e)
        {
            SetMergeCode();
        }

        internal void SetDropLink()
        {
            bCopyLink = true;
            UpdateButtonLayout();
        }

        internal void SetMergeCode()
        {
            bool bCan = CheckDropLink();
            if (bCan)
                bCopyLink = false;
            else
                bCopyLink = true;
            UpdateButtonLayout();
        }

        private void UpdateButtonLayout()
        {
            bool bCan = CheckDropLink();
            btnCopyLink.IsChecked = bCopyLink;
            btnMergeCode.IsChecked = !bCopyLink;
            btnMergeCode.IsEnabled = bCan;
            DropChanged?.Invoke(this, new LibraryDropChangedEventArgs(this, bCopyLink));
        }

        private void btnProtectKey_Click(object sender, RoutedEventArgs e)
        {
            //ProtectLibrary();
            // using (var cursor = new WaitCursor())
            {
                try
                {
                    btnMergeCode.IsEnabled = ProtectLibrary();
                }
                catch (Exception ex)
                {
                    SymbolGallery.LibraryHelper.ShowMessage(String.Format(Properties.Resources.LibraryError, ex.Message),true);
                }
            }
        }
        internal bool CheckDropLink()
        {
            bool bRet = true;
            try
            {
                var elastic = GetDynamic();
                if (bIsEditable)
                {
                    if (elastic.Key is String && String.IsNullOrEmpty(elastic.Key as String))
                    {
                        return true;
                    }
                    else
                        return false;
                }
                else if (bElastic)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }

            //bRet = false;

            //var keyControl = new Controls.EnterKey();
            //var newDialog = new GeneralDialogContent(keyControl)
            //{
            //    Owner = this.FindParent<Window>(),
            //    Title = Properties.Resources.EnterLicenseKey
            //};

            //if (newDialog.ShowDialog() == true)
            //{
            //    if (keyControl.keyBox.Password.Length > 0 &&
            //        keyControl.keyBox.Password == elastic.Key)
            //    {
            //        bElastic = true;
            //        bRet = true;
            //    }
            //}

            //return bRet;
        }

        internal void AddFolder(String name)
        {
            var treeItem = treeListControl.GetSelectedNodes().FirstOrDefault() as TreeListNode;
            if (treeItem == null || !mapModelItem.ContainsKey(treeItem.Tag.ToString()))
                return;

            var path = treeItem.Tag as String;
            treeItem.IsExpanded = true;
            if (String.IsNullOrEmpty(path))
                path = startingFolder;
            if (fileSystemProvider != null)
            {
                fileSystemProvider.CreateFolder(new FileManagerFolder(fileSystemProvider, path),
                        name);
            }
            else
            {
                Directory.CreateDirectory(String.Format("{0}\\{1}", path, name));
            }
            var node = AddTreeItem(String.Format("{0}\\{1}", path, name));
            treeListControl.SelectNode(node);
        }

        internal string GetDefaultFolder()
        {
            var path = String.Format("{0}\\{1}", startingFolder, Properties.Settings.Default.DefaultSymbolFolder);
            bool exists;
            if (fileSystemProvider != null)
            {
                var folder = new FileManagerFolder(fileSystemProvider, path);
                exists = fileSystemProvider.Exists(folder);
            }
            else
            {
                exists = Directory.Exists(path);
            }
            if(!exists)
                AddFolder(Properties.Settings.Default.DefaultSymbolFolder);
            var item = treeListControl.GetTreeItem(path);
            if (item != null)
            {
                treeListControl.SelectNode(item);
                return path;
            }

            return string.Empty;
        }

        internal void DeleteSelectedFolder()
        {
            var treeItem = treeListControl.GetSelectedNodes().FirstOrDefault() as TreeListNode;
            if (treeItem == null)
                return;
            var path = treeItem.Tag as String;
            if (path == startingFolder)
                return;

            if (fileSystemProvider != null)
            {
                var folder = new FileManagerFolder(fileSystemProvider, path);
                if (fileSystemProvider.Exists(folder))
                {
                    if (fileSystemProvider.GetFiles(folder).ToList().Count == 0 &&
                        fileSystemProvider.GetFiles(folder).ToList().Count == 0 ||
                        SymbolGallery.LibraryHelper.ShowYesNo(Properties.Resources.FolderNotEmpty) == MessageBoxResult.Yes)
                        fileSystemProvider.DeleteFolder(folder);
                    else
                        return;
                }
            }
            else
            {
                if (Directory.Exists(path))
                {
                    try
                    {
                        Directory.Delete(path);
                    }
                    catch (Exception ex)
                    {
                        if (SymbolGallery.LibraryHelper.ShowYesNo(Properties.Resources.FolderNotEmpty) == MessageBoxResult.Yes)
                            Directory.Delete(path, true);
                        else
                            return;
                    }
                }
            }
            var parent = treeItem.ParentNode;
            if (parent == null)
                treeListView.Nodes.Remove(treeItem);
            else
                parent.Nodes.Remove(treeItem);
            if (mapModelList.ContainsKey(treeItem.Tag.ToString()))
                mapModelList.Remove(treeItem.Tag.ToString());
            if (mapModelItem.ContainsKey(treeItem.Tag.ToString()))
                mapModelItem.Remove(treeItem.Tag.ToString());
        }

        internal String GetSelectedPath()
        {
            var treeItem = treeListControl.GetSelectedNodes().FirstOrDefault() as TreeListNode;
            if (treeItem == null)
                return String.Empty;
            var path = treeItem.Tag as String;
            if (path == startingFolder)
                return String.Empty;
            return path;
        }

        public void Dispose()
        {
            if (fileSystemProvider != null && fileSystemProvider is IDisposable)
            {
                (fileSystemProvider as IDisposable).Dispose();
                fileSystemProvider = null;
            }

            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }

            mapModelList.Clear();
            mapModelItem.Clear();
        }

        private void treeViewFolder_LoadOnDemand(TreeListNode sender)
        {
            var path = sender.Tag.ToString();
            if (sender.Tag == null)
            {
                if (fileSystemProvider != null)
                {
                    var task1 = Task.Factory.StartNew(() =>
                    {
                        var startingfolder = new FileManagerFolder(fileSystemProvider, startingFolder);
                        if (!fileSystemProvider.Exists(startingfolder))
                            fileSystemProvider.CreateFolder(null, startingFolder);
                        return fileSystemProvider.GetFolders(startingfolder).ToList();
                    });
                    var task2 = task1.ContinueWith(ret =>
                    {
                        foreach (var f in ret.Result)
                        {
                            AddTreeItem(f.FullName);
                        }

                        //args.TreeView//item.IsLoadOnDemand = false;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                    task1.ContinueWith((t) =>
                    {
                        //Dispatcher.InvokeIfRequired(() => args.TreeViewItem.IsLoadOnDemand = false);

                        Debug.WriteLine("I have observed a {0}",
                            t.Exception.InnerException.GetType().Name);
                    }, TaskContinuationOptions.OnlyOnFaulted);
                    task2.ContinueWith((t) =>
                    {
                        //Dispatcher.InvokeIfRequired(() => args.TreeViewItem.IsLoadOnDemand = false);

                        Debug.WriteLine("I have observed a {0}",
                            t.Exception.InnerException.GetType().Name);
                    }, TaskContinuationOptions.OnlyOnFaulted);
                }
                else
                {
                    var task1 = Task.Factory.StartNew(() =>
                    {
                        if (!Directory.Exists(startingFolder))
                            Directory.CreateDirectory(startingFolder);
                        var folders = Directory.GetDirectories(startingFolder);
                        return (from f in folders where !hiddenFolderList.Contains(f.ToUpper()) select f).ToList();
                    });
                    var task2 = task1.ContinueWith(ret =>
                    {
                        foreach (var f in ret.Result)
                        {
                            AddTreeItem(f);
                        }

                        //args.TreeView//item.IsLoadOnDemand = false;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                    task1.ContinueWith((t) =>
                    {
                        //Dispatcher.InvokeIfRequired(() => args.TreeViewItem.IsLoadOnDemand = false);
                        Debug.WriteLine("I have observed a {0}",
                            t.Exception.InnerException.GetType().Name);
                    }, TaskContinuationOptions.OnlyOnFaulted);
                    task2.ContinueWith((t) =>
                    {
                        //Dispatcher.InvokeIfRequired(() => args.TreeViewItem.IsLoadOnDemand = false);
                        Debug.WriteLine("I have observed a {0}",
                            t.Exception.InnerException.GetType().Name);
                    }, TaskContinuationOptions.OnlyOnFaulted);
                }
            }
            else
            {
                var item = sender;
                var search = item.Tag as String;
                if (fileSystemProvider != null)
                {
                    var task1 = Task.Factory.StartNew(() =>
                    {
                        var folder = new FileManagerFolder(fileSystemProvider, search);
                        if (fileSystemProvider.Exists(folder))
                        {
                            return fileSystemProvider.GetFolders(folder).ToList();
                        }
                        return null;
                    });
                    var task2 = task1.ContinueWith(ret =>
                    {
                        foreach (var f in ret.Result)
                        {
                            AddTreeItem(f.FullName);
                        }

                        //if (!isDynamicFolder)
                        //    //item.IsLoadOnDemand = false;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                    task1.ContinueWith((t) =>
                    {
                        ////Dispatcher.InvokeIfRequired(() => item.IsLoadOnDemand = false);
                        
                        Debug.WriteLine("I have observed a {0}",
                            t.Exception.InnerException.GetType().Name);
                    }, TaskContinuationOptions.OnlyOnFaulted);
                    task2.ContinueWith((t) =>
                    {
                        ////Dispatcher.InvokeIfRequired(() => item.IsLoadOnDemand = false);

                        Debug.WriteLine("I have observed a {0}",
                            t.Exception.InnerException.GetType().Name);
                    }, TaskContinuationOptions.OnlyOnFaulted);

                    if (isDynamicFolder)
                    {
                        var task3 = Task.Factory.StartNew(() =>
                        {
                            var folder = new FileManagerFolder(fileSystemProvider, search);
                            var listFiles = fileSystemProvider.GetFiles(folder);
                            var listXaml = (from c in listFiles/*.AsParallel()*/
                                            where System.IO.Path.GetExtension(c.FullName) == SymbolGalleryUI.patternXaml
                                            select c.FullName).ToList();
                            return listXaml;
                        });
                        var task4 = task3.ContinueWith(ret =>
                        {
                            ////item.IsLoadOnDemand = false;
                            ret.Result.ForEach(file => AddTreeItem(file, false));
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                        task3.ContinueWith((t) =>
                        {
                            ////Dispatcher.InvokeIfRequired(() => item.IsLoadOnDemand = false);
                            Debug.WriteLine("I have observed a {0}",
                                t.Exception.InnerException.GetType().Name);
                        }, TaskContinuationOptions.OnlyOnFaulted);
                        task4.ContinueWith((t) =>
                        {
                            ////Dispatcher.InvokeIfRequired(() => item.IsLoadOnDemand = false);
                            Debug.WriteLine("I have observed a {0}",
                                t.Exception.InnerException.GetType().Name);
                        }, TaskContinuationOptions.OnlyOnFaulted);
                    }
                }
                else
                {
                    var task1 = Task.Factory.StartNew(() =>
                    {
                        var folders = Directory.GetDirectories(search);
                        return (from f in folders where !hiddenFolderList.Contains(f.ToUpper()) select f).ToList();
                    });
                    var task2 = task1.ContinueWith(ret =>
                    {
                        foreach (var f in ret.Result)
                        {
                            AddTreeItem(f);
                        }

                        //if (!isDynamicFolder)
                        //    //item.IsLoadOnDemand = false;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                    task1.ContinueWith((t) =>
                    {
                        //Dispatcher.InvokeIfRequired(() => item.IsLoadOnDemand = false);
                        
                        Debug.WriteLine("I have observed a {0}",
                            t.Exception.InnerException.GetType().Name);
                    }, TaskContinuationOptions.OnlyOnFaulted);
                    task2.ContinueWith((t) =>
                    {
                        //Dispatcher.InvokeIfRequired(() => item.IsLoadOnDemand = false);

                        Debug.WriteLine("I have observed a {0}",
                            t.Exception.InnerException.GetType().Name);
                    }, TaskContinuationOptions.OnlyOnFaulted);

                    if (isDynamicFolder)
                    {
                        var task3 = Task.Factory.StartNew(() =>
                        {
                            return Directory.GetFiles(search, SymbolGalleryUI.searchPatternXaml);
                        });
                        var task4 = task3.ContinueWith(ret =>
                        {
                            foreach (var file in ret.Result)
                                AddTreeItem(file, false);

                            //item.IsLoadOnDemand = false;
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                        task3.ContinueWith((t) =>
                        {
                            //Dispatcher.InvokeIfRequired(() => item.IsLoadOnDemand = false);

                            Debug.WriteLine("I have observed a {0}",
                                t.Exception.InnerException.GetType().Name);
                        }, TaskContinuationOptions.OnlyOnFaulted);
                        task4.ContinueWith((t) =>
                        {
                            //Dispatcher.InvokeIfRequired(() => item.IsLoadOnDemand = false);

                            Debug.WriteLine("I have observed a {0}",
                                t.Exception.InnerException.GetType().Name);
                        }, TaskContinuationOptions.OnlyOnFaulted);
                    }
                }
            }
        }

        void SearchInFolders(IEnumerable<FileManagerFolder> folders, string search)
        {
            foreach(var folder in folders)
            {
                if (cts != null)
                    cts.Token.ThrowIfCancellationRequested();

                SearchInFolders(fileSystemProvider.GetFolders(folder), search);
                var listFiles = fileSystemProvider.GetFiles(folder);
                var listXaml = (from c in listFiles/*.AsParallel()*/
                                where System.IO.Path.GetExtension(c.FullName) == SymbolGalleryUI.patternXaml &&
                                Regex.IsMatch(c.Name, search, RegexOptions.IgnoreCase)
                                select c.FullName).ToList();
                listXaml.ForEach(s =>
                    {
                        if (cts != null)
                            cts.Token.ThrowIfCancellationRequested();
                        listFound.Add(s);
                    });
            }
        }

        void SearchInFolders(String[] folders, string search)
        {
            foreach (var folder in folders)
            {
                if (cts != null)
                    cts.Token.ThrowIfCancellationRequested();

                SearchInFolders(Directory.GetDirectories(folder), search);
                var listFiles = Directory.GetFiles(folder);
                var listXaml = (from c in listFiles/*.AsParallel()*/
                                where System.IO.Path.GetExtension(c) == SymbolGalleryUI.patternXaml &&
                                Regex.IsMatch(System.IO.Path.GetFileNameWithoutExtension(c), search, RegexOptions.IgnoreCase)
                                select c).ToList();
                listXaml.ForEach(s =>
                {
                    if (cts != null)
                        cts.Token.ThrowIfCancellationRequested();
                    listFound.Add(s);
                });
            }
        }

        void SearchForFolders(IEnumerable<FileManagerFolder> folders, string search)
        {
            foreach (var folder in folders)
            {
                if (cts != null)
                    cts.Token.ThrowIfCancellationRequested();

                SearchForFolders(fileSystemProvider.GetFolders(folder), search);
                if (Regex.IsMatch(folder.Name, search, RegexOptions.IgnoreCase))
                    listFound.Add(folder.FullName);
            }
        }

        void SearchForFolders(String[] folders, string search)
        {
            foreach (var folder in folders)
            {
                if (cts != null)
                    cts.Token.ThrowIfCancellationRequested();

                SearchForFolders(Directory.GetDirectories(folder), search);
                if (Regex.IsMatch(System.IO.Path.GetFileNameWithoutExtension(folder), search, RegexOptions.IgnoreCase))
                    listFound.Add(folder);
            }
        }

        private void btnNewFolder_Click(object sender, RoutedEventArgs e)
        {
            AddNewFolder();
        }

        internal void AddNewFolder()
        {
            var control = new Controls.NewFolder();
            var wnd = new GeneralDialogContent(control, GeneralDialogButtons.OkCancelButtons)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = ""
            };
            if (wnd.ShowDialog() == true)
            {
                try
                {
                    AddFolder(control.txtboxFolderName.Text);
                }
                catch (Exception ex)
                {
                    SymbolGallery.LibraryHelper.ShowMessage(Properties.Resources.CannotCreateFolder, true);
                }
            }
        }

        private void btnDeleteFolder_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedFolder();
        }

        SafeObservableCollection<String> listFound = new SafeObservableCollection<String>();
        CancellationTokenSource cts;
        internal void SearchSelectedFolder(string search)
        {
            listFound.Clear();
            cts = new CancellationTokenSource();
            listBoxFound.Visibility = System.Windows.Visibility.Visible;
            btnClose.Visibility = System.Windows.Visibility.Visible;
            progressBar.Visibility = System.Windows.Visibility.Visible;
            treeListControl.Visibility = System.Windows.Visibility.Collapsed;
            listBoxFound.ItemsSource = listFound;

            if (fileSystemProvider != null)
            {
                if (isDynamicFolder)
                {
                    var task1 = Task.Factory.StartNew(() =>
                    {
                        var startingfolder = new FileManagerFolder(fileSystemProvider, startingFolder);
                        SearchInFolders(fileSystemProvider.GetFolders(startingfolder), search);
                    });
                    var task2 = task1.ContinueWith(ret =>
                    {
                        cts.Dispose();
                        cts = null;
                        progressBar.Visibility = System.Windows.Visibility.Collapsed;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                else
                {
                    var task1 = Task.Factory.StartNew(() =>
                    {
                        var startingfolder = new FileManagerFolder(fileSystemProvider, startingFolder);
                        SearchForFolders(fileSystemProvider.GetFolders(startingfolder), search);
                    });
                    var task2 = task1.ContinueWith(ret =>
                    {
                        cts.Dispose();
                        cts = null;
                        progressBar.Visibility = System.Windows.Visibility.Collapsed;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            else
            {
                if (isDynamicFolder)
                {
                    var task1 = Task.Factory.StartNew(() =>
                    {
                        SearchInFolders(Directory.GetDirectories(startingFolder), search);
                    });
                    var task2 = task1.ContinueWith(ret =>
                    {
                        cts.Dispose();
                        cts = null;
                        progressBar.Visibility = System.Windows.Visibility.Collapsed;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                else
                {
                    var task1 = Task.Factory.StartNew(() =>
                    {
                        SearchForFolders(Directory.GetDirectories(startingFolder), search);
                    });
                    var task2 = task1.ContinueWith(ret =>
                    {
                        cts.Dispose();
                        cts = null;
                        progressBar.Visibility = System.Windows.Visibility.Collapsed;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            treeListControl.Visibility = System.Windows.Visibility.Visible;
            listBoxFound.Visibility = System.Windows.Visibility.Collapsed;
            btnClose.Visibility = System.Windows.Visibility.Collapsed;
            progressBar.Visibility = System.Windows.Visibility.Collapsed;
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }
        }

        private void listBoxRecent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            var filePath = listBoxFound.SelectedItem as String;
            if (String.IsNullOrEmpty(filePath))
                return;
            var temp = SelectedChanged;
            if (temp != null)
                temp(this, new LibrarySelectionChangedEventArgs(filePath, isDynamicFolder, fileSystemProvider));
        }
        public void LoadFromXml(string filepath)
        {
            //mapItems.Clear();

            try
            {
                var keyexpandolist = Utilities.XmlHelper.GetExpandoAttributeFromXml(File.ReadAllText(filepath), "resources",true);
                if (keyexpandolist.Count() != 0)
                {
                    keyexpandolist.ToList().ForEach(e =>
                    {
                        var regkeydictionary = e as IDictionary<string, object>;
                        regkeydictionary.ToList().ForEach(r =>
                        {
                            mapItems.Add(r.Key.ToString(), r.Value.ToString());
                        });
                    });
                }
            }
            catch (Exception e)
            {

            }

        }       
    }
}
