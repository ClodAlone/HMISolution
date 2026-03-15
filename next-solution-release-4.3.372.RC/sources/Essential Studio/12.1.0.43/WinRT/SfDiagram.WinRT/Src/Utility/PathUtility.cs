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

#if WINRT_USING
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes; 
#else
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
#endif

namespace Syncfusion.UI.Xaml.Diagram.Utility
{
    internal static class PathUtility
    {
        internal static Geometry ParseGeometry(this string data)
        {

#if WPF
             return Geometry.Parse(data);
#else
            string p = "<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Data=\"" + data + "\"/>";
            Path o = p.LoadXaml() as Path;
            PathGeometry geo = Clone(o.Data as PathGeometry);
            return geo;
#endif
        }

        internal static PathGeometry Clone(this PathGeometry pathGeometry)
        {
            PathFigureCollection collClone = new PathFigureCollection();
            foreach (PathFigure item in pathGeometry.Figures)
            {
                PathFigure clone = new PathFigure()
                {
                    IsClosed = item.IsClosed,
                    IsFilled = item.IsFilled,
                    Segments = Clone(item.Segments),
                    StartPoint = item.StartPoint
                };
                collClone.Add(clone);
            }
            return new PathGeometry()
            {
                Figures = collClone,
                FillRule = pathGeometry.FillRule,
                Transform = pathGeometry.Transform
            };
        }

        internal static PathSegmentCollection Clone(this PathSegmentCollection pathSegColl)
        {
            PathSegmentCollection collClone = new PathSegmentCollection();

            foreach (PathSegment item in pathSegColl)
            {
                PathSegment clone = null;
                if (item is LineSegment)
                {
                    LineSegment seg = item as LineSegment;
                    clone = new LineSegment() { Point = seg.Point };
                }
                else if (item is PolyLineSegment)
                {
                    PolyLineSegment seg = item as PolyLineSegment;
                    clone = new PolyLineSegment() { Points = getPoints(seg.Points) };
                }
                else if (item is BezierSegment)
                {
                    BezierSegment seg = item as BezierSegment;
                    clone = new BezierSegment()
                    {
                        Point1 = seg.Point1,
                        Point2 = seg.Point2,
                        Point3 = seg.Point3
                    };
                }
                else if (item is PolyBezierSegment)
                {
                    PolyBezierSegment seg = item as PolyBezierSegment;
                    clone = new PolyBezierSegment() { Points = getPoints(seg.Points) };
                }
                else if (item is PolyQuadraticBezierSegment)
                {
                    PolyQuadraticBezierSegment seg = item as PolyQuadraticBezierSegment;
                    clone = new PolyQuadraticBezierSegment() { Points = getPoints(seg.Points) };
                }
                else if (item is QuadraticBezierSegment)
                {
                    QuadraticBezierSegment seg = item as QuadraticBezierSegment;
                    clone = new QuadraticBezierSegment() { Point1 = seg.Point1, Point2 = seg.Point2 };
                }
                else if (item is ArcSegment)
                {
                    ArcSegment seg = item as ArcSegment;
                    clone = new ArcSegment()
                    {
                        IsLargeArc = seg.IsLargeArc,
                        Point = seg.Point,
                        RotationAngle = seg.RotationAngle,
                        Size = seg.Size,
                        SweepDirection = seg.SweepDirection
                    };
                }

                collClone.Add(clone);
            }
            return collClone;
        }

        internal static void Clone(this PathSegmentCollection pathSegColl, PathSegmentCollection collClone)
        {
            collClone.Clear();
            foreach (PathSegment item in pathSegColl)
            {
                PathSegment clone = null;
                if (item is LineSegment)
                {
                    LineSegment seg = item as LineSegment;
                    clone = new LineSegment() { Point = seg.Point };
                }
                else if (item is PolyLineSegment)
                {
                    PolyLineSegment seg = item as PolyLineSegment;
                    clone = new PolyLineSegment() { Points = getPoints(seg.Points) };
                }
                else if (item is BezierSegment)
                {
                    BezierSegment seg = item as BezierSegment;
                    clone = new BezierSegment()
                    {
                        Point1 = seg.Point1,
                        Point2 = seg.Point2,
                        Point3 = seg.Point3
                    };
                }
                else if (item is PolyBezierSegment)
                {
                    PolyBezierSegment seg = item as PolyBezierSegment;
                    clone = new PolyBezierSegment() { Points = getPoints(seg.Points) };
                }
                else if (item is PolyQuadraticBezierSegment)
                {
                    PolyQuadraticBezierSegment seg = item as PolyQuadraticBezierSegment;
                    clone = new PolyQuadraticBezierSegment() { Points = getPoints(seg.Points) };
                }
                else if (item is QuadraticBezierSegment)
                {
                    QuadraticBezierSegment seg = item as QuadraticBezierSegment;
                    clone = new QuadraticBezierSegment() { Point1 = seg.Point1, Point2 = seg.Point2 };
                }
                else if (item is ArcSegment)
                {
                    ArcSegment seg = item as ArcSegment;
                    clone = new ArcSegment()
                    {
                        IsLargeArc = seg.IsLargeArc,
                        Point = seg.Point,
                        RotationAngle = seg.RotationAngle,
                        Size = seg.Size,
                        SweepDirection = seg.SweepDirection
                    };
                }
                collClone.Add(clone);
            }
        }

        internal static void Clone(this PathFigureCollection pathFigureColl, PathFigureCollection clonePathFigure)
        {
            clonePathFigure.Clear();
            foreach (var figure in pathFigureColl)
            {
                PathFigure cloneFigure = new PathFigure()
                {
                    IsClosed = figure.IsClosed,
                    IsFilled = figure.IsFilled,
                    StartPoint = figure.StartPoint
                };

                figure.Segments.Clone(cloneFigure.Segments);
                clonePathFigure.Add(cloneFigure);
            }
        }

