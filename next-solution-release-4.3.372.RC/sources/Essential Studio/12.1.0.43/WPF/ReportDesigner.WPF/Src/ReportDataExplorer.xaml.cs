#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
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
using Syncfusion.Windows.ReportDesigner.Resources;
using Syncfusion.Windows.Tools.Controls;
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;

namespace Syncfusion.Windows.Reports.Designer
{
    /// <summary>
    /// Interaction logic for ReportData.xaml
    /// </summary>

    public partial class ReportDataExplorer : UserControl
    {
        DatabaseAccessTypes databaseAccess = DatabaseAccessTypes.None;

        internal string VisualStyle
        { 
            get; 
            set; 
        }

        internal DesignMode DesignerMode
        {
            get
            {
                return (DesignMode)GetValue(DesignModeProperty);
            }
            set
            {
                SetValue(DesignModeProperty, value);
            }
        }

        internal static readonly DependencyProperty DesignModeProperty =
            DependencyProperty.Register("DesignerMode", typeof(DesignMode), typeof(ReportDataExplorer), new UIPropertyMetadata(DesignMode.RDL, OnDesignViewPropertyChanged));
        
        internal static void OnDesignViewPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDataExplorer dataExplorer = dependencyObject as ReportDataExplorer;
            if (e.NewValue != e.OldValue)
            {
                if (e.NewValue.ToString() == DesignMode.RDLC.ToString())
                {
                    dataExplorer.mnu_DataSource.Visibility = Visibility.Collapsed;   
                }
            }
        }

        public ReportDesignView ReportDesignView
        {
            get { return (ReportDesignView)GetValue(ReportDesignViewProperty); }
            set { SetValue(ReportDesignViewProperty, value); }
        }

        public static readonly DependencyProperty ReportDesignViewProperty =
            DependencyProperty.Register("ReportDesignView", typeof(ReportDesignView), typeof(ReportDataExplorer), new UIPropertyMetadata(null, OnReportDesignViewPropertyChanged));

        internal static void OnReportDesignViewPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ReportDataExplorer dataExplorer = dependencyObject as ReportDataExplorer;

