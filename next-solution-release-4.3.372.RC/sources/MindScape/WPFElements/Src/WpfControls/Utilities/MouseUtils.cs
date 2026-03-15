using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Runtime.InteropServices;
using System.Security;

namespace Mindscape.WpfElements
{
  internal static class MouseUtils
  {
    [SecuritySafeCritical]
    internal static Point GetPosition(Visual relativeTo)
    {
      IntegerPoint point = new IntegerPoint();
      GetCursorPos(ref point);
      Window window = VisualTreeUtils.FindAncestor<Window>(relativeTo);
      if (window == null)
      {
        return new Point();
      }
      return relativeTo.PointFromScreen(new Point(point.X, point.Y));
    }

    internal static Point GetCursorPosition()
    {
      IntegerPoint point = new IntegerPoint();
      GetCursorPos(ref point);
      return new Point(point.X, point.Y);
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetCursorPos(ref IntegerPoint pt);
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct IntegerPoint
  {
    public Int32 X;
    public Int32 Y;
  };
}
