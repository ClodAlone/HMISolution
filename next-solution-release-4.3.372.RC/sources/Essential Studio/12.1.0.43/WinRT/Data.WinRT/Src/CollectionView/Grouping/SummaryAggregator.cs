#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Syncfusion.Data.Extensions;
#if WPF
using System.Data;
#if !SyncfusionFramework3_5
using System.Threading.Tasks;
#endif
#elif WinRt
using System.Threading.Tasks;
#endif

namespace Syncfusion.Data
{

    internal class CountAggregate : ISummaryExpressionAggregate, ISummaryAdjustible, ISummaryAggregateForGroup
#if !SILVERLIGHT && !SyncfusionFramework3_5 && !WP && !WinRT
, ISummaryParallelizable
#endif
    {
        public CountAggregate()
        {
        }

        public int Count
        {
            get;
            set;
        }

#if WPF
        public Action<IEnumerable, string, PropertyDescriptor> CalculateAggregateFunc()
#else
        public Action<IEnumerable, string, PropertyInfo> CalculateAggregateFunc()
#endif
        {
            return (items, property, pd) =>
            {
                IQueryable queryable = items.AsQueryable();
#if !SILVERLIGHT && !SyncfusionFramework3_5 && !WP && !WinRT
                if (this.CanParallelize)
                {
                    var enumerator = items.GetEnumerator();
                    var hasItem = enumerator.MoveNext();
                    if (hasItem)
                    {
                        var parallelQuery = EnumerableExtensions.GetParallelQuery(items, enumerator.Current.GetType());
                        queryable = parallelQuery.AsQueryable();
                    }
                }
#endif
                if (pd.Name == "Count")
                    this.Count = queryable.Count();
                
            };
        }

#if WPF
        public Action<IEnumerable, string, PropertyDescriptor> CalculateAggregateFuncForGroup()
#else
        public Action<IEnumerable, string, PropertyInfo> CalculateAggregateFuncForGroup()
#endif
        {
            return (items, property, pd) =>
            {
                IQueryable queryable = items.AsQueryable();
#if !SILVERLIGHT && !SyncfusionFramework3_5 && !WP && !WinRT
                if (this.CanParallelize)
                {
                    var enumerator = items.GetEnumerator();
                    var hasItem = enumerator.MoveNext();
                    if (hasItem)
                    {
                        var parallelQuery = EnumerableExtensions.GetParallelQuery(items, enumerator.Current.GetType());
                        queryable = parallelQuery.AsQueryable();
                    }
                }
#endif
                if (pd.Name == "Count")
                {
                    this.Count = Convert.ToInt32(queryable.Sum("Count"));
                }
            };
        }

        #region ISummaryExpressionAggregate Members

#if WPF
        public Action<IEnumerable, string, Expression<Func<string, object, object>>, PropertyDescriptor> CalculateAggregateExpressionFunc()
#else
        public Action<IEnumerable, string, Expression<Func<string, object, object>>, PropertyInfo> CalculateAggregateExpressionFunc()
#endif
        {
            return (items, property, expressionFunc, pd) =>
            {
                if (pd.Name == "Count")
                {
                    this.Count = items.AsQueryable().Count();
                }
            };
        }

        #endregion

        #region ISummaryParallelizable Members

#if !SILVERLIGHT && !SyncfusionFramework3_5 && !WP && !WinRT
        public bool CanParallelize
        {
            get;
            set;
        }
#endif

        #endregion

        #region ISummaryAdjustible interface

        public object AdjustSummaryCalculation(object currentValue, object oldValue, object newValue, string propertyName)
        {
            this.Count = (Int32)currentValue;
            if (oldValue != null)
                this.Count--;
            if (newValue != null)
                this.Count++;
            return this.Count;
        }

        #endregion

    }

