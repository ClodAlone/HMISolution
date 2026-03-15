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
using Syncfusion.Linq;
using Syncfusion.Windows.Data;

namespace Syncfusion.Windows.Controls.Grid
{
    internal static class FilterHelpers
    {
        /// <summary>
        /// Gets the type of the predicate.
        /// </summary>
        /// <param name="filterType">Type of the filter.</param>
        /// <returns></returns>
        internal static PredicateType GetPredicateType(String filterType)
        {
            PredicateType predicate = (PredicateType)Enum.Parse(typeof(PredicateType), filterType, true);
            return predicate;

        }

        /// <summary>
        /// Gets the type of the filter.
        /// </summary>
        /// <param name="filterType">Type of the filter.</param>
        /// <returns></returns>
        internal static FilterType GetFilterType(String filterType)
        {
            if (filterType == GridDataResourceWrapper.EqualsSmall)
            {
                return FilterType.Equals;
            }
            else if (filterType == GridDataResourceWrapper.DoesNotEquals)
                return FilterType.NotEquals;
            else if (filterType == GridDataResourceWrapper.IsGreaterThan)
                return FilterType.GreaterThan;
            else if (filterType == GridDataResourceWrapper.IsGreaterThanOrEqualto)
                return FilterType.GreaterThanOrEqual;
            else if (filterType == GridDataResourceWrapper.IsLessThan)
                return FilterType.LessThan;
            else if (filterType == GridDataResourceWrapper.IsLessThanorEqualto)
                return FilterType.LessThanOrEqual;
            else if (filterType == GridDataResourceWrapper.BeginsWith)
                return FilterType.StartsWith;
            else if (filterType == GridDataResourceWrapper.DoesNotBeginsWith)
                return FilterType.StartsWith;
            else if (filterType == GridDataResourceWrapper.EndsWithSmall)
                return FilterType.EndsWith;
            else if (filterType == GridDataResourceWrapper.DoesNotEndWith)
                return FilterType.EndsWith;
            else if (filterType == GridDataResourceWrapper.ContainsSmall)
                return FilterType.Contains;
            else if (filterType == GridDataResourceWrapper.DoesNotContain)
                return FilterType.Contains;
            else if (filterType == GridDataResourceWrapper.isbefore)
                return FilterType.LessThan;
            else if (filterType == GridDataResourceWrapper.isbeforeorequalto)
                return FilterType.LessThanOrEqual;
            else if (filterType == GridDataResourceWrapper.isafter)
                return FilterType.GreaterThan;
            else if (filterType == GridDataResourceWrapper.isafterorequalto)
                return FilterType.GreaterThanOrEqual;
            return FilterType.Equals;
        }

    }     
}
