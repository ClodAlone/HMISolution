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
using Syncfusion.Linq;
using System.ComponentModel;
using Syncfusion.Windows.Data;
using System.Xml.Serialization;
using System.Data;
using Syncfusion.Windows.Shared;
using System.Collections.Specialized;
using Syncfusion.Windows.Controls.Cells;

namespace Syncfusion.Windows.Controls.Grid
{
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

    [Serializable]
    public class GridCondition: Freezable
    {
        public GridCondition()
        {
            this.ConditionType = GridConditionType.Equals;
            this.PredicateType = PredicateType.Or;
        }

        private object _value;

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

        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class GridConditionalFormat: Freezable
    {
        public static readonly DependencyProperty ConditionsProperty = DependencyProperty.Register("Conditions", typeof(FreezableCollection<GridCondition>), typeof(GridConditionalFormat), new PropertyMetadata(null));

        public FreezableCollection<GridCondition> Conditions
        {
            get
            {
                return (FreezableCollection<GridCondition>)this.GetValue(GridConditionalFormat.ConditionsProperty);
            }
            set
            {
                this.SetValue(GridConditionalFormat.ConditionsProperty, value);
            }
        }

        public static readonly DependencyProperty StyleProperty = DependencyProperty.Register("Style", typeof(GridStyleInfo), typeof(GridConditionalFormat), new PropertyMetadata(null));

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
        /// Adjust the row and column index in formula
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

        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }

        public GridConditionalFormat()
        {

            this.Conditions = new FreezableCollection<GridCondition>();
            this.defaultStyleStore = new GridStyleInfoStore();
            ((INotifyCollectionChanged)this.Conditions).CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnConditionsCollectionChanged);
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
        internal Delegate GetCompiledDelegate(GridStyleInfo style, Type type)
        {
            if (compiledDelg == null)
            {
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

                //(WPF-12563)below code is commented to avoid type cast exception when trying convert double into int.
                //if (type == typeof(int))
                //{
                //    paramExp = System.Linq.Expressions.Expression.Parameter(typeof(int), ApplyTo);
                //}
                if (type == typeof(double) || type == typeof(int))
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
                            if (paramExp.Type == typeof(string) && (condition.ConditionType != GridConditionType.Equals || condition.ConditionType != GridConditionType.NotEquals))
                                continue;
                            if (!firstLoop)
                            {
                                predicate = this.Predicate(paramExp, ApplyTo, condition.ConditionType, condition.Value);
                                firstLoop = true;
                            }
                            else
                            {
                                if (condition.PredicateType == PredicateType.And)
                                {
                                    predicate = predicate.AndAlsoPredicate(this.Predicate(paramExp, ApplyTo, condition.ConditionType, condition.Value));
                                }
                                else if (condition.PredicateType == PredicateType.Or)
                                {
                                    predicate = predicate.OrElsePredicate(this.Predicate(paramExp, ApplyTo, condition.ConditionType, condition.Value));
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

        private System.Linq.Expressions.Expression Predicate(ParameterExpression paramExpression, string propertyName, GridConditionType conditionType, object value)
        {
            System.Linq.Expressions.Expression predicate = null;
            var result = NullableHelperInternal.ChangeType(value, paramExpression.Type);
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
    }
}