    internal class Int32Aggregate : ISummaryExpressionAggregate, ISummaryAdjustible , ISummaryAggregateForGroup
#if !SILVERLIGHT && !SyncfusionFramework3_5 && !WP && !WinRT
, ISummaryParallelizable
#endif
    {
        public Int32Aggregate()
        {
        }

        public int Count
        {
            get;
            set;
        }

        public int Max
        {
            get;
            set;
        }

        public int Min
        {
            get;
            set;
        }

        public int Average
        {
            get;
            set;
        }

        public int Sum
        {
            get;
            set;
        }

        #region ISummaryAggregate Members

#if WPF
        public Action<IEnumerable, string, PropertyDescriptor> CalculateAggregateFunc()
#else
        public Action<IEnumerable, string, PropertyInfo> CalculateAggregateFunc()
#endif
        {
            return (items, property, pd) =>
            {
                var aggregateType = pd.Name;
                var queryable = items.AsQueryable();
#if !SILVERLIGHT && !SyncfusionFramework3_5 && !WP && !WinRT
                if (this.CanParallelize)
                {
                    var enumerator = items.GetEnumerator();
                    var hasItem = enumerator.MoveNext();
                    if (hasItem)
                    {
                        var parallelQuery = EnumerableExtensions.GetParallelQuery(items, enumerator.Current.GetType());
                        queryable = parallelQuery.AsQueryable();
                    }
                }
#endif
                switch (aggregateType)
                {
                    case "Count":
                        this.Count = queryable.Count();
                        break;
                    case "Max":
                        this.Max = Convert.ToInt32(queryable.Max(property));
                        break;
                    case "Min":
                        this.Min = Convert.ToInt32(queryable.Min(property));
                        break;
                    case "Average":
                        this.Average = Convert.ToInt32(queryable.Average(property));
                        break;
                    case "Sum":
                        this.Sum = Convert.ToInt32(queryable.Sum(property));
                        break;
                }
            };
        }

        #endregion

#if WPF
        public Action<IEnumerable, string, PropertyDescriptor> CalculateAggregateFuncForGroup()
#else
        public Action<IEnumerable, string, PropertyInfo> CalculateAggregateFuncForGroup()
#endif
        {
            return (items, property, pd) =>
            {
                var aggregateType = pd.Name;
                var queryable = items.AsQueryable();
#if !SILVERLIGHT && !SyncfusionFramework3_5 && !WP && !WinRT
                if (this.CanParallelize)
                {
                    var enumerator = items.GetEnumerator();
                    var hasItem = enumerator.MoveNext();
                    if (hasItem)
                    {
                        var parallelQuery = EnumerableExtensions.GetParallelQuery(items, enumerator.Current.GetType());
                        queryable = parallelQuery.AsQueryable();
                    }
                }
#endif
                switch (aggregateType)
                {
                    case "Count":
                        this.Count = Convert.ToInt32(queryable.Sum("Count"));
                        break;
                    case "Max":
                        this.Max = Convert.ToInt32(queryable.Max(property));
                        break;
                    case "Min":
                        this.Min = Convert.ToInt32(queryable.Min(property));
                        break;
                    case "Average":
                        this.Average = Convert.ToInt32(queryable.Average(property));
                        break;
                    case "Sum":
                        this.Sum = Convert.ToInt32(queryable.Sum(property));
                        break;
                }
            };
        }

        #region ISummaryExpressionAggregate Members

#if WPF
        public Action<IEnumerable, string, Expression<Func<string, object, object>>, PropertyDescriptor> CalculateAggregateExpressionFunc()
#else
        public Action<IEnumerable, string, Expression<Func<string, object, object>>, PropertyInfo> CalculateAggregateExpressionFunc()
#endif
        {
            return (items, propertyName, expressionFunc, pd) =>
            {
                var aggregateType = pd.Name;
                var queryable = items.AsQueryable();
#if !SILVERLIGHT && !SyncfusionFramework3_5 && !WP && !WinRT
                if (this.CanParallelize)
                {
                    var enumerator = items.GetEnumerator();
                    var hasItem = enumerator.MoveNext();
                    if (hasItem)
                    {
                        var parallelQuery = EnumerableExtensions.GetParallelQuery(items, enumerator.Current.GetType());
                        queryable = parallelQuery.AsQueryable();
                    }
                }
#endif
                switch (aggregateType)
                {
                    case "Count":
                        this.Count = queryable.Count();
                        break;
                    case "Max":
                        this.Max = Convert.ToInt32(queryable.Max(propertyName, expressionFunc));
                        break;
                    case "Min":
                        this.Min = Convert.ToInt32(queryable.Min(propertyName, expressionFunc));
                        break;
                    case "Sum":
                        this.Sum = Convert.ToInt32(queryable.Sum(propertyName, expressionFunc));
                        break;
                    case "Average":
                        this.Average = Convert.ToInt32(queryable.Average(propertyName, expressionFunc));
                        break;
                }
            };
        }

        #endregion

        #region ISummaryParallelizable Members
#if !SILVERLIGHT && !SyncfusionFramework3_5 && !WP && !WinRT
        public bool CanParallelize
        {
            get;
            set;
        }
#endif
        #endregion

        #region Summary Adjustible

        public object AdjustSummaryCalculation(object currentValue, object oldValue, object newValue, string propertyName)
        {
            switch (propertyName)
            {
                case "Count":
                    this.Count = Convert.ToInt32(currentValue);
                    if (oldValue != null)
                        this.Count--;
                    if (newValue != null)
                        this.Count++;
                    return this.Count;

                case "Max":
                    this.Max = Convert.ToInt32(currentValue);
                    return this.Max;

                case "Min":
                    this.Min = Convert.ToInt32(currentValue);
                    return this.Min;

                case "Average":
                    this.Average = Convert.ToInt32(currentValue);
                    if (oldValue != null)
                        this.Average -= Convert.ToInt32(oldValue);
                    if (newValue != null)
                        this.Average += Convert.ToInt32(newValue);
                    return this.Average;

                case "Sum":
                    this.Sum = Convert.ToInt32(currentValue);
                    if (oldValue != null)
                        this.Sum -= Convert.ToInt32(oldValue);
                    if (newValue != null)
                        this.Sum += Convert.ToInt32(newValue);
                    return this.Sum;
            }
            return 0;
        }

        #endregion

        
    }

