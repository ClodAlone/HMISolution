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
using System.Windows;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Reflection;
#if !WinRT
using Syncfusion.Windows.Data;
using Syncfusion.Linq;
using Syncfusion.Windows.Controls.Cells;

namespace Syncfusion.Windows.Controls.Grid
{
#else
using Windows.UI.Xaml;
using Syncfusion.WinRT.Controls.Cells;
using Syncfusion.WinRT.Styles;

namespace Syncfusion.WinRT.Controls.Grid
{
#endif
    /// <summary>
    /// Specifies the type of condition to be used in Conditional Formatting.
    /// </summary>
    public enum GridConditionType
    {
        /// <summary>
        /// Performs an Equals opertion on the operands.
        /// </summary>
        Equals,
        /// <summary>
        /// Performs a NotEquals operation on the operands.
        /// </summary>
        NotEquals,
        /// <summary>
        /// Performs a LessThan operation on the operands.
        /// </summary>
        LessThan,
        /// <summary>
        /// Performs a LessThanOrEqual operation on the operands.
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// Performs a GreatherThan operation on the operands.
        /// </summary>
        GreaterThan,
        /// <summary>
        /// Performs a GreaterThanOrEqual operation on the operands.
        /// </summary>
        GreaterThanOrEqual
        // Contains
    }

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCondition: DependencyObject
    {
        public GridCondition()
        {
            this.ConditionType = GridConditionType.Equals;
            this.PredicateType = PredicateType.Or;
        }

        private object _value;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value 
        { 
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public GridConditionType ConditionType { get; set; }

        public PredicateType PredicateType { get; set; }
    }

#if WinRT
    public enum PredicateType
    {
        And = 0,

        Or = 1,
    }
#endif
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridConditionalFormat : DependencyObject
    {

        public static readonly DependencyProperty ConditionsProperty = DependencyProperty.Register("Conditions", typeof(ObservableCollection<GridCondition>), typeof(GridConditionalFormat), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the conditions.
        /// </summary>
        /// <value>The conditions.</value>
        public ObservableCollection<GridCondition> Conditions
        {
            get
            {
                return (ObservableCollection<GridCondition>)this.GetValue(GridConditionalFormat.ConditionsProperty);
            }
            set
            {
                this.SetValue(GridConditionalFormat.ConditionsProperty, value);
            }
        }

        public static readonly DependencyProperty StyleProperty = DependencyProperty.Register("Style", typeof(GridStyleInfo), typeof(GridConditionalFormat), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        public GridStyleInfo Style
        {
            get
            {
                return (GridStyleInfo)this.GetValue(GridConditionalFormat.StyleProperty);
            }
            set
            {
                this.SetValue(GridConditionalFormat.StyleProperty, value);
            }
        }

        /// <summary>
        /// Used to change the row and column index in the formula
        /// </summary>
        public RowColumnIndex Cell
        {
            get;
            set;
        }

        internal GridStyleInfoStore defaultStyleStore { get; set; }

        #region Formulas

        private GridFormulaTag formulaTag;

        public GridFormulaTag FormulaTag
        {
            get
            {
                return formulaTag;
            }
            set
            {
                formulaTag = value;
            }
        }

        public bool HasFormulaTag
        {
            get
            {
                return FormulaTag != null;
            }
        }

        private string formulaText;

        public string FormulaText
        {
            get
            {
                return formulaText;
            }
            set
            {
                formulaText = value;
            }
        }

        #endregion

        [XmlIgnore]
        internal bool IsDirty
        {
            get;
            set;
        }

        public GridConditionalFormat()
        {
            this.Conditions = new ObservableCollection<GridCondition>();
            this.defaultStyleStore = new GridStyleInfoStore();
            this.Conditions.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnConditionsCollectionChanged);
            this.IsDirty = true;
            this.Cell = RowColumnIndex.Empty;
        }
        
        void OnConditionsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.IsDirty = true;
        }

        public void InitializeFrom(GridConditionalFormat other)
        {
            this.Conditions = other.Conditions;
            this.IsDirty = other.IsDirty;
            this.Style = other.Style;
        }

        private Delegate compiledDelg = null;
        private Type _compiledDelgType;
        /// <summary>
        /// Gets the compiled delegate.
        /// </summary>
        /// <param name="type">The Type.</param>
        /// <returns></returns>
        internal Delegate GetCompiledDelegate(GridStyleInfo style, Type type)
        {
            if (compiledDelg == null || _compiledDelgType != type)
            {
                _compiledDelgType = type;
                ParameterExpression paramExp;
                String ApplyTo = string.Empty;
                if (style.ApplyConditionalFormatBasedOn == ApplyConditionalBasedOn.FormulaValue)
                {
                    ApplyTo = "FormulaConditionalFormat";
                }
                else
                {
                    ApplyTo = "CellValue";
                }
                if (type == typeof(int))
                {
                    paramExp = System.Linq.Expressions.Expression.Parameter(typeof(int), ApplyTo);
                }
                else if (type == typeof(double))
                {
                    paramExp = System.Linq.Expressions.Expression.Parameter(typeof(double), ApplyTo);
                }
                else if (type == typeof(bool))
                {
                    paramExp = System.Linq.Expressions.Expression.Parameter(typeof(bool), ApplyTo);
                }
                else
                {
                    paramExp = System.Linq.Expressions.Expression.Parameter(typeof(string), ApplyTo);
                }
                
                //ObservableCollection<GridCondition> Conditions = ConvertStringToCondition(this.Condition);
                System.Linq.Expressions.Expression predicate = null;
                var firstLoop = false;
                if (style.ApplyConditionalFormatBasedOn == ApplyConditionalBasedOn.CellValue && Conditions.Count > 0)
                {
                    foreach (var condition in Conditions)
                    {
                        if (condition.Value != null)
                        {
                            if (!firstLoop)
                            {
                                predicate = this.Predicate(paramExp, ApplyTo, condition.ConditionType, condition.Value);
                                firstLoop = true;
                            }
                            else
                            {
                                if (condition.PredicateType == PredicateType.And)
                                {
#if !WinRT
                                    predicate = predicate.AndAlsoPredicate(this.Predicate(paramExp, ApplyTo, condition.ConditionType, condition.Value));
#else
                                    predicate = Expression.AndAlso(this.Predicate(paramExp, ApplyTo, condition.ConditionType, condition.Value),null);
#endif

                                }
                                else if (condition.PredicateType == PredicateType.Or)
                                {
#if !WinRT
                                    predicate = predicate.OrElsePredicate(this.Predicate(paramExp, ApplyTo, condition.ConditionType, condition.Value));
#else
                                    predicate = Expression.OrElse(this.Predicate(paramExp, ApplyTo, condition.ConditionType, condition.Value),null);
#endif
                                }
                            }
                        }
                    }
                }
                else if(style.ApplyConditionalFormatBasedOn == ApplyConditionalBasedOn.FormulaValue)
                {
                    predicate = this.Predicate(paramExp, ApplyTo, GridConditionType.Equals, "TRUE");
                }
                if (predicate != null)
                {
                    var lambda = System.Linq.Expressions.Expression.Lambda(predicate, paramExp);
                    this.compiledDelg = lambda.Compile();
                }
            }
            return compiledDelg;
        }

        /// <summary>
        /// Predicates the specified param expression.
        /// </summary>
        /// <param name="paramExpression">The Parameter Expression.</param>
        /// <param name="propertyName">Name of the Property.</param>
        /// <param name="conditionType">Type of the ondition.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private System.Linq.Expressions.Expression Predicate(ParameterExpression paramExpression, string propertyName, GridConditionType conditionType, object value)
        {
            System.Linq.Expressions.Expression predicate = null;
#if WinRT
            var result = this.ChangeType(value, paramExpression.Type);
#else
            var result = NullableHelperInternal.ChangeType(value, paramExpression.Type);
#endif
            if (paramExpression.Type == typeof(string))
            {
                result = (result as string).Trim('\"');
            }
            switch (conditionType)
            {
                case GridConditionType.Equals:
                    predicate = System.Linq.Expressions.Expression.Equal(paramExpression, System.Linq.Expressions.Expression.Constant(result, paramExpression.Type));
                    break;

                case GridConditionType.GreaterThan:
                    predicate = System.Linq.Expressions.Expression.GreaterThan(paramExpression, System.Linq.Expressions.Expression.Constant(result, paramExpression.Type));
                    break;
                case GridConditionType.GreaterThanOrEqual:
                    predicate = System.Linq.Expressions.Expression.GreaterThanOrEqual(paramExpression, System.Linq.Expressions.Expression.Constant(result, paramExpression.Type));
                    break;
                case GridConditionType.LessThan:
                    predicate = System.Linq.Expressions.Expression.LessThan(paramExpression, System.Linq.Expressions.Expression.Constant(result, paramExpression.Type));
                    break;
                case GridConditionType.LessThanOrEqual:
                    predicate = System.Linq.Expressions.Expression.LessThanOrEqual(paramExpression, System.Linq.Expressions.Expression.Constant(result, paramExpression.Type));
                    break;
                case GridConditionType.NotEquals:
                    predicate = System.Linq.Expressions.Expression.NotEqual(paramExpression, System.Linq.Expressions.Expression.Constant(result, paramExpression.Type));
                    break;
            }
            return predicate;
        }

        /// <summary>
        /// Converts the string to condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        private ObservableCollection<GridCondition> ConvertStringToCondition(string condition)
        {
            ObservableCollection<GridCondition> Conditions = new ObservableCollection<GridCondition>();
            condition = condition.Trim();
            condition = condition.Trim(new char[] { '(', ')', '{', '}', '[', ']' });
            condition = condition.Replace("&&", "AND");
            condition = condition.Replace("||", "OR");
            condition = condition.ToUpper();
            string temp = condition;
            condition = condition.Replace("VALUE", "");
            var strConditions = condition.Split(new string[] { "AND", "OR" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var item in strConditions)
            {
                int length = item.Length + 5;
                GridCondition con = new GridCondition();
                if (temp.Length != length && temp[length] == 'A')
                {
                    con.PredicateType = PredicateType.And;
                }
                else
                {
                    con.PredicateType = PredicateType.Or;
                }
                string eachcondition = item.Trim();
                if (eachcondition[0] == '<' && eachcondition[1] == '=')
                {
                    con.ConditionType = GridConditionType.LessThanOrEqual;
                    con.Value = item.Substring(2);
                }
                else if (eachcondition[0] == '>' && eachcondition[1] == '=')
                {
                    con.ConditionType = GridConditionType.GreaterThanOrEqual;
                    con.Value = item.Substring(2);
                }
                else if (eachcondition[0] == '<')
                {
                    con.ConditionType = GridConditionType.LessThan;
                    con.Value = item.Substring(1);
                }
                else if (eachcondition[0] == '>')
                {
                    con.ConditionType = GridConditionType.GreaterThan;
                    con.Value = item.Substring(1);
                }
                else if (eachcondition[0] == '=' && eachcondition[1] == '=')
                {
                    con.ConditionType = GridConditionType.Equals;
                    con.Value = item.Substring(2);
                }
                else if (eachcondition[0] == '!' && eachcondition[1] == '=')
                {
                    con.ConditionType = GridConditionType.NotEquals;
                    con.Value = item.Substring(2);
                }
                else
                {
                    return new ObservableCollection<GridCondition>();
                }
                Conditions.Add(con);
            }
            return Conditions;
        }

        public object ChangeType(object value, Type type)
        {
            Type nullableUnderlyingType = Nullable.GetUnderlyingType(type);
            if (nullableUnderlyingType != null)
            {
                if (value is string && nullableUnderlyingType != typeof(string))
                {
                    if (ValueConvert.IsEmpty((string)value))
                        return null;
                }

                value = ChangeType(value, nullableUnderlyingType);
                if (value==null)
                    return null;
                return value;
            }
            else
                return value;
        }
    }
}