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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.OlapSilverlight.Manager;
using Syncfusion.OlapSilverlight.Data;
using Syncfusion.OlapSilverlight.Common;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using Syncfusion.OlapSilverlight.Reports;
using Syncfusion.Windows.Shared;
using Syncfusion.Silverlight.Client.Olap.Resources;
using System.Globalization;
using Syncfusion.Windows.Controls.Theming;

namespace Syncfusion.Silverlight.Tools.Olap
{
    /// <summary>
    /// TreeView control to display the metadata of a cube.
    /// </summary>
    [DesignTimeVisible(false)]
    public class CubeDimensionBrowser : CDTreeView
    {

        #region Private members

        OlapDataManager _olapDataManager;
        const string  resourceUri = @"/Syncfusion.OlapClient.Silverlight;component/Tools/CubeDimensionBrowser/ImageDictionary.xaml";
        ResourceDictionary resourceDictionary = new ResourceDictionary();

        private DimensionCollection _removedDimensions;


        public List<String> _removedElements;
        private ContextMenuAdv ContextMenu = new ContextMenuAdv();
        string calc_name;
        
        #endregion

         #region Delegate
        /// <summary>
        /// Declaring delegate for tracing Nodeimagechanging event
        /// </summary>
        /// <param name="e"></param>
        public delegate void NodeImageChangingDelegate(NodeImageChangingEvntArgs e);

        /// <summary>
        /// Declaring delegate for setting caption value for the MeasureGroup
        /// </summary>
        /// <param name="e"></param>
        public delegate void MeasureGroupCaptionDelegate(MeasureGroupCaptionEventArgs e);

        #endregion


        #region Event
        /// <summary>
        /// Declaring event for tracking NodeImage changes
        /// </summary>
        public event NodeImageChangingDelegate NodeImageChanging;

        /// <summary>
        /// Declaring event for setting caption value for the MeasureGroup
        /// </summary>
        public event MeasureGroupCaptionDelegate Updating;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CubeDimensionBrowser"/> class.
        /// </summary>
        public CubeDimensionBrowser()
        {
            this.BorderBrush = (Brush)new SolidColorBrush(Colors.Gray);
            this.BorderThickness = new Thickness(0.5);
            this.DefaultStyleKey = typeof(CubeDimensionBrowser);
            resourceDictionary.Source = new Uri(resourceUri, UriKind.RelativeOrAbsolute);
            this.NodeExpanded += CubeDimensionBrowser_NodeExpanded;
            this.NodeCollapsed += CubeDimensionBrowser_NodeCollapsed;
            this.NodeClicked += CubeDimensionBrowser_NodeClicked;
            this.NodeReleased += CubeDimensionBrowser_NodeReleased;
            this.MouseRightButtonDown += new MouseButtonEventHandler(CubeDimensionBrowser_MouseRightButtonDown);
        }
        

        #endregion

        #region Dependency Property

        /// <summary>
        /// Gets or sets the corner radius.
        /// </summary>
        /// <value>The corner radius.</value>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(CubeDimensionBrowser), new PropertyMetadata(null));

        #region TreeViewItemStyle

        /// <summary>
        /// Gets or sets the TreeViewItemStyle.
        /// </summary>
        /// <value>The corner radius.</value>
        public Style TreeViewItemStyle
        {
            get { return (Style)GetValue(TreeViewItemStyleProperty); }
            set { SetValue(TreeViewItemStyleProperty, value); }
        }

        public static readonly DependencyProperty TreeViewItemStyleProperty =
            DependencyProperty.Register("TreeViewItemStyle", typeof(Style), typeof(CubeDimensionBrowser), new PropertyMetadata(null, OnTreeViewItemStyleChanged));

        private static void OnTreeViewItemStyleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            System.Windows.ResourceDictionary source = new System.Windows.ResourceDictionary();
            source.Source = new Uri("/Syncfusion.OlapClient.Silverlight;component/Tools/CubeDimensionBrowser/CubeDimensionBrowser.xaml", UriKind.RelativeOrAbsolute);
            CubeDimensionBrowser c = (CubeDimensionBrowser)sender;
            c.ItemContainerStyle = e.NewValue as Style;
            c.Refresh();
        }
        #endregion

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the olap data manager.
        /// </summary>
        /// <value>The olap data manager.</value>
        public OlapDataManager OlapDataManager
        {
            get
            {
                return _olapDataManager;
            }
            set
            {
                _olapDataManager = value;
                if (_olapDataManager != null)
                {
                    _olapDataManager.AxisElementChanged -= new AxisElementChangedEventHandler(OlapDataManager_AxisElementChanged);
                    _olapDataManager.AxisElementChanged += new AxisElementChangedEventHandler(OlapDataManager_AxisElementChanged);
                }   
            }
        }

