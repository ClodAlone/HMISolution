#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows.Data;
    using Syncfusion.Linq;
    using System.Reflection;
    using System.Linq.Expressions;
#if !SILVERLIGHT
    using System.Data;
#endif
#if !SyncfusionFramework4_0
    public delegate void Action<T1, T2, T3, T4, T5>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5);
#endif

    /// <summary>
    /// Common properties exposed through ISummaryRow for getting information on the
    /// summary values to be computed for the Groups in <see cref="TopLevelGroup"/>.
    /// </summary>
    public interface ISummaryRow
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        bool IsVisible
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show summary in row].
        /// </summary>
        /// <value><c>true</c> if [show summary in row]; otherwise, <c>false</c>.</value>
        bool ShowSummaryInRow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the summary columns.
        /// </summary>
        /// <value>The summary columns.</value>
        ObservableCollection<ISummaryColumn> SummaryColumns
        {
            get;
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the title column count.
        /// </summary>
        /// <value>The title column count.</value>
        int TitleColumnCount
        {
            get;
            set;
        }
    }

    public interface ISummaryColumn
    {
        /// <summary>
        /// Gets or sets the custom aggregate. Should implement <see cref="ISummaryAggregate" /> interface to delegate the custom summaries.
        /// </summary>
        /// <value>The custom aggregate.</value>
        ISummaryAggregate CustomAggregate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        string Format
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the mapping.
        /// </summary>
        /// <value>The name of the mapping.</value>
        string MappingName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the summary.
        /// </summary>
        /// <value>The type of the summary.</value>
        SummaryType SummaryType
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Different Enum types of SummaryType.
    /// </summary>
    public enum SummaryType
    {
        /// <summary>
        /// Specify the count aggregate for the <see cref="ISummaryColumn"/>.
        /// </summary>
        CountAggregate,
        /// <summary>
        /// Specify Double value aggregate for the <see cref="ISummaryColumn"/>.
        /// </summary>
        DoubleAggregate,
        /// <summary>
        /// Specify Int32 value aggregate for the <see cref="ISummaryColumn"/>.
        /// </summary>
        Int32Aggregate,
        /// <summary>
        /// Specify custom aggregate for the <see cref="ISummaryColumn"/>. Implement <see cref="ISummaryAggregate"/> or <see cref="ISummaryExpressionAggregate"/> interface to delegate the summary computation.
        /// </summary>
        Custom
    }

    /// <summary>
    /// Implement this interface to delegate the summary computation.
    /// <code lang="C#">    
    ///     class GridDataInt32Aggregate : ISummaryAggregate
    ///     {
    ///         public GridDataInt32Aggregate()
    ///         {
    ///         }
    /// 
    ///         public int Sum
    ///         {
    ///             get;
    ///             set;
    ///         }
    /// 
    ///         public Action&lt;IEnumerable, string,
    /// PropertyDescriptor&gt; CalculateAggregateFunc()
    ///         {
    ///             return (items, property, pd) =&gt;
    ///             {
    ///                 if (pd.Name == &quot;Sum&quot;)
    ///                     this.Sum =
    /// Convert.ToInt32(items.AsQueryable().Sum(property));
    ///             };
    ///         }
    ///     }
    /// </code>
    /// </summary>
    public interface ISummaryAggregate
    {
#if !SILVERLIGHT
        /// <summary>
        /// Calculates the aggregate func.
        /// </summary>
        /// <returns></returns>
        Action<IEnumerable, string, PropertyDescriptor> CalculateAggregateFunc();
#else
        /// <summary>
        /// Calculates the aggregate func.
        /// </summary>
        /// <returns></returns>
        Action<IEnumerable, string, PropertyInfo> CalculateAggregateFunc();
#endif
    }

#if !SILVERLIGHT
    public interface ISummaryAdjustible
    {
        void SetCurrentValue(object currentValue, string propertyName);
        void AdjustForOldContribution(object oldValue, string propertyName);
        void AdjustForNewContribution(object newValue, string propertyName);
        object GetCurrentValue(string propertyName);
    }
#endif

    /// <summary>
    /// Implement this interface to delegate custom summary computation. This interface
    /// also extends summary computation for unbound values.
    /// <code lang="C#">    
    ///     internal class CountAggregate : ISummaryExpressionAggregate
    ///     {
    ///         public CountAggregate()
    ///         {
    ///         }
    /// 
    ///         public int Count
    ///         {
    ///             get;
    ///             set;
    ///         }
    ///         #region ISummaryExpressionAggregate Members
    ///         public Action&lt;IEnumerable, string,
    /// Expression&lt;Func&lt;string, object, object&gt;&gt;, PropertyDescriptor&gt;
    /// CalculateAggregateExpressionFunc()
    ///         {
    ///             return (items, property, expressionFunc, pd) =&gt;
    ///             {
    ///                 if (pd.Name == &quot;Count&quot;)
    ///                 {
    ///                     this.Count = items.AsQueryable().Count();
    ///                 }
    ///             };
    ///         }
    /// 
    ///         #endregion
    ///     }
    /// </code>
    /// </summary>
    public interface ISummaryExpressionAggregate : ISummaryAggregate
    {
#if !SILVERLIGHT
        /// <summary>
        /// Calculates the aggregate expression func.
        /// </summary>
        /// <returns></returns>
        Action<IEnumerable, string, Expression<Func<string, object, object>>, Expression<Func<string, object, object>>, PropertyDescriptor> CalculateAggregateExpressionFunc();
               
#else
        /// <summary>
        /// Calculates the aggregate expression func.
        /// </summary>
        /// <returns></returns>
        Action<IEnumerable, string, Expression<Func<string, object, object>>, Expression<Func<string, object, object>>, PropertyInfo> CalculateAggregateExpressionFunc();
#endif
    }

#if !SILVERLIGHT && SyncfusionFramework4_0
    /// <summary>
    /// Implement this interface to know if the Summary expressions can use PLINQ
    /// </summary>
    internal interface ISummaryParallelizable
    {
        bool CanParallelize
        {
            get;
            set;
        }
    }
#endif

    #region Aggregate classes
    internal class CountAggregate : ISummaryExpressionAggregate
#if !SILVERLIGHT && SyncfusionFramework4_0
, ISummaryParallelizable
#endif
#if !SILVERLIGHT
        , ISummaryAdjustible
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

#if !SILVERLIGHT
        public Action<IEnumerable, string, PropertyDescriptor> CalculateAggregateFunc()
#else 
        public Action<IEnumerable, string, PropertyInfo> CalculateAggregateFunc()
#endif
        {
            return (items, property, pd) =>
            {
                var queryable = items.AsQueryable();
#if !SILVERLIGHT && SyncfusionFramework4_0
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
                    this.Count = queryable.Count();
                }
            };
        }

        #region ISummaryExpressionAggregate Members
