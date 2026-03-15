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
#if !SILVERLIGHT
    using System.Data;
    using Syncfusion.Windows.Shared;
    using System.Collections.Specialized;
#else
    using System.Reflection;
#endif

    /// <summary>
    /// Specifies the type of condition to be used in Conditional Formatting.
    /// </summary>
    public enum GridDataConditionType
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

    /// <summary>
    /// Define the conditional format setting for the <see cref="GridDataControl"/>.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class GridDataConditionalFormat
#if !SILVERLIGHT
        : Freezable
#else
 : DependencyObject
#endif

    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataConditionalFormat"/> class.
        /// </summary>
        public GridDataConditionalFormat()
        {
#if !SILVERLIGHT
            this.Conditions = new FreezableCollection<GridDataCondition>();
            ((INotifyCollectionChanged)this.Conditions).CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnConditionsCollectionChanged);
#else
            this.Conditions = new ObservableCollection<GridDataCondition>();
            this.Conditions.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnConditionsCollectionChanged);
#endif
            this.IsDirty = true;
            
        }

        /// <summary>
        /// Initializes from an other instance of <see cref="GridDataConditionalFormat"/>. This creates a cloned copy of the original object instance.
        /// </summary>
        /// <param name="other">The other.</param>
        public void InitializeFrom(GridDataConditionalFormat other)
        {
            this.ApplyStyleToColumn = other.ApplyStyleToColumn;
            this.Conditions = other.Conditions;
            this.IsDirty = other.IsDirty;
            this.Name = other.Name;
            this.Style = other.Style;
        }

        void OnConditionsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.IsDirty = true;
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataConditionalFormat.Name"/> property.
        /// </summary>
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(GridDataConditionalFormat), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return (string)this.GetValue(GridDataConditionalFormat.NameProperty);
            }

            set
            {
                this.SetValue(GridDataConditionalFormat.NameProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataConditionalFormat.Style"/> property.
        /// </summary>
        public static readonly DependencyProperty StyleProperty = DependencyProperty.Register("Style", typeof(GridDataStyleInfo), typeof(GridDataConditionalFormat), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="GridDataStyleInfo"/> for the specified condition.
        /// </summary>
        /// <value>The style.</value>
        public GridDataStyleInfo Style
        {
            get
            {
                return (GridDataStyleInfo)this.GetValue(GridDataConditionalFormat.StyleProperty);
            }

            set
            {
                this.SetValue(GridDataConditionalFormat.StyleProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataConditionalFormat.ApplyStyleToColumn"/> property.
        /// </summary>
        public static readonly DependencyProperty ApplyStyleToColumnProperty = DependencyProperty.Register("ApplyStyleToColumn", typeof(string), typeof(GridDataConditionalFormat), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the apply style to column name specified.
        /// </summary>
        /// <value>The apply style to column.</value>
        public string ApplyStyleToColumn
        {
            get
            {
                return (string)this.GetValue(GridDataConditionalFormat.ApplyStyleToColumnProperty);
            }

            set
            {
                this.SetValue(GridDataConditionalFormat.ApplyStyleToColumnProperty, value);
            }
        }

#if !SILVERLIGHT

        public static readonly DependencyProperty ConditionsProperty = DependencyProperty.Register("Conditions", typeof(FreezableCollection<GridDataCondition>), typeof(GridDataConditionalFormat));

        /// <summary>
        /// Gets or sets the conditions.
        /// </summary>
        /// <value>The conditions.</value>
        public FreezableCollection<GridDataCondition> Conditions
        {
            get
            {
                return (FreezableCollection<GridDataCondition>)GetValue(GridDataConditionalFormat.ConditionsProperty);
            }
            set
            {
                SetValue(GridDataConditionalFormat.ConditionsProperty, value);
            }
        }
#else

               /// <summary>
        /// Gets or sets the conditions.
        /// </summary>
        /// <value>The conditions.</value>
        public ObservableCollection<GridDataCondition> Conditions
        {
            get;
            set;
        }


#endif

        [XmlIgnore]
        public GridDataTableModel Model
        {
            get;
            private set;
        }

        internal void SetTableModel(GridDataTableModel model)
        {
            this.Model = model;
        }

        [XmlIgnore]
        internal bool IsDirty
        {
            get;
            set;
        }

#if !SILVERLIGHT
        internal Delegate GetCompiledDelegateForDataRow()
        {
            if (this.compiledDelg == null)
            {
                this.IsDirty = true;
            }

            if (this.IsDirty && this.Model != null && this.Model.SourceListCount > 0 && this.Conditions.Count > 0)
            {
                System.Linq.Expressions.Expression predicate = null;
                var firstLoop = false;
                var paramExp = System.Linq.Expressions.Expression.Parameter(typeof(DataRow), "r");
                foreach (var condition in this.Conditions)
                {
                    var dataView = this.Model.View.SourceCollection as DataView;
                    var table = dataView.Table;
                    var colIndex = table.Columns.IndexOf(condition.ColumnName);
                    bool check = ((condition.ConditionType != GridDataConditionType.Equals && condition.ConditionType != GridDataConditionType.NotEquals)) && table.Columns[colIndex].DataType == typeof(string);
                    if (!check)
                    {
                        if (!firstLoop)
                        {
                            predicate = Predicate(paramExp, colIndex, condition.ConditionType, condition.Value);
                            firstLoop = true;
                        }
                        else
                        {
                            if (condition.PredicateType == PredicateType.And)
                            {
                                predicate = predicate.AndPredicate(Predicate(paramExp, colIndex, condition.ConditionType, condition.Value));
                            }
                            else if (condition.PredicateType == PredicateType.Or)
                            {
                                predicate = predicate.OrPredicate(Predicate(paramExp, colIndex, condition.ConditionType, condition.Value));
                            }
                        }
                    }
                }

                var lambda = System.Linq.Expressions.Expression.Lambda(predicate, paramExp);
                this.compiledDelg = lambda.Compile();
                this.IsDirty = false;
            }

            return this.compiledDelg;
        }
#endif

        private Delegate compiledDelg = null;
        internal Delegate GetCompiledDelegate()
        {
            if (this.compiledDelg == null)
            {
                this.IsDirty = true;
            }
            
            if (this.IsDirty && this.Model != null && this.Model.SourceListCount > 0 && this.Conditions.Count > 0)
            {
                ParameterExpression paramExp = null;// = System.Linq.Expressions.Expression.Parameter(typeof(DataRow), "r");

                var underlyingRecordType = this.Model.TableProperties.SourceType;
                if (underlyingRecordType == null)
                    underlyingRecordType = ((GridDataRecord)this.Model.View.Records[0]).Data.GetType();

                paramExp = underlyingRecordType.Parameter();
                System.Linq.Expressions.Expression predicate = null;
                var firstLoop = false;
                foreach (var condition in this.Conditions)
                {
//#if SyncfusionFramework4_0

#if !SILVERLIGHT && SyncfusionFramework4_0
                    bool dynamicproperties= this.Model.TableProperties.IsDynamicItemsSource || (typeof(ICustomTypeDescriptor).IsAssignableFrom(underlyingRecordType));
#elif !SILVERLIGHT && SyncfusionFramework3_5
                    bool dynamicproperties= (typeof(ICustomTypeDescriptor).IsAssignableFrom(underlyingRecordType));
#else
                    bool dynamicproperties = this.Model.TableProperties.IsDynamicItemsSource;
#endif
                    var expressionFunc = dynamicproperties ? (this.Model.View as CollectionViewAdv).GetExpressionFunc(condition.ColumnName) :
                        this.Model.GetUnboundExpressionFunc(condition.ColumnName);
                    var VisibleColumn = this.Model.TableProperties.VisibleColumns[condition.ColumnName];
                    Type VisibleColumnType = null;
                    if (VisibleColumn != null)
                        VisibleColumnType = VisibleColumn.VisibleColumnType != null ? VisibleColumn.VisibleColumnType : VisibleColumn.ColumnType;

                    if (!firstLoop)
                    {
                        if (dynamicproperties)
                            predicate = this.Predicate(paramExp, condition.ColumnName, condition.ConditionType, condition.Value, expressionFunc, dynamicproperties, VisibleColumnType);
                        else
                            predicate = this.Predicate(paramExp, condition.ColumnName, condition.ConditionType, condition.Value, expressionFunc);
                        firstLoop = true;
                    }
                    else
                    {
                        if (condition.PredicateType == PredicateType.And)
                        {
                            if (dynamicproperties)
                                predicate = predicate.AndAlsoPredicate(this.Predicate(paramExp, condition.ColumnName, condition.ConditionType, condition.Value, expressionFunc, dynamicproperties, VisibleColumnType));
                            else
                                predicate = predicate.AndAlsoPredicate(this.Predicate(paramExp, condition.ColumnName, condition.ConditionType, condition.Value, expressionFunc));
                        }
                        else if (condition.PredicateType == PredicateType.Or)
                        {
                            if (dynamicproperties)
                                predicate = predicate.OrElsePredicate(this.Predicate(paramExp, condition.ColumnName, condition.ConditionType, condition.Value, expressionFunc, dynamicproperties, VisibleColumnType));
                            else
                                predicate = predicate.OrElsePredicate(this.Predicate(paramExp, condition.ColumnName, condition.ConditionType, condition.Value, expressionFunc));
                        }
                    }
                }

                var lambda = System.Linq.Expressions.Expression.Lambda(predicate, paramExp);
                this.compiledDelg = lambda.Compile();
               
                this.IsDirty = false;
            }

            return this.compiledDelg;
        }

        internal void ResetCompiledDelegate()
        {
            this.compiledDelg = null;
        }
#if !SILVERLIGHT
        static class DataRowExpressionBuilder
        {
            public static T GetValueForDataRow<T>(DataRow row, int colIndex)
            {
                var value = row[colIndex];
                return value != DBNull.Value ? (T)value : default(T);
            }
        }

        private static System.Linq.Expressions.Expression GetItemArrayExpression(int colIndex, ParameterExpression p)
        {
            var getItemArrayMethod = typeof(DataRow).GetMethods().FirstOrDefault(m => m.Name == "get_ItemArray");
            var property = System.Linq.Expressions.Expression.Property(p, getItemArrayMethod);
            var colIndexExp = System.Linq.Expressions.Expression.Constant(colIndex, typeof(int));
            var exp = System.Linq.Expressions.Expression.ArrayIndex(property, colIndexExp);
            exp = System.Linq.Expressions.Expression.NotEqual(exp, System.Linq.Expressions.Expression.Constant(DBNull.Value));
            return exp;
        }

        private System.Linq.Expressions.Expression Predicate(ParameterExpression paramExpression, int colIndex, GridDataConditionType conditionType, object value)
        {
            System.Linq.Expressions.Expression predicate = null;
            DataView dataView = this.Model.View.SourceCollection as DataView;
            var table = dataView.Table;
            var type = table.Columns[colIndex].DataType;
            value = Syncfusion.Windows.ComponentModel.NullableHelper.ChangeType(value, type);
            var valueForDataRowMethod = typeof(DataRowExpressionBuilder).GetMethods().FirstOrDefault(m => m.Name == "GetValueForDataRow" && m.IsStatic && m.IsGenericMethod);
            var genericWrapper = valueForDataRowMethod.MakeGenericMethod(new Type[] { type });
            var methodCallExp = System.Linq.Expressions.Expression.Call(genericWrapper, new System.Linq.Expressions.Expression[] { paramExpression, System.Linq.Expressions.Expression.Constant(colIndex) });
            switch (conditionType)
            {
                case GridDataConditionType.Equals:
                    {
                        var getItemArrayMethod = typeof(DataRow).GetMethods().FirstOrDefault(m => m.Name == "get_ItemArray");
                        var objectToString = typeof(object).GetMethods().FirstOrDefault(m => m.Name == "ToString");
                        var stringEquals = typeof(string).GetMethods().FirstOrDefault(m => m.Name == "op_Equality");
                        var ex1 = System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression.ArrayIndex(System.Linq.Expressions.Expression.Property(paramExpression, getItemArrayMethod), System.Linq.Expressions.Expression.Constant(colIndex)), objectToString, new System.Linq.Expressions.Expression[0]);
                        var ex2 = System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression.Constant(value), objectToString, new System.Linq.Expressions.Expression[0]);
                        predicate = System.Linq.Expressions.Expression.Equal(ex1, ex2, false, stringEquals);
                    }
                    break;
                case GridDataConditionType.GreaterThan:
                    {
                        var result = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Constant(value), type);
                        var expression2 = System.Linq.Expressions.Expression.Convert(result, type);
                        expression2 = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Constant(value), type);
                        predicate = System.Linq.Expressions.Expression.GreaterThan(methodCallExp, expression2);
                    }
                    break;
                case GridDataConditionType.GreaterThanOrEqual:
                    {
                        var result = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Constant(value), type);
                        var expression2 = System.Linq.Expressions.Expression.Convert(result, type);
                        expression2 = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Constant(value), type);
                        predicate = System.Linq.Expressions.Expression.GreaterThanOrEqual(methodCallExp, expression2);
                    }
                    break;
                case GridDataConditionType.LessThan:
                    {
                        var result = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Constant(value), type);
                        var expression2 = System.Linq.Expressions.Expression.Convert(result, type);
                        expression2 = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Constant(value), type);
                        predicate = System.Linq.Expressions.Expression.LessThan(methodCallExp, expression2);
                    }
                    break;
                case GridDataConditionType.LessThanOrEqual:
                    {
                        var result = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Constant(value), type);
                        var expression2 = System.Linq.Expressions.Expression.Convert(result, type);
                        expression2 = System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Constant(value), type);
                        predicate = System.Linq.Expressions.Expression.LessThanOrEqual(methodCallExp, expression2);
                    }
                    break;
                case GridDataConditionType.NotEquals:
                    {
                        var getItemArrayMethod = typeof(DataRow).GetMethods().FirstOrDefault(m => m.Name == "get_ItemArray");
                        var objectToString = typeof(object).GetMethods().FirstOrDefault(m => m.Name == "ToString");
                        var stringEquals = typeof(string).GetMethods().FirstOrDefault(m => m.Name == "op_Inequality");
                        var ex1 = System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression.ArrayIndex(System.Linq.Expressions.Expression.Property(paramExpression, getItemArrayMethod), System.Linq.Expressions.Expression.Constant(colIndex)), objectToString, new System.Linq.Expressions.Expression[0]);
                        var ex2 = System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression.Constant(value), objectToString, new System.Linq.Expressions.Expression[0]);
                        predicate = System.Linq.Expressions.Expression.NotEqual(ex1, ex2, false, stringEquals);
                    }
                    break;
            }

            return predicate;
        }
