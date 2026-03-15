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
using Syncfusion.Olap.Reports;
using System.Globalization;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Syncfusion.Olap.MDXQueryParser
{
    /// <summary>
    /// Parsing the MDX Query
    /// </summary>
    public class Parser
    {
        #region Fields
        public static Select select;
        public static string MDXQueryNew = string.Empty;
        public static OlapReport olapReport = null;
        public static bool subselectoption = false;
        public static bool measureOption = false;
        public static char tempchar;
        #endregion

        #region Helper Methods

        /// <summary>
        /// Generate the Olap Report Builder
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static OlapReport generateReport(string query)
        {
            olapReport = new OlapReport();
            string newMDXQuery = query.Replace("\r\n"," ");
            MDXQueryNew = RemoveExtraSpace(newMDXQuery);
            MatchParentheses(MDXQueryNew, '(', ')');
            MatchParentheses(MDXQueryNew, '{', '}');
            MatchParentheses(MDXQueryNew, '[', ']');

            Tokens tokens = new Tokens(MDXQueryNew);

            CheckWithSelect(tokens);

            //cube name
            string[] olapCubeName = select.FromClause.CubeName.Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
            olapReport.CurrentCubeName = olapCubeName[0].Trim().ToString();
            olapReport.EngineVersion = QueryBuilderEngineVersions.Version2;

            MeasureElements measureElement = new MeasureElements();
            foreach (Axis axisNew in select.Axes)
            {   
                if ((axisNew.Name == KeywordConstants.Columns) || (axisNew.Name == KeywordConstants.ColumnsNum))
                {
                    getTuples(olapReport.CategoricalElements, axisNew, measureElement);
                    if (!CheckMeasureAxis(olapReport))
                    {
                        AddMeasureElement(olapReport.CategoricalElements, measureElement);
                    }
                }
                if ((axisNew.Name == KeywordConstants.Rows) || (axisNew.Name == KeywordConstants.RowsNum))
                {
                    getTuples(olapReport.SeriesElements, axisNew, measureElement);
                    if (!CheckMeasureAxis(olapReport))
                    {
                        AddMeasureElement(olapReport.SeriesElements, measureElement);
                    }
                }
            }

            if (select.Wheres.Tuples.Count > 0)
            {
                Axis ax = null;
                Items itemsSlicer = new Items();
                itemsSlicer = olapReport.SlicerElements;
                foreach (Tuple tuples in select.Wheres.Tuples)
                {
                    getTuplesMembers(itemsSlicer, tuples, ax, measureElement);
                }
                AddMeasureElement(olapReport.SlicerElements, measureElement);
            }           
            return olapReport;
        }

        private static bool CheckMeasureAxis(OlapReport olapReport)
        {
            if (CheckMeasureItems(olapReport.CategoricalElements))
            {
                return true;
            }
            if (CheckMeasureItems(olapReport.SeriesElements))
            {
                return true;
            }
            return false;
        }

        private static bool CheckMeasureItems(Items items)
        {
            foreach (Item item in items)
            {
                if (item.ElementValue is MeasureElements)
                {
                    return true;
                }
            }
            return false;
        }

        private static void AddMeasureElement(Items items, MeasureElements measureElement)
        {
            if (measureElement.Elements.Count > 0)
            {
                items.Add(new Item { ElementValue = measureElement });
                measureOption = true;
            }
        }

        public static void getTuples(Items items, Axis axis)
        {
            getTuples(items, axis, null);
        }

        public static void getTuples(Items items, Axis axis, MeasureElements measureElement)
        {
            foreach (Tuple tupleNew in axis.TupleSet)
            {
                getTuplesMembers(items, tupleNew, axis, measureElement);
            }
        }

        public static void getTuplesMembers(Items items, Tuple tuples, Axis axis)
        {
            getTuplesMembers(items, tuples, axis, null);
        }

        public static void getTuplesMembers(Items items, Tuple tuples, Axis axis, MeasureElements measureElement)
        {
            foreach (IMember member in tuples.MemberCollection)
            {
                if (member.UniqueName != null)
                {
                    getMembers(items, member, measureElement);
                }
            }
            foreach (IKeyword ikeyword in tuples.IkeyWordCollection)
            {
                if (ikeyword is Filter)
                {
                    getFilter(items, ikeyword, axis, measureElement);
                }
                else if (ikeyword is TopCount)
                {
                    getTopCount(items, ikeyword, axis, measureElement);
                }
                else if (ikeyword is Order)
                {
                    getOrders(items, ikeyword, axis, measureElement);
                }
                else if (ikeyword is Hierarchies)
                {
                    getHierarchies(items, ikeyword, measureElement);
                }
            }
        }

        public static void getFilter(Items items, IKeyword ikeyword, Axis axis)
        {
            getFilter(items, ikeyword, axis, null);
        }

        public static void getFilter(Items items, IKeyword ikeyword, Axis axis, MeasureElements measureElement)
        {
            AxisPosition axPos = new AxisPosition();
            Filter filterNew = (Filter)ikeyword;

            if (axis.Name != null)
            {
                if (axis.Name == KeywordConstants.Columns || axis.Name == KeywordConstants.ColumnsNum)
                {
                    axPos = AxisPosition.Categorical;
                }
                else
                {
                    axPos = AxisPosition.Series;
                }
            }
            else
            {
                axPos = AxisPosition.Slicer;
            }
            string[] tempMes = filterNew.members[0].UniqueName.ToString().ToUpper().Trim().Split(new string[] { "[","]",".","MEASURES" }, StringSplitOptions.RemoveEmptyEntries);
            MeasureElements measureElementColumn = new MeasureElements();
            measureElementColumn.Elements.Add(new MeasureElement { Name = tempMes[0] });

            FilterElement filterElement = new FilterElement(axPos);            
            filterElement.IsFilterCondition = true;
            FilterCase filterCaseVal = (FilterCase)Enum.Parse(typeof(FilterCase), filterNew.filterCases[0].ToString(), true);
            filterElement.FilterCase = filterCaseVal;
            getFilterTupleMembers(filterElement, filterNew.tuple, axis, measureElement);
            filterElement.FilterValue.Add(new MeasureElement { Name = tempMes[0], Visible = true });
            filterElement.FilterValue.Add(new FilterValue { Filter_Value = filterNew.filterValues[0] });

            olapReport.FilterElements.Add(new Item { ElementValue = filterElement });
            items.Add(new Item { ElementValue = measureElementColumn });
            items.IsFilterOrSortOn = true;
            filterElement = null;
        }

        public static void getFilterTupleMembers(FilterElement filterElements, Tuple tuples, Axis axis)
        {
            getFilterTupleMembers(filterElements, tuples, axis, null);
        }

        public static void getFilterTupleMembers(FilterElement filterElements, Tuple tuples, Axis axis, MeasureElements measureElement)
        {
            foreach (IMember member in tuples.MemberCollection)
            {
                if (member.UniqueName != null)
                {
                    getFilterMembers(filterElements, member, measureElement);
                }
            }
            foreach (IKeyword ikeyword in tuples.IkeyWordCollection)
            {
                if (ikeyword is Hierarchies)
                {
                    getFilterHierarchies(filterElements, ikeyword, measureElement);
                }
            }            
        }

        public static void getFilterMembers(FilterElement filterElements, IMember member)
        {
            getFilterMembers(filterElements, member, null);
        }

        public static void getFilterMembers(FilterElement filterElements, IMember member, MeasureElements measureElement)
        {
            if (measureElement == null)
                measureElement = new MeasureElements();
            if (member.UniqueName.ToUpper().Contains("MEASURES"))
            {
                string[] measure = member.UniqueName.Trim().Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                string[] measuresnew = measure[1].Trim().Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                measureElement.Elements.Add(new MeasureElement { Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(measuresnew[0].Trim().ToLower()) });
                filterElements.Elements.Add(measureElement);
            }
            else
            {
                DimensionElement dimensionElement = new DimensionElement();
                string[] dimension = member.UniqueName.Trim().Split(new string[] { ".", "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                dimensionElement.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dimension[0].Trim().ToLower());
                dimensionElement.HierarchyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dimension[1].Trim().ToLower());
                filterElements.Elements.Add(dimensionElement);
            }
        }

        public static void getFilterHierarchies(FilterElement filterElements, IKeyword ikeyword)
        {
            getFilterHierarchies(filterElements, ikeyword, null);
        }

        public static void getFilterHierarchies(FilterElement filterElements, IKeyword ikeyword, MeasureElements measureElement)
        {
            if (ikeyword is Hierarchies)
            {
                Hierarchies hierarchies = (Hierarchies)ikeyword;
                foreach (Hierarchize hierarchize in hierarchies.Members)
                {
                    if (hierarchize.Name == KeywordConstants.Hierarchize)
                    {
                        getFilterMembers(filterElements, hierarchize.DrillDownLevel.MemberName, measureElement);
                    }
                }
            }
        }

        public static void getTopCount(Items items, IKeyword ikeyword, Axis axis)
        {
            getTopCount(items, ikeyword, axis, null);
        }

        public static void getTopCount(Items items, IKeyword ikeyword, Axis axis, MeasureElements measureElement)
        {
            AxisPosition axPos = new AxisPosition();
            TopCount topCountNew = (TopCount)ikeyword;
            if (axis.Name != null)
            {
                if (axis.Name == KeywordConstants.Columns || axis.Name == KeywordConstants.ColumnsNum)
                {
                    axPos = AxisPosition.Categorical;
                }
                else
                {
                    axPos = AxisPosition.Series;
                }
            }
            else
            {
                axPos = AxisPosition.Slicer;
            }
            TopCountElement topCountElement = new TopCountElement(axPos, topCountNew.topCountValue);
            topCountElement.MeasureName = "";
            items.Add(new Item { ElementValue = topCountElement });
            getTuplesMembers(items, topCountNew.tuple, axis, measureElement);
        }

        public static void getOrders(Items items, IKeyword ikeyword, Axis axis)
        {
            getOrders(items, ikeyword, axis, null);
        }

        public static void getOrders(Items items,IKeyword ikeyword, Axis axis, MeasureElements measureElement)
        {
            if(ikeyword is Order)
            {
                AxisPosition axPos = new AxisPosition();
                SortOrder sortOrderval = new SortOrder();
                Order ordernew = (Order)ikeyword;
                if (axis.Name != null)
                {
                    if (axis.Name == KeywordConstants.Columns || axis.Name == KeywordConstants.ColumnsNum)
                    {
                        axPos = AxisPosition.Categorical;
                    }
                    else
                    {
                        axPos = AxisPosition.Series;
                    }
                }
                else
                {
                    axPos = AxisPosition.Slicer;
                }
                if (ordernew.SortingOrder != null)
                {
                    if (ordernew.SortingOrder == SortOrder.ASC.ToString())
                    {
                        sortOrderval = SortOrder.ASC;
                    }
                    if (ordernew.SortingOrder == SortOrder.BASC.ToString())
                    {
                        sortOrderval = SortOrder.BASC;
                    }
                    if (ordernew.SortingOrder == SortOrder.BDESC.ToString())
                    {
                        sortOrderval = SortOrder.BDESC;
                    }
                    if (ordernew.SortingOrder == SortOrder.DESC.ToString())
                    {
                        sortOrderval = SortOrder.DESC;
                    }
                }
                else
                {
                    sortOrderval = SortOrder.ASC;
                }
                SortElement sortElement = new SortElement(axPos, sortOrderval, true);
                sortElement.Element.UniqueName = ordernew.members[0].UniqueName.ToString();
                items.Add(new Item { ElementValue = sortElement });
                getTuplesMembers(items, ordernew.tuple, axis, measureElement);
            }
        }

        public static void getHierarchies(Items items, IKeyword ikeyword)
        {
            getHierarchies(items, ikeyword, null);
        }

        public static void getHierarchies(Items items, IKeyword ikeyword, MeasureElements measureElement)
        {
            //for drill down level
            if (ikeyword is Hierarchies)
            {
                Hierarchies hierarchies = (Hierarchies)ikeyword;
                foreach (Hierarchize hierarchize in hierarchies.Members)
                {
                    if (hierarchize.Name == KeywordConstants.Hierarchize)
                    {
                        getMembers(items, hierarchize.DrillDownLevel.MemberName, measureElement);
                    }
                }
            }
        }

        public static void getMembers(Items items, IMember member)
        {
            getMembers(items, member, null);
        }

        public static void getMembers(Items items, IMember member, MeasureElements measureElement)
        {
            if (measureElement == null)
                measureElement = new MeasureElements();
            if (member.UniqueName.ToUpper().Contains("MEASURES"))
            {
                string[] measure = member.UniqueName.Trim().Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                string[] measuresnew = measure[1].Trim().Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                measureElement.Elements.Add(new MeasureElement { Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(measuresnew[0].Trim().ToLower())});
            }
            else
            {
                DimensionElement dimensionElement = new DimensionElement();
                string[] dimension = member.UniqueName.Trim().Split(new string[] { ".", "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                dimensionElement.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dimension[0].Trim().ToLower());
                dimensionElement.HierarchyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dimension[1].Trim().ToLower());
                items.Add(new Item { ElementValue = dimensionElement });
            }
        }

        /// <summary>
        /// Check the keyword select or with
        /// </summary>
        private static void CheckWithSelect(Tokens tokens)
        {            
            if (tokens.curToken.ToUpper() == KeywordConstants.Select)
            {
                select = new Select();
                SelectQuery(tokens);
            }
            else
            {
                throw new QuerySyntaxError("PARSER ERROR: SELECT Keyword Expected.");
            }
        }

        ///// <summary>
        ///// With query syntax checking
        ///// </summary>
        //private static void WithQuery()
        //{
        //    tokens.CheckToken(KeywordConstants.With);
        //    tokens.CheckToken(KeywordConstants.Members);
        //}

        /// <summary>
        /// Select query Syntax checking
        /// </summary>
        private static void SelectQuery(Tokens tokens)
        {
            tokens.CheckToken(KeywordConstants.Select);

            TupleSets(tokens);

            tokens.CheckToken(KeywordConstants.From);

            if (tokens.curToken == "(")
            {
                SubSelect(tokens);
            }
            else
            {
                //add the cube name in the cubename object
                select.FromClause.CubeName = getCubeName(tokens).Trim();
                tokens.SplitToken();
            }
            if (tokens.curToken.ToUpper() == KeywordConstants.Where)
            {
                WhereQuery(tokens);
            }
            else
            {
                if (!tokens.EndReader && tokens.curToken.ToUpper() == KeywordConstants.Cell)
                {
                    CheckCellProperty(tokens);
                }
            }

        }

        /// <summary>
        /// check if cell property exist
        /// </summary>
        private static void CheckCellProperty(Tokens tokens)
        {
            tokens.SplitToken();
            tokens.CheckToken(KeywordConstants.Properties);
            if (tokens.EndReader)
            {
                throw new QuerySyntaxError("PARSER ERROR: Cell Properties must have a Property.");
            }
            else
            {
                CellsProperties cprop = new CellsProperties();
                string cellprop = string.Empty;
                while (!tokens.EndReader)
                {
                    CellProperties(tokens.curToken, cprop);
                    tokens.SplitToken();
                    if (!tokens.EndReader)
                    {
                        tokens.CheckToken(",");
                    }
                }
            }
        }

        /// <summary>
        /// check the cell properties and add
        /// </summary>
        /// <param name="cellproperty">The cellproperty.</param>
        /// <param name="cellprops">The cellprops.</param>
        public static void CellProperties(string cellproperty, CellsProperties cellprops)
        {
            string[] cellproperties = new string[] { KeywordConstants.Value, KeywordConstants.FormatString, KeywordConstants.FormattedValue };
            string cp = cellproperty.ToUpper().Trim();
            bool cellProp = false;

            foreach (string c in cellproperties)
            {
                if (c == cp)
                {
                    //add the cell property value
                    if (cp == KeywordConstants.Value)
                    {
                        cellprops.Value = KeywordConstants.Value;
                    }
                    if (cp == KeywordConstants.FormatString)
                    {
                        cellprops.FormatString = KeywordConstants.FormatString;
                    }
                    if (cp == KeywordConstants.FormattedValue)
                    {
                        cellprops.FormattedValue = KeywordConstants.FormattedValue;
                    }
                    cellProp = true;
                    break;
                }
            }
            if (cellProp == false)
            {
                throw new QuerySyntaxError("PARSER ERROR: Cell Property must have a valid Property.");
            }
        }

        /// <summary>
        /// check if dimension properties exists
        /// </summary>
        private static void CheckDimesionProperty(Tokens tokens)
        {
            tokens.SplitToken();
            tokens.CheckToken(KeywordConstants.Properties);
            if (tokens.curToken == KeywordConstants.On)
            {
                throw new QuerySyntaxError("PARSER ERROR: Dimension Properties must have a Property.");
            }
            else
            {
                while (tokens.curToken != KeywordConstants.On)
                {
                    DimensionProperties(tokens.curToken);
                    tokens.SplitToken();
                    if (tokens.curToken != KeywordConstants.On)
                    {
                        tokens.CheckToken(",");
                    }
                }
            }
        }

        /// <summary>
        /// check and add the dimension property
        /// </summary>
        /// <param name="Dimensionproperty"></param>
        private static void DimensionProperties(string Dimensionproperty)
        {
            string[] Dimensionproperties = new string[] { KeywordConstants.MemberType, KeywordConstants.ParentUniqueName };
            string dm = Dimensionproperty.ToUpper().Trim();
            bool dimProp = false;
            foreach (string d in Dimensionproperties)
            {
                if (d == dm)
                {
                    //add the dimension property value
                    dimProp = true;
                    break;
                }
            }
            if (dimProp == false)
            {
                throw new QuerySyntaxError("PARSER ERROR: Dimension Property must have a valid Property.");
            }
        }

        /// <summary>
        /// get the cube name and return
        /// </summary>
        /// <returns></returns>
        private static string getCubeName(Tokens tokens)
        {
            string cubeName = string.Empty;
            if (tokens.curToken.Contains("["))
            {
                cubeName += tokens.curToken;
                cubeName += " ";
                while (!tokens.curToken.Contains("]"))
                {
                    tokens.SplitToken();
                    cubeName += tokens.curToken;
                    cubeName += " ";
                    if (tokens.EndReader && (!tokens.curToken.Contains("]")))
                    {
                        throw new QuerySyntaxError("PARSER ERROR: ']' Expected in Cube Name.");
                    }
                }
            }
            else
            {
                throw new QuerySyntaxError("PARSER ERROR: '[' Expected in Cube Name.");
            }
            return cubeName.Trim();
        }

        public static void TupleSets()
        {
            TupleSets(null);
        }

        /// <summary>
        /// get the tuple set on rows and columns
        /// </summary>
        public static void TupleSets(Tokens tokens)
        {
            string fieldColumn = string.Empty;
            string onvalue = string.Empty;
            string oncol = string.Empty;
            string axisval = string.Empty;

            if (tokens != null)
            {
                fieldColumn = getTuple(tokens);

                tokens.SplitToken();
                if ((tokens.curToken.ToUpper().Contains(KeywordConstants.Columns)) || (tokens.curToken.ToUpper().Contains(KeywordConstants.ColumnsNum)))
                {
                    //validate and add the measure and dimension
                    axisval = tokens.curToken.ToUpper() == KeywordConstants.Columns ? KeywordConstants.Columns : KeywordConstants.ColumnsNum;
                    Axis axisColumn = new Axis();
                    axisColumn.Name = axisval.ToString();
                    Validate(fieldColumn, olapReport.CategoricalElements, axisColumn);
                    fieldColumn = string.Empty;

                    tokens.SplitToken();

                    if (tokens.curToken == ",")
                    {
                        tokens.CheckToken(",");
                        if (tokens.curToken.ToUpper() != KeywordConstants.From)
                        {
                            fieldColumn = getTuple(tokens);

                            tokens.SplitToken();
                            if ((tokens.curToken.ToUpper() == KeywordConstants.Rows) || (tokens.curToken.ToUpper() == KeywordConstants.RowsNum))
                            {
                                //validate and add the measure and dimension
                                axisval = tokens.curToken.ToUpper() == KeywordConstants.Rows ? KeywordConstants.Rows : KeywordConstants.RowsNum;
                                Axis axisRow = new Axis();
                                axisRow.Name = axisval.ToString();
                                Validate(fieldColumn, olapReport.SeriesElements, axisRow);
                                fieldColumn = string.Empty;
                                tokens.SplitToken();
                            }
                            else
                            {
                                throw new QuerySyntaxError("PARSER ERROR: Expected Row Axis");
                            }
                        }
                        else
                        {
                            throw new QuerySyntaxError("PARSER ERROR: Expected Tuples in Row Axis");
                        }
                    }

                }
                else
                {
                    if ((tokens.curToken.ToUpper().Contains(KeywordConstants.Rows)) || (tokens.curToken.ToUpper().Contains(KeywordConstants.RowsNum)))
                    {
                        //validate and add the measure and dimension
                        axisval = tokens.curToken.ToUpper() == KeywordConstants.Rows ? KeywordConstants.Rows : KeywordConstants.RowsNum;
                        Axis axisRow = new Axis();
                        axisRow.Name = axisval.ToString();
                        Validate(fieldColumn, olapReport.SeriesElements, axisRow);
                        fieldColumn = string.Empty;

                        tokens.SplitToken();
                        tokens.CheckToken(",");

                        fieldColumn = getTuple(tokens);

                        tokens.SplitToken();
                        if ((tokens.curToken.ToUpper() == KeywordConstants.Columns) || (tokens.curToken.ToUpper() == KeywordConstants.ColumnsNum))
                        {
                            //validate and add the measure and dimension
                            axisval = tokens.curToken.ToUpper() == KeywordConstants.Columns ? KeywordConstants.Columns : KeywordConstants.ColumnsNum;
                            Axis axisColumn = new Axis();
                            axisColumn.Name = axisval.ToString();
                            Validate(fieldColumn, olapReport.CategoricalElements, axisColumn);

                            tokens.SplitToken();
                        }
                        else
                        {
                            throw new QuerySyntaxError("PARSER ERROR: Axis numbers specified in a query must be sequentially specified.");
                        }
                    }
                    else
                    {
                        throw new QuerySyntaxError("PARSER ERROR: Axis is not specified.");
                    }
                }
            }
        }

        /// <summary>
        /// split the TupleSet into tuples
        /// </summary>
        /// <returns></returns>
        private static string getTuple(Tokens tokens)
        {
            string fields = string.Empty;
            if (tokens.curToken.ToUpper() == KeywordConstants.Non)
            {
                tokens.SplitToken();
                tokens.CheckToken(KeywordConstants.Empty);
            }
            fields += tokens.curToken;
            fields += " ";            
            while (tokens.curToken.ToUpper() != KeywordConstants.On)
            {
                tokens.SplitToken();
                if (tokens.curToken.ToUpper() == KeywordConstants.Dimension)
                {
                    CheckDimesionProperty(tokens);
                }
                else
                {
                    if (tokens.curToken.ToUpper() != KeywordConstants.On)
                    {
                        fields += tokens.curToken;
                        fields += " ";
                    }
                }
            }
            return fields.ToString();
        }

        /// <summary>
        /// check the where query syntax
        /// </summary>
        private static void WhereQuery(Tokens tokens)
        {
            string whereField = string.Empty;
            tokens.SplitToken();
            whereField += tokens.curToken;
            whereField += " ";
            while (!tokens.EndReader)
            {
                tokens.SplitToken();
                if (!tokens.EndReader && tokens.curToken.ToUpper() == KeywordConstants.Cell)
                {
                    CheckCellProperty(tokens);
                }
                else
                {
                    whereField += tokens.curToken;
                    whereField += " ";
                }
            }
            Axis slicer = new Axis();
            slicer.Name = null;
            Validate(whereField, olapReport.SlicerElements, slicer);
        }

        /// <summary>
        /// check the parentheses
        /// </summary>
        private static void MatchParentheses(string expression, char left, char right)
        {
            int count = 0;
            int index = 0;
            char lParam = left;
            char rParam = right;
            string emptyError = "PARSER ERROR: Empty between '" + lParam + "' and '" + rParam + "' in the Query.";
            while (index < expression.Length)
            {
                if (expression[index] == lParam)
                {
                    ++count;
                }
                if (expression[index] == rParam)
                {
                    --count;
                }
                index++;
                if (count < 0)
                {
                    throw new QuerySyntaxError("PARSER ERROR: Are you missing '" + lParam + "'. Because extra '" + rParam + "' in the Query.");
                }
            }
            if (count > 0)
            {
                throw new QuerySyntaxError("PARSER ERROR: Are you missing '" + rParam + "'. Because extra '" + lParam + "' in the Query.");
            }
            index = 0;
            while (index < expression.Length)
            {
                if (expression[index] == lParam)
                {
                    if (expression[index + 1] == rParam)
                    {
                        throw new QuerySyntaxError(emptyError);
                    }
                    int i = index + 1;
                    while (expression[i + 1] == ' ')
                    {
                        if (expression[i + 2] == rParam)
                        {
                            throw new QuerySyntaxError(emptyError);
                        }
                        i++;
                    }
                }
                index++;
            }
        }

        /// <summary>
        /// Remove the Un wanted Space in the MDXquery
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static string RemoveExtraSpace(string query)
        {
            string temp = string.Empty;
            string[] sparr = query.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string MDXQuery = string.Empty;
            string newQuery = string.Empty;
            for (int s = 0; s < sparr.Length; s++)
            {
                temp = sparr[s].Trim().ToString();
                if (sparr[s].ToUpper().Trim() == KeywordConstants.From)
                {
                    newQuery += KeywordConstants.From;
                }
                else
                {
                    newQuery += sparr[s].Trim().ToString();
                }
                if ((temp.ToString() != "("))
                {
                    if ((s != sparr.Length - 1))
                    {
                        if ((sparr[s + 1].ToString() != ")"))
                        {
                            newQuery += " ";
                        }
                    }
                }
            }
            string[] sp = newQuery.Split(new string[] { KeywordConstants.From }, StringSplitOptions.None);
            for (int f = 0; f < sp.Length; f++)
            {
                if (f == 0)
                {
                    MDXQuery += sp[f];
                    MDXQuery += " " + KeywordConstants.From + " ";
                }
                else
                {
                    string tempString = sp[f].Replace("(", " ( ");
                    tempString = tempString.Replace(")", " ) ");
                    MDXQuery += tempString;
                    if (f != sp.Length - 1)
                    {
                        MDXQuery += " " + KeywordConstants.From + " ";
                    }
                }
            }
            MDXQuery = MDXQuery.Replace(",", " , ");
            MDXQuery = MDXQuery.Replace("{", " { ");
            MDXQuery = MDXQuery.Replace("}", " } ");
            MDXQuery = MDXQuery.Replace(")", " ) ");
            MDXQuery = MDXQuery.Replace("(", " ( ");
            MDXQuery = MDXQuery.Replace("*", " * ");
            return MDXQuery;
        }

        /// <summary>
        /// Check the Sub Select Query
        /// </summary>
        private static void SubSelect(Tokens tokens)
        {
            subselectoption = true;
            select.SubSelect = new Select();
            tokens.SplitToken();
            tokens.CheckToken(KeywordConstants.Select);

            TupleSets(tokens);

            tokens.CheckToken(KeywordConstants.From);

            //add the cube name in the cube name object
            select.FromClause.CubeName = getCubeName(tokens).Trim();

            tokens.SplitToken();

            tokens.CheckToken(")");
        }

        /// <summary>
        /// validate and add the measure and dimension
        /// </summary>
        /// <param name="equation"></param>
        /// <param name="items"></param>
        /// <param name="ax"></param>
        /// <returns></returns>
        public static bool Validate(string equation, Items items, Axis ax)
        {
            string newEquation = getTupleFormat(equation.Trim());
            Stack stack = new Stack();
            string a = string.Empty;
            string x = string.Empty;
            Tuple tuple = new Tuple();
            Filter filter = new Filter();
            Order order = new Order();
            TopCount topCount =  new TopCount();
            Hierarchize hierarchizeelement = null;
            DrillDownLevel drilldownlevelelement = null;
            DrillDownMember parentDrillDownMember = new DrillDownMember();
            DrillDownMember currentDrillDownMember = null;
            DrillDownMember prevDrillDownMember = null;            
            Hierarchies hierarchies = new Hierarchies();            
            tuple.IkeyWordCollection = new IkeywordCollection();
            tuple.MemberCollection = new MemberCollection();
            bool orderval = false;
            bool filterval = false;
            bool topcountval = false;
            string temp = string.Empty;            
            
            for (int i = 0; i < newEquation.Length; i++)
            {
                char current = newEquation[i];
                string str = current.ToString();
                switch (str)
                {
                    case "(":
                    case "{":
                        if (temp != string.Empty)
                        {
                            stack.Push(temp.Trim());
                            if(temp.ToUpper().Trim().Contains(KeywordConstants.Order))
                            {
                                orderval = true;
                            }
                            if (temp.ToUpper().Trim().Contains(KeywordConstants.Filter))
                            {
                                filterval = true;
                            }
                            if (temp.ToUpper().Trim().Contains(KeywordConstants.TopCount))
                            {
                                topcountval = true;
                            }
                        }
                        stack.Push(str);
                        temp = string.Empty;
                        break;

                    case "*":
                    case ",":
                        if (temp != string.Empty && temp.Contains("[") && temp.Contains(".") && temp.Contains("]"))
                        {
                            if (!orderval && !filterval)
                            {
                                if (temp.ToUpper().Trim().Contains("MEASURES"))
                                {
                                    Measure measures = new Measure();
                                    measures.UniqueName = temp.ToUpper().Trim();
                                    tuple.MemberCollection.Add(measures);
                                    temp = string.Empty;
                                }
                                else
                                {
                                    Dimension dimensions = new Dimension();
                                    dimensions.UniqueName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(temp.Trim().ToLower());
                                    tuple.MemberCollection.Add(dimensions);
                                    temp = string.Empty;
                                }
                            }
                            else
                            {
                                temp += str.ToString();
                            }
                        }
                        break;

                    case ")":
                    case "}":
                        {
                            a = (string)stack.Pop();
                            if (!stack.IsEmpty() && (str == ")"))
                            {
                                x = (string)stack.Pop();
                                if (stack.IsEmpty() && x == "(")
                                {
                                    stack.Push(x);
                                }
                                else
                                { 
                                    if (x == "(" && a == "(")
                                    {   
                                        stack.Push(x);
                                        break;
                                    }
                                }
                            }
                            if (x != string.Empty || temp != string.Empty)
                            {

                                if (x.ToUpper() == KeywordConstants.Hierarchize)
                                {
                                    hierarchizeelement = new Hierarchize();
                                    hierarchizeelement.Name = KeywordConstants.Hierarchize;
                                    if (currentDrillDownMember != null)
                                    {
                                        hierarchizeelement.DrillDownMember = currentDrillDownMember;
                                    }
                                    else if (drilldownlevelelement != null)
                                    {
                                        hierarchizeelement.DrillDownLevel = drilldownlevelelement;
                                    }
                                    if (!hierarchizeelement.Validate())
                                    {
                                        throw new QuerySyntaxError("Error in Hierarchize");
                                    }
                                    hierarchizeelement.DrillDownLevel = drilldownlevelelement;
                                    hierarchizeelement.DrillDownMember = parentDrillDownMember;
                                    hierarchies.Members.Add(hierarchizeelement);
                                    x = string.Empty;
                                }
                                if (x.ToUpper() == KeywordConstants.Filter)
                                {
                                    filter.Name = x.ToUpper().ToUpper().Trim();
                                    filter.tuple = new Tuple();
                                    filter.tuple.IkeyWordCollection = new IkeywordCollection();
                                    filter.tuple.MemberCollection = new MemberCollection();
                                    filter.filterCases = new List<ParseFilterCase>();
                                    filter.filterValues = new List<double>();
                                    filter.members = new MemberCollection();
                                    string[] filOper = { ">=","<=","!=",">","<","=" };

                                    for (int filVal = 0; filVal < filOper.Length; filVal++)
                                    {
                                        string[] filterMemVal = temp.Split(new string[] { filOper[filVal] }, StringSplitOptions.RemoveEmptyEntries);
                                        if (filterMemVal.Length >= 2)
                                        {
                                            ParseFilterCase filCase = new ParseFilterCase();
                                            filCase = (ParseFilterCase)filVal;
                                            filter.filterCases.Add(filCase);
                                            for (int filMem = 0; filMem < filterMemVal.Length;filMem++ )
                                            {
                                                if (filterMemVal[filMem].ToUpper().Contains(KeywordConstants.Measures))
                                                {
                                                    Measure measures = new Measure();
                                                    measures.UniqueName = filterMemVal[filMem].ToUpper().Trim();
                                                    filter.members.Add(measures);
                                                }
                                                else
                                                {
                                                    filter.filterValues.Add(Convert.ToDouble(filterMemVal[filMem]));
                                                }
                                            }
                                            break;
                                        }
                                    }                                    
                                    tuple.IkeyWordCollection.Add(hierarchies);
                                    for (int ordm = 0; ordm < tuple.MemberCollection.Count; ordm++)
                                    {
                                        filter.tuple.MemberCollection.Add(tuple.MemberCollection[ordm]);
                                    }
                                    for (int ordk = 0; ordk < tuple.IkeyWordCollection.Count; ordk++)
                                    {
                                        filter.tuple.IkeyWordCollection.Add(tuple.IkeyWordCollection[ordk]);
                                    }
                                    tuple.IkeyWordCollection.Clear();
                                    tuple.MemberCollection.Clear();
                                    tuple.IkeyWordCollection.Add(filter);
                                    if (!filter.Validate())
                                    {
                                        throw new QuerySyntaxError("Error in Filter");
                                    }
                                    x = string.Empty;
                                    temp = string.Empty;
                                }
                                if (temp != string.Empty)
                                {
                                    string[] temparr = temp.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    for (int ms = 0; ms < temparr.Length; ms++)
                                    {
                                        if (temparr[ms].ToUpper().Trim().Contains("MEASURES"))
                                        {
                                            if (!orderval && !filterval)
                                            {
                                                Measure measures = new Measure();
                                                measures.UniqueName = temparr[ms].ToUpper().Trim();
                                                tuple.MemberCollection.Add(measures);
                                                temp = string.Empty;
                                            }
                                        }
                                        else
                                        {
                                            if (x.ToUpper() == KeywordConstants.DrillDownLevel)
                                            {
                                                drilldownlevelelement = new DrillDownLevel();
                                                drilldownlevelelement.Name = KeywordConstants.DrillDownLevel;
                                                Dimension dimension = new Dimension();
                                                dimension.UniqueName = temparr[ms].Trim();
                                                drilldownlevelelement.MemberName = dimension;
                                                if (!drilldownlevelelement.Validate())
                                                {
                                                    throw new QuerySyntaxError("Error in DrillDownLevel");
                                                }
                                                x = string.Empty;
                                            }
                                            else if (x.ToUpper() == KeywordConstants.DrillDownMember)
                                            {
                                                currentDrillDownMember = new DrillDownMember();
                                                Dimension dimension = new Dimension();
                                                currentDrillDownMember.Name = KeywordConstants.DrillDownMember;
                                                dimension.UniqueName = temparr[ms].Trim().ToString();
                                                currentDrillDownMember.MemberName = dimension;
                                                if (prevDrillDownMember == null)
                                                {
                                                    parentDrillDownMember.ChildDrillDownMember = currentDrillDownMember;
                                                    parentDrillDownMember.DrillDownLevel = drilldownlevelelement;
                                                }
                                                else
                                                {
                                                    prevDrillDownMember.ChildDrillDownMember = currentDrillDownMember;
                                                }
                                                prevDrillDownMember = currentDrillDownMember;
                                                if (!currentDrillDownMember.Validate())
                                                {
                                                    throw new QuerySyntaxError("Error in DrillDownMember");
                                                }
                                                x = string.Empty;
                                            }
                                            else
                                            {                                                
                                                if (x.ToUpper() == KeywordConstants.TopCount)
                                                {
                                                    topCount.Name = x.ToUpper().ToUpper().Trim();
                                                    topCount.tuple = new Tuple();
                                                    topCount.tuple.IkeyWordCollection = new IkeywordCollection();
                                                    topCount.tuple.MemberCollection = new MemberCollection();                                                    
                                                    topCount.topCountValue = Convert.ToInt32(temparr[0].ToUpper().Trim());
                                                    tuple.IkeyWordCollection.Add(hierarchies);
                                                    for (int ordm = 0; ordm < tuple.MemberCollection.Count; ordm++)
                                                    {
                                                        topCount.tuple.MemberCollection.Add(tuple.MemberCollection[ordm]);
                                                    }
                                                    for (int ordk = 0; ordk < tuple.IkeyWordCollection.Count; ordk++)
                                                    {
                                                        topCount.tuple.IkeyWordCollection.Add(tuple.IkeyWordCollection[ordk]);
                                                    }
                                                    tuple.IkeyWordCollection.Clear();
                                                    tuple.MemberCollection.Clear();
                                                    tuple.IkeyWordCollection.Add(topCount);  
                                                    if (!topCount.Validate())
                                                    {
                                                        throw new QuerySyntaxError("Error in Top Count");
                                                    }
                                                    x = string.Empty;
                                                    temp = string.Empty;
                                                }
                                                if (x.ToUpper() == KeywordConstants.Order)
                                                {                                                    
                                                    order.Name = x.ToUpper().ToUpper().Trim();
                                                    order.tuple = new Tuple();
                                                    order.tuple.IkeyWordCollection = new IkeywordCollection();
                                                    order.tuple.MemberCollection = new MemberCollection();
                                                    order.members = new MemberCollection();
                                                    for (int ordmem = 0; ordmem < temparr.Length; ordmem++ )
                                                    {
                                                        if (temparr[ordmem].ToUpper().Contains(KeywordConstants.Measures) && order.members.Count<=0)
                                                        {
                                                          string[] orderMeasureName = temparr[ordmem].Split(new string[] { "*" }, StringSplitOptions.RemoveEmptyEntries);
                                                          for (int oms = 0; oms < orderMeasureName.Length; oms++ )
                                                          {
                                                              Measure measures = new Measure();
                                                              measures.UniqueName = orderMeasureName[oms].ToUpper().Trim();
                                                              order.members.Add(measures);
                                                          }
                                                        }
                                                        else
                                                        {
                                                            order.SortingOrder = temparr[ordmem].Trim().ToString();
                                                        }
                                                    }

                                                    tuple.IkeyWordCollection.Add(hierarchies);                                                    
                                                    for (int ordm = 0; ordm < tuple.MemberCollection.Count; ordm++)
                                                    {
                                                        order.tuple.MemberCollection.Add(tuple.MemberCollection[ordm]); 
                                                    }
                                                    for (int ordk = 0; ordk < tuple.IkeyWordCollection.Count; ordk++)
                                                    {
                                                        order.tuple.IkeyWordCollection.Add(tuple.IkeyWordCollection[ordk]);
                                                    }                                                    
                                                    tuple.IkeyWordCollection.Clear();
                                                    tuple.MemberCollection.Clear();
                                                    tuple.IkeyWordCollection.Add(order);                                                    
                                                    if (!order.Validate())
                                                    {
                                                        throw new QuerySyntaxError("Error in Order");
                                                    }
                                                    x = string.Empty;
                                                    temp = string.Empty;
                                                }
                                                if (temparr[ms] != string.Empty && temparr[ms].Contains("[") && temparr[ms].Contains("."))
                                                {
                                                    if (current == '}')
                                                    { 
                                                        tempchar=' ';
                                                        if (newEquation.Length != i + 1)
                                                        {
                                                            tempchar = newEquation[i + 1];
                                                        }
                                                        if (tempchar != ')')
                                                        {
                                                            Dimension dimensions = new Dimension();
                                                            dimensions.UniqueName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(temparr[ms].Trim().ToLower());
                                                            tuple.MemberCollection.Add(dimensions);
                                                            temp = string.Empty;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if ((a == "(" && str != ")") || (a == "{" && str != "}"))
                            {
                                throw new QuerySyntaxError("PARSER ERROR: for '" + str + "'");
                            }
                            else if (a != "{" && (!temp.ToUpper().Contains("MEASURES")))
                            {
                                temp = string.Empty;
                            }
                            break;
                        }
                    default:
                        if (str.Trim() != string.Empty && str.Trim() != "*" && str.Trim() != ",")
                        {
                            temp += str.ToString();
                        }
                        if (temp.Contains("[") || temp.Contains("]") || IsNumeric(temp.Trim()))
                        {
                            if (str == " " || str == ",")
                            {
                                temp += str.ToString();
                            }
                        }
                        break;
                }
            }
            if(order.Name==null && filter.Name==null && topCount.Name==null)
            {
                tuple.IkeyWordCollection.Add(hierarchies);
            }

            if (stack.IsEmpty())
            {
                //new
                if (!tuple.Validate())
                {
                    throw new QuerySyntaxError("Invalid Tuple");
                }
                if (ax.Name != null)
                {                    
                    ax.TupleSet.Add(tuple);                 
                    if (!subselectoption)
                    {
                        select.Axes.Add(ax);
                    }
                    else
                    {
                        select.SubSelect.Axes.Add(ax);
                    }
                }
                else
                {
                    select.Wheres.Tuples.Add(tuple);
                }

                return true;
            }

            return false;
        }

        public static string getTupleFormat(string query)
        {
            string formattedQuery = string.Empty;
            if (!query.Contains('(') && !query.Contains('{') && !query.Contains(')') && !query.Contains('}'))
            {
                if (query.Contains(','))
                {
                    string[] squery = query.Trim().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    int len = squery.Length;
                    for (int str = 0; str < squery.Length; str++)
                    {
                        formattedQuery += "{";
                        formattedQuery += squery[str].Trim();
                        formattedQuery += "}";
                        if (str != squery.Length - 1)
                        {
                            formattedQuery += ",";
                        }
                    }
                }
                else
                {
                    if (query.Contains('.'))
                    {
                        formattedQuery += "{";
                        formattedQuery += query.Trim();
                        formattedQuery += "}";
                    }
                    else
                    {
                        throw new QuerySyntaxError("PARSER ERROR: '.' is expected.");
                    }
                }
            }
            else
            {
                formattedQuery = query;
            }
            return formattedQuery;
        }
        #endregion        
        public static bool IsNumeric(string strToCheck)
        {
            return Regex.IsMatch(strToCheck, "^\\d+(\\.\\d+)?$");
        }
    }
}