#if !SILVERLIGHT
        public Action<IEnumerable, string, Expression<Func<string, object, object>>, Expression<Func<string, object, object>>, PropertyDescriptor> CalculateAggregateExpressionFunc()
#else
        public Action<IEnumerable, string, Expression<Func<string, object, object>>, Expression<Func<string, object, object>>, PropertyInfo> CalculateAggregateExpressionFunc()        
#endif
        {
            return (items, property, expressionFunc, typeFunc, pd) =>
            {
                if (pd.Name == "Count")
                {
                    this.Count = items.AsQueryable().Count();
                }
            };
        }

        #endregion

#if !SILVERLIGHT && SyncfusionFramework4_0
        #region ISummaryParallelizable Members

        public bool CanParallelize
        {
            get;
            set;
        }

        #endregion
#endif

        #region ISummaryAdjustible interface

        public void AdjustForOldContribution(object oldValue, string propertyName)
        {
            this.Count--;
        }

        public void AdjustForNewContribution(object newValue, string propertyName)
        {
            this.Count++;
        }

        public void SetCurrentValue(object currentValue, string propertyName)
        {
            if (propertyName == "Count")
            {
                this.Count = (int)currentValue;
            }
        }

        public object GetCurrentValue(string propertyName)
        {
            if (propertyName == "Count")
            {
                return this.Count;
            }

            return null;
        }
        
        #endregion
    }

    internal class Int32Aggregate : ISummaryExpressionAggregate
