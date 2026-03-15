#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !Orubase
namespace Syncfusion.Windows.Data
#else 
namespace Orubase.Linq.Data
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
#if !Orubase
    using Syncfusion.Linq;
#else 
    using Orubase.Linq;
#endif

    /// <summary>
    /// Holds values for Filtering.
    /// </summary>
    public class FilterPredicate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterPredicate"/> class.
        /// </summary>
        public FilterPredicate()
        {

        }

        /// <summary>
        /// Gets or sets the type of the filter.
        /// </summary>
        /// <value>The type of the filter.</value>
        public FilterType FilterType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the filter value.
        /// </summary>
        /// <value>The filter value.</value>
        public object FilterValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the predicate.
        /// </summary>
        /// <value>The type of the predicate.</value>
        public PredicateType PredicateType
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the type of the filterbehavior.
        /// </summary>
        /// <value>The type of the predicate.</value>
        public FilterBehavior FilterBehavior
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is case sensitive.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is case sensitive; otherwise, <c>false</c>.
        /// </value>
        public bool IsCaseSensitive
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Predicate type defined for Filters.
    /// </summary>
    public enum PredicateType
    {
        /// <summary>
        /// Does an AND operation on filters.
        /// </summary>
        And,
        /// <summary>
        /// Does an OR operation on filters.
        /// </summary>
        Or
    }
    /// <summary>
    /// Default Filter Operator is defined for Filters.
    /// </summary>
    public enum FilterOperatorType
        {
        /// <summary>
        /// Does an Equals operation on filters.
        /// </summary>
        Equals,
        /// <summary>
        /// Does an StartsWith operation on filters.
        /// </summary>
        StartsWith,
        /// <summary>
        /// Does an Contains operation on filters.
        /// </summary>
        Contains
        }
}
