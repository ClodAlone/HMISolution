//-------------------------------------------------------------------------------------------------
// <copyright file="MetaTreeHelper.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;


#if !SILVERLIGHT
using Syncfusion.Olap.Reports;

namespace Syncfusion.Olap.Data
{
#else
using Syncfusion.OlapSilverlight.Reports;

namespace Syncfusion.OlapSilverlight.Data
{
#endif
    /// <summary>
    /// Represents the helper methods for MetaTree node objects.
    /// </summary>
    public class MetaTreeHelper
    {
        #region FillNodes

        //// Dimension element collection meta tree creation.
        /// <summary>
        /// Fills the meta tree node.
        /// </summary>
        /// <param name="mtNodeCube">The cube MetaTree node.</param>
        /// <param name="includeMembers">if set to <c>true</c> [include members].</param>
        /// <param name="dimensionCollection">The dimension collection.</param>
        public static void FillMetaTreeNode(MetaTreeNode mtNodeCube, bool includeMembers, DimensionCollection dimensionCollection)
        {
            foreach (Dimension dimensionObj in dimensionCollection)
            {
                MetaTreeNode mtn = new MetaTreeNode();
                mtNodeCube.ChildNodes.Add(mtn);
                //// Overridng the Parent Node form the base implementation
                //// As we need all members Node root parent to be only upto dimension node
                mtn.ParentNode = null;
                FillMetaTreeNode(mtn, includeMembers, dimensionObj);
            }
        }