#if !SILVERLIGHT && SyncfusionFramework4_0
, ISummaryParallelizable
#endif
#if !SILVERLIGHT
        , ISummaryAdjustible
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

#if !SILVERLIGHT
        public Action<IEnumerable, string, PropertyDescriptor> CalculateAggregateFunc()
#else
        public Action<IEnumerable, string, PropertyInfo> CalculateAggregateFunc()
#endif
        {
            return (items, property, pd) =>
            {
                var aggregateType = pd.Name;
                var queryable = items.AsQueryable();
#if !SILVERLIGHT && SyncfusionFramework4_0
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

        #region ISummaryExpressionAggregate Members

#if !SILVERLIGHT
        public Action<IEnumerable, string, Expression<Func<string, object, object>>, Expression<Func<string, object, object>>, PropertyDescriptor> CalculateAggregateExpressionFunc()
#else
        public Action<IEnumerable, string, Expression<Func<string, object, object>>, Expression<Func<string, object, object>>, PropertyInfo> CalculateAggregateExpressionFunc()
#endif
        {
            return (items, propertyName, expressionFunc, typeFunc, pd) =>
            {
                var aggregateType = pd.Name;
                var queryable = items.AsQueryable();
#if !SILVERLIGHT && SyncfusionFramework4_0
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
                        this.Max = Convert.ToInt32(queryable.Max(propertyName, expressionFunc, typeFunc));
                        break;
                    case "Min":
                        this.Min = Convert.ToInt32(queryable.Min(propertyName, expressionFunc, typeFunc));
                        break;
                    case "Sum":
                        this.Sum = Convert.ToInt32(queryable.Sum(propertyName, expressionFunc, typeFunc));
                        break;
                    case "Average":
                        this.Average = Convert.ToInt32(queryable.Average(propertyName, expressionFunc, typeFunc));
                        break;
                }
            };
        }

        #endregion

#if !SILVERLIGHT && SyncfusionFramework4_0
        #region ISummaryParallelizable Members

        public bool CanParallelize
        {
            get;
            set;
        }

        #endregion
#endif

        #region Summary Adjustible
        public void SetCurrentValue(object currentValue, string propertyName)
        {
            switch (propertyName)
            {
                case "Count":
                    this.Count = (int)currentValue;
                    break;
                case "Max":
                    this.Max = (int)currentValue;
                    break;
                case "Min":
                    this.Min = (int)currentValue;
                    break;
                case "Average":
                    this.Average = (int)currentValue;
                    break;
                case "Sum":
                    this.Sum = (int)currentValue;
                    break;
            }
        }

        public void AdjustForOldContribution(object oldValue, string propertyName)
        {
            switch (propertyName)
            {
                case "Count":
                    this.Count--;
                    break;
                case "Max":                    
                    break;
                case "Sum":
                    this.Sum -= Convert.ToInt32(oldValue);
                    break;
            }
        }

        public void AdjustForNewContribution(object newValue, string propertyName)
        {
            switch (propertyName)
            {
                case "Sum":
                    this.Sum += Convert.ToInt32(newValue);
                    break;
            }
        }

        public object GetCurrentValue(string propertyName)
        {
            switch (propertyName)
            {
                case "Sum":
                    return this.Sum;
            }

            return 0;
        }
        
        #endregion
    }

    /// Count, Minimum, Maximum, Sum, and Average for double fields.
    internal class DoubleAggregate : ISummaryExpressionAggregate
