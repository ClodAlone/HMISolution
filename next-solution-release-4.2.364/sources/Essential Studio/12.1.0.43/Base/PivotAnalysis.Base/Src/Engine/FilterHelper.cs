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
using System.Xml.Serialization;
using System.Reflection;
using System.Collections;


#if XLSIO
namespace Syncfusion.XlsIO.Implementation.PivotAnalysis
#elif !SILVERLIGHT
namespace Syncfusion.PivotAnalysis.Base
#else
namespace Syncfusion.PivotAnalysis.Base.Silverlight
#endif

{
    #region FilterHelper class
    /// <summary>
    /// This class encapsulates support for computing filter values and expressions. It is primarily intended
    /// as a internal use class.
    /// </summary>
    public class FilterHelper
    {
        bool caseSensitive = true;

        /// <summary>
        /// Gets or sets whether comparisons are case sensitive. The default is true.
        /// </summary>
        public bool CaseSensitive
        {
            get { return caseSensitive; }
            set { caseSensitive = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public FilterHelper()
        {
            this.filterExpressions = new List<FilterExpression>();
        }


        /// <summary>
        /// Gets the count of the filters currently in this helper class.
        /// </summary>
        public int Count
        {
            get { return filterExpressions.Count; }
        }


        /// <summary>
        /// Gets an particular FilterExpression in the order that the expressions have been added.
        /// </summary>
        /// <param name="i">The index of desired FilterExpression.</param>
        /// <returns>The FilterExpression.</returns>
        public FilterExpression this[int i]
        {
            get
            {
                if (i < Count && i > -1)
                {
                    return filterExpressions[i]; ;
                }
                return null;
            }
        }

        internal List<FilterExpression> filterExpressions { get; set; }

        /// <summary>
        /// Adds a FilterExpression.
        /// </summary>
        /// <param name="name">The name of this expression.</param>
        /// <param name="expression">A string holding a well-formed logical expression</param>

        public void AddFilterExpression(string name, string expression)
        {
            FilterExpression exp = new FilterExpression(name, expression);
            filterExpressions.Add(exp);
            exp.CaseSensitive = this.CaseSensitive;
        }

        /// <summary>
        /// Removes a FilterExpression.
        /// </summary>
        /// <param name="exp">The name of the FilterExpression to be removed.</param>
        /// <returns>True if the expression was successfully removed, false otherwise.</returns>

        public bool RemoveFilterExpression(FilterExpression exp)
        {
            return filterExpressions.Remove(exp);
        }

        /// <summary>
        /// Clears all FilterExpressions.
        /// </summary>
        public void Clear()
        {
            filterExpressions.Clear();
        }
    }

    #endregion

    #region FilterExpression class

    /// <summary>
    /// This class encapsulates the information needed to define a filter.
    /// </summary>
    public class FilterExpression
    {
        /// <summary>
        /// Empty Constructor of FilterExpression
        /// </summary>
        public FilterExpression()
        {

        }

        /// <summary>
        /// A constructor method that executes on instantiation of FilterExpression class.
        /// </summary>
        /// <param name="dimensionName">Name of the PivotItem</param>
        public FilterExpression(string dimensionName)
            : this(dimensionName, dimensionName, null)
        {

        }

        /// <summary>
        /// A constructor method that executes on instantiation of FilterExpression class.
        /// </summary>
        /// <param name="dimensionName">Name of the PivotItem</param>
        /// <param name="dimensionHeader">Friendly Text of the PivotItem</param>
        /// <param name="expression">Logical expression defining the filter</param>
        public FilterExpression(string dimensionName, string dimensionHeader, string expression)
        {

            this.DimensionName = dimensionName;

            this.DimensionHeader = dimensionHeader;

            this.expression = expression;

        }

        /// <summary>
        /// A constructor method that executes on instantiation of FilterExpression class.
        /// </summary>
        /// <param name="dimensionName">Name of the PivotItem</param>
        /// <param name="dimensionHeader">Friendly Text of the PivotItem</param>
        /// <param name="expression">Logical expression defining the filter</param>
        /// <param name="format">Format of the expression</param>
        public FilterExpression(string dimensionName, string dimensionHeader, string expression, string format)
        {

            this.DimensionName = dimensionName;

            this.DimensionHeader = dimensionHeader;

            this.expression = expression;
            this.Format = format;
        }

        /// <summary>
        /// A constructor method that executes on instantiation of FilterExpression class.
        /// </summary>
        /// <param name="name">It indicates the DimensionName</param>
        ///<param name="expression">The well formed logical expression defining the filter.</param>
        public FilterExpression(string name, string expression)
        {
            this.expression = expression;
            this.name = this.DimensionName = this.DimensionHeader = name;

        }
        /// <summary>
        /// Gets or sets the Dimension name.
        /// </summary>
        private string dimensionName;

        /// <summary>
        /// Determines the dimension to be set
        /// </summary>
        public string DimensionName
        {
            get { return dimensionName; }
            set
            {
                dimensionName = value;
                if (DimensionHeader == null)
                    DimensionHeader = DimensionName;
            }
        }
        private string format;

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        public string Format
        {
            get { return format; }
            set
            {
                format = value;
            }
        }
        /// <summary>
        /// Gets or sets the Dimension header.
        /// </summary>
        public string DimensionHeader { get; set; }

        Delegate evaluator = null;
        /// <summary>
        /// Evaluates the given value
        /// </summary>
        [XmlIgnore]
        public Delegate Evaluator
        {
            get { return evaluator; }
            set { evaluator = value; }
        }


        /// <summary>
        /// Use this method to retrieve the computed value of this expression on an object.
        /// </summary>
        /// <param name="component">The object to be evaluated.</param>
        /// <returns>The computed value.</returns>
        /// <remarks>
        /// The first time this method is call, a delegate for the FilterExpression is created, and then this delegate
        /// is calls passing in the component. Subsequent calls to this method just result into the existing delegate call.
        /// </remarks>
        public object ComputedValue(object component)
        {
            if (evaluator == null)
            {
                evaluator = component.GetCompiledExpression(CaseSensitive, expression, out error, this.Name);
            }
            if (evaluator != null)
            {
                return evaluator.DynamicInvoke(component);
            }
            return null;
        }


        ExpressionError error = ExpressionError.None;
        /// <summary>
        /// Gets the last error that was logged during the compilation and calculation phases.
        /// </summary>
        public ExpressionError Error
        {
            get { return error; }
        }
        /// <summary>
        /// Gets a descriptive string for the last error raised.
        /// </summary>
        public string ErrorString
        {
            get
            {
                if (error != ExpressionError.ExceptionRaised)
                {
                    return error.ToString();
                }
                return CalculationExtensions.ErrorString;
            }
        }

        string name = "";
        /// <summary>
        /// Gets or sets the name of this FilterExpression.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        string expression = "";
        /// <summary>
        /// Gets or sets the well-formed logical expression that defines this FilterExpression.
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

        //object m_Tag;
        //public object TagName
        //{
        //    get
        //    {
        //        return m_Tag;
        //    }
        //    set
        //    {
        //        m_Tag = value;
        //    }
        //}
        /// <summary>
        /// Denotes the Name
        /// </summary>
        [XmlIgnore]
        public object Tag { get; set; }


        bool caseSensitive = true;
        /// <summary>
        /// Gets or sets whether ths expression should be treated in a case sensitive manner.
        /// </summary>
        public bool CaseSensitive
        {
            get { return caseSensitive; }
            set { caseSensitive = value; }
        }
    }

    #endregion

    #region CalculationExtensions class

    internal static class CalculationExtensions
    {
        const char stringMarker = (char)130;
        const char compiledExpressionMarker = (char)131;
        const char geMarker = (char)132;
        const char leMarker = (char)133;
        const char neMarker = (char)134;
        const char andMarker = (char)135;
        const char orMarker = (char)136;
        // const char notMarker = (char)137;
        const char startsWithMarker = (char)138;
        const char endsWithMarker = (char)139;
        const char containsMarker = (char)140;
        const char dayMarker = (char)141;
        const char weekMarker = (char)142;
        const char monthMarker = (char)143;
        const char quarterMarker = (char)144;
        const char yearMarker = (char)145;
        const char inMarker = (char)146;
        const char plusMarker = (char)147;//'+';
        const char minusMarker = (char)148;//'-';
        const char multMarker = (char)149;//'*';
        const char divideMarker = (char)150;//'/';
        const char powerMarker = (char)151;//'^';
        const char modMarker = (char)152;//'%';
        const char greaterMarker = (char)153;//'>';
        const char lesserMarker = (char)154;//'<';
        const char equalMarker = (char)155;//'=';
        const char toStringMarker = (char)156;
        const char formatMarker = (char)157;
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
                                          //      notMarker,
                                                startsWithMarker,
                                                endsWithMarker,
                                                containsMarker,
                                                dayMarker,
                                                weekMarker,
                                                monthMarker,
                                                quarterMarker,
                                                yearMarker,
                                                inMarker,
                                                plusMarker,
                                                minusMarker,
                                                multMarker,
                                                divideMarker,
                                                powerMarker,
                                                modMarker,
                                                greaterMarker,
                                                lesserMarker,
                                                equalMarker,
                                                toStringMarker,
                                                formatMarker};
        static char[] unaryOperations = new[]{ dayMarker,
                                                weekMarker,
                                                monthMarker,
                                                quarterMarker,
                                                yearMarker };

        /// <summary>
        /// Gets or sets a collection of property types associated with an ExpandoObject/dynamic object available in .NET 4.0.
        /// </summary>
        public static Dictionary<string, Type> DynamicPropertyTypeTable { get; set; }

        internal static PivotEngine Engine { get; set; }

        internal static Delegate GetCompiledExpression(this object source, bool caseSensitive, string formula, out ExpressionError error, string fieldName)
        {
            error = ExpressionError.None;
            ErrorString = "";
            const char underScoreMarker = (char)129;
            var type = source.GetType();
            var paramExp = System.Linq.Expressions.Expression.Parameter(type, type.Name);
            formula = TokenizeStrings(formula, ref error);

            StringBuilder sb = new StringBuilder(formula);
            sb = sb.Replace(" OR ", orMarker.ToString()).Replace(" AND ", andMarker.ToString()).Replace(" >= ", geMarker.ToString()).Replace(" <= ", leMarker.ToString()).Replace(" <> ", neMarker.ToString())
                    .Replace(" || ", orMarker.ToString()); 

            if (!formula.Contains(fieldName))
                sb = sb.Replace(" = ", equalMarker.ToString()).Replace(leftBracket.ToString(), "").Replace(rightBracket.ToString(), "");
            else
                sb = sb.Replace(" = ", equalMarker.ToString());

            sb = sb.Replace(" STARTSWITH ", startsWithMarker.ToString()).Replace(" StartsWith ", startsWithMarker.ToString()).Replace(" startswith ", startsWithMarker.ToString())
                .Replace(" TOSTRING ", toStringMarker.ToString()).Replace(" ToString ", toStringMarker.ToString()).Replace(" tostring ", toStringMarker.ToString())
                .Replace(" ENDSWITH ", endsWithMarker.ToString()).Replace(" EndsWith ", endsWithMarker.ToString()).Replace(" endswith ", endsWithMarker.ToString())
                .Replace(" CONTAINS ", containsMarker.ToString()).Replace(" Contains ", containsMarker.ToString()).Replace(" contains ", containsMarker.ToString())
                .Replace("DAY(", dayMarker.ToString()).Replace("Day(", dayMarker.ToString()).Replace("day(", dayMarker.ToString())
                .Replace("WEEK(", weekMarker.ToString()).Replace("Week(", weekMarker.ToString()).Replace("week(", weekMarker.ToString())
                .Replace("MONTH(", monthMarker.ToString()).Replace("Month(", monthMarker.ToString()).Replace("month(", monthMarker.ToString())
                .Replace("QUARTER(", quarterMarker.ToString()).Replace("Quarter(", quarterMarker.ToString()).Replace("quarter(", quarterMarker.ToString())
                .Replace("YEAR(", yearMarker.ToString()).Replace("Year(", yearMarker.ToString()).Replace("year(", yearMarker.ToString())
                .Replace(" IN ", inMarker.ToString()).Replace(" In ", inMarker.ToString()).Replace(" in ", inMarker.ToString())

                    .Replace(" ? ", formatMarker.ToString())
                    .Replace(" > ", greaterMarker.ToString())
                    .Replace(" < ", lesserMarker.ToString());
                   if(!formula.Contains(fieldName))
                   {
                    sb.Replace(" + ", plusMarker.ToString());
                    sb.Replace(" - ", minusMarker.ToString());
                    sb.Replace(" * ", multMarker.ToString());
                    sb.Replace(" / ", divideMarker.ToString());
                    sb.Replace(" ^ ", powerMarker.ToString());
                    sb.Replace(" % ", modMarker.ToString());
                   }
                    

                if (sb.ToString().Contains(underScoreMarker.ToString()))
                    sb.Replace(underScoreMarker.ToString(), " ");
            
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
                    if (locRightParen + 1 < formula.Length)
                    {
                        loc = formula.Substring(locRightParen + 1).IndexOfAny(unaryOperations);
                        if (loc > -1)
                        {
                            loc += locRightParen + 1;
                        }
                    }
                    else
                    {
                        loc = -1;
                    }
                }
                if(!formula.Contains(fieldName))
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
                    Expression exp = source.GetExpressionPiece(caseSensitive, paramExp, formula, ref error,null);
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
        public static Delegate GetCompiledExpression(this object source, bool caseSensitive, string formula, out ExpressionError error)
        {
            return GetCompiledExpression(source, caseSensitive, formula, out error, string.Empty);
        }

        private static string GetSimpleExpression(this object source, bool caseSensitive, string formula, ParameterExpression paramExp, ref ExpressionError error)
        {
            var type = source.GetType();
            bool finished = source.CompileToExpression(paramExp, caseSensitive, ref formula, new char[] { inMarker }, allOperations, out error);
            if (!finished && error == ExpressionError.None)
            {
                finished = source.CompileToExpression(paramExp, caseSensitive, ref formula, new char[] { formatMarker, toStringMarker, startsWithMarker, endsWithMarker, containsMarker, dayMarker, weekMarker, monthMarker, quarterMarker, yearMarker }, allOperations, out error);
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
                                        // finished = source.CompileToExpression(paramExp, caseSensitive, ref formula, new char[] { notMarker }, allOperations, out error);
                                        // if (!finished && error == ExpressionError.None)
                                        {
                                            finished = source.CompileToExpression(paramExp, caseSensitive, ref formula, new char[] { andMarker, orMarker }, allOperations, out error);
                                        }
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
                        System.Linq.Expressions.Expression exp = null;
#if SyncfusionFramework4_0
                        if (paramExp.Type.Name == "ExpandoObject" && DynamicPropertyTypeTable != null && DynamicPropertyTypeTable.ContainsKey(right))
                        {
                            string funcName = "GetDynamicValue";

                            var method = typeof(CalculationExtensions).GetMethods().Where(m => m.Name == funcName).FirstOrDefault();

                            exp = Expression.Call(
                                null,
                                method,
                                 new Expression[] { paramExp, Expression.Constant(right) }
                                );
                            right = compiledExpressionMarker + expressions.Count.ToString() + compiledExpressionMarker;
                            expressions.Add(right, exp);
                        }
#endif
                        exp = source.GetExpression(caseSensitive, paramExp, left, right, formula[loc], ref error);
                        key = compiledExpressionMarker + expressions.Count.ToString() + compiledExpressionMarker;
                        expressions.Add(key, exp);
                    }
                    else
                    {
                        //need to get left and right...
                        string left = formula.Substring(start + 1, loc - start - 1).Trim();
                        string right = formula.Substring(loc + 1, end - loc - 1);
                        System.Linq.Expressions.Expression exp = null;

                        var type = source.GetType();
                        IDictionary dict = source as IDictionary;

#if SyncfusionFramework4_0
                        if (paramExp.Type.Name == "ExpandoObject" && DynamicPropertyTypeTable != null && DynamicPropertyTypeTable.ContainsKey(left))
                        {
                            string funcName = "GetDynamicValue";

                            var method = typeof(CalculationExtensions).GetMethods().Where(m => m.Name == funcName).FirstOrDefault();

                            exp = Expression.Call(
                                null,
                                method,
                                 new Expression[] { paramExp, Expression.Constant(left) }
                                );
                            left = compiledExpressionMarker + expressions.Count.ToString() + compiledExpressionMarker;
                            expressions.Add(left, exp);
                        }
                        else
#endif
                            if (dict != null)
                            {
                                if (dict.Contains(right))
                                {
                                    exp = GetDictionaryLookUpExpression(paramExp, right);
                                    right = compiledExpressionMarker + expressions.Count.ToString() + compiledExpressionMarker;
                                    expressions.Add(right, exp);
                                }
                                if (dict.Contains(left))
                                {
                                    exp = GetDictionaryLookUpExpression(paramExp, left);
                                    left = compiledExpressionMarker + expressions.Count.ToString() + compiledExpressionMarker;
                                    expressions.Add(left, exp);
                                }
                            }

                        exp = source.GetExpression(caseSensitive, paramExp, left, right, formula[loc], ref error);
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

        public static object GetDynamicValue(object o, string property)
        {
            if (o is IDictionary<string, object>)
            {
                return ((IDictionary<string, object>)o)[property];
            }
            return null;
        }

        public static double GetDictionaryDoubleValue(IDictionary o, string property)
        {
            if (o != null && o[property] != null)
            {
                return (double)o[property];
            }
            return double.NaN;
        }

        internal static string ErrorString = "";

        private static System.Linq.Expressions.Expression GetExpression(this object source, bool caseSensitive, ParameterExpression paramExp, string left, string right, char operand, ref ExpressionError error)
        {
            if (operand == formatMarker)
            {//this is used to handle formats applied to DateTime (like MMM and yyyy).
                operand = toStringMarker;
            }

            System.Linq.Expressions.Expression leftExp = source.GetExpressionPiece(caseSensitive, paramExp, left, ref error, null);
            System.Linq.Expressions.Expression rightExp = source.GetExpressionPiece(caseSensitive, paramExp, right, ref error, leftExp == null ? null : leftExp.Type);

            if (!unaryOperations.Contains(operand) && operand != toStringMarker)
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
                        case inMarker:
                            {
                                string s = rightExp.ToString();
                                BinaryList lookups = GetLookUpList(s.Substring(1, s.Length - 2));

                                string funcName = "CheckInList";

                                var method = typeof(CalculationExtensions).GetMethods().Where(m => m.Name == funcName).FirstOrDefault();
                                var underlyingType = leftExp.Type;

                                if (underlyingType == typeof(string))
                                {
                                    exp = Expression.Call(
                                        null,
                                        method,
                                         new Expression[] { leftExp, System.Linq.Expressions.Expression.Constant(lookups) }
                                        );
                                }
                                else
                                {
                                    throw new InvalidOperationException("Underlying type is not a string");
                                }

                            }
                            break;
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

                        //case notMarker:
                        //    exp = Expression.Not(leftExp);
                        //    break;
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
                                        exp = Expression.Call(
                                            leftExp,
                                            stringMethod,
                                            new Expression[] { rightExp });
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException("Underlying type is not a string");
                                    }
                                }
                            }
                            break;
                        case toStringMarker:
                            {
                                string funcName = GetFunctionName(operand);
                                if (funcName.Length > 0)
                                {
                                    var underlyingType = leftExp.Type;
                                    var stringMethod = underlyingType.GetMethods().Where(m => m.Name == funcName && m.GetParameters().Count() == 1).FirstOrDefault();

                                    exp = Expression.Call(
                                        leftExp,
                                        stringMethod,
                                        new Expression[] { Expression.Constant(right, typeof(string)) });
                                }
                            }
                            break;
                        case dayMarker:
                        case weekMarker:
                        case monthMarker:
                        case yearMarker:
                            {
                                string funcName = GetFunctionName(operand);
                                if (funcName.Length > 0)
                                {

                                    var dateProperty = typeof(DateTime).GetProperties().Where(m => m.Name == funcName).FirstOrDefault();
                                    var underlyingType = rightExp.Type;
                                    if (DynamicPropertyTypeTable != null)
                                    {
                                        rightExp = System.Linq.Expressions.Expression.Convert(rightExp, typeof(DateTime));
                                        underlyingType = rightExp.Type;
                                    }

                                    if (underlyingType == typeof(DateTime))
                                    {
                                        exp = Expression.Property(rightExp, dateProperty);
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException("Underlying type is not a DateTime");
                                    }
                                }
                            }
                            break;
                        case quarterMarker:
                            {
                                var dateProperty = typeof(DateTime).GetProperties().Where(m => m.Name == "Month").FirstOrDefault();
                                var underlyingType = rightExp.Type;
                                if (DynamicPropertyTypeTable != null)
                                {
                                    rightExp = System.Linq.Expressions.Expression.Convert(rightExp, typeof(DateTime));
                                    underlyingType = rightExp.Type;
                                }
                                if (underlyingType == typeof(DateTime))
                                {
                                    exp = Expression.Property(rightExp, dateProperty);
                                    exp = Expression.Subtract(exp, Expression.Constant(1));
                                    exp = Expression.Divide(exp, Expression.Constant(3));
                                    exp = Expression.Add(exp, Expression.Constant(1));
                                }
                                else
                                {
                                    throw new InvalidOperationException("Underlying type is not a DateTime");
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

        private static char listSeparator = ',';

        public static char ListSeparator
        {
            get { return listSeparator; }
            set { listSeparator = value; }
        }

        private static BinaryList GetLookUpList(string item)
        {
            BinaryList list = new BinaryList();
            string[] items = item.Split(new char[] { ListSeparator });
            foreach (string s in items)
            {
                list.AddIfUnique(s);
            }

            return list;
        }

        public static bool CheckInList(object s, BinaryList list)
        {
            return list.BinarySearch(s.ToString()) > -1;
        }

        private static string GetFunctionName(char c)
        {
            // DateTime d;

            string s = "";
            switch (c)
            {
                case toStringMarker:
                    s = "ToString";
                    break;
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
            if (leftExp.Type != rightExp.Type)
            {
                //                if (leftExp.NodeType != ExpressionType.Constant &&
                //                    rightExp.NodeType == ExpressionType.Constant)
                //                {
                //#if !SILVERLIGHT
                //                    rightExp = System.Linq.Expressions.Expression.Constant(Convert.ChangeType(rightExp.ToString(), leftExp.Type));
                //#else
                //                    rightExp = System.Linq.Expressions.Expression.Constant(Convert.ChangeType(rightExp.ToString(), leftExp.Type, null));
                //#endif
                //                }
                //                else if (leftExp.NodeType == ExpressionType.Constant &&
                //                    rightExp.NodeType != ExpressionType.Constant)
                //                {
                //#if !SILVERLIGHT
                //                    leftExp = System.Linq.Expressions.Expression.Constant(Convert.ChangeType(leftExp.ToString(), rightExp.Type));
                //#else
                //                    leftExp = System.Linq.Expressions.Expression.Constant(Convert.ChangeType(leftExp.ToString(), rightExp.Type, null));
                //#endif
                //                }
                //                else 
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
                else //different types...
                {
                    try
                    {
                        bool setError = true;
#if SILVERLIGHT
                        if (CanConvertTypes(rightExp.Type, leftExp.Type))
                        {
                             rightExp = System.Linq.Expressions.Expression.Convert(rightExp, leftExp.Type);
                             setError = false;
                        }
                        else if (CanConvertTypes(leftExp.Type, rightExp.Type))
                        {

                            leftExp = System.Linq.Expressions.Expression.Convert(leftExp, rightExp.Type);
                            setError = false;
                        }
#else
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
                                if (rightExp.Type.IsValueType && leftExp.NodeType == ExpressionType.Call && ((MethodCallExpression)leftExp).Method.Name == "ToString")
                                {//required to handle DateTime formats like yyyy and double formatting like 000
                                    string funcName = "ToString";
                                    var underlyingType = rightExp.Type;
                                    MethodInfo stringMethod = null;
                                    if (((MethodCallExpression)leftExp).Object.Type == underlyingType || ((MethodCallExpression)leftExp).Object.Type != typeof(DateTime))
                                    {
                                        stringMethod = underlyingType.GetMethods().Where(m => m.Name == funcName && m.GetParameters().Count() == 1).FirstOrDefault();
                                        rightExp = Expression.Call(
                                        rightExp,
                                        stringMethod, ((MethodCallExpression)leftExp).Arguments);
                                    }
                                    else
                                    {
                                        stringMethod = underlyingType.GetMethods().Where(m => m.Name == funcName && m.GetParameters().Count() == 0).FirstOrDefault();
                                        rightExp = Expression.Call(
                                        rightExp,
                                        stringMethod);
                                    }
                                }
                                else
                                {
                                    if (leftExp.Type == typeof(string))
                                    {
                                        string funcName = "ToString";
                                        var underlyingType = rightExp.Type;
                                        MethodInfo stringMethod = underlyingType.GetMethods().Where(m => m.Name == funcName && m.GetParameters().Count() == 0).FirstOrDefault();
                                        rightExp = Expression.Call(
                                        rightExp,
                                        stringMethod);
                                    }
                                    else
                                    {
                                        rightExp = System.Linq.Expressions.Expression.Convert(rightExp, leftExp.Type);
                                    }
                                }
                                setError = false;
                            }
                        }
#endif
                        if (setError)
                        {
                            error = ExpressionError.CannotCompareDifferentTypes;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (leftExp.NodeType != ExpressionType.Constant &&
                            rightExp.NodeType == ExpressionType.Constant)
                        {
//#if !SILVERLIGHT
//                            rightExp = System.Linq.Expressions.Expression.Constant(Convert.ChangeType(rightExp.ToString(), leftExp.Type));
//#else
                             if (leftExp.Type == typeof(DateTime))
                            {
                                DateTime parsedDate;
                                string rightVal = (rightExp is ConstantExpression) ? (rightExp as ConstantExpression).Value.ToString() : rightExp.ToString();
                                //string[] formats = new string[] { "d", "dd", "ddd", "f", "ff", "fff", "ffff", "fffff", "ffffff", "fffffff", "F", "FF", "FFF", "FFFF", "FFFFF", "FFFFFF", "FFFFFFF", "g", "gg", "hh", "HH", "m", "mm", "M", "MM", "MMM", "MMMM", "s", "ss", "t", "tt", "y", "yy", "yyy", "yyyy", "yyyyy", "z", "zz", "zzz", ":", "/" };
                                //string[] formats = new string[] { "d", "dd", "ddd", "f", "ff", "fff", "ffff", "fffff", "ffffff", "fffffff", "F", "FF", "FFF", "FFFF", "FFFFF", "FFFFFF", "FFFFFFF", "G", "g", "gg", "hh", "HH", "m", "mm", "M", "MM", "MMMM", "o", "O", "r", "R", "s", "ss", "t", "tt", "T", "u", "U", "y", "yy", "yyy", "yyyyy", "Y", "z", "zz", "zzz", ":", "/" };
                                  string[] formats = new string[] { "yyyyy", "yyyy", "yyy", "yy", "y", "MMMM", "MMM", "MM", "M", "ddd", "dd", "d", "G", "gg", "g", "HH", "hh", "mm", "m", "ss", "s", "tt", "t", "T", "u", "U", "zzz", "zz", "z", "FFFFFFF", "FFFFFF", "FFFFF", "FFFF", "FFF", "FF", "F", "fffffff", "ffffff", "fffff", "ffff", "fff", "ff", "f", "o", "O", "r", "R", "/", ":" };
                                if (DateTime.TryParseExact(rightVal, formats, null, System.Globalization.DateTimeStyles.None, out parsedDate))
                                {
                                    rightExp = System.Linq.Expressions.Expression.Constant(parsedDate, leftExp.Type);
                                    error = ExpressionError.None;
                                    return;
                                }
                            }
                            rightExp = System.Linq.Expressions.Expression.Constant(Convert.ChangeType(rightExp.ToString(), leftExp.Type, null));
//#endif
                            error = ExpressionError.None;
                            return;
                        }
                        else if (leftExp.NodeType == ExpressionType.Constant &&
                            rightExp.NodeType != ExpressionType.Constant)
                        {
#if !SILVERLIGHT
                            leftExp = System.Linq.Expressions.Expression.Constant(Convert.ChangeType(leftExp.ToString(), rightExp.Type));
#else
                            leftExp = System.Linq.Expressions.Expression.Constant(Convert.ChangeType(leftExp.ToString(), rightExp.Type, null));
#endif
                            error = ExpressionError.None;
                            return;
                        }
                        error = ExpressionError.ExceptionRaised;
                        ErrorString = ex.Message;
                    }
                }
            }
        }

#if SILVERLIGHT
static bool CanConvertTypes(Type type1, Type type2) 
    {
        if (type1 == type2)
            return true;
     
        switch (Type.GetTypeCode(type1)) 
        {
            case TypeCode.Decimal:
                return type2 == typeof(double) || type2 == typeof(float) || type2 == typeof(int) || type2 == typeof(decimal) || type2 == typeof(Int16) || type2 == typeof(Int64);
            case TypeCode.Double:
                return type2 == typeof(double) || type2 == typeof(float) || type2 == typeof(int) || type2 == typeof(Int16) || type2 == typeof(Int64);
            case TypeCode.Single:
                return type2 == typeof(float) || type2 == typeof(int) || type2 == typeof(Int16) || type2 == typeof(Int64);
            case TypeCode.Int16:
                return type2 == typeof(Int16);
            case TypeCode.Int32:
                return type2 == typeof(int) || type2 == typeof(Int16);
            case TypeCode.String:
                return true;
            case TypeCode.Int64:
                return  type2 == typeof(int) || type2 == typeof(Int16) || type2 == typeof(Int64);
            case TypeCode.DateTime:
                    return true;
            default:
                break;
        }
        return false;
    } 

#endif

        //pass in a string containing a left or right part, and get an Expression back.
        private static System.Linq.Expressions.Expression GetExpressionPiece(this object source, bool caseSensitive, ParameterExpression paramExp, string piece, ref ExpressionError error, object leftExpType)
        {
            if (piece.StartsWith(compiledExpressionMarker.ToString()) && expressions.ContainsKey(piece))
            {
                return expressions[piece];
            }

            var type = source.GetType();
            IDictionary dict = source as IDictionary;
            if (dict != null)
            {
                if (leftExpType == null)
                    leftExpType = typeof(double);

                if (dict.Contains(piece))
                {
                    return GetDictionaryLookUpExpression(paramExp, piece);
                }
            }
#if SILVERLIGHT
            if (Engine.IsDataDynamic)
            {
                IDictionary<string, object> obj = source as IDictionary<string, object>;
                if (obj != null)
                {
                    IDictionary<string, object> pc = Engine.ItemProperties as IDictionary<string, object>;
                    if (!caseSensitive && !pc.ContainsKey(piece))
                    {
                        string s = piece.ToLower();
                        foreach (string name in pc.Keys)
                        {
                            if (s == name.ToLower())
                            {
                                piece = name;
                                break;
                            }
                        }
                    }
                    if (pc.ContainsKey(piece))
                    {
                        return System.Linq.Expressions.Expression.PropertyOrField(paramExp, piece);
                    }
                }
            }
            else
            {
                Syncfusion.Windows.Data.PropertyInfoCollection pdc = new Syncfusion.Windows.Data.PropertyInfoCollection(type);
                if (!caseSensitive && !pdc.ContainsKey(piece))
                {
                    string s = piece.ToLower();
                    foreach (string name in pdc.Keys)
                    {
                        if (s == name.ToLower())
                        {
                            piece = name;
                            break;
                        }
                    }
                }
                if (pdc.ContainsKey(piece))
                {
                    return System.Linq.Expressions.Expression.PropertyOrField(paramExp, piece);
                }
            }
#else
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(type);
            if (!caseSensitive && pdc[piece] == null)
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
#endif
            if ((Type)leftExpType == typeof(float) || (Type)leftExpType == typeof(double) || (Type)leftExpType == typeof(int))
            {
                double d = 0;
#if SyncfusionFramework4_0
                if (paramExp.Name != "ExpandoObject" && double.TryParse(piece, out d))
#else
            if (double.TryParse(piece, out d))
#endif
                {
                    return System.Linq.Expressions.Expression.Constant(d);
                }
            }
            int loc = piece.IndexOf(formatMarker);
            if (loc > -1)
            {
                piece = piece.Replace(formatMarker.ToString(), "");
            }
            loc = piece.IndexOf(stringMarker);
            while (loc > -1)
            {
                int end = piece.IndexOf(stringMarker, loc + 1);
                string key = piece.Substring(loc, end - loc + 1);
                piece = piece.Replace(key, strings[key]);
                loc = piece.IndexOf(stringMarker);
            }
            return System.Linq.Expressions.Expression.Constant(piece);
        }

        private static Expression GetDictionaryLookUpExpression(ParameterExpression paramExp, string piece)
        {
            string funcName = "GetDictionaryDoubleValue";
            var method = typeof(CalculationExtensions).GetMethods().Where(m => m.Name == funcName).FirstOrDefault();
            return Expression.Call(
                        null,
                        method,
                         new Expression[] { paramExp, Expression.Constant(piece) }
                        );
        }

        /// <summary>
        /// Replaces strings in formulas with tokens so string remain immutable during parsing.
        /// </summary>
        /// <param name="formula">The formula with quoted strings.</param>
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
    }

    /// <summary>
    /// Denoted the different Error types
    /// </summary>
    public enum ExpressionError
    {
        /// <summary>
        /// Denotes no Error
        /// </summary>
        None,
        /// <summary>
        /// Denotes MissingRightQuote
        /// </summary>
        MissingRightQuote,
        /// <summary>
        /// Denotes MismatchedParentheses
        /// </summary>
        MismatchedParentheses,
        /// <summary>
        /// Denotes CannotCompareDifferentTypes
        /// </summary>
        CannotCompareDifferentTypes,
        /// <summary>
        /// Denotes UnknownOperator
        /// </summary>
        UnknownOperator,
        /// <summary>
        /// Denotes NotAValidFormula
        /// </summary>
        NotAValidFormula,
        /// <summary>
        /// DenotesExceptionRaised
        /// </summary>
        ExceptionRaised
    }

    #endregion

}
