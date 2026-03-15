#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if ( WINRT )
using Syncfusion.XlsIO.Implementation.WINRT;
using System.Threading.Tasks;
#elif WP
using Syncfusion.XlsIO.Implementation.WP;
#elif SILVERLIGHT
using Syncfusion.XlsIO.Implementation.Silverlight;
#endif
using System;
using System.Collections.Generic;
using System.Text;

#if ( WINRT )
namespace Syncfusion.XlsIO.Metro.Implementation.Drawing
#elif WP
namespace Syncfusion.XlsIO.WP.Implementation.Drawing
#else
namespace Syncfusion.XlsIO.Silverlight.Implementation.Drawing
#endif
{

    public struct RectangleF
    {
        public static readonly RectangleF Empty;
        private double x;
        private double y;
        private double width;
        private double height;

        public RectangleF(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public RectangleF(Point location, Size size)
        {
            this.x = location.X;
            this.y = location.Y;
            this.width = size.Width;
            this.height = size.Height;
        }

        public static RectangleF FromLTRB(double left, double top, double right, double bottom)
        {
            return new RectangleF(left, top, right - left, bottom - top);
        }

        //public Point Location
        //{
        //    get
        //    {
        //        return new Point(this.X, this.Y);
        //    }
        //    set
        //    {
        //        this.X = value.X;
        //        this.Y = value.Y;
        //    }
        //}
        //public Size Size
        //{
        //    get
        //    {
        //        return new Size(this.Width, this.Height);
        //    }
        //    set
        //    {
        //        this.Width = value.Width;
        //        this.Height = value.Height;
        //    }
        //}
        public double X
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
        public double Y
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
        public double Width
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
        public double Height
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
        public double Left
        {
            get
            {
                return this.X;
            }
        }
        public double Top
        {
            get
            {
                return this.Y;
            }
        }
        public double Right
        {
            get
            {
                return (this.X + this.Width);
            }
        }
        public double Bottom
        {
            get
            {
                return (this.Y + this.Height);
            }
        }
        public bool IsEmpty
        {
            get
            {
                return ((((this.height == 0) && (this.width == 0)) && (this.x == 0)) && (this.y == 0));
            }
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Rectangle))
            {
                return false;
            }
            Rectangle rectangle = (Rectangle)obj;
            return ((((rectangle.X == this.X) && (rectangle.Y == this.Y)) && (rectangle.Width == this.Width)) && (rectangle.Height == this.Height));
        }

        public static bool operator ==(RectangleF left, RectangleF right)
        {
            return ((((left.X == right.X) && (left.Y == right.Y)) && (left.Width == right.Width)) && (left.Height == right.Height));
        }

        public static bool operator !=(RectangleF left, RectangleF right)
        {
            return !(left == right);
        }

        //public static Rectangle Ceiling(RectangleF value)
        //{
        //    return new Rectangle((int)Math.Ceiling((double)value.X), (int)Math.Ceiling((double)value.Y), (int)Math.Ceiling((double)value.Width), (int)Math.Ceiling((double)value.Height));
        //}

        //public static Rectangle Truncate(RectangleF value)
        //{
        //    return new Rectangle((int)value.X, (int)value.Y, (int)value.Width, (int)value.Height);
        //}

        //public static Rectangle Round(RectangleF value)
        //{
        //    return new Rectangle((int)Math.Round((double)value.X), (int)Math.Round((double)value.Y), (int)Math.Round((double)value.Width), (int)Math.Round((double)value.Height));
        //}

        public bool Contains(double x, double y)
        {
            return ((((this.X <= x) && (x < (this.X + this.Width))) && (this.Y <= y)) && (y < (this.Y + this.Height)));
        }

        public bool Contains(Point pt)
        {
            return this.Contains(pt.X, pt.Y);
        }

        public bool Contains(Rectangle rect)
        {
            return ((((this.X <= rect.X) && ((rect.X + rect.Width) <= (this.X + this.Width))) && (this.Y <= rect.Y)) && ((rect.Y + rect.Height) <= (this.Y + this.Height)));
        }

        public override int GetHashCode()
        {
            int _x = (int)this.X;
            int _y = (int)this.Y;
            int _width = (int)this.Width;
            int _height = (int)this.Height;

            return (((_x ^ ((_y << 13) | (_y >> 0x13))) ^ ((_width << 0x1a) | (_width >> 6))) ^ ((_height << 7) | (_height >> 0x19)));
        }

        public void Inflate(double width, double height)
        {
            this.X -= width;
            this.Y -= height;
            this.Width += 2 * width;
            this.Height += 2 * height;
        }

        public void Inflate(Size size)
        {
            this.Inflate(size.Width, size.Height);
        }

        public static RectangleF Inflate(RectangleF rect, double x, double y)
        {
            RectangleF rectangle = rect;
            rectangle.Inflate(x, y);
            return rectangle;
        }

        public void Intersect(RectangleF rect)
        {
            RectangleF rectangle = Intersect(rect, this);
            this.X = rectangle.X;
            this.Y = rectangle.Y;
            this.Width = rectangle.Width;
            this.Height = rectangle.Height;
        }

        public static RectangleF Intersect(RectangleF a, RectangleF b)
        {
            double x = Math.Max(a.X, b.X);
            double num2 = Math.Min((int)(a.X + a.Width), (int)(b.X + b.Width));
            double y = Math.Max(a.Y, b.Y);
            double num4 = Math.Min((int)(a.Y + a.Height), (int)(b.Y + b.Height));
            if ((num2 >= x) && (num4 >= y))
            {
                return new RectangleF(x, y, num2 - x, num4 - y);
            }
            return Empty;
        }

        public bool IntersectsWith(Rectangle rect)
        {
            return ((((rect.X < (this.X + this.Width)) && (this.X < (rect.X + rect.Width))) && (rect.Y < (this.Y + this.Height))) && (this.Y < (rect.Y + rect.Height)));
        }

        public static RectangleF Union(RectangleF a, RectangleF b)
        {
            double x = Math.Min(a.X, b.X);
            double num2 = Math.Max((int)(a.X + a.Width), (int)(b.X + b.Width));
            double y = Math.Min(a.Y, b.Y);
            double num4 = Math.Max((int)(a.Y + a.Height), (int)(b.Y + b.Height));
            return new RectangleF(x, y, num2 - x, num4 - y);
        }

        public void Offset(Point pos)
        {
            this.Offset(pos.X, pos.Y);
        }

        public void Offset(int x, int y)
        {
            this.X += x;
            this.Y += y;
        }

        public override string ToString()
        {
            return ("{X=" + this.X.ToString() + ",Y=" + this.Y.ToString() + ",Width=" + this.Width.ToString() + ",Height=" + this.Height.ToString() + "}");
        }

        static RectangleF()
        {
            Empty = new RectangleF();
        }
    }
}