#endif

        private bool ConditionValueIsColumn(object conditionValue, out string columnName)
        {
            var result = false;
            columnName = string.Empty;
            if (conditionValue != null && conditionValue.ToString() != string.Empty)
            {
                var value = conditionValue.ToString();
                if (value.Contains("["))
                {
                    var startIdx = value.IndexOf("[");
                    var column = value.Substring(startIdx + 1, value.LastIndexOf("]") - 1);
                    var itemProperties = this.Model.View.GetItemProperties();
                    var pd = itemProperties.GetPropertyDescriptor(column);
                    if (pd != null)
                    {
                        columnName = column;
                        result = true;
                    }
                }
            }
            return result;
        }

        private System.Linq.Expressions.Expression Predicate(ParameterExpression paramExpression, string propertyName, GridDataConditionType conditionType, object value, Expression<Func<string, object, object>> recordExpression)
        {
            return Predicate(paramExpression, propertyName, conditionType, value, recordExpression, false, null);
        }
        private System.Linq.Expressions.Expression Predicate(ParameterExpression paramExpression, string propertyName, GridDataConditionType conditionType, object value, Expression<Func<string, object, object>> recordExpression, bool IsDynamicItemSource, Type ColumnType)
        {
            System.Linq.Expressions.Expression predicate = null;
            string columName2 = string.Empty;
            if (!this.ConditionValueIsColumn(value, out columName2))
            {
                switch (conditionType)
                {
                    case GridDataConditionType.Equals:
                        if (recordExpression == null)
                        {
                            predicate = paramExpression.Equal(propertyName, value);
                        }
                        else
                        {
                            if (!IsDynamicItemSource || ColumnType == null)
                            {
                                var type = this.Model.GetUnboundType(propertyName);
                                predicate = paramExpression.Equal(propertyName, value, type, recordExpression);
                            }
                            else
                                predicate = paramExpression.Equal(propertyName, value, ColumnType, recordExpression);
                        }
                        break;
                    case GridDataConditionType.GreaterThan:
                        if (recordExpression == null)
                        {
                            predicate = paramExpression.GreaterThan(propertyName, value);
                        }
                        else
                        {
                            if (!IsDynamicItemSource || ColumnType == null)
                            {
                                var type = this.Model.GetUnboundType(propertyName);
                                predicate = paramExpression.GreaterThan(propertyName, value, type, recordExpression);
                            }
                            else
                                predicate = paramExpression.GreaterThan(propertyName, value, ColumnType, recordExpression);
                        }
                        break;
                    case GridDataConditionType.GreaterThanOrEqual:
                        if (recordExpression == null)
                        {
                            predicate = paramExpression.GreaterThanOrEqual(propertyName, value);
                        }
                        else
                        {
                            if (!IsDynamicItemSource || ColumnType == null)
                            {
                                var type = this.Model.GetUnboundType(propertyName);
                                predicate = paramExpression.GreaterThanOrEqual(propertyName, value, type, recordExpression);
                            }
                            else
                                predicate = paramExpression.GreaterThanOrEqual(propertyName, value, ColumnType, recordExpression);
                        }
                        break;
                    case GridDataConditionType.LessThan:
                        if (recordExpression == null)
                        {
                            predicate = paramExpression.LessThan(propertyName, value);
                        }
                        else
                        {
                            if (!IsDynamicItemSource || ColumnType == null)
                            {
                                var type = this.Model.GetUnboundType(propertyName);
                                predicate = paramExpression.LessThan(propertyName, value, type, recordExpression);
                            }
                            else
                                predicate = paramExpression.LessThan(propertyName, value, ColumnType, recordExpression);
                        }
                        break;
                    case GridDataConditionType.LessThanOrEqual:
                        if (recordExpression == null)
                        {
                            predicate = paramExpression.LessThanOrEqual(propertyName, value);
                        }
                        else
                        {
                            if (!IsDynamicItemSource || ColumnType == null)
                            {
                                var type = this.Model.GetUnboundType(propertyName);
                                predicate = paramExpression.LessThanOrEqual(propertyName, value, type, recordExpression);
                            }
                            else
                                predicate = paramExpression.LessThanOrEqual(propertyName, value, ColumnType, recordExpression);
                        }
                        break;
                    case GridDataConditionType.NotEquals:
                        if (recordExpression == null)
                        {
                            predicate = paramExpression.NotEqual(propertyName, value);
                        }
                        else
                        {
                            if (!IsDynamicItemSource || ColumnType == null)
                            {
                                var type = this.Model.GetUnboundType(propertyName);
                                predicate = paramExpression.NotEqual(propertyName, value, type, recordExpression);
                            }
                            else
                                predicate = paramExpression.NotEqual(propertyName, value, ColumnType, recordExpression);
                        }
                        break;
                    //case GridDataConditionType.Contains:
                    //    break;
                }
            }
            else
            {
                switch (conditionType)
                {
                    case GridDataConditionType.Equals:
                        predicate = paramExpression.Equal(propertyName, columName2);
                        break;
                    case GridDataConditionType.GreaterThan:
                        predicate = paramExpression.GreaterThan(propertyName, columName2);
                        break;
                    case GridDataConditionType.GreaterThanOrEqual:
                        predicate = paramExpression.GreaterThanOrEqual(propertyName, columName2);
                        break;
                    case GridDataConditionType.LessThan:
                        predicate = paramExpression.LessThan(propertyName, columName2);
                        break;
                    case GridDataConditionType.LessThanOrEqual:
                        predicate = paramExpression.LessThanOrEqual(propertyName, columName2);
                        break;
                    case GridDataConditionType.NotEquals:
                        predicate = paramExpression.NotEqual(propertyName, columName2);
                        break;
                    //case GridDataConditionType.Contains:
                    //    break;
                }
            }

            return predicate;
        }