    internal class DoubleAggregate : ISummaryExpressionAggregate, ISummaryAdjustible , ISummaryAggregateForGroup
#if !SILVERLIGHT && !SyncfusionFramework3_5 && !WP && !WinRT
, ISummaryParallelizable
#endif
    {
        public DoubleAggregate()
        {
        }

        public int Count
        {
            get;
            set;
        }

        public double Max
        {
            get;
            set;
        }

        public double Min
        {
            get;
            set;
        }

        public double Average
        {
            get;
            set;
        }

        public double Sum
        {
            get;
            set;
        }

#if WPF
        public Action<IEnumerable, string, PropertyDescriptor> CalculateAggregateFunc()
#else
        public Action<IEnumerable, string, PropertyInfo> CalculateAggregateFunc()
#endif
        {
            return (items, property, pd) =>
            {
                var aggregateType = pd.Name;
                var queryable = items.AsQueryable();
#if !SILVERLIGHT && !SyncfusionFramework3_5 && !WP && !WinRT
                if (this.CanParallelize)
                {
                    var enumerator = items.GetEnumerator();
                    var hasItem = enumerator.MoveNext();
                    if (hasItem)
                    {
                        var parallelQuery = EnumerableExtensions.GetParallelQuery(items, enumerator.Current.GetType());
                        queryable = parallelQuery.AsQueryable();
                    }
                }
#endif
                switch (aggregateType)
                {
                    case "Count":
                        this.Count = queryable.Count();
                        break;
                    case "Max":
                        this.Max = Convert.ToDouble(queryable.Max(property));
                        break;
                    case "Min":
                        this.Min = Convert.ToDouble(queryable.Min(property));
                        break;
                    case "Average":
                        this.Average = Convert.ToDouble(queryable.Average(property));
                        break;
                    case "Sum":
                        this.Sum = Convert.ToDouble(queryable.Sum(property));
                        break;
                }
            };
        }

#if WPF
        public Action<IEnumerable, string, PropertyDescriptor> CalculateAggregateFuncForGroup()
#else
        public Action<IEnumerable, string, PropertyInfo> CalculateAggregateFuncForGroup()
#endif
        {
            return (items, property, pd) =>
            {
                var aggregateType = pd.Name;
                var queryable = items.AsQueryable();
#if !SILVERLIGHT && !SyncfusionFramework3_5 && !WP && !WinRT
                if (this.CanParallelize)
                {
                    var enumerator = items.GetEnumerator();
                    var hasItem = enumerator.MoveNext();
                    if (hasItem)
                    {
                        var parallelQuery = EnumerableExtensions.GetParallelQuery(items, enumerator.Current.GetType());
                        queryable = parallelQuery.AsQueryable();
                    }
                }
#endif
                switch (aggregateType)
                {
                    case "Count":
                        this.Count = Convert.ToInt32(queryable.Sum("Count"));
                        break;
                    case "Max":
                        this.Max = Convert.ToDouble(queryable.Max(property));
                        break;
                    case "Min":
                        this.Min = Convert.ToDouble(queryable.Min(property));
                        break;
                    case "Average":
                        this.Average = Convert.ToDouble(queryable.Average(property));
                        break;
                    case "Sum":
                        this.Sum = Convert.ToDouble(queryable.Sum(property));
                        break;
                }
            };
        }

        #region ISummaryExpressionAggregate Members

#if WPF
        public Action<IEnumerable, string, Expression<Func<string, object, object>>, PropertyDescriptor> CalculateAggregateExpressionFunc()
#else
        public Action<IEnumerable, string, Expression<Func<string, object, object>>, PropertyInfo> CalculateAggregateExpressionFunc()
#endif
        {
            return (items, propertyName, expressionFunc, pd) =>
            {
                var aggregateType = pd.Name;
                var queryable = items.AsQueryable();
#if !SILVERLIGHT && !SyncfusionFramework3_5 && !WP && !WinRT
                if (this.CanParallelize)
                {
                    var enumerator = items.GetEnumerator();
                    var hasItem = enumerator.MoveNext();
                    if (hasItem)
                    {
                        var parallelQuery = EnumerableExtensions.GetParallelQuery(items, enumerator.Current.GetType());
                        queryable = parallelQuery.AsQueryable();
                    }
                }
#endif
                switch (aggregateType)
                {
                    case "Count":
                        this.Count = queryable.Count();
                        break;
                    case "Max":
                        this.Max = Convert.ToDouble(queryable.Max(propertyName, expressionFunc));
                        break;
                    case "Min":
                        this.Min = Convert.ToDouble(queryable.Min(propertyName, expressionFunc));
                        break;
                    case "Sum":
                        this.Sum = Convert.ToDouble(queryable.Sum(propertyName, expressionFunc));
                        break;
                    case "Average":
                        this.Average = Convert.ToDouble(queryable.Average(propertyName, expressionFunc));
                        break;
                }
            };
        }

        #endregion

        #region ISummaryParallelizable Members

#if !SILVERLIGHT && !SyncfusionFramework3_5 && !WP && !WinRT
        public bool CanParallelize
        {
            get;
            set;
        }
#endif
        #endregion

        #region Summary Adjustible

        public object AdjustSummaryCalculation(object currentValue, object oldValue, object newValue, string propertyName)
        {
            switch (propertyName)
            {
                case "Count":
                    this.Count = Convert.ToInt16(currentValue);
                    if (oldValue != null)
                        this.Count--;
                    if (newValue != null)
                        this.Count++;
                    return this.Count;

                case "Max":
                    this.Max = Convert.ToDouble(currentValue);
                    if (newValue != null && this.Max < (double)newValue)
                        this.Max = Convert.ToDouble(newValue);
                    return this.Max;

                case "Min":
                    this.Min = Convert.ToDouble(currentValue);
                    if (newValue != null && this.Min < (double)newValue)
                        this.Min = Convert.ToDouble(newValue);
                    return this.Min;

                case "Average":
                    this.Average = Convert.ToDouble(currentValue);
                    if (oldValue != null)
                        this.Average -= Convert.ToDouble(oldValue);
                    if (newValue != null)
                        this.Average += Convert.ToDouble(newValue);
                    return this.Average;

                case "Sum":
                    this.Sum = Convert.ToDouble(currentValue);
                    if (oldValue != null)
                        this.Sum -= Convert.ToDouble(oldValue);
                    if (newValue != null)
                        this.Sum += Convert.ToDouble(newValue);
                    return this.Sum;
            }
            return 0;
        }

        #endregion


        
    }

#if WPF
    #region DataTable summary support

