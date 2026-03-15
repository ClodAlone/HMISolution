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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Represents the theta axis of a <see cref="PolarChart"/>.
  /// The theta axis is used for plotting angular coordinates.
  /// </summary>
  public class ThetaAxis : PolarAxisBase
  {
    static ThetaAxis()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ThetaAxis),
        new FrameworkPropertyMetadata(typeof(ThetaAxis)));
    }

    /// <summary>
    /// Manages the creation and placement of axis labels.
    /// </summary>
    protected override void UpdateAxisCore()
    {
      //IList<AxisLabel> labels = new List<AxisLabel>();
      MajorTickMarks.Clear();

      double centerX = ActualWidth / 2.0;
      double centerY = ActualHeight / 2.0;
      double axisRadius = (Math.Min(ActualWidth, ActualHeight) / 2.0) - 10;

      string labelFormat = LabelFormat;

      double currentTick = Minimum;
      int labelCount = 0;
      while (currentTick < Maximum && Spacing > 0)
      {
        double angle = ConvertLogicalToPhysical(currentTick);
        Point vector = GeometryUtils.GetPosition(angle, axisRadius);
        double x = centerX + vector.X;
        double y = centerY - vector.Y;

        //AxisLabel label = GetAxisLabel();
        //label.Label = GetLabel(currentTick);
        // TODO: fix this by using a render transform and use recycling.
        //label.PositionOffset = x;
        //label.LevelOffset = y;
        /*label.RenderTransform = new TranslateTransform() { X = x, Y = y };
        label.HorizontalAlignment = HorizontalAlignment.Left;
        label.VerticalAlignment = VerticalAlignment.Top;
        label.Width = 100;
        label.Height = 100;*/

        object labelContent = GetLabel(currentTick);
        string labelString = GetLabelString(labelFormat, labelContent);
        AddAxisLabel(labelCount, labelContent, labelString, x, y);

        //labels.Add(label);

        MajorTickMarks.Add(angle);

        currentTick += Spacing;
        labelCount++;
      }
      RemoveLeftOverAxisLabels(labelCount);

      //Labels = new ReadOnlyCollection<AxisLabel>(labels);
    }

    internal override void UpdateSpacing()
    {
      if (MajorTickSpacing != 0)
      {
        Spacing = MajorTickSpacing;
      }
      else
      {
        double range = Maximum - Minimum;
        double spacing = range / 19;
        if (LabelMap != null)
        {
          spacing = 1;
        }
        else if (range != 0)
        {
          spacing = NumericalUtils.CalculateTickMarkSpacing(spacing);
          if (range == 360)
          {
            spacing = 30;
          }
        }
        Spacing = spacing == 0 ? 1 : spacing;
      }
    }

    internal double ConvertLogicalToPhysical(double logicalTheta)
    {
      if (Minimum == Maximum)
      {
        return 0;
      }

      double logicalSize = Maximum - Minimum;
      double physicalSize = 360.0;
      double ratio = logicalSize / physicalSize;
      double result = (logicalTheta - Minimum) / ratio;
      if (IsReversed)
      {
        result = 360 - result;
      }
      result += StartAngle;
      result %= 360;
      return result;
    }

    internal double ConvertPhysicalToLogical(double physicalTheta)
    {
      if (Minimum == Maximum)
      {
        return 0;
      }

      physicalTheta -= StartAngle;
      while (physicalTheta < 0)
      {
        physicalTheta += 360;
      }
      if (IsReversed)
      {
        physicalTheta = 360 - physicalTheta;
      }

      double logicalSize = Maximum - Minimum;
      double physicalSize = 360.0;
      double ratio = logicalSize / physicalSize;
      double result = (physicalTheta * ratio) + Minimum;

      return result;
    }

    #region StartAngle Property

    /// <summary>
    /// Gets or sets the angle in degrees where the minimum of the theta scale starts.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="StartAngleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double StartAngle
    {
      get { return (double)GetValue(StartAngleProperty); }
      set { SetValue(StartAngleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="StartAngle"/> property.
    /// </summary>
    public static readonly DependencyProperty StartAngleProperty =
      DependencyProperty.Register("StartAngle", typeof(double), typeof(ThetaAxis),
      new FrameworkPropertyMetadata(OnStartAngleChanged));

    private static void OnStartAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ThetaAxis)d).OnStartAngleChanged();
    }

    private void OnStartAngleChanged()
    {
      UpdateAxis();
    }

    #endregion // StartAngle Property

    #region IsReversed Property

    /// <summary>
    /// Gets or sets whether or not the axis is rendered anticlock wise.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsReversedProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsReversed
    {
      get { return (bool)GetValue(IsReversedProperty); }
      set { SetValue(IsReversedProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsReversed"/> property.
    /// </summary>
    public static readonly DependencyProperty IsReversedProperty =
      DependencyProperty.Register("IsReversed", typeof(bool), typeof(ThetaAxis),
      new FrameworkPropertyMetadata(OnIsReversedChanged));

    private static void OnIsReversedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ThetaAxis)d).OnIsReversedChanged();
    }

    private void OnIsReversedChanged()
    {
      UpdateAxis();
    }

    #endregion // IsReversed Property

    /*private AxisLabel GetAxisLabel()
    {
      AxisLabel label = new AxisLabel();
      label.LabelTemplate = LabelTemplate;
      label.Foreground = Foreground;
      return label;
    }*/
  }
}
