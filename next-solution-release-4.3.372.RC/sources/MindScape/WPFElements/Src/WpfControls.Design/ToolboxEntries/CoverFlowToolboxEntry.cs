using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry CoverFlowToolboxEntry = new ToolboxEntry(

      typeof(CoverFlow),
      "An animated selection control",

      new DependencyProperty[] {
      },

      new PropertyCategory[] {
        new PropertyCategory("Behavior", new DependencyProperty[] {
          CoverFlow.MouseSelectionModeProperty,
        }),
        new PropertyCategory("Appearance", new DependencyProperty[] {
          CoverFlow.ItemHeightProperty,
          CoverFlow.ItemSeparationProperty,
          CoverFlow.ItemWidthProperty,
          CoverFlow.ReflectionBrushProperty,
          CoverFlow.ReflectionScaleYProperty,
          CoverFlow.ScaleXProperty,
          CoverFlow.ScaleYProperty,
          CoverFlow.SelectedItemSeparationProperty,
          CoverFlow.ShearAngleProperty,
          CoverFlow.ShowReflectionProperty,
        }),
      }

      );
  }
}