        internal static void Clone(this PathGeometry source, PathGeometry target)
        {
            source.FillRule = target.FillRule;
            source.Figures.Clone(target.Figures);
        }

        internal static PointCollection getPoints(this PointCollection pointCollection)
        {
            PointCollection coll = new PointCollection();
            foreach (var item in pointCollection)
            {
                coll.Add(item);
            }
            return coll;
        }

        internal static Point GetEndPoint(this PathSegment segment)
        {
            if (segment is LineSegment)
            {
                return (segment as LineSegment).Point;
            }
            else if (segment is ArcSegment)
            {
                return (segment as ArcSegment).Point;
            }
            else if (segment is BezierSegment)
            {
                return (segment as BezierSegment).Point3;
            }
            else if (segment is PolyBezierSegment)
            {
                return (segment as PolyBezierSegment).Points.Last();
            }
            else if (segment is PolyLineSegment)
            {
                return (segment as PolyLineSegment).Points.Last();
            }
            else if (segment is PolyQuadraticBezierSegment)
            {
                return (segment as PolyQuadraticBezierSegment).Points.Last();
            }
            else if (segment is QuadraticBezierSegment)
            {
                return (segment as QuadraticBezierSegment).Point2;
            }
            else
            {
                throw new InvalidOperationException("Invalid segment");
            }
        }

        internal static void SetEndPoint(this PathSegment segment, Point endPoint)
        {
            if (segment is LineSegment)
            {
                (segment as LineSegment).Point = endPoint;
            }
            else if (segment is ArcSegment)
            {
                (segment as ArcSegment).Point = endPoint;
            }
            else if (segment is BezierSegment)
            {
                (segment as BezierSegment).Point3 = endPoint;
            }
            else if (segment is PolyBezierSegment)
            {
                var pts = (segment as PolyBezierSegment).Points;
                pts[pts.Count - 1] = endPoint;
            }
            else if (segment is PolyLineSegment)
            {
                var pts = (segment as PolyLineSegment).Points;
                pts[pts.Count - 1] = endPoint;
            }
            else if (segment is PolyQuadraticBezierSegment)
            {
                var pts = (segment as PolyQuadraticBezierSegment).Points;
                pts[pts.Count - 1] = endPoint;
            }
            else if (segment is QuadraticBezierSegment)
            {
                (segment as QuadraticBezierSegment).Point2 = endPoint;
            }
            else
            {
                throw new InvalidOperationException("Invalid segment");
            }
        }

        internal static IEnumerable<Point?> GetNextPoint(this PathSegment segment)
        {
            if (segment is LineSegment)
            {
                yield return (segment as LineSegment).Point;
            }
            else if (segment is PolyLineSegment)
            {
                foreach (var point in (segment as PolyLineSegment).Points)
                {
                    yield return point;
                }
            }
        }

        internal static string ToStreamGeometry(this Geometry geometry)
        {
            if (geometry is PathGeometry)
            {
                string s = string.Empty;
                PathFigureCollection collection = (geometry as PathGeometry).Figures;
                foreach (PathFigure f in collection)
                {
                    s += "M" + f.StartPoint.ToString();
                    foreach (PathSegment item in f.Segments)
                    {
                        //PathSegmentCollection pathSegColl = f.Segments;
                        //foreach (PathSegment item in pathSegColl)
                        //{

                        if (item is LineSegment)
                        {
                            LineSegment seg = item as LineSegment;
                            //s = "L" + seg.Point.ToString();
                            s += " " + seg.Point.ToString();
                        }
                        else if (item is PolyLineSegment)
                        {
                            PolyLineSegment seg = item as PolyLineSegment;
                            //s = "L" + seg.Point.ToString();
                        }
                        else if (item is BezierSegment)
                        {
                            BezierSegment seg = item as BezierSegment;
                            s += "C" + seg.Point1.ToString();
                            s += "," + seg.Point2.ToString();
                            s += "," + seg.Point3.ToString();
                        }
                        else if (item is PolyBezierSegment)
                        {
                            PolyBezierSegment seg = item as PolyBezierSegment;

                            //s = "L" + seg.Point.ToString();
                        }
                        else if (item is PolyQuadraticBezierSegment)
                        {
                            PolyQuadraticBezierSegment seg = item as PolyQuadraticBezierSegment;

                        }
                        else if (item is QuadraticBezierSegment)
                        {

                            QuadraticBezierSegment seg = item as QuadraticBezierSegment;
                            s += "Q" + seg.Point1.ToString();
                            s += seg.Point2.ToString();

                        }
                        else if (item is ArcSegment)
                        {
                            ArcSegment seg = item as ArcSegment;
                            s += " " + "A" + seg.Size.ToString();
                            s += " " + seg.RotationAngle.ToString();
                            if (seg.IsLargeArc)
                            {
                                s += " " + 1;
                            }
                            else
                            {
                                s += " " + 0;
                            }

                            if (seg.SweepDirection == SweepDirection.Clockwise)
                            {
                                s += " " + 1;
                            }
                            else
                            {
                                s += " " + 0;
                            }

                            s += " " + seg.Point;
                        }
                    }
                    if (f.IsClosed)
                    {
                        s += "Z";
                    }
                }
                return s;
                //return s += "z";
            }
#if WPF
            else if (geometry.GetType() == typeof(StreamGeometry))
            {
              (geometry as StreamGeometry).ToString();
            }
            
#endif
            return string.Empty;
        }
    }
}