    /// <summary>
    /// <code lang="C#">internal class GridDataTableCountAggregate :
    /// GridDataTableAggregator, ISummaryAggregate
    ///     {
    ///         public GridDataTableCountAggregate(DataTable table)
    ///             : base(table)
    ///         {
    ///         }</code>
    /// <para></para>
    /// <para></para>
    /// <para></para>        public int Count 
    /// <para></para>
    /// <para></para>        { 
    /// <para></para>
    /// <para></para>            get; 
    /// <para></para>
    /// <para></para>            set; 
    /// <para></para>
    /// <para></para>        } 
    /// <para></para>
    /// <para></para>
    /// <para>      <code lang="C#">  #region IGridDataSummaryAggregate Members
    /// </code></para>
    /// <para></para>
    /// <para></para>
    /// <para></para>        public Action&lt;IEnumerable, string,
    /// PropertyDescriptor&gt; CalculateAggregateFunc() 
    /// <para></para>
    /// <para></para>        { 
    /// <para></para>
    /// <para></para>            return (items, property, pd) =&gt; 
    /// <para></para>
    /// <para></para>            { 
    /// <para></para>
    /// <para></para>                var table = this.GetClonedTable(items); 
    /// <para></para>
    /// <para></para>                if (pd.Name == &quot;Count&quot;) 
    /// <para></para>
    /// <para></para>               <code lang="C#"> { </code>
    /// <para></para>
    /// <para></para>                    this.Count =
    /// (int)table.Compute(string.Format(&quot;Count({0})&quot;, property),
    /// string.Empty); 
    /// <para></para>
    /// <para></para>                } 
    /// <para></para>
    /// <para></para>            }; 
    /// <para></para>
    /// <para></para>        } 
    /// <para></para>
    /// <para></para>
    /// <para></para>        #endregion 
    /// <para></para>
    /// <para></para>    }
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public class DataTableAggregator
    {
        public DataTableAggregator(DataTable table)
        {
            this.Table = table;
        }

        public DataTable Table
        {
            get;
            private set;
        }

        protected DataTable GetClonedTable(IEnumerable items, DataTable Dtable)
        {
            var table = Dtable.Clone();
            if (this.IsRowView(items))
            {
                foreach (DataRowView rView in items)
                {
                    table.ImportRow(rView.Row);
                }
            }
            else
            {
                foreach (DataRow row in items)
                {
                    table.ImportRow(row);
                }
            }
            return table;
        }

        private bool IsRowView(IEnumerable items)
        {
            var enumerator = items.GetEnumerator();
            if (enumerator.MoveNext())
            {
                return (enumerator.Current as DataRowView) != null;
            }
            return false;
        }
    }

