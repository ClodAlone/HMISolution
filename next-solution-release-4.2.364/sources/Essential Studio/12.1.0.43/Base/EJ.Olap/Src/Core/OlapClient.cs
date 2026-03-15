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
using Syncfusion.Olap.Data;
using Syncfusion.Olap.Reports;
using Syncfusion.Olap.Manager;
using Syncfusion.JavaScript.Olap;
using Syncfusion.Olap.MDXQueryBuilder;
using Syncfusion.Olap.Common;
using System.Web.UI;
using System.Web;
using Syncfusion.JavaScript.Olap.Models;

namespace Syncfusion.JavaScript.Olap
{
    public class OlapClient : Syncfusion.JavaScript.Control
    {
        #region wrapperclass
        public OlapClientProperties OlapClientModel
        {
            get;
            set;
        }
        private OlapDataManager DataManager
        {
            get;
            set;
        }
        public override string TagName
        {
            get
            {
                return "div";
            }
        }
        public override string PluginName
        {
            get { return "ejOlapClient"; }
        }
        protected override object Model
        {
            get { return this.OlapClientModel; }
        }

        public OlapClient() { }
        public OlapClient(String id, OlapClientProperties propModel)
        {
            this.ID = id;
            this.OlapClientModel = propModel;
        }

        public override HtmlString CreateContainer(string controlId)
        {
            StringBuilder tag = new StringBuilder();

            tag.Append("<")
               .Append(TagName)
               .Append(" id=\"")
               .Append(controlId + "\"")
               .Append("></")
               .Append(TagName)
               .Append(">");
            return new HtmlString(String.Format(tag.ToString()));
        }
        #endregion

        List<object> treeNodes = new List<object>();
        CubeSchema cubeSchema = null;
        MetaTreeNode _metaTreeNode = null;
        OlapGrid htmlHelper = new OlapGrid();
        bool isMeasuresRemoved;

        private Dictionary<string, object> GetJsonData(string action, OlapDataManager dataManager, string cubeName, string clientParams, string dimensionName, string cellPostion, string headerInfo, string dropType, string nodeInfo, string toolbarOperation, string reportName)
        {
            if (dataManager != null)
                this.DataManager = dataManager;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            if (action == "initializeClient")
            {
                dict.Add("Cubes", new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(FindCubeNames(dataManager)));
                dict.Add("Columns", FindElements(dataManager.CurrentReport, AxisPosition.Categorical));
                dict.Add("Rows", FindElements(dataManager.CurrentReport, AxisPosition.Series));
                dict.Add("Slicers", FindElements(dataManager.CurrentReport, AxisPosition.Slicer));
                dict.Add("CubeTreeInfo", new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(FindCubeTreeInfo(dataManager)));
                if (dataManager.Reports.Count == 0)
                    dataManager.SetCurrentReport(new OlapReport() { Name = "Default Report", CurrentCubeName = dataManager.CurrentCubeName });
                dict.Add("CurrentReport", Utils.SerializeOlapReport(dataManager.CurrentReport));
                dict.Add("ClientReports", Common.SerializeObject<OlapReportCollection>(dataManager.Reports).Compress());
                dict.Add("ReportsCount", dataManager.Reports.Count);
                dict.Add("ReportList", new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(GetReportList(dataManager.Reports)));
            }
            else if (action == "initializeGrid")
            {
                if (clientParams != null)
                    dict = htmlHelper.GetJsonData(action, dataManager, clientParams);
                else
                    dict = htmlHelper.GetJsonData(action, dataManager);
            }
            else if (action == "drillDownGrid")
            {
                if (clientParams != null)
                    dict = htmlHelper.GetJsonData(action, null, dataManager, cellPostion, headerInfo, clientParams);
                else
                    dict = htmlHelper.GetJsonData(action, dataManager, cellPostion, headerInfo);
            }
            else if (action == "FetchSortState")
            {
               
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                dictionary.Add("FetchSortState", OnLoadSortFilterState(dataManager, nodeInfo,"sortAndFilter"));
                return dictionary;
            }
            else if (action == "filtering" || action == "removeSplitButton")
            {
                if (action == "filtering")
                    dataManager = OnFilterElement(dataManager, clientParams);
                else if (action == "removeSplitButton")
                    dataManager = OnSplitButtonRemove(dataManager, clientParams);
                dict.Add("UpdatedReport", Utils.SerializeOlapReport(dataManager.CurrentReport));
                dict.Add("ClientReports", Common.SerializeObject<OlapReportCollection>(dataManager.Reports).Compress());
            }
            else if (action == "fetchMemberTreeNodes")
            {
                dict.Add("EditorTreeInfo", new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(FindEditorTreeInfo(dataManager, dimensionName)));
            }
            else if (action == "nodeDropped")
            {
                dataManager = OnElementDropped(dataManager, nodeInfo, dropType);
                dict.Add("Columns", FindElements(dataManager.CurrentReport, AxisPosition.Categorical));
                dict.Add("Rows", FindElements(dataManager.CurrentReport, AxisPosition.Series));
                dict.Add("Slicers", FindElements(dataManager.CurrentReport, AxisPosition.Slicer));
                dict.Add("UpdatedReport", Utils.SerializeOlapReport(dataManager.CurrentReport));
                dict.Add("ClientReports", Common.SerializeObject<OlapReportCollection>(dataManager.Reports).Compress());
            }
            else if (action == "memberExpanded")
            {
                dict.Add("ChildNodes", new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(GetChildNodes(dataManager, cubeName, dimensionName, cellPostion, nodeInfo, clientParams)));
            }
            else if (action == "toolbarOperation")
            {
                dataManager = ReportManipulations(dataManager, reportName, toolbarOperation);
                dict.Add("CurrentReport", Utils.SerializeOlapReport(dataManager.CurrentReport));
                dict.Add("Reports", Common.SerializeObject<OlapReportCollection>(dataManager.Reports).Compress());
                dict.Add("ReportsCount", dataManager.Reports.Count);
                dict.Add("Columns", FindElements(dataManager.CurrentReport, AxisPosition.Categorical));
                dict.Add("Rows", FindElements(dataManager.CurrentReport, AxisPosition.Series));
                dict.Add("Slicers", FindElements(dataManager.CurrentReport, AxisPosition.Slicer));
                dict.Add("CurrentAction", toolbarOperation);
                dict.Add("ReportList", new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(GetReportList(dataManager.Reports)));
                dict.Add("RenamedReport", dataManager.CurrentReport.Name);
                return dict;
            }
            else
            {
                dataManager = OnCubeChanged(dataManager, cubeName);
                dict.Add("NewReport", Utils.SerializeOlapReport(dataManager.CurrentReport));
                dict.Add("CubeTreeInfo", new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(FindCubeTreeInfo(dataManager)));
                dict.Add("ClientReports", Common.SerializeObject<OlapReportCollection>(dataManager.Reports).Compress());
                dict.Add("ReportsCount", dataManager.Reports.Count);
                dict.Add("ReportList", new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(GetReportList(dataManager.Reports)));
            }
            return dict;
        }



        public Dictionary<string, object> GetJsonData(string action, OlapDataManager dataManager)
        {
            return GetJsonData(action, dataManager, null, null, null, null, null, null, null, null, null);
        }

        public Dictionary<string, object> GetJsonData(string action, OlapDataManager dataManager, string clientParams)
        {
            if (action == "fetchMemberTreeNodes")
                return GetJsonData(action, dataManager, null, null, clientParams, null, null, null, null, null, null);
            else if (action == "cubeChanged")
                return GetJsonData(action, dataManager, clientParams, null, null, null, null, null, null, null, null);
            else
                return GetJsonData(action, dataManager, null, clientParams, null, null, null, null, null, null, null);
        }

        public Dictionary<string, object> GetJsonData(string action, OlapDataManager dataManager, string clientParams, string clientInfo)
        {
            if (action == "drillDownGrid")
                return GetJsonData(action, dataManager, null, null, null, clientParams, clientInfo, null, null, null, null);
            else if (action == "toolbarOperation")
                return GetJsonData(action, dataManager, null, null, null, null, null, null, null, clientParams, clientInfo);
            else
                return GetJsonData(action, dataManager, null, null, null, null, null, clientParams, clientInfo, null, null);
        }

        public Dictionary<string, object> GetJsonData(string action, OlapDataManager dataManager, string clientParams, string clientInfo, string gridLayout)
        {
            if (action == "drillDownGrid")
                return GetJsonData(action, dataManager, null, gridLayout, null, clientParams, clientInfo, null, null, null, null);
            else
                return null;
        }
        public Dictionary<string, object> GetJsonData(string action, OlapDataManager dataManager, bool checkedStatus, string parentNode, string tag, string dimensionName, string cubeName)
        {
            return GetJsonData(action, dataManager, cubeName, parentNode, dimensionName, checkedStatus.ToString(), null, null, tag, null, null);
        }
        int nodeCount = 0;
        public List<object> FindCubeTreeInfo(OlapDataManager DataManager)
        {
            if (DataManager != null)
            {
                if (DataManager.CurrentCubeName == null)
                    DataManager.CurrentCubeName = DataManager.DataProvider.GetCubes[0].Name;
                var mtNodes = new MetaTreeNodeCollection(null);
                var mtNode = new MetaTreeNode
                {
                    NodeType = MetaTreeNodeType.Cube,
                    Name = DataManager.CurrentCubeName,
                    Caption = DataManager.CurrentCubeName
                };
                mtNodes.Add(mtNode);
                cubeSchema = DataManager.CurrentCubeSchema;
                MetaTreeHelper.FillMetaTreeNode(mtNode, cubeSchema.Measures, true, false);
                MetaTreeHelper.FillMetaTreeNode(mtNode, false, cubeSchema.Dimensions);
                foreach (var root in mtNodes[0].ChildNodes)
                {
                    treeNodes.Add(new { id = root.UniqueName + "_" + nodeCount, name = root.Caption, hasChildren = root.HasValidChildren, spriteCssClass = GetTreeNodeIcons(root), tag = root.UniqueName });
                    if (root.ChildNodes.Count > 0)
                        GenerateChildTreeNodes(root.ChildNodes, root.UniqueName + "_" + nodeCount);
                    nodeCount++;
                }
                return treeNodes;
            }
            return null;
        }

        private void GenerateChildTreeNodes(MetaTreeNodeCollection metaTreeNodeCollectionObj, string pUniqueName)
        {
            nodeCount++;
            foreach (var root in metaTreeNodeCollectionObj)
            {
                if (string.IsNullOrEmpty(root.UniqueName) && root.NodeType == MetaTreeNodeType.DisplayFolder)
                    root.UniqueName = "[" + root.ParentNode.Caption + "_" + root.Caption + "].DF";
                treeNodes.Add(new { id = root.UniqueName + "_" + nodeCount, pid = pUniqueName, name = root.Caption, hasChildren = root.HasValidChildren, spriteCssClass = GetTreeNodeIcons(root), tag = root.UniqueName });
                if (root.ChildNodes.Count > 0)
                    GenerateChildTreeNodes(root.ChildNodes, root.UniqueName + "_" + nodeCount);
                nodeCount++;
            }
        }

        private string GetTreeNodeIcons(MetaTreeNode metaTreeNode)
        {
            if (metaTreeNode.NodeType == MetaTreeNodeType.Dimension)
                return "dimensionCDB" + " " + "e-icon";
            else if (metaTreeNode.NodeType == MetaTreeNodeType.DisplayFolder)
                return "folderCDB" + " " + "e-icon";
            else if (metaTreeNode.NodeType == MetaTreeNodeType.Hierarchy)
            {
                if (metaTreeNode.ChildNodes.Count > 1)
                    return "hierarchyCDB" + " " + "e-icon";
                else
                    return "attributeCDB" + " " + "e-icon";
            }
            else if (metaTreeNode.NodeType == MetaTreeNodeType.Level)
                return "level" + metaTreeNode.LevelDepth + " " + "e-icon";
            else if (metaTreeNode.NodeType == MetaTreeNodeType.Measure)
                return "chartCDB" + " " + "e-icon";
            else if (metaTreeNode.NodeType == MetaTreeNodeType.MeasureGroup)
                return "folderCDB" + " " + "e-icon";
            else if (metaTreeNode.NodeType == MetaTreeNodeType.NamedSet)
                return "namedSetCDB" + " " + "e-icon";

            return string.Empty;
        }

        public List<object> FindCubeNames(OlapDataManager datamanager)
        {
            List<object> cubeNames = new List<object>();
            foreach (CubeInfo cube in (datamanager.DataProvider).GetCubes)
            {
                cubeNames.Add(new { name = cube.Name });
            }
            return cubeNames;
        }
        public string FindElements(OlapReport olapReport, AxisPosition axis)
        {
            string uniqueNames = string.Empty;
            if (axis == AxisPosition.Categorical)
                uniqueNames = this.FindUniqueName(olapReport.CategoricalElements);
            else if (axis == AxisPosition.Series)
                uniqueNames = this.FindUniqueName(olapReport.SeriesElements);
            else if (axis == AxisPosition.Slicer)
                uniqueNames = this.FindUniqueName(olapReport.SlicerElements);
            return uniqueNames;
        }

