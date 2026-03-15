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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections.ObjectModel;
using Syncfusion.Linq;

    /// <summary>
    /// Specifies properties for filtering with <see cref="ICollectionViewAdv"/>.
    /// </summary>
    public interface IFilterDefinition
    {
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
        /// Gets the filters.
        /// </summary>
        /// <value>The filters.</value>
        ObservableCollection<FilterPredicate> Filters
        {
            get;
        }

        FilterBehavior FilterBehavior
        {
            get;
            set;
        }
    }
}
