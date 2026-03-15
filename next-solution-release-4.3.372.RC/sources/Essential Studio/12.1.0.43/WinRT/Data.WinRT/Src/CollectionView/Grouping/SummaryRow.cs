#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Data.Extensions;

namespace Syncfusion.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using System.Linq.Expressions;
#if WPF
    using System.Data;
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
        //bool IsVisible
        //{
        //    get;
        //    set;
        //}

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
        //int TitleColumnCount
        //{
        //    get;
        //    set;
        //}
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

    public interface ISummaryAggregate
    {
#if WPF
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

    internal interface ISummaryAggregateForGroup
    {
#if WPF
        /// <summary>
        /// Calculates the aggregate func.
        /// </summary>
        /// <returns></returns>
        Action<IEnumerable, string, PropertyDescriptor> CalculateAggregateFuncForGroup();
#else
        /// <summary>
        /// Calculates the aggregate func.
        /// </summary>
        /// <returns></returns>
        Action<IEnumerable, string, PropertyInfo> CalculateAggregateFuncForGroup();
#endif
    }

    public interface ISummaryAdjustible
    {
        object AdjustSummaryCalculation(object currentValue, object oldValue, object newValue, string propertyName);
    }

    public interface ISummaryExpressionAggregate : ISummaryAggregate
    {
#if WPF
        /// <summary>
        /// Calculates the aggregate expression func.
        /// </summary>
        /// <returns></returns>
        Action<IEnumerable, string, Expression<Func<string, object, object>>, PropertyDescriptor> CalculateAggregateExpressionFunc();
#else
        /// <summary>
        /// Calculates the aggregate expression func.
        /// </summary>
        /// <returns></returns>
        Action<IEnumerable, string, Expression<Func<string, object, object>>, PropertyInfo> CalculateAggregateExpressionFunc();
#endif
    }

#if !SILVERLIGHT && !SyncfusionFramework3_5
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



}
