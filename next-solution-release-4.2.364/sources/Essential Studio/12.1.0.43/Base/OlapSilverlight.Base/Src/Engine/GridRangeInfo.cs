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
using Syncfusion.OlapSilverlight.Common;

namespace Syncfusion.OlapSilverlight.Engine
{
    public class GridRangeInfo
    {
        internal int _top;
        internal int _left;
        internal int _bottom;
        internal int _right;
        internal GridRangeInfoType _rangeType;

        public readonly static GridRangeInfo Empty = new GridRangeInfo();

        public GridRangeInfo()
        {
            _top = _left = _bottom = _right = 0;
            _rangeType = GridRangeInfoType.Empty;
        }
        public int Left
        {
            get
            {
                if ((_rangeType & GridRangeInfoType.Rows) != 0 || _rangeType == GridRangeInfoType.Empty)
                    return 0;
                else
                    return _left ;
            }
          
        }
        public int Top
        {
            get
            {
                if ((_rangeType & GridRangeInfoType.Cols) != 0 || _rangeType == GridRangeInfoType.Empty)
                    return 0;
                else
                    return _top ;
            }
           
        }
        public int Right
        {
            get
            {
                if ((_rangeType & GridRangeInfoType.Rows) != 0 || _rangeType == GridRangeInfoType.Empty)
                    return 0;
                else
                    return _right  ;
            }
         
        }
        public int Bottom
        {
            get
            {
                if ((_rangeType & GridRangeInfoType.Cols) != 0 || _rangeType == GridRangeInfoType.Empty)
                    return 0;
                else
                    return _bottom ;
            }
           
        }
        public int Height
        {
            get
            {
                return _bottom   - _top  + 1;
            }
         
        }
        public int Width
        {
            get
            {
                return _right   - _left  + 1;
            }
           
        }
    

        public bool IsEmpty
        {
            //[DebuggerStepThrough()]
            get
            {
                return ((object)this) == null || _rangeType == GridRangeInfoType.Empty;
            }
        }

        public GridRangeInfoType RangeType
        {
            get
            {
                return _rangeType;
            }
        }


        public static GridRangeInfo FromTlhw(int top, int left, int height, int width)
        {
            if (height <= 0 || width <= 0)
                return GridRangeInfo.Empty;

            return new GridRangeInfo(top, left, top + height - 1, left + width - 1);
        }

        internal GridRangeInfo(int top, int left, int bottom, int right)
        {
            this._top = Math.Min(top, bottom);
            this._left = Math.Min(left, right);
            this._bottom = Math.Max(top, bottom);
            this._right = Math.Max(left, right);
            this._rangeType = GridRangeInfoType.Cells;
            if (this._bottom == int.MaxValue || this._right == int.MaxValue
                || this._left < 0 || this._top < 0)
                throw new ArgumentException();
        }

        

        public bool Contains(GridRangeInfo range)
        {
            return !IsEmpty && this.IntersectRange(range).Equals(range);
        }

        public static GridRangeInfo IntersectRange(GridRangeInfo r1, GridRangeInfo r2)
        {
            GridRangeInfo r = new GridRangeInfo();
            r._left = Math.Max(r1._left, r2._left);
            r._right = Math.Min(r1._right, r2._right);
            r._top = Math.Max(r1._top, r2._top);
            r._bottom = Math.Min(r1._bottom, r2._bottom);
            if (r._left <= r._right && r._top <= r._bottom)
                r.EnsureRangeType();
            else
                r = GridRangeInfo.Empty;

            return r;
        }

        public GridRangeInfo IntersectRange(GridRangeInfo range)
        {
            return GridRangeInfo.IntersectRange(this, range);
        }

        private void EnsureRangeType()
        {
            if (this._left == 0 && this._right == int.MaxValue)
            {
                if (this._top == 0 && this._bottom == int.MaxValue)
                    this._rangeType = GridRangeInfoType.Table;
                else
                    this._rangeType = GridRangeInfoType.Rows;
            }
            else if (this._top == 0 && this._bottom == int.MaxValue)
                this._rangeType = GridRangeInfoType.Cols;
            else
                this._rangeType = GridRangeInfoType.Cells;
        }

        public static GridRangeInfo UnionRange(GridRangeInfo r1, GridRangeInfo r2)
        {
            if (r1.IsEmpty && r2.IsEmpty)
                return GridRangeInfo.Empty;
            else if (r1.IsEmpty)
                return r2;
            else if (r2.IsEmpty)
                return r1;
            else
            {
                GridRangeInfo r = new GridRangeInfo();
                r._left = Math.Min(r1._left, r2._left);
                r._right = Math.Max(r1._right, r2._right);
                r._top = Math.Min(r1._top, r2._top);
                r._bottom = Math.Max(r1._bottom, r2._bottom);
                if (r._left <= r._right && r._top <= r._bottom)
                    r.EnsureRangeType();
                else
                    r = GridRangeInfo.Empty;
                return r;
            }
        }
    }    
}
