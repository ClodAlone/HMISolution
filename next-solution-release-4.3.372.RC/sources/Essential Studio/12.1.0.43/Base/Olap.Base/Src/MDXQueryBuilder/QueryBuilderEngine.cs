//-------------------------------------------------------------------------------------------------
// <copyright file="QueryBuilderEngine.cs" company="syncfusion">
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
using System.Text;

#if !SILVERLIGHT
using Syncfusion.Olap.Common;
using Syncfusion.Olap.Data;
using Syncfusion.Olap.DataProvider;
using Syncfusion.Olap.Reports;
using Syncfusion.Olap.Engine;

namespace Syncfusion.Olap.MDXQueryBuilder
#else
using Syncfusion.OlapSilverlight.Reports;
using Syncfusion.OlapSilverlight.Common;
using Syncfusion.OlapSilverlight.Data;

namespace Syncfusion.OlapSilverlight.MDXQueryBuilder
#endif
{
    /// <summary>
    /// This engine generates the MDX query based on the MDXQuerySpecification
    /// </summary>
    public class QueryBuilderEngine
    {
        #region Public Methods
        /// <summary>
        /// Generates the MDX Query.
        /// </summary>
        /// <param name="mdxQuerySpecification">The MDX query specification.</param>
        /// <returns>MDX query of type string</returns>
        public static string GenerateQueryEx(MDXQuerySpecification mdxQuerySpecification)
        {
            return GenerateQueryEx(mdxQuerySpecification, false, Providers.SSAS, null, null, false, DrillType.DrillMember,true,true);
        }

        /// <summary>
        /// Generates the MDX query.
        /// </summary>
        /// <param name="mdxQuerySpecification">The MDX query specification.</param>
        /// <param name="isCount">if set to <c>true</c> [is count].</param>
        /// <returns>MDX query of type string.</returns>
        public static string GenerateQueryEx(MDXQuerySpecification mdxQuerySpecification, bool isCount, Providers ProviderName, List<SlicerRangeFiltersInfo> SlicerRangeInfoFields, SerializableDictionary<string, HeaderPositionsInfo> DrilledCells, bool ShowLevelTypeAll, DrillType DrillType, bool VisualTotalVisibility,bool UseDefaultMember)
        {
            try
            {
                return QueryBuilderEngineVersion3.GenerateQueryEx(mdxQuerySpecification, isCount, ProviderName, SlicerRangeInfoFields, DrilledCells, ShowLevelTypeAll, DrillType, VisualTotalVisibility,UseDefaultMember);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        /// <summary>
        /// This Engine generates the MDXQuery from the MDXQuerySpecification.
        /// This version includes removal of cubeSchema, Exclude and Subset feature is included.
        /// </summary>
        internal class QueryBuilderEngineVersion3
        {
            #region Variables
            static StringBuilder _columnPropertiesQueryString = new StringBuilder();
            static StringBuilder _rowPropertiesQueryString = new StringBuilder();
            #endregion

            #region GUIDs constants

            /// <summary>
            /// Represents a GUID for a column set.
            /// </summary>
            public const string ColumnGeneratedSet = "[e16a30d0-2174-4874-8dae-a5085a75a3e2]";
            /// <summary>
            /// Represents a GUID for a Row set.
            /// </summary>
            public const string RowGeneratedSet = "[d1876d2b-e50e-4547-85fe-5b8ed9d629de]";
            /// <summary>
            /// Represents a GUID for a column paged set.
            /// </summary>
            public const string ColumnPagedSet = "[a69e818b-1183-4635-af2a-08199b74b905]";
            /// <summary>
            /// Represents a GUID for a row paged set.
            /// </summary>
            public const string RowPagedSet = "[4d3bfd6d-d89a-487a-b5a9-348689f2db58]";
            /// <summary>
            /// Represents a GUID for a column count.
            /// </summary>
            public const string ColumnsCount = "[Measures].[3d268ce0-664d-4092-b9cb-fece97175006]";
            /// <summary>
            /// Represents a GUID for a row count.
            /// </summary>
            public const string RowsCount = "[Measures].[8d7fe8c1-f09f-410e-b9ba-eaab75a1fc3e]";

            #endregion

            #region Private Methods

            /// <summary>
            /// Builds the Slicer elements.
            /// </summary>
            /// <param name="items">The items of the Axis</param>
            /// <param name="query">The query to be generated</param>
            /// <param name="sortElement">The sort elements</param>
            /// <param name="filterElement">The filter elements</param>
            /// <param name="isGrandTotalOn">if set to <c>true</c> [is grand total on].</param>
            /// <returns>true when query is formed correctly</returns>
            static bool BuildSlicerElements(Items items, StringBuilder query, SortElement sortElement, List<FilterElement> filterElement, bool isGrandTotalOn, bool isCount, Providers ProviderName, bool ShowLevelTypeAll)
            {
                // bool value to identify whether KPI Elements present in this Axis
                bool isKPIpresent = false;

                #region Appending Filter Clause
                bool isFilterOn = false, isSortOn = false;

                if (filterElement != null)
                {
                    if (filterElement.Count > 0)
                    {
                        isFilterOn = filterElement[0].IsFilterCondition;
                    }
                }

                if (sortElement != null)
                {
                    isSortOn = sortElement.IsSortOn;
                }

                if (isSortOn)
                {
                    if (isFilterOn)
                    {
                        query.Append(" ORDER(FILTER");
                    }
                    else
                    {
                        query.Append(" ORDER");
                    }
                }
                else if (isFilterOn)
                {
                    query.Append(" FILTER");
                }
                #endregion

                if (items.Count > 0 && !items.IsFilterOrSortOn && ProviderName != Providers.ActivePivot)
                {
                    query.Append("{");
                }

                isKPIpresent = BuildSlicerItem(items, query, isGrandTotalOn, isCount, ProviderName, ShowLevelTypeAll);
                if (items.Count > 0 && !items.IsFilterOrSortOn && ProviderName != Providers.ActivePivot)
                {
                    query.Append("}");
                }

                return isKPIpresent;
            }
            /// <summary>
            /// Builds the axis item.
            /// </summary>
            /// <param name="items">The items.</param>
            /// <param name="query">The query.</param>
            /// <param name="isGrandTotalOn">if set to <c>true</c> [is grand total on].</param>
            /// <param name="sortElement">The sort element.</param>
            /// <param name="filterElement">The filter element.</param>
            /// <param name="topCountElement">The top count element.</param>
            /// <param name="subSetElement">The sub set element.</param>
            /// <param name="drilledCells">The drilled cells.</param>
            /// <returns>
            /// bool value representing if KPI is present or not
            /// </returns>
            private static bool BuildAxisItem(Items items, StringBuilder query, bool isGrandTotalOn, SortElement sortElement, List<FilterElement> filterElement, TopCountElement topCountElement, SubsetElement subSetElement, HeaderPositionsInfo drilledCells, Providers ProviderName, bool ShowLevelTypeAll, DrillType DrillType,bool useDefaultMember,bool isPaging,bool visualTotalVisibility)
            {
                // bool value to identify whether KPI Elements present in this Axis
                bool isKPIpresent = false;
                bool isAxisItemAppended = false;
                bool isMeasuresAppended = false;
                bool isFilterOn = false, isSortOn = false, isSubSetOn = false;
                if (subSetElement != null)
                {
                    isSubSetOn = true;
                    query.Append(" SUBSET(");
                }
                #region Appending DrillPosition
                if (DrillType == DrillType.DrillPosition && drilledCells != null && drilledCells.Count > 0)
                    query.Append("Hierarchize({");
                #endregion

                IterateAndUpdateAxis(items, query, isGrandTotalOn, sortElement, filterElement, topCountElement, subSetElement,
                    ref isKPIpresent, ref isAxisItemAppended, ref isMeasuresAppended, ref  isFilterOn, ref isSortOn, ref isSubSetOn, ProviderName, ShowLevelTypeAll, DrillType,useDefaultMember,isPaging,visualTotalVisibility);

                #region Appending DrillPosition
                if (DrillType == DrillType.DrillPosition && drilledCells != null && drilledCells.Count > 0)
                {
                    query.Append("}");
                    for (int j = 0; j < drilledCells.Count; j++)
                    {
                        query.Append("+");
                        var drillInfo = drilledCells[j];
                        var validHierarchyName = drillInfo.Select(d => d.Key).Last(h => !h.StartsWith("Empty"));
                        var item = items.List.FirstOrDefault(l => l.ElementValue is DimensionElement && (l.ElementValue as DimensionElement).HierarchyName == validHierarchyName);
                        var newItems = new Items(items.List.Where((l, i) => i > items.List.IndexOf(item)).ToList());

                        query.Append("{").Append(GenerateCellPositionMdx(HeaderPositionsInfo.GetHierarchyStrings(drillInfo)));
                        if (newItems.Count > 0)
                        {
                            query.Append("*");
                            IterateAndUpdateAxis(newItems, query, isGrandTotalOn, sortElement, filterElement, topCountElement, subSetElement,
                                ref isKPIpresent, ref isAxisItemAppended, ref isMeasuresAppended, ref  isFilterOn, ref isSortOn, ref isSubSetOn, ProviderName, ShowLevelTypeAll, DrillType,useDefaultMember,isPaging,visualTotalVisibility);
                        }
                        query.Append("}");
                    }
                    query.Append(")");
                }
                #endregion

                #region Appending Filter
                if (filterElement != null)
                {
                    if (filterElement.Count > 0)
                    {
                        ////Appending for Non-Empty
                        query.Append("(");
                    }

                    if (filterElement != null)
                    {
                        if (filterElement.Count > 0)
                        {
                            isFilterOn = filterElement[0].IsFilterCondition;
                        }
                    }
                    // NOTE: This has been commented while implementing sorting and filtering in JS.
                    if (isSortOn)
                    {
                        if (isFilterOn)
                        {
                            //query.Append("ORDER(FILTER(");
                            query.Append("(FILTER(");
                        }
                       // else
                       // {
                       //     query.Append("ORDER(");
                       // }
                    }
                    else if (isFilterOn)
                    {
                        query.Append("FILTER(");
                    }

                    for (int i = 0; i < filterElement.Count; i++)
                    {
                        int j;
                        int filterValueCount = 0;
                        bool isItemAppended = true;
                        for (j = 0; j < filterElement[i].Elements.Count; j++)
                        {
                            Element inner_item = filterElement[i].Elements[j];

                            #region DimensionElement
                            if (inner_item is DimensionElement)
                            {
                                if (j > 0 && isItemAppended)
                                {
                                    query.Append(" * ");
                                }
                                ////Appending Visual totals for getting the proper totals
                                if (ProviderName != Providers.ActivePivot && ProviderName != Providers.Mondrian)
                                {
                                    if (visualTotalVisibility)
                                        query.Append(" VISUALTOTALS(");
                                    else
                                        query.Append("(");
                                }
                                #region Appending Filter Clause
                                bool isTopCountOn = false;

                                if (topCountElement != null)
                                {
                                    isTopCountOn = true;
                                }

                                if (sortElement != null)
                                {
                                    isSortOn = sortElement.IsSortOn;
                                }
                                // NOTE: This has been commented while implementing sorting and filtering in JS.
                                //if (isSortOn)
                                //{
                                //    query.Append("ORDER");
                               // }

                                if (isTopCountOn)
                                {
                                    query.Append(" TOPCOUNT");
                                }
                                else
                                {
                                    query.Append(" ");
                                }

                                #endregion

                                ////Appending braces for the upcoming tuple
                                if (ProviderName != Providers.ActivePivot)
                                {
                                    query.Append("(");
                                }

                                BuildDimensionElement(query, isGrandTotalOn, inner_item, null, ProviderName, ShowLevelTypeAll, DrillType, useDefaultMember, isPaging, visualTotalVisibility);

                                // NOTE: This has been commented while implementing sorting and filtering in JS.
                                //#region Appending Sort
                                //if (sortElement != null && isSortOn == true)
                                //{
                                //    query.Append(", (");
                                //    query.Append(sortElement.Element.UniqueName);
                                //    query.Append("), ");
                                //    query.Append(sortElement.SortOrder.ToString());
                                //}
                                //#endregion

                                #region Appending TopCount
                                if (topCountElement != null && isTopCountOn == true)
                                {
                                    query.Append(", ");
                                    query.Append(topCountElement.FieldCount.ToString());
                                    if (topCountElement.MeasureName != string.Empty)
                                    {
                                        query.Append(", ");
                                        query.Append(topCountElement.MeasureName);
                                    }
                                }
                                #endregion

                                //// Closing the braces for the tuple
                                if (ProviderName != Providers.ActivePivot)
                                {
                                    query.Append(" )");
                                }

                                ////Closing the braces for Visual totals
                                if (ProviderName != Providers.ActivePivot && ProviderName != Providers.Mondrian)
                                {
                                    query.Append(" )");
                                }
                                isItemAppended = true;
                            }
                            #endregion

                            #region KPIElement
                            if (inner_item is KpiElements)
                            {
                                KpiElements _kpiElements = (KpiElements)inner_item;
                                if (FindIndexOfMeasureElements(filterElement[i].Elements) >= 0)
                                {
                                    if (j == 0)
                                    {
                                        isItemAppended = false;
                                    }

                                    continue;
                                }
                                else
                                {
                                    isKPIpresent = true;
                                    if (j > 0 && isItemAppended)
                                    {
                                        query.Append(" * ");
                                    }

                                    query.Append("{");
                                    for (int _i = 0; _i < _kpiElements.Elements.Count; _i++)
                                    {
                                        if (_i > 0)
                                        {
                                            query.Append(",");
                                        }

                                        bool isValueAppended = false;
                                        if (_kpiElements.Elements[_i].ShowKPIValue)
                                        {
                                            query.Append("KPIVALUE(\"" + _kpiElements.Elements[_i].Name + "\")");
                                            isValueAppended = true;
                                        }

                                        if (_kpiElements.Elements[_i].ShowKPIGoal)
                                        {
                                            if (isValueAppended)
                                            {
                                                query.Append(",");
                                            }

                                            query.Append("KPIGOAL(\"" + _kpiElements.Elements[_i].Name + "\")");
                                            isValueAppended = true;
                                        }

                                        if (_kpiElements.Elements[_i].ShowKPIStatus)
                                        {
                                            if (isValueAppended)
                                            {
                                                query.Append(",");
                                            }

                                            query.Append("KPISTATUS(\"" + _kpiElements.Elements[_i].Name + "\")");
                                            isValueAppended = true;
                                        }

                                        if (_kpiElements.Elements[_i].ShowKPITrend)
                                        {
                                            if (isValueAppended)
                                            {
                                                query.Append(",");
                                            }

                                            query.Append("KPITREND(\"" + _kpiElements.Elements[_i].Name + "\")");
                                            isValueAppended = true;
                                        }
                                    }

                                    query.Append("}");
                                    isItemAppended = true;
                                }
                            }
                            #endregion

                            #region MeasureElements
                            if (inner_item is MeasureElements)
                            {
                                MeasureElements _measureElements = (MeasureElements)inner_item;
                                KpiElements kpiElements;

                                int kpiIndex = FindIndexOfKPIElements(filterElement[i].Elements);
                                if (kpiIndex >= 0)
                                {
                                    kpiElements = (KpiElements)filterElement[i].Elements[kpiIndex];
                                    filterElement[i].Elements.RemoveAt(kpiIndex);
                                    isKPIpresent = true;
                                }
                                else
                                {
                                    kpiElements = null;
                                }

                                if (j > 0 && isItemAppended)
                                {
                                    query.Append(" * ");
                                }

                                query.Append("{");
                                for (int _i = 0; _i < _measureElements.Elements.Count; _i++)
                                {
                                    if (_i > 0)
                                    {
                                        query.Append(",");
                                    }

                                    query.Append(_measureElements.Elements[_i].UniqueName);
                                }

                                if (kpiElements != null)
                                {
                                    query.Append(",");
                                    for (int _i = 0; _i < kpiElements.Elements.Count; _i++)
                                    {
                                        if (_i > 0)
                                        {
                                            query.Append(",");
                                        }

                                        bool isValueAppended = false;
                                        if (kpiElements.Elements[_i].ShowKPIValue)
                                        {
                                            query.Append("KPIVALUE(\"" + kpiElements.Elements[_i].Name + "\")");
                                            isValueAppended = true;
                                        }

                                        if (kpiElements.Elements[_i].ShowKPIGoal)
                                        {
                                            if (isValueAppended)
                                            {
                                                query.Append(",");
                                            }

                                            query.Append("KPIGOAL(\"" + kpiElements.Elements[_i].Name + "\")");
                                            isValueAppended = true;
                                        }

                                        if (kpiElements.Elements[_i].ShowKPIStatus)
                                        {
                                            if (isValueAppended)
                                            {
                                                query.Append(",");
                                            }

                                            query.Append("KPISTATUS(\"" + kpiElements.Elements[_i].Name + "\")");
                                            isValueAppended = true;
                                        }

                                        if (kpiElements.Elements[_i].ShowKPITrend)
                                        {
                                            if (isValueAppended)
                                            {
                                                query.Append(",");
                                            }

                                            query.Append("KPITREND(\"" + kpiElements.Elements[_i].Name + "\")");
                                            isValueAppended = true;
                                        }
                                    }
                                }

                                query.Append("}");
                                isItemAppended = true;
                            }
                            #endregion

                            #region MemberElement
                            if (inner_item is MemberElement)
                            {
                                if (j > 0 && isItemAppended)
                                {
                                    query.Append(" * ");
                                }

                                MemberElement memberElement = (MemberElement)inner_item;
                                if (memberElement.ChildMemberElements.Count > 0)
                                {
                                    query.Append("{");
                                    for (int k = 0; k < memberElement.ChildMemberElements.Count; k++)
                                    {
                                        MemberElement childMemberElement = memberElement.ChildMemberElements[k];
                                        if (k > 0)
                                        {
                                            query.Append(",");
                                        }

                                        query.Append(childMemberElement.UniqueName);
                                    }

                                    query.Append("}");
                                }
                                else
                                {
                                    query.Append(memberElement.UniqueName);
                                }

                                isItemAppended = true;
                            }
                            #endregion

                            #region LevelElement
                            else if (inner_item is LevelElement)
                            {
                                if (j > 0 && isItemAppended)
                                {
                                    query.Append(" * ");
                                }

                                LevelElement levelElement = inner_item as LevelElement;
                                if (levelElement != null)
                                {
                                    if (levelElement.ParentHierarchy != null && levelElement.Name != string.Empty)
                                    {
                                        StringBuilder memberString = new StringBuilder();
                                        GetChildMemberElementSet(levelElement.MemberElements, true, levelElement, memberString, query, ProviderName, ShowLevelTypeAll, DrillType,useDefaultMember);
                                        isItemAppended = true;
                                    }
                                    else if (levelElement.ParentHierarchy == null || levelElement.ParentHierarchy.UniqueName == string.Empty)
                                    {
                                        throw new Exception("ParentHierarchy should be specified for a LevelElement");
                                    }
                                    else
                                    {
                                        throw new Exception("Please specify the Level Name");
                                    }
                                }
                            }
                            #endregion

                            #region NamedSetElement
                            else if (inner_item is NamedSetElement)
                            {
                                if (j > 0 & isAxisItemAppended)
                                {
                                    query.Append(" * ");
                                }

                                NamedSetElement namedSetElement = (NamedSetElement)inner_item;
                                query.Append("{");
                                query.Append(namedSetElement.UniqueName);
                                query.Append("}");
                            }
                            #endregion
                        }

                        if (filterElement[i].Elements.Count > 0)
                        {
                            query.Append(",\n\t\t");
                        }

                        if (filterElement.Count > 1 && i == 0)
                        {
                            query.Append("(");
                        }

                        if (filterElement[i].FilterValue.Count > 1 && filterElement[i].FilterValue.Count <= 2)
                        {
                            for (j = 0; j < filterElement[i].FilterValue.Count; j++)
                            {
                                filterValueCount = j;
                                Element inner_item = filterElement[i].FilterValue[j];

                                #region FilterValueMeasureElement
                                if (inner_item is MeasureElement)
                                {
                                    MeasureElement measureElement = inner_item as MeasureElement;
                                    if (measureElement.Visible)
                                    {
                                        query.Append(measureElement.UniqueName);
                                    }
                                }
                                else if (inner_item is DimensionElement)
                                {
                                    DimensionElement dimensionElement = inner_item as DimensionElement;
                                    if (dimensionElement.Visible)
                                    {
                                        query.Append(dimensionElement.Hierarchy.UniqueName);
                                    }
                                }
                                #endregion
                            }
                        }

                        #region Appending Numeric Cases
                        {
                            Element inner_item = (Element)filterElement[i].FilterValue[j - 1];

                            if (inner_item is FilterValue)
                            {
                                FilterValue filter = inner_item as FilterValue;
                                switch (filterElement[i].FilterCase)
                                {
                                    case FilterCase.GreaterThan:
                                        query.Append(" > ");
                                        query.Append(filter.Filter_Value.ToString());
                                        break;
                                    case FilterCase.EqualTo:
                                        query.Append(" = ");
                                        query.Append(filter.Filter_Value.ToString());
                                        break;
                                    case FilterCase.GreaterThanOrEqualTo:
                                        query.Append(" >= ");
                                        query.Append(filter.Filter_Value.ToString());
                                        break;
                                    case FilterCase.LessThan:
                                        query.Append(" < ");
                                        query.Append(filter.Filter_Value.ToString());
                                        break;
                                    case FilterCase.LessThanOrEqualTo:
                                        query.Append(" <= ");
                                        query.Append(filter.Filter_Value.ToString());
                                        break;
                                    case FilterCase.NotEquals:
                                        query.Append(" <> ");
                                        query.Append(filter.Filter_Value.ToString());
                                        break;
                                    case FilterCase.Between:
                                        if (i == 0)
                                            query.Append(" > ");
                                        else
                                            query.Append(" < ");
                                        query.Append(filter.Filter_Value.ToString());
                                        break;
                                    case FilterCase.NotBetween:
                                        if (i == 0)
                                            query.Append(" < ");
                                        else
                                            query.Append(" > ");
                                        query.Append(filter.Filter_Value.ToString());
                                        break;
                                }
                            }

                            if (filterElement.Count > 1 && (i >= 0 && i < (filterElement.Count - 1)))
                            {
                                if (filterElement[i].FilterCase == FilterCase.NotBetween)
                                    query.Append(" OR ");
                                else
                                    query.Append(" AND ");
                            }
                        }

                        if ((filterElement.Count > 1) && (i == (filterElement.Count - 1)))
                        {
                            query.Append(")");
                        }
                        #endregion

                        //if (isSortOn == true && isFilterOn == true && (i == (filterElement.Count - 1)))
                        //{
                        //    query.Append(")");
                        //}
                    }

                    if (isFilterOn)
                    {
                        query.Append(")");
                    }

                    if (filterElement.Count > 0)
                    {
                        ////Appending close braces for Non Empty
                        query.Append(")");
                    }
                }
                #endregion

                #region Appending Sort
                if (sortElement != null && isSortOn == true)
                {
                    query.Insert(0, "ORDER(");
                    query.Append(", (");
                    query.Append(sortElement.Element.UniqueName);
                    query.Append("), ");
                    query.Append(sortElement.SortOrder.ToString());
                    query.Append(")");
                }
                #endregion

                #region Appending SubSet
                if (subSetElement != null && isSubSetOn == true)
                {
                    query.Append(", ");
                    query.Append(subSetElement.StartIndex.ToString());
                    if (subSetElement.EndIndex > 0)
                    {
                        query.Append(", ");
                        query.Append(subSetElement.EndIndex.ToString());
                    }
                    query.Append(")");
                }
                #endregion

                return isKPIpresent;
            }
            static List<Item> tempElement = new List<Item>();
            static void IterateAndUpdateAxis(Items items, StringBuilder query, bool isGrandTotalOn, SortElement sortElement, List<FilterElement> filterElement, TopCountElement topCountElement, SubsetElement subSetElement,
                ref bool isKPIpresent, ref bool isAxisItemAppended, ref bool isMeasuresAppended, ref bool isFilterOn, ref bool isSortOn, ref bool isSubSetOn, Providers ProviderName, bool ShowLevelTypeAll, DrillType DrillType,bool useDefaultMember, bool isPaging,bool visualTotalVisibility)
            {
                tempElement.Clear();
                for (int index = 0; index < items.Count && !items.IsFilterOrSortOn; index++)
                {
                    Item item = items[index]; int count = 0; bool isLastItem = false; bool isSameHierarchy = false;
                    if (tempElement.Count == 0 && !(item.ElementValue is MeasureElements))
                    {
                        tempElement.AddRange(items.List.Where(i=>i.ElementValue.ElementName == item.ElementValue.ElementName));
                    }
                    else if(tempElement.Count > 0)
                    {
                        if (tempElement.Any(i => i.ElementValue.ElementName == item.ElementValue.ElementName))
                        {
                            isSameHierarchy = true;
                            count++;
                        }
                        else
                            tempElement.Add(item);
                    }

                    //for (int i = 0; i < items.Count; i++)
                    //{
                    //    if ((item.ElementValue.ElementName == items[i].ElementValue.ElementName)&&(item.Axis==items[i].Axis) && !(item.ElementValue is MeasureElements))
                    //    {
                    //        count++;
                    //    }
                    //}

                    if (index == 0 && tempElement.Count > 1)
                    {
                        isSameHierarchy = true;
                    }
                    if ((index == items.Count - 1))
                        isLastItem = true;
                    else if(isSameHierarchy)
                        isLastItem = false;
                        #region DimensionElement
                        if (item.ElementValue is DimensionElement)
                        {
                            if (index > 0 && isAxisItemAppended)
                            {
                                query.Append(" * ");
                            }
                            ////Appending Visual totals for getting the proper totals
                            if (ProviderName != Providers.ActivePivot && ProviderName != Providers.Mondrian)
                            {
                                if (visualTotalVisibility)
                                    query.Append(" VISUALTOTALS(");
                                else
                                    query.Append("(");
                            }
                            #region Appending Filter Clause
                            bool isTopCountOn = false;

                            if (filterElement != null)
                            {
                                if (filterElement.Count > 0)
                                {
                                    isFilterOn = filterElement[0].IsFilterCondition;
                                }
                            }

                            if (sortElement != null)
                            {
                                isSortOn = sortElement.IsSortOn;
                            }

                            if (topCountElement != null)
                            {
                                isTopCountOn = true;
                            }

                            if (isTopCountOn)
                            {
                                query.Append(" TOPCOUNT");
                            }
                            else
                            {
                                query.Append(" ");
                            }
                            // NOTE: This has been commented while implementing sorting and filtering in JS.
                            if (isSortOn)
                            {
                                if (isFilterOn)
                                {
                                    //query.Append("ORDER(FILTER");
                                    query.Append("(FILTER");
                                }
                                else
                                {
                                 //   query.Append("ORDER");
                                }
                            }
                            else if (isFilterOn)
                            {
                                query.Append("FILTER");
                            }
                            #endregion

                            if (ProviderName != Providers.ActivePivot)
                            {
                                query.Append("(");
                            }

                            BuildDimensionElement(query, isGrandTotalOn, item, ProviderName, ShowLevelTypeAll, DrillType, useDefaultMember,isLastItem,isPaging,isSameHierarchy,visualTotalVisibility);
                            
                            // NOTE: This has been commented while implementing sorting and filtering in JS.
                            //#region Appending Sort
                            //if (sortElement != null && isSortOn == true)
                            //{
                            //    query.Append(",\n\t\t");
                            //    query.Append(" (");
                            //    query.Append(sortElement.Element.UniqueName);
                            //    query.Append(")");
                            //    query.Append(",\n\t\t");
                            //    query.Append(sortElement.SortOrder.ToString());
                            //    isSortOn = false;
                            //}
                            //#endregion


                            #region Appending TopCount
                            if (topCountElement != null && isTopCountOn == true)
                            {
                                query.Append(", ");
                                query.Append(topCountElement.FieldCount.ToString());
                                if (topCountElement.MeasureName != string.Empty)
                                {
                                    query.Append(", ");
                                    query.Append(topCountElement.MeasureName);
                                }
                            }
                            #endregion

                            if (ProviderName != Providers.ActivePivot)
                            {
                                query.Append(") ");
                            }


                            ////Closing the braces for Visual totals
                            if (ProviderName != Providers.ActivePivot && ProviderName != Providers.Mondrian)
                            {
                                query.Append(")");
                            }

                            isAxisItemAppended = true;
                        }
                        #endregion

                        #region KPIElement
                        else if (item.ElementValue is KpiElements)
                        {
                            KpiElements _kpiElements = (KpiElements)item.ElementValue;
                            if (FindIndexOfMeasureElements(items) >= 0)
                            {
                                if (index == 0)
                                {
                                    isAxisItemAppended = false;
                                }

                                continue;
                            }
                            else
                            {
                                isKPIpresent = true;
                                if (index > 0 && isAxisItemAppended)
                                {
                                    query.Append(" * ");
                                }

                                query.Append("{");
                                for (int _i = 0; _i < _kpiElements.Elements.Count; _i++)
                                {
                                    if (_i > 0)
                                    {
                                        query.Append(",");
                                    }

                                    bool isValueAppended = false;
                                    if (_kpiElements.Elements[_i].ShowKPIValue)
                                    {
                                        query.Append("KPIVALUE(\"" + _kpiElements.Elements[_i].Name + "\")");
                                        isValueAppended = true;
                                    }

                                    if (_kpiElements.Elements[_i].ShowKPIGoal)
                                    {
                                        if (isValueAppended)
                                        {
                                            query.Append(",");
                                        }

                                        query.Append("KPIGOAL(\"" + _kpiElements.Elements[_i].Name + "\")");
                                        isValueAppended = true;
                                    }

                                    if (_kpiElements.Elements[_i].ShowKPIStatus)
                                    {
                                        if (isValueAppended)
                                        {
                                            query.Append(",");
                                        }

                                        query.Append("KPISTATUS(\"" + _kpiElements.Elements[_i].Name + "\")");
                                        isValueAppended = true;
                                    }

                                    if (_kpiElements.Elements[_i].ShowKPITrend)
                                    {
                                        if (isValueAppended)
                                        {
                                            query.Append(",");
                                        }

                                        query.Append("KPITREND(\"" + _kpiElements.Elements[_i].Name + "\")");
                                        isValueAppended = true;
                                    }
                                }

                                query.Append("}");
                                isAxisItemAppended = true;
                            }
                        }
                        #endregion

                        #region Virtual KPI
                        else if (item.ElementValue is VirtualKpiElement)
                        {
                            VirtualKpiElement _virtualKpiElement = (item.ElementValue as VirtualKpiElement);
                            bool isValueAppend = false;
                            StringBuilder virutalKPIQuery = new StringBuilder();
                            if (!string.IsNullOrEmpty(_virtualKpiElement.KpiValueExpression))
                            {
                                virutalKPIQuery.Append("[Measures].[" + _virtualKpiElement.Name + " Value]");
                                isValueAppend = true;
                            }
                            if (!string.IsNullOrEmpty(_virtualKpiElement.KpiGoalExpression))
                            {
                                if (isValueAppend)
                                {
                                    virutalKPIQuery.Append(",");
                                }
                                virutalKPIQuery.Append("[Measures].[" + _virtualKpiElement.Name + " Goal]");
                                isValueAppend = true;
                            }
                            if (!string.IsNullOrEmpty(_virtualKpiElement.KpiStatusExpression))
                            {
                                if (isValueAppend)
                                {
                                    virutalKPIQuery.Append(",");
                                }
                                virutalKPIQuery.Append("[Measures].[" + _virtualKpiElement.Name + " Status]");
                                isValueAppend = true;
                            }
                            if (!string.IsNullOrEmpty(_virtualKpiElement.KpiTrendExpression))
                            {
                                if (isValueAppend)
                                {
                                    virutalKPIQuery.Append(",");
                                }
                                virutalKPIQuery.Append("[Measures].[" + _virtualKpiElement.Name + " Trend]");
                                isValueAppend = true;
                            }

                            if ((index > 0 && isAxisItemAppended) && !isMeasuresAppended)
                            {
                                query.Append(" * ");
                                query.Append("{" + virutalKPIQuery.ToString());
                                query.Append("}");
                            }
                            else if (index > 0 && isAxisItemAppended && isMeasuresAppended)
                            {
                                string temp = query.ToString();
                                int end = temp.LastIndexOf("[" + PropertyConstants.MeasrueNodeName + "]");
                                int brace = temp.IndexOf("}", end);

                                query = query.Insert(brace, ", " + virutalKPIQuery.ToString());
                            }
                            else
                            {
                                query.Append("{" + virutalKPIQuery + "}");
                            }
                            //isKPIpresent = true;
                            isAxisItemAppended = true;
                            isMeasuresAppended = true;
                        }
                        #endregion

                        #region CalculatedMember

                        else if (item.ElementValue is CalculatedMember)
                        {
                            CalculatedMember _calcaulateMember = (CalculatedMember)item.ElementValue;
                            if (_calcaulateMember.Type == TypeOfMember.Dimension)
                            {
                                if (index > 0 && isAxisItemAppended)
                                {
                                    query.Append(" * ");
                                }
                                query.Append("{" + _calcaulateMember.UniqueName + "}");
                                isAxisItemAppended = true;
                            }
                            else if (_calcaulateMember.Type == TypeOfMember.Measure)
                            {
                                if ((index > 0 && isAxisItemAppended) && !isMeasuresAppended)
                                {
                                    query.Append(" * ");
                                    query.Append("{" + _calcaulateMember.UniqueName + "}");
                                }
                                else if (index > 0 && isAxisItemAppended && isMeasuresAppended)
                                {
                                    string temp = query.ToString();
                                    int end = temp.ToUpper().LastIndexOf("[" + PropertyConstants.MeasrueNodeName.ToUpper() + "]");
                                    int brace = temp.IndexOf("}", end);

                                    query = query.Insert(brace, ", " + _calcaulateMember.UniqueName);
                                }
                                else
                                {
                                    query.Append("{" + _calcaulateMember.UniqueName + "}");
                                    isAxisItemAppended = true;
                                }

                                isMeasuresAppended = true;
                            }
                        }

                        #endregion

                        #region MeasureElement

                        else if (item.ElementValue is MeasureElements && (item.ElementValue as MeasureElements).Elements.Count > 0)
                        {
                            MeasureElements _measureElements = (MeasureElements)item.ElementValue;
                            KpiElements kpiElements;
                            StringBuilder measureQuery = new StringBuilder();

                            ////Finding the index of KPIElements
                            int kpiIndex = FindIndexOfKPIElements(items);
                            if (kpiIndex >= 0)
                            {
                                kpiElements = (KpiElements)items[kpiIndex].ElementValue;
                                items.RemoveAt(kpiIndex);
                                isKPIpresent = true;
                            }
                            else
                            {
                                kpiElements = null;
                            }


                            for (int _i = 0; _i < _measureElements.Elements.Count; _i++)
                            {
                                if (_i > 0)
                                {
                                    measureQuery.Append(", ");
                                }

                                measureQuery.Append(_measureElements.Elements[_i].UniqueName);
                            }

                            if (kpiElements != null)
                            {
                                measureQuery.Append(", ");
                                for (int _i = 0; _i < kpiElements.Elements.Count; _i++)
                                {
                                    if (_i > 0)
                                    {
                                        measureQuery.Append(", ");
                                    }

                                    bool isValueAppended = false;
                                    if (kpiElements.Elements[_i].ShowKPIValue)
                                    {
                                        measureQuery.Append("KPIVALUE(\"" + kpiElements.Elements[_i].Name + "\")");
                                        isValueAppended = true;
                                    }

                                    if (kpiElements.Elements[_i].ShowKPIGoal)
                                    {
                                        if (isValueAppended)
                                        {
                                            measureQuery.Append(",");
                                        }

                                        measureQuery.Append("KPIGOAL(\"" + kpiElements.Elements[_i].Name + "\")");
                                        isValueAppended = true;
                                    }

                                    if (kpiElements.Elements[_i].ShowKPIStatus)
                                    {
                                        if (isValueAppended)
                                        {
                                            measureQuery.Append(",");
                                        }

                                        measureQuery.Append("KPISTATUS(\"" + kpiElements.Elements[_i].Name + "\")");
                                        isValueAppended = true;
                                    }

                                    if (kpiElements.Elements[_i].ShowKPITrend)
                                    {
                                        if (isValueAppended)
                                        {
                                            measureQuery.Append(", ");
                                        }

                                        measureQuery.Append("KPITREND(\"" + kpiElements.Elements[_i].Name + "\")");
                                        isValueAppended = true;
                                    }
                                }
                            }

                            if (index > 0 && isAxisItemAppended && !isMeasuresAppended)
                            {
                                query.Append(" * ");
                                query.Append("{" + measureQuery + "}");
                            }
                            else if (index > 0 && isAxisItemAppended && isMeasuresAppended)
                            {
                                string temp = query.ToString();
                                int end = temp.LastIndexOf("[" + PropertyConstants.MeasrueNodeName + "]");
                                int brace = temp.IndexOf("}", end);

                                query = query.Insert(brace, ", " + measureQuery);
                            }
                            else
                            {
                                query.Append("{" + measureQuery + "}");
                            }

                            isAxisItemAppended = true;
                            isMeasuresAppended = true;
                        }
                        #endregion

                        #region MemberElement
                        else if (item.ElementValue is MemberElement)
                        {
                            if (index > 0 && isAxisItemAppended)
                            {
                                query.Append(" * ");
                            }

                            MemberElement memberElement = (MemberElement)item.ElementValue;
                            {
                                if (ShowLevelTypeAll)
                                    query.Append(string.Format("(Except(({0}),{0}))", memberElement.UniqueName));
                                else
                                    query.Append(string.Format("(Except(DrillDownLevel({0}),{0}))", memberElement.UniqueName));
                            }

                            isAxisItemAppended = true;
                        }
                        #endregion

                        #region LevelElement
                        else if (item.ElementValue is LevelElement)
                        {
                            if (index > 0 && isAxisItemAppended)
                            {
                                query.Append(" * ");
                            }

                            DimensionElement dimensionElement = new DimensionElement();
                            HierarchyElement hierarchyElement = new HierarchyElement();
                            LevelElement levelElement = item.ElementValue as LevelElement;
                            hierarchyElement = levelElement.ParentHierarchy;
                            dimensionElement.Name = levelElement.DimensionName;
                            if (levelElement != null)
                            {
                                if (levelElement.ParentHierarchy != null && levelElement.Name != string.Empty)
                                {
                                    StringBuilder memberString = new StringBuilder();
                                    ////query.Append("Hierarchize(");
                                    GetChildMemberElementSet(levelElement.MemberElements, true, levelElement, memberString, query, ProviderName, ShowLevelTypeAll, DrillType, useDefaultMember);
                                    ////query.Append(")");
                                    isAxisItemAppended = true;
                                }
                                else if (levelElement.ParentHierarchy == null || levelElement.ParentHierarchy.UniqueName == string.Empty)
                                {
                                    throw new Exception("ParentHierarchy should be specified for a LevelElement");
                                }
                                else
                                {
                                    throw new Exception("Please specify the Level Name");
                                }
                            }
                        }
                        #endregion

                        #region NamedSetElement
                        else if (item.ElementValue is NamedSetElement)
                        {
                            if (index > 0 & isAxisItemAppended)
                            {
                                query.Append(" * ");
                            }

                            NamedSetElement namedSetElement = (NamedSetElement)item.ElementValue;
                            query.Append("{");
                            query.Append(namedSetElement.UniqueName);
                            query.Append("}");

                            isAxisItemAppended = true;
                        }
                    #endregion
                }
                #region Appending Sort
                if (sortElement != null && isSortOn == true)
                {
                    query.Insert(0,"ORDER(");
                    query.Append(",\n\t\t");
                    query.Append(" (");
                    query.Append(sortElement.Element.UniqueName);
                    query.Append(")");
                    query.Append(",\n\t\t");
                    query.Append(sortElement.SortOrder.ToString());
                    query.Append(")");
                    isSortOn = false;
                }
                #endregion
            }

            /// <summary>
            /// for Building the Items based on the categories specified.
            /// </summary>
            /// <param name="items">Axis items collection</param>
            /// <param name="query">query to be generated</param>
            /// <param name="isGrandTotalOn">whether Grand total is on or not</param>
            /// <returns>bool value representing if KPI is present or not</returns>
            private static bool BuildSlicerItem(Items items, StringBuilder query, bool isGrandTotalOn, bool isCount, Providers ProviderName, bool ShowLevelTypeAll)
            {
                // bool value to identify whether KPI Elements present in this Axis
                bool isKPIpresent = false;
                bool isItemAppended = true;
                for (int i = 0; i < items.Count && !items.IsFilterOrSortOn; i++)
                {
                    Item item = items[i];

                    #region DimensionElement
                    if (item.ElementValue is DimensionElement)
                    {
                        if (i > 0 & isItemAppended)
                        {
                            if (ProviderName == Providers.ActivePivot)
                            {
                                if (query.ToString().Contains((char)255))
                                    query.Append(" ");
                                else
                                    query.Append(", ");
                            }
                            else
                            {
                                query.Append(" * ");
                            }
                        }

                        BuildSlicedDimension(query, isGrandTotalOn, item, ProviderName);
                        isItemAppended = true;
                    }
                    #endregion

                    #region KPIElement
                    else if (item.ElementValue is KpiElements)
                    {
                        KpiElements _kpiElements = (KpiElements)item.ElementValue;
                        if (FindIndexOfMeasureElements(items) >= 0)
                        {
                            if (i == 0)
                            {
                                isItemAppended = false;
                            }

                            continue;
                        }
                        else
                        {
                            isKPIpresent = true;
                            if (i > 0 && isItemAppended)
                            {
                                query.Append(" * ");
                            }

                            query.Append("{");
                            for (int _i = 0; _i < _kpiElements.Elements.Count; _i++)
                            {
                                if (_i > 0)
                                {
                                    query.Append(",");
                                }

                                bool isValueAppended = false;
                                if (_kpiElements.Elements[_i].ShowKPIValue)
                                {
                                    query.Append("KPIVALUE(\"" + _kpiElements.Elements[_i].Name + "\")");
                                    isValueAppended = true;
                                }

                                if (_kpiElements.Elements[_i].ShowKPIGoal)
                                {
                                    if (isValueAppended)
                                    {
                                        query.Append(",");
                                    }

                                    query.Append("KPIGOAL(\"" + _kpiElements.Elements[_i].Name + "\")");
                                    isValueAppended = true;
                                }

                                if (_kpiElements.Elements[_i].ShowKPIStatus)
                                {
                                    if (isValueAppended)
                                    {
                                        query.Append(",");
                                    }

                                    query.Append("KPISTATUS(\"" + _kpiElements.Elements[_i].Name + "\")");
                                    isValueAppended = true;
                                }

                                if (_kpiElements.Elements[_i].ShowKPITrend)
                                {
                                    if (isValueAppended)
                                    {
                                        query.Append(",");
                                    }

                                    query.Append("KPITREND(\"" + _kpiElements.Elements[_i].Name + "\")");
                                    isValueAppended = true;
                                }
                            }

                            query.Append("}");
                            isItemAppended = true;
                        }
                    }
                    #endregion

                    #region MeasureElement
                    else if (item.ElementValue is MeasureElements || item.ElementValue is CalculatedMember)
                    {
                        MeasureElements measureElements = null;
                        CalculatedMember calcMember = null;
                        if (item.ElementValue is MeasureElements)
                        {
                            measureElements = item.ElementValue as MeasureElements;
                        }
                        else
                        {
                            calcMember = item.ElementValue as CalculatedMember;
                        }

                        KpiElements kpiElements;

                        ////Finding the index of KPIElements
                        int kpiIndex = FindIndexOfKPIElements(items);
                        if (kpiIndex >= 0)
                        {
                            kpiElements = (KpiElements)items[kpiIndex].ElementValue;
                            items.RemoveAt(kpiIndex);
                            isKPIpresent = true;
                        }
                        else
                        {
                            kpiElements = null;
                        }

                        if (i > 0 && isItemAppended)
                        {
                            if (ProviderName != Providers.ActivePivot)
                            {
                                query.Append(" * ");
                            }
                        }

                        if (!(ProviderName == Providers.ActivePivot))
                        {
                            query.Append("{");
                        }
                        if (measureElements != null)
                        {
                            for (int _i = 0; _i < measureElements.Elements.Count; _i++)
                            {
                                if (_i > 0)
                                {
                                    query.Append(",");
                                }
                                if (ProviderName == Providers.ActivePivot)
                                    query.Insert(0, measureElements.Elements[_i].UniqueName + (char)255);
                                else
                                    query.Append(measureElements.Elements[_i].UniqueName);
                                
                            }
                        }
                        else if (calcMember != null)
                        {
                            if (isCount && calcMember.Type == TypeOfMember.Measure)
                            {
                                if (calcMember.Expression.StartsWith(Utils.QuoteIdentifier(PropertyConstants.MeasrueNodeName)))
                                {
                                    System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(@"\[Measures\]\.\[([^\]]+)]");
                                    System.Text.RegularExpressions.MatchCollection matches = re.Matches(calcMember.Expression);
                                    if (matches.Count > 0)
                                    {
                                        query.Append(matches[0].Value); 
                                    }
                                }
                                else if (calcMember.Expression.StartsWith(Utils.QuoteIdentifier(PropertyConstants.Measures)))
                                {
                                    System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(@"\[MEASURES\]\.\[([^\]]+)]");
                                    System.Text.RegularExpressions.MatchCollection matches = re.Matches(calcMember.Expression);
                                    if (matches.Count > 0)
                                    {
                                        query.Append(matches[0].Value);
                                    }
                                }
                            }
                            else
                            {
                                query.Append(calcMember.UniqueName);
                            }
                        }

                        if (kpiElements != null)
                        {
                            query.Append(",");
                            for (int _i = 0; _i < kpiElements.Elements.Count; _i++)
                            {
                                if (_i > 0)
                                {
                                    query.Append(",");
                                }

                                bool isValueAppended = false;
                                if (kpiElements.Elements[_i].ShowKPIValue)
                                {
                                    query.Append("KPIVALUE(\"" + kpiElements.Elements[_i].Name + "\")");
                                    isValueAppended = true;
                                }

                                if (kpiElements.Elements[_i].ShowKPIGoal)
                                {
                                    if (isValueAppended)
                                    {
                                        query.Append(",");
                                    }

                                    query.Append("KPIGOAL(\"" + kpiElements.Elements[_i].Name + "\")");
                                    isValueAppended = true;
                                }

                                if (kpiElements.Elements[_i].ShowKPIStatus)
                                {
                                    if (isValueAppended)
                                    {
                                        query.Append(",");
                                    }

                                    query.Append("KPISTATUS(\"" + kpiElements.Elements[_i].Name + "\")");
                                    isValueAppended = true;
                                }

                                if (kpiElements.Elements[_i].ShowKPITrend)
                                {
                                    if (isValueAppended)
                                    {
                                        query.Append(",");
                                    }

                                    query.Append("KPITREND(\"" + kpiElements.Elements[_i].Name + "\")");
                                    isValueAppended = true;
                                }
                            }
                        }

                        if (ProviderName != Providers.ActivePivot)
                        {
                            query.Append("}");
                        }
                        isItemAppended = true;
                    }
                    #endregion

                    #region MemberElement
                    else if (item.ElementValue is MemberElement)
                    {
                        if (i > 0 && isItemAppended)
                        {
                            query.Append(" * ");
                        }

                        MemberElement memberElement = (MemberElement)item.ElementValue;
                        if (ShowLevelTypeAll)
                            query.Append(string.Format("(Except(({0}),{0}))", memberElement.UniqueName));
                        else
                            query.Append(string.Format("(Except(DrillDownLevel({0}),{0}))", memberElement.UniqueName));
                        isItemAppended = true;
                    }
                    #endregion

                    #region LevelElement
                    else if (item.ElementValue is LevelElement)
                    {
                        if (i > 0 && isItemAppended)
                        {
                            query.Append(" * ");
                        }

                        DimensionElement dimensionElement = new DimensionElement();
                        HierarchyElement hierarchyElement = new HierarchyElement();
                        LevelElement levelElement = item.ElementValue as LevelElement;
                        hierarchyElement = levelElement.ParentHierarchy;
                        dimensionElement.Name = levelElement.DimensionName;
                        if (levelElement != null)
                        {
                            if (levelElement.ParentHierarchy != null && levelElement.Name != string.Empty)
                            {
                                StringBuilder memberString = new StringBuilder();
                                query.Append("{");
                                query.Append(GetSlicedChildMembers(levelElement.MemberElements, true));
                                query.Append("}");
                                isItemAppended = true;
                            }
                            else if (levelElement.ParentHierarchy == null || levelElement.ParentHierarchy.UniqueName == string.Empty)
                            {
                                throw new Exception("ParentHierarchy should be specified for a LevelElement");
                            }
                            else
                            {
                                throw new Exception("Please specify the Level Name");
                            }
                        }
                    }
                    #endregion

                    #region NamedSetElement
                    else if (item.ElementValue is NamedSetElement)
                    {
                        if (i > 0 & isItemAppended)
                        {
                            query.Append(" * ");
                        }

                        NamedSetElement namedSetElement = (NamedSetElement)item.ElementValue;
                        query.Append("{");
                        query.Append(namedSetElement.UniqueName);
                        query.Append("}");
                    }
                    #endregion
                }

                return isKPIpresent;
            }

            /// <summary>
            /// for building DimensionElement if multiple hierarchies are provided
            /// This method is specific for Filter Elements
            /// </summary>
            /// <param name="query">query to be generated</param>
            /// <param name="isGrandTotalOn">Grand total is on or not</param>
            /// <param name="element">Dimension Element name</param>
            /// <param name="excludedElement">The Excluded Dimension Element name</param>
            private static void BuildDimensionElement(StringBuilder query, bool isGrandTotalOn, Element element, Element excludedElement, Providers ProviderName, bool ShowLevelTypeAll, DrillType DrillType,bool useDefaultMember,bool isPaging, bool visualTotalsVisibility)
            {
                query.Append("{");
                DimensionElement dimensionElement = (DimensionElement)element;
                if (dimensionElement == null || dimensionElement.Name == string.Empty)
                {
                    throw new Exception("Dimension Name should be specified");
                }

                if (!(dimensionElement.Hierarchy != null))
                {
                    GetDefaultLevel(null, query, dimensionElement, ProviderName, ShowLevelTypeAll, useDefaultMember,false,isPaging,false,visualTotalsVisibility);
                }
                else
                {
                    BuildHierarchyElement(query, isGrandTotalOn, dimensionElement, excludedElement, ProviderName, ShowLevelTypeAll, DrillType, useDefaultMember,isPaging, visualTotalsVisibility);
                }

                query.Append("}");
            }

            /// <summary>
            /// Builds the hierarchy element from eht dimensionElement.
            /// </summary>
            /// <param name="query">The query.</param>
            /// <param name="isGrandTotalOn">if set to <c>true</c> [is grand total on].</param>
            /// <param name="dimensionElement">The dimension element.</param>
            /// <param name="excludedElement">The excluded dimension element.</param>
            private static void BuildHierarchyElement(StringBuilder query, bool isGrandTotalOn, DimensionElement dimensionElement, Element excludedElement, Providers ProviderName, bool ShowLevelTypeAll, DrillType DrillType, bool useDefaultMember,bool isPaging,bool visualTotalsVisibility)
            {
                int hierarchyCount = 0;
                query.Append("{");

                #region adding HierarchyElement
                HierarchyElement hierarchyElement = dimensionElement.Hierarchy;
                if (hierarchyElement == null)
                {
                    throw new Exception("Hierarchy object not found");
                }

                if (hierarchyCount > 0)
                {
                    query.Append(" * ");
                }

                if (!(hierarchyElement.LevelElements.Count > 0))
                {
                    throw new Exception("LevelElement should be specified");
                }
                else
                {
                    LevelElement levelElementCollection = (LevelElement)hierarchyElement.LevelElements[0];
                    BuildLevelElements(query, isGrandTotalOn, levelElementCollection, dimensionElement, excludedElement, ProviderName, ShowLevelTypeAll, DrillType, useDefaultMember,isPaging,visualTotalsVisibility);
                }

                hierarchyCount++;
                #endregion

                ////closing the braces for HierarchyElement
                query.Append("}");
            }

            /// <summary>
            /// Builds the level elements from the dimensionElements hierarchy.
            /// </summary>
            /// <param name="query">The query.</param>
            /// <param name="isGrandTotalOn">if set to <c>true</c> [is grand total on].</param>
            /// <param name="levelElement">The level element.</param>
            /// <param name="dimensionElement">The dimension element.</param>
            /// <param name="excludedElement">The excluded element.</param>
            private static void BuildLevelElements(StringBuilder query, bool isGrandTotalOn, LevelElement levelElement, DimensionElement dimensionElement, Element excludedElement, Providers ProviderName, bool ShowLevelTypeAll, DrillType DrillType, bool useDefaultMember,bool isPaging, bool visualTotalsVisibility)
            {
                int levelCount = 0;
                query.Append("{");
                #region adding LevelElement
                if (levelCount > 0)
                {
                    query.Append(", ");
                }

                if (!(levelElement.MemberElements.Count > 0))
                {
                    GetDefaultLevel(null, query, dimensionElement, ProviderName, ShowLevelTypeAll, useDefaultMember,false,isPaging,false,visualTotalsVisibility);
                }
                else
                {
                    MemberElementCollection memberElementCollection = (MemberElementCollection)levelElement.MemberElements;
                    BuildMemberElement(query, isGrandTotalOn, levelElement, dimensionElement, excludedElement, ProviderName, ShowLevelTypeAll, DrillType,useDefaultMember);
                }

                levelCount++;
                #endregion

                ////closing the braces for LevelElement
                query.Append("}");
            }

            /// <summary>
            /// Builds the member element from the level Element.
            /// </summary>
            /// <param name="query">The query.</param>
            /// <param name="isGrandTotalOn">if set to <c>true</c> [is grand total on].</param>
            /// <param name="levelElement">The level element.</param>
            /// <param name="dimensionElement">The dimension element.</param>
            /// <param name="excludedElement">The excluded dimension element.</param>
            private static void BuildMemberElement(StringBuilder query, bool isGrandTotalOn, LevelElement levelElement, DimensionElement dimensionElement, Element excludedElement, Providers ProviderName, bool ShowLevelTypeAll, DrillType DrillType, bool useDefaultMember)
            {
                StringBuilder memberString = new StringBuilder();
                MemberElementCollection memberElementCollection = levelElement.MemberElements;
                DimensionElement excludedDimension = (DimensionElement)excludedElement;
                int count = memberElementCollection.Count;
                bool exceptFlag = false;
                if (excludedDimension != null)
                {
                    query.Append("Except(");
                    exceptFlag = true;
                }

                StringBuilder tempQuery = new StringBuilder();
                if (count > 0)
                {
                    GetChildMemberElementSet(memberElementCollection, true, levelElement, memberString, query, ProviderName, ShowLevelTypeAll, DrillType,useDefaultMember);
                    tempQuery.Append(AppendExceptElements(null, exceptFlag, ProviderName));
                }
                else
                {
                    if (ShowLevelTypeAll)
                        query.Append("({");
                    else
                        query.Append("DrilldownLevel({");
                    if (ProviderName == Providers.Mondrian)
                        query.Append(levelElement.ParentHierarchy.ParentDimension.UniqueName);
                    else
                        query.Append(levelElement.ParentHierarchy.UniqueName);
                    tempQuery.Append(AppendExceptElements(null, exceptFlag, ProviderName));
                    query.Append("})");
                }

                if (exceptFlag)
                {
                    query.Append(tempQuery);
                }
            }

            /// <summary>
            /// for building DimensionElement if multiple hierarchies are provided
            /// </summary>
            /// <param name="query">query to be generated</param>
            /// <param name="isGrandTotalOn">grand total is on or not</param>
            /// <param name="item">item collection</param>
            [System.ComponentModel.Description("for building DimensionElement if multiple hierarchies are provided")]
            private static void BuildSlicedDimension(StringBuilder query, bool isGrandTotalOn, Item item, Providers ProviderName)
            {
                if (ProviderName != Providers.ActivePivot)
                {
                    query.Append("{");
                }
                DimensionElement dimensionElement = (DimensionElement)item.ElementValue;
                if (dimensionElement == null || dimensionElement.Name == string.Empty)
                {
                    throw new Exception("Dimension Name is must be specified");
                }

                if (!(dimensionElement.Hierarchy != null))
                {
                    GetSlicedDefaultLevel(query, dimensionElement, ProviderName);
                }
                else
                {
                    BuildSlicedHierarchy(query, isGrandTotalOn, dimensionElement, ProviderName);
                }

                ////closing the braces for DimensionElement
                if (ProviderName != Providers.ActivePivot)
                {
                    query.Append("}");
                }
            }

            /// <summary>
            /// for building DimensionElement if multiple hierarchies are provided
            /// </summary>
            /// <param name="query">query to be generated</param>
            /// <param name="isGrandTotalOn">grand total is on or not</param>
            /// <param name="item">item collection</param>
            [System.ComponentModel.Description("for building DimensionElement if multiple hierarchies are provided")]
            private static void BuildDimensionElement(StringBuilder query, bool isGrandTotalOn, Item item, Providers ProviderName, bool ShowLevelTypeAll, DrillType DrillType, bool useDefaultMember, bool isLastItem, bool isPaging, bool isSameHyra,bool visualTotalsVisibility)
            {
                DimensionElement dimensionElement = (DimensionElement)item.ElementValue;
                if (dimensionElement == null || dimensionElement.Name == string.Empty)
                {
                    throw new Exception("Dimension Name should be specified");
                }

                if (!(dimensionElement.Hierarchy != null))
                {
                    GetDefaultLevel(item, query, dimensionElement, ProviderName, ShowLevelTypeAll,useDefaultMember,isLastItem,isPaging,isSameHyra,visualTotalsVisibility);
                }
                else
                {
                    BuildHierarchyElement(item, query, isGrandTotalOn, dimensionElement, ProviderName, ShowLevelTypeAll, DrillType,useDefaultMember,isLastItem,isPaging,isSameHyra,visualTotalsVisibility);
                }

                StringBuilder tempPropertyString = null;

                if (item.Axis == AxisPosition.Categorical)
                {
                    tempPropertyString = _columnPropertiesQueryString;
                }
                else
                {
                    tempPropertyString = _rowPropertiesQueryString;
                }

#if !SILVERLIGHT
                for (int i = 0; i < dimensionElement.MemberProperties.Count; i++)
                {
                    MemberProperty dimensionProperty = dimensionElement.MemberProperties[i];
                    if (dimensionProperty != null)
                    {
                        if (i > 0 || !String.IsNullOrEmpty(tempPropertyString.ToString()))
                        {
                            tempPropertyString.Append(",");
                        }

                        tempPropertyString.Append(dimensionProperty.UniqueName.ToString());
                    }
                }
#endif
            }

            /// <summary>
            /// Builds the filter condition.
            /// </summary>
            /// <param name="filter">The filter items</param>
            /// <param name="itemsColumn">The column items</param>
            /// <param name="itemsRow">The row items</param>            
            /// <returns>filter criteria string</returns>
            static string BuildFilterCondition(Where filter, Items itemsColumn, Items itemsRow, bool isCount, Providers ProviderName, List<SlicerRangeFiltersInfo> SlicerRangeInfoFields, bool ShowLevelTypeAll)
            {
                StringBuilder query = new StringBuilder();

                var whereItems = filter.Items.List.Where(i => i.ElementValue is NamedSetElement && (i.ElementValue as NamedSetElement).IsQueryScoped).ToList();
                filter.Items.List.RemoveAll(f => whereItems.Contains(f));

                if (itemsColumn.Count > 0 || itemsRow.Count > 0)
                {
                    BuildFilteredElements(itemsColumn, itemsRow, query, false, Utils.QuoteIdentifier(filter.QuerySpecification.CubeName), isCount);
                }
                else
                {
                    query.Append(Utils.QuoteIdentifier(filter.QuerySpecification.CubeName));
                }

                StringBuilder whereQuery = new StringBuilder();
                if (whereItems.Count > 0)
                {
                    whereQuery.Append(" WHERE (");

                    for (int i = 0; i < whereItems.Count; i++)
                    {
                        NamedSetElement ne = whereItems[i].ElementValue as NamedSetElement;
                        if (ne == null) continue;
                        if (i != 0) whereQuery.Append(" * ");
                        whereQuery.Append("{").Append(ne.UniqueName).Append("}");
                    }
                }

                if (filter.Items.Count > 0 || SlicerRangeInfoFields.Count > 0)
                {
                    if (ProviderName != Providers.ActivePivot)
                    {
                        if (whereQuery.Length == 0)
                            whereQuery.Append(" WHERE (");
                        else
                            whereQuery.Append(" * ");
                    }

                    if (filter.Items.Count > 0)
                        BuildSlicerElements(filter.Items, whereQuery, null, null, false, isCount, ProviderName, ShowLevelTypeAll);

                    if (SlicerRangeInfoFields.Count > 0)
                    {
                        if (whereQuery[whereQuery.Length - 1] == '}')
                            whereQuery.Append(" * ");
                        List<string> rangeValueCollection = new List<string>();
                        foreach (SlicerRangeFiltersInfo rangevalue in SlicerRangeInfoFields)
                        {
                            if (rangevalue.StartValue.IndexOfAny(new char[] { '[', ']' }) == 0 && rangevalue.EndValue.IndexOfAny(new char[] { '[', ']' }) == 0)
                                rangeValueCollection.Add("(" + rangevalue.StartValue + ":" + rangevalue.EndValue + ")");
                            else
                                rangeValueCollection.Add("([" + rangevalue.DimensionName + "].[" + rangevalue.HierarchyName + "].[" + rangevalue.LevelName + "].[" + rangevalue.StartValue + "]:[" + rangevalue.DimensionName + "].[" + rangevalue.HierarchyName + "].[" + rangevalue.LevelName + "].[" + rangevalue.EndValue + "])");
                        }
                        for (int i = 0; i < rangeValueCollection.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(rangeValueCollection[i]))
                            {
                                whereQuery.Append("{" + rangeValueCollection[i]);
                                for (int j = i + 1; j < rangeValueCollection.Count; j++)
                                {
                                    if (rangeValueCollection[i].Split('.')[0] == rangeValueCollection[j].Split('.')[0])
                                    {
                                        whereQuery.Append("," + rangeValueCollection[j]);
                                        rangeValueCollection[j] = string.Empty;
                                    }
                                }
                                whereQuery.Append("} *");
                            }
                        }
                        whereQuery.Remove(whereQuery.Length - 1, 1);
                    }

                }
                if (whereQuery.Length > 0)
                {
                    if (ProviderName == Providers.ActivePivot)
                    {
                        string[] whereQueryParts = whereQuery.ToString().Split((char)255);
                        if (whereQueryParts.Length > 1)
                        {
                            string whereWithMemberQuery = " WHERE (";
                            for (int q = 0; q < whereQueryParts.Length - 1; q++)
                            {
                                whereWithMemberQuery += whereQueryParts[q];
                                if (q < whereQueryParts.Length - 2)
                                    whereWithMemberQuery += ",";
                            }
                            query.Append(whereWithMemberQuery);
                            query.Append(")");
                            query.Append((char)255);
                            query.Append(whereQueryParts[whereQueryParts.Length - 1]);
                        }
                        else
                        {
                            whereQuery.Insert(0, " WHERE (");
                            whereQuery.Append(")");
                            query.Append(whereQuery);
                        }
                    }
                    else
                    {
                        whereQuery.Append(")");
                        query.Append(whereQuery);
                    }
                }

                return query.ToString();
            }

            /// <summary>
            /// Appends the child members.
            /// </summary>
            /// <param name="memberElements">The member elements.</param>
            /// <returns>string appended with the child members.</returns>
            private static string AppendChildMembers(MemberElementCollection memberElements)
            {
                StringBuilder tempQuery = new StringBuilder();
                bool isAppended = false;
                for (int i = 0; i < memberElements.Count; i++)
                {
                    MemberElement memberElement = memberElements[i];

                    if (i > 0 && isAppended)
                    {
                        tempQuery.Append(", ");
                    }

                    if (memberElement.ChildMemberElements.Count > 0)
                    {
                        tempQuery.Append(AppendChildMembers(memberElement.ChildMemberElements));
                        isAppended = true;
                    }
                    else
                    {
                        tempQuery.Append(memberElement.UniqueName);
                        isAppended = true;
                    }
                }

                return tempQuery.ToString();
            }

            /// <summary>
            /// Appends all the members in the level element.
            /// </summary>
            /// <param name="levelElement">The level element.</param>
            /// <returns>The concatenated value of all the member elements</returns>
            private static string AppendParentMember(LevelElement levelElement)
            {
                StringBuilder tempString = new StringBuilder();
                for (int i = 0; i < levelElement.MemberElements.Count; i++)
                {
                    MemberElement memberElement = levelElement.MemberElements[i];
                    if (i > 0)
                    {
                        tempString.Append(", ");
                    }

                    tempString.Append(memberElement.UniqueName);
                }

                return tempString.ToString();
            }

            /// <summary>
            /// Appends the last child member of each member or else the current member is appended if no child is available.
            /// </summary>
            /// <param name="parentMemberElement">The parent member element.</param>
            /// <returns>The Concatenated Parent Strings</returns>
            private static string AppendParentMember(MemberElement parentMemberElement)
            {
                StringBuilder tempString = new StringBuilder();
                for (int i = 0; i < parentMemberElement.ChildMemberElements.Count; i++)
                {
                    MemberElement memberElement = parentMemberElement.ChildMemberElements[i];
                    if (i > 0)
                    {
                        tempString.Append(", ");
                    }

                    if (memberElement.ChildMemberElements.Count > 0)
                    {
                        tempString.Append(AppendChildMember(memberElement));
                    }
                    else
                    {
                        tempString.Append(memberElement.UniqueName);
                    }
                }

                return tempString.ToString();
            }

            /// <summary>
            /// Appends the last child member of each member or else the current member is appended if no child is available.
            /// </summary>
            /// <param name="parentMemberElement">The parent member element.</param>
            /// <returns>The concatenated string containing all the child member Elements</returns>
            private static string AppendChildMember(MemberElement parentMemberElement)
            {
                StringBuilder tempString = new StringBuilder();
                for (int i = 0; i < parentMemberElement.ChildMemberElements.Count; i++)
                {
                    MemberElement memberElement = parentMemberElement.ChildMemberElements[i];

                    if (i > 0)
                    {
                        tempString.Append(", ");
                    }

                    if (memberElement.ChildMemberElements.Count > 0)
                    {
                        tempString.Append(AppendChildMember(memberElement));
                    }
                    else
                    {
                        tempString.Append(memberElement.UniqueName);
                    }
                }

                return tempString.ToString();
            }

            /// <summary>
            /// Builds the filtered elements.  Used for building the subquery part to filter the specific members in 
            /// both Axis.
            /// </summary>
            /// <param name="itemsColumn">The items column.</param>
            /// <param name="itemsRow">The items row.</param>
            /// <param name="query">The query.</param>
            /// <param name="isSlicerAppended">if set to <c>true</c> [is slicer appended].</param>
            /// <param name="cubeName">Name of the cube with Quoted Identifier</param>
            private static void BuildFilteredElements(Items itemsColumn, Items itemsRow, StringBuilder query, bool isSlicerAppended, string cubeName, bool isCount)
            {
                bool isRowAppended = false;
                bool isColumnAppended = false;

                #region Appending Column Filters
                StringBuilder tempString = new StringBuilder();
                for (int i = 0; i < itemsColumn.Count; i++)
                {
                    Item item = itemsColumn[i];

                    #region DimensionElement
                    if (item.ElementValue is DimensionElement)
                    {
                        DimensionElement dimensionElement = (DimensionElement)item.ElementValue;
                        if (dimensionElement.Hierarchy.LevelElements.Count > 0)
                        {
                            LevelElement levelElement = (LevelElement)dimensionElement.Hierarchy.LevelElements[0];
                            if (levelElement.MemberElements.Count > 0 && levelElement.IncludeAvailableMembers)
                            {
                                for (int l = 0; l < levelElement.MemberElements.Count; l++)
                                {
                                    StringBuilder columnTempString = new StringBuilder();
                                    if (l == 0 && isSlicerAppended)
                                    {
                                        columnTempString.Append("* {");
                                    }
                                    else if (l == 0 && i > 0 && isColumnAppended)
                                    {
                                        columnTempString.Append("* {");
                                    }
                                    else if (l == 0)
                                    {
                                        columnTempString.Append(" {");
                                    }

                                    if (l > 0 && isColumnAppended)
                                    {
                                        columnTempString.Append(", ");
                                    }

                                    tempString.Append(columnTempString);
                                    tempString.Append(levelElement.MemberElements[l].UniqueName);
                                    //tempString.Append(AppendParentMember(levelElement));
                                    isColumnAppended = true;
                                }

                                if (isColumnAppended)
                                {
                                    tempString.Append("}");
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("LevelElement should be specified");
                        }
                    }
                    #endregion

                    #region LevelElement
                    if (item.ElementValue is LevelElement)
                    {
                        LevelElement levelElement = (LevelElement)item.ElementValue;
                        if (levelElement.MemberElements.Count > 0)
                        {
                            for (int j = 0; j < levelElement.MemberElements.Count; j++)
                            {
                                bool isChildAppended = false;
                                if (j == 0 && isSlicerAppended)
                                {
                                    tempString.Append("* {");
                                }
                                else if (j == 0 && i > 0 && isColumnAppended)
                                {
                                    tempString.Append("* {");
                                }
                                else if (j == 0)
                                {
                                    tempString.Append(" {");
                                }

                                if (j > 0 && isColumnAppended)
                                {
                                    tempString.Append(", ");
                                }

                                MemberElement memberElement = levelElement.MemberElements[j];

                                if (memberElement.ChildMemberElements.Count > 0)
                                {
                                    tempString.Append(AppendChildMembers(memberElement.ChildMemberElements));
                                    isColumnAppended = true;
                                    isChildAppended = true;
                                }

                                if (!isChildAppended)
                                {
                                    tempString.Append(memberElement.UniqueName);
                                    isColumnAppended = true;
                                }
                            }

                            if (isColumnAppended)
                            {
                                tempString.Append("}");
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                if (isColumnAppended)
                {
                    tempString.Append("  ON COLUMNS ");
                }

                #region Appending Row Filters
                for (int i = 0; i < itemsRow.Count; i++)
                {
                    Item item = itemsRow[i];

                    #region DimensionElement
                    if (item.ElementValue is DimensionElement)
                    {
                        DimensionElement dimensionElement = (DimensionElement)item.ElementValue;
                        if (dimensionElement.Hierarchy.LevelElements.Count > 0)
                        {
                            LevelElement levelElement = (LevelElement)dimensionElement.Hierarchy.LevelElements[0];
                            if (levelElement.MemberElements.Count > 0 && levelElement.IncludeAvailableMembers)
                            {
                                for (int l = 0; l < levelElement.MemberElements.Count; l++)
                                {
                                    StringBuilder rowTempString = new StringBuilder();
                                    if (l == 0 && isColumnAppended && !isRowAppended)
                                    {
                                        rowTempString.Append(", {");
                                    }
                                    else if ((l == 0 && i > 0 && isRowAppended) || (l == 0 && !isColumnAppended && isSlicerAppended && i >= 0))
                                    {
                                        rowTempString.Append("* {");
                                    }
                                    else if (l == 0)
                                    {
                                        rowTempString.Append(" {");
                                    }

                                    if (l > 0 && isRowAppended)
                                    {
                                        rowTempString.Append(", ");
                                    }

                                    tempString.Append(rowTempString);
                                    tempString.Append(levelElement.MemberElements[l].UniqueName);
                                   // tempString.Append(AppendParentMember(levelElement));
                                    isRowAppended = true;
                                }

                                if (isRowAppended)
                                {
                                    tempString.Append("}");
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("LevelElement should be specified");
                        }
                    }
                    #endregion

                    #region LevelElement
                    if (item.ElementValue is LevelElement)
                    {
                        LevelElement levelElement = (LevelElement)item.ElementValue;
                        if (levelElement.MemberElements.Count > 0)
                        {
                            for (int j = 0; j < levelElement.MemberElements.Count; j++)
                            {
                                bool isChildAppended = false;
                                if (j == 0 && isColumnAppended && i == 0)
                                {
                                    tempString.Append(", {");
                                }
                                else if ((j == 0 && i > 0 && isRowAppended) || (j == 0 && !isColumnAppended && isSlicerAppended && i >= 0))
                                {
                                    tempString.Append("* {");
                                }
                                else if (j == 0 && isSlicerAppended)
                                {
                                    tempString.Append(" * {");
                                }
                                else if (j == 0)
                                {
                                    tempString.Append(" {");
                                }

                                if (j > 0 && isRowAppended)
                                {
                                    tempString.Append(", ");
                                }

                                MemberElement memberElement = levelElement.MemberElements[j];
                                if (memberElement.ChildMemberElements.Count > 0)
                                {
                                    tempString.Append(AppendChildMembers(memberElement.ChildMemberElements));
                                    isRowAppended = true;
                                    isChildAppended = true;
                                }

                                if (!isChildAppended)
                                {
                                    tempString.Append(memberElement.UniqueName);
                                    isRowAppended = true;
                                }
                            }

                            if (isRowAppended)
                            {
                                tempString.Append("}");
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                if (isSlicerAppended)
                {
                    if (isRowAppended && isColumnAppended)
                    {
                        tempString.Append(" ON ROWS FROM ");
                        query.Append(tempString.ToString());
                    }
                    else if (isRowAppended && !isColumnAppended)
                    {
                        tempString.Append(" ON COLUMNS FROM ");
                        query.Append(tempString.ToString());
                    }
                    else if (!isRowAppended && isColumnAppended)
                    {
                        tempString.Append(" FROM ");
                        query.Append(tempString.ToString());
                    }
                    else
                    {
                        tempString.Append(" ON COLUMNS FROM ");
                        query.Append(tempString.ToString());
                    }
                }
                else
                {
                    if (isRowAppended && isColumnAppended)
                    {
                        tempString.Append(" ON ROWS FROM ");
                    }
                    else if (isRowAppended && !isColumnAppended)
                    {
                        tempString.Append(" ON COLUMNS FROM ");
                    }
                    else if (!isRowAppended && isColumnAppended)
                    {
                        tempString.Append(" FROM ");
                    }

                    if (tempString.ToString() == string.Empty)
                    {
                        query.Append(cubeName);
                    }
                    else
                    {
                        query.Append(" ( SELECT ");
                        query.Append(tempString.ToString());
                        query.Append(cubeName);
                        query.Append(" )");
                    }
                }
            }

            /// <summary>
            /// for building hierarchyElement if multiple levels are provided
            /// </summary>
            /// <param name="query">query to be generated</param>
            /// <param name="isGrandTotalOn">grand total is on or not</param>
            /// <param name="dimensionElement">holds the dimension element</param>
            private static void BuildSlicedHierarchy(StringBuilder query, bool isGrandTotalOn, DimensionElement dimensionElement, Providers ProviderName)
            {
                int hierarchyCount = 0;
                if (ProviderName != Providers.ActivePivot)
                {
                    query.Append("{");
                }

                #region adding HierarchyElement
                HierarchyElement hierarchyElement = dimensionElement.Hierarchy;
                if (hierarchyElement == null)
                {
                    throw new Exception("Hierarchy must be specified");
                }

                if (hierarchyCount > 0)
                {
                    query.Append(" * ");
                }

                if (!(hierarchyElement.LevelElements.Count > 0))
                {
                    GetSlicedDefaultLevel(query, dimensionElement, ProviderName);
                }
                else
                {
                    if (ProviderName == Providers.ActivePivot)
                    {
                        foreach (LevelElement levelElement in hierarchyElement.LevelElements)
                            BuildSlicedLevel(query, isGrandTotalOn, levelElement, dimensionElement, ProviderName);
                    }
                    else
                    {
                        foreach (LevelElement levelElement in hierarchyElement.LevelElements)
                            BuildSlicedLevel(query, isGrandTotalOn, levelElement, dimensionElement, ProviderName);
                        if (hierarchyElement.LevelElements.Count > 1)
                            query.Replace("{(" + hierarchyElement.UniqueName + ")}", "");
                        query.Replace("}{", "},{");
                    }
                }

                hierarchyCount++;
                #endregion

                ////closing the braces for HierarchyElement
                if (ProviderName != Providers.ActivePivot)
                {
                    query.Append("}");
                }
            }

            /// <summary>
            /// for building hierarchyElement if multiple levels are provided
            /// </summary>
            /// <param name="item">holds the current item</param>
            /// <param name="query">query to be generated</param>
            /// <param name="isGrandTotalOn">grand total is on or not</param>
            /// <param name="dimensionElement">holds the dimension element</param>
            private static void BuildHierarchyElement(Item item, StringBuilder query, bool isGrandTotalOn, DimensionElement dimensionElement, Providers ProviderName, bool ShowLevelTypeAll, DrillType DrillType,bool useDefaultMember,bool isLastItem,bool isPaging,bool isSameHyra,bool visualTotalsVisibility)
            {
                int hierarchyCount = 0;
                query.Append("{");

                #region adding HierarchyElement
                HierarchyElement hierarchyElement = dimensionElement.Hierarchy;
                if (hierarchyElement == null)
                {
                    throw new Exception("Hierarchy object not found");
                }

                if (hierarchyCount > 0)
                {
                    query.Append(" * ");
                }

                if (!(hierarchyElement.LevelElements.Count > 0))
                {
                    throw new Exception("LevelElement should be specified");
                }
                else
                {
                    LevelElement levelElement = (LevelElement)hierarchyElement.LevelElements[0];
                    BuildLevelElements(item, query, isGrandTotalOn, levelElement, dimensionElement, ProviderName, ShowLevelTypeAll, DrillType, useDefaultMember,isLastItem,isPaging,isSameHyra,visualTotalsVisibility);
                }

                hierarchyCount++;
                #endregion

                ////closing the braces for HierarchyElement
                query.Append("}");
            }

            /// <summary>
            /// for generating query for all the sub level elements 
            /// </summary>
            /// <param name="query">query to be generated</param>
            /// <param name="isGrandTotalOn">whether grand to is on</param>
            /// <param name="levelElement">The Level Element</param>
            /// <param name="dimensionElement">The Excluded Dimension Element</param>            
            private static void BuildSlicedLevel(StringBuilder query, bool isGrandTotalOn, LevelElement levelElement, DimensionElement dimensionElement, Providers ProviderName)
            {
                int levelCount = 0;
                if (ProviderName != Providers.ActivePivot)
                {
                    query.Append("{");
                }

                #region adding LevelElement

                if (levelCount > 0)
                {
                    query.Append(", ");
                }

                if (!(levelElement.MemberElements.Count > 0))
                {
                    GetSlicedDefaultLevel(query, dimensionElement, ProviderName);
                }
                else
                {
                    if (ProviderName == Providers.ActivePivot)
                    {
                        query.Insert(0, dimensionElement.UniqueName + ".[PL_" + dimensionElement.Name + "_FMS]" + (char)255);
                        query.Append(PropertyConstants.Member + " " + dimensionElement.UniqueName + ".[PL_" + dimensionElement.Name + "_FMS]" + " AS Aggregate({");
                        BuildSlicedMembers(query, isGrandTotalOn, levelElement, dimensionElement, ProviderName);
                        query.Append("})");
                    }
                    else
                    {
                        //MemberElementCollection memberElementCollection = (MemberElementCollection)levelElement.MemberElements;
                        BuildSlicedMembers(query, isGrandTotalOn, levelElement, dimensionElement, ProviderName);
                    }
                }

                levelCount++;
                #endregion

                ////closing the braces for LevelElement
                if (ProviderName != Providers.ActivePivot)
                {
                    query.Append("}");
                }
            }

            /// <summary>
            /// for generating query for all the sub level elements 
            /// </summary>
            /// <param name="item">Holds the current item</param>
            /// <param name="query">query to be generated</param>
            /// <param name="isGrandTotalOn">whether grand to is on</param>
            /// <param name="levelElement">The Current Level Element</param>
            /// <param name="dimensionElement">The Excluded Dimension Element</param>            
            private static void BuildLevelElements(Item item, StringBuilder query, bool isGrandTotalOn, LevelElement levelElement, DimensionElement dimensionElement, Providers ProviderName, bool ShowLevelTypeAll, DrillType DrillType,bool useDefaultMember,bool isLastItem,bool isPaging,bool isSameHyra,bool visualTotalsVisibility)
            {
                int levelCount = 0;
                query.Append("{");

                #region adding LevelElement

                if (levelCount > 0)
                {
                    query.Append(", ");
                }

                if (!(levelElement.MemberElements.Count > 0) || (isPaging && isSameHyra && !isLastItem))
                {
                    GetDefaultLevel(item, query, dimensionElement, ProviderName, ShowLevelTypeAll, useDefaultMember, isLastItem, isPaging, isSameHyra, visualTotalsVisibility);
                }
                else
                {
                    MemberElementCollection memberElementCollection = (MemberElementCollection)levelElement.MemberElements;
                    BuildMemberElement(item, query, isGrandTotalOn, levelElement, dimensionElement, ProviderName, ShowLevelTypeAll, DrillType, useDefaultMember, visualTotalsVisibility);
                }

                levelCount++;
                #endregion

                ////closing the braces for LevelElement
                query.Append("}");
            }

            /// <summary>
            /// for generating all the sub members as query from the member element collection
            /// </summary>
            /// <param name="query">query to be generated</param>
            /// <param name="isGrandTotalOn">whether grand total is on</param>
            /// <param name="levelElement">a level element object</param>
            /// <param name="dimensionElement">a dimension element object</param>
            private static void BuildSlicedMembers(StringBuilder query, bool isGrandTotalOn, LevelElement levelElement, DimensionElement dimensionElement, Providers ProviderName)
            {
                MemberElementCollection memberElementCollection = levelElement.MemberElements;
                int count = memberElementCollection.Count;
                bool breakFlag = false;
                if (count > 0)
                {
                    query.Append(GetSlicedChildMembers(memberElementCollection, true));
                    breakFlag = true;
                }

                if (!breakFlag)
                {
                    if (ProviderName == Providers.Mondrian)
                        query.Append(levelElement.ParentHierarchy.ParentDimension.UniqueName);
                    else
                        query.Append(levelElement.ParentHierarchy.UniqueName);
                }
            }

            /// <summary>
            /// for generating all the sub members as query from the member element collection
            /// </summary>
            /// <param name="item">Holds the current item</param>
            /// <param name="query">query to be generated</param>
            /// <param name="isGrandTotalOn">whether grand total is on</param>
            /// <param name="levelElement">a level element object</param>
            /// <param name="dimensionElement">The Excluded Dimension Element</param>            
            private static void BuildMemberElement(Item item, StringBuilder query, bool isGrandTotalOn, LevelElement levelElement, DimensionElement dimensionElement, Providers ProviderName, bool ShowLevelTypeAll, DrillType DrillType,bool useDefaultMember, bool visualTotalsVisibility)
            {
                StringBuilder memberString = new StringBuilder();
                StringBuilder excludeQuery = new StringBuilder();
                string drillString;
                if (ProviderName == Providers.Mondrian)
                    drillString = levelElement.ParentHierarchy.ParentDimension.UniqueName;
                else if (ProviderName == Providers.ActivePivot)
                {
                    if (levelElement.Name != null && !(levelElement.Name.Equals("ALL")))
                    {
                        drillString = dimensionElement.UniqueName + ".[" + levelElement.Name + "].Members";
                    }
                    else
                    {
                        drillString = dimensionElement.UniqueName + ".[ALL].[AllMember]";
                    }
                }
                else
                    drillString = levelElement.ParentHierarchy.UniqueName;

                MemberElementCollection memberElementCollection = levelElement.MemberElements;
                if (memberElementCollection.Count > 0)
                {
                    GetChildMemberElementSet(memberElementCollection, true, levelElement, new StringBuilder(), memberString, ProviderName, ShowLevelTypeAll, DrillType,useDefaultMember);
                }
                else
                {
                    if (ShowLevelTypeAll)
                        memberString.Append("({").Append(drillString).Append("})");
                    else
                        memberString.Append("DrilldownLevel({").Append(drillString).Append("})");
                }

                if (item != null && item.ExcludedElementValue != null)
                {
                    int maxLevel = 0;
                    if (ProviderName == Providers.ActivePivot)
                    {
                        var excludedLevelElements = ((item.ExcludedElementValue as DimensionElement).Hierarchy.LevelElements as LevelElementCollection);
                        for (int i = 0; i < excludedLevelElements.Count; i++)
                        {
                            if (excludedLevelElements[i].MemberElements.Count > 0)
                                maxLevel = (excludedLevelElements[i].MemberElements as MemberElementCollection)[0].Level > maxLevel ? (excludedLevelElements[i].MemberElements as MemberElementCollection)[0].Level : maxLevel;
                        }
                    } 
                    var exceptElements = string.Empty;
                    if (ProviderName == Providers.ActivePivot)
                    {
                        string lastMemberName = string.Empty;
                        exceptElements = AppendExceptElements(item, ref lastMemberName, ProviderName);
                        string[] splitMemberNames = lastMemberName.Split('.');
                        if (splitMemberNames.Length > 2 && splitMemberNames[1] == "[ALL]")
                        {
                            drillString = dimensionElement.UniqueName + ".[ALL].[AllMember]";
                        }

                        if (visualTotalsVisibility)
                            query.AppendFormat("hierarchize(Union(Intersect(VISUALTOTALS(Except(Descendants({0}, {1}, SELF_AND_BEFORE) {2}), {3}) , Except({3} {2}))",
                                    drillString, maxLevel, exceptElements, memberString.ToString());
                        else
                            query.AppendFormat("hierarchize(Union(Intersect((Except(Descendants({0}, {1}, SELF_AND_BEFORE) {2}), {3}) , Except({3} {2}))",
                                drillString, maxLevel, exceptElements, memberString.ToString());

                    }
                    else
                    {
                        exceptElements = AppendExceptElements(item, ref maxLevel, ProviderName);
                        if (ProviderName != Providers.Mondrian)
                        {
                            for (int levelPosition = maxLevel; levelPosition > 0; levelPosition--)
                            {
                                if (levelPosition > 1)
                                    excludeQuery.AppendFormat("Except(Descendants({0}, {1}, SELF_AND_BEFORE) {2},", drillString, levelPosition, exceptElements);
                                else
                                    excludeQuery.AppendFormat("Except(Descendants({0}, {1}, SELF_AND_BEFORE) {2}", drillString, levelPosition, exceptElements);
                            }
                            if (visualTotalsVisibility)
                                query.AppendFormat("hierarchize(Union(Intersect(VISUALTOTALS({{ {0} }}), {2}) , Except({2} {1}))",
                                                    excludeQuery.ToString(), exceptElements, memberString.ToString());
                            else
                                query.AppendFormat("hierarchize(Union(Intersect(({{ {0} }}), {2}) , Except({2} {1}))",
                                                excludeQuery.ToString(), exceptElements, memberString.ToString());
                        }
                        else
                        {
                            query.AppendFormat("hierarchize(Union(Intersect((Except(Descendants({0}, {1}, SELF_AND_BEFORE) {2}), {3}) , Except({3} {2}))",
                                    drillString, maxLevel, exceptElements, memberString.ToString());
                        }
                    }

                }
                else
                    query.Append(memberString);
            }

            /// <summary>
            /// Appends the except elements present in the Dimension Element of the item.
            /// </summary>
            /// <param name="item">The current item.</param>
            /// <param name="exceptFlag">if set to <c>true</c> [except flag].</param>
            /// <returns>The Concatenated Except Elements</returns>
            private static string AppendExceptElements(Item item, bool exceptFlag, Providers ProviderName)
            {
                if (!exceptFlag) return "";
                int i = 0;
                return AppendExceptElements(item, ref i, ProviderName);
            }
            private static string AppendExceptElements(Item item, ref int maxLevel, Providers ProviderName)
            {
                StringBuilder tempQuery = new StringBuilder();
                bool isAppended = false;
                if (item != null)
                {
                    {
                        DimensionElement dimensionElement = (DimensionElement)item.ExcludedElementValue;
                        if (dimensionElement != null)
                        {
                            tempQuery.Append(", {");
                            if (dimensionElement.Hierarchy.LevelElements.Count > 0)
                            {
                                foreach (LevelElement levelElement in dimensionElement.Hierarchy.LevelElements)
                                {
                                    foreach (MemberElement memberElement in levelElement.MemberElements)
                                    {
                                        if (isAppended)
                                        {
                                            tempQuery.Append(", ");
                                        }

                                        if (memberElement.ChildMemberElements.Count > 0)
                                        {
                                            tempQuery.Append(AppendChildMember(memberElement));
                                        }
                                        else
                                        {
                                            if ((ProviderName == Providers.Mondrian) && memberElement.UniqueName.Split('.').Count() == 4)
                                            {
                                                string[] temp = memberElement.UniqueName.Split('.');
                                                if (temp[0] == temp[2])
                                                    tempQuery.Append(temp[0] + "." + temp[3]);
                                                else
                                                    tempQuery.Append(temp[0] + "." + temp[2] + "." + temp[3]);
                                            }
                                            else
                                                tempQuery.Append(memberElement.UniqueName);
                                        }
                                        isAppended = true;
                                        if (ProviderName != Providers.ActivePivot)
                                        {
                                            if (maxLevel < memberElement.Level)
                                                maxLevel = memberElement.Level;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception("LevelElement should be specified");
                            }

                            tempQuery.Append("})");
                        }
                    }
                }
                return tempQuery.ToString();
            }

            private static string AppendExceptElements(Item item, ref string lastMemberName, Providers ProviderName)
            {
                StringBuilder tempQuery = new StringBuilder();
                bool isAppended = false;
                if (item != null)
                {
                    {
                        DimensionElement dimensionElement = (DimensionElement)item.ExcludedElementValue;
                        if (dimensionElement != null)
                        {
                            tempQuery.Append(", {");
                            if (dimensionElement.Hierarchy.LevelElements.Count > 0)
                            {
                                foreach (LevelElement levelElement in dimensionElement.Hierarchy.LevelElements)
                                {
                                    foreach (MemberElement memberElement in levelElement.MemberElements)
                                    {
                                        if (isAppended)
                                        {
                                            tempQuery.Append(", ");
                                        }

                                        if (memberElement.ChildMemberElements.Count > 0)
                                        {
                                            tempQuery.Append(AppendChildMember(memberElement));
                                        }
                                        else
                                        {
                                            if ((ProviderName == Providers.Mondrian) && memberElement.UniqueName.Split('.').Count() == 4)
                                            {
                                                string[] temp = memberElement.UniqueName.Split('.');
                                                if (temp[0] == temp[2])
                                                    tempQuery.Append(temp[0] + "." + temp[3]);
                                                else
                                                    tempQuery.Append(temp[0] + "." + temp[2] + "." + temp[3]);
                                            }
                                            else
                                            {
                                                tempQuery.Append(memberElement.UniqueName);
                                                lastMemberName = memberElement.UniqueName;
                                            }
                                        }
                                        isAppended = true;
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception("LevelElement should be specified");
                            }

                            tempQuery.Append("})");
                        }
                    }
                }
                return tempQuery.ToString();
            }
            /// <summary>
            /// returns the index value of KPIElements from the collection
            /// </summary>
            /// <param name="items">items in the axis</param>
            /// <returns>index of the KPI Element in the items collection</returns>
            static int FindIndexOfKPIElements(Items items)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    Item item = items[i];
                    if (item.ElementValue is KpiElements)
                    {
                        return i;
                    }
                }

                return -1;
            }

            /// <summary>
            /// returns the index value of KPIElements from the collection
            /// </summary>
            /// <param name="elements">Elements in the items</param>
            /// <returns>index of the KPI Element in the Element collection</returns>
            static int FindIndexOfKPIElements(ElementCollection elements)
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i] is KpiElements)
                    {
                        return i;
                    }
                }

                return -1;
            }

            /// <summary>
            /// returns the index value of Measure Elements from the collection
            /// </summary>
            /// <param name="elements">Elements contained in item all the axis values</param>
            /// <returns>index of the MeasureElement in the axis items</returns>
            static int FindIndexOfMeasureElements(ElementCollection elements)
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i] is MeasureElements)
                    {
                        return i;
                    }
                }

                return -1;
            }

            /// <summary>
            /// returns the index value of Measure Elements from the collection
            /// </summary>
            /// <param name="items">items contains all the axis values</param>
            /// <returns>index of the MeasureElement in the axis items</returns>
            static int FindIndexOfMeasureElements(Items items)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    Item item = items[i];
                    if (item.ElementValue is MeasureElements)
                    {
                        return i;
                    }
                }

                return -1;
            }

            /// <summary>
            /// Recursive invocation of this method fetches the least level members from the specified
            /// level.
            /// </summary>
            /// <param name="memberElementCollection">The member element collection.</param>
            /// <param name="isSlicer">Denotes whether invoked from slicer or from other axis.</param>
            /// <returns>Gets the concatenated strings with the last child elements</returns>
            static string GetSlicedChildMembers(MemberElementCollection memberElementCollection, bool isSlicer)
            {
                StringBuilder tempquery = new StringBuilder();
                for (int i = 0; i < memberElementCollection.Count; i++)
                {
                    bool breakFlag = false;
                    MemberElement memberElement = memberElementCollection[i];
                    if (i > 0)
                    {
                        tempquery.Append(", ");
                    }

                    if (memberElement.ChildMemberElements.Count > 0)
                    {
                        breakFlag = true;
                        tempquery.Append(GetSlicedChildMembers(memberElement.ChildMemberElements, isSlicer));
                    }

                    if (!breakFlag)
                    {
                        tempquery.Append(memberElement.UniqueName);
                    }
                }

                return tempquery.ToString();
            }

            /// <summary>
            /// Recursive invocation of this method fetches the least level members from the specified
            /// level.
            /// </summary>
            /// <param name="memberElementCollection">The member element collection.</param>
            /// <param name="parentLevel">if set to <c>true</c> [parent level].</param>
            /// <param name="levelElement">The level element.</param>
            /// <param name="parentString">The parent string.</param>
            /// <param name="query">The query.</param>
            /// <returns>Gets the Concatenated Child elements recursively</returns>
            static string GetChildMemberElementSet(MemberElementCollection memberElementCollection, bool parentLevel, LevelElement levelElement, StringBuilder parentString, StringBuilder query, Providers ProviderName, bool ShowLevelTypeAll, DrillType DrillType, bool useDefaultMember)
            {
                StringBuilder tempquery = new StringBuilder();
                var _memberElement = memberElementCollection.Cast<MemberElement>().Select(m => m).Where(m => m.ShowChildMembers == true);
                if (DrillType == Reports.DrillType.DrillReplace && _memberElement.Count() > 0)
                {
                    GetChildMemberElementSet(_memberElement.ElementAt(0).ChildMemberElements, false, levelElement, parentString, query, ProviderName, ShowLevelTypeAll, DrillType,useDefaultMember);
                }
                else if (DrillType == Reports.DrillType.DrillReplace && memberElementCollection._parentElement is MemberElement && (memberElementCollection._parentElement as MemberElement).ShowChildMembers == true)
                {
                    query.Append((memberElementCollection._parentElement as MemberElement).UniqueName + ".Children");
                }
                else
                {
                    #region ParentString Manipulation
                    if (parentString == null || parentString.ToString() == string.Empty)
                    {
                        bool isAppended = false;
                        if (ProviderName == Providers.ActivePivot)
                        {
                            if (levelElement.Name == "ALL")
                                parentString.Append("Hierarchize(DrilldownMember(DrilldownLevel({");
                            else
                                parentString.Append("Hierarchize(DrilldownMember({");
                        }
                        else
                        {
                            if (ShowLevelTypeAll)
                                parentString.Append("DrilldownMember({");
                            else
                                parentString.Append("DrilldownMember(DrilldownLevel({");
                        }
                        if ((ProviderName == Providers.Mondrian))
                            parentString.Append(levelElement.ParentHierarchy.ParentDimension.UniqueName);
                        else if ((ProviderName == Providers.ActivePivot))
                        {
                            if (levelElement.Name == "ALL")
                            {
                                parentString.Append(levelElement.ParentHierarchy.ParentDimension.UniqueName + ".[" + levelElement.Name + "].[AllMember]");
                            }
                            else
                            {
                                parentString.Append(levelElement.ParentHierarchy.ParentDimension.UniqueName + ".[" + levelElement.Name + "].Members");
                            }
                        }
                        else
                        {
                            if (useDefaultMember)
                                parentString.Append(levelElement.ParentHierarchy.UniqueName); ////parentString.Append(levelElement.UniqueName); //
                            else
                            {
                                if (levelElement.ParentHierarchy.IsAttributeHierarchy)
                                    parentString.Append(levelElement.ParentHierarchy.UniqueName + ".Members");
                                else
                                    parentString.Append(levelElement.ParentHierarchy.UniqueName + ".[All]");
                            }
                        }
                        if (ShowLevelTypeAll || (ProviderName == Providers.ActivePivot && levelElement.Name != "ALL"))
                            parentString.Append("},{");
                        else
                            parentString.Append("}),{");
                        for (int i = 0; i < memberElementCollection.Count; i++)
                        {
                            MemberElement memberElement = memberElementCollection[i];

                            if (memberElement.ShowChildMembers)
                            {
                                if (i > 0 && isAppended)
                                {
                                    parentString.Append(",");
                                }

                                parentString.Append(memberElement.UniqueName);
                                isAppended = true;
                            }
                        }

                        if (ProviderName == Providers.ActivePivot)
                        {
                            parentString.Append("}))");
                        }
                        else
                        {
                            parentString.Append("})");
                        }
                    }
                    else
                    {
                        parentString.Append(",{");
                        bool isAppended = false;
                        for (int i = 0; i < memberElementCollection.Count; i++)
                        {
                            MemberElement memberElement = memberElementCollection[i];
                            if (memberElement.ShowChildMembers)
                            {
                                if (i > 0 && isAppended)
                                {
                                    parentString.Append(",");
                                }

                                parentString.Append(memberElement.UniqueName);
                                isAppended = true;
                            }
                        }

                        parentString.Append("})");
                    }
                    #endregion

                    #region Recursive invocation based on the memberElements.ChildElements
                    for (int i = 0; i < memberElementCollection.Count; i++)
                    {
                        MemberElement memberElement = memberElementCollection[i];
                        if (memberElement.ChildMemberElements.Count > 0)
                        {
                            query.Append("DrilldownMember(");
                            tempquery.Append(GetChildMemberElementSet(memberElement.ChildMemberElements, false, levelElement, parentString, query, ProviderName, ShowLevelTypeAll, DrillType,useDefaultMember));
                        }
                    }

                    if (parentLevel)
                    {
                        query.Append(parentString.ToString());
                    }
                    #endregion
                }
                return tempquery.ToString();
            }

            /// <summary>
            /// getting default members of level elements if in case the levels are not specified
            /// in either Dimension Element or HierarchyElement
            /// </summary>
            /// <param name="query">query to be generated</param>
            /// <param name="dimensionElement">Dimension Element</param>
            private static void GetSlicedDefaultLevel(StringBuilder query, DimensionElement dimensionElement, Providers ProviderName)
            {
                if (dimensionElement.Hierarchy != null)
                {
                    if (ProviderName != Providers.ActivePivot)
                    {
                        query.Append("(");
                    }
                    if (ProviderName == Providers.Mondrian)
                        query.Append(dimensionElement.UniqueName);
                    else if (ProviderName == Providers.ActivePivot)
                    {
                        if (dimensionElement.Hierarchy.LevelElements.Count <= 1)
                            query.Append(string.Format("{0}.[ALL].[AllMember]", dimensionElement.UniqueName));
                    }
                    else
                    {
                        query.Append(dimensionElement.Hierarchy.LevelElements[0].UniqueName);
                    }
                    if (ProviderName != Providers.ActivePivot)
                    {
                        query.Append(")");
                    }
                }
                else
                {
                    throw new Exception("Hierarchy should be specified");
                }
            }

            /// <summary>
            /// getting default members of level elements if in case the levels are not specified
            /// in either Dimension Element or HierarchyElement
            /// </summary>
            /// <param name="item">Holds the current item</param>
            /// <param name="query">query to be generated</param>
            /// <param name="dimensionElement">Dimension Element</param>
            private static void GetDefaultLevel(Item item, StringBuilder query, DimensionElement dimensionElement, Providers ProviderName, bool ShowLevelTypeAll,bool useDefaultMember,bool isLastItem,bool isPaging,bool isSameHyra, bool visualTotalsVisibility)
            {
                if (dimensionElement.Hierarchy != null)
                {
                    string drillUniqueName = string.Empty;
                    StringBuilder excludeQuery =  new StringBuilder();
                    StringBuilder drillQuery;
                    if (ProviderName == Providers.ActivePivot)
                    {
                        if (dimensionElement.Hierarchy != null && dimensionElement.Hierarchy.LevelElements[0] != null
                            && dimensionElement.Hierarchy.LevelElements[0].Name != null && !(dimensionElement.Hierarchy.LevelElements[0].Name.Equals("ALL")))
                        {

                            drillUniqueName = dimensionElement.UniqueName + ".[" + dimensionElement.Hierarchy.LevelElements[0].Name + "].Members";
                        }
                        else
                        {
                            drillUniqueName = dimensionElement.UniqueName + ".[ALL].[AllMember]";
                        }
                    }
                    else if (ProviderName == Providers.Mondrian)
                        drillUniqueName = "[" + dimensionElement.HierarchyName + "]";
                    else
                    {
                        if (dimensionElement.Hierarchy.LevelElements.Count == 1 && useDefaultMember)
                            drillUniqueName = dimensionElement.Hierarchy.UniqueName;
                        else if (dimensionElement.Hierarchy.LevelElements.Count == 1)
                        {
                            if (dimensionElement.Hierarchy.IsAttributeHierarchy)
                                drillUniqueName = dimensionElement.Hierarchy.UniqueName + ".Members";
                            else
                                drillUniqueName = dimensionElement.Hierarchy.UniqueName + ".[All]";
                        }

                        else if (dimensionElement.Hierarchy.LevelElements.Count > 1)
                        {
                            foreach (var item1 in dimensionElement.Hierarchy.LevelElements)
                            {
                                drillUniqueName += ((LevelElement)item1).UniqueName + ",";
                            }
                            drillUniqueName = drillUniqueName.TrimEnd(',');
                        }
                    }
                    if (ProviderName == Providers.ActivePivot)
                    {
                        if (dimensionElement.Hierarchy != null && dimensionElement.Hierarchy.LevelElements[0] != null && dimensionElement.Hierarchy.LevelElements[0].Name != "ALL")
                            drillQuery = new StringBuilder().AppendFormat("Hierarchize({{ {0} }})", drillUniqueName);
                        else
                            drillQuery = new StringBuilder().AppendFormat("Hierarchize(Drilldownlevel({{ {0} }}))", drillUniqueName);
                    }
                    else
                    {
                        if (ShowLevelTypeAll)
                        {
                            drillQuery = new StringBuilder().AppendFormat("{{ {0} }}", drillUniqueName);
                        }
                        else
                        {
                            if (!isPaging)
                            {
                                drillQuery = new StringBuilder().AppendFormat("Drilldownlevel({{ {0} }})", drillUniqueName);
                            }
                            else
                            {
                                if (!isLastItem && isSameHyra)
                                {
                                    drillUniqueName = drillUniqueName + ".CHILDREN";
                                    drillQuery = new StringBuilder().AppendFormat("Drilldownlevel({{ {0} }})", drillUniqueName);
                                }
                                else
                                    drillQuery = new StringBuilder().AppendFormat("Drilldownlevel({{ {0} }})", drillUniqueName);                             
                            }
                        }
                    }

                    if (item != null && item.ExcludedElementValue != null)
                    {
                        int maxLevel = 0;
                        if (ProviderName == Providers.ActivePivot)
                        {
                            var excludedLevelElements = ((item.ExcludedElementValue as DimensionElement).Hierarchy.LevelElements as LevelElementCollection);
                            for (int i = 0; i < excludedLevelElements.Count; i++)
                            {
                                if (excludedLevelElements[i].MemberElements.Count > 0)
                                    maxLevel = (excludedLevelElements[i].MemberElements as MemberElementCollection)[0].Level > maxLevel ? (excludedLevelElements[i].MemberElements as MemberElementCollection)[0].Level : maxLevel;
                            }
                        }
                        var exceptElements = string.Empty;
                        if (ProviderName == Providers.ActivePivot)
                        {
                            string lastMemberName = string.Empty;
                            exceptElements = AppendExceptElements(item, ref lastMemberName, ProviderName);
                            string[] splitMemberNames = lastMemberName.Split('.');
                            if (splitMemberNames.Length > 2 && splitMemberNames[1] == "[ALL]")
                            {
                                drillUniqueName = dimensionElement.UniqueName + ".[ALL].[AllMember]";
                            }
                            if (visualTotalsVisibility)
                                query.AppendFormat("hierarchize(Union(Intersect(VISUALTOTALS(Except(Descendants({0}, {1}, SELF_AND_BEFORE) {2}), {3}) , Except({3} {2}))",
                                                    drillUniqueName, maxLevel, exceptElements, drillQuery.ToString());
                            else
                                query.AppendFormat("hierarchize(Union(Intersect((Except(Descendants({0}, {1}, SELF_AND_BEFORE) {2}), {3}) , Except({3} {2}))",
                                                drillUniqueName, maxLevel, exceptElements, drillQuery.ToString());
                        }
                        else
                        {
                            exceptElements = AppendExceptElements(item, ref maxLevel, ProviderName);

                            if (ProviderName != Providers.Mondrian)
                            {
                                for (int levelPosition = maxLevel; levelPosition > 0; levelPosition--)
                                {
                                    if (levelPosition > 1)
                                        excludeQuery.AppendFormat("Except(Descendants({0}, {1}, SELF_AND_BEFORE) {2},", drillUniqueName, levelPosition, exceptElements);
                                    else
                                        excludeQuery.AppendFormat("Except(Descendants({0}, {1}, SELF_AND_BEFORE) {2}", drillUniqueName, levelPosition, exceptElements);
                                }
                                if (visualTotalsVisibility)
                                    query.AppendFormat("hierarchize(Union(Intersect(VISUALTOTALS({{ {0} }}), {2}) , Except({2} {1}))",
                                                        excludeQuery.ToString(), exceptElements, drillQuery.ToString());
                                else
                                    query.AppendFormat("hierarchize(Union(Intersect(({{ {0} }}), {2}) , Except({2} {1}))",
                                                       excludeQuery.ToString(), exceptElements, drillQuery.ToString());
                            }
                            else
                            {
                                query.AppendFormat("hierarchize(Union(Intersect((Except(Descendants({0}, {1}, SELF_AND_BEFORE) {2}), {3}) , Except({3} {2}))",
                                                    drillUniqueName, maxLevel, exceptElements, drillQuery.ToString());
                            }
                        }
                    }
                    else
                        query.Append(drillQuery);
                }
                else
                {
                    throw new Exception("Hierarchy should be specified");
                }
            }
            #endregion

            #region Public Methods
            /// <summary>
            /// Generates the MDX Query based on the MDXQuerySpecification
            /// </summary>
            /// <param name="mdxQuerySpecification">The MDX query specification.</param>
            /// <returns>MDX query of type string</returns>
            public static string GenerateQueryEx(MDXQuerySpecification mdxQuerySpecification, bool isCount, Providers ProviderName, List<SlicerRangeFiltersInfo> SlicerRangeInfoFields, SerializableDictionary<string, HeaderPositionsInfo> DrilledCells, bool ShowLevelTypeAll, DrillType DrillType,bool visualTotalVisibility,bool UseDefaultMember)
            {
                try
                {
                    #region Initilization
                    bool columnCountAppended = false, rowCountAppended = false; ;

                    Items calautaltedItems = new Items();

                    foreach (Item item in mdxQuerySpecification.With.Items)
                    {
                        calautaltedItems.Add(item);
                    }

                    Items customNamedSet = new Items();
                    var namedItems = mdxQuerySpecification.Select.Items.List.Where(i => i.ElementValue is NamedSetElement && (i.ElementValue as NamedSetElement).IsQueryScoped).ToList();
                    namedItems.AddRange(mdxQuerySpecification.Slicer.Items.List.Where(i => i.ElementValue is NamedSetElement && (i.ElementValue as NamedSetElement).IsQueryScoped));

                    foreach (var namedSet in namedItems)
                        customNamedSet.Add(namedSet.Clone());

                    var var_itemsColumn = mdxQuerySpecification.Select.Items.List.Where(i => (i.Axis == AxisPosition.Categorical && !(i.ElementValue is SortElement) && !(i.ElementValue is TopCountElement) && !(i.ElementValue is SubsetElement) && !i.IsFilterOrSortOn)).Select(i => i);
                    Items itemsColumn = new Items();
                    foreach (Item item in var_itemsColumn)
                    {
                        itemsColumn.Add(item);
                    }

                    var var_itemsRow = mdxQuerySpecification.Select.Items.List.Where(i => (i.Axis == AxisPosition.Series && !(i.ElementValue is SortElement) && !(i.ElementValue is TopCountElement) && !(i.ElementValue is SubsetElement) && !i.IsFilterOrSortOn)).Select(i => i);
                    Items itemsRow = new Items();
                    foreach (Item item in var_itemsRow)
                    {
                        itemsRow.Add(item);
                    }

                    var itemsColumns = mdxQuerySpecification.Filter.Items.List.Where(i => i.Axis == AxisPosition.Categorical).Select(i => i.ElementValue).Cast<FilterElement>().Select(i => i);
                    var itemsRows = mdxQuerySpecification.Filter.Items.List.Where(i => i.Axis == AxisPosition.Series).Select(i => i.ElementValue).Cast<FilterElement>().Select(i => i);

                    FilterElement filterElementColumns = null, filterElementRows = null;
                    List<FilterElement> filterElementRowList = new List<FilterElement>();
                    List<FilterElement> filterElementColumnList = new List<FilterElement>();

                    foreach (FilterElement __filterElement in itemsColumns)
                    {
                        filterElementColumnList.Add(__filterElement);
                    }

                    foreach (FilterElement __filterElement in itemsRows)
                    {
                        filterElementRowList.Add(__filterElement);
                    }

                    if (itemsColumns.Count() > 0)
                    {
                        filterElementColumns = itemsColumns.First();
                    }

                    if (itemsRows.Count() > 0)
                    {
                        filterElementRows = itemsRows.First();
                    }

                    ////Extracting Sort
                    var __itemSortCategory = mdxQuerySpecification.Select.Items.List.Where(i => i.Axis == AxisPosition.Categorical).Where(i => i.ElementValue is SortElement).Select(i => i);
                    var __itemSortSeries = mdxQuerySpecification.Select.Items.List.Where(i => i.Axis == AxisPosition.Series).Where(i => i.ElementValue is SortElement).Select(i => i);

                    SortElement sortElementsCategory = null;
                    SortElement sortElementsSeries = null;
                    foreach (var item in __itemSortCategory)
                    {
                        sortElementsCategory = item.ElementValue as SortElement;
                    }

                    foreach (var item in __itemSortSeries)
                    {
                        sortElementsSeries = item.ElementValue as SortElement;
                    }

                    if (sortElementsCategory == null)
                    {
                        //// Creating a fake SortElement Object
                        sortElementsCategory = new SortElement(AxisPosition.Slicer, SortOrder.ASC, false);
                    }

                    if (sortElementsSeries == null)
                    {
                        //// Creating a fake SortElement Object
                        sortElementsSeries = new SortElement(AxisPosition.Slicer, SortOrder.ASC, false);
                    }

                    ////Extracting TopCount items
                    var __itemTopCountCategory = mdxQuerySpecification.Select.Items.List.Where(i => i.Axis == AxisPosition.Categorical).Where(i => i.ElementValue is TopCountElement).Select(i => i);
                    var __itemTopCountSeries = mdxQuerySpecification.Select.Items.List.Where(i => i.Axis == AxisPosition.Series).Where(i => i.ElementValue is TopCountElement).Select(i => i);

                    TopCountElement topCountCategory = null;
                    TopCountElement topCountSeries = null;
                    foreach (var item in __itemTopCountCategory)
                    {
                        topCountCategory = item.ElementValue as TopCountElement;
                    }

                    foreach (var item in __itemTopCountSeries)
                    {
                        topCountSeries = item.ElementValue as TopCountElement;
                    }

                    if (topCountCategory == null)
                    {
                        //// Creating a fake TopCount Object
                        topCountCategory = new TopCountElement(AxisPosition.Slicer, 0);
                    }

                    if (topCountSeries == null)
                    {
                        //// Creating a fake TopCount Object
                        topCountSeries = new TopCountElement(AxisPosition.Slicer, 0);
                    }

                    ////Extracting SubSetElement items
                    var __itemSubSetCategory = mdxQuerySpecification.Select.Items.List.Where(i => i.Axis == AxisPosition.Categorical).Where(i => i.ElementValue is SubsetElement).Select(i => i).FirstOrDefault();
                    var __itemSubSetSeries = mdxQuerySpecification.Select.Items.List.Where(i => i.Axis == AxisPosition.Series).Where(i => i.ElementValue is SubsetElement).Select(i => i).FirstOrDefault();

                    SubsetElement subsetElementCategory = null;
                    SubsetElement subsetElementSeries = null;
                    if (__itemSubSetCategory != null)
                    {
                        subsetElementCategory = (__itemSubSetCategory as Item).ElementValue as SubsetElement;
                    }

                    if (__itemSubSetSeries != null)
                    {
                        subsetElementSeries = (__itemSubSetSeries as Item).ElementValue as SubsetElement;
                    }

                    if (subsetElementCategory == null)
                    {
                        //// Creating a fake TopCount Object
                        subsetElementCategory = new SubsetElement();
                        subsetElementCategory.Axis = AxisPosition.Slicer;
                    }
                    else
                    {
                        subsetElementCategory.Axis = AxisPosition.Categorical;
                    }

                    if (subsetElementSeries == null)
                    {
                        //// Creating a fake TopCount Object
                        subsetElementSeries = new SubsetElement();
                        subsetElementSeries.Axis = AxisPosition.Slicer;
                    }
                    else
                    {
                        subsetElementSeries.Axis = AxisPosition.Series;
                    }
                    #endregion

                    #region Appending QueryScoped NamedSet

                    StringBuilder withQuery = new StringBuilder();

                    if (customNamedSet.Count > 0)
                    {
                        foreach (var item in customNamedSet)
                        {
                            NamedSetElement namedSet = item.ElementValue as NamedSetElement;
                            if (namedSet.IsQueryScoped && namedSet.SetQuery != string.Empty)
                                CreateSet(namedSet.UniqueName, namedSet.SetQuery, withQuery);
                        }
                    }

                    #endregion

                    #region Appending CalculatedMember

                    if (calautaltedItems.Count > 0)
                    {
                        foreach (Item member in calautaltedItems)
                        {
                            if (member.ElementValue is CalculatedMember)
                                withQuery.Append((member.ElementValue as CalculatedMember).CustomExpression);
                            else if (member.ElementValue is VirtualKpiElement)
                            {
                                VirtualKpiElement virtualKpiElement = (member.ElementValue as VirtualKpiElement);
                                string kpiQuery = string.Empty;
                                if (!string.IsNullOrEmpty(virtualKpiElement.KpiValueExpression))
                                    kpiQuery += " MEMBER [Measures].[" + virtualKpiElement.Name + " Value] AS " + virtualKpiElement.KpiValueExpression;
                                if (!string.IsNullOrEmpty(virtualKpiElement.KpiGoalExpression))
                                    kpiQuery += " MEMBER [Measures].[" + virtualKpiElement.Name + " Goal] AS " + virtualKpiElement.KpiGoalExpression;
                                if (!string.IsNullOrEmpty(virtualKpiElement.KpiStatusExpression))
                                    kpiQuery += " MEMBER [Measures].[" + virtualKpiElement.Name + " Status] AS " + virtualKpiElement.KpiStatusExpression;
                                if (!string.IsNullOrEmpty(virtualKpiElement.KpiTrendExpression))
                                    kpiQuery += " MEMBER [Measures].[" + virtualKpiElement.Name + " Trend] AS " + virtualKpiElement.KpiTrendExpression;
                                withQuery.Append(kpiQuery);
                            }
                        }
                        withQuery.Replace("'", "");
                    }

                    #endregion
                    #region Column Axis Element Generation

                    StringBuilder columnQuery = new StringBuilder();

                    if (itemsColumn.Count > 0 || filterElementColumnList.Count > 0 || filterElementColumns != null)
                    {
                        columnCountAppended = true;

                        SortElement tempSortElementColumns = sortElementsCategory.Axis == AxisPosition.Categorical ? sortElementsCategory : null;
                        TopCountElement tempTopCountElementColumns = topCountCategory.Axis == AxisPosition.Categorical ? topCountCategory : null;
                        SubsetElement subsetElementColumns = subsetElementCategory.Axis == AxisPosition.Categorical ? subsetElementCategory : null;

                        mdxQuerySpecification.IsKPI = BuildAxisItem(itemsColumn, columnQuery, mdxQuerySpecification.ShowGrandTotal, tempSortElementColumns, filterElementColumnList, tempTopCountElementColumns, subsetElementColumns, DrilledCells["ColumnHeader"], ProviderName, ShowLevelTypeAll, DrillType,UseDefaultMember,mdxQuerySpecification.IsPagingEnabled,visualTotalVisibility);

                        if (mdxQuerySpecification.IsKPI)
                        {
                            mdxQuerySpecification.KPIAxis = AxisPosition.Categorical;
                        }
                    }
                    #endregion

                    #region Row Axis Element Generation

                    StringBuilder rowQuery = new StringBuilder();

                    if ((itemsRow.Count > 0 || filterElementRowList.Count > 0 || filterElementRows != null) && (itemsColumn.Count > 0 || filterElementColumnList.Count > 0 || filterElementColumns != null))
                    {
                        rowCountAppended = true;

                        SortElement tempSortElementRows = sortElementsSeries.Axis == AxisPosition.Series ? sortElementsSeries : null;
                        TopCountElement tempTopCountElementRows = topCountSeries.Axis == AxisPosition.Series ? topCountSeries : null;
                        SubsetElement subsetElementRows = subsetElementSeries.Axis == AxisPosition.Series ? subsetElementSeries : null;

                        bool isKPIIncluded = BuildAxisItem(itemsRow, rowQuery, mdxQuerySpecification.ShowGrandTotal, tempSortElementRows, filterElementRowList, tempTopCountElementRows, subsetElementRows, DrilledCells["RowHeader"], ProviderName, ShowLevelTypeAll, DrillType,UseDefaultMember,mdxQuerySpecification.IsPagingEnabled,visualTotalVisibility);

                        if (!mdxQuerySpecification.IsKPI)
                        {
                            mdxQuerySpecification.IsKPI = isKPIIncluded;
                            mdxQuerySpecification.KPIAxis = AxisPosition.Series;
                        }
                    }

                    filterElementColumns = null; filterElementRows = null;
                    filterElementRowList = null; filterElementColumnList = null;
                    #endregion

                    #region Appending NonEmpty

                    var columnItemCount = columnQuery.ToString().Split(new string[] { "*" }, StringSplitOptions.RemoveEmptyEntries).Length;
                    var rowItemCount = rowQuery.ToString().Split(new string[] { "*" }, StringSplitOptions.RemoveEmptyEntries).Length;
                    string columnString = columnQuery.ToString(), rowString = rowQuery.ToString();

                    if (columnQuery.Length > 0 && (rowQuery.Length > 0 || isCount) && !mdxQuerySpecification.ShowEmptyColumnData)
                        columnQuery = new StringBuilder().Append(ApplyNonEmpty(columnString, rowString, itemsColumn.Count == 1 && itemsColumn.List[0].ElementValue is MeasureElements, mdxQuerySpecification.IsPagingEnabled, ProviderName));
                    if (rowQuery.Length > 0 && !mdxQuerySpecification.ShowEmptyRowData)
                        rowQuery = new StringBuilder().Append(ApplyNonEmpty(rowString, columnString, itemsRow.Count == 1 && itemsRow.List[0].ElementValue is MeasureElements, mdxQuerySpecification.IsPagingEnabled, ProviderName));

                    #endregion

                    #region Appending Count
                    if (isCount)
                    {
                        if (columnQuery.Length > 0)
                        {
                            CreateSet(ColumnGeneratedSet, columnQuery.ToString(), withQuery);
                            columnQuery = new StringBuilder().AppendFormat(" MEMBER {0} AS Count({1})", ColumnsCount, ColumnGeneratedSet);
                        }
                        if (rowQuery.Length > 0)
                        {
                            CreateSet(RowGeneratedSet, rowQuery.ToString(), withQuery);
                            rowQuery = new StringBuilder().AppendFormat(" MEMBER {0} AS Count ({1})", RowsCount, RowGeneratedSet);
                        }
                    }
                    #endregion

                    #region Applying Paging

                    if (!isCount && mdxQuerySpecification.IsPagingEnabled)
                    {
                        if (columnQuery.Length != 0)
                        {
                            Page page = new Page(mdxQuerySpecification.Page.CategorialCurrentPage, mdxQuerySpecification.Page.CategorialPageSize);
                            CreatePagedSet(ColumnPagedSet, columnQuery.ToString(), withQuery, page);
                            CreateSet(ColumnGeneratedSet, GenerateAscendants(page, ColumnPagedSet, columnItemCount).ToString(), withQuery);
                            columnQuery = new StringBuilder().Append(ColumnPagedSet);
                        }

                        if (rowQuery.Length != 0)
                        {
                            Page page = new Page(mdxQuerySpecification.Page.SeriesCurrentPage, mdxQuerySpecification.Page.SeriesPageSize);
                            CreatePagedSet(RowPagedSet, rowQuery.ToString(), withQuery, page);
                            CreateSet(RowGeneratedSet, GenerateAscendants(page, RowPagedSet, rowItemCount).ToString(), withQuery);
                            rowQuery = new StringBuilder().Append(RowPagedSet);
                        }
                    }

                    #endregion

                    #region Prepending WITH

                    StringBuilder query = new StringBuilder();

                    if (withQuery.Length > 0 || isCount)
                    {
                        withQuery.Insert(0, " WITH ");
                        if (isCount) withQuery.Append(columnQuery).Append(rowQuery);
                        query.Append(withQuery);
                    }
                    #endregion

                    #region Appending Select

                    if (!isCount)
                    {
                        if (columnQuery.Length > 0) columnQuery.Append(GeneratePropertyString("COLUMNS", _columnPropertiesQueryString, ProviderName));
                        if (rowQuery.Length > 0) rowQuery.Append(GeneratePropertyString("ROWS", _rowPropertiesQueryString, ProviderName));

                        _columnPropertiesQueryString = new StringBuilder();
                        _rowPropertiesQueryString = new StringBuilder();

                        if ((columnQuery.Length < 1) && (rowQuery.Length < 1))
                        {
                            query.Append(" SELECT {} ON 0 ");
                        }
                        else if (ProviderName != Providers.Mondrian)
                        {
                            query.AppendFormat(" SELECT {0} {1} {2} {3} ", columnQuery.Length < 1 || rowQuery.Length < 1 ? " NON EMPTY " : "", columnQuery, columnQuery.Length > 0 && rowQuery.Length > 0 ? "," : "", rowQuery);
                        }
                        else
                        {
                            query.AppendFormat(" SELECT {0} {1} {2} ", columnQuery, columnQuery.Length > 0 && rowQuery.Length > 0 ? "," : "", rowQuery);
                        }
                    }
                    #endregion

                    #region Appending Count

                    else
                    {
                        query.AppendFormat(" SELECT {{ {0} {1} {2} }} ON AXIS(0)", columnCountAppended ? ColumnsCount : "", rowCountAppended && columnCountAppended ? "," : "", rowCountAppended ? RowsCount : "");
                    }
                    #endregion

                    #region Appending Cube Name/Filter/Where Clause

                    query.Append(" FROM ");
                    if (ProviderName == Providers.ActivePivot)
                    {
                        string[] whereWithParts = BuildFilterCondition(mdxQuerySpecification.Slicer, itemsColumn, itemsRow, isCount, ProviderName, SlicerRangeInfoFields, ShowLevelTypeAll).Split((char)255);
                        if (whereWithParts.Length > 1)
                        {
                            if (query.ToString().StartsWith("WITH "))
                            {
                                query.Insert((query.ToString().IndexOf("WITH ") + "WITH ".Length), whereWithParts[whereWithParts.Length - 1] + string.Empty);
                            }
                            else
                            {
                                if (whereWithParts[whereWithParts.Length - 1] != string.Empty)
                                    query.Insert(0, "WITH " + whereWithParts[whereWithParts.Length - 1] + string.Empty);
                            }
                            for (int q = 0; q < whereWithParts.Length - 1; q++)
                            {
                                query.Append(whereWithParts[q]);
                            }
                        }
                        else
                        {
                            query.Append(whereWithParts[whereWithParts.Length - 1]);
                        }
                    }
                    else
                    {
                        query.Append(BuildFilterCondition(mdxQuerySpecification.Slicer, itemsColumn, itemsRow, isCount, ProviderName, SlicerRangeInfoFields, ShowLevelTypeAll));
                    }

                    #endregion

                    #region Appending Cell Properties
                    if (!isCount && ProviderName != Providers.Mondrian)
                        query.Append("  CELL PROPERTIES VALUE, FORMAT_STRING, FORMATTED_VALUE");
                    #endregion

                    return query.ToString();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            #endregion

            #region Private Static Methods

            private static string ApplyNonEmpty(string currentItem, string otherItem, bool isMeasureOnly, bool isPagingEnabled, Providers ProviderName)
            {
                if (ProviderName == Providers.Mondrian) return currentItem;
                if (isMeasureOnly) 
                    otherItem = string.Empty;
                else if (!isPagingEnabled)
                    otherItem = otherItem.Length < 1 ? "[Measures].DefaultMember" : otherItem;
                if (ProviderName == Providers.ActivePivot)
                    if (isPagingEnabled)
                        return string.Format(" Hierarchize(NONEMPTY({0}{1}{2})) ", currentItem, otherItem.Length > 0 ? "," : "", otherItem);
                    else
                        return string.Format("NON EMPTY {0}", currentItem); ////This line won't support the Paging feature.
                if (isMeasureOnly && !isPagingEnabled)
                    return string.Format(" NON EMPTY({0}{1}{2}) ", currentItem, otherItem.Length > 0 ? "," : "", otherItem);
                else
                    return string.Format(" NONEMPTY({0}{1}{2}) ", currentItem, otherItem.Length > 0 ? "," : "", otherItem);
            }

            private static string GeneratePropertyString(string axis, StringBuilder propertyQueryString, Providers ProviderName)
            {
                return string.Format(" dimension properties MEMBER_TYPE, PARENT_UNIQUE_NAME{0} {1} ON {2}", propertyQueryString.Length == 0 ? "" : ",", propertyQueryString.ToString(), axis);
            }

            private static void CreatePagedSet(string setName, string setQuery, StringBuilder withQuery, Page pageValues)
            {
                withQuery.AppendFormat(" SET {0} as SUBSET({{ {1} }}, {2}, {3})", setName, setQuery, pageValues.GetStartIndex(), pageValues.PageSize);
            }

            private static void CreateSet(string setName, string setQuery, StringBuilder withQuery)
            {
                withQuery.AppendFormat(" SET {0} as {1}", setName, setQuery);
            }

            private static string GenerateCellPositionMdx(Dictionary<string, string> positionDect)
            {
                return string.Join("*", positionDect.Select(d => "{" + d.Value + "}").ToArray());
            }

            private static StringBuilder GenerateAscendants(Page pageValues, string axisString, int itemCount)
            {
                if (pageValues.PageSize < 1) return new StringBuilder(axisString);

                StringBuilder pagedQuery = new StringBuilder(" Distinct({Hierarchize({ Generate({").Append(axisString).Append("}, ");

                for (var i = 0; i < itemCount; i++)
                {
                    pagedQuery.AppendFormat(" {0} Ascendants({1}.Current.item({2}))", i == 0 ? "" : " * ", axisString, i);
                }

                return pagedQuery.Append(")})})");
            }

            #endregion
        }
    }
}