        /// <summary>
        /// Gets or sets value for sort order od Measures in CubeDimensionBrowser
        /// </summary>
        public SortCubeMeasureOrder MeasureSortOrderInCubeBrowser { get; set; }

        /// <summary>
        /// Gets or sets the drag drop manager.
        /// </summary>
        /// <value>The drag drop manager.</value>
        public DragDropManager DragDropManager
        {
            get
            {
                return this.OlapDataManager.DragDropManager;
            }
            set
            {
                this.OlapDataManager.DragDropManager = value;
            }
        }

        /// <summary>
        /// Gets or sets the removed dimensions.
        /// </summary>
        /// <value>The removed dimensions.</value>
        public DimensionCollection ExcludedDimensions
        {
            get
            {
                return _removedDimensions = _removedDimensions ?? new DimensionCollection();
            }
            set
            { 
                _removedDimensions = value; 
            }
        }

        /// <summary>
        /// Gets or sets the removed measures and dimensions.
        /// </summary>
        /// <value>The removed dimensions measures and dimensions.</value>    
        public List<String> ExcludedElements
        {
            get
            {
                return _removedElements = _removedElements ?? new List<String>();
            }
            set
            {
                _removedElements = value;
            }
        }        


        #endregion

        #region Private methods

        void OlapDataManager_AxisElementChanged(object sender, AxisElementChangedEventArgs e)
        {
            if (this.Items.Count > 0 && this.Items[0] is MetaTreeNode && this.OlapDataManager.CurrentReport != null && this.OlapDataManager.CurrentReport.CalculatedMembers.Count > 0)
            {
                MetaTreeNode calcMemberHeaderNode = (this.Items[0] as MetaTreeNode).ChildNodes.Select(i => i).Where(j => j.NodeType == MetaTreeNodeType.CalculatedMemberGroup).FirstOrDefault();
                if (calcMemberHeaderNode != null)
                {
                    bool isCalcMembersUpdated = true;
                    foreach (var item in this.OlapDataManager.CurrentReport.CalculatedMembers)
                    {
                        if (!(item.ElementValue is CalculatedMember && calcMemberHeaderNode.ChildNodes.Any(i => i.UniqueName == (item.ElementValue as CalculatedMember).UniqueName)))
                        {
                            isCalcMembersUpdated = false;
                            break;
                        }
                    }
                    if (!isCalcMembersUpdated)
                    {
                        MetaTreeHelper.FillMetaTreeNode(calcMemberHeaderNode, this.OlapDataManager.CurrentReport.CalculatedMembers, true);
                    }
                }
                else
                {
                    Refresh();
                }

            }
            if (this.Items.Count > 0 && this.Items[0] is MetaTreeNode && (this.OlapDataManager as OlapDataManager).VirtualKpiElements.Count > 0)
            {
                bool isKPIUpdated = true;
                MetaTreeNode virtualKPINode = (this.Items[0] as MetaTreeNode).ChildNodes.Where(j => j.NodeType == MetaTreeNodeType.VirtualKPIGroup).FirstOrDefault();
                if (virtualKPINode != null)
                {
                    if (this.OlapDataManager.CurrentReport.VirtualKpiElements.Count > 0)
                    {
                        foreach (var item in this.OlapDataManager.CurrentReport.VirtualKpiElements)
                        {
                            if (item.ElementValue is VirtualKpiElement && !(this.OlapDataManager.VirtualKpiElements.List.Any(i => (i.ElementValue as VirtualKpiElement).UniqueName.ToUpper() == (item.ElementValue as VirtualKpiElement).UniqueName.ToUpper())))
                                (this.OlapDataManager as OlapDataManager).VirtualKpiElements.Add(item);
                        }
                    }
                    if ((this.OlapDataManager as OlapDataManager).VirtualKpiElements.Count > 0)
                    {
                        foreach (var item in (this.OlapDataManager as OlapDataManager).VirtualKpiElements)
                        {
                            if (item.ElementValue is VirtualKpiElement && !(this.OlapDataManager.CurrentReport.VirtualKpiElements.List.Any(i => (i.ElementValue as VirtualKpiElement).UniqueName.ToUpper() == (item.ElementValue as VirtualKpiElement).UniqueName.ToUpper())))
                                this.OlapDataManager.CurrentReport.VirtualKpiElements.Add(item);
                        }
                    }
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
                        MetaTreeHelper.FillMetaTreeNode(virtualKPINode, false, (this.OlapDataManager as OlapDataManager).VirtualKpiElements);
                        Refresh();
                    }
                }
                else
                {
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Refreshes the cube dimension browser with new items.
        /// </summary>
        internal void Refresh()
        {
            if (this.OlapDataManager != null && !String.IsNullOrEmpty(this.OlapDataManager.CurrentCubeName))
            {
                MetaTreeNodeCollection metaCollection = new MetaTreeNodeCollection(null);
                CubeSchema cubeSchema = this.OlapDataManager.CurrentCubeSchema;
                if (cubeSchema!=null)
                {
                    MetaTreeNode cubeNode = new MetaTreeNode();

                    cubeNode.NodeType = MetaTreeNodeType.Cube;
                    cubeNode.Name = this.OlapDataManager.CurrentCubeName;
                    if (cubeSchema.CubeInfo != null)
                        cubeNode.Caption = cubeSchema.CubeInfo.Caption;
                    else
                        cubeNode.Caption = this.OlapDataManager.CurrentCubeName;

                    cubeNode.NodeCheckedType = MetaTreeNodeCheckedType.SomeChildChecked;
                    ////Fills the Calculated members if any in current report.
                    if (this.OlapDataManager.CurrentReport != null && this.OlapDataManager.CurrentReport.CalculatedMembers.Count > 0)
                    {
                        MetaTreeHelper.FillMetaTreeNode(cubeNode, this.OlapDataManager.CurrentReport.CalculatedMembers, false);
                    }

                    ///By using this dictionary values caption of the MeasureGroup for tranlated cube is getting from customer 
                    ///by raising the event and that caption value is used to fill the MetaTree in CubeDimension Browser

                    List<string> groupNameCollection = new List<string>();

                    foreach (var measure in cubeSchema.Measures)
                    {
                        if (!groupNameCollection.Contains(measure.GroupName))
                            groupNameCollection.Add(measure.GroupName);
                    }

                    Dictionary<string, string> captions = new Dictionary<string, string>();

                    foreach (var measureGroupName in groupNameCollection)
                    {
                        captions.Add(measureGroupName, null);
                    }
                    MeasureGroupCaptionEventArgs groupCaptions = new MeasureGroupCaptionEventArgs { TranslatedCaptions=captions};
                    if (Updating != null)
                    {
                        Updating(groupCaptions);
                    }
                    MetaTreeHelper.FillMetaTreeNode(cubeNode, cubeSchema.Measures, true, false, this.MeasureSortOrderInCubeBrowser, groupCaptions.TranslatedCaptions);

                    if (cubeSchema.Kpis.Count > 0)
                    {
                        MetaTreeHelper.FillMetaTreeNode(cubeNode, cubeSchema.Kpis, true, false);
                    }
                    if ((this.OlapDataManager as OlapDataManager).VirtualKpiElements.Count > 0)
                    {
                        MetaTreeHelper.FillMetaTreeNode(cubeNode, false, (this.OlapDataManager as OlapDataManager).VirtualKpiElements);
                    }
                    else if (this.OlapDataManager.CurrentReport.VirtualKpiElements != null && this.OlapDataManager.CurrentReport.VirtualKpiElements.Count > 0)
                    {
                        MetaTreeHelper.FillMetaTreeNode(cubeNode, false, this.OlapDataManager.CurrentReport.VirtualKpiElements);
                    }
                    if (this.ExcludedDimensions.Count > 0 || this.ExcludedElements.Count > 0)
                    {
                        if (this.ExcludedDimensions.Count > 0)
                        {
                            MetaTreeHelper.FillMetaTreeNode(cubeNode, false, cubeSchema.Dimensions, this.ExcludedDimensions, cubeSchema.NamedSets);
                        }
                        if (this.ExcludedElements.Count > 0)
                        {
                            MetaTreeHelper.FillMetaTreeNode(cubeNode, false, cubeSchema, this.ExcludedElements);
                        }
                    }
                    else
                    {
                        MetaTreeHelper.FillMetaTreeNode(cubeNode, false, cubeSchema.Dimensions, cubeSchema.NamedSets);
                    }

                    metaCollection.Add(cubeNode); 
                }
                this.ItemsSource = metaCollection;
            }
            
        }

        #endregion

        #region Overrided method
        /// <summary>
        /// Overrides the OnApply Template
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.SetValue(ContextMenuAdvService.ContextMenuAdvProperty, ContextMenu);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            (element as CDTreeViewItem).IsExpanded = true;    
        }

        #endregion

        #region Events

        #region CubeSchema change handler
        /// <summary>
        /// Handles the CubeSchemaChanged event of the OlapDataManager.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.OlapSilverlight.Manager.CubeSchemaChangedEventArgs"/> instance containing the event data.</param>
        //void OlapDataManager_CubeSchemaChanged(object sender, CubeSchemaChangedEventArgs e)
        //{            
        //    this.Refresh();
        //}
        #endregion
        
        #region DragDrop handler

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.ReleaseMouseCapture();
        }

