#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
#if WinRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Syncfusion.WinRT.Data;
#else
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    public class GridUnBoundColumn : GridTemplateColumn, IDisposable
    {
        #region Fields
        Delegate evaluator = null;
	    ExpressionError error = ExpressionError.None;
        #endregion

        #region Internal Fields
        internal Func<string, object, object> UnBoundFunc = null;
        internal UnboundPropertiesChanged UnboundPropertiesChanged;
        #endregion

        #region ctor
        public GridUnBoundColumn()
        {
            this.IsUnbound = true;
        }
        #endregion

        #region Public Dependency Properties
        public bool CaseSensitive
        {
            get { return (bool)GetValue(CaseSensitiveProperty); }
            set { SetValue(CaseSensitiveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CaseSensitive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CaseSensitiveProperty =
            GridDependencyProperty.Register("CaseSensitive", typeof(bool), typeof(GridUnBoundColumn), new GridPropertyMetadata(true));

        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Format.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FormatProperty =
            GridDependencyProperty.Register("Format", typeof(string), typeof(GridUnBoundColumn), new GridPropertyMetadata(string.Empty, OnFormatPropertyChanged));

        public string Expression
        {
            get { return (string)GetValue(ExpressionProperty); }
            set { SetValue(ExpressionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Expression.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpressionProperty =
            GridDependencyProperty.Register("Expression", typeof(string), typeof(GridUnBoundColumn), new GridPropertyMetadata(string.Empty, OnExpressionPropertyChanged));

        #endregion

        #region Dependency Call back
        private static void OnExpressionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = (d as GridUnBoundColumn);
            if (column.UnboundPropertiesChanged != null)
            {
                column.evaluator = null;
                column.UnboundPropertiesChanged(column);
            }
        }

        private static void OnFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = (d as GridUnBoundColumn);
            if (column.UnboundPropertiesChanged != null)
            {
                column.evaluator = null;
                column.UnboundPropertiesChanged(column);
            }
        }

        #endregion

        #region Internal Properties
        internal ExpressionError Error
        {
            get { return error; }
        }
        #endregion

        #region internal methods
        /// <summary>
        /// Gets Computed value for expression in UnBoundColumn
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal object ComputedValue(object record)
        {
            if (evaluator == null)
            {
                evaluator = record.GetCompiledExpression(CaseSensitive, Expression, out error);
            }
            if (evaluator != null)
            {
                return evaluator.DynamicInvoke(record);
            }
            return null;
        }
        #endregion

        #region private methods
        void IDisposable.Dispose()
        {

        }
        #endregion
    }
    internal delegate void UnboundPropertiesChanged(GridUnBoundColumn column);
    public static class CalulationExtensions
    {
        #region Fields
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

        internal static string ErrorString = "";
        

        //notes:
        //1) The logical operators And, Or, Not must be sandwiched between blanks, and either all caps, no caps, or first cap only.
        //2) To use column names as And, Or, Not, they must be included in []'s
        //3) For any other column name, the brackets are optional.
        //handles comparing a property to a constant
        #endregion

        #region internal method
        /// <summary>
        /// Gets Compiled Expression
        /// </summary>
        /// <param name="source"></param>
        /// <param name="caseSensitive">If set to <see langword="true"/>, then ; otherwise, .</param>
        /// <param name="formula"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal static Delegate GetCompiledExpression(this object source, bool caseSensitive, string formula, out ExpressionError error)
        {
            error = ExpressionError.None;
            ErrorString = "";
            var type = source.GetType();
            var paramExp = System.Linq.Expressions.Expression.Parameter(type, type.Name);
            formula = TokenizeStrings(formula, ref error);

            StringBuilder sb = new StringBuilder(formula);
            sb = sb.Replace(">=", geMarker.ToString()).Replace("<=", leMarker.ToString()).Replace("<>", neMarker.ToString()).Replace("!=", neMarker.ToString())
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
        #endregion

        #region private methods
        /// <summary>
        /// Get SimpleExpression
        /// </summary>
        /// <param name="source"></param>
        /// <param name="caseSensitive">If set to <see langword="true"/>, then ; otherwise, .</param>
        /// <param name="formula"></param>
        /// <param name="paramExp"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static string GetSimpleExpression(this object source, bool caseSensitive, string formula, ParameterExpression paramExp, ref ExpressionError error)
        {
            var type = source.GetType();
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
                                        if (!finished && error == ExpressionError.None)
                                            finished = source.ComplieToExpressionSingleValue(paramExp, caseSensitive, ref formula, new char[] { }, allOperations, out error);
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

        /// <summary>
        /// Compile to Expression
        /// </summary>
        /// <param name="source"></param>
        /// <param name="paramExp"></param>
        /// <param name="caseSensitive">If set to <see langword="true"/>, then ; otherwise, .</param>
        /// <param name="formula"></param>
        /// <param name="operations"></param>
        /// <param name="allOperations"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        /// <remarks></remarks>
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

        private static bool ComplieToExpressionSingleValue(this object source, ParameterExpression paramExp, bool caseSensitive, ref string formula, char[] operations, char[] allOperations, out ExpressionError error)
        {
            error = ExpressionError.None;
            string key = "";
            string left = formula.Trim();
            string right = "0";
            System.Linq.Expressions.Expression exp = source.GetExpression(caseSensitive, paramExp, left, right, '+', ref error);
            key = compiledExpressionMarker + expressions.Count.ToString() + compiledExpressionMarker;
            expressions.Add(key, exp);
            string s = "";
            s += key;
            formula = s;
            return formula.StartsWith(compiledExpressionMarker.ToString()) && formula.EndsWith(compiledExpressionMarker.ToString()) && formula.IndexOf(compiledExpressionMarker, 1, formula.Length - 2) == -1;
        }

        /// <summary>
        /// Casting the type according to the expression
        /// </summary>
        /// <param name="leftExp"></param>
        /// <param name="rightExp"></param>
        /// <param name="error"></param>
        /// <remarks></remarks>
        private static void CoerceType(ref System.Linq.Expressions.Expression leftExp, ref System.Linq.Expressions.Expression rightExp, ref ExpressionError error)
        {
            if (leftExp != null && leftExp.Type != rightExp.Type)
            {
                if (leftExp.Type == typeof(decimal))
                    leftExp = System.Linq.Expressions.Expression.Convert(leftExp, typeof(double));
                if (rightExp.Type == typeof(decimal))
                    rightExp = System.Linq.Expressions.Expression.Convert(rightExp, typeof(double));
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
            }
        }

        /// <summary>
        /// Gets Expression
        /// </summary>
        /// <param name="source"></param>
        /// <param name="caseSensitive">If set to <see langword="true"/>, then ; otherwise, .</param>
        /// <param name="paramExp"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="operand"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        /// <remarks></remarks>
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
#if WinRT
                                    var stringMethod = typeof(string).GetRuntimeMethods().Where(m => m.Name == funcName).FirstOrDefault();
#else
                                    var stringMethod = typeof(string).GetMethods().Where(m => m.Name == funcName).FirstOrDefault();
#endif

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
#if WinRT
                                    var dateProperty = typeof(DateTime).GetRuntimeProperties().Where(m => m.Name == funcName).FirstOrDefault();
#else
                                    var dateProperty = typeof(DateTime).GetProperties().Where(m => m.Name == funcName).FirstOrDefault();
#endif
                                    var underlyingType = rightExp.Type;

                                    if (underlyingType == typeof(DateTime))
                                    {
                                        exp = System.Linq.Expressions.Expression.Property(rightExp, dateProperty);
                                    }
                                    else if (underlyingType == typeof(DateTime?))
                                    {
#if WinRT
                                        var valueProperty = typeof(DateTime?).GetRuntimeProperties().Where(m => m.Name == "Value").FirstOrDefault();
#else
                                        var valueProperty = typeof(DateTime?).GetProperties().Where(m => m.Name == "Value").FirstOrDefault();
#endif
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

        /// <summary>
        /// Gets function name
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        /// <remarks></remarks>
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

        /// <summary>
        /// Gets Expression piece
        /// </summary>
        /// <param name="source"></param>
        /// <param name="caseSensitive">If set to <see langword="true"/>, then ; otherwise, .</param>
        /// <param name="paramExp"></param>
        /// <param name="piece"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static System.Linq.Expressions.Expression GetExpressionPiece(this object source, bool caseSensitive, ParameterExpression paramExp, string piece, ref ExpressionError error)
        {
            if (piece.StartsWith(compiledExpressionMarker.ToString()) && expressions.ContainsKey(piece))
            {
                return expressions[piece];
            }

            var type = source.GetType();

#if !WPF
            var pdc = new PropertyInfoCollection(type);
            PropertyInfo value;
            if (pdc.TryGetValue(piece, out value))
            {
#else
            var pdc = TypeDescriptor.GetProperties(type);
#endif
                        
                if (!caseSensitive && pdc[piece] == null)
                {
                    string s = piece.ToLower();
#if !WPF
                    foreach (var kvp in pdc)
                    {
                        if (s == kvp.Value.Name.ToLower())
                        {
                            piece = kvp.Value.Name;
                            break;
                        }
                    }

                }
#else
                    foreach (PropertyDescriptor kvp in pdc)
                    {
                        if (s == kvp.Name.ToLower())
                        {
                            piece = kvp.Name;
                            break;
                        }
                    }
#endif                
            }
            else
            {
                if (!caseSensitive)
                {
                    string s1 = piece.ToLower();

#if !WPF
                    foreach (var kvp in pdc)
                    {
                        if (s1 == kvp.Value.Name.ToLower())
                        {
                            piece = kvp.Value.Name;
                            break;
                        }
                    }
#else
                    foreach (PropertyDescriptor kvp in pdc)
                    {
                        if (s1 == kvp.Name.ToLower())
                        {
                            piece = kvp.Name;
                            break;
                        }
                    }
#endif
                }
            }


#if !WPF
            PropertyInfo pInfo;
            if (pdc.TryGetValue(piece, out pInfo))
#else
                if (pdc[piece] != null)
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
        /// Tokenize the strings
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        /// <remarks></remarks>
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
        #endregion

    }
}
