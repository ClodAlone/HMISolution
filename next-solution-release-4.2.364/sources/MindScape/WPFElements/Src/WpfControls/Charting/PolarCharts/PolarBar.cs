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
using System.ComponentModel;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Represents a single data point in a <see cref="RoseSeries"/>.
  /// </summary>
  public class PolarBar : PolarDataPoint, INotifyPropertyChanged
  {
    private PathGeometry _geometry;

    static PolarBar()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PolarBar),
        new FrameworkPropertyMetadata(typeof(PolarBar)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolarBar"/> class.
    /// </summary>
    /// <param name="data">The data object that the <see cref="PolarBar"/> plots.</param>
    internal PolarBar(object data)
    {
      DataContext = data;
    }

    /// <summary>
    /// Gets the geometry of this <see cref="PolarBar"/>.
    /// </summary>
    public PathGeometry PathData
    {
      get { return GeometryUtils.ClonePathGeometry(_geometry); }
      private set
      {
        _geometry = value;
        OnPropertyChanged("PathData");
      }
    }

    internal void UpdatePathData(double currentHeight, double previousHeight, double barWidthAngle)
    {
      double barHeight = currentHeight - previousHeight;
      PathGeometry geo = new PathGeometry();
      PathFigure figure = new PathFigure();
      figure.IsClosed = true;
      figure.IsFilled = true;
      figure.StartPoint = new Point(0, barHeight);

      Point vector = GeometryUtils.GetPosition(barWidthAngle / 2.0, currentHeight);

      if (previousHeight == 0)
      {
        LineSegment segment1 = new LineSegment();
        segment1.Point = new Point(-vector.X, currentHeight - vector.Y);

        ArcSegment segment2 = new ArcSegment();
        segment2.Size = new Size(currentHeight, currentHeight);
        segment2.RotationAngle = barWidthAngle;
        segment2.IsLargeArc = false;
        segment2.SweepDirection = SweepDirection.Clockwise;
        segment2.Point = new Point(vector.X, currentHeight - vector.Y);

        LineSegment segment3 = new LineSegment();
        segment3.Point = new Point(0, barHeight);

        figure.Segments.Add(segment1);
        figure.Segments.Add(segment2);
        figure.Segments.Add(segment3);

        Height = currentHeight + 10;
      }
      else
      {
        Point innerVector = GeometryUtils.GetPosition(barWidthAngle / 2.0, previousHeight);

        figure.StartPoint = new Point(innerVector.X, currentHeight - innerVector.Y);

        ArcSegment segment0 = new ArcSegment();
        segment0.Size = new Size(previousHeight, previousHeight);
        segment0.RotationAngle = barWidthAngle;
        segment0.IsLargeArc = false;
        segment0.SweepDirection = SweepDirection.Counterclockwise;
        segment0.Point = new Point(-innerVector.X, currentHeight - innerVector.Y);

        LineSegment segment1 = new LineSegment();
        segment1.Point = new Point(-vector.X, currentHeight - vector.Y);

        ArcSegment segment2 = new ArcSegment();
        segment2.Size = new Size(currentHeight, currentHeight);
        segment2.RotationAngle = barWidthAngle;
        segment2.IsLargeArc = false;
        segment2.SweepDirection = SweepDirection.Clockwise;
        segment2.Point = new Point(vector.X, currentHeight - vector.Y);

        LineSegment segment3 = new LineSegment();
        segment3.Point = new Point(innerVector.X, currentHeight - innerVector.Y);

        /*ArcSegment segment4 = new ArcSegment();
        segment4.Size = new Size(previousHeight, previousHeight);
        segment4.RotationAngle = barWidthAngle / 2.0;
        segment4.IsLargeArc = false;
        segment4.SweepDirection = SweepDirection.Counterclockwise;
        segment4.Point = new Point(0, barHeight);*/

        figure.Segments.Add(segment0);
        figure.Segments.Add(segment1);
        figure.Segments.Add(segment2);
        figure.Segments.Add(segment3);

        Height = currentHeight - innerVector.Y + 10;
      }

      geo.Figures.Add(figure);
      PathData = geo;

      Width = Math.Max(0, vector.X * 2);
    }

    /// <summary>
    /// Raised when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}
