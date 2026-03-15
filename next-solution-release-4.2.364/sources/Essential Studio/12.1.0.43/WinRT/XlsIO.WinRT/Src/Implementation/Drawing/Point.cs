#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.XlsIO.Implementation.WINRT
{
    public struct Point
    {
        public static readonly Point Empty;
        private int x;
        private int y;
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Point(Size sz)
        {
            this.x = sz.Width;
            this.y = sz.Height;
        }

        public Point(int dw)
        {
            this.x = (short)LOWORD(dw);
            this.y = (short)HIWORD(dw);
        }

        public bool IsEmpty
        {
            get
            {
                return ((this.x == 0) && (this.y == 0));
            }
        }
        public int X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }
        public int Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }
        //public static implicit operator PointF(Point p)
        //{
        //    return new PointF((float)p.X, (float)p.Y);
        //}

        public static explicit operator Size(Point p)
        {
            return new Size(p.X, p.Y);
        }

        public static Point operator +(Point pt, Size sz)
        {
            return Add(pt, sz);
        }

        public static Point operator -(Point pt, Size sz)
        {
            return Subtract(pt, sz);
        }

        public static bool operator ==(Point left, Point right)
        {
            return ((left.X == right.X) && (left.Y == right.Y));
        }

        public static bool operator !=(Point left, Point right)
        {
            return !(left == right);
        }

        public static Point Add(Point pt, Size sz)
        {
            return new Point(pt.X + sz.Width, pt.Y + sz.Height);
        }

        public static Point Subtract(Point pt, Size sz)
        {
            return new Point(pt.X - sz.Width, pt.Y - sz.Height);
        }

        //public static Point Ceiling(PointF value)
        //{
        //    return new Point((int)Math.Ceiling((double)value.X), (int)Math.Ceiling((double)value.Y));
        //}

        //public static Point Truncate(PointF value)
        //{
        //    return new Point((int)value.X, (int)value.Y);
        //}

        //public static Point Round(PointF value)
        //{
        //    return new Point((int)Math.Round((double)value.X), (int)Math.Round((double)value.Y));
        //}

        public override bool Equals(object obj)
        {
            if (!(obj is Point))
            {
                return false;
            }
            Point point = (Point)obj;
            return ((point.X == this.X) && (point.Y == this.Y));
        }

        public override int GetHashCode()
        {
            return (this.x ^ this.y);
        }

        public void Offset(int dx, int dy)
        {
            this.X += dx;
            this.Y += dy;
        }

        public void Offset(Point p)
        {
            this.Offset(p.X, p.Y);
        }

        public override string ToString()
        {
            return ("{X=" + this.X.ToString() + ",Y=" + this.Y.ToString() + "}");
        }

        private static int HIWORD(int n)
        {
            return ((n >> 0x10) & 0xffff);
        }

        private static int LOWORD(int n)
        {
            return (n & 0xffff);
        }

        static Point()
        {
            Empty = new Point();
        }
    }

}
