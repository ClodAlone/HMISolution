#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.UI.Xaml.Grid
{
    [ClassReference(IsReviewed = false)]
    public class CoveredCellInfo : IComparable<CoveredCellInfo>, IComparable
    {
        #region Fields

        int row;
        int left;
        int right;
        string name;
        int rowSpan;

        #endregion

        #region Property

        /// <summary>
        /// Gets Row index for the coveredCell.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int Row
        {
            get { return row; }
        }

        /// <summary>
        /// Gets Left index for the cell.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int Left
        {
            get { return left; }
        }

        /// <summary>
        /// Gets Right index for the cell.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int Right
        {
            get { return right; }
        }

        /// <summary>
        /// Gets Width for the cell.
        /// </summary>
        /// <value></value>
        /// <remarks>difference between left and right index of the cell</remarks>
        public int Width
        {
            get { return Right - Left + 1; }
        }

        /// <summary>
        /// Gets Name of the StackedColumn corresponding to the Cell.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets or sets RowSpan for the cell.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public int RowSpan
        {
            get { return rowSpan; }
            internal set { rowSpan = value; }
        }
        #endregion

        #region Ctor

        public CoveredCellInfo(int left,int right)
        {
            this.left = left;
            this.right = right;
        }

        public CoveredCellInfo(int row, int left, int right)
        {
            this.row = row;
            this.left = Math.Min(left, right);
            this.right = Math.Max(left, right);
        }

        public CoveredCellInfo(string name, int left, int right)
        {
            this.name = name;
            this.left = left;
            this.right = right;
        }

        #endregion

        #region override methods

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as CoveredCellInfo;
            if (other == null)
                return false;

            return row == other.row
                && left == other.left
                && right == other.right;
        }

        public override string ToString()
        {
            return String.Format("{0} ( Row = {1} Left = {2} Right = {3} )", GetType().Name, Row, Left, Right);
        }

        #endregion

        #region IComparable

        public int CompareTo(CoveredCellInfo other)
        {
            int cmp = other.row - row;
            if (cmp == 0)
            {
                cmp = other.left - left;
                if (cmp == 0)
                {
                    cmp = other.right - right;
                }
            }
            return cmp;
        }

        int IComparable.CompareTo(object obj)
        {
            return CompareTo((CoveredCellInfo)obj);
        }

        #endregion
    }
}
