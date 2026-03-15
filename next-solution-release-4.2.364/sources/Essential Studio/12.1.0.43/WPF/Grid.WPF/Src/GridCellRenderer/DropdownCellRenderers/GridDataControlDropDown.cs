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
using System.Windows;
using Syncfusion.Windows.Controls.Cells;

namespace Syncfusion.Windows.Controls.Grid.GridCellRenderer.DropdownCellRenderers
{
    public class GridDataControlDropDown : GridCellDropDownControlBase
    {
        public GridDataControlDropDown()
            :base()
        {
          
        }

        public GridDataControl DropDownGrid
        {
            get
            {
                if (this.PopupContent != null)
                {
                    if (this.PopupContent.Content == null)
                    {
                        GridDataControl grid = new GridDataControl();

                        grid.AllowDelete = false;
                        grid.AllowEdit = false;
                        grid.AllowGroup = false;
                        grid.AutoPopulateRelations = false;
                        grid.ShowAddNewRow = false;
                        grid.ShowColumnOptions = false;
                        grid.ShowRecordPlusMinus = false;
                        this.PopupContent.Content = grid;
                        return grid;
                    }
                    
                    return this.PopupContent.Content as GridDataControl;
                }
                return null;
            }
        }

        //protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    Point point = e.GetPosition(this.DropDownGrid);
        //    RowColumnIndex end =  this.DropDownGrid.InternalGrid.PointToCellRowColumnIndex(point);

        //    int rowIndex = end.RowIndex;
        //    int colIndex = end.ColumnIndex;

        //    if (rowIndex == 0 || (colIndex == 0 && this.DropDownGrid.ShowRowHeader))
        //        return;

        //    base.OnMouseLeftButtonUp(e);
        //}

        protected override bool OnIsDropDownOpenChanging(bool isShowing)
        {
            if (!isShowing)
            {

            }
            return base.OnIsDropDownOpenChanging(isShowing);
        }
        protected override void OnIsDropDownOpenChanged()
        {
            base.OnIsDropDownOpenChanged();
        }
    }
}
