#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using Syncfusion.UI.Xaml.Collections.ComponentModel;
using Syncfusion.Data;

namespace Syncfusion.UI.Xaml.Grid
{

    public delegate void GridFilterEventHandler(object sender, GridFilterEventArgs e);

    public class GridFilterEventArgs : GridHandledEventArgs
    {

        public GridFilterEventArgs(GridColumn column, List<FilterPredicate> filterpredicates, object originalSource)
            : base(originalSource)
        {
            this.column = column;
            this.filterpredicates = filterpredicates;
        }

        private GridColumn column;
        public GridColumn Column
        {
            get
            {
                return column;
            }
        }

        private List<FilterPredicate> filterpredicates;
        public List<FilterPredicate> FilterPredicates
        {
            get
            {
                return filterpredicates;
            }
        }
    }



    public delegate void GridFilterItemsPopulatingEventHandler(object sender, GridFilterItemsPopulatingEventArgs e);


    public class GridFilterItemsPopulatingEventArgs : GridHandledEventArgs
    {

        public GridFilterItemsPopulatingEventArgs(IEnumerable<FilterElement> itemsSource, GridColumn column, GridFilterControl filterControl, object originalSource)
            : base(originalSource)
        {
            this.ItemsSource = itemsSource;
            this.column = column;
            this.FilterControl = filterControl;
        }

        private GridColumn column;
        public GridColumn Column
        {
            get
            {
                return column;
            }
        }

        public IEnumerable<FilterElement> ItemsSource { get; set; }

        public GridFilterControl FilterControl { get; internal set; }
    }


    public delegate void GridFilterItemsPopulatedEventHandler(object sender, GridFilterItemsPopulatedEventArgs e);


    public class GridFilterItemsPopulatedEventArgs : GridEventArgs
    {
        public GridFilterItemsPopulatedEventArgs(IEnumerable<FilterElement> itemsSource, GridColumn column, GridFilterControl filterControl, object originalSource)
            : base(originalSource)
        {
            this.ItemsSource = itemsSource;
            this.column = column;
            this.FilterControl = filterControl;
        }

        private GridColumn column;
        public GridColumn Column
        {
            get
            {
                return column;
            }
        }

        public IEnumerable<FilterElement> ItemsSource { get; set; }

        public GridFilterControl FilterControl { get; internal set; }
    }
}
