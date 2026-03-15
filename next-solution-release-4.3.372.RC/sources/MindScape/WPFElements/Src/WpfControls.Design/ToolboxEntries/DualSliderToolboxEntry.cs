using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry DualSliderToolboxEntry = new ToolboxEntry(

      typeof(DualSlider),
      "A control that supports selecting two values, such as the start and end of a range, using a slider with two thumbs",

      new DependencyProperty[] {
      },

      new PropertyCategory[] {
        new PropertyCategory("Appearance", new DependencyProperty[] {
          DualSlider.OrientationProperty,
          DualSlider.TickSpacingProperty,
          DualSlider.ShowTopLeftTickMarksProperty,
          DualSlider.ShowBottomRightTickMarksProperty,
          DualSlider.EndBufferProperty
        }),
        new PropertyCategory("Behavior", new DependencyProperty[] {
          DualSlider.SmallChangeProperty,
          DualSlider.LargeChangeProperty,
          DualSlider.MaximumProperty,
          DualSlider.MinimumProperty,
          DualSlider.IsInstantMoveEnabledProperty,
          DualSlider.SnapToTickMarksProperty,
          DualSlider.IsMouseWheelEnabledProperty,
          DualSlider.MaximumRangeProperty,
          DualSlider.MinimumRangeProperty,
          DualSlider.AllowOverlapProperty,
        }),
        new PropertyCategory("Data", new DependencyProperty[] {
          DualSlider.RangeStartProperty,
          DualSlider.RangeEndProperty,
        }),
      }

      );
  }
}
