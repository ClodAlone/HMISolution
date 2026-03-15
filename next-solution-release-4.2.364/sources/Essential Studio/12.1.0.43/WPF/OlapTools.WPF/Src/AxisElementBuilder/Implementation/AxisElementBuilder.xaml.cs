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
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using Syncfusion.Olap.Data;
    using Syncfusion.Olap.Manager;
    using Syncfusion.Olap.MDXQueryBuilder;
    using Syncfusion.Olap.Reports;
    using System.Collections.Generic;

    /// <summary>
    /// Interaction logic for AxisElementBuilder.xaml
    /// </summary>
    public partial class AxisElementBuilder : UserControl
    {
        #region Variables

        private IOlapDataManager _OlapDataManager;

        private int itemSeletedIndex = -1;

        public static readonly DependencyProperty CurrentThemeProperty =
            DependencyProperty.Register("CurrentTheme", typeof(SolidColorBrush), typeof(AxisElementBuilder), new UIPropertyMetadata(Brushes.Blue));

        public static readonly DependencyProperty AutoExecuteProperty =
            DependencyProperty.Register("AutoExecute", typeof(bool), typeof(AxisElementBuilder), new UIPropertyMetadata(true));

        #endregion

        #region Costructor

        public AxisElementBuilder()
        {
            // Initializing component
            InitializeComponent();
            this.MetaTreeNodes = new ObservableCollection<MetaTreeNode>();
            this.listBoxAxisElements.ItemsSource = this.MetaTreeNodes;
            this.listBoxAxisElements.ContextMenuOpening += new ContextMenuEventHandler(listBoxAxisElements_ContextMenuOpening);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets AxisElementBuilder axis.
        /// </summary>
        /// <value>The axis.</value>
        public AxisPosition Axis { get; set; }

        /// <summary>
        /// Gets or sets the current theme.
        /// </summary>
        /// <value>The current theme.</value>
        public SolidColorBrush CurrentTheme
        {
            get { return (SolidColorBrush)GetValue(CurrentThemeProperty); }
            set { SetValue(CurrentThemeProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the elements will automatically execute the query.
        /// </summary>
        /// <value><c>true</c> if [auto execute]; otherwise, <c>false</c>.</value>
        public bool AutoExecute
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
        /// Gets the Selected element form the metatreenode.
        /// </summary>
        /// <value>The get element items.</value>
        public Items GetElementItems
        {
            get
            {
                // Filling the Selected Collection
                MetaTreeNodeCollection mtNodeCollection = new MetaTreeNodeCollection(null);
                foreach (MetaTreeNode metaTreeNode in this.MetaTreeNodes)
                {
                    MetaTreeHelper.FillSelectedNodeCollectionVersion3(mtNodeCollection, metaTreeNode);
                }

                // Converting the MetaTree to Element Items
                QueryBuilderEngineHelper.SetProviderName(this.OlapDataManager.DataProvider.ProviderName);
                return QueryBuilderEngineHelper.GetElementItemsFromMetaTree(mtNodeCollection, this.Axis);
            }
        }

        /// <summary>
        /// Gets or sets the AxisElementBuilder elements MetaTreeNodesq
        /// </summary>
        /// <value>The meta tree nodes.</value>
        public ObservableCollection<MetaTreeNode> MetaTreeNodes { get; private set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public IOlapDataManager OlapDataManager
        {
            get
            {
                return this._OlapDataManager;
            }

            set
            {
                this._OlapDataManager = value;
                if (this._OlapDataManager != null)
                {
                    string aeBuilderName = GetAxisBuilderElementName(this.Axis);
                    //// Adding the current object to the mode properties for future use
                    this.OlapDataManager.Properties.Add(aeBuilderName, this);

                    this.MetaTreeNodes.Clear();
                    this.OlapDataManager.ReportChanged += new ReportChangedEventHandler(OlapDataManager_ReportChanged);
                    this.OlapDataManager.AxisElementChanged += new AxisElementChangedEventHandler(OlapDataManager_AxisElementChanged);
                    this.OlapDataManager.AxisElementModified += new AxisElementModifiedEventHandler(OlapDataManager_AxisElementModified);
                    (this.OlapDataManager as OlapDataManager).ActiveReportChanged += new ActiveReportChangedEventHandler(AxisElementBuilder_ActiveReportChanged);
                }
            }
        }



        /// <summary>
        /// Gets or sets the AxisElementBuilderStyle.
        /// </summary>
        /// <value>The AxisElementBuilderStyle.</value>
        public static DependencyProperty AxisElementBuilderStyleProperty =
        DependencyProperty.Register("AxisElementBuilderStyle", typeof(Style), typeof(AxisElementBuilder), new UIPropertyMetadata());

        public Style AxisElementBuilderStyle
        {
            get
            {
                return (Style)GetValue(AxisElementBuilderStyleProperty);
            }

            set
            {
                SetValue(AxisElementBuilderStyleProperty, value);
                this.listBoxAxisElements.Style = AxisElementBuilderStyle;
            }
        }

        #endregion

        #region Customized Node Generation based on cube objects


        /*
         *  ==================================================================================================
         * 
         *  Node generation already exist's in MetaTreeNode helper, but this scoped region methods generated
         *  customized node's which are required for the member editor
         * 
         *  ===================================================================================================
         */

        private MetaTreeNode GetDimensionNode(Dimension dimensionObj)
        {
            return this.GetHierarchyNode(dimensionObj.Hierarchies, dimensionObj.DefaultHierarchyName, dimensionObj.Name, dimensionObj.Caption);
        }

        private MetaTreeNode GetHierarchyNode(HierarchyCollection hierarchyCollection, string defalutHierarchyName, string dimensionName, string dimensionCaption)
        {
            if (hierarchyCollection.Count > 0)
            {
                Hierarchy hierarchyObj = null;
                foreach (Hierarchy __hierarchyObj in hierarchyCollection)
                {
                    if (__hierarchyObj.UniqueName == defalutHierarchyName)
                    {
                        hierarchyObj = __hierarchyObj;
                        break;
                    }
                }
                if (hierarchyObj == null)
                {
                    hierarchyObj = hierarchyCollection[0];
                }

                return this.GetDefaultLevelMembersNode(hierarchyObj, dimensionName, dimensionCaption);
            }
            return null;
        }

        private MetaTreeNode GetDefaultLevelMembersNode(Hierarchy hierarchyObj, string dimensionName, string dimensionCaption)
        {
            if (hierarchyObj.Levels.Count > 0)
            {
                Level levelObj = null;
                foreach (Level __levelObj in hierarchyObj.Levels)
                {
                    if (__levelObj.UniqueName == hierarchyObj.DefaultLevelUniqueName)
                    {
                        levelObj = __levelObj;
                        break;
                    }
                }

                if (levelObj == null)
                {
                    levelObj = hierarchyObj.Levels[0];
                }

                return this.GetMemberNode(hierarchyObj, levelObj, dimensionName, dimensionCaption);
            }

            return null;
        }

        private MetaTreeNode GetMemberNode(Hierarchy hierarchyObj, Level levelObj, string dimensionName, string dimensionCaption)
        {
            if (levelObj != null)
            {
                MetaTreeNode metaTreeNode = new MetaTreeNode();
                metaTreeNode.Caption = dimensionCaption;
                metaTreeNode.Name = dimensionName;
                metaTreeNode.UniqueName = hierarchyObj.ParentDimension.UniqueName;
                metaTreeNode.NodeType = MetaTreeNodeType.Dimension;
                Dimension dimensionObj = new Dimension();
                dimensionObj = hierarchyObj.ParentDimension;
                metaTreeNode.Properties.Add(new Property(PropertyConstants.Dimension, dimensionObj));
                metaTreeNode.Properties.Add(new Property(PropertyConstants.Level, levelObj));
                metaTreeNode.Properties.Add(new Property(PropertyConstants.Hierarchy, hierarchyObj));
                //metaTreeNode.ForceAcceptChanges();
                foreach (Member memberObj in levelObj.Members)
                {
                    MetaTreeNode newMemberNode = new MetaTreeNode();
                    MetaTreeHelper.FillMetaTreeNode(newMemberNode, memberObj, true);
                    newMemberNode.ParentNode = metaTreeNode;
                    metaTreeNode.ChildNodes.Add(newMemberNode);
                }
                metaTreeNode.IsSelected = true;
                return metaTreeNode;
            }

            return null;
        }


        #endregion

        #region Synchronization

        private void SynchronizeItems(Items items)
        {
            // Filling the Selected Collection
            MetaTreeNodeCollection mtNodeCollection = new MetaTreeNodeCollection(null);
            foreach (MetaTreeNode metaTreeNode in this.MetaTreeNodes)
            {
                MetaTreeHelper.FillSelectedNodeCollectionVersion3(mtNodeCollection, metaTreeNode);
            }
            //// Synchronize the items count
            for (int i = 0; i < items.Count; i++)
            {
                bool isExist = false;
                foreach (MetaTreeNode metaTreeNode in mtNodeCollection)
                {
                    if (items[i].ElementValue is MeasureElements)
                    {
                        if (metaTreeNode.NodeType == MetaTreeNodeType.MeasureGroup)
                            isExist = true;
                    }
                    else if (items[i].ElementValue is KpiElements)
                    {
                        if (metaTreeNode.NodeType == MetaTreeNodeType.KPI_ROOT)
                            isExist = true;
                    }
                    else if (items[i].ElementValue is NamedSetElement)
                    {
                        if (metaTreeNode.NodeType == MetaTreeNodeType.NamedSet)
                            isExist = true;
                    }
                    else if (items[i].ElementValue is CalculatedMember)
                    {
                        if (metaTreeNode.NodeType == MetaTreeNodeType.CalculatedMember && metaTreeNode.UniqueName == (items[i].ElementValue as CalculatedMember).UniqueName)
                            isExist = true;
                    }
                    else if (items[i].ElementValue is VirtualKpiElement)
                    {
                        if (metaTreeNode.NodeType == MetaTreeNodeType.VirtualKPIMember)
                            isExist = true;
                    }
                    else
                    {
                        if (metaTreeNode.Caption.ToUpper() == items[i].ElementValue.Name.ToUpper())
                        {
                            {
                                if (((Property)((PropertyCollection)metaTreeNode.Properties)[2]).Name == "HIERARCHY")
                                {
                                    if (((DimensionElement)items[i].ElementValue).HierarchyName == ((Hierarchy)((Property)((PropertyCollection)metaTreeNode.Properties)[2]).Value).Name)
                                    {
                                        if (((LevelElementCollection)((HierarchyElement)((DimensionElement)items[i].ElementValue).Hierarchy).LevelElements)[0].Name == ((Level)((Property)((PropertyCollection)metaTreeNode.Properties)[1]).Value).Name)
                                        {
                                            isExist = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (((DimensionElement)items[i].ElementValue).HierarchyName.ToUpper() == ((Hierarchy)((Property)((PropertyCollection)metaTreeNode.Properties)[1]).Value).Name.ToUpper())
                                    {
                                        if (((LevelElementCollection)((HierarchyElement)((DimensionElement)items[i].ElementValue).Hierarchy).LevelElements)[0].Name.ToUpper() == ((Level)((Property)((PropertyCollection)metaTreeNode.Properties)[2]).Value).Name.ToUpper())
                                        {
                                            isExist = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (!isExist)
                {
                    items.RemoveAt(i);
                    i--;
                }
            }

            if (mtNodeCollection.Count > 0)
            {
                //// Synchronize the element selection
                QueryBuilderEngineHelper.SetProviderName(this.OlapDataManager.DataProvider.ProviderName);
                QueryBuilderEngineHelper.UpdateItemsFromMetaTree(mtNodeCollection, this.Axis, items);
            }
            else
            {
                items.Clear();
            }
        }

        /// <summary>
        /// Unselects the nodes to the supplies MetaTreeNode base on the level element supplied
        /// </summary>
        /// <param name="excludedLevelElement">Excluding level element</param>
        /// <param name="metaTreeNodeMembers">MetaTreeNode set</param>
        /// <returns></returns>
        private bool UpdateExcludeNodes(LevelElement excludedLevelElement, MetaTreeNode metaTreeNodeMembers)
        {
            bool childUpdated = false;
            Property property = metaTreeNodeMembers.Properties.FindByName(PropertyConstants.Member);
            if (property != null && property.Value is Member)
            {
                Member memberObj = property.Value as Member;
                if (excludedLevelElement.UniqueName == memberObj.LevelUniqueName || excludedLevelElement.UniqueName.Split('.')[0] + ".[" + excludedLevelElement.Name + "]" == memberObj.LevelUniqueName)
                {
                    foreach (MemberElement memberElement in excludedLevelElement.MemberElements)
                    {
                        string[] memberObjNames = memberObj.UniqueName.Split('.');
                        if (memberObj.CustomUniqueName == memberElement.UniqueName ||
                            memberObj.UniqueName == memberElement.UniqueName || memberObjNames[0] + "." + memberObj.LevelUniqueName + "." + memberObjNames[memberObjNames.Length - 1] == memberElement.UniqueName)
                        {
                            metaTreeNodeMembers.IsSelected = false;
                            metaTreeNodeMembers.AcceptIsSelectedChanges(true);
                            childUpdated = true;
                        }
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

            return childUpdated;
        }

        #endregion

        #region Load Elements

        private void LoadElements(AxisElementBuilder aeBuilder, Items items)
        {
            CubeSchema cubeSchema = aeBuilder.OlapDataManager.CurrentCubeSchema;
            if (cubeSchema == null)
            {
                cubeSchema = aeBuilder.OlapDataManager.DataProvider.GetCubeSchema(aeBuilder.OlapDataManager.CurrentCubeName);
            }

            aeBuilder.MetaTreeNodes.Clear();
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

                    //// Adding selected measure elements
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

                    //// To identify the drag source adding a identifier property
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
                    if (dimensionElement.Name != string.Empty)
                    {
                        //// Getting the dimension by element unique name
                        Dimension dimensionObj = cubeSchema.GetDimensionByUniqueName(dimensionElement.UniqueName);
                        //string dimensionName = dimensionElement.Name;
                        //// Creating dimension node
                        MetaTreeNode metaTreeNode = new MetaTreeNode(dimensionObj.Name, dimensionObj.Caption, dimensionObj.Description);
                        if (dimensionObj != null)
                        {
                            metaTreeNode.Properties.Add(new Property(PropertyConstants.Dimension, dimensionObj));
                        }
                        metaTreeNode.UniqueName = dimensionObj.UniqueName;
                        metaTreeNode.NodeType = MetaTreeNodeType.Dimension;
                        //// Extracting the level element
                        LevelElement levelElement = dimensionElement.Hierarchy.LevelElements[0];
                        foreach (Hierarchy hierarchyObj in dimensionObj.Hierarchies)
                        {
                            if (hierarchyObj.UniqueName.ToUpper() == dimensionElement.Hierarchy.UniqueName.ToUpper() || (dimensionObj.UniqueName + ".[" + hierarchyObj.Name + "]").ToUpper() == dimensionElement.Hierarchy.UniqueName.ToUpper())
                            {
                                //// Adding the hierarchy object to the MetaTreeNode properties
                                metaTreeNode.Properties.Add(new Property(PropertyConstants.Hierarchy, hierarchyObj));
                                foreach (Level levelObj in hierarchyObj.Levels)
                                {
                                    if (levelObj.UniqueName.ToUpper() == levelElement.UniqueName.ToUpper() || (dimensionObj.UniqueName + ".[" + hierarchyObj.Name + "].[" + levelObj.Name + "]").ToUpper() == levelElement.UniqueName.ToUpper() || levelObj.ParentHierarchy.DefaultMemberUniqueName.ToUpper() == levelElement.UniqueName.ToUpper())
                                    {
                                        //// Adding the level to the metatreenode properties
                                        metaTreeNode.Properties.Add(new Property(PropertyConstants.Level, levelObj));
                                        foreach (Member memberObj in levelObj.Members)
                                        {
                                            MetaTreeNode metaTreeNodeMembers = new MetaTreeNode();
                                            if (excludedDimensionElement != null)
                                            {
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
                                            metaTreeNode.ChildNodes.Add(metaTreeNodeMembers);
                                        }
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        //// To identify the drag source adding a identifier property
                        metaTreeNode.Properties.Add(new Property(PropertyConstants.AxisElements, this));
                        this.MetaTreeNodes.Add(metaTreeNode);
                    }

                    #endregion
                }
                else if (item.ElementValue is KpiElements)
                {
                    ////TODO
                    KpiElements kpiElements = item.ElementValue as KpiElements;

                    MetaTreeNode metaTreeNodekpis = new MetaTreeNode(PropertyConstants.KPI, PropertyConstants.KPI, PropertyConstants.KPI);
                    metaTreeNodekpis.UniqueName = PropertyConstants.MeasrueNodeName;
                    MetaTreeHelper.FillMetaTreeNode(kpiElements, metaTreeNodekpis, cubeSchema.Kpis, true);
                    //// To identify the drag source adding a identifier property
                    metaTreeNodekpis.Properties.Add(new Property(PropertyConstants.AxisElements, null));
                    //// Adding to the MetaTreeNode collection
                    this.MetaTreeNodes.Add(metaTreeNodekpis);
                }
                else if (item.ElementValue is VirtualKpiElement)
                {
                    ////TODO
                    VirtualKpiElement virtualKpiElement = item.ElementValue as VirtualKpiElement;

                    MetaTreeNode metaTreeNodekpis = new MetaTreeNode(virtualKpiElement.Name, virtualKpiElement.Name, virtualKpiElement.Name);
                    metaTreeNodekpis.UniqueName = virtualKpiElement.UniqueName;
                    metaTreeNodekpis.Name = virtualKpiElement.Name;
                    metaTreeNodekpis.NodeType = MetaTreeNodeType.VirtualKPIMember;
                    if ((this.OlapDataManager as OlapDataManager).ActiveReport != null && (this.OlapDataManager as OlapDataManager).UseSharedDataManager)
                        MetaTreeHelper.FillMetaTreeNode(virtualKpiElement, metaTreeNodekpis, (this.OlapDataManager as OlapDataManager).ActiveReport.VirtualKpiElements, true);
                    else
                        MetaTreeHelper.FillMetaTreeNode(virtualKpiElement, metaTreeNodekpis, this.OlapDataManager.CurrentReport.VirtualKpiElements, true);
                  //  MetaTreeHelper.FillMetaTreeNode(metaTreeNodekpis, true, this.OlapDataManager.CurrentReport.VirtualKpiElements);
                    //// To identify the drag source adding a identifier property
                    metaTreeNodekpis.Properties.Add(new Property(PropertyConstants.VirtualKpiNodeName,virtualKpiElement));
                    metaTreeNodekpis.Properties.Add(new Property(PropertyConstants.AxisElements, null));
                    //// Adding to the MetaTreeNode collection
                    this.MetaTreeNodes.Add(metaTreeNodekpis);
                }
                else if (item.ElementValue is NamedSetElement)
                {
                    NamedSetElement namedSetElement = item.ElementValue as NamedSetElement;
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
                        metaTreeNodeNamedSet.Properties.Add(new Property(PropertyConstants.AxisElements, this));
                        //metaTreeNodeNamedSet.IsSelected = null;
                        //metaTreeNodeNamedSet.__IsSelected = null;
                        metaTreeNodeNamedSet.NodeCheckedType = MetaTreeNodeCheckedType.CurrentChecked;
                    }
                    //MetaTreeHelper.FillMetaTreeNode(metaTreeNodeNamedSet, cubeSchema.Kpis, false, true);
                    //// Adding to the MetaTreeNode collection
                    this.MetaTreeNodes.Add(metaTreeNodeNamedSet);
                }
                else if (item.ElementValue is CalculatedMember)
                {
                    #region Load Calculated Members
                    CalculatedMember calcMember = item.ElementValue as CalculatedMember;
                    MetaTreeNode metaTreeNodeCalcMember = new MetaTreeNode(calcMember.Name, calcMember.Name, calcMember.Name);
                    metaTreeNodeCalcMember.UniqueName = calcMember.UniqueName;
                    metaTreeNodeCalcMember.NodeType = MetaTreeNodeType.CalculatedMember;
                    metaTreeNodeCalcMember.Properties.Add(new Property(PropertyConstants.CalculatedMemberNodeName, calcMember));
                    metaTreeNodeCalcMember.Properties.Add(new Property(PropertyConstants.AxisElements, null));
                    this.MetaTreeNodes.Add(metaTreeNodeCalcMember);
                    #endregion
                }
            }
        }

        #endregion

        #region List Interaction

        void listBoxAxisElements_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (!((sender as ListBox).Items.Count > 0))
            {
                e.Handled = true;
            }
            else if ((sender as ListBox).Items.Count > 0 && (sender as ListBox).SelectedIndex < 0)
            {
                e.Handled = true;
            }
        }

        private void MenuItem_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (this.MetaTreeNodes.Count > 0 && this.itemSeletedIndex >= 0)
            {
                MetaTreeNode mtNode = this.MetaTreeNodes[itemSeletedIndex];
                if (mtNode != null)
                {
                    //// Clearing the AxisElement info, if exist
                    Property property = mtNode.Properties.FindByName(PropertyConstants.AxisElements);

                    if (property != null)
                        mtNode.Properties.Remove(property);
                    //// Removing the node form current nodes collection
                    this.MetaTreeNodes.Remove(mtNode);
                    //// Triggering to refresh the controls
                    this.RefreshOlapDataManagerElementItems(this, true);
                }
            }
        }

        private void ListBoxBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try
                {
                    if (sender is ListBox)
                    {
                        ListBox lstView = (ListBox)sender;
                        if (lstView.SelectedItem != null)
                        {
                            DragDropEffects allowedEffects = DragDropEffects.Move;
                            DragDrop.DoDragDrop(this, lstView.SelectedItem, allowedEffects);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception Message");
                }
            }

            if (this.listBoxAxisElements.SelectedIndex >= 0)
            {
                itemSeletedIndex = this.listBoxAxisElements.SelectedIndex;
                this.listBoxAxisElements.SelectedIndex = -1;
            }
        }

        private void ListBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(MetaTreeNode)))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        #endregion

        #region List Drop

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.GetDataPresent(typeof(MetaTreeNode)))
                {
                    MetaTreeNode clipBoardNode = (MetaTreeNode)e.Data.GetData(typeof(MetaTreeNode));
                    Property axisElementProperty = clipBoardNode.Properties.FindByName(PropertyConstants.AxisElements);
                    //// if drag source is cube dimension browser
                    if (axisElementProperty == null)
                    {
                        //// Marking to show all the objects are selected initially
                        clipBoardNode.IsSelected = true;
                        clipBoardNode.AcceptIsSelectedChanges(true);

                        #region Drag source CubeDimension browser

                        object dragSource = e.Data.GetData("DragSource");
                        if (dragSource is CubeDimensionBrowser)
                        {
                            MetaTreeNode processingNode = null;
                            MetaTreeNode parentDimensionNode = clipBoardNode.GetRootNode() as MetaTreeNode;
                            if (this.OlapDataManager.DataProvider.ProviderName != Syncfusion.Olap.DataProvider.Providers.ActivePivot)
                            {
                                if (clipBoardNode.NodeType == MetaTreeNodeType.Member && clipBoardNode.ParentNode != null)
                                    clipBoardNode = clipBoardNode.ParentNode.ParentNode;
                                else if (clipBoardNode.NodeType == MetaTreeNodeType.Level)
                                    clipBoardNode = clipBoardNode.ParentNode;
                            }

                            if (clipBoardNode.Properties.Count > 0)
                            {
                                Property nodeProperty = clipBoardNode.Properties[0];

                                #region When Dimension

                                //// When dimension node dragged
                                if (nodeProperty.Value is Dimension)
                                {
                                    processingNode = this.GetDimensionNode(clipBoardNode.Properties.FindByName(PropertyConstants.Dimension).Value as Dimension);
                                }
                                #endregion

                                #region When Hierarchy

                                ///// When Hierarchy node dragged
                                else if (nodeProperty.Value is Hierarchy)
                                {
                                    if (parentDimensionNode.Properties.Count > 0)
                                    {
                                        Dimension dimensionObj = parentDimensionNode.Properties.FindByName(PropertyConstants.Dimension).Value as Dimension;
                                        processingNode = this.GetDefaultLevelMembersNode(clipBoardNode.Properties[0].Value as Hierarchy, dimensionObj.Name, dimensionObj.Caption);
                                    }
                                }
                                #endregion

                                #region When Level
                                //// When Level node dragged
                                else if (nodeProperty.Value is Level)
                                {
                                    if (parentDimensionNode.Properties.Count > 0)
                                    {
                                        Dimension dimensionObj = parentDimensionNode.Properties.FindByName(PropertyConstants.Dimension).Value as Dimension;
                                        //// This line are commented out since each level node takes its own parent node as a member and modified the alternate way
                                        //MetaTreeNode metaHierarchy = this.GetHierarchyNode(dimensionObj.Hierarchies, dimensionObj.DefaultHierarchyName, dimensionObj.Name, dimensionObj.Caption);
                                        MetaTreeNode metaHierarchy = this.GetHierarchyNode(dimensionObj.Hierarchies, clipBoardNode.ParentNode.UniqueName, dimensionObj.Name, dimensionObj.Caption);
                                        Hierarchy hierarchyObj = metaHierarchy.Properties.FindByName(PropertyConstants.Hierarchy).Value as Hierarchy;
                                        processingNode = this.GetMemberNode(hierarchyObj, clipBoardNode.Properties[0].Value as Level, dimensionObj.Name, dimensionObj.Caption);
                                    }
                                }

                                #endregion

                                #region When NamedSet
                                ///// When Named set dragged
                                else if (nodeProperty.Value is NamedSet)
                                {
                                    AxisElementBuilder axisBuilder = GetNamedSetAxisBuilder();

                                    if (axisBuilder == null)
                                        axisBuilder = this;

                                    if (parentDimensionNode.Properties.Count > 0)
                                    {
                                        Dimension dimensionObj = parentDimensionNode.Properties.FindByName(PropertyConstants.Dimension).Value as Dimension;
                                        NamedSet namedSetObj = (NamedSet)clipBoardNode.Properties.FindByName(PropertyConstants.NamedSet).Value;

                                        #region Handling the removal of Named Set under the same dimension
                                        //// Check if the named set already exists in the same axis
                                        for (int i = 0; i < axisBuilder.MetaTreeNodes.Count; i++)
                                        {
                                            MetaTreeNode childNode = axisBuilder.MetaTreeNodes[i];
                                            //// If NamedSet is in Slicer and Parent Dimension exists
                                            //// then do not remove the child node
                                            if (childNode.NodeType == MetaTreeNodeType.NamedSet &&
                                                childNode.UniqueName == namedSetObj.ParentDimensionName &&
                                                ValidateNamedSetPosition(dimensionObj.Name))
                                            {
                                                axisBuilder.MetaTreeNodes.Remove(childNode);
                                                break;
                                            }
                                        }
                                        #endregion

                                        if (!ValidateNamedSetPosition(dimensionObj.Name))
                                        {
                                            throw new Exception("Named Set cannot be present in Slicer if the parent Dimension exists in any other axis");
                                        }

                                        processingNode = new MetaTreeNode(namedSetObj.Name, namedSetObj.Name, namedSetObj.Description);
                                        processingNode.UniqueName = namedSetObj.ParentDimensionName;
                                        processingNode.NodeType = MetaTreeNodeType.NamedSet;

                                        MetaTreeNode metaTreeNode = new MetaTreeNode(namedSetObj.Name, namedSetObj.Name, namedSetObj.Description);
                                        metaTreeNode.UniqueName = namedSetObj.ParentDimensionName;
                                        metaTreeNode.NodeType = MetaTreeNodeType.NamedSet;
                                        Property propertyMember = metaTreeNode.Properties.FindByName(PropertyConstants.NamedSet);
                                        if (propertyMember == null)
                                        {
                                            metaTreeNode.Properties.Add(PropertyConstants.NamedSet, namedSetObj);
                                            metaTreeNode.Properties.Add(PropertyConstants.Dimension, dimensionObj);
                                            processingNode.Properties.Add(PropertyConstants.NamedSet, namedSetObj);
                                            processingNode.Properties.Add(PropertyConstants.Dimension, dimensionObj);
                                        }
                                        metaTreeNode.ParentNode = processingNode;
                                        //metaTreeNode.SetIsChecked(true, false, true);
                                        //metaTreeNode.ForceAcceptChanges();
                                        processingNode.ChildNodes.Add(metaTreeNode);
                                    }
                                }

                                #endregion

                                #region When KpiCollection

                                ////// When Kpi group dragged
                                //else if (nodeProperty.Value is KpiCollection)
                                //{
                                //    if (this.Axis == AxisPosition.Slicer)
                                //    {
                                //        throw new Exception("Kpi's Cannot be Sliced");
                                //    }
                                //}

                                #endregion

                                #region When Kpi

                                //// When kpi object dragged
                                else if (nodeProperty.Value is Kpi || nodeProperty.Value is KpiCollection)
                                {
                                    #region Processing When Kpi Dragged

                                    MetaTreeNode kpiProcessingNode = null;

                                    /*
                                     * ==========================================================
                                     * Invalid senarios exceptions
                                     * ==========================================================
                                     */

                                    #region Invalid Senario Exceptions

                                    //// KPI cannot be sliced
                                    if (this.Axis == AxisPosition.Slicer)
                                    {
                                        throw new Exception("Since KPI is a calculated measure it cannot be sliced");
                                    }

                                    //// Measure and KPI should be in same axis, check for the same
                                    else if (this.Axis == AxisPosition.Categorical)
                                    {
                                        /// Get the series element and check if measure exis, if yes then
                                        /// throw the exception
                                        Items items = GetItemsByAxis(AxisPosition.Series);
                                        MeasureElements measureElements = GetMeasureElements(items);
                                        if (measureElements != null)
                                            throw new Exception("Measure and KPI should be in same axis");
                                    }
                                    else if (this.Axis == AxisPosition.Series)
                                    {
                                        /// Get the categorical element and check if measure exis, if yes then
                                        /// throw the exception
                                        Items items = GetItemsByAxis(AxisPosition.Categorical);
                                        MeasureElements measureElements = GetMeasureElements(items);
                                        if (measureElements != null)
                                            throw new Exception("Measure and KPI should be in same axis");
                                    }

                                    #endregion

                                    /*
                             * ==========================================================
                             * Getting the kpi processing node(Main Kpi node)
                             * ==========================================================
                             */

                                    if (clipBoardNode.NodeType == MetaTreeNodeType.KPI_Value ||
                                        clipBoardNode.NodeType == MetaTreeNodeType.KPI_Goal ||
                                        clipBoardNode.NodeType == MetaTreeNodeType.KPI_Status ||
                                        clipBoardNode.NodeType == MetaTreeNodeType.KPI_Trend)
                                    {
                                        kpiProcessingNode = clipBoardNode.ParentNode;
                                        if (kpiProcessingNode.IsSelected == true)
                                        {
                                            kpiProcessingNode.IsSelected = false;
                                            kpiProcessingNode.AcceptIsSelectedChanges(false);
                                        }

                                        clipBoardNode.IsSelected = true;
                                        clipBoardNode.AcceptIsSelectedChanges(true);

                                    }
                                    else
                                    {
                                        kpiProcessingNode = clipBoardNode;
                                    }

                                    /*
                                     * ==========================================================
                                     * Referencing the axis builder where the KPI belongs to.
                                     * ==========================================================
                                     */

                                    AxisElementBuilder axisBuilder = GetKpiAxisBuilder();
                                    if (axisBuilder == null)
                                        axisBuilder = this;

                                    /*
                                     * ==========================================================
                                     * Processing the Kpi
                                     * ==========================================================
                                     */

                                    MetaTreeNode kpiGroupNode = null;

                                    //// Check if the Kpi already exist in the same axis
                                    foreach (MetaTreeNode childNode in axisBuilder.MetaTreeNodes)
                                    {
                                        if (childNode.NodeType == MetaTreeNodeType.KPI_ROOT
                                            && childNode.Name == PropertyConstants.KPI)
                                        {
                                            kpiGroupNode = childNode;
                                            break;
                                        }
                                    }

                                    if (kpiGroupNode == null)
                                    {
                                        kpiGroupNode = new MetaTreeNode(PropertyConstants.KPI,
                                            PropertyConstants.KPI, PropertyConstants.KPI);
                                        kpiGroupNode.NodeType = MetaTreeNodeType.KPI_ROOT;

                                        if (parentDimensionNode.Properties.Count > 0)
                                        {
                                            Property property = parentDimensionNode.Properties.FindByName(PropertyConstants.KPI);
                                            if (property != null)
                                            {
                                                KpiCollection kpiCollection = property.Value as KpiCollection;
                                                if (kpiCollection != null)
                                                    kpiGroupNode.Properties.Add(new Property(PropertyConstants.KPI, kpiCollection));
                                            }
                                        }

                                        kpiGroupNode.ChildNodes.Add(kpiProcessingNode);

                                    }
                                    else
                                    {

                                        //// Remove the kpi from the other axis and add it to the current one
                                        axisBuilder.MetaTreeNodes.Remove(kpiGroupNode);
                                        //// Adding the node to current node collection
                                        this.MetaTreeNodes.Add(kpiGroupNode);

                                        if (kpiGroupNode.Name == kpiProcessingNode.Name)
                                        {
                                            this.RefreshOlapDataManagerElementItems(this);
                                            return;
                                        }

                                        foreach (MetaTreeNode node in kpiGroupNode.ChildNodes)
                                        {
                                            ////if already node exist in the collection, then do not proecess; exit
                                            if (node.Name == kpiProcessingNode.Name)
                                            {
                                                node.ChildNodes.Where(i => i.Name == clipBoardNode.Name).Select(i => i.IsSelected = true);
                                                node.AcceptIsSelectedChanges(true);
                                                this.RefreshOlapDataManagerElementItems(this);
                                                return;
                                            }
                                        }
                                        kpiGroupNode.ChildNodes.Add(kpiProcessingNode);
                                        this.RefreshOlapDataManagerElementItems(this);
                                        return;
                                    }

                                    processingNode = kpiGroupNode;

                                    #endregion
                                }

                                #endregion

                                #region When Measure

                                else if (nodeProperty.Value is Measure || nodeProperty.Value is CalculatedMember || nodeProperty.Value is VirtualKpiElement)
                                {
                                    CalculatedMember calcMember = nodeProperty.Value as CalculatedMember;
                                    if (calcMember != null && calcMember.Type == TypeOfMember.Dimension)
                                    {
                                        #region Processig when Calculated Dimensions dragged
                                        processingNode = clipBoardNode;
                                        #endregion
                                    }
                                    else
                                    {
                                        #region Processing When Measure dragged

                                        /*
                             * ==========================================================
                             * Referencing the axis builder where the measure belongs to.
                             * ==========================================================
                             */


                                        AxisElementBuilder axisBuilder = GetMeasureAxisBuilder();
                                        if (axisBuilder == null)
                                            axisBuilder = this;

                                        /*
                                         * ==========================================================
                                         * Getting the measure parent node
                                         * ==========================================================
                                         */
                                        MetaTreeNode measureGroupNode = null;
                                        List<MetaTreeNode> listOfCalcMeasures = null;

                                        //// Check if the measure already exist in the same axis
                                        foreach (MetaTreeNode childNode in axisBuilder.MetaTreeNodes)
                                        {
                                            if (childNode.NodeType == MetaTreeNodeType.CalculatedMember
                                                && childNode.Properties[0] != null && childNode.Properties[0].Value is CalculatedMember
                                                && (childNode.Properties[0].Value as CalculatedMember).Type == TypeOfMember.Measure)
                                            {
                                                listOfCalcMeasures = listOfCalcMeasures ?? new List<MetaTreeNode>();
                                                listOfCalcMeasures.Add(childNode);
                                            }
                                            else if (childNode.NodeType == MetaTreeNodeType.MeasureGroup
                                                && childNode.Name == PropertyConstants.MeasrueNodeName)
                                            {
                                                measureGroupNode = childNode;
                                            }
                                        }

                                        /*
                                         * ==========================================================
                                         * Invalid scenarios exceptions
                                         * ==========================================================
                                         */

                                        #region Invalid Senario Exceptions

                                        ////More than one measure cannot be added to the slicer, check for the same
                                        if (this.Axis == AxisPosition.Slicer)
                                        {
                                            if (measureGroupNode != null && listOfCalcMeasures != null)
                                            {
                                                throw new Exception("More than one measure cannot be sliced");
                                            }
                                            else if (this.OlapDataManager != null && (this.OlapDataManager as OlapDataManager).ActiveReport != null && (this.OlapDataManager as OlapDataManager).UseSharedDataManager)
                                            {
                                                MeasureElements measureElements = GetMeasureElements((this.OlapDataManager as OlapDataManager).ActiveReport.CategoricalElements);
                                                if (measureElements == null)
                                                    measureElements = GetMeasureElements((this.OlapDataManager as OlapDataManager).ActiveReport.SeriesElements);
                                                CalculatedMember calcMeasure = GetCalculatedMeasureElements((this.OlapDataManager as OlapDataManager).ActiveReport.CategoricalElements);
                                                if (measureElements != null && calcMeasure != null)
                                                    throw new Exception("More than one measure cannot be sliced");
                                                else if (measureElements != null && measureElements.Elements.Count > 1)
                                                    throw new Exception("More than one measure cannot be sliced");
                                            }
                                            else if (this.OlapDataManager != null && this.OlapDataManager.CurrentReport != null)
                                            {

                                                MeasureElements measureElements = GetMeasureElements(this.OlapDataManager.CurrentReport.CategoricalElements);
                                                if (measureElements == null)
                                                    measureElements = GetMeasureElements(this.OlapDataManager.CurrentReport.SeriesElements);
                                                CalculatedMember calcMeasure = GetCalculatedMeasureElements(this.OlapDataManager.CurrentReport.CategoricalElements);
                                                if (measureElements != null || calcMeasure != null)
                                                    throw new Exception("More than one measure cannot be sliced");
                                            }
                                        }

                                        else
                                        {
                                            AxisElementBuilder kpiAxis = GetKpiAxisBuilder();
                                            if (kpiAxis != null)
                                            {
                                                if (kpiAxis.Axis != this.Axis)
                                                {
                                                    throw new Exception("Measure and KPI should be in same axis");
                                                }
                                            }
                                        }
                                        if (nodeProperty.Value is VirtualKpiElement)
                                        {
                                            if (this.Axis == AxisPosition.Slicer)
                                            {
                                                throw new Exception("Since KPI is a calculated measure it cannot be sliced");
                                            }

                                       //// Measure and KPI should be in same axis, check for the same
                                            else if (this.Axis == AxisPosition.Categorical)
                                            {
                                                /// Get the series element and check if measure exis, if yes then
                                                /// throw the exception
                                                Items items = GetItemsByAxis(AxisPosition.Series);
                                                MeasureElements measureElements = GetMeasureElements(items);
                                                if (measureElements != null)
                                                    throw new Exception("Measure and KPI should be in same axis");
                                            }
                                            else if (this.Axis == AxisPosition.Series)
                                            {
                                                /// Get the categorical element and check if measure exis, if yes then
                                                /// throw the exception
                                                Items items = GetItemsByAxis(AxisPosition.Categorical);
                                                MeasureElements measureElements = GetMeasureElements(items);
                                                if (measureElements != null)
                                                    throw new Exception("Measure and KPI should be in same axis");
                                            }
                                        }
                                        #endregion

                                        //// If any calculated measures exist in source axis then remove it and into current axis and refresh the axis with current items.
                                        if (listOfCalcMeasures != null)
                                        {
                                            RefreshElementItems(axisBuilder);
                                            foreach (MetaTreeNode item in listOfCalcMeasures)
                                            {
                                                if (axisBuilder.MetaTreeNodes.Remove(item))
                                                {
                                                    this.MetaTreeNodes.Add(item);
                                                    RefreshElements(item, false);
                                                }
                                            }
                                            if (this.MetaTreeNodes.Count > 0 && !this.MetaTreeNodes.Where(i => i.Name == clipBoardNode.Name).Any() && (clipBoardNode.NodeType == MetaTreeNodeType.CalculatedMember || clipBoardNode.NodeType == MetaTreeNodeType.VirtualKPIMember))
                                            {
                                                this.MetaTreeNodes.Add(clipBoardNode);
                                                RefreshElements(clipBoardNode, false);
                                            }
                                            if ((this.OlapDataManager != null && clipBoardNode.NodeType != MetaTreeNodeType.Measure && measureGroupNode == null)
                                                || (this.OlapDataManager != null && (clipBoardNode.NodeType == MetaTreeNodeType.CalculatedMember || clipBoardNode.NodeType == MetaTreeNodeType.VirtualKPIMember) && measureGroupNode != null))
                                            {
                                               this.RefreshOlapDataManagerElementItems(this);
                                               return;
                                            }
                                        }

                                        //// When no children available initialize the NodeGroup and Add
                                        //// the first element
                                        if (measureGroupNode == null)
                                        {
                                            if (nodeProperty.Value is Measure)
                                            {
                                                measureGroupNode = new MetaTreeNode(PropertyConstants.MeasrueNodeName,
                                                    PropertyConstants.MeasrueNodeName, PropertyConstants.MeasrueNodeName);
                                                measureGroupNode.NodeType = MetaTreeNodeType.MeasureGroup;

                                                if (parentDimensionNode.Properties.Count > 0)
                                                {
                                                    Property property = parentDimensionNode.Properties.FindByName(PropertyConstants.MeasureGroupName);
                                                    if (property != null)
                                                    {
                                                        MeasureCollection measureCollection = property.Value as MeasureCollection;
                                                        if (measureCollection != null)
                                                            measureGroupNode.Properties.Add(new Property(PropertyConstants.MeasureGroupName, measureCollection));
                                                    }
                                                }
                                                measureGroupNode.ChildNodes.Add(clipBoardNode.Clone() as MetaTreeNode);
                                            }
                                        }
                                        //// if already children's exit add the children to the current elements collection
                                        else if(clipBoardNode.NodeType!=MetaTreeNodeType.VirtualKPIMember && clipBoardNode.NodeType!=MetaTreeNodeType.VirtualKPI_Goal&&clipBoardNode.NodeType!=MetaTreeNodeType.VirtualKPI_Status&&clipBoardNode.NodeType!=MetaTreeNodeType.VirtualKPI_Trend&&clipBoardNode.NodeType!=MetaTreeNodeType.VirtualKPI_Value)
                                        {
                                            //if (axisBuilder.Axis != this.Axis)
                                            {
                                                //// Remove the measure from the other axis and add it to the current one
                                                axisBuilder.MetaTreeNodes.Remove(measureGroupNode);
                                                //// Synchronizing meta tree node and Items of axisBuilder.
                                                RefreshElementItems(axisBuilder);
                                                //// Adding the node to current node collection
                                                this.MetaTreeNodes.Add(measureGroupNode);
                                            }
                                            if (nodeProperty.Value is Measure)
                                            {
                                                foreach (MetaTreeNode node in measureGroupNode.ChildNodes)
                                                {
                                                    //// Already node exit in the collection, do not process; Exit
                                                    if (node.Name == clipBoardNode.Name)
                                                    {
                                                        this.RefreshOlapDataManagerElementItems(this);
                                                        return;
                                                    }
                                                }
                                                measureGroupNode.ChildNodes.Add(clipBoardNode.Clone() as MetaTreeNode);
                                                this.RefreshOlapDataManagerElementItems(this);
                                                return;
                                            }
                                        }

                                        if (calcMember != null && calcMember.Type == TypeOfMember.Measure)
                                        {
                                            processingNode = clipBoardNode;
                                        }
                                        else if (nodeProperty.Value is VirtualKpiElement)
                                        {
                                            if (clipBoardNode.NodeType == MetaTreeNodeType.VirtualKPI_Value ||
                                                clipBoardNode.NodeType == MetaTreeNodeType.VirtualKPI_Goal ||
                                                clipBoardNode.NodeType == MetaTreeNodeType.VirtualKPI_Status ||
                                                clipBoardNode.NodeType == MetaTreeNodeType.VirtualKPI_Trend)
                                            {
                                                processingNode = clipBoardNode.ParentNode;
                                                if (processingNode.IsSelected == true)
                                                {
                                                    processingNode.IsSelected = false;
                                                    processingNode.AcceptIsSelectedChanges(false);
                                                }

                                                clipBoardNode.IsSelected = true;
                                                clipBoardNode.AcceptIsSelectedChanges(true);

                                            }
                                            else
                                            {
                                                processingNode = clipBoardNode;
                                            }
                                        }
                                        else
                                        {
                                            processingNode = measureGroupNode;
                                        }

                                        #endregion
                                    }
                                }

                                #endregion
                            }

                            #region Processing
                            //processingNode.Properties.Add(new Property(PropertyConstants.AxisElements, this));
                            //// Adding dragged node to the current nodes collection
                            if (!this.CheckForExistence(processingNode) || processingNode.Name == PropertyConstants.KPI)
                            {
                                this.MetaTreeNodes.Add(processingNode);
                                if (this.OlapDataManager != null)
                                {
                                    //// Refreshing the elements
                                    RefreshElements(processingNode, false);
                                    this.RefreshOlapDataManagerElementItems(this);
                                }
                                else
                                {
                                    throw new OlapDataManagerException("Manager is null in axis elements");
                                }
                            }
                            else
                            {
                                MessageBox.Show("This Element is already exist in the current axis", "Current Report", MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                            #endregion
                        }

                        #endregion
                    }
                    //// If drag source axis element builder
                    else
                    {
                        #region Drag source AxisElement builder

                        //// Getting the current report object
                        OlapReport currentReport = ((this.OlapDataManager as OlapDataManager).ActiveReport != null && (this.OlapDataManager as OlapDataManager).UseSharedDataManager) ? (this.OlapDataManager as OlapDataManager).ActiveReport : this.OlapDataManager.CurrentReport;

                        //// Store whether dragged node is a calculated measure.
                        bool isCalcMeasure = clipBoardNode.NodeType == MetaTreeNodeType.CalculatedMember && clipBoardNode.Properties[0] != null && clipBoardNode.Properties[0].Value is CalculatedMember && (clipBoardNode.Properties[0].Value as CalculatedMember).Type == TypeOfMember.Measure;

                        switch (this.Axis)
                        {
                            case AxisPosition.Categorical:
                                {
                                    #region Invalid Criteria Exception

                                    //// Check if measure exist in different axis
                                    if ((clipBoardNode.Name == PropertyConstants.KPI && clipBoardNode.NodeType == MetaTreeNodeType.KPI_ROOT) || (clipBoardNode.NodeType == MetaTreeNodeType.VirtualKPIMember))
                                    {
                                        AxisElementBuilder axisMeasure = GetMeasureAxisBuilder();
                                        if (axisMeasure != null)
                                        {
                                            if (axisMeasure.Axis != this.Axis)
                                                throw new Exception("Measure and KPI should be in same axis");
                                        }
                                    }

                                    //// Check if kpi exist in different axis
                                    if (isCalcMeasure || (clipBoardNode.Name == PropertyConstants.MeasrueNodeName && clipBoardNode.NodeType == MetaTreeNodeType.MeasureGroup))
                                    {
                                        AxisElementBuilder axisKpi = GetKpiAxisBuilder();
                                        if (axisKpi != null)
                                        {
                                            if (axisKpi.Axis != this.Axis)
                                                throw new Exception("Measure and KPI should be in same axis");
                                        }
                                    }

                                    #endregion

                                    if (isCalcMeasure || (clipBoardNode.Name == PropertyConstants.MeasrueNodeName && clipBoardNode.NodeType == MetaTreeNodeType.MeasureGroup))
                                    {
                                        AxisElementBuilder measureAxis = GetMeasureAxisBuilder();
                                        if (measureAxis != null)
                                        {
                                            var allMeasureNodes = measureAxis.MetaTreeNodes.Where(i => (i.NodeType == MetaTreeNodeType.MeasureGroup)
                                                || (i.NodeType == MetaTreeNodeType.CalculatedMember && i.Properties[0] != null && i.Properties[0].Value is CalculatedMember && (i.Properties[0].Value as CalculatedMember).Type == TypeOfMember.Measure));

                                            foreach (MetaTreeNode mtNode in allMeasureNodes)
                                            {
                                                Item item = RemoveIfSameDimensionElementExist(currentReport.SeriesElements, mtNode);
                                                if (item == null)
                                                {
                                                    item = RemoveIfSameDimensionElementExist(currentReport.SlicerElements, mtNode);
                                                    if (item == null)
                                                    {
                                                        item = RemoveIfSameDimensionElementExist(currentReport.CategoricalElements, mtNode);
                                                    }
                                                }

                                                if (item != null)
                                                {
                                                    //// moving the element to categorical
                                                    currentReport.CategoricalElements.Add(item);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //// For categorical check if the elements exist in series or in slicer, if found move the
                                        //// the element to categorical axis
                                        Item item = RemoveIfSameDimensionElementExist(currentReport.SeriesElements, clipBoardNode);
                                        if (item == null)
                                        {
                                            item = RemoveIfSameDimensionElementExist(currentReport.SlicerElements, clipBoardNode);
                                            if (item == null)
                                            {
                                                item = RemoveIfSameDimensionElementExist(currentReport.CategoricalElements, clipBoardNode);
                                            }
                                            else if (item.ElementValue is DimensionElement)
                                            {
                                                DimensionElement dimension = (item.ElementValue as DimensionElement);
                                                if (dimension.Hierarchy != null)
                                                {
                                                    foreach (LevelElement level in dimension.Hierarchy.LevelElements)
                                                    {
                                                        level.MemberElements.Clear();
                                                    }
                                                }
                                            }
                                        }

                                        if (item != null)
                                        {
                                            //// moving the element to categorical
                                            currentReport.CategoricalElements.Add(item);
                                        }
                                    }
                                    break;
                                }
                            case AxisPosition.Series:
                                {
                                    #region Invalid Criteria Exception

                                    //// Check if measure exist in different axis
                                    if ((clipBoardNode.Name == PropertyConstants.KPI && clipBoardNode.NodeType == MetaTreeNodeType.KPI_ROOT) || (clipBoardNode.NodeType == MetaTreeNodeType.VirtualKPIMember))
                                    {
                                        AxisElementBuilder axisMeasure = GetMeasureAxisBuilder();
                                        if (axisMeasure != null)
                                        {
                                            if (axisMeasure.Axis != this.Axis)
                                                throw new Exception("Measure and KPI should be in same axis");
                                        }
                                    }

                                    //// Check if kpi exist in different axis
                                    if (isCalcMeasure || (clipBoardNode.Name == PropertyConstants.MeasrueNodeName && clipBoardNode.NodeType == MetaTreeNodeType.MeasureGroup))
                                    {
                                        AxisElementBuilder axisKpi = GetKpiAxisBuilder();
                                        if (axisKpi != null)
                                        {
                                            if (axisKpi.Axis != this.Axis)
                                                throw new Exception("Measure and KPI should be in same axis");
                                        }
                                    }
                                    //// Check if virtual kpi exist in different axis
                                    if (isCalcMeasure || (clipBoardNode.NodeType == MetaTreeNodeType.VirtualKPIMember))
                                    {
                                        AxisElementBuilder axisKpi = GetKpiAxisBuilder();
                                        if (axisKpi != null)
                                        {
                                            if (axisKpi.Axis != this.Axis)
                                                throw new Exception("Measure and KPI should be in same axis");
                                        }
                                    }
                                    #endregion

                                    if (isCalcMeasure || (clipBoardNode.Name == PropertyConstants.MeasrueNodeName && clipBoardNode.NodeType == MetaTreeNodeType.MeasureGroup))
                                    {
                                        AxisElementBuilder measureAxis = GetMeasureAxisBuilder();
                                        if (measureAxis != null)
                                        {
                                            var allMeasureNodes = measureAxis.MetaTreeNodes.Where(i => (i.NodeType == MetaTreeNodeType.MeasureGroup)
                                                || (i.NodeType == MetaTreeNodeType.CalculatedMember && i.Properties[0] != null && i.Properties[0].Value is CalculatedMember && (i.Properties[0].Value as CalculatedMember).Type == TypeOfMember.Measure));

                                            foreach (MetaTreeNode mtNode in allMeasureNodes)
                                            {
                                                Item item = RemoveIfSameDimensionElementExist(currentReport.CategoricalElements, mtNode);
                                                if (item == null)
                                                {
                                                    item = RemoveIfSameDimensionElementExist(currentReport.SlicerElements, mtNode);
                                                    if (item == null)
                                                    {
                                                        item = RemoveIfSameDimensionElementExist(currentReport.SeriesElements, mtNode);
                                                    }
                                                }

                                                if (item != null)
                                                {
                                                    //// Moving the element to series
                                                    currentReport.SeriesElements.Add(item);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        /// For series check if the elements exists in categorical or in slicer, if found move the 
                                        /// elements to series
                                        Item item = RemoveIfSameDimensionElementExist(currentReport.CategoricalElements, clipBoardNode);
                                        if (item == null)
                                        {
                                            item = RemoveIfSameDimensionElementExist(currentReport.SlicerElements, clipBoardNode);
                                            if (item == null)
                                            {
                                                item = RemoveIfSameDimensionElementExist(currentReport.SeriesElements, clipBoardNode);
                                            }
                                            else if (item.ElementValue is DimensionElement)
                                            {
                                                DimensionElement dimension = (item.ElementValue as DimensionElement);
                                                if (dimension.Hierarchy != null)
                                                {
                                                    foreach (LevelElement level in dimension.Hierarchy.LevelElements)
                                                    {
                                                        level.MemberElements.Clear();
                                                    }
                                                }
                                            }
                                        }

                                        if (item != null)
                                        {
                                            //// Moving the element to series
                                            currentReport.SeriesElements.Add(item);
                                        }
                                    }
                                    break;
                                }
                            case AxisPosition.Slicer:
                                {
                                    //// More than one measure cannot be drop in slicer
                                    if (clipBoardNode.NodeType == MetaTreeNodeType.MeasureGroup)
                                    {
                                        var checkedNodeCount = clipBoardNode.ChildNodes.Where(i => i.NodeCheckedType == MetaTreeNodeCheckedType.CurrentChecked).Count();
                                        if (checkedNodeCount > 1)
                                            throw new Exception("More than one measure cannot be sliced");
                                    }

                                    if (isCalcMeasure)
                                    {
                                        AxisElementBuilder measureAxis = GetMeasureAxisBuilder();
                                        if (measureAxis != null && measureAxis.MetaTreeNodes.Any(i => (i.NodeType == MetaTreeNodeType.CalculatedMember && i.Properties[0] != null && i.Properties[0].Value is CalculatedMember && (i.Properties[0].Value as CalculatedMember).Type == TypeOfMember.Measure) || (i.NodeType == MetaTreeNodeType.Measure || i.NodeType == MetaTreeNodeType.MeasureGroup)))
                                        {
                                            throw new Exception("More than one measure cannot be sliced");
                                        }
                                    }

                                    //// KPI's cannot be dropped in slicer
                                    if ((clipBoardNode.NodeType == MetaTreeNodeType.KPI_ROOT && clipBoardNode.Name == PropertyConstants.KPI) || clipBoardNode.NodeType==MetaTreeNodeType.VirtualKPIMember)
                                    {
                                        throw new Exception("Since KPI is a calculated measure it cannot be sliced");
                                    }

                                    /// Check for Parent Dimension of Namedset 
                                    if (clipBoardNode.NodeType == MetaTreeNodeType.NamedSet)
                                    {
                                        if (!ValidateNamedSetPosition(clipBoardNode.UniqueName))
                                        {
                                            throw new Exception("Named Set cannot be present in Slicer if the parent Dimension exists in any other axis");
                                        }
                                    }

                                    /// For slicer check if the elements exists in categorical or in series, if found move the 
                                    /// elements to slicer
                                    Item item = RemoveIfSameDimensionElementExist(currentReport.CategoricalElements, clipBoardNode);
                                    if (item == null)
                                    {
                                        item = RemoveIfSameDimensionElementExist(currentReport.SeriesElements, clipBoardNode);
                                        if (item == null)
                                            item = RemoveIfSameDimensionElementExist(currentReport.SlicerElements, clipBoardNode);
                                    }

                                    if (item != null)
                                    {
                                        if (item.ElementValue is DimensionElement)
                                        {
                                            QueryBuilderEngineHelper.SetProviderName(this.OlapDataManager.DataProvider.ProviderName);
                                            item.ElementValue = QueryBuilderEngineHelper.GetElementValue(clipBoardNode, true);
                                        }

                                        //// Moving the element to slicer
                                        currentReport.SlicerElements.Add(item);
                                    }
                                    break;
                                }
                        }

                        //// Triggering the axis elements to re-load based on the elements
                        //this.OlapDataManager.NotifyReportChanged(currentReport);
                        //// Triggering the controls to re-load based on the new elements
                        NotifyElementModified();

                        #endregion
                    }
                }
            }
        }

        private bool CheckForExistence(MetaTreeNode processingNode)
        {
            foreach (MetaTreeNode node in this.MetaTreeNodes)
            {
                if ((node.NodeType == MetaTreeNodeType.CalculatedMember && node.UniqueName == processingNode.UniqueName)
                    || (node.NodeType == MetaTreeNodeType.NamedSet && node.Name == processingNode.Name) || (node.NodeType == MetaTreeNodeType.VirtualKPIMember && node.UniqueName == processingNode.UniqueName))
                {
                    return true;
                }
                else if (!string.IsNullOrEmpty(node.UniqueName) && node.UniqueName == processingNode.UniqueName
                    && node.Properties.Count > 1 && processingNode.Properties.Count > 2
                    && node.Properties[1].Value is Hierarchy && processingNode.Properties[2].Value is Hierarchy)
                {
                    if ((node.Properties[1].Value as Hierarchy).UniqueName == (processingNode.Properties[2].Value as Hierarchy).UniqueName)
                    {
                        //// This line are commented out since each level node takes its own parent node as a member and modified the alternate way
                        //if ((node.Properties[2].Value as Level).UniqueName == (processingNode.Properties[1].Value as Level).UniqueName)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal void RefreshElements(MetaTreeNode processingNode, bool raiseNotifyElement)
        {
            OlapReport currentReport = ((this.OlapDataManager as OlapDataManager).ActiveReport != null && (this.OlapDataManager as OlapDataManager).UseSharedDataManager) ? (this.OlapDataManager as OlapDataManager).ActiveReport : this.OlapDataManager.CurrentReport;
            switch (this.Axis)
            {
                case AxisPosition.Categorical:
                    {
                        /// remove if the item exist in series/slicer
                        this.RemoveIfSameDimensionElementExist(currentReport.SeriesElements, processingNode);
                        this.RemoveIfSameDimensionElementExist(currentReport.SlicerElements, processingNode);

                        SubsetElement tempSubsetElement = currentReport.CategoricalElements.SubSetElement;
                        currentReport.CategoricalElements = this.GetElementItems;
                        if (tempSubsetElement != null)
                        {
                            currentReport.CategoricalElements.SubSetElement = tempSubsetElement;
                        }
                        break;
                    }
                case AxisPosition.Series:
                    {
                        /// remove if the item exist in categorical/slicer
                        this.RemoveIfSameDimensionElementExist(currentReport.CategoricalElements, processingNode);
                        this.RemoveIfSameDimensionElementExist(currentReport.SlicerElements, processingNode);

                        SubsetElement tempSubsetElement = currentReport.SeriesElements.SubSetElement;
                        currentReport.SeriesElements = this.GetElementItems;
                        if (tempSubsetElement != null)
                        {
                            currentReport.SeriesElements.SubSetElement = tempSubsetElement;
                        }
                        break;
                    }
                case AxisPosition.Slicer:
                    {
                        /// remove if the item exist in series/categorical
                        this.RemoveIfSameDimensionElementExist(currentReport.SeriesElements, processingNode);
                        this.RemoveIfSameDimensionElementExist(currentReport.CategoricalElements, processingNode);

                        currentReport.SlicerElements = this.GetElementItems;
                        break;
                    }
            }

            if (raiseNotifyElement && !(this.OlapDataManager as OlapDataManager).UseSharedDataManager)
            {
                //// Triggering the axis elements to re-load based on the elements
                NotifyElementModified();
            }
            else if(raiseNotifyElement)
            {
                (this.OlapDataManager as OlapDataManager).NotifyActiveReportChanged(currentReport,true);
            }
        }

        private void NotifyReportChanged(OlapReport currentReport)
        {
            if (AutoExecute)
                this.OlapDataManager.NotifyReportChanged(currentReport);
            else
                this.OlapDataManager.RaiseAxisElementModified();
        }

        private void NotifyElementModified()
        {
            if ((this.OlapDataManager as OlapDataManager).UseSharedDataManager && (this.OlapDataManager as OlapDataManager).ActiveReport != null)
            {
                (this.OlapDataManager as OlapDataManager).NotifyActiveReportChanged((this.OlapDataManager as OlapDataManager).ActiveReport,true);
            }
            else
            {
                if (AutoExecute)
                    this.OlapDataManager.NotifyElementModified();
                else
                    this.OlapDataManager.RaiseAxisElementModified();
            }
        }

        private bool ValidateNamedSetPosition(string parentDimensionName)
        {
            AxisElementBuilder axisBuilderDimension = GetDimensionAxisBuilder(parentDimensionName);

            //// Check if the Parent Dimension of the named set already exist in the same axis
            //// Then check for the axis if it is slicer then throw exception.
            if (axisBuilderDimension != null && this.Axis == AxisPosition.Slicer)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Helper methods

        private static Items GetCurrentAxisItems(AxisElementBuilder axisElementBuilder)
        {
            OlapReport currentReport = ((axisElementBuilder.OlapDataManager as OlapDataManager).ActiveReport != null && (axisElementBuilder.OlapDataManager as OlapDataManager).UseSharedDataManager) ? (axisElementBuilder.OlapDataManager as OlapDataManager).ActiveReport : axisElementBuilder.OlapDataManager.CurrentReport;
            Items items = null;
            if (axisElementBuilder.Axis == AxisPosition.Categorical)
            {
                items = currentReport.CategoricalElements;
            }
            else if (axisElementBuilder.Axis == AxisPosition.Series)
            {
                items = currentReport.SeriesElements;
            }
            else if (axisElementBuilder.Axis == AxisPosition.Slicer)
            {
                items = currentReport.SlicerElements;
            }
            return items;
        }

        /// <summary>
        /// Removing item in the items collection, if the same element exis in the 
        /// dragged node
        /// </summary>
        /// <param name="items">Items collection</param>
        /// <param name="draggedNode">Dragged node</param>
        /// <returns>Removed item</returns>
        private Item RemoveIfSameDimensionElementExist(Items items, MetaTreeNode draggedNode)
        {
            if (draggedNode != null)
            {
                for (int j = 0; j < items.Count; j++)
                {
                    Item item = items[j];
                    if ((draggedNode.NodeType == MetaTreeNodeType.CalculatedMember && item.ElementValue is CalculatedMember && (item.ElementValue as CalculatedMember).UniqueName.ToUpper() == draggedNode.UniqueName.ToUpper())
                        || (item.ElementValue is MeasureElements && draggedNode.Name == PropertyConstants.MeasrueNodeName)
                        || (item.ElementValue is KpiElements && draggedNode.Name == PropertyConstants.KPI) ||(item.ElementValue is VirtualKpiElement && draggedNode.NodeType == MetaTreeNodeType.VirtualKPIMember && (item.ElementValue as VirtualKpiElement).UniqueName.ToUpper() == draggedNode.UniqueName.ToUpper()))
                    {
                        items.Remove(item);
                        return item;
                    }
                    else if (item.ElementValue is NamedSetElement && draggedNode.Name.ToUpper() == item.ElementValue.Name.ToUpper())
                    {
                        NamedSetElement namedSetElement = item.ElementValue as NamedSetElement;
                        if (draggedNode.UniqueName.ToUpper() == namedSetElement.DimensionName.ToUpper())
                        {
                            items.Remove(item);
                            return item;
                        }
                    }
                    //// Dimension node
                    else if (item.ElementValue.Name != null && (item.ElementValue.Name.ToUpper() == draggedNode.Name.ToUpper()
                        || item.ElementValue.Name.ToUpper() == draggedNode.Caption.ToUpper()))
                    {
                        if (((Property)((PropertyCollection)draggedNode.Properties)[2]).Name == "HIERARCHY")
                        {
                            if (((DimensionElement)item.ElementValue).HierarchyName.ToUpper() == ((Hierarchy)((Property)((PropertyCollection)draggedNode.Properties)[2]).Value).Name.ToUpper())
                            {
                                if (((LevelElementCollection)((HierarchyElement)((DimensionElement)item.ElementValue).Hierarchy).LevelElements)[0].Name.ToUpper() == ((Level)((Property)((PropertyCollection)draggedNode.Properties)[1]).Value).Name.ToUpper()
                                    || ((LevelElementCollection)((HierarchyElement)((DimensionElement)item.ElementValue).Hierarchy).LevelElements)[0].UniqueName.ToUpper() == ((Level)((Property)((PropertyCollection)draggedNode.Properties)[1]).Value).UniqueName.ToUpper())
                                {
                                    items.Remove(item);
                                    return item;
                                }
                            }
                        }
                        else
                        {
                            if (((DimensionElement)item.ElementValue).HierarchyName.ToUpper() == ((Hierarchy)((Property)((PropertyCollection)draggedNode.Properties)[1]).Value).Name.ToUpper())
                            {
                                if (((LevelElementCollection)((HierarchyElement)((DimensionElement)item.ElementValue).Hierarchy).LevelElements)[0].Name.ToUpper() == ((Level)((Property)((PropertyCollection)draggedNode.Properties)[2]).Value).Name.ToUpper())
                                {
                                    items.Remove(item);
                                    return item;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        private Items GetItemsByAxis(AxisPosition axis)
        {
            if (this.OlapDataManager != null)
            {
                OlapReport currentReport = ((this.OlapDataManager as OlapDataManager).ActiveReport != null && (this.OlapDataManager as OlapDataManager).UseSharedDataManager) ? (this.OlapDataManager as OlapDataManager).ActiveReport
                    :this.OlapDataManager.CurrentReport;

                if (axis == AxisPosition.Categorical)
                    return currentReport.CategoricalElements;
                else if (axis == AxisPosition.Series)
                    return currentReport.SeriesElements;
                else if (axis == AxisPosition.Slicer)
                    return currentReport.SlicerElements;
            }
            return null;
        }

        /// <summary>
        /// Searching for the measure node from the MeasureNodes collection based on the 
        /// measure element supplied
        /// </summary>
        /// <param name="metaTreeNodeMeasuresSource">MeasureNodes collection</param>
        /// <param name="measureElement">measure element</param>
        /// <returns>Found measure node</returns>
        private MetaTreeNode GetMeasureNode(MetaTreeNode metaTreeNodeMeasuresSource, MeasureElement measureElement, bool selectedState)
        {
            foreach (MetaTreeNode childNode in metaTreeNodeMeasuresSource.ChildNodes)
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

        private MeasureElements GetMeasureElements(Items items)
        {
            foreach (Item item in items)
            {
                if (item.ElementValue is MeasureElements)
                    return item.ElementValue as MeasureElements;
            }
            return null;
        }

        private CalculatedMember GetCalculatedMeasureElements(Items items)
        {
            foreach (Item item in items)
            {
                if (item.ElementValue is CalculatedMember)
                    return item.ElementValue as CalculatedMember;
            }
            return null;
        }

        private static string GetAxisBuilderElementName(AxisPosition axisPosition)
        {
            if (axisPosition == AxisPosition.Categorical)
            {
                return PropertyConstants.AxisElementBuilderCategorical;
            }
            else if (axisPosition == AxisPosition.Series)
            {
                return PropertyConstants.AxisElementBuilderSeries;
            }
            else if (axisPosition == AxisPosition.Slicer)
            {
                return PropertyConstants.AxisElementBuilderSlicer;
            }
            return string.Empty;
        }

        private AxisElementBuilder GetMeasureAxisBuilder()
        {
            if (this.OlapDataManager != null)
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
            }
            return null;
        }

        private AxisElementBuilder GetDimensionAxisBuilder(string dimensionName)
        {
            if (this.OlapDataManager != null)
            {
                var axis = this.OlapDataManager.Properties.Where(a => a.Value is AxisElementBuilder);
                if (axis != null && axis.Count() > 0)
                {
                    foreach (var item in axis)
                    {
                        AxisElementBuilder axisBuilder = item.Value as AxisElementBuilder;
                        var metaTreeNode = axisBuilder.MetaTreeNodes.Where(i => i.NodeType == MetaTreeNodeType.Dimension && i.Caption == dimensionName);
                        if (metaTreeNode != null && metaTreeNode.Count() > 0)
                            return axisBuilder;
                    }
                }
            }
            return null;
        }

        private AxisElementBuilder GetNamedSetAxisBuilder()
        {
            if (this.OlapDataManager != null)
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
            }
            return null;
        }
        private AxisElementBuilder GetVirtualKpiAxisBuilder()
        {
            if (this.OlapDataManager != null)
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
            }
            return null;
        }
        private AxisElementBuilder GetKpiAxisBuilder()
        {
            if (this.OlapDataManager != null)
            {
                var axis = this.OlapDataManager.Properties.Where(a => a.Value is AxisElementBuilder);
                if (axis != null && axis.Count() > 0)
                {
                    foreach (var item in axis)
                    {
                        AxisElementBuilder axisBuilder = item.Value as AxisElementBuilder;
                        var metaTreeNode = axisBuilder.MetaTreeNodes.Where(i => (i.NodeType == MetaTreeNodeType.KPI_ROOT && i.Name == PropertyConstants.KPI) || i.NodeType==MetaTreeNodeType.VirtualKPIMember);
                        if (metaTreeNode != null && metaTreeNode.Count() > 0)
                            return axisBuilder;
                    }
                }
            }
            return null;
        }


        #endregion

        #region Manager triggers

        /// <summary>
        /// Refreshes the OlapDataManager with the latest available items
        /// </summary>
        /// <param name="axisElementBuiler">source axis builder</param>
        internal void RefreshOlapDataManagerElementItems(AxisElementBuilder axisElementBuiler)
        {
            RefreshOlapDataManagerElementItems(axisElementBuiler, AutoExecute);
        }

        public void RefreshOlapDataManagerElementItems(AxisElementBuilder axisElementBuiler, bool execute)
        {
            if (axisElementBuiler == null)
                axisElementBuiler = this;
           
            if (execute == true && !(axisElementBuiler.OlapDataManager as OlapDataManager).UseSharedDataManager)
            {
                RefreshElementItems(axisElementBuiler);

                //// Triggering the manager to notify control's to reload
                axisElementBuiler.OlapDataManager.NotifyElementModified();
            }
            else if ((axisElementBuiler.OlapDataManager as OlapDataManager).ActiveReport != null)
            {
                RefreshElementItems(axisElementBuiler);

                (axisElementBuiler.OlapDataManager as OlapDataManager).NotifyActiveReportChanged((axisElementBuiler.OlapDataManager as OlapDataManager).ActiveReport,true);
            }
            else
            {
                RefreshElementItems(axisElementBuiler);
            }
        }

        /// <summary>
        /// Refreshes the OlapDataManager with the latest available items
        /// </summary>
        /// <param name="axisElementBuiler">source axis builder</param>"
        internal void RefreshOlapDataManagerElementItems(AxisElementBuilder axisElementBuiler, bool refreshElement, AxisType axisType)
        {
            if (axisType == AxisType.Slicer)
            {
                if (axisElementBuiler == null)
                    axisElementBuiler = this;

                if (refreshElement)
                {
                    RefreshElementItems(axisElementBuiler);
                }
                else
                {
                    MessageBox.Show("More than one measure cannot be sliced");
                }
                //// Triggering the manager to notify control's to reload
                if ((axisElementBuiler.OlapDataManager as OlapDataManager).UseSharedDataManager && (axisElementBuiler.OlapDataManager as OlapDataManager).ActiveReport != null)
                {
                    (axisElementBuiler.OlapDataManager as OlapDataManager).NotifyActiveReportChanged();
                }
                else
                {
                    if (AutoExecute)
                        axisElementBuiler.OlapDataManager.NotifyElementModified();
                    else
                        this.OlapDataManager.RaiseAxisElementModified();
                }
            }

        }

        internal void RefreshElementItems(AxisElementBuilder axisElementBuiler)
        {
            //// Get the current manager from the manager
            OlapReport currentReport = ((this.OlapDataManager as OlapDataManager).ActiveReport != null && (this.OlapDataManager as OlapDataManager).UseSharedDataManager) ? (this.OlapDataManager as OlapDataManager).ActiveReport
                    :this.OlapDataManager.CurrentReport;
            /// if filtering applied then it will not be persisted upon draggers so 
            /// so clearing here
            if (currentReport.FilterElements.Count > 0)
            {
                currentReport.FilterElements.Clear();
                currentReport.CategoricalElements.IsFilterOrSortOn = false;
                currentReport.SeriesElements.IsFilterOrSortOn = false;
                currentReport.SlicerElements.IsFilterOrSortOn = false;
            }
            //// Synchronizing element based on axis modified
            switch (axisElementBuiler.Axis)
            {
                case AxisPosition.Categorical:
                    {
                        if (currentReport.TogglePivot)
                            axisElementBuiler.SynchronizeItems(currentReport.SeriesElements);
                        else
                            axisElementBuiler.SynchronizeItems(currentReport.CategoricalElements);
                        break;
                    }
                case AxisPosition.Series:
                    {
                        if (currentReport.TogglePivot)
                            axisElementBuiler.SynchronizeItems(currentReport.CategoricalElements);
                        else
                            axisElementBuiler.SynchronizeItems(currentReport.SeriesElements);
                        break;
                    }
                case AxisPosition.Slicer:
                    {
                        axisElementBuiler.SynchronizeItems(currentReport.SlicerElements);
                        break;
                    }
            }
            if (currentReport != null)
                AddVirtualKpisToReport(axisElementBuiler, currentReport);
            if (currentReport != null && (this.OlapDataManager as OlapDataManager).UseSharedDataManager)
            {
                AddCalcMemberToActiveReport(axisElementBuiler, currentReport);
            }
        }

        private static void AddCalcMemberToActiveReport(AxisElementBuilder axisElementBuiler, OlapReport currentReport)
        {
            currentReport.CalculatedMembers.Clear();
            if (currentReport.CategoricalElements.List.Where(i => i.ElementValue is CalculatedMember).Any())
            {
                currentReport.CategoricalElements.List.ForEach(i =>
                {
                    if (i.ElementValue is CalculatedMember)
                        currentReport.CalculatedMembers.Add(new Item
                        {
                            Axis = axisElementBuiler.Axis,
                            ElementValue = i.ElementValue,
                            ExcludedElementValue = i.ExcludedElementValue,
                            IsFilterOrSortOn = i.IsFilterOrSortOn,
                            ItemName = i.ItemName
                        });
                });
            }
            if (currentReport.SlicerElements.List.Where(i => i.ElementValue is CalculatedMember).Any())
            {
                currentReport.SlicerElements.List.ForEach(i =>
                {
                    if (i.ElementValue is CalculatedMember)
                        currentReport.CalculatedMembers.Add(new Item
                        {
                            Axis = axisElementBuiler.Axis,
                            ElementValue = i.ElementValue,
                            ExcludedElementValue = i.ExcludedElementValue,
                            IsFilterOrSortOn = i.IsFilterOrSortOn,
                            ItemName = i.ItemName
                        });
                });
            }
            if (currentReport.SeriesElements.List.Where(i => i.ElementValue is CalculatedMember).Any())
            {
                currentReport.SeriesElements.List.ForEach(i =>
                {
                    if (i.ElementValue is CalculatedMember)
                        currentReport.CalculatedMembers.Add(new Item
                        {
                            Axis = axisElementBuiler.Axis,
                            ElementValue = i.ElementValue,
                            ExcludedElementValue = i.ExcludedElementValue,
                            IsFilterOrSortOn = i.IsFilterOrSortOn,
                            ItemName = i.ItemName
                        });
                });
            }
        }

        private static void AddVirtualKpisToReport(AxisElementBuilder axisElementBuiler, OlapReport currentReport)
        {
            currentReport.VirtualKpiElements.Clear();
            if (currentReport.CategoricalElements.List.Where(i => i.ElementValue is VirtualKpiElement).Any())
            {
                currentReport.CategoricalElements.List.ForEach(i =>
                {
                    if (i.ElementValue is VirtualKpiElement)
                        currentReport.VirtualKpiElements.Add(new Item
                        {
                            Axis = axisElementBuiler.Axis,
                            ElementValue = i.ElementValue,
                            ExcludedElementValue = i.ExcludedElementValue,
                            IsFilterOrSortOn = i.IsFilterOrSortOn,
                            ItemName = i.ItemName
                        });
                });
            }
            if (currentReport.SlicerElements.List.Where(i => i.ElementValue is VirtualKpiElement).Any())
            {
                currentReport.SlicerElements.List.ForEach(i =>
                {
                    if (i.ElementValue is VirtualKpiElement)
                        currentReport.VirtualKpiElements.Add(new Item
                        {
                            Axis = axisElementBuiler.Axis,
                            ElementValue = i.ElementValue,
                            ExcludedElementValue = i.ExcludedElementValue,
                            IsFilterOrSortOn = i.IsFilterOrSortOn,
                            ItemName = i.ItemName
                        });
                });
            }
            if (currentReport.SeriesElements.List.Where(i => i.ElementValue is VirtualKpiElement).Any())
            {
                currentReport.SeriesElements.List.ForEach(i =>
                {
                    if (i.ElementValue is VirtualKpiElement)
                        currentReport.VirtualKpiElements.Add(new Item
                        {
                            Axis = axisElementBuiler.Axis,
                            ElementValue = i.ElementValue,
                            ExcludedElementValue = i.ExcludedElementValue,
                            IsFilterOrSortOn = i.IsFilterOrSortOn,
                            ItemName = i.ItemName
                        });
                });
            }
        }

        private void OlapDataManager_ReportChanged(object sender, ReportChangedEventArgs e)
        {
            OlapDataManager olapDataManager = sender as OlapDataManager;
            RefreshAxisElements(olapDataManager);
        }

        private void RefreshAxisElements(OlapDataManager olapDataManager)
        {
            OlapReport currentReport = ((olapDataManager as OlapDataManager).ActiveReport != null && (olapDataManager as OlapDataManager).UseSharedDataManager) ? olapDataManager.ActiveReport
                    : olapDataManager.CurrentReport;
            if (this.Axis == AxisPosition.Categorical)
            {
                if (currentReport.TogglePivot)
                    this.LoadElements(this, currentReport.SeriesElements);
                else
                    this.LoadElements(this, currentReport.CategoricalElements);
            }
            else if (this.Axis == AxisPosition.Series)
            {
                if (currentReport.TogglePivot)
                    this.LoadElements(this, currentReport.CategoricalElements);
                else
                    this.LoadElements(this, currentReport.SeriesElements);
            }
            else if (this.Axis == AxisPosition.Slicer)
            {
                this.LoadElements(this, currentReport.SlicerElements);
            }
        }

        void OlapDataManager_AxisElementChanged(object sender, AxisElementChangedEventArgs e)
        {
            OlapDataManager olapDataManager = sender as OlapDataManager;
            RefreshAxisElements(olapDataManager);
        }

        void OlapDataManager_AxisElementModified(object sender, AxisElementModifiedEventArgs e)
        {
            OlapDataManager olapDataManager = sender as OlapDataManager;
            RefreshAxisElements(olapDataManager);
        }

        void AxisElementBuilder_ActiveReportChanged(object sender, ActiveReportChangedEventArgs e)
        {
            OlapDataManager olapDataManager = sender as OlapDataManager;
            olapDataManager.ActiveReport = e.NewActiveReport;
            RefreshAxisElements(olapDataManager);
        }

        #endregion

    }
}

/// <summary>
/// Get's the list of selected measures
/// </summary>
/// <param name="metaTreeNode"></param>
/// <returns></returns>
//private MeasureCollection GetSelectedMeasures(MetaTreeNode metaTreeNode)
//{
//    MeasureCollection measureCollection = new MeasureCollection();
//    foreach (MetaTreeNode mtNode in metaTreeNode.ChildNodes)
//    {
//        if (mtNode.NodeType == MetaTreeNodeType.DisplayFolder)
//        {
//            //// Recursive call to get the selected measures
//            MeasureCollection childNodes = GetSelectedMeasures(mtNode);
//            foreach (Measure childMeasure in childNodes)
//            {
//                measureCollection.Add(childMeasure);
//            }
//        }
//        else if (mtNode.NodeType == MetaTreeNodeType.Measure && mtNode.NodeCheckedType == MetaTreeNodeCheckedType.CurrentChecked)
//        {
//            measureCollection.Add((Measure)mtNode.Properties[0].Value);
//        }
//    }
//    return measureCollection;
//}

///// <summary>
///// Gets the name of the axis builder element.
///// </summary>
///// <param name="axisPosition">The axis position.</param>
///// <returns>Returns the Element Name of Type string</returns>
//private static string GetAxisBuilderElementName(AxisPosition axisPosition)
//{
//    if (axisPosition == AxisPosition.Categorical)
//    {
//        return PropertyConstants.AxisElementBuilderCategorical;
//    }
//    else if (axisPosition == AxisPosition.Series)
//    {
//        return PropertyConstants.AxisElementBuilderSeries;
//    }
//    else if (axisPosition == AxisPosition.Slicer)
//    {
//        return PropertyConstants.AxisElementBuilderSlicer;
//    }

//    return string.Empty;
//}


///// <summary>
///// Updates the measure set is selected.
///// </summary>
///// <param name="isSelected">if set to <c>true</c> [is selected].</param>
///// <param name="currentMetaTreeNode">The current meta tree node.</param>
///// <param name="parentMeasureNode">The parent measure node.</param>
///// <returns>Returns the type of bool</returns>
//private bool UpdateMeasureSetIsSelected(bool isSelected, MetaTreeNode currentMetaTreeNode, MetaTreeNode parentMeasureNode)
//{
//    if (parentMeasureNode.UniqueName == currentMetaTreeNode.UniqueName)
//    {
//        //currentMetaTreeNode.SetIsChecked(true, false, true);
//        //currentMetaTreeNode.ForceAcceptChanges();
//        //currentMetaTreeNode.AcceptParentNodeChanges();
//        return true;
//    }

//    foreach (MetaTreeNode metaTreeNode in parentMeasureNode.ChildNodes)
//    {
//        if (metaTreeNode.UniqueName == currentMetaTreeNode.UniqueName)
//        {
//            //metaTreeNode.SetIsChecked(true, false, true);
//            //metaTreeNode.ForceAcceptChanges();
//            //metaTreeNode.AcceptParentNodeChanges();
//            return true;
//        }
//        else if (metaTreeNode.NodeType == MetaTreeNodeType.DisplayFolder)
//        {
//            if (metaTreeNode.ChildNodes.Count > 0)
//            {
//                bool isUpdated = this.UpdateMeasureSetIsSelected(isSelected, currentMetaTreeNode, metaTreeNode);
//                if (isUpdated)
//                {
//                    //metaTreeNode.SetIsChecked(null, false, true);
//                    //metaTreeNode.ForceAcceptChanges();
//                    //metaTreeNode.AcceptParentNodeChanges();
//                    return true;
//                }
//            }
//        }
//    }

//    return false;
//}

///// <summary>
///// Updates the KPI set is selected.
///// </summary>
///// <param name="IsSelected">if set to <c>true</c> [is selected].</param>
///// <param name="metaTreeCurrentNode">The meta tree current node.</param>
///// <param name="metaTreeKPI">The meta tree KPI.</param>
///// <returns>
///// returns bools
///// </returns>
//private bool UpdateKPISetIsSelected(string kpiName, string kpiElementName, bool isKPIElementdragged, MetaTreeNode metaTreeKPI)
//{
//    if (metaTreeKPI.NodeType == MetaTreeNodeType.DisplayFolder
//        || metaTreeKPI.NodeType == MetaTreeNodeType.KPI_ROOT)
//    {
//        if (metaTreeKPI.ChildNodes.Count > 0)
//        {
//            foreach (MetaTreeNode metaTreeNode in metaTreeKPI.ChildNodes)
//            {
//                bool isUpdated = this.UpdateKPISetIsSelected(kpiName, kpiElementName, isKPIElementdragged, metaTreeNode);
//                if (isUpdated)
//                {
//                    return true;
//                }
//            }
//        }
//    }
//    else
//    {
//        if (metaTreeKPI.NodeType == MetaTreeNodeType.KPI)
//        {
//            if (metaTreeKPI.Name == kpiName)
//            {
//                foreach (MetaTreeNode metaTreeNode in metaTreeKPI.ChildNodes)
//                {
//                    if ((isKPIElementdragged) && (metaTreeNode.IsSelected == true || metaTreeNode.Name == kpiElementName))
//                    {
//                        //metaTreeNode.SetIsChecked(true, false, true);
//                        //metaTreeNode.ForceAcceptChanges();
//                    }
//                    else if (!isKPIElementdragged)
//                    {
//                        //metaTreeNode.SetIsChecked(true, false, true);
//                        //metaTreeNode.ForceAcceptChanges();
//                    }
//                }

//                //metaTreeKPI.SetIsChecked(null, false, true);
//                //metaTreeKPI.ForceAcceptChanges();
//                //metaTreeKPI.AcceptParentNodeChanges();
//                return true;
//            }
//            else
//            {
//                foreach (MetaTreeNode _mtNode in metaTreeKPI.ChildNodes)
//                {
//                    if (_mtNode.Name == kpiName)
//                    {
//                        //_mtNode.SetIsChecked(true, false, true);
//                        //_mtNode.ForceAcceptChanges();
//                        //_mtNode.AcceptParentNodeChanges();
//                        return true;
//                    }
//                }
//            }
//        }

//    }

//    return false;
//}


///// <summary>
///// Handles the DrillDown event of the OlapDataManager control.
///// </summary>
///// <param name="sender">The source of the event.</param>
///// <param name="e">The <see cref="Syncfusion.Olap.OlapDataManager.DrillDownEventArgs"/> instance containing the event data.</param>
//private void OlapDataManager_DrillDown(object sender, DrillDownEventArgs e)
//{
//    OlapDataManager cubeOlapDataManager = sender as OlapDataManager;
//    if (e.IsValidAxisPosition)
//    {
//        // Loading the drill down values
//        if (this.Axis == e.AxisPosition)
//        {
//            if (e.AxisPosition == AxisPosition.Categorical)
//            {
//                this.LoadElements(this, cubeOlapDataManager.CurrentReport.CategoricalElements);
//            }
//            else if (e.AxisPosition == AxisPosition.Series)
//            {
//                this.LoadElements(this, cubeOlapDataManager.CurrentReport.SeriesElements);
//            }
//            else if (e.AxisPosition == AxisPosition.Slicer)
//            {
//                this.LoadElements(this, cubeOlapDataManager.CurrentReport.SlicerElements);
//            }
//        }
//    }
//    else
//    {
//        // Initial Loading the values
//        if (this.Axis == AxisPosition.Categorical)
//        {
//            this.LoadElements(this, cubeOlapDataManager.CurrentReport.CategoricalElements);
//        }

//        if (this.Axis == AxisPosition.Series)
//        {
//            this.LoadElements(this, cubeOlapDataManager.CurrentReport.SeriesElements);
//        }

//        if (this.Axis == AxisPosition.Slicer)
//        {
//            this.LoadElements(this, cubeOlapDataManager.CurrentReport.SlicerElements);
//        }
//    }
//}

/// <summary>
/// Removes if same dimension element exist.
/// </summary>
/// <param name="metaTreeNodeCollection">The meta tree node collection.</param>
/// <param name="mtNode">The mt node.</param>
/// <returns>returns bool</returns>
//private bool RemoveIfSameDimensionElementExist(ObservableCollection<MetaTreeNode> metaTreeNodeCollection, MetaTreeNode mtNode)
//{
//    bool isRemoved = false;
//    string[] levels = mtNode.UniqueName.Split('.');
//    for (int i = 0; i < metaTreeNodeCollection.Count; i++)
//    {
//        MetaTreeNode metaTreeNodeItem = metaTreeNodeCollection[i];
//        string[] temp_level = metaTreeNodeItem.UniqueName.Split('.');
//        if (temp_level[0] == levels[0])
//        {
//            metaTreeNodeCollection.Remove(metaTreeNodeItem);
//            isRemoved = true;
//            i--;
//        }
//    }

//    return isRemoved;
//}

//private void FillMeasures(MetaTreeNode metaTreeNodeMeasuresSource, MetaTreeNode selectedNode, MeasureElement measureElement)
//{
//    foreach (MetaTreeNode childNode in metaTreeNodeMeasuresSource.ChildNodes)
//    {
//        if (childNode.UniqueName.ToLower() == measureElement.UniqueName.ToLower())
//        {
//            childNode.IsSelected = true;
//            childNode.AcceptIsSelectedChanges();
//            selectedNode.ChildNodes.Add(childNode);
//            return true;
//        }
//        else
//        {
//            if (childNode.ChildNodes.Count > 0)
//            {
//                bool result = this.FillMeasures(childNode, selectedNode, measureElement);
//                if (result)
//                {
//                    //metaTreeNodeChildMeasures.IsSelected = null;
//                    return true;
//                }
//            }
//        }
//    }

//    return false;
//}

//private void FillMembers(MetaTreeNode metaTreeNodeMembers, MemberElement memberElement)
//{
//    foreach (MemberElement __memberElement in memberElement.ChildMemberElements)
//    {
//        var __metaTreeNodeMember = metaTreeNodeMembers.ChildNodes.Where(m => m.UniqueName.ToLower() == __memberElement.UniqueName.ToLower());
//        if (__metaTreeNodeMember.Count() > 0)
//        {
//            MetaTreeNode temp__metaTreeNodeMember = __metaTreeNodeMember.First() as MetaTreeNode;
//            if (__memberElement.ChildMemberElements.Count > 0)
//            {
//                //temp__metaTreeNodeMember.IsSelected = null;
//                //temp__metaTreeNodeMember.__IsSelected = null;
//                temp__metaTreeNodeMember.NodeCheckedType = MetaTreeNodeCheckedType.SomeChildChecked;
//                this.FillMembers(temp__metaTreeNodeMember, __memberElement);
//            }
//            else
//            {
//                //temp__metaTreeNodeMember.IsSelected = true;
//                //temp__metaTreeNodeMember.__IsSelected = true;
//                temp__metaTreeNodeMember.NodeCheckedType = MetaTreeNodeCheckedType.CurrentChecked;
//            }
//            if (temp__metaTreeNodeMember.IsSelected == true)
//            {
//                temp__metaTreeNodeMember.NodeCheckedType = MetaTreeNodeCheckedType.CurrentChecked;
//            }
//            else if (temp__metaTreeNodeMember.IsSelected == null)
//            {
//                temp__metaTreeNodeMember.NodeCheckedType = MetaTreeNodeCheckedType.SomeChildChecked;
//            }
//            else
//            {
//                temp__metaTreeNodeMember.NodeCheckedType = MetaTreeNodeCheckedType.NoneSelected;
//            }
//        }
//        else
//        {
//            MetaTreeHelper.ForceFillChildNodes(metaTreeNodeMembers);
//            //// Adding a fake Member element for making recursive call
//            MemberElement __temp_memberElement = new MemberElement();
//            __temp_memberElement.ChildMemberElements.Add(__memberElement);
//            this.FillMembers(metaTreeNodeMembers, __temp_memberElement);
//            //metaTreeNodeMembers.IsSelected = null;
//            //metaTreeNodeMembers.__IsSelected = null;
//            metaTreeNodeMembers.NodeCheckedType = MetaTreeNodeCheckedType.SomeChildChecked;
//        }
//    }
//}

//private bool FillKPIs(MetaTreeNode metaTreeNodeKPIs, KpiElement kpiElement)
//{
//    foreach (MetaTreeNode metaTreeNodeChildKPIs in metaTreeNodeKPIs.ChildNodes)
//    {
//        if (metaTreeNodeChildKPIs.UniqueName == kpiElement.Name)
//        {
//            metaTreeNodeChildKPIs.IsSelected = null;
//            //metaTreeNodeChildKPIs.__IsSelected = null;
//            metaTreeNodeChildKPIs.NodeCheckedType = MetaTreeNodeCheckedType.SomeChildChecked;
//            foreach (MetaTreeNode mtNode in metaTreeNodeChildKPIs.ChildNodes)
//            {
//                if (mtNode.NodeType == MetaTreeNodeType.KPI_Goal)
//                {
//                    //mtNode.IsSelected = kpiElement.ShowKPIGoal;
//                    //mtNode.__IsSelected = kpiElement.ShowKPIGoal;
//                    mtNode.NodeCheckedType = MetaTreeNodeCheckedType.CurrentChecked;
//                }
//                else if (mtNode.NodeType == MetaTreeNodeType.KPI_Status)
//                {
//                    //mtNode.IsSelected = kpiElement.ShowKPIStatus;
//                    //mtNode.__IsSelected = kpiElement.ShowKPIStatus;
//                    mtNode.NodeCheckedType = MetaTreeNodeCheckedType.CurrentChecked;
//                }
//                else if (mtNode.NodeType == MetaTreeNodeType.KPI_Trend)
//                {
//                    //mtNode.IsSelected = kpiElement.ShowKPITrend;
//                    //mtNode.__IsSelected = kpiElement.ShowKPITrend;
//                    mtNode.NodeCheckedType = MetaTreeNodeCheckedType.CurrentChecked;
//                }
//                else if (mtNode.NodeType == MetaTreeNodeType.KPI_Value)
//                {
//                    //mtNode.IsSelected = kpiElement.ShowKPIValue;
//                    //mtNode.__IsSelected = kpiElement.ShowKPIValue;
//                    mtNode.NodeCheckedType = MetaTreeNodeCheckedType.CurrentChecked;
//                }
//            }
//            return true;
//        }
//        else
//        {
//            if (metaTreeNodeChildKPIs.ChildNodes.Count > 0)
//            {
//                bool result = this.FillKPIs(metaTreeNodeChildKPIs, kpiElement);
//                if (result)
//                {
//                    //metaTreeNodeChildKPIs.IsSelected = null;
//                    //metaTreeNodeChildKPIs.__IsSelected = null;
//                    return true;
//                }
//            }
//        }
//    }

//    return false;
//}

//void OlapDataManager_AxisElementChanged(object sender, AxisElementChangedEventArgs e)
//{
//    OlapDataManager cubeOlapDataManager = sender as OlapDataManager;
//    if (e.IsValidAxisPosition)
//    {
//        // Loading the drill down values
//        if (this.Axis == e.AxisPosition)
//        {
//            if (e.AxisPosition == AxisPosition.Categorical)
//            {
//                this.LoadElements(this, cubeOlapDataManager.CurrentReport.CategoricalElements);
//            }
//            else if (e.AxisPosition == AxisPosition.Series)
//            {
//                this.LoadElements(this, cubeOlapDataManager.CurrentReport.SeriesElements);
//            }
//            else if (e.AxisPosition == AxisPosition.Slicer)
//            {
//                this.LoadElements(this, cubeOlapDataManager.CurrentReport.SlicerElements);
//            }
//        }
//    }
//    else
//    {
//        // Initial Loading the values
//        if (this.Axis == AxisPosition.Categorical)
//        {
//            this.LoadElements(this, cubeOlapDataManager.CurrentReport.CategoricalElements);
//        }

//        if (this.Axis == AxisPosition.Series)
//        {
//            this.LoadElements(this, cubeOlapDataManager.CurrentReport.SeriesElements);
//        }

//        if (this.Axis == AxisPosition.Slicer)
//        {
//            this.LoadElements(this, cubeOlapDataManager.CurrentReport.SlicerElements);
//        }
//    }
//}