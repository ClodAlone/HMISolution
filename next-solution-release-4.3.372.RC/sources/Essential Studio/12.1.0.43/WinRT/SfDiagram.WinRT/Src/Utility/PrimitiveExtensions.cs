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

#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;
#else
using System.Windows.Shapes;
using VirtualKey = System.Windows.Input.Key;
using System.Windows.Input;
#endif

namespace Syncfusion.UI.Xaml.Diagram.Utility
{
    /// <summary>
    /// Utility class provides extension methords for Primitive DataType: Double, Points, Size
    /// </summary>
    internal static class DoubleExtensions
    {
        public static bool IsValid(this double d)
        {
            if (double.IsNaN(d) || double.IsInfinity(d))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static double Valid(this double value)
        {
            return IsValid(value) ? value : 0d;
        }

        public static double Max(this double a, double b, bool checkValid = true)
        {
            if (checkValid)
            {
                return Math.Max(a.Valid(), b.Valid());
            }
            else
            {
                return Math.Max(a, b);
            }
        }

        public static double Min(this double a, double b, bool checkValid = true)
        {
            if (checkValid)
            {
                return Math.Min(a.Valid(), b.Valid());
            }
            else
            {
                return Math.Min(a, b);
            }
        }

        public static double Round(this double value, IEnumerable<double> snapintervals)
        {
            double cutoff = snapintervals.Sum();
            double quotient = Math.Floor(Math.Abs(value) / cutoff);
            double bal = value % cutoff;
            double prev = quotient * cutoff;

            if (value >= 0)
            {
                for (int i = 0; i < snapintervals.Count(); i++)
                {
                    if (bal <= snapintervals.ElementAt(i))
                    {
                        return prev + (bal < (snapintervals.ElementAt(i) / 2) ? 0 : snapintervals.ElementAt(i));
                    }
                    else
                    {
                        prev += snapintervals.ElementAt(i);
                        bal -= snapintervals.ElementAt(i);
                    }
                }
            }
            else
            {
                prev = prev * -1;
                for (int i = snapintervals.Count() - 1; i >= 0; i--)
                {
                    if (Math.Abs(bal) <= snapintervals.ElementAt(i))
                    {
                        return prev - (Math.Abs(bal) < (snapintervals.ElementAt(i) / 2) ? 0 : snapintervals.ElementAt(i));
                    }
                    else
                    {
                        prev -= snapintervals.ElementAt(i);
                        bal += snapintervals.ElementAt(i);
                    }
                }
            }
            return value;
        }

        public static double Round(this double value, double cutoff)
        {
            return (Math.Floor(value / cutoff) + (value % cutoff > cutoff / 2 ? 1 : 0)) * cutoff;
        }

    }

    internal static class SizeExtensions
    {
        public static Size Max(this Size a, Size b, bool checkValid = true)
        {
            if (checkValid)
            {
                return new Size(
                    a.Width.Max(b.Width, checkValid),
                    a.Height.Max(b.Height, checkValid)
                    );
            }
            else
            {
                return new Size(
                    a.Width.Max(b.Width, checkValid),
                    a.Height.Max(b.Height, checkValid)
                    );
            }
        }

        public static Size Union(this Size a, Size b, bool checkValid = true)
        {
            if (checkValid)
            {
                return new Size(a.Width.Valid() + b.Width.Valid(), a.Height.Valid() + b.Height.Valid());
            }
            else
            {
                return new Size(a.Width + b.Width, a.Height + b.Height);
            }
        }

        public static Size Valid(this Size a)
        {
            return new Size(a.Width.Valid(), a.Height.Valid());
        }

    }


    internal static class PointExtensions
    {
        public static bool IsValid(this Point s)
        {
            return s.X.IsValid() && s.Y.IsValid();
        }

        public static double FindLength(this Point s, Point e)
        {
            double length;
            length = Math.Sqrt(Math.Pow((s.X - e.X), 2) + Math.Pow((s.Y - e.Y), 2));
            return length;
        }

        public static double FindAngle(this Point s, Point e)
        {
            if (s.Equals(e))
            {
                return 0d;
            }
            Point r = new Point(e.X, s.Y);
            double sr = s.FindLength(r);
            double re = r.FindLength(e);
            double es = e.FindLength(s);
            double ang = Math.Asin(re / es);
            ang = ang * 180 / Math.PI;
            if (s.X < e.X)
            {
                if (s.Y < e.Y)
                {

                }
                else
                {
                    ang = 360 - ang;
                }
            }
            else
            {
                if (s.Y < e.Y)
                {
                    ang = 180 - ang;
                }
                else
                {
                    ang = 180 + ang;
                }
            }
            return ang;
        }