        //// Measure Element meta tree creation.
        /// <summary>
        /// Fills the meta tree node.
        /// </summary>
        /// <param name="measureElements">The measure elements.</param>
        /// <param name="mtNodeCube">The cube MetaTree node.</param>
        /// <param name="measureCollection">The measure collection.</param>
        /// <param name="isGroupVisible">if set to <c>true</c> [is group visible].</param>
        /// <param name="isOverrideCurrentNode">if set to <c>true</c> [is override current node].</param>
        public static void FillMetaTreeNode(MeasureElements measureElements, MetaTreeNode mtNodeCube, MeasureCollection measureCollection, bool isGroupVisible, bool isOverrideCurrentNode)
        {
            try
            {
                MetaTreeNode metaTreeNode = null;
                if (isOverrideCurrentNode)
                {
                    metaTreeNode = mtNodeCube;
                    metaTreeNode.ChildNodes.Clear();
                    metaTreeNode.Properties.Clear();
                }
                else
                {
                    metaTreeNode = new MetaTreeNode(PropertyConstants.MeasrueNodeName, PropertyConstants.MeasrueNodeName, PropertyConstants.MeasrueNodeName);
                    //metaTreeNode.OlapDataManager = mtNodeCube.OlapDataManager;
                }

                Property property = metaTreeNode.Properties.FindByName(PropertyConstants.MeasureGroupName);
                if (property == null)
                {
                    metaTreeNode.Properties.Add(PropertyConstants.MeasureGroupName, measureCollection);
                }

                metaTreeNode.UniqueName = PropertyConstants.MeasrueNodeName;
                metaTreeNode.NodeType = MetaTreeNodeType.MeasureGroup;

                //// Adding the included measure elements
                for (int i = 0; i < measureElements.Elements.Count; i++)
                {
                    MeasureElement measureElement = measureElements.Elements[i];
                    FillMetaTreeNode(measureCollection, isGroupVisible, metaTreeNode, measureElement);
                }

                //// Adding the excluded measure elements
                for (int i = 0; i < measureElements.ExcludedMeasures.Count; i++)
                {
                    MeasureElement measureElement = measureElements.ExcludedMeasures[i];
                    FillMetaTreeNode(measureCollection, isGroupVisible, metaTreeNode, measureElement);
                }

                if (!isOverrideCurrentNode)
                {
                    mtNodeCube.ChildNodes.Add(metaTreeNode);
                    //// Overridng the Parent Node form the base implementation
                    //// As Measure node parent to be Measure group
                    metaTreeNode.ParentNode = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void FillMetaTreeNode(MeasureCollection measureCollection, bool isGroupVisible, MetaTreeNode metaTreeNode, MeasureElement measureElement)
        {
            foreach (Measure measureObj in measureCollection)
            {
                if (measureObj.UniqueName.ToLower() == measureElement.UniqueName.ToLower())
                {
                    MetaTreeNode mtn = new MetaTreeNode();
                    mtn.Caption = measureObj.Caption;
                    mtn.Description = measureObj.Description;
                    mtn.UniqueName = measureObj.UniqueName;
                    mtn.Name = measureObj.Name;
                    mtn.NodeType = MetaTreeNodeType.Measure;
                    //mtn.OlapDataManager = metaTreeNode.OlapDataManager;
                    mtn.Properties.Add(PropertyConstants.Measure, measureObj);
                    if (measureObj.GroupName.Length > 0 && isGroupVisible)
                    {
                        MetaTreeNode mtNodeGroup = metaTreeNode.ChildNodes[measureObj.GroupName];
                        MetaTreeNode mtNodefolder = metaTreeNode.ChildNodes[measureObj.DisplayFolder];
                        if (mtNodeGroup == null)
                        {
                            mtNodeGroup = new MetaTreeNode(measureObj.GroupName, measureObj.GroupName, measureObj.GroupName);
                            //mtNodeGroup.OlapDataManager = metaTreeNode.OlapDataManager;
                            mtNodeGroup.NodeType = MetaTreeNodeType.DisplayFolder;
                            if (!string.IsNullOrEmpty(measureObj.DisplayFolder))
                            {
                                if (mtNodefolder == null)
                                {
                                    mtNodefolder = new MetaTreeNode(measureObj.DisplayFolder, measureObj.DisplayFolder, measureObj.DisplayFolder);
                                    mtNodefolder.NodeType = MetaTreeNodeType.DisplayFolder;
                                    mtNodeGroup.ChildNodes.Add(mtNodefolder);
                                }
                            }
                            metaTreeNode.ChildNodes.Add(mtNodeGroup);
                        }

                        if (!string.IsNullOrEmpty(measureObj.DisplayFolder))
                            mtNodefolder.ChildNodes.Add(mtn);
                        else
                            mtNodeGroup.ChildNodes.Add(mtn);
                        //mtNodeGroup.ChildNodes.Add(mtn);
                        //// Overring the Parent Node form the base implementation
                        //// since it will add the current parent and the parent node, 
                        //// here we need to measure node as to be the parent node
                        mtn.ParentNode = metaTreeNode;
                    }
                    else
                    {
                        metaTreeNode.ChildNodes.Add(mtn);
                        //// Overridng the Parent Node form the base implementation
                        //// since it will add the current parent and the parent node, 
                        //// here we need measure node as to be the parent node
                        mtn.ParentNode = metaTreeNode;
                    }
                }
            }
        }

        /// <summary>
        /// Fills the Metatree Node
        /// </summary>
        /// <param name="virtualKpiElement">The virtualKpiElement</param>
        /// <param name="metaTreeNodekpis">The Cube MetaTree Node</param>
        /// <param name="items">The items</param>
        /// <param name="isOverideNode">if set to <c>true</c> [is override current node].</param>
        public static void FillMetaTreeNode(VirtualKpiElement virtualKpiElement, MetaTreeNode metaTreeNodekpis, Items items, bool isOverideNode)
        {
            try
            {
                MetaTreeNode metaTreeNode = null;
                if (isOverideNode)
                {
                    metaTreeNode = metaTreeNodekpis;
                    metaTreeNode.ChildNodes.Clear();
                    metaTreeNode.Properties.Clear();
                }
                else
                {
                    metaTreeNode = new MetaTreeNode(PropertyConstants.VirtualKpiHeaderName, PropertyConstants.VirtualKpiHeaderName, PropertyConstants.VirtualKpiHeaderName);
                    metaTreeNode.NodeType = MetaTreeNodeType.VirtualKPIGroup;
                    metaTreeNode.NodeCheckedType = MetaTreeNodeCheckedType.NoneSelected;
                }
                foreach (Item item in items)
                {
                    if (item.ElementValue is VirtualKpiElement)
                    {
                        VirtualKpiElement vkpi = item.ElementValue as VirtualKpiElement;
                        if (vkpi.ElementName == virtualKpiElement.ElementName)
                        {
                            MetaTreeNode vkpiNode = GetVirtualKpiNode(virtualKpiElement, items);
                            metaTreeNodekpis.ChildNodes.Add(vkpiNode);
                            vkpiNode.ParentNode = metaTreeNodekpis;
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
        //// Kpi Element meta tree creation.
        /// <summary>
        /// Fills the meta tree node.
        /// </summary>
        /// <param name="kpiElements">The KPI elements.</param>
        /// <param name="mtNodeCube">The cube MetaTree node.</param>
        /// <param name="kpiCollection">The KPI collection.</param>
        /// <param name="isOverrideCurrentNode">if set to <c>true</c> [is override current node].</param>
        public static void FillMetaTreeNode(KpiElements kpiElements, MetaTreeNode mtNodeCube, KpiCollection kpiCollection, bool isOverrideCurrentNode)
        {
            try
            {
                MetaTreeNode metaTreeNode = null;
                if (isOverrideCurrentNode)
                {
                    metaTreeNode = mtNodeCube;
                    metaTreeNode.ChildNodes.Clear();
                    metaTreeNode.Properties.Clear();
                }
                else
                {
                    metaTreeNode = new MetaTreeNode(PropertyConstants.KPI, PropertyConstants.KPI, PropertyConstants.KPI);
                }

                Property property = metaTreeNode.Properties.FindByName(PropertyConstants.KPI);
                if (property == null)
                {
                    metaTreeNode.Properties.Add(PropertyConstants.KPI, kpiCollection);
                }

                metaTreeNode.UniqueName = PropertyConstants.KPI;
                metaTreeNode.NodeType = MetaTreeNodeType.KPI_ROOT;
                foreach (KpiElement kpiElement in kpiElements.Elements)
                {

                    foreach (Kpi kpiObj in kpiCollection)
                    {
                        if (kpiObj.Name == kpiElement.Name)
                        {
                            MetaTreeNode kpiNode = GetKpiNode(kpiObj, kpiElement);
                            mtNodeCube.ChildNodes.Add(kpiNode);
                            kpiNode.ParentNode = mtNodeCube;
                        }
                        //MetaTreeNode mtn = new MetaTreeNode();
                        //mtn.Caption = kpiObj.Caption;
                        //mtn.Description = kpiObj.Description;
                        //mtn.UniqueName = (kpiObj.UniqueName == string.Empty) ? kpiObj.Name : kpiObj.UniqueName;
                        //mtn.Name = kpiObj.Name;
                        //mtn.NodeType = MetaTreeNodeType.KPI;
                        //mtn.Properties.Add(PropertyConstants.KPI, kpiObj);
                        //FillMetaTreeNode(mtn, kpiObj);
                        //if (FillKPIs(mtn, kpiElement))
                        //{
                        //    //mtn.IsSelected = null;
                        //    //mtn.__IsSelected = null;
                        //    //mtn.NodeCheckedType = MetaTreeNodeCheckedType.SomeChildChecked;
                        //    mtNodeCube.ChildNodes.Add(mtn);
                        //    mtn.ParentNode = mtNodeCube;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Fills the meta tree node.
        /// </summary>
        /// <param name="mtNodeCube">The Cube Meta Tree Node.</param>
        /// <param name="calculatedMembers">The calculated members.</param>
        /// <param name="isOverrideCurrentNode">if set to <c>true</c> [is override current node].</param>
        public static void FillMetaTreeNode(MetaTreeNode mtNodeCube, Items calculatedMembers, bool isOverrideCurrentNode)
        {
            MetaTreeNode mtNodeHeader = null;
            if (isOverrideCurrentNode)
            {
                mtNodeHeader = mtNodeCube;
                mtNodeHeader.NodeCheckedType = MetaTreeNodeCheckedType.SomeChildChecked;
                mtNodeHeader.ChildNodes.Clear();
                mtNodeHeader.Properties.Clear();
            }
            else
            {
                mtNodeHeader = new MetaTreeNode(PropertyConstants.CalculatedMemberHeaderName, PropertyConstants.CalculatedMemberHeaderName, PropertyConstants.CalculatedMemberHeaderName);
                mtNodeHeader.NodeType = MetaTreeNodeType.CalculatedMemberGroup;
                mtNodeHeader.NodeCheckedType = MetaTreeNodeCheckedType.NoneSelected;
            }

            foreach (Item item in calculatedMembers)
            {
                if (item.ElementValue is CalculatedMember)
                {
                    CalculatedMember calcMember = item.ElementValue as CalculatedMember;
                    MetaTreeNode mtNodeChild = new MetaTreeNode(calcMember.Name, calcMember.Name, calcMember.Name);
                    mtNodeChild.UniqueName = calcMember.UniqueName;
                    mtNodeChild.NodeType = MetaTreeNodeType.CalculatedMember;
                    mtNodeChild.Properties.Add(new Property(PropertyConstants.CalculatedMemberNodeName, calcMember));
                    mtNodeChild.UniqueName = calcMember.UniqueName;
                    mtNodeHeader.ChildNodes.Add(mtNodeChild);
                }
            }

            if (!isOverrideCurrentNode)
            {
                mtNodeCube.ChildNodes.Add(mtNodeHeader);
                mtNodeHeader.ParentNode = null;
            }
        }

        /// <summary>
        /// Fills the meta tree node.
        /// </summary>
        /// <param name="mtNodeCube">The Cube Meta Tree Node.</param>
        /// <param name="virtualKpiElements">The Virtual KPI members.</param>
        /// <param name="isOverrideCurrentNode">if set to <c>true</c> [is override current node].</param>
        public static void FillMetaTreeNode(MetaTreeNode mtNodeCube, bool isOverrideCurrentNode, Items virtualKpiElements)
        {
            MetaTreeNode mtNodeHeader = null;
            if (isOverrideCurrentNode)
            {
                mtNodeHeader = mtNodeCube;
                mtNodeHeader.NodeCheckedType = MetaTreeNodeCheckedType.SomeChildChecked;
                mtNodeHeader.ChildNodes.Clear();
                mtNodeHeader.Properties.Clear();
            }
            else
            {
                mtNodeHeader = new MetaTreeNode(PropertyConstants.VirtualKpiHeaderName, PropertyConstants.VirtualKpiHeaderName, PropertyConstants.VirtualKpiHeaderName);
                mtNodeHeader.NodeType = MetaTreeNodeType.VirtualKPIGroup;
                mtNodeHeader.NodeCheckedType = MetaTreeNodeCheckedType.NoneSelected;
            }

            foreach (Item item in virtualKpiElements)
            {
                if (item.ElementValue is VirtualKpiElement)
                {
                    VirtualKpiElement virtualKpiElement = item.ElementValue as VirtualKpiElement;
                    mtNodeHeader.ChildNodes.Add(GetVirtualKpiNode(virtualKpiElement, virtualKpiElements));
                }
            }

            if (!isOverrideCurrentNode)
            {
                mtNodeCube.ChildNodes.Add(mtNodeHeader);
                mtNodeHeader.ParentNode = null;
            }
        }



        private static MetaTreeNode GetVirtualKpiNode(VirtualKpiElement virtualKpiElement, Items items)
        {
            MetaTreeNode metaTreeNodeKpi = new MetaTreeNode(virtualKpiElement.Name, virtualKpiElement.Name, virtualKpiElement.Name);
            metaTreeNodeKpi.UniqueName = virtualKpiElement.UniqueName;
            metaTreeNodeKpi.NodeType = MetaTreeNodeType.VirtualKPIMember;
            metaTreeNodeKpi.Properties.Add(new Property(PropertyConstants.VirtualKpiNodeName, virtualKpiElement));

            MetaTreeNode valueNode = GetVirtualKpiNode(PropertyConstants.Value, metaTreeNodeKpi, virtualKpiElement);
            MetaTreeNode goalNode = GetVirtualKpiNode(PropertyConstants.Goal, metaTreeNodeKpi, virtualKpiElement);
            MetaTreeNode statusNode = GetVirtualKpiNode(PropertyConstants.Status, metaTreeNodeKpi, virtualKpiElement);
            MetaTreeNode trendNode = GetVirtualKpiNode(PropertyConstants.Trend, metaTreeNodeKpi, virtualKpiElement);
            if (virtualKpiElement.KpiValueExpression != null)
            {
                SetNodeStatus(valueNode, virtualKpiElement.ShowVirtualKPIValue);
                metaTreeNodeKpi.ChildNodes.Add(valueNode);
            }

            if (virtualKpiElement.KpiGoalExpression != null)
            {
                SetNodeStatus(goalNode, virtualKpiElement.ShowVirtualKPIGoal);
                metaTreeNodeKpi.ChildNodes.Add(goalNode);
            }

            if (virtualKpiElement.KpiStatusExpression != null)
            {
                SetNodeStatus(statusNode, virtualKpiElement.ShowVirtualKPIStatus);
                metaTreeNodeKpi.ChildNodes.Add(statusNode);
            }

            if (virtualKpiElement.KpiTrendExpression != null)
            {
                SetNodeStatus(trendNode, virtualKpiElement.ShowVirtualKPITrend);
                metaTreeNodeKpi.ChildNodes.Add(trendNode);
            }

            var checkState = valueNode.CheckedState.ToString() + goalNode.CheckedState.ToString() + statusNode.CheckedState.ToString() + trendNode.CheckedState.ToString();
            if (checkState.Contains("32") || checkState.Contains("23"))
                metaTreeNodeKpi.IsSelected = null;

            metaTreeNodeKpi.AcceptIsSelectedChanges(false);

            return metaTreeNodeKpi;
        }

        private static MetaTreeNode GetVirtualKpiNode(string type, MetaTreeNode mtNodeChild, VirtualKpiElement virtualKpiElement)
        {
            MetaTreeNode mtnode = new MetaTreeNode
            {
                Name = type,
                Caption = type,
                Description = type,
                UniqueName = type
            };
            mtnode.ParentNode = mtNodeChild;
            switch (type)
            {
                case PropertyConstants.Value:
                    {
                        mtnode.NodeType = MetaTreeNodeType.VirtualKPI_Value;
                        mtnode.Properties.Add(PropertyConstants.VirtualKPI_Value, virtualKpiElement);
                        break;
                    }
                case PropertyConstants.Goal:
                    {
                        mtnode.NodeType = MetaTreeNodeType.VirtualKPI_Goal;
                        mtnode.Properties.Add(PropertyConstants.VirtualKPI_Goal, virtualKpiElement);
                        break;
                    }
                case PropertyConstants.Status:
                    {
                        mtnode.NodeType = MetaTreeNodeType.VirtualKPI_Status;
                        mtnode.Properties.Add(PropertyConstants.VirtualKPI_Status, virtualKpiElement);
                        break;
                    }
                case PropertyConstants.Trend:
                    {
                        mtnode.NodeType = MetaTreeNodeType.VirtualKPI_Trend;
                        mtnode.Properties.Add(PropertyConstants.VirtualKPI_Trend, virtualKpiElement);
                        break;
                    }
            }
            return mtnode;
        }

        ///For getting translated caption for Measure Group name
        /// <summary>
        /// Fills the meta tree node.
        /// </summary>
        /// <param name="mtNodeCube">The cube MetaTree node.</param>
        /// <param name="measureCollection">The measure collection.</param>
        /// <param name="isGroupVisible">[is group visible].</param>
        /// <param name="isOverrideCurrentNode">[is override current node]</param>
        /// <param name="sortOrder">used to change the order</param>
        /// <param name="groupCaptions">used get the caption for MeasureGroup</param>
        public static void FillMetaTreeNode(MetaTreeNode mtNodeCube, MeasureCollection measureCollection, bool isGroupVisible, bool isOverrideCurrentNode, SortCubeMeasureOrder sortOrder, Dictionary<string,string> groupCaptions)
        {
            try
            {
                MetaTreeNode metaTreeNode = null;
                List<Measure> measureCollectionList = null;
                if (isOverrideCurrentNode)
                {
                    metaTreeNode = mtNodeCube;
                    metaTreeNode.ChildNodes.Clear();
                    metaTreeNode.Properties.Clear();
                }
                else
                {
                    metaTreeNode = new MetaTreeNode(PropertyConstants.MeasrueNodeName, PropertyConstants.MeasrueNodeName, PropertyConstants.MeasrueNodeName);
                    //metaTreeNode.OlapDataManager = mtNodeCube.OlapDataManager;
                }

                Property property = metaTreeNode.Properties.FindByName(PropertyConstants.MeasureGroupName);
                if (property == null)
                {
                    metaTreeNode.Properties.Add(PropertyConstants.MeasureGroupName, measureCollection);
                }

                metaTreeNode.UniqueName = PropertyConstants.MeasrueNodeName;
                metaTreeNode.NodeType = MetaTreeNodeType.MeasureGroup;
                if (sortOrder == SortCubeMeasureOrder.Default)
                    measureCollectionList = measureCollection.ToList();
                else if (sortOrder == SortCubeMeasureOrder.DSC)
                    measureCollectionList = measureCollection.OrderByDescending(m => m.Caption).ToList();
                else 
                    measureCollectionList = measureCollection.OrderBy(m => m.Caption).ToList();
                
                foreach (Measure measureObj in measureCollectionList)
                {
                    if (measureObj.Visible)
                    {
                        MetaTreeNode mtn = new MetaTreeNode();
                        mtn.Caption = measureObj.Caption;
                        mtn.Description = measureObj.Description;
                        mtn.UniqueName = measureObj.UniqueName;
                        mtn.Name = measureObj.Name;
                        mtn.NodeType = MetaTreeNodeType.Measure;
                        mtn.Properties.Add(PropertyConstants.Measure, measureObj);
                        if (measureObj.GroupName.Length > 0 && isGroupVisible)
                        {
                            MetaTreeNode mtNodeGroup = metaTreeNode.ChildNodes[measureObj.GroupName];
                            if (mtNodeGroup == null)
                            {
                                 string caption;
                                 if (groupCaptions == null)
                                 {
                                     mtNodeGroup = new MetaTreeNode(measureObj.GroupName, measureObj.GroupName, measureObj.GroupName);
                                 }
                                 else
                                 {
                                     if (groupCaptions.TryGetValue(measureObj.GroupName, out caption))
                                     {
                                         if (caption != null)
                                             mtNodeGroup = new MetaTreeNode(measureObj.GroupName, caption, measureObj.GroupName);
                                         else
                                             //To avoid displaying (Blank) when caption is null
                                             mtNodeGroup = new MetaTreeNode(measureObj.GroupName, measureObj.GroupName, measureObj.GroupName);
                                     }
                                     else
                                         mtNodeGroup = new MetaTreeNode(measureObj.GroupName, measureObj.GroupName, measureObj.GroupName);
                                 }
                                mtNodeGroup.NodeType = MetaTreeNodeType.DisplayFolder;
                                if (!string.IsNullOrEmpty(measureObj.DisplayFolder))
                                {
                                    AddChildNode(measureObj.DisplayFolder, mtNodeGroup, mtn);
                                }
                                metaTreeNode.ChildNodes.Add(mtNodeGroup);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(measureObj.DisplayFolder))
                                    AddChildNode(measureObj.DisplayFolder, mtNodeGroup, mtn);
                            }

                            if (string.IsNullOrEmpty(measureObj.DisplayFolder))
                                mtNodeGroup.ChildNodes.Add(mtn);
                            //// Overring the Parent Node form the base implementation
                            //// since it will add the current parent and the parent node, 
                            //// here we need to measure node as to be the parent node
                            mtn.ParentNode = metaTreeNode;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(measureObj.DisplayFolder))
                            {
                                AddChildNode(measureObj.DisplayFolder, metaTreeNode, mtn);
                            }
                            else
                            {
                                metaTreeNode.ChildNodes.Add(mtn);
                                //// Overridng the Parent Node form the base implementation
                                //// since it will add the current parent and the parent node, 
                                //// here we need measure node as to be the parent node
                                mtn.ParentNode = metaTreeNode;
                            }
                        }
                    }
                }
                if (sortOrder == SortCubeMeasureOrder.ASC)
                {
                    var ascNodes = metaTreeNode.ChildNodes.OrderBy(m => m.Caption).Where(m => m.NodeType == MetaTreeNodeType.DisplayFolder).ToList();
                    for (int i = 0; i < ascNodes.Count(); i++)
                        metaTreeNode.ChildNodes[i] = ascNodes[i];
                }

                else if (sortOrder == SortCubeMeasureOrder.DSC)
                {
                    var dscNodes = metaTreeNode.ChildNodes.OrderByDescending(m => m.Caption).Where(m => m.NodeType == MetaTreeNodeType.DisplayFolder).ToList();
                    for (int i = 0; i < dscNodes.Count(); i++)
                        metaTreeNode.ChildNodes[i] = dscNodes[i];
                }

                if (!isOverrideCurrentNode)
                {
                    mtNodeCube.ChildNodes.Add(metaTreeNode);
                    //// Overridng the Parent Node form the base implementation
                    //// As Measure node parent to be Measure group
                    metaTreeNode.ParentNode = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //// Measure Element collection meta tree creation.
        /// <summary>
        /// Fills the meta tree node.
        /// </summary>
        /// <param name="mtNodeCube">The cube MetaTree node.</param>
        /// <param name="measureCollection">The measure collection.</param>
        /// <param name="isGroupVisible">if set to <c>true</c> [is group visible].</param>
        /// <param name="isOverrideCurrentNode">if set to <c>true</c> [is override current node].</param>
        public static void FillMetaTreeNode(MetaTreeNode mtNodeCube, MeasureCollection measureCollection, bool isGroupVisible, bool isOverrideCurrentNode)
        {
            FillMetaTreeNode(mtNodeCube, measureCollection, isGroupVisible, isOverrideCurrentNode, SortCubeMeasureOrder.ASC); 
        }

        //// Measure Element collection meta tree creation with ordering support.
        /// <summary>
        ///  Fills the meta tree node.
        /// </summary>
        /// <param name="mtNodeCube">The cube MetaTree node.</param>
        /// <param name="measureCollection">The measure collection.</param>
        /// <param name="isGroupVisible">[is group visible].</param>
        /// <param name="isOverrideCurrentNode"> [is override current node]</param>
        /// <param name="sortOrder">used to change the order</param>
        public static void FillMetaTreeNode(MetaTreeNode mtNodeCube, MeasureCollection measureCollection, bool isGroupVisible, bool isOverrideCurrentNode, SortCubeMeasureOrder sortOrder)
        {
            FillMetaTreeNode(mtNodeCube, measureCollection, isGroupVisible, isOverrideCurrentNode, SortCubeMeasureOrder.ASC, null);
        }

        private static void AddChildNode(string displayFolder, MetaTreeNode mtNodeGroup, MetaTreeNode mtn)
        {
            MetaTreeNode localmtFolder = null;
            if (!string.IsNullOrEmpty(displayFolder))
            {
                foreach (var item in displayFolder.Split('\\'))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        localmtFolder = new MetaTreeNode(item, item, item);
                        localmtFolder.NodeType = MetaTreeNodeType.DisplayFolder;
                        var count = mtNodeGroup.ChildNodes.Where(s => s.Name == item).Count();
                        if (count == 0)
                        {
                            mtNodeGroup.ChildNodes.Add(localmtFolder);
                            if (displayFolder.Contains('\\'))
                            {
                                string localstringValue = displayFolder.Substring(displayFolder.IndexOf('\\'));
                                localstringValue = localstringValue.Substring(localstringValue.IndexOf('\\') + 1);
                                AddChildNode(localstringValue, localmtFolder, mtn);
                            }
                            else
                                localmtFolder.ChildNodes.Add(mtn);
                        }
                        else
                        {
                            if (displayFolder.Contains('\\'))
                            {
                                string localstringValue = displayFolder.Substring(displayFolder.IndexOf('\\'));
                                localstringValue = localstringValue.Substring(localstringValue.IndexOf('\\') + 1);
                                AddChildNode(localstringValue, mtNodeGroup.ChildNodes[item], mtn);
                            }
                            else
                                mtNodeGroup.ChildNodes[item].ChildNodes.Add(mtn);
                        }

                        break;
                    }
                }
            }
        }


        //// Kpi Element collection meta tree creation.
        /// <summary>
        /// Fills the meta tree node.
        /// </summary>
        /// <param name="mtNodeCube">The cube MetaTree node.</param>
        /// <param name="kpiCollection">The KPI collection.</param>
        /// <param name="isGroupVisible">if set to <c>true</c> [is group visible].</param>
        /// <param name="isOverrideCurrentNode">if set to <c>true</c> [is override current node].</param>
        public static void FillMetaTreeNode(MetaTreeNode mtNodeCube, KpiCollection kpiCollection, bool isGroupVisible, bool isOverrideCurrentNode)
        {
            try
            {
                MetaTreeNode metaTreeNode = null;
                if (isOverrideCurrentNode)
                {
                    metaTreeNode = mtNodeCube;
                    metaTreeNode.ChildNodes.Clear();
                    metaTreeNode.Properties.Clear();
                }
                else
                {
                    metaTreeNode = new MetaTreeNode(PropertyConstants.KPI, PropertyConstants.KPI, PropertyConstants.KPI);
                }
                Property property = metaTreeNode.Properties.FindByName(PropertyConstants.KPI);
                if (property == null)
                {
                    metaTreeNode.Properties.Add(PropertyConstants.KPI, kpiCollection);
                }
                metaTreeNode.UniqueName = PropertyConstants.KPI;
                metaTreeNode.NodeType = MetaTreeNodeType.KPI_ROOT;
                foreach (Kpi kpiObj in kpiCollection)
                {
                    MetaTreeNode mtn = new MetaTreeNode();
                    mtn.Caption = kpiObj.Caption;
                    mtn.Description = kpiObj.Description;
                    mtn.UniqueName = (kpiObj.UniqueName == string.Empty) ? kpiObj.Name : kpiObj.UniqueName;
                    mtn.Name = kpiObj.Name;
                    mtn.NodeType = MetaTreeNodeType.KPI;
                    mtn.Properties.Add(PropertyConstants.KPI, kpiObj);
                    AddKpiAttributes(mtn, kpiObj);
                    string[] displayFolder = kpiObj.DisplayFolder.Split('\\');
                    int j = displayFolder.Count();//GetLevelOfKPI(kpiObj.DisplayFolder);
                    if (kpiObj.DisplayFolder == string.Empty)
                        j = 0;
                    if (j > 0)
                    {
                        MetaTreeNode mtNodeGroup = metaTreeNode.ChildNodes[displayFolder[0]];
                        if (mtNodeGroup == null)
                        {
                            mtNodeGroup = new MetaTreeNode(displayFolder[0], displayFolder[0], displayFolder[0]);
                            mtNodeGroup.NodeType = MetaTreeNodeType.DisplayFolder;
                            metaTreeNode.ChildNodes.Add(mtNodeGroup);
                        }
                        if (kpiObj.DisplayFolder.Contains('\\'))
                            AddChildNode(kpiObj.DisplayFolder.Substring(kpiObj.DisplayFolder.IndexOf('\\')).Remove(0, 1), mtNodeGroup, mtn);
                        else
                            mtNodeGroup.ChildNodes.Add(mtn);
                    }
                    else if (j == 0)
                    {
                        metaTreeNode.ChildNodes.Add(mtn);
                        mtn.ParentNode = metaTreeNode;
                    }
                    else
                    {
                        string kpiDisplayFolder = kpiObj.DisplayFolder;
                        MetaTreeNode mtNodeGroup;
                        mtNodeGroup = metaTreeNode.ChildNodes[kpiDisplayFolder];
                        if (mtNodeGroup == null)
                        {
                            mtNodeGroup = new MetaTreeNode(kpiDisplayFolder, kpiDisplayFolder, kpiDisplayFolder);
                            mtNodeGroup.NodeType = MetaTreeNodeType.DisplayFolder;
                            metaTreeNode.ChildNodes.Add(mtNodeGroup);
                        }

                        mtNodeGroup.ChildNodes.Add(mtn);
                        mtn.ParentNode = mtNodeGroup;
                    }
                }

                if (!isOverrideCurrentNode)
                {
                    mtNodeCube.ChildNodes.Add(metaTreeNode);
                    //// Overridng the Parent Node form the base implementation
                    //// As Measure node parent to be Measure group
                    metaTreeNode.ParentNode = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //// Dimension meta tree creation.
        /// <summary>
        /// Fills the meta tree node.
        /// </summary>
        /// <param name="mtNodeDimension">The dimension MetaTree node.</param>
        /// <param name="includeMembers">if set to <c>true</c> [include members].</param>
        /// <param name="dimensionObj">The dimension obj.</param>
        public static void FillMetaTreeNode(MetaTreeNode mtNodeDimension, bool includeMembers, Dimension dimensionObj)
        {
            mtNodeDimension.Name = dimensionObj.Name;
            mtNodeDimension.Caption = dimensionObj.Caption;
            mtNodeDimension.UniqueName = dimensionObj.UniqueName;
            mtNodeDimension.Description = dimensionObj.Description;
            mtNodeDimension.NodeType = MetaTreeNodeType.Dimension;
            Property propertyDimension = mtNodeDimension.Properties.FindByName(PropertyConstants.Dimension);
            if (propertyDimension == null)
            {
                mtNodeDimension.Properties.Add(PropertyConstants.Dimension, dimensionObj);
            }
            ////Clearing the Hierarchy objects if exist
            mtNodeDimension.ChildNodes.Clear();
            foreach (Hierarchy hierarchy in dimensionObj.Hierarchies)
            {
                MetaTreeNode mtNodeHierarchy = new MetaTreeNode { Name = hierarchy.Name, UniqueName = hierarchy.UniqueName, Caption = hierarchy.Caption, Description = hierarchy.Description };
                mtNodeHierarchy.NodeType = MetaTreeNodeType.Hierarchy;
                mtNodeHierarchy.Properties.Add(PropertyConstants.Hierarchy, hierarchy);
                if (hierarchy.UniqueName == dimensionObj.DefaultHierarchyName)
                {
                    mtNodeHierarchy.IsSelected = true;
                    if (dimensionObj.Hierarchies.Count > 1)
                    {
                        mtNodeDimension.IsSelected = null;
                    }
                    else
                    {
                        mtNodeDimension.IsSelected = true;
                    }
                }

                if (hierarchy.DisplayFolder.Length > 0)
                {
                    MetaTreeNode mtNodeHierarchyDisplayFolder = mtNodeDimension.ChildNodes[hierarchy.DisplayFolder];
                    if (mtNodeHierarchyDisplayFolder == null)
                    {
                        mtNodeHierarchyDisplayFolder = new MetaTreeNode { Name = hierarchy.DisplayFolder, Caption = hierarchy.DisplayFolder };
                        mtNodeHierarchyDisplayFolder.NodeType = MetaTreeNodeType.DisplayFolder;
                        if (mtNodeHierarchy.IsSelected == true)
                        {
                            mtNodeHierarchyDisplayFolder.IsSelected = null;
                        }

                        mtNodeDimension.ChildNodes.Add(mtNodeHierarchyDisplayFolder);
                    }

                    mtNodeHierarchyDisplayFolder.ChildNodes.Add(mtNodeHierarchy);
                }
                else
                {
                    mtNodeDimension.ChildNodes.Add(mtNodeHierarchy);
                }

                foreach (Level level in hierarchy.Levels)
                {
                    if (level.Name != "(All)")
                    {
                        MetaTreeNode mtNodeLevel = new MetaTreeNode();
                        if (level.UniqueName == hierarchy.DefaultLevelName)
                        {
                            mtNodeLevel.IsSelected = true;
                            if (hierarchy.Levels.Count > 1)
                            {
                                mtNodeLevel.IsSelected = null;
                            }
                            else
                            {
                                mtNodeLevel.IsSelected = true;
                            }
                        }

                        FillMetaTreeNode(mtNodeLevel, level, false, false);
                        mtNodeHierarchy.ChildNodes.Add(mtNodeLevel);
                        if (includeMembers)
                        {
                            ////Adding a Fake Object
                            mtNodeLevel.ChildNodes.Add(new MetaTreeNode());
                        }
                    }
                }
            }
            //// Sorting according to the Hierarchies
            int tempCount = mtNodeDimension.ChildNodes.Count;
            for (int i = 0; i < tempCount; i++)
            {
                MetaTreeNode mtNode = mtNodeDimension.ChildNodes[i];
                if (mtNode.NodeType != MetaTreeNodeType.DisplayFolder)
                {
                    if (mtNode.ChildNodes.Count > 1)
                    {
                        mtNodeDimension.ChildNodes.Remove(mtNode);
                        mtNodeDimension.ChildNodes.Insert(mtNodeDimension.ChildNodes.Count, mtNode);
                        i--;
                        tempCount--;
                    }
                }
            }
        }

        //// Hierarchy meta tree creation.
        /// <summary>
        /// Fills the meta tree node.
        /// </summary>
        /// <param name="mtNodeHierarchy">The hierarchy MetaTree node.</param>
        /// <param name="hierarchy">The hierarchy object.</param>
        public static void FillMetaTreeNode(MetaTreeNode mtNodeHierarchy, Hierarchy hierarchy)
        {
            mtNodeHierarchy.Name = hierarchy.Name;
            mtNodeHierarchy.Caption = hierarchy.Caption;
            mtNodeHierarchy.UniqueName = hierarchy.UniqueName;
            mtNodeHierarchy.Description = hierarchy.Description;
            mtNodeHierarchy.NodeType = MetaTreeNodeType.Hierarchy;
            Property propertyHierarchy = mtNodeHierarchy.Properties.FindByName(PropertyConstants.Hierarchy);
            if (propertyHierarchy == null)
            {
                mtNodeHierarchy.Properties.Add(PropertyConstants.Hierarchy, hierarchy);
            }

            mtNodeHierarchy.ChildNodes.Clear();
            foreach (Level level in hierarchy.Levels)
            {
                if (level.Name != "(All)")
                {
                    MetaTreeNode mtNodeLevel = new MetaTreeNode();
                    if (level.UniqueName == hierarchy.DefaultLevelName)
                    {
                        mtNodeLevel.IsSelected = true;
                        if (hierarchy.Levels.Count > 1)
                        {
                            mtNodeLevel.IsSelected = null;
                        }
                        else
                        {
                            mtNodeLevel.IsSelected = true;
                        }
                    }

                    FillMetaTreeNode(mtNodeLevel, level, false, false);
                    mtNodeHierarchy.ChildNodes.Add(mtNodeLevel);
                    ////Adding a Fake Object
                    mtNodeLevel.ChildNodes.Add(new MetaTreeNode());
                }
            }
        }

        //// Named set Dimension meta tree creation.
        /// <summary>
        /// Fills the meta tree node.
        /// </summary>
        /// <param name="mtNodeDimension">The dimension MetaTree node.</param>
        /// <param name="includeMembers">if set to <c>true</c> [include members].</param>
        /// <param name="dimensionObj">The dimension obj.</param>
        /// <param name="namedSetCollection">The named set collection.</param>
        public static void FillMetaTreeNode(MetaTreeNode mtNodeDimension, bool includeMembers, Dimension dimensionObj, NamedSetCollection namedSetCollection)
        {
            mtNodeDimension.Name = dimensionObj.Name;
            mtNodeDimension.Caption = dimensionObj.Caption;
            mtNodeDimension.UniqueName = dimensionObj.UniqueName;
            mtNodeDimension.Description = dimensionObj.Description;
            mtNodeDimension.NodeType = MetaTreeNodeType.Dimension;
            Property propertyDimension = mtNodeDimension.Properties.FindByName(PropertyConstants.Dimension);
            if (propertyDimension == null)
            {
                mtNodeDimension.Properties.Add(PropertyConstants.Dimension, dimensionObj);
            }
            ////Clearing the Hierarchy objects if exist
            mtNodeDimension.ChildNodes.Clear();
            foreach (Hierarchy hierarchy in dimensionObj.Hierarchies)
            {
                MetaTreeNode mtNodeHierarchy = new MetaTreeNode { Name = hierarchy.Name, UniqueName = hierarchy.UniqueName, Caption = hierarchy.Caption, Description = hierarchy.Description };
                mtNodeHierarchy.NodeType = MetaTreeNodeType.Hierarchy;
                mtNodeHierarchy.Properties.Add(PropertyConstants.Hierarchy, hierarchy);

                if (hierarchy.UniqueName == dimensionObj.DefaultHierarchyName)
                {
                    mtNodeDimension.Properties.Add(PropertyConstants.Hierarchy, hierarchy);
                    mtNodeHierarchy.IsSelected = true;
                    if (dimensionObj.Hierarchies.Count > 1)
                    {
                        mtNodeDimension.IsSelected = null;
                    }
                    else
                    {
                        mtNodeDimension.IsSelected = true;
                    }
                }

                if (hierarchy.DisplayFolder.Length > 0)
                {
                    MetaTreeNode mtNodeHierarchyDisplayFolder = mtNodeDimension.ChildNodes[hierarchy.DisplayFolder];
                    if (mtNodeHierarchyDisplayFolder == null)
                    {
                        mtNodeHierarchyDisplayFolder = new MetaTreeNode { Name = hierarchy.DisplayFolder, Caption = hierarchy.DisplayFolder };
                        mtNodeHierarchyDisplayFolder.NodeType = MetaTreeNodeType.DisplayFolder;
                        if (mtNodeHierarchy.IsSelected == true)
                        {
                            mtNodeHierarchyDisplayFolder.IsSelected = null;
                        }

                        mtNodeDimension.ChildNodes.Add(mtNodeHierarchyDisplayFolder);
                    }

                    mtNodeHierarchyDisplayFolder.ChildNodes.Add(mtNodeHierarchy);
                }
                else
                {
                    mtNodeDimension.ChildNodes.Add(mtNodeHierarchy);
                }

                foreach (NamedSet namedSet in namedSetCollection)
                {
                    if (namedSet.ParentHierarchyName == hierarchy.UniqueName)
                    {
                        MetaTreeNode mtNodeNamedSet = new MetaTreeNode { Name = namedSet.Name, UniqueName = namedSet.UniqueName, Caption = namedSet.Name, Description = namedSet.Description };
                        mtNodeNamedSet.NodeType = MetaTreeNodeType.NamedSet;
                        mtNodeNamedSet.Properties.Add(PropertyConstants.NamedSet, namedSet);
                        if (hierarchy.UniqueName == dimensionObj.DefaultHierarchyName)
                        {
                            mtNodeNamedSet.IsSelected = true;
                            if (dimensionObj.Hierarchies.Count > 1)
                            {
                                mtNodeDimension.IsSelected = null;
                            }
                            else
                            {
                                mtNodeDimension.IsSelected = true;
                            }
                        }

                        MetaTreeNode mtNodeNamedSetDisplayFolder = mtNodeDimension.ChildNodes[PropertyConstants.NamedSetDisplayFolder];
                        if (mtNodeNamedSetDisplayFolder == null)
                        {
                            mtNodeNamedSetDisplayFolder = new MetaTreeNode { Name = namedSet.DisplayFolder, Caption = namedSet.DisplayFolder };
                            mtNodeNamedSetDisplayFolder.NodeType = MetaTreeNodeType.DisplayFolder;
                            if (mtNodeHierarchy.IsSelected == true)
                            {
                                mtNodeNamedSetDisplayFolder.IsSelected = null;
                            }
                        }
                        if (!string.IsNullOrEmpty(namedSet.DisplayFolder))
                            AddChildNode(namedSet.DisplayFolder, mtNodeDimension, mtNodeNamedSet);
                        else
                            mtNodeDimension.ChildNodes.Add(mtNodeNamedSet);
                    }
                }

                foreach (Level level in hierarchy.Levels)
                {
                    //if (level.Name != "(All)" || level.Name != "ALL")
                    if (level.LevelType != LevelTypeEnum.All)
                    {
                        MetaTreeNode mtNodeLevel = new MetaTreeNode();
                        //// UniqueName wont match the DefaultLevelName so it was changed to DefaultLevelUniqueName 
                        ////if (level.UniqueName == hierarchy.DefaultLevelName)
                        if (level.UniqueName == hierarchy.DefaultLevelUniqueName)
                        {
                            mtNodeDimension.Properties.Add(PropertyConstants.Level, level);
                            mtNodeLevel.IsSelected = true;
                            if (hierarchy.Levels.Count > 1)
                            {
                                mtNodeLevel.IsSelected = null;
                            }
                            else
                            {
                                mtNodeLevel.IsSelected = true;
                            }
                        }

                        FillMetaTreeNode(mtNodeLevel, level, false, false);
                        mtNodeHierarchy.ChildNodes.Add(mtNodeLevel);
                        if (includeMembers)
                        {
                            ////Adding a Fake Object
                            mtNodeLevel.ChildNodes.Add(new MetaTreeNode());
                        }
                    }
                }
            }
            //// Sorting according to the Hierarchies
            int tempCount = mtNodeDimension.ChildNodes.Count;
            for (int i = 0; i < tempCount; i++)
            {
                MetaTreeNode mtNode = mtNodeDimension.ChildNodes[i];
                if (mtNode.NodeType != MetaTreeNodeType.DisplayFolder)
                {
                    if (mtNode.ChildNodes.Count > 1)
                    {
                        mtNodeDimension.ChildNodes.Remove(mtNode);
                        mtNodeDimension.ChildNodes.Insert(mtNodeDimension.ChildNodes.Count, mtNode);
                        i--;
                        tempCount--;
                    }
                }
            }
        }

        //// Level meta tree creation.
        /// <summary>
        /// Fills the meta tree node.
        /// </summary>
        /// <param name="metaTreeLevel">The meta tree level.</param>
        /// <param name="levelObj">The level obj.</param>
        /// <param name="overrideOnDemandLoad">if set to <c>true</c> [override on demand load].</param>
        /// <param name="includeChildMembers">if set to <c>true</c> [include child members].</param>
        public static void FillMetaTreeNode(MetaTreeNode metaTreeLevel, Level levelObj, bool overrideOnDemandLoad, bool includeChildMembers)
        {
            metaTreeLevel.Name = levelObj.Name;
            metaTreeLevel.Caption = levelObj.Caption;
            metaTreeLevel.UniqueName = levelObj.UniqueName;
            metaTreeLevel.Description = levelObj.Description;
            metaTreeLevel.NodeType = MetaTreeNodeType.Level;
            metaTreeLevel.LevelDepth = levelObj.LevelDepth;
            Property propertyLevel = metaTreeLevel.Properties.FindByName(PropertyConstants.Level);
            if (propertyLevel == null)
            {
                metaTreeLevel.Properties.Add(PropertyConstants.Level, levelObj);
            }
            //// Improving performance by avoiding unnecessary loading of data
            if (!levelObj.IsMemberLoadedOnDemand || overrideOnDemandLoad)
            {
                if (includeChildMembers)
                {
                    metaTreeLevel.ChildNodes.Clear();
                    int tempCount = 0;
                    foreach (Member member in levelObj.Members)
                    {
                        MetaTreeNode metaTreeNodeMember = new MetaTreeNode { Name = member.Name, UniqueName = member.UniqueName, Caption = member.Caption, Description = member.Description };
                        if (member.UniqueName == levelObj.ParentHierarchy.DefaultMemberUniqueName)
                        {
                            metaTreeNodeMember.IsSelected = true;
                            if (levelObj.MemberCount > 1)
                            {
                                metaTreeLevel.IsSelected = null;
                            }
                            else
                            {
                                metaTreeLevel.IsSelected = true;
                            }
                        }

                        metaTreeNodeMember.NodeType = MetaTreeNodeType.Member;
                        metaTreeNodeMember.Properties.Add(PropertyConstants.Member, member);
                        //// Adding a fake object if this has childrens
                        if (member.HasChildMembers)
                        {
                            metaTreeNodeMember.ChildNodes.Add(new MetaTreeNode());
                        }

                        metaTreeLevel.ChildNodes.Add(metaTreeNodeMember);
                        tempCount++;
                        if (tempCount > 100)
                        {
                            metaTreeLevel.ChildNodes.Add(new MetaTreeNode { Caption = "..." });
                            break;
                        }
                    }
                }
            }
        }

        //// Member meta tree creation.
        /// <summary>
        /// Fills the meta tree node.
        /// </summary>
        /// <param name="metaTreeMember">The meta tree member.</param>
        /// <param name="memberObj">The member obj.</param>
        /// <param name="overrideOnDemandLoad">if set to <c>true</c> [override on demand load].</param>
        public static void FillMetaTreeNode(MetaTreeNode metaTreeMember, Member memberObj, bool overrideOnDemandLoad)
        {
            metaTreeMember.Name = memberObj.Name;
            metaTreeMember.Caption = memberObj.Caption;
            metaTreeMember.UniqueName = memberObj.UniqueName;
            metaTreeMember.Description = memberObj.Description;
            metaTreeMember.NodeType = MetaTreeNodeType.Member;
            Property propertyMember = metaTreeMember.Properties.FindByName(PropertyConstants.Member);
            if (propertyMember == null)
            {
                metaTreeMember.Properties.Add(PropertyConstants.Member, memberObj);
            }
            if (memberObj.HasChildMembers)
            {
                //// Improving performance by avoiding unnecessary loading of data
                if (!memberObj.IsMemberLoadedOnDemand || overrideOnDemandLoad)
                {
                    ////if (includeChildMembers)
                    {
                        metaTreeMember.ChildNodes.Clear();
                        foreach (Member member in memberObj.ChildMembers)
                        {
                            MetaTreeNode metaTreeNodeMember = new MetaTreeNode { Name = "Blank", UniqueName = "Blank", Caption = "Blank" };
                            // metaTreeNodeMember.NodeType = MetaTreeNodeType.None;
                            //// Adding a fake object if this has childrens
                            if (member.HasChildMembers)
                            {
                                metaTreeNodeMember.ChildNodes.Add(new MetaTreeNode());
                            }

                            metaTreeNodeMember.Properties.Add(PropertyConstants.Member, member);
                            metaTreeMember.ChildNodes.Add(metaTreeNodeMember);
                        }
                    }
                }
            }
        }

        //// Member meta tree creation with or without childmembers.
        /// <summary>
        /// Fills the meta tree node.
        /// </summary>
        /// <param name="metaTreeMember">The meta tree member.</param>
        /// <param name="memberObj">The member obj.</param>
        /// <param name="overrideOnDemandLoad">if set to <c>true</c> [override on demand load].</param>
        /// <param name="includeChildMembers">if set to <c>true</c> [include child members].</param>
        public static void FillMetaTreeNode(MetaTreeNode metaTreeMember, Member memberObj, bool overrideOnDemandLoad, bool includeChildMembers)
        {
            metaTreeMember.Name = memberObj.Name;
            metaTreeMember.Caption = memberObj.Caption;
            metaTreeMember.UniqueName = memberObj.UniqueName;
            metaTreeMember.Description = memberObj.Description;
            metaTreeMember.NodeType = MetaTreeNodeType.Member;
            Property propertyMember = metaTreeMember.Properties.FindByName(PropertyConstants.Member);
            if (propertyMember == null)
            {
                metaTreeMember.Properties.Add(PropertyConstants.Member, memberObj);
            }
            metaTreeMember.AcceptIsSelectedChanges(false);
            if (memberObj.HasChildMembers)
            {
#if !SILVERLIGHT
                if (memberObj.ChildMembers != null && memberObj.ChildMembers.Count > 0)
                    metaTreeMember.HasChildMembers = true;
#endif
                //// Improving performance by avoiding unnecessary loading of data
                if (!memberObj.IsMemberLoadedOnDemand || overrideOnDemandLoad)
                {
                    if (includeChildMembers)
                    {
                        metaTreeMember.ChildNodes.Clear();
#if SILVERLIGHT
                        if (memberObj.ChildMembers.Count > 0)
                        {
#endif
                            foreach (Member member in memberObj.ChildMembers)
                            {
                                MetaTreeNode metaTreeNodeMember = new MetaTreeNode { Name = member.Name, UniqueName = member.UniqueName, Caption = member.Caption };
                                metaTreeNodeMember.NodeType = MetaTreeNodeType.Member;
                                //// if the parent node is unselected then upon drilldown child nodes should also be unselected
                                if (metaTreeMember.IsSelected == false)
                                {
                                    metaTreeNodeMember.IsSelected = false;
                                    metaTreeNodeMember.AcceptIsSelectedChanges(false);
                                }
                                else
                                {
                                    metaTreeNodeMember.IsSelected = true;
                                    metaTreeNodeMember.AcceptIsSelectedChanges(false);
                                }
                                //// Adding a fake object if this has childrens 
                                if (member.HasChildMembers)
                                {
                                    metaTreeNodeMember.ChildNodes.Add(new MetaTreeNode());
                                }

                                metaTreeNodeMember.Properties.Add(PropertyConstants.Member, member);
                                metaTreeMember.ChildNodes.Add(metaTreeNodeMember);
                            }
#if SILVERLIGHT
                        }
                        else if (memberObj.HasChildMembers && memberObj.ChildMembers.Count == 0)
                        {
                            metaTreeMember.ChildNodes.Add(new MetaTreeNode());
                        }
#endif
                    }
                }
            }
        }

        public static void FillMetaTreeNode(MetaTreeNode metaTreeMember, Member memberObj, bool overrideOnDemandLoad, bool includeChildMembers, int maxLevel)
        {
            metaTreeMember.Name = memberObj.Name;
            metaTreeMember.Caption = memberObj.Caption;
            metaTreeMember.UniqueName = memberObj.UniqueName;
            metaTreeMember.Description = memberObj.Description;
            metaTreeMember.NodeType = MetaTreeNodeType.Member;
            Property propertyMember = metaTreeMember.Properties.FindByName(PropertyConstants.Member);
            if (propertyMember == null)
            {
                metaTreeMember.Properties.Add(PropertyConstants.Member, memberObj);
            }
            if (memberObj.HasChildMembers)
            {
#if !SILVERLIGHT
                if (memberObj.ChildMembers != null && memberObj.ChildMembers.Count > 0)
                    metaTreeMember.HasChildMembers = true;
#endif
                //// Improving performance by avoiding unnecessary loading of data
                if (!memberObj.IsMemberLoadedOnDemand || overrideOnDemandLoad)
                {
                    if (includeChildMembers)
                    {
                        metaTreeMember.ChildNodes.Clear();
#if SILVERLIGHT
                        if (memberObj.ChildMembers.Count > 0)
                        {
#endif
                            foreach (Member member in memberObj.ChildMembers)
                            {
                                MetaTreeNode metaTreeNodeMember = new MetaTreeNode { Name = member.Name, UniqueName = member.UniqueName, Caption = member.Caption };
                                metaTreeNodeMember.NodeType = MetaTreeNodeType.Member;
                                //// if the parent node is unselected then upon drilldown child nodes should also be unselected
                                if (metaTreeMember.IsSelected == false)
                                {
                                    metaTreeNodeMember.IsSelected = false;
                                    metaTreeNodeMember.AcceptIsSelectedChanges(false);
                                }
                                else
                                {
                                    metaTreeNodeMember.IsSelected = true;
                                    metaTreeNodeMember.AcceptIsSelectedChanges(false);
                                }
                                //// Adding a fake object if this has childrens 
                                if (member.HasChildMembers && member.LevelDepth < maxLevel)
                                {
                                    MetaTreeHelper.FillMetaTreeNode(metaTreeNodeMember, member, true, true, maxLevel);
                                }

                                metaTreeNodeMember.Properties.Add(PropertyConstants.Member, member);
                                metaTreeMember.ChildNodes.Add(metaTreeNodeMember);
                            }
#if SILVERLIGHT
                        }
                        else if (memberObj.HasChildMembers && memberObj.ChildMembers.Count == 0)
                        {
                            metaTreeMember.ChildNodes.Add(new MetaTreeNode());
                        }
#endif
                    }
                }
            }
        }

        //// Identifies the required meta tree type and create it by calling specific overload with require parameters.
        /// <summary>
        /// Fills the meta tree node.
        /// </summary>
        /// <param name="mtNode">The MetaTree node.</param>
        /// <param name="element">The element.</param>
        public static void FillMetaTreeNode(MetaTreeNode mtNode, object element)
        {
            if (element is MeasureCollection)
            {
                FillMetaTreeNode(mtNode, (MeasureCollection)element, false, true);
            }
            else if (element is DimensionCollection)
            {
                FillMetaTreeNode(mtNode, (DimensionCollection)element);
            }
            else if (element is Dimension)
            {
                FillMetaTreeNode(mtNode, (Dimension)element);
            }
            else if (element is Hierarchy)
            {
                FillMetaTreeNode(mtNode, (Hierarchy)element);
            }
            else if (element is Level)
            {
                FillMetaTreeNode(mtNode, (Level)element, true, true);
            }
            else if (element is Member)
            {
                FillMetaTreeNode(mtNode, (Member)element, true, true);
            }
        }

        //// Dimension and Measure element collection. It will be used for creating the meta tree with measures and dimensions eg., CubeDimensionBrowser.
        /// <summary>
        /// Fills the meta tree node.
        /// </summary>
        /// <param name="mtNodeCube">The mt node cube.</param>
        /// <param name="includeMembers">if set to <c>true</c> [include members].</param>
        /// <param name="dimensionCollection">The dimension collection.</param>
        /// <param name="namedSetCollection">The named set collection.</param>
        public static void FillMetaTreeNode(MetaTreeNode mtNodeCube, bool includeMembers, DimensionCollection dimensionCollection, NamedSetCollection namedSetCollection)
        {
            foreach (Dimension dimensionObj in dimensionCollection)
            {
                if (dimensionObj.Visible)
                {
                    MetaTreeNode mtn = new MetaTreeNode();
                    mtNodeCube.ChildNodes.Add(mtn);
                    //// Overridng the Parent Node form the base implementation
                    //// As we need all members Node root parent to be only upto dimension node
                    mtn.ParentNode = null;
                    FillMetaTreeNode(mtn, includeMembers, dimensionObj, namedSetCollection);
                }
            }
        }

        /// <summary>
        /// Fills the meta tree node.
        /// </summary>
        /// <param name="mtNodeCube">The meta tree node for cube.</param>
        /// <param name="includeMembers">if set to <c>true</c> [include members].</param>
        /// <param name="dimensionCollection">The dimension collection.</param>
        /// <param name="removedDimensionCollection">The removed dimension collection.</param>
        /// <param name="namedSetCollection">The named set collection.</param>
        public static void FillMetaTreeNode(MetaTreeNode mtNodeCube, bool includeMembers, DimensionCollection dimensionCollection, DimensionCollection removedDimensionCollection, NamedSetCollection namedSetCollection)
        {
            IEnumerable<string> excludedDimNames = removedDimensionCollection.Select(i => i.Name);
            IEnumerable<string> excludedDimUniqueNames = removedDimensionCollection.Select(j => j.UniqueName);
            bool hasExcludedName;
            foreach (Dimension dimensionObj in dimensionCollection)
            {
                hasExcludedName = false;
                var returnedName = excludedDimNames.Where(g => g.Equals(dimensionObj.Name)).FirstOrDefault();
                var returnedUniqueName = excludedDimUniqueNames.Where(h => h.Equals(dimensionObj.UniqueName)).FirstOrDefault();
                if ((!dimensionObj.Visible) || (returnedName != null && returnedName.Equals(dimensionObj.Name)) || (returnedUniqueName != null && returnedUniqueName.Equals(dimensionObj.UniqueName)))
                {
                    hasExcludedName = true;
                }
                if (!hasExcludedName)
                {
                    MetaTreeNode mtn = new MetaTreeNode();
                    mtNodeCube.ChildNodes.Add(mtn);
                    //// Overridng the Parent Node form the base implementation
                    //// As we need all members Node root parent to be only upto dimension node
                    mtn.ParentNode = null;
                    FillMetaTreeNode(mtn, includeMembers, dimensionObj, namedSetCollection);
                }
            }
        }

        /// <summary>
        /// Fills the meta tree node by hiding some measures and dimensions.
        /// </summary>
        /// <param name="mtNodeCube">The mt node cube.</param>
        /// <param name="includeMembers">if set to <c>true</c> [include members].</param>
        /// <param name="cubeSchema">CubeSchema</param>
        /// <param name="removedElements">The removed elements.</param>
        public static void FillMetaTreeNode(MetaTreeNode mtNodeCube, bool includeMembers, CubeSchema cubeSchema, List<String> removedElements)
        {
            IEnumerable<string> excludedDimNames = removedElements.Select(i => i);
            bool hasExcludedName;

            //For Hiding the measure nodes
            MetaTreeNode metaTreeNode = null;

            metaTreeNode = new MetaTreeNode(PropertyConstants.MeasrueNodeName, PropertyConstants.MeasrueNodeName, PropertyConstants.MeasrueNodeName);

            MetaTreeNode mtMeasureNode = mtNodeCube.ChildNodes.FindByUniqueName("Measures");

            //To remove the old measure and included the new one
            mtNodeCube.ChildNodes.Remove(mtMeasureNode);
            mtNodeCube.ChildNodes.Insert(0, metaTreeNode);

            MeasureCollection measureCollection = cubeSchema.Measures;
            Property property = mtNodeCube.Properties.FindByName(PropertyConstants.MeasureGroupName);
            if (property == null)
            {
                metaTreeNode.Properties.Add(PropertyConstants.MeasureGroupName, measureCollection);
            }

            metaTreeNode.UniqueName = PropertyConstants.MeasrueNodeName;
            metaTreeNode.NodeType = MetaTreeNodeType.MeasureGroup;
            foreach (Measure measureObj in measureCollection)
            {
                hasExcludedName = false;
                var returnedName = excludedDimNames.Where(g => g.Equals(measureObj.Name)).FirstOrDefault();
                if ((!measureObj.Visible) || (returnedName != null && returnedName.Equals(measureObj.Name)))
                {
                    hasExcludedName = true;
                }
                //To add the measures that are not in the ExcludedElements                 
                if (!hasExcludedName)
                {
                    MetaTreeNode mtn = new MetaTreeNode();
                    mtn.Caption = measureObj.Caption;
                    mtn.Description = measureObj.Description;
                    mtn.UniqueName = measureObj.UniqueName;
                    mtn.Name = measureObj.Name;
                    mtn.NodeType = MetaTreeNodeType.Measure;
                    mtn.Properties.Add(PropertyConstants.Measure, measureObj);
                    if (measureObj.GroupName.Length > 0)
                    {
                        MetaTreeNode mtNodeGroup = metaTreeNode.ChildNodes[measureObj.GroupName];
                        if (mtNodeGroup == null)
                        {
                            mtNodeGroup = new MetaTreeNode(measureObj.GroupName, measureObj.GroupName, measureObj.GroupName);
                            mtNodeGroup.NodeType = MetaTreeNodeType.DisplayFolder;
                            metaTreeNode.ChildNodes.Add(mtNodeGroup);
                        }

                        mtNodeGroup.ChildNodes.Add(mtn);
                        mtn.ParentNode = metaTreeNode;
                    }
                    else
                    {
                        metaTreeNode.ChildNodes.Add(mtn);
                        mtn.ParentNode = metaTreeNode;
                    }
                }
            }
            metaTreeNode.ParentNode = null;

            //For hiding the Dimensions
            DimensionCollection dimensionCollection = cubeSchema.Dimensions;
            foreach (Dimension dimensionObj in dimensionCollection)
            {
                hasExcludedName = false;
                var returnedName = excludedDimNames.Where(g => g.Equals(dimensionObj.Name)).FirstOrDefault();
                if ((!dimensionObj.Visible) || (returnedName != null && returnedName.Equals(dimensionObj.Name)))
                {
                    hasExcludedName = true;
                }
                //To add the dimensions that are not in the ExcludedElements 
                if (!hasExcludedName)
                {
                    MetaTreeNode mtn = new MetaTreeNode();
                    mtNodeCube.ChildNodes.Add(mtn);
                    //// Overridng the Parent Node form the base implementation
                    //// As we need all members Node root parent to be only upto dimension node
                    mtn.ParentNode = null;
                    FillMetaTreeNode(mtn, includeMembers, dimensionObj, cubeSchema.NamedSets);
                }
            }
        }



        #endregion

        /// <summary>
        /// Removes the unselected node.
        /// </summary>
        /// <param name="metaTreeNode">The meta tree node.</param>
        static void RemoveUnselectedNode(MetaTreeNode metaTreeNode)
        {
            if (metaTreeNode.ChildNodes.Count == 1)
            {
                if (metaTreeNode.ChildNodes[0].UniqueName == string.Empty
                    && metaTreeNode.ChildNodes[0].NodeCheckedType == MetaTreeNodeCheckedType.NoneSelected &&
                    metaTreeNode.ChildNodes[0].NodeType != MetaTreeNodeType.DisplayFolder)
                {
                    metaTreeNode.ChildNodes.Remove(metaTreeNode.ChildNodes[0]);
                }
                else
                {
                    MetaTreeNode mtNode = metaTreeNode.ChildNodes[0];
                    if (mtNode.IsSelected != null || mtNode.IsSelected == false)
                    {
                        if (mtNode.IsSelected != true)
                        {
                            metaTreeNode.ChildNodes.Remove(mtNode);
                        }
                        else
                        {
                            RemoveUnselectedNode(mtNode);
                        }
                    }
                    else
                    {
                        RemoveUnselectedNode(mtNode);
                    }
                }
            }
            else
            {
                for (int i = 0; i < metaTreeNode.ChildNodes.Count; i++)
                {
                    MetaTreeNode _metaTreeNode = metaTreeNode.ChildNodes[i];
                    if (_metaTreeNode.IsSelected != null || _metaTreeNode.IsSelected == false)
                    {
                        if (_metaTreeNode.IsSelected != true)
                        {
                            metaTreeNode.ChildNodes.Remove(_metaTreeNode);
                            i--;
                        }
                        else
                        {
                            RemoveUnselectedNode(_metaTreeNode);
                        }
                    }
                    else
                    {
                        RemoveUnselectedNode(_metaTreeNode);
                    }
                }
            }
        }

        private static bool FillKPIs(MetaTreeNode metaTreeNodeKPIs, KpiElement kpiElement)
        {
            if (metaTreeNodeKPIs.UniqueName.ToLower() == kpiElement.Name.ToLower())
            {
                //metaTreeNodeKPIs.IsSelected = null;
                //metaTreeNodeKPIs.__IsSelected = null;
                metaTreeNodeKPIs.NodeCheckedType = MetaTreeNodeCheckedType.SomeChildChecked;
                foreach (MetaTreeNode mtNode in metaTreeNodeKPIs.ChildNodes)
                {
                    if (mtNode.NodeType == MetaTreeNodeType.KPI_Goal)
                    {
                        //mtNode.IsSelected = kpiElement.ShowKPIGoal;
                        //mtNode.__IsSelected = kpiElement.ShowKPIGoal;
                        mtNode.NodeCheckedType = MetaTreeNodeCheckedType.CurrentChecked;
                    }
                    else if (mtNode.NodeType == MetaTreeNodeType.KPI_Status)
                    {
                        //mtNode.IsSelected = kpiElement.ShowKPIStatus;
                        //mtNode.__IsSelected = kpiElement.ShowKPIStatus;
                        mtNode.NodeCheckedType = MetaTreeNodeCheckedType.CurrentChecked;
                    }
                    else if (mtNode.NodeType == MetaTreeNodeType.KPI_Trend)
                    {
                        //mtNode.IsSelected = kpiElement.ShowKPITrend;
                        //mtNode.__IsSelected = kpiElement.ShowKPITrend;
                        mtNode.NodeCheckedType = MetaTreeNodeCheckedType.CurrentChecked;
                    }
                    else if (mtNode.NodeType == MetaTreeNodeType.KPI_Value)
                    {
                        //mtNode.IsSelected = kpiElement.ShowKPIValue;
                        //mtNode.__IsSelected = kpiElement.ShowKPIValue;
                        mtNode.NodeCheckedType = MetaTreeNodeCheckedType.CurrentChecked;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Fills the selected node collection.
        /// </summary>
        /// <param name="mtNodeCollection">The MetaTree node collection.</param>
        /// <param name="mtNode">The MetaTree node.</param>
        public static void FillSelectedNodeCollection(MetaTreeNodeCollection mtNodeCollection, MetaTreeNode mtNode)
        {
            MetaTreeNode _metaTreeNode = (MetaTreeNode)mtNode.Clone();
            if (_metaTreeNode.IsSelected == true || _metaTreeNode.IsSelected == null)
            {
                RemoveUnselectedNode(_metaTreeNode);
                mtNodeCollection.Add(_metaTreeNode);
            }
        }

        /// <summary>
        /// Fills the selected node collection version3.
        /// </summary>
        /// <param name="mtNodeCollection">The MetaTree node collection.</param>
        /// <param name="mtNode">The MetaTree node.</param>
        public static void FillSelectedNodeCollectionVersion3(MetaTreeNodeCollection mtNodeCollection, MetaTreeNode mtNode)
        {
            MetaTreeNode _metaTreeNode = (MetaTreeNode)mtNode.Clone();
            if (_metaTreeNode.IsSelected == true || _metaTreeNode.IsSelected == null || _metaTreeNode.IsSelected == false)
            {
                ////RemoveUnselectedNode(_metaTreeNode);
                mtNodeCollection.Add(_metaTreeNode);
            }
        }

        /// <summary>
        /// Gets the selected children node count.
        /// </summary>
        /// <param name="mtNode">The MetaTree node.</param>
        /// <returns></returns>
        static int GetSelectedChildrenNodeCount(MetaTreeNode mtNode)
        {
            int count = 0;
            foreach (var mtChildNode in mtNode.ChildNodes)
            {
                if (mtChildNode.IsSelected == true)
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Forces the fill child nodes.
        /// </summary>
        /// <param name="mtn">The MetaTree node.</param>
        public static void ForceFillChildNodes(MetaTreeNode mtn)
        {
            //// If Child node exist then On Demand member load override not required.
            bool IsOverride = !mtn.HasValidChildren;
            if (IsOverride)
            {
                if (mtn.Properties.Count > 0)
                {
                    if (mtn.Properties[0].Value is Level)
                    {
                        MetaTreeHelper.FillMetaTreeNode(mtn, (Level)mtn.Properties[0].Value, IsOverride, true);
                    }

                    if (mtn.Properties[0].Value is Member)
                    {
                        MetaTreeHelper.FillMetaTreeNode(mtn, (Member)mtn.Properties[0].Value, IsOverride, true);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the level of the KPI.
        /// </summary>
        /// <param name="displayFolderName">Display name of the folder.</param>
        /// <returns></returns>
        public static int GetLevelOfKPI(string displayFolderName)
        {
            int count = 0;
            string s = displayFolderName;
            while (s.Contains("\\"))
            {
                s = s.Substring(displayFolderName.IndexOf("\\") + 1);
                count++;
            }

            return count;
        }

        private static void AddKpiAttributes(MetaTreeNode parentMetaTreeNode, Kpi kpiObj)
        {
            parentMetaTreeNode.ChildNodes.Add(GetKpiNode(PropertyConstants.Value, parentMetaTreeNode, kpiObj));
            parentMetaTreeNode.ChildNodes.Add(GetKpiNode(PropertyConstants.Goal, parentMetaTreeNode, kpiObj));
            parentMetaTreeNode.ChildNodes.Add(GetKpiNode(PropertyConstants.Status, parentMetaTreeNode, kpiObj));
            parentMetaTreeNode.ChildNodes.Add(GetKpiNode(PropertyConstants.Trend, parentMetaTreeNode, kpiObj));
        }

        private static MetaTreeNode GetKpiNode(string type, MetaTreeNode parentMetatreeNode, Kpi kpiObject)
        {
            MetaTreeNode metaTreeNode = new MetaTreeNode
            {
                Name = type,
                Caption = type,
                Description = type,
                UniqueName = type
            };
            metaTreeNode.ParentNode = parentMetatreeNode;
            switch (type)
            {
                case PropertyConstants.Value:
                    {
                        metaTreeNode.NodeType = MetaTreeNodeType.KPI_Value;
                        metaTreeNode.Properties.Add(PropertyConstants.KPI_VALUE, kpiObject);
                        break;
                    }
                case PropertyConstants.Goal:
                    {
                        metaTreeNode.NodeType = MetaTreeNodeType.KPI_Goal;
                        metaTreeNode.Properties.Add(PropertyConstants.KPI_GOAL, kpiObject);
                        break;
                    }
                case PropertyConstants.Status:
                    {
                        metaTreeNode.NodeType = MetaTreeNodeType.KPI_Status;
                        metaTreeNode.Properties.Add(PropertyConstants.KPI_STATUS, kpiObject);
                        break;
                    }
                case PropertyConstants.Trend:
                    {
                        metaTreeNode.NodeType = MetaTreeNodeType.KPI_Trend;
                        metaTreeNode.Properties.Add(PropertyConstants.KPI_TREND, kpiObject);
                        break;
                    }
            }
            return metaTreeNode;
        }

        private static MetaTreeNode GetKpiNode(Kpi kpiObject, KpiElement kpiElement)
        {
            MetaTreeNode metaTreeNodeKpi = new MetaTreeNode(kpiObject.Name, kpiObject.Name, kpiObject.Name);
            metaTreeNodeKpi.UniqueName = kpiObject.Name;
            metaTreeNodeKpi.NodeType = MetaTreeNodeType.KPI;
            metaTreeNodeKpi.Properties.Add(new Property(PropertyConstants.KPI, kpiObject));

            MetaTreeNode valueNode = GetKpiNode(PropertyConstants.Value, metaTreeNodeKpi, kpiObject);
            SetNodeStatus(valueNode, kpiElement.ShowKPIValue);
            metaTreeNodeKpi.ChildNodes.Add(valueNode);

            MetaTreeNode goalNode = GetKpiNode(PropertyConstants.Goal, metaTreeNodeKpi, kpiObject);
            SetNodeStatus(goalNode, kpiElement.ShowKPIGoal);
            metaTreeNodeKpi.ChildNodes.Add(goalNode);

            MetaTreeNode statusNode = GetKpiNode(PropertyConstants.Status, metaTreeNodeKpi, kpiObject);
            SetNodeStatus(statusNode, kpiElement.ShowKPIStatus);
            metaTreeNodeKpi.ChildNodes.Add(statusNode);

            MetaTreeNode trendNode = GetKpiNode(PropertyConstants.Trend, metaTreeNodeKpi, kpiObject);
            SetNodeStatus(trendNode, kpiElement.ShowKPITrend);
            metaTreeNodeKpi.ChildNodes.Add(trendNode);

            var checkState = valueNode.CheckedState.ToString() + goalNode.CheckedState.ToString() + statusNode.CheckedState.ToString() + trendNode.CheckedState.ToString();
            if (checkState.Contains("32") || checkState.Contains("23"))
                metaTreeNodeKpi.IsSelected = null;

            metaTreeNodeKpi.AcceptIsSelectedChanges(false);

            return metaTreeNodeKpi;
        }

        private static void SetNodeStatus(MetaTreeNode node, bool isSelected)
        {
            if (isSelected)
            {
                node.IsSelected = true;
                node.AcceptIsSelectedChanges(false);
            }
            else
            {
                node.IsSelected = false;
                node.AcceptIsSelectedChanges(false);
            }
        }
    }
}

