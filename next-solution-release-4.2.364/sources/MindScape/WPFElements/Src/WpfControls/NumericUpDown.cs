using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control for editing a decimal value with additional "up" and "down" commands.
  /// </summary>
  public class NumericUpDown : RangeBase
  {
    static NumericUpDown()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown),
        new FrameworkPropertyMetadata(typeof(NumericUpDown)));
      MinimumProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(1d));
      MaximumProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata((double)(Int32.MaxValue)));
      ValueProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(1d));
      SmallChangeProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(1d));
      LargeChangeProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(10d));
    }
  }
}