#if !SILVERLIGHT && SyncfusionFramework4_0
, ISummaryParallelizable
#endif
#if !SILVERLIGHT
        , ISummaryAdjustible
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

#if !SILVERLIGHT
        public Action<IEnumerable, string, PropertyDescriptor> CalculateAggregateFunc()
#else
        public Action<IEnumerable, string, PropertyInfo> CalculateAggregateFunc()
#endif
        {
            return (items, property, pd) =>
            {
                var aggregateType = pd.Name;
                var queryable = items.AsQueryable();
#if !SILVERLIGHT && SyncfusionFramework4_0
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

        #region ISummaryExpressionAggregate Members

#if !SILVERLIGHT
        public Action<IEnumerable, string, Expression<Func<string, object, object>>, Expression<Func<string, object, object>>, PropertyDescriptor> CalculateAggregateExpressionFunc()
#else
        public Action<IEnumerable, string, Expression<Func<string, object, object>>, Expression<Func<string, object, object>>, PropertyInfo> CalculateAggregateExpressionFunc()
#endif
        {
            return (items, propertyName, expressionFunc, typeFunc, pd) =>
            {
                var aggregateType = pd.Name;
                var queryable = items.AsQueryable();
#if !SILVERLIGHT && SyncfusionFramework4_0
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
                        this.Max = Convert.ToDouble(queryable.Max(propertyName, expressionFunc, typeFunc));
                        break;
                    case "Min":
                        this.Min = Convert.ToDouble(queryable.Min(propertyName, expressionFunc, typeFunc));
                        break;
                    case "Sum":
                        this.Sum = Convert.ToDouble(queryable.Sum(propertyName, expressionFunc, typeFunc));
                        break;
                    case "Average":
                        this.Average = Convert.ToDouble(queryable.Average(propertyName, expressionFunc, typeFunc));
                        break;
                }
            };
        }

        #endregion

#if !SILVERLIGHT && SyncfusionFramework4_0
        #region ISummaryParallelizable Members

        public bool CanParallelize
        {
            get;
            set;
        }

        #endregion
#endif

#region Summary Adjustible

        public void SetCurrentValue(object currentValue, string propertyName)
        {
            switch (propertyName)
            {
                case "Count":
                    this.Count = (int)currentValue;
                    break;
                case "Max":
                    this.Max = (double)currentValue;
                    break;
                case "Min":
                    this.Min = (double)currentValue;
                    break;
                case "Average":
                    this.Average = (double)currentValue;
                    break;
                case "Sum":
                    this.Sum = (double)currentValue;
                    break;
            }
        }

        public void AdjustForOldContribution(object oldValue, string propertyName)
        {
            switch (propertyName)
            {
                case "Count":
                    this.Count--;
                    break;
                case "Max":
                    break;
                case "Sum":
                    this.Sum -= Convert.ToDouble(oldValue);
                    break;
            }
        }

        public void AdjustForNewContribution(object newValue, string propertyName)
        {
            switch (propertyName)
            {
                case "Sum":
                    this.Sum += Convert.ToDouble(newValue);
                    break;
            }
        }

        public object GetCurrentValue(string propertyName)
        {
            switch (propertyName)
            {
                case "Sum":
                    return this.Sum;
            }

            return 0;
        }

#endregion

    }
    #endregion
#if !SILVERLIGHT
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

        protected DataTable GetClonedTable(IEnumerable items)
        {
            var table = this.Table.Clone();
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

    internal class DataTableCountAggregate : DataTableAggregator, ISummaryAggregate
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
                var table = this.GetClonedTable(items);
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
    }

    internal class DataTableInt32Aggregate : DataTableAggregator, ISummaryAggregate
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
                var table = this.GetClonedTable(items);
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
    }

    internal class DataTableDoubleAggregate : DataTableAggregator, ISummaryAggregate
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
                var table = this.GetClonedTable(items);
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
    }

    #endregion

#endif
}