#if !SILVERLIGHT 
        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
#endif
    }

    /// <summary>
    /// Define the condition settings for the <see cref="GridDataControl"/> to apply.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class GridDataCondition
#if !SILVERLIGHT
        : Freezable
#else
 : DependencyObject
#endif
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataCondition"/> class.
        /// </summary>
        public GridDataCondition()
        {
            this.ConditionType = GridDataConditionType.Equals;
            this.PredicateType = PredicateType.Or;
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataCondition.ConditionType"/> property.
        /// </summary>
        public static readonly DependencyProperty GridDataConditionTypeProperty = DependencyProperty.Register("GridDataConditionType", typeof(GridDataConditionType), typeof(GridDataCondition), new PropertyMetadata(GridDataConditionType.Equals));

        /// <summary>
        /// Gets or sets the type of the condition.
        /// </summary>
        /// <value>The type of the condition.</value>
        public GridDataConditionType ConditionType
        {
            get
            {
                return (GridDataConditionType)this.GetValue(GridDataCondition.GridDataConditionTypeProperty);
            }

            set
            {
                this.SetValue(GridDataCondition.GridDataConditionTypeProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataCondition.Value"/> property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(GridDataCondition), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get
            {
                return this.GetValue(GridDataCondition.ValueProperty);
            }

            set
            {
                this.SetValue(GridDataCondition.ValueProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="PredicateType"/> property.
        /// </summary>
        public static readonly DependencyProperty PredicateTypeProperty = DependencyProperty.Register("PredicateType", typeof(PredicateType), typeof(GridDataCondition), new PropertyMetadata(PredicateType.And));

        /// <summary>
        /// Gets or sets the type of the predicate.
        /// </summary>
        /// <value>The type of the predicate.</value>
        public PredicateType PredicateType
        {
            get
            {
                return (PredicateType)this.GetValue(GridDataCondition.PredicateTypeProperty);
            }

            set
            {
                this.SetValue(GridDataCondition.PredicateTypeProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataCondition.ColumnName"/> property.
        /// </summary>
        public static readonly DependencyProperty ColumnNameProperty = DependencyProperty.Register("ColumnName", typeof(string), typeof(GridDataCondition), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName
        {
            get
            {
                return (string)this.GetValue(GridDataCondition.ColumnNameProperty);
            }

            set
            {
                this.SetValue(GridDataCondition.ColumnNameProperty, value);
            }
        }

#if !SILVERLIGHT
        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
#endif
    }
}
