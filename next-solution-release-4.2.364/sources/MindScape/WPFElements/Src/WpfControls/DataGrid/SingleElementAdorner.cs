using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;

namespace Mindscape.WpfElements.WpfDataGrid
{
  internal class SingleElementAdorner : Adorner
  {
    private UIElement _element;

    public SingleElementAdorner(UIElement adornedElement, UIElement element)
      : base(adornedElement)
    {
      _element = element;
      AddVisualChild(element);
      AddLogicalChild(element);
    }

    protected override Visual GetVisualChild(int index)
    {
      if (index == 0)
      {
        return _element;
      }
      throw new ArgumentOutOfRangeException("index");
    }

    protected override int VisualChildrenCount
    {
      get { return 1; }
    }

    private static readonly Size NoConstraint = new Size(Double.MaxValue, Double.MaxValue);

    protected override Size MeasureOverride(Size constraint)
    {
      _element.Measure(NoConstraint);
      return _element.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      if (!(RenderTransform is TranslateTransform))
      {
        _element.Arrange(new Rect());
      }
      else
      {
        _element.Arrange(new Rect(finalSize));
      }
      return finalSize;
    }
  }
}
