#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System;

#if !WinRT
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.ComponentModel;

namespace Syncfusion.Windows.Controls.Grid
#else
using Syncfusion.WinRT.Controls.Cells;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridOverlappingCellInfoCollection : List<OverlappingCellInfo>, IOverlappingCellProvider,IDisposable
    {
        GridModel gridModel;
        GridRangeInfoList ranges;

        public GridOverlappingCellInfoCollection(GridModel gridModel)
        {
            this.gridModel = gridModel;
        }

        public GridRangeInfoList Ranges
        {
            get
            {
                if (ranges == null)
                {
                    ranges = new GridRangeInfoList();
                    // TODO: Support for row, column ranges
                    foreach (OverlappingCellInfo cc in this)
                        ranges.Add(GridRangeInfo.Cells(cc.Top, cc.Left, cc.Bottom, cc.Right));
                }
                return ranges;
            }
        }

        public bool Find(int rowIndex, int columnIndex, out GridRangeInfo coveredRange)
        {
            OverlappingCellInfo cc = GetOverlappingCell(rowIndex, columnIndex);
            if (cc != null)
            {
                coveredRange = GridRangeInfo.Cells(cc.Top, cc.Left, cc.Bottom, cc.Right);
                return true;
            }
            else
            {
                coveredRange = GridRangeInfo.Cell(rowIndex, columnIndex);
                return false;
            }
        }

        internal GridRangeInfo FindRange(int rowIndex, int colIndex)
        {
            OverlappingCellInfo cc = GetOverlappingCell(rowIndex, colIndex);
            if (cc != null)
            {
                return GridRangeInfo.Cells(cc.Top, cc.Left, cc.Bottom, cc.Right);
            }
            else
            {
                return GridRangeInfo.Empty;
            }
        }

        #region IImageCellProvider

        public OverlappingCellInfo GetOverlappingCell(int rowIndex, int columnIndex)
        {
            OverlappingCellInfo imageSpan = null;
            foreach (OverlappingCellInfo item in this)
            {
                if (item.Top == rowIndex && item.Left == columnIndex)
                {
                    return item;
                }
                if (item.Top <= rowIndex && item.Left <= columnIndex && item.Bottom >= rowIndex && item.Right >= columnIndex)
                {
                    imageSpan = item;
                }
            }
            return imageSpan;
        }

        public bool IsEmpty
        {
            get
            {
                return this.Count == 0;
            }
        }
        #endregion

        public void Dispose()
        {
            this.gridModel = null;
            if (ranges != null)
                this.ranges.Clear();
            this.Clear();
        }
    }

}
