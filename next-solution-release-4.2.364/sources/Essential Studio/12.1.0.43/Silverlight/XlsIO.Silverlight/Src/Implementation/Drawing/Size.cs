#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.XlsIO.Implementation.Silverlight
{
    public struct Size
    {
        public static readonly Size Empty;
        private int width;
        private int height;
        public Size(Point pt)
        {
            this.width = pt.X;
            this.height = pt.Y;
        }

        public Size(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public static implicit operator SizeF(Size p)
        {
            return new SizeF((float)p.Width, (float)p.Height);
        }

        public static Size operator +(Size sz1, Size sz2)
        {
            return Add(sz1, sz2);
        }

        public static Size operator -(Size sz1, Size sz2)
        {
            return Subtract(sz1, sz2);
        }

        public static bool operator ==(Size sz1, Size sz2)
        {
            return ((sz1.Width == sz2.Width) && (sz1.Height == sz2.Height));
        }

        public static bool operator !=(Size sz1, Size sz2)
        {
            return !(sz1 == sz2);
        }

        public static explicit operator Point(Size size)
        {
            return new Point(size.Width, size.Height);
        }

        public bool IsEmpty
        {
            get
            {
                return ((this.width == 0) && (this.height == 0));
            }
        }
        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }
        public int Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }
        public static Size Add(Size sz1, Size sz2)
        {
            return new Size(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }

        public static Size Ceiling(SizeF value)
        {
            return new Size((int)Math.Ceiling((double)value.Width), (int)Math.Ceiling((double)value.Height));
        }

        public static Size Subtract(Size sz1, Size sz2)
        {
            return new Size(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        }

        public static Size Truncate(SizeF value)
        {
            return new Size((int)value.Width, (int)value.Height);
        }

        public static Size Round(SizeF value)
        {
            return new Size((int)Math.Round((double)value.Width), (int)Math.Round((double)value.Height));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Size))
            {
                return false;
            }
            Size size = (Size)obj;
            return ((size.width == this.width) && (size.height == this.height));
        }

        public override int GetHashCode()
        {
            return (this.width ^ this.height);
        }

        public override string ToString()
        {
            return ("{Width=" + this.width.ToString() + ", Height=" + this.height.ToString() + "}");
        }

        static Size()
        {
            Empty = new Size();
        }
    }
}