        public static Point Transform(this Point s, double length, double angle)
        {
            return new Point()
                {
                    X = s.X + length * Math.Cos(angle * Math.PI / 180),
                    Y = s.Y + length * Math.Sin(angle * Math.PI / 180)
                };
        }

        public static OrthogonalDirection ToDirection(this Point s, Point e)
        {
            double diff = s.X - e.X;
            diff = diff >= 0 ? diff : -diff;
            double diff1 = s.Y - e.Y;
            diff1 = diff1 >= 0 ? diff1 : -diff1;
            if (diff == 0)
            {
                if (s.Y > e.Y)
                {
                    return OrthogonalDirection.Top;
                }
                else
                {
                    return OrthogonalDirection.Bottom;
                }
            }
            else
            {
                if (s.X < e.X)
                {
                    return OrthogonalDirection.Right;
                }
                else
                {
                    return OrthogonalDirection.Left;
                }
            }
        }
    }

    internal static class EnumExtensions
    {
        public static double ToAngle(this OrthogonalDirection s, double runAngle = 0)
        {
            switch (s)
            {
                case OrthogonalDirection.Left:
                case OrthogonalDirection.Top:
                case OrthogonalDirection.Right:
                case OrthogonalDirection.Bottom:
                    return (int)s;
                case OrthogonalDirection.Straight:
                case OrthogonalDirection.ClockWise90:
                case OrthogonalDirection.Opposite:
                case OrthogonalDirection.AntiClockWise90:
                    return runAngle + (int)s - 1;
            }
            return double.NaN;
        }

#if WINRT
        public static bool Contains(this CoreVirtualKeyStates s, CoreVirtualKeyStates t)
        {
            return (s & t) != 0;
        }

        public static bool ContainsAll(this CoreVirtualKeyStates s, CoreVirtualKeyStates t)
        {
            return (s & t) == t;
        }
#endif

        public static bool Contains(this NodeConstraints s, NodeConstraints t)
        {
            return (s & t) != 0;
        }

        public static bool ContainsAll(this NodeConstraints s, NodeConstraints t)
        {
            return (s & t) == t;
        }

        public static bool Contains(this ConnectorConstraints s, ConnectorConstraints t)
        {
            return (s & t) != 0;
        }

        public static bool Contains(this PortConstraints s, PortConstraints t)
        {
            return (s & t) != 0;
        }

        public static bool Contains(this GraphConstraints s, GraphConstraints t)
        {
            return (s & t) != 0;
        }

        public static bool Contains(this MultipleSelectionMode s, MultipleSelectionMode t)
        {
            return (s & t) != 0;
        }

        public static bool Contains(this SnapConstraints s, SnapConstraints t)
        {
            return (s & t) != 0;
        }

        public static bool Contains(this StencilConstraints s, StencilConstraints t)
        {
            return (s & t) != 0;
        }

        public static bool Contains(this SnapToObject s, SnapToObject t)
        {
            return (s & t) != 0;
        }

        public static bool Contains(this BezierSmoothness s, BezierSmoothness t)
        {
            return (s & t) != 0;
        }

        public static bool Contains(this SegmentConstraints s, SegmentConstraints t)
        {
            return (s & t) != 0;
        }

        public static bool Contains(this SnapChanges s, SnapChanges t)
        {
            return (s & t) != 0;
        }

        public static bool Contains(this SnapReason s, SnapReason t)
        {
            return (s & t) != 0;
        }

        public static bool Contains(this FitToPage s, FitToPage t)
        {
            return (s & t) != 0;
        }
#if TOUCH
        public static bool Contains(this ManipulationModes s, ManipulationModes t)
        { 
            return (s & t) != 0;
        }
#endif

        public static bool Contains(this Tool s, Tool t)
        {
            return (s & t) != 0;
        }

#if WINRT
        internal static bool IsPressed(this VirtualKey key)
        {
            return (CoreWindow.GetForCurrentThread().GetKeyState(key) & CoreVirtualKeyStates.Down) != CoreVirtualKeyStates.None;
        }
#endif

    }

