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
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections;

namespace Syncfusion.Olap.Engine.CalculationColumn
{
    /// <summary>
    /// This class supports the calculations of expressions and summaries on an IEnumerable object. See the
    /// InitSummaryLibrary and InitFunctionLibrary methods to see code for adding support for specific 
    /// summaries and functions. Summaries are agregation calculations applied to IEnumerable lists (eg, StdDev), and
    /// functions are calculation applied to singular objects, usually members of an IEnumerable list (eg, Cos).
    /// </summary>
    public class ExpressionHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionHelper"/> class.
        /// </summary>
        public ExpressionHelper()
        {
            object o = SummaryLibrary; //forces initialization...
        }

        #region public properties

        bool caseSensitive = true;

        /// <summary>
        /// Gets or sets whether the case is considered in the parsing of formulas.
        /// </summary>
        public bool CaseSensitive
        {
            get { return caseSensitive; }
            set { caseSensitive = value; }
        }

        private static char listSeparator = ',';
        private static char quoteMark = '"';

        /// <summary>
        /// Gets or sets the quote mark.
        /// </summary>
        /// <value>The quote mark.</value>
        public static char QuoteMark
        {
            get
            {
                return ExpressionHelper.quoteMark;
            }
            set { ExpressionHelper.quoteMark = value; }
        }

        /// <summary>
        /// Gets or sets the list separator.
        /// </summary>
        /// <value>The list separator.</value>
        public static char ListSeparator
        {
            get
            {
               return ExpressionHelper.listSeparator;
            }
            set { ExpressionHelper.listSeparator = value; }
        }

        ExpressionError error = ExpressionError.None;
        /// <summary>
        /// Gets the error result from the most recent calculation.
        /// </summary>
        public ExpressionError Error
        {
            get { return error; }
        }
        /// <summary>
        /// Gets the description of the most recent calculation.
        /// </summary>
        public string ErrorString
        {
            get
            {
                if (error != ExpressionError.ExceptionRaised)
                {
                    return error.ToString();
                }
                return CalulationExtensions.ErrorString;
            }
        }

        #endregion

        #region public Methods

        /// <summary>
        /// Gets whether this string is the name of an expression.
        /// </summary>
        /// <param name="expressionName">The name of the expression.</param>
        /// <returns>True if there is an expression with this name.</returns>
        public bool IsExpressionName(string expressionName)
        {
            return expressionDefinitions.ContainsKey(expressionName);
        }

        /// <summary>
        /// Gets the value of an expression for a given item
        /// </summary>
        /// <param name="helper">The Expression helper.</param>
        /// <param name="expressionName">The name of the expression.</param>
        /// <param name="item">The item for which the expression is to be evaluated.</param>
        /// <returns>Computed Expression</returns>
        public static object GetComputedValue(ExpressionHelper helper, string expressionName, object item)
        {
            if (helper.IsExpressionName(expressionName))
            {
                return helper.expressionDefinitions[expressionName].ComputedValue(item);
            }

            return null;
        }

        /// <summary>
        /// Computes a summary calculation.
        /// </summary>
        /// <param name="summaryFormula">The summary formula.</param>
        /// <param name="dataSource">The data source.</param>
        /// <returns></returns>
        public object ComputeSummary(string summaryFormula, IEnumerable dataSource)
        {
            summaryFormula = ProcessLongNamesAndConstants(summaryFormula);
            error = ExpressionError.None;
            Delegate evaluator = dataSource.GetCompiledSummary(summaryFormula.Replace(" ", ""), out error, this);
            if (evaluator != null)
            {
                object o = null;
                try
                {
                    o = evaluator.DynamicInvoke(dataSource);
                    return o;
                }
                catch (Exception ex)
                {
                    CalulationExtensions.ErrorString = ex.Message;
                }
            }
            return null;
        }

        /// <summary>
        /// Adds a visible expression column displaying the passed-in expression.
        /// </summary>
        /// <param name="name">The MappingName for the added column.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>True if a column was added.</returns>
        public bool AddExpression(string name, string expression)
        {
            return AddExpression(name, expression, true);
        }

