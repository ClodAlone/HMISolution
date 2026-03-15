using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Xml;
using System.Windows.Markup;
using Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Utilities.WPF;
using DevExpress.Xpf.WindowsUI;
using DevExpress.Xpf.Grid;
using WPFUtilities;
using DevExpress.Xpf.Grid.TreeList;

namespace ScreenManager
{
    /// <summary>
    /// Interaction logic for NewScreenType.xaml
    /// </summary>
    public partial class NewScreenType : UserControl
    {
        public String fileName { get; set; }
        public String xamlCode { get; set; }

        public String sourcefileName { get; set; }

        Dictionary<FrameworkElement, String> mapCreatedControls = new Dictionary<FrameworkElement, String>();
        Dictionary<FrameworkElement, String> mapCreatedFiles = new Dictionary<FrameworkElement, String>();
        Dictionary<TreeListNode, ObservableCollection<FrameworkElement>> mapFileList = new Dictionary<TreeListNode, ObservableCollection<FrameworkElement>>();
        ObservableCollection<FrameworkElement> listStyledObject = new ObservableCollection<FrameworkElement>();
        Dictionary<TreeListNode, Dictionary<String, TreeListNode>> nodeChildrenHeaders = new Dictionary<TreeListNode, Dictionary<String, TreeListNode>>();
        List<TreeListNode> breadthFirstNodesMap = new List<TreeListNode>();

        bool isPopup;
        bool isDataContextChanging;

        
        public NewScreenType(bool bPopup = false)
        {
            InitializeComponent();
            DataContext = this;
            isPopup = bPopup;

            using (new WaitCursor())
            {
                String startingPath = ApplicationPropertiesHelper.GetProperty<String>("CommonFolder");
                string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
                var ResourceFileName = String.Format("{0}.{1}\\NewScreenTypes", startingPath,mainversion);
                LoadSymbols(ResourceFileName);
                //listBox.ItemsSource = listStyledObject;
                //if(listStyledObject.Count > 0)
                //    listBox.SelectedItem = listStyledObject[0];
            }

            if (isPopup)
            {
                Loaded += (o, e) => 
                {
                    var wnd = this.FindParent<Window>();
                    wnd.Closing += (s, c) =>
                    {
                        isDataContextChanging = true;
                        if (listBox.SelectedItem != null && listBox.SelectedItem is FrameworkElement &&
                            mapCreatedControls.ContainsKey(listBox.SelectedItem as FrameworkElement) &&
                            mapCreatedFiles.ContainsKey(listBox.SelectedItem as FrameworkElement))
                        {
                            xamlCode = mapCreatedControls[listBox.SelectedItem as FrameworkElement];
                            sourcefileName = mapCreatedFiles[listBox.SelectedItem as FrameworkElement];
                        }
                        DataContext = xamlCode;
                        isDataContextChanging = false;
                    };
                };

                DataContextChanged += (o, e) =>
                {
                    if (!isDataContextChanging && DataContext is String)
                    {

                    }
                };
            }
        }