        private string FindUniqueName(Items elements)
        {            
            MetaTreeNode mtNodeCaption = null;            
            bool dimensionCaption=false;
            string elementNames = string.Empty;
            string elementCaption = string.Empty;
            foreach (Item element in elements)
            {
                if (element.ElementValue.GetType().Name == "DimensionElement")
                {
                    elementNames += "#" + (element.ElementValue.ElementName) + '.' + ((element.ElementValue) as DimensionElement).Hierarchy.ElementName;
                      if (this.DataManager.ConnectionString.ToLower().Replace(" ", String.Empty).Split(';', '=').Contains("localeidentifier"))
                        {
                            if (this.DataManager.CurrentCubeSchema != null)
                                cubeSchema = this.DataManager.CurrentCubeSchema;
                            else
                                cubeSchema = this.DataManager.DataProvider.GetCubeSchema(this.DataManager.CurrentReport.CurrentCubeName);
                            mtNodeCaption = LoadElements(cubeSchema, element);
                            elementCaption += "~" + mtNodeCaption.ToString();
                            dimensionCaption = true;
                        }                                                
                }
                else if (element.ElementValue.GetType().Name == "MeasureElements")
                    elementNames += "#Measures";
            }
           if (dimensionCaption == true )            
                elementNames += '~' + elementCaption;
            return elementNames;
        }

        public List<object> FindEditorTreeInfo(OlapDataManager manager, string dimensionName)
        {
            List<object> editorTree = new List<object>();
            MetaTreeNode seriesmtNode = null;
            string[] memberEditorArgs = dimensionName.Split(':');

            var axis = memberEditorArgs[0];
            string heirachyName = memberEditorArgs[1];

            if (axis == "Columns")
            {
                foreach (var catItem in manager.CurrentReport.CategoricalElements)
                {
                    Item item = null;
                    if (catItem.ElementValue is MeasureElements && memberEditorArgs[1] == "Measures")
                        item = catItem;
                    else if (catItem.ElementValue is DimensionElement && (catItem.ElementValue.Name + "." + (catItem.ElementValue as DimensionElement).Hierarchy.Name) == heirachyName)
                        item = catItem;
                    if (item != null)
                    {
                        seriesmtNode = LoadElements(manager.CurrentCubeSchema, catItem);
                        List<string> MemberEditorExcludeElements = GetExcludeElements(item.ExcludedElementValue);
                        foreach (var root in seriesmtNode.ChildNodes)
                        {
                            var tagAttr = (root.NodeType == MetaTreeNodeType.DisplayFolder || root.NodeType == MetaTreeNodeType.Measure || root.NodeType == MetaTreeNodeType.MeasureGroup) ? "" : root.UniqueName + "<<" + (root.Properties[0].Value as Member).LevelUniqueName;
                            var idAttr = root.Caption.Replace(" ", "_") + "_" + (root.LevelDepth + 1);
                            bool isChecked = true;
                            if (MemberEditorExcludeElements.Contains(root.UniqueName))
                                isChecked = false;
                            editorTree.Add(new { id = idAttr, name = root.Caption, hasChildren = root.HasChildMembers, checkedStatus = isChecked, tag = tagAttr, uniqueName = root.UniqueName });
                        }
                        break;
                    }
                }
            }
            else if (axis == "Rows")
            {
                foreach (var seriesItem in manager.CurrentReport.SeriesElements)
                {
                    Item item = null;
                    if (seriesItem.ElementValue is MeasureElements && memberEditorArgs[1] == "Measures")
                        item = seriesItem;
                    else if (seriesItem.ElementValue is DimensionElement && (seriesItem.ElementValue.Name + "." + (seriesItem.ElementValue as DimensionElement).Hierarchy.Name) == heirachyName)
                        item = seriesItem;
                    if (item != null)
                    {
                        seriesmtNode = LoadElements(manager.CurrentCubeSchema, seriesItem);
                        List<string> MemberEditorExcludeElements = GetExcludeElements(item.ExcludedElementValue);
                        foreach (var root in seriesmtNode.ChildNodes)
                        {
                            var tagAttr = (root.NodeType == MetaTreeNodeType.DisplayFolder || root.NodeType == MetaTreeNodeType.Measure || root.NodeType == MetaTreeNodeType.MeasureGroup) ? "" : root.UniqueName + "<<" + (root.Properties[0].Value as Member).LevelUniqueName;
                            var idAttr = root.Caption.Replace(" ", "_") + "_" + (root.LevelDepth + 1);
                            bool isChecked = true;
                            if (MemberEditorExcludeElements.Contains(root.UniqueName))
                                isChecked = false;
                            editorTree.Add(new { id = idAttr, name = root.Caption, hasChildren = root.HasChildMembers, checkedStatus = isChecked, tag = tagAttr, uniqueName = root.UniqueName });
                        }
                        break;
                    }
                }
            }
            else if (axis == "Slicers")
            {
                foreach (var slicerItem in manager.CurrentReport.SlicerElements)
                {
                    Item item = null;
                    if (slicerItem.ElementValue is MeasureElements && memberEditorArgs[1] == "Measures")
                        item = slicerItem;
                    else if (slicerItem.ElementValue is DimensionElement && (slicerItem.ElementValue.Name + "." + (slicerItem.ElementValue as DimensionElement).Hierarchy.Name) == heirachyName)
                        item = slicerItem;
                    if (item != null)
                    {
                        var slicerNode = LoadElements(manager.CurrentCubeSchema, slicerItem);
                        List<string> MemberEditorExcludeElements = GetExcludeElements(item.ExcludedElementValue);
                        foreach (var root in slicerNode.ChildNodes)
                        {
                            var tagAttr = (root.NodeType == MetaTreeNodeType.DisplayFolder || root.NodeType == MetaTreeNodeType.Measure || root.NodeType == MetaTreeNodeType.MeasureGroup) ? "" : root.UniqueName + "<<" + (root.Properties[0].Value as Member).LevelUniqueName;
                            var idAttr = root.Caption.Replace(" ", "_") + "_" + (root.LevelDepth + 1);
                            bool isChecked = true;
                            if (MemberEditorExcludeElements.Contains(root.UniqueName))
                                isChecked = false;
                            editorTree.Add(new { id = idAttr, name = root.Caption, hasChildren = root.HasChildMembers, checkedStatus = isChecked, tag = tagAttr, uniqueName = root.UniqueName });
                        }
                        break;
                    }
                }
            }
            return editorTree;
        }

        private List<string> GetExcludeElements(Element element)
        {
            List<string> MemberEditorExcludeElements = new List<string>();
            if (element is DimensionElement)
            {
                LevelElementCollection levelElementCollection = (element as DimensionElement).Hierarchy.LevelElements;
                foreach (LevelElement _levelElement in levelElementCollection)
                {
                    MemberElementCollection memberElementCollection = _levelElement.MemberElements;
                    foreach (MemberElement memberElement in memberElementCollection)
                    {
                        MemberEditorExcludeElements.Add(memberElement.UniqueName);
                    }
                }
            }
            return MemberEditorExcludeElements;
        }