        /// <summary>
        /// Handles the NodeReleased event of the CubeDimensionBrowser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void CubeDimensionBrowser_NodeReleased(object sender, NodeReleasedEventArgs e)
        {
            if (this.DragDropManager != null)
            {
                this.DragDropManager.DragDropPopup.IsOpen = false;
                this.DragDropManager = null;
            }
        }

        /// <summary>
        /// Handles the NodeClicked event of the CubeDimensionBrowser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void CubeDimensionBrowser_NodeClicked(object sender, NodeClickedEventArgs e)
        {
            if ((sender as CDTreeViewItem) != null)
            {
                MetaTreeNode selectedNode = (sender as CDTreeViewItem).Header as MetaTreeNode;
                if (this.DragDropManager == null && selectedNode != null)
                {
                    if (selectedNode.NodeType != MetaTreeNodeType.Cube && selectedNode.NodeType != MetaTreeNodeType.MeasureGroup && selectedNode.NodeType != MetaTreeNodeType.DisplayFolder && selectedNode.NodeType != MetaTreeNodeType.None && selectedNode.NodeType != MetaTreeNodeType.KPI_ROOT && selectedNode.NodeType != MetaTreeNodeType.CalculatedMemberGroup && selectedNode.NodeType != MetaTreeNodeType
                        .VirtualKPIGroup)
                    {
                        this.DragDropManager = new DragDropManager();
                        this.DragDropManager.SelectedNode = selectedNode;
                        this.DragDropManager.Source = this;

                        Popup dragDropPopup = new Popup();
                        ContentControl popupContent = new ContentControl();
                        popupContent.IsHitTestVisible = true;
                        DataTemplate template = this.ItemTemplate as DataTemplate;
                        template.LoadContent();
                        popupContent.ContentTemplate = template;
                        popupContent.Content = this.DragDropManager.SelectedNode;
                        popupContent.Opacity = 0.5;
                        dragDropPopup.Child = popupContent;
                        dragDropPopup.Child.Visibility = System.Windows.Visibility.Collapsed;
                        dragDropPopup.IsOpen = true;
                        dragDropPopup.Opacity = 0.2;
                        this.DragDropManager.DragDropPopup = dragDropPopup;
                        this.CaptureMouse();
                    }
                }
            }
        }
        /// <summary>
        /// Handles the Right Mouse Button Click event of the CubeDimensionBrowser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void CubeDimensionBrowser_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            bool a = false;
            this.ContextMenu.Items.Clear();
            if (e.OriginalSource is TextBlock)
            {
                calc_name = (e.OriginalSource as TextBlock).Text;
                for (int i = 0; (i < this.OlapDataManager.CurrentReport.CalculatedMembers.Count); i++)
                {
                    if (((calc_name == this.OlapDataManager.CurrentReport.CalculatedMembers[i].ElementValue.ElementName) && ((((System.Windows.FrameworkElement)(sender)).Name) != "CalcMeasureTreeView")))
                        a = true;
                }
                for (int i = 0; i < this.OlapDataManager.CurrentReport.VirtualKpiElements.Count; i++)
                {
                    if (((calc_name == this.OlapDataManager.CurrentReport.VirtualKpiElements[i].ElementValue.ElementName) && ((((System.Windows.FrameworkElement)(sender)).Name) != "cubeDimensionBrowser")))
                        a = true;
                }
            }
            if (a)
            {
                ContextMenuItemAdv item = new ContextMenuItemAdv { Header = SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_CalcMember_Remove") };
                ContextMenuItemAdv edit = new ContextMenuItemAdv { Header = SR.GetString(CultureInfo.CurrentUICulture, "OlapClient_CalcMember_Edit") };
                this.ContextMenu.Items.Add(item);
                for (int i=0;i < this.OlapDataManager.CurrentReport.VirtualKpiElements.Count; i++)
                {
                    if ((calc_name == this.OlapDataManager.CurrentReport.VirtualKpiElements[i].ElementValue.ElementName) && ((((System.Windows.FrameworkElement)(sender)).Name) != "cubeDimensionBrowser"))
                    {
                        this.ContextMenu.Items.Add(edit);
                    }
                }
                item.Click += new RoutedEventHandler(item_Click);
                edit.Click += new RoutedEventHandler(edit_Click);
            }
        }