    internal static class RectExtensions
    {
        public static bool IsIntersect(this Rect s, ref Rect t)
        {
            if (s.Right < t.Left || s.Bottom < t.Top || s.Left > t.Right || s.Top > t.Bottom)
            {
                return false;
            }
            return true;
            //s.Intersect(t);
            //return !s.IsEmpty;
        }

        public static bool IsIntersect(this Rect s, Rect t)
        {
            if (s.Right < t.Left || s.Bottom < t.Top || s.Left > t.Right || s.Top > t.Bottom)
            {
                return false;
            }
            return true;
            //s.Intersect(t);
            //return !s.IsEmpty;
        }

        public static bool Contains(this Rect s, Rect t)
        {
            if (t.Left >= s.Left && t.Right <= s.Right &&
                t.Top >= s.Top && t.Bottom <= s.Bottom)
            {
                return true;
            }
            return false;
        }
    }

    internal static class EditableExtensions
    {
        public static bool IsDeltable(this Editable s)
        {
            return (s & Editable.Delete) != Editable.None;
        }
        public static bool IsEditable(this Editable s)
        {
            return (s & (Editable.Angle | Editable.Length | Editable.Point)) != Editable.None;
        }
        public static bool AngleEditable(this Editable s)
        {
            return (s & Editable.Angle) != Editable.None;
        }
        public static bool LengthEditable(this Editable s)
        {
            return (s & Editable.Length) != Editable.None;
        }
        public static bool PointEditable(this Editable s)
        {
            return (s & Editable.Point) != Editable.None;
        }
    }

    internal static class LineExtensions
    {
        public static bool Intersect(this LineUtil s, LineUtil t, out Point POI)
        {
            POI = new Point(0, 0);
            LineUtil L1 = s;
            LineUtil L2 = t;
            double d = (L2.Y2 - L2.Y1) * (L1.X2 - L1.X1) - (L2.X2 - L2.X1) * (L1.Y2 - L1.Y1);
            double n_a = (L2.X2 - L2.X1) * (L1.Y1 - L2.Y1) - (L2.Y2 - L2.Y1) * (L1.X1 - L2.X1);
            double n_b = (L1.X2 - L1.X1) * (L1.Y1 - L2.Y1) - (L1.Y2 - L1.Y1) * (L1.X1 - L2.X1);

            if (d == 0)
                return false;

            double ua = n_a / d;
            double ub = n_b / d;

            if (ua >= 0d && ua <= 1d && ub >= 0d && ub <= 1d)
            {
                POI.X = L1.X1 + (ua * (L1.X2 - L1.X1));
                POI.Y = L1.Y1 + (ua * (L1.Y2 - L1.Y1));
                return true;
            }
            return false;
        }

        public static Point Intersect(this Point start1, Point end1, Point start2, Point end2)
        {
            LineUtil l1 = new LineUtil() { X1 = start1.X, Y1 = start1.Y, X2 = end1.X, Y2 = end1.Y };
            LineUtil l2 = new LineUtil() { X1 = start2.X, Y1 = start2.Y, X2 = end2.X, Y2 = end2.Y };
            if (l1.Intersect(l2, out end2))
            {
                return end2;
            }
            else
            {
                return new Point(0, 0);
            }
        }

        public static List<Point> Intersect(this Point lineStart, Point lineEnd, List<Point> polyLine)
        {
            List<Point> intersect = new List<Point>();
            for (int i = 0; i < polyLine.Count - 1; i++)
            {
                Point p = lineStart.Intersect(lineEnd, polyLine[i], polyLine[i + 1]);
                if (!p.Equals(new Point(0, 0)))
                {
                    intersect.Add(p);
                }
            }
            return intersect;
        }

        public static List<Point> Intersect(this List<Point> polyLine1, List<Point> polyLine2, bool self)
        {
            if (self && polyLine2.Count >= 2)
            {
                polyLine2.RemoveAt(0);
                polyLine2.RemoveAt(0);
            }
            List<Point> intersect = new List<Point>();
            for (int i = 0; i < polyLine1.Count - 1; i++)
            {
                intersect.AddRange(polyLine1[i].Intersect(polyLine1[i + 1], polyLine2));
                if (self && polyLine2.Count >= 1)
                {
                    polyLine2.RemoveAt(0);
                }
            }
            return intersect;
        }
    }


    internal struct LineUtil
    {
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
    }
}