        private MetaTreeNode LoadElements(CubeSchema cubeSchema, Item item)
        {
            CubeSchema m_cubeSchema = cubeSchema;
            if (item.ElementValue is MeasureElements)
            {
                MeasureElements measureElements = item.ElementValue as MeasureElements;
                MetaTreeNode measureGroupNode = new MetaTreeNode(PropertyConstants.MeasrueNodeName,
                PropertyConstants.MeasrueNodeName, PropertyConstants.MeasrueNodeName) { UniqueName = PropertyConstants.MeasrueNodeName };
                measureGroupNode.NodeType = MetaTreeNodeType.MeasureGroup;
                measureGroupNode.Properties.Add(new Property(PropertyConstants.MeasureGroupName, m_cubeSchema.Measures));

                MetaTreeNode metaTreeNodeMeasures = new MetaTreeNode
                {
                    Name = PropertyConstants.MeasrueNodeName,
                    UniqueName = PropertyConstants.MeasrueNodeName,
                    Caption = PropertyConstants.MeasrueNodeName,
                    Description = PropertyConstants.MeasrueNodeName
                };
                MetaTreeHelper.FillMetaTreeNode(measureElements, metaTreeNodeMeasures, m_cubeSchema.Measures, false, true);

                foreach (MeasureElement measureElement in measureElements.Elements)
                {
                    MetaTreeNode metaTreeNode = GetMeasureNode(metaTreeNodeMeasures, measureElement);
                    if (metaTreeNode != null)
                        measureGroupNode.ChildNodes.Add(metaTreeNode);
                }
                measureGroupNode.Properties.Add(new Property(PropertyConstants.AxisElements, null));
                return measureGroupNode;
            }
            else if (item.ElementValue is DimensionElement)
            {
                DimensionElement dimensionElement = item.ElementValue as DimensionElement;
                DimensionElement excludedDimensionElement = item.ExcludedElementValue as DimensionElement;
                string dimensionName = dimensionElement.Name;
                if (dimensionElement.Name != string.Empty)
                {
                    Dimension dimensionObj = m_cubeSchema.GetDimensionByUniqueName(dimensionElement.UniqueName);
                    string dimensionCaption = dimensionObj.Caption;
                    string dimensionDescription = dimensionObj.Description;
                    string dimensionUniqueName = dimensionObj.UniqueName;
                    MetaTreeNode metaTreeNode = new MetaTreeNode(dimensionName, dimensionCaption, dimensionDescription);
                    if (dimensionObj != null)
                    {
                        metaTreeNode.Properties.Add(new Property(PropertyConstants.Dimension, dimensionObj));
                    }
                    metaTreeNode.UniqueName = dimensionUniqueName.Replace("[", "").Replace("]", "");
                    LevelElement levelElement = dimensionElement.Hierarchy.LevelElements[0];
                    foreach (Hierarchy hierarchyObj in dimensionObj.Hierarchies)
                    {
                        if (hierarchyObj.UniqueName == dimensionElement.Hierarchy.UniqueName || dimensionObj.UniqueName + ".[" + hierarchyObj.Name + "]" == dimensionElement.Hierarchy.UniqueName)
                        {
                            metaTreeNode.Properties.Add(new Property(PropertyConstants.Hierarchy, hierarchyObj));
                            foreach (Level lElement in hierarchyObj.Levels)
                            {
                                Level levelObj = lElement;
                                if (levelObj.UniqueName == levelElement.UniqueName || dimensionObj.UniqueName + ".[" + hierarchyObj.Name + "].[" + levelObj.Name + "]" == levelElement.UniqueName)
                                {
                                    if (levelObj.LevelType == LevelTypeEnum.All && hierarchyObj.Levels.Count > 1)
                                        levelObj = hierarchyObj.Levels[1];
                                    metaTreeNode.Properties.Add(new Property(PropertyConstants.Level, levelObj));
                                    foreach (Member memberObj in levelObj.Members)
                                    {
                                        MetaTreeNode metaTreeNodeMembers = new MetaTreeNode();
                                        MetaTreeHelper.FillMetaTreeNode(metaTreeNodeMembers, memberObj, false, true);
                                        metaTreeNode.ChildNodes.Add(metaTreeNodeMembers);
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    metaTreeNode.Properties.Add(new Property(PropertyConstants.AxisElements, this));
                    return metaTreeNode;
                }
            }
            else if (item.ElementValue is NamedSetElement)
            {
                NamedSetElement namedSetElement = item.ElementValue as NamedSetElement;
                NamedSet namedSetObj = m_cubeSchema.GetNamedSetByUniqueName(namedSetElement.UniqueName);
                MetaTreeNode metaTreeNodeNamedSet = new MetaTreeNode(namedSetElement.Name, namedSetElement.Name, namedSetElement.Name);
                if (namedSetObj != null)
                {
                    metaTreeNodeNamedSet.Description = namedSetObj.Description;
                    metaTreeNodeNamedSet.Properties.Add(new Property(PropertyConstants.NamedSet, namedSetObj));
                    Dimension dimensionObj = m_cubeSchema.GetDimensionByUniqueName(namedSetElement.DimensionUniqueName);
                    metaTreeNodeNamedSet.UniqueName = namedSetObj.ParentDimensionName;
                    metaTreeNodeNamedSet.Properties.Add(new Property(PropertyConstants.Dimension, dimensionObj));
                    metaTreeNodeNamedSet.NodeCheckedType = MetaTreeNodeCheckedType.CurrentChecked;
                }
                return metaTreeNodeNamedSet;
            }
            return null;
        }
        private MetaTreeNode GetMeasureNode(MetaTreeNode metaTreeNodeMeasuresSource, MeasureElement measureElement)
        {
            foreach (MetaTreeNode childNode in metaTreeNodeMeasuresSource.ChildNodes)
            {
                if (childNode.UniqueName.ToLower() == measureElement.UniqueName.ToLower())
                {
                    childNode.IsSelected = true;
                    childNode.AcceptIsSelectedChanges(true);
                    return childNode;
                }
                else
                {
                    if (childNode.ChildNodes.Count > 0)
                    {
                        MetaTreeNode node = this.GetMeasureNode(childNode, measureElement);
                        if (node != null)
                            return node;
                    }
                }
            }
            return null;
        }
        private OlapDataManager OnUpdateFilterElement(OlapDataManager dataManager)
        {
            string rowfilterState = string.Empty, columnfilterState = string.Empty;
            if(dataManager.CurrentReport.SeriesElements.Count==0 || (dataManager.CurrentReport.SeriesElements.Count == 1 && dataManager.CurrentReport.SeriesElements[0].ElementValue is SortElement))
                OnRemoveFilterElement(dataManager);
            else
                rowfilterState = OnLoadSortFilterState(dataManager, "Row", "filterOnly");
            if (dataManager.CurrentReport.CategoricalElements.Count == 0 || (dataManager.CurrentReport.CategoricalElements.Count == 1 && dataManager.CurrentReport.CategoricalElements[0].ElementValue is SortElement))
                OnRemoveFilterElement(dataManager);
            else
                columnfilterState = OnLoadSortFilterState(dataManager, "Column", "filterOnly");
            dataManager.CurrentReport.FilterElements.Clear();
            dataManager.CurrentReport.CategoricalElements.IsFilterOrSortOn = false;
            dataManager.CurrentReport.SeriesElements.IsFilterOrSortOn = false;
            dataManager.CurrentReport.SlicerElements.IsFilterOrSortOn = false;
            dataManager.CurrentReport.ShowExpanders = true;
            if (!string.IsNullOrEmpty(rowfilterState) && !(dataManager.CurrentReport.SeriesElements.Count == 0) && !(isMeasuresRemoved))
                ReportManipulations(dataManager, (":: ::Row" + "||" + rowfilterState.Replace("<<", "::")), "SortOrFilter");
            if (!string.IsNullOrEmpty(columnfilterState) && !(dataManager.CurrentReport.CategoricalElements.Count == 0) && !(isMeasuresRemoved))
                ReportManipulations(dataManager, (":: ::Column" + "||" + columnfilterState.Replace("<<", "::")), "SortOrFilter");
            return dataManager;
        }
        private OlapDataManager OnRemoveFilterElement(OlapDataManager dataManager)
        {
            dataManager.CurrentReport.FilterElements.Clear();
            dataManager.CurrentReport.CategoricalElements.IsFilterOrSortOn = false;
            dataManager.CurrentReport.SeriesElements.IsFilterOrSortOn = false;
            dataManager.CurrentReport.SlicerElements.IsFilterOrSortOn = false;
            dataManager.CurrentReport.ShowExpanders = true;
            return dataManager;
        }
        private string OnLoadSortFilterState(OlapDataManager DataManager, string axis, string stateType)
        {
            string measuresList = string.Empty, sortingDetails = string.Empty, filterDetail = string.Empty;
            if (stateType.Contains("sortAndFilter") || stateType == "sortAndFilter")
            {
                MeasureCollection reportMeasures = new MeasureCollection();
                SortElement sortelemnt;

                foreach (var categItem in DataManager.CurrentReport.CategoricalElements)
                {
                    measuresList = OnFindMeasureElements(categItem);
                    if (!(string.IsNullOrEmpty(measuresList)))
                        break;
                }
                if (string.IsNullOrEmpty(measuresList))
                    foreach (var serisItem in DataManager.CurrentReport.SeriesElements)
                    {
                        measuresList = OnFindMeasureElements(serisItem);
                        if (!(string.IsNullOrEmpty(measuresList)))
                            break;
                    }
                if (string.IsNullOrEmpty(measuresList))
                    foreach (var slicerItem in DataManager.CurrentReport.SlicerElements)
                    {
                        measuresList = OnFindMeasureElements(slicerItem);
                        if (!(string.IsNullOrEmpty(measuresList)))
                            break;
                    }
                switch (axis)
                {
                    case "Row":
                        {
                            int rowValue = -1;
                            rowValue = this.CheckSortElements(DataManager.CurrentReport.SeriesElements);
                            if (rowValue >= 0)
                            {
                                sortelemnt = DataManager.CurrentReport.SeriesElements[rowValue].ElementValue as SortElement;
                                sortingDetails += ((sortelemnt.SortOrder == Syncfusion.Olap.Reports.SortOrder.ASC || sortelemnt.SortOrder == Syncfusion.Olap.Reports.SortOrder.BASC) ? ("ASC" + "<<" + (sortelemnt.SortOrder == Syncfusion.Olap.Reports.SortOrder.BASC ? "UPH" : "PH")) : ("DESC" + "<<" + (sortelemnt.SortOrder == Syncfusion.Olap.Reports.SortOrder.BDESC ? "UPH" : "PH"))) + "<<" + (sortelemnt.Element.UniqueName).Split('.')[1].Replace("[", "").Replace("]", "");
                            }
                        }
                        break;
                    case "Column":
                        {
                            int colValue = -1;
                            colValue = this.CheckSortElements(DataManager.CurrentReport.CategoricalElements);
                            if (colValue >= 0)
                            {
                                sortelemnt = DataManager.CurrentReport.CategoricalElements[colValue].ElementValue as SortElement;
                                sortingDetails += ((sortelemnt.SortOrder == Syncfusion.Olap.Reports.SortOrder.ASC || sortelemnt.SortOrder == Syncfusion.Olap.Reports.SortOrder.BASC) ? ("ASC" + "<<" + (sortelemnt.SortOrder == Syncfusion.Olap.Reports.SortOrder.BASC ? "UPH" : "PH")) : ("DESC" + "<<" + (sortelemnt.SortOrder == Syncfusion.Olap.Reports.SortOrder.BDESC ? "UPH" : "PH"))) + "<<" + (sortelemnt.Element.UniqueName).Split('.')[1].Replace("[", "").Replace("]", "");
                                //sortingDetails += sortelemnt.SortOrder == Syncfusion.Olap.Reports.SortOrder.ASC ? "ASC" : "DESC" + "<<" +  (sortelemnt.Element.UniqueName).Split('.')[1].Replace("[", "").Replace("]", ""); 
                            }
                        }
                        break;
                }
            }
            if (DataManager.CurrentReport.FilterElements.Count > 0)
            {
                foreach (Item filterElement in DataManager.CurrentReport.FilterElements)
                {
                    if (axis == "Row" && filterElement.Axis == AxisPosition.Series)
                    {
                        if (string.IsNullOrEmpty(filterDetail))
                            filterDetail +=( stateType == "filterOnly"? (((FilterElement)filterElement.ElementValue).FilterValue[0] as MeasureElement).UniqueName :(((FilterElement)filterElement.ElementValue).FilterValue[0] as MeasureElement).UniqueName.Split('.')[1].Replace("[", "").Replace("]", "")) + "<<" + Convert.ToString(((FilterElement)filterElement.ElementValue).FilterCase) + "<<" + (((FilterElement)filterElement.ElementValue).FilterValue[1] as FilterValue).Filter_Value;
                        else
                            filterDetail += "<<" + (((FilterElement)filterElement.ElementValue).FilterValue[1] as FilterValue).Filter_Value;

                    }
                    else if (axis == "Column" && filterElement.Axis == AxisPosition.Categorical)
                    {
                        if (string.IsNullOrEmpty(filterDetail))
                            filterDetail += ( stateType == "filterOnly"? (((FilterElement)filterElement.ElementValue).FilterValue[0] as MeasureElement).UniqueName :(((FilterElement)filterElement.ElementValue).FilterValue[0] as MeasureElement).UniqueName.Split('.')[1].Replace("[", "").Replace("]", "")) + "<<" + Convert.ToString(((FilterElement)filterElement.ElementValue).FilterCase) + "<<" + (((FilterElement)filterElement.ElementValue).FilterValue[1] as FilterValue).Filter_Value;
                        else
                            filterDetail += "<<" + (((FilterElement)filterElement.ElementValue).FilterValue[1] as FilterValue).Filter_Value;
                    }
                }
            }
            else
                filterDetail += "";
            return (stateType == "filterOnly"?filterDetail :( measuresList + "||" + sortingDetails + "||" + filterDetail));
        }
        private string OnFindMeasureElements(Item axisElement)
        {
            string measuresList = string.Empty;
            if (axisElement != null && axisElement.ElementValue is MeasureElements)
            {
                MeasureElements measureElements = axisElement.ElementValue as MeasureElements;
                foreach (var measureElement in measureElements.Elements)
                {
                    if (measureElement is MeasureElement)
                    {
                        MeasureElement element = measureElement as MeasureElement;
                        measuresList = measuresList != string.Empty ? (measuresList + "__" + ((element.UniqueName).Split('.')[1].Replace("[", "").Replace("]", ""))) : (element.UniqueName).Split('.')[1].Replace("[", "").Replace("]", "");
                    }
                }
            }
            return measuresList; 
        }
        public OlapDataManager OnFilterElement(OlapDataManager DataManager, string clientParams)
        {
            if (string.IsNullOrEmpty(clientParams))
                return DataManager;
            if (clientParams.Contains("Measures"))
            {
                var valueOfMeasures = clientParams.Split(new string[] { "Measures:" }, StringSplitOptions.RemoveEmptyEntries);
                var measures = (valueOfMeasures.Length > 0) ? valueOfMeasures[0] : string.Empty;
                bool isValid = true;
                for (int i = DataManager.CurrentReport.CategoricalElements.Count - 1; i >= 0; i--)
                {
                    if (DataManager.CurrentReport.CategoricalElements[i].ElementValue is MeasureElements)
                    {
                        if (string.IsNullOrEmpty(measures))
                            DataManager.CurrentReport.CategoricalElements.RemoveAt(i);
                        else
                        {
                            var item = DataManager.CurrentReport.CategoricalElements[i].ElementValue as MeasureElements;
                            for (int j = item.Elements.Count - 1; j >= 0; j--)
                            {
                                if (!measures.Contains(item.Elements[j].ElementName))
                                    item.Elements.RemoveAt(j);
                            }
                        }
                        isValid = false;
                    }
                }
                if (isValid)
                {
                    for (int i = DataManager.CurrentReport.SeriesElements.Count - 1; i >= 0; i--)
                    {
                        if (DataManager.CurrentReport.SeriesElements[i].ElementValue is MeasureElements)
                        {
                            if (string.IsNullOrEmpty(measures))
                                DataManager.CurrentReport.SeriesElements.RemoveAt(i);
                            else
                            {
                                var item = DataManager.CurrentReport.SeriesElements[i].ElementValue as MeasureElements;
                                for (int j = item.Elements.Count - 1; j >= 0; j--)
                                {
                                    if (!measures.Contains(item.Elements[j].ElementName))
                                        item.Elements.RemoveAt(j);
                                }
                            }
                            isValid = false;
                        }
                    }
                }
                if (isValid)
                {
                    for (int i = DataManager.CurrentReport.SlicerElements.Count - 1; i >= 0; i--)
                    {
                        if (DataManager.CurrentReport.SlicerElements[i].ElementValue is MeasureElements)
                        {
                            if (string.IsNullOrEmpty(measures))
                                DataManager.CurrentReport.SlicerElements.RemoveAt(i);
                            else
                            {
                                var item = DataManager.CurrentReport.SlicerElements[i].ElementValue as MeasureElements;
                                for (int j = item.Elements.Count - 1; j >= 0; j--)
                                {
                                    if (!measures.Contains(item.Elements[j].ElementName))
                                        item.Elements.RemoveAt(j);
                                }
                            }
                            isValid = false;
                        }
                    }
                }
            }
            else
            {
                string[] editOkArgs = clientParams.Split(new string[] { "CHECKED" }, StringSplitOptions.None);
                Item reportItem = null; bool isSlicer = false;

                foreach (var element in DataManager.CurrentReport.SeriesElements)
                {
                    reportItem = GetReportItem(clientParams, element);
                    if (reportItem != null)
                        break;
                }
                if (reportItem == null)
                    foreach (var element in DataManager.CurrentReport.CategoricalElements)
                    {
                        reportItem = GetReportItem(clientParams, element);
                        if (reportItem != null)
                            break;
                    }
                if (reportItem == null)
                    foreach (var element in DataManager.CurrentReport.SlicerElements)
                    {
                        reportItem = GetReportItem(clientParams, element);
                        if (reportItem != null)
                        {
                            isSlicer = true;
                            break;
                        }
                    }

                if (reportItem != null)
                {
                    DimensionElement _inlcudeElement = reportItem.ElementValue as DimensionElement;
                    DimensionElement _excludeElement = reportItem.ExcludedElementValue == null ? null : reportItem.ExcludedElementValue as DimensionElement;
                    var unCheckedNodes = editOkArgs[0].Split(new String[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                    var checkedNodes = editOkArgs.Length < 2 ? new string[1] { "" } : editOkArgs[1].Split(new String[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                    if (isSlicer)
                    {
                        foreach (var item in checkedNodes)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                var values = item.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                                if (values.Length > 1)
                                {
                                    Member _member = new Member() { LevelDepth = Convert.ToInt32(values[0].Remove(0, values[0].Length - 1)), Name = values[0].Replace("_", " ").Remove(values[0].Length - 2), UniqueName = values[1].Split(new String[] { "<<" }, StringSplitOptions.RemoveEmptyEntries)[0], LevelUniqueName = values[1].Split(new String[] { "<<" }, StringSplitOptions.RemoveEmptyEntries)[1] };
                                    this.RemoveExcludedDimensionIfExists(ref _excludeElement, _member, _inlcudeElement, _inlcudeElement.Hierarchy, _inlcudeElement.Hierarchy.LevelElements[0]);
                                }
                            }
                        }
                        foreach (var item in unCheckedNodes)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                var values = item.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                                if (values.Length > 1)
                                {
                                    Member _member = new Member() { LevelDepth = Convert.ToInt32(values[0].Remove(0, values[0].Length - 1)), Name = values[0].Replace("_", " ").Remove(values[0].Length - 2), UniqueName = values[1].Split(new String[] { "<<" }, StringSplitOptions.RemoveEmptyEntries)[0], LevelUniqueName = values[1].Split(new String[] { "<<" }, StringSplitOptions.RemoveEmptyEntries)[1] };
                                    this.RemoveExcludedDimensionIfExists(ref _excludeElement, _member, _inlcudeElement, _inlcudeElement.Hierarchy, _inlcudeElement.Hierarchy.LevelElements[0]);
                                    this.GetExcludedDimension(ref _excludeElement, _member, _inlcudeElement, _inlcudeElement.Hierarchy, _inlcudeElement.Hierarchy.LevelElements[0]);
                                }
                            }
                        }
                        reportItem.ExcludedElementValue = _excludeElement;
                        this.LoadElementsToLevel(reportItem,DataManager);
                    }
                    else
                    {
                        foreach (var item in checkedNodes)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                var values = item.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                                if (values.Length > 1)
                                {
                                    Member _member = new Member() { LevelDepth = Convert.ToInt32(values[0].Remove(0, values[0].Length - 1)), Name = values[0].Replace("_", " ").Remove(values[0].Length - 2), UniqueName = values[1].Split(new String[] { "<<" }, StringSplitOptions.RemoveEmptyEntries)[0], LevelUniqueName = values[1].Split(new String[] { "<<" }, StringSplitOptions.RemoveEmptyEntries)[1] };
                                    this.RemoveExcludedDimensionIfExists(ref _excludeElement, _member, _inlcudeElement, _inlcudeElement.Hierarchy, _inlcudeElement.Hierarchy.LevelElements[0]);
                                }
                            }
                        }
                        foreach (var item in unCheckedNodes)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                var values = item.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                                if (values.Length > 1)
                                {
                                    Member _member = new Member() { LevelDepth = Convert.ToInt32(values[0].Remove(0, values[0].Length - 1)), Name = values[0].Replace("_", " ").Remove(values[0].Length - 2), UniqueName = values[1].Split(new String[] { "<<" }, StringSplitOptions.RemoveEmptyEntries)[0], LevelUniqueName = values[1].Split(new String[] { "<<" }, StringSplitOptions.RemoveEmptyEntries)[1] };
                                    this.RemoveExcludedDimensionIfExists(ref _excludeElement, _member, _inlcudeElement, _inlcudeElement.Hierarchy, _inlcudeElement.Hierarchy.LevelElements[0]);
                                    this.GetExcludedDimension(ref _excludeElement, _member, _inlcudeElement, _inlcudeElement.Hierarchy, _inlcudeElement.Hierarchy.LevelElements[0]);
                                    this.RemoveExcludedDimensionIfExists(ref _inlcudeElement, _member, _inlcudeElement, _inlcudeElement.Hierarchy, _inlcudeElement.Hierarchy.LevelElements[0]);
                                }
                            }
                        }
                        reportItem.ExcludedElementValue = _excludeElement;
                    }
                }
            }
            int elementPosition = CheckSortElements( DataManager.CurrentReport.CategoricalElements);
            if (elementPosition>=0)
            {
                DataManager.CurrentReport.CategoricalElements.RemoveAt(elementPosition);
            }
            elementPosition = CheckSortElements(DataManager.CurrentReport.SeriesElements);
            if (elementPosition >= 0)
            {
                DataManager.CurrentReport.SeriesElements.RemoveAt(elementPosition);
            }
            
            if (DataManager.CurrentReport.FilterElements.Count > 0)
            {
                DataManager.CurrentReport.FilterElements.Clear();
                DataManager.CurrentReport.CategoricalElements.IsFilterOrSortOn = false;
                DataManager.CurrentReport.SeriesElements.IsFilterOrSortOn = false;
                DataManager.CurrentReport.SlicerElements.IsFilterOrSortOn = false;
                DataManager.CurrentReport.ShowExpanders = true;
               // OnUpdateFilterElement(DataManager);
            }
            return AddReports(DataManager);
        }

        private DimensionElement GetExcludedDimension(ref DimensionElement excludedDimensionElement, Member excludedMember, DimensionElement dimensionObj, HierarchyElement hierarchyObj, LevelElement levelObj)
        {
            if (excludedDimensionElement == null)
            {
                excludedDimensionElement = new DimensionElement();
                excludedDimensionElement.Name = dimensionObj.Name;
                excludedDimensionElement.AddLevel(hierarchyObj.Name, levelObj.Name);
            }

            LevelElement levelElement = null;
            foreach (LevelElement lvlElements in excludedDimensionElement.Hierarchy.LevelElements)
            {
                if (lvlElements.UniqueName == excludedMember.LevelUniqueName || lvlElements.UniqueName == hierarchyObj.UniqueName + "." + excludedMember.LevelUniqueName)
                {
                    levelElement = lvlElements;
                }
            }

            if (levelElement != null)
            {
                bool isValid = true;
                foreach (MemberElement element in levelElement.MemberElements)
                {
                    if (element.UniqueName == excludedMember.UniqueName)
                    {
                        isValid = false;
                        break;
                    }
                }
                if (isValid)
                {
                    levelElement.IncludeAvailableMembers = true;
                    if (string.IsNullOrEmpty(excludedMember.UniqueName))
                    {
                        levelElement.Add(excludedMember.Name, excludedMember.LevelDepth);
                    }
                    else
                    {
                        levelElement.Add(excludedMember.Name, excludedMember.UniqueName, excludedMember.LevelDepth);
                    }
                }
            }
            else
            {
                string[] s = excludedMember.LevelUniqueName.Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                string levelName;
                if (s.Length == 3)
                    levelName = s[2];
                else
                    levelName = s[4];
                excludedDimensionElement.Hierarchy.Add(levelName);
                if (string.IsNullOrEmpty(excludedMember.UniqueName))
                {
                    excludedDimensionElement.Hierarchy.LevelElements[levelName].Add(excludedMember.Name, excludedMember.LevelDepth);
                }
                else
                {
                    excludedDimensionElement.Hierarchy.LevelElements[levelName].Add(excludedMember.Name, excludedMember.UniqueName, excludedMember.LevelDepth);
                }
                excludedDimensionElement.Hierarchy.LevelElements[levelName].IncludeAvailableMembers = true;
            }
            return excludedDimensionElement;
        }

        private void RemoveExcludedDimensionIfExists(ref DimensionElement excludedDimensionElement, Member excludedMember, DimensionElement dimensionObj, HierarchyElement hierarchyObj, LevelElement levelObj)
        {
            if (excludedDimensionElement != null)
            {
                LevelElement levelElement = null;
                foreach (LevelElement lvlElements in excludedDimensionElement.Hierarchy.LevelElements)
                {
                    if (lvlElements.UniqueName == excludedMember.LevelUniqueName || lvlElements.UniqueName == hierarchyObj.UniqueName + "." + excludedMember.LevelUniqueName || lvlElements.UniqueName == dimensionObj.UniqueName + "." + excludedMember.LevelUniqueName)
                    {
                        levelElement = lvlElements;
                    }
                }

                if (levelElement != null)
                {
                    for (int i = levelElement.MemberElements.Count - 1; i >= 0; i--)
                    {
                        if (levelElement.MemberElements[i].UniqueName == excludedMember.UniqueName)
                        {
                            levelElement.MemberElements.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        private static Item GetReportItem(string value, Item element)
        {
            if (element.ElementValue is DimensionElement)
            {
                if (value.Contains((element.ElementValue as DimensionElement).Hierarchy.UniqueName) || value.Contains(element.ElementValue.Name + "." + (element.ElementValue as DimensionElement).Hierarchy.Name))
                {
                    return element;
                }
            }
            return null;
        }

        public OlapDataManager OnSplitButtonRemove(OlapDataManager DataManager, string clientParams)
        {
            string[] editorArgs = clientParams.Split(':');

            var axisName = editorArgs[0];
            string heirachy = editorArgs[1];

            string rowfilterState = string.Empty, columnfilterState = string.Empty;
            if (axisName == "Columns")
            {
                for (int i = DataManager.CurrentReport.CategoricalElements.Count - 1; i >= 0; i--)
                {
                    Item catItem = DataManager.CurrentReport.CategoricalElements[i];
                    if (catItem.ElementValue is MeasureElements && editorArgs[1] == "Measures")
                    {
                        DataManager.CurrentReport.CategoricalElements.RemoveAt(i);
                        isMeasuresRemoved = true;
                    }
                    else if (catItem.ElementValue is DimensionElement && (catItem.ElementValue.Name + "." + (catItem.ElementValue as DimensionElement).Hierarchy.Name) == heirachy)
                        DataManager.CurrentReport.CategoricalElements.RemoveAt(i);
                    else if (catItem.ElementValue is NamedSetElement && catItem.ElementValue.Name == heirachy)
                        DataManager.CurrentReport.CategoricalElements.RemoveAt(i);
                }
            }
            else if (axisName == "Rows")
            {
                for (int i = DataManager.CurrentReport.SeriesElements.Count - 1; i >= 0; i--)
                {
                    Item seriesItem = DataManager.CurrentReport.SeriesElements[i];
                    if (seriesItem.ElementValue is MeasureElements && editorArgs[1] == "Measures")
                    {
                        DataManager.CurrentReport.SeriesElements.RemoveAt(i);
                        isMeasuresRemoved = true;
                    }
                    else if (seriesItem.ElementValue is DimensionElement && (seriesItem.ElementValue.Name + "." + (seriesItem.ElementValue as DimensionElement).Hierarchy.Name) == heirachy)
                        DataManager.CurrentReport.SeriesElements.RemoveAt(i);
                    else if (seriesItem.ElementValue is NamedSetElement && seriesItem.ElementValue.Name == heirachy)
                        DataManager.CurrentReport.SeriesElements.RemoveAt(i);
                }
            }
            else if (axisName == "Slicers")
            {
                for (int i = DataManager.CurrentReport.SlicerElements.Count - 1; i >= 0; i--)
                {
                    Item slicerItem = DataManager.CurrentReport.SlicerElements[i];
                    if (slicerItem.ElementValue is MeasureElements && editorArgs[1] == "Measures")
                    {
                        DataManager.CurrentReport.SlicerElements.RemoveAt(i);
                        isMeasuresRemoved = true;
                    }
                    else if (slicerItem.ElementValue is DimensionElement && (slicerItem.ElementValue.Name + "." + (slicerItem.ElementValue as DimensionElement).Hierarchy.Name) == heirachy)
                        DataManager.CurrentReport.SlicerElements.RemoveAt(i);
                    else if (slicerItem.ElementValue is NamedSetElement && slicerItem.ElementValue.Name == heirachy)
                        DataManager.CurrentReport.SlicerElements.RemoveAt(i);
                }
            }
            if (DataManager.CurrentReport.FilterElements.Count > 0)
            {
                OnUpdateFilterElement(DataManager);
            }
            return AddReports(DataManager);
        }

        public OlapDataManager OnElementDropped(OlapDataManager dataManager, string eventArgument, string droppedElement)
        {
            string[] droppedElementValue = new string[4];
            droppedElementValue = eventArgument.Split(new string[] { "--" }, StringSplitOptions.None);
            int index = droppedElementValue[3] != string.Empty ? Int32.Parse(droppedElementValue[3]) : -1;

            if (droppedElement == "TreeNode" && droppedElementValue.Length > 1)
            {
                foreach (CubeInfo item in dataManager.DataProvider.GetCubes)
                {
                    if (item.Caption.Equals(droppedElementValue[0]))
                        dataManager.CurrentCubeName =
                            dataManager.CurrentReport.CurrentCubeName = item.Name;
                }
                MetaTreeNode m_metaTreeNode = this.MetaTreeNode(dataManager, droppedElementValue[1]);
                if (m_metaTreeNode != null)
                {
                    MetaTreeNode parentDimensionNode = m_metaTreeNode.GetRootNode().Clone() as MetaTreeNode;
                    if (m_metaTreeNode.NodeType != MetaTreeNodeType.DisplayFolder &&
                        m_metaTreeNode.NodeType != MetaTreeNodeType.MeasureGroup)
                    {
                        AxisPosition axis = (AxisPosition)Enum.Parse(typeof(AxisPosition), droppedElementValue[2]);
                        MetaTreeNode metaTreeNode = GetDroppedChildren(dataManager, m_metaTreeNode, true, axis);
                        this.UpdateOlapDataManagerElementItems(dataManager, axis, metaTreeNode, index);
                    }
                }
            }

            if (droppedElement == "SplitButton")
            {
                string[] argInfo = droppedElementValue[1].Split(':');
                Items addInCollection = null, removeInCollection = null;

                if (argInfo[0] == "Columns")
                    removeInCollection = dataManager.CurrentReport.CategoricalElements;
                else if (argInfo[0] == "Rows")
                    removeInCollection = dataManager.CurrentReport.SeriesElements;
                else
                    removeInCollection = dataManager.CurrentReport.SlicerElements;

                if (droppedElementValue[2] == "Categorical")
                    addInCollection = dataManager.CurrentReport.CategoricalElements;
                else if (droppedElementValue[2] == "Series")
                    addInCollection = dataManager.CurrentReport.SeriesElements;
                else
                    addInCollection = dataManager.CurrentReport.SlicerElements;

                for (int i = removeInCollection.Count - 1; i >= 0; i--)
                {
                    if (removeInCollection[i].ElementValue is MeasureElements && argInfo[1].Contains("Measure"))
                    {
                        var draggedItem = removeInCollection[i];
                        removeInCollection.RemoveAt(i);
                        if (index < 0)
                            addInCollection.Add(draggedItem);
                        else
                            addInCollection.List.Insert(index, draggedItem);
                    }
                    else if (removeInCollection[i].ElementValue is DimensionElement &&
                        removeInCollection[i].ElementValue.Name == argInfo[1].Split('.')[0] &&
                        (removeInCollection[i].ElementValue as DimensionElement).HierarchyName ==
                        argInfo[1].Split('.')[1])
                    {
                        var draggedItem = removeInCollection[i];
                        if (argInfo[0] == "Rows" && droppedElementValue[2] == "Slicer" && dataManager.CurrentReport.SeriesElements[i].ElementValue is DimensionElement)
                        {
                            LevelElementCollection levelElementCollection = (dataManager.CurrentReport.SeriesElements[i].ElementValue as DimensionElement).Hierarchy.LevelElements;
                            foreach (LevelElement level in levelElementCollection)
                            {
                                level.MemberElements.Clear();
                            }
                            if (dataManager.CurrentReport.SeriesElements[i].ExcludedElementValue != null)
                                this.LoadElementsToLevel(dataManager.CurrentReport.SeriesElements[i], dataManager);
                            removeInCollection.RemoveAt(i);
                            if (index < 0)
                                dataManager.CurrentReport.SlicerElements.Add(draggedItem);
                            else
                                dataManager.CurrentReport.SlicerElements.List.Insert(index, draggedItem);
                        }
                        else if (argInfo[0] == "Columns" && droppedElementValue[2] == "Slicer" && dataManager.CurrentReport.CategoricalElements[i].ElementValue is DimensionElement)
                        {
                            LevelElementCollection levelElementCollection = (dataManager.CurrentReport.CategoricalElements[i].ElementValue as DimensionElement).Hierarchy.LevelElements;
                            foreach (LevelElement level in levelElementCollection)
                            {
                                level.MemberElements.Clear();
                            }
                            if (dataManager.CurrentReport.CategoricalElements[i].ExcludedElementValue != null)
                                this.LoadElementsToLevel(dataManager.CurrentReport.CategoricalElements[i], dataManager);
                            removeInCollection.RemoveAt(i);
                            if (index < 0)
                                dataManager.CurrentReport.SlicerElements.Add(draggedItem);
                            else
                                dataManager.CurrentReport.SlicerElements.List.Insert(index, draggedItem);
                        }
                        else if (argInfo[0] == "Slicers" && droppedElementValue[2] == "Series" && dataManager.CurrentReport.SlicerElements[i].ElementValue is DimensionElement)
                        {
                            LevelElementCollection levelElementCollection = (dataManager.CurrentReport.SlicerElements[i].ElementValue as DimensionElement).Hierarchy.LevelElements;
                            foreach (LevelElement level in levelElementCollection)
                            {
                                level.MemberElements.Clear();
                            }
                            removeInCollection.RemoveAt(i);
                            if (index < 0)
                                dataManager.CurrentReport.SeriesElements.Add(draggedItem);
                            else
                                dataManager.CurrentReport.SeriesElements.List.Insert(index, draggedItem);
                        }
                        else if (argInfo[0] == "Slicers" && droppedElementValue[2] == "Categorical" && dataManager.CurrentReport.SlicerElements[i].ElementValue is DimensionElement)
                        {
                            LevelElementCollection levelElementCollection = (dataManager.CurrentReport.SlicerElements[i].ElementValue as DimensionElement).Hierarchy.LevelElements;
                            foreach (LevelElement level in levelElementCollection)
                            {
                                level.MemberElements.Clear();
                            }
                            removeInCollection.RemoveAt(i);
                            if (index < 0)
                                dataManager.CurrentReport.CategoricalElements.Add(draggedItem);
                            else
                                dataManager.CurrentReport.CategoricalElements.List.Insert(index, draggedItem);
                        }
                        else
                        {
                            removeInCollection.RemoveAt(i);
                            if (index < 0)
                                addInCollection.Add(draggedItem);
                            else
                                addInCollection.List.Insert(index, draggedItem);
                        }
                    }
                    else if (removeInCollection[i].ElementValue is NamedSetElement &&
                        removeInCollection[i].ElementValue.Name == argInfo[1].Split('.')[0])
                    {
                        var draggedItem = removeInCollection[i];
                        removeInCollection.RemoveAt(i);
                        if (index < 0)
                            addInCollection.Add(draggedItem);
                        else
                            addInCollection.List.Insert(index, draggedItem);
                    }
                }
            }
            if (dataManager.CurrentReport.FilterElements.Count > 0)
            {
                OnUpdateFilterElement(dataManager);
            }
            return AddReports(dataManager);
        }

        internal MetaTreeNode LoadElementsToLevel(Item item, OlapDataManager dataManager)
        {
            CubeSchema m_cubeSchema = dataManager.CurrentCubeSchema;
            if (m_cubeSchema == null)
            {
                m_cubeSchema = dataManager.DataProvider.GetCubeSchema(dataManager.CurrentCubeName);
            }
            if (item.ElementValue is DimensionElement)
            {
                #region Load Dimension elements

                //// Extracting the selected dimension element
                DimensionElement dimensionElement = item.ElementValue as DimensionElement;
                //// Extracting the excluded dimension element 
                DimensionElement excludedDimensionElement = item.ExcludedElementValue as DimensionElement;

                List<string> excludeElements = new List<string>();
                int maxLevel = 0;
                if (excludedDimensionElement != null)
                {
                    LevelElementCollection levelElementCollection = excludedDimensionElement.Hierarchy.LevelElements;
                    foreach (LevelElement _levelElement in levelElementCollection)
                    {
                        MemberElementCollection memberElementCollection = _levelElement.MemberElements;
                        foreach (MemberElement memberElement in memberElementCollection)
                        {
                            maxLevel = memberElement.Level > maxLevel ? memberElement.Level : maxLevel;
                            excludeElements.Add(memberElement.UniqueName);
                        }
                    }
                }

                string dimensionName = dimensionElement.Name;
                if (dimensionElement.Name != string.Empty)
                {
                    //// Getting the dimension by element unique name
                    Dimension dimensionObj = m_cubeSchema.GetDimensionByUniqueName(dimensionElement.UniqueName);
                    string dimensionCaption = dimensionObj.Caption;
                    string dimensionDescription = dimensionObj.Description;
                    string dimensionUniqueName = dimensionObj.UniqueName;
                    //// Creating dimension node
                    MetaTreeNode metaTreeNode = new MetaTreeNode(dimensionName, dimensionCaption, dimensionDescription);
                    if (dimensionObj != null)
                    {
                        metaTreeNode.Properties.Add(new Property(PropertyConstants.Dimension, dimensionObj));
                    }
                    metaTreeNode.UniqueName = dimensionUniqueName.Replace("[", "").Replace("]", "");
                    //// Extracting the level element
                    LevelElement levelElement = dimensionElement.Hierarchy.LevelElements[0];
                    foreach (Hierarchy hierarchyObj in dimensionObj.Hierarchies)
                    {
                        if (hierarchyObj.UniqueName == dimensionElement.Hierarchy.UniqueName || dimensionObj.UniqueName + ".[" + hierarchyObj.Name + "]" == dimensionElement.Hierarchy.UniqueName)
                        {
                            //// Adding the hierarchy object to the MetaTreeNode properties
                            metaTreeNode.Properties.Add(new Property(PropertyConstants.Hierarchy, hierarchyObj));
                            foreach (Level levelObj in hierarchyObj.Levels)
                            {
                                if (levelObj.UniqueName == levelElement.UniqueName || dimensionObj.UniqueName + ".[" + hierarchyObj.Name + "].[" + levelObj.Name + "]" == levelElement.UniqueName)
                                {
                                    //// Adding the level to the MetaTreeNode properties
                                    metaTreeNode.Properties.Add(new Property(PropertyConstants.Level, levelObj));
                                    foreach (Member memberObj in levelObj.Members)
                                    {
                                        MetaTreeNode metaTreeNodeMembers = new MetaTreeNode();
                                        MetaTreeHelper.FillMetaTreeNode(metaTreeNodeMembers, memberObj, true, true, maxLevel);
                                        metaTreeNode.ChildNodes.Add(metaTreeNodeMembers);
                                    }
                                    MetaTreeNode _mtNode = null;
                                    foreach (string elementUniqueName in excludeElements)
                                    {
                                        _mtNode = this.GetMetaTreeNode(metaTreeNode, elementUniqueName);

                                        if (_mtNode != null && (_mtNode.Name != String.Empty || _mtNode.Caption != string.Empty))
                                        {
                                            _mtNode.IsSelected = false;
                                            _mtNode.AcceptIsSelectedChanges(true);
                                        }
                                    }
                                    item.ElementValue = QueryBuilderEngineHelper.GetElementValue(metaTreeNode, true);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    //// To identify the dragsource adding a identifier property
                    metaTreeNode.Properties.Add(new Property(PropertyConstants.AxisElements, this));

                    return metaTreeNode;
                }
                #endregion
            }
            return null;
        }

        private MetaTreeNode GetMetaTreeNode(MetaTreeNode mtNode, string uniqueName)
        {
            if (mtNode.UniqueName.ToUpper() == uniqueName.ToUpper())
            {
                return mtNode;
            }
            foreach (MetaTreeNode _mtNode in mtNode.ChildNodes)
            {
                MetaTreeNode __mtNode = GetMetaTreeNode(_mtNode, uniqueName);
                if (__mtNode != null)
                {
                    return __mtNode;
                }
            }
            return null;
        }
        private OlapDataManager AddReports(OlapDataManager Datamanager)
        {
            for (var i = 0; i < Datamanager.Reports.Count; i++)
            {
                if (Datamanager.Reports[i].Name == Datamanager.CurrentReport.Name)
                {
                    Datamanager.Reports[i] = Datamanager.CurrentReport;
                    break;
                }
            }
            return Datamanager;
        }
        private MetaTreeNode GetDroppedChildren(OlapDataManager dataManger, MetaTreeNode mtNode, bool isDropped, AxisPosition axis)
        {
            MetaTreeNode parentDimensionNode = mtNode.GetRootNode().Clone() as MetaTreeNode;
            CubeSchema cubeSchema = dataManger.CurrentCubeSchema;
            if (cubeSchema == null)
                cubeSchema = dataManger.DataProvider.GetCubeSchema(dataManger.CurrentCubeName);
            MetaTreeNode metaTreeNode = new MetaTreeNode();
            if (mtNode.NodeType == MetaTreeNodeType.Level && dataManger.DataProvider.ProviderName != Syncfusion.Olap.DataProvider.Providers.ActivePivot)
                mtNode = mtNode.ParentNode;
            if (mtNode.NodeType == MetaTreeNodeType.Dimension)
            {
                Dimension dimensionObj = cubeSchema.GetDimensionByUniqueName(mtNode.UniqueName);
                if (dimensionObj != null)
                {
                    metaTreeNode = this.AddDimension(dimensionObj);
                }
            }
            else if (mtNode.NodeType == MetaTreeNodeType.Hierarchy)
            {
                Hierarchy hierarchyObj = cubeSchema.GetHierarchyByUniqueName(mtNode.UniqueName);
                Dimension dimensionObj = hierarchyObj.ParentDimension;
                metaTreeNode = this.AddHierarchy(hierarchyObj, dimensionObj.Name, dimensionObj.Caption);
            }
            else if (mtNode.NodeType == MetaTreeNodeType.Level)
            {
                Level level = cubeSchema.GetLevelByUniqueName(mtNode.UniqueName);
                Hierarchy parentHierarchy = level.ParentHierarchy;
                Dimension parentDimension = parentHierarchy.ParentDimension;
                metaTreeNode = this.AddMembers(parentHierarchy, level, parentDimension.Name, parentDimension.Caption);
            }
            else if (mtNode.NodeType == MetaTreeNodeType.NamedSet)
            {
                Dimension dimensionObj = parentDimensionNode.Properties.FindByName(PropertyConstants.Dimension).Value as Dimension;
                NamedSet namedSetObj = (NamedSet)mtNode.Properties.FindByName(PropertyConstants.NamedSet).Value;
                metaTreeNode.UniqueName = namedSetObj.ParentDimensionName;
                metaTreeNode.Caption = namedSetObj.ParentDimensionName;
                metaTreeNode.Name = namedSetObj.ParentDimensionName;
                metaTreeNode.NodeType = MetaTreeNodeType.NamedSet;
                MetaTreeNode _metaTreeNode = new MetaTreeNode(namedSetObj.Name, namedSetObj.Name, namedSetObj.Description);
                _metaTreeNode.UniqueName = namedSetObj.ParentDimensionName;
                _metaTreeNode.NodeType = MetaTreeNodeType.NamedSet;
                Property propertyMember = _metaTreeNode.Properties.FindByName(PropertyConstants.NamedSet);
                if (propertyMember == null)
                {
                    metaTreeNode.Properties.Add(new Property(PropertyConstants.NamedSet, namedSetObj));
                    metaTreeNode.Properties.Add(new Property(PropertyConstants.Dimension, dimensionObj));

                    _metaTreeNode.Properties.Add(new Property(PropertyConstants.NamedSet, namedSetObj));
                    _metaTreeNode.Properties.Add(new Property(PropertyConstants.Dimension, dimensionObj));
                }
                _metaTreeNode.ParentNode = metaTreeNode;
                _metaTreeNode.SetIsChecked(true, false, true);
                metaTreeNode.ChildNodes.Add(_metaTreeNode);
            }
            else if (mtNode.Properties[0].Value is MeasureCollection)
            {
                MeasureCollection selectedMeasure = UpdateSelectedMeasures(mtNode);
                if (axis == AxisPosition.Slicer)
                {
                    foreach (Measure measure in selectedMeasure)
                    {
                        if (measure.MeasureAggregator != 1)
                        {
                            throw new Exception("Calculated Member cannot be in Sliced");
                        }
                    }
                }
            }
            else if (mtNode.NodeType == MetaTreeNodeType.Measure)
            {
                MetaTreeNode measureGroupNode = GetParentGroupMetaTreeeNode(dataManger, axis, mtNode);
                if (measureGroupNode == null)
                {
                    for (int i = 0; i < dataManger.CurrentReport.CategoricalElements.Count; i++)
                    {
                        if (dataManger.CurrentReport.CategoricalElements[i].ElementValue is MeasureElements)
                        {
                            measureGroupNode = this.LoadElements(dataManger.CurrentCubeSchema, dataManger.CurrentReport.CategoricalElements[i]);
                            break;
                        }
                    }
                    foreach (Item item in dataManger.CurrentReport.SeriesElements)
                    {
                        if (item.ElementValue is MeasureElements)
                        {
                            measureGroupNode = this.LoadElements(dataManger.CurrentCubeSchema, item);
                            break;
                        }
                    }
                    foreach (Item item in dataManger.CurrentReport.SlicerElements)
                    {
                        if (item.ElementValue is MeasureElements)
                        {
                            measureGroupNode = this.LoadElements(dataManger.CurrentCubeSchema, item);
                            break;
                        }
                    }
                }
                if (measureGroupNode == null)
                {
                    measureGroupNode = new MetaTreeNode(PropertyConstants.MeasrueNodeName,
                                  PropertyConstants.MeasrueNodeName, PropertyConstants.MeasrueNodeName);
                    measureGroupNode.NodeType = MetaTreeNodeType.MeasureGroup;
                    measureGroupNode.Properties.Add(new Property(PropertyConstants.MeasureGroupName, cubeSchema.Measures));
                    mtNode.IsSelected = true;
                    measureGroupNode.ChildNodes.Add(mtNode);
                    metaTreeNode = measureGroupNode;
                    mtNode.AcceptIsSelectedChanges(true);
                }
                else
                {
                    mtNode.IsSelected = true;
                    measureGroupNode.ChildNodes.Add(mtNode.Clone() as MetaTreeNode);
                    metaTreeNode = measureGroupNode;
                    mtNode.AcceptIsSelectedChanges(true);
                }
                if (axis == AxisPosition.Slicer)
                {
                    if (measureGroupNode.ChildNodes.Count > 1)
                    {
                        for (int i = measureGroupNode.ChildNodes.Count - 2; i >= 0; i--)
                            measureGroupNode.ChildNodes.RemoveAt(i);
                    }
                    else if (dataManger != null)
                    {
                        OlapReport currentReport = dataManger.CurrentReport;
                        MeasureElements measureElements = GetMeasureElements(currentReport.CategoricalElements);
                        if (measureElements == null)
                            measureElements = GetMeasureElements(currentReport.SeriesElements);

                        if (measureElements != null)
                            throw new Exception("More than one measure cannot be sliced");
                    }
                }
            }
            if (metaTreeNode == null)
            {
                metaTreeNode = mtNode;
            }
            if (dataManger != null && isDropped)
            {
                bool isRemoveIfSameDimensionElementExist = RemoveIfSameDimensionElementExist(dataManger, dataManger.CurrentReport.CategoricalElements, metaTreeNode, AxisPosition.Categorical);

                if (!isRemoveIfSameDimensionElementExist)
                {
                    isRemoveIfSameDimensionElementExist = RemoveIfSameDimensionElementExist(dataManger, dataManger.CurrentReport.SeriesElements, metaTreeNode, AxisPosition.Series);
                }
                if (!isRemoveIfSameDimensionElementExist)
                {
                    isRemoveIfSameDimensionElementExist = RemoveIfSameDimensionElementExist(dataManger, dataManger.CurrentReport.SlicerElements, metaTreeNode, AxisPosition.Slicer);
                }
                if (isRemoveIfSameDimensionElementExist)
                {

                }
            }
            if (metaTreeNode.ChildNodes.Count == 0)
            {
                metaTreeNode.NodeCheckedType = MetaTreeNodeCheckedType.CurrentChecked;
            }
            return metaTreeNode;
        }

        private MeasureElements GetMeasureElements(Items elements)
        {
            foreach (var item in elements)
            {
                if (item.ElementValue is MeasureElements)
                    return item.ElementValue as MeasureElements;
            }
            return null;
        }

        private MetaTreeNode AddDimension(Dimension dimension)
        {
            return this.AddHierarchies(dimension.Hierarchies, dimension.DefaultHierarchyName, dimension.Name, dimension.Caption);
        }

        private MetaTreeNode AddHierarchies(HierarchyCollection hierarchyCollection, string defalutHierarchyName,
                                            string dimensionName, string dimensionCaption)
        {
            if (hierarchyCollection.Count > 0)
            {
                Hierarchy hierarchyObj = hierarchyCollection.FirstOrDefault(__hierarchyObj => __hierarchyObj.UniqueName == defalutHierarchyName) ??
                                         hierarchyCollection[0];

                return this.AddHierarchy(hierarchyObj, dimensionName, dimensionCaption);
            }
            return null;
        }

        private MetaTreeNode AddHierarchy(Hierarchy hierarchyObj, string dimensionName, string dimensionCaption)
        {
            return this.AddDefaultLevelMembers(hierarchyObj, hierarchyObj.Levels, hierarchyObj.DefaultLevelUniqueName,
                                               dimensionName, dimensionCaption);
        }

        private MetaTreeNode AddMembers(Hierarchy hierarchyObj, Level levelObj, string dimensionName, string dimensionCaption)
        {
            if (levelObj != null)
            {
                var metaTreeNode = new MetaTreeNode
                {
                    Name = dimensionName,
                    Caption = dimensionCaption,
                    UniqueName = dimensionName,
                    NodeType = MetaTreeNodeType.None
                };
                Dimension dimensionObj = hierarchyObj.ParentDimension;
                metaTreeNode.Properties.Add(new Property(PropertyConstants.Dimension, dimensionObj));
                metaTreeNode.Properties.Add(new Property(PropertyConstants.Level, levelObj));
                metaTreeNode.Properties.Add(new Property(PropertyConstants.Hierarchy, hierarchyObj));
                foreach (var memberObj in levelObj.Members)
                {
                    var newMemberNode = new MetaTreeNode();
                    MetaTreeHelper.FillMetaTreeNode(newMemberNode, memberObj, true);
                    newMemberNode.ParentNode = metaTreeNode;
                    metaTreeNode.ChildNodes.Add(newMemberNode);
                }
                return metaTreeNode;
            }
            return null;
        }

        private MetaTreeNode AddDefaultLevelMembers(Hierarchy hierarchyObj, LevelCollection levelCollection,
                                                    string defaultLevelName, string dimensionName,
                                                    string dimensionCaption)
        {
            if (levelCollection.Count > 0)
            {
                Level levelObj = levelCollection.FirstOrDefault(__levelObj => __levelObj.UniqueName == defaultLevelName) ??
                                 levelCollection[0];

                return AddMembers(hierarchyObj, levelObj, dimensionName, dimensionCaption);
            }
            return null;
        }

        private MeasureCollection UpdateSelectedMeasures(MetaTreeNode metaTreeNode)
        {
            MeasureCollection measureCollection = new MeasureCollection();
            foreach (MetaTreeNode mtNode in metaTreeNode.ChildNodes)
            {
                if (mtNode.NodeType == MetaTreeNodeType.DisplayFolder)
                {
                    foreach (MetaTreeNode _mtNode in mtNode.ChildNodes)
                    {
                        if (_mtNode.NodeType == MetaTreeNodeType.Measure && _mtNode.NodeCheckedType == MetaTreeNodeCheckedType.CurrentChecked)
                        {
                            measureCollection.Add((Measure)_mtNode.Properties[0].Value);
                        }
                    }
                }
                else if (mtNode.NodeType == MetaTreeNodeType.Measure)
                {
                    if (mtNode.NodeCheckedType == MetaTreeNodeCheckedType.CurrentChecked)
                    {
                        measureCollection.Add((Measure)mtNode.Properties[0].Value);
                    }
                }
            }
            return measureCollection;
        }

        private MetaTreeNode GetParentGroupMetaTreeeNode(OlapDataManager dataManager, AxisPosition axis, MetaTreeNode mtNode)
        {
            Items items = null;
            if (axis == AxisPosition.Categorical)
                items = dataManager.CurrentReport.CategoricalElements;
            if (axis == AxisPosition.Series)
                items = dataManager.CurrentReport.SeriesElements;
            if (axis == AxisPosition.Slicer)
                items = dataManager.CurrentReport.SlicerElements;

            foreach (Item item in items)
            {
                if (item.ElementValue is MeasureElements)
                {
                    return this.LoadElements(dataManager.CurrentCubeSchema, item);
                }
            }
            return null;
        }

        private bool RemoveIfSameDimensionElementExist(OlapDataManager dataManager, Items itemCollection, MetaTreeNode mtNode, AxisPosition axis)
        {
            bool isRemoved = false;
            string[] temp_level = null;
            string[] levels = mtNode.UniqueName.Split('.');
            for (int i = 0; i < itemCollection.Count; i++)
            {
                if (!(itemCollection[i].ElementValue is SortElement) && itemCollection[i].ElementValue is DimensionElement)
                {
                    temp_level = ((DimensionElement)itemCollection[i].ElementValue).Name.Split('.');
                }
                else if (!(itemCollection[i].ElementValue is SortElement) && itemCollection[i].ElementValue is NamedSetElement)
                {
                    temp_level[0] = ((NamedSetElement)itemCollection[i].ElementValue).DimensionName;
                }

                else if (!(itemCollection[i].ElementValue is SortElement) && itemCollection[i].ElementValue is MeasureElements)
                {
                    foreach (MeasureElement measureElement in ((MeasureElements)itemCollection[i].ElementValue).Elements)
                    {
                        temp_level = measureElement.UniqueName.Split('.');
                        temp_level[0] = temp_level[0].Replace("[", "").Replace("]", "");
                    }
                }
                if (temp_level!=null && !(itemCollection[i].ElementValue is SortElement) && temp_level[0].ToLower() == levels[0].ToLower() && temp_level[0].ToLower() != "measures")
                {
                    if (((Property)((PropertyCollection)mtNode.Properties)[2]).Name == "HIERARCHY")
                    {
                        if (((DimensionElement)itemCollection[i].ElementValue).HierarchyName == ((Hierarchy)((Property)((PropertyCollection)mtNode.Properties)[2]).Value).Name)
                        {
                            if (((LevelElementCollection)((HierarchyElement)((DimensionElement)itemCollection[i].ElementValue).Hierarchy).LevelElements)[0].Name == ((Level)((Property)((PropertyCollection)mtNode.Properties)[1]).Value).Name)
                            {
                                isRemoved = RemoveIfSameDimensionElementExist(dataManager, i, axis);
                            }
                        }
                    }
                    else
                    {
                        if (!(itemCollection[i].ElementValue is SortElement) && ((DimensionElement)itemCollection[i].ElementValue).HierarchyName == ((Hierarchy)((Property)((PropertyCollection)mtNode.Properties)[1]).Value).Name)
                        {
                            if (((LevelElementCollection)((HierarchyElement)((DimensionElement)itemCollection[i].ElementValue).Hierarchy).LevelElements)[0].Name == ((Level)((Property)((PropertyCollection)mtNode.Properties)[2]).Value).Name)
                            {
                                isRemoved = RemoveIfSameDimensionElementExist(dataManager, i, axis);
                            }
                        }
                    }
                }
                else if (temp_level != null && temp_level[0].ToLower() == levels[0].ToLower() && levels[0].ToLower() == "measures")
                {
                    isRemoved = RemoveIfSameDimensionElementExist(dataManager, i, axis);
                }
            }
            return isRemoved;
        }

        private bool RemoveIfSameDimensionElementExist(OlapDataManager dataManager, int i, AxisPosition axis)
        {
            if (axis == AxisPosition.Categorical)
            {
                dataManager.CurrentReport.CategoricalElements.RemoveAt(i);
            }
            if (axis == AxisPosition.Series)
            {
                dataManager.CurrentReport.SeriesElements.RemoveAt(i);
            }
            if (axis == AxisPosition.Slicer)
            {
                dataManager.CurrentReport.SlicerElements.RemoveAt(i);
            }
            return true;
        }

        private MetaTreeNode MetaTreeNode(IOlapDataManager m_olapDataManager, string valuePath)
        {
            MetaTreeNodeCollection mTreeNodeCollection = GetTreeNodeCollection(m_olapDataManager);
            this._metaTreeNode = null;
            foreach (var mTreeNode in mTreeNodeCollection[0].ChildNodes)
            {
                if (valuePath == mTreeNode.UniqueName)
                    return mTreeNode;
                else
                    this.GetMetaTreeNode(mTreeNode.ChildNodes, valuePath);
                if (this._metaTreeNode != null)
                    return this._metaTreeNode;
            }
            return null;
        }

        private MetaTreeNodeCollection GetTreeNodeCollection(IOlapDataManager m_olapDataManager)
        {
            var mtNodes = new MetaTreeNodeCollection(null);
            var mtNode = new MetaTreeNode
            {
                NodeType = MetaTreeNodeType.Cube,
                Name = m_olapDataManager.CurrentCubeName,
                Caption = m_olapDataManager.CurrentCubeName
            };
            mtNodes.Add(mtNode);
            cubeSchema = m_olapDataManager.CurrentCubeSchema;
            MetaTreeHelper.FillMetaTreeNode(mtNode, cubeSchema.Measures, true, false);
            MetaTreeHelper.FillMetaTreeNode(mtNode, false, cubeSchema.Dimensions, cubeSchema.NamedSets);
            return mtNodes;
        }

        private void GetMetaTreeNode(MetaTreeNodeCollection metaTreeNodeCollection, string valuePath)
        {
            foreach (var mTreeNode in metaTreeNodeCollection)
            {
                if (valuePath == mTreeNode.UniqueName)
                {
                    this._metaTreeNode = mTreeNode;
                    break;
                }
                else
                    GetMetaTreeNode(mTreeNode.ChildNodes, valuePath);
            }
        }

        private void UpdateOlapDataManagerElementItems(OlapDataManager dataManager, AxisPosition axis, MetaTreeNode mtNode, int droppedPosition)
        {
            if (axis == AxisPosition.Categorical)
            {
                foreach (Item item in this.GetElementItems(dataManager, axis, mtNode))
                {
                    if (item.ElementValue is MeasureElements)
                    {
                        MeasureElements measureElements = GetMeasureElements(dataManager.CurrentReport.CategoricalElements);
                        if (measureElements != null)
                        {
                            bool isSameMeasureElementUpdate = false;
                            for (int j = 0; j < measureElements.Elements.Count; j++)
                            {
                                if (measureElements.Elements[j].UniqueName == ((MeasureElements)item.ElementValue).Elements[0].UniqueName)
                                {
                                    isSameMeasureElementUpdate = true;
                                    break;
                                }
                            }
                            if (!isSameMeasureElementUpdate)
                                measureElements.Elements.Add(((MeasureElements)item.ElementValue).Elements[0]);
                        }
                        else
                        {
                            dataManager.CurrentReport.CategoricalElements.Add(item);
                        }
                    }
                    else
                    {
                        if (droppedPosition < 0)
                            dataManager.CurrentReport.CategoricalElements.Add(item);
                        else
                            dataManager.CurrentReport.CategoricalElements.List.Insert(droppedPosition, item);
                    }
                }
            }
            else if (axis == AxisPosition.Series)
            {
                foreach (Item item in this.GetElementItems(dataManager, axis, mtNode))
                {
                    if (item.ElementValue is MeasureElements)
                    {
                        MeasureElements measureElements = GetMeasureElements(dataManager.CurrentReport.SeriesElements);
                        if (measureElements != null)
                        {
                            bool isSameMeasureElementUpdate = false;
                            for (int j = 0; j < measureElements.Elements.Count; j++)
                            {
                                if (measureElements.Elements[j].UniqueName == ((MeasureElements)item.ElementValue).Elements[0].UniqueName)
                                {
                                    isSameMeasureElementUpdate = true;
                                    break;
                                }
                            }
                            if (!isSameMeasureElementUpdate)
                                measureElements.Elements.Add(((MeasureElements)item.ElementValue).Elements[0]);
                        }
                        else
                        {
                            dataManager.CurrentReport.SeriesElements.Add(item);
                        }
                    }
                    else
                    {
                        if (droppedPosition < 0)
                            dataManager.CurrentReport.SeriesElements.Add(item);
                        else
                            dataManager.CurrentReport.SeriesElements.List.Insert(droppedPosition, item);
                    }
                }
            }
            else if (axis == AxisPosition.Slicer && mtNode != null)
            {
                foreach (Item item in this.GetElementItems(dataManager, axis, mtNode))
                {
                    if (item.ElementValue is MeasureElements)
                    {
                        MeasureElements measureElements = GetMeasureElements(dataManager.CurrentReport.SlicerElements);
                        if (measureElements != null)
                        {
                            bool isSameMeasureElementUpdate = false;
                            for (int j = 0; j < measureElements.Elements.Count; j++)
                            {
                                if (measureElements.Elements[j].UniqueName == ((MeasureElements)item.ElementValue).Elements[0].UniqueName)
                                {
                                    isSameMeasureElementUpdate = true;
                                    break;
                                }
                            }
                            if (!isSameMeasureElementUpdate)
                                measureElements.Elements.Add(((MeasureElements)item.ElementValue).Elements[0]);
                        }
                        else
                        {
                            dataManager.CurrentReport.SlicerElements.Add(item);
                        }
                    }
                    else
                    {
                        if (droppedPosition < 0)
                            dataManager.CurrentReport.SlicerElements.Add(item);
                        else
                            dataManager.CurrentReport.SlicerElements.List.Insert(droppedPosition, item);
                    }
                }
            }
        }

        private Items GetElementItems(OlapDataManager dataManager, AxisPosition axis, MetaTreeNode mtNode)
        {
            MetaTreeNodeCollection mtNodeCollection = new MetaTreeNodeCollection(null);
            MetaTreeHelper.FillSelectedNodeCollectionVersion3(mtNodeCollection, mtNode);
            mtNode.AcceptIsSelectedChanges(true);
            QueryBuilderEngineHelper.SetProviderName(dataManager.DataProvider.ProviderName);
            return QueryBuilderEngineHelper.GetElementItemsFromMetaTree(mtNodeCollection, axis);
        }

        public OlapDataManager OnCubeChanged(OlapDataManager DataManager, string cubeName)
        {
            DataManager.CurrentCubeName = cubeName;
            if (DataManager.Reports != null)
            {
                DataManager.Reports.Clear(); DataManager.CurrentReport = new OlapReport() { Name = "Default Report" };
                DataManager.CurrentReport.CurrentCubeName = cubeName;
                DataManager.Reports.Add(DataManager.CurrentReport);
            }
            return DataManager;
        }

        public OlapDataManager ReportManipulations(OlapDataManager DataManager, string reportName, string operationName)
        {
            if (operationName == "New Report" || operationName.Contains("New Report"))
            {
                DataManager.Reports.Clear();
                DataManager.SetCurrentReport(new OlapReport() { Name = reportName });
                AddReports(DataManager);
            }
            else if (operationName == "Add Report" || operationName.Contains("Add Report"))
            {
                DataManager.SetCurrentReport(new OlapReport() { Name = reportName });
                AddReports(DataManager);
            }
            else if (operationName == "Remove Report" || operationName.Contains("Remove Report"))
            {
                if (DataManager.Reports.Count > 1)
                {
                    for (int i = DataManager.Reports.Count - 1; i >= 0; i--)
                    {
                        if (DataManager.Reports[i].Name == DataManager.CurrentReport.Name)
                        {
                            DataManager.Reports.RemoveAt(i);
                            break;
                        }
                    }
                    DataManager.CurrentReport = DataManager.Reports[DataManager.Reports.Count - 1];
                }
            }
            else if (operationName == "Rename Report" || operationName.Contains("Rename Report"))
            {
                if (DataManager.Reports.Count > 0)
                {
                    for (int i = DataManager.Reports.Count - 1; i >= 0; i--)
                    {
                        if (DataManager.Reports[i].Name == DataManager.CurrentReport.Name)
                        {
                            DataManager.Reports[i].Name = reportName;
                            DataManager.CurrentReport.Name = reportName;
                            break;
                        }
                    }
                }
            }
            else if (operationName == "Report Change" || operationName.Contains("Report Change"))
            {
                DataManager.SetCurrentReport(DataManager.Reports.FindReportByName(reportName));
            }
            else if (operationName == "SortOrFilter" || operationName.Contains("SortOrFilter"))
            {
                string[] filterSortArgs = reportName.Split(new string[] { "||" }, StringSplitOptions.None);
                string[] sortingOkArgs = filterSortArgs[0].Split(new string[] { "::" }, StringSplitOptions.None);
                AxisPosition AxisPosition = (sortingOkArgs[2] == "Row")? AxisPosition.Series: AxisPosition.Categorical;
                string[] filterOkArgs = filterSortArgs[1].Split(new string[] { "::" }, StringSplitOptions.None);
                if (!string.IsNullOrEmpty(sortingOkArgs[0]) && sortingOkArgs[0] != " " && !(sortingOkArgs[0] == "Disable Sorting" || sortingOkArgs[0].Contains("Disable Sorting")))
                {
                    Syncfusion.Olap.Reports.SortOrder sortOrder = new Syncfusion.Olap.Reports.SortOrder();

                    if (sortingOkArgs[1] == "ASC" || sortingOkArgs[1].Contains("ASC"))
                    {
                        if (sortingOkArgs[3] == "PHT" || sortingOkArgs[3].Contains("PHT"))
                        {
                            sortOrder = Syncfusion.Olap.Reports.SortOrder.ASC;
                            DataManager.CurrentReport.ShowExpanders = (ValidatePreserveHierarchy(DataManager.CurrentReport, "Row") && ValidatePreserveHierarchy(DataManager.CurrentReport, "Column"));
                        }
                        else
                        {
                            sortOrder = Syncfusion.Olap.Reports.SortOrder.BASC;
                            DataManager.CurrentReport.ShowExpanders = false;
                        }
                    }

                    else if (sortingOkArgs[1] == "DESC" || sortingOkArgs[1].Contains("DESC"))
                    {
                        if (sortingOkArgs[3] == "PHT" || sortingOkArgs[3].Contains("PHT"))
                        {
                            sortOrder = Syncfusion.Olap.Reports.SortOrder.DESC;
                            DataManager.CurrentReport.ShowExpanders = (ValidatePreserveHierarchy(DataManager.CurrentReport, "Row") && ValidatePreserveHierarchy(DataManager.CurrentReport, "Column"));
                        }
                        else
                        {
                            sortOrder = Syncfusion.Olap.Reports.SortOrder.BDESC;
                            DataManager.CurrentReport.ShowExpanders = false;
                        }
                    }
                    SortElement sortElement = null;
                    if (sortingOkArgs[2] == "Row" || sortingOkArgs[2].Contains("Row"))
                    {
                        int rowValue = -1;
                        rowValue = this.CheckSortElements(DataManager.CurrentReport.SeriesElements);
                        if (rowValue >= 0)
                        {
                            ((SortElement)DataManager.CurrentReport.SeriesElements[rowValue].ElementValue).Axis = AxisPosition.Series;
                            ((SortElement)DataManager.CurrentReport.SeriesElements[rowValue].ElementValue).SortOrder = sortOrder;
                            ((SortElement)DataManager.CurrentReport.SeriesElements[rowValue].ElementValue).Element.UniqueName = sortingOkArgs[0];
                        }
                        else
                        {
                            sortElement = new SortElement(AxisPosition.Series, sortOrder, true);
                            sortElement.Element.UniqueName = sortingOkArgs[0];
                            DataManager.CurrentReport.SeriesElements.Add(new Item { ElementValue = sortElement });
                        }

                    }
                    else if (sortingOkArgs[2] == "Column" || sortingOkArgs[2].Contains("Column"))
                    {
                        int columnValue = -1;
                        columnValue = this.CheckSortElements(DataManager.CurrentReport.CategoricalElements);
                        if (columnValue >= 0)
                        {
                            ((SortElement)DataManager.CurrentReport.CategoricalElements[columnValue].ElementValue).Axis = AxisPosition.Categorical;
                            ((SortElement)DataManager.CurrentReport.CategoricalElements[columnValue].ElementValue).SortOrder = sortOrder;
                            ((SortElement)DataManager.CurrentReport.CategoricalElements[columnValue].ElementValue).Element.UniqueName = sortingOkArgs[0];
                        }
                        else
                        {
                            sortElement = new SortElement(AxisPosition.Categorical, sortOrder, true);
                            sortElement.Element.UniqueName = sortingOkArgs[0];
                            DataManager.CurrentReport.CategoricalElements.Add(new Item { ElementValue = sortElement });
                        }
                    }
                }
                else if (sortingOkArgs[0] == "Disable Sorting" || sortingOkArgs[0].Contains("Disable Sorting"))
                {
                    if (sortingOkArgs[2] == "Column")
                    {
                        int columnValue = -1;
                        columnValue = this.CheckSortElements(DataManager.CurrentReport.CategoricalElements);
                        if (columnValue >= 0)
                        {
                            DataManager.CurrentReport.CategoricalElements.RemoveAt(columnValue);
                        }
                    }
                    else if (sortingOkArgs[2] == "Row")
                    {
                        int rowValue = -1;
                        rowValue = this.CheckSortElements(DataManager.CurrentReport.SeriesElements);
                        if (rowValue >= 0)
                        {
                            DataManager.CurrentReport.SeriesElements.RemoveAt(rowValue);
                        }
                    }
                    DataManager.CurrentReport.ShowExpanders = (ValidatePreserveHierarchy(DataManager.CurrentReport, "Row") && ValidatePreserveHierarchy(DataManager.CurrentReport, "Column"));
                }
                if (filterOkArgs[0] != "" && !string.IsNullOrWhiteSpace(filterOkArgs[0]) && !(filterOkArgs[0] == "Disable Filtering" || filterOkArgs[0].Contains("Disable Filtering")))
                {
                    DataManager.CurrentReport.FilterElements.RemoveAll(AxisPosition);
                    List<FilterElement> filterElement = new List<FilterElement>();
                   
                    FilterCase filterCase = (FilterCase)Enum.Parse(typeof(FilterCase), filterOkArgs[1]);
                    FilterElement filterElements = new FilterElement(AxisPosition);
                    filterElements.FilterCase = filterCase;
                    filterElements.IsFilterCondition = true;
                    filterElements.Visible = true;

                    //// If filter is set to series elements then adding the series elements to the
                    //// filtercollection and clearing the values from the series elements
                    if (AxisPosition == AxisPosition.Series)
                    {
                        foreach (Item item in DataManager.CurrentReport.SeriesElements)
                        {
                            if(!(item.ElementValue is SortElement))
                            filterElements.Elements.Add(item.ElementValue);
                        }

                        DataManager.CurrentReport.SeriesElements.IsFilterOrSortOn = true;
                    }
                    else if (AxisPosition == AxisPosition.Categorical)
                    {
                        foreach (Item item in DataManager.CurrentReport.CategoricalElements)
                        {
                            if (!(item.ElementValue is SortElement))
                            filterElements.Elements.Add(item.ElementValue);
                        }

                        DataManager.CurrentReport.CategoricalElements.IsFilterOrSortOn = true;
                    }

                    AddFilterValues(filterOkArgs[0], filterOkArgs[2], filterElements);
                    filterElement.Add(filterElements);
                    DataManager.CurrentReport.FilterElements.Add(new Item { Axis = AxisPosition, ElementValue = filterElement[0] });
                    if (filterOkArgs.Count()>3 && !string.IsNullOrEmpty(filterOkArgs[3]) && filterOkArgs[3] != " ")
                    {
                        FilterElement dummyFilterElement = new FilterElement(AxisPosition);
                        dummyFilterElement.FilterCase = filterCase;
                        dummyFilterElement.IsFilterCondition = true;
                        dummyFilterElement.Visible = true;
                        
                        filterElement.Add(dummyFilterElement);
                        AddFilterValues(filterOkArgs[0], filterOkArgs[3], filterElement[1]);
                        DataManager.CurrentReport.FilterElements.Add(new Item { Axis = AxisPosition, ElementValue = filterElement[1] });
                    }
                    DataManager.CurrentReport.ShowExpanders = false;
                }
                else if (filterOkArgs[0] == "Disable Filtering" || filterOkArgs[0].Contains("Disable Filtering"))
                {
                    DataManager.CurrentReport.FilterElements.RemoveAll(AxisPosition);
                    if (AxisPosition == AxisPosition.Series)
                    {
                        DataManager.CurrentReport.SeriesElements.IsFilterOrSortOn = false;
                    }
                    else if (AxisPosition == AxisPosition.Categorical)
                    {
                        DataManager.CurrentReport.CategoricalElements.IsFilterOrSortOn = false;
                    }
                    DataManager.CurrentReport.ShowExpanders = (ValidatePreserveHierarchy(DataManager.CurrentReport, "Row") && ValidatePreserveHierarchy(DataManager.CurrentReport, "Column")); 
                }
                this.UpdateReportCollection(DataManager);
            }
            return DataManager;
        }
        private void AddFilterValues(string measure, string value, FilterElement filterElements)
        {
            filterElements.FilterValue.Add(new MeasureElement { UniqueName = measure, Visible = true });
            filterElements.FilterValue.Add(new FilterValue { Filter_Value = double.Parse(value), Visible = true });
        }
        private bool ValidatePreserveHierarchy(OlapReport olapreport,string axis)
        {
            int sortElementIndex = -1;
            switch (axis)
            {
                case "Row":
                    {
                        sortElementIndex = CheckSortElements(olapreport.CategoricalElements);
                        if (sortElementIndex >= 0)
                        {
                            SortElement sortElement = olapreport.CategoricalElements[sortElementIndex].ElementValue as SortElement;
                            if (sortElement.SortOrder == Syncfusion.Olap.Reports.SortOrder.BDESC || sortElement.SortOrder == Syncfusion.Olap.Reports.SortOrder.BASC)
                                return false;
                        }
                        if (olapreport.FilterElements.Count > 0)
                            foreach (Item filterElement in olapreport.FilterElements)
                            {
                                if (filterElement.Axis == AxisPosition.Categorical)
                                {
                                    return false;
                                }
                            }
                    }
                    break;
                case "Column":
                    {
                        sortElementIndex = CheckSortElements(olapreport.SeriesElements);
                        if (sortElementIndex >= 0)
                        {
                            SortElement sortElement = olapreport.SeriesElements[sortElementIndex].ElementValue as SortElement;
                            if (sortElement.SortOrder == Syncfusion.Olap.Reports.SortOrder.BDESC || sortElement.SortOrder == Syncfusion.Olap.Reports.SortOrder.BASC)
                                return false;
                        }
                        if (olapreport.FilterElements.Count > 0)
                            foreach (Item filterElement in olapreport.FilterElements)
                            {
                                if (filterElement.Axis == AxisPosition.Series)
                                {
                                    return false;
                                }
                            }
                    }
                    break;
            }
            return true;
        }
        private OlapDataManager UpdateReportCollection(OlapDataManager DataManager)
        {
            int reporCount = 0;
            foreach (OlapReport olapReport in DataManager.Reports)
            {
                if (olapReport.Name == DataManager.CurrentReport.Name)
                {
                    DataManager.Reports[reporCount] = DataManager.CurrentReport;
                    break;
                }
                reporCount += 1;
            }
            return DataManager;
        }
        private int CheckSortElements(Items items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Element sortElement = items[i].ElementValue;
                if (sortElement is SortElement)
                {
                    return i;
                }
            }
            return -1;
        }
        public OlapReportCollection DeserializedReports(string reports)
        {
            string decompressedReports = reports.Decompress();
            decompressedReports = decompressedReports.Remove(0, decompressedReports.IndexOf('<'));
            return Common.DeserializeObject<OlapReportCollection>(decompressedReports);
        }

        public List<object> GetReportList(OlapReportCollection olapReportCollection)
        {
            List<object> reportList = new List<object>();
            foreach (OlapReport report in olapReportCollection)
            {
                reportList.Add(new { name = report.Name });
            }
            return reportList;
        }

        private List<object> GetChildNodes(OlapDataManager dataManager, string cubeName, string dimensionName, string checkedState, string tag, string parentNode)
        {
            var memberCollection = dataManager.DataProvider.GetChildMembers(tag.Split(new string[] { "<<" }, StringSplitOptions.RemoveEmptyEntries)[0], cubeName, false);
            if (checkedState.ToLower() == "false")
            {
                var jsonTreeNode = memberCollection.Select(m => new { id = m.Caption.Replace(" ", "_") + "_" + m.LevelDepth, pid = parentNode, name = m.Caption, hasChildren = m.HasChildMembers, tag = m.UniqueName + "<<" + m.LevelUniqueName, checkedStatus = false }).ToList();
                return jsonTreeNode.ToList<object>();
            }
            else
            {
                Item reportItem = null;
                foreach (var element in dataManager.CurrentReport.SeriesElements)
                {
                    reportItem = GetReportItem(dimensionName, element);
                    if (reportItem != null)
                        break;
                }
                if (reportItem == null)
                    foreach (var element in dataManager.CurrentReport.CategoricalElements)
                    {
                        reportItem = GetReportItem(dimensionName, element);
                        if (reportItem != null)
                            break;
                    }
                if (reportItem == null)
                    foreach (var element in dataManager.CurrentReport.SlicerElements)
                    {
                        reportItem = GetReportItem(dimensionName, element);
                        if (reportItem != null)
                            break;
                    }

                if (reportItem != null && reportItem.ExcludedElementValue == null)
                {
                    var jsonTreeNode = memberCollection.Select(m => new { id = m.Caption.Replace(" ", "_") + "_" + m.LevelDepth, pid = parentNode, name = m.Caption, hasChildren = m.HasChildMembers, tag = m.UniqueName + "<<" + m.LevelUniqueName, checkedStatus = true }).ToList();
                    return jsonTreeNode.ToList<object>();
                }
                else if (reportItem != null)
                {
                    List<string> m_excludeElementsUniqueName = new List<string>();
                    var m_levelElements = (reportItem.ExcludedElementValue as DimensionElement).Hierarchy.LevelElements;
                    foreach (LevelElement _levelElement in m_levelElements)
                    {
                        GetExcludeElementsUniqueName(_levelElement.MemberElements, ref m_excludeElementsUniqueName);
                    }
                    var jsonTreeNode = memberCollection.Select(m => new { id = m.Caption.Replace(" ", "_") + "_" + m.LevelDepth, pid = parentNode, name = m.Caption, hasChildren = m.HasChildMembers, tag = m.UniqueName + "<<" + m.LevelUniqueName, checkedStatus = m_excludeElementsUniqueName.Contains(m.UniqueName) ? false : true }).ToList();
                    return jsonTreeNode.ToList<object>();
                }
                return null;
            }
        }

        private void GetExcludeElementsUniqueName(MemberElementCollection memberElementCollection, ref List<string> m_excludeElementsUniqueName)
        {
            foreach (MemberElement _memberElement in memberElementCollection)
            {
                if (_memberElement.ChildMemberElements.Count == 0)
                    m_excludeElementsUniqueName.Add(_memberElement.UniqueName);
                else
                    GetExcludeElementsUniqueName(_memberElement.ChildMemberElements, ref m_excludeElementsUniqueName);
            }
        }
    }
}