            if (dataExplorer != null)
            {
                if (e.NewValue != e.OldValue)
                {
                    if (e.OldValue != null)
                    {
                        ReportDesignView view = e.OldValue as ReportDesignView;
                        view.RemoveReportData(dataExplorer);
                    }

                    if (e.NewValue != null)
                    {
                        ReportDesignView view = e.NewValue as ReportDesignView;
                        dataExplorer.DesignerMode = view.DesignMode;
                        view.AddReportData(dataExplorer);
                    }
                }
            }
        }

        internal Syncfusion.Windows.Reports.Designer.Controls.DesignPanel DesignPanel 
        { 
            get; 
            set;
        }

        public ReportDataExplorer()
        {
            InitializeComponent();
            this.InitilizeTreeView();
            UpdateCulture();
            this.PreviewKeyUp += new KeyEventHandler(ReportData_PreviewKeyUp);
        }

        internal void UpdateCulture()
        {
            TreeViewItemAdvCollection coll = (TreeViewItemAdvCollection)Resources["DefaultTreeNodes"];
            foreach (var item in coll)
            {
                item.Header = SR.GetString(CultureInfo.CurrentUICulture, item.Name);
               // ContextMenuService.SetPlacement(item, System.Windows.Controls.Primitives.PlacementMode.Bottom);
                
                if (item.Items != null && item.Items.Count > 0)
                {
                    foreach (var children in item.Items)
                    {
                        if (children is TreeViewItemAdv)
                        {
                            string cultureHeader=SR.GetString(CultureInfo.CurrentUICulture, (children as TreeViewItemAdv).Name);
                            if (!String.IsNullOrEmpty(cultureHeader))
                            {
                                (children as TreeViewItemAdv).Header = cultureHeader;
                            }
                        }
                    }
                }
            }

            this.headerNew.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerNew");
            this.headerNew.ToolTip = SR.GetString(CultureInfo.CurrentUICulture, "headerNew");
            this.mnu_DataSource.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerDataSource");
            this.mnu_DataSet.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerDataSet");
            this.mnu_Parameter.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerParameter");
            this.mnu_Image.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerImage");



        }

        void ReportData_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                TreeViewItemAdv viewitemadv = this.TreeView1.SelectedItem as TreeViewItemAdv;
                if (viewitemadv.Parent.ToString().ToLower() == SR.GetString(CultureInfo.CurrentUICulture, "headerParameters").ToLower())
                {
                    this.DesignPanel.RemoveReportParameter(this.TreeView1.SelectedItem.ToString());
                }
                else if (viewitemadv.Parent.ToString().ToLower() == SR.GetString(CultureInfo.CurrentUICulture, "headerImages").ToLower())
                {
                    this.DesignPanel.RemoveEmbeddedImage(this.TreeView1.SelectedItem.ToString());
                }
                else if (viewitemadv.Parent.ToString().ToLower() == SR.GetString(CultureInfo.CurrentUICulture, "headerDataSources").ToLower())
                {
                    this.DesignPanel.RemoveDataSource(this.TreeView1.SelectedItem.ToString());
                }
                else if (viewitemadv.Parent.ToString().ToLower() == SR.GetString(CultureInfo.CurrentUICulture, "headerDataSets").ToLower())
                {
                    this.DesignPanel.RemoveDataSet(this.TreeView1.SelectedItem.ToString());
                }
                else if (viewitemadv.ParentItemsControl.Parent.ToString().ToLower() == SR.GetString(CultureInfo.CurrentUICulture, "headerDataSets").ToLower())
                {
                    this.DesignPanel.RemoveDatasetFields(viewitemadv.Parent.ToString(), this.TreeView1.SelectedItem.ToString());
                }
            }
        }

        #region toolbar_new menu items and treeview_menu_itemClick

        private void MenuAddDataSourceItem_Click(object sender, RoutedEventArgs e)
        {
            this.DesignPanel.AddDataSource();
        }

        private void MenuAddDataSetItem_Click(object sender, RoutedEventArgs e)
        {
            this.DesignPanel.CurrentDataSource = null;

            if ((TreeViewItemAdv)((MenuItem)sender).Tag != null)
            {
                this.DesignPanel.CurrentDataSource = ((TreeViewItemAdv)((MenuItem)sender).Tag).Header.ToString();
            }

            this.DesignPanel.AddDataSet();
        }

        private void MenuItemAddParameter_Click(object sender, RoutedEventArgs e)
        {
            this.DesignPanel.AddReportParameter();
        }

        private void MenuItemAddImage_Click(object sender, RoutedEventArgs e)
        {
            this.DesignPanel.AddEmbeddedImage();
        }

        #endregion

        #region treeview_expand and Collapsed

        private void TreeViewItemAdv_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItemAdv node = sender as TreeViewItemAdv;

            if (node.Items != null && node.Items.Count > 0)
            {
                if (this.VisualStyle == "Metro")
                {
                    ((TreeViewItemAdv)sender).LeftImageSource = (DrawingImage)this.Resources["MetroFolderOpen"];
                }

                else if (this.VisualStyle == "Blend")
                {
                    ((TreeViewItemAdv)sender).LeftImageSource = (DrawingImage)this.Resources["BlendFolderOpen"];
                }

                else
                {
                    ((TreeViewItemAdv)sender).LeftImageSource = (BitmapImage)this.Resources["FolderOpen"];
                }
            }
        }

        private void TreeViewItemAdv_Collapsed(object sender, RoutedEventArgs e)
        {
            if (this.VisualStyle == "Metro")
            {
                ((TreeViewItemAdv)sender).LeftImageSource = (DrawingImage)this.Resources["MetroFolderClose"];

            }

            else if (this.VisualStyle == "Blend")
            {
                ((TreeViewItemAdv)sender).LeftImageSource = (DrawingImage)this.Resources["BlendFolderClose"];

            }

            else
            {
                ((TreeViewItemAdv)sender).LeftImageSource = (BitmapImage)this.Resources["FolderClose"];
            }
        }

        #endregion

        #region Image Related

        internal void EmbeddedImagesUpdate(Syncfusion.RDL.DOM.EmbeddedImages embeddedImages)
        {
            TreeViewItemAdv treeViewItem = ReturnTreeNode(SR.GetString(CultureInfo.CurrentUICulture, "headerImages"));
            treeViewItem.Items.Clear();

            if (embeddedImages != null&&embeddedImages.Count>0)
            {
                foreach (Syncfusion.RDL.DOM.EmbeddedImage embeddedImage in embeddedImages)
                {
                    this.AddEmbeddedImage(treeViewItem, embeddedImage);
                }
            }
        }

        private void AddEmbeddedImage(Syncfusion.RDL.DOM.EmbeddedImage embeddedImage)
        {
            TreeViewItemAdv treeViewItem = ReturnTreeNode(SR.GetString(CultureInfo.CurrentUICulture, "headerImages"));
            this.AddEmbeddedImage(treeViewItem, embeddedImage);
        }

        private void AddEmbeddedImage(TreeViewItemAdv treeViewItem, Syncfusion.RDL.DOM.EmbeddedImage embeddedImage)
        {
            TreeViewItemAdv treeViewImage = new TreeViewItemAdv();
            treeViewImage.Header = embeddedImage.Name;
            ContextMenu contextMenuImage = new ContextMenu();
            MenuItem menuItemImage = new MenuItem();
            menuItemImage.Header = "Delete";
            menuItemImage.Tag = treeViewImage;
            menuItemImage.Click += new RoutedEventHandler(MenuItemImageDelete_Click);
            contextMenuImage.Items.Add(menuItemImage);
            treeViewImage.ContextMenu = contextMenuImage;
            treeViewImage.Tag = embeddedImage;
            treeViewItem.Items.Add(treeViewImage);

            if (treeViewItem.Items.Count > 0)
            {
                treeViewItem.IsExpanded = true;
            }
            else
            {
                treeViewItem.IsExpanded = false;
            }
        }

        #endregion

        internal void ParametersUpdate(Syncfusion.RDL.DOM.ReportParameters reportParameters)
        {
            TreeViewItemAdv tvi = this.ReturnTreeNode(SR.GetString(CultureInfo.CurrentUICulture, "headerParameters"));
            tvi.Items.Clear();
            if (reportParameters != null)
            {
                foreach (Syncfusion.RDL.DOM.ReportParameter parameter in reportParameters)
                {
                    TreeViewItemAdv tviParams = new TreeViewItemAdv();
                    ContextMenu contMenu = this.AddParametersContextMenu(tviParams);
                    tviParams.ContextMenu = contMenu;
                    char[] delimetersForReportParameters = { ' ', '@', ':' };
                    tviParams.Header = parameter.Name.Trim(delimetersForReportParameters).ToString();
                    tviParams.IsEditable = false;
                    tviParams.Tag = tvi.Header;
                    tviParams.AllowDrop = false;
                    Image img = new Image();
                    if (this.VisualStyle == "Metro")
                    {
                        img.Source = this.FindResource("DesignerParameters") as DrawingImage;
                    }
                    tviParams.LeftImageSource = img.Source;
                    tviParams.ImageHeight = 15;
                    tviParams.ImageWidth = 15;
                    tvi.Items.Add(tviParams);
                }

                if (tvi.Items.Count > 0)
                {
                    tvi.IsExpanded = true;
                }
                else
                {
                    tvi.IsExpanded = false;
                }
            }
        }

        private void MenuAddParameterDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            this.DesignPanel.RemoveReportParameter(((TreeViewItemAdv)((MenuItem)sender).Tag).Header.ToString());
        }

        private void MenuAddParameterPropertiesItem_Click(object sender, RoutedEventArgs e)
        {
            this.DesignPanel.ModifyReportParameter(((TreeViewItemAdv)((MenuItem)sender).Tag).Header.ToString());
        }

        private void MenuAddDataSetItemProperties_Click(object sender, RoutedEventArgs e)
        {
            this.DesignPanel.ModifyDataSet(((TreeViewItemAdv)((MenuItem)sender).Tag).Header.ToString());
        }

        private void MenuAddDataSetDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            this.DesignPanel.RemoveDataSet(((TreeViewItemAdv)((MenuItem)sender).Tag).Header.ToString());
        }

        private void MenuAddDataSourceItemProperties_Click(object sender, RoutedEventArgs e)
        {
            this.DesignPanel.ModifyDataSource(((TreeViewItemAdv)((MenuItem)sender).Tag).Header.ToString());
        }

        private void MenuAddDataSourceDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            this.DesignPanel.RemoveDataSource(((TreeViewItemAdv)((MenuItem)sender).Tag).Header.ToString());
        }

        private void menuItemField_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItemAdv viewitemadv = this.TreeView1.SelectedItem as TreeViewItemAdv;
            this.DesignPanel.RemoveDatasetFields(viewitemadv.Parent.ToString(), this.TreeView1.SelectedItem.ToString());
        }

        void contextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
        }

        private ContextMenu AddDataSourceContextMenu(TreeViewItemAdv tvi)
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            menuItem.Click += new RoutedEventHandler(MenuAddDataSetItem_Click);
            menuItem.Header = Syncfusion.Windows.Reports.Designer.Properties.Resources.headerAddDataSet.ToString();
            menuItem.Tag = tvi;
            contextMenu.Items.Add(menuItem);
            MenuItem menuItemDelete = this.AddDataSourceDeleteMenuItem(Syncfusion.Windows.Reports.Designer.Properties.Resources.headerDelete.ToString(), tvi);
            contextMenu.Items.Add(menuItemDelete);
            if (DesignerMode != DesignMode.RDLC)
            {
                MenuItem menuItemDataSource = this.AddDataSourceMenuItem(Syncfusion.Windows.Reports.Designer.Properties.Resources.headerDataSourceProp.ToString(), tvi);
                contextMenu.Items.Add(menuItemDataSource);
            }
            return contextMenu;
        }

        private ContextMenu AddDataSetContextMenu(TreeViewItemAdv tvi)
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItemDataSourceDelete = this.AddDataSetDeleteMenuItem(Syncfusion.Windows.Reports.Designer.Properties.Resources.headerDelete.ToString(), tvi);
            MenuItem menuItemDataSource = this.AddDataSetMenuItem(Syncfusion.Windows.Reports.Designer.Properties.Resources.headerDataSetProp.ToString(), tvi);
            contextMenu.Items.Add(menuItemDataSourceDelete);
            contextMenu.Items.Add(menuItemDataSource);
            return contextMenu;
        }

        private MenuItem AddDataSourceMenuItem(string menuHeader, TreeViewItemAdv tvi)
        {
            MenuItem menuItem = new MenuItem();
            menuItem.Click += new RoutedEventHandler(MenuAddDataSourceItemProperties_Click);
            menuItem.Header = menuHeader;
            menuItem.Tag = tvi;
            return menuItem;
        }

        private MenuItem AddDataSourceDeleteMenuItem(string menuHeader, TreeViewItemAdv tvi)
        {
            MenuItem menuItem = new MenuItem();
            menuItem.Click += new RoutedEventHandler(MenuAddDataSourceDeleteItem_Click);
            menuItem.Header = menuHeader;
            menuItem.Tag = tvi;
            return menuItem;
        }

        internal void DataSourcesUpdate(Syncfusion.RDL.DOM.DataSources reportDataSources)
        {
            TreeViewItemAdv treeViewItem = ReturnTreeNode(SR.GetString(CultureInfo.CurrentUICulture, "headerDataSources"));
            treeViewItem.Items.Clear();

            if (this.DesignerMode == DesignMode.RDLC)
            {
                treeViewItem.Visibility = Visibility.Collapsed;
                return;
            }

            if (reportDataSources != null && reportDataSources.Count > 0)
            {
                foreach (Syncfusion.RDL.DOM.DataSource reportDataSource in reportDataSources)
                {
                    this.AddDataSource(treeViewItem, reportDataSource);
                }
            }
            else
            {
                treeViewItem.IsExpanded = false;
            }
        }

        private void AddDataSource(Syncfusion.RDL.DOM.DataSource reportDataSource)
        {
            TreeViewItemAdv treeViewItem = ReturnTreeNode(SR.GetString(CultureInfo.CurrentUICulture, "headerDataSources"));
            this.AddDataSource(treeViewItem, reportDataSource);
        }

        private void AddDataSource(TreeViewItemAdv treeViewItem, Syncfusion.RDL.DOM.DataSource reportDataSource)
        {
            TreeViewItemAdv tvi = new TreeViewItemAdv();
            tvi.Name = reportDataSource.Name;
            ContextMenu contextMenu = this.AddDataSourceContextMenu(tvi);
            tvi.ContextMenu = contextMenu;
            tvi.Header = reportDataSource.Name;
            tvi.IsEditable = false;
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            string appLocation = AppDomain.CurrentDomain.BaseDirectory.ToString();
            tvi.MouseRightButtonDown += new MouseButtonEventHandler(tvi_MouseRightButtonDown);

            if (this.VisualStyle == "Metro")
            {
                image.Source = (DrawingImage)this.Resources["MetroDataSource"];
            }
            else
            {
                if (reportDataSource.DataSourceReference != null)
                {
                    image.Source = new BitmapImage(new Uri(@"pack:application:,,,/Syncfusion.ReportDesigner.WPF;componentImages/DataSource.png", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    image.Source = new BitmapImage(new Uri(@"pack:application:,,,/Syncfusion.ReportDesigner.WPF;componentImages/staticdatasource.png", UriKind.RelativeOrAbsolute));
                }
            }

            tvi.LeftImageSource = image.Source;
            tvi.ImageWidth = 20;
            tvi.ImageHeight = 20;
            tvi.AllowDrop = false;
            DeSelectTree();
            tvi.IsSelected = true;
            treeViewItem.Items.Add(tvi);

            if (treeViewItem.Items.Count > 0)
            {
                treeViewItem.IsExpanded = true;
            }
            else
            {
                treeViewItem.IsExpanded = false;
            }
        }

        internal void DataSetsUpdate(Syncfusion.RDL.DOM.DataSets reportDataSets)
        {
            TreeViewItemAdv treeViewItem = ReturnTreeNode(SR.GetString(CultureInfo.CurrentUICulture, "headerDataSets"));
            treeViewItem.Items.Clear();
            foreach (RDL.DOM.DataSet set in reportDataSets)
            {
                string name = set.Query.DataSourceName;
            }
            if (reportDataSets != null && reportDataSets.Count > 0)
            {
                foreach (Syncfusion.RDL.DOM.DataSet reportDataSet in reportDataSets)
                {
                    this.AddDataSet(treeViewItem, reportDataSet);
                }
            }
        }

        private void AddDataSet(Syncfusion.RDL.DOM.DataSet reportDataSet)
        {
            TreeViewItemAdv treeViewItem = ReturnTreeNode(SR.GetString(CultureInfo.CurrentUICulture, "headerDataSets"));
            this.AddDataSet(treeViewItem, reportDataSet);
        }

        private void AddDataSet(TreeViewItemAdv treeViewItem, Syncfusion.RDL.DOM.DataSet reportDataSet)
        {
            TreeViewItemAdv tviDataSet = new TreeViewItemAdv();
            ContextMenu contextMenu = this.AddDataSetContextMenu(tviDataSet);
            tviDataSet.ContextMenu = contextMenu;
            tviDataSet.Header = reportDataSet.Name;
            tviDataSet.IsEditable = false;
            tviDataSet.Tag = reportDataSet;
            tviDataSet.AllowDrop = false;
            tviDataSet.IsExpanded = true;

            foreach (Syncfusion.RDL.DOM.Field field in reportDataSet.Fields)
            {
                TreeViewItemAdv tviField = new TreeViewItemAdv();
                ContextMenu contextMenuField = new ContextMenu();
                MenuItem menuItemField = new MenuItem();
                menuItemField.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerDelete");
                menuItemField.Tag = tviField;
                menuItemField.Click += new RoutedEventHandler(menuItemField_Click);
                contextMenuField.Items.Add(menuItemField);
                tviField.ContextMenu = contextMenuField;
                if (this.databaseAccess == DatabaseAccessTypes.Read)
                {
                    tviField.ContextMenu.Visibility = Visibility.Collapsed;
                }

                tviField.IsEditable = false;
                tviField.Tag = tviDataSet.Header;
                tviField.Header = field.Name;
                tviDataSet.Items.Add(tviField);
            }

            tviDataSet.Name = reportDataSet.Name;
            treeViewItem.Items.Add(tviDataSet);
            tviDataSet.IsSelected = true;

            if (treeViewItem.Items.Count > 0)
            {
                treeViewItem.IsExpanded = true;
            }
            else
            {
                treeViewItem.IsExpanded = false;
            }
        }

        private ContextMenu AddParametersContextMenu(TreeViewItemAdv tviParams)
        {
            try
            {
                ContextMenu contextMenu = new ContextMenu();
                MenuItem menuItemParameterDelete = this.AddParameterDeleteMenuItem(Syncfusion.Windows.Reports.Designer.Properties.Resources.headerDelete.ToString(), tviParams);
                MenuItem menuItemParamter = this.AddParameterMenuItem(Syncfusion.Windows.Reports.Designer.Properties.Resources.headerParmProp.ToString(), tviParams);
                contextMenu.Items.Add(menuItemParameterDelete);
                contextMenu.Items.Add(menuItemParamter);
                return contextMenu;
            }
            catch
            {
                return null;
            }
        }

        private MenuItem AddParameterDeleteMenuItem(string menuHeader, TreeViewItemAdv tviParams)
        {
            MenuItem menuItem = new MenuItem();
            menuItem.Click += new RoutedEventHandler(MenuAddParameterDeleteItem_Click);
            menuItem.Header = menuHeader;
            menuItem.Tag = tviParams;
            return menuItem;
        }

        private MenuItem AddParameterMenuItem(string menuHeader, TreeViewItemAdv tviParams)
        {
            MenuItem menuItem = new MenuItem();
            menuItem.Click += new RoutedEventHandler(MenuAddParameterPropertiesItem_Click);
            menuItem.Header = menuHeader;
            menuItem.Tag = tviParams;
            return menuItem;
        }

        private MenuItem AddDataSetMenuItem(string menuHeader, TreeViewItemAdv tvi)
        {
            MenuItem menuItem = new MenuItem();
            menuItem.Click += new RoutedEventHandler(MenuAddDataSetItemProperties_Click);
            menuItem.Header = menuHeader;
            menuItem.Tag = tvi;
            return menuItem;
        }

        private MenuItem AddDataSetDeleteMenuItem(string menuHeader, TreeViewItemAdv tvi)
        {
            MenuItem menuItem = new MenuItem();
            menuItem.Click += new RoutedEventHandler(MenuAddDataSetDeleteItem_Click);
            menuItem.Header = menuHeader;
            menuItem.Tag = tvi;
            return menuItem;
        }

        private void MenuItemImageDelete_Click(object sender, RoutedEventArgs e)
        {
            MenuItem treeviewItem = (MenuItem)e.Source;
            TreeViewItemAdv tvi = treeviewItem.Tag as TreeViewItemAdv;
            this.DesignPanel.RemoveEmbeddedImage(tvi.Header.ToString());
        }

        internal void InitilizeTreeView()
        {
            TreeViewItemAdvCollection collection = new TreeViewItemAdvCollection();
            collection = this.Resources["DefaultTreeNodes"] as TreeViewItemAdvCollection;

            if (collection != null)
            {
                this.TreeView1.Items.Clear();

                foreach (var item in collection)
                {
                    this.TreeView1.Items.Add(item);
                }
            }
        }

        void TreeView1_DragStart(object sender, DragTreeViewItemAdvEventArgs e)
        {
            TreeObjectCollection collection = (e.Data.GetData(typeof(TreeObjectCollection)) as TreeObjectCollection);
            TreeViewItemAdv treeItem = collection[0] as TreeViewItemAdv;

            if ((treeItem.Tag != null && treeItem.Tag is string) || treeItem.Tag is RDL.DOM.EmbeddedImage)
            {
                HitTestResult result = VisualTreeHelper.HitTest(this.TreeView1, Mouse.GetPosition(this.TreeView1));

                if (result != null)
                {
                    var parent = VisualTreeHelper.GetParent(result.VisualHit);

                    while (parent != null && parent != TreeView1 && !(parent is TreeViewItemAdv))
                    {
                        parent = VisualTreeHelper.GetParent(parent);
                    }
                }
            }
            else
            {
                e.AllowDragDrop = false;
            }

            DeSelectTree();
        }

        #region updatestyle_and_returntreenodes

        private void UpdateTreeNodeStyle(TreeViewItemAdv node)
        {
            if (this.VisualStyle == "Metro")
            {
                node.LeftImageSource = (DrawingImage)this.Resources["MetroFolderClose"];
            }
            else if (this.VisualStyle == "Blend")
            {
                node.LeftImageSource = (DrawingImage)this.Resources["BlendFolderClose"];
            }
            else
            {
                node.LeftImageSource = (BitmapImage)this.Resources["FolderClose"];
            }

            if (node.Items != null && node.Items.Count > 0 && node.IsExpanded)
            {
                if (this.VisualStyle == "Metro")
                {
                    node.LeftImageSource = (DrawingImage)this.Resources["MetroFolderOpen"];
                }
                else if (this.VisualStyle == "Blend")
                {
                    node.LeftImageSource = (DrawingImage)this.Resources["BlendFolderOpen"];
                }

                else
                {
                    node.LeftImageSource = (BitmapImage)this.Resources["FolderOpen"];
                }
            }
        }

        internal void UpdateTreeStyle(string visualstyle)
        {
            VisualStyle = visualstyle;
            TreeViewItemAdv dataSet = ReturnTreeNode(SR.GetString(CultureInfo.CurrentUICulture, "headerDataSets"));
            TreeViewItemAdv dataSource = ReturnTreeNode(SR.GetString(CultureInfo.CurrentUICulture, "headerDataSources"));
            TreeViewItemAdv image = ReturnTreeNode(SR.GetString(CultureInfo.CurrentUICulture, "headerImages"));
            TreeViewItemAdv parameter = this.ReturnTreeNode(SR.GetString(CultureInfo.CurrentUICulture, "headerParameters"));
            TreeViewItemAdv buildIn = this.ReturnTreeNode(SR.GetString(CultureInfo.CurrentUICulture, "headerBuiltFields"));

            this.UpdateTreeNodeStyle(dataSet);
            this.UpdateTreeNodeStyle(dataSource);
            this.UpdateTreeNodeStyle(image);
            this.UpdateTreeNodeStyle(parameter);
            this.UpdateTreeNodeStyle(buildIn);
        }


        private TreeViewItemAdv ReturnTreeNode(string itemName)
        {
            foreach (TreeViewItemAdv item in this.TreeView1.Items)
            {
                if (item.Header.ToString().ToLower() == itemName.ToString().ToLower())
                {
                    return item;
                }
            }

            return null;
        }
        #endregion

        #region deselct_treenodes

        private void DeSelectTree()
        {
            foreach (TreeViewItemAdv treeItem in this.TreeView1.Items)
            {
                DeSelectTreeItem(treeItem);
            }
        }

        private void DeSelectTreeItem(TreeViewItemAdv treeItem)
        {
            treeItem.IsSelected = false;

            foreach (TreeViewItemAdv treeViewItem in treeItem.Items)
            {
                DeSelectTreeItem(treeViewItem);
            }
        }

        #endregion

        void tvi_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItemAdv dataSource = sender as TreeViewItemAdv;

            var datasets = (from dataset in this.DesignPanel.DataSets
                            where dataset.Query.DataSourceName == dataSource.Name
                            select dataset).ToList();

            if (datasets.Count > 0)
            {
                (dataSource.ContextMenu.Items[1] as MenuItem).IsEnabled = false;
            }

            else
            {
                (dataSource.ContextMenu.Items[1] as MenuItem).IsEnabled = true;
            }
        }
    }
}
