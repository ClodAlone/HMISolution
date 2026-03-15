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
  /// Represents the rho axis of a <see cref="PolarChart"/>.
  /// The rho axis is used for plotting radial coordinates.
  /// </summary>
  public class RhoAxis : PolarAxisBase
  {
    static RhoAxis()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(RhoAxis),
        new FrameworkPropertyMetadata(typeof(RhoAxis)));
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
      //double axisRadius = (Math.Min(ActualWidth, ActualHeight) / 2.0);

      //double ratio = (Maximum - Minimum) / axisRadius;
      string labelFormat = LabelFormat;

      double currentTick = Minimum;
      int labelCount = 0;
      while (currentTick <= Maximum && Spacing > 0)
      {
        double x = centerX;
        double physicalTick = ConvertLogicalToPhysical(currentTick);
        double y = centerY - physicalTick;

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

        MajorTickMarks.Add(physicalTick);

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
        double spacing = range / 7;
        if (LabelMap != null)
        {
          spacing = 1;
        }
        else if (range != 0)
        {
          spacing = NumericalUtils.CalculateTickMarkSpacing(spacing);
        }
        Spacing = spacing;
      }
    }

    internal double ConvertLogicalToPhysical(double logicalRho)
    {
      if (Minimum == Maximum)
      {
        return 0;
      }

      double logicalSize = Maximum - Minimum;
      double physicalSize = (Math.Min(ActualWidth, ActualHeight) / 2.0);
      double ratio = logicalSize / physicalSize;
      double result = (logicalRho - Minimum) / ratio;
      return result;
    }

    internal double ConvertPhysicalToLogical(double physicalRho)
    {
      if (Minimum == Maximum)
      {
        return 0;
      }

      double logicalSize = Maximum - Minimum;
      double physicalSize = (Math.Min(ActualWidth, ActualHeight) / 2.0);
      double ratio = logicalSize / physicalSize;
      double result = (physicalRho * ratio) + Minimum;
      return result;
    }

    // TODO: may be able to merge ConvertLogicalToPhysicalSize with ConvertLogicalToPhysical

    /// <summary>
    /// Converts the given logical size to a physical size.
    /// </summary>
    /// <param name="logicalSize">The logical size to convert.</param>
    /// <returns>The physical interpretation of the given logical size.</returns>
    internal double ConvertLogicalToPhysicalSize(double logicalSize)
    {
      double totalLogicalSize = Maximum - Minimum;
      double totalPhysicalSize = (Math.Min(ActualWidth, ActualHeight) / 2.0);
      double ratio = totalPhysicalSize == 0 ? 0 : totalLogicalSize / totalPhysicalSize;
      return ratio == 0 ? 0 : logicalSize / ratio;
    }

    /*private AxisLabel GetAxisLabel()
    {
      AxisLabel label = new AxisLabel();
      label.LabelTemplate = LabelTemplate;
      label.Foreground = Foreground;
      return label;
    }*/
  }
}
