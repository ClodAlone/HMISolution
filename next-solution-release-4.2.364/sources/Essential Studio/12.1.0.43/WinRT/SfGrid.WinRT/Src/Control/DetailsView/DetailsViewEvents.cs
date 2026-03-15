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
using System.Linq;
using System.Text;

namespace Syncfusion.UI.Xaml.Grid
{
    public class GridDetailsViewExpandingEventArgs : GridCancelEventArgs
    {
        public GridDetailsViewExpandingEventArgs(object dataGrid)
            : base(dataGrid)
        {

        }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>The record.</value>
        public object Record { get; internal set; }

        /// <summary>
        /// Gets or sets the DetailsViewItemsSource.
        /// </summary>
        /// <value>Key: RelationalColumn.</value>
        /// <value>Value: DetailsViewItemsSource.</value>
        public Dictionary<string, IEnumerable> DetailsViewItemsSource { get; internal set; }
    }

    /// <summary>
    /// Provides the delegate for <see cref="DetailsViewExpanding"/> event.
    /// </summary>
    public delegate void GridDetailsViewExpandingEventHandler(object sender, GridDetailsViewExpandingEventArgs e);

    public class GridDetailsViewExpandedEventArgs : GridEventArgs
    {
        public GridDetailsViewExpandedEventArgs(object dataGrid)
            : base(dataGrid)
        {

        }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>The record.</value>
        public object Record { get; internal set; }

        /// <summary>
        /// Gets or sets the DetailsViewItemsSource.
        /// </summary>
        /// <value>Key: RelationalColumn.</value>
        /// <value>Value: DetailsViewItemsSource.</value>
        public Dictionary<string, IEnumerable> DetailsViewItemsSource { get; set; }
    }

    /// <summary>
    /// Provides the delegate for <see cref="DetailsViewExpanded"/> event.
    /// </summary>
    public delegate void GridDetailsViewExpandedEventHandler(object sender, GridDetailsViewExpandedEventArgs e);

    public class GridDetailsViewCollapsingEventArgs : GridCancelEventArgs
    {
        public GridDetailsViewCollapsingEventArgs(object dataGrid)
            : base(dataGrid)
        {

        }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>The record.</value>
        public object Record { get; internal set; }
    }

    /// <summary>
    /// Provides the delegate for <see cref="DetailsViewCollapsing"/> event.
    /// </summary>
    public delegate void GridDetailsViewCollapsingEventHandler(object sender, GridDetailsViewCollapsingEventArgs e);

    public class GridDetailsViewCollapsedEventArgs : GridEventArgs
    {
        public GridDetailsViewCollapsedEventArgs(object dataGrid)
            : base(dataGrid)
        {

        }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>The record.</value>
        public object Record { get; internal set; }
    }

    /// <summary>
    /// Provides the delegate for <see cref="DetailsViewCollapsed"/> event.
    /// </summary>
    public delegate void GridDetailsViewCollapsedEventHandler(object sender, GridDetailsViewCollapsedEventArgs e);

    public class AutoGeneratingRelationsArgs : GridCancelEventArgs
    {
        public GridViewDefinition GridViewDefinition { get; set; }

        public AutoGeneratingRelationsArgs(GridViewDefinition gridView, object originalSource)
            : base(originalSource)
        {
            GridViewDefinition = gridView;
        }
    }
    public delegate void AutoGeneratingRelationsEventHandler(object sender, AutoGeneratingRelationsArgs e);
}
