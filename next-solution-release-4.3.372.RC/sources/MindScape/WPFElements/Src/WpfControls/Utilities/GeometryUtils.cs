using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Mindscape.WpfElements
{
  internal static class GeometryUtils
  {
    public static double DistanceMagnitude(Point pt1, Point pt2)
    {
      double deltaX = pt1.X - pt2.X;
      double deltaY = pt1.Y - pt2.Y;
      return (deltaX * deltaX) + (deltaY * deltaY);
    }

    public static double Distance(Point pt1, Point pt2)
    {
      return Math.Sqrt(DistanceMagnitude(pt1, pt2));
    }

    public static Path BuildPath(PathSegment segment, out PathFigure figure)
    {
      Path path = new Path();
      PathGeometry geometry = new PathGeometry();
      figure = new PathFigure();
      figure.Segments.Add(segment);
      geometry.Figures.Add(figure);
      path.Data = geometry;
      return path;
    }

    /// <summary>
    /// Returns the bounding box height of a rectangle that has been rotated by the given angle.
    /// The zero angle points east and rotates in a clockwise diraction.
    /// </summary>
    /// <param name="width">The width of the rotating rectangle.</param>
    /// <param name="height">The height of the rotating rectangle.</param>
    /// <param name="angle">The angle to rotate the rectangle in degrees.</param>
    /// <returns>The height of the bounding box of the rotated rectangle.</returns>
    public static double GetRotatedHeight(double width, double height, double angle)
    {
      // Modifying the angle so we only need to work in 1 trigonometry quadrent:
      if (angle > 270)
      {
        angle = 360 - angle;
      }
      else if (angle > 180)
      {
        angle -= 180;
      }
      else if (angle > 90)
      {
        angle = 180 - angle;
      }
      // Convert the angle to radians:
      double radians = angle / (180 / Math.PI);
      // Calculate rotated height:
      double widthHeight = Math.Sin(radians) * width;
      double heightHeight = Math.Cos(radians) * height;
      return Math.Max(0, widthHeight + heightHeight);
    }

    public static double GetAngle(Point vector)
    {
      double result = Math.Atan(Math.Abs(vector.Y) / Math.Abs(vector.X));
      result *= 180 / Math.PI;
      if (vector.X >= 0 && vector.Y < 0)
      {
        result = 90 - result;
      }
      else if (vector.X > 0 && vector.Y >= 0)
      {
        result += 90;
      }
      else if (vector.X <= 0 && vector.Y > 0)
      {
        result = 180 + (90 - result);
      }
      else if (vector.X < 0 && vector.Y <= 0)
      {
        result += 270;
      }
      return result;
    }

    public static Point GetPosition(double angle, double length)
    {
      angle /= 180.0 / Math.PI;
      double x = length * Math.Sin(angle);
      double y = length * Math.Cos(angle);
      return new Point(x, y);
    }

    public static PathGeometry ClonePathGeometry(PathGeometry geo)
    {
      PathFigure figure = geo.Figures[0];

      PathGeometry geometryClone = new PathGeometry();
      PathFigure figureClone = new PathFigure();
      figureClone.IsClosed = figure.IsClosed;
      figureClone.IsFilled = figure.IsFilled;
      figureClone.StartPoint = figure.StartPoint;

      foreach (PathSegment segment in figure.Segments)
      {
        figureClone.Segments.Add(ClonePathSegment(segment));
      }
      geometryClone.Figures.Add(figureClone);
      return geometryClone;
    }

    public static PathSegment ClonePathSegment(PathSegment segment)
    {
      if (segment is LineSegment)
      {
        return CloneLineSegment(segment as LineSegment);
      }
      if (segment is ArcSegment)
      {
        return CloneArcSegment(segment as ArcSegment);
      }
      return null;
    }

    public static LineSegment CloneLineSegment(LineSegment segment)
    {
      LineSegment lineSegmentClone = new LineSegment();
      lineSegmentClone.Point = segment.Point;
      return lineSegmentClone;
    }

    public static ArcSegment CloneArcSegment(ArcSegment segment)
    {
      ArcSegment arcSegmentClone = new ArcSegment();
      arcSegmentClone.IsLargeArc = segment.IsLargeArc;
      arcSegmentClone.Point = segment.Point;
      arcSegmentClone.RotationAngle = segment.RotationAngle;
      arcSegmentClone.Size = segment.Size;
      arcSegmentClone.SweepDirection = segment.SweepDirection;
      return arcSegmentClone;
    }
  }
}
