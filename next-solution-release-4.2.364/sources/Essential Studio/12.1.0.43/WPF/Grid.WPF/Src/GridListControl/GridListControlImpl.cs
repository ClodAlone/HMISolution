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
using System.Windows;
using Syncfusion.Windows.Collections;
using Syncfusion.Windows.GridCommon;

#if ENABLE_PARTIAL_TRUST
using System.Security;
#endif

namespace Syncfusion.Windows.Controls.Grid
{
#if ENABLE_PARTIAL_TRUST
    [SecuritySafeCritical]
#endif

    public class GridListControlImpl : GridControlBase
    {
        public GridListControlImpl()
        {
            Model = new GridListModel();
            this.Loaded += new RoutedEventHandler(GridListControlImpl_Loaded);
        }

        void GridListControlImpl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(GridListControlImpl_Loaded);
            this.Model.Options.ColumnSizer = (this.Model.DropDownColumnSizer == GridControlLengthUnitType.AutoOnLoad) ? GridControlLengthUnitType.Auto : (this.Model.DropDownColumnSizer == GridControlLengthUnitType.AutoOnLoadWithLastColumnFill)? GridControlLengthUnitType.AutoWithLastColumnFill: this.Model.DropDownColumnSizer;
            this.Model.Sizer.ListenToSizeChanged = false;
        }

        public new GridListModel Model
        {
            get { return (GridListModel)base.Model; }
            set { base.Model = value; }
        }

        protected override void WireModel()
        {
            base.WireModel();
            ColumnWidthsProvider = Model.Columns;
        }

        protected override void UnwireModel()
        {
            base.UnwireModel();
        }

        protected override void OnCurrentCellMoved(GridCurrentCellMovedEventArgs e)
        {
            Model.CurrentIndex = CurrentCell.RowIndex - 1;
            base.OnCurrentCellMoved(e);
        }

    }
}