    internal class DataTableCountAggregate : DataTableAggregator, ISummaryAggregate, ISummaryAdjustible, ISummaryAggregateForGroup
    {
        public DataTableCountAggregate(DataTable table)
            : base(table)
        {
        }

        public int Count
        {
            get;
            set;
        }

        #region IGridDataSummaryAggregate Members

        public Action<IEnumerable, string, PropertyDescriptor> CalculateAggregateFunc()
        {
            return (items, property, pd) =>
            {
                var table = this.GetClonedTable(items, (items.AsQueryable().ElementAt(0) as DataRowView).DataView.Table);
                var name = (items.AsQueryable().ElementAt(0) as DataRowView).DataView.Table.TableName;
                if (pd.Name == "Count")
                {
                    var value = table.Compute(string.Format("Count([{0}])", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Count = Convert.ToInt32(value);
                    }
                }
            };
        }

        #endregion

        public Action<IEnumerable, string, PropertyDescriptor> CalculateAggregateFuncForGroup()
        {
            return (items, property, pd) =>
            {
                var table = this.GetClonedTable(items, (items.AsQueryable().ElementAt(0) as DataRowView).DataView.Table);
                var name = (items.AsQueryable().ElementAt(0) as DataRowView).DataView.Table.TableName;
                if (pd.Name == "Count")
                {
                    var value = table.Compute(string.Format("Sum({0})", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Count = Convert.ToInt32(value);
                    }
                }
            };
        }

        public object AdjustSummaryCalculation(object currentValue, object oldValue, object newValue, string propertyName)
        {
            this.Count = (Int32)currentValue;
            if (oldValue != null)
                this.Count--;
            if (newValue != null)
                this.Count++;
            return this.Count;
        }


    }

    internal class DataTableInt32Aggregate : DataTableAggregator, ISummaryAggregate, ISummaryAdjustible, ISummaryAggregateForGroup
    {
        public DataTableInt32Aggregate(DataTable table)
            : base(table)
        {
        }

        public int Count
        {
            get;
            set;
        }

        public int Max
        {
            get;
            set;
        }

        public int Min
        {
            get;
            set;
        }

        public int Average
        {
            get;
            set;
        }

        public int Sum
        {
            get;
            set;
        }

        #region IGridDataSummaryAggregate Members

        public Action<IEnumerable, string, PropertyDescriptor> CalculateAggregateFunc()
        {
            return (items, property, pd) =>
            {
                var table = this.GetClonedTable(items, (items.AsQueryable().ElementAt(0) as DataRowView).DataView.Table);
                var name = (items.AsQueryable().ElementAt(0) as DataRowView).DataView.Table.TableName;
                if (pd.Name == "Count")
                {
                    var value = table.Compute(string.Format("Count({0})", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Count = Convert.ToInt32(value);
                    }
                }
                else if (pd.Name == "Max")
                {
                    var value = table.Compute(string.Format("Max({0})", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Max = Convert.ToInt32(value);
                    }
                }
                else if (pd.Name == "Min")
                {
                    var value = table.Compute(string.Format("Min({0})", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Min = Convert.ToInt32(value);
                    }
                }
                else if (pd.Name == "Average")
                {
                    var value = table.Compute(string.Format("Avg({0})", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Average = Convert.ToInt32(value);
                    }
                }
                else if (pd.Name == "Sum")
                {
                    var value = table.Compute(string.Format("Sum({0})", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Sum = Convert.ToInt32(value);
                    }
                }
            };
        }

        #endregion

        public Action<IEnumerable, string, PropertyDescriptor> CalculateAggregateFuncForGroup()
        {
            return (items, property, pd) =>
            {
                var table = this.GetClonedTable(items, (items.AsQueryable().ElementAt(0) as DataRowView).DataView.Table);
                var name = (items.AsQueryable().ElementAt(0) as DataRowView).DataView.Table.TableName;

                if (pd.Name == "Count")
                {
                    var value = table.Compute(string.Format("Sum({0})", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Count = Convert.ToInt32(value);
                    }
                }
                else if (pd.Name == "Max")
                {
                    var value = table.Compute(string.Format("Max({0})", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Max = Convert.ToInt32(value);
                    }
                }
                else if (pd.Name == "Min")
                {
                    var value = table.Compute(string.Format("Min({0})", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Min = Convert.ToInt32(value);
                    }
                }
                else if (pd.Name == "Average")
                {
                    var value = table.Compute(string.Format("Avg({0})", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Average = Convert.ToInt32(value);
                    }
                }
                else if (pd.Name == "Sum")
                {
                    var value = table.Compute(string.Format("Sum({0})", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Sum = Convert.ToInt32(value);
                    }
                }
            };
        }

        public object AdjustSummaryCalculation(object currentValue, object oldValue, object newValue, string propertyName)
        {
            switch (propertyName)
            {
                case "Count":
                    this.Count = Convert.ToInt16(currentValue);
                    if (oldValue != null)
                        this.Count--;
                    if (newValue != null)
                        this.Count++;
                    return this.Count;

                case "Max":
                    this.Max = Convert.ToInt32(currentValue);
                    return this.Max;

                case "Min":
                    this.Min = Convert.ToInt32(currentValue);
                    return this.Min;

                case "Average":
                    this.Average = Convert.ToInt32(currentValue);
                    if (oldValue != null)
                        this.Average -= Convert.ToInt32(currentValue);
                    if (newValue != null)
                        this.Average += Convert.ToInt32(newValue);
                    return this.Average;

                case "Sum":
                    this.Sum = Convert.ToInt32(currentValue);
                    if (oldValue != null)
                        this.Sum -= Convert.ToInt32(oldValue);
                    if (newValue != null)
                        this.Sum += Convert.ToInt32(newValue);
                    return this.Sum;
            }
            return 0;
        }
    }

    internal class DataTableDoubleAggregate : DataTableAggregator, ISummaryAggregate, ISummaryAdjustible, ISummaryAggregateForGroup
    {
        public DataTableDoubleAggregate(DataTable table)
            : base(table)
        {
        }

        public double Count
        {
            get;
            set;
        }

        public double Max
        {
            get;
            set;
        }

        public double Min
        {
            get;
            set;
        }

        public double Average
        {
            get;
            set;
        }

        public double Sum
        {
            get;
            set;
        }

        #region ISummaryAggregate Members
        public Action<IEnumerable, string, PropertyDescriptor> CalculateAggregateFunc()
        {
            return (items, property, pd) =>
            {
                var table = this.GetClonedTable(items, (items.AsQueryable().ElementAt(0) as DataRowView).DataView.Table);
                var name = (items.AsQueryable().ElementAt(0) as DataRowView).DataView.Table.TableName;
                if (pd.Name == "Count")
                {
                    var value = table.Compute(string.Format("Count([{0}])", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Count = Convert.ToDouble(value);
                    }
                }
                else if (pd.Name == "Max")
                {
                    var value = table.Compute(string.Format("Max([{0}])", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Max = Convert.ToDouble(value);
                    }
                }
                else if (pd.Name == "Min")
                {
                    var value = table.Compute(string.Format("Min([{0}])", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Min = Convert.ToDouble(value);
                    }
                }
                else if (pd.Name == "Average")
                {
                    var value = table.Compute(string.Format("Avg([{0}])", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Average = Convert.ToDouble(value);
                    }
                }
                else if (pd.Name == "Sum")
                {
                    var value = table.Compute(string.Format("Sum([{0}])", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Sum = Convert.ToDouble(value);
                    }
                }
            };
        }

        #endregion


        public Action<IEnumerable, string, PropertyDescriptor> CalculateAggregateFuncForGroup()
        {
            return (items, property, pd) =>
            {
                var table = this.GetClonedTable(items, (items.AsQueryable().ElementAt(0) as DataRowView).DataView.Table);
                var name = (items.AsQueryable().ElementAt(0) as DataRowView).DataView.Table.TableName;
                if (pd.Name == "Count")
                {
                    var value = table.Compute(string.Format("Sum({0})", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Count = Convert.ToDouble(value);
                    }
                }
                else if (pd.Name == "Max")
                {
                    var value = table.Compute(string.Format("Max([{0}])", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Max = Convert.ToDouble(value);
                    }
                }
                else if (pd.Name == "Min")
                {
                    var value = table.Compute(string.Format("Min([{0}])", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Min = Convert.ToDouble(value);
                    }
                }
                else if (pd.Name == "Average")
                {
                    var value = table.Compute(string.Format("Avg([{0}])", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Average = Convert.ToDouble(value);
                    }
                }
                else if (pd.Name == "Sum")
                {
                    var value = table.Compute(string.Format("Sum([{0}])", property), string.Empty);
                    if (value != DBNull.Value)
                    {
                        this.Sum = Convert.ToDouble(value);
                    }
                }
            };
        }

        public object AdjustSummaryCalculation(object currentValue, object oldValue, object newValue, string propertyName)
        {
            switch (propertyName)
            {
                case "Count":
                    this.Count = Convert.ToInt16(currentValue);
                    if (oldValue != null)
                        this.Count--;
                    if (newValue != null)
                        this.Count++;
                    return this.Count;

                case "Max":
                    this.Max = Convert.ToDouble(currentValue);
                    return this.Max;

                case "Min":
                    this.Min = Convert.ToDouble(currentValue);
                    return this.Min;

                case "Average":
                    this.Average = Convert.ToDouble(currentValue);
                    if (oldValue != null)
                        this.Average -= Convert.ToDouble(currentValue);
                    if (newValue != null)
                        this.Average += Convert.ToDouble(newValue);
                    return this.Average;

                case "Sum":
                    this.Sum = Convert.ToDouble(currentValue);
                    if (oldValue != null)
                        this.Sum -= Convert.ToDouble(oldValue);
                    if (newValue != null)
                        this.Sum += Convert.ToDouble(newValue);
                    return this.Sum;
            }
            return 0;
        }
    }

    #endregion
#endif

}
