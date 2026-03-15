#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using Syncfusion.Linq;
#if !SILVERLIGHT
    using Syncfusion.Windows.Shared;
#endif
    using Syncfusion.Windows.ComponentModel;
    using Syncfusion.Windows.Data;
    using System.Linq.Expressions;
    using System.Xml.Serialization;
#if SILVERLIGHT
    using System.Reflection;
#endif

    public class GridDataUnboundVisibleColumn : GridDataVisibleColumn
    {
        public GridDataUnboundVisibleColumn()
        {
            this.IsUnbound = true;
            this.AllowFilter = false;
        }

        public override void InitializeFrom(GridDataVisibleColumn other)
        {
            base.InitializeFrom(other);
            var otherUnboundColumn = other as GridDataUnboundVisibleColumn;
            if (otherUnboundColumn != null)
            {
                this.Format = otherUnboundColumn.Format;
                this.Expression = otherUnboundColumn.Expression;
                this.CaseSensitive = otherUnboundColumn.CaseSensitive;
            }
        }

        #region UnBoundFunc
        /// <summary>
        /// To store the unbound function of appropriate column
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        internal Func<string, object, object> UnboundFunc = null;

        #endregion

        #region Format property
        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register(
            "Format",
            typeof(string),
            typeof(GridDataUnboundVisibleColumn),
            new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the format for the Unbound column.
        /// </summary>
        /// <remarks>
        /// '{Freight} for {ShipCity}'
        /// <para></para>
        /// <para>This will replace the values from the formatted string with the record
        /// values.</para>
        /// </remarks>
        /// <value>
        /// The format string.
        /// </value>
        [TypeConverter(typeof(GridDataFormatConverter))]
        public string Format
        {
            get
            {
                return (string)this.GetValue(GridDataUnboundVisibleColumn.FormatProperty);
            }

            set
            {
                this.SetValue(GridDataUnboundVisibleColumn.FormatProperty, value);
            }
        }
        #endregion

        #region Expression property
        public static readonly DependencyProperty ExpressionProperty = DependencyProperty.Register(
            "Expression",
            typeof(string),
            typeof(GridDataUnboundVisibleColumn),
            new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets an expression used to define the value displayed in the Unbound column.
        /// </summary>
        /// <remarks>
        /// 'Freight / Price * 100'
        /// <para></para>
        /// <para>This expression displays the percentage computed by dividing the property Freight 
        /// by the property price.</para>
        /// </remarks>
        /// <value>
        /// The expression string.
        /// </value>
        public string Expression
        {
            get
            {
                return (string)this.GetValue(GridDataUnboundVisibleColumn.ExpressionProperty);
            }

            set
            {
                this.SetValue(GridDataUnboundVisibleColumn.ExpressionProperty, value);
            }
        }
        #endregion

        #region CaseSensitive property
        public static readonly DependencyProperty CaseSensitiveProperty = DependencyProperty.Register(
            "CaseSensitive",
            typeof(bool),
            typeof(GridDataUnboundVisibleColumn),
            new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets whether case matters in the Property names used in the Expression string.
        /// </summary>
        /// <remarks>
        /// The default value is true.
        /// </remarks>
        /// <value>
        /// Whether case is taken into account in Property names for the Expression string.
        /// </value>
        public bool CaseSensitive
        {
            get
            {
                return (bool)this.GetValue(GridDataUnboundVisibleColumn.CaseSensitiveProperty);
            }

            set
            {
                this.SetValue(GridDataUnboundVisibleColumn.CaseSensitiveProperty, value);
            }
        }
        #endregion

        #region error support

        ExpressionError error = ExpressionError.None;
        internal ExpressionError Error
        {
            get { return error; }
        }
        /// <summary>
        /// Gets the error message, if any, associated with the parsing of the Expression string.
        /// </summary>
        [XmlIgnore]
        public string ErrorString
        {
            get
            {
                if (error != ExpressionError.ExceptionRaised)
                {
                    if (error != ExpressionError.None)
                    {
                        return error.ToString();
                    }
                    return "";
                }
                return CalulationExtensions.ErrorString;
            }
        }
        #endregion

        #region calculation access
        Delegate evaluator = null;
        //private bool needToHook = true;
        internal object ComputedValue(object record)
        {
            if (evaluator == null)
            {
                /*if (needToHook)
                {
                    if (this.TableModel != null)
                    {
                        GridControlBase grid = this.TableModel.Views.First() as GridControlBase;
                        if (grid != null)
                        {
                            //use this to unsubscribe to events
                            //this.TableModel.Disposing += new EventHandler(TableModel_Disposing);
                            needToHook = false;
                        }
                    }
                }*/
                evaluator = record.GetCompiledExpression(CaseSensitive, Expression, out error);
            }
            if (evaluator != null)
            {
                return evaluator.DynamicInvoke(record);
            }
            return null;
        }

        /*void TableModel_Disposing(object sender, EventArgs e)
        {
            GridControlBase grid = this.TableModel.Views.First() as GridControlBase;
            if (grid != null && !needToHook)
            {
                this.TableModel.Disposing -= new EventHandler(TableModel_Disposing);
                needToHook = true;
            }
        }*/
        #endregion
    }

    public enum ExpressionError
    {
        None,
        MissingRightQuote,
        MismatchedParentheses,
        CannotCompareDifferentTypes,
        UnknownOperator,
        NotAValidFormula,
        ExceptionRaised
    }

    #region CalulationExtensions

    public static class CalulationExtensions
    {
        const char stringMarker = (char)130;
        const char compiledExpressionMarker = (char)131;
        const char geMarker = (char)132;
        const char leMarker = (char)133;
        const char neMarker = (char)134;
        const char andMarker = (char)135;
        const char orMarker = (char)136;
        const char notMarker = (char)137;
        const char startsWithMarker = (char)138;
        const char endsWithMarker = (char)139;
        const char containsMarker = (char)140;
        const char dayMarker = (char)141;
        const char weekMarker = (char)142;
        const char monthMarker = (char)143;
        const char quarterMarker = (char)144;
        const char yearMarker = (char)145;
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
        static Dictionary<string, string> strings = new Dictionary<string, string>();
        static Dictionary<string, System.Linq.Expressions.Expression> expressions = new Dictionary<string, System.Linq.Expressions.Expression>();
        static char[] allOperations = new char[]{  geMarker,
                                                leMarker,
                                                neMarker,
                                                andMarker,
                                                orMarker,
                                                notMarker,
                                                startsWithMarker,
                                                endsWithMarker,
                                                containsMarker,
                                                dayMarker,
                                                weekMarker,
                                                monthMarker,
                                                quarterMarker,
                                                yearMarker,
                                                plusMarker,
                                                minusMarker,
                                                multMarker,
                                                divideMarker,
                                                powerMarker,
                                                modMarker,
                                                greaterMarker,
                                                lesserMarker,
                                                equalMarker };
        static char[] unaryOperations = new[]{ dayMarker,
                                                weekMarker,
                                                monthMarker,
                                                quarterMarker,
                                                yearMarker };

        //notes:
        //1) The logical operators And, Or, Not must be sandwiched between blanks, and either all caps, no caps, or first cap only.
        //2) To use column names as And, Or, Not, they must be included in []'s
        //3) For any other column name, the brackets are optional.
        //handles comparing a property to a constant
        internal static Delegate GetCompiledExpression(this object source, bool caseSensitive, string formula, out ExpressionError error)
        {
            error = ExpressionError.None;
            ErrorString = "";
            var type = source.GetType();
            var paramExp = System.Linq.Expressions.Expression.Parameter(type, type.Name);
            formula = TokenizeStrings(formula, ref error);

            StringBuilder sb = new StringBuilder(formula);
            sb = sb.Replace(">=", geMarker.ToString()).Replace("<=", leMarker.ToString()).Replace("<>", neMarker.ToString())
                 .Replace(" AND ", andMarker.ToString()).Replace(" And ", andMarker.ToString()).Replace(" and ", andMarker.ToString())
                 .Replace(" OR ", orMarker.ToString()).Replace(" Or ", orMarker.ToString()).Replace(" or ", orMarker.ToString())
                 .Replace(" NOT", notMarker.ToString()).Replace(" Not", notMarker.ToString()).Replace(" not", notMarker.ToString())
                 .Replace(" STARTSWITH ", startsWithMarker.ToString()).Replace(" StartsWith ", startsWithMarker.ToString()).Replace(" startswith ", startsWithMarker.ToString())
                  .Replace(" ENDSWITH ", endsWithMarker.ToString()).Replace(" EndsWith ", endsWithMarker.ToString()).Replace(" endswith ", endsWithMarker.ToString())
                   .Replace(" CONTAINS ", containsMarker.ToString()).Replace(" Contains ", containsMarker.ToString()).Replace(" contains ", containsMarker.ToString())
                   .Replace("DAY(", dayMarker.ToString()).Replace("Day(", dayMarker.ToString()).Replace("day(", dayMarker.ToString())
                   .Replace("WEEK(", weekMarker.ToString()).Replace("Week(", weekMarker.ToString()).Replace("week(", weekMarker.ToString())
                   .Replace("MONTH(", monthMarker.ToString()).Replace("Month(", monthMarker.ToString()).Replace("month(", monthMarker.ToString())
                   .Replace("QUARTER(", quarterMarker.ToString()).Replace("Quarter(", quarterMarker.ToString()).Replace("quarter(", quarterMarker.ToString())
                   .Replace("YEAR(", yearMarker.ToString()).Replace("Year(", yearMarker.ToString()).Replace("year(", yearMarker.ToString())
                 .Replace("[", String.Empty).Replace("]", String.Empty);

            formula = sb.ToString();
            int loc = formula.IndexOfAny(unaryOperations);
            while (loc > -1 && loc < formula.Length && error == ExpressionError.None)
            {
                int locRightParen = formula.IndexOf(rightParen, loc + 1);
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
                    string piece = formula.Substring(start + 1, loc - start - 1);
                    string token = source.GetSimpleExpression(caseSensitive, piece, paramExp, ref error);
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
                loc = formula.IndexOf(rightParen);
            }
            if (error == ExpressionError.None)
            {
                string token = source.GetSimpleExpression(caseSensitive, formula, paramExp, ref error);
                if (token == null && error == ExpressionError.None)
                {
                    System.Linq.Expressions.Expression exp = source.GetExpressionPiece(caseSensitive, paramExp, formula, ref error);
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

        private static string GetSimpleExpression(this object source, bool caseSensitive, string formula, ParameterExpression paramExp, ref ExpressionError error)
        {
            // var type = source.GetType(); Unused local variable
            bool finished = source.CompileToExpression(paramExp, caseSensitive, ref formula, new char[] { startsWithMarker, endsWithMarker, containsMarker, dayMarker, weekMarker, monthMarker, quarterMarker, yearMarker }, allOperations, out error);
            if (!finished && error == ExpressionError.None)
            {
                finished = source.CompileToExpression(paramExp, caseSensitive, ref formula, new char[] { multMarker, divideMarker }, allOperations, out error);
                if (!finished && error == ExpressionError.None)
                {
                    if (!finished && error == ExpressionError.None)
                    {
                        finished = source.CompileToExpression(paramExp, caseSensitive, ref formula, new char[] { plusMarker, minusMarker }, allOperations, out error);
                        if (!finished && error == ExpressionError.None)
                        {
                            finished = source.CompileToExpression(paramExp, caseSensitive, ref formula, new char[] { powerMarker, modMarker }, allOperations, out error);
                            if (!finished && error == ExpressionError.None)
                            {
                                finished = source.CompileToExpression(paramExp, caseSensitive, ref formula, new char[] { geMarker, leMarker, neMarker, lesserMarker, greaterMarker, equalMarker }, allOperations, out error);
                                if (!finished && error == ExpressionError.None)
                                {
                                    finished = source.CompileToExpression(paramExp, caseSensitive, ref formula, new char[] { notMarker }, allOperations, out error);
                                    if (!finished && error == ExpressionError.None)
                                    {
                                        finished = source.CompileToExpression(paramExp, caseSensitive, ref formula, new char[] { andMarker, orMarker }, allOperations, out error);
                                    }
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

        private static bool CompileToExpression(this object source, ParameterExpression paramExp, bool caseSensitive, ref string formula, char[] operations, char[] allOperations, out ExpressionError error)
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
                        System.Linq.Expressions.Expression exp = source.GetExpression(caseSensitive, paramExp, left, right, formula[loc], ref error);
                        key = compiledExpressionMarker + expressions.Count.ToString() + compiledExpressionMarker;
                        expressions.Add(key, exp);
                    }
                    else
                    {
                        //need to get left and right...
                        string left = formula.Substring(start + 1, loc - start - 1).Trim();
                        string right = formula.Substring(loc + 1, end - loc - 1).Trim();
                        System.Linq.Expressions.Expression exp = source.GetExpression(caseSensitive, paramExp, left, right, formula[loc], ref error);
                        key = compiledExpressionMarker + expressions.Count.ToString() + compiledExpressionMarker;
                        expressions.Add(key, exp);
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

        internal static string ErrorString = "";

        private static System.Linq.Expressions.Expression GetExpression(this object source, bool caseSensitive, ParameterExpression paramExp, string left, string right, char operand, ref ExpressionError error)
        {
            System.Linq.Expressions.Expression leftExp = source.GetExpressionPiece(caseSensitive, paramExp, left, ref error);
            System.Linq.Expressions.Expression rightExp = source.GetExpressionPiece(caseSensitive, paramExp, right, ref error);

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
                            exp = System.Linq.Expressions.Expression.GreaterThanOrEqual(leftExp, rightExp);
                            break;
                        case leMarker:
                            exp = System.Linq.Expressions.Expression.LessThanOrEqual(leftExp, rightExp);
                            break;
                        case neMarker:
                            exp = System.Linq.Expressions.Expression.NotEqual(leftExp, rightExp);
                            break;
                        case andMarker:
                            exp = System.Linq.Expressions.Expression.And(leftExp, rightExp);
                            break;
                        case orMarker:
                            exp = System.Linq.Expressions.Expression.Or(leftExp, rightExp);
                            break;
                        case notMarker:
                            exp = System.Linq.Expressions.Expression.Not(leftExp);
                            break;
                        case startsWithMarker:
                        case endsWithMarker:
                        case containsMarker:
                            {
                                string funcName = GetFunctionName(operand);
                                if (funcName.Length > 0)
                                {
                                    var stringMethod = typeof(string).GetMethods().Where(m => m.Name == funcName).FirstOrDefault();
                                    var underlyingType = leftExp.Type;

                                    if (underlyingType == typeof(string))
                                    {
                                        exp = System.Linq.Expressions.Expression.Call(
                                            leftExp,
                                            stringMethod,
                                            new System.Linq.Expressions.Expression[] { rightExp });
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException("Underlying type is not a string");
                                    }
                                }
                            }
                            break;
                        case dayMarker:
                        case weekMarker:
                        case monthMarker:
                        case quarterMarker:
                        case yearMarker:
                            {
                                string funcName = GetFunctionName(operand);
                                if (funcName.Length > 0)
                                {
                                    var dateProperty = typeof(DateTime).GetProperties().Where(m => m.Name == funcName).FirstOrDefault();
                                    var underlyingType = rightExp.Type;

                                    if (underlyingType == typeof(DateTime))
                                    {
                                        exp = System.Linq.Expressions.Expression.Property(rightExp, dateProperty);
                                    }
                                    else if (underlyingType == typeof(DateTime?))
                                    {
                                        var valueProperty = typeof(DateTime?).GetProperties().Where(m => m.Name == "Value").FirstOrDefault();
                                        var e1 = System.Linq.Expressions.Expression.Property(rightExp, valueProperty);
                                        exp = System.Linq.Expressions.Expression.Property(e1, dateProperty);
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException("Underlying type is not a DateTime or DateTime?");
                                    }
                                }
                            }
                            break;
                        case plusMarker:
                            exp = System.Linq.Expressions.Expression.Add(leftExp, rightExp);
                            break;
                        case minusMarker:
                            exp = System.Linq.Expressions.Expression.Subtract(leftExp, rightExp);
                            break;
                        case multMarker:
                            exp = System.Linq.Expressions.Expression.Multiply(leftExp, rightExp);
                            break;
                        case divideMarker:
                            exp = System.Linq.Expressions.Expression.Divide(leftExp, rightExp);
                            break;
                        case powerMarker:
                            exp = System.Linq.Expressions.Expression.Power(leftExp, rightExp);
                            break;
                        case modMarker:
                            exp = System.Linq.Expressions.Expression.Modulo(leftExp, rightExp);
                            break;
                        case greaterMarker:
                            exp = System.Linq.Expressions.Expression.GreaterThan(leftExp, rightExp);
                            break;
                        case lesserMarker:
                            exp = System.Linq.Expressions.Expression.LessThan(leftExp, rightExp);
                            break;
                        case equalMarker:
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

        private static string GetFunctionName(char c)
        {
            string s = string.Empty;
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
                case dayMarker:
                    s = "Day";
                    break;
                case weekMarker:
                    s = "Week";
                    break;
                case monthMarker:
                    s = "Month";
                    break;
                case quarterMarker:
                    s = "Quarter";
                    break;
                case yearMarker:
                    s = "Year";
                    break;
                default:
                    break;
            }
            return s;
        }

        private static void CoerceType(ref System.Linq.Expressions.Expression leftExp, ref System.Linq.Expressions.Expression rightExp, ref ExpressionError error)
        {
            if (leftExp != null && leftExp.Type != rightExp.Type)
            {
                //if (leftExp.NodeType != ExpressionType.Constant &&
                //    rightExp.NodeType == ExpressionType.Constant)
                //{
                //    rightExp = System.Linq.Expressions.Expression.Constant(Convert.ChangeType(rightExp.ToString(), leftExp.Type));
                //}
                //else if (leftExp.NodeType == ExpressionType.Constant &&
                //    rightExp.NodeType != ExpressionType.Constant)
                //{
                //    leftExp = System.Linq.Expressions.Expression.Constant(Convert.ChangeType(leftExp.ToString(), rightExp.Type));
                //}
                //else 
                if (leftExp.Type == typeof(double) &&
                    (rightExp.Type == typeof(int) || rightExp.Type == typeof(float)))
                {
                    rightExp = System.Linq.Expressions.Expression.Convert(rightExp, typeof(double));
                }
                else if (rightExp.Type == typeof(double) &&
                    (leftExp.Type == typeof(int) || leftExp.Type == typeof(float)))
                {
                    leftExp = System.Linq.Expressions.Expression.Convert(leftExp, typeof(double));
                }
#if !SILVERLIGHT
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
                            error = ExpressionError.CannotCompareDifferentTypes;
                        }
                    }
                    catch (Exception ex)
                    {
                        error = ExpressionError.ExceptionRaised;
                        ErrorString = ex.Message;
                    }
                }
#endif
            }
        }

        //pass in a string containing a left or right part, and get an Expression back.
        private static System.Linq.Expressions.Expression GetExpressionPiece(this object source, bool caseSensitive, ParameterExpression paramExp, string piece, ref ExpressionError error)
        {
            if (piece.StartsWith(compiledExpressionMarker.ToString()) && expressions.ContainsKey(piece))
            {
                return expressions[piece];
            }

            var type = source.GetType();
#if !SILVERLIGHT
            var pdc = TypeDescriptor.GetProperties(type);
#else
            var pdc = new PropertyInfoCollection(type);
#endif
            if (!caseSensitive && pdc[piece] == null)
            {
                string s = piece.ToLower();
#if !SILVERLIGHT
                foreach (PropertyDescriptor pd in pdc)
                {
                    if (s == pd.Name.ToLower())
                    {
                        piece = pd.Name;
                        break;
                    }
                }
#else
                foreach (var kvp in pdc)
                {
                    if (s == kvp.Value.Name.ToLower())
                    {
                        piece = kvp.Value.Name;
                        break;
                    }
                }
#endif
            }
#if !SILVERLIGHT
            if (pdc[piece] != null)
#else
            PropertyInfo pInfo;
            if (pdc.TryGetValue(piece, out pInfo))
#endif
            {
                return System.Linq.Expressions.Expression.PropertyOrField(paramExp, piece);
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
        /// <param name="strings">A dictonary that maps tokens to raw string values.</param>
        /// <param name="error">Error code if any.</param>
        /// <returns>A string that represnts the formula with quoted strings replaced by tokens.</returns>
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

        public static Delegate GetCompiledExpression(this Type type, bool caseSensitive, string formula, out ExpressionError error)
        {
            error = ExpressionError.None;
            ErrorString = "";
            var paramExp = System.Linq.Expressions.Expression.Parameter(type, type.Name);
            formula = TokenizeStrings(formula, ref error);

            StringBuilder sb = new StringBuilder(formula);
            sb = sb.Replace(">=", geMarker.ToString()).Replace("<=", leMarker.ToString()).Replace("<>", neMarker.ToString())
                 .Replace(" AND ", andMarker.ToString()).Replace(" And ", andMarker.ToString()).Replace(" and ", andMarker.ToString())
                 .Replace(" OR ", orMarker.ToString()).Replace(" Or ", orMarker.ToString()).Replace(" or ", orMarker.ToString())
                 .Replace(" NOT", notMarker.ToString()).Replace(" Not", notMarker.ToString()).Replace(" not", notMarker.ToString())
                 .Replace(" STARTSWITH ", startsWithMarker.ToString()).Replace(" StartsWith ", startsWithMarker.ToString()).Replace(" startswith ", startsWithMarker.ToString())
                  .Replace(" ENDSWITH ", endsWithMarker.ToString()).Replace(" EndsWith ", endsWithMarker.ToString()).Replace(" endswith ", endsWithMarker.ToString())
                   .Replace(" CONTAINS ", containsMarker.ToString()).Replace(" Contains ", containsMarker.ToString()).Replace(" contains ", containsMarker.ToString())
                   .Replace("DAY(", dayMarker.ToString()).Replace("Day(", dayMarker.ToString()).Replace("day(", dayMarker.ToString())
                   .Replace("WEEK(", weekMarker.ToString()).Replace("Week(", weekMarker.ToString()).Replace("week(", weekMarker.ToString())
                   .Replace("MONTH(", monthMarker.ToString()).Replace("Month(", monthMarker.ToString()).Replace("month(", monthMarker.ToString())
                   .Replace("QUARTER(", quarterMarker.ToString()).Replace("Quarter(", quarterMarker.ToString()).Replace("quarter(", quarterMarker.ToString())
                   .Replace("YEAR(", yearMarker.ToString()).Replace("Year(", yearMarker.ToString()).Replace("year(", yearMarker.ToString())
                 .Replace("[", String.Empty).Replace("]", String.Empty);

            formula = sb.ToString();
            int loc = formula.IndexOfAny(unaryOperations);
            while (loc > -1 && loc < formula.Length && error == ExpressionError.None)
            {
                int locRightParen = formula.IndexOf(rightParen, loc + 1);
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
                    string piece = formula.Substring(start + 1, loc - start - 1);
                    string token = type.GetSimpleExpression(caseSensitive, piece, paramExp, ref error);
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
                loc = formula.IndexOf(rightParen);
            }
            if (error == ExpressionError.None)
            {
                string token = type.GetSimpleExpression(caseSensitive, formula, paramExp, ref error);
                if (token == null && error == ExpressionError.None)
                {
                    System.Linq.Expressions.Expression exp = type.GetExpressionPiece(caseSensitive, paramExp, formula, ref error);
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

        private static string GetSimpleExpression(this Type type, bool caseSensitive, string formula, ParameterExpression paramExp, ref ExpressionError error)
        {
            bool finished = type.CompileToExpression(paramExp, caseSensitive, ref formula, new char[] { startsWithMarker, endsWithMarker, containsMarker, dayMarker, weekMarker, monthMarker, quarterMarker, yearMarker }, allOperations, out error);
            if (!finished && error == ExpressionError.None)
            {
                finished = type.CompileToExpression(paramExp, caseSensitive, ref formula, new char[] { multMarker, divideMarker }, allOperations, out error);
                if (!finished && error == ExpressionError.None)
                {
                    if (!finished && error == ExpressionError.None)
                    {
                        finished = type.CompileToExpression(paramExp, caseSensitive, ref formula, new char[] { plusMarker, minusMarker }, allOperations, out error);
                        if (!finished && error == ExpressionError.None)
                        {
                            finished = type.CompileToExpression(paramExp, caseSensitive, ref formula, new char[] { powerMarker, modMarker }, allOperations, out error);
                            if (!finished && error == ExpressionError.None)
                            {
                                finished = type.CompileToExpression(paramExp, caseSensitive, ref formula, new char[] { geMarker, leMarker, neMarker, lesserMarker, greaterMarker, equalMarker }, allOperations, out error);
                                if (!finished && error == ExpressionError.None)
                                {
                                    finished = type.CompileToExpression(paramExp, caseSensitive, ref formula, new char[] { notMarker }, allOperations, out error);
                                    if (!finished && error == ExpressionError.None)
                                    {
                                        finished = type.CompileToExpression(paramExp, caseSensitive, ref formula, new char[] { andMarker, orMarker }, allOperations, out error);
                                    }
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

        private static bool CompileToExpression(this Type type, ParameterExpression paramExp, bool caseSensitive, ref string formula, char[] operations, char[] allOperations, out ExpressionError error)
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
                        System.Linq.Expressions.Expression exp = type.GetExpression(caseSensitive, paramExp, left, right, formula[loc], ref error);
                        key = compiledExpressionMarker + expressions.Count.ToString() + compiledExpressionMarker;
                        expressions.Add(key, exp);
                    }
                    else
                    {
                        //need to get left and right...
                        string left = formula.Substring(start + 1, loc - start - 1).Trim();
                        string right = formula.Substring(loc + 1, end - loc - 1).Trim();
                        System.Linq.Expressions.Expression exp = type.GetExpression(caseSensitive, paramExp, left, right, formula[loc], ref error);
                        key = compiledExpressionMarker + expressions.Count.ToString() + compiledExpressionMarker;
                        expressions.Add(key, exp);
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

        private static System.Linq.Expressions.Expression GetExpression(this Type type, bool caseSensitive, ParameterExpression paramExp, string left, string right, char operand, ref ExpressionError error)
        {
            System.Linq.Expressions.Expression leftExp = type.GetExpressionPiece(caseSensitive, paramExp, left, ref error);
            System.Linq.Expressions.Expression rightExp = type.GetExpressionPiece(caseSensitive, paramExp, right, ref error);

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
                            exp = System.Linq.Expressions.Expression.GreaterThanOrEqual(leftExp, rightExp);
                            break;
                        case leMarker:
                            exp = System.Linq.Expressions.Expression.LessThanOrEqual(leftExp, rightExp);
                            break;
                        case neMarker:
                            exp = System.Linq.Expressions.Expression.NotEqual(leftExp, rightExp);
                            break;
                        case andMarker:
                            exp = System.Linq.Expressions.Expression.And(leftExp, rightExp);
                            break;
                        case orMarker:
                            exp = System.Linq.Expressions.Expression.Or(leftExp, rightExp);
                            break;
                        case notMarker:
                            exp = System.Linq.Expressions.Expression.Not(leftExp);
                            break;
                        case startsWithMarker:
                        case endsWithMarker:
                        case containsMarker:
                            {
                                string funcName = GetFunctionName(operand);
                                if (funcName.Length > 0)
                                {
                                    var stringMethod = typeof(string).GetMethods().Where(m => m.Name == funcName).FirstOrDefault();
                                    var underlyingType = leftExp.Type;

                                    if (underlyingType == typeof(string))
                                    {
                                        exp = System.Linq.Expressions.Expression.Call(
                                            leftExp,
                                            stringMethod,
                                            new System.Linq.Expressions.Expression[] { rightExp });
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException("Underlying type is not a string");
                                    }
                                }
                            }
                            break;
                        case dayMarker:
                        case weekMarker:
                        case monthMarker:
                        case quarterMarker:
                        case yearMarker:
                            {
                                string funcName = GetFunctionName(operand);
                                if (funcName.Length > 0)
                                {
                                    var dateProperty = typeof(DateTime).GetProperties().Where(m => m.Name == funcName).FirstOrDefault();
                                    var underlyingType = rightExp.Type;

                                    if (underlyingType == typeof(DateTime))
                                    {
                                        exp = System.Linq.Expressions.Expression.Property(rightExp, dateProperty);
                                    }
                                    else if (underlyingType == typeof(DateTime?))
                                    {
                                        var valueProperty = typeof(DateTime?).GetProperties().Where(m => m.Name == "Value").FirstOrDefault();
                                        var e1 = System.Linq.Expressions.Expression.Property(rightExp, valueProperty);
                                        exp = System.Linq.Expressions.Expression.Property(e1, dateProperty);
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException("Underlying type is not a DateTime or DateTime?");
                                    }
                                }
                            }
                            break;
                        case plusMarker:
                            exp = System.Linq.Expressions.Expression.Add(leftExp, rightExp);
                            break;
                        case minusMarker:
                            exp = System.Linq.Expressions.Expression.Subtract(leftExp, rightExp);
                            break;
                        case multMarker:
                            exp = System.Linq.Expressions.Expression.Multiply(leftExp, rightExp);
                            break;
                        case divideMarker:
                            exp = System.Linq.Expressions.Expression.Divide(leftExp, rightExp);
                            break;
                        case powerMarker:
                            exp = System.Linq.Expressions.Expression.Power(leftExp, rightExp);
                            break;
                        case modMarker:
                            exp = System.Linq.Expressions.Expression.Modulo(leftExp, rightExp);
                            break;
                        case greaterMarker:
                            exp = System.Linq.Expressions.Expression.GreaterThan(leftExp, rightExp);
                            break;
                        case lesserMarker:
                            exp = System.Linq.Expressions.Expression.LessThan(leftExp, rightExp);
                            break;
                        case equalMarker:
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

        private static System.Linq.Expressions.Expression GetExpressionPiece(this Type type, bool caseSensitive, ParameterExpression paramExp, string piece, ref ExpressionError error)
        {
            if (piece.StartsWith(compiledExpressionMarker.ToString()) && expressions.ContainsKey(piece))
            {
                return expressions[piece];
            }

#if !SILVERLIGHT
            var pdc = TypeDescriptor.GetProperties(type);
#else
            var pdc = new PropertyInfoCollection(type);
#endif
            if (!caseSensitive && pdc[piece] == null)
            {
                string s = piece.ToLower();
#if !SILVERLIGHT
                foreach (PropertyDescriptor pd in pdc)
                {
                    if (s == pd.Name.ToLower())
                    {
                        piece = pd.Name;
                        break;
                    }
                }
#else
                foreach (var kvp in pdc)
                {
                    if (s == kvp.Value.Name.ToLower())
                    {
                        piece = kvp.Value.Name;
                        break;
                    }
                }
#endif
            }
#if !SILVERLIGHT
            if (pdc[piece] != null)
#else
            PropertyInfo pInfo;
            if (pdc.TryGetValue(piece, out pInfo))
#endif
            {
                return System.Linq.Expressions.Expression.PropertyOrField(paramExp, piece);
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
    }
    #endregion
}
