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
#if SILVERLIGHT
using Syncfusion.OlapSilverlight.Reports;
#else
using Syncfusion.Olap.Reports;
#endif
using System.Text.RegularExpressions;

#if SILVERLIGHT
namespace Syncfusion.OlapSilverlight.MDXQueryParser
#else
namespace Syncfusion.Olap.MDXQueryParser
#endif
{
    public class MDXToOlapParser
    {
        static OlapReport olapReport = null;
        static Dictionary<string, string> calculatedMembers = new Dictionary<string, string>();

        public static OlapReport GenerateOlapReport(string mdxQuery)
        {
            olapReport = new OlapReport();

            //Clears the calculated members
            calculatedMembers.Clear();

            //Check for empty MDX, if MDX is empty then it returns empty OLAP report
            if (string.IsNullOrEmpty(mdxQuery))
                return olapReport;

            mdxQuery = mdxQuery.Replace("\r\n", " ").ToUpper();

            ///Match the Parentheses (),{},[]
            MatchParentheses(mdxQuery, '(', ')');
            MatchParentheses(mdxQuery, '{', '}');
            MatchParentheses(mdxQuery, '[', ']');

            ///Block I seperates the WITH query if found
            string withQuery = string.Empty;

            int withIndex = mdxQuery.IndexOf(KeywordConstants.WITH);
            int selectIndex = mdxQuery.IndexOf(KeywordConstants.SELECT);

            if (withIndex > -1 && selectIndex > -1)
            {
                withQuery = mdxQuery.Substring(withIndex, selectIndex - withIndex);
                ProcessWithClause(withQuery);
            }

            ///Block II seperated the SELECT query and process it
            string selectQuery = string.Empty;
            int fromIndex = mdxQuery.IndexOf(KeywordConstants.FROM);
            if (selectIndex > -1 && fromIndex > -1)
            {
                selectQuery = mdxQuery.Substring(selectIndex, fromIndex - selectIndex);
                ProcessSelectClause(selectQuery);
            }

            ///Block III handles FROM, WHERE queries and SUB SELECT query
            string fromQuery = string.Empty;
            string whereQuery = string.Empty;
            string cellProperties = string.Empty;
            int cellPropertiesIndex = -1;
            string subSelectQuery = string.Empty;
            int whereIndex = mdxQuery.IndexOf(KeywordConstants.WHERE);
            if (fromIndex > -1)
            {

                if (!(whereIndex > -1))
                {
                    fromQuery = mdxQuery.Substring(fromIndex);
                    cellPropertiesIndex = fromQuery.IndexOf(KeywordConstants.CELL_PROPERTIES);
                    if (cellPropertiesIndex > -1)
                    {
                        cellProperties = fromQuery.Substring(cellPropertiesIndex);

                        int subSelectIndex = fromQuery.IndexOf(KeywordConstants.SELECT);
                        if (subSelectIndex > -1)
                        {
                            subSelectQuery = fromQuery.Substring(subSelectIndex, cellPropertiesIndex - subSelectIndex).TrimEnd(new char[] { ')', ' ' });
                            string cubeName = subSelectQuery.Substring(subSelectQuery.IndexOf(KeywordConstants.FROM));
                            olapReport.CurrentCubeName = cubeName.Replace(KeywordConstants.FROM, string.Empty).Trim().Trim(new char[] { '[', ']' });
                            ProcessSubSelect(subSelectQuery);
                        }
                        else
                        {
                            string cubeName = fromQuery.Substring(0, cellPropertiesIndex).Split(new string[] { KeywordConstants.FROM }, StringSplitOptions.RemoveEmptyEntries)[0];
                            olapReport.CurrentCubeName = cubeName.Trim().Trim(new char[] { '[', ']' });
                        }
                    }
                    else
                    {
                        int subSelectIndex = fromQuery.IndexOf(KeywordConstants.SELECT);
                        if (subSelectIndex > -1)
                        {
                            subSelectQuery = fromQuery.Substring(subSelectIndex).TrimEnd(new char[] { ')', ' ' });
                            string cubeName = subSelectQuery.Substring(subSelectQuery.IndexOf(KeywordConstants.FROM));
                            olapReport.CurrentCubeName = cubeName.Replace(KeywordConstants.FROM, string.Empty).Trim().Trim(new char[] { '[', ']' });
                            ProcessSubSelect(subSelectQuery);
                        }
                        else
                        {
                            string cubeName = fromQuery.Split(new string[] { KeywordConstants.FROM }, StringSplitOptions.RemoveEmptyEntries)[0];
                            olapReport.CurrentCubeName = cubeName.Trim().Trim(new char[] { '[', ']' });
                        }
                    }
                }
                else
                {
                    fromQuery = mdxQuery.Substring(fromIndex, whereIndex - fromIndex);
                    whereQuery = mdxQuery.Substring(whereIndex);
                    cellPropertiesIndex = whereQuery.IndexOf(KeywordConstants.CELL_PROPERTIES);
                    if (cellPropertiesIndex > -1)
                    {
                        cellProperties = whereQuery.Substring(cellPropertiesIndex);
                        whereQuery = whereQuery.Substring(0, cellPropertiesIndex);
                        string cubeName = fromQuery.Split(new string[] { KeywordConstants.FROM }, StringSplitOptions.RemoveEmptyEntries)[0];
                        olapReport.CurrentCubeName = cubeName.Trim().Trim(new char[] { '[', ']' });
                    }
                    else
                    {
                        string cubeName = fromQuery.Substring(0).Split(new string[] { KeywordConstants.FROM }, StringSplitOptions.RemoveEmptyEntries)[0];
                        olapReport.CurrentCubeName = cubeName.Trim().Trim(new char[] { '[', ']' });
                    }
                    ProcessWhereClause(whereQuery);
                }
            }

            return olapReport;
        }

        /// <summary>
        /// Process the WITH clause with MEMBER function
        /// </summary>
        /// <param name="withQuery">string</param>
        static void ProcessWithClause(string withQuery)
        {
            withQuery = withQuery.Trim().Remove(0, KeywordConstants.WITH.Count()).Trim();
            string[] members = withQuery.Split(new string[] { KeywordConstants.MEMBER }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in members)
            {
                string key = item.Substring(0, item.IndexOf(" AS ")).Trim();
                string value = item.Substring(item.IndexOf(" AS ")).Remove(0, 3).Trim('\'', ' ');
                calculatedMembers.Add(key, value);
            }
        }

        /// <summary>
        ///Process the SELECT query and seperate it into COLUMN query and ROW query
        /// </summary>
        /// <param name="selectQuery">string</param>
        static void ProcessSelectClause(string selectQuery)
        {
            string removeSelect = selectQuery.Replace(KeywordConstants.SELECT, string.Empty).Trim();

            if (removeSelect.IndexOf(KeywordConstants.COLUMNS) < removeSelect.IndexOf(KeywordConstants.ROWS) || removeSelect.IndexOf(KeywordConstants.ROWS) == -1)
            {
                ///Processing Axis0 or COLUMN //TODO: Need to work for Keywords (Axis(0) and 0) instead of COLUMNS Keyword
                int axis0Index = (removeSelect.IndexOf(KeywordConstants.COLUMNS) > -1) ? removeSelect.IndexOf(KeywordConstants.COLUMNS) : removeSelect.LastIndexOf('0');
                string columnQuery = removeSelect.Substring(0, axis0Index).Trim(',', ' ');
                columnQuery = columnQuery.TrimEnd(new char[] { 'O', 'N' }).Trim();
                if (columnQuery != string.Empty)
                    ProcessAxisQuery(columnQuery, AxisPosition.Categorical);

                ///Processing Axis1 or ROW
                string rowQuery = removeSelect.Substring(axis0Index).Replace(KeywordConstants.COLUMNS, string.Empty).Replace(KeywordConstants.ROWS, string.Empty).Trim(',', ' ');
                rowQuery = rowQuery.TrimEnd(new char[] { 'O', 'N' }).Trim();
                if (rowQuery != string.Empty)
                    ProcessAxisQuery(rowQuery, AxisPosition.Series);
            }
            else if (removeSelect.IndexOf(KeywordConstants.ROWS) != -1 && (removeSelect.IndexOf(KeywordConstants.ROWS) < removeSelect.IndexOf(KeywordConstants.COLUMNS)))
            {
                int axis1Index = (removeSelect.IndexOf(KeywordConstants.ROWS) > -1) ? removeSelect.IndexOf(KeywordConstants.ROWS) : removeSelect.LastIndexOf('0');
                string rowQuery = removeSelect.Substring(0, axis1Index).Trim(',', ' ');
                rowQuery = rowQuery.TrimEnd('O', 'N').Trim();
                if (rowQuery != string.Empty)
                    ProcessAxisQuery(rowQuery, AxisPosition.Series);

                string columnQuery = removeSelect.Substring(axis1Index).Replace(KeywordConstants.ROWS, string.Empty).Replace(KeywordConstants.COLUMNS, string.Empty).Trim(',', ' ');
                columnQuery = columnQuery.TrimEnd('O', 'N').Trim();
                if (columnQuery != string.Empty)
                    ProcessAxisQuery(columnQuery, AxisPosition.Categorical);
            }

        }

        /// <summary>
        /// Method that process WHERE clause
        /// </summary>
        /// <param name="whereQuery">string</param>
        static void ProcessWhereClause(string whereQuery)
        {
            //Set UseWhereClauseForSlicing to true in order to use WHERE clause.
            olapReport.UseWhereClauseForSlicing = true;

            string slicerQuery = whereQuery.Replace(KeywordConstants.WHERE, "").Trim();
            slicerQuery = RemoveMatchedBraces(slicerQuery, '(', ')');
            slicerQuery = RemoveMatchedBraces(slicerQuery, '{', '}');
            slicerQuery = RemoveMatchedBraces(slicerQuery, '(', ')');
            slicerQuery = RemoveMatchedBraces(slicerQuery, '{', '}');
            string[] crossElements = null;
            if (slicerQuery.Contains(KeywordConstants.CROSSJOIN))
            {
                crossElements = ExtractElementFromCrossJoinFunction(slicerQuery).ToArray();
            }
            else if (slicerQuery.Contains(KeywordConstants.ASTERISK))
            {
                crossElements = ExtractElementFromCrossJoin(slicerQuery);
            }

            if (crossElements != null)
            {
                foreach (var item in crossElements)
                {
                    ProcessElements(AxisPosition.Slicer, item.Trim('{', '}', ' '));
                }
            }
            else
            {
                if (slicerQuery.Contains(KeywordConstants.RANGE_IDENTIFIER))
                {
                    string[] rangeElements = slicerQuery.Split(KeywordConstants.RANGE_IDENTIFIER);
                    olapReport.SlicerRangeFilters.Add(new SlicerRangeFiltersInfo(rangeElements[0].Trim(), rangeElements[1].Trim()));
                }
                else
                {
                    ProcessElements(AxisPosition.Slicer, slicerQuery.Trim('{', '}'));
                }
            }
        }

        /// <summary>
        /// Process SUB SELECT clause
        /// </summary>
        /// <param name="subSelectQuery">string</param>
        static void ProcessSubSelect(string subSelectQuery)
        {
            //Set UseWhereClauseForSlicing to false here inorder to use SUB SELECT clause.
            olapReport.UseWhereClauseForSlicing = false;

            string subSelect = subSelectQuery.Replace(KeywordConstants.SELECT, string.Empty).Trim();

            int axis0Index = subSelect.IndexOf(KeywordConstants.COLUMNS) > -1 ? subSelect.IndexOf(KeywordConstants.COLUMNS) : subSelect.LastIndexOf('0');
            string slicerQuery = subSelect.Substring(0, axis0Index).Trim(new char[] { ',', ' ' });
            slicerQuery = slicerQuery.TrimEnd(new char[] { 'O', 'N' }).Trim();
            slicerQuery = RemoveMatchedBraces(slicerQuery, '(', ')');
            slicerQuery = RemoveMatchedBraces(slicerQuery, '{', '}');
            if (slicerQuery.Contains(KeywordConstants.RANGE_IDENTIFIER))
            {
                slicerQuery = RemoveMatchedBraces(slicerQuery, '(', ')');
                string[] rangeElement = slicerQuery.Split(KeywordConstants.RANGE_IDENTIFIER);
                olapReport.SlicerRangeFilters.Add(new SlicerRangeFiltersInfo(rangeElement[0].Trim(), rangeElement[1].Trim()));
            }
            else
            {
                AddElementToSlicer(slicerQuery, AxisPosition.Categorical);
            }

            string rowQuery = subSelect.Substring(axis0Index).Replace(KeywordConstants.COLUMNS, string.Empty).Replace(KeywordConstants.ROWS, string.Empty).Trim(new char[] { ',', ' ' });
            rowQuery = rowQuery.Remove(rowQuery.IndexOf(KeywordConstants.FROM));
            if (rowQuery != string.Empty)
            {
                rowQuery = rowQuery.TrimEnd(new char[] { 'O', 'N', ' ' }).Trim();
                rowQuery = RemoveMatchedBraces(rowQuery, '(', ')');
                rowQuery = RemoveMatchedBraces(rowQuery, '{', '}');
                if (rowQuery.Contains(KeywordConstants.RANGE_IDENTIFIER))
                {
                    rowQuery = RemoveMatchedBraces(slicerQuery, '(', ')');
                    string[] rangeElement = rowQuery.Split(KeywordConstants.RANGE_IDENTIFIER);
                    olapReport.SlicerRangeFilters.Add(new SlicerRangeFiltersInfo(rangeElement[0].Trim(), rangeElement[1].Trim()));
                }
                else
                {
                    AddElementToSlicer(rowQuery, AxisPosition.Series);
                }
            }
        }

        /// <summary>
        /// Method extracts dimension element and measure element from query and add the same into slicer axis
        /// </summary>
        /// <param name="slicerQuery">string</param>
        private static void AddElementToSlicer(string slicerQuery, AxisPosition axisPostion)
        {
            if (slicerQuery.Contains(KeywordConstants.MEASURES))
            {
                MeasureElements measureElements = GetMeasureElements(slicerQuery);
                olapReport.SlicerElements.Add(measureElements);
                //olapReport.SlicerElements.Add(new Item { Axis = axisPostion, ElementValue = measureElements });
            }
            else
            {
                DimensionElement dimensionElement = GetDimensionElement(slicerQuery);
                olapReport.SlicerElements.Add(dimensionElement);
                //adds the dimension element in slicer axis
                // olapReport.SlicerElements.Add(new Item { Axis = axisPostion, ElementValue = dimensionElement });
            }
        }

        /// <summary>
        /// Process the axis query
        /// </summary>
        /// <param name="axisQuery">string</param>
        /// <param name="axisPosition">AxisPostion</param>
        static void ProcessAxisQuery(string axisQuery, AxisPosition axisPosition)
        {
            string tempQuery = axisQuery;
            string dimensionProperties = string.Empty;
            string rejoinedQuery = string.Empty;

            ///Processing Dimension property
            int dimensionPropertyIndex = axisQuery.IndexOf(KeywordConstants.DIMENSION_PROPERTIES);
            if (dimensionPropertyIndex > -1)
            {
                dimensionProperties = axisQuery.Substring(dimensionPropertyIndex);
                tempQuery = axisQuery.Substring(0, dimensionPropertyIndex);
            }

            if (tempQuery.Contains("(") && tempQuery.Contains(")") || tempQuery.Contains(KeywordConstants.NON_EMPTY))
            {
                tempQuery = RemoveMatchedBraces(tempQuery, '(', ')').Trim();

                //Process NONEMPTY function and NON, EMPTY keyword of MDX
                rejoinedQuery = (tempQuery.StartsWith(KeywordConstants.NONEMPTY) || tempQuery.StartsWith(KeywordConstants.NON_EMPTY)) ? ProcessNonEmpty(tempQuery) : tempQuery;

                if (rejoinedQuery.Contains(KeywordConstants.ASTERISK) || rejoinedQuery.StartsWith(KeywordConstants.CROSSJOIN))
                {
                    string[] crossJoinElements = ProcessCrossJoin(rejoinedQuery);
                    foreach (var item in crossJoinElements)
                    {
                        ProcessFunctions(axisPosition, item);
                    }
                }
                else
                {
                    ProcessFunctions(axisPosition, rejoinedQuery);
                }
            }
            else if (tempQuery.Contains(KeywordConstants.ASTERISK))
            {
                string[] crossJoinElements = ProcessCrossJoin(tempQuery);
                foreach (var item in crossJoinElements)
                {
                    ProcessFunctions(axisPosition, item);
                }
            }
            else
            {
                string elementQuery = tempQuery;
                elementQuery = RemoveMatchedBraces(elementQuery, '{', '}');
                ProcessElements(axisPosition, elementQuery);
            }
        }

        /// <summary>
        /// Process the special function and keywords of MDX
        /// </summary>
        /// <param name="axisPosition"></param>
        /// <param name="item"></param>
        private static void ProcessFunctions(AxisPosition axisPosition, string item)
        {
            string elementQuery = item.Trim('{', '}', ' ');
            if (elementQuery.StartsWith(("(")))
                elementQuery = RemoveMatchedBraces(elementQuery, '(', ')');

            if (elementQuery.StartsWith(KeywordConstants.VISUALTOTALS))
                elementQuery = ProcessVisualTotals(elementQuery);

            if (elementQuery.StartsWith(KeywordConstants.HIERARCHIZE))
                elementQuery = ProcessHierarchize(elementQuery);

            if (elementQuery.StartsWith(KeywordConstants.DRILLDOWNLEVEL))
                elementQuery = ProcessDrillDownLevel(elementQuery);

            if (elementQuery.StartsWith(KeywordConstants.DRILLDOWNMEMBER))
                elementQuery = ProcessDrillDownMember(elementQuery);

            if (elementQuery.StartsWith(KeywordConstants.TOPCOUNT))
            {
                ProcessTopCount(elementQuery, axisPosition);
                elementQuery = string.Empty;
            }
            if (elementQuery.StartsWith(KeywordConstants.EXCEPT))
            {
                ProcessExcept(elementQuery, axisPosition);
                elementQuery = string.Empty;
            }
            if (elementQuery.StartsWith(KeywordConstants.ORDER))
            {
                ProcessOrder(elementQuery, axisPosition);
                elementQuery = string.Empty;
            }
            if (elementQuery.StartsWith(KeywordConstants.FILTER))
            {
                ProcessFilter(elementQuery, axisPosition);
                elementQuery = string.Empty;
            }
            if (elementQuery.StartsWith(KeywordConstants.SUBSET))
            {
                ProcessSubSetElement(elementQuery, axisPosition);
                elementQuery = string.Empty;
            }

            ///Keyword INTERSECT,UNION, DESCENDANTS not having support in OlapReport.
            if (elementQuery.Contains(KeywordConstants.INTERSECT) || elementQuery.Contains(KeywordConstants.UNION) || elementQuery.Contains(KeywordConstants.DESCENDANTS))
                throw new Exception();

            if (elementQuery != string.Empty)
            {
                if (elementQuery.Contains('}') && elementQuery.Contains('}') && elementQuery.Contains(','))
                {
                    elementQuery = "{" + elementQuery + "}";
                }

                if (elementQuery.StartsWith("[") && elementQuery.EndsWith("]"))
                {
                    ProcessElements(axisPosition, elementQuery);
                }
                else
                {
                    foreach (var element in GetTuple(elementQuery))
                    {
                        ProcessElements(axisPosition, element);
                    }
                }
            }
        }

        private static List<string> GetTuple(string query)
        {
            List<string> tuples = new List<string>();
            int lbrace = 0, rbrace = 0;
            string tuple = string.Empty;
            foreach (var item in query)
            {
                tuple += item.ToString();
                if (item == KeywordConstants.LBRACE)
                    lbrace++;
                if (item == KeywordConstants.RBRACE)
                    rbrace++;
                if (item == KeywordConstants.COMMA && lbrace != 0 && lbrace == rbrace)
                {
                    tuples.Add(tuple.Trim(','));
                    tuple = string.Empty;
                }
            }
            if (tuple != string.Empty)
                tuples.Add(tuple);
            return tuples;
        }

        /// <summary>
        /// Process the NONEMPTY function and NON EMPTY keyword of MDX
        /// </summary>
        /// <param name="tempQuery">string</param>
        /// <returns>string</returns>
        private static string ProcessNonEmpty(string tempQuery)
        {
            List<string> nonEmptyParams = null;
            if (tempQuery.Trim().StartsWith(KeywordConstants.NONEMPTY))
                nonEmptyParams = ParamSeperator(tempQuery, KeywordConstants.NONEMPTY);
            else if (tempQuery.Trim().StartsWith(KeywordConstants.NON_EMPTY))
                nonEmptyParams = ParamSeperator(tempQuery, KeywordConstants.NON_EMPTY);
            else if (tempQuery.Trim().StartsWith(KeywordConstants.NONEMPTYCROSSJOIN))
                nonEmptyParams = ParamSeperator(tempQuery, KeywordConstants.NONEMPTYCROSSJOIN);
            return string.IsNullOrEmpty(nonEmptyParams[0]) ? string.Empty : nonEmptyParams[0];
        }

        /// <summary>
        /// Process the SUBSET element of MDX
        /// </summary>
        /// <param name="query">string</param>
        /// <param name="axisPosition">AxisPosition</param>
        static void ProcessSubSetElement(string query, AxisPosition axisPosition)
        {
            List<string> paramList = ParamSeperator(query, KeywordConstants.SUBSET);
            if (paramList.Count > 0)
            {
                string[] crossJoinElements = paramList[0].Split('*');

                foreach (var item in crossJoinElements)
                {
                    ProcessVisualTotals(axisPosition, item);
                }
                SubsetElement subSetElement = new SubsetElement();
                if (paramList.Count == 3)
                {
                    subSetElement.StartIndex = Convert.ToInt32(paramList[1]);
                    subSetElement.EndIndex = Convert.ToInt32(paramList[2]);
                }
                else
                    subSetElement.StartIndex = Convert.ToInt32(paramList[1]);

                AddElementToAxes(axisPosition, subSetElement);
            }
        }

        /// <summary>
        /// method used to return the param of the given method
        /// </summary>
        /// <param name="query">string</param>
        /// <param name="functionName">string</param>
        /// <returns>List<string></returns>
        private static List<string> ParamSeperator(string query, string functionName)
        {
            query = query.Trim(' ').Remove(0, functionName.Count());
            query = RemoveMatchedBraces(query.Trim(), '(', ')');

            string tempString = string.Empty;
            List<string> paramList = new List<string>();
            int lparen = 0, rparen = 0, lbrace = 0, rbrace = 0;
            foreach (var item in query)
            {
                tempString += item.ToString();
                if (item == '(')
                    lparen++;
                else if (item == ')')
                    rparen++;
                else if (item == '{')
                    lbrace++;
                else if (item == '}')
                    rbrace++;
                if (item == ',' && lparen == rparen && lbrace == rbrace)
                {
                    paramList.Add(tempString.Trim(',', ' '));
                    tempString = string.Empty;
                }
            }
            if (tempString != string.Empty)
                paramList.Add(tempString.Trim());
            return paramList;
        }



        /// <summary>
        /// Process the CROSSJOIN method.
        /// </summary>
        /// <param name="axisPosition"></param>
        /// <param name="crossJoinQuery"></param>
        private static string[] ProcessCrossJoin(string crossJoinQuery)
        {
            string[] crossElements = null;
            if (crossJoinQuery.Contains(KeywordConstants.CROSSJOIN))
            {
                crossElements = ExtractElementFromCrossJoinFunction(crossJoinQuery);
            }
            else if (crossJoinQuery.Contains(KeywordConstants.ASTERISK))
            {
                crossElements = ExtractElementFromCrossJoin(crossJoinQuery);
            }
            return crossElements;
        }

        /// <summary>
        /// Extract Elements from query which having *
        /// </summary>
        /// <param name="Query">string</param>
        /// <returns>string[]</returns>
        static string[] ExtractElementFromCrossJoin(string Query)
        {
            List<string> elements = new List<string>();
            string temp_String = string.Empty;
            int lparen = 0, rparen = 0;
            foreach (var item in Query)
            {
                temp_String += item.ToString();
                if (item == '(')
                    lparen++;
                else if (item == ')')
                    rparen++;
                if (item == '*' && lparen == rparen)
                {
                    elements.Add(temp_String.Trim('*', ' '));
                    temp_String = string.Empty;
                }
            }
            if (temp_String != string.Empty)
                elements.Add(temp_String.Trim());
            return elements.ToArray();
        }

        /// <summary>
        /// Extract Elements from CROSSJOIN function
        /// </summary>
        /// <param name="Query">string</param>
        /// <returns>string[]</returns>
        private static string[] ExtractElementFromCrossJoinFunction(string crossJoinQuery)
        {
            List<string> elementList = new List<string>();
            string temp_String = string.Empty;
            int listindex = 0;
            while (crossJoinQuery.Contains(KeywordConstants.CROSSJOIN))
            {
                crossJoinQuery = crossJoinQuery.Remove(0, KeywordConstants.CROSSJOIN.Count());
                crossJoinQuery = RemoveMatchedBraces(crossJoinQuery, '(', ')');
                temp_String = string.Empty;
                int lparen = 0, rparen = 0, lbrace = 0, rbrace = 0, index = 0, index1 = 0;
                foreach (var item in crossJoinQuery)
                {
                    temp_String += item.ToString();
                    index++;
                    if (item == '(')
                        lparen++;
                    else if (item == ')')
                        rparen++;
                    else if (item == '{')
                        lbrace++;
                    else if (item == '}')
                        rbrace++;
                    if (lparen == rparen && lbrace == rbrace && item == ',' && temp_String.Contains(KeywordConstants.CROSSJOIN))
                    {
                        elementList.Add(crossJoinQuery.Substring(index));
                        crossJoinQuery = temp_String.TrimEnd(',');
                        temp_String = string.Empty;
                    }
                    else if (lparen == rparen && lbrace == rbrace && item == ',')
                    {
                        elementList.Insert(index1, temp_String.Trim(',', ' '));
                        temp_String = string.Empty;
                        index1++;
                        listindex = index1;
                    }

                }
            }
            if (temp_String != string.Empty && !(temp_String.Contains(KeywordConstants.CROSSJOIN)))
                elementList.Insert(listindex, temp_String);
            return elementList.ToArray();
        }


        private static string ProcessVisualTotals(string query)
        {
            string elementQuery = query.Trim('{', '}', ' ');

            if (elementQuery.StartsWith(KeywordConstants.VISUALTOTALS))
            {
                elementQuery = elementQuery.Replace(KeywordConstants.VISUALTOTALS, string.Empty);
                elementQuery = RemoveMatchedBraces(elementQuery, '(', ')');
                elementQuery = RemoveMatchedBraces(elementQuery, '{', '}');
            }
            return elementQuery;
        }

        private static string ProcessHierarchize(string query)
        {
            string elementQuery = query.Trim('{', '}', ' ');
            if (elementQuery.StartsWith(KeywordConstants.HIERARCHIZE))
            {
                //olapReport.DrillType = DrillType.DrillPosition;
                elementQuery = elementQuery.Replace(KeywordConstants.HIERARCHIZE, string.Empty);
                elementQuery = RemoveMatchedBraces(elementQuery, '(', ')');
                elementQuery = RemoveMatchedBraces(elementQuery, '{', '}');
            }
            return elementQuery;
        }


        private static void ProcessVisualTotals(AxisPosition axisPosition, string query)
        {
            string elementQuery = query.Trim('{', '}', ' ');

            if (elementQuery.StartsWith(KeywordConstants.VISUALTOTALS))
            {
                elementQuery = elementQuery.Replace(KeywordConstants.VISUALTOTALS, string.Empty);
                elementQuery = RemoveMatchedBraces(elementQuery, '(', ')');
                elementQuery = RemoveMatchedBraces(elementQuery, '{', '}');
            }
            if (elementQuery.StartsWith(KeywordConstants.HIERARCHIZE))
            {
                //olapReport.DrillType = DrillType.DrillPosition;
                elementQuery = elementQuery.Replace(KeywordConstants.HIERARCHIZE, string.Empty);
                elementQuery = RemoveMatchedBraces(elementQuery, '(', ')');
                elementQuery = RemoveMatchedBraces(elementQuery, '{', '}');
            }

            if (elementQuery.StartsWith(KeywordConstants.TOPCOUNT))
                ProcessTopCount(elementQuery, axisPosition);

            if (elementQuery.StartsWith(KeywordConstants.EXCEPT))
            {
                ProcessExcept(elementQuery, axisPosition);
            }
            if (elementQuery.StartsWith(KeywordConstants.ORDER))
                ProcessOrder(elementQuery, axisPosition);

            if (elementQuery.StartsWith(KeywordConstants.FILTER))
                ProcessFilter(elementQuery, axisPosition);

            elementQuery = ProcessDrillDownMember(RemoveMatchedBraces(elementQuery, '(', ')'));

            if (elementQuery.StartsWith("[") && elementQuery.EndsWith("]"))
            {
                ProcessElements(axisPosition, elementQuery);
            }
            else if (elementQuery.StartsWith("(") && elementQuery.Contains(")"))
            {
                foreach (var item in elementQuery.Split(KeywordConstants.COMMA))
                {
                    string tempItem = item.Trim('(', ')', '{', '}', ' ');
                    ProcessElements(axisPosition, tempItem);
                }
            }
        }

        static void ProcessFilter(string query, AxisPosition axisPosition)
        {
            List<string> functionParameters = ParamSeperator(query, KeywordConstants.FILTER);
            string[] crossElement = null;
            if (functionParameters.Count > 0)
            {
                if (functionParameters[0].StartsWith(KeywordConstants.CROSSJOIN))
                {
                    crossElement = ExtractElementFromCrossJoinFunction(functionParameters[0]);
                }
                else
                    crossElement = ExtractElementFromCrossJoin(functionParameters[0]);

                if (functionParameters[0].StartsWith(KeywordConstants.VISUALTOTALS))
                {
                    ProcessVisualTotals(axisPosition, functionParameters[0]);
                }

                string filterExpression = functionParameters[1];
                string[] filOper = { ">=", "<=", "!=", ">", "<", "=", "<>" };

                string[] filterValues = filterExpression.Split(filOper, StringSplitOptions.RemoveEmptyEntries);

                FilterElement filterElement = new FilterElement(axisPosition);
                filterElement.IsFilterCondition = true;
                foreach (var item in crossElement)
                {
                    string tempItem = item.Trim();
                    if (tempItem.Contains(KeywordConstants.MEASURES))
                    {
                        Regex regex = new Regex(KeywordConstants.MEASURES_EXP);
                        var uniqueName = regex.Match(tempItem).Value;
                        MeasureElements measureElements = new MeasureElements();
                        measureElements.Elements.Add(GetMeasureElement(uniqueName));
                        filterElement.Elements.Add(measureElements);
                    }
                    else
                    {
                        Regex regex = new Regex(KeywordConstants.DIMENSION_EXP);
                        var uniqueName = regex.Match(tempItem).Value.TrimEnd('.');
                        DimensionElement dimensionElement = GetDimensionElement(uniqueName);
                        filterElement.Elements.Add(dimensionElement);
                        AddElementToAxes(axisPosition, dimensionElement, true);
                    }
                }

                Regex regex1 = new Regex(KeywordConstants.FILTER_EXP);
                var mathches = regex1.Matches(filterExpression);
                string filterCondition = string.Empty;
                foreach (Match item in mathches)
                {
                    filterCondition += item.Value;
                }
                switch (filterCondition)
                {
                    case "<":
                        filterElement.FilterCase = FilterCase.LessThan;
                        break;
                    case ">":
                        filterElement.FilterCase = FilterCase.GreaterThan;
                        break;
                    case "<=":
                        filterElement.FilterCase = FilterCase.LessThanOrEqualTo;
                        break;
                    case ">=":
                        filterElement.FilterCase = FilterCase.GreaterThanOrEqualTo;
                        break;
                    case "=":
                        filterElement.FilterCase = FilterCase.EqualTo;
                        break;
                    case "<>":
                        filterElement.FilterCase = FilterCase.NotEquals;
                        break;
                    case "!=":
                        filterElement.FilterCase = FilterCase.NotEquals;
                        break;
                }
                foreach (var item in filterValues)
                {
                    if (item.Contains(KeywordConstants.MEASURES))
                    {
                        Regex regex = new Regex(KeywordConstants.MEASURES_EXP);
                        string uniqueName = regex.Match(item).Value;
                        MeasureElement measureElement = GetMeasureElement(uniqueName);
                        measureElement.Visible = true;
                        filterElement.FilterValue.Add(measureElement);
                    }
                    else if (IsNumeric(item.Trim()))
                    {
                        filterElement.FilterValue.Add(new FilterValue()
                        {
                            Filter_Value = Convert.ToDouble(item.Trim())
                        });
                    }
                    else
                    {
                        Regex regex = new Regex(KeywordConstants.DIMENSION_EXP);
                        string uniqueName = regex.Match(item).Value.TrimEnd('.');
                        DimensionElement dimensionElement = GetDimensionElement(uniqueName);
                        dimensionElement.Visible = true;
                        filterElement.FilterValue.Add(dimensionElement);
                    }

                }
                olapReport.FilterElements.Add(new Item { Axis = axisPosition, ElementValue = filterElement });
            }
        }


        static bool IsNumeric(string strToCheck)
        {
            return Regex.IsMatch(strToCheck, "^\\d+(\\.\\d+)?$");
        }


        private static void ProcessOrder(string Query, AxisPosition axisPosition)
        {
            List<string> functionParam = ParamSeperator(Query, KeywordConstants.ORDER);
            if (functionParam.Count > 0)
            {
                foreach (var item in ExtractElementFromCrossJoin(functionParam[0]))
                {
                    ProcessVisualTotals(axisPosition, item);
                }
                string uniqueName = functionParam[1].Trim('(', ')').Trim('{', '}');
#if SILVERLIGHT
                SortOrder sortOrder = (SortOrder)Enum.Parse(typeof(SortOrder), functionParam[2].Trim('(', ')').Trim('{', '}'),true);
#else
                SortOrder sortOrder = (SortOrder)Enum.Parse(typeof(SortOrder), functionParam[2].Trim('(', ')').Trim('{', '}'));
#endif
                SortElement sortElement = new SortElement(axisPosition, sortOrder, true);
                sortElement.Element.UniqueName = uniqueName;

                AddElementToAxes(axisPosition, sortElement);
            }
        }

        /// <summary>
        /// Method processes the Except function
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="axisPosition"></param>
        private static void ProcessExcept(string Query, AxisPosition axisPosition)
        {
            List<string> functionParams = ParamSeperator(Query, KeywordConstants.EXCEPT);
            if (functionParams.Count > 0)
                AddExcludedElement(functionParams[0], functionParams[1], axisPosition);
            else
                throw new Exception("Error in Except function");
        }

        /// <summary>
        /// Add excluded element in axes
        /// </summary>
        /// <param name="element">string</param>
        /// <param name="excludedElement">string</param>
        /// <param name="axisPosition">AxisPosition</param>
        static void AddExcludedElement(string element, string excludedElement, AxisPosition axisPosition)
        {
            DimensionElement dimensionElement = GetDimensionElement(element);
            DimensionElement excludedDimensionElement = GetDimensionElement(excludedElement);
            if (axisPosition == AxisPosition.Categorical)
                olapReport.CategoricalElements.Add(dimensionElement, excludedDimensionElement);
            else if (axisPosition == AxisPosition.Series)
                olapReport.SeriesElements.Add(dimensionElement, excludedDimensionElement);
        }

        /// <summary>
        /// Process the DrillDownMember function of MDX
        /// </summary>
        /// <param name="elementQuery">strin</param>
        /// <returns>List</returns>
        private static string ProcessDrillDownMember(string elementQuery)
        {
            List<string> drillDownMembers = new List<string>();
            string localQuery = string.Empty;
            while (elementQuery.StartsWith(KeywordConstants.DRILLDOWNMEMBER))
            {
                int drillDownMemIndex = elementQuery.IndexOf(KeywordConstants.LPAREN);
                elementQuery = elementQuery.Substring(drillDownMemIndex);
                elementQuery = RemoveMatchedBraces(elementQuery, '(', ')');
                elementQuery = RemoveMatchedBraces(elementQuery, '{', '}');
                string localQuery1 = elementQuery;
                localQuery = ProcessDrillDownMember(elementQuery, localQuery);
                elementQuery = localQuery;

                string drillDownMember = RemoveMatchedBraces(localQuery1.Substring(localQuery.Length).Trim(','), '{', '}');
                if (drillDownMember != string.Empty)
                    drillDownMembers.Add(drillDownMember);
            }
            elementQuery = RemoveMatchedBraces(elementQuery, '(', ')');
            elementQuery = ProcessDrillDownLevel(elementQuery);
            return elementQuery;
        }

        /// <summary>
        /// Process the DrillDownLevel member of MDX
        /// </summary>
        /// <param name="elementQuery">string</param>
        /// <returns>string</returns>
        private static string ProcessDrillDownLevel(string elementQuery)
        {
            if (elementQuery.StartsWith(KeywordConstants.DRILLDOWNLEVEL))
            {
                elementQuery = elementQuery.Replace(KeywordConstants.DRILLDOWNLEVEL, string.Empty);
                elementQuery = RemoveMatchedBraces(elementQuery, '(', ')');
                elementQuery = RemoveMatchedBraces(elementQuery, '{', '}');
            }
            return elementQuery;
        }

        /// <summary>
        /// Process the TopCountElement
        /// </summary>
        /// <param name="rejoinedQuery">string</param>
        /// <param name="axisposition">AxisPosition</param>
        private static void ProcessTopCount(string rejoinedQuery, AxisPosition axisposition)
        {
            string topCountElementQuery = rejoinedQuery;
            topCountElementQuery = RemoveMatchedBraces(topCountElementQuery, '(', ')');
            topCountElementQuery = RemoveMatchedBraces(topCountElementQuery, '{', '}');
            topCountElementQuery = RemoveMatchedBraces(topCountElementQuery, '(', ')');
            topCountElementQuery = RemoveMatchedBraces(topCountElementQuery, '{', '}');

            List<string> topCountParameters = ParamSeperator(topCountElementQuery, KeywordConstants.TOPCOUNT);

            topCountParameters[0] = RemoveMatchedBraces(topCountParameters[0], '{', '}');

            foreach (var item in ExtractElementFromCrossJoin(topCountParameters[0]))
            {
                if (item.StartsWith(KeywordConstants.FILTER))
                {
                    ProcessFilter(item, axisposition);
                }
                else
                {
                    string tempItem = item;
                    if (tempItem.StartsWith(KeywordConstants.DRILLDOWNLEVEL))
                        tempItem = ProcessDrillDownLevel(tempItem);
                    AddElementToAxes(axisposition, GetDimensionElement(tempItem));
                }
            }

            TopCountElement topCountElement = new TopCountElement();
            topCountElement.Axis = axisposition;
            topCountElement.FieldCount = Convert.ToInt32(topCountParameters[1].Trim());
            if (topCountParameters.Count() > 2)
            {
                string[] measureName = topCountParameters[2].Split('.');
                topCountElement.MeasureName = measureName[1].Trim(' ', '[', ']');
            }
            AddElementToAxes(axisposition, topCountElement);
        }


        /// <summary>
        /// Method process the drill down member
        /// </summary>
        /// <param name="elementQuery">string</param>
        /// <param name="localQuery">string</param>
        /// <returns>string</returns>
        private static string ProcessDrillDownMember(string elementQuery, string localQuery)
        {
            localQuery = elementQuery;
            int lparen = 0, rparen = 0, lbrace = 0, rbrace = 0;
            string temp_String = string.Empty;
            foreach (var item in elementQuery)
            {
                temp_String += item.ToString();
                if (item == '(')
                    lparen++;
                else if (item == ')')
                    rparen++;
                else if (item == '{')
                    lbrace++;
                else if (item == '}')
                    rbrace++;
                if (lparen != 0 && lparen == rparen)
                {
                    lparen = rparen = rbrace = lbrace = 0;
                    localQuery = temp_String;
                    break;
                }
            }
            return localQuery;
        }

        /// <summary>
        /// Process the dimension and measure elemenet specially it take cares of multiple drilldownmembers
        /// </summary>
        /// <param name="axisPosition">AxisPosition</param>
        /// <param name="query">string</param>
        /// <param name="drillDownMembers">List<string></param>
        private static void ProcessElements(AxisPosition axisPosition, string query, List<string> drillDownMembers)
        {
            if (drillDownMembers.Count == 0)
            {
                ProcessElements(axisPosition, query);
            }
            else
            {
                DimensionElement dimensionElement = new DimensionElement();
                string[] dimensionNames = query.Split('.');
                string dimensionName = dimensionNames[0].Replace("[", "").Replace("]", "").Trim();
                string hierarchyName = dimensionNames[1].Replace("[", "").Replace("]", "").Trim();
                string levelName = string.Empty;
                dimensionElement.Name = dimensionName;
                dimensionElement.HierarchyName = hierarchyName;

                //TODO: Need to handle multiple drilldownMember.
                if (dimensionNames.Count() > 2)
                    levelName = dimensionNames[2].Replace("[", "").Replace("]", "");

                if (drillDownMembers[drillDownMembers.Count - 1] != null)
                {
                    levelName = drillDownMembers[drillDownMembers.Count - 1].Split('.')[2].Trim(new char[] { '[', ']', ' ' });
                    dimensionElement.AddLevel(hierarchyName, levelName);
                    dimensionElement.Hierarchy.LevelElements[levelName].Add(new MemberElement { UniqueName = drillDownMembers[drillDownMembers.Count - 1] });
                    dimensionElement.DrillState = DrillState.ExpandToLevel;
                    dimensionElement.DrillUpDownLevel = levelName;
                    //dimensionElement.DrillUpDownMember = drillDownMembers[drillDownMembers.Count - 1].Split('.')[3].Trim(new char[] { '[', ']', '&', ' ' });
                    dimensionElement.DrillUpDownMember = drillDownMembers[drillDownMembers.Count - 1];
                }
                AddElementToAxes(axisPosition, dimensionElement);
            }
        }


        /// <summary>
        /// Process the dimension and measure elements
        /// </summary>
        /// <param name="position">AxisPosition</param>
        /// <param name="query">string</param>
        private static void ProcessElements(AxisPosition position, string query)
        {
            if (query.Contains(KeywordConstants.MEASURES))
            {
                string[] measures = query.Split(',');
                MeasureElements measureElements = new MeasureElements();
                foreach (var item in measures)
                {
                    string elementItem = item.Trim('{', '}', '(', ')', ' ');
                    var calcMember = calculatedMembers.FirstOrDefault(s => s.Key == elementItem.Trim());
                    if (calcMember.Value != null)
                    {
                        CalculatedMember calculatedMember = GetCalculatedMember(calcMember.Key);
                        olapReport.CalculatedMembers.Add(calculatedMember);
                        AddElementToAxes(position, calculatedMember);
                    }
                    else if (elementItem.Contains(KeywordConstants.MEASURES))
                    {
                        measureElements.Add(GetMeasureElement(elementItem));
                    }
                    //else if (elementItem.Contains(KeywordConstants.DOT))
                    else if (Regex.IsMatch(elementItem, KeywordConstants.DIMENSION_EXP) && elementItem.Contains(KeywordConstants.DOT))
                    {
                        DimensionElement dimensionElement = GetDimensionElement(elementItem);
                        AddElementToAxes(position, dimensionElement);
                    }
                    else
                    {
                        NamedSetElement namedSetElement = new NamedSetElement();
                        namedSetElement.Name = elementItem.Trim('[', ']', ' ');
                        AddElementToAxes(position, namedSetElement);
                    }
                }
                if (measureElements.Elements.Count > 0)
                    AddElementToAxes(position, measureElements);
            }
            else if (query.Contains(KeywordConstants.DOT))
            {
                var calcMember = calculatedMembers.FirstOrDefault(s => s.Key == query);
                if (calcMember.Value != null)
                {
                    CalculatedMember calculatedMember = GetCalculatedMember(calcMember.Key);
                    olapReport.CalculatedMembers.Add(calculatedMember);
                    AddElementToAxes(position, calculatedMember);

                }
                else
                {
                    DimensionElement dimensionElement = GetDimensionElement(query);
                    AddElementToAxes(position, dimensionElement);
                }
            }
            else if (query.Contains(KeywordConstants.COMMA) && calculatedMembers.Count > 0)
            {
                string[] calcMembers = query.Split(',');
                foreach (var item in calcMembers)
                {
                    var calcMember = calculatedMembers.FirstOrDefault(s => s.Key == item);
                    if (calcMember.Value != null)
                    {
                        CalculatedMember calculatedMember = GetCalculatedMember(calcMember.Key);
                        olapReport.CalculatedMembers.Add(calculatedMember);
                        AddElementToAxes(position, calculatedMember);
                    }
                }
            }
            else if (calculatedMembers.Count > 0 && calculatedMembers.Any(i => i.Key == query))
            {
                var calcMember = calculatedMembers.FirstOrDefault(s => s.Key == query);
                if (calcMember.Value != null)
                {
                    CalculatedMember calculatedMember = GetCalculatedMember(calcMember.Key);
                    olapReport.CalculatedMembers.Add(calculatedMember);
                    AddElementToAxes(position, calculatedMember);
                }
            }
            else
            {
                NamedSetElement namedSetElement = new NamedSetElement();
                namedSetElement.Name = query.Trim('[', ']', '{', '}', ' ');
                AddElementToAxes(position, namedSetElement);
            }
        }

        /// <summary>
        /// Add elements to corresponding axis.
        /// </summary>
        /// <param name="position">AxisPosition</param>
        /// <param name="elementValue">Element</param>
        private static void AddElementToAxes(AxisPosition position, Element elementValue)
        {
            switch (position)
            {
                case AxisPosition.Categorical:
                    olapReport.CategoricalElements.Add(elementValue);
                    break;
                case AxisPosition.Series:
                    olapReport.SeriesElements.Add(elementValue);
                    break;
                case AxisPosition.Slicer:
                    olapReport.SlicerElements.Add(elementValue);
                    break;
            }
        }


        private static void AddElementToAxes(AxisPosition position, Element elementValue, bool isSortOrFilterOn)
        {
            switch (position)
            {
                case AxisPosition.Categorical:
                    olapReport.CategoricalElements.Add(elementValue);
                    olapReport.CategoricalElements.IsFilterOrSortOn = isSortOrFilterOn;
                    break;
                case AxisPosition.Series:
                    olapReport.SeriesElements.Add(elementValue);
                    olapReport.SeriesElements.IsFilterOrSortOn = isSortOrFilterOn;
                    break;
                case AxisPosition.Slicer:
                    olapReport.SlicerElements.Add(elementValue);
                    olapReport.SlicerElements.IsFilterOrSortOn = isSortOrFilterOn;
                    break;
            }
        }

        /// <summary>
        /// Processes the query and gives the corresponding MeasureElement
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private static MeasureElements GetMeasureElements(string query)
        {
            MeasureElements measureElements = new MeasureElements();
            string[] dimensions = query.Split(',');
            foreach (var item in dimensions)
            {
                var calcMember = calculatedMembers.Where(s => s.Key == item.Trim());

                if (calcMember.Count() > 0)
                {
                    CalculatedMember calculatedMember = GetCalculatedMember(calcMember.FirstOrDefault().Key);

                }
                else
                {
                    MeasureElement measureElement = GetMeasureElement(item);
                    measureElements.Add(measureElement);
                }

            }
            return measureElements;
        }


        private static CalculatedMember GetCalculatedMember(string key)
        {
            string value = calculatedMembers.FirstOrDefault(s => s.Key == key.Trim()).Value;
            CalculatedMember calculatedMember = new CalculatedMember();
            if (key.Contains(KeywordConstants.MEASURES))
            {
                calculatedMember.Name = key.Substring(key.LastIndexOf('[')).Trim('[', ']', ' ');
                calculatedMember.Type = TypeOfMember.Measure;
                Regex regex = new Regex(KeywordConstants.MEASURES_EXP);
                string uniqueName = regex.Match(value).Value;
                calculatedMember.AddElement(GetMeasureElement(uniqueName));
            }
            else if (key.Contains(KeywordConstants.DOT))
            {
                calculatedMember.Type = TypeOfMember.Dimension;
                calculatedMember.Name = key.Substring(key.LastIndexOf('[')).Trim('[', ']', ' ');
                Regex regex = new Regex(KeywordConstants.DIMENSION_EXP);
                string uniqueName = regex.Match(value).Value;
                string[] elements = uniqueName.Split('.');
                DimensionElement dimensionElement = new DimensionElement();
                dimensionElement.Name = elements[0].Trim('[', ']', ' ');
                dimensionElement.HierarchyName = elements[1].Trim('[', ']', ' ');
                if (elements.Count() > 2)
                {
                    dimensionElement.AddLevel(elements[1].Trim('[', ']', ' '), elements[2].Trim('[', ']', ' '));
                }
                calculatedMember.AddElement(dimensionElement);
            }
            else
            {
                calculatedMember.Name = key.Substring(key.LastIndexOf('[')).Trim('[', ']', ' ');
                calculatedMember.Type = TypeOfMember.Measure;
                Regex regex = new Regex(KeywordConstants.MEASURES_EXP);
                string uniqueName = regex.Match(value).Value;
                calculatedMember.AddElement(GetMeasureElement(uniqueName));
            }
            calculatedMember.Expression = value;
            return calculatedMember;
        }

        private static MeasureElement GetMeasureElement(string item)
        {
            MeasureElement measureElement = new MeasureElement()
            {
                UniqueName = item.Trim(),
                Name = item.Replace("[", "").Replace("]", "").Replace(KeywordConstants.MEASURES, "").Replace(".", "").Trim()
            };
            return measureElement;
        }

        /// <summary>
        /// Process the query and gives corresponding DimensionElement
        /// </summary>
        /// <param name="query">string</param>
        /// <returns>DimensionElement</returns>
        private static DimensionElement GetDimensionElement(string query)
        {
            DimensionElement dimensionElement = new DimensionElement();

            string[] dimensionElements = query.Trim('{', '}', ' ').Split(',');
            string[] dimensionNames = dimensionElements[0].Split('.');
            string dimensionName = dimensionNames[0].Replace("[", "").Replace("]", "").Trim();
            string hierarchyName = dimensionNames[1].Replace("[", "").Replace("]", "").Trim();
            string levelName = string.Empty;
            dimensionElement.Name = dimensionName;
            dimensionElement.HierarchyName = hierarchyName;

            if (dimensionNames.Count() > 2 && dimensionNames[2] != "MEMBERS" && dimensionNames[2] != "CHILDREN")
            {
                if (dimensionNames[2].StartsWith("&"))
                {
                    dimensionElement.AddLevel(hierarchyName, levelName);

                    foreach (var item in dimensionElements)
                    {
                        string[] memberElements = item.Split('.');
                        dimensionElement.Hierarchy.LevelElements[levelName].MemberElements.Add(
                            new MemberElement
                            {
                                UniqueName = item,
                                ShowChildMembers = true
                            });
                    }
                    dimensionElement.Hierarchy.LevelElements[levelName].IncludeAvailableMembers = true;
                }
                else
                {
                    if (dimensionNames.Count() > 3)
                    {
                        levelName = dimensionNames[2].Trim('[', ']', ' ');
                        dimensionElement.AddLevel(hierarchyName, levelName);
                    }
                    else
                    {
                        foreach (var item in dimensionElements)
                        {
                            levelName = item.Split('.')[2].Trim('[', ']', ' ');
                            dimensionElement.Hierarchy.LevelElements.Add(new LevelElement { Name = levelName });
                        }
                    }
                }
            }
            if (dimensionNames.Count() > 3)
            {
                if (dimensionNames[3].StartsWith("&"))
                {

                    foreach (var item in dimensionElements)
                    {
                        dimensionElement.Hierarchy.LevelElements[levelName].Add(
                                            new MemberElement
                                            {
                                                UniqueName = item,
                                                ShowChildMembers = true
                                            });
                    }
                    dimensionElement.Hierarchy.LevelElements[levelName].IncludeAvailableMembers = true;
                }
                else
                {
                    foreach (var item in dimensionElements)
                    {
                        string[] memberElements = item.Split('.');
                        dimensionElement.Hierarchy.LevelElements[levelName].Add(
                            new MemberElement
                            {
                                UniqueName = item,
                                Name = memberElements[3].Trim('[', ']').Trim(),
                                ShowChildMembers = true
                            });
                    }
                    dimensionElement.Hierarchy.LevelElements[levelName].IncludeAvailableMembers = true;
                }
            }
            if (levelName == string.Empty)
                dimensionElement.AddLevel(hierarchyName, levelName);
            return dimensionElement;
        }



        /// <summary>
        /// Method removes the matched parenthesis from start of line '(' and end of the line ')'
        /// </summary>
        /// <param name="query"></param>
        /// <param name="lparen"></param>
        /// <param name="rparen"></param>
        /// <returns></returns>
        private static string RemoveMatchedBraces(string query, char lparen, char rparen)
        {
            while (query.IndexOf(lparen) == 0 && query.LastIndexOf(rparen) == (query.Length - 1))
            {
                int elementindex = query.IndexOf(lparen);
                query = query.Remove(elementindex, 1).Trim();
                query = query.Remove(query.LastIndexOf(rparen), 1).Trim();
            }
            return query;
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
                    throw new Exception("PARSER ERROR: Are you missing '" + lParam + "'. Because extra '" + rParam + "' in the Query.");
                }
            }
            if (count > 0)
            {
                throw new Exception("PARSER ERROR: Are you missing '" + rParam + "'. Because extra '" + lParam + "' in the Query.");
            }
        }
    }
}