        void edit_Click(object sender, RoutedEventArgs e)
        {
            VirtualKpiEditor kpiEditor = new VirtualKpiEditor(this.OlapDataManager as OlapDataManager);
            kpiEditor.VisualStyle = (Windows.Shared.VisualStyle)Enum.Parse(typeof(Windows.Shared.VisualStyle), (this.OlapDataManager.Properties[0].Value as AxisElementBuilder).SplitButtonVisualStyle.ToString(), true);
            SkinManager.SetVisualStyle(kpiEditor, (Windows.Controls.Theming.VisualStyle)Enum.Parse(typeof(Windows.Controls.Theming.VisualStyle), kpiEditor.VisualStyle.ToString(), true));           
            kpiEditor.cubeDimensionBrowser.ItemsSource = this.ItemsSource;
            kpiEditor.cubeDimensionBrowser.Style = this.Style;
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
                    // kpiEditor.Focus();
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
            for (int i = 0; i < this.OlapDataManager.VirtualKpiElements.Count; i++)
            {
                if (calc_name == this.OlapDataManager.VirtualKpiElements[i].ElementValue.ElementName)
                    this.OlapDataManager.VirtualKpiElements.RemoveAt(i);
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
            this.OlapDataManager.NotifyElementChanged();
            this.Refresh();
        }
        #endregion

        #region Folder icon handler
        /// <summary>
        /// Handles the NodeExpanded event of the CDTreeViewItem to change the folder icon of the node based on its state.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void CubeDimensionBrowser_NodeExpanded(object sender, RoutedEventArgs e)
        {    
            MetaTreeNode currentNode = (sender as CDTreeViewItem).DataContext as MetaTreeNode;
   
            if (currentNode.NodeType == MetaTreeNodeType.MeasureGroup || currentNode.NodeType == MetaTreeNodeType.DisplayFolder)
            { 
                ////Iterate through the control template of the CDTreeViewItem to change the folder image in the template.

                //// Getting the inner button present in the control template of CDTreeViewItem.
                var button = (VisualTreeHelper.GetChild(sender as DependencyObject, 0) as System.Windows.Controls.Grid).Children[2];
               
                //// Getting the ContentPresenter in side the inner button.
                var cntPresenter = (VisualTreeHelper.GetChild(button as DependencyObject,0) as System.Windows.Controls.Grid).Children[1];

                //// Getting the Stackpanel inside the Hierarchical data template.
                var cntControl = (VisualTreeHelper.GetChild((cntPresenter as ContentPresenter).Content as DependencyObject, 0) as StackPanel).Children[0];
                //Assigning default node image to Datatemplate variable.
                DataTemplate nodeImage = this.resourceDictionary["OpenFolder"] as DataTemplate;
                NodeImageChangingEvntArgs eventArgs = new NodeImageChangingEvntArgs { NodeImage = nodeImage };
                if (NodeImageChanging != null)
                {
                    NodeImageChanging(eventArgs);
               }
                //// Assigning the opened node image to the content control present inside the Hierarchical data template.
                (cntControl as ContentControl).ContentTemplate = eventArgs.NodeImage;   
            }
        }

        /// <summary>
        /// Handles the NodeCollapsed event of the CDTreeViewItem to change the folder icon of the node based on its state.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void CubeDimensionBrowser_NodeCollapsed(object sender, RoutedEventArgs e)
        {
            MetaTreeNode currentNode = (sender as CDTreeViewItem).DataContext as MetaTreeNode;

            if (currentNode.NodeType == MetaTreeNodeType.MeasureGroup || currentNode.NodeType == MetaTreeNodeType.DisplayFolder)
            {
                ////Iterate through the control template of the CDTreeViewItem to change the folder image in the template.

                //// Getting the inner button present in the control template of CDTreeViewItem.
                var button = (VisualTreeHelper.GetChild(sender as DependencyObject, 0) as System.Windows.Controls.Grid).Children[2];

                //// Getting the ContentPresenter in side the inner button.
                var cntPresenter = (VisualTreeHelper.GetChild(button as DependencyObject, 0) as System.Windows.Controls.Grid).Children[1];

                //// Getting the Stackpanel inside the Hierarchical data template.
                var cntControl = (VisualTreeHelper.GetChild((cntPresenter as ContentPresenter).Content as DependencyObject, 0) as StackPanel).Children[0];
                 //Assigning default Node image to Datatemplate variable.
                DataTemplate nodeImage = this.resourceDictionary["CloseFolder"] as DataTemplate;
                NodeImageChangingEvntArgs eventArgs = new NodeImageChangingEvntArgs { NodeImage = nodeImage, IsCollapsedNode = true };
                if (NodeImageChanging != null)
                {
                    NodeImageChanging(eventArgs);
                }
                //// Assigning the opened node image to the content control present inside the Hierarchical data template.
                (cntControl as ContentControl).ContentTemplate = eventArgs.NodeImage;
            }
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// Class for NodeImageChanging event's argument.
    /// </summary>
    public class NodeImageChangingEvntArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets value for checking whether it is Collapsed or Expanded node
        /// </summary>
        public bool IsCollapsedNode { get; internal set; }
        /// <summary>
        /// Gets or sets the NodeImage as Datatemplate
        /// </summary>
        public DataTemplate NodeImage { get; set; }
    }

    public class MeasureGroupCaptionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets value of MeasureGroup caption
        /// </summary>
        public Dictionary<string, string> TranslatedCaptions = new Dictionary<string, string>();

    }

