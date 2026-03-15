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
using Syncfusion.OlapSilverlight.Reports;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using Syncfusion.OlapSilverlight.Common;
using Syncfusion.OlapSilverlight.MDXQueryBuilder;
using System.ComponentModel;
using Syncfusion.Windows.Controls.Theming;
using Syncfusion.Silverlight.Client.Olap.Resources;
using System.Globalization;

namespace Syncfusion.Silverlight.Tools.Olap
{
    [DesignTimeVisible(false)]
    public class AxisElementBuilder : ListBox
    {
        #region Dependency Property

        #region AllowMultiMemberSelection
        /// <summary>
        /// Gets or sets a value indicating whether [show checkbox in MemberEditor TreeView].
        /// </summary>
        /// <value><c>true</c> if [show checkbox in MemberEditor TreeView]; otherwise, <c>false</c>.</value>
        public bool AllowMultiMemberSelection
        {
            get { return (bool)GetValue(MultiMemberSelectionProperty); }
            set { SetValue(MultiMemberSelectionProperty, value); }

        }
        //using AllowMultiMemberSelection Dependency Property
        internal static readonly DependencyProperty MultiMemberSelectionProperty = DependencyProperty.Register("AllowMultiMemberSelection", typeof(bool), typeof(AxisElementBuilder), new PropertyMetadata(true));

        #endregion

        #region SplitButtonDisplayMode

        public SplitButtonDisplayMode SplitButtonDisplayMode
        {
            get { return (SplitButtonDisplayMode)GetValue(ShowAttributeNameProperty); }
            set { SetValue(ShowAttributeNameProperty, value); }
        }
        /// <summary>
        /// Using DependencyProperty as the backing store for SplitButtonDisplayMode 
        /// </summary>
        internal static readonly DependencyProperty ShowAttributeNameProperty = DependencyProperty.Register("SplitButtonDisplayMode", typeof(SplitButtonDisplayMode), typeof(AxisElementBuilder), new PropertyMetadata(SplitButtonDisplayMode.WithoutHierarchyCaption));

        #endregion