        public IEnumerable<String> GetListCategories(String Type, String fileType, bool deepsearch)
        {
            string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
            var startingPath = ApplicationPropertiesHelper.GetProperty<String>("CommonFolder");
            var folder = String.Format("{0}.{2}\\{1}", startingPath,Type,mainversion);

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
        public IEnumerable<FrameworkElement> GetScreenList(String Type, String fileType, String Category)
        {
            var search = String.Format("*{0}", fileType);
            string[] directoryGetFiles = Directory.GetFiles(Category, search);
            
            foreach (var fileOn in directoryGetFiles)
            {
                var element = new FrameworkElement();
                FileInfo file = new FileInfo(fileOn);
                if (file.Extension.Equals(".xaml"))
                {
                    var filePng = System.IO.Path.ChangeExtension(fileOn, "png");
                    if (File.Exists(filePng))
                    {
                        var img = new BitmapImage();
                        img.BeginInit();
                        img.UriSource = new Uri(filePng, UriKind.RelativeOrAbsolute);
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.EndInit();
                        element = new Image() { Source = img, Margin = new Thickness(20) };
                        element.Name = DependencyObjectExtensions.AdaptName(System.IO.Path.GetFileNameWithoutExtension(fileOn));

                        //listStyledObject.Add(element);
                        mapCreatedControls.Add(element, File.ReadAllText(fileOn));
                        mapCreatedFiles.Add(element, fileOn);
                    }
                    else
                    {
                        try
                        {
                            using (XmlReader xmlReader = XmlReader.Create(fileOn))
                            {
                                Object obj = XamlReader.Load(xmlReader);
                                element = obj as FrameworkElement;
                                if (element != null)
                                {
                                    element.Name = DependencyObjectExtensions.AdaptName(System.IO.Path.GetFileNameWithoutExtension(fileOn));
                                    //listStyledObject.Add(element);
                                    mapCreatedControls.Add(element, File.ReadAllText(fileOn));
                                    mapCreatedFiles.Add(element, fileOn);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(ex.ToString());
                        }
                    }
                }
                yield return element;
            }
            //mapCreatedControls = mapCreatedControls;
            //mapCreatedFiles = mapCreatedFiles;
        }
        private void LoadTreeViewItems(TreeListControl treeView, TreeListNode treeViewItem, string lastSelectedType, string lastSelectedSubType,bool lastparent)
        {
            var categoriesd = GetListCategories(lastSelectedType, lastSelectedSubType, false).ToList();
            using (var cursor = new WaitCursor())
            {
                foreach (var category in categoriesd)
                {
                    var itemRootHeader = Path.GetFileName(category);
                    var itemRoot = treeView.AddNode(new TreeItemControl(itemRootHeader), treeViewItem);

                    if (!nodeChildrenHeaders.ContainsKey(treeViewItem))
                        nodeChildrenHeaders.Add(treeViewItem, new Dictionary<string, TreeListNode>());
                    if (!nodeChildrenHeaders[treeViewItem].ContainsKey(itemRootHeader))
                        nodeChildrenHeaders[treeViewItem].Add(itemRootHeader, itemRoot);
                    else
                        nodeChildrenHeaders[treeViewItem][itemRootHeader] = itemRoot;

                    // bool bPopulated = true;

                    var screenList = GetScreenList(lastSelectedType, lastSelectedSubType, category);
                    mapFileList[itemRoot] = new ObservableCollection<FrameworkElement>(screenList);
                    listStyledObject = mapFileList[itemRoot];
                    listBox.ItemsSource = listStyledObject;
                    if (listStyledObject.Count > 0)
                        listBox.SelectedItem = listStyledObject[0];

                    var _lastpath = string.Format("{0}\\{1}", lastSelectedType, itemRootHeader);
                    bool last = categoriesd.LastOrDefault() == category;

                    LoadTreeViewItems(treeView, itemRoot, _lastpath, lastSelectedSubType, last);
                    
                    //if (last && lastparent)
                    //{
                    //    if(!itemRoot.HasItems)
                    //        itemRoot.IsSelected = true;
                    //    itemRoot.IsExpanded = true;
                    //}
                }
                //);
            }
        }

        void TreeViewAdv_SelectedItemChanged(object sender, TreeListSelectionChangedEventArgs e)
        {
            var selectedItem = (sender as TreeListControl).GetSelectedNodes().FirstOrDefault();
            if (selectedItem == null)
                return;

            if (mapFileList.ContainsKey(selectedItem))
            {
                listStyledObject = mapFileList[selectedItem];
                listBox.ItemsSource = listStyledObject;
                if (listStyledObject.Count > 0)
                    listBox.SelectedItem = listStyledObject[0];
            }
            else
            {
                listStyledObject = new ObservableCollection<FrameworkElement>();
                listBox.ItemsSource = listStyledObject;
            }
        }

        private bool IsToBeSelected(string _lastpath)
        {
            if (File.Exists(String.Format("{0}\\stindex.dat", _lastpath)))
                return true;
            else
                return false;
        }

        void LoadSymbols(string Path)
        {
            if (Directory.Exists(Path))
            {
                /********************************************/
                var treeViewAdv = TryFindResource("treeview") as TreeListControl;

                treeViewAdv.Visibility = Visibility.Collapsed;

                var itemRoot = treeViewAdv.AddNode(new TreeItemControl("Screens"));

                //mapFileList[itemRoot] = new ObservableCollection<FrameworkElement>();
                //LoadTreeViewItems(treeViewAdv, itemRoot, "NewScreenTypes", ".xaml");
                using (var cursor = new WaitCursor())
                {
                    var screenList = GetScreenList("NewScreenTypes", ".xaml", Path);
                    mapFileList[itemRoot] = new ObservableCollection<FrameworkElement>(screenList);
                    listStyledObject = mapFileList[itemRoot];
                    listBox.ItemsSource = listStyledObject;
                    if (listStyledObject.Count > 0)
                        listBox.SelectedItem = listStyledObject[0];

                    LoadTreeViewItems(treeViewAdv, itemRoot, "NewScreenTypes", ".xaml",true);
                    NodesBreadthFirst(itemRoot, true);
                }

                //scroller.Content = treeViewAdv;
                Grid.SetColumn(treeViewAdv,0);
                Grid.SetRow(treeViewAdv, 0);
                TreeviewContainerGrid.Children.Add(treeViewAdv);

                treeViewAdv.Visibility = Visibility.Visible;

                itemRoot.IsExpanded = true;
                treeViewAdv.SelectNode(itemRoot);

                if (mapFileList.ContainsKey(itemRoot))
                {
                    listStyledObject = mapFileList[itemRoot];
                    listBox.ItemsSource = listStyledObject;
                    if (listStyledObject.Count > 0)
                        listBox.SelectedItem = listStyledObject[0];
                }
                else
                {
                    listStyledObject = new ObservableCollection<FrameworkElement>();
                    listBox.ItemsSource = listStyledObject;
                }

                SelectDefaultItem(treeViewAdv, itemRoot,string.Empty);

                /********************************************/

                //string[] directoryGetFiles = Directory.GetFiles(Path, "*.xaml");
                //if (directoryGetFiles.Length > 0)
                //{
                //    FileInfo dir = new FileInfo(Path);

                //    Array.ForEach(directoryGetFiles, fileOn =>
                //    {
                //        FileInfo file = new FileInfo(fileOn);
                //        if (file.Extension.Equals(".xaml"))
                //        {
                //            var filePng = System.IO.Path.ChangeExtension(fileOn, "png");
                //            if (File.Exists(filePng))
                //            {
                //                var img = new BitmapImage();
                //                img.BeginInit();
                //                img.UriSource = new Uri(filePng, UriKind.RelativeOrAbsolute);
                //                img.CacheOption = BitmapCacheOption.OnLoad;
                //                img.EndInit();
                //                var element = new Image() { Source = img, Margin = new Thickness(20) };
                //                element.Name = DependencyObjectExtensions.AdaptName(System.IO.Path.GetFileNameWithoutExtension(fileOn));

                //                listStyledObject.Add(element);
                //                mapCreatedControls.Add(element, File.ReadAllText(fileOn));
                //                mapCreatedFiles.Add(element, fileOn);
                //            }
                //            else
                //            {
                //                try
                //                {
                //                    using (XmlReader xmlReader = XmlReader.Create(fileOn))
                //                    {
                //                        Object obj = XamlReader.Load(xmlReader);
                //                        FrameworkElement element = obj as FrameworkElement;
                //                        if (element != null)
                //                        {
                //                            element.Name = DependencyObjectExtensions.AdaptName(System.IO.Path.GetFileNameWithoutExtension(fileOn));
                //                            listStyledObject.Add(element);
                //                            mapCreatedControls.Add(element, File.ReadAllText(fileOn));
                //                            mapCreatedFiles.Add(element, fileOn);
                //                        }
                //                    }
                //                }
                //                catch (Exception ex)
                //                {
                //                    Trace.TraceError(ex.ToString());
                //                }
                //            }
                //        }
                //    });
                //}

                //Array.ForEach(Directory.GetDirectories(Path), LoadSymbols);
            }
        }

        private void NodesBreadthFirst(TreeListNode parentNode, bool bStart)
        {
            if (bStart)
                breadthFirstNodesMap.Clear();

            if (!breadthFirstNodesMap.Contains(parentNode))
                breadthFirstNodesMap.Add(parentNode);

            if (!nodeChildrenHeaders.ContainsKey(parentNode))
                return;

            breadthFirstNodesMap.AddRange(nodeChildrenHeaders[parentNode].Values);

            foreach (var child in nodeChildrenHeaders[parentNode].Values)
                breadthFirstNodesMap.AddRange(child.Nodes);

            foreach (var child in nodeChildrenHeaders[parentNode].Values)
                NodesBreadthFirst(child, false);
        }

        private void SelectDefaultItem(TreeListControl treeViewAdv, TreeListNode itemRoot, string scategories)
        {
            try
            {
                if (string.IsNullOrEmpty(scategories))
                {
                    string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
                    var _selfile = String.Format("{0}.{1}\\NewScreenTypes\\stindex.dat", ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"),mainversion);
                    if (File.Exists(_selfile))
                    {
                        scategories = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(_selfile));
                        if (string.IsNullOrEmpty(scategories))
                            return;
                    }
                }

                using (var cursor = new WaitCursor())
                {
                    var categories = scategories.Split('\\');
                    if (categories != null && categories.Count() > 0)
                    {
                        var _itemRoot = (from TreeListNode node in itemRoot.Nodes
                                         where string.Compare((node.Content as TreeItemControl).ItemHeader.ToString(), categories[0], false) == 0
                                         select node).FirstOrDefault();

                        if (_itemRoot != null)
                        {
                            _itemRoot.IsExpanded = true;
                            treeViewAdv.SelectNode(_itemRoot);
                            if (mapFileList.ContainsKey(_itemRoot))
                            {
                                listStyledObject = mapFileList[_itemRoot];
                                listBox.ItemsSource = listStyledObject;
                                if (listStyledObject.Count > 0)
                                {
                                    listBox.SelectedItem = listStyledObject[0];
                                    if (mapCreatedControls.ContainsKey(listStyledObject[0]))
                                    {
                                        xamlCode = mapCreatedControls[listStyledObject[0]];
                                        sourcefileName = mapCreatedFiles[listStyledObject[0]];
                                    }
                                }
                            }
                            else
                            {
                                listStyledObject = new ObservableCollection<FrameworkElement>();
                                listBox.ItemsSource = listStyledObject;
                            }
                            if (categories.Count() > 1)
                                SelectDefaultItem(treeViewAdv, _itemRoot, scategories.Remove(0, categories[0].Length + 1));
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var v in e.AddedItems)
            {
                if (!mapCreatedControls.ContainsKey(v as FrameworkElement))
                    continue;
                xamlCode = mapCreatedControls[v as FrameworkElement];
                sourcefileName = mapCreatedFiles[v as FrameworkElement];
            }
        }

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var wnd = this.FindParent<Window>();
            if (wnd == null)
                return;

            if(listBox.SelectedItem != null)
            {
                if (mapCreatedControls.ContainsKey(listBox.SelectedItem as FrameworkElement))
                {
                    xamlCode = mapCreatedControls[listBox.SelectedItem as FrameworkElement];
                    sourcefileName = mapCreatedFiles[listBox.SelectedItem as FrameworkElement];
                }
            }

            wnd.DialogResult = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var ret = WinUIMessageBox.Show(
                                Window.GetWindow(this),
                                String.Format(Properties.Resources.NewScreenAskDelete, System.IO.Path.GetFileNameWithoutExtension(sourcefileName)),
                                Properties.Resources.NewScreenAskDeleteTitle,
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question,
                                MessageBoxResult.No, MessageBoxOptions.None,
                                DevExpress.Xpf.Core.FloatingMode.Window
                                );
            if (ret != MessageBoxResult.Yes)
                return;
            try
            {
                File.Delete(sourcefileName);
                var found = (from c in mapCreatedFiles where c.Value == sourcefileName select c.Key).ToList();
                if (found.Count > 0 && listStyledObject.Contains(found[0]))
                {
                    listStyledObject.Remove(found[0]);
                    if (listStyledObject.Count > 0)
                        listBox.SelectedIndex = 0;
                }
            }
            catch(Exception ex)
            {
                WinUIMessageBox.Show(
                                    Window.GetWindow(this),
                                    String.Format(Properties.Resources.NewScreenErrorDelete, ex.Message, System.IO.Path.GetFileNameWithoutExtension(sourcefileName)),
                                    Properties.Resources.NewScreenAskDeleteTitle,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error,
                                    MessageBoxResult.None, MessageBoxOptions.None,
                                    DevExpress.Xpf.Core.FloatingMode.Window
                                    );
            }
        }
    }

}
