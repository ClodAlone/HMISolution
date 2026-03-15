#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;

#if !WinRT
using Syncfusion.Windows.Controls.Cells;
namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.Controls.Cells;
namespace Syncfusion.WinRT.Controls.Grid
#endif
{
    //class GridCellInfoInRow : Dictionary<int, GridStyleInfoStore>
    //{
    //}

    //class GridCellRows : Dictionary<int, GridCellInfoInRow>
    //{
    //}
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridCellData : RowColumnIndexValueArray<GridStyleInfoStore>
    {
        //GridCellRows rows = new GridCellRows();

        //public GridStyleInfoStore this[int rowIndex, int columnIndex]
        //{
        //    get
        //    {
        //        GridCellInfoInRow row;
        //        if (rows.ContainsKey(rowIndex))
        //        {
        //            row = rows[rowIndex];
        //            if (row.ContainsKey(columnIndex))
        //                return row[columnIndex];
        //        }
        //        return null;
        //    }
        //    set
        //    {
        //        GridCellInfoInRow row;
        //        if (rows.ContainsKey(rowIndex))
        //            row = rows[rowIndex];
        //        else
        //            rows[rowIndex] = row = new GridCellInfoInRow();

        //        row[columnIndex] = value;
        //    }
        //}
    }

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridStyleInfoIndexer
    {
        Dictionary<int, GridStyleInfo> styles;
        public GridStyleInfoIndexer()
        {
            this.styles = new Dictionary<int, GridStyleInfo>();
        }

        public void TryGetValue(int index, out GridStyleInfo rowStyle)
        {
            this.styles.TryGetValue(index, out rowStyle);
        }

        public GridStyleInfo this[int index]
        {
            get
            {
                GridStyleInfo rowStyle;
                this.styles.TryGetValue(index, out rowStyle);
                if (rowStyle == null)
                {
                    rowStyle = new GridStyleInfo();
                    this.styles.Add(index, rowStyle);
                }

                return rowStyle;
            }

            set
            {
                GridStyleInfo rowStyle;
                this.styles.TryGetValue(index, out rowStyle);
                if (rowStyle != value)
                {
                    this.styles.Add(index, value);
                }
            }
        }
    }

}
