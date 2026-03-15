#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
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
using Syncfusion.Windows.Tools.Controls;
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;

namespace Syncfusion.Windows.Reports.Designer
{
    /// <summary>
    /// Interaction logic for ReportData.xaml
    /// </summary>

    public partial class ReportData : UserControl
    {
        DatabaseAccessTypes databaseAccess = DatabaseAccessTypes.None;

        internal string VisualStyle
        { 
            get; 
            set; 
        }

        public DesignMode DesignerMode
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

        public static readonly DependencyProperty DesignModeProperty =
            DependencyProperty.Register("DesignerMode", typeof(DesignMode), typeof(ReportData), new UIPropertyMetadata(DesignMode.RDL, null));

        internal Syncfusion.Windows.Reports.Designer.Controls.DesignPanel designpanel { get; set; }

        public ReportData()
        {
            InitializeComponent();
            this.InitilizeTreeView();
            this.PreviewKeyUp += new KeyEventHandler(ReportData_PreviewKeyUp);
        }

        void ReportData_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                TreeViewItemAdv viewitemadv = this.TreeView1.SelectedItem as TreeViewItemAdv;
                if (viewitemadv.Parent.ToString().ToLower() == RESX.headerParameters.ToString().ToLower())
                {
                    this.designpanel.RemoveReportParameter(this.TreeView1.SelectedItem.ToString());
                }
                else if (viewitemadv.Parent.ToString().ToLower() == RESX.headerImages.ToString().ToLower())
                {
                    this.designpanel.RemoveEmbeddedImage(this.TreeView1.SelectedItem.ToString());
                }
                else if (viewitemadv.Parent.ToString().ToLower() == RESX.headerDataSources.ToString().ToLower())
                {
                    this.designpanel.RemoveDataSource(this.TreeView1.SelectedItem.ToString());
                }
                else if (viewitemadv.Parent.ToString().ToLower() == RESX.headerDataSets.ToString().ToLower())
                {
                    this.designpanel.RemoveDataSet(this.TreeView1.SelectedItem.ToString());
                }
                else if (viewitemadv.ParentItemsControl.Parent.ToString().ToLower() == RESX.headerDataSets.ToString().ToLower())
                {
                    this.designpanel.RemoveDatasetFields(viewitemadv.Parent.ToString(), this.TreeView1.SelectedItem.ToString());
                }
            }
        }

        public ItemsControl GetSelectedTreeViewItemParent(TreeViewItemAdv item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItemAdv || parent is TreeViewAdv))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as ItemsControl;
        }

        #region toolbar_new menu items and treeview_menu_itemClick

        private void MenuAddDataSourceItem_Click(object sender, RoutedEventArgs e)
        {
            this.designpanel.ModifyDataSource(((TreeViewItemAdv)((MenuItem)sender).Tag).Header.ToString());
        }
        private void MenuAddDataSetItem_Click(object sender, RoutedEventArgs e)
        {
            this.designpanel.AddDataSet();
        }
        private void MenuItemAddParameter_Click(object sender, RoutedEventArgs e)
        {
            this.designpanel.AddReportParameter();
        }
        private void MenuItemAddImage_Click(object sender, RoutedEventArgs e)
        {
            this.designpanel.AddEmbeddedImage();
        }
        #endregion

        #region treeview_expand and Collapsed
        private void TreeViewItemAdv_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItemAdv node = sender as TreeViewItemAdv;

            if (node.Items != null && node.Items.Count > 0)
            {
                if (this.VisualStyle != "Metro")
                {
                    ((TreeViewItemAdv)sender).LeftImageSource = (BitmapImage)this.Resources["FolderOpen"];
                }
                else
                {
                    ((TreeViewItemAdv)sender).LeftImageSource = (DrawingImage)this.Resources["MetroFolderOpen"];
                }
            }
        }

        private void TreeViewItemAdv_Collapsed(object sender, RoutedEventArgs e)
        {
            if (this.VisualStyle != "Metro")
            {
                ((TreeViewItemAdv)sender).LeftImageSource = (BitmapImage)this.Resources["FolderClose"];
            }
            else
            {
                ((TreeViewItemAdv)sender).LeftImageSource = (DrawingImage)this.Resources["MetroFolderClose"];
            }
        }
        #endregion

        #region Image Related

        internal void EmbeddedImagesUpdate(Syncfusion.Windows.Reports.DOM.EmbeddedImages embeddedImages)
        {
            TreeViewItemAdv treeViewItem = ReturnTreeNode(RESX.headerImages.ToString());
            treeViewItem.Items.Clear();

            if (embeddedImages != null&&embeddedImages.Count>0)
            {
                this.RaiseEmbeddedImages(embeddedImages);
                foreach (Syncfusion.Windows.Reports.DOM.EmbeddedImage embeddedImage in embeddedImages)
                {
                    this.AddEmbeddedImage(treeViewItem, embeddedImage);
                }
            }
        }

        private void AddEmbeddedImage(Syncfusion.Windows.Reports.DOM.EmbeddedImage embeddedImage)
        {
            TreeViewItemAdv treeViewItem = ReturnTreeNode(RESX.headerImages.ToString());
            this.AddEmbeddedImage(treeViewItem, embeddedImage);
        }

        private void AddEmbeddedImage(TreeViewItemAdv treeViewItem, Syncfusion.Windows.Reports.DOM.EmbeddedImage embeddedImage)
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

        internal void ParametersUpdate(Syncfusion.Windows.Reports.DOM.ReportParameters reportParameters)
        {
            TreeViewItemAdv tvi = this.ReturnTreeNode(RESX.headerParameters.ToString());
            tvi.Items.Clear();
            if (reportParameters != null)
            {
                foreach (Syncfusion.Windows.Reports.DOM.ReportParameter parameter in reportParameters)
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
                    img.Source = this.FindResource("DesignerParameters") as DrawingImage;
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
            this.designpanel.RemoveReportParameter(((TreeViewItemAdv)((MenuItem)sender).Tag).Header.ToString());
        }

        private void MenuAddParameterPropertiesItem_Click(object sender, RoutedEventArgs e)
        {
            this.designpanel.ModifyReportParameter(((TreeViewItemAdv)((MenuItem)sender).Tag).Header.ToString());
        }

        private void MenuAddDataSetItemProperties_Click(object sender, RoutedEventArgs e)
        {
            this.designpanel.ModifyDataSet(((TreeViewItemAdv)((MenuItem)sender).Tag).Header.ToString());
        }

        private void MenuAddDataSetDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            this.designpanel.RemoveDataSet(((TreeViewItemAdv)((MenuItem)sender).Tag).Header.ToString());
        }

        private void MenuAddDataSourceDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            this.designpanel.RemoveDataSource(((TreeViewItemAdv)((MenuItem)sender).Tag).Header.ToString());
        }

        private void Menu_DataSource_Click(object sender, RoutedEventArgs e)
        {
            this.designpanel.AddDataSource();
        }

        private void menuItemField_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItemAdv viewitemadv = this.TreeView1.SelectedItem as TreeViewItemAdv;
            this.designpanel.RemoveDatasetFields(viewitemadv.Parent.ToString(), this.TreeView1.SelectedItem.ToString());
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
            menuItem.Click += new RoutedEventHandler(MenuAddDataSourceItem_Click);
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

        internal void DataSourcesUpdate(Syncfusion.Windows.Reports.DOM.DataSources reportDataSources)
        {
            if (this.DesignerMode == DesignMode.RDLC)
            {
                return;
            }

            TreeViewItemAdv treeViewItem = ReturnTreeNode(RESX.headerDataSources.ToString());
            treeViewItem.Items.Clear();

            if (reportDataSources != null && reportDataSources.Count > 0)
            {
                foreach (Syncfusion.Windows.Reports.DOM.DataSource reportDataSource in reportDataSources)
                {
                    this.AddDataSource(treeViewItem, reportDataSource);
                }
            }
        }

        private void AddDataSource(Syncfusion.Windows.Reports.DOM.DataSource reportDataSource)
        {
            TreeViewItemAdv treeViewItem = ReturnTreeNode(RESX.headerDataSources.ToString());
            this.AddDataSource(treeViewItem, reportDataSource);
        }

        private void AddDataSource(TreeViewItemAdv treeViewItem, Syncfusion.Windows.Reports.DOM.DataSource reportDataSource)
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


        internal void DataSetsUpdate(Syncfusion.Windows.Reports.DOM.DataSets reportDataSets)
        {
            TreeViewItemAdv treeViewItem = ReturnTreeNode(RESX.headerDataSets.ToString());
            treeViewItem.Items.Clear();
            foreach (DOM.DataSet set in reportDataSets)
            {
                string name = set.Query.DataSourceName;
            }
            if (reportDataSets != null && reportDataSets.Count > 0)
            {
                this.RaiseAddDataSets(reportDataSets);
                foreach (Syncfusion.Windows.Reports.DOM.DataSet reportDataSet in reportDataSets)
                {
                    this.AddDataSet(treeViewItem, reportDataSet);
                }
            }     
        }

        private void AddDataSet(Syncfusion.Windows.Reports.DOM.DataSet reportDataSet)
        {
            TreeViewItemAdv treeViewItem = ReturnTreeNode(RESX.headerDataSets.ToString());
            this.AddDataSet(treeViewItem, reportDataSet);
        }

        private void AddDataSet(TreeViewItemAdv treeViewItem, Syncfusion.Windows.Reports.DOM.DataSet reportDataSet)
        {
            TreeViewItemAdv tviDataSet = new TreeViewItemAdv();
            ContextMenu contextMenu = this.AddDataSetContextMenu(tviDataSet);
            tviDataSet.ContextMenu = contextMenu;
            tviDataSet.Header = reportDataSet.Name;
            tviDataSet.IsEditable = false;
            tviDataSet.Tag = reportDataSet;
            tviDataSet.AllowDrop = false;
            tviDataSet.IsExpanded = true;

            foreach (Syncfusion.Windows.Reports.DOM.Field field in reportDataSet.Fields)
            {
                TreeViewItemAdv tviField = new TreeViewItemAdv();
                ContextMenu contextMenuField = new ContextMenu();
                MenuItem menuItemField = new MenuItem();
                menuItemField.Header = RESX.headerDelete.ToString();
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
            this.designpanel.RemoveEmbeddedImage(tvi.Header.ToString());
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

        #region updatestyle_and_returntreenodes

        private void UpdateTreeNodeStyle(TreeViewItemAdv node)
        {
            if (this.VisualStyle == "Metro")
            {
                node.LeftImageSource = (DrawingImage)this.Resources["MetroFolderClose"];
            }
            else
            {
                node.LeftImageSource = (BitmapImage)this.Resources["FolderClose"];
            }

            if (node.Items != null && node.Items.Count > 0 && node.IsExpanded)
            {
                if (this.VisualStyle != "Metro")
                {
                    node.LeftImageSource = (BitmapImage)this.Resources["FolderOpen"];
                }
                else
                {
                    node.LeftImageSource = (DrawingImage)this.Resources["MetroFolderOpen"];
                }
            }
        }

        internal void UpdateTreeStyle(string visualstyle)
        {
            VisualStyle = visualstyle;

            TreeViewItemAdv dataSet = ReturnTreeNode(RESX.headerDataSets.ToString());
            TreeViewItemAdv dataSource = ReturnTreeNode(RESX.headerDataSources.ToString());
            TreeViewItemAdv image = ReturnTreeNode(RESX.headerImages.ToString());
            TreeViewItemAdv parameter = this.ReturnTreeNode(RESX.headerParameters.ToString());
            TreeViewItemAdv buildIn = this.ReturnTreeNode("Built-in Fields");

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

        #region  custom events
        
        internal event UpdateDataSetsEventHandler AddDataSets;

        internal void RaiseAddDataSets(Syncfusion.Windows.Reports.DOM.DataSets dataSet)
        {
            if (this.AddDataSets != null)
            {
                this.AddDataSets(this, new UpdateDataSetsEventArgs {dataSets=dataSet});
            }
        }

        internal event UpdateImagesEventHandler AddEmbeddedImages;

        internal void RaiseEmbeddedImages(Syncfusion.Windows.Reports.DOM.EmbeddedImages embeddedImage)
        {
            if (this.AddEmbeddedImages != null)
            {
                this.AddEmbeddedImages(this, new UpdateImagesEventArgs { embeddedImages = embeddedImage });
            }
        }

        #endregion

        void tvi_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItemAdv dataSource = sender as TreeViewItemAdv;

            var datasets = (from dataset in this.designpanel.DataSets
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