    #region Cube Dimension Browser TreeView

    /// <summary>
    /// Tree View for Cube Dimension Browser
    /// </summary>
    [DesignTimeVisible(false)]
    public class CDTreeView : TreeView
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
           // this.AddHandler(MouseLeftButtonDownEvent, NodeClicked, true);
        }
        /// <summary>
        /// To get expanded event from the child nodes
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            CDTreeViewItem treeViewItem = new CDTreeViewItem();
            treeViewItem.Expanded += NodeExpanded;
            treeViewItem.NodeExpanded += NodeExpanded;
            treeViewItem.Collapsed += NodeCollapsed;
            treeViewItem.NodeCollapsed += NodeCollapsed;

            treeViewItem.NodeClicked += NodeClicked;
            treeViewItem.NodeReleased += NodeReleased;
            return treeViewItem;
        }

        public event RoutedEventHandler NodeExpanded;
        public event RoutedEventHandler NodeCollapsed;
        public event NodeClickedHandler NodeClicked;
        public event NodeReleasedHandler NodeReleased;
    }

    /// <summary>
    /// Tree View Item for Cube Dimension Browser
    /// </summary>
    [DesignTimeVisible(false)]
   public class CDTreeViewItem : TreeViewItem
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //this.AddHandler(MouseLeftButtonDownEvent, NodeClicked, true);
            //this.AddHandler(MouseLeftButtonUpEvent, NodeReleased, true);
        }
        /// <summary>
        /// To tag the Expanded event for all the child nodes
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            CDTreeViewItem treeViewItem = new CDTreeViewItem();
            treeViewItem.Expanded += NodeExpanded;
            treeViewItem.NodeExpanded += NodeExpanded;
            treeViewItem.Collapsed += NodeCollapsed;
            treeViewItem.NodeCollapsed += NodeCollapsed;

            treeViewItem.NodeClicked += NodeClicked;
            treeViewItem.NodeReleased += NodeReleased;
            return treeViewItem;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.NodeClicked(this, new NodeClickedEventArgs());
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            this.NodeReleased(this, new NodeReleasedEventArgs());
        }

        public event RoutedEventHandler NodeExpanded;
        public event RoutedEventHandler NodeCollapsed;
        public event NodeClickedHandler NodeClicked;
        public event NodeReleasedHandler NodeReleased; 
    }

    public class NodeClickedEventArgs : EventArgs { }
    public class NodeReleasedEventArgs : EventArgs { }

    public delegate void NodeClickedHandler(object sender, NodeClickedEventArgs e);
    public delegate void NodeReleasedHandler(object sender, NodeReleasedEventArgs e);
   

    #endregion
}
