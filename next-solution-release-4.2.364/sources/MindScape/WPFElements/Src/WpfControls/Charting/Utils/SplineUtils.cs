using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Collections.Generic;

namespace Mindscape.WpfElements.Charting
{
  internal class SplineUtils
  {
    internal static PathGeometry CreateSpline(PointCollection points)
    {
      return CreateSpline(points, 0.4, null, false, false, 2, new List<int>());
    }

    internal static PathGeometry CreateSpline(PointCollection points, List<int> missingIndices)
    {
      return CreateSpline(points, 0.4, null, false, false, 2, missingIndices);
    }

    internal static PathGeometry CreateSpline(PointCollection points, bool isClosed, bool isFilled)
    {
      return CreateSpline(points, 0.4, null, isClosed, isFilled, 2, new List<int>());
    }

    internal static PathGeometry CreateSpline(PointCollection points, double tension)
    {
      return CreateSpline(points, tension, null, false, false, 2, new List<int>());
    }

    internal static PathGeometry CreateSpline(PointCollection points, double tension, bool isClosed, bool isFilled)
    {
      return CreateSpline(points, tension, null, isClosed, isFilled, 2, new List<int>());
    }

    internal static PathGeometry CreateSpline(PointCollection points, double tension, DoubleCollection tensions, bool isClosed, bool isFilled, double tolerance)
    {
      return CreateSpline(points, tension, tensions, isClosed, isFilled, tolerance, new List<int>());
    }

    internal static PathGeometry CreateSpline(PointCollection points, double tension, DoubleCollection tensions, bool isClosed, bool isFilled, double tolerance, List<int> missingIndices)
    {
      if (points == null || points.Count < 1)
      {
        return null;
      }

      PolyLineSegment polyLineSegment = new PolyLineSegment();
      PathFigure pathFigure = new PathFigure();
      pathFigure.IsClosed = isClosed;
      pathFigure.IsFilled = isFilled;
      pathFigure.StartPoint = points[0];
      pathFigure.Segments.Add(polyLineSegment);
      PathGeometry pathGeometry = new PathGeometry();
      pathGeometry.Figures.Add(pathFigure);

      if (points.Count < 2)
      {
        return pathGeometry;
      }
      else if (points.Count == 2)
      {
        if (!isClosed)
        {
          Segment(polyLineSegment.Points, points[0], points[0], points[1], points[1], tension, tension, tolerance);
        }
        else
        {
          Segment(polyLineSegment.Points, points[1], points[0], points[1], points[0], tension, tension, tolerance);
          Segment(polyLineSegment.Points, points[0], points[1], points[0], points[1], tension, tension, tolerance);
        }
      }
      else
      {
        bool useTensionCollection = tensions != null && tensions.Count > 0;
        int currentIndex = 0;

        for (int i = 0; i < points.Count; i++)
        {
          if (missingIndices.Count > 0 && missingIndices.Count > currentIndex && missingIndices[currentIndex] == i)
          {
            if (polyLineSegment.Points.Count == 0)
            {
              pathGeometry.Figures.RemoveAt(pathGeometry.Figures.Count - 1);
            }
            polyLineSegment = new PolyLineSegment();
            pathFigure = new PathFigure();
            pathFigure.StartPoint = points[Math.Min(i + 1, points.Count - 1)];
            pathFigure.Segments.Add(polyLineSegment);
            pathGeometry.Figures.Add(pathFigure);
            currentIndex++;
            continue;
          }
          double T1 = useTensionCollection ? tensions[i % tensions.Count] : tension;
          double T2 = useTensionCollection ? tensions[(i + 1) % tensions.Count] : tension;

          if (i == 0)
          {
            Segment(polyLineSegment.Points, isClosed ? points[points.Count - 1] : points[0], points[0], points[1], points[2], T1, T2, tolerance);
          }
          else if (i == points.Count - 2)
          {
            Segment(polyLineSegment.Points, points[i - 1], points[i], points[i + 1], isClosed ? points[0] : points[i + 1], T1, T2, tolerance);
          }
          else if (i == points.Count - 1)
          {
            if (isClosed)
            {
              Segment(polyLineSegment.Points, points[i - 1], points[i], points[0], points[1], T1, T2, tolerance);
            }
          }
          else
          {
            Segment(polyLineSegment.Points, points[i - 1], points[i], points[i + 1], points[i + 2], T1, T2, tolerance);
          }
        }
      }

      return pathGeometry;
    }

    private static void Segment(PointCollection points, Point pt0, Point pt1, Point pt2, Point pt3, double T1, double T2, double tolerance)
    {
      // See Petzold, "Programming Microsoft Windows with C#", pages 645-646 or 
      //     Petzold, "Programming Microsoft Windows with Microsoft Visual Basic .NET", pages 638-639
      // for derivation of the following formulas:

      double SX1 = T1 * (pt2.X - pt0.X);
      double SY1 = T1 * (pt2.Y - pt0.Y);
      double SX2 = T2 * (pt3.X - pt1.X);
      double SY2 = T2 * (pt3.Y - pt1.Y);

      double AX = SX1 + SX2 + 2 * pt1.X - 2 * pt2.X;
      double AY = SY1 + SY2 + 2 * pt1.Y - 2 * pt2.Y;
      double BX = -2 * SX1 - SX2 - 3 * pt1.X + 3 * pt2.X;
      double BY = -2 * SY1 - SY2 - 3 * pt1.Y + 3 * pt2.Y;

      double CX = SX1;
      double CY = SY1;
      double DX = pt1.X;
      double DY = pt1.Y;

      int count = Math.Max(5, (int)((Math.Abs(pt1.X - pt2.X) + Math.Abs(pt1.Y - pt2.Y)) / tolerance));
      
      for (int i = 1; i < count; i++)
      {
        double t = (double)i / (count - 1);
        Point pt = new Point(AX * t * t * t + BX * t * t + CX * t + DX, AY * t * t * t + BY * t * t + CY * t + DY);
        points.Add(pt);
      }
    }
  }
}
