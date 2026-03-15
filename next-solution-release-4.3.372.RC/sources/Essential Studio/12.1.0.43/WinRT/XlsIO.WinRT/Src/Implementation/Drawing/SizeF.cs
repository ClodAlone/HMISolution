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
    public struct SizeF
    {
        public static readonly SizeF Empty;
        private float width;
        private float height;
        public SizeF(SizeF size)
        {
            this.width = size.width;
            this.height = size.height;
        }

        //public SizeF(PointF pt)
        //{
        //    this.width = pt.X;
        //    this.height = pt.Y;
        //}

        public SizeF(float width, float height)
        {
            this.width = width;
            this.height = height;
        }

        public static SizeF operator +(SizeF sz1, SizeF sz2)
        {
            return Add(sz1, sz2);
        }

        public static SizeF operator -(SizeF sz1, SizeF sz2)
        {
            return Subtract(sz1, sz2);
        }

        public static bool operator ==(SizeF sz1, SizeF sz2)
        {
            return ((sz1.Width == sz2.Width) && (sz1.Height == sz2.Height));
        }

        public static bool operator !=(SizeF sz1, SizeF sz2)
        {
            return !(sz1 == sz2);
        }

        //public static explicit operator PointF(SizeF size)
        //{
        //    return new PointF(size.Width, size.Height);
        //}

        public bool IsEmpty
        {
            get
            {
                return ((this.width == 0f) && (this.height == 0f));
            }
        }
        public float Width
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
        public float Height
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
        public static SizeF Add(SizeF sz1, SizeF sz2)
        {
            return new SizeF(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }

        public static SizeF Subtract(SizeF sz1, SizeF sz2)
        {
            return new SizeF(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SizeF))
            {
                return false;
            }
            SizeF ef = (SizeF)obj;
            return (((ef.Width == this.Width) && (ef.Height == this.Height)) && ef.GetType().Equals(base.GetType()));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //public PointF ToPointF()
        //{
        //    return (PointF)this;
        //}

        public Size ToSize()
        {
            return Size.Truncate(this);
        }

        public override string ToString()
        {
            return ("{Width=" + this.width.ToString() + ", Height=" + this.height.ToString() + "}");
        }

        static SizeF()
        {
            Empty = new SizeF();
        }
    }
}
