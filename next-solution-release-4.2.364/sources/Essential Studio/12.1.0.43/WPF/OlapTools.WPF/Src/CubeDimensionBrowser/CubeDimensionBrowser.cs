#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Olap
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Syncfusion.Olap.Data;
    using Syncfusion.Olap.Manager;
    using Syncfusion.Windows.Shared;
    using Syncfusion.Windows.Tools.Controls;
    using System.Linq;
    using System.Collections;
    using Syncfusion.Olap.Reports;
    using Syncfusion.Windows.Shared.Olap;
    using System.Globalization;

    //[SkinType(SkinVisualStyle = Skin.Office2007Blue,
    //Type = typeof(CubeDimensionBrowser), XamlResource = "/Syncfusion.OlapTools.WPF;component/CubeDimensionBrowser/Themes/MainTemplate.xaml")]
    //[SkinType(SkinVisualStyle = Skin.Office2007Black,
    //Type = typeof(CubeDimensionBrowser), XamlResource = "/Syncfusion.OlapTools.WPF;component/CubeDimensionBrowser/Themes/MainTemplate.xaml")]
    //[SkinType(SkinVisualStyle = Skin.Office2007Silver,
    //Type = typeof(CubeDimensionBrowser), XamlResource = "/Syncfusion.OlapTools.WPF;component/CubeDimensionBrowser/Themes/MainTemplate.xaml")]
    //[SkinType(SkinVisualStyle = Skin.Office2003,
    //Type = typeof(CubeDimensionBrowser), XamlResource = "/Syncfusion.OlapTools.WPF;component/CubeDimensionBrowser/Themes/MainTemplate.xaml")]
    //[SkinType(SkinVisualStyle = Skin.Blend,
    //Type = typeof(CubeDimensionBrowser), XamlResource = "/Syncfusion.OlapTools.WPF;component/CubeDimensionBrowser/Themes/MainTemplate.xaml")]
    //[SkinType(SkinVisualStyle = Skin.Default,
    //Type = typeof(CubeDimensionBrowser), XamlResource = "/Syncfusion.OlapTools.WPF;component/CubeDimensionBrowser/Themes/MainTemplate.xaml")]
    /// <summary>
    /// Lists the Meta Data of the Selected Cube
    /// </summary>
    public class CubeDimensionBrowser : TreeView
    {

        #region Variables

        /// <summary>
        /// AllowNodeDragging Dependency Property Implementation
        /// </summary>
        public static readonly DependencyProperty AllowNodeDraggingProperty =
            DependencyProperty.Register("AllowNodeDragging", typeof(bool), typeof(CubeDimensionBrowser), new UIPropertyMetadata(false));

        /// <summary>
        /// IncludeMembers Dependency Property Implementation
        /// </summary>
        public static readonly DependencyProperty IncludeMembersProperty =
            DependencyProperty.Register("IncludeMembers", typeof(bool), typeof(CubeDimensionBrowser), new UIPropertyMetadata(false));

        /// <summary>
        /// OlapDataManager Dependency Property Implementation
        /// </summary>
        public static readonly DependencyProperty OlapDataManagerProperty =
            DependencyProperty.Register("OlapDataManager", typeof(IOlapDataManager), typeof(CubeDimensionBrowser), new UIPropertyMetadata(CubeDimensionBrowser.OlapDataManagerChanged));


        public MetaTreeNode MetaTreenode
        {
            get { return (MetaTreeNode)GetValue(MetaTreenodeProperty); }
            set { SetValue(MetaTreenodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MetaTreenodeProperty =
            DependencyProperty.Register("MetaTreenode", typeof(MetaTreeNode), typeof(CubeDimensionBrowser), new UIPropertyMetadata(CubeDimensionBrowser.MetatTreeChanged));


        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the <see cref="CubeDimensionBrowser"/> class.
        /// </summary>
        static CubeDimensionBrowser()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CubeDimensionBrowser), new FrameworkPropertyMetadata(typeof(CubeDimensionBrowser)));
        }
        public CubeDimensionBrowser()
        {
            ResourceDictionary rs = new ResourceDictionary();
            rs.Source = new Uri(@"Syncfusion.OlapTools.WPF;component/CubeDimensionBrowser/Themes/MainTemplate.xaml", UriKind.RelativeOrAbsolute);
            this.ItemTemplate = rs["PART_HierarchicalTemplate"] as DataTemplate;
            this.Style = rs["DefaultCubeDimensionBrowser"] as Style;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow node dragging].
        /// </summary>
        /// <value><c>true</c> if [allow node dragging]; otherwise, <c>false</c>.</value>
        public bool AllowNodeDragging
        {
            get { return (bool)GetValue(AllowNodeDraggingProperty); }
            set { SetValue(AllowNodeDraggingProperty, value); }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the cube.
        /// </summary>
        /// <value>The name of the cube.</value>
        public string CubeName { get; set; }
        string calc_name;
        private ContextMenu Contextmenu = new ContextMenu();
        /// <summary>
        /// Gets or sets a value indicating whether [include members].
        /// </summary>
        /// <value><c>true</c> if [include members]; otherwise, <c>false</c>.</value>
        public bool IncludeMembers
        {
            get { return (bool)GetValue(IncludeMembersProperty); }
            set { SetValue(IncludeMembersProperty, value); }
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public IOlapDataManager OlapDataManager
        {
            get { return (IOlapDataManager)GetValue(OlapDataManagerProperty); }
            set { SetValue(OlapDataManagerProperty, value); }
        }

        /// <summary>
        /// Gets or sets value for enabling sorting Measures in CubeDimensionBrowser
        /// </summary>


        public SortCubeMeasureOrder MeasureSortOrderInCubeBrowser
        {
            get { return (SortCubeMeasureOrder)GetValue(MeasureSortOrderInCubeBrowserProperty); }
            set { SetValue(MeasureSortOrderInCubeBrowserProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowCubeBrowserMeasureSorting.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MeasureSortOrderInCubeBrowserProperty =
            DependencyProperty.Register("MeasureSortOrderInCubeBrowser", typeof(SortCubeMeasureOrder), typeof(CubeDimensionBrowser), new UIPropertyMetadata(SortCubeMeasureOrder.ASC,
                (obj, args) => 
                {
                    CubeDimensionBrowser cubeDimensionBrowser = (CubeDimensionBrowser)obj;
                    if (cubeDimensionBrowser.OlapDataManager != null)
                    {
                        cubeDimensionBrowser.InitilizeClientDimensionBrowser();
                    }
                }));

        /// <summary>
        /// Gets or sets the measure group caption
        /// </summary>
        public System.Collections.Generic.Dictionary<string,string> MeasureGroupNameCaption
        {
            get { return (System.Collections.Generic.Dictionary<string,string>)GetValue(MeasureGroupNameCaptionProperty); }
            set { SetValue(MeasureGroupNameCaptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MeasureGroupNameCaption.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Declaring dependency property to get the measure group caption
        /// </summary>
        public static readonly DependencyProperty MeasureGroupNameCaptionProperty =
            DependencyProperty.Register("MeasureGroupNameCaption", typeof(System.Collections.Generic.Dictionary<string, string>), typeof(CubeDimensionBrowser), new UIPropertyMetadata(null,
                (obj, arg) =>
                {
                    CubeDimensionBrowser cubeDimensionBrowser = (CubeDimensionBrowser)obj;
                    if (cubeDimensionBrowser.OlapDataManager != null)
                    {
                        cubeDimensionBrowser.InitilizeClientDimensionBrowser();
                    }
                }));

        #endregion

        #region Events

        private void ClientDimensionBrowserItem_Expanded(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is TreeViewItem)
            {
                try
                {
                    //// GetTopParent method is available in the same class to load the top level window and to display the 
                    //// waiting dialog., 
                    TreeViewItem item = (TreeViewItem)e.OriginalSource;
                    item.IsSelected = true;
                    MetaTreeNode mtn = (MetaTreeNode)this.SelectedItem;
                    MetaTreeHelper.ForceFillChildNodes(mtn);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void CubeDimensionBrowser_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(MetaTreeNode)))
            {
                MetaTreeNode mtNode = (MetaTreeNode)e.Data.GetData(typeof(MetaTreeNode));
                if (mtNode.NodeType != MetaTreeNodeType.Cube && mtNode.NodeType != MetaTreeNodeType.MeasureGroup && mtNode.NodeType != MetaTreeNodeType.DisplayFolder && mtNode.NodeType != MetaTreeNodeType.None && mtNode.NodeType != MetaTreeNodeType.KPI_ROOT && mtNode.NodeType != MetaTreeNodeType.CalculatedMemberGroup)
                {
                    e.Effects = DragDropEffects.Move;
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }
        private void CubeDimensionBrowser_NodeRightClicked(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                try
                {
                    bool a = false;
                    if (this.Contextmenu != null)
                        this.Contextmenu.Items.Clear();
                    if (e.OriginalSource is TextBlock)
                    {
                        calc_name = (e.OriginalSource as TextBlock).Text;
                        for (int i = 0; i < this.OlapDataManager.CurrentReport.CalculatedMembers.Count; i++) 
                        {
                            if ((calc_name == this.OlapDataManager.CurrentReport.CalculatedMembers[i].ElementValue.ElementName) && ((((System.Windows.FrameworkElement)(sender)).Name) != "CalcMeasureTreeView"))
                                a = true;
                        }
                        for (int i = 0; i < this.OlapDataManager.CurrentReport.VirtualKpiElements.Count; i++)
                        {
                            if ((calc_name == this.OlapDataManager.CurrentReport.VirtualKpiElements[i].ElementValue.ElementName) && ((((System.Windows.FrameworkElement)(sender)).Name) == "cubeDimensionBrowser"))
                                a = true;
                        }
                    }
                    if (a)
                    {
                        MenuItem item = new MenuItem { Header = SR.GetString(CultureInfo.CurrentUICulture, "CubeDimensionBrowser_RightClick_Remove") };
                        this.Contextmenu.Items.Add(item);
                        MenuItem edit = new MenuItem { Header = SR.GetString(CultureInfo.CurrentUICulture, "CubeDimensionBrowser_RightClick_Edit") };
                        for (int i = 0; i < this.OlapDataManager.CurrentReport.VirtualKpiElements.Count; i++)
                        {
                            if ((calc_name == this.OlapDataManager.CurrentReport.VirtualKpiElements[i].ElementValue.ElementName) && ((((System.Windows.FrameworkElement)(sender)).Name) == "cubeDimensionBrowser"))
                                this.Contextmenu.Items.Add(edit);
                        }
                        edit.Click += new RoutedEventHandler(edit_Click);
                        this.Contextmenu.IsOpen = true;
                        item.Click += new RoutedEventHandler(item_Click);
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        void edit_Click(object sender, RoutedEventArgs e)
        {
            KPIEditor kpiEditor = new KPIEditor(this.OlapDataManager as OlapDataManager);
            SkinStorage.SetVisualStyle(kpiEditor, SkinStorage.GetVisualStyle(this));
            Window parentWindow = Common.GetParentWindow<Window>(this);
            if (parentWindow != null)
            {
                kpiEditor.Owner = parentWindow;
                kpiEditor.ShowInTaskbar = false;
            }
            for (int i = 0; i < this.OlapDataManager.CurrentReport.VirtualKpiElements.Count; i++)
            {
                if ((calc_name == this.OlapDataManager.CurrentReport.VirtualKpiElements[i].ElementValue.ElementName) && ((((System.Windows.FrameworkElement)(sender)).Name) != "cubeDimensionBrowser"))
                {
                    kpiEditor.txtKpiName.Text = calc_name;
                    kpiEditor.isEdit = true;
                    if (((this.OlapDataManager.CurrentReport.VirtualKpiElements[i].ElementValue as VirtualKpiElement).KpiValueExpression != null))
                        kpiEditor.txtValueExpression.Text = ((this.OlapDataManager.CurrentReport.VirtualKpiElements[i].ElementValue as VirtualKpiElement)).KpiValueExpression;
                    else
                        kpiEditor._value.IsEnabled = false;
                    if (((this.OlapDataManager.CurrentReport.VirtualKpiElements[i].ElementValue as VirtualKpiElement).KpiGoalExpression != null))
                        kpiEditor.txtGoalExpression.Text = ((this.OlapDataManager.CurrentReport.VirtualKpiElements[i].ElementValue as VirtualKpiElement)).KpiGoalExpression;
                    else
                        kpiEditor._goal.IsEnabled = false;
                    if (((this.OlapDataManager.CurrentReport.VirtualKpiElements[i].ElementValue as VirtualKpiElement).KpiStatusExpression != null))
                        kpiEditor.txtStatusExpression.Text = ((this.OlapDataManager.CurrentReport.VirtualKpiElements[i].ElementValue as VirtualKpiElement)).KpiStatusExpression;
                    else
                        kpiEditor._status.IsEnabled = false;
                    if (((this.OlapDataManager.CurrentReport.VirtualKpiElements[i].ElementValue as VirtualKpiElement).KpiTrendExpression != null))
                        kpiEditor.txtTrendExpression.Text = ((this.OlapDataManager.CurrentReport.VirtualKpiElements[i].ElementValue as VirtualKpiElement)).KpiTrendExpression;
                    else
                        kpiEditor._trend.IsEnabled = false;
                    kpiEditor.ShowDialog();
                }
            }
            
            
        }
        /// <summary>
        /// Handles the Right Click event when Context menu is been called.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.RoutedEventArgs"/> instance containing the event data.</param>
        void item_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this.OlapDataManager.CurrentReport.CalculatedMembers.Count; i++)
            {
                if (calc_name == this.OlapDataManager.CurrentReport.CalculatedMembers[i].ElementValue.ElementName)
                    this.OlapDataManager.CurrentReport.CalculatedMembers.RemoveAt(i);
            }
            for (int i = 0; i < this.OlapDataManager.CurrentReport.VirtualKpiElements.Count; i++)
            {
                if (calc_name == this.OlapDataManager.CurrentReport.VirtualKpiElements[i].ElementValue.ElementName)
                    this.OlapDataManager.CurrentReport.VirtualKpiElements.RemoveAt(i);
            }
            for (int i = 0; i < (this.OlapDataManager as OlapDataManager).VirtualKpiElements.Count; i++)
            {
                if (calc_name == (this.OlapDataManager as OlapDataManager).VirtualKpiElements[i].ElementValue.ElementName)
                    (this.OlapDataManager as OlapDataManager).VirtualKpiElements.RemoveAt(i);
            }
            // Removing Calculated Members from the Categorical Elements
            if (this.OlapDataManager.CurrentReport.CategoricalElements.Count > 0)
            {
                for (int i = 0; i < this.OlapDataManager.CurrentReport.CategoricalElements.Count; i++)
                {
                    if (calc_name == this.OlapDataManager.CurrentReport.CategoricalElements[i].ElementValue.ElementName)
                        this.OlapDataManager.CurrentReport.CategoricalElements.RemoveAt(i);
                }
            }
            // Removing Calculated Members from the Series Elements
            if (this.OlapDataManager.CurrentReport.SeriesElements.Count > 0)
            {
                for (int i = 0; i < this.OlapDataManager.CurrentReport.SeriesElements.Count; i++)
                {
                    if (calc_name == this.OlapDataManager.CurrentReport.SeriesElements[i].ElementValue.ElementName)
                        this.OlapDataManager.CurrentReport.SeriesElements.RemoveAt(i);
                }
            }
            // Removing Calculated Members from the Slicer Elements
            if (this.OlapDataManager.CurrentReport.SlicerElements.Count > 0)
            {
                for (int i = 0; i < this.OlapDataManager.CurrentReport.SlicerElements.Count; i++)
                {
                    if (this.SelectedItem.ToString() == this.OlapDataManager.CurrentReport.SlicerElements[i].ElementValue.ElementName)
                        this.OlapDataManager.CurrentReport.SlicerElements.RemoveAt(i);
                }
            }
            (this.OlapDataManager as OlapDataManager).NotifyElementModified();
            this.RefreshNodes();
        }
        private void CubeDimensionBrowser_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                try
                {
                    if (!(e.OriginalSource is System.Windows.Controls.Primitives.Thumb))
                    {
                        if (this.SelectedItem is MetaTreeNode)
                        {
                            MetaTreeNode mtNode = (MetaTreeNode)this.SelectedItem;
                            if (mtNode.NodeType != MetaTreeNodeType.Cube && mtNode.NodeType != MetaTreeNodeType.MeasureGroup && mtNode.NodeType != MetaTreeNodeType.DisplayFolder && mtNode.NodeType != MetaTreeNodeType.None && mtNode.NodeType != MetaTreeNodeType.KPI_ROOT && mtNode.NodeType != MetaTreeNodeType.CalculatedMemberGroup && mtNode.NodeType != MetaTreeNodeType.VirtualKPIGroup)
                            {
                                if (!(mtNode.NodeType == MetaTreeNodeType.Member && this.Name == "cubeDimensionBrowser")) //We do not allow the drag support for Members in Main view instead Calculated Member Editor view
                                {
                                    DragDropEffects allowedEffects = DragDropEffects.Move;
                                    var dataObject = new DataObject(this.SelectedItem);
                                    dataObject.SetData("DragSource", this);
                                    DragDrop.DoDragDrop(this, dataObject, allowedEffects);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception Message");
                }
            }
        }

        /// <summary>
        /// Cubes the model changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OlapDataManagerChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            CubeDimensionBrowser cubeDimensionBrowser = (CubeDimensionBrowser)dependencyObject;
            if (cubeDimensionBrowser.OlapDataManager != null)
            {
                cubeDimensionBrowser.InitilizeClientDimensionBrowser();
            }
        }

        /// <summary>
        /// Cubes the model changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void MetatTreeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            CubeDimensionBrowser cubeDimensionBrowser = (CubeDimensionBrowser)dependencyObject;
            if (cubeDimensionBrowser.OlapDataManager != null)
            {
                cubeDimensionBrowser.InitilizeClientDimensionBrowser();
            }
        }

        #endregion

        #region Private Methods

        private Window GetTopParent()
        {
            DependencyObject dpParent = this.Parent;
            do
            {
                dpParent = LogicalTreeHelper.GetParent(dpParent);
            } while (dpParent.GetType().BaseType != typeof(RibbonWindow) && dpParent.GetType().BaseType != typeof(ChromelessWindow));

            return dpParent as Window;
        }

        private void InitilizeClientDimensionBrowser()
        {
            if (this.OlapDataManager != null)
            {
                this.OlapDataManager.CubeChanged -= new CubeChangedEventHandler(this.OlapDataManager_CubeChanged);
                this.OlapDataManager.CubeChanged += new CubeChangedEventHandler(this.OlapDataManager_CubeChanged);
                this.OlapDataManager.AxisElementChanged -= new AxisElementChangedEventHandler(OlapDataManager_AxisElementChanged);
                this.OlapDataManager.AxisElementChanged += new AxisElementChangedEventHandler(OlapDataManager_AxisElementChanged);
                (this.OlapDataManager as OlapDataManager).ActiveReportChanged += new ActiveReportChangedEventHandler(CubeDimensionBrowser_ActiveReportChanged); 
                //// Initially Loading the nodes
                RefreshNodes();
            }
        }



        void CubeDimensionBrowser_ActiveReportChanged(object sender, ActiveReportChangedEventArgs e)
        {
            if (this.Items.Count > 0 && this.Items[0] is MetaTreeNode && (this.OlapDataManager as OlapDataManager).UseSharedDataManager && (this.OlapDataManager as OlapDataManager).CalculatedMembers.Count > 0)
            {
                MetaTreeNode calcMemberHeaderNode = (this.Items[0] as MetaTreeNode).ChildNodes.Select(i => i).Where(j => j.NodeType == MetaTreeNodeType.CalculatedMemberGroup).FirstOrDefault();
                if (calcMemberHeaderNode != null)
                {
                    bool isCalcMembersUpdated = true;
                    foreach (var item in (this.OlapDataManager as OlapDataManager).CalculatedMembers)
                    {
                        if (!(item.ElementValue is CalculatedMember && calcMemberHeaderNode.ChildNodes.Any(i => i.UniqueName.ToUpper() == (item.ElementValue as CalculatedMember).UniqueName.ToUpper())))
                        {
                            isCalcMembersUpdated = false;
                            break;
                        }
                    }
                    if (!isCalcMembersUpdated)
                    {
                        MetaTreeHelper.FillMetaTreeNode(calcMemberHeaderNode, (this.OlapDataManager as OlapDataManager).CalculatedMembers, true);
                        this.Items.Refresh();
                    }
                }
                else
                {
                    RefreshNodes();
                }

            }
           if (this.Items.Count > 0 && this.Items[0] is MetaTreeNode && (this.OlapDataManager as OlapDataManager).VirtualKpiElements.Count > 0)
            {
                MetaTreeNode virtualKPINode = (this.Items[0] as MetaTreeNode).ChildNodes.Where(j => j.NodeType == MetaTreeNodeType.VirtualKPIGroup).FirstOrDefault();
                if (virtualKPINode != null)
                {
                    bool isKPIUpdated = true;
                    foreach (var item in (this.OlapDataManager as OlapDataManager).VirtualKpiElements)
                    {
                        if (!(item.ElementValue is VirtualKpiElement && virtualKPINode.ChildNodes.Any(i => i.UniqueName.ToUpper() == (item.ElementValue as VirtualKpiElement).UniqueName.ToUpper())))
                        {
                            isKPIUpdated = false;
                            break;
                        }
                    }
                    if (!isKPIUpdated)
                    {
                        MetaTreeHelper.FillMetaTreeNode(virtualKPINode, true, (this.OlapDataManager as OlapDataManager).VirtualKpiElements);
                        this.Items.Refresh();
                    }
                }
                else
                    RefreshNodes();
            }
        }

        //private void OlapDataManager_ReportChanged(object sender, ReportChangedEventArgs e)
        //{
        //    MetaTreeNode metaTreeNode = (MetaTreeNode)this.SelectedItem;
        //    Syncfusion.Olap.Manager.OlapDataManager olapDataManager = sender as Syncfusion.Olap.Manager.OlapDataManager;
        //    Syncfusion.Olap.Reports.MeasureElements measureElements = FindMeasureAxis(olapDataManager);
        //    if (metaTreeNode != null)
        //    {
        //        UpdateMeasureElements(measureElements, metaTreeNode);
        //    }
        //}

        private Syncfusion.Olap.Reports.MeasureElements FindMeasureAxis(OlapDataManager olapDataManager)
        {
            Syncfusion.Olap.Reports.MeasureElements measureElements = null;
            measureElements = FindMeasureInItems((olapDataManager.UseSharedDataManager && olapDataManager.ActiveReport != null)?olapDataManager.ActiveReport.CategoricalElements: olapDataManager.CurrentReport.CategoricalElements);
            if (measureElements == null)
            {
                measureElements = FindMeasureInItems((olapDataManager.UseSharedDataManager && olapDataManager.ActiveReport != null) ? olapDataManager.ActiveReport.SeriesElements : olapDataManager.CurrentReport.SeriesElements);
            }
            if (measureElements == null)
            {
                measureElements = FindMeasureInItems((olapDataManager.UseSharedDataManager && olapDataManager.ActiveReport != null) ? olapDataManager.ActiveReport.SlicerElements : olapDataManager.CurrentReport.SlicerElements);
            }
            return measureElements;
        }

        private Syncfusion.Olap.Reports.MeasureElements FindMeasureInItems(Syncfusion.Olap.Reports.Items items)
        {
            foreach (Syncfusion.Olap.Reports.Item item in items)
            {
                if (item.ElementValue is Syncfusion.Olap.Reports.MeasureElements)
                {
                    return item.ElementValue as Syncfusion.Olap.Reports.MeasureElements;
                }
            }
            return null;
        }

        private void UpdateMeasureElements(Syncfusion.Olap.Reports.MeasureElements measureElements, MetaTreeNode metaTreeNodeMeasures)
        {
            if (metaTreeNodeMeasures.NodeType == MetaTreeNodeType.DisplayFolder
                || metaTreeNodeMeasures.NodeType == MetaTreeNodeType.MeasureGroup)
            {
                if (metaTreeNodeMeasures.ChildNodes.Count > 0)
                {
                    foreach (MetaTreeNode measureNode in metaTreeNodeMeasures.ChildNodes)
                    {
                        UpdateMeasureElements(measureElements, measureNode);
                    }
                }
            }
            else if (metaTreeNodeMeasures.NodeType == MetaTreeNodeType.Measure)
            {
                if (metaTreeNodeMeasures.Properties.Count > 0)
                {
                    Measure measureObj = metaTreeNodeMeasures.Properties[0].Value as Measure;
                    {
                        if (measureObj != null)
                        {
                            if (measureElements != null)
                            {
                                int elementCount = measureElements.Elements.Count;
                                for (int i = 0; i < elementCount; i++)
                                {
                                    Syncfusion.Olap.Reports.MeasureElement measureElement = (Syncfusion.Olap.Reports.MeasureElement)measureElements.Elements[i];
                                    Property property = measureElement.Properties.FindByName(PropertyConstants.MeasrueNodeName);
                                    if (property != null)
                                    {
                                        MetaTreeNode metaTreeNode = (MetaTreeNode)property.Value;
                                        metaTreeNode.IsSelected = true;
                                        metaTreeNode.AcceptIsSelectedChanges(false);
                                    }
                                    else if (measureElement.UniqueName.ToLower() == measureObj.UniqueName.ToLower())
                                    {
                                        metaTreeNodeMeasures.IsSelected = true;
                                        metaTreeNodeMeasures.AcceptIsSelectedChanges(false);
                                    }
                                    else
                                    {
                                        metaTreeNodeMeasures.IsSelected = false;
                                        metaTreeNodeMeasures.AcceptIsSelectedChanges(false);
                                    }
                                }
                            }
                            else
                            {
                                metaTreeNodeMeasures.IsSelected = false;
                                metaTreeNodeMeasures.AcceptIsSelectedChanges(false);
                                return;
                            }
                        }
                    }
                }
            }
            else if (metaTreeNodeMeasures.NodeType == MetaTreeNodeType.Cube)
            {
                foreach (MetaTreeNode childMetaTreeNode in metaTreeNodeMeasures.ChildNodes)
                {
                    UpdateMeasureElements(measureElements, childMetaTreeNode);
                }
            }
        }

        private void OlapDataManager_CubeChanged(object sender, EventArgs e)
        {
            RefreshNodes();
        }

        private void OlapDataManager_AxisElementChanged(object sender, AxisElementChangedEventArgs e)
        {
            if ((this.OlapDataManager as OlapDataManager).UseSharedDataManager)
            {
                return;
            }
            if (this.Items.Count > 0 && this.Items[0] is MetaTreeNode && this.OlapDataManager.CurrentReport != null && this.OlapDataManager.CurrentReport.CalculatedMembers.Count > 0)
            {
                MetaTreeNode calcMemberHeaderNode = (this.Items[0] as MetaTreeNode).ChildNodes.Select(i => i).Where(j => j.NodeType == MetaTreeNodeType.CalculatedMemberGroup).FirstOrDefault();
                if (calcMemberHeaderNode != null)
                {
                    bool isCalcMembersUpdated = true;
                    foreach (var item in this.OlapDataManager.CurrentReport.CalculatedMembers)
                    {
                        if (!(item.ElementValue is CalculatedMember && calcMemberHeaderNode.ChildNodes.Any(i => i.UniqueName.ToUpper() == (item.ElementValue as CalculatedMember).UniqueName.ToUpper())))
                        {
                            isCalcMembersUpdated = false;
                            break;
                        }
                    }
                    if (!isCalcMembersUpdated)
                    {
                        MetaTreeHelper.FillMetaTreeNode(calcMemberHeaderNode, this.OlapDataManager.CurrentReport.CalculatedMembers, true);
                        this.Items.Refresh();
                    }
                }
                else
                {
                    RefreshNodes();
                }
                
            }
            else if (this.Items.Count > 0 && this.Items[0] is MetaTreeNode && this.OlapDataManager.CurrentReport != null && this.OlapDataManager.CurrentReport.VirtualKpiElements.Count > 0)
            {
                MetaTreeNode virtualKPINode = (this.Items[0] as MetaTreeNode).ChildNodes.Where(j => j.NodeType == MetaTreeNodeType.VirtualKPIGroup).FirstOrDefault();
                if (virtualKPINode != null)
                {
                    bool isKPIUpdated = true;
                    foreach (var item in this.OlapDataManager.CurrentReport.VirtualKpiElements)
                    {
                        if (!(item.ElementValue is VirtualKpiElement && virtualKPINode.ChildNodes.Any(i => i.UniqueName.ToUpper() == (item.ElementValue as VirtualKpiElement).UniqueName.ToUpper())))
                        {
                            isKPIUpdated = false;
                            break;
                        }
                    }
                    if (!isKPIUpdated)
                    {
                        MetaTreeHelper.FillMetaTreeNode(virtualKPINode, true, this.OlapDataManager.CurrentReport.VirtualKpiElements);
                        this.Items.Refresh();
                    }
                }
                else
                    RefreshNodes();
            }
        }

        /// <summary>
        /// Refreshes the nodes.
        /// </summary>
        internal void RefreshNodes()
        {
            if (this.OlapDataManager != null &&
                this.OlapDataManager.CurrentCubeName != null &&
                this.OlapDataManager.CurrentCubeName != string.Empty)
            {
                if ((this.OlapDataManager as OlapDataManager).UseSharedDataManager)
                {
                    // Temp Node for displaying it in Hierarchy
                    MetaTreeNodeCollection temp_mtnCollection = new MetaTreeNodeCollection(null);

                    //// Adding the cube node to the temp collection
                    MetaTreeNode mtNodeCube = new MetaTreeNode();
                    //mtNodeCube.OlapDataManager = this.OlapDataManager;
                    mtNodeCube.NodeType = MetaTreeNodeType.Cube;
                    mtNodeCube.Name = this.OlapDataManager.CurrentCubeName;
                    mtNodeCube.Caption = this.OlapDataManager.CurrentCubeName;
                    mtNodeCube.NodeCheckedType = MetaTreeNodeCheckedType.SomeChildChecked;
                    temp_mtnCollection.Add(mtNodeCube);

                    CubeSchema cubeSchema = this.OlapDataManager.CurrentCubeSchema;

                    //// Fill the calculated members if any.
                    if ((this.OlapDataManager as OlapDataManager).CalculatedMembers.Count > 0)
                    {
                        MetaTreeHelper.FillMetaTreeNode(mtNodeCube, (this.OlapDataManager as OlapDataManager).CalculatedMembers, false);
                    }
                    //// Filling the cubeNode with Measures
                        MetaTreeHelper.FillMetaTreeNode(mtNodeCube, cubeSchema.Measures, true, false,this.MeasureSortOrderInCubeBrowser,this.MeasureGroupNameCaption);

                    //// Filling the cubeNode with KPI's
                    if (cubeSchema.Kpis.Count > 0)
                    {
                        MetaTreeHelper.FillMetaTreeNode(mtNodeCube, cubeSchema.Kpis, true, false);
                    }

                    if ((this.OlapDataManager as OlapDataManager).VirtualKpiElements.Count > 0)
                    {
                        MetaTreeHelper.FillMetaTreeNode(mtNodeCube, false, (this.OlapDataManager as OlapDataManager).VirtualKpiElements);
                    }
                    //// Filling the cubeNode with Dimensions
                    MetaTreeHelper.FillMetaTreeNode(mtNodeCube, this.IncludeMembers, cubeSchema.Dimensions, cubeSchema.NamedSets);
                    this.ItemsSource = temp_mtnCollection;
                }
                else
                {
                    // Temp Node for displaying it in Hierarchy
                    MetaTreeNodeCollection temp_mtnCollection = new MetaTreeNodeCollection(null);

                    //// Adding the cube node to the temp collection
                    MetaTreeNode mtNodeCube = new MetaTreeNode();
                    //mtNodeCube.OlapDataManager = this.OlapDataManager;
                    mtNodeCube.NodeType = MetaTreeNodeType.Cube;
                    mtNodeCube.Name = this.OlapDataManager.CurrentCubeName;
                    mtNodeCube.Caption = this.OlapDataManager.CurrentCubeName;
                    mtNodeCube.NodeCheckedType = MetaTreeNodeCheckedType.SomeChildChecked;
                    temp_mtnCollection.Add(mtNodeCube);

                    CubeSchema cubeSchema = this.OlapDataManager.CurrentCubeSchema;

                    //// Fill the calculated members if any.
                    if (this.OlapDataManager.CurrentReport != null && this.OlapDataManager.CurrentReport.CalculatedMembers.Count > 0)
                    {
                        MetaTreeHelper.FillMetaTreeNode(mtNodeCube, this.OlapDataManager.CurrentReport.CalculatedMembers, false);
                    }
                    //// Filling the cubeNode with Measures
                        MetaTreeHelper.FillMetaTreeNode(mtNodeCube, cubeSchema.Measures, true, false,this.MeasureSortOrderInCubeBrowser,this.MeasureGroupNameCaption);

                    //// Filling the cubeNode with KPI's
                    if (cubeSchema.Kpis.Count > 0)
                    {
                        MetaTreeHelper.FillMetaTreeNode(mtNodeCube, cubeSchema.Kpis, true, false);
                    }

                    if (this.OlapDataManager.CurrentReport != null && this.OlapDataManager.CurrentReport.VirtualKpiElements.Count > 0)
                    {
                        MetaTreeHelper.FillMetaTreeNode(mtNodeCube, false, this.OlapDataManager.CurrentReport.VirtualKpiElements);
                    }
                    //// Filling the cubeNode with Dimensions
                    MetaTreeHelper.FillMetaTreeNode(mtNodeCube, this.IncludeMembers, cubeSchema.Dimensions, cubeSchema.NamedSets);
                    this.ItemsSource = temp_mtnCollection;
                }
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.AddHandler(TreeViewItem.ExpandedEvent, new RoutedEventHandler(this.ClientDimensionBrowserItem_Expanded));
            this.AddHandler(CubeDimensionBrowser.MouseRightButtonDownEvent,new System.Windows.Input.MouseButtonEventHandler(this.CubeDimensionBrowser_NodeRightClicked));
            if (this.AllowNodeDragging)
            { 
                this.AddHandler(CubeDimensionBrowser.MouseMoveEvent, new System.Windows.Input.MouseEventHandler(this.CubeDimensionBrowser_MouseMove));
                this.AddHandler(CubeDimensionBrowser.DragOverEvent, new DragEventHandler(this.CubeDimensionBrowser_DragOver));
            }
        }



        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property.Name.Equals("VisualStyle"))
            {
                ResourceDictionary rs = new ResourceDictionary();
                rs.Source = new Uri("/Syncfusion.OlapTools.WPF;component/CubeDimensionBrowser/Themes/MainTemplate.xaml", UriKind.RelativeOrAbsolute);
                switch (e.NewValue.ToString())
                {
                    case "Default":
                        this.Style = rs["DefaultCubeDimensionBrowser"] as Style;
                        break;
                    case "Office2003":
                        this.Style = rs["Office2003CubeDimensionBrowser"] as Style;
                        break;
                    case "Blend":
                        this.Style = rs["BlendCubeDimensionBrowser"] as Style;
                        break;
                    case "Office2007Blue":
                        this.Style = rs["Office2007BlueCubeDimensionBrowser"] as Style;
                        break;
                    case "Office2007Black":
                        this.Style = rs["Office2007BlackCubeDimensionBrowser"] as Style;
                        break;
                    case "Office2007Silver":
                        this.Style = rs["Office2007SilverCubeDimensionBrowser"] as Style;
                        break;
                    case "Office2010Blue":
                        this.Style = rs["Office2010BlueCubeDimensionBrowser"] as Style;
                        break;
                    case "Office2010Black":
                        this.Style = rs["Office2007BlackCubeDimensionBrowser"] as Style;
                        break;
                    case "Office2010Silver":
                        this.Style = rs["Office2010SilverCubeDimensionBrowser"] as Style;
                        break;
                    case "Metro":
                        this.Style = rs["MetroCubeDimensionBrowser"] as Style;
                        break;
                    case "Transparent":
                        this.Style = rs["TransparentCubeDimensionBrowser"] as Style;
                        break;
                    default:
                        this.Style = rs["DefaultCubeDimensionBrowser"] as Style;
                        break;
                }
            }
        }
        #endregion
    }
}
