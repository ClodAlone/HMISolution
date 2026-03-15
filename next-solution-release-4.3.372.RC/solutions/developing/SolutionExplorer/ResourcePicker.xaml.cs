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
using Utilities;
using Utilities.WPF;
using UFProjectManager.ComponentService;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using WPFUtilities;
using DocumentManager.ComponentService;

namespace UFProjectManager
{
    class InnerControl
    {
        #region Constructors
        public InnerControl(UserControl content)
        {
            Content = content;
        }
        #endregion

        #region Properties
        public UserControl Content { get; private set; }
        #endregion
    }

    /// <summary>
    /// Interaction logic for ResourcePicker.xaml
    /// </summary>
    public partial class ResourcePicker : UserControl, IDisposable
    {
        #region Declarations
        readonly static object DummyNode = new Object();

        readonly BitmapImage openFolderImg;
        readonly BitmapImage closedFolderImg;

        readonly UFProjectDocument Document;

        readonly Dictionary<String, DXTabItem> mapTabs = new Dictionary<string, DXTabItem>();

        readonly List<ResourcePicker> listChildControls = new List<ResourcePicker>();
        readonly List<string> listChildLoaded = new List<string>();

        bool bControlIsSelected;
        #endregion

        public ResourcePicker(UFProjectDocument doc)
        {
            InitializeComponent();

            Document = doc;
            openFolderImg = UFProjectManagerComponent.GetControlImage("OpenFolderSmall", true);
            closedFolderImg = UFProjectManagerComponent.GetControlImage("CloseFolderSmall", true);

            var list = Document.ResourcTypes;
            list.Sort();
            list.ForEach(type =>
            {
                var docManager = Document.GetResourceDocumentManager(type);
                if (docManager.isMultipleResource)
                {
                    var treeControl = new TreeListControl()
                    {
                        SelectionMode = MultiSelectMode.Row
                    };

                    treeControl.Columns.Add(new TreeListColumn() { FieldName = "Content", CellTemplate = FindResource("innerControlTemplate") as DataTemplate });

                    treeControl.GotFocus += (o, e) => { bControlIsSelected = true; };
                    treeControl.LostFocus += (o, e) =>
                    {
                        Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            bControlIsSelected = false;
                        });
                    };

                    treeControl.Loaded += (s, e) =>
                    {
                        treeControl.View.AutoWidth = true;
                        treeControl.View.AllowEditing = false;
                        treeControl.View.ShowColumnHeaders = false;
                        treeControl.View.ShowIndicator = false;
                        treeControl.View.ShowHorizontalLines = false;
                        treeControl.View.ShowVerticalLines = false;
                        treeControl.View.ShowNodeImages = true;
                        treeControl.View.NodeImageSize = new Size(16, 16);
                        treeControl.View.NodeExpanding += subitem_Expanded;
                    };

                    var tabItemText = new DXTabItem()
                    {
                        Header = docManager.TypeLabel,
                        Content = treeControl
                    };

                    mapTabs.Add(type, tabItemText);

                    var folder = Document.GetResourceFolderWatcher(type);
                    AddTreeItem(folder, treeControl.View.Nodes, Document, treeControl);
                }
            });
        }

        void AddOrUpdateChildProject()
        {
            foreach (var resource in Document.GetAllChilds())
            {
                var project = (resource as UFProjectDocument).FilePath;
                if (!listChildLoaded.Contains(project))
                {
                    var list = (
                        from c in mapTabs.Values
                        where c.Content is TreeListControl
                        select c.Content as TreeListControl).ToList();

                    foreach (var treeControl in list)
                    {
                        AddTreeItem(resource, treeControl.View.Nodes, Document, treeControl);
                    }
                    listChildLoaded.Add(project);
                }
            }
        }

        void subitem_Expanded(object sender, TreeListNodeAllowEventArgs e)
        {
            TreeListNode item = e.Node;

            if (item == null || item.Tag == null)
                return;
            e.Handled = true;
            item.Nodes.Clear();

            using (new WaitCursor())
            {   
                if (item.Tag is IDocument)
                {
                    var document = item.Tag as UFProjectDocument;
                    LoadChildTree(document, item);
                }
                else if (item.Tag is ResourceFolderWatcher)
                {
                    var type = item.Tag as ResourceFolderWatcher;
                    var resources = type.ListResources.ToList();
                    foreach (var resource in resources)
                        AddTreeItem(resource, item.Nodes, Document, null);

                    var folders = type.ListFolders.ToList();
                    foreach (var folder in folders)
                        AddTreeItem(folder, item.Nodes, Document, null);

                    type.ListResources.CollectionChanged += (o, ev) =>
                    {
                        Dispatcher.InvokeIfRequired(() =>
                        {
                            if (ev.NewItems != null)
                            {
                                TreeListNode treeItem = null;
                                foreach (var uri in ev.NewItems)
                                {
                                    var i = (from p in item.Nodes
                                             where p.Tag as Uri == uri as Uri
                                             select p).ToList();
                                    if (i.Count == 0)
                                    {
                                        item.IsExpanded = true;
                                        treeItem = AddTreeItem(uri, item.Nodes, Document, null);
                                    }
                                    else
                                        treeItem = i[0];
                                }
                                if (treeItem != null)
                                {
                                    var treeControl = e.OriginalSource as TreeListControl;
                                    if (treeControl != null)
                                        treeControl.SelectNode(treeItem);
                                }
                            }
                            if (ev.OldItems != null)
                            {
                                foreach (var uri in ev.OldItems)
                                {
                                    var i = (from p in item.Nodes
                                             where p.Tag as Uri == uri as Uri
                                             select p).ToList();
                                    i.ForEach(j => item.Nodes.Remove(j));
                                }
                            }
                        });
                    };

                    type.ListFolders.CollectionChanged += (o, ev) =>
                    {
                        Dispatcher.InvokeIfRequired(() =>
                        {
                            if (ev.NewItems != null)
                            {
                                TreeListNode treeItem = null;
                                foreach (var folder in ev.NewItems)
                                {
                                    var i = (from p in item.Nodes
                                             where p.Tag == folder
                                             select p).ToList();
                                    if (i.Count == 0)
                                    {
                                        item.IsExpanded = true;
                                        treeItem = AddTreeItem(folder, item.Nodes, Document, null);
                                    }
                                    else
                                        treeItem = i[0];
                                }
                                if (treeItem != null)
                                {
                                    var treeControl = e.OriginalSource as TreeListControl;
                                    if (treeControl != null)
                                        treeControl.SelectNode(treeItem);
                                }
                            }
                            if (ev.OldItems != null)
                            {
                                foreach (var folder in ev.OldItems)
                                {
                                    var i = (from p in item.Nodes
                                             where p.Tag == folder
                                             select p).ToList();
                                    i.ForEach(j => item.Nodes.Remove(j));
                                }
                            }
                        });
                    };
                }

            }
        }

        private void LoadChildTree(UFProjectDocument doc, TreeListNode item)
        {
            if (doc != null)
            {
                var split = Filter.Split(';');
                var list = doc.ResourcTypes;
                list.Sort();
                foreach (var type in list.Where(type => split.Contains(type)))
                {
                    var docManager = doc.GetResourceDocumentManager(type);
                    if (docManager.isMultipleResource)
                    {
                        var folder = doc.GetResourceFolderWatcher(type);
                        AddTreeItem(folder, item.Nodes, doc, null);
                    }
                }
                var docChild = doc.GetAllChilds();
                foreach (var resource in docChild)
                {
                    AddTreeItem(resource, item.Nodes, doc, null);
                }
            }
        }

        void ItemHeader_Validate(object sender, TreeListCellValidationEventArgs e)
        {
            
        }

        bool bSelectCurrent;
        private TreeListNode AddTreeItem(Object tag, TreeListNodeCollection parent, UFProjectDocument doc, TreeListControl treeListControl)
        {
            var newitem = new TreeListNode()
            {
                Tag = tag
            };

            if (tag is Uri)
            {
                var uri = tag as Uri;
                var model = new UriModel(uri);

                var header = new ResourceTreeControl() { DataContext = model };
                header.btnOpen.Visibility = System.Windows.Visibility.Collapsed;
                header.btnDelete.Visibility = System.Windows.Visibility.Collapsed;
                header.btnRename.Visibility = System.Windows.Visibility.Collapsed;

                newitem.Content = new InnerControl(header);
                newitem.Image = doc.GetResourceImage(uri);

                bool isChildProjectUri = doc.ListChildProjectPaths.Contains(uri);
                if (isChildProjectUri)
                {
                    newitem.Nodes.Add(new TreeListNode { Tag = DummyNode });
                }
                else
                {
                    header.MouseDoubleClick += (o, e) =>
                    {
                        bControlIsSelected = true;
                        this.FindParent<Window>().DialogResult = true;
                    };
                }
            }
            else if (tag is ResourceFolderWatcher)
            {
                var folder = tag as ResourceFolderWatcher;
                var header = new FolderTreeControl() { DataContext = folder };
                header.btnRename.Visibility = System.Windows.Visibility.Collapsed;
                header.btnDelete.Visibility = System.Windows.Visibility.Collapsed;
                newitem.Content = new InnerControl(header);
                newitem.Image = closedFolderImg;
                newitem.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "IsExpanded")
                    {
                        newitem.Image = newitem.IsExpanded ? openFolderImg : closedFolderImg;
                    }
                };

                folder.newCommandEvent += (o, e) => 
                {
                    newitem.IsExpanded = true;
                };

                folder.newFolderCommandEvent += (o, e) =>
                {
                    newitem.IsExpanded = true;
                };

                newitem.Nodes.Add(new TreeListNode() { Tag = DummyNode });
            }
            else if (tag is IDocument)
            {
                var document = tag as UFProjectDocument;
                var uri = new Uri(document.ProjectFolder, UriKind.RelativeOrAbsolute);
                var model = new UriModel(uri);

                var header = new ResourceTreeControl() { DataContext = model };
                header.btnOpen.Visibility = System.Windows.Visibility.Collapsed;
                header.btnDelete.Visibility = System.Windows.Visibility.Collapsed;
                header.btnRename.Visibility = System.Windows.Visibility.Collapsed;

                newitem.Content = new InnerControl(header);
                newitem.Image = doc.GetResourceImage(uri);

                treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);
            }

            parent.Add(newitem);
            return newitem;
        }

        void UpdateTabsFromFilter()
        {
            TabControlExtprop.Items.Clear();

            var split = Filter.Split(';');
            for (int i = 0; i < split.Length; ++i)
            {
                if (mapTabs.ContainsKey(split[i]))
                    TabControlExtprop.Items.Add(mapTabs[split[i]]);
            }
        }

        #region Properties
        String filter;
        public String Filter
        {
            get
            {
                return filter;
            }
            set
            {
                if (value == filter)
                {
                    AddOrUpdateChildProject();
                    return;
                }
                filter = value;
                UpdateTabsFromFilter();
                AddOrUpdateChildProject();
                listChildControls.ForEach(control => control.Filter = Filter);
            }
        }

        static Uri FindSelected(ResourcePicker picker)
        {
            foreach(var control in picker.listChildControls)
            {
                var ret = FindSelected(control);
                if (ret != null)
                    return ret;
            }
            var selected = (from c in picker.listChildControls where c.bControlIsSelected select c).ToList();
            if (selected.Count > 0)
            {
                selected.ForEach(control => control.bControlIsSelected = false);
                return selected[0].Selected;
            }

            return null;
        }

        public Uri Selected
        {
            get
            {
                var uri = FindSelected(this);
                if (uri != null)
                    return uri;

                var tab = TabControlExtprop.SelectedItem as DXTabItem;
                if (tab == null)
                    return null;
                var tree = tab.Content as TreeListControl;
                var item = tree.GetSelectedNodes();
                if (item == null || item.Length == 0)
                    return null;
                var ret = item[0].Tag as Uri;
                bool isChildProjectUri = Document.ListChildProjectPaths.Contains(ret);
                if (isChildProjectUri)
                {
                    var control = item[0].Content as InnerControl;
                    if (control != null && control.Content is ResourcePicker)
                        return (control.Content as ResourcePicker).Selected;
                    return null;
                }
                return ret;
            }
        }

        #endregion

        public void Dispose()
        {
            listChildControls.ForEach(control => control.Dispose());
            listChildControls.Clear();

            foreach (DXTabItem item in mapTabs.Values)
            {
                if (item.Content is IDisposable)
                    (item.Content as IDisposable).Dispose();
            }
            TabControlExtprop.Items.Clear();
            TabControlExtprop.Dispose();
        }
    }
}
