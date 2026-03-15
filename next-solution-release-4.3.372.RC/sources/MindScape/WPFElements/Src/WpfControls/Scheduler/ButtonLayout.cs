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

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  internal struct ButtonLayout
  {
    private readonly double _height;
    private readonly double _width;
    private readonly Thickness _margin;

    public ButtonLayout(double height, double width, Thickness margin)
    {
      _height = height;
      _width = width;
      _margin = margin;
    }

    public double Height
    {
      get { return _height; }
    }

    public double Width
    {
      get { return _width; }
    }

    public Thickness Margin
    {
      get { return _margin; }
    }

    public void ApplyTo(FrameworkElement element)
    {
      element.Width = _width;
      element.Height = _height;
      element.Margin = _margin;
    }
  }
}