        /// <summary>
        /// Gets or sets whether the elements will automatically execute the query.
        /// </summary>
        /// <value><c>true</c> if [auto execute]; otherwise, <c>false</c>.</value>
        internal bool AutoExecute
        {
            get
            {
                return (bool)GetValue(AutoExecuteProperty);
            }

            set
            {
                SetValue(AutoExecuteProperty, value);
            }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AutoExecute
        /// </summary>
        internal static readonly DependencyProperty AutoExecuteProperty = DependencyProperty.Register("AutoExecute", typeof(bool), typeof(AxisElementBuilder), new PropertyMetadata(true));
        /// <summary>
        ///  Gets or sets a value indicating whether[IsMeasureEditorOpen]
        /// </summary>
        /// <value><c>true</c> if [IsMeasureEditorOpen]; otherwise, <c>false</c>.</value>
        public bool IsMeasureEditorOpen
        {
            get { return (bool)GetValue(IsMeasureEditorOpenProperty); }
            set { SetValue(IsMeasureEditorOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMeasureEditorOpen.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsMeasureEditorOpenProperty =
            DependencyProperty.Register("IsMeasureEditorOpen", typeof(bool), typeof(AxisElementBuilder), new PropertyMetadata(true));
        /// <summary>
        ///  Gets or sets a value indicating whether[IsMemberEditorOpen]
        /// </summary>
        /// <value><c>true</c> if [IsMemberEditorOpen]; otherwise, <c>false</c>.</value>
        public bool IsMemberEditorOpen
        {
            get { return (bool)GetValue(IsMemberEditorOpenProperty); }
            set { SetValue(IsMemberEditorOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMemberEditorOpen.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsMemberEditorOpenProperty =
            DependencyProperty.Register("IsMemberEditorOpen", typeof(bool), typeof(AxisElementBuilder), new PropertyMetadata(true));
        
        #region SplitButtonStyle

        public VisualStyle SplitButtonVisualStyle
        {
            get { return (VisualStyle)GetValue(SplitButtonVisualStyleProperty); }
            set { SetValue(SplitButtonVisualStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MetaTreeNode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SplitButtonVisualStyleProperty =
            DependencyProperty.Register("SplitButtonVisualStyle", typeof(VisualStyle), typeof(AxisElementBuilder), new PropertyMetadata(VisualStyle.Default, OnSplitButtonVisualStyleChanged));

        private static void OnSplitButtonVisualStyleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            System.Windows.ResourceDictionary r1 = new System.Windows.ResourceDictionary();
            r1.Source = new Uri("/Syncfusion.OlapClient.Silverlight;component/Tools/AxisElementBuilder/AxisElementBuilder.xaml", UriKind.RelativeOrAbsolute);
            AxisElementBuilder builder = (AxisElementBuilder)sender;
            switch ((VisualStyle)e.NewValue)
            {
                case VisualStyle.Blend:
                    builder.ItemTemplate = r1["BlendStyle"] as DataTemplate;
                    break;
                case VisualStyle.Default:
                    builder.ItemTemplate = r1["DefaultStyle"] as DataTemplate;
                    break;
                case VisualStyle.Office2007Black:
                    builder.ItemTemplate = r1["Office2007BlackStyle"] as DataTemplate;
                    break;
                case VisualStyle.Office2007Blue:
                    builder.ItemTemplate = r1["Office2007BlueStyle"] as DataTemplate;
                    break;
                case VisualStyle.Office2007Silver:
                    builder.ItemTemplate = r1["Office2007SilverStyle"] as DataTemplate;
                    break;
                case VisualStyle.Metro:
                    builder.ItemTemplate = r1["MetroStyle"] as DataTemplate;
                    break;
                case VisualStyle.Office2010Black:
                case VisualStyle.Office2010Blue:
                case VisualStyle.Office2010Silver:
                    builder.ItemTemplate = r1[e.NewValue + "Style"] as DataTemplate;
                    break;
                case VisualStyle.Transparent:
                    builder.ItemTemplate = r1["TransparentStyle"] as DataTemplate;
                    break;
                default:
                    builder.ItemTemplate = r1["DefaultStyle"] as DataTemplate;
                    break;
            }
        }

        #endregion

        #endregion

        #region Private member

        OlapDataManager _OlapDataManager;

        #endregion

        #region Constructor

        public AxisElementBuilder()
        {
            this.DefaultStyleKey = typeof(AxisElementBuilder);
            this.BorderBrush = (Brush)new SolidColorBrush(Colors.Gray);
            this.BorderThickness = new Thickness(0.5);
            this.MetaTreeNodes = new ObservableCollection<MetaTreeNode>();
            this.ItemsSource = this.MetaTreeNodes;
            this.SelectionChanged += AxisElementBuilder_SelectionChanged;
            System.Windows.ResourceDictionary r1 = new System.Windows.ResourceDictionary();
            r1.Source = new Uri("/Syncfusion.OlapClient.Silverlight;component/Tools/AxisElementBuilder/AxisElementBuilder.xaml", UriKind.RelativeOrAbsolute);
            this.ItemTemplate = r1["DefaultStyle"] as DataTemplate;
        }

        #endregion

        internal bool IsSavedReport { get; set; }

        #region Public properties

        /// <summary>
        /// Gets or sets the olap data manager.
        /// </summary>
        /// <value>The olap data manager.</value>
        public OlapDataManager OlapDataManager
        {
            get
            {
                return _OlapDataManager;
            }
            set
            {
                _OlapDataManager = value;

                this.OlapDataManager.Properties.Add(this.Name, this);
                this.OlapDataManager.AxisElementChanged += OlapDataManager_AxisElementChanged;
                this.OlapDataManager.CubeSchemaChanged += OlapDataManager_CubeSchemaChanged;
                this.OlapDataManager.LevelMembersObtained -= OlapDataManager_LevelElementObtained;
                this.OlapDataManager.LevelMembersObtained += OlapDataManager_LevelElementObtained;
                this.OlapDataManager.ChildMembersObtained += new ChildMembersObtainedHandler(OlapDataManager_ChildMembersObtained);
            }
        }

        /// <summary>
        /// Gets or sets the axis.
        /// </summary>
        /// <value>The axis.</value>
        public AxisPosition Axis { get; set; }

        /// <summary>
        /// Gets or sets the meta tree nodes.
        /// </summary>
        /// <value>The meta tree nodes.</value>
        public ObservableCollection<MetaTreeNode> MetaTreeNodes { get; set; }

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
        /// Gets or sets the report items.
        /// </summary>
        /// <value>The report items.</value>
        public Items ReportItems
        {
            get
            {
                if (this.OlapDataManager != null && this.OlapDataManager.CurrentReport != null)
                {
                    this.OlapDataManager.MdxQuery = null;
                    if (this.Axis == AxisPosition.Categorical)
                    {
                        if (this.OlapDataManager.CurrentReport.TogglePivot)
                            return this.OlapDataManager.CurrentReport.SeriesElements;
                        else
                            return this.OlapDataManager.CurrentReport.CategoricalElements;
                    }
                    else if (this.Axis == AxisPosition.Series)
                    {
                        if (this.OlapDataManager.CurrentReport.TogglePivot)
                            return this.OlapDataManager.CurrentReport.CategoricalElements;
                        else
                            return this.OlapDataManager.CurrentReport.SeriesElements;
                    }
                    else if (this.Axis == AxisPosition.Slicer)
                    {
                        return this.OlapDataManager.CurrentReport.SlicerElements;
                    }
                }
                return null;
            }
            set
            {
                if (this.OlapDataManager != null && this.OlapDataManager.CurrentReport != null)
                {
                    if (this.Axis == AxisPosition.Categorical)
                    {
                        this.OlapDataManager.CurrentReport.CategoricalElements = value;
                    }
                    else if (this.Axis == AxisPosition.Series)
                    {
                        this.OlapDataManager.CurrentReport.SeriesElements = value;
                    }
                    else if (this.Axis == AxisPosition.Slicer)
                    {
                        this.OlapDataManager.CurrentReport.SlicerElements = value;
                    }
                }
            }
        }

        #endregion

        #region Public method

        public void ReAarrangeElemets(MetaTreeNode currentNode)
        {
            Items tempItems = this.ReportItems;
            if (currentNode != null)
            {
                foreach (Item item in this.ReportItems)
                {
                    if ((currentNode.NodeType == MetaTreeNodeType.Dimension && (item.ElementValue is DimensionElement) && (item.ElementValue as DimensionElement).Name == currentNode.Name && (item.ElementValue as DimensionElement).Hierarchy.Name.ToUpper() == (currentNode.Properties[1].Value as Hierarchy).Name.ToUpper()) ||
                        (currentNode.NodeType == MetaTreeNodeType.MeasureGroup && item.ElementValue is MeasureElements) ||
                        (currentNode.NodeType == MetaTreeNodeType.KPI_ROOT && item.ElementValue is KpiElements) ||
                        (currentNode.NodeType == MetaTreeNodeType.NamedSet && item.ElementValue is NamedSetElement) ||
                        (currentNode.NodeType == MetaTreeNodeType.CalculatedMember && item.ElementValue is CalculatedMember && currentNode.UniqueName.ToUpper() == (item.ElementValue as CalculatedMember).UniqueName.ToUpper())||
                        (currentNode.NodeType==MetaTreeNodeType.VirtualKPIMember && item.ElementValue is VirtualKpiElement && currentNode.UniqueName.ToUpper()== (item.ElementValue as VirtualKpiElement).UniqueName.ToUpper()))
                    {
                        tempItems.Remove(item);
                        break;
                    }
                }
            }
            else
            {
                tempItems = new Items();
                tempItems.AddRange(this.ReportItems);
                this.ReportItems.Clear();
                foreach (MetaTreeNode metaTree in this.MetaTreeNodes)
                {
                    for (int i = 0; i < tempItems.Count; i++)
                    {
                        if ((metaTree.NodeType == MetaTreeNodeType.Dimension && (tempItems[i].ElementValue is DimensionElement) && (tempItems[i].ElementValue as DimensionElement).Name == currentNode.Name && (tempItems[i].ElementValue as DimensionElement).Hierarchy.Name == (currentNode.Properties[1].Value as Hierarchy).Name) ||
                            (metaTree.NodeType == MetaTreeNodeType.MeasureGroup && tempItems[i].ElementValue is MeasureElements) ||
                            (metaTree.NodeType == MetaTreeNodeType.KPI_ROOT && tempItems[i].ElementValue is KpiElements) ||
                            (metaTree.NodeType == MetaTreeNodeType.NamedSet && tempItems[i].ElementValue is NamedSetElement) ||
                            (metaTree.NodeType == MetaTreeNodeType.CalculatedMember && tempItems[i].ElementValue is CalculatedMember && metaTree.UniqueName == (tempItems[i].ElementValue as CalculatedMember).UniqueName))
                        {
                            this.ReportItems.Add(tempItems[i]);
                            tempItems.Remove(tempItems[i]);
                            break;
                        }
                    }
                }
            }
            if (this.AutoExecute)
                this.OlapDataManager.NotifyElementChanged(this.Axis);
            else
                this.OlapDataManager.RefreshAxisElementBuilder();
        }

        #endregion

        #region Private methods

        private void Refresh()
        {
            if (this.OlapDataManager.CurrentReport != null)
            {
                try
                {
                    this.LoadElements(this.ReportItems);
                }
                catch (Exception ex)
                {
                    Syncfusion.Windows.Tools.Controls.WindowControl.ShowAlert(ex.Message, SR.GetString(CultureInfo.CurrentUICulture,"Exception_ErrorWhileLoadingReportItems"), Windows.Tools.Controls.DialogIcon.Error, Windows.Tools.Controls.DialogButton.OK, null, Windows.Tools.Controls.AnimationType.Zoom);
                }
            }
        }

        internal ObservableCollection<MemberElement> ExcludedMemberElements { get; set; }
        /// <summary>
        /// Loads the elements.
        /// </summary>
        /// <param name="items">The items.</param>
        private void LoadElements(Items items)
        {
            this.MetaTreeNodes.Clear();
            CubeSchema cubeSchema = this.OlapDataManager.CurrentCubeSchema;
            ExcludedMemberElements = null;
            foreach (Item item in items)
            {
                if (item.ElementValue is MeasureElements)
                {
                    #region Load Measure elements

                    //// Extracting the measure elements from the element collection
                    MeasureElements measureElements = item.ElementValue as MeasureElements;

                    //// Creating a main measures node
                    MetaTreeNode measureGroupNode = new MetaTreeNode(PropertyConstants.MeasrueNodeName,
                        PropertyConstants.MeasrueNodeName, PropertyConstants.MeasrueNodeName);
                    measureGroupNode.NodeType = MetaTreeNodeType.MeasureGroup;
                    measureGroupNode.Properties.Add(new Property(PropertyConstants.MeasureGroupName, cubeSchema.Measures));


                    //// Creating a temporary meta tree node in which we will populate all the measures
                    MetaTreeNode metaTreeNodeMeasures = new MetaTreeNode
                    {
                        Name = PropertyConstants.MeasrueNodeName,
                        UniqueName = PropertyConstants.MeasrueNodeName,
                        Caption = PropertyConstants.MeasrueNodeName,
                        Description = PropertyConstants.MeasrueNodeName
                    };
                    //// Populating the measures from the cube schema
                    MetaTreeHelper.FillMetaTreeNode(measureElements, metaTreeNodeMeasures, cubeSchema.Measures, false, true);

                    //// Adding measure elements
                    foreach (MeasureElement measureElement in measureElements.Elements)
                    {
                        //// Searching the measureNode from the collection based on the element supplied
                        MetaTreeNode metaTreeNode = GetMeasureNode(metaTreeNodeMeasures, measureElement, true);
                        if (metaTreeNode != null)
                            measureGroupNode.ChildNodes.Add(metaTreeNode);
                    }

                    //// Adding excluded measure elements
                    foreach (MeasureElement measureElement in measureElements.ExcludedMeasures)
                    {
                        //// Searching the measureNode from the collection based on the element supplied
                        MetaTreeNode metaTreeNode = GetMeasureNode(metaTreeNodeMeasures, measureElement, false);
                        if (metaTreeNode != null)
                            measureGroupNode.ChildNodes.Add(metaTreeNode);
                    }

                    //// To identify the dragsource adding a identifier property
                    measureGroupNode.Properties.Add(new Property(PropertyConstants.AxisElements, null));
                    //// Adding the measure to the current node collection
                    this.MetaTreeNodes.Add(measureGroupNode);

                    #endregion
                }
                else if (item.ElementValue is DimensionElement)
                {
                    #region Load Dimension elements

                    //// Extracting the selected dimension element
                    DimensionElement dimensionElement = item.ElementValue as DimensionElement;
                    //// Extracting the excluded dimension element 
                    DimensionElement excludedDimensionElement = item.ExcludedElementValue as DimensionElement;
                    string dimensionName = dimensionElement.Name;
                    //bool isChildUpdated = false;
                    if (dimensionElement.Name != string.Empty)
                    {
                        //// Getting the dimension by element unique name
                        Dimension dimensionObj = cubeSchema.GetDimensionByUniqueName(dimensionElement.UniqueName);
                        //// Creating dimension node
                        MetaTreeNode metaTreeNode = new MetaTreeNode(dimensionName, dimensionObj.Caption, dimensionName);
                       
                        if (dimensionObj != null)
                        {
                            metaTreeNode.Properties.Add(new Property(PropertyConstants.Dimension, dimensionObj));
                        }

                        //// uniqueName is changed from dimension Name to unique name
                        //// metaTreeNode.UniqueName = dimensionName;
                        metaTreeNode.UniqueName = dimensionElement.UniqueName;
                        metaTreeNode.NodeType = MetaTreeNodeType.Dimension;
                        // Extracting the level element
                        LevelElement levelElement = dimensionElement.Hierarchy.LevelElements[0];

                        if ((excludedDimensionElement != null) && (excludedDimensionElement.Hierarchy.LevelElements.Count >= 1))
                        {
                            ExcludedMemberElements = new ObservableCollection<MemberElement>();
                            foreach (LevelElement lvlElement in excludedDimensionElement.Hierarchy.LevelElements)
                            {
                                foreach (MemberElement memberElement in lvlElement.MemberElements)
                                {
                                    ExcludedMemberElements.Add(memberElement);
                                }
                            }
                        }
                        foreach (Hierarchy hierarchyObj in dimensionObj.Hierarchies)
                        {
                            if (hierarchyObj.UniqueName.ToUpper() == dimensionElement.Hierarchy.UniqueName.ToUpper() || (dimensionObj.UniqueName + ".[" + hierarchyObj.Name + "]").ToUpper() == dimensionElement.Hierarchy.UniqueName.ToUpper())
                            {
                                //// Adding the hierarchy object to the MetaTreeNode properties
                                metaTreeNode.Properties.Add(new Property(PropertyConstants.Hierarchy, hierarchyObj));
                                if (SplitButtonDisplayMode == SplitButtonDisplayMode.WithHierarchyCaption)
                                {
                                    metaTreeNode.Caption += "." + hierarchyObj.Caption;
                                }
                                foreach (Level levelObj in hierarchyObj.Levels)
                                {
                                    levelObj.ParentHierarchy = hierarchyObj;
                                    if (levelObj.UniqueName.ToUpper() == levelElement.UniqueName.ToUpper() || (dimensionObj.UniqueName + ".[" + hierarchyObj.Name + "].[" + levelObj.Name + "]").ToUpper() == levelElement.UniqueName.ToUpper() || (levelObj.ParentHierarchy.DefaultMemberUniqueName.ToUpper() == levelElement.UniqueName.ToUpper()))
                                    {
                                        //// Adding the level to the meta tree node properties
                                        metaTreeNode.Properties.Add(new Property(PropertyConstants.Level, levelObj));

                                        if (levelObj.Members == null || levelObj.Members.Count < 0)
                                        {
                                            this.GetLevelMembers(levelObj);
                                        }
                                        else
                                        {
                                            foreach (Member memberObj in levelObj.Members)
                                            {
                                                MetaTreeNode metaTreeNodeMembers = new MetaTreeNode();
                                                if (excludedDimensionElement != null)
                                                {
                                                    LevelElementCollection _levelElementCollection = ((item.ElementValue) as DimensionElement).Hierarchy.LevelElements;
                                                    if (_levelElementCollection.Count > 0)
                                                    {
                                                        foreach (var _levelElement in _levelElementCollection)
                                                        {
                                                            if (_levelElement.MemberElements.Count > 0)
                                                            {
                                                                UpdateIncludeNodes(_levelElement, metaTreeNodeMembers);
                                                            }
                                                        }
                                                    }
                                                    MetaTreeHelper.FillMetaTreeNode(metaTreeNodeMembers, memberObj, true, true);
                                                    foreach (LevelElement excludedLevelElement in excludedDimensionElement.Hierarchy.LevelElements)
                                                    {
                                                        //// Synchronizing the exclude nodes
                                                        UpdateExcludeNodes(excludedLevelElement, metaTreeNodeMembers);
                                                    }
                                                }
                                                else
                                                {
                                                    MetaTreeHelper.FillMetaTreeNode(metaTreeNodeMembers, memberObj, true, true);
                                                }
                                                //if (metaTreeNodeMembers.IsSelected.Value)
                                                //    this.OlapDataManager.GetChildMembers(metaTreeNodeMembers.UniqueName, this.OlapDataManager.CurrentCubeName);
                                                metaTreeNode.ChildNodes.Add(metaTreeNodeMembers);
                                            }
                                        }

                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        // To identify the dragsource adding a identifier property
                        metaTreeNode.Properties.Add(new Property(PropertyConstants.AxisElements, this));
                        this.MetaTreeNodes.Add(metaTreeNode);
                    }

                    #endregion
                }
                else if (item.ElementValue is VirtualKpiElement)
                {
                    VirtualKpiElement virtualKpiElement = item.ElementValue as VirtualKpiElement;

                    MetaTreeNode metaTreeNodekpis = new MetaTreeNode(virtualKpiElement.Name,virtualKpiElement.Name,virtualKpiElement.Name);
                    metaTreeNodekpis.UniqueName = virtualKpiElement.UniqueName;
                    metaTreeNodekpis.Name = virtualKpiElement.Name;
                    metaTreeNodekpis.NodeType = MetaTreeNodeType.VirtualKPIMember;
                    MetaTreeHelper.FillMetaTreeNode(virtualKpiElement, metaTreeNodekpis, this.OlapDataManager.CurrentReport.VirtualKpiElements, true);
                    //// To identify the drag source adding a identifier property
                    metaTreeNodekpis.Properties.Add(new Property(PropertyConstants.VirtualKpiNodeName, virtualKpiElement));
                    metaTreeNodekpis.Properties.Add(new Property(PropertyConstants.AxisElements, null));
                    //// Adding to the MetaTreeNode collection
                    this.MetaTreeNodes.Add(metaTreeNodekpis);
                }
                else if (item.ElementValue is KpiElements)
                {
                    #region Load Kpi elements

                    KpiElements kpiElements = item.ElementValue as KpiElements;

                    MetaTreeNode metaTreeNodekpis = new MetaTreeNode(PropertyConstants.KPI, PropertyConstants.KPI, PropertyConstants.KPI);
                    metaTreeNodekpis.UniqueName = PropertyConstants.MeasrueNodeName;
                    MetaTreeHelper.FillMetaTreeNode(kpiElements, metaTreeNodekpis, cubeSchema.Kpis, true);
                    //// To identify the dragsource adding a identifier property
                    metaTreeNodekpis.Properties.Add(new Property(PropertyConstants.AxisElements, null));
                    //// Adding to the MetaTreeNode collection
                    this.MetaTreeNodes.Add(metaTreeNodekpis);

                    #endregion
                }
                else if (item.ElementValue is NamedSetElement)
                {
                    #region Load NamedSet elements

                    NamedSetElement namedSetElement = item.ElementValue as NamedSetElement;
                    if (!namedSetElement.IsQueryScoped)
                    {
                        NamedSet namedSetObj = cubeSchema.GetNamedSetByUniqueName(namedSetElement.UniqueName);
                        MetaTreeNode metaTreeNodeNamedSet = new MetaTreeNode(namedSetElement.Name, namedSetElement.Name, namedSetElement.Name);
                        metaTreeNodeNamedSet.NodeType = MetaTreeNodeType.NamedSet;
                        if (namedSetObj != null)
                        {
                            metaTreeNodeNamedSet.Description = namedSetObj.Description;
                            metaTreeNodeNamedSet.Properties.Add(new Property(PropertyConstants.NamedSet, namedSetObj));
                            Dimension dimensionObj = cubeSchema.GetDimensionByUniqueName(namedSetElement.DimensionUniqueName);
                            metaTreeNodeNamedSet.UniqueName = namedSetObj.ParentDimensionName;
                            metaTreeNodeNamedSet.Properties.Add(new Property(PropertyConstants.Dimension, dimensionObj));
                            //metaTreeNodeNamedSet.IsSelected = null;
                            //metaTreeNodeNamedSet.__IsSelected = null;
                            metaTreeNodeNamedSet.NodeCheckedType = MetaTreeNodeCheckedType.CurrentChecked;
                        }
                        //MetaTreeHelper.FillMetaTreeNode(metaTreeNodeNamedSet, cubeSchema.Kpis, false, true);
                        //// Adding to the MetaTreeNode collection
                        this.MetaTreeNodes.Add(metaTreeNodeNamedSet);
                    }
                    else
                        this.MetaTreeNodes.Add(new MetaTreeNode(namedSetElement.Name, namedSetElement.Name, namedSetElement.Name) { NodeType = MetaTreeNodeType.NamedSet });
                    #endregion
                }
                else if (item.ElementValue is CalculatedMember)
                {
                    #region Load Calculated Members
                    CalculatedMember calcMember = item.ElementValue as CalculatedMember;
                    MetaTreeNode metaTreeNodeCalcMember = new MetaTreeNode(calcMember.Name, calcMember.Name, calcMember.Name);
                    metaTreeNodeCalcMember.UniqueName = calcMember.UniqueName;
                    metaTreeNodeCalcMember.NodeType = MetaTreeNodeType.CalculatedMember;
                    metaTreeNodeCalcMember.Properties.Add(new Property(PropertyConstants.CalculatedMemberNodeName, calcMember));
                    metaTreeNodeCalcMember.UniqueName = calcMember.UniqueName;
                    this.MetaTreeNodes.Add(metaTreeNodeCalcMember);
                    #endregion
                }
            }
        }

        internal bool UpdateExcludeNodes(LevelElement excludedLevelElement, MetaTreeNode metaTreeNodeMembers)
        {
            bool childUpdated = false;
            Property property = metaTreeNodeMembers.Properties.FindByName(PropertyConstants.Member);
            if (property != null && property.Value is Member)
            {
                Member memberObj = property.Value as Member;
                if (this.OlapDataManager.ProviderName == Providers.SSAS)
                {
                    if (excludedLevelElement.UniqueName == memberObj.LevelUniqueName) //|| excludedLevelElement.UniqueName.Split('.')[0] + ".[" + excludedLevelElement.Name + "]" == memberObj.LevelUniqueName)
                    {
                        if (excludedLevelElement.MemberElements.Count > 0)
                        {
                            foreach (MemberElement memberElement in excludedLevelElement.MemberElements)
                            {
                                string[] memberObjNames = memberObj.UniqueName.Split('.');
                                if (memberObj.UniqueName == memberElement.UniqueName) //|| memberObjNames[0] + "." + memberObj.LevelUniqueName + "." + memberObjNames[memberObjNames.Length - 1] == memberElement.UniqueName || memberObj.CustomUniqueName == memberElement.UniqueName || memberObj.Name == memberElement.Name)
                                {
                                    metaTreeNodeMembers.IsSelected = false;
                                    metaTreeNodeMembers.AcceptIsSelectedChanges(true);
                                    childUpdated = true;
                                }
                            }
                        }
                        else if (excludedLevelElement.MemberElements.Count > 0)
                        {
                            metaTreeNodeMembers.IsSelected = null;
                            metaTreeNodeMembers.AcceptIsSelectedChanges(true);
                            childUpdated = true;
                        }
                    }
                    else
                    {
                        if (!metaTreeNodeMembers.HasValidChildren)
                            MetaTreeHelper.FillMetaTreeNode(metaTreeNodeMembers, memberObj, true, true);
                        foreach (MetaTreeNode childMetaTreeNode in metaTreeNodeMembers.ChildNodes)
                        {
                            if (UpdateExcludeNodes(excludedLevelElement, childMetaTreeNode))
                            {
                                childUpdated = true;
                            }
                        }
                    }
                }
                else
                {
                    if (excludedLevelElement.UniqueName == memberObj.LevelUniqueName || excludedLevelElement.UniqueName.Split('.')[0] + ".[" + excludedLevelElement.Name + "]" == memberObj.LevelUniqueName)
                    {
                        if (excludedLevelElement.MemberElements.Count > 0)
                        {
                            foreach (MemberElement memberElement in excludedLevelElement.MemberElements)
                            {
                                string[] memberObjNames = memberObj.UniqueName.Split('.');
                                if (memberObj.UniqueName == memberElement.UniqueName || memberObjNames[0] + "." + memberObj.LevelUniqueName + "." + memberObjNames[memberObjNames.Length - 1] == memberElement.UniqueName || memberObj.CustomUniqueName == memberElement.UniqueName || memberObj.Name == memberElement.Name)
                                {
                                    metaTreeNodeMembers.IsSelected = false;
                                    metaTreeNodeMembers.AcceptIsSelectedChanges(true);
                                    childUpdated = true;
                                }
                            }
                        }
                        else if (excludedLevelElement.MemberElements.Count > 0)
                        {
                            metaTreeNodeMembers.IsSelected = null;
                            metaTreeNodeMembers.AcceptIsSelectedChanges(true);
                            childUpdated = true;
                        }
                    }
                    else
                    {
                        if (!metaTreeNodeMembers.HasValidChildren)
                            MetaTreeHelper.FillMetaTreeNode(metaTreeNodeMembers, memberObj, true, true);
                        foreach (MetaTreeNode childMetaTreeNode in metaTreeNodeMembers.ChildNodes)
                        {
                            if (UpdateExcludeNodes(excludedLevelElement, childMetaTreeNode))
                            {
                                childUpdated = true;
                            }
                        }
                    }
                }
                if (excludedLevelElement.MemberElements.Count > 0)
                {
                    for (int i = 0; i < excludedLevelElement.MemberElements.Count; i++)
                        if (excludedLevelElement.MemberElements[i].RootNodeCaption == metaTreeNodeMembers.Caption)
                        {
                            metaTreeNodeMembers.IsSelected = null;
                            metaTreeNodeMembers.AcceptIsSelectedChanges(true);
                            childUpdated = true;
                        }
                }
            }

            return childUpdated;
        }

        private void UpdateIncludeNodes(LevelElement includeLevelElement, MetaTreeNode metaTreeNodeMembers)
        {
            Property property = metaTreeNodeMembers.Properties.FindByName(PropertyConstants.Member);
            if (property != null && property.Value is Member)
            {
                Member memberObj = property.Value as Member;
                if (includeLevelElement.UniqueName == memberObj.LevelUniqueName || includeLevelElement.UniqueName.Split('.')[0] + ".[" + includeLevelElement.Name + "]" == memberObj.LevelUniqueName)
                {
                    foreach (MemberElement memberElement in includeLevelElement.MemberElements)
                    {
                        string[] memberObjNames = memberObj.UniqueName.Split('.');
                        if (memberObj.Name != memberElement.Name)
                        {
                            metaTreeNodeMembers.IsSelected = false;
                            metaTreeNodeMembers.AcceptIsSelectedChanges(true);
                        }
                        else
                        {
                            metaTreeNodeMembers.IsSelected = true;
                            metaTreeNodeMembers.AcceptIsSelectedChanges(true);
                            break;
                        }
                    }
                }
            }
        }

        private MetaTreeNode GetMeasureNode(MetaTreeNode metaTreeNodeMeasures, MeasureElement measureElement, bool selectedState)
        {
            foreach (MetaTreeNode childNode in metaTreeNodeMeasures.ChildNodes)
            {
                //// Searching for the measure node and marking as IsSelected = true;
                if (childNode.UniqueName.ToLower() == measureElement.UniqueName.ToLower())
                {
                    childNode.IsSelected = selectedState;
                    childNode.AcceptIsSelectedChanges(true);
                    return childNode;
                }
                else
                {
                    if (childNode.ChildNodes.Count > 0)
                    {
                        MetaTreeNode node = this.GetMeasureNode(childNode, measureElement, selectedState);
                        if (node != null)
                            return node;
                    }
                }
            }
            return null;
        }

        private void AddToReport(Items sourceItems)
        {
            if (sourceItems != null)
            {
                if (this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.MeasureGroup || this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.CalculatedMember && this.DragDropManager.SelectedNode.Properties[0] != null && this.DragDropManager.SelectedNode.Properties[0].Value is CalculatedMember && (this.DragDropManager.SelectedNode.Properties[0].Value as CalculatedMember).Type == TypeOfMember.Measure)
                {
                    AxisElementBuilder measureAxis = FindMeasureAxis();
                    if (measureAxis != null)
                    {
                        MetaTreeNode measureNode = this.FindMeasureNode(measureAxis.MetaTreeNodes.ToList(), null, false);
                        if (measureNode != null)
                        {
                            this.ReportItems.Add(this.Synchronize(sourceItems, measureNode));
                        }

                        List<MetaTreeNode> calcMeasureNodes = measureAxis.MetaTreeNodes.Where(i => i.NodeType == MetaTreeNodeType.CalculatedMember && (i.Properties[0].Value as CalculatedMember).Type == TypeOfMember.Measure).ToList<MetaTreeNode>();
                        foreach (MetaTreeNode item in calcMeasureNodes)
                        {
                            this.ReportItems.Add(this.Synchronize(sourceItems, item));
                        }
                    }
                }
                else
                {
                    this.ReportItems.Add(this.Synchronize(sourceItems, this.DragDropManager.SelectedNode));
                }
            }
        }

        internal Item Synchronize(Items sourceItems, MetaTreeNode currentNode)
        {
            Item tempItem = null;
            for (int i = 0; i < sourceItems.Count; i++)
            {
                if (currentNode.NodeType == MetaTreeNodeType.Dimension)// && sourceItems[i].ElementValue.Name == currentNode.Name)
                {
                    if (sourceItems[i].ElementValue.Name != null && sourceItems[i].ElementValue.Name.ToUpper() == currentNode.Name.ToUpper())
                    {
                        if (this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.Dimension)
                        {
                            if (DragDropManager.SelectedNode.UniqueName.ToUpper() != (sourceItems[i].ElementValue as DimensionElement).UniqueName.ToUpper())
                                continue;
                        }
                        else if (this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.Hierarchy)
                        {
                            if ((this.DragDropManager.SelectedNode.Properties[0].Value as Hierarchy).UniqueName.ToUpper() != (sourceItems[i].ElementValue as DimensionElement).Hierarchy.UniqueName.ToUpper())
                                continue;
                        }
                        else if (this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.Level)
                        {
                            if ((this.DragDropManager.SelectedNode.Properties[0].Value as Level).UniqueName.ToUpper() != (sourceItems[i].ElementValue as DimensionElement).Hierarchy.LevelElements[0].UniqueName.ToUpper())
                                continue;
                        }
                        MetaTreeNode draggedNodeItem = new MetaTreeNode();
                        if (this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.Dimension)
                        {
                            draggedNodeItem = this.GetDimensionNode(this.DragDropManager.SelectedNode.Properties.FindByName(PropertyConstants.Dimension).Value as Dimension,this.DragDropManager.SelectedNode.Caption);
                        }
                        if (this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.Hierarchy)
                        {
                            draggedNodeItem = this.GetDefaultLevelMembersNode(this.DragDropManager.SelectedNode.Properties[0].Value as Hierarchy, this.DragDropManager.SelectedNode.GetRootNode().Name, this.DragDropManager.SelectedNode.Caption);
                        }
                        if (this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.Level)
                        {
                            Dimension dimensionObj = this.DragDropManager.SelectedNode.GetRootNode().Properties.FindByName(PropertyConstants.Dimension).Value as Dimension;
                            MetaTreeNode hierarchyNode = this.GetHierarchyNode(dimensionObj.Hierarchies, dimensionObj.DefaultHierarchyName, dimensionObj.Name, this.DragDropManager.SelectedNode.Caption);
                            Hierarchy hierarchyobj = hierarchyNode.Properties.FindByName(PropertyConstants.Hierarchy).Value as Hierarchy;
                            draggedNodeItem = this.GetMemberNode(hierarchyobj, this.DragDropManager.SelectedNode.Properties[0].Value as Level, dimensionObj.Name, this.DragDropManager.SelectedNode.Caption);
                        }
                        tempItem = sourceItems[i];
                        sourceItems.Remove(tempItem);
                        break;
                    }
                }
                else if (currentNode.NodeType == MetaTreeNodeType.MeasureGroup && sourceItems[i].ElementValue is MeasureElements)
                {
                    tempItem = sourceItems[i];
                    sourceItems.Remove(tempItem);
                    break;
                }
                else if (currentNode.NodeType == MetaTreeNodeType.CalculatedMember && sourceItems[i].ElementValue is CalculatedMember)                    
                {
                    if (currentNode.Properties[0] != null && currentNode.Properties[0].Value is CalculatedMember)
                    {
                        if ((sourceItems[i].ElementValue as CalculatedMember).UniqueName == (currentNode.Properties[0].Value as CalculatedMember).UniqueName)
                        {
                            tempItem = sourceItems[i];
                            sourceItems.Remove(tempItem);
                            break;
                        }
                    }

                }
                else if (currentNode.NodeType == MetaTreeNodeType.KPI_ROOT && sourceItems[i].ElementValue is KpiElements)
                {
                    tempItem = sourceItems[i];
                    sourceItems.Remove(tempItem);
                    break;
                }
                else if (currentNode.NodeType == MetaTreeNodeType.VirtualKPIMember && sourceItems[i].ElementValue is VirtualKpiElement)
                {
                    tempItem = sourceItems[i];
                    sourceItems.Remove(tempItem);
                    break;
                }
                else if (currentNode.NodeType == MetaTreeNodeType.NamedSet && sourceItems[i].ElementValue.Name == currentNode.Name)
                {
                    tempItem = sourceItems[i];
                    sourceItems.Remove(tempItem);
                    break;
                }
            }
            return tempItem;
        }

        private Items GetItemsFromMetaTree()
        {
            MetaTreeNodeCollection mtNodeCollection = new MetaTreeNodeCollection(null);
            foreach (MetaTreeNode metaTreeNode in this.MetaTreeNodes)
            {
                MetaTreeHelper.FillSelectedNodeCollectionVersion3(mtNodeCollection, metaTreeNode);
            }

            // Converting the MetaTree to Element Items
           // QueryBuilderEngineHelper.SetProviderName(this.OlapDataManager.ProviderName);
            return QueryBuilderEngineHelper.GetElementItemsFromMetaTree(mtNodeCollection, this.Axis);
        }

        private MetaTreeNode GetDimensionNode(Dimension dimensionObj,string caption)
        {
            return this.GetHierarchyNode(dimensionObj.Hierarchies, dimensionObj.DefaultHierarchyName, dimensionObj.Name,caption);
        }

        private MetaTreeNode GetHierarchyNode(HierarchyCollection hierarchyCollection, string defalutHierarchyName, string dimensionName,string caption)
        {
            if (hierarchyCollection.Count > 0)
            {
                Hierarchy hierarchyObj = null;
                foreach (Hierarchy hierarchy in hierarchyCollection)
                {
                    if (hierarchy.UniqueName == defalutHierarchyName)
                    {
                        hierarchyObj = hierarchy;
                        break;
                    }
                }
                if (hierarchyObj == null)
                {
                    hierarchyObj = hierarchyCollection[0];
                }

                return this.GetDefaultLevelMembersNode(hierarchyObj, dimensionName,caption);
            }
            return null;
        }

        private MetaTreeNode GetDefaultLevelMembersNode(Hierarchy hierarchyObj, string dimensionName,string caption)
        {
            if (hierarchyObj.Levels.Count > 0)
            {
                Level levelObj = null;
                foreach (Level level in hierarchyObj.Levels)
                {
                    if (level.UniqueName == hierarchyObj.DefaultLevelUniqueName)
                    {
                        levelObj = level;
                        break;
                    }
                }

                if (levelObj == null)
                {
                    levelObj = hierarchyObj.Levels[0];
                }

                return this.GetMemberNode(hierarchyObj, levelObj, dimensionName,caption);
            }

            return null;
        }

        private MetaTreeNode GetMemberNode(Hierarchy hierarchyObj, Level levelObj, string dimensionName,string caption)
        {
            if (levelObj != null)
            {
                MetaTreeNode metaTreeNode = new MetaTreeNode();
                metaTreeNode.Caption = caption;
                metaTreeNode.Name = dimensionName;
                metaTreeNode.UniqueName = dimensionName;
                metaTreeNode.NodeType = MetaTreeNodeType.Dimension;
                Dimension dimensionObj = new Dimension();
                dimensionObj = hierarchyObj.ParentDimension;
                metaTreeNode.Properties.Add(new Property(PropertyConstants.Dimension, dimensionObj));
                metaTreeNode.Properties.Add(new Property(PropertyConstants.Level, levelObj));
                metaTreeNode.Properties.Add(new Property(PropertyConstants.Hierarchy, hierarchyObj));
                //metaTreeNode.ForceAcceptChanges();
                //foreach (Member memberObj in levelObj.Members)
                //{
                //    MetaTreeNode newMemberNode = new MetaTreeNode();
                //    MetaTreeHelper.FillMetaTreeNode(newMemberNode, memberObj, true);
                //    newMemberNode.ParentNode = metaTreeNode;
                //    metaTreeNode.ChildNodes.Add(newMemberNode);
                //}
                metaTreeNode.IsSelected = true;
                return metaTreeNode;
            }

            return null;
        }

        private void GetLevelMembers(Level levelObj)
        {
            this.OlapDataManager.GetLevelMembers(levelObj.UniqueName, this.OlapDataManager.CurrentCubeName);
        }

        public void UpdateChild(AxisElementBuilder axisElementBuiler)
        {
            //// Get the current manager from the manager
            OlapReport currentReport = this.OlapDataManager.CurrentReport;
            //// if filtering applied then it will not be persisted upon dragges 
            /// so clearing here
            if (currentReport.FilterElements.Count > 0)
            {
                currentReport.FilterElements.Clear();
                currentReport.CategoricalElements.IsFilterOrSortOn = false;
                currentReport.SeriesElements.IsFilterOrSortOn = false;
                currentReport.SlicerElements.IsFilterOrSortOn = false;
            }

            MetaTreeNodeCollection mtNodeCollection = new MetaTreeNodeCollection(null);
            foreach (MetaTreeNode metaTreeNode in this.MetaTreeNodes)
            {
                MetaTreeHelper.FillSelectedNodeCollectionVersion3(mtNodeCollection, metaTreeNode);
            }

            Items tempItems = this.ReportItems;

            if (mtNodeCollection.Count > 0)
            {
                //// Synchronize the element selection
                //QueryBuilderEngineHelper.SetProviderName(this.OlapDataManager.ProviderName);
                QueryBuilderEngineHelper.UpdateItemsFromMetaTree(mtNodeCollection, this.Axis, tempItems);
            }
            else
            {
                tempItems.Clear();
            }
            if (this.AutoExecute)
                this.OlapDataManager.NotifyElementChanged(this.Axis);
            else
                this.OlapDataManager.RefreshAxisElementBuilder();
        }

        private bool CheckMeasureRestriction(MetaTreeNode draggedNode)
        {
            //// Check if Measure exist in different axis
            if (draggedNode.NodeType == MetaTreeNodeType.KPI || draggedNode.NodeType == MetaTreeNodeType.KPI_ROOT || draggedNode.NodeType==MetaTreeNodeType.VirtualKPIMember || draggedNode.NodeType==MetaTreeNodeType.VirtualKPIGroup)
            {
                AxisElementBuilder axisMeasure = FindMeasureAxis();
                if (axisMeasure != null)
                {
                    if (axisMeasure.Axis != this.Axis)
                        return true;
                }
            }
            return false;
        }
        private bool CheckVirtualKpiRestriction(MetaTreeNode draggedNode)
        {
            //// Check if kpi exist in different axis
            if (draggedNode.NodeType == MetaTreeNodeType.MeasureGroup)
            {
                AxisElementBuilder axisKpi = FindVirtualKPIAxis();
                if (axisKpi != null)
                {
                    if (axisKpi.Axis != this.Axis)
                        return true;
                }
            }
            return false;
        }
        private bool CheckKpiRestriction(MetaTreeNode draggedNode)
        {
            //// Check if kpi exist in different axis
            if (draggedNode.NodeType == MetaTreeNodeType.MeasureGroup)
            {
                AxisElementBuilder axisKpi = FindKPIAxis();
                if (axisKpi != null)
                {
                    if (axisKpi.Axis != this.Axis)
                        return true;
                }
            }
            return false;
        }

        private bool CanAddCalculatedMemberAtSlicer(MetaTreeNode currentNode)
        {
            if (currentNode.Properties[0] != null && currentNode.Properties[0].Value is CalculatedMember)
            {
                CalculatedMember calcMember = currentNode.Properties[0].Value as CalculatedMember;
                if (calcMember.Type == TypeOfMember.Measure)
                {
                    AxisElementBuilder measureAxis = FindMeasureAxis();
                    if (measureAxis != null)
                    {
                        if (measureAxis.MetaTreeNodes.Any(i => i.NodeType == MetaTreeNodeType.MeasureGroup))
                        {
                            return false;
                        }

                        if (measureAxis.MetaTreeNodes.Count(i => i.NodeType == MetaTreeNodeType.CalculatedMember && i.Properties[0] != null && i.Properties[0].Value is CalculatedMember && (i.Properties[0].Value as CalculatedMember).Type == TypeOfMember.Measure) > 1)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool CanAddMeasureAtSlicer()
        {
            AxisElementBuilder measureAxis = FindMeasureAxis();
            if (measureAxis != null)
            {
                if (measureAxis.MetaTreeNodes.Count(i => i.NodeType == MetaTreeNodeType.CalculatedMember && i.Properties[0] != null && i.Properties[0].Value is CalculatedMember && (i.Properties[0].Value as CalculatedMember).Type == TypeOfMember.Measure) > 1)
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidateNamedSetPosition(string parentDimensionName)
        {
            AxisElementBuilder axisBuilderDimension = FinDimensionAxis(parentDimensionName);

            //// Check if the Parent Dimension of the named set already exist in the same axis
            //// Then check for the axis if it is slicer then throw exception.
            if (axisBuilderDimension != null && this.Axis == AxisPosition.Slicer)
            {
                return false;
            }

            return true;
        }

        private MetaTreeNode FindMeasureNode(List<MetaTreeNode> currentNodes, MetaTreeNode draggedNode, bool withUniqueName)
        {
            foreach (MetaTreeNode child in currentNodes)
            {
                if (withUniqueName && child.UniqueName == draggedNode.UniqueName)
                {
                    return child;
                }
                else if (!withUniqueName && child.NodeType == MetaTreeNodeType.MeasureGroup)
                {
                    return child;
                }
            }
            return null;
        }

        private MetaTreeNode FindKpiNode(List<MetaTreeNode> currentNodes, MetaTreeNode draggedNode, bool withUniqueName)
        {
            foreach (MetaTreeNode child in currentNodes)
            {
                if (withUniqueName && child.UniqueName == draggedNode.UniqueName)
                {
                    return child;
                }
                else if (!withUniqueName && child.NodeType == MetaTreeNodeType.KPI_ROOT && child.Name == PropertyConstants.KPI)
                {
                    return child;
                }
            }
            return null;
        }
        private AxisElementBuilder FindVirtualKPIAxis()
        {
            var axis = this.OlapDataManager.Properties.Where(a => a.Value is AxisElementBuilder);
            if (axis != null && axis.Count() > 0)
            {
                foreach (var item in axis)
                {
                    AxisElementBuilder axisBuilder = item.Value as AxisElementBuilder;
                    var metaTreeNode = axisBuilder.MetaTreeNodes.Where(i => i.NodeType == MetaTreeNodeType.VirtualKPIMember);
                    if (metaTreeNode != null && metaTreeNode.Count() > 0)
                        return axisBuilder;
                }
            }
            return null;
        }
        private AxisElementBuilder FindKPIAxis()
        {
            var axis = this.OlapDataManager.Properties.Where(a => a.Value is AxisElementBuilder);
            if (axis != null && axis.Count() > 0)
            {
                foreach (var item in axis)
                {
                    AxisElementBuilder axisBuilder = item.Value as AxisElementBuilder;
                    var metaTreeNode = axisBuilder.MetaTreeNodes.Where(i => i.NodeType == MetaTreeNodeType.KPI_ROOT && i.Name == PropertyConstants.KPI);
                    if (metaTreeNode != null && metaTreeNode.Count() > 0)
                        return axisBuilder;
                }
            }
            return null;
        }

        private AxisElementBuilder FindMeasureAxis()
        {
            var axis = this.OlapDataManager.Properties.Where(a => a.Value is AxisElementBuilder);
            if (axis != null && axis.Count() > 0)
            {
                foreach (var item in axis)
                {
                    AxisElementBuilder axisBuilder = item.Value as AxisElementBuilder;
                    var metaTreeNode = axisBuilder.MetaTreeNodes.Where(i => (i.NodeType == MetaTreeNodeType.MeasureGroup && i.Name == PropertyConstants.MeasrueNodeName) || (i.NodeType == MetaTreeNodeType.CalculatedMember && i.Properties[0] != null && i.Properties[0].Value is CalculatedMember && (i.Properties[0].Value as CalculatedMember).Type == TypeOfMember.Measure));
                    if (metaTreeNode != null && metaTreeNode.Count() > 0)
                        return axisBuilder;
                }
            }
            return null;
        }

        private AxisElementBuilder FindNamedsetAxis()
        {
            var axis = this.OlapDataManager.Properties.Where(a => a.Value is AxisElementBuilder);
            if (axis != null && axis.Count() > 0)
            {
                foreach (var item in axis)
                {
                    AxisElementBuilder axisBuilder = item.Value as AxisElementBuilder;
                    var metaTreeNode = axisBuilder.MetaTreeNodes.Where(i => i.NodeType == MetaTreeNodeType.NamedSet);
                    if (metaTreeNode != null && metaTreeNode.Count() > 0)
                        return axisBuilder;
                }
            }
            return null;
        }

        internal AxisElementBuilder FinDimensionAxis(string dimensionName)
        {
            if (this.OlapDataManager != null)
            {
                var axis = this.OlapDataManager.Properties.Where(a => a.Value is AxisElementBuilder);
                if (axis != null && axis.Count() > 0)
                {
                    foreach (var item in axis)
                    {
                        AxisElementBuilder axisBuilder = item.Value as AxisElementBuilder;
                        var metaTreeNode = axisBuilder.MetaTreeNodes.Where(i => i.NodeType == MetaTreeNodeType.Dimension && i.Name.ToUpper() == dimensionName.ToUpper());
                        if (metaTreeNode != null && metaTreeNode.Count() > 0)
                            return axisBuilder;
                    }
                }
            }
            return null;
        }

        private AxisElementBuilder FinDimensionAxis(string dimensionName, string hierarchyName)
        {
            if (this.OlapDataManager != null)
            {
                var axis = this.OlapDataManager.Properties.Where(a => a.Value is AxisElementBuilder);
                if (axis != null && axis.Count() > 0)
                {
                    foreach (var item in axis)
                    {
                        AxisElementBuilder axisBuilder = item.Value as AxisElementBuilder;
                        var metaTreeNode = axisBuilder.MetaTreeNodes.Where(i => i.NodeType == MetaTreeNodeType.Dimension && i.Name.ToUpper() == dimensionName.ToUpper() && (i.Properties[1].Value as Hierarchy).Name.ToUpper() == hierarchyName.ToUpper());
                        if (metaTreeNode != null && metaTreeNode.Count() > 0)
                            return axisBuilder;
                    }
                }
            }
            return null;
        }

        internal Item FindDimensionItem(AxisElementBuilder axis, MetaTreeNode dimensionNode)
        {
            foreach (Item reportItem in axis.ReportItems)
            {
                if (reportItem.ElementValue is DimensionElement && (reportItem.ElementValue as DimensionElement).UniqueName.ToUpper() == dimensionNode.UniqueName.ToUpper() &&
                    (((reportItem.ElementValue as DimensionElement).Hierarchy.UniqueName.ToUpper() == (dimensionNode.Properties[1].Value as Hierarchy).UniqueName.ToUpper()) || ((reportItem.ElementValue as DimensionElement).Hierarchy.UniqueName.ToUpper() == (dimensionNode.UniqueName + "." + (dimensionNode.Properties[1].Value as Hierarchy).UniqueName).ToUpper())))
                    return reportItem;
            }
            return null;
        }

        internal MetaTreeNode FindDimensionNode(ObservableCollection<MetaTreeNode> metaTreeNodes, string dimnesionName)
        {
            foreach (MetaTreeNode treeNode in metaTreeNodes)
            {
                if (treeNode.NodeType == MetaTreeNodeType.Dimension && treeNode.Name.ToUpper() == dimnesionName.ToUpper())
                {
                    return treeNode;
                }
            }
            return null;
        }

        private MetaTreeNode FindDimensionNode(ObservableCollection<MetaTreeNode> metaTreeNodes, string dimnesionName, string hierarchyName)
        {
            foreach (MetaTreeNode treeNode in metaTreeNodes)
            {
                if (treeNode.NodeType == MetaTreeNodeType.Dimension && treeNode.Name.ToUpper() == dimnesionName.ToUpper() && (treeNode.Properties[1].Value as Hierarchy).Name.ToUpper() == hierarchyName.ToUpper())
                {
                    return treeNode;
                }
            }
            return null;
        }

        #endregion

        #region Overrided methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            base.MouseLeftButtonUp += new MouseButtonEventHandler(OnAxisMouseLeftButtonUp);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.ReleaseMouseCapture();
        }

        Item GetDraggedItem(Items frmMetaTree, Items frmReport)
        {
            if (frmMetaTree != null)
            {
                foreach (Item item in frmMetaTree)
                {
                    if (frmReport.Count != 0)
                    {
                        foreach (Item reItem in frmReport)
                        {
                            if (item.ElementValue is DimensionElement)
                            {
                                if (reItem.ElementValue is DimensionElement)
                                {
                                    if (((item.ElementValue as DimensionElement).Hierarchy as HierarchyElement).UniqueName != ((reItem.ElementValue as DimensionElement).Hierarchy as HierarchyElement).UniqueName)
                                    {
                                        return item;
                                    }
                                }
                                else
                                    return item;
                            }
                        }
                    }
                    else
                        return item;
                }
            }
            return null;
        }

        Items GetDraggedMeasure(Items frmMetaTree, Items frmReport)
        {   
            Item dragItem = new Item(); Item repMesItem = new Item();
            foreach (Item reItem in frmReport)
            {
                foreach (Item item in frmMetaTree)
                {
                    if (item.ElementValue is MeasureElements)
                    {
                        if (item.ElementValue is MeasureElements == reItem.ElementValue is MeasureElements)
                        {
                            dragItem = item;
                            repMesItem = reItem;
                        }
                    }
                }
            }

            if (dragItem.ElementValue is MeasureElements)
            {
                frmReport.Remove(repMesItem);
                frmReport.Add(dragItem);
            }
            return frmReport;
        }

        Items GetDraggedKPI(Items frmMetaTree, Items frmReport)
        {
            Item dragItem = new Item(); Item repMesItem = new Item();
            foreach (Item reItem in frmReport)
            {
                foreach (Item item in frmMetaTree)
                {
                    if (item.ElementValue is KpiElements)
                    {
                        if (item.ElementValue is KpiElements == reItem.ElementValue is KpiElements)
                        {
                            dragItem = item;
                            repMesItem = reItem;
                        }
                        else
                            dragItem = item;
                    }
                }
            }

            if (dragItem.ElementValue is KpiElements)
            {
                frmReport.Remove(repMesItem);
                frmReport.Add(dragItem);
            }
            return frmReport;
        }

        Items GetAxisItemsFromReport()
        {
            Items reportItems = new Items();
            if (this.Axis == AxisPosition.Categorical)
                reportItems = this.OlapDataManager.CurrentReport.CategoricalElements;
            else if (this.Axis == AxisPosition.Series)
                reportItems = this.OlapDataManager.CurrentReport.SeriesElements;
            else if (this.Axis == AxisPosition.Slicer)
                reportItems = this.OlapDataManager.CurrentReport.SlicerElements;
            return reportItems;
        }

        void OnAxisMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
            //{
            //    base.OnMouseLeftButtonUp(e);

            if (this.DragDropManager != null)
            {
                if (this.DragDropManager.Source is AxisElementBuilder && this.Axis == this.DragDropManager.SourceAxis)
                {
                    #region Invalid dropping region

                    this.DragDropManager.DragDropPopup.IsOpen = false;
                    this.DragDropManager = null;
                    this.ReleaseMouseCapture();
                    this.SelectedIndex = -1;

                    #endregion
                }
                else
                {
                    try
                    {
                        if (this.OlapDataManager.CurrentReport.EnablePaging)
                        {
                            ////Resets the current page sizes on current report
                            ResetCurrentPages();
                        }

                        MetaTreeNode draggedNode = this.DragDropManager.SelectedNode;
                        MetaTreeNode rootNode = draggedNode.GetRootNode();

                        if (draggedNode.NodeType == MetaTreeNodeType.Level && this.OlapDataManager.ProviderName != Providers.ActivePivot)
                            draggedNode = draggedNode.ParentNode;

                        Property nodeProperty = draggedNode.Properties[0];


                        if (this.DragDropManager.Source is AxisElementBuilder)
                        {
                            #region Source is AxisElementBuilder

                            Items tempItems = null;
                            switch (this.DragDropManager.SourceAxis)
                            {
                                case AxisPosition.Categorical:
                                    tempItems = this.OlapDataManager.CurrentReport.CategoricalElements;
                                    break;
                                case AxisPosition.Series:
                                    tempItems = this.OlapDataManager.CurrentReport.SeriesElements;
                                    break;
                                case AxisPosition.Slicer:
                                    tempItems = this.OlapDataManager.CurrentReport.SlicerElements;
                                    break;
                            }

                            ///Synchronize the element value and excluded element value with ReportItems
                            MetaTreeNodeCollection mtNodeCollection = new MetaTreeNodeCollection(null);
                            mtNodeCollection.Add(draggedNode);
                            if (mtNodeCollection.Count > 0)
                            {
                              // QueryBuilderEngineHelper.UpdateItemsFromMetaTree(mtNodeCollection, this.Axis, tempItems);
                            }

                            if (this.Axis == AxisPosition.Categorical || this.Axis == AxisPosition.Series)
                            {
                                if (this.CheckKpiRestriction(draggedNode) || this.CheckMeasureRestriction(draggedNode) || this.CheckVirtualKpiRestriction(draggedNode))
                                {
                                    throw new Exception(SR.GetString(CultureInfo.CurrentUICulture, "Exception_MeasureAndKPIShouldComeUnderSameAxis"));
                                }
                                else
                                {
                                    this.AddToReport(tempItems);
                                }
                            }
                            else
                            {
                                //// More than one measure cannot be drop in slicer
                                if (draggedNode.NodeType == MetaTreeNodeType.MeasureGroup && draggedNode.ChildNodes.Count > 1 || !CanAddMeasureAtSlicer())
                                {
                                    throw new Exception(SR.GetString(CultureInfo.CurrentUICulture,"Exception_MoreThanOneMeasureCouldNotBeDropInSlicer"));
                                }
                                //// KPI's cannot be dropped in slicer
                                else if ((draggedNode.NodeType == MetaTreeNodeType.KPI_ROOT && draggedNode.Name == PropertyConstants.KPI) || (draggedNode.NodeType==MetaTreeNodeType.VirtualKPIMember))
                                {
                                    throw new Exception(SR.GetString(CultureInfo.CurrentUICulture,"Exception_SinceKPIisACalculatedMeasureItCannotBeSliced"));
                                }
                                //// Check for Parent Dimension of Namedset 
                                else if (draggedNode.NodeType == MetaTreeNodeType.NamedSet && !ValidateNamedSetPosition(draggedNode.UniqueName))
                                {
                                    throw new Exception(SR.GetString(CultureInfo.CurrentUICulture,"Exception_NamedSetCannotBePresentInSlicerIfTheParentDimensionExistsInAnyOtherAxis"));
                                }
                                else if (draggedNode.NodeType == MetaTreeNodeType.CalculatedMember && !CanAddCalculatedMemberAtSlicer(draggedNode))
                                {
                                    throw new Exception(SR.GetString(CultureInfo.CurrentUICulture,"Exception_MoreThanOneMeasureCouldNotBeDropInSlicer"));
                                }
                                else
                                {
                                    this.AddToReport(tempItems);
                                }

                            }
                            if (this.AutoExecute)
                                this.OlapDataManager.NotifyElementChanged();
                            else
                                this.OlapDataManager.RefreshAxisElementBuilder();

                            #endregion
                        }
                        else if (this.DragDropManager.Source is CubeDimensionBrowser)
                        {
                            #region Source is CubeDimension Browser

                            if (rootNode.Name != PropertyConstants.KPI && this.CheckForExistence(rootNode) ||( rootNode.Name == PropertyConstants.VirtualKpiHeaderName && this.CheckForExistence(draggedNode)))
                            {
                                throw new Exception(SR.GetString(CultureInfo.CurrentUICulture,"Exception_ThisElementIsAlreadyExistInTheCurrentAxis"));
                            }
                            else if (nodeProperty.Value is Dimension)
                            {
                                #region Dropping Dimension

                                Item tempItem = null;

                                this.MetaTreeNodes.Add(this.GetDimensionNode(draggedNode.Properties.FindByName(PropertyConstants.Dimension).Value as Dimension,draggedNode.Caption));
                                //this.MetaTreeNodes.Add(draggedNode);
                                tempItem = this.Synchronize(this.OlapDataManager.CurrentReport.CategoricalElements, draggedNode);
                                if (tempItem == null)
                                    tempItem = this.Synchronize(this.OlapDataManager.CurrentReport.SeriesElements, draggedNode);
                                if (tempItem == null)
                                    tempItem = this.Synchronize(this.OlapDataManager.CurrentReport.SlicerElements, draggedNode);
                                if (IsSavedReport)
                                {
                                    Items reportItems = new Items();
                                    reportItems = GetAxisItemsFromReport();
                                    Item draggedItem = new Item();
                                    Items axisItems = this.GetItemsFromMetaTree();
                                    draggedItem = GetDraggedItem(axisItems, reportItems);
                                    reportItems.Add(draggedItem);
                                    this.ReportItems = reportItems;
                                } 
                                else
                                {
                                    this.ReportItems = this.GetItemsFromMetaTree();
                                }

                                #endregion
                            }
                            else if (nodeProperty.Value is Hierarchy)
                            {
                                #region Dropping Hierarchy

                                Dimension dimensionObj = rootNode.Properties.FindByName(PropertyConstants.Dimension).Value as Dimension;
                                MetaTreeNode tempNode = this.GetDefaultLevelMembersNode(draggedNode.Properties[0].Value as Hierarchy, dimensionObj.Name,draggedNode.Caption);
                                this.MetaTreeNodes.Add(tempNode);
                                Item tempItem = null;

                                tempItem = this.Synchronize(this.OlapDataManager.CurrentReport.CategoricalElements, tempNode);
                                if (tempItem == null)
                                    tempItem = this.Synchronize(this.OlapDataManager.CurrentReport.SeriesElements, tempNode);
                                if (tempItem == null)
                                    tempItem = this.Synchronize(this.OlapDataManager.CurrentReport.SlicerElements, tempNode);
                                if (IsSavedReport)
                                {
                                    Items reportItems = new Items();
                                    reportItems = GetAxisItemsFromReport();
                                    Items axisItems = this.GetItemsFromMetaTree();
                                    Item dragItem = new Item();
                                    dragItem = GetDraggedItem(axisItems, reportItems);
                                    reportItems.Add(dragItem);
                                    this.ReportItems = reportItems;
                                }
                                else
                                {
                                    this.ReportItems = this.GetItemsFromMetaTree();
                                }

                                #endregion
                            }
                            else if (nodeProperty.Value is Level)
                            {
                                #region Dropping Level elements

                                Dimension dimensionObj = rootNode.Properties.FindByName(PropertyConstants.Dimension).Value as Dimension;
                                //// This line are commented out since each level node takes its own parent node as a member and modified the alternate way
                                //MetaTreeNode hierarchyNode = this.GetHierarchyNode(dimensionObj.Hierarchies, dimensionObj.DefaultHierarchyName, dimensionObj.Name);
                                MetaTreeNode hierarchyNode = this.GetHierarchyNode(dimensionObj.Hierarchies, draggedNode.ParentNode.UniqueName, dimensionObj.Name,dimensionObj.Caption);
                                Hierarchy hierarchyobj = hierarchyNode.Properties.FindByName(PropertyConstants.Hierarchy).Value as Hierarchy;
                                MetaTreeNode tempNode = this.GetMemberNode(hierarchyobj, draggedNode.Properties[0].Value as Level, dimensionObj.Name,dimensionObj.Caption);

                                this.MetaTreeNodes.Add(tempNode);
                                Item tempItem = null;

                                tempItem = this.Synchronize(this.OlapDataManager.CurrentReport.CategoricalElements, tempNode);
                                if (tempItem == null)
                                    tempItem = this.Synchronize(this.OlapDataManager.CurrentReport.SeriesElements, tempNode);
                                if (tempItem == null)
                                    tempItem = this.Synchronize(this.OlapDataManager.CurrentReport.SlicerElements, tempNode);
                                if (IsSavedReport)
                                {
                                    Items reportItems = new Items();
                                    reportItems = GetAxisItemsFromReport();
                                    Items axisItems = this.GetItemsFromMetaTree();
                                    Item dragItem = new Item();

                                    dragItem = GetDraggedItem(axisItems, reportItems);
                                    reportItems.Add(dragItem);
                                    this.ReportItems = reportItems;
                                }
                                else
                                {
                                    this.ReportItems = this.GetItemsFromMetaTree();
                                }
                                #endregion
                            }
                            else if (draggedNode.NodeType == MetaTreeNodeType.Measure || draggedNode.NodeType == MetaTreeNodeType.CalculatedMember || draggedNode.NodeType == MetaTreeNodeType.VirtualKPIGroup || draggedNode.NodeType == MetaTreeNodeType.VirtualKPIMember || draggedNode.NodeType == MetaTreeNodeType.VirtualKPI_Value || draggedNode.NodeType == MetaTreeNodeType.VirtualKPI_Trend || draggedNode.NodeType == MetaTreeNodeType.VirtualKPI_Status || draggedNode.NodeType == MetaTreeNodeType.VirtualKPI_Goal)
                            {
                                CalculatedMember calcMember = (draggedNode.Properties[0] != null && draggedNode.Properties[0].Value is CalculatedMember) ? draggedNode.Properties[0].Value as CalculatedMember : null;
                                VirtualKpiElement virtualKpi = (draggedNode.Properties[0] != null && draggedNode.Properties[0].Value is VirtualKpiElement) ? draggedNode.Properties[0].Value as VirtualKpiElement : null;
                                if (calcMember != null && calcMember.Type == TypeOfMember.Dimension)
                                {
                                    #region Dropping Calculated Dimension Members
                                    Item tempItem = null;

                                    this.MetaTreeNodes.Add(draggedNode);
                                    //this.MetaTreeNodes.Add(draggedNode);
                                    tempItem = this.Synchronize(this.OlapDataManager.CurrentReport.CategoricalElements, draggedNode);
                                    if (tempItem == null)
                                        tempItem = this.Synchronize(this.OlapDataManager.CurrentReport.SeriesElements, draggedNode);
                                    if (tempItem == null)
                                        tempItem = this.Synchronize(this.OlapDataManager.CurrentReport.SlicerElements, draggedNode);

                                    this.ReportItems = this.GetItemsFromMetaTree();
                                    #endregion
                                }
                                else if (virtualKpi != null)
                                {
                                    //  Item tempItem = null;
                                    AxisElementBuilder measureAxis = FindMeasureAxis();
                                    AxisElementBuilder kpiAxis = FindKPIAxis();

                                    if (measureAxis != null && measureAxis.Axis != this.Axis)
                                    {
                                        throw new Exception(SR.GetString(CultureInfo.CurrentUICulture, "Exception_MeasureAndKPIShouldComeUnderSameAxis"));
                                    }
                                    else
                                    {
                                        if (draggedNode.NodeType == MetaTreeNodeType.VirtualKPI_Value || draggedNode.NodeType == MetaTreeNodeType.VirtualKPI_Goal || draggedNode.NodeType == MetaTreeNodeType.VirtualKPI_Status || draggedNode.NodeType == MetaTreeNodeType.VirtualKPI_Trend)
                                        {
                                            if (draggedNode.ParentNode.IsSelected == true)
                                            {
                                                draggedNode.ParentNode.IsSelected = false;
                                                draggedNode.ParentNode.AcceptIsSelectedChanges(false);
                                            }
                                            draggedNode.IsSelected = true;
                                            draggedNode.AcceptIsSelectedChanges(true);
                                            draggedNode = draggedNode.ParentNode;
                                        }
                                        this.MetaTreeNodes.Add(draggedNode);
                                        this.ReportItems = this.GetItemsFromMetaTree();
                                    }
                                }
                                else
                                {
                                    #region Dropping Measure/Calculated Measure elements

                                    MetaTreeNode measureNodes = null;
                                    MetaTreeNode tempNode = null;
                                    if (this.Axis == AxisPosition.Slicer)
                                    {
                                        AxisElementBuilder kpiAxis = FindKPIAxis();
                                        AxisElementBuilder measureAxis = FindMeasureAxis();
                                        //// Check whether Kpi and measure are in same axis or not , if not throw exception
                                        if (measureAxis != null || (kpiAxis != null && kpiAxis.Axis != this.Axis))
                                        {
                                            throw new Exception(SR.GetString(CultureInfo.CurrentUICulture, "Exception_MeasureAndKPIShouldComeUnderSameAxis") + "\n" + SR.GetString(CultureInfo.CurrentUICulture, "Exception_AndMoreThanOneMeasureCouldNotBeDroppedInSlicer"));
                                        }
                                        else if (kpiAxis == null || kpiAxis.Axis == this.Axis)
                                        {
                                            UpdateMetaTreeNodes(draggedNode, rootNode, ref measureNodes, ref tempNode, measureAxis);

                                            if (IsSavedReport)
                                            {
                                                Items reportItems = new Items();
                                                reportItems = GetAxisItemsFromReport();
                                                Items axisItems = this.GetItemsFromMetaTree();

                                                this.ReportItems = GetDraggedMeasure(axisItems, reportItems);
                                            }
                                            else
                                            {
                                                this.ReportItems = this.GetItemsFromMetaTree();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        AxisElementBuilder measureAxis = FindMeasureAxis();
                                        if (measureAxis != null && measureAxis.Axis == this.Axis)
                                        {
                                            if (draggedNode.NodeType == MetaTreeNodeType.CalculatedMember)
                                            {
                                                this.MetaTreeNodes.Add(draggedNode);
                                            }
                                            else
                                            {
                                                measureNodes = FindMeasureNode(this.MetaTreeNodes.ToList(), null, false);
                                                if (measureNodes != null)
                                                {
                                                    tempNode = FindMeasureNode(measureNodes.ChildNodes.ToList(), draggedNode, true);

                                                    if (tempNode == null)
                                                    {
                                                        measureNodes.ChildNodes.Add(draggedNode);
                                                    }
                                                }
                                                else
                                                {
                                                    UpdateMetaTreeNodes(draggedNode, rootNode, ref measureNodes, ref tempNode, measureAxis);
                                                }
                                            }
                                            if (IsSavedReport)
                                            {
                                                Items reportItems = new Items();
                                                reportItems = GetAxisItemsFromReport();
                                                Items axisItems = this.GetItemsFromMetaTree();

                                                this.ReportItems = GetDraggedMeasure(axisItems, reportItems);

                                            }
                                            else
                                            {
                                                this.ReportItems = this.GetItemsFromMetaTree();
                                            }
                                        }
                                        else
                                        {
                                            AxisElementBuilder kpiAxis = FindKPIAxis();
                                            AxisElementBuilder virtualKpiAxis = FindVirtualKPIAxis();
                                            //// Check whether Kpi and measure are in same axis or not , if not throw exception
                                            if (kpiAxis != null && kpiAxis.Axis != this.Axis)
                                            {
                                                throw new Exception(SR.GetString(CultureInfo.CurrentUICulture, "Exception_MeasureAndKPIShouldComeUnderSameAxis"));
                                            }
                                            if (virtualKpiAxis != null && virtualKpiAxis.Axis != this.Axis)
                                            {
                                                throw new Exception(SR.GetString(CultureInfo.CurrentUICulture, "Exception_MeasureAndKPIShouldComeUnderSameAxis"));
                                            }
                                            else
                                            {
                                                if (!IsSavedReport)
                                                {
                                                    UpdateMetaTreeNodes(draggedNode, rootNode, ref measureNodes, ref tempNode, measureAxis);

                                                    this.ReportItems = this.GetItemsFromMetaTree();
                                                }
                                                else
                                                {
                                                    Items reportItems = new Items();
                                                    reportItems = GetAxisItemsFromReport();
                                                    UpdateMetaTreeNodes(draggedNode, rootNode, ref measureNodes, ref tempNode, measureAxis);
                                                    Items axisItems = this.GetItemsFromMetaTree();
                                                    Item dragItem = new Item(); 
                                                    if (reportItems != null)
                                                    {
                                                        foreach (Item reItem in reportItems)
                                                        {
                                                            foreach (Item item in axisItems)
                                                            {
                                                                if (item.ElementValue is MeasureElements)
                                                                {
                                                                    dragItem = item;
                                                                }
                                                            }
                                                        }
                                                        if (dragItem.ElementValue is MeasureElements)
                                                        {
                                                            reportItems.Add(dragItem);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        reportItems = this.GetItemsFromMetaTree();
                                                    }
                                                    
                                                    this.ReportItems = reportItems;
                                                }
                                            }
                                        }
                                    }

                                    #endregion
                                }
                            }
                            else if (draggedNode.NodeType == MetaTreeNodeType.KPI_Goal || draggedNode.NodeType == MetaTreeNodeType.KPI_ROOT ||
                                    draggedNode.NodeType == MetaTreeNodeType.KPI_Status || draggedNode.NodeType == MetaTreeNodeType.KPI_Trend ||
                                    draggedNode.NodeType == MetaTreeNodeType.KPI_Value || draggedNode.NodeType == MetaTreeNodeType.KPI)
                            {
                                #region Dropping Kpi elements

                                if (draggedNode.NodeType != MetaTreeNodeType.KPI_ROOT && draggedNode.NodeType != MetaTreeNodeType.KPI)
                                {
                                    if (draggedNode.ParentNode.IsSelected == true)
                                    {
                                        draggedNode.ParentNode.IsSelected = false;
                                        draggedNode.ParentNode.AcceptIsSelectedChanges(false);
                                    }
                                    draggedNode.IsSelected = true;
                                    draggedNode.AcceptIsSelectedChanges(true);
                                    draggedNode = draggedNode.ParentNode;
                                }
                                if (this.Axis == AxisPosition.Slicer)
                                {
                                    throw new Exception(SR.GetString(CultureInfo.CurrentUICulture,"Exception_SinceKPIisACalculatedMeasureItCannotBeSliced"));
                                }
                                else
                                {
                                    MetaTreeNode kpiNodes = null;
                                    MetaTreeNode tempNode = null;
                                    kpiNodes = FindKpiNode(this.MetaTreeNodes.ToList(), null, false);
                                    if (kpiNodes != null)
                                    {
                                        tempNode = FindKpiNode(kpiNodes.ChildNodes.ToList(), draggedNode, true);

                                        if (tempNode == null)
                                        {
                                            kpiNodes.ChildNodes.Add(draggedNode);
                                        }
                                        if (IsSavedReport)
                                        {
                                            Items reportItems = new Items();
                                            reportItems = GetAxisItemsFromReport();

                                            Items axisItems = this.GetItemsFromMetaTree();

                                            this.ReportItems = GetDraggedKPI(axisItems, reportItems);
                                        }
                                        else
                                        {
                                            this.ReportItems = GetItemsFromMetaTree();
                                        }
                                    }
                                    else
                                    {
                                        AxisElementBuilder measureAxis = FindMeasureAxis();
                                        AxisElementBuilder kpiAxis = FindKPIAxis();

                                        if (measureAxis != null && measureAxis.Axis != this.Axis)
                                        {
                                            throw new Exception(SR.GetString(CultureInfo.CurrentUICulture,"Exception_MeasureAndKPIShouldComeUnderSameAxis"));
                                        }
                                        else
                                        {
                                            if (kpiAxis != null)
                                            {
                                                kpiNodes = FindKpiNode(kpiAxis.MetaTreeNodes.ToList(), null, false);
                                                this.Synchronize(kpiAxis.ReportItems, kpiNodes);

                                                tempNode = FindKpiNode(kpiNodes.ChildNodes.ToList(), draggedNode, true);

                                                if (tempNode == null)
                                                {
                                                    kpiNodes.ChildNodes.Add(draggedNode);
                                                }
                                            }
                                            if (kpiNodes != null)
                                                this.MetaTreeNodes.Add(kpiNodes);
                                            else
                                            {
                                                kpiNodes = new MetaTreeNode(PropertyConstants.KPI, PropertyConstants.KPI, PropertyConstants.KPI);
                                                kpiNodes.NodeType = MetaTreeNodeType.KPI_ROOT;

                                                if (rootNode != null && rootNode.Properties.Count > 0)
                                                {
                                                    Property property = rootNode.Properties.FindByName(PropertyConstants.KPI);
                                                    if (property != null)
                                                    {
                                                        KpiCollection kpiCollection = property.Value as KpiCollection;
                                                        if (kpiCollection != null)
                                                            kpiNodes.Properties.Add(new Property(PropertyConstants.KPI, kpiCollection));
                                                    }
                                                }

                                                kpiNodes.ChildNodes.Add(draggedNode);
                                                this.MetaTreeNodes.Add(kpiNodes);
                                            }
                                            if (IsSavedReport)
                                            {
                                                Items reportItems = new Items();
                                                reportItems = GetAxisItemsFromReport();

                                                Items axisItems = this.GetItemsFromMetaTree();

                                                this.ReportItems = GetDraggedKPI(axisItems, reportItems);
                                            }
                                            else
                                            {
                                                this.ReportItems = this.GetItemsFromMetaTree();
                                            }
                                        }
                                    }
                                }

                                #endregion
                            }
                            else if (draggedNode.NodeType == MetaTreeNodeType.NamedSet)
                            {
                                #region Dropping NamedSet elements

                                AxisElementBuilder namedSetAxis = FindNamedsetAxis();

                                if (namedSetAxis == null)
                                    namedSetAxis = this;

                                if (rootNode.Properties.Count > 0)
                                {
                                    Dimension dimensionObj = rootNode.Properties.FindByName(PropertyConstants.Dimension).Value as Dimension;
                                    NamedSet namedSetObj = (NamedSet)draggedNode.Properties.FindByName(PropertyConstants.NamedSet).Value;

                                    #region Handling the removal of Named Set under the same dimension
                                    //// Check if the named set already exists in the same axis
                                    for (int i = 0; i < namedSetAxis.MetaTreeNodes.Count; i++)
                                    {
                                        MetaTreeNode childNode = namedSetAxis.MetaTreeNodes[i];
                                        //// If NamedSet is in Slicer and Parent Dimension exists
                                        //// then do not remove the child node
                                        if (childNode.NodeType == MetaTreeNodeType.NamedSet &&
                                            childNode.UniqueName == namedSetObj.ParentDimensionName &&
                                            ValidateNamedSetPosition(dimensionObj.Name))
                                        {
                                            namedSetAxis.MetaTreeNodes.Remove(childNode);
                                            break;
                                        }
                                    }
                                    #endregion

                                    if (!ValidateNamedSetPosition(dimensionObj.Name))
                                    {
                                        throw new Exception(SR.GetString(CultureInfo.CurrentUICulture,"Exception_NamedSetCannotBePresentInSlicerIfTheParentDimensionExistsInAnyOtherAxis"));
                                    }

                                    draggedNode = new MetaTreeNode(namedSetObj.Name, namedSetObj.Name, namedSetObj.Description);
                                    draggedNode.UniqueName = namedSetObj.ParentDimensionName;
                                    draggedNode.NodeType = MetaTreeNodeType.NamedSet;

                                    MetaTreeNode metaTreeNode = new MetaTreeNode(namedSetObj.Name, namedSetObj.Name, namedSetObj.Description);
                                    metaTreeNode.UniqueName = namedSetObj.ParentDimensionName;
                                    metaTreeNode.NodeType = MetaTreeNodeType.NamedSet;
                                    Property propertyMember = metaTreeNode.Properties.FindByName(PropertyConstants.NamedSet);
                                    if (propertyMember == null)
                                    {
                                        metaTreeNode.Properties.Add(PropertyConstants.NamedSet, namedSetObj);
                                        metaTreeNode.Properties.Add(PropertyConstants.Dimension, dimensionObj);
                                        draggedNode.Properties.Add(PropertyConstants.NamedSet, namedSetObj);
                                        draggedNode.Properties.Add(PropertyConstants.Dimension, dimensionObj);
                                    }
                                    metaTreeNode.ParentNode = draggedNode;
                                    //metaTreeNode.SetIsChecked(true, false, true);
                                    //metaTreeNode.ForceAcceptChanges();
                                    draggedNode.ChildNodes.Add(metaTreeNode);
                                    this.MetaTreeNodes.Add(draggedNode);
                                    this.ReportItems = GetItemsFromMetaTree();
                                }
                                #endregion
                            }

                            if (!IsSavedReport)
                            {
                                ///Synchronize the element value and excluded element value with ReportItems
                                MetaTreeNodeCollection mtNodeCollection = new MetaTreeNodeCollection(null);
                                foreach (MetaTreeNode metaTreeNode in this.MetaTreeNodes)
                                {
                                    MetaTreeHelper.FillSelectedNodeCollectionVersion3(mtNodeCollection, metaTreeNode);
                                }
                                Items tempItems = this.ReportItems;
                                if (mtNodeCollection.Count > 0)
                                {
                                    QueryBuilderEngineHelper.UpdateItemsFromMetaTree(mtNodeCollection, this.Axis, tempItems);
                                }
                                else
                                {
                                    tempItems.Clear();
                                }
                            }
                            if (this.AutoExecute)
                                this.OlapDataManager.NotifyElementChanged();
                            else
                                this.OlapDataManager.RefreshAxisElementBuilder();
                            #endregion
                        }

                        if (this.DragDropManager != null)
                        {
                            this.DragDropManager.DragDropPopup.IsOpen = false;
                            this.DragDropManager = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        this.DragDropManager.DragDropPopup.IsOpen = false;
                        this.DragDropManager = null;
                        
                        Syncfusion.Windows.Tools.Controls.WindowControl.ShowAlert(ex.Message,SR.GetString(CultureInfo.CurrentUICulture, "Exception_ErrorWhileDragandDrop"), Windows.Tools.Controls.DialogIcon.Error, Windows.Tools.Controls.DialogButton.OK, null, Windows.Tools.Controls.AnimationType.Zoom);
                    }
                }
            }
            this.ReleaseMouseCapture();
        }

        private void AddVirtualKpitoCurrentReport(OlapSilverlight.Reports.Items items)
        {
            this.OlapDataManager.CurrentReport.VirtualKpiElements.Clear();
            foreach (Item item in items)
            {
                if (item.Axis == AxisPosition.Categorical)
                {
                    if (item.ElementValue is VirtualKpiElement)
                    {
                        this.OlapDataManager.CurrentReport.VirtualKpiElements.Add(new Item
                        {
                            ElementValue = item.ElementValue,
                            ExcludedElementValue = item.ExcludedElementValue,
                            IsFilterOrSortOn = item.IsFilterOrSortOn,
                            ItemName = item.ItemName
                        });
                    }
                }
                if (item.Axis == AxisPosition.Series)
                {
                    if (item.ElementValue is VirtualKpiElement)
                    {
                        this.OlapDataManager.CurrentReport.VirtualKpiElements.Add(new Item
                        {
                            ElementValue = item.ElementValue,
                            ExcludedElementValue = item.ExcludedElementValue,
                            IsFilterOrSortOn = item.IsFilterOrSortOn,
                            ItemName = item.ItemName
                        });
                    }
                }
                if (item.Axis == AxisPosition.Slicer)
                {
                    if (item.ElementValue is VirtualKpiElement)
                    {
                        this.OlapDataManager.CurrentReport.VirtualKpiElements.Add(new Item
                        {
                            ElementValue = item.ElementValue,
                            ExcludedElementValue = item.ExcludedElementValue,
                            IsFilterOrSortOn = item.IsFilterOrSortOn,
                            ItemName = item.ItemName
                        });
                    }
                }
            }
        }

        private void UpdateMetaTreeNodes(MetaTreeNode draggedNode, MetaTreeNode rootNode, ref MetaTreeNode measureNodes, ref MetaTreeNode tempNode, AxisElementBuilder measureAxis)
        {
            List<MetaTreeNode> calcMeasureNodes = null;
            if (measureAxis != null)
            {
                if (draggedNode.NodeType == MetaTreeNodeType.CalculatedMember)
                {
                    measureNodes = this.FindMeasureNode(measureAxis.MetaTreeNodes.ToList(), null, false);
                    if (measureNodes != null)
                        this.Synchronize(measureAxis.ReportItems, measureNodes);
                    calcMeasureNodes = measureAxis.MetaTreeNodes.Where(j => j.NodeType == MetaTreeNodeType.CalculatedMember && j.Properties[0]!=null && j.Properties[0].Value is CalculatedMember && (j.Properties[0].Value as CalculatedMember).Type == TypeOfMember.Measure).ToList<MetaTreeNode>();
                    if (calcMeasureNodes != null)
                    {
                        foreach (var item in calcMeasureNodes)
                        {
                            this.Synchronize(measureAxis.ReportItems, item);
                        }
                    }
                }
                else
                {
                    measureNodes = this.FindMeasureNode(measureAxis.MetaTreeNodes.ToList(), null, false);
                    if (measureNodes != null)
                    {
                        this.Synchronize(measureAxis.ReportItems, measureNodes);
                        tempNode = FindMeasureNode(measureNodes.ChildNodes.ToList(), draggedNode, true);
                        if (tempNode == null)
                        {
                            measureNodes.ChildNodes.Add(draggedNode);
                        }
                    }

                    calcMeasureNodes = measureAxis.MetaTreeNodes.Where(j => j.NodeType == MetaTreeNodeType.CalculatedMember && j.Properties[0] != null && j.Properties[0].Value is CalculatedMember && (j.Properties[0].Value as CalculatedMember).Type == TypeOfMember.Measure).ToList<MetaTreeNode>();
                    if (calcMeasureNodes != null)
                    {
                        foreach (var item in calcMeasureNodes)
                        {
                            this.Synchronize(measureAxis.ReportItems, item);
                        }
                    }
                }        

            }

            if (draggedNode.NodeType == MetaTreeNodeType.CalculatedMember)
            {
                if (measureNodes != null)
                {
                    this.MetaTreeNodes.Add(measureNodes);
                }
                if (calcMeasureNodes != null)
                {
                    foreach (MetaTreeNode item in calcMeasureNodes)
                    {
                        if (item.Properties[0] != null && item.Properties[0].Value is CalculatedMember)
                        {
                            CalculatedMember memberGoingToAdd = item.Properties[0].Value as CalculatedMember;
                            if (!(this.MetaTreeNodes.Any(i => i.UniqueName == memberGoingToAdd.UniqueName)))
                            {
                                this.MetaTreeNodes.Add(item);
                            }
                        }
                    }
                }

                if (draggedNode.Properties[0] != null && draggedNode.Properties[0].Value is CalculatedMember)
                {
                    CalculatedMember memberGoingToAdd = draggedNode.Properties[0].Value as CalculatedMember;
                    if (!(this.MetaTreeNodes.Any(i => i.UniqueName == memberGoingToAdd.UniqueName)))
                    {
                        this.MetaTreeNodes.Add(draggedNode);
                    }
                }
            }
            else
            {
                if (calcMeasureNodes != null)
                {
                    foreach (MetaTreeNode item in calcMeasureNodes)
                    {
                        if (item.Properties[0] != null && item.Properties[0].Value is CalculatedMember)
                        {
                            CalculatedMember memberGoingToAdd = item.Properties[0].Value as CalculatedMember;
                            if (!(this.MetaTreeNodes.Any(i => i.UniqueName == memberGoingToAdd.UniqueName)))
                            {
                                this.MetaTreeNodes.Add(item);
                            }
                        }
                    }
                }

                if (measureNodes != null)
                {
                    this.MetaTreeNodes.Add(measureNodes);
                }
                else
                {
                    measureNodes = new MetaTreeNode(PropertyConstants.MeasrueNodeName, PropertyConstants.MeasrueNodeName, PropertyConstants.MeasrueNodeName);
                    measureNodes.NodeType = MetaTreeNodeType.MeasureGroup;

                    if (rootNode.Properties.Count > 0)
                    {
                        Property property = rootNode.Properties.FindByName(PropertyConstants.MeasureGroupName);
                        if (property != null)
                        {
                            MeasureCollection measureCollection = property.Value as MeasureCollection;
                            if (measureCollection != null)
                                measureNodes.Properties.Add(new Property(PropertyConstants.MeasureGroupName, measureCollection));
                        }
                    }
                    measureNodes.ChildNodes.Add(draggedNode);
                    this.MetaTreeNodes.Add(measureNodes);
                }
            }
        }

        /// <summary>
        /// Checks for existence.
        /// </summary>
        /// <param name="draggedNode">The dragged node.</param>
        /// <returns></returns>
        private bool CheckForExistence(MetaTreeNode draggedNode)
        {
            //Item draggedNodeItem = QueryBuilderEngineHelper.GetElementItemFromMetaTree(draggedNode, this.Axis);
            if (this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.Dimension || this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.Hierarchy || this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.Level)
            {
                MetaTreeNode draggedNodeItem = new MetaTreeNode();
                if (this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.Dimension)
                {
                    draggedNodeItem = this.GetDimensionNode(this.DragDropManager.SelectedNode.Properties.FindByName(PropertyConstants.Dimension).Value as Dimension, this.DragDropManager.SelectedNode.Caption);
                }
                if (this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.Hierarchy)
                {
                    draggedNodeItem = this.GetDefaultLevelMembersNode(this.DragDropManager.SelectedNode.Properties[0].Value as Hierarchy, this.DragDropManager.SelectedNode.GetRootNode().Name, this.DragDropManager.SelectedNode.Caption);
                }
                if (this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.Level)
                {
                    Dimension dimensionObj = this.DragDropManager.SelectedNode.GetRootNode().Properties.FindByName(PropertyConstants.Dimension).Value as Dimension;
                    //// This line are commented out since each level node takes its own parent node as a member and modified the alternate way
                    //MetaTreeNode hierarchyNode = this.GetHierarchyNode(dimensionObj.Hierarchies, dimensionObj.DefaultHierarchyName, dimensionObj.Name);
                    MetaTreeNode hierarchyNode = this.GetHierarchyNode(dimensionObj.Hierarchies, this.DragDropManager.SelectedNode.ParentNode.UniqueName, dimensionObj.Name, this.DragDropManager.SelectedNode.Caption);
                    Hierarchy hierarchyobj = hierarchyNode.Properties.FindByName(PropertyConstants.Hierarchy).Value as Hierarchy;
                    draggedNodeItem = this.GetMemberNode(hierarchyobj, this.DragDropManager.SelectedNode.Properties[0].Value as Level, dimensionObj.Name, this.DragDropManager.SelectedNode.Caption);
                }
                foreach (MetaTreeNode node in this.MetaTreeNodes)
                {
                    if (node.Name.ToUpper() == draggedNodeItem.Name.ToUpper() && (node.Properties[1].Value as Hierarchy).Name.ToUpper() == (draggedNodeItem.Properties[2].Value as Hierarchy).Name.ToUpper())// &&
                            //(node.Properties[2].Value as Level).Name == (draggedNodeItem.Properties[1].Value as Level).Name)
                        return true;
                }
            }
            else if (this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.Measure)
            {
                MetaTreeNode mtNode = this.MetaTreeNodes.Select(m => m).Where(m => m.NodeType == MetaTreeNodeType.MeasureGroup).FirstOrDefault();
                if (mtNode != null && mtNode.ChildNodes.Where(n => n.UniqueName == this.DragDropManager.SelectedNode.UniqueName).Any())
                    return true;
            }
            else if (this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.CalculatedMember)
            {
                return this.MetaTreeNodes.Select(m => m).Where(m => m.UniqueName == this.DragDropManager.SelectedNode.UniqueName).Any();
            }
            else if (this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.VirtualKPIMember || this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.VirtualKPI_Value || this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.VirtualKPI_Goal || this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.VirtualKPI_Status || this.DragDropManager.SelectedNode.NodeType == MetaTreeNodeType.VirtualKPI_Trend)
            {
                MetaTreeNode mtnode = this.MetaTreeNodes.Select(m => m).Where(m => m.UniqueName == this.DragDropManager.SelectedNode.UniqueName).FirstOrDefault();
                if (mtnode != null && mtnode.UniqueName == this.DragDropManager.SelectedNode.UniqueName)
                    return true;
            }
            else
            {
                foreach (MetaTreeNode node in this.MetaTreeNodes)
                {
                    if (node.UniqueName == draggedNode.UniqueName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Resets the current pages.
        /// </summary>
        private void ResetCurrentPages()
        {
            this.OlapDataManager.CurrentReport.PagerOptions.SeriesCurrentPage = 1;
            this.OlapDataManager.CurrentReport.PagerOptions.CategorialCurrentPage = 1;
        }
        #endregion

        #region Events

        void OlapDataManager_AxisElementChanged(object sender, AxisElementChangedEventArgs e)
        {
            if (e == null || this.Axis == e.NewPosition)
            {
                Refresh();
            }
        }

        void OlapDataManager_CubeSchemaChanged(object sender, CubeSchemaChangedEventArgs e)
        {
            this.Refresh();
        }

        internal void OlapDataManager_ChildMembersObtained(object sender, ChildMembersObtaindedEventArgs e)
        {
            if (e.ChildMembers.Count <= 0)
                return;
            MemberCollection childMemberCollection = e.ChildMembers;
            if (true)
            {
                MetaTreeNode childNode = new MetaTreeNode();
                foreach (Member item in e.ChildMembers)
                {
                    MetaTreeHelper.FillMetaTreeNode(childNode, item, true);
                }
            }
        }

        
        
        internal void OlapDataManager_LevelElementObtained(object sender, LevelMembersObtainedEventArgs e)
        {
            if (e.LevelMembers.Count == 0)
                return;
            MemberCollection leveMembers = e.LevelMembers;
            Item reportItem = null; MetaTreeNode dimensionNode = null;
            string dimnesionName = leveMembers[0].UniqueName.Substring(1, leveMembers[0].UniqueName.IndexOf("]") - 1);
            string hierarchyName = leveMembers[0].UniqueName.Split('.')[1].Replace("[", "").Replace("]", "");
            if (this.OlapDataManager.ProviderName == Providers.ActivePivot || this.OlapDataManager.ProviderName == Providers.Mondrian)
            {
                if (dimnesionName.Contains('.'))
                    dimnesionName = dimnesionName.Substring(0, dimnesionName.IndexOf('.'));
                AxisElementBuilder axis = this.FinDimensionAxis(dimnesionName);
                dimensionNode = this.FindDimensionNode(axis.MetaTreeNodes, dimnesionName);
                reportItem = this.FindDimensionItem(axis, dimensionNode);
            }
            else
            {
                AxisElementBuilder axis = this.FinDimensionAxis(dimnesionName, hierarchyName);
                if (axis != null)
                {
                    dimensionNode = this.FindDimensionNode(axis.MetaTreeNodes, dimnesionName, hierarchyName);
                    reportItem = this.FindDimensionItem(axis, dimensionNode);
                }
            }
            DimensionElement exculdedDimension = null;
            if (reportItem != null)
            {
                exculdedDimension = reportItem.ExcludedElementValue as DimensionElement;
            }
            if (dimensionNode != null && dimensionNode.ChildNodes.Count < 1)
            {
                Level level = null;
                string levelName = leveMembers[0].UniqueName.Remove(leveMembers[0].UniqueName.LastIndexOf("."));
                if (levelName.Split('.').Count() <= 2)
                {
                    level = this.OlapDataManager.CurrentCubeSchema.GetLevelByUniqueName(leveMembers[0].LevelUniqueName);
                }
                else
                {
                    level = this.OlapDataManager.CurrentCubeSchema.GetLevelByUniqueName(levelName);
                }

                if (level != null)
                {
                    level.Members = leveMembers;
                }

                foreach (Member lvlMemeber in leveMembers)
                {
                    MetaTreeNode childNode = new MetaTreeNode();
                    MetaTreeHelper.FillMetaTreeNode(childNode, lvlMemeber, true, true);
                   
                    LevelElementCollection _levelElementCollection = ((reportItem.ElementValue) as DimensionElement).Hierarchy.LevelElements;
                    if (_levelElementCollection.Count > 0)
                    {
                        foreach (var _levelElement in _levelElementCollection)
                        {
                            if (_levelElement.MemberElements.Count > 0)
                            {
                                UpdateIncludeNodes(_levelElement, childNode);
                            }
                        }
                    }
                    if (exculdedDimension != null)
                    {
                        if (exculdedDimension.Hierarchy.LevelElements.Count > 0)
                        {
                            foreach (var _levelElement in exculdedDimension.Hierarchy.LevelElements)
                            {
                                if (_levelElement.MemberElements.Count > 0)
                                {
                                    UpdateExcludeNodes(_levelElement, childNode);
                                }
                            }
                        }
                    }

                    dimensionNode.ChildNodes.Add(childNode);
                }
            }
        }

        void AxisElementBuilder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectedItem != null)
            {
                if (this.DragDropManager != null)
                {
                    this.OnAxisMouseLeftButtonUp(sender, null);
                    return;
                }
                this.DragDropManager = new DragDropManager();
                this.DragDropManager.SelectedNode = (this.SelectedItem as MetaTreeNode).Clone() as MetaTreeNode;
                this.DragDropManager.Source = this;
                this.DragDropManager.SourceAxis = this.Axis;

                Popup dragDropPopup = new Popup();
                ContentControl popupContent = new ContentControl();
                popupContent.IsHitTestVisible = false;
                DataTemplate template = this.ItemTemplate as DataTemplate;
                template.LoadContent();
                popupContent.ContentTemplate = template;
                popupContent.Content = this.DragDropManager.SelectedNode;
                popupContent.Opacity = 0.5;
                dragDropPopup.Child = popupContent;
                dragDropPopup.Child.Visibility = System.Windows.Visibility.Collapsed;
                dragDropPopup.IsOpen = true;
                dragDropPopup.Opacity = 0.5;
                this.DragDropManager.DragDropPopup = dragDropPopup;
                this.CaptureMouse();
            }
            //this.SelectedIndex = -1;
        }

        #endregion
    }
    #region SplitButtonDisplayMode
    /// <summary>
    /// SplitButtonDisplayModes of OlapClient
    /// </summary>
    public enum SplitButtonDisplayMode
    {
        /// <summary>
        /// Display Cation without Hierarchy
        /// </summary>
        WithoutHierarchyCaption,

        /// <summary>
        /// Display Cation with Hierarchy
        /// </summary>
        WithHierarchyCaption
    }
    #endregion
}
