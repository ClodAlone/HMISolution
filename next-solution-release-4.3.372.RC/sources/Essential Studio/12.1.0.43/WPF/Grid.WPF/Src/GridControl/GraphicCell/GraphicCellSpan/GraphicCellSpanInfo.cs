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

namespace Syncfusion.Windows.Controls.Grid
{
    public class GraphicCellSpanInfo : IDisposable, IComparable<GraphicCellSpanInfo>, IComparable
    {
        private int rowIndex;
        private int columnIndex;
        private double width = 300;
        private double height = 300;
        private double offsetX = 0;
        private double offsetY = 0;
        private int cellindex;
        private string name = string.Empty;

        public int RowIndex
        {
            get { return rowIndex; }
            set { rowIndex = value; }
        }

        public int ColumnIndex
        {
            get { return columnIndex; }
            set { columnIndex = value; }
        }

        public double Width
        {
            get { return width; }
            set { width = value; }
        }

        public double Height
        {
            get { return height; }
            set { height = value; }
        }

        public double OffsetX
        {
            get { return offsetX; }
            set { offsetX = value; }
        }

        public double OffsetY
        {
            get { return offsetY; }
            set { offsetY = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int CellSpanIndex
        {
            get { return cellindex; }
            set { cellindex = value; }
        }

        public GraphicCellSpanInfo()
        {

        }

        public GraphicCellSpanInfo(int row,int column)
        {
            this.rowIndex = row;
            this.columnIndex = column;
        }

        public GraphicCellSpanInfo(int row, int column, double width, double height)
        {
            this.rowIndex = row;
            this.columnIndex = column;
            this.width = width;
            this.height = height;
        }

        public override string ToString()
        {
            return String.Format("{0} ( RowIndex = {1} ColumnIndex = {2} Width = {3} Height = {4})",
               GetType().Name, RowIndex, ColumnIndex, Width, Height);
        }

        public override bool Equals(object obj)
        {
            GraphicCellSpanInfo other = obj as GraphicCellSpanInfo;
            if (other == null)
                return false;

            return rowIndex == other.rowIndex
                && columnIndex == other.columnIndex
                && width == other.width
                && height == other.height
                && offsetX == other.offsetX
                && offsetY == other.offsetY
                && name == other.name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public int CompareTo(GraphicCellSpanInfo other)
        {
            int cmp = other.rowIndex - rowIndex;
            if (cmp == 0)
            {
                cmp = other.columnIndex - columnIndex;
                if (cmp == 0)
                {
                    cmp = (int)(other.width - width);
                    if (cmp == 0)
                        cmp = (int)(other.height = height);
                }
            }
            return cmp;
        }

        public int CompareTo(object obj)
        {
            return CompareTo((GraphicCellSpanInfo)obj);
        }

        public virtual void Dispose()
        {
            
        }
    }
}