        /// <summary>
        /// Adds an expression to this ExpressionHelper class.
        /// </summary>
        /// <param name="name">The name assigned to this expression.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="showInGrid">Indicates whether a visible column should be added to the underlying grid.</param>
        /// <returns>True if expression was added.</returns>
        public bool AddExpression(string name, string expression, bool showInGrid)
        {
            expression = ProcessLongNamesAndConstants(expression);
            if (!IsExpressionName(name))
            {
                expressionDefinitions.Add(name, new ExpressionObject(name, expression, this));
            
                //if (grid != null && showInGrid && NeedToAddVisibleColumn(name))
                //{
                //    GridDataUnboundVisibleColumn col = new GridDataUnboundVisibleColumn();
                //    col.MappingName = name;
                //    col.HeaderText = name;

                //    grid.VisibleColumns.Add(col);
                //}
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes an expression.
        /// </summary>
        /// <param name="name">The name of the expression to be removed.</param>
        /// <returns>True if the expression was removed.</returns>
        public bool RemoveExpression(string name)
        {
            if (IsExpressionName(name))
            {
                expressionDefinitions.Remove(name);
                //if (grid != null)
                //{
                //    var list = grid.VisibleColumns.Where(col => col.MappingName == name).ToList();
                //    if (list.Count == 1)
                //    {
                //        grid.VisibleColumns.Remove(list[0]);
                //    }
                //}
                return true;
            }
            return false;
        }

        /// <summary>
        /// Changes an expression.
        /// </summary>
        /// <param name="name">The name of the expression to be changed.</param>
        /// <param name="expression">A string containing the new expression.</param>
        /// <returns>True if the change was done.</returns>
        public bool ChangeExpression(string name, string expression)
        {
            if (IsExpressionName(name))
            {
                expressionDefinitions[name] = new ExpressionObject(name, expression, this);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clears all expressions from this ExpressionHelper.
        /// </summary>
        /// <returns>True if all the expressions are removed.</returns>
        public bool Clear()
        {
            expressionDefinitions.Clear();
            //if (grid != null)
            //{
            //    var list = new List<GridDataVisibleColumn>();
            //    foreach (GridDataVisibleColumn col in grid.VisibleColumns)
            //    {
            //        if (col is GridDataUnboundVisibleColumn)
            //        {
            //            list.Add(col);
            //        }
            //    }
            //    foreach (GridDataVisibleColumn col in list)
            //    {
            //        grid.VisibleColumns.Remove(col);
            //    }
            //}
            return true;
        }

        #endregion

        #region private methods, fields and properties

        #region properties
        private Dictionary<string, ExpressionObject> expressionColumnsContent = null;
        internal Dictionary<string, ExpressionObject> expressionDefinitions 
        {
            get
            {
                if (expressionColumnsContent == null)
                {
                    expressionColumnsContent = new Dictionary<string, ExpressionObject>();
                }

                return expressionColumnsContent;
            }
        }

        #endregion

        #region grid event handlers

        //void grid_ModelLoaded(object sender, EventArgs e)
        //{
        //    this.grid.Model.QueryUnboundColumnValue += new GridDataQueryUnboundColumnCellEventHandler(Model_QueryUnboundColumnValue);
        //    this.grid.Model.Views.First().CurrentCellMoved += new GridCurrentCellMovedEventHandler(ExpressionColumnHelper_CurrentCellMoved);
        //    this.grid.Model.Views.First().CurrentCellValidated += new Syncfusion.Windows.ComponentModel.GridRoutedEventHandler(ExpressionColumnHelper_CurrentCellValidated);
        //}

        //to trigger updates for changes...
        //private bool cellChanged = false;
        //private int changedRow = -1;
        //void ExpressionColumnHelper_CurrentCellValidated(object sender, Syncfusion.Windows.ComponentModel.SyncfusionRoutedEventArgs args)
        //{
        //    cellChanged = true;
        //    changedRow = grid.Model.CurrencyManager.CurrentRowIndex;
        //}

        //void ExpressionColumnHelper_CurrentCellMoved(object sender, GridCurrentCellMovedEventArgs e)
        //{
        //    if (cellChanged)
        //    {
        //        grid.Model.CurrencyManager.EndEdit();
        //        grid.Model.Views.First().InvalidateCell(GridRangeInfo.Row(changedRow));
        //        cellChanged = false;
        //    }
        //}

        //void Model_QueryUnboundColumnValue(object sender, GridDataQueryUnboundColumnCellEventArgs args)
        //{
        //    object o = GetComputedValue(this, args.UnboundColumn.MappingName, ((GridDataRecord)args.Record).Data);
        //    if (o != null)
        //    {
        //        args.Style.CellValue = o;
        //        args.Handled = true;
        //    }
        //}

        #endregion

        #region private methods ProcessLongNamesAndConstants, NeedToAddVisibleColumn
        private string ProcessLongNamesAndConstants(string expression)
        {
            expression = expression.Replace("Fields.", "").Replace(".Value", "").Replace("FIELDS.", "").Replace(".VALUE", "").Replace("fields.", "").Replace(".value", "");
            if (expression.IndexOf("Constants.") > -1 || expression.IndexOf("CONSTANTS.") > -1 || expression.IndexOf("constants.") > -1)
            {
                StringBuilder sb = new StringBuilder(expression);
                foreach (string key in NamedConstants.Keys)
                {
                    string value = NamedConstants[key].ToString();
                    string name = "Constants." + key;
                    sb = sb.Replace(name, value).Replace(name.ToUpper(), value.ToUpper()).Replace(name.ToLower(), value.ToLower());
                }

                expression = sb.ToString();
            }
            return expression;
        }

        //called only from a method that first tests to see if grid in not null...
        private bool NeedToAddVisibleColumn(string name)
        {
            bool b = true;
            //if (grid.VisibleColumns.Count > 0)
            //{
            //    foreach (GridDataVisibleColumn col in grid.VisibleColumns)
            //    {
            //        if (col.MappingName == name || col.HeaderText == name)
            //        {
            //            b = false;
            //            break;
            //        }
            //    }
            //}
            return b;
        }
        #endregion
       
        #endregion

        #region summary library code

        private Dictionary<string, SummarySignature> summaryLibrary;

        internal Dictionary<string, SummarySignature> SummaryLibrary
        {
            get
            {
                if (summaryLibrary == null)
                {
                    summaryLibrary = new Dictionary<string, SummarySignature>();
                    InitSummaryLibrary();
                }

                return summaryLibrary;
            }
        }

        /// <summary>
        /// Adds a summary calculation to the SummaryLibrary.
        /// </summary>
        /// <param name="name">The name of the summary calculation.</param>
        /// <param name="exp">LambdaExpression defining the summary calculation.</param>
        /// <returns>True if the summary was added.</returns>
        public bool AddSummary(string name, LambdaExpression exp)
        {
            if (summaryLibrary == null)
            {
                summaryLibrary = new Dictionary<string, SummarySignature>();
            }

            if (exp == null)
            {
                return false;
            }

            string key = GetKey(name, exp);
            if (summaryLibrary.ContainsKey(key))
            {
                summaryLibrary[key] = new SummarySignature(name, exp);
            }
            else
            {
                summaryLibrary.Add(key, new SummarySignature(name, exp));
            }

            return true;
        }

        /// <summary>
        /// Removes a summary from SummaryLibrary.
        /// </summary>
        /// <param name="name">The name of the expression to be removed.</param>
        /// <param name="exp">The expression to be removed.</param>
        /// <returns>True if the removal was done.</returns>
        public bool RemoveSummary(string name, LambdaExpression exp)
        {
            if (summaryLibrary == null)
            {
                return false;
            }

            string key = GetKey(name, exp);
            if (summaryLibrary.ContainsKey(key))
            {
                summaryLibrary[key].RemoveExpression(exp);
                if (summaryLibrary[key].LambdaExpressions.Count == 0)
                {
                    summaryLibrary.Remove(key);
                }
                return true;
            }

            return false;
        }

        #region summary calculation definitions
        /// <summary>
        /// Computes the maximum value in the given list.
        /// </summary>
        /// <param name="list">A list containing numeric values.</param>
        /// <returns>The maximum value.</returns>
        public double ComputeMaximum(IEnumerable list)
        {
            double d1 = double.MinValue;
            double d;
            try
            {
                foreach (object o in list)
                {
                    if (o != null)
                    {
                        d = (double)Convert.ChangeType(o, typeof(double));
                        d1 = Math.Max(d, d1);
                    }
                }
            }
            catch(Exception ex)
            {
                CalulationExtensions.ErrorString = ex.Message;
            }

            return d1;
        }

        /// <summary>
        /// Computes the minimum value for a given list.
        /// </summary>
        /// <param name="list">A list of numeric values.</param>
        /// <returns>The minimum value.</returns>
        public double ComputeMinimum(IEnumerable list)
        {
            double d1 = double.MaxValue;
            double d;
            try
            {
                foreach (object o in list)
                {
                    if (o != null)
                    {
                        d = (double)Convert.ChangeType(o, typeof(double));
                        d1 = Math.Min(d, d1);
                    }
                }
            }
            catch (Exception ex)
            {
                CalulationExtensions.ErrorString = ex.Message;
            }
            return d1;
        }

        /// <summary>
        /// Computes the sum of the values in a given list.
        /// </summary>
        /// <param name="list">A list of numerica values.</param>
        /// <returns>The sum.</returns>
        public double ComputeSum(IEnumerable list)
        {
            //// TODO - double.min returns the wrong result
            double d1 = 0;//double.MinValue;
            double d;

            try
            {
                foreach (object o in list)
                {
                    if (o != null)
                    {
                        d = (double)Convert.ChangeType(o, typeof(double));
                        d1 += d;
                    }
                }
            }
            catch (Exception ex)
            {
                CalulationExtensions.ErrorString = ex.Message;
            }
            return d1;
        }

        /// <summary>
        /// Returns the number of items in a given list.
        /// </summary>
        /// <param name="list">A list of items.</param>
        /// <returns>The number of items in the list.</returns>
        public int ComputeCount(IEnumerable list)
        {
            int count = 0;
            foreach (object o in list)
            {
                count++;
            }
            return count;
        }

        /// <summary>
        /// Returns the sample standard deviation of a list of values.
        /// </summary>
        /// <param name="list">The list of numeric values.</param>
        /// <returns>The standard deviation of the list of numbers.</returns>
        public double ComputeStdDev(IEnumerable list)
        {
            double xbar = 0;
            double d = 0;
            int count = 0;
            double sumx2 = 0;
            double d1 = 0;
            try
            {
                foreach (object o in list)
                {
                    if (o != null)
                    {
                        d = (double)Convert.ChangeType(o, typeof(double));
                        xbar += d;
                        count++;
                    }
                }
                xbar = xbar / count;

                foreach (object o in list)
                {
                    if (o != null)
                    {
                        d = (double)Convert.ChangeType(o, typeof(double));
                        d1 = d - xbar;
                        sumx2 += d1 * d1;
                    }
                }
            }
            catch (Exception ex)
            {
                CalulationExtensions.ErrorString = ex.Message;
            }
            return Math.Sqrt(sumx2 / (count - 1));
        }

        #endregion

        /// <summary>
        /// Intializes the default summary library.
        /// </summary>
        public virtual void InitSummaryLibrary()
        {
            AddSummary("Maximum", (System.Linq.Expressions.Expression<Func<IEnumerable, double>>)((list) =>
                 ComputeMaximum(list)));
            AddSummary("Minimum", (System.Linq.Expressions.Expression<Func<IEnumerable, double>>)((list) =>
                 ComputeMaximum(list)));
            AddSummary("Sum", (System.Linq.Expressions.Expression<Func<IEnumerable, double>>)((list) =>
                 ComputeSum(list)));
            AddSummary("Count", (System.Linq.Expressions.Expression<Func<IEnumerable, int>>)((list) =>
                 ComputeCount(list)));
            AddSummary("StdDev", (System.Linq.Expressions.Expression<Func<IEnumerable, double>>)((list) =>
                ComputeStdDev(list)));            
        }
        #endregion

        #region function library code

        //Function names:
        // 1) must be alphanumeric
        // 2) cannot contain spaces
        // 3) are case insensitive
        // 4) must start with Alpha
        // 5) function signatures must be unique wrt to name and number of arguments - ie, if you have 2 functions with the same name, they must have a different number of arguments

        private Dictionary<string, FunctionSignature> functionLibrary;

        internal Dictionary<string, FunctionSignature> FunctionLibrary
        {
            get
            {
                if (functionLibrary == null)
                {
                    functionLibrary = new Dictionary<string, FunctionSignature>();
                    InitFunctionLibrary();
                }
                return functionLibrary;
            }
       }

        internal string GetKey(string name, LambdaExpression exp)
        {
            return name.ToUpper();
        }

        /// <summary>
        /// Adds a function calculation.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <param name="exp">The expression defining the function.</param>
        /// <returns>True if the function is added.</returns>
        public bool AddFunction(string name, LambdaExpression exp)
        {
            if (functionLibrary == null)
            {
                functionLibrary = new Dictionary<string, FunctionSignature>();
            }
            if (exp == null)
                return false;
            string key = GetKey(name, exp);
            if (functionLibrary.ContainsKey(key))
            {
                functionLibrary[key] = new FunctionSignature(name, exp);
            }
            else
            {
                functionLibrary.Add(key, new FunctionSignature(name, exp));
            }
            return true;
        }

        /// <summary>
        /// Removes a function.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <param name="exp">The function to be removed.</param>
        /// <returns>True if the function is removed.</returns>
        public bool RemoveFunction(string name, LambdaExpression exp)
        {
            if (functionLibrary == null)
            {
                return false;
            }

            string key = GetKey(name, exp);
            if (functionLibrary.ContainsKey(key))
            {
                functionLibrary[key].RemoveExpression(exp);
                if (functionLibrary[key].LambdaExpressions.Count == 0)
                {
                    functionLibrary.Remove(key);
                }
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Initializes the default function library.
        /// </summary>
        public virtual void InitFunctionLibrary()
        {
            AddFunction("Cos", (System.Linq.Expressions.Expression<Func<double, double>>)((x) => Math.Cos(x)));
            AddFunction("Tan", (System.Linq.Expressions.Expression<Func<double, double>>)((x) => Math.Tan(x)));
            AddFunction("Sin", (System.Linq.Expressions.Expression<Func<double, double>>)((x) => Math.Sin(x)));
            AddFunction("Max", (System.Linq.Expressions.Expression<Func<double, double, double>>)((x, y) => Math.Max(x, y)));
            AddFunction("Day", (System.Linq.Expressions.Expression<Func<DateTime, int>>)((x) => x.Day));
            AddFunction("Month", (System.Linq.Expressions.Expression<Func<DateTime, int>>)((x) => x.Month));
            AddFunction("Second", (System.Linq.Expressions.Expression<Func<DateTime, int>>)((x) => x.Second));
            AddFunction("Minute", (System.Linq.Expressions.Expression<Func<DateTime, int>>)((x) => x.Minute));
            AddFunction("Year", (System.Linq.Expressions.Expression<Func<DateTime, int>>)((x) => x.Year));
        }
        #endregion  

        #region named constants support
        private Dictionary<string, object> namedConstants;

        internal Dictionary<string, object> NamedConstants
        {
            get 
            {
                if (namedConstants == null)
                {
                    namedConstants = new Dictionary<string, object>();
                }
                return namedConstants; 
            }
            set { namedConstants = value; }
        }

        /// <summary>
        /// Adds the named constant.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddNamedConstant(string name, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }
            if (IsNamedConstant(name))
            {
                NamedConstants[name.ToUpper()] = value;
            }
            else
            {
                NamedConstants.Add(name.ToUpper(), value);
            }
        }

        /// <summary>
        /// Removes the named constant.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void RemoveNamedConstant(string name, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }
            if (IsNamedConstant(name))
            {
                NamedConstants.Remove(name.ToUpper());
            }
        }

        /// <summary>
        /// Changes the named constant.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="newValue">The new value.</param>
        public void ChangeNamedConstant(string name, object newValue)
        {
            if (newValue == null)
            {
                throw new ArgumentNullException();
            }
            if (IsNamedConstant(name))
            {
                NamedConstants[name.ToUpper()] = newValue;
            }
        }

        /// <summary>
        /// Clears the named constants.
        /// </summary>
        public void ClearNamedConstants()
        {
            NamedConstants.Clear();
        }

        /// <summary>
        /// Determines whether [is named constant] [the specified name].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if [is named constant] [the specified name]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNamedConstant(string name)
        {
            return NamedConstants.ContainsKey(name.ToUpper());
        }

        #endregion
    }


    #region ExpressionObject class
    /// <summary>
    /// A wrapper class for an expression object added to an ExpressionHelper class.
    /// </summary>
    public class ExpressionObject
    {
        private ExpressionHelper helper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">MappingName for this expression.</param>
        /// <param name="expression">String holding the formula defining this expression.</param>
        /// <param name="helper">The ExpressionHelper.</param>
        public ExpressionObject(string name, string expression, ExpressionHelper helper)
        {
            this.name = name;
            this.expression = expression;
            this.helper = helper;
        }
         
        Delegate evaluator = null;

        /// <summary>
        /// Returns the computed value of this expression.
        /// </summary>
        /// <param name="record">The object on which the expression is being computed.</param>
        /// <returns>The computed value.</returns>
        public object ComputedValue(object record)
        {
            if (evaluator == null)
            {
                evaluator = record.GetCompiledExpression(expression, out error, helper);
            }
            if (evaluator != null)
            {
                return evaluator.DynamicInvoke(record);
            }
            return null;
        }


        ExpressionError error = ExpressionError.None;
        /// <summary>
        /// Gets the ExpressionError associated with the last computation of this ExpressionObject.
        /// </summary>
        public ExpressionError Error
        {
            get { return error; }
        }
        /// <summary>
        /// Gets the string that reflects the ExpressionError associated with the last computation of this ExpressionObject.
        /// </summary>
        public string ErrorString
        {
            get 
            {
                if (error != ExpressionError.ExceptionRaised)
                {
                    return error.ToString();
                }
                return CalulationExtensions.ErrorString;
            }
        }

        string name;
        /// <summary>
        /// Gets or sets the MappingName of this ExpressionObject.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        string expression = "";

        /// <summary>
        /// Gets or sets the expression of this ExpressionObject.
        /// </summary>
        public string Expression
        {
            get 
            { 
                return expression; 
            }
            set 
            {
                evaluator = null;
                expression = value; 
            }
        }
    }

    #endregion

    #region static extension class that supports the parsing and compiling of expression functions and summary calculations.

    internal static class CalulationExtensions
    {
        const char stringMarker = (char)130;
        const char compiledExpressionMarker = (char)131;
        const char geMarker = (char)132;
        const char leMarker = (char)133;
        const char neMarker = (char)134;
        const char andMarker = (char)135;
        const char orMarker = (char)136;
        const char startsWithMarker = (char)137;
        const char endsWithMarker = (char)138;
        const char containsMarker = (char)139;
        const char unaryMinus = (char)140;
        const char plusMarker = '+';
        const char minusMarker = '-';
        const char multMarker = '*';
        const char divideMarker = '/';
        const char powerMarker = '^';
        const char modMarker = '%';
        const char greaterMarker = '>';
        const char lesserMarker = '<';
        const char equalMarker = '=';
        const char quoteMarker = '"';
        const char leftBracket = '[';
        const char rightBracket = ']';
        const char leftParen = '(';
        const char rightParen = ')';
        static char[] unaryMinusMarkersChars = new char[] { leftParen, lesserMarker, greaterMarker, neMarker, equalMarker, modMarker, powerMarker, divideMarker, multMarker, minusMarker, plusMarker };
        static string unaryMinusMarkers = new string(unaryMinusMarkersChars);
        static Dictionary<string, string> strings = new Dictionary<string, string>();
        static Dictionary<string, System.Linq.Expressions.Expression> expressions = new Dictionary<string, System.Linq.Expressions.Expression>();
        static char[] allOperations = new char[]{  geMarker,
                                                leMarker,
                                                neMarker,
                                                andMarker,
                                                orMarker,
                                                startsWithMarker,
                                                endsWithMarker,
                                                containsMarker,
                                                unaryMinus,
                                                plusMarker,
                                                minusMarker,
                                                multMarker,
                                                divideMarker,
                                                powerMarker,
                                                modMarker,
                                                greaterMarker,
                                                lesserMarker,
                                                equalMarker };
        static char[] unaryOperations = new[]{  unaryMinus};

    /// <summary>
    /// Returns a delegate that computes the summary defined in the passed-in formula.
    /// </summary>
    /// <param name="source">The argument.</param>
    /// <param name="formula">The formula that defines the summary.</param>
    /// <param name="error">Any error encountered.</param>
    /// <param name="helper">The ExpressionHelper object.</param>
    /// <returns>The compiled delegate for this formula.</returns>
        public static Delegate GetCompiledSummary(this IEnumerable source, string formula, out ExpressionError error, ExpressionHelper helper)
        {
            error = ExpressionError.None;
            ErrorString = "";
            var type = source.GetType();
            var paramExp = System.Linq.Expressions.Expression.Parameter(type, type.Name);
            formula = TokenizeStrings(formula, ref error);

            formula = TokenizeUnaryMinus(formula, ref error);

            int loc = formula.IndexOfAny(unaryOperations);
            while (loc > -1 && loc < formula.Length && error == ExpressionError.None)
            {
                int locRightParen;
                if (formula[loc] == unaryMinus)
                {
                    locRightParen = loc + 1;
                }
                else
                {
                    locRightParen = formula.IndexOf(rightParen, loc + 1);
                    if (locRightParen == -1)
                    {
                        error = ExpressionError.MismatchedParentheses;
                    }
                    else
                    {
                        string s = formula.Substring(0, locRightParen);
                        if (locRightParen < formula.Length - 1)
                        {
                            s = s + formula.Substring(locRightParen + 1);
                        }
                        formula = s;
                    }

                }
                loc = (locRightParen + 1) >= formula.Length ? -1 : formula.Substring(locRightParen + 1).IndexOfAny(unaryOperations);
                if (loc > -1)
                {
                    loc += locRightParen + 1;
                }
            }

            loc = formula.IndexOf(rightParen);
            while (loc > -1 && loc < formula.Length && error == ExpressionError.None)
            {
                int start = formula.Substring(0, loc).LastIndexOf(leftParen);
                if (start == -1)
                {
                    error = ExpressionError.MismatchedParentheses;
                }
                else
                {
                    if (!CheckAndProcessSummary(ref formula, start, loc, ref error, paramExp, source, helper) && error == ExpressionError.None)
                    {
                        string piece = formula.Substring(start + 1, loc - start - 1);
                        string token = source.GetSimpleExpression(piece, paramExp, ref error, helper);
                        string s = "";
                        if (start > 0)
                        {
                            s = formula.Substring(0, start);
                        }
                        s += token;
                        if (loc < formula.Length - 1)
                        {
                            s += formula.Substring(loc + 1);
                        }
                        formula = s;
                    }
                }
                loc = formula.IndexOf(rightParen);
            }
            if (error == ExpressionError.None)
            {
                string token = source.GetSimpleExpression(formula, paramExp, ref error, helper);
                if (token == null && error == ExpressionError.None)
                {
                    Expression exp = source.GetExpressionPiece(paramExp, formula, ref error, helper);
                    if (exp != null)
                    {
                        var lambda = System.Linq.Expressions.Expression.Lambda(exp, paramExp);
                        strings.Clear();
                        return lambda.Compile();
                    }
                }

                if (error == ExpressionError.None)
                {
                    var lambda = System.Linq.Expressions.Expression.Lambda(expressions[token], paramExp);
                    strings.Clear();
                    return lambda.Compile();
                }
            }
            if (error == ExpressionError.None)
            {
                error = ExpressionError.NotAValidFormula;
            }
            strings.Clear();
            return null;
        }


        //notes:
        //1) The logical operators And, Or, Not must be sandwiched between blanks, and either all caps, no caps, or first cap only.
        //2) To use column names as And, Or, Not, they must be included in []'s
        //3) For any other column name, the brackets are optional.
        //handles comparing a property to a constant
        /// <summary>
        /// Gets a compiled function expression defined in the passed-in formula.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="formula"></param>
        /// <param name="error"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static Delegate GetCompiledExpression(this object source, string formula, out ExpressionError error, ExpressionHelper helper)
        {
            error = ExpressionError.None;
            ErrorString = "";
            var type = source.GetType();
            var paramExp = System.Linq.Expressions.Expression.Parameter(type, type.Name);
            formula = TokenizeStrings(formula, ref error);

            formula = TokenizeUnaryMinus(formula, ref error);

            StringBuilder sb = new StringBuilder(formula);
            sb = sb.Replace(">=", geMarker.ToString()).Replace("<=", leMarker.ToString()).Replace("<>", neMarker.ToString())
                 .Replace(" AND ", andMarker.ToString()).Replace(" And ", andMarker.ToString()).Replace(" and ", andMarker.ToString())
                 .Replace(" OR ", orMarker.ToString()).Replace(" Or ", orMarker.ToString()).Replace(" or ", orMarker.ToString())
                 .Replace(" STARTSWITH ", startsWithMarker.ToString()).Replace(" StartsWith ", startsWithMarker.ToString()).Replace(" startswith ", startsWithMarker.ToString())
                 .Replace(" ENDSWITH ", endsWithMarker.ToString()).Replace(" EndsWith ", endsWithMarker.ToString()).Replace(" endswith ", endsWithMarker.ToString())
                 .Replace(" CONTAINS ", containsMarker.ToString()).Replace(" Contains ", containsMarker.ToString()).Replace(" contains ", containsMarker.ToString())
                 .Replace(leftBracket.ToString(), "").Replace(rightBracket.ToString(), "");
             
            formula = sb.ToString();
            int loc = formula.IndexOfAny(unaryOperations);
            while (loc > -1 && loc < formula.Length && error == ExpressionError.None)
            {
                int locRightParen;
                if (formula[loc] == unaryMinus)
                {
                    locRightParen = loc + 1;
                }
                else
                {
                    locRightParen = formula.IndexOf(rightParen, loc + 1);
                    if (locRightParen == -1)
                    {
                        error = ExpressionError.MismatchedParentheses;
                    }
                    else
                    {
                        string s = formula.Substring(0, locRightParen);
                        if (locRightParen < formula.Length - 1)
                        {
                            s = s + formula.Substring(locRightParen + 1);
                        }
                        formula = s;
                    }

                }
                loc = (locRightParen + 1) >= formula.Length ? -1 : formula.Substring(locRightParen + 1).IndexOfAny(unaryOperations);
                if (loc > -1)
                {
                    loc += locRightParen + 1;
                }
            }

            loc = formula.IndexOf(rightParen);
            while (loc > -1 && loc < formula.Length && error == ExpressionError.None)
            {
                int start = formula.Substring(0, loc).LastIndexOf(leftParen);
                if (start == -1)
                {
                    error = ExpressionError.MismatchedParentheses;
                }
                else
                {
                    if (!CheckAndProcessFunction(ref formula, start, loc, ref error, paramExp, source, helper) && error == ExpressionError.None)
                    {
                        string piece = formula.Substring(start + 1, loc - start - 1);
                        string token = source.GetSimpleExpression(piece, paramExp, ref error, helper);
                        string s = "";
                        if (start > 0)
                        {
                            s = formula.Substring(0, start);
                        }
                        s += token;
                        if (loc < formula.Length - 1)
                        {
                            s += formula.Substring(loc + 1);
                        }
                        formula = s;
                    }
                }
                loc = formula.IndexOf(rightParen);
            }
            if (error == ExpressionError.None)
            {
                string token = source.GetSimpleExpression(formula, paramExp, ref error, helper);
                if (token == null && error == ExpressionError.None)
                {
                    Expression exp = source.GetExpressionPiece(paramExp, formula, ref error, helper);
                    if (exp != null)
                    {
                        var lambda = System.Linq.Expressions.Expression.Lambda(exp, paramExp);
                        strings.Clear();
                        return lambda.Compile();
                    }
                }
                
                if (error == ExpressionError.None)
                {
                    var lambda = System.Linq.Expressions.Expression.Lambda(expressions[token], paramExp);
                    strings.Clear();
                    return lambda.Compile();
                }
            }
            if (error == ExpressionError.None)
            {
                error = ExpressionError.NotAValidFormula;
            }
            strings.Clear();
            return null;
        }

        /// <summary>
        /// Replaces unary minus signs with a token marker.
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static string TokenizeUnaryMinus(string formula, ref ExpressionError error)
        {
            int loc = formula.IndexOf(minusMarker);
            int i;
            while (loc > -1 && loc < formula.Length)
            {
                if (loc == 0 || 
                    (i = unaryMinusMarkers.IndexOf(formula[loc - 1])) > -1)
                {
                    string s = formula.Substring(0, loc) + unaryMinus;
                    if (loc + 1 < formula.Length)
                        s += formula.Substring(loc + 1);
                    formula = s; 
                }
                loc = formula.IndexOf(minusMarker, loc + 1);
            }
            return formula;
        }

        /// <summary>
        /// Accepts a list of record objects and returns a list of specific property values from the objects.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="pd"></param>
        /// <returns></returns>
        private static IEnumerable GetPropertyValues(IEnumerable list, PropertyDescriptor pd)
        {
            List<object> a = new List<Object>();
            foreach(object o in list)
            {
                a.Add(pd.GetValue(o));
            }
            return a;
        }

        /// <summary>
        /// Accepts a list of record objects and returns a list of specific computed values on the record objects where the formula is defined in a string.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="formula"></param>
        /// <param name="pExp"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        private static IEnumerable GetComputedValues(IEnumerable list, string formula, ParameterExpression pExp, ExpressionHelper helper)
        {
            Delegate evaluator = null;

            List<object> a = new List<Object>();
            foreach (object o in list)
            {
                if (evaluator == null)
                {
                    ExpressionError error = ExpressionError.None;
                    evaluator = o.GetCompiledExpression(formula, out error, helper);
                    if (error != ExpressionError.None)
                    {
                        return a;
                    }
                }
                try
                {
                    a.Add(evaluator.DynamicInvoke(o));
                }
                catch (Exception ex)
                {
                    errorString = ex.Message;
                }
            }
            return a;
        }

        /// <summary>
        /// Accepts a list of record objects and returns a list of specific computed values on the record objects where the formula is defined in an Expression.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="exp"></param>
        /// <param name="pExp"></param>
        /// <returns></returns>
        private static IEnumerable GetComputedValues(IEnumerable list, Expression exp, ParameterExpression pExp)
        {
            List<object> a = new List<Object>();
            try
            {
                Delegate evaluator = System.Linq.Expressions.Expression.Lambda(exp, pExp).Compile();
                foreach (object o in list)
                {
                    a.Add(evaluator.DynamicInvoke(o));
                }
            }
            catch (Exception)
            {

            }
            return a;
        }

        /// <summary>
        /// Accepts a list and returns a list of Expression values where the expression if defined by an ExpressionHelper.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="expressionName">Name of the expression.</param>
        /// <param name="helper">The helper.</param>
        /// <returns></returns>
        private static IEnumerable GetUnboundValues(IEnumerable list, string expressionName, ExpressionHelper helper)
        {
            List<object> a = new List<Object>();
            if (helper.IsExpressionName(expressionName))
            {
                foreach (object o in list)
                {
                    a.Add(ExpressionHelper.GetComputedValue(helper, expressionName, o));
                }
            }
            return a;
        }

        /// <summary>
        /// Determines whether a set of parentheses are associated with a Summary calculation and parses it if it is.
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="start"></param>
        /// <param name="loc"></param>
        /// <param name="error"></param>
        /// <param name="paramExp"></param>
        /// <param name="source"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        private static bool CheckAndProcessSummary(ref string formula, int start, int loc, ref ExpressionError error, ParameterExpression paramExp, IEnumerable source, ExpressionHelper helper)
        {
            bool handled = false;
            if (start > 0)
            {
                int i = start - 1;
                while (i > -1 && char.IsLetterOrDigit(formula[i]))
                    i--;
                if (i != start - 1)
                {
                    string name = formula.Substring(i + 1, start - i - 1);

                    if (name.Length > 0 && helper.SummaryLibrary.ContainsKey(name.ToUpper()))
                    {
                        string arg = formula.Substring(start + 1, loc - start - 1);

                        string[] parameterStrings = GetParameters(arg);
                       // int numberParameters = parameterStrings.GetLength(0);
                        string key = "_1";
                        if (!helper.SummaryLibrary[name.ToUpper()].LambdaExpressions.ContainsKey(key))
                        {
                            error = ExpressionError.InvalidNumberOfFunctionArguments;
                            return handled;
                        }

                        LambdaExpression function = helper.SummaryLibrary[name.ToUpper()].LambdaExpressions[key];

                        Expression exp = null;
                        var type = source.AsQueryable().ElementType;
                        PropertyDescriptor pd = TypeDescriptor.GetProperties(type)[arg];
                        List<Expression> parameters = new List<Expression>();
                        if (pd != null)
                        {
                            exp = (System.Linq.Expressions.Expression<Func<IEnumerable, IEnumerable>>)((list) =>
                                    GetPropertyValues(list, pd));
                        }
                        else //unbound column or formula
                        {
                           if(helper.IsExpressionName(arg))
                            {
                                exp = (System.Linq.Expressions.Expression<Func<IEnumerable, IEnumerable>>)((list) =>
                                        GetUnboundValues(list, arg, helper));
                            }
                            else  //formula
                            {
                                ParameterExpression paramObjectExp = System.Linq.Expressions.Expression.Parameter(type, type.Name);
                       
                                if (arg.IndexOf(compiledExpressionMarker) == 0 && expressions.ContainsKey(arg))
                                {
                                    exp = expressions[arg];
                                    exp = (System.Linq.Expressions.Expression<Func<IEnumerable, IEnumerable>>)((list) =>
                                       GetComputedValues(list, exp, paramObjectExp));
                                }
                                else
                                {
                                    exp = (System.Linq.Expressions.Expression<Func<IEnumerable, IEnumerable>>)((list) =>
                                       GetComputedValues(list, arg, paramObjectExp, helper));
                               }
                            }
                            
                        }
                        exp = System.Linq.Expressions.Expression.Invoke(
                                   exp, paramExp);
                        parameters.Add(exp);
                        exp = System.Linq.Expressions.Expression.Invoke(
                                    function, parameters);
                       
                        key = compiledExpressionMarker + expressions.Count.ToString() + compiledExpressionMarker;
                        expressions.Add(key, exp);

                        string s = "";
                        if (i > -1)
                            s += formula.Substring(0, i + 1);
                        s += key;
                        if (loc < formula.Length - 1)
                            s += formula.Substring(loc + 1);
                        formula = s;
                        handled = true;
                    }
                }
            }

            return handled;
        }

        /// <summary>
        /// Checks if a set of parenthesis is part of a fumction call, and parses it if needed.
        /// </summary>
        /// <param name="formula">String containing the formula.</param>
        /// <param name="start">Index of the left parenthesis in the formula string.</param>
        /// <param name="loc">Index of the right parenthesis in the formula string.</param>
        /// <param name="error">Returns an error code if needed.</param>
        /// <param name="paramExp">An expression representing the argument of the extension.</param>
        /// <param name="source">The parameter expression.</param>
        /// <param name="helper">The helper.</param>
        /// <returns>
        /// Returns true if a function call has been recognized and processed.
        /// </returns>
        private static bool CheckAndProcessFunction(ref string formula, int start, int loc, ref ExpressionError error, ParameterExpression paramExp, object source, ExpressionHelper helper)
        {
            bool handled = false;
            if (start > 0)
            {
                int i = start - 1;
                while (i > -1 && char.IsLetterOrDigit(formula[i]))
                    i--;
                if (i != start - 1)
                {
                    string name = formula.Substring(i + 1, start - i - 1);

                    if (name.Length > 0 && helper.FunctionLibrary.ContainsKey(name.ToUpper()))
                    {
                        string arg = formula.Substring(start + 1, loc - start - 1);

                        string[] parameterStrings = GetParameters(arg);
                        int numberParameters = parameterStrings.GetLength(0);

                        if (!helper.FunctionLibrary[name.ToUpper()].LambdaExpressions.ContainsKey(numberParameters))
                        {
                            error = ExpressionError.InvalidNumberOfFunctionArguments;
                            return handled;
                        }

                        LambdaExpression function = helper.FunctionLibrary[name.ToUpper()].LambdaExpressions[numberParameters];
                        Expression[] parameters = new Expression[numberParameters];
                        Expression exp = null;
                        for (int argNumber = 0; argNumber < numberParameters; ++argNumber)
                        {

                            exp = null;
                            arg = parameterStrings[argNumber];
                            
                            if (arg.IndexOf(compiledExpressionMarker) > -1)
                            {
                                if (expressions.ContainsKey(arg))
                                {
                                    exp = expressions[arg];
                                }
                                else
                                {
                                    string token = source.GetSimpleExpression(arg, paramExp, ref error, helper);

                                    if (token == null && error == ExpressionError.None)
                                    {
                                        exp = source.GetExpressionPiece(paramExp, arg, ref error, helper);
                                    }
                                    else if (token != null && error == ExpressionError.None)
                                    {
                                        exp = expressions[token];
                                    }
                                }
                            }
                            else
                            {
                                string token = source.GetSimpleExpression(arg, paramExp, ref error, helper);

                                if (token == null && error == ExpressionError.None)
                                {
                                    exp = source.GetExpressionPiece(paramExp, arg, ref error, helper);
                                }
                                else if (token != null && error == ExpressionError.None)
                                {
                                    exp = expressions[token];
                                }
                            }

                            if (exp.Type != function.Parameters[argNumber].Type)
                            {
                                TypeConverter typeConverter = TypeDescriptor.GetConverter(exp.Type);
                                if (typeConverter != null && typeConverter.CanConvertTo(function.Parameters[argNumber].Type))
                                {
                                    exp = System.Linq.Expressions.Expression.Convert(exp, function.Parameters[argNumber].Type);
                                }
                                else
                                {
                                    error = ExpressionError.ArgumentTypeMismatch;
                                    return false;
                                }
                            }
                            parameters[argNumber] = exp;
                        }

                        exp = System.Linq.Expressions.Expression.Invoke(
                                    function, parameters);

                        string key = compiledExpressionMarker + expressions.Count.ToString() + compiledExpressionMarker;
                        expressions.Add(key, exp);

                        string s = "";
                        if (i > -1)
                            s += formula.Substring(0, i + 1);
                        s += key;
                        if (loc < formula.Length - 1)
                            s += formula.Substring(loc + 1);
                        formula = s;
                        handled = true;
                    }
                }
            }

            return handled;
        }

        /// <summary>
        /// Returns a collection of parameter objects.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static string[] GetParameters(string arg)
        {
            List<string> list = new List<string>();

            int loc = 0;
            int quoteCount = 0;
            string s = "";
            while (loc < arg.Length)
            {
                if (arg[loc] == ExpressionHelper.ListSeparator && quoteCount == 0)
                {
                    list.Add(s);
                    s = "";
                    loc++;
                }
                else
                {
                    if (arg[loc] == ExpressionHelper.QuoteMark)
                        quoteCount = (quoteCount + 1) % 2;
                    s += arg[loc];
                    loc++;
                }
            }
            if(s.Length > 0)
                list.Add(s.Trim());
            return list.ToArray();
        }

        /// <summary>
        /// Parses a simple expression which is one without parentheses. Handles calculation operator precedence by successive internal calls for particular set of operatord.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="formula"></param>
        /// <param name="paramExp"></param>
        /// <param name="error"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        private static string GetSimpleExpression(this object source, string formula, ParameterExpression paramExp, ref ExpressionError error, ExpressionHelper helper)
        {
            var type = source.GetType();
            bool finished = source.CompileToExpression(paramExp, ref formula, new char[] { startsWithMarker, endsWithMarker, containsMarker, powerMarker, unaryMinus }, allOperations, out error, helper);
            if (!finished && error == ExpressionError.None)
            {
                finished = source.CompileToExpression(paramExp, ref formula, new char[] { multMarker, divideMarker }, allOperations, out error, helper);
                if (!finished && error == ExpressionError.None)
                {
                    if (!finished && error == ExpressionError.None)
                    {
                        finished = source.CompileToExpression(paramExp, ref formula, new char[] { plusMarker, minusMarker }, allOperations, out error, helper);
                        if (!finished && error == ExpressionError.None)
                        {
                            finished = source.CompileToExpression(paramExp, ref formula, new char[] { modMarker }, allOperations, out error, helper);
                            if (!finished && error == ExpressionError.None)
                            {
                                finished = source.CompileToExpression(paramExp, ref formula, new char[] { geMarker, leMarker, neMarker, lesserMarker, greaterMarker, equalMarker}, allOperations, out error, helper);
                                if (!finished && error == ExpressionError.None)
                                {
                                    finished = source.CompileToExpression(paramExp, ref formula, new char[] { andMarker, orMarker }, allOperations, out error, helper);
                                }
                            }
                        }
                    }
                }
            }
            if (finished && error == ExpressionError.None)
            {
                return formula;
            }
            
            return null;
        }

        /// <summary>
        /// Compiles a simple expression targeting specific operators.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="paramExp"></param>
        /// <param name="formula"></param>
        /// <param name="operations"></param>
        /// <param name="allOperations"></param>
        /// <param name="error"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        private static bool CompileToExpression(this object source, ParameterExpression paramExp, ref string formula, char[] operations, char[] allOperations, out ExpressionError error, ExpressionHelper helper)
        {
            error = ExpressionError.None;
            int loc = 0;
            while (loc > -1 && loc < formula.Length)
            {
                loc = formula.IndexOfAny(operations);
                if (loc > -1)
                {
                    int start = formula.Substring(0, loc).LastIndexOfAny(allOperations);
                    int end = formula.IndexOfAny(allOperations, loc + 1);
                    if (end == -1)
                    {
                        end = formula.Length;
                    }
                    string key = "";
                    if (unaryOperations.Contains(formula[loc]))
                    {
                        string left = "";
                        string right = formula.Substring(loc + 1, end - loc - 1).Trim();
                        System.Linq.Expressions.Expression exp = source.GetExpression(paramExp, left, right, formula[loc], ref error, helper);
                        key = compiledExpressionMarker + expressions.Count.ToString() + compiledExpressionMarker;
                        expressions.Add(key, exp);
                    }
                    else
                    {
                        //need to get left and right...
                        string left = formula.Substring(start + 1, loc - start - 1).Trim();
                        string right = formula.Substring(loc + 1, end - loc - 1).Trim();
                        if (left.Length == 0)
                        {
                            error = ExpressionError.InvalidLeftOperand;
                        }
                        else if (right.Length == 0)
                        {
                            error = ExpressionError.InvalidRightOperand;
                        }
                        else
                        {
                            System.Linq.Expressions.Expression exp = source.GetExpression(paramExp, left, right, formula[loc], ref error, helper);
                            key = compiledExpressionMarker + expressions.Count.ToString() + compiledExpressionMarker;
                            expressions.Add(key, exp);
                        }
                    }
                    string s = "";
                    if (start > 0)
                    {
                        s = formula.Substring(0, start + 1);
                    }
                    s += key;
                    if (end < formula.Length - 1)
                    {
                        s += formula.Substring(end);
                    }
                    formula = s;
                    loc = 0;
                }
            }

            return formula.StartsWith(compiledExpressionMarker.ToString()) && formula.EndsWith(compiledExpressionMarker.ToString()) && formula.IndexOf(compiledExpressionMarker, 1, formula.Length - 2) == -1;
        }

        static string errorString = "";

        internal static string ErrorString
        {
            get { return CalulationExtensions.errorString; }
            set { CalulationExtensions.errorString = value; }
        }

        /// <summary>
        /// Returns an Expression defined for a string holding a well-formed string expression of holding a single binary operator with left and right pieces.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="paramExp"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="operand"></param>
        /// <param name="error"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        private static System.Linq.Expressions.Expression GetExpression(this object source, ParameterExpression paramExp, string left, string right, char operand, ref ExpressionError error, ExpressionHelper helper)
        {
            System.Linq.Expressions.Expression leftExp = source.GetExpressionPiece(paramExp, left, ref error, helper);
            System.Linq.Expressions.Expression rightExp = source.GetExpressionPiece(paramExp, right, ref error, helper);

            if (!unaryOperations.Contains(operand))
            {
                CoerceType(ref leftExp, ref rightExp, ref error);
            }
            System.Linq.Expressions.Expression exp = null;
            if (error == ExpressionError.None)
            {
                try
                {
                    switch (operand)
                    {
                        case geMarker:
                            EnsureNumericExpressions(ref leftExp, ref rightExp);
                            exp = System.Linq.Expressions.Expression.GreaterThanOrEqual(leftExp, rightExp);
                            break;
                        case leMarker:
                            EnsureNumericExpressions(ref leftExp, ref rightExp);
                            exp = System.Linq.Expressions.Expression.LessThanOrEqual(leftExp, rightExp);
                            break;
                        case neMarker:
                            EnsureNumericExpressions(ref leftExp, ref rightExp);
                            exp = System.Linq.Expressions.Expression.NotEqual(leftExp, rightExp);
                            break;
                        case andMarker:
                            exp = System.Linq.Expressions.Expression.And(leftExp, rightExp);
                            break;
                        case orMarker:
                            exp = System.Linq.Expressions.Expression.Or(leftExp, rightExp);
                            break;
                        case startsWithMarker:
                        case endsWithMarker:
                        case containsMarker:
                            {
                                string funcName = GetFunctionName(operand);
                                if (funcName.Length > 0)
                                {
                                    var stringMethod = typeof(string).GetMethods().Where(m => m.Name == funcName).FirstOrDefault();
                                    var targetType = typeof(string);
                                    if (leftExp.Type != targetType)
                                    {
                                        leftExp = Expression.Call(
                                            leftExp,
                                            leftExp.Type.GetMethods().Where(m => m.Name == "ToString").FirstOrDefault(),
                                            null);
                                    }
                                    if (rightExp.Type != targetType)
                                    {
                                        rightExp = Expression.Call(
                                            rightExp,
                                            rightExp.Type.GetMethods().Where(m => m.Name == "ToString").FirstOrDefault(),
                                            null);
                                    }

                                    if (leftExp.Type == targetType)//typeof(string))
                                    {
                                        exp = Expression.Call(
                                            leftExp,
                                            stringMethod,
                                            new Expression[] { rightExp });
                                    }
                                    else
                                    {
                                        error = ExpressionError.ArgumentTypeMismatch;
                                        //throw new InvalidOperationException("Underlying type is not a string");
                                    }
                                }
                            }
                           break;
                        case unaryMinus:
                           EnsureNumericExpressions(ref leftExp, ref rightExp);
                           exp = System.Linq.Expressions.Expression.Negate(rightExp);
                           break;
                        case plusMarker:
                           EnsureNumericExpressions(ref leftExp, ref rightExp);
                            exp = System.Linq.Expressions.Expression.Add(leftExp, rightExp);
                            break;
                        case minusMarker:
                            EnsureNumericExpressions(ref leftExp, ref rightExp);
                            exp = System.Linq.Expressions.Expression.Subtract(leftExp, rightExp);
                            break;
                        case multMarker:
                            EnsureNumericExpressions(ref leftExp, ref rightExp);
                            exp = System.Linq.Expressions.Expression.Multiply(leftExp, rightExp);
                            break;
                        case divideMarker:
                            EnsureNumericExpressions(ref leftExp, ref rightExp);
                            exp = System.Linq.Expressions.Expression.Divide(leftExp, rightExp);
                            break;
                        case powerMarker:
                            EnsureNumericExpressions(ref leftExp, ref rightExp);
                            exp = System.Linq.Expressions.Expression.Power(leftExp, rightExp);
                            break;
                        case modMarker:
                            EnsureNumericExpressions(ref leftExp, ref rightExp);
                            exp = System.Linq.Expressions.Expression.Modulo(leftExp, rightExp);
                            break;
                        case greaterMarker:
                            EnsureNumericExpressions(ref leftExp, ref rightExp);
                            exp = System.Linq.Expressions.Expression.GreaterThan(leftExp, rightExp);
                            break;
                        case lesserMarker:
                            EnsureNumericExpressions(ref leftExp, ref rightExp);
                            exp = System.Linq.Expressions.Expression.LessThan(leftExp, rightExp);
                            break;
                        case equalMarker:
                            EnsureNumericExpressions(ref leftExp, ref rightExp);
                            exp = System.Linq.Expressions.Expression.Equal(leftExp, rightExp);
                            break;
                        default:
                            error = ExpressionError.UnknownOperator;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    error = ExpressionError.ExceptionRaised;
                    ErrorString = ex.Message;
                }
            }
            return exp;
        }

        /// <summary>
        /// Used to coerce argument types to numeric values.
        /// </summary>
        /// <param name="leftExp"></param>
        /// <param name="rightExp"></param>
        private static void EnsureNumericExpressions(ref Expression leftExp, ref Expression rightExp)
        {
            if (leftExp.Type == typeof(object) || leftExp.Type == typeof(string))
            {
                leftExp = CovertObjectToDouble(leftExp);
            }
            if (rightExp.Type == typeof(object) || rightExp.Type == typeof(string))
            {
                rightExp = CovertObjectToDouble(rightExp);
            }
        }

        /// <summary>
        /// Matches a token marker with a proper function name.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static string GetFunctionName(char c)
        {
            string s = "";
            switch (c)
            {
                case startsWithMarker:
                    s = "StartsWith";
                    break;
                case endsWithMarker:
                    s = "EndsWith";
                    break;
                case containsMarker:
                    s = "Contains";
                    break;
                default:
                    break;
            }
            return s;
        }

        /// <summary>
        /// Converts an Expression to an Expression of a double type.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private static Expression CovertObjectToDouble(Expression exp)
        {
                var method = typeof(double).GetMethods().Where(m => m.Name == "Parse").FirstOrDefault();
                var targetType = typeof(double);
                exp = Expression.Call(
                        exp,
                        exp.Type.GetMethods().Where(m => m.Name == "ToString").FirstOrDefault(),
                        null);


                exp = Expression.Call(method, new Expression[] { exp });
             
            return exp;
        }

        /// <summary>
        /// Forces type matches.
        /// </summary>
        /// <param name="leftExp"></param>
        /// <param name="rightExp"></param>
        /// <param name="error"></param>
        private static void CoerceType(ref System.Linq.Expressions.Expression leftExp, ref System.Linq.Expressions.Expression rightExp, ref ExpressionError error)
        {
            if (leftExp.Type != rightExp.Type)
            {
               if (leftExp.Type == typeof(double) &&
                    (rightExp.Type == typeof(int) || rightExp.Type == typeof(float)))
                {
                    rightExp = System.Linq.Expressions.Expression.Convert(rightExp, typeof(double));
                }
                else if (leftExp.Type == typeof(double) && rightExp.Type == typeof(object))
                {
                    rightExp = CovertObjectToDouble(rightExp);
                }
                else if (rightExp.Type == typeof(double) &&
                    (leftExp.Type == typeof(int) || leftExp.Type == typeof(float)))
                {
                    leftExp = System.Linq.Expressions.Expression.Convert(leftExp, typeof(double));
                }
                else if (rightExp.Type == typeof(double) && leftExp.Type == typeof(object))
                {
                    leftExp = CovertObjectToDouble(leftExp);
                }
                else //different types...
                {
                    try
                    {
                        bool setError = true;
                        TypeConverter typeConverter = TypeDescriptor.GetConverter(leftExp.Type);
                        if (typeConverter != null && typeConverter.CanConvertTo(rightExp.Type))
                        {
                            leftExp = System.Linq.Expressions.Expression.Convert(leftExp, rightExp.Type);
                            setError = false;
                        }
                        else
                        {
                            typeConverter = TypeDescriptor.GetConverter(rightExp.Type);
                            if (typeConverter != null && typeConverter.CanConvertTo(leftExp.Type))
                            {
                                rightExp = System.Linq.Expressions.Expression.Convert(rightExp, leftExp.Type);
                                setError = false;
                            }
                        }
                        if (setError)
                        {
                            if (rightExp.Type.ToString().IndexOf("Func") == -1 &&
                                leftExp.Type.ToString().IndexOf("Func") == -1)
                            {
                                error = ExpressionError.CannotCompareDifferentTypes;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        error = ExpressionError.ExceptionRaised;
                        ErrorString = ex.Message;
                    }
                }
            }
        }

        //pass in a string containing a left or right part, and get an Expression back.
        /// <summary>
        /// Returns an Exression for a string formula that has no operator.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="paramExp"></param>
        /// <param name="piece"></param>
        /// <param name="error"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        private static System.Linq.Expressions.Expression GetExpressionPiece(this object source, ParameterExpression paramExp, string piece, ref ExpressionError error, ExpressionHelper helper)
        {
            if (piece.StartsWith(compiledExpressionMarker.ToString()) && expressions.ContainsKey(piece))
            {
                return expressions[piece];
            }

            var type = source.GetType();
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(type);
            if (!helper.CaseSensitive && pdc[piece] == null)
            {
                string s = piece.ToLower();
                foreach (PropertyDescriptor pd in pdc)
                {
                    if (s == pd.Name.ToLower())
                    {
                        piece = pd.Name;
                        break;
                    }
                }
            }
           
            if (pdc[piece] != null)
            {
                return System.Linq.Expressions.Expression.PropertyOrField(paramExp, piece);
            }
            else if (helper.IsExpressionName(piece))
            {
                Expression body = Expression.Call(typeof(ExpressionHelper).GetMethod("GetComputedValue"), new Expression[] { Expression.Constant(helper), Expression.Constant(piece), paramExp });
                //Expression e = Expression.Lambda(body);
                return body;
            }
            double d = 0;
            if (double.TryParse(piece, out d))
            {
                return System.Linq.Expressions.Expression.Constant(d);
            }
            int loc = piece.IndexOf(stringMarker);
            while (loc > -1)
            {
                int end = piece.IndexOf(stringMarker, loc + 1);
                string key = piece.Substring(loc, end - loc + 1);
                piece = piece.Replace(key, strings[key]);
                loc = piece.IndexOf(stringMarker);
            }
            return System.Linq.Expressions.Expression.Constant(piece);
        }

        /// <summary>
        /// Replaces strings in formulas with tokens so string remain immutable during parsing.
        /// </summary>
        /// <param name="formula">The formula with quoted strings.</param>
        /// <param name="error">Error code if any.</param>
        /// <returns>
        /// A string that represnts the formula with quoted strings replaced by tokens.
        /// </returns>
        private static string TokenizeStrings(string formula, ref ExpressionError error)
        {
            error = ExpressionError.None;
            int loc = 0;
            int count = 0;
            strings.Clear();
            StringBuilder sb = new StringBuilder();
            while (loc < formula.Length && loc > -1)
            {
                int startLoc = loc;
                loc = formula.IndexOf(quoteMarker, loc);
                if (loc > -1)
                {
                    sb.Append(formula.Substring(startLoc, loc - startLoc));
                    int nextLoc = formula.IndexOf(quoteMarker, loc + 1);
                    if (nextLoc == -1)
                    {
                        error = ExpressionError.MissingRightQuote;
                        break;
                    }
                    string key = stringMarker + count.ToString() + stringMarker;
                    count++;
                    strings.Add(key, formula.Substring(loc + 1, nextLoc - loc - 1));
                    sb.Append(key);
                    loc = nextLoc + 1;
                }
                else
                {
                    sb.Append(formula.Substring(startLoc));
                }
            }
            return sb.ToString();
        }
    }
    #endregion

    #region wrapper class for Summary information

    internal class SummarySignature
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SummarySignature"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="exp">The exp.</param>
        public SummarySignature(string name, LambdaExpression exp)
        {
            this.Name = name;
            AddExpression(exp);
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }


        private Dictionary<string, LambdaExpression> expressions;
        /// <summary>
        /// Gets the lambda expressions.
        /// </summary>
        /// <value>The lambda expressions.</value>
        public Dictionary<string, LambdaExpression> LambdaExpressions
        {
            get
            {
                if (expressions == null)
                    expressions = new Dictionary<string, LambdaExpression>();
                return expressions;
            }
        }

        /// <summary>
        /// Adds the expression.
        /// </summary>
        /// <param name="exp">The exp.</param>
        public void AddExpression(LambdaExpression exp)
        {
            string s = GetSignatureString(exp);
            if (LambdaExpressions.ContainsKey(s))
            {
                LambdaExpressions[s] = exp;
            }
            else
            {
                LambdaExpressions.Add(s, exp);
            }
        }

        /// <summary>
        /// Gets the signature string.
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns></returns>
        public static string GetSignatureString(LambdaExpression exp)
        {
            return "_1"; //ignore signatures for time being...
            //string s = "";
            //foreach (ParameterExpression pExp in exp.Parameters)
            //{
            //    if (pExp.Type.IsGenericType)
            //    {
            //        foreach (Type t in pExp.Type.GetGenericArguments())
            //        {
            //            s += "_" + t.Name;
            //        }
            //    }
            //    else
            //    {
            //        s += "_" + pExp.Type.Name;
            //    }
            //}
            //return s.ToUpper();
             
        }

        /// <summary>
        /// Removes the expression.
        /// </summary>
        /// <param name="exp">The exp.</param>
        public void RemoveExpression(LambdaExpression exp)
        {
            string s = GetSignatureString(exp);
            if (LambdaExpressions.ContainsKey(s))
            {
                LambdaExpressions.Remove(s);
            }
        }

    }
    #endregion

    #region wrapper class for expression functions

    internal class FunctionSignature
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionSignature"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="exp">The exp.</param>
        public FunctionSignature(string name,  LambdaExpression exp)
        {
            this.Name = name;
            AddExpression(exp);
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }
        private Dictionary<int, LambdaExpression> expressions;

        /// <summary>
        /// Gets the lambda expressions.
        /// </summary>
        /// <value>The lambda expressions.</value>
        public Dictionary<int, LambdaExpression> LambdaExpressions
        {
            get
            {
                if (expressions == null)
                    expressions = new Dictionary<int, LambdaExpression>();
                return expressions;
            }
        }

        /// <summary>
        /// Adds the expression.
        /// </summary>
        /// <param name="exp">The exp.</param>
        public void AddExpression(LambdaExpression exp)
        {
            if (LambdaExpressions.ContainsKey(exp.Parameters.Count))
            {
                LambdaExpressions[exp.Parameters.Count] = exp;
            }
            else
            {
                LambdaExpressions.Add(exp.Parameters.Count, exp);
            }
        }

        /// <summary>
        /// Removes the expression.
        /// </summary>
        /// <param name="exp">The exp.</param>
        public void RemoveExpression(LambdaExpression exp)
        {
            if (LambdaExpressions.ContainsKey(exp.Parameters.Count))
            {
                LambdaExpressions.Remove(exp.Parameters.Count);            
            }
        }
    }

    #endregion

    /// <summary>
    /// Represents the expression error enumeration constants.
    /// </summary>
    public enum ExpressionError
    {
        /// <summary>
        /// Represents none.
        /// </summary>
        None,
        /// <summary>
        /// Represents the missing right quote mark.
        /// </summary>
        MissingRightQuote,
        /// <summary>
        /// Represents the mismatched parenthesis.
        /// </summary>
        MismatchedParentheses,
        /// <summary>
        /// Represents the different type as cannot compare.
        /// </summary>
        CannotCompareDifferentTypes,
        /// <summary>
        /// Represents unknown error.
        /// </summary>
        UnknownOperator,
        /// <summary>
        /// Represents invalid formula.
        /// </summary>
        NotAValidFormula,
        /// <summary>
        /// Represents whether any exception raised.
        /// </summary>
        ExceptionRaised,
        /// <summary>
        /// Represents improper character preceding left parentheses.
        /// </summary>
        ImproperCharacterPrecedingLeftParentheses,
        /// <summary>
        /// Represents unknown function.
        /// </summary>
        UnknownFunction,
        /// <summary>
        /// Represents the invalid number of function arguments.
        /// </summary>
        InvalidNumberOfFunctionArguments,
        /// <summary>
        /// Represents the argument type mismatch.
        /// </summary>
        ArgumentTypeMismatch,
        /// <summary>
        /// Represents the invalid left operand.
        /// </summary>
        InvalidLeftOperand,
        /// <summary>
        /// Represents the invalid right operand.
        /// </summary>
        InvalidRightOperand,
        /// <summary>
        /// Represents the unknown expression name.
        /// </summary>
        UnknownExpressionName,
        /// <summary>
        /// Represents the invalid summary argument.
        /// </summary>
        InvalidSummaryArgument
    }
   
}
